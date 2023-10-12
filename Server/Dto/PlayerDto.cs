using System.ComponentModel.DataAnnotations;

namespace Server.Dto;

public class PlayerDto
{
    [Required]
    [MaxLength(20)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
