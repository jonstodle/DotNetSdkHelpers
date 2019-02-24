using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetSdkHelpers.Commands;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

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

        public static async Task<List<Release>> GetReleases()
        {
            var client = new HttpClient();
            return JsonConvert.DeserializeObject<List<Release>>(
                await client
                    .GetStringAsync(
                        "https://raw.githubusercontent.com/dotnet/core/master/release-notes/releases.json"));
        }

        public Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var output = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardOutput = true,
            })?
                .StandardOutput.ReadToEnd();
            Console.WriteLine(output?.Trim() ?? "Unable to fetch current SDK version");
            return Task.FromResult(0);
        }
    }
}