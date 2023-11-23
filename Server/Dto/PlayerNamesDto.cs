using System.ComponentModel.DataAnnotations;

namespace Server.Dto;

public class PlayerNamesDto
{
    [Required]
    public string Player1Name { get; set; } = string.Empty;
    [Required]
    public string Player2Name { get; set; } = string.Empty;
}
