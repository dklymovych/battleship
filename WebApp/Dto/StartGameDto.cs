namespace WebApp.Dto;

public class StartGameDto
{
    public string player1Name { get; set; } = string.Empty;
    public string player2Name { get; set; } = string.Empty;
    public bool makeMove { get; set; }
    public int[] battlefield { get; set; } = null!;
}
