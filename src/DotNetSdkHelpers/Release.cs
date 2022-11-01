using System;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public class Release
    {
        [JsonProperty("release-date")]
        public DateTimeOffset ReleaseDate { get; set; }

        [JsonProperty("release-version")]
        public string ReleaseVersion { get; set; } = null!;

        [JsonProperty("security")]
        public bool Security { get; set; }

        [JsonProperty("release-notes")]
        public Uri ReleaseNotes { get; set; } = null!;

        [JsonProperty("sdk")]
        public Sdk Sdk { get; set; } = null!;
    }
}
