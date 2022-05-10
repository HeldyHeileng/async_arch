using Newtonsoft.Json;

namespace tasks.Models;

public class AccountEvent
{    
    [JsonProperty("event_id")]
    public Guid EventId { get; set; }
    [JsonProperty("event_version")]
    public int Version { get; set; }
    [JsonProperty("event_time")]
    public DateTime Event_Time { get; set; }
    public string Producer { get; set; }
    [JsonProperty("event_name")]
    public string EventName { get; set; }

    public Account Data { get; set; }
}
