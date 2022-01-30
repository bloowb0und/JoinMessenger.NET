using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NextGenWPF.Services.NetworkConnection
{
    public class NetworkRequest
    {
        const string URL = "https://localhost:5001/api";
        public Uri URI = new Uri(URL);
        public HttpMethod method;
        public Dictionary<string, string> headers = new Dictionary<string, string>();
        public string body;
        public string mediaType;
        public NetworkRequest(Uri uri, HttpMethod method, string mediatype)
        {
            var s = URL + uri.OriginalString;
            this.URI = new Uri(s);
            this.method = method;
            this.mediaType = mediatype;
        }
        protected void AddHeader(KeyValuePair<string, string> header)
        {
            this.headers.Add(header.Key, header.Value);
        }
    }
}
