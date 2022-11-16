using Newtonsoft.Json;

namespace DotNetSdkHelpers.Models;

public class Sdk
{
    [JsonProperty("version")]
    public string Version { get; set; } = null!;

    [JsonProperty("version-display")]
    public string VersionDisplay { get; set; } = null!;

    [JsonProperty("files")]
    public List<FileDetails> Files { get; set; } = null!;
}
