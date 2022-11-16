using Newtonsoft.Json;

namespace DotNetSdkHelpers.Models;

public class ReleasesIndexResponse
{
    [JsonProperty("releases-index")]
    public List<ReleaseChannel> Releases { get; } = new List<ReleaseChannel>();
}
