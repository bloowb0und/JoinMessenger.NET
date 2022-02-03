using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Core;
using BLL;
using BLL.Abstractions.Interfaces;
using BLL.Services;
using DAL.Abstractions.Interfaces;
using DAL.Contexts;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;

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

            // BLL Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            services.AddScoped<IServerService, ServerService>();

            // DAL Services
            services.AddSingleton<MessengerContext>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<App>();


            DependencyRegistrar.ConfigureServices(services);
        }
    }
}