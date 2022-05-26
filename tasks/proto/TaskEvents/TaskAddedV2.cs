﻿using proto.Tools;

namespace proto;

[ProtoContract]
public class TaskAddedV2
{
    [ProtoMember(1)]
    public Guid PublicId { get; set; }
    [ProtoMember(2)]
    public string Name { get; set; }
    [ProtoMember(3)]
    public string? Description { get; set; }
    [ProtoMember(4)]
    public Guid AccountId { get; set; }
    [ProtoMember(5)]
    public DateTimeWrapper CreatedAt { get; set; }
    [ProtoMember(6)]
    public string JiraId { get; set; }
}