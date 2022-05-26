
namespace proto;

[ProtoContract]
public class AccountBalanceChangedV1
{
    [ProtoMember(1)]
    public Guid AccountId { get; set; }
    [ProtoMember(2)]
    public int Balance { get; set; }

}
