using System.Collections;
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
    [Route("api/home")]
    public class HomeController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public HomeController(IHotelInfoRepository hotelInfoRepository, IMapper mapper)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SearchResultDto>>> GetSearchResults(
            string? checkInDate, string? checkOutDate, int starRate, int numberOfRooms = 1, int adults = 2, int children = 0)
        {
            var result = new List<SearchResultDto>()
            {
                new SearchResultDto()
                {
                    HotelName = "Plaza Hotel",
                    Latitude = (decimal)12.32342342,
                    Longitude = (decimal)32.23245675,
                    RoomPrice = 100,
                    RoomType = "Double",
                    CityName = "Ramallah",
                    RoomPhotoUrl = "https://images.pexels.com/photos/164595/pexels-photo-164595.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Discount = (decimal)0.20,
                    Amenities = new List<RoomAmenityDto>()
                    {
                        new RoomAmenityDto()
                        {
                            Name = "wifi",
                            Description = "Very fast wifi in the room."
                        },
                        new RoomAmenityDto()
                        {
                            Name = "Room Service",
                            Description = "Very Fast room service available."
                        }
                    },
                    StarRating = 5,
                },
                new SearchResultDto()
                {
                    HotelName = "Sunset Resort",
                    Latitude = (decimal)34.123456,
                    Longitude = (decimal)-118.321654,
                    RoomPrice = 150,
                    RoomType = "King Suite",
                    CityName = "Los Angeles",
                    RoomPhotoUrl = "https://images.pexels.com/photos/271619/pexels-photo-271619.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Discount = (decimal)0.15,
                    Amenities = new List<RoomAmenityDto>()
                    {
                        new RoomAmenityDto()
                        {
                            Name = "Private Balcony",
                            Description = "Enjoy a private balcony with a view."
                        },
                        new RoomAmenityDto()
                        {
                            Name = "Mini Bar",
                            Description = "Complimentary mini bar in the room."
                        }
                    },
                    StarRating = 4,
                },
                new SearchResultDto()
                {
                    HotelName = "Ocean View Inn",
                    Latitude = (decimal)40.712776,
                    Longitude = (decimal)-74.005974,
                    RoomPrice = 120,
                    RoomType = "Standard",
                    CityName = "New York",
                    RoomPhotoUrl = "https://images.pexels.com/photos/2467285/pexels-photo-2467285.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Discount = (decimal)0.10,
                    Amenities = new List<RoomAmenityDto>()
                    {
                        new RoomAmenityDto()
                        {
                            Name = "City View",
                            Description = "Enjoy a stunning city view from your room."
                        },
                        new RoomAmenityDto()
                        {
                            Name = "Free Breakfast",
                            Description = "Complimentary breakfast included."
                        }
                    },
                    StarRating = 3,
                },
                new SearchResultDto()
                {
                    HotelName = "Mountain Retreat",
                    Latitude = (decimal)39.550051,
                    Longitude = (decimal)-105.782067,
                    RoomPrice = 180,
                    RoomType = "Cabin",
                    CityName = "Denver",
                    RoomPhotoUrl = "https://images.pexels.com/photos/97083/pexels-photo-97083.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Discount = (decimal)0.25,
                    Amenities = new List<RoomAmenityDto>()
                    {
                        new RoomAmenityDto()
                        {
                            Name = "Fireplace",
                            Description = "Cozy up with a fireplace in your cabin."
                        },
                        new RoomAmenityDto()
                        {
                            Name = "Hiking Trails",
                            Description = "Access to scenic hiking trails."
                        }
                    },
                    StarRating = 4,
                },
                new SearchResultDto()
                {
                    HotelName = "Seaside Retreat",
                    Latitude = (decimal)37.774929,
                    Longitude = (decimal)-122.419416,
                    RoomPrice = 130,
                    RoomType = "Ocean View",
                    CityName = "San Francisco",
                    RoomPhotoUrl = "https://images.pexels.com/photos/271643/pexels-photo-271643.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Discount = (decimal)0.12,
                    Amenities = new List<RoomAmenityDto>()
                    {
                        new RoomAmenityDto()
                        {
                            Name = "Ocean View Balcony",
                            Description = "Enjoy the sound of the waves from your balcony."
                        },
                        new RoomAmenityDto()
                        {
                            Name = "Spa Services",
                            Description = "Relax with in-room spa services."
                        }
                    },
                    StarRating = 4,
                },
                
            };


            return Ok(result);
        }

        [HttpGet("/users/{userId}/recent-hotels")]
        public async Task<IActionResult> GetRecentlyVisitedHotels(int userId)
        {
            var (hotels, paginationMetaData) = await _hotelInfoRepository.GetHotelsAsync(null, null, 10, 1);
            var hotelsToReturn = _mapper.Map<IEnumerable<HotelSearchResult>>(hotels.Take(5));
            return Ok(hotelsToReturn);
        }

        [HttpGet("featured-deals")]
        public async Task<IActionResult> GetFeaturedDeals()
        {
            var (hotels, paginationMetaData) = await _hotelInfoRepository.GetHotelsAsync(null, null, 5, 1);
            var hotelsToReturn = _mapper.Map<IEnumerable<HotelSearchResult>>(hotels);
            return Ok(hotelsToReturn);
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var (cities, paginationMetaData) = await _hotelInfoRepository.GetCitiesAsync(null, null, 5, 1);
            var citiesToReturn = _mapper.Map<IEnumerable<CitySearchResult>>(cities);
            return Ok(citiesToReturn);
        }
        
    }
}