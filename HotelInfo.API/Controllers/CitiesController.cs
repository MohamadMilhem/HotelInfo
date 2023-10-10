using AutoMapper;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public CitiesController(IHotelInfoRepository hotelInfoRepository, IMapper mapper)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutHotels>>> GetCities(
            string? name, string? searchQuery, int pageSize = 10, int pageNumber = 1)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }
            var (cityEntities, paginationMetaData) = await _hotelInfoRepository.GetCitiesAsync(name, searchQuery, pageSize, pageNumber);
            var results = _mapper.Map<IEnumerable<CityWithoutHotels>>(cityEntities);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityAsync(int id, bool includeHotels = false)
        {
            var city = await _hotelInfoRepository.GetCityAsync(id, includeHotels);

            if (city == null)
            {
                return NotFound();
            }

            if (includeHotels)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutHotels>(city));

        }

    }
}
