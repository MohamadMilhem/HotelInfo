namespace HotelInfo.API.Models;

public class Destination
{
    public string CityName { get; set; } = String.Empty;
    public string CountryName { get; set; } = String.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
}