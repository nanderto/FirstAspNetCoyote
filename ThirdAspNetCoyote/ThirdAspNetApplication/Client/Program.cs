using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Coyote.Runtime;
using ThirdCoyoteLibrary;

namespace ThirdAspNetApplication.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("We are Here");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();

            builder.Services.AddSingleton<ThirdCoyoteLibrary.ICoyoteRuntime, CoyoteRuntime>();

            builder.Services.AddSingleton<Microsoft.Coyote.Runtime.ICoyoteRuntime, Microsoft.Coyote.RuntimeCoyote>();

            builder.Services.AddSingleton<IStartUp, ThirdCoyoteLibrary.StartUp>();

            await builder.Build().RunAsync();
        }
    }
}
