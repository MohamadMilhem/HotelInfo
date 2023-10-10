using AutoMapper;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        [HttpGet("{id}", Name = "GetCity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<CityDto>> CreateCity(CityForCreationDto city)
        {
            var cityToStore = _mapper.Map<Entites.City>(city);

            await _hotelInfoRepository.CreateCityAsync(cityToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var cityToReturn = _mapper.Map<CityDto>(cityToStore);

            return CreatedAtRoute("GetCity",
                new { id = cityToReturn.Id },
                cityToReturn);
        }

        [HttpPut("{cityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCity(int cityId, CityForUpdateDto cityForUpdateDto) 
        { 
            var cityEntity = await _hotelInfoRepository.GetCityAsync(cityId, false);

            if (cityEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(cityForUpdateDto, cityEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{cityId}")]
        public async Task<ActionResult> PartiallyUpdateCity(int cityId,
            JsonPatchDocument<CityForUpdateDto> patchDocument)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var cityEntity = await _hotelInfoRepository.GetCityAsync(cityId, false);


            var cityToUpdate = _mapper.Map<CityForUpdateDto>(cityEntity);

            patchDocument.ApplyTo(cityToUpdate, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(cityToUpdate, cityEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }

    }

    



    
}
