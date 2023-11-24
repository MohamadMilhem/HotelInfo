namespace HotelInfo.API.Models;

public class SearchResultDto
{
    public string HotelName { get; set; } = String.Empty;
    public int StarRating { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal RoomPrice { get; set; }
    public string? RoomType { get; set; }
    public string CityName { get; set; } = String.Empty;
    public string? RoomPhotoUrl { get; set; }
    public decimal Discount { get; set; }
    public List<RoomAmenityDto> Amenities { get; set; } = new List<RoomAmenityDto>();
}

