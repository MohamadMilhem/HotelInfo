﻿namespace HotelInfo.API.Models;

public class RecentHotelResultDto
{
    public string HotelName { get; set; }
    public int StarRating { get; set; }
    public DateTime VisitDate { get; set; }
    public string CityName { get; set; }
    public string? ThumbnailUrl { get; set; }
}