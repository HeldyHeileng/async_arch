using analytics.Context;
using analytics.Models;

namespace analytics.Controllers;

public class TransactionController
{
    private readonly ApplicationContext _dbContext;

    public TransactionController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Save(Transaction transaction)
    {
        _dbContext.Transactions.Add(transaction);
        _dbContext.SaveChanges();
    }
}
