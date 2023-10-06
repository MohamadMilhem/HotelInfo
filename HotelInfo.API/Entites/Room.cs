using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelInfo.API.Entites
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string RoomNumber { get; set; } = null!;

        public Room( string roomNumber, string thumbnailImageUrl)
        {

            RoomNumber = roomNumber;
        }
    }
}
