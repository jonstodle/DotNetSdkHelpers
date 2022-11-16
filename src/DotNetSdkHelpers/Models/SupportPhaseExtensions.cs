using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DotNetSdkHelpers.Models;

public static class SupportPhaseExtensions
{
    public static string Display(this SupportPhase phase)
    {
        return typeof(SupportPhase).GetMember(phase.ToString()).First().GetCustomAttribute<DisplayAttribute>()!.Name!;
    }
}
