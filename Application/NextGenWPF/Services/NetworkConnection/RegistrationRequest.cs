using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NextGenWPF.Services.NetworkConnection
{
    public class RegistrationRequest : JsonRequest
    {
        public RegistrationRequest(string username, string email, string password, string name) : base(@"/auth/register", HttpMethod.Post)
        {
            this.body = JsonConvert.SerializeObject(new LoginBody()
            {
                Email = email,
                Password = password,
                Username = username,
                Name = name,
            });
        }
        private class LoginBody
        {
            [JsonProperty("email")]
            public string Email { get; set; }
            [JsonProperty("password")]
            public string Password { get; set; }
            [JsonProperty("login")]
            public string Username { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }

        }
    }
}
