using Newtonsoft.Json;
using Core.Models;
using NextGenWPF.Services.Navigations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using NextGenWPF.Services.NetworkConnection;

namespace NextGenWPF.Services
{
    public class DesignTime : IAutorizationService, IRegistrationService
    {
        public async Task<bool> Autorization(User user)
        {
            var req = new NetworkService();
            var result = await req.SendRequestAsync(new LoginRequest(user.Login, user.Password));
            if (result.ResponseCode == 200)
            {
                return true;
            }
            return false;
        }

        public bool Registration(User user)
        {
            throw new NotImplementedException();
        }
    }
}
