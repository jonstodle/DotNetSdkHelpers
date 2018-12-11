using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Switches to the specified .NET Core SDK version")]
    public class Set
    {
        [Argument(0, Description = "'latest' or a specific version")]
        public string Version { get; }

        public async Task<int> OnExecuteAsync()
        {
            if (Version.Equals("latest", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    File.Delete("global.json");
                }
                catch
                {
                    // ignored
                }

                if (new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.FullName is string parentDirectoryPath &&
                    Path.Combine(parentDirectoryPath, "global.json") is string parentGlobalJsonPath &&
                    File.Exists(parentGlobalJsonPath) &&
                    Prompt.GetYesNo(
                        "There's a global.json in your parent directory. Do you want to delete it? (N/y)",
                        false))
                {
                    try
                    {
                        File.Delete(parentGlobalJsonPath);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo("dotnet", "--version")
                {
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            process.WaitForExit();
            Console.WriteLine($".NET Core SDK version switched: {process.StandardOutput.ReadLine()}");

            return 0;
        }
    }
}