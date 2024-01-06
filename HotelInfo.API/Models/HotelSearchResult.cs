using HotelInfo.API.Entites;

namespace HotelInfo.API.Models;

public class HotelSearchResult
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? CityName { get; set; }
    public string Description { get; set; } = null!;
    public HotelType HotelType { get; set; }
    public int StarRating { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public PhotoDto ThumbnailImage { get; set; }
}