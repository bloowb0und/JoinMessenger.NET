using GalaSoft.MvvmLight.Command;
using NextGenWPF.Services.Navigations;
using NextGenWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NextGenWPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Dictionary<PageKeys, string> _pageKeys = new Dictionary<PageKeys, string>
        {
            {PageKeys.RegistrationPage, "RegistrationPage.xaml"},
            {PageKeys.LoginPage, "LoginPage.xaml"},
            {PageKeys.StartPage, "StartPage.xaml" },
            {PageKeys.MainPage, "MainPage.xaml" },
        };

        private IDisposable currentPageKeyDisposable;

        private INavigationService _navigationService;

        public MainWindowViewModel(INavigationService navigationService)
        {
           this._navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
           this.currentPageKeyDisposable = this._navigationService.CurrentPageObservable.Subscribe((page) =>
           {
               if (this._pageKeys.ContainsKey(page))
               {
                   this.PathToView = this._pageKeys[page];
                   this.OnPropertyChanged(nameof(PathToView));
               }
           });
        }
        public string PathToView { get; private set; }

        #region dispose
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.currentPageKeyDisposable?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion dispose    
    }
}