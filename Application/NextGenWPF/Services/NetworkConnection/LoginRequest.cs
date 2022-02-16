using System.Net.Http;
using Newtonsoft.Json;

namespace NextGenWPF.Services.NetworkConnection
{
    public class LoginRequest : JsonRequest
    {
        public LoginRequest(string loginstring, string password) : base(@"/test/test", HttpMethod.Post)
        {
            // this.body = JsonConvert.SerializeObject(new LoginBody()
            // {
            //     LoginString = loginstring,
            //     Password = password
            //     
            // });
        }
        private class LoginBody
        {
            [JsonProperty ("login")]
            public string LoginString { get; set; }
            [JsonProperty ("password")]
            public string Password { get; set; }

        }
    }
}
