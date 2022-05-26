using proto.Tools;

namespace proto;

[ProtoContract]
public class TransactionAppliedV1
{
    [ProtoMember(1)]
    public int Id { get; set; }
    [ProtoMember(2)]
    public Guid PublicId { get; set; }
    [ProtoMember(3)]
    public Guid AccountId { get; set; }
    [ProtoMember(4)]
    public Guid? TaskId { get; set; }
    [ProtoMember(5)]
    public int Credit { get; set; }
    [ProtoMember(6)]
    public int Debit { get; set; }
    [ProtoMember(7)]
    public string Description { get; set; }
    [ProtoMember(8)]
    public DateTimeWrapper CreatedAt { get; set; }
    [ProtoMember(9)]
    public TransactionTypeV1 Type { get; set; }
}

public enum TransactionTypeV1
{
    Withdraw,
    Deposit,
    Payment
}