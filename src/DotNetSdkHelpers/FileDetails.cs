using System;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public class FileDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rid")]
        public string Rid { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}