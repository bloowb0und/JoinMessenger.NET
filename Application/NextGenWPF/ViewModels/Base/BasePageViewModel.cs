namespace NextGenWPF.ViewModels.Base
{
    public class BasePageViewModel : ViewModelBase, IPageViewModel
    {
        protected bool PageLoaded { get; private set; }

        void IPageViewModel.OnPageLoaded()
        {
            this.PageLoaded = true;
            this.OnPageLoaded();
        }

        protected virtual void OnPageLoaded()
        {
        }

        void IPageViewModel.OnPageUnloaded()
        {
            this.PageLoaded = false;
            this.OnPageUnloaded();
        }

        protected virtual void OnPageUnloaded()
        {
        }
    }
}
