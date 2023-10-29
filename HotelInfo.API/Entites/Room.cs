using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelInfo.API.Entites
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string RoomNumber { get; set; } = null!;
        public Hotel? Hotel { get; set; }
        [ForeignKey(nameof(HotelId))]
        public int HotelId { get; set; }
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
        public RoomClass? RoomClass { get; set; }
        [ForeignKey(nameof(RoomClassId))] 
        public int? RoomClassId { get; set; }

        public Room( string roomNumber)
        {

            RoomNumber = roomNumber;
        }
    }
}
