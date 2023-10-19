using AutoMapper;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/Rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public RoomsController(IHotelInfoRepository hotelInfoRepository, IMapper mapper)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieve information about a specific room by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the room to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing information about the specified room. This may include a 200 OK response when successful, or a 404 Not Found response if the room is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of room information.</response>
        /// <response code="404">Indicates that the specified room was not found.</response>
        [HttpGet("{id}", Name = "GetRoom")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomAsync(int id)
        {
            var room = await _hotelInfoRepository.GetRoomAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RoomDto>(room));

        }


        /// <summary>
        /// Update information about a specific room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to update.</param>
        /// <param name="room">The data for updating the room.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the room is not found.
        /// </returns>
        /// <response code="204">Indicates a successful update with no content returned.</response>
        /// <response code="404">Indicates that the specified room was not found.</response>
        [HttpPut("{roomId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateRoomAsync(int roomId, RoomForUpdateDto room)
        {
            var roomEntity = await _hotelInfoRepository
                .GetRoomAsync(roomId);

            if (roomEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(room, roomEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Partially update information about a specific room using a JSON patch document.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to partially update.</param>
        /// <param name="patchDocument">A JSON patch document containing the changes to apply to the room.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the partial update operation. This may include a 204 No Content response when successful, a 400 Bad Request response if the request is invalid, or a 404 Not Found response if the room is not found.
        /// </returns>
        /// <response code="204">Indicates a successful partial update with no content returned.</response>
        /// <response code="400">Indicates a bad request due to an invalid patch document or other errors.</response>
        /// <response code="404">Indicates that the specified room was not found.</response>
        [HttpPatch("{roomId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PartiallyUpdateRoomAsync(int roomId,
            JsonPatchDocument<RoomForUpdateDto> patchDocument)
        {

            var roomEntity = await _hotelInfoRepository.GetRoomAsync(roomId);

            if (roomEntity == null)
            {
                return NotFound();
            }

            var roomToUpdate = _mapper.Map<RoomForUpdateDto>(roomEntity);

            patchDocument.ApplyTo(roomToUpdate);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(roomToUpdate, roomEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Retrieve a list of photos of a specific room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room for which you want to retrieve photos.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of photos in the specified room.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of photos of the specified room.</response>
        /// <response code="404">Indicates that the specified room was not found.</response>
        [HttpGet("{roomId}/photos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosAsync(int roomId)
        {
            if (!await _hotelInfoRepository.RoomExistsAsync(roomId))
            {
                return NotFound();
            }

            var photos = await _hotelInfoRepository.GetPhotosRoomAysnc(roomId);

            return Ok(_mapper.Map<IEnumerable<PhotoDto>>(photos));

        }
        /// <summary>
        /// Add a new photo to a specific room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room where the photo will be added.</param>
        /// <param name="photoForCreationDto">The data for creating the photo.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost("{roomId}/photos")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PhotoDto>> AddPhoto(int roomId, PhotoForCreationDto photoForCreationDto)
        {
            if (!await _hotelInfoRepository.RoomExistsAsync(roomId))
            {
                return NotFound();
            }

            var photoToStore = _mapper.Map<Entites.Photo>(photoForCreationDto);

            await _hotelInfoRepository.AddPhotoToRoom(roomId, photoToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var photoToReturn = _mapper.Map<PhotoDto>(photoToStore);

            return CreatedAtRoute("GetPhoto",
                new { Id = photoToReturn.Id },
                photoToReturn);

        }
        /// <summary>
        /// Delete a photo for a specific room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room that the photo belongs to.</param>
        /// <param name="photoId">The unique identifier of the photo to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the city or photo does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified room or photo was not found.</response>
        [HttpDelete("{roomId}/photos/{photoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePhoto(int roomId, int photoId)
        {
            if (!await _hotelInfoRepository.RoomExistsAsync(roomId))
            {
                return NotFound();
            }

            var room = await _hotelInfoRepository.GetRoomWithPhotosAsync(roomId);

            var photoToDelete = room.Photos
                .Where(photo => photo.Id == photoId)
                .SingleOrDefault();

            if (photoToDelete == null)
            {
                return NotFound();
            }

            _hotelInfoRepository.DeletePhoto(photoToDelete);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
