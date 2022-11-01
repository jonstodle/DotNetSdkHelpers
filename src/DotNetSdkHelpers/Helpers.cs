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
        private static readonly HttpClient Client = new();

        public static async Task<List<ReleaseChannel>> GetReleaseChannels() =>
            JsonConvert.DeserializeObject<ReleasesIndexResponse>(
                    await Client.GetStringAsync(
                        new Uri("https://raw.githubusercontent.com/dotnet/core/master/release-notes/releases-index.json")))!
                .Releases;

        public static async Task UpdateReleases(this ReleaseChannel channel)
        {
            if (channel == null)
            {
                return;
            }

            channel.Releases = JsonConvert.DeserializeObject<ReleasesResponse>(
                    await Client.GetStringAsync(channel.ReleasesJson))!
                .Releases;
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
}
