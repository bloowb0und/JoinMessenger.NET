using GalaSoft.MvvmLight.Command;
using Core.Models;
using NextGenWPF.Services;
using NextGenWPF.Services.Navigations;
using NextGenWPF.ViewModels.Base;
using System.Windows;

namespace NextGenWPF.ViewModels
{
    public class LoginPageViewModel : BasePageViewModel
    {
        public string NameLabelText => "Nickname";
        public string PasswordLabelText => "Password";

        private IAutorizationService _autorization;

        private INavigationService _navigationService;
        public LoginPageViewModel(IAutorizationService autorization, INavigationService navigationService)
        {
            _autorization = autorization;
            LoginCommand = new RelayCommand(this.Login);
            _navigationService = navigationService;
        }
        public RelayCommand MoveToStartPage { get; }

        public RelayCommand MoveToRegistrationPage { get; }
        public RelayCommand LoginCommand { get; }

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

        private void Login()
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
            var result = _autorization.Autorization(user).Result;
            if (result)
            {
                MessageBox.Show("Come home, sweety)", "Login");
                this._navigationService.NavigateTo(PageKeys.StartPage);
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
