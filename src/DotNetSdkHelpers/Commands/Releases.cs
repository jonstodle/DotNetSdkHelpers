using System;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Lists all available releases of .NET Core SDKs")]
    public class Releases : Command
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        [Option(Description = "Only show LTS releases")]
        public bool LtsOnly { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty

        public override async Task Run()
        {
            var releases = await GetReleases();
            
            foreach (var release in releases
                .Where(r => !LtsOnly || r.IsLtsRuntime)
                .OrderByDescending(r => r.SdkVersion))
            {
                Console.WriteLine($"{release.SdkVersion}{(release.IsLtsRuntime ? " (LTS)" : "")}");
            }
        }
    }
}