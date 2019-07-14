using System;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command("list", "ls", Description = "Lists all installed .NET Core SDKs")]
    public class List : Command
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        [Argument(0, Description = "Filters list to only SDKs starting with the provided string.")]
        public string Filter { get; } = "";
        // ReSharper restore UnassignedGetOnlyAutoProperty
        public override Task Run()
        {
            var output = CaptureOutput("dotnet", "--list-sdks").Trim();
            
            var sdks = output.Split(Environment.NewLine)
                .Where(sdk => sdk.StartsWith(Filter, StringComparison.OrdinalIgnoreCase))
                .Select(sdk =>
                    {
                        var parts = sdk.Split(' ');
                        return (version: parts[0], location: parts[1]);
                    })
                .ToList();

            var longestName = sdks.Max(sdk => sdk.version.Length);
            
            Console.WriteLine(string.Join(
                Environment.NewLine,
                sdks
                    .Select(sdk => $"{sdk.version.PadRight(longestName)} {sdk.location}")));
            
            return Task.CompletedTask;
        }
    }
}