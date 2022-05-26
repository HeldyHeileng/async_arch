namespace analytics.Models;

public class Transaction
{
    public int Id { get; set; }
    public int Credit { get; set; }
    public int Debit { get; set; }
    public DateTime CreatedAt { get; set; }
    public TransactionType Type { get; set; }
    public Guid? TaskId { get; set; }
}
public enum TransactionType
{
    Withdraw,
    Deposit,
    Payment
}