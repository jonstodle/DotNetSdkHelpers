using System;
using System.Threading.Tasks;
using DotNetSdkHelpers.Commands;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers
{
    [Command("dotnet-sdk", Description = "Manage .NET Core SDKs"),
     Subcommand("set", typeof(Set)),
     Subcommand("list", typeof(List)),
     Subcommand("releases", typeof(Releases)),
     Subcommand("get", typeof(Get))]
    class Program
    {
        static void Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        public Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var output = CaptureOutput("dotnet", "--version");
            Console.WriteLine(output?.Trim() ?? "Unable to fetch current SDK version");
            return Task.FromResult(0);
        }
    }
}