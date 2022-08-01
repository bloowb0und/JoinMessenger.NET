using NextGenWPF.Services.Navigations;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace NextGenWPF.Services.Implementations
{
    public class SwitchNavigationService : ISwitchNavigationService
    {
        private const PageKeys DEFAULT_PAGE_KEY = PageKeys.LoginPage;

        private readonly BehaviorSubject<PageKeys> _behaviorSubject = new BehaviorSubject<PageKeys>(DEFAULT_PAGE_KEY);

        public IObservable<PageKeys> CurrentPageObservable => this._behaviorSubject.AsObservable();

        public void NavigateTo(PageKeys page)
        {
            this._behaviorSubject.OnNext(page);
        }
    }
}
