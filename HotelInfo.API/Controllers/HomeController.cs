using System.Collections;
using System.Security.Cryptography;
using AutoMapper;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic.CompilerServices;

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
            string? checkInDate, string? checkOutDate, string? city, int starRate , string? sort, int numberOfRooms = 1, int adults = 2, int children = 0)
        {
            var result = new List<SearchResultDto>()
            {
                new SearchResultDto()
                {
                    HotelId = 1,
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
                    HotelId = 2,
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
                    HotelId = 3,
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
                    HotelId = 4,
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
                    HotelId = 5,
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
            if (!string.IsNullOrEmpty(city))
            {
                result = result.Where(res =>
                    res.CityName.Contains(city, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var descending = sort.StartsWith("-");
                var sortProperty = descending ? sort.Substring(1) : sort;

                var propertyInfo  = typeof(SearchResultDto).GetProperty(sortProperty);

                if (propertyInfo  != null)
                {
                    result = descending 
                        ? result.OrderByDescending(res => propertyInfo.GetValue(res, null)).ToList()
                        : result.OrderBy(res => propertyInfo.GetValue(res, null)).ToList();
                }
            }
            return Ok(result);
        }

        [HttpGet("/users/{userId}/recent-hotels")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RecentHotelResultDto>>> GetRecentlyVisitedHotels(int userId)
        {
            var result = new List<RecentHotelResultDto>()
            {
                new RecentHotelResultDto()
                {
                    HotelId = 1,
                    HotelName = "Plaza Hotel",
                    StarRating = 5,
                    VisitDate = new DateTime(2022, 11, 24),
                    CityName = "Ramallah",
                    ThumbnailUrl = "https://cf.bstatic.com/xdata/images/hotel/max1024x768/98271882.jpg?k=cc5964ba081d4c585e3daa9d1c532a8c002c563637238f9bc94896c5daa98496&o=&hp=1",
                    PriceLowerBound = 100,
                    PriceUpperBound = 1000,
                },
                new RecentHotelResultDto()
                {
                    HotelId = 2,
                    HotelName = "Sunset Resort",
                    StarRating = 4,
                    VisitDate = new DateTime(2022, 10, 15),
                    CityName = "Los Angeles",
                    ThumbnailUrl = "https://dynamic-media-cdn.tripadvisor.com/media/photo-o/25/2a/c2/96/sunset-resort.jpg?w=700&h=-1&s=1",
                    PriceLowerBound = 300,
                    PriceUpperBound = 2000,
                },
                new RecentHotelResultDto()
                {
                    HotelId = 3,
                    HotelName = "Ocean View Inn",
                    StarRating = 3,
                    VisitDate = new DateTime(2022, 9, 5),
                    CityName = "New York",
                    ThumbnailUrl = "https://images.pexels.com/photos/258154/pexels-photo-258154.jpeg?auto=compress&cs=tinysrgb&w=600",
                    PriceLowerBound = 500,
                    PriceUpperBound = 2500,
                },
                new RecentHotelResultDto()
                {
                    HotelId = 4,
                    HotelName = "Mountain Retreat",
                    StarRating = 4,
                    VisitDate = new DateTime(2022, 8, 20),
                    CityName = "Denver",
                    ThumbnailUrl = "https://images.pexels.com/photos/2034335/pexels-photo-2034335.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    PriceLowerBound = 700,
                    PriceUpperBound = 2200,
                },
                new RecentHotelResultDto()
                {
                    HotelId = 5,
                    HotelName = "Seaside Retreat",
                    StarRating = 4,
                    VisitDate = new DateTime(2022, 7, 10),
                    CityName = "San Francisco",
                    ThumbnailUrl = "https://images.pexels.com/photos/1134176/pexels-photo-1134176.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    PriceLowerBound = 1000,
                    PriceUpperBound = 2000,
                }
            };

            return Ok(result);
        }
        

        [HttpGet("featured-deals")]
        public async Task<ActionResult<IEnumerable<FeaturedDealDto>>> GetFeaturedDeals()
        {
            var result = new List<FeaturedDealDto>()
            {
                    new FeaturedDealDto()
                    {
                        HotelId = 1,
                        OriginalRoomPrice = 200,
                        Discount = (decimal)0.50,
                        FinalPrice = 100,
                        CityName = "Ramallah",
                        HotelName = "Plaza Hotel",
                        HotelStarRating = 5,
                        Title = "Luxury South Suite",
                        Description = "Experience ultimate luxury in our South Suite at Plaza Hotel. This spacious suite offers breathtaking views of the city and is equipped with state-of-the-art amenities for an unforgettable stay.",
                        RoomPhotoUrl = "https://images.pexels.com/photos/271618/pexels-photo-271618.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    },
                    new FeaturedDealDto()
                    {
                        HotelId = 2,
                        OriginalRoomPrice = 150,
                        Discount = (decimal)0.40,
                        FinalPrice = 90,
                        CityName = "Los Angeles",
                        HotelName = "Sunset Resort",
                        HotelStarRating = 4,
                        Title = "Ocean View Retreat",
                        Description = "Escape to the serenity of Sunset Resort's Ocean View Retreat. Enjoy the calming sounds of the waves and stunning views of the ocean from your cozy room. Perfect for a relaxing getaway.",
                        RoomPhotoUrl = "https://images.pexels.com/photos/172872/pexels-photo-172872.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    },
                    new FeaturedDealDto()
                    {
                        HotelId = 4,
                        OriginalRoomPrice = 180,
                        Discount = (decimal)0.45,
                        FinalPrice = 99,
                        CityName = "Denver",
                        HotelName = "Mountain Retreat",
                        HotelStarRating = 4,
                        Title = "Mountain View Cabin",
                        Description = "Unplug and unwind in our Mountain View Cabin at Mountain Retreat. Surrounded by nature, this cozy cabin offers a peaceful retreat with access to scenic hiking trails.",
                        RoomPhotoUrl = "https://images.pexels.com/photos/271643/pexels-photo-271643.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    },
                    new FeaturedDealDto()
                    {
                        HotelId = 5,
                        OriginalRoomPrice = 130,
                        Discount = (decimal)0.35,
                        FinalPrice = 85,
                        CityName = "San Francisco",
                        HotelName = "Seaside Retreat",
                        HotelStarRating = 4,
                        Title = "Seaside Retreat",
                        Description = "Embrace the coastal charm at Seaside Retreat. Our Seaside Escape package offers a comfortable stay with ocean views and convenient access to the city's attractions.",
                        RoomPhotoUrl = "https://images.pexels.com/photos/2873951/pexels-photo-2873951.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    }
            };
            return Ok(result);
        }

        [HttpGet("/destinations/trending")]
        public async Task<ActionResult<IEnumerable<Destination>>> GetTrendingDestinations()
        {
            var result = new List<Destination>()
            {
                new Destination()
                {
                    CityId = 1,
                    CityName = "Ramallah",
                    CountryName = "Palestine",
                    Description = "Explore the vibrant city of Ramallah, known for its rich history and cultural diversity. Discover historical landmarks, bustling markets, and enjoy the warmth of Palestinian hospitality.",
                    ThumbnailUrl = "https://www.irishtimes.com/resizer/zKAUput0v-pdbzu3keSyhWThHJY=/1600x0/filters:format(jpg):quality(70)/cloudfront-eu-central-1.images.arcpublishing.com/irishtimes/N4P6EWEXC55QQBJJ5Q43X66BPU.jpg"
                },
                new Destination()
                {
                    CityId = 3,
                    CityName = "New York",
                    CountryName = "United States",
                    Description = "Experience the iconic cityscape of New York, where skyscrapers touch the clouds and diverse cultures converge. Visit famous landmarks, explore Central Park, and indulge in world-class dining.",
                    ThumbnailUrl = "https://worldstrides.com/wp-content/uploads/2015/07/iStock_000040849990_Large.jpg"
                },
                new Destination()
                {
                    CityId = 6,
                    CityName = "Paris",
                    CountryName = "France",
                    Description = "Fall in love with the romantic charm of Paris, the 'City of Lights.' Admire the Eiffel Tower, stroll along the Seine River, and savor delicious pastries in charming cafes.",
                    ThumbnailUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4b/La_Tour_Eiffel_vue_de_la_Tour_Saint-Jacques%2C_Paris_ao%C3%BBt_2014_%282%29.jpg/1200px-La_Tour_Eiffel_vue_de_la_Tour_Saint-Jacques%2C_Paris_ao%C3%BBt_2014_%282%29.jpg"
                },
                new Destination()
                {
                    CityId = 7,
                    CityName = "Tokyo",
                    CountryName = "Japan",
                    Description = "Immerse yourself in the futuristic cityscape of Tokyo. Discover a perfect blend of traditional temples and modern technology. Experience vibrant street life and exquisite Japanese cuisine.",
                    ThumbnailUrl = "https://www.gotokyo.org/en/plan/tokyo-outline/images/main.jpg"
                },
                new Destination()
                {
                    CityId = 8,
                    CityName = "Cape Town",
                    CountryName = "South Africa",
                    Description = "Enjoy the breathtaking landscapes of Cape Town. From the iconic Table Mountain to pristine beaches, experience the natural beauty and cultural richness of this South African gem.",
                    ThumbnailUrl = "https://cdn.britannica.com/42/126842-050-0803BC41/Sea-Point-Cape-Town-SAf.jpg"
                }
            };
            return Ok(result);
        }
        
    }
}