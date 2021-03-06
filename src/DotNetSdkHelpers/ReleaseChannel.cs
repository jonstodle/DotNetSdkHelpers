using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public enum SupportPhases
    {
        Preview,
        Current,
        Lts,
        Eol,
        Maintenance,
        Rc,
    }

    public static class SupportPhasesMixins
    {
        public static string Display(this SupportPhases This)
        {
            var result = This.ToString();
            if (This == SupportPhases.Eol || This == SupportPhases.Lts || This == SupportPhases.Rc)
                result = result.ToUpper();
            return result;
        }
    }
    public class ReleaseChannel
    {
        [JsonProperty("channel-version")]
        public string ChannelVersion { get; set; }

        [JsonProperty("latest-release")]
        public string LatestRelease { get; set; }

        [JsonProperty("latest-release-date")]
        public DateTimeOffset LatestReleaseDate { get; set; }

        [JsonProperty("security")]
        public bool Security { get; set; }

        [JsonProperty("latest-runtime")]
        public string LatestRuntime { get; set; }

        [JsonProperty("latest-sdk")]
        public string LatestSdk { get; set; }

        [JsonProperty("product")]
        public string Product { get; set; }

        [JsonProperty("support-phase")]
        public SupportPhases SupportPhase { get; set; }

        [JsonProperty("eol-date")]
        public DateTimeOffset? EolDate { get; set; }

        [JsonProperty("releases.json")]
        public Uri ReleasesJson { get; set; }

        [JsonIgnore]
        public IEnumerable<Release> Releases { get; set; }
    }
}
