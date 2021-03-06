using analytics.Context;
using Microsoft.AspNetCore.Mvc;

namespace analytics.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController
{
    private readonly ApplicationContext _dbContext;

    public AnalyticsController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("TodayIncome")] 
    public int GetTodayIncome()
    {
        return _dbContext.Transactions
            .Where(x => 
                x.CreatedAt.Date == DateTime.UtcNow.Date &&
                x.Type != Models.TransactionType.EndOfTheDayPayment)
            .Sum(x => x.Amount);
    }

    [HttpGet("NegativeAccountsCount")]
    public int GetNegativeAccountsCount()
    {
        return _dbContext.Accounts
            .Where(x => x.Balance < 0)
            .Count();
    }

    [HttpGet("TodaysMostValuableTask")]
    public int GetTodaysMostValuableTask()
    {
        return _dbContext.Transactions
            .Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date && x.Type == Models.TransactionType.CompleteTaskAssess)
            .Max(x => x.Amount);
    }
 }
