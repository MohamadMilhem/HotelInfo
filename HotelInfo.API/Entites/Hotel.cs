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
        public HotelType HotelType { get; set; }

        public Hotel(string name, string thumbnailImageUrl) 
        {
            Name = name;
        }

    }
}
