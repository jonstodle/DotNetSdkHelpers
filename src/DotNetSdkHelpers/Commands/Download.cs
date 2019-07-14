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
        [Argument(0, Description = "Version to download, or 'latest' (or empty)")]
        public string Version { get; }

        [Option(CommandOptionType.SingleValue, Description =
            "The platform to download for. Defaults to the current platform on Windows and MacOS")]
        public string Platform { get; }

        [Option(CommandOptionType.NoValue, Description = "Whether to include preview versions.")]
        public bool IncludePreview { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty

        public override async Task Run()
        {
            Console.WriteLine("Resolving version to download...");
            
            var version = Version ?? "latest";
            var platform = GetPlatformString();

            if (platform is null)
                throw new CliException("Unable to detect platform. Specify a platform using the --platform flag.");

//            if (!((await GetReleases())
//                .Where(r => IncludePreview || !r.IsPreview)
//                .OrderByDescending(r => r.SdkVersion)
//                .FirstOrDefault(r => version.Equals("latest") ||
//                                     r.SdkVersion.StartsWith(version, StringComparison.OrdinalIgnoreCase))
//                is Release release))
//                throw new CliException("Unable to identify a release with specified version.");
            var release = (await GetReleases()).First();

//            var downloadUrl = release[$"sdk-{platform}{GetPackageExtension(platform)}"];
            var downloadUrl = release.Product;
            var fileDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads", downloadUrl.Split('/').Last());
            var client = new HttpClient();

            var downloadMessage = $"Downloading .NET Core SDK version {release.ChannelVersion} for {platform}";
            
            using (var fileStream = new FileStream(fileDownloadPath, FileMode.Create))
            using (var stream = await client.GetStreamAsync(downloadUrl))
            {
                var buffer = new byte[(long) Math.Pow(2, 20)];
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