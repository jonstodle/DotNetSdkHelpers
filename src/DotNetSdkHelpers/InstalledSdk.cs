namespace DotNetSdkHelpers;

public class InstalledSdk
{
    public string Version { get; }
    public string Location { get; }

    public InstalledSdk(string version, string location)
    {
        Version = version;
        Location = location;
    }
}
