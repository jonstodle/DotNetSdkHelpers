namespace DotNetSdkHelpers.Models;

/// <summary>
/// The release type of the .NET version (standard/long term): <c>https://github.com/dotnet/core/blob/main/release-policies.md</c>
/// </summary>
public enum ReleaseType
{
    /// <summary>
    /// Standard term support (supported for 18 months, released in even-numbered years).
    /// </summary>
    Sts,

    /// <summary>
    /// Long term support (supported for three years, released in odd-numbered years).
    /// </summary>
    Lts,
}
