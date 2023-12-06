using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace Server.Dto;

public class JoinRoomDto
{
    [Required]
    public JsonObject ShipsPosition { get; set; } = null!;
}
