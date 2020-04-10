using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StoryBuckets.Client.Components.Counter;
using StoryBuckets.Client.Components.Bucket;

namespace StoryBuckets.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder.Services);

            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();
            

            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICounterViewModel, CounterViewModel>();
            services.AddScoped<IBucketViewModel, BucketViewModel>();
        }
    }
}
