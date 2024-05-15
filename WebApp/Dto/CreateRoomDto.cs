using System.Text.Json.Nodes;

namespace WebApp.Dto;

public class CreateRoomDto
{
    public bool IsPublic { get; set; }
    public JsonObject ShipsPosition { get; set; } = null!;
}

public class GameCodeDto
{
    public string gameCode { get; set; } = string.Empty;
}
