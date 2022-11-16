using Newtonsoft.Json;

namespace DotNetSdkHelpers.Models;

public class ReleasesResponse
{
    [JsonProperty("release-type")]
    public ReleaseType ReleaseType { get; set; }

    [JsonProperty("support-phase")]
    public string SupportPhase { get; set; } = null!;

    [JsonProperty("releases")]
    public List<Release> Releases { get; } = new List<Release>();
}
