using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Web_DongHo_WebAssembly.Models;

namespace Web_DongHo_WebAssembly
{
    public class Program
    {
        private static async Task DebugDelayAsync()
        {
            await Task.Delay(5000);
        }
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Web_DongHo_WebAssembly.App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44355/") });
            builder.Services.AddSingleton<AuthState>();

            await DebugDelayAsync();
            await builder.Build().RunAsync();
        }
    }
}
