using HotelInfo.API.Entites;

namespace HotelInfo.API.Models;

public class FeaturedDealDto
{
    public int HotelId { get; set; }
    public decimal OriginalRoomPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalPrice { get; set; }
    public string CityName { get; set; } = String.Empty;
    public string HotelName { get; set; } = String.Empty;
    public int HotelStarRating { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? RoomPhotoUrl { get; set; } 
}