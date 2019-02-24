using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetSdkHelpers
{
    public static class Helpers
    {
        private static HttpClient _client = new HttpClient();

        public static async Task<List<Release>> GetReleases()
        {
            return JsonConvert.DeserializeObject<List<Release>>(
                await _client
                    .GetStringAsync(
                        "https://raw.githubusercontent.com/dotnet/core/master/release-notes/releases.json"));
        }

        public static string CaptureOutput(string fileName, string arguments) =>
            Process.Start(new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                })?
                .StandardOutput.ReadToEnd();
    }
}