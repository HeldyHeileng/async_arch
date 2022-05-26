using Newtonsoft.Json;

namespace accounting.Models;

public class Account
{
    [JsonProperty("public_id")]
    public Guid AccountId { get; set; }
    [JsonProperty("full_name")]
    public string? FullName { get; set; }
    public int Balance { get; set; } = 0;

}
