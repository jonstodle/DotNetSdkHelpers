using System.Diagnostics;
using DotNetSdkHelpers.Models;
using Newtonsoft.Json;

namespace DotNetSdkHelpers;

public static class Helpers
{
    public static async Task<List<ReleaseChannel>> GetReleaseChannels()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(new Uri("https://raw.githubusercontent.com/dotnet/core/master/release-notes/releases-index.json"));
        return JsonConvert.DeserializeObject<ReleasesIndexResponse>(response)!.Releases;
    }

    public static string CaptureOutput(string fileName, string arguments) =>
        Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
        })!
        .StandardOutput.ReadToEnd();

    public static List<InstalledSdk> GetInstalledSdks() =>
        CaptureOutput("dotnet", "--list-sdks")
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
}
