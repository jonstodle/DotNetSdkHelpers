using Newtonsoft.Json;

namespace DotNetSdkHelpers.Models;

public class ReleaseChannel
{
    [JsonProperty("channel-version")]
    public string ChannelVersion { get; set; } = null!;

    [JsonProperty("latest-sdk")]
    public string LatestSdk { get; set; } = null!;

    [JsonProperty("release-type")]
    public ReleaseType ReleaseType { get; set; }

    [JsonProperty("support-phase")]
    public SupportPhase SupportPhase { get; set; }

    [JsonProperty("releases.json")]
    public Uri ReleasesJson { get; set; } = null!;

    [JsonIgnore]
    public IEnumerable<Release> Releases { get; set; } = null!;

    public async Task UpdateReleases()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(ReleasesJson);
        Releases = JsonConvert.DeserializeObject<ReleasesResponse>(response)!.Releases;
    }
}
