using Newtonsoft.Json;

namespace analytics.Models;

public class Account
{
    [JsonProperty("public_id")]
    public Guid AccountId { get; set; }
    public int Balance { get; set; } = 0;

}
