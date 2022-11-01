using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DotNetSdkHelpers.Models;

/// <summary>
/// Support phase of the release: <c>https://github.com/dotnet/core/blob/main/release-policies.md</c>
/// </summary>
public enum SupportPhase
{
    /// <summary>
    /// Not supported, offered to the community to test and give feedback.
    /// </summary>
    [EnumMember(Value = "preview")]
    [Display(Name = "Preview")]
    Preview,

    /// <summary>
    /// Supported by Microsoft in production. These are release candidate builds.
    /// </summary>
    [EnumMember(Value = "go-live")]
    [Display(Name = "Go-Live")]
    GoLive,

    /// <summary>
    /// Generally available, actively supported.
    /// </summary>
    [EnumMember(Value = "active")]
    [Display(Name = "Active")]
    Active,

    /// <summary>
    /// Last six months of support, improvements limited to security fixes.
    /// </summary>
    [EnumMember(Value = "maintenance")]
    [Display(Name = "Maintenance")]
    Maintenance,

    /// <summary>
    /// End of support.
    /// </summary>
    [EnumMember(Value = "eol")]
    [Display(Name = "EOL")]
    Eol,
}
