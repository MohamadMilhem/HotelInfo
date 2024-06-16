using HotelInfo.API.Entites;

namespace HotelInfo.API.Models;

public class CitySearchResult
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public PhotoDto ThumbnailImage { get; set; }
}