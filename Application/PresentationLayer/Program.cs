using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Core;
using BLL;

namespace PresentationLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<App>()?.StartApp();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();
            
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            
            services.AddScoped<App>();

            DependencyRegistrar.ConfigureServices(services);
        }
    }
}