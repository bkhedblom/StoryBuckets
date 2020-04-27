using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using StoryBuckets.Shared;
using StoryBuckets.Integrations;
using StoryBuckets.DataStores;
using StoryBuckets.Services;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.Stories;
using Utils;
using StoryBuckets.Options;
using StoryBuckets.Integrations.CsvIntegration;

namespace StoryBuckets.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFilesystemIo, FilesystemIo>();
            services.AddTransient<IIntegrationPathProvider, AppDataPathProvider>();
            services.AddTransient<IStoragePathProvider, AppDataPathProvider>();
            services.AddTransient<IStorageFolderProvider, StorageFolderProvider>();
            services.AddTransient<IDataStore<Story>, InMemoryFileBackedStoryDataStore>();
            services.AddTransient<IFileReader, HantverkarprogrammetBacklogExportCsvReader>();
            services.AddTransient<IIntegration, FileIntegration>();
            services.AddTransient<IStoryService, StoryService>();
            services.AddTransient<IBucketService, BucketService>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
