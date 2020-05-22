using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StoryBuckets.Client.Components.Counter;
using StoryBuckets.Client.Components.SortingBuckets;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;

namespace StoryBuckets.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            AddServiceInjection(builder.Services);
            AddModelInjection(builder.Services);
            AddViewModelInjection(builder.Services);
            
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();
            

            await builder.Build().RunAsync();
        }
        private static void AddServiceInjection(IServiceCollection services)
        {
            services.AddScoped<IHttpClient, StoryBucketsHttpClient>();
            services.AddScoped<IDataReader<SyncableStory>, StorySync>();
            services.AddScoped<IDataReader<IBucketModel>, BucketSync>();
            services.AddScoped<IDataCreator<IBucketModel>, BucketSync>();
        }

        private static void AddModelInjection(IServiceCollection services)
        {
            services.AddScoped<IStorylist, Storylist>();
        }

        private static void AddViewModelInjection(IServiceCollection services)
        {
            services.AddScoped<ICounterViewModel, CounterViewModel>();
            services.AddScoped<ISortingBucketsViewModel, SortingBucketsViewModel>();
        }

        
    }
}
