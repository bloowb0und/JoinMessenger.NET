using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NextGenWPF.Services.NetworkConnection
{
    public class RecoverRequest : JsonRequest
    {
        public RecoverRequest(string email) : base(@"/auth/forgot", HttpMethod.Post)
        {
            this.body = JsonConvert.SerializeObject(new LoginBody()
            {
                Email = email,
            });
        }
        private class LoginBody
        {
            [JsonProperty("email")]
            public string Email { get; set; }

        }
    }
}
