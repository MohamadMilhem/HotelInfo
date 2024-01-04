using System.Reflection.Metadata.Ecma335;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelInfo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IHotelInfoRepository _hotelInfoRepository;

    public BookingsController(IHotelInfoRepository hotelInfoRepository)
    {
        _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentException(nameof(hotelInfoRepository));
    }

    [HttpGet("{bookingId}", Name = "GetBooking")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookingDetailsDto>> GetBooking(int bookingId)
    {
        string datePart = DateTime.Now.ToString("yyyyMMdd");
        string randomPart = new Random().Next(1000, 9999).ToString();
        var result = new BookingDetailsDto()
        {
            CustomerName = "Mazen",
            HotelName = "Gaza Hotel",
            RoomNumber = "313",
            RoomType = "Suite",
            BookingDateTime = new DateTime(2023, 10, 7, 6, 25, 0),
            TotalCost = 2000,
            PaymentMethod = "Cash",
            BookingStatus = "Confirmed",
            ConfirmationNumber = $"{datePart}-{randomPart}"
        };
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBooking(BookingRequest bookingRequest)
    {
        string datePart = DateTime.Now.ToString("yyyyMMdd");
        string randomPart = new Random().Next(1000, 9999).ToString();
        var result = new BookingDetailsDto()
        {
            CustomerName = "Mazen",
            HotelName = "Gaza Hotel",
            RoomNumber = "313",
            RoomType = "Suite",
            BookingDateTime = new DateTime(2023, 10, 7, 6, 25, 0),
            TotalCost = 2000,
            PaymentMethod = "Cash",
            BookingStatus = "Confirmed",
            ConfirmationNumber = $"{datePart}-{randomPart}"
        };
        return CreatedAtRoute("GetBooking", new {bookingId = 3}, result);
    }
}