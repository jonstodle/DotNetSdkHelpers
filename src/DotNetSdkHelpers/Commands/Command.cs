namespace DotNetSdkHelpers.Commands;

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
            await Console.Error.WriteLineAsync(e.Message);
            Console.ResetColor();
            Environment.Exit(e.ExitCode);
        }
    }

    public abstract Task Run();
}
