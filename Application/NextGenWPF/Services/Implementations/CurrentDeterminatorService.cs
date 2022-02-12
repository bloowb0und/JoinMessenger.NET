using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NextGenWPF.Services.Implementations
{
    public class CurrentDeterminatorService : ICurrentDeterminatorService
    {
        private BehaviorSubject<User> _behaviorUser = new BehaviorSubject<User>(new User());
        public IObservable<User> userSubject => this._behaviorUser.AsObservable();
        
        public void SetCurrentUser(User user)
        {
            _behaviorUser.OnNext(user);            
        }
    }
}
