namespace WebApp.Dto;

public class ScoreboardDto
{
    public ScoreboardItemDto[] scoreboard { get; set; } = null!;
}

public class ScoreboardItemDto
{
    public string username { get; set; } = string.Empty;
    public int winRate { get; set; }
    public int numberOfGames { get; set; }
}
