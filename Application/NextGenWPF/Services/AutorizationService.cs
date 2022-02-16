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
        public async Task<User> Autorization(User user)
        {
            var req = new NetworkService();
            var result = await req.SendRequestAsync(new LoginRequest(user.Login, user.Password));
            if (result.ResponseCode == 200)
            {
                
                return JsonConvert.DeserializeObject<User>( result.ResponseBody);;
            }
            return null;
        }

         public async Task<bool> Registration(User user)
         {
             var req = new NetworkService();
             var result = await req.SendRequestAsync(new RegistrationRequest(user.Login, user.Email, user.Password,"Sweety"));
             if (result.ResponseCode == 200)
             {
                return true;
             }
             return false;
         }
        public async Task<bool> Recover(string mail)
        {
            var req = new NetworkService();
            var result = await req.SendRequestAsync(new RecoverRequest(mail));
            if (result.ResponseCode == 200)
            {
                return true;
            }
            return false;
        }
    }
}
