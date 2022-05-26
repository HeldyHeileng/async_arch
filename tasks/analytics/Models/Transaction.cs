namespace analytics.Models;

public class Transaction
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public TransactionType Type { get; set; }
    public Guid? TaskId { get; set; }
}

public enum TransactionType
{
    AssignmentCharge,
    CompleteTaskAssess,
    EndOfTheDayPayment
}