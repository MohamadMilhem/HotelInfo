using AutoMapper;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/room-classes")]
    public class RoomClassesController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public RoomClassesController(IHotelInfoRepository hotelInfoRepository, IMapper mapper)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// Retrieve information about a specific room class by its unique identifier.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing information about the specified room class. This may include a 200 OK response when successful, or a 404 Not Found response if the room class is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of room class information.</response>
        /// <response code="404">Indicates that the specified room class was not found.</response>
        [HttpGet("{roomClassId}", Name = "GetRoomClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomClassAsync(int roomClassId)
        {
            var roomClass = await _hotelInfoRepository.GetRoomClassAsync(roomClassId);

            if (roomClass == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RoomClassDto>(roomClass));

        }
        /// <summary>
        /// Create a new room class.
        /// </summary>
        /// <param name="roomClass">The data for creating the new room class.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the newly created room class or a 201 Created response.
        /// </returns>
        /// <response code="201">Returns the newly created room class when the operation is successful.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RoomClassDto>> CreateRoomClass(RoomClassForCreationDto roomClass)
        {
            var roomClassToStore = _mapper.Map<Entites.RoomClass>(roomClass);

            await _hotelInfoRepository.CreateRoomClassAsync(roomClassToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var roomClassToReturn = _mapper.Map<RoomClassDto>(roomClassToStore);

            return CreatedAtRoute("GetRoomClass",
                new { roomClassId = roomClassToReturn.Id },
                roomClassToReturn);
        }
        /// <summary>
        /// Update information about a specific room Class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room Class to update.</param>
        /// <param name="roomClass">The data for updating the room Class.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the room class is not found.
        /// </returns>
        /// <response code="204">Indicates a successful update with no content returned.</response>
        /// <response code="404">Indicates that the specified room class was not found.</response>
        [HttpPut("{roomClassId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateRoomClassAsync(int roomClassId, RoomClassForCreationDto roomClass)
        {
            var roomClassEntity = await _hotelInfoRepository
                .GetRoomClassAsync(roomClassId);

            if (roomClassEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(roomClass, roomClassEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Partially update information about a specific room class using a JSON patch document.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class to partially update.</param>
        /// <param name="patchDocument">A JSON patch document containing the changes to apply to the room class.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the partial update operation. This may include a 204 No Content response when successful, a 400 Bad Request response if the request is invalid, or a 404 Not Found response if the room class is not found.
        /// </returns>
        /// <response code="204">Indicates a successful partial update with no content returned.</response>
        /// <response code="400">Indicates a bad request due to an invalid patch document or other errors.</response>
        /// <response code="404">Indicates that the specified room class was not found.</response>
        [HttpPatch("{roomClassId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PartiallyUpdateRoomClassAsync(int roomClassId,
            JsonPatchDocument<RoomClassForUpdateDto> patchDocument)
        {

            var roomClassEntity = await _hotelInfoRepository.GetRoomClassAsync(roomClassId);

            if (roomClassEntity == null)
            {
                return NotFound();
            }

            var roomClassToUpdate = _mapper.Map<RoomClassForUpdateDto>(roomClassEntity);

            patchDocument.ApplyTo(roomClassToUpdate);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(roomClassToUpdate, roomClassEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Retrieve a list of photos of a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class for which you want to retrieve photos.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of photos in the specified room class.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of photos of the specified room class.</response>
        /// <response code="404">Indicates that the specified room class was not found.</response>
        [HttpGet("{roomClassId}/photos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosAsync(int roomClassId)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            var photos = await _hotelInfoRepository.GetPhotosRoomClassAsync(roomClassId);

            return Ok(_mapper.Map<IEnumerable<PhotoDto>>(photos));

        }
        /// <summary>
        /// Add a new photo to a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class where the photo will be added.</param>
        /// <param name="photoForCreationDto">The data for creating the photo.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost("{roomClassId}/photos")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PhotoDto>> AddPhoto(int roomClassId, PhotoForCreationDto photoForCreationDto)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            var photoToStore = _mapper.Map<Entites.Photo>(photoForCreationDto);

            await _hotelInfoRepository.AddPhotoToRoomClass(roomClassId, photoToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var photoToReturn = _mapper.Map<PhotoDto>(photoToStore);

            return CreatedAtRoute("GetPhoto",
                new { photoId = photoToReturn.Id },
                photoToReturn);

        }
        /// <summary>
        /// Delete a photo for a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class that the photo belongs to.</param>
        /// <param name="photoId">The unique identifier of the photo to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the room class or photo does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified room class or photo was not found.</response>
        [HttpDelete("{roomClassId}/photos/{photoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePhoto(int roomClassId, int photoId)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            var roomClass = await _hotelInfoRepository.GetRoomClassWithPhotosAsync(roomClassId);

            var photoToDelete = roomClass.Photos
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
        /// Retrieve a list of amenities within a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class for which you want to retrieve amenities.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of amenities in the specified room class. This may include a 200 OK response when successful, or a 404 Not Found response if the room class is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of amenities in the specified room class.</response>
        /// <response code="404">Indicates that the specified room class was not found.</response>
        [HttpGet("{roomClassId}/amenities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RoomAmenityDto>>> GetRoomAmenitiesAsync(int roomClassId)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            var amenities = await _hotelInfoRepository
                .GetRoomAmenitiesForRoomClassAsync(roomClassId);

            return Ok(_mapper.Map<IEnumerable<RoomAmenityDto>>(amenities));

        }
        /// <summary>
        /// Add a new room amenity to a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class where the room amenity will be added.</param>
        /// <param name="roomAmenityForCreationDto">The data for creating the room amenity.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost("{roomClassId}/amenities")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RoomAmenityDto>> AddRoomAmenity(int roomClassId,  RoomAmenityForCreationDto roomAmenityForCreationDto)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            var roomAmenityToStore = _mapper.Map<Entites.RoomAmenity>(roomAmenityForCreationDto);

            await _hotelInfoRepository.AddRoomAmenityToRoomClass(roomClassId, roomAmenityToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var roomAmenityToReturn = _mapper.Map<RoomAmenityDto>(roomAmenityToStore);

            return CreatedAtRoute("GetRoomAmenity",
                new { roomAmenityId = roomAmenityToReturn.Id },
                roomAmenityToReturn);

        }
        /// <summary>
        /// Delete a room amenity from a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class that the room amenity belongs to.</param>
        /// <param name="roomAmenityId">The unique identifier of the room amenity to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the room class or room amenity does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified room or room amenity was not found.</response>
        [HttpDelete("{roomClassId}/amenities/{roomAmenityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteRoomAmenity(int roomClassId, int roomAmenityId)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            var roomClass = await _hotelInfoRepository.GetRoomClassWithRoomAmenitiesAsync(roomClassId);

            var roomAmenityToDelete = roomClass.RoomAmenities
                .Where(roomAmenity => roomAmenity.Id == roomAmenityId)
                .SingleOrDefault();

            if (roomAmenityToDelete == null)
            {
                return NotFound();
            }

            _hotelInfoRepository.DeleteRoomAmenityFromRoomClass(roomClassId, roomAmenityToDelete);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Retrieve a list of rooms within a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class for which you want to retrieve rooms.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of rooms in the specified room class. This may include a 200 OK response when successful, or a 404 Not Found response if the room class is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of rooms in the specified room class.</response>
        /// <response code="404">Indicates that the specified room class was not found.</response>
        [HttpGet("{roomClassId}/rooms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsAsync(int roomClassId)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            var roomClass = await _hotelInfoRepository.GetRoomClassWithRooms(roomClassId);

            return Ok(_mapper.Map<IEnumerable<RoomDto>>(roomClass.Rooms));

        }
        /// <summary>
        /// Add a new room within a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class where the room will be added.</param>
        /// <param name="roomId">The unique identifier of the room that will be added.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation. This may include a 201 Created response when successful, or a 404 Not Found response if the room class is not found.
        /// </returns>
        /// <response code="201">Indicates a successful creation of a room within the room class.</response>
        /// <response code="404">Indicates that the specified room class was not found.</response>
        [HttpPost("{roomClassId}/rooms")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomDto>> AddRoom(int roomClassId, int roomId)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            if (!await _hotelInfoRepository.RoomExistsAsync(roomId))
            {
                return NotFound();
            }

            var roomToAdd = await _hotelInfoRepository.GetRoomAsync(roomId);

            await _hotelInfoRepository.AddRoomToRoomClassAsync(roomClassId ,roomToAdd);
            await _hotelInfoRepository.SaveChangesAsync();

            var roomToReturn = _mapper.Map<RoomDto>(roomToAdd);

            return CreatedAtRoute("GetRoom",
                new { roomId = roomToReturn.Id },
                roomToReturn);

        }

        /// <summary>
        /// Delete a room within a specific room class.
        /// </summary>
        /// <param name="roomClassId">The unique identifier of the room class that contains the room.</param>
        /// <param name="roomId">The unique identifier of the room to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the room class or room is not found.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified room class or room was not found.</response>
        [HttpDelete("{roomClassId}/rooms/{roomId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteRoom(int roomClassId, int roomId)
        {
            if (!await _hotelInfoRepository.RoomClassExistsAsync(roomClassId))
            {
                return NotFound();
            }

            var roomClass = await _hotelInfoRepository.GetRoomClassWithRooms(roomClassId);

            var roomToDelete = roomClass.Rooms
                .Where(room => room.Id == roomId)
                .SingleOrDefault();

            if (roomToDelete == null)
            {
                return NotFound();
            }

            roomClass.Rooms.Remove(roomToDelete);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
