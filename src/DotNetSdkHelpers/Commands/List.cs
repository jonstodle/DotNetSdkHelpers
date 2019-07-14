using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Lists all installed .NET Core SDKs")]
    public class List : Command
    {
        public override Task Run()
        {
            var output = CaptureOutput("dotnet", "--list-sdks");
            Console.WriteLine(output.Trim());
            return Task.CompletedTask;
        }
    }
}