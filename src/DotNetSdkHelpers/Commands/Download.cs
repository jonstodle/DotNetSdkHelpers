using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description =
        "Downloads the provided release version & platform."),
    Subcommand(typeof(DownloadList))]
    public class Download : Command
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        [Argument(0, Description = "'lts', 'current', 'preview' or a specific version. (Default: 'current')")]
        public string Version { get; } = "current";

        [Option(CommandOptionType.SingleValue, Description =
            "The platform to download for. Defaults to the current platform on Windows and MacOS")]
        public string Platform { get; }
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

            if (GetInstalledSdks()
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
                // ReSharper disable once PossibleNullReferenceException
                file.Name);
            
            var client = new HttpClient();

            var downloadMessage = $"Downloading .NET Core SDK version {release.Sdk.Version} for {platform}";
            
            using (var fileStream = new FileStream(fileDownloadPath, FileMode.Create))
            using (var stream = await client.GetStreamAsync(file.Url))
            {
                var buffer = new byte[(long)(20 * Math.Pow(2, 10))];
                int bytesRead;
                var bytesWritten = 0;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    bytesWritten += bytesRead;

                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"{downloadMessage}: {bytesWritten / Math.Pow(2, 20):N2} MiB");
                }

                Console.WriteLine();
            }
            
            // TODO: Validate hash

            Process.Start(new ProcessStartInfo
            {
                FileName = fileDownloadPath,
                UseShellExecute = true,
            });
        }

        private string GetPlatformString()
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

        private async Task<Release> GetRelease(string version)
        {
            var isPreview = version.Equals("preview", StringComparison.OrdinalIgnoreCase);
            var isCurrent = version.Equals("current", StringComparison.OrdinalIgnoreCase);
            var isLts = version.Equals("lts", StringComparison.OrdinalIgnoreCase);
            
            var channel = await GetReleaseChannel();
            if (channel is null)
                return null;

            return await FindRelease();


            async Task<ReleaseChannel> GetReleaseChannel()
            {
                var channels = await GetReleaseChannels();
                if (isPreview)
                    return channels.Find(c => c.SupportPhase == SupportPhases.Preview);
                if (isCurrent)
                    return channels.Find(c => c.SupportPhase == SupportPhases.Current);
                if (isLts)
                    return channels.Find(c => c.SupportPhase == SupportPhases.Lts);
                
                var vPrefix = string.Join("", version.Take(3));
                return channels.Find(c => c.ChannelVersion.StartsWith(vPrefix, StringComparison.OrdinalIgnoreCase));
            }

            async Task<Release> FindRelease()
            {
                var releases = await GetReleases(channel.ReleasesJson);
                if (isPreview || isCurrent || isLts)
                    return releases.FirstOrDefault();
                
                return releases
                    .FirstOrDefault(r => r.Sdk.Version.StartsWith(version, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}