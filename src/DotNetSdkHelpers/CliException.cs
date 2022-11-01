namespace DotNetSdkHelpers;

public class CliException : Exception
{
    public int ExitCode { get; } = 1;

    public CliException(string message) : base(message)
    {
    }
}
