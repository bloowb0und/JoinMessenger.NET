using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NextGenWPF.Services.NetworkConnection
{
    public class JsonRequest : NetworkRequest
    {
        public JsonRequest(string uri, HttpMethod method) : base(new Uri(uri, UriKind.Relative), method,@"application/json")
        {
            this.headers.Add("ContentType", @"application/json");
        }
    }
}
