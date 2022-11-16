using Newtonsoft.Json;

namespace DotNetSdkHelpers.Models;

public class FileDetails
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("rid")]
    public string Rid { get; set; } = null!;

    [JsonProperty("url")]
    public Uri Url { get; set; } = null!;

    [JsonProperty("hash")]
    public string Hash { get; set; } = null!;
}
