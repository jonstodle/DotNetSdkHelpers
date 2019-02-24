using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Switches to the specified .NET Core SDK version")]
    public class Set
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        [Argument(0, Description = "'latest' or a specific version")]
        [Required]
        public string Version { get; }
        // ReSharper restore UnassignedGetOnlyAutoProperty

        public int OnExecute()
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
                var sdkOutput = CaptureOutput("dotnet", "--list-sdks");
                if (!(sdkOutput
                        .Split(Environment.NewLine)
                        .Select(line => line.Split(' ').First())
                        .FirstOrDefault(v => v.Equals(Version, StringComparison.OrdinalIgnoreCase))
                    is string selectedVersion))
                {
                    Console.WriteLine($"The {Version} version of .Net Core SDK was not found");
                    Console.WriteLine("Run \"dotnet sdk list\" to make sure you have it installed");
                    return 1;
                }

                File.WriteAllText(
                    "global.json",
                    JsonConvert.SerializeObject(new
                    {
                        sdk = new
                        {
                            version = selectedVersion
                        }
                    }));
            }

            var output = CaptureOutput("dotnet", "--version");
            Console.WriteLine($".NET Core SDK version switched: {output.Trim()}");

            return 0;
        }
    }
}