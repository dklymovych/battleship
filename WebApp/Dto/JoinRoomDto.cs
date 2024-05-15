namespace WebApp.Dto;

using ShipsPositionType = Dictionary<string, List<List<CoordinateDto>>>;

public class JoinRoomDto
{
    public ShipsPositionType ShipsPosition { get; set; } = null!;
}
