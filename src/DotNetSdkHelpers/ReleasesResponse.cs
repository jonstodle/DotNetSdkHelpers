using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public class ReleasesResponse
    {
        [JsonProperty("channel-version")]
        public string ChannelVersion { get; set; } = null!;

        [JsonProperty("latest-release")]
        public string LatestRelease { get; set; } = null!;

        [JsonProperty("latest-release-date")]
        public DateTimeOffset LatestReleaseDate { get; set; }

        [JsonProperty("latest-runtime")]
        public string LatestRuntime { get; set; } = null!;

        [JsonProperty("latest-sdk")]
        public string LatestSdk { get; set; } = null!;

        [JsonProperty("support-phase")]
        public string SupportPhase { get; set; } = null!;

        [JsonProperty("eol-date")]
        public object EolDate { get; set; } = null!;

        [JsonProperty("lifecycle-policy")]
        public Uri LifecyclePolicy { get; set; } = null!;

        [JsonProperty("releases")]
        public List<Release> Releases { get; set; } = null!;
    }
}
