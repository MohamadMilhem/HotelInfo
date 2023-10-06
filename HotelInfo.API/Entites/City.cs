﻿using System.ComponentModel.DataAnnotations;
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
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>(); 
        
        public City(string name, string thumbnailImageUrl) 
        {
            Name = name;
        }

    }
}
