using HotelInfo.API.Entites;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelInfo.API.Models
{
    public class HotelWithoutRooms
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public City? City { get; set; }
        public int CityId { get; set; }
        public string Description { get; set; } = null!;
        public HotelType HotelType { get; set; }
        public int StarRating { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
