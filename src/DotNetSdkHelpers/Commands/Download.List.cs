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
                var channels = (await GetReleaseChannels())
                    .Where(CreateFilterPredicate())
                    .OrderByDescending(r => r.ChannelVersion)
                    .ToList();

                var longestChannelVersion = channels.Max(c => c.ChannelVersion.Length);
                var longestLatestSdk = channels.Max(c => c.LatestSdk.Length) + 2;

                foreach (var channel in channels)
                    Console.WriteLine(
                        $"{channel.ChannelVersion.PadRight(longestChannelVersion)} {$"({channel.LatestSdk})".PadRight(longestLatestSdk)} - {channel.SupportPhase.Display()}");
            }
            else
            {
                var channels = (await GetReleaseChannels())
                    .Where(r => r.ChannelVersion.StartsWith(Version, StringComparison.OrdinalIgnoreCase));

                var releaseJsons = await Task.WhenAll(channels.Select(c => GetReleases(c.ReleasesJson)));
                
                foreach (var sdk in releaseJsons
                    .SelectMany(rs => rs.Select(r => r.Sdk))
                    .OrderByDescending(s => s.Version))
                {
                    Console.WriteLine($"{sdk.Version}");
                }
            }
        }

        private Func<ReleaseChannel, bool> CreateFilterPredicate()
        {
            if (LtsOnly)
                return r => r.SupportPhase == SupportPhases.Lts;
            if (PreviewOnly)
                return r => r.SupportPhase == SupportPhases.Preview;
            return r => true;
        }
    }
}