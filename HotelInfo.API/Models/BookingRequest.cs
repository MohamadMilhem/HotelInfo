namespace HotelInfo.API.Models;

public class BookingRequest
{
    public string CustomerName { get; set; } = String.Empty;
    public string HotelName { get; set; } = String.Empty;
    public string RoomNumber { get; set; } = String.Empty;
    public string RoomType { get; set; } = String.Empty;
    public DateTime BookingDateTime { get; set; }
    public decimal TotalCost { get; set; }
    public string PaymentMethod { get; set; } = String.Empty;
}