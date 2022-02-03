using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextGenWPF.Services
{
    public interface ICurrentDeterminatorService
    {
        public User GetCurrentUser();
        public void SetCurrentUser(User user);
    }
}
