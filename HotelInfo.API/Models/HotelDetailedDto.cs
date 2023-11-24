﻿namespace HotelInfo.API.Models;

public class HotelDetailedDto
{
    public string HotelName { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public List<FilterAmenityDto> Amenities { get; set; }
    public int StarRating { get; set; }
    public int AvailableRooms { get; set; }
    public string ImageUrl { get; set; }    
}