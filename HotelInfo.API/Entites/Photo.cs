using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelInfo.API.Entites
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }
        public int? CityId { get; set; }

        [ForeignKey(nameof(HotelId))]
        public Hotel? Hotel { get; set; }
        public int? HotelId { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }
        public int? RoomId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = null!;
        public Photo(string url)
        {
            Url = url;
        }
    }
}
