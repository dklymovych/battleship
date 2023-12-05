using System.ComponentModel.DataAnnotations;

namespace Server.Dto;

public class CoordinateDto
{
    [Required]
    public int X { get; set; }
    [Required]
    public int Y { get; set; }
}
