using AutoMapper;
using Azure;
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

        [HttpPut("{hotelId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int hotelId, HotelForUpdateDto hotelForUpdateDto)
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


    }
}
