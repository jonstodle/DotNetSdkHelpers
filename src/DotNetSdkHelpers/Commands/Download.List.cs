using System;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command("list", "ls", Description = "List the available versions to download")]
    public class DownloadList : Command
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        [Argument(0, Description = "View all variants of a version by passing the version number.")]
        public string Version { get; } = "";
        
        [Option(CommandOptionType.NoValue, Description = "Show LTS versions only.")]
        public bool LtsOnly { get; }
        
        [Option(CommandOptionType.NoValue, Description = "Show preview versions only.")]
        public bool PreviewOnly { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty

        public override async Task Run()
        {
            if (LtsOnly && PreviewOnly && string.IsNullOrWhiteSpace(Version))
                throw new CliException("Only one of \"--lts-only\" or \"--preview-only\" are allowed at the same time.");
            
            Console.WriteLine("Getting available releases...");

            if (string.IsNullOrWhiteSpace(Version))
            {
                var releases = (await GetReleases())
                    .Where(CreateFilterPredicate())
                    .OrderByDescending(r => r.ChannelVersion)
                    .ToList();

                var longestChannelVersion = releases.Max(r => r.ChannelVersion.Length);
                var longestLatestSdk = releases.Max(r => r.LatestSdk.Length) + 2;

                foreach (var release in releases)
                    Console.WriteLine(
                        $"{release.ChannelVersion.PadRight(longestChannelVersion)} {$"({release.LatestSdk})".PadRight(longestLatestSdk)} - {release.SupportPhase.Display()}");
            }
            else
            {
                var channels = await GetReleases();
            }
        }

        private Func<Release, bool> CreateFilterPredicate()
        {
            if (LtsOnly)
                return r => r.SupportPhase == SupportPhases.Lts;
            if (PreviewOnly)
                return r => r.SupportPhase == SupportPhases.Preview;
            return r => true;
        }
    }
}