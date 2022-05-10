using Microsoft.AspNetCore.Mvc;
using System.Web;
using tasks.Context;
using tasks.Kafka;
using tasks.Models;

namespace tasks.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    ApplicationContext _dbContext;
    AccountController _accountController;
    EventProducer _produser;

    public TaskController(ApplicationContext dbContext,
        AccountController accountController, 
        EventProducer producer)
    {
        _dbContext = dbContext;
        _accountController = accountController;
        _produser = producer;
    }

    [HttpPut("AddTask")]
    
    public async void AddTask(string description)
    {
        await _accountController.GetCurrentUser(Uri.EscapeDataString(Request.Cookies["_auth_session"] ?? ""));
        
        var workerId = _accountController.GetRandomWorkerId();
        var task = new TrackerTask()
        {
            Description = description,
            AccountId = workerId,
            PublicId = Guid.NewGuid(),
            Status = TaskStatusEnum.Active
        };

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();
        _produser.Produce("task.Added", task);

    }

    [HttpPost("CompleteTask")] 
    public async Task CompleteTask(int taskId)
    {
        var userAccount = await _accountController.GetCurrentUser(Uri.EscapeDataString(Request.Cookies["_auth_session"] ?? ""));

        var task = _dbContext.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null)
        {
            throw new Exception("task not found");
        }

        if (task.AccountId != userAccount.AccountId)
        {
            throw new Exception("not the owner");
        }

        task.Status = TaskStatusEnum.Completed;
        _dbContext.Tasks.Update(task);
        await _dbContext.SaveChangesAsync();
        _produser.Produce("task.Completed", task);
    }

    [HttpPost("Shuffle")]
    public async Task Shuffle()
    {
        var userAccount = await _accountController.GetCurrentUser(Uri.EscapeDataString(Request.Cookies["_auth_session"] ?? ""));

        if (userAccount.Role != "admin" && userAccount.Role != "manager")
        {
            throw new Exception("not enough role");
        }

        var tasksToShuffle = GetActiveTasksList();
        var activeAccounts = _accountController.GetActiveWorkerList();

        if (activeAccounts == null)
        {
            throw new Exception("no workers");
        }

        Random random = new Random();
        tasksToShuffle.ForEach(t => t.AccountId = activeAccounts[random.Next(activeAccounts.Count)].AccountId);
        _dbContext.Tasks.UpdateRange(tasksToShuffle);
        await _dbContext.SaveChangesAsync();
        _produser.Produce("task.Shuffled", tasksToShuffle);
    }

    [NonAction]
    public List<TrackerTask> GetActiveTasksList()
    {
        return _dbContext.Tasks.Where(t => t.Status == TaskStatusEnum.Active).ToList();
    }
}
