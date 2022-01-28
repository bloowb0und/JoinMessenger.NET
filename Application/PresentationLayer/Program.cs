using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Core;
using BLL;
using BLL.Abstractions;

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


            services.AddScoped<IServerService, ServerService>();
            services.AddScoped<IServerInvitationService, ServerInvitationService>();

            DependencyRegistrar.ConfigureServices(services);
        }
    }
}