using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

using ShipsPosition = Dictionary<string, List<List<Coordinate>>>;

public class Room
{
    [Key]
    [Column(TypeName = "varchar(6)")]
    public string GameCode { get; set; } = string.Empty;
    public bool IsPublic { get; set; }

    [ForeignKey("Player1Id")]
    public Player Player1 { get; set; } = null!;
    [ForeignKey("Player2Id")]
    public Player? Player2 { get; set; }
    [ForeignKey("WinnerId")]
    public Player? Winner { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? GameStartedAt { get; set; }
    [Column(TypeName = "timestamp")]
    public DateTime? GameEndedAt { get; set; }

    [Column(TypeName = "json")]
    public ShipsPosition ShipsPosition1 { get; set; } = null!;
    [Column(TypeName = "json")]
    public ShipsPosition? ShipsPosition2 { get; set; }
}
