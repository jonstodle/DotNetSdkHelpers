using System;
using System.Collections.Generic;

namespace DotNetSdkHelpers
{
    public class Release : Dictionary<string, string>
    {
        public string RuntimeVersion => this["version-runtime"];
        public string SdkVersion => this["version-sdk"];
        public string RuntimeDisplayVersion => this["version-runtime-display"];
        public string SdkDisplayVersion => this["version-sdk-display"];
        public bool IsLtsRuntime => bool.TryParse(this["lts-runtime"].ToLowerInvariant(), out var isLts) && isLts;
        public bool IsLtsSdk => bool.TryParse(this["lts-sdk"].ToLowerInvariant(), out var isLts) && isLts;
        public bool IsPreview => SdkVersion.Contains("preview", StringComparison.OrdinalIgnoreCase);
    }
}