using System.ComponentModel.DataAnnotations;

namespace Server.Dto;

public class BattlefieldDto
{
    [Required]
    public int[] Battlefield { get; set; } = null!;
    [Required]
    public bool IsMoveAllowed { get; set; }
}
