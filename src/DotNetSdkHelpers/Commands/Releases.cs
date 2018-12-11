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
        [Option(Description = "Only show LTS releases")]
        public bool OnlyLts { get; }

        public async Task<int> OnExecuteAsync()
        {
            var releases = await Program.GetReleases();
            
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