using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NextGenWPF.Services.NetworkConnection
{
    public class NetworkRequest
    {
        public Uri URI;
        public HttpMethod method;
        public Dictionary<string, string> headers;
        public string body;
        public string mediaType;
        public NetworkRequest(Uri uri, HttpMethod method, string mediatype)
        {
            this.URI = uri;
            this.method = method;
            this.mediaType = mediatype;
        }
        protected void AddHeader(KeyValuePair<string, string> header)
        {
            this.headers.Add(header.Key, header.Value);
        }
    }
}
