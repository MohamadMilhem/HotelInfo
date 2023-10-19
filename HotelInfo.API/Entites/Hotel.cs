using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelInfo.API.Entites
{
    public class Hotel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }
        public int CityId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;
        public HotelType HotelType { get; set; }
        public int StarRating { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public ICollection<HotelAmenity> hotelAmenities { get; set; } = new List<HotelAmenity>();

        public Hotel(string name) 
        {
            Name = name;
        }

    }
}
