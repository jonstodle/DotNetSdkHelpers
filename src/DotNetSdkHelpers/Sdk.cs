using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public partial class Sdk
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("version-display")]
        public string VersionDisplay { get; set; }

        [JsonProperty("runtime-version")]
        public string RuntimeVersion { get; set; }

        [JsonProperty("vs-version")]
        public string VsVersion { get; set; }

        [JsonProperty("vs-support")]
        public string VsSupport { get; set; }

        [JsonProperty("csharp-version")]
        public string CsharpVersion { get; set; }

        [JsonProperty("fsharp-version")]
        public string FsharpVersion { get; set; }

        [JsonProperty("vb-version")]
        public string VbVersion { get; set; }

        [JsonProperty("files")]
        public List<FileDetails> Files { get; set; }
    }
}