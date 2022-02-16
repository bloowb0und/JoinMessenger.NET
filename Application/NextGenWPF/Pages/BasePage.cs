using NextGenWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NextGenWPF.Pages
{
    public class BasePage : Page
    {
        public BasePage()
        {
            this.Loaded += this.BasePageLoaded;
            this.Unloaded += this.BasePageUnloaded;
        }
        private void BasePageLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is IPageViewModel viewModel)
            {
                viewModel.OnPageLoaded();
            }
        }

        private void BasePageUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is IPageViewModel viewModel)
            {
                viewModel.OnPageUnloaded();
            }
        }
    }
}
