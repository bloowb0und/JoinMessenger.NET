using Microsoft.Extensions.DependencyInjection;
using NextGenWPF.Services;
using NextGenWPF.Services.Implementations;
using NextGenWPF.Services.Navigations;
using NextGenWPF.ViewModels;

namespace NextGenWPF.Pages
{
    public class ApplicationLocator
    {
        private ServiceProvider serviceProvider;
        public LoginPageViewModel LoginPageViewModel { get; }
        public RegistrationPageViewModel RegistrationPageViewModel { get; }
        public StartPageViewModel StartPageViewModel { get; }
        public MainWindowViewModel MainWindowViewModel { get; }
        public ApplicationLocator()
        {
#if DEBUG
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
            StartPageViewModel = serviceProvider.GetRequiredService<StartPageViewModel>();
            LoginPageViewModel = serviceProvider.GetRequiredService<LoginPageViewModel>();
            MainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            RegistrationPageViewModel = serviceProvider.GetRequiredService<RegistrationPageViewModel>();
#else
            var dbconnection = ConfigurationManager.ConnectionStrings["defaultDbConnection"].ConnectionString;
#endif
        }
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<StartPageViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IAutorizationService, DesignTime>();
            services.AddSingleton<IRegistrationService, DesignTime>();
            services.AddSingleton<LoginPageViewModel>();
            services.AddSingleton<RegistrationPageViewModel>();
        }
    }
}
