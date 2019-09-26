using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public class ReleasesIndexResponse
    {
        [JsonProperty("releases-index")]
        public List<ReleaseChannel> Releases { get; set; }
    }
}