﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfoAPI.Entities;

public class City
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [MaxLength(50)]
    [Required] 
    public string Name { get; set; }
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public ICollection<PointOfInterest> PointsOfInterests { get; set; } = new List<PointOfInterest>();


    public City(string name)
    {
        Name = name;
    }
}