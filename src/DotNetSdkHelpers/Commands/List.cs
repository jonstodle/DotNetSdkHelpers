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
            var sdks = GetInstalledSdks()
                .Where(sdk => sdk.Version.StartsWith(Filter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var longestName = sdks.Max(sdk => sdk.Version.Length);
            
            Console.WriteLine(string.Join(
                Environment.NewLine,
                sdks
                    .Select(sdk => $"{sdk.Version.PadRight(longestName)} {sdk.Location}")));
            
            return Task.CompletedTask;
        }
    }
}