namespace HotelInfo.API.Models;

public class RoomAvailabilityResultDto
{
    public int RoomId { get; set; }
    public int RoomNumber { get; set; }
    public string? RoomPhotoUrl { get; set; }
    public string? RoomType { get; set; }
    public int CapacityOfAdults { get; set; }
    public int CapacityOfChildren { get; set; }
    public List<FilterAmenityDto> RoomAmenities { get; set; }
    public decimal Price { get; set; }
    public bool Availability { get; set; }
}