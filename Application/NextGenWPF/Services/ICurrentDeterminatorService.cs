using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextGenWPF.Services
{
    public interface ICurrentDeterminatorService
    {
        IObservable<User> userSubject { get; }
        public void SetCurrentUser(User user);
    }
}
