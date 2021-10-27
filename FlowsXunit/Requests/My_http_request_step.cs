using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Requests
{
    public class My_http_request_step
        : IStepVerify<My_http_request_step>
    {
        private readonly string _id;
        public string Response { get; private set; }

        public My_http_request_step(string id)
        {
            _id = id;
        }

        public async Task<My_http_request_step> SendAndVerifyAsync()
        {
            // send http request and return response
            await SendAsync();

            Assert.NotNull(Response);

            return this;
        }

        public async Task<My_http_request_step> SendAsync()
        {
            // send http request and return response

            Response = "http response";

            return this;
        }
    }

    internal interface IStepVerify<TStep>
    {
        Task<TStep> SendAndVerifyAsync();
        Task<TStep> SendAsync();
    }
}
