using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public static class Helpers
    {
        private static readonly HttpClient Client = new HttpClient();

        public static async Task<List<ReleaseChannel>> GetReleaseChannels() =>
            JsonConvert.DeserializeObject<ReleasesIndexResponse>(
                    await Client.GetStringAsync(
                        "https://raw.githubusercontent.com/dotnet/core/master/release-notes/releases-index.json"))
                .Releases;

        public static async Task UpdateReleases(this ReleaseChannel channel) =>
            channel.Releases = JsonConvert.DeserializeObject<ReleasesResponse>(
                    await Client.GetStringAsync(channel.ReleasesJson))
                .Releases;

        public static string CaptureOutput(string fileName, string arguments) =>
            Process.Start(new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                })?
                .StandardOutput.ReadToEnd();

        public static List<InstalledSdk> GetInstalledSdks() =>
            CaptureOutput("dotnet", "--list-sdks")
                .Trim()
                .Split(Environment.NewLine)
                .Select(sdk =>
                {
                    var splitIndex = sdk.IndexOf(' ');
                    var version = sdk.Substring(0, splitIndex);
                    var location = sdk.Substring(splitIndex).Trim().Trim('[', ']');
                    return new InstalledSdk(version, location);
                })
                .ToList();
    }
}
