using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFLesDemo.Entities;

public class ScrumboardDbContext : DbContext
{
    public DbSet<Scrumboard> Scrumboards { get; set; }
    public DbSet<ScrumboardColumn> Columns { get; set; }
    public DbSet<Card> Cards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=scrumboardlesdemo;User Id=postgres;Password=postgres;");
    }
}