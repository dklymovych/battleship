using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Database;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) {}

    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<PlayerMove> History { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().HasAlternateKey(p => p.Username);
    }
}
