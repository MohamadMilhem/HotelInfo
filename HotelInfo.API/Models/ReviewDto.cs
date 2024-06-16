namespace HotelInfo.API.Models;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public string CustomerName { get; set; }
    public decimal Rating { get; set; }
    public string? Description { get; set; }
}