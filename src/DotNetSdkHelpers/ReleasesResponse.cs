using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public class ReleasesResponse
    {
        [JsonProperty("channel-version")]
        public string ChannelVersion { get; set; }

        [JsonProperty("latest-release")]
        public string LatestRelease { get; set; }

        [JsonProperty("latest-release-date")]
        public DateTimeOffset LatestReleaseDate { get; set; }

        [JsonProperty("latest-runtime")]
        public string LatestRuntime { get; set; }

        [JsonProperty("latest-sdk")]
        public string LatestSdk { get; set; }

        [JsonProperty("support-phase")]
        public string SupportPhase { get; set; }

        [JsonProperty("eol-date")]
        public object EolDate { get; set; }

        [JsonProperty("lifecycle-policy")]
        public Uri LifecyclePolicy { get; set; }

        [JsonProperty("releases")]
        public List<Release> Releases { get; set; }
    }
}