using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command("list", "ls", Description = "Lists .NET Core SDKs")]
    public class List : Command
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        [Argument(0, Description = "Filters list to only SDKs starting with the provided string.")]
        public string Filter { get; } = "";

        [Option(CommandOptionType.NoValue, Description = "List SDKs available to download; default is to list installed SDKs.")]
        public bool Available { get; }

        [Option(CommandOptionType.NoValue, Description = "Show available LTS versions only.")]
        public bool LtsOnly { get; }

        [Option(CommandOptionType.NoValue, Description = "Show available preview versions only.")]
        public bool PreviewOnly { get; }

        [Option("--all", CommandOptionType.NoValue, Description = "Show all available versions including all previews and patch versions.")]
        public bool All { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty

        public override async Task Run()
        {
            if(Available)
                await ListAvailable();
            else
                ListInstalled();
        }

        public void ListInstalled()
        {
            if (LtsOnly || PreviewOnly || All)
                throw new CliException("The \"--all\", \"--lts-only\", and \"--preview-only\" filters are only supported when listing available SDKs.");

            var sdks = GetInstalledSdks()
                .Where(sdk => sdk.Version.StartsWith(Filter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var longestName = sdks.Max(sdk => sdk.Version.Length);

            Console.WriteLine(string.Join(
                Environment.NewLine,
                sdks
                    .Select(sdk => $"{sdk.Version.PadRight(longestName)} [{sdk.Location}]")));
        }

        public async Task ListAvailable()
        {
            if ((All && (LtsOnly || PreviewOnly)) ||(LtsOnly && PreviewOnly))
                throw new CliException("Only one of \"--all\", \"--lts-only\", and \"--preview-only\" are allowed at the same time.");

            Console.WriteLine("Getting available releases...");
            var channels = (await GetReleaseChannels())
                .Where(CreateFilterPredicate())
                .Where(CreateSupportPhasePredicate());

            IEnumerable<ReleaseMeta> releases;
            if (All)
            {
                await Task.WhenAll(channels.Select(c => c.UpdateReleases()));
                releases = channels.SelectMany(ExpandAllReleases);
            }
            else
            {
                releases = channels
                    .Select(r => new ReleaseMeta
                        {
                            ChannelVersion = r.ChannelVersion,
                            SupportPhase = r.SupportPhase,
                            Version = r.LatestSdk
                        });
            }

            releases = releases
                .OrderByDescending(r => r.ChannelVersion)
                .ThenByDescending(r => r.Version)
                .ToList();

            var longestChannelVersion = releases.Max(c => c.ChannelVersion.Length);
            var longestReleaseVersion = releases.Max(c => c.Version.Length) + 2;

            foreach (var release in releases)
            {
                var supportPhaseDisplay = release.SupportPhase == null ? null : $" - {release.SupportPhase.Value.Display()}";
                Console.WriteLine($"{release.ChannelVersion.PadRight(longestChannelVersion)} {$"({release.Version})".PadRight(longestReleaseVersion)}{supportPhaseDisplay}");
            }
        }

        private Func<ReleaseChannel, bool> CreateFilterPredicate()
        {
            if(!string.IsNullOrEmpty(Filter))
                return r => r.ChannelVersion.StartsWith(Filter, StringComparison.OrdinalIgnoreCase);
            return r => true;
        }

        private Func<ReleaseChannel, bool> CreateSupportPhasePredicate()
        {
            if (LtsOnly)
                return r => r.SupportPhase == SupportPhases.Lts;
            if (PreviewOnly)
                return r => r.SupportPhase == SupportPhases.Preview;
            return r => true;
        }

        private IEnumerable<ReleaseMeta> ExpandAllReleases(ReleaseChannel channel)
        {
            foreach (var release in channel.Releases)
            {
                var isLatest = channel.LatestSdk == release.Sdk.Version;
                yield return new ReleaseMeta
                {
                    ChannelVersion = channel.ChannelVersion,
                    SupportPhase = isLatest ? (SupportPhases?)channel.SupportPhase : null,
                    Version = release.Sdk.Version
                };
            }
        }

        /// <summary>
        /// Metadata class supporting the listing of SDK releases. Aggregates data across
        /// release channels and individual releases within the channels.
        /// </summary>
        private class ReleaseMeta
        {
            public SupportPhases? SupportPhase { get; set; }
            public string ChannelVersion { get; set; } = null!;
            public string Version { get; set; } = null!;
        }
    }
}
