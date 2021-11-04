using System.Threading.Tasks;

namespace FlowsXunit.Requests
{
    internal interface IHttpRequestStep<TRequest, TResponse>
    { 
        Task<TRequest> SendAndVerifyAsync();
        Task<TResponse> SendAsync();
        TResponse Response { get; }
    }
}
