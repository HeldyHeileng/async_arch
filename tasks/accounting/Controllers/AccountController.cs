using accounting.Kafka;
using accounting.Models;
using Microsoft.EntityFrameworkCore;
using accounting.Context;
using Microsoft.AspNetCore.Mvc;

namespace accounting.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController
{
    private readonly ApplicationContext _dbContext;
    private readonly EventProducer _eventProducer;

    public AccountController(ApplicationContext dbContext, EventProducer eventProducer)
    {
        _dbContext = dbContext;
        _eventProducer = eventProducer;
    }

    [NonAction]
    public void CreateOrUpdateAccount(Account account)
    {
        var dbAccount = _dbContext
            .Accounts
            .AsNoTracking()
            .FirstOrDefault(a => a.AccountId == account.AccountId);

        if (dbAccount == null)
        {
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return;
        }

        _dbContext.Update(account);
        _dbContext.SaveChanges();
        return;
    }
    
    [HttpGet("Get")]
    public Account? Get(Guid id)
    {
        return _dbContext
            .Accounts
            .AsNoTracking()
            .FirstOrDefault(a => a.AccountId == id);
    }

    [HttpPost("EndOfTheDayTrigger")]
    public void EndOfTheDayImitation()
    {
        var accountsToPay = _dbContext.Accounts.AsNoTracking().Where(x => x.Balance > 0).ToList();

        var transactions = accountsToPay.Select(x => new Transaction()
        {
            PublicId = new Guid(),
            AccountId = x.AccountId,
            Amount = x.Balance,
            Description = $"End of the day payment for {DateTime.UtcNow.Date}",
            Type = TransactionType.EndOfTheDayPayment,
            CreatedAt = DateTime.UtcNow,
        })
            .ToList();

        accountsToPay.ForEach(a => a.Balance = 0);


        _dbContext.AddRange(transactions);
        _dbContext.UpdateRange(accountsToPay);
        _dbContext.SaveChanges();

        transactions.ForEach(async t => await _eventProducer.Produce("transaction", "transaction-added", t));
        accountsToPay.ForEach(async a => await _eventProducer.Produce("account", "account-balance-changed", a));
    }
    
}
