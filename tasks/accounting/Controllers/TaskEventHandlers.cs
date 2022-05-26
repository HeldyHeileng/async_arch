using accounting.Kafka;
using accounting.Models;
using accounting.Context;
using Microsoft.EntityFrameworkCore;

namespace accounting.Controllers;

public class TaskEventHandlers
{
    private readonly ApplicationContext _dbContext;
    private readonly EventProducer _eventProducer;
    private readonly AccountController _accountController;

    public TaskEventHandlers(
        ApplicationContext dbContext,
        EventProducer eventProducer,
        AccountController accountController)
    {
        _dbContext = dbContext;
        _eventProducer = eventProducer;
        _accountController = accountController;
    }
    public async Task TaskAddedHandler(TrackerTask task)
    {
        Random random = new Random();
        task.AssignCost = -random.Next(10, 20);
        task.CompleteCost = random.Next(20, 40);

        var transaction = new Transaction()
        {
            PublicId = new Guid(),
            AccountId = task.AccountId,
            TaskId = task.PublicId,
            Description = $"{task.JiraId} {task.Name}: {task.Description}",
            Type = TransactionType.AssignmentCharge,
            Amount = task.AssignCost,
            CreatedAt = DateTime.UtcNow
        };

        var account = _accountController.Get(task.AccountId);

        if (account == null)
        {
            throw new Exception("too soon");
        }

        account.Balance += task.AssignCost;

        using var tran =_dbContext.Database.BeginTransaction();

        try
        {
            _dbContext.Tasks.Add(task);
            _dbContext.Transactions.Add(transaction);
            _dbContext.Accounts.Update(account);
            _dbContext.SaveChanges();

            tran.Commit();
        }
        catch (Exception ex)
        {
            tran.Rollback();
            throw new Exception("something's gone wrong");
        }

        await _eventProducer.Produce("transaction", "transaction-added", transaction);
        await _eventProducer.Produce("account", "account-balance-changed", account);
    }

    public async Task TaskCompletedHandler(TrackerTask task)
    {
        var dbTask = _dbContext.Tasks.AsNoTracking().FirstOrDefault(x => x.PublicId == task.PublicId);

        if (dbTask == null)
        {
            throw new Exception("task not found");
        }

        var transaction = new Transaction()
        {
            PublicId = new Guid(),
            AccountId = dbTask.AccountId,
            TaskId = dbTask.PublicId,
            Description = $"{dbTask.JiraId} {dbTask.Name}: {dbTask.Description}",
            Type = TransactionType.CompleteTaskAssess,
            Amount = dbTask.CompleteCost,
            CreatedAt = DateTime.UtcNow
        };

        var account = _accountController.Get(task.AccountId);

        if (account == null)
        {
            throw new Exception("too soon");
        }

        account.Balance += dbTask.CompleteCost;

        using var tran = _dbContext.Database.BeginTransaction();

        try
        {
            _dbContext.Transactions.Add(transaction);
            _dbContext.Accounts.Update(account);
            _dbContext.SaveChanges();

            tran.Commit();
        }
        catch (Exception ex)
        {
            tran.Rollback();
            throw new Exception("something's gone wrong");
        }

        await _eventProducer.Produce("transaction", "transaction-added", transaction);
        await _eventProducer.Produce("account", "account-balance-changed", account);
    }

    public async Task TaskShuffledHandler(TrackerTask task)
    {
        var dbTask = _dbContext.Tasks.AsNoTracking().FirstOrDefault(x => x.PublicId == task.PublicId);

        if (dbTask == null)
        {
            throw new Exception("task not found");
        }
        
       var transaction = new Transaction()
        {
            PublicId = new Guid(),
            AccountId = dbTask.AccountId,
            TaskId = dbTask.PublicId,
            Description = $"{dbTask.JiraId} {dbTask.Name}: {dbTask.Description}",
            Type = TransactionType.AssignmentCharge,
            Amount = dbTask.AssignCost,
            CreatedAt = DateTime.UtcNow
        };

        var account = _accountController.Get(task.AccountId);

        if (account == null)
        {
            throw new Exception("too soon");
        }

        account.Balance -= task.AssignCost;

        using var tran = _dbContext.Database.BeginTransaction();

        try
        {
            _dbContext.Transactions.Add(transaction);
            _dbContext.Accounts.Update(account);
            _dbContext.SaveChanges();

            tran.Commit();
        }
        catch (Exception ex)
        {
            tran.Rollback();
            throw new Exception("something's gone wrong");
        }

        await _eventProducer.Produce("transaction", "transaction-added", transaction);
        await _eventProducer.Produce("account", "account-balance-changed", account);
    }
}
