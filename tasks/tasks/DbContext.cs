using Microsoft.EntityFrameworkCore;
using tasks.Models;

namespace tasks.Context;

public class ApplicationContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<TrackerTask> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=6543;Database=tasks;Username=postgres;Password=password");
    }
}
