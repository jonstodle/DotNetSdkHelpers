namespace DotNetSdkHelpers
{
    public readonly struct Sdk
    {
        public string Version { get; }
        public string Location { get; }
        
        public Sdk(string version, string location)
        {
            Version = version;
            Location = location;
        }

        public bool IsDefault => string.IsNullOrWhiteSpace(Version) && string.IsNullOrWhiteSpace(Location);
    }
}