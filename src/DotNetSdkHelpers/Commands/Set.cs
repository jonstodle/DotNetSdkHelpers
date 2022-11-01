using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Creates a global.json to switch to the specified .NET Core SDK version")]
    [SuppressMessage("CA1716", "CA1716", Justification = "Convention for CommandLineUtils.")]
    public class Set : Command
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        [Argument(0, Description = "'stable', 'preview' or a specific version")]
        [Required]
        public string Version { get; } = null!;
        // ReSharper restore UnassignedGetOnlyAutoProperty

        public override Task Run()
        {
            if (Version.Equals("preview", StringComparison.OrdinalIgnoreCase))
            {
                var selectedSdk = GetInstalledSdks()
                    .LastOrDefault(sdk => sdk.Version.Contains("preview", StringComparison.OrdinalIgnoreCase));
                if (selectedSdk == null || selectedSdk.IsDefault)
                    throw new CliException(string.Join(
                        Environment.NewLine,
                        $"A preview version of .NET Core SDK was not found.",
                        "Run \"dotnet sdk list\" to make sure you have one installed."));

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
                DeleteFile("global.json");
                if (new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.FullName is string parentDirectoryPath &&
                    Path.Combine(parentDirectoryPath, "global.json") is string parentGlobalJsonPath &&
                    File.Exists(parentGlobalJsonPath) &&
                    Prompt.GetYesNo(
                        "There's a global.json in your parent directory. Do you want to delete it?",
                        false))
                {
                    DeleteFile(parentGlobalJsonPath);
                }
            }
            else
            {
                var sdks = GetInstalledSdks();
                var selectedSdk = sdks
                        .LastOrDefault(sdk => sdk.Version.StartsWith(Version, StringComparison.OrdinalIgnoreCase));
                if (selectedSdk == null || selectedSdk.IsDefault)
                    throw new CliException(string.Join(
                        Environment.NewLine,
                        $"The {Version} version of .NET Core SDK was not found.",
                        "Run \"dotnet sdk list\" to make sure you have it installed."));

                File.WriteAllText(
                    "global.json",
                    JsonConvert.SerializeObject(new
                    {
                        sdk = new
                        {
                            version = selectedSdk.Version
                        }
                    }, Formatting.Indented));
            }

            var output = CaptureOutput("dotnet", "--version");
            Console.WriteLine($".NET Core SDK version switched: {output.Trim()}");

            return Task.CompletedTask;
        }

        [SuppressMessage("CA1031", "CA1031", Justification = "We don't care a lot if we can't delete the file for whatever reason.")]
        private static void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                // ignored
            }
        }
    }
}
