using System.Threading.Tasks;
using DotNetSdkHelpers.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace DotNetSdkHelpers
{
    [Command(Description = "Manage .NET Core SDKs"),
    Subcommand("list", typeof(List))]
    class Program
    {
        static void Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        public Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.FromResult(0);
        }
    }
}