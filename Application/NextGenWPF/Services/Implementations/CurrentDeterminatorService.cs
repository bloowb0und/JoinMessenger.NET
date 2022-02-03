using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextGenWPF.Services.Implementations
{
    public class CurrentDeterminatorService : ICurrentDeterminatorService
    {
        private User CurrentUser { get; set; }
        public User GetCurrentUser()
        {
            return CurrentUser;
        }

        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }
    }
}
