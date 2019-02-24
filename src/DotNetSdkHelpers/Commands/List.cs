using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Lists all installed .NET Core SDKs")]
    public class List
    {
        public Task<int> OnExecuteAsync()
        {
            var output = CaptureOutput("dotnet", "--list-sdks");
            Console.WriteLine(output.Trim());
            return Task.FromResult(0);
        }
    }
}