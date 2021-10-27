using FlowsXunit.FlowXunitExtensions;
using System;
using Xunit;

namespace Examples
{
    public class When_you_have_a_new_stack : FlowSteps<When_you_have_a_new_stack.State>, IFlowFailAllSteps
    {
        public When_you_have_a_new_stack(State state)
            : base(state)
        {
        }

        [Step(nameof(S1_should_be_empty))]
        public void S1_should_be_empty()
        {
            Assert.True(state.Stack.IsEmpty);
        }

        [Step(nameof(S2_should_not_allow_you_to_call_PopTheory))]
        public void S2_should_not_allow_you_to_call_PopTheory()
        {
            Assert.Throws<InvalidOperationException>(() => state.Stack.Pop());
        }

        [Step(nameof(S4_should_not_allow_you_to_call_Pop))]
        public void S4_should_not_allow_you_to_call_Pop()
        {
            Assert.NotNull(state.Stack.Pop());
        }

        [Step(nameof(S3_should_not_allow_you_to_call_Pop1))]
        public void S3_should_not_allow_you_to_call_Pop1()
        {
            state.Stack.Push("el");
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
