using System;
using System.Collections.Generic;
using System.Text;

namespace NextGenWPF.Services.Navigations
{
    public interface ISwitchNavigationService
    {
        IObservable<PageKeys> CurrentPageObservable { get; }
        void NavigateTo(PageKeys page);
    }
}
