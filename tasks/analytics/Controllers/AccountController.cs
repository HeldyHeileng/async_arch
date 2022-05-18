using analytics.Kafka;
using analytics.Models;
using Microsoft.EntityFrameworkCore;
using analytics.Context;
using Microsoft.AspNetCore.Mvc;

namespace analytics.Controllers;

public class AccountController
{
    private readonly ApplicationContext _dbContext;

    public AccountController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    [NonAction]
    public void CreateAccount(Account account)
    {
        var dbAccount = _dbContext
            .Accounts
            .AsNoTracking()
            .FirstOrDefault(a => a.AccountId == account.AccountId);

        if (dbAccount != null)
        {
            return;
        }

        _dbContext.Accounts.Add(account);
        _dbContext.SaveChanges();
        return;
    }


    [NonAction]
    public void UpdateAccount(Account account)
    {
        var dbAccount = _dbContext
            .Accounts
            .AsNoTracking()
            .FirstOrDefault(a => a.AccountId == account.AccountId);

        if (dbAccount == null)
        {
            return;
        }

        _dbContext.Update(account);
        _dbContext.SaveChanges();
        return;
    }
}
