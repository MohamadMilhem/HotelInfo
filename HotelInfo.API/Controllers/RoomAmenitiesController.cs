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
    [Route("api/roomAmenities")]
    public class RoomAmenitiesController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public RoomAmenitiesController(IHotelInfoRepository hotelInfoRepository, IMapper mapper)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieve a list of room amenities based on search criteria.
        /// </summary>
        /// <param name="name">The name of the room amenities to search for, or a part of it.</param>
        /// <param name="pageSize">The number of results to display per page (default is 10).</param>
        /// <param name="pageNumber">The page number for paginated results (default is 1).</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of room amenities that match the specified search criteria.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of room amenities based on the search criteria.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoomAmenity>>> GetRoomAmenities(
            string? name, int pageSize = 10, int pageNumber = 1)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }
            var (amenitiesEntities, paginationMetaData) = await _hotelInfoRepository
                .GetRoomAmenitiesAsync(name, pageSize, pageNumber);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(amenitiesEntities);
        }
        /// <summary>
        /// Retrieve information about a room amenity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the room amenity to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing information about the specified room amenity. This may include a 200 OK response when successful, or a 404 Not Found response if the room amenity is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of room amenity information.</response>
        /// <response code="404">Indicates that the specified room amenity was not found.</response>
        [HttpGet("{id}", Name = "GetRoomAmenity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomAmenityAsync(int id)
        {
            var amenity = await _hotelInfoRepository.GetRoomAmenityAsync(id);

            if (amenity == null)
            {
                return NotFound();
            }
            
            return Ok(amenity);
        }
        
        /// <summary>
        /// Update information about a specific room amenity.
        /// </summary>
        /// <param name="roomAmenityId">The unique identifier of the room amenity to update.</param>
        /// <param name="roomAmenityForUpdateDto">The data for updating the room amenity.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the room amenity is not found.
        /// </returns>
        /// <response code="204">Indicates a successful update with no content returned.</response>
        /// <response code="404">Indicates that the specified room amenity was not found.</response>
        [HttpPut("{roomAmenityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateHotelAmenity(int roomAmenityId, RoomAmenityForUpdateDto roomAmenityForUpdateDto)
        {
            var roomAmenityEntity = await _hotelInfoRepository
                .GetRoomAmenityAsync(roomAmenityId);

            if (roomAmenityEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(roomAmenityForUpdateDto, roomAmenityEntity);
            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        
        /// <summary>
        /// Partially update information about a specific room amenity.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the room amenity to partially update.</param>
        /// <param name="patchDocument">A JSON patch document containing the changes to apply to the room amenity.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the partial update operation. This may include a 204 No Content response when successful, a 400 Bad Request response if the request is invalid, or a 404 Not Found response if the room amenity is not found.
        /// </returns>
        /// <response code="204">Indicates a successful partial update with no content returned.</response>
        /// <response code="400">Indicates a bad request due to an invalid patch document or other errors.</response>
        /// <response code="404">Indicates that the specified room amenity was not found.</response>
        [HttpPatch("{roomAmenityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PartiallyUpdateRoomAmenity(int roomAmenityId,
            JsonPatchDocument<RoomAmenityForUpdateDto> patchDocument)
        {

            var roomAmenityEntity = await _hotelInfoRepository.GetRoomAmenityAsync(roomAmenityId);

            if (roomAmenityEntity == null)
            {
                return NotFound();
            }

            var roomAmenityToUpdate = _mapper.Map<RoomAmenityForUpdateDto>(roomAmenityEntity);

            patchDocument.ApplyTo(roomAmenityToUpdate);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(roomAmenityToUpdate, roomAmenityEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        
    }
}
