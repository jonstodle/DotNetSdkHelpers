using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public partial class ReleasesIndexResponse
    {
        [JsonProperty("releases-index")]
        public List<ReleaseChannel> Releases { get; set; }
    }
}