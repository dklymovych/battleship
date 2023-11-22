using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace Server.Dto;

public class CreateRoomDto
{
    [Required]
    [StringLength(6)]
    public string GameCode { get; set; } = string.Empty;
    [Required]
    public bool IsPublic { get; set; }
    [Required]
    public JsonObject ShipsPosition { get; set; } = null!;
}
