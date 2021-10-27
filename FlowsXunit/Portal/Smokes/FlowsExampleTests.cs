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

        [Step(nameof(should_be_empty)), StepOrder(1)]
        public void should_be_empty()
        {
            Assert.True(state.Stack.IsEmpty);
        }

        [Step, StepOrder(2)]
        public void should_not_allow_you_to_call_PopTheory()
        {
            Assert.Throws<InvalidOperationException>(() => state.Stack.Pop());
        }

        [Step, StepOrder(4)]
        public void should_not_allow_you_to_call_Pop()
        {
            Assert.NotNull(state.Stack.Pop());
        }

        [Step, StepOrder(3)]
        public void should_not_allow_you_to_call_Pop1()
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
