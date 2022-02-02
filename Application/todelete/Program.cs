using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todelete
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
            services.AddScoped<IServerInvitationService, EmailNotificationService>();

            // DAL Services
            services.AddSingleton<MessengerContext>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<App>();


            DependencyRegistrar.ConfigureServices(services);
        }
    }
}
