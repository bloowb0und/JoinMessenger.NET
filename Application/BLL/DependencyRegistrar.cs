using BLL.Abstractions.Interfaces;
using BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public class DependencyRegistrar
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            DAL.DependencyRegistrar.ConfigureServices(services);
        }
    }
}