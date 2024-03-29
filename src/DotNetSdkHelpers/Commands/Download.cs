using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using DotNetSdkHelpers.Models;
using McMaster.Extensions.CommandLineUtils;

namespace DotNetSdkHelpers.Commands;

[Command(Description = "Downloads the provided release version & platform.")]
public class Download : Command
{
    // ReSharper disable UnassignedGetOnlyAutoProperty
    [Argument(0, Description = "'active', 'go-live', 'preview' or a specific version. (Default: 'active')")]
    public string Version { get; } = "active";

    [Option(CommandOptionType.NoValue, Description =
        "Indicate the version specified is the runtime version, NOT the SDK version.")]
    public bool Runtime { get; }

    [Option(CommandOptionType.SingleValue, Description =
        "The platform to download for. Defaults to the current platform on Windows and MacOS.")]
    public string? Platform { get; }

    [Option(CommandOptionType.NoValue, Description = "Indicate that validation of hash should NOT be done.")]
    public bool NoHashValidation { get; }
    // ReSharper restore UnassignedGetOnlyAutoProperty

    public override async Task Run()
    {
        Console.WriteLine("Resolving version to download...");

        var platform = GetPlatformString() ?? Platform;
        if (platform is null)
            throw new CliException("Unable to detect platform. Specify a platform using the --platform flag.");

        var release = await GetRelease(Version);
        if (release is null)
            throw new CliException($"Unable to resolve a version matching {Version}");

        if (DotNet.GetInstalledSdks()
                .Select(s => s.Version)
                .Contains(release.Sdk.Version, StringComparer.OrdinalIgnoreCase) &&
            !Prompt.GetYesNo(
                $"SDK version {release.Sdk.Version} is already installed on this machine. Download anyway?",
                false))
            throw new CliException("Download canceled");

        var file = release.Sdk.Files
            .FirstOrDefault(f => f.Rid.Equals(platform, StringComparison.OrdinalIgnoreCase));
        if (file?.Url is null)
            throw new CliException($"Unable to find a download for platform '{platform}'");

        var fileDownloadPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads",
            file.Name);

        using var client = new HttpClient();

        var downloadMessage = $"Downloading .NET Core SDK version {release.Sdk.Version} for {platform}";

        using (var fileStream = new FileStream(fileDownloadPath, FileMode.Create))
        using (var stream = await client.GetStreamAsync(file.Url))
        {
            var buffer = new byte[(long) (20 * Math.Pow(2, 10))];
            int bytesRead;
            var bytesWritten = 0;
            while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                bytesWritten += bytesRead;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"{downloadMessage}: {bytesWritten / Math.Pow(2, 20):N2} MiB");
            }

            Console.WriteLine();
        }

        if (!NoHashValidation)
        {
            Console.WriteLine("Validating hash...");
            using var hasher = SHA512.Create();
            using var fileStream = File.OpenRead(fileDownloadPath);
            var hash = hasher.ComputeHash(fileStream);
            var hashString = BitConverter.ToString(hash).Replace("-", "", StringComparison.Ordinal);
            if (!file.Hash.Equals(hashString, StringComparison.OrdinalIgnoreCase))
                throw new CliException(string.Join(
                    Environment.NewLine,
                    "Calculated hash did not match the one specified by Microsoft.",
                    $"Microsoft provided hash: {file.Hash}",
                    $"Locally computed hash:   {hashString}"));
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = fileDownloadPath,
            UseShellExecute = true,
        });
    }

    private static string? GetPlatformString()
    {
        var architecture = Environment.Is64BitOperatingSystem ? "x64" : "x32";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return $"win-{architecture}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return $"osx-{architecture}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return $"linux-{architecture}";
        return null;
    }

    private async Task<Release?> GetRelease(string version)
    {
        var channel = await GetReleaseChannel();
        if (channel is null)
            return null;

        return await FindRelease();

        async Task<ReleaseChannel?> GetReleaseChannel()
        {
            var channels = await DotNet.GetReleaseChannels();
            switch (version.ToUpperInvariant())
            {
                case "PREVIEW":
                    return channels.Find(c => c.SupportPhase == SupportPhase.Preview);
                case "GO-LIVE":
                    return channels.Find(c => c.SupportPhase == SupportPhase.GoLive);
                case "ACTIVE":
                    return channels.Find(c => c.SupportPhase == SupportPhase.Active);
                default:
                    var vPrefix = string.Join("", version.Take(3));
                    return channels.Find(c => c.ChannelVersion.StartsWith(vPrefix, StringComparison.OrdinalIgnoreCase));
            }
        }

        async Task<Release?> FindRelease()
        {
            await channel.UpdateReleases();
            var releases = channel.Releases;
            return version.ToUpperInvariant() switch
            {
                "PREVIEW" or "GO-LIVE" or "ACTIVE" => releases.FirstOrDefault(),
                _ => Runtime
                        ? releases
                            .FirstOrDefault(r => r.ReleaseVersion.StartsWith(version, StringComparison.OrdinalIgnoreCase))
                        : releases
                            .FirstOrDefault(r => r.Sdk.Version.StartsWith(version, StringComparison.OrdinalIgnoreCase)),
            };
        }
    }
}
