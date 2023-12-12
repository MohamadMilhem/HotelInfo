﻿using System.Reflection.Metadata.Ecma335;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelInfo.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IHotelInfoRepository _hotelInfoRepository;

    public BookingsController(IHotelInfoRepository hotelInfoRepository)
    {
        _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentException(nameof(hotelInfoRepository));
    }

    [HttpGet("{bookingId}", Name = "GetBooking")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BookingDetailsDto>> GetBooking(int bookingId)
    {
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
        };
        return result;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBooking(BookingDetailsDto bookingDetailsDto)
    {
        return CreatedAtRoute("GetBooking", new {bookingId = 3}, bookingDetailsDto);
    }
}