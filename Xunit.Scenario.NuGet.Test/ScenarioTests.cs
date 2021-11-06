using Xunit.Scenario.Extension;
using System.Threading.Tasks;

namespace ScenarioAutoGeneration
{
    [Scenario(@"
* send first request
* send second request
* send third request
* send fourth request
")]
    public partial class RequestFlowAutoGeneratedScenario : ScenarioSteps<RequestFlowAutoGeneratedScenario.State>
    {
        public RequestFlowAutoGeneratedScenario(State state) : base(state)
        {

        }

        public partial Task S1_send_first_request()
        {
            return Task.CompletedTask;
        }

        public partial Task S2_send_second_request() { return Task.CompletedTask; }

        public partial Task S3_send_third_request() { return Task.CompletedTask; }

        public partial Task S4_send_fourth_request() { return Task.CompletedTask; }

        public class State { }
    }
}