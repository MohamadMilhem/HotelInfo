using AutoMapper;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/hotels")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public HotelsController(IHotelInfoRepository hotelInfoRepository, IMapper mapper)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieve a list of hotels based on search criteria.
        /// </summary>
        /// <param name="name">The name of the hotels to search for, or a part of it.</param>
        /// <param name="searchQuery">A string to search for in hotel descriptions.</param>
        /// <param name="pageSize">The number of results to display per page (default is 10).</param>
        /// <param name="pageNumber">The page number for paginated results (default is 1).</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of hotels that match the specified search criteria.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of hotels based on the search criteria.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelWithoutRooms>>> GetHotels(
            string? name, string? searchQuery, int pageSize = 10, int pageNumber = 1)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }
            var (hotelsEntities, paginationMetaData) = await _hotelInfoRepository.GetHotelsAsync(name, searchQuery, pageSize, pageNumber);
            var results = _mapper.Map<IEnumerable<HotelWithoutRooms>>(hotelsEntities);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(results);
        }

        /// <summary>
        /// Retrieve information about a hotel by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the hotel to retrieve.</param>
        /// <param name="includeRooms">Indicates whether or not to include room details for the hotel.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing information about the specified hotel. This may include a 200 OK response when successful, or a 404 Not Found response if the hotel is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of hotel information.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpGet("{id}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelAsync(int id, bool includeRooms = false)
        {
            var hotel = await _hotelInfoRepository.GetHotelAsync(id, includeRooms);

            if (hotel == null)
            {
                return NotFound();
            }

            if (includeRooms)
            {
                return Ok(_mapper.Map<HotelDto>(hotel));
            }

            return Ok(_mapper.Map<HotelWithoutRooms>(hotel));

        }

        /// <summary>
        /// Update information about a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel to update.</param>
        /// <param name="hotelForUpdateDto">The data for updating the hotel.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the hotel is not found.
        /// </returns>
        /// <response code="204">Indicates a successful update with no content returned.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpPut("{hotelId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateHotel(int hotelId, HotelForUpdateDto hotelForUpdateDto)
        {
            var hotelEntity = await _hotelInfoRepository
                .GetHotelAsync(hotelId, false);

            if (hotelEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(hotelForUpdateDto, hotelEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Partially update information about a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel to partially update.</param>
        /// <param name="patchDocument">A JSON patch document containing the changes to apply to the hotel.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the partial update operation. This may include a 204 No Content response when successful, a 400 Bad Request response if the request is invalid, or a 404 Not Found response if the hotel is not found.
        /// </returns>
        /// <response code="204">Indicates a successful partial update with no content returned.</response>
        /// <response code="400">Indicates a bad request due to an invalid patch document or other errors.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpPatch("{hotelId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PartiallyUpdateHotel(int hotelId,
            JsonPatchDocument<HotelForUpdateDto> patchDocument)
        {

            var hotelEntity = await _hotelInfoRepository.GetHotelAsync(hotelId, false);

            if (hotelEntity == null)
            {
                return NotFound();
            }

            var hotelToUpdate = _mapper.Map<HotelForUpdateDto>(hotelEntity);

            patchDocument.ApplyTo(hotelToUpdate);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(hotelToUpdate, hotelEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Retrieve a list of rooms within a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel for which you want to retrieve rooms.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of rooms in the specified hotel. This may include a 200 OK response when successful, or a 404 Not Found response if the hotel is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of rooms in the specified hotel.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpGet("{hotelId}/rooms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsAsync(int hotelId)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var rooms = await _hotelInfoRepository.GetRoomsAsync(hotelId);

            return Ok(_mapper.Map<IEnumerable<RoomDto>>(rooms));

        }

        /// <summary>
        /// Create a new room within a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel where the room will be created.</param>
        /// <param name="roomForCreationDto">The data for creating the room.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation. This may include a 201 Created response when successful, or a 404 Not Found response if the hotel is not found.
        /// </returns>
        /// <response code="201">Indicates a successful creation of a room within the hotel.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpPost("{hotelId}/rooms")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDto>> CreateRoom(int hotelId, RoomForCreationDto roomForCreationDto)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var roomToStore = _mapper.Map<Entites.Room>(roomForCreationDto);

            await _hotelInfoRepository.CreateRoomAsync(hotelId, roomToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var roomToReturn = _mapper.Map<RoomDto>(roomToStore);

            return CreatedAtRoute("GetRoom",
                new { Id = roomToReturn.Id },
                roomToReturn);

        }

        /// <summary>
        /// Delete a room within a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel that contains the room.</param>
        /// <param name="roomId">The unique identifier of the room to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the hotel or room is not found.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified hotel or room was not found.</response>
        [HttpDelete("{hotelId}/rooms/{roomId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteRoom(int hotelId, int roomId)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var hotel = await _hotelInfoRepository.GetHotelAsync(hotelId, true);

            var roomToDelete = hotel.Rooms
                .Where(room => room.Id == roomId)
                .SingleOrDefault();

            if (roomToDelete == null)
            {
                return NotFound();
            }

            _hotelInfoRepository.DeleteRoom(roomToDelete);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Retrieve a list of photos of a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel for which you want to retrieve photos.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of photos in the specified hotel.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of photos of the specified hotel.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpGet("{hotelId}/photos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosAsync(int hotelId)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var photos = await _hotelInfoRepository.GetPhotosHotelAsync(hotelId);

            return Ok(_mapper.Map<IEnumerable<PhotoDto>>(photos));

        }
        /// <summary>
        /// Add a new photo to a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel where the photo will be added.</param>
        /// <param name="photoForCreationDto">The data for creating the photo.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost("{hotelId}/photos")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PhotoDto>> AddPhoto(int hotelId,  PhotoForCreationDto photoForCreationDto)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var photoToStore = _mapper.Map<Entites.Photo>(photoForCreationDto);

            await _hotelInfoRepository.AddPhotoToHotel(hotelId, photoToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var photoToReturn = _mapper.Map<PhotoDto>(photoToStore);

            return CreatedAtRoute("GetPhoto",
                new { Id = photoToReturn.Id },
                photoToReturn);

        }
        /// <summary>
        /// Delete a photo for a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel that the photo belongs to.</param>
        /// <param name="photoId">The unique identifier of the photo to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the hotel or photo does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified hotel or photo was not found.</response>
        [HttpDelete("{hotelId}/photos/{photoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePhoto(int hotelId, int photoId)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var hotel = await _hotelInfoRepository.GetHotelWithPhotosAsync(hotelId);

            var photoToDelete = hotel.Photos
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
        /// <summary>
        /// Retrieve a list of amenities within a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel for which you want to retrieve amenities.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of amenities in the specified hotel. This may include a 200 OK response when successful, or a 404 Not Found response if the hotel is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of amenities in the specified hotel.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpGet("{hotelId}/amenities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<HotelAmenityDto>>> GetHotelAmenitiesAsync(int hotelId)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var amenities = await _hotelInfoRepository.GetHotelAmenitiesAsync(hotelId);

            return Ok(_mapper.Map<IEnumerable<HotelAmenityDto>>(amenities));

        }
        /// <summary>
        /// Add a new hotel amenity to a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel where the hotel amenity will be added.</param>
        /// <param name="hotelAmenityForCreationDto">The data for creating the hotel amenity.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost("{hotelId}/amenities")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<HotelAmenityDto>> AddHotelAmenity(int hotelId,  HotelAmenityForCreationDto hotelAmenityForCreationDto)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var hotelAmenityToStore = _mapper.Map<Entites.HotelAmenity>(hotelAmenityForCreationDto);

            await _hotelInfoRepository.AddHotelAmenity(hotelId, hotelAmenityToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var hotelAmenityToReturn = _mapper.Map<HotelAmenityDto>(hotelAmenityToStore);

            return CreatedAtRoute("GetHotelAmenity",
                new { Id = hotelAmenityToReturn.Id },
                hotelAmenityToReturn);

        }
        /// <summary>
        /// Delete a hotel amenity from a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel that the hotel amenity belongs to.</param>
        /// <param name="hotelAmenityId">The unique identifier of the hotel amenity to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the hotel or hotel amenity does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified hotel or hotel amenity was not found.</response>
        [HttpDelete("{hotelId}/amenities/{hotelAmenityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteHotelAmenity(int hotelId, int hotelAmenityId)
        {
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var hotel = await _hotelInfoRepository.GetHotelWithHotelAmenitiesAsync(hotelId);

            var hotelAmenityToDelete = hotel.HotelAmenities
                .Where(hotelAmenity => hotelAmenity.Id == hotelAmenityId)
                .SingleOrDefault();

            if (hotelAmenityToDelete == null)
            {
                return NotFound();
            }

            _hotelInfoRepository.DeleteHotelAmenity(hotelId, hotelAmenityToDelete);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
