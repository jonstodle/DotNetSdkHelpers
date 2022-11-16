using DotNetSdkHelpers.Models;
using McMaster.Extensions.CommandLineUtils;

namespace DotNetSdkHelpers.Commands;

[Command("list", "ls", Description = "Lists .NET Core SDKs")]
public class List : Command
{
    // ReSharper disable UnassignedGetOnlyAutoProperty
    [Argument(0, Description = "Filters list to only SDKs starting with the provided string.")]
    public string Filter { get; } = "";

    [Option(CommandOptionType.NoValue, Description = "List SDKs available to download; default is to list installed SDKs.")]
    public bool Available { get; }

    [Option("-r|--release-type", "Show available versions matching the specified release types.", CommandOptionType.MultipleValue)]
    [AllowedValues("sts", "lts")]
    public IEnumerable<string>? ReleaseTypes { get; }

    [Option("-s|--support-phase", "Show available versions matching the specified support phases.", CommandOptionType.MultipleValue)]
    [AllowedValues("preview", "go-live", "active", "maintenance", "eol")]
    public IEnumerable<string>? SupportPhases { get; }

    [Option("--all", CommandOptionType.NoValue, Description = "Show all available versions including all support phases and release types.")]
    public bool All { get; }
    // ReSharper restore UnassignedGetOnlyAutoProperty

    public override async Task Run()
    {
        if (Available)
            await ListAvailable();
        else
            ListInstalled();
    }

    public void ListInstalled()
    {
        if (All || (ReleaseTypes != null && ReleaseTypes.Any()) || (SupportPhases != null && SupportPhases.Any()))
            throw new CliException("The \"--all\", \"--release-type\", and \"--support-phase\" filters are only supported when listing available SDKs.");

        var sdks = DotNet.GetInstalledSdks()
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
        if (All && ((ReleaseTypes != null && ReleaseTypes.Any()) || (SupportPhases != null && SupportPhases.Any())))
            throw new CliException("The \"--all\" parameter may not be used in combination with \"--support-phase\" and \"--release-type\".");

        Console.WriteLine("Getting available releases...");
        var channels = (await DotNet.GetReleaseChannels())
         .Where(CreateFilterPredicate())
         .Where(CreateSupportPhasePredicate())
         .Where(CreateReleaseTypePredicate());

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
                    ReleaseType = r.ReleaseType,
                    Version = r.LatestSdk
                });
        }

        releases = releases
            .OrderByDescending(r => r.ChannelVersion)
            .ThenByDescending(r => r.Version)
            .ToList();

        var longestChannelVersion = releases.Max(c => c.ChannelVersion.Length);
        var longestReleaseVersion = releases.Max(c => c.Version.Length) + 2;
        var longestSupportPhase = releases.Where(c => c.SupportPhase != null).Max(c => c.SupportPhase!.Value.ToString().Length) + 2;

        foreach (var release in releases)
        {
            var supportPhaseDisplay = release.SupportPhase == null ? "" : release.SupportPhase.Value.Display();
            var releaseTypeDisplay = release.ReleaseType == null ? "" : release.ReleaseType.Value.ToString().ToUpperInvariant();
            Console.WriteLine($"{release.ChannelVersion.PadRight(longestChannelVersion)} {$"({release.Version})".PadRight(longestReleaseVersion)}  {supportPhaseDisplay.PadRight(longestSupportPhase)}{releaseTypeDisplay}");
        }
    }

    private Func<ReleaseChannel, bool> CreateFilterPredicate()
    {
        if (!string.IsNullOrEmpty(Filter))
            return r => r.ChannelVersion.StartsWith(Filter, StringComparison.OrdinalIgnoreCase);
        return r => true;
    }

    private Func<ReleaseChannel, bool> CreateSupportPhasePredicate()
    {
        if (SupportPhases != null)
        {
            Func<ReleaseChannel, bool> predicate = r => false;
            foreach (var supportPhase in SupportPhases)
            {
                var parsed = supportPhase switch
                {
                    "preview" => SupportPhase.Preview,
                    "go-live" => SupportPhase.GoLive,
                    "active" => SupportPhase.Active,
                    "maintenance" => SupportPhase.Maintenance,
                    "eol" => SupportPhase.Eol,
                    _ => throw new CliException($"The value \"{supportPhase}\" is not a valid \"--support-phase\"."),
                };
                var capture = predicate;
                predicate = r => capture(r) || r.SupportPhase == parsed;
            }

            return predicate;
        }
        else
        {
            return r => true;
        }
    }

    private Func<ReleaseChannel, bool> CreateReleaseTypePredicate()
    {
        if (ReleaseTypes != null)
        {
            Func<ReleaseChannel, bool> predicate = r => false;
            foreach (var releaseType in ReleaseTypes)
            {
                var parsed = releaseType switch
                {
                    "sts" => ReleaseType.Sts,
                    "lts" => ReleaseType.Lts,
                    _ => throw new CliException($"The value \"{releaseType}\" is not a valid \"--release-type\"."),
                };
                var capture = predicate;
                predicate = r => capture(r) || r.ReleaseType == parsed;
            }

            return predicate;
        }
        else
        {
            return r => true;
        }
    }

    private IEnumerable<ReleaseMeta> ExpandAllReleases(ReleaseChannel channel)
    {
        foreach (var release in channel.Releases)
        {
            var isLatest = channel.LatestSdk == release.Sdk.Version;
            yield return new ReleaseMeta
            {
                ChannelVersion = channel.ChannelVersion,
                SupportPhase = isLatest ? channel.SupportPhase : null,
                ReleaseType = isLatest ? channel.ReleaseType : null,
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
        public SupportPhase? SupportPhase { get; set; }
        public ReleaseType? ReleaseType { get; set; }
        public string ChannelVersion { get; set; } = null!;
        public string Version { get; set; } = null!;
    }
}
