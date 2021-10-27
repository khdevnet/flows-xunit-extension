using FlowsXunit.FlowXunitExtensions;
using System.Threading.Tasks;
using Xunit;

namespace Requests
{
    public class When_http_request_flow : FlowSteps<When_http_request_flow.State>, IFlowFailNotRunnedSteps
    {
        private const string HttpResponseExpected = "http response";

        public When_http_request_flow(State state)
            : base(state)
        {
        }

        [Step(nameof(should_send_request_and_verify_in_test)), StepOrder(1)]
        public async Task should_send_request_and_verify_in_test()
        {
            var request = await new My_http_request_step("id").SendAsync();

            Assert.True(request.Response == HttpResponseExpected);
        }

        [Step, StepOrder(2)]
        public async Task should_send_request_and_verify_in_inside_request()
        {
            await new My_http_request_step("id").SendAndVerifyAsync();
        }

        public class State
        {
            public MyStack<string> Stack;
            public State()
            {
                Stack = new MyStack<string>();
            }

        }
    }
}
