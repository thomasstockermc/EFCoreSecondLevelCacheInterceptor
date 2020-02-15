using EFCoreSecondLevelCacheInterceptor.Tests.DataLayer.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EFCoreSecondLevelCacheInterceptor.Tests.DataLayer;

namespace EFCoreSecondLevelCacheInterceptor.AspNetCoreSample
{
    public class Startup
    {
        private readonly string _contentRootPath;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _contentRootPath = env.ContentRootPath;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider()

            // Installing Redis on Windows: http://taswar.zeytinsoft.com/intro-to-redis-for-net-developers/
            // Install the Redis binaries in the default NuGet tools directory: https://www.nuget.org/packages/Redis-64/
            // Different ways to configure Redis: https://stackexchange.github.io/StackExchange.Redis/Configuration#configuration-options
            // Redis Desktop Manager: https://github.com/uglide/RedisDesktopManager
            // options.UseRedisCacheProvider(Configuration["RedisConfiguration"])
            );

            var connectionString = Configuration["ConnectionStrings:ApplicationDbContextConnection"];
            if (connectionString.Contains("%CONTENTROOTPATH%"))
            {
                connectionString = connectionString.Replace("%CONTENTROOTPATH%", _contentRootPath);
            }
            services.AddConfiguredMsSqlDbContext(connectionString);

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IServiceScopeFactory scopeFactory)
        {
            scopeFactory.Initialize();
            scopeFactory.SeedData();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}