using FlowsXunit.FlowXunitExtensions;
using System.Threading.Tasks;

namespace Requests
{
    [Scenario(Text = @"
When http request flow
* send first request
* send second request
* send third request
* send first request
* send second request
* send third request
* send first request
* send second request
* send third request
")]
    public partial class MyWhen_http_request_flow
    {
        public partial async Task Test1()
        {

        }
    }


    //public partial class MyWhen_http_request_flow
    //{
    //    [Step]
    //    public partial Task Test1();
    //}
}
