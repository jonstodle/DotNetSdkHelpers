using System;
using System.Threading.Tasks;

namespace DotNetSdkHelpers.Commands
{
    public abstract class Command
    {
        public async Task OnExecuteAsync()
        {
            try
            {
                await Run();
            }
            catch (CliException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e.Message);
                Console.ResetColor();
                Environment.Exit(e.ExitCode);
            }
        }

        public abstract Task Run();
    }
}