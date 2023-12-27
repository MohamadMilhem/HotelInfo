using AutoMapper;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

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
        /// <param name="hotelId">The unique identifier of the hotel to retrieve.</param>
        /// <param name="includeRooms">Indicates whether or not to include room details for the hotel.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing information about the specified hotel. This may include a 200 OK response when successful, or a 404 Not Found response if the hotel is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of hotel information.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpGet("{hotelId}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelAsync(int hotelId, bool includeRooms = false)
        {
            
            var result = new HotelDetailedDto()
            {
                HotelName = "Plaza Hotel",
                Location = "Ramallah, Palestine",
                Description =
                    "Experience luxury and comfort at Plaza Hotel, located in the heart of Ramallah. Our hotel offers a perfect blend of modern amenities and traditional hospitality.",
                Amenities = new List<FilterAmenityDto>()
                {
                    new FilterAmenityDto()
                        { Name = "Free Wi-Fi", Description = "High-speed internet available in all rooms." },
                    new FilterAmenityDto()
                        { Name = "Fitness Center", Description = "Stay fit with our well-equipped fitness center." },
                    new FilterAmenityDto()
                        { Name = "Swimming Pool", Description = "Relax by the poolside and enjoy a refreshing swim." }
                },
                StarRating = 5,
                AvailableRooms = 50,
                Longitude =  (decimal)35.206938,
                Latitude = (decimal)31.916989,
                ImageUrl = "https://cf.bstatic.com/xdata/images/hotel/max1280x900/98271882.jpg?k=cc5964ba081d4c585e3daa9d1c532a8c002c563637238f9bc94896c5daa98496&o=&hp=1"
            };
            return Ok(result);
            
            var hotel = await _hotelInfoRepository.GetHotelAsync(hotelId, includeRooms);

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
        [Authorize(policy:"Admin")]
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
        [Authorize(policy:"Admin")]
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
        public async Task<ActionResult<IEnumerable<RoomAvailabilityResultDto>>> GetRoomsAsync(int hotelId, string checkInDate, string checkOutDate)
        {
            var result =  new List<RoomAvailabilityResultDto>()
            {
                new RoomAvailabilityResultDto()
                {
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

            return Ok(result);
            
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var rooms = await _hotelInfoRepository.GetRoomsAsync(hotelId);

            return Ok(_mapper.Map<IEnumerable<RoomDto>>(rooms));

        }
        
        [HttpGet("{hotelId}/available-rooms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RoomAvailabilityResultDto>>> GetAvailableRoomsAsync(int hotelId, string checkInDate, string CheckOutDate)
        {
            var result =  new List<RoomAvailabilityResultDto>()
            {
                new RoomAvailabilityResultDto()
                {
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

            return Ok(result);
            
            if (!await _hotelInfoRepository.HotelExistsAsync(hotelId))
            {
                return NotFound();
            }

            var rooms = await _hotelInfoRepository.GetRoomsAsync(hotelId);

            return Ok(_mapper.Map<IEnumerable<RoomDto>>(rooms));

        }
        
        /// <summary>
        /// Retrieve a list of reviews to a specific hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel for which you want to retrieve reviews.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of reviews for the specified hotel. This may include a 200 OK response when successful, or a 404 Not Found response if the hotel is not found.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of reviews in the specified hotel.</response>
        /// <response code="404">Indicates that the specified hotel was not found.</response>
        [HttpGet("{hotelId}/reviews")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsAsync(int hotelId)
        {
            List<ReviewDto> result = new List<ReviewDto>
            {
                new ReviewDto { ReviewId = 1, CustomerName = "Alice Johnson", Rating = 4, Description = "Enjoyed my stay. The room was comfortable, and the staff was friendly." },
                new ReviewDto { ReviewId = 2, CustomerName = "Bob Smith", Rating = 5, Description = "Outstanding service! The hotel exceeded my expectations in every way." },
                new ReviewDto { ReviewId = 3, CustomerName = "Charlie Brown", Rating = 3, Description = "Decent stay, but the room could have been cleaner." },
                new ReviewDto { ReviewId = 4, CustomerName = "David Wilson", Rating = 5, Description = "Amazing experience. The hotel staff went above and beyond to make me feel welcome." },
                new ReviewDto { ReviewId = 5, CustomerName = "Eva Miller", Rating = 2, Description = "Disappointing. The room was not as advertised, and the service was lacking." },
                new ReviewDto { ReviewId = 6, CustomerName = "Frank Davis", Rating = 4, Description = "Good value for money. The location was convenient, and the amenities were sufficient." },
                new ReviewDto { ReviewId = 7, CustomerName = "Grace Taylor", Rating = 5, Description = "Absolutely fantastic! I can't wait to come back to this hotel." },
                new ReviewDto { ReviewId = 8, CustomerName = "Harry Turner", Rating = 3, Description = "Average stay. Some improvements could be made to enhance the guest experience." },
                new ReviewDto { ReviewId = 9, CustomerName = "Ivy Clark", Rating = 4, Description = "Pleasant atmosphere and helpful staff. I had a relaxing time at the hotel." },
                new ReviewDto { ReviewId = 10, CustomerName = "Jack White", Rating = 5, Description = "Exceptional service and luxurious accommodations. Highly recommended!" }
            };
            return Ok(result);
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
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDto>> AddRoom(int hotelId, RoomForCreationDto roomForCreationDto)
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
                new { roomId = roomToReturn.Id },
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
        [Authorize(policy:"Admin")]
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
        [HttpGet("{hotelId}/gallery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosAsync(int hotelId)
        {
            hotelId = 1;
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
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                new { photoId = photoToReturn.Id },
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
        [Authorize(policy:"Admin")]
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
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                new { hotelAmenityId = hotelAmenityToReturn.Id },
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
        [Authorize(policy:"Admin")]
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
