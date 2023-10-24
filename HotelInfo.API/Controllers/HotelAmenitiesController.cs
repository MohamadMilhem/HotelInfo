using AutoMapper;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/hotelAmenities")]
    public class HotelAmenitiesController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public HotelAmenitiesController(IHotelInfoRepository hotelInfoRepository, IMapper mapper)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieve a list of hotels amenities based on search criteria.
        /// </summary>
        /// <param name="name">The name of the hotels amenities to search for, or a part of it.</param>
        /// <param name="pageSize">The number of results to display per page (default is 10).</param>
        /// <param name="pageNumber">The page number for paginated results (default is 1).</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of hotels amenities that match the specified search criteria.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of hotels amenities based on the search criteria.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelAmenity>>> GetHotelAmenities(
            string? name, int pageSize = 10, int pageNumber = 1)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }
            var (amenitiesEntities, paginationMetaData) = await _hotelInfoRepository.GetHotelAmenitiesAsync(name, pageSize, pageNumber);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(amenitiesEntities);
        }
        /// <summary>
        /// Retrieve information about a hotel amenity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the hotel amenity to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing information about the specified amenity. This may include a 200 OK response when successful, or a 404 Not Found response if the amenity is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of hotel amenity information.</response>
        /// <response code="404">Indicates that the specified hotel amenity was not found.</response>
        [HttpGet("{id}", Name = "GetHotelAmenity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelAmenityAsync(int id)
        {
            var amenity = await _hotelInfoRepository.GetHotelAmenityAsync(id);

            if (amenity == null)
            {
                return NotFound();
            }
            
            return Ok(amenity);
        }
        
        /// <summary>
        /// Update information about a specific hotel amenity.
        /// </summary>
        /// <param name="hotelAmenityId">The unique identifier of the hotel amenity to update.</param>
        /// <param name="hotelAmenityForUpdateDto">The data for updating the hotel amenity.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the hotel amenity is not found.
        /// </returns>
        /// <response code="204">Indicates a successful update with no content returned.</response>
        /// <response code="404">Indicates that the specified hotel amenity was not found.</response>
        [HttpPut("{hotelAmenityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateHotelAmenity(int hotelAmenityId, HotelAmenityForUpdateDto hotelAmenityForUpdateDto)
        {
            var hotelAmenityEntity = await _hotelInfoRepository
                .GetHotelAmenityAsync(hotelAmenityId);

            if (hotelAmenityEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(hotelAmenityForUpdateDto, hotelAmenityEntity);
            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        
        /// <summary>
        /// Partially update information about a specific hotel amenity.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel amenity to partially update.</param>
        /// <param name="patchDocument">A JSON patch document containing the changes to apply to the hotel amenity.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the partial update operation. This may include a 204 No Content response when successful, a 400 Bad Request response if the request is invalid, or a 404 Not Found response if the hotel amenity is not found.
        /// </returns>
        /// <response code="204">Indicates a successful partial update with no content returned.</response>
        /// <response code="400">Indicates a bad request due to an invalid patch document or other errors.</response>
        /// <response code="404">Indicates that the specified hotel amenity was not found.</response>
        [HttpPatch("{hotelAmenityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PartiallyUpdateHotelAmenity(int hotelAmenityId,
            JsonPatchDocument<HotelAmenityForUpdateDto> patchDocument)
        {

            var hotelAmenityEntity = await _hotelInfoRepository.GetHotelAmenityAsync(hotelAmenityId);

            if (hotelAmenityEntity == null)
            {
                return NotFound();
            }

            var hotelAmenityToUpdate = _mapper.Map<HotelAmenityForUpdateDto>(hotelAmenityEntity);

            patchDocument.ApplyTo(hotelAmenityToUpdate);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(hotelAmenityToUpdate, hotelAmenityEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        
    }
}
