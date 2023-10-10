using AutoMapper;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/Rooms")]
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

        [HttpGet("{id}", Name = "GetRoom")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomAsync(int id)
        {
            var room = await _hotelInfoRepository.GetRoomAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RoomDto>(room));

        }


        [HttpPut("{roomId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int roomId, RoomForUpdateDto room)
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


        [HttpPatch("{roomId}")]
        public async Task<ActionResult> PartiallyUpdateRoom(int roomId,
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
    }
}
