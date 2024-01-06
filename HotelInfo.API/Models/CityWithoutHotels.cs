namespace HotelInfo.API.Models
{
    public class CityWithoutHotels
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = null!;
    }
}
