using GalaSoft.MvvmLight.Command;
using NextGenWPF.Services.Navigations;
using NextGenWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextGenWPF.ViewModels
{
    public class StartPageViewModel: BasePageViewModel
    {
        public StartPageViewModel(INavigationService navigationService)
        {
            this._navigationService = navigationService;
            this.MoveToLoginPage = new RelayCommand(this.MoveToLogin);
            this.MoveToRegistrationPage = new RelayCommand(this.MoveToRegistration);
        }
        public RelayCommand MoveToLoginPage { get; }

        public string CurrentPath { get; private set; }
        public RelayCommand MoveToRegistrationPage { get; }

        private readonly INavigationService _navigationService;

        protected override void OnPageLoaded()
        {
            base.OnPageLoaded();
        }
        private void MoveToLogin()
        {
            if (this.PageLoaded)
            {
                this.CurrentPath = "LoginPage.xaml";
                //this._navigationService.NavigateTo(PageKeys.LoginPage);
                this.OnPropertyChanged(nameof(CurrentPath));
            }
        }
        private void MoveToRegistration()
        {
            if (this.PageLoaded)
            {
                this.CurrentPath = "RegistrationPage.xaml";
                //this._navigationService.NavigateTo(PageKeys.RegistrationPage);
                this.OnPropertyChanged(nameof(CurrentPath));
            }
        }
    }
}
