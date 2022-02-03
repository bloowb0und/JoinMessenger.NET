﻿using GalaSoft.MvvmLight.Command;
using Core.Models;
using NextGenWPF.Services;
using NextGenWPF.Services.Navigations;
using NextGenWPF.ViewModels.Base;
using System.Windows;

namespace NextGenWPF.ViewModels
{
    public class RegistrationPageViewModel : BasePageViewModel
    {
        public string NameLabelText => "Nickname";
        public string PasswordLabelText => "Password";
        public string EmailLabelText => "Email";

        private IRegistrationService _registration;

        private INavigationService _navigationService;
        public RegistrationPageViewModel(IRegistrationService registration, INavigationService navigationService)
        {
            _registration = registration;
            RegistrationCommand = new RelayCommand(Registration);
            _navigationService = navigationService;
            this.MoveToStartPage = new RelayCommand(this.MoveToStart);
            this.MoveToLoginPage = new RelayCommand(this.MoveToLogin);
        }

        public RelayCommand RegistrationCommand { get; }
        public RelayCommand MoveToLoginPage { get; }

        public RelayCommand MoveToStartPage { get; }

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
        private string email;
        public string Email
        {
            get => this.email;
            set
            {
                this.email = value;
                this.OnPropertyChanged();
            }
        }

        private async void Registration()
        {
            RegistrationCommand.RaiseCanExecuteChanged();
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username) || !IsValidEmail(email))
            {
                return;
            }
            var user = new User
            {
                Password = password,
                Login = username,
                Email = email
            };
            var result =await _registration.Registration(user);
            if (result)
            {
                MessageBox.Show("Welcome home, sweety)", "Registrate");
                this._navigationService.NavigateTo(PageKeys.StartPage);
                this.Email = string.Empty;
                this.Password = string.Empty;
                this.Username = string.Empty;   
            }
            else
            {
                var act = MessageBox.Show("Something went wrong?", "Try",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.Yes);
                if (act == MessageBoxResult.Yes)
                {
                    this.Email = string.Empty;
                    this.Password = string.Empty;
                    this.Username = string.Empty;
                }
                else
                {
                    this._navigationService.NavigateTo(PageKeys.StartPage);
                    this.Email = string.Empty;
                    this.Password = string.Empty;
                    this.Username = string.Empty;
                }
            }
        }
        private void MoveToStart()
        {
            if (this.PageLoaded)
            {
                this._navigationService.NavigateTo(PageKeys.StartPage);
            }
        }

        private void MoveToLogin()
        {
            if (this.PageLoaded)
            {
                this._navigationService.NavigateTo(PageKeys.LoginPage);
            }
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
