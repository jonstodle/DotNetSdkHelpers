namespace DotNetSdkHelpers
{
    public readonly struct InstalledSdk
    {
        public string Version { get; }
        public string Location { get; }
        
        public InstalledSdk(string version, string location)
        {
            Version = version;
            Location = location;
        }

        public bool IsDefault => string.IsNullOrWhiteSpace(Version) && string.IsNullOrWhiteSpace(Location);
    }
}