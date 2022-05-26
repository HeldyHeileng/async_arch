using accounting.Kafka;
using accounting.Models;
using accounting.Context;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace accounting.Controllers;

public class TaskEventHandlers
{
    private readonly ApplicationContext _dbContext;
    private readonly EventProducer _eventProducer;
    private readonly AccountController _accountController;
    private readonly IMapper _mapper;

    public TaskEventHandlers(
        ApplicationContext dbContext,
        EventProducer eventProducer,
        AccountController accountController,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _eventProducer = eventProducer;
        _accountController = accountController;
        _mapper = mapper;
    }
    public async Task TaskAddedHandler(proto.TaskAddedV2 eventData)
    {
        try
        { 
            TrackerTask task = _mapper.Map<TrackerTask>(eventData);

            var random = new Random();
            task.AssignCost = -random.Next(10, 20);
            task.CompleteCost = random.Next(20, 40);

            var transaction = new Transaction()
            {
                PublicId = Guid.NewGuid(),
                AccountId = task.AccountId,
                TaskId = task.PublicId,
                Description = $"{task.JiraId} {task.Name}: {task.Description}",
                Type = TransactionType.Withdraw,
                Debit = task.AssignCost,
                CreatedAt = DateTime.UtcNow
            };

            var account = _accountController.Get(task.AccountId);

            if (account == null)
            {
                //почему-то задача пришла раньше чем пользователь, сохраним его имя и прочая инфа могут дойти попозже
                _accountController.CreateOrUpdateAccount(new Account() { AccountId = task.AccountId }); 
                account = _accountController.Get(task.AccountId);
                if (account == null) { return; }
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
            catch (Exception)
            {
                //в случае если событие не выполнилось для начала можно просто попробовать повторить выполнение несколько раз,
                //и если n попыток не удались записать это в логи или метрики
                //если со временем окажется что слишком много событий нужно перезапускать руками,
                //можно будет придумать пути автоматизации имея примеры проблем на руках
                tran.Rollback();
                throw new Exception("something's gone wrong");
            }

            await _eventProducer.Produce<proto.TransactionAppliedV1>("transaction", "transaction-applied", transaction);
            await _eventProducer.Produce<proto.AccountBalanceChangedV1>("account", "account-balance-changed", account);
    }
        catch(Exception ex)
        {
            //log
        }
    }

    public async Task TaskCompletedHandler(proto.TaskCompletedV1 eventData)
    {
        try 
        { 
            TrackerTask task = _mapper.Map<TrackerTask>(eventData);

            var dbTask = _dbContext.Tasks.AsNoTracking().FirstOrDefault(x => x.PublicId == task.PublicId);

            if (dbTask == null)
            {
                // вот тут можно сгенерировать цены и сохранить таску без описания,
                // но тогда выше при обработке добавления таски нужно будет проверить есть ли таска уже и пропустить генерирование цен если есть
                throw new Exception("task not found");
            }

            var transaction = new Transaction()
            {
                PublicId = Guid.NewGuid(),
                AccountId = dbTask.AccountId,
                TaskId = dbTask.PublicId,
                Description = $"{dbTask.JiraId} {dbTask.Name}: {dbTask.Description}",
                Type = TransactionType.Deposit,
                Debit = dbTask.CompleteCost,
                CreatedAt = DateTime.UtcNow
            };

            var account = _accountController.Get(task.AccountId);

            if (account == null)
            {
                //почему-то задача пришла раньше чем пользователь, сохраним его имя и прочая инфа могут дойти попозже
                _accountController.CreateOrUpdateAccount(new Account() { AccountId = task.AccountId });
                account = _accountController.Get(task.AccountId);
                if (account == null) { return; }
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
            catch (Exception)
            {
                tran.Rollback();
                throw new Exception("something's gone wrong");
            }

            await _eventProducer.Produce<proto.TransactionAppliedV1>("transaction", "transaction-applied", transaction);
            await _eventProducer.Produce<proto.AccountBalanceChangedV1>("account", "account-balance-changed", account);
        }
        catch(Exception ex)
        {
            //log
        }
    }

    public async Task TaskShuffledHandler(proto.TaskShuffledV1 eventData)
    {
        try 
        {
            TrackerTask task = _mapper.Map<TrackerTask>(eventData);
            var dbTask = _dbContext.Tasks.AsNoTracking().FirstOrDefault(x => x.PublicId == task.PublicId);

            if (dbTask == null)
            {
                // тут также если шафл вдруг пришел раньше создания - можно сохранить то что знаем о таске, задать ей цены
                throw new Exception("task not found");
            }

            var transaction = new Transaction()
            {
                PublicId = Guid.NewGuid(),
                AccountId = dbTask.AccountId,
                TaskId = dbTask.PublicId,
                Description = $"{dbTask.JiraId} {dbTask.Name}: {dbTask.Description}",
                Type = TransactionType.Withdraw,
                Debit = dbTask.AssignCost,
                CreatedAt = DateTime.UtcNow
            };

            var account = _accountController.Get(task.AccountId);

            if (account == null)
            {
                _accountController.CreateOrUpdateAccount(new Account() { AccountId = task.AccountId });
                account = _accountController.Get(task.AccountId);
                if (account == null) { return; }
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
            catch (Exception)
            {
                tran.Rollback();
                throw new Exception("something's gone wrong");
            }

            await _eventProducer.Produce<proto.TransactionAppliedV1>("transaction", "transaction-applied", transaction);
            await _eventProducer.Produce<proto.AccountBalanceChangedV1>("account", "account-balance-changed", account);
        }
        catch(Exception ex)
        {
            //log
        }
    }
}
