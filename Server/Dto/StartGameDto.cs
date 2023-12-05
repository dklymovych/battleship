using System.ComponentModel.DataAnnotations;

namespace Server.Dto;

public class StartGameDto
{
    [Required]
    public string Player1Name { get; set; } = string.Empty;
    [Required]
    public string Player2Name { get; set; } = string.Empty;
    [Required]
    public bool MakeMove { get; set; } 
    [Required]
    public int[] Battlefield { get; set; } = null!;
}
