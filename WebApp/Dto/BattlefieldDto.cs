namespace WebApp.Dto;

public class BattlefieldDto
{
    public int[] battlefield { get; set; } = null!;
    public bool isMoveAllowed { get; set; }
    public string? winnerName { get; set; }
}

public class CoordinateDto
{
    public int X { get; set; }
    public int Y { get; set; }
}
