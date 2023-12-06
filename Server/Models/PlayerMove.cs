using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

public class PlayerMove
{
    public int Id { get; set; }
    public string GameCode { get; set; } = string.Empty;
    [ForeignKey("GameCode")]
    public Room Room { get; set; } = null!;
    [ForeignKey("PlayerId")]
    public Player Player { get; set; } = null!;
    public bool IsHit { get; set; }
    public bool IsDestroy { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}
