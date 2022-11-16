using Newtonsoft.Json;

namespace DotNetSdkHelpers.Models;

public class Release
{
    [JsonProperty("release-version")]
    public string ReleaseVersion { get; set; } = null!;

    [JsonProperty("sdk")]
    public Sdk Sdk { get; set; } = null!;
}
