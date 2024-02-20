using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class PhotoController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        private readonly BlobServiceClient _blobServiceClient;
        const int maxPageSize = 20;
        public PhotoController(IHotelInfoRepository hotelInfoRepository, IMapper mapper, BlobServiceClient blobServiceClient)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _blobServiceClient = blobServiceClient;
        }

        /// <summary>
        /// Retrieve information about a specific photo by its unique identifier.
        /// </summary>
        /// <param name="photoId">The unique identifier of the photo to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing information about the specified photo. This may include a 200 OK response when successful, or a 404 Not Found response if the photo is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of photo information.</response>
        /// <response code="404">Indicates that the specified Photo was not found.</response>
        [HttpGet("{photoId}", Name = "GetPhoto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPhotoAsync(int photoId)
        {
            var photo = await _hotelInfoRepository.GetPhotoAsync(photoId);

            if (photo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PhotoDto>(photo));

        }

        /// <summary>
        /// Update information about a specific photo.
        /// </summary>
        /// <param name="photoId">The unique identifier of the photo to update.</param>
        /// <param name="photo">The data for updating the photo.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the photo is not found.
        /// </returns>
        /// <response code="204">Indicates a successful update with no content returned.</response>
        /// <response code="404">Indicates that the specified photo was not found.</response>
        [HttpPut("{photoId}")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePhotoAsync(int photoId, PhotoForUpdateDto photo)
        {
            var photoEntity = await _hotelInfoRepository.GetPhotoAsync(photoId);

            if (photoEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(photo, photoEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Partially update information about a specific photo using a JSON patch document.
        /// </summary>
        /// <param name="photoId">The unique identifier of the photo to partially update.</param>
        /// <param name="patchDocument">A JSON patch document containing the changes to apply to the photo.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the partial update operation. This may include a 204 No Content response when successful, a 400 Bad Request response if the request is invalid, or a 404 Not Found response if the photo is not found.
        /// </returns>
        /// <response code="204">Indicates a successful partial update with no content returned.</response>
        /// <response code="400">Indicates a bad request due to an invalid patch document or other errors.</response>
        /// <response code="404">Indicates that the specified photo was not found.</response>
        [HttpPatch("{photoId}")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PartiallyUpdateRoomAsync(int photoId,
            JsonPatchDocument<PhotoForUpdateDto> patchDocument)
        {

            var photoEntity = await _hotelInfoRepository.GetPhotoAsync(photoId);

            if (photoEntity == null)
            {
                return NotFound();
            }

            var photoToUpdate = _mapper.Map<PhotoForUpdateDto>(photoEntity);

            patchDocument.ApplyTo(photoToUpdate);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(photoToUpdate, photoEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Upload new photo.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadPhoto([FromForm] FileUpload fileUpload)
        {
            try
            {
                if (fileUpload.files.Length > 0)
                {
                    var logedInUser = "web";
                    var containerClient = _blobServiceClient.GetBlobContainerClient(logedInUser);
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                    var newBlobName = $"{Guid.NewGuid().ToString()}.jpg";
                    await containerClient.UploadBlobAsync(newBlobName, fileUpload.files.OpenReadStream());

                    var photoForCreationDto = new PhotoForCreationDto() 
                    {
                        Url = $"https://cdne-hotel-reservation-webapi-dev-001.azureedge.net/{logedInUser}/{newBlobName}"
                    };
                    var photoToStore = _mapper.Map<Entites.Photo>(photoForCreationDto);

                    await _hotelInfoRepository.CreatePhotoAsync(photoToStore);
                    await _hotelInfoRepository.SaveChangesAsync();

                    var photoToReturn = _mapper.Map<PhotoDto>(photoToStore);

                    return CreatedAtRoute("GetPhoto",
                        new { photoId = photoToReturn.Id },
                        photoToReturn);

                }
                else
                {
                    return BadRequest("A non-empty request body is required.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
