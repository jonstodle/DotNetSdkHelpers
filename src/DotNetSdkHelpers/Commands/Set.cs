using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Switches to the specified .NET Core SDK version")]
    public class Set : Command
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        [Argument(0, Description = "'stable', 'preview' or a specific version")]
        [Required]
        public string Version { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty

        public override Task Run()
        {
            if (Version.Equals("preview", StringComparison.OrdinalIgnoreCase))
            {
                var selectedSdk = GetInstalledSdks()
                    .LastOrDefault(sdk => sdk.Version.Contains("preview", StringComparison.OrdinalIgnoreCase));
                if (selectedSdk.IsDefault)
                    throw new CliException(string.Join(
                        Environment.NewLine,
                        $"A preview version of .Net Core SDK was not found",
                        "Run \"dotnet sdk list\" to make sure you have one installed"));
                
                File.WriteAllText(
                    "global.json",
                    JsonConvert.SerializeObject(new
                    {
                        sdk = new
                        {
                            version = selectedSdk.Version
                        }
                    }));
            }
            else if (Version.Equals("stable", StringComparison.OrdinalIgnoreCase))
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
                        "There's a global.json in your parent directory. Do you want to delete it?",
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
            else
            {
                var sdks = GetInstalledSdks();
                var selectedSdk = sdks
                        .LastOrDefault(sdk => sdk.Version.StartsWith(Version, StringComparison.OrdinalIgnoreCase));
                if (selectedSdk.IsDefault)
                    throw new CliException(string.Join(
                        Environment.NewLine,
                        $"The {Version} version of .Net Core SDK was not found",
                        "Run \"dotnet sdk list\" to make sure you have it installed"));

                File.WriteAllText(
                    "global.json",
                    JsonConvert.SerializeObject(new
                    {
                        sdk = new
                        {
                            version = selectedSdk.Version
                        }
                    }));
            }

            var output = CaptureOutput("dotnet", "--version");
            Console.WriteLine($".NET Core SDK version switched: {output.Trim()}");

            return Task.CompletedTask;
        }
    }
}