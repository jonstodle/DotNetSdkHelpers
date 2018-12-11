using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Lists all available releases of .NET Core SDKs")]
    public class Releases
    {
        public static string ReleasesJsonUrl =
            "https://raw.githubusercontent.com/dotnet/core/master/release-notes/releases.json";

        [Option(Description = "Only show LTS releases")]
        public bool OnlyLts { get; }

        public async Task<int> OnExecuteAsync()
        {
            var client = new HttpClient();
            var releases = JsonConvert.DeserializeObject<List<Release>>(await client.GetStringAsync(ReleasesJsonUrl));

            foreach (var release in releases
                .Where(r => !OnlyLts || r.IsRuntimeLts)
                .OrderByDescending(r => r.SdkVersion))
            {
                Console.WriteLine($"{release.SdkVersion}{(release.IsRuntimeLts ? " (LTS)" : "")}");
            }

            return 0;
        }
    }
}