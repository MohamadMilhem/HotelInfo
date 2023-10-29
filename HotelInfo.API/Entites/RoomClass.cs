﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelInfo.API.Entites;

public class RoomClass
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public decimal StandardCost { get; set; }
    public string? Description { get; set; }
    public ICollection<Room> Rooms { get; set; } = new List<Room>();

    public RoomClass(decimal standardCost, string? description)
    {
        StandardCost = standardCost;
        Description = description;
    }
}