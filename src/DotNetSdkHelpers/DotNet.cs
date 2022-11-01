using System.Diagnostics;
using DotNetSdkHelpers.Models;
using Newtonsoft.Json;

namespace DotNetSdkHelpers;

public static class DotNet
{
    public static List<InstalledSdk> GetInstalledSdks() =>
        CaptureOutput("--list-sdks")
            .Trim()
            .Split(Environment.NewLine)
            .Select(sdk =>
            {
                var splitIndex = sdk.IndexOf(' ', StringComparison.Ordinal);
                var version = sdk[..splitIndex];
                var location = sdk[splitIndex..].Trim().Trim('[', ']');
                return new InstalledSdk(version, location);
            })
            .ToList();

    public static string GetVersion() => CaptureOutput("--version").Trim();

    public static async Task<List<ReleaseChannel>> GetReleaseChannels()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(new Uri("https://raw.githubusercontent.com/dotnet/core/master/release-notes/releases-index.json"));
        return JsonConvert.DeserializeObject<ReleasesIndexResponse>(response)!.Releases;
    }

    private static string CaptureOutput(string arguments) =>
        Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            RedirectStandardOutput = true,
        })!
        .StandardOutput.ReadToEnd();
}
