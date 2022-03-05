using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NextGenWPF.Services.NetworkConnection
{
    public class BaseRequest : JsonRequest
    {
        public BaseRequest(string uri, HttpMethod method, string token) : base(uri, method)
        {
            this.AddHeader(new KeyValuePair<string, string>("Autorization", token));
        }
    }
}
