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
        public async Task<IActionResult> GetSearchResults(
            string? name, string? searchQuery, int pageSize = 10, int pageNumber = 1)
        {
            var (cities, paginationMetaDataForCities) = await _hotelInfoRepository.GetCitiesAsync(name, searchQuery, pageSize, pageNumber);
            var (hotels, paginationMetaDataForHotels) = await _hotelInfoRepository.GetHotelsAsync(name, searchQuery, pageSize, pageNumber);

            Response.Headers.Add("Cities-Pagination", JsonSerializer.Serialize(paginationMetaDataForCities));
            Response.Headers.Add("Hotels-Pagination", JsonSerializer.Serialize(paginationMetaDataForHotels));

            var citiesToReturn = _mapper.Map<IEnumerable<CitySearchResult>>(cities);
            var hotelsToReturn = _mapper.Map<IEnumerable<HotelSearchResult>>(hotels);
            
            var results = new
            {
                Cities = citiesToReturn,
                Hotels = hotelsToReturn
            };
            return Ok(results);
        }

        [HttpGet("RecentlyVisited")]
        public async Task<IActionResult> GetRecentlyVisitedHotels()
        {
            var (hotels, paginationMetaData) = await _hotelInfoRepository.GetHotelsAsync(null, null, 10, 1);
            var hotelsToReturn = _mapper.Map<IEnumerable<HotelSearchResult>>(hotels.Take(5));
            return Ok(hotelsToReturn);
        }

        [HttpGet("FeaturedDeals")]
        public async Task<IActionResult> GetFeaturedDeals()
        {
            var (hotels, paginationMetaData) = await _hotelInfoRepository.GetHotelsAsync(null, null, 5, 1);
            var hotelsToReturn = _mapper.Map<IEnumerable<HotelSearchResult>>(hotels);
            return Ok(hotelsToReturn);
        }

        [HttpGet("Cities")]
        public async Task<IActionResult> GetCities()
        {
            var (cities, paginationMetaData) = await _hotelInfoRepository.GetCitiesAsync(null, null, 5, 1);
            var citiesToReturn = _mapper.Map<IEnumerable<CitySearchResult>>(cities);
            return Ok(citiesToReturn);
        }
        
    }
}