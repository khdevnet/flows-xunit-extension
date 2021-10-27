using System.Threading.Tasks;
using FlowsXunit.FlowXunitExtensions;

namespace CustomerPortal.Features.SmsVerification
{
    public class When_verify_customer_and_book_appointment_successfully : FlowSteps<When_verify_customer_and_book_appointment_successfully.State>, IFlowFailAllSteps
    {
        public When_verify_customer_and_book_appointment_successfully(State state) : base(state)
        {
        }

        [Step($"{nameof(S1_Create_new_customer_with_valuation_step)}")]
        public async Task S1_Create_new_customer_with_valuation_step()
        {
        }

        [Step($"{nameof(S2_Start_phone_number_verification_step)}")]
        public async Task S2_Start_phone_number_verification_step()
        {
        }

        public class State
        {
            public string Customer { get; set; }
           
        }
    }
}
