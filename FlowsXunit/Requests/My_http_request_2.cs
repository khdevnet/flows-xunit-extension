using FlowsXunit.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Requests
{
    public class My_http_request_2
        : IHttpRequestStep<My_http_request_2, string>
    {
        private readonly string _id;
        public string Response { get; private set; }

        public My_http_request_2(string id)
        {
            _id = id;
        }

        public async Task<My_http_request_2> SendAndVerifyAsync()
        {
            // send http request and return response
            Response = await SendAsync();

            // verify
            Assert.NotNull(Response);

            return this;
        }

        public async Task<string> SendAsync()
        {
            // send http request and return response

            return "http response" + _id;
        }
    }
}
