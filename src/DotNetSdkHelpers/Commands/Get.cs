using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description =
        "Downloads the provided release version & platform. (\'\' or \'latest\' for the latest release.")]
    public class Get
    {
        [Argument(0, Description = "Version to download, or 'latest'")]
        public string Version { get; }

        [Option(CommandOptionType.SingleValue, Description =
            "The platform to download for. Defaults to the current platform on Windows and MacOS")]
        public string Platform { get; }

        [Option(CommandOptionType.NoValue, Description = "Whether to include preview versions.")]
        public bool IncludePreview { get; }

        public async Task<int> OnExecuteAsync()
        {
            var version = Version ?? "latest";
            var platform = GetPlatformString();

            if (platform is null)
            {
                Console.Error.WriteLine("Unable to detect platform. Specify a platform using the --platform flag.");
                return 1;
            }

            if (!((await Program.GetReleases())
                .Where(r => IncludePreview || !r.IsPreview)
                .FirstOrDefault(r => version.Equals("latest") ||
                                     r.SdkVersion.Equals(version, StringComparison.OrdinalIgnoreCase))
                is Release release))
            {
                Console.Error.WriteLine("Unable to identify release with specified version.");
                return 1;
            }

            var fileName = $"sdk-{platform}{GetPackageExtension(platform)}";
            var fileDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads", fileName);
            var client = new HttpClient();
            using (var fileStream = new FileStream(
                fileDownloadPath,
                FileMode.Create))
            {
                Console.WriteLine($"Downloading .NET Core SDK version {release.SdkVersion} for platform {platform}...");
                var stream = await client.GetStreamAsync(release[fileName]);
                stream.CopyTo(fileStream);
            }

            Process.Start(fileDownloadPath);

            return 0;
        }

        private string GetPlatformString()
        {
            var architecture = Environment.Is64BitOperatingSystem ? "x64" : "x32";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return $"win-{architecture}";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return $"mac-{architecture}";
            return null;
        }

        private string GetPackageExtension(string platform)
        {
            if (platform.Contains("win", StringComparison.OrdinalIgnoreCase))
                return ".exe";
            if (platform.Contains("mac", StringComparison.OrdinalIgnoreCase))
                return ".pkg";
            return "";
        }
    }
}