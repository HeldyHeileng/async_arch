namespace accounting.Models;

public class Transaction
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? TaskId { get; set; }
    public int Debit { get; set; }
    public int Credit { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public TransactionType Type { get; set; }

}

public enum TransactionType
{
    Withdraw,
    Deposit,
    Payment
}