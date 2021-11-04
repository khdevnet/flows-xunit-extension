using Xunit.Scenario;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Scenario.Examples.Collections;

namespace Requests
{
    [Collection(nameof(BeverlyHillsShop))]
    public class When_http_request_flow : ScenarioSteps<When_http_request_flow.State>, IScenarioFailNotRunnedSteps
    {
        private const string HttpResponseExpected = "http response";
        public BeverlyHillsShop Shop { get; }

        public When_http_request_flow(State state, BeverlyHillsShop shop)
            : base(state)
        {
            Shop = shop;
            Shop.ShopId += nameof(When_http_request_flow);
        }

        [Step("read file")]
        public async Task read_file()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
        }

        [Step(nameof(should_send_request_and_verify_in_test)), StepOrder(1)]
        public async Task should_send_request_and_verify_in_test()
        {
            var response = await new My_http_request("id").SendAsync();

            Assert.True(response == HttpResponseExpected);
        }

        [Step, StepOrder(2)]
        public async Task should_send_request_and_verify_in_inside_request()
        {
            await new My_http_request("id").SendAndVerifyAsync();
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

    [Collection(nameof(BeverlyHillsShop))]
    public class When_http_request_share_state_flow : ScenarioSteps<When_http_request_share_state_flow.State>, IScenarioFailNotRunnedSteps
    {
        private const string HttpResponseExpected = "http response";

        public When_http_request_share_state_flow(State state, BeverlyHillsShop shop)
            : base(state)
        {
            Shop = shop;
            Shop.ShopId += nameof(When_http_request_share_state_flow);
        }

        public BeverlyHillsShop Shop { get; }

        [Step(nameof(should_send_request_and_verify_in_test)), StepOrder(1)]
        public async Task should_send_request_and_verify_in_test()
        {
            var response = await new My_http_request("id").SendAsync();
            Assert.True(response == HttpResponseExpected);

            state.FirstRequestResponse = response;
        }

        [Step, StepOrder(2)]
        public async Task should_send_request_using_first_request_response()
        {
            var response = await new My_http_request_2(this.state.FirstRequestResponse).SendAsync();
            Assert.True(response == HttpResponseExpected + HttpResponseExpected);
        }

        public class State
        {
            public MyStack<string> Stack;
            public State()
            {
                Stack = new MyStack<string>();
            }

            public string FirstRequestResponse { get; internal set; }
        }
    }
}
