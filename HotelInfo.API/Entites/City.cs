using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelInfo.API.Entites
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>(); 
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        
        public City(string name) 
        {
            Name = name;
        }

    }
}
