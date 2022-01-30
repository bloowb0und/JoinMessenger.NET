using NextGenWPF.Services.NetworkConnection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NextGenWPF.Services.Navigations
{
    public class NetworkService
    {
        private HttpClient client = null;

        public NetworkService()
        {
            this.client = new HttpClient();
            this.client.BaseAddress =new Uri("https://localhost:5001/api");
        }
        public Task<NetworkResponse> SendRequestAsync(NetworkRequest request)
        {
            return Task.Run(async () => await this.SendRequest(request));
        }

        private async Task<NetworkResponse> SendRequest(NetworkRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (var httpRequest = new HttpRequestMessage()
            {
                Method = request.method,
                Content = request.body == null ? null : new StringContent(request.body, Encoding.UTF8, request.mediaType),
                RequestUri = request.URI
            })
            {
                foreach (var item in request.headers)
                {
                    httpRequest.Headers.Add(item.Key, item.Value);
                }
                return await GetResponse(httpRequest);
            }
        }

        private async Task<NetworkResponse> GetResponse(HttpRequestMessage message)
        {
            NetworkResponse result = null;

            using (CancellationTokenSource source = new CancellationTokenSource(100000))
            {
                try
                {
                    var response = await this.client.SendAsync(message, source.Token);

                    result = new NetworkResponse()
                    {
                        ResponseCode = ((int)response.StatusCode),
                        ResponseBody = await response.Content.ReadAsStringAsync()
                    };
                }
                catch (Exception ex)
                {
                    result = new NetworkResponse()
                    {
                        ResponseCode = -1
                    };
                }
            }

            return result;
        }

        #region IDisposable
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    //Managed resources
                    this.client?.Dispose();
                }

                //Unmanaged resources

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
