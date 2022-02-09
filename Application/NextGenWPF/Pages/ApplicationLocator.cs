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
        public MainPageViewModel MainPageViewModel { get; }
        public ApplicationLocator()
        {
#if DEBUG
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
            StartPageViewModel = serviceProvider.GetRequiredService<StartPageViewModel>();
            LoginPageViewModel = serviceProvider.GetRequiredService<LoginPageViewModel>();
            MainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            MainPageViewModel = serviceProvider.GetRequiredService<MainPageViewModel>();
            RegistrationPageViewModel = serviceProvider.GetRequiredService<RegistrationPageViewModel>();
#else
            var dbconnection = ConfigurationManager.ConnectionStrings["defaultDbConnection"].ConnectionString;
#endif
        }
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<StartPageViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISwitchNavigationService, SwitchNavigationService>();
            services.AddSingleton<IAutorizationService, DesignTime>();
            services.AddSingleton<IRegistrationService, DesignTime>();
            services.AddSingleton<ICurrentDeterminatorService,CurrentDeterminatorService>();
            services.AddSingleton<LoginPageViewModel>();
            services.AddSingleton<RegistrationPageViewModel>();
            services.AddSingleton<MainPageViewModel>();
        }
    }
}
