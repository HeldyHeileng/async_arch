using tasks.Models;
using tasks.Context;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace tasks.Controllers;

public class AccountController
{
    private readonly ApplicationContext _dbContext; 

    public AccountController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Account> GetActiveWorkerList()
    {
        return _dbContext
            .Accounts
            .Where(a => a.Role == "worker")
            .ToList();
    }

    public Guid GetRandomWorkerId()
    {
        var list = GetActiveWorkerList();
        var random = new Random();
        int index = random.Next(list.Count);
        random.Next(list.Count);
        return list[index].AccountId;
    }

    public async Task<Account> GetCurrentUser(string _auth_session)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("cookie", "_auth_session=" + _auth_session);
        var json = await client.GetStringAsync("http://localhost:3000/accounts/current.json");
        var account = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(json);
        if (account == null)
        {
            throw new Exception("user not found");
        }
        return account;
    }

    public void CreateOrUpdateAccount(Account account)
    {
        var dbAccount =_dbContext
            .Accounts
            .AsNoTracking()
            .FirstOrDefault(a => a.AccountId == account.AccountId);

        if (dbAccount == null)
        {
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return;
        }

        account.Role ??= dbAccount.Role;

        _dbContext.Update(account);
        _dbContext.SaveChanges();
        return;
    }

    public Account? Get(Guid id)
    {
        return _dbContext.Accounts.FirstOrDefault(a => a.AccountId == id);
    }
}
