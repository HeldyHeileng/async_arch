using Microsoft.EntityFrameworkCore;
using accounting.Models;
namespace accounting.Context;

public class ApplicationContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<TrackerTask> Tasks { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=6543;Database=accounting;Username=postgres;Password=password");
    }
}
