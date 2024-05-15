namespace WebApp.Dto;

using ShipsPositionType = Dictionary<string, List<List<CoordinateDto>>>;

public class CreateRoomDto
{
    public bool IsPublic { get; set; }
    public ShipsPositionType ShipsPosition { get; set; } = null!;
}

public class GameCodeDto
{
    public string gameCode { get; set; } = string.Empty;
}
