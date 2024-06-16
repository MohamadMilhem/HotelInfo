using AutoMapper;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/rooms")]
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
        /// <param name="roomId">The unique identifier of the room to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing information about the specified room. This may include a 200 OK response when successful, or a 404 Not Found response if the room is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of room information.</response>
        /// <response code="404">Indicates that the specified room was not found.</response>
        [HttpGet("{roomId}", Name = "GetRoom")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomAsync(int roomId)
        {
            var result =  new List<RoomAvailabilityResultDto>()
            {
                new RoomAvailabilityResultDto()
                {
                    RoomId = 1,
                    RoomNumber = 101,
                    RoomPhotoUrl = "https://cf.bstatic.com/xdata/images/hotel/max1280x900/33143786.jpg?k=4d0bca9d9795b80beb2cd9786946e043b23d1372eb633d5855d3aba6343d68d4&o=&hp=1",
                    RoomType = "Standard",
                    CapacityOfAdults = 2,
                    CapacityOfChildren = 1,
                    RoomAmenities = new List<FilterAmenityDto>()
                    {
                        new FilterAmenityDto() { Name = "Free Wi-Fi", Description = "High-speed internet available in all rooms." },
                        new FilterAmenityDto() { Name = "TV", Description = "Flat-screen TV with cable channels." },
                        new FilterAmenityDto() { Name = "Air Conditioning", Description = "Individually controlled air conditioning." }
                    },
                    Price = 150,
                    Availability = true
                },
                new RoomAvailabilityResultDto()
                {
                    RoomId = 2,
                    RoomNumber = 201,
                    RoomPhotoUrl = "https://cf.bstatic.com/xdata/images/hotel/max1280x900/33144464.jpg?k=98997407dcb7957422ca16005b3d19b574ff93b3097e8802fc1f6862eb041e2b&o=&hp=1",
                    RoomType = "Suite",
                    CapacityOfAdults = 3,
                    CapacityOfChildren = 0,
                    RoomAmenities = new List<FilterAmenityDto>()
                    {
                        new FilterAmenityDto() { Name = "Jacuzzi", Description = "Private jacuzzi for relaxation." },
                        new FilterAmenityDto() { Name = "Mini Bar", Description = "Stocked mini bar with a selection of beverages." },
                        new FilterAmenityDto() { Name = "Ocean View", Description = "Stunning ocean view from the room." }
                    },
                    Price = 250,
                    Availability = false // Not available
                },
                new RoomAvailabilityResultDto()
                {
                    RoomId = 3,
                    RoomNumber = 301,
                    RoomPhotoUrl = "https://cf.bstatic.com/xdata/images/hotel/max1280x900/31823343.jpg?k=cbd94934282436e4989a72c9bdf725fa45d668902fe7ebc745d8986b73452e18&o=&hp=1",
                    RoomType = "Deluxe",
                    CapacityOfAdults = 2,
                    CapacityOfChildren = 2,
                    RoomAmenities = new List<FilterAmenityDto>()
                    {
                        new FilterAmenityDto() { Name = "King Size Bed", Description = "Spacious king-size bed for a comfortable stay." },
                        new FilterAmenityDto() { Name = "City View", Description = "Enjoy a panoramic view of the city." },
                        new FilterAmenityDto() { Name = "Room Service", Description = "24/7 room service available." }
                    },
                    Price = 180,
                    Availability = true
                },
                new RoomAvailabilityResultDto()
                {
                    RoomId = 4,
                    RoomNumber = 401,
                    RoomPhotoUrl = "https://cf.bstatic.com/xdata/images/hotel/max1280x900/33518061.jpg?k=f1817f9efd359ed681c1c5ca21faf9374a124f24788ce8a21c0d3d24a098386f&o=&hp=1",
                    RoomType = "Economy",
                    CapacityOfAdults = 1,
                    CapacityOfChildren = 0,
                    RoomAmenities = new List<FilterAmenityDto>()
                    {
                        new FilterAmenityDto() { Name = "Budget-Friendly", Description = "An economical choice for solo travelers." },
                        new FilterAmenityDto() { Name = "Single Bed", Description = "Comfortable single bed for a restful sleep." }
                    },
                    Price = 80,
                    Availability = false // Not available
                },
                new RoomAvailabilityResultDto()
                {
                    RoomId = 5,
                    RoomNumber = 501,
                    RoomPhotoUrl = "https://cf.bstatic.com/xdata/images/hotel/max1280x900/33142365.jpg?k=edd59d67083a8d9d3c984a43fb270a7faad8d76ec5bfd82ab64e31293e2aa022&o=&hp=1",
                    RoomType = "Family Suite",
                    CapacityOfAdults = 4,
                    CapacityOfChildren = 2,
                    RoomAmenities = new List<FilterAmenityDto>()
                    {
                        new FilterAmenityDto() { Name = "Adjoining Rooms", Description = "Ideal for families with connecting rooms." },
                        new FilterAmenityDto() { Name = "Kitchenette", Description = "Convenient kitchenette for family meals." },
                        new FilterAmenityDto() { Name = "Play Area", Description = "Dedicated play area for children." }
                    },
                    Price = 300,
                    Availability = false // Not available
                },
                new RoomAvailabilityResultDto()
                {
                    RoomId = 6,
                    RoomNumber = 601,
                    RoomPhotoUrl = "https://cf.bstatic.com/xdata/images/hotel/max1280x900/33142495.jpg?k=9956c92b062724ceb086158d062ca22b4d10a5737fd2fc08879f44d2842d5091&o=&hp=1",
                    RoomType = "Executive Suite",
                    CapacityOfAdults = 2,
                    CapacityOfChildren = 1,
                    RoomAmenities = new List<FilterAmenityDto>()
                    {
                        new FilterAmenityDto() { Name = "Business Center Access", Description = "Exclusive access to the hotel's business center." },
                        new FilterAmenityDto() { Name = "Meeting Room", Description = "Private meeting room for business needs." },
                        new FilterAmenityDto() { Name = "Complimentary Breakfast", Description = "Daily complimentary breakfast included." }
                    },
                    Price = 220,
                    Availability = true
                },
                // Add more room availability data here...
            };
            var room = result.SingleOrDefault(room => room.RoomId == roomId);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
            
            /*
            var room = await _hotelInfoRepository.GetRoomAsync(roomId);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RoomDto>(room));
            */
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
        [Authorize(policy:"Admin")]
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
        [Authorize(policy:"Admin")]
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

            var photos = await _hotelInfoRepository.GetPhotosRoomAsync(roomId);

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
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                new { photoId = photoToReturn.Id },
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
        [Authorize(policy:"Admin")]
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
            
            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
                /// <summary>
        /// Retrieve a list of amenities within a specific room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room for which you want to retrieve amenities.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of amenities in the specified room. This may include a 200 OK response when successful, or a 404 Not Found response if the room is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of amenities in the specified room.</response>
        /// <response code="404">Indicates that the specified room was not found.</response>
        [HttpGet("{roomId}/amenities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RoomAmenityDto>>> GetRoomAmenitiesAsync(int roomId)
        {
            if (!await _hotelInfoRepository.RoomExistsAsync(roomId))
            {
                return NotFound();
            }

            var amenities = await _hotelInfoRepository.GetRoomAmenitiesAsync(roomId);

            return Ok(_mapper.Map<IEnumerable<RoomAmenityDto>>(amenities));

        }
        /// <summary>
        /// Add a new room amenity to a specific room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room where the room amenity will be added.</param>
        /// <param name="roomAmenityForCreationDto">The data for creating the room amenity.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost("{roomId}/amenities")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RoomAmenityDto>> AddRoomAmenity(int roomId,  RoomAmenityForCreationDto roomAmenityForCreationDto)
        {
            if (!await _hotelInfoRepository.RoomExistsAsync(roomId))
            {
                return NotFound();
            }

            var roomAmenityToStore = _mapper.Map<Entites.RoomAmenity>(roomAmenityForCreationDto);

            await _hotelInfoRepository.AddRoomAmenity(roomId, roomAmenityToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var roomAmenityToReturn = _mapper.Map<RoomAmenityDto>(roomAmenityToStore);

            return CreatedAtRoute("GetRoomAmenity",
                new { roomAmenityId = roomAmenityToReturn.Id },
                roomAmenityToReturn);

        }
        /// <summary>
        /// Delete a room amenity from a specific room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room that the room amenity belongs to.</param>
        /// <param name="roomAmenityId">The unique identifier of the room amenity to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the room or room amenity does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified room or room amenity was not found.</response>
        [HttpDelete("{roomId}/amenities/{roomAmenityId}")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteRoomAmenity(int roomId, int roomAmenityId)
        {
            if (!await _hotelInfoRepository.RoomExistsAsync(roomId))
            {
                return NotFound();
            }

            var room = await _hotelInfoRepository.GetRoomWithRoomAmenitiesAsync(roomId);

            var roomAmenityToDelete = room.RoomAmenities
                .Where(roomAmenity => roomAmenity.Id == roomAmenityId)
                .SingleOrDefault();

            if (roomAmenityToDelete == null)
            {
                return NotFound();
            }

            _hotelInfoRepository.DeleteRoomAmenity(roomId, roomAmenityToDelete);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
