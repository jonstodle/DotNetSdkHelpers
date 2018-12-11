using System.Diagnostics;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace DotNetSdkHelpers.Commands
{
    [Command(Description = "Lists all installed .NET Core SDKs")]
    public class List
    {
        public Task<int> OnExecuteAsync()
        {
            Process.Start("dotnet", "--list-sdks");
            return Task.FromResult(2);
        }
    }
}