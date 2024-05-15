using System.Text.Json.Nodes;

namespace WebApp.Dto;

public class JoinRoomDto
{
    public JsonObject ShipsPosition { get; set; } = null!;
}
