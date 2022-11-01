using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public class Sdk
    {
        [JsonProperty("version")]
        public string Version { get; set; } = null!;

        [JsonProperty("version-display")]
        public string VersionDisplay { get; set; } = null!;

        [JsonProperty("runtime-version")]
        public string RuntimeVersion { get; set; } = null!;

        [JsonProperty("vs-version")]
        public string VsVersion { get; set; } = null!;

        [JsonProperty("vs-support")]
        public string VsSupport { get; set; } = null!;

        [JsonProperty("csharp-version")]
        public string CsharpVersion { get; set; } = null!;

        [JsonProperty("fsharp-version")]
        public string FsharpVersion { get; set; } = null!;

        [JsonProperty("vb-version")]
        public string VbVersion { get; set; } = null!;

        [JsonProperty("files")]
        public List<FileDetails> Files { get; set; } = null!;
    }
}
