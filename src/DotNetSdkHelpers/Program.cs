using DotNetSdkHelpers.Commands;
using McMaster.Extensions.CommandLineUtils;
using static DotNetSdkHelpers.Helpers;

namespace DotNetSdkHelpers;

[Command("dotnet-sdk", Description = "Manage .NET Core SDKs"),
 Subcommand(typeof(Set)),
 Subcommand(typeof(List)),
 Subcommand(typeof(Download))]
class Program
{
    static void Main(string[] args)
    {
        using var app = new CommandLineApplication<Program>(
            PhysicalConsole.Singleton,
            Directory.GetCurrentDirectory());
        app.Conventions.UseDefaultConventions();
        app.UsePagerForHelpText = false;
        app.Execute(args);
    }

    [SuppressMessage("CA1822", "CA1822", Justification = "Convention for CommandLineUtils.")]
    public Task<int> OnExecuteAsync()
    {
        var output = CaptureOutput("dotnet", "--version");
        Console.WriteLine(output?.Trim() ?? "Unable to fetch current SDK version");
        return Task.FromResult(0);
    }
}
