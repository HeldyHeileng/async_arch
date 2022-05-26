using Microsoft.EntityFrameworkCore;
using analytics.Models;
namespace analytics.Context;

public class ApplicationContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=6543;Database=analytics;Username=postgres;Password=password");
    }
}
