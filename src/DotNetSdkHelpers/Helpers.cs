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

        public static async Task<List<Release>> GetReleases() =>
            JsonConvert.DeserializeObject<ReleasesIndexResponse>(
                    await Client
                        .GetStringAsync(
                            "https://raw.githubusercontent.com/dotnet/core/master/release-notes/releases-index.json"))
                .Releases;

        public static string CaptureOutput(string fileName, string arguments) =>
            Process.Start(new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                })?
                .StandardOutput.ReadToEnd();

        public static List<Sdk> GetInstalledSdks() =>
            CaptureOutput("dotnet", "--list-sdks")
                .Trim()
                .Split(Environment.NewLine)
                .Select(sdk =>
                {
                    var parts = sdk.Split(' ');
                    return new Sdk(parts[0], parts[1]);
                })
                .ToList();
    }
}