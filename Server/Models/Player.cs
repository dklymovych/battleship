using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

public class Player
{
    public int Id { get; set; }
    [Column(TypeName = "varchar(20)")]
    public string Username { get; set; } = string.Empty;
    [Column(TypeName = "varchar")]
    public string Password { get; set; } = string.Empty;
    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }
}
