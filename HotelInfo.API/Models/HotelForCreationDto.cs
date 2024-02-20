namespace HotelInfo.API.Models
{
    public class HotelForCreationDto
    { 
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public HotelType HotelType { get; set; }
        public int StarRating { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
