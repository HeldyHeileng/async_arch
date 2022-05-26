namespace proto;

[ProtoContract]
public class BusEvent<T>
{    
    [ProtoMember(1)]
    public Guid EventId { get; set; }
    [ProtoMember(2)]
    public int Version { get; set; }
    [ProtoMember(3)]
    public DateTime ProducedAt { get; set; }
    [ProtoMember(4)]
    public string Producer { get; set; }
    [ProtoMember(5)]
    public string EventName { get; set; }
    [ProtoMember(6)]
    public T Data { get; set; }
}
