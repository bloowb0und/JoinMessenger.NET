using System;
using System.Collections.Generic;
using System.Text;

namespace NextGenWPF.Services.NetworkConnection
{
    public class NetworkResponse
    {
        public NetworkResponse()
        {
        }

        public int ResponseCode { get; set; }
        public string ResponseBody { get; set; }
    }
}
