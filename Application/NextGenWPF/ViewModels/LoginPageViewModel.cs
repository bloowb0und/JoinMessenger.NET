using GalaSoft.MvvmLight.Command;
using Core.Models;
using NextGenWPF.Services;
using NextGenWPF.Services.Navigations;
using NextGenWPF.ViewModels.Base;
using System.Windows;
using NextGenWPF.Pages;

namespace NextGenWPF.ViewModels
{
    public class LoginPageViewModel : BasePageViewModel
    {
        public string NameLabelText => "Nickname";
        public string PasswordLabelText => "Password";

        private IAutorizationService _autorization;

        private INavigationService _navigationService;
        private ICurrentDeterminatorService _currentDeterminatorService;
        private ISwitchNavigationService _switchNavigationService;
        public LoginPageViewModel(IAutorizationService autorization, INavigationService navigationService, ICurrentDeterminatorService currentDeterminatorService, ISwitchNavigationService switchNavigationService)
        {
            _autorization = autorization;
            LoginCommand = new RelayCommand(this.Login);
            RecoverCommand = new RelayCommand(this.RecoverPassword);
            MoveToRegistrationPage = new RelayCommand(this.MoveToRegistration);
            _navigationService = navigationService;
            _currentDeterminatorService = currentDeterminatorService;
            _switchNavigationService = switchNavigationService;
        }
        public string CurrentPath { get; set; }
        public RelayCommand MoveToRegistrationPage { get; }
        public RelayCommand LoginCommand { get; }
        public RelayCommand RecoverCommand { get; }

        private string username;
        public string Username
        {
            get => this.username;
            set
            {
                this.username = value;
                this.OnPropertyChanged();
            }
        }
        private string password;
        public string Password
        {
            get => this.password;
            set
            {
                this.password = value;
                this.OnPropertyChanged();
            }
        }
        private void MoveToRegistration()
        {
            if (this.PageLoaded)
            {
                this.CurrentPath = "RegistrationPage.xaml";
                this.OnPropertyChanged(nameof(CurrentPath));
            }
        }
        protected override void OnPageLoaded()
        {
            base.OnPageLoaded();
        }
        private async void Login()
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
            {
                return;
            }
            var user = new User
            {
                Password = password,
                Login = username
            };
            var result = await _autorization.Autorization(user);
            if (result!=null)
            {
                _currentDeterminatorService.SetCurrentUser(result);
                MessageBox.Show("Come home, sweety)", "Login");
                this._navigationService.NavigateTo(PageKeys.MainPage);
                this.Password = string.Empty;
                this.Username = string.Empty;
            }
            else
            {
                var act = MessageBox.Show("We can't find such sweety like u( Do u want to become our member?", "Login",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.Yes);
                if (act == MessageBoxResult.Yes)
                {
                    this.Password = string.Empty;
                    this.Username = string.Empty;
                    this.MoveToRegistrationPage.Execute(this);
                    this._switchNavigationService.NavigateTo(PageKeys.RegistrationPage);
                }
                else
                {
                    this._navigationService.NavigateTo(PageKeys.StartPage);
                    this.Password = string.Empty;
                    this.Username = string.Empty;
                }
            }
        }
        private async void RecoverPassword()
        {
            var result =  await _autorization.Recover(this.Username);
            if (result)
            {
                MessageBox.Show("Try again, sweety)", "Recover");
                this._switchNavigationService.NavigateTo(PageKeys.LoginPage);
                this.Password = string.Empty;
                this.Username = string.Empty;
            }
            else
            {
                var act = MessageBox.Show("We can't find such sweety like u( Do u want to become our member?", "Login",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.Yes);
                if (act == MessageBoxResult.Yes)
                {
                    this.MoveToRegistrationPage.Execute(this);
                    this._switchNavigationService.NavigateTo(PageKeys.RegistrationPage);
                    this.Password = string.Empty;
                    this.Username = string.Empty;
                }
                else
                {
                    this._navigationService.NavigateTo(PageKeys.StartPage);
                    this.Password = string.Empty;
                    this.Username = string.Empty;
                }
            }
        }

    }
}
