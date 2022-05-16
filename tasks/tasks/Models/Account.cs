using Newtonsoft.Json;

namespace tasks.Models;

public class Account
{
    [JsonProperty("public_id")]
    public Guid AccountId { get; set; }
    public string? Role { get; set; }
    [JsonProperty("full_name")]
    public string? FullName { get; set; }
    public bool Active { get; set; }

}
