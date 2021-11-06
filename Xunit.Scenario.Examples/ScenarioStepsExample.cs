using Xunit.Scenario.Extension;
using System.Threading.Tasks;
using Xunit;

namespace Examples
{
    public class ScenarioStepsExample : ScenarioSteps<ScenarioStepsExample.State>, IScenarioFailNotRunnedSteps
    {
        public ScenarioStepsExample(State state)
            : base(state)
        {
        }

        [Fact(DisplayName = nameof(should_be_runned_first)), StepOrder(1)]
        public void should_be_runned_first()
        {
            Assert.True(state.Stack.IsEmpty);
        }

        // Shouldbe runned manually
        // Should fail
        //[Fact, StepOrder(2)]
        //public void should_be_runned_second_and_fail()
        //{
        //    state.Stack.Pop();
        //}

        //[Fact, StepOrder(4)]
        //public void should_be_failed_because_of_fail_second()
        //{
        //    Assert.NotNull(state.Stack.Pop());
        //}

        //[Fact, StepOrder(3)]
        //public void should_be_failed_because_of_fail_first()
        //{
        //    state.Stack.Push("el");
        //}

        public class State
        {
            public MyStack<string> Stack;
            public State()
            {
                Stack = new MyStack<string>();
            }

        }
    }

    public class When_order_by_attribute : ScenarioSteps<When_order_by_attribute.State>, IScenarioFailNotRunnedSteps
    {
        public When_order_by_attribute(State state)
            : base(state)
        {
        }

        [Fact, StepOrder(1)]
        public void should_be_empty()
        {
            Assert.True(this.state.Stack.IsEmpty);
            this.state.Stack.Push(nameof(should_be_empty));
        }

        [Fact, StepOrder(2)]
        public void should_be_second()
        {
            Assert.True(this.state.Stack.elements.Count == 1);
            this.state.Stack.Push(nameof(should_be_second));
        }

        [Fact, StepOrder(4)]
        public void should_be_fourth()
        {
            Assert.True(this.state.Stack.elements.Count == 3);
            this.state.Stack.Push(nameof(should_be_fourth));
        }

        [Fact, StepOrder(3)]
        public void should_be_third()
        {
            Assert.True(this.state.Stack.elements.Count == 2);
            this.state.Stack.Push(nameof(should_be_third));
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

    public class When_order_by_name : ScenarioSteps<When_order_by_name.State>, IScenarioFailNotRunnedSteps
    {
        public When_order_by_name(State state)
            : base(state)
        {
        }

        [Fact(DisplayName = $"{nameof(S1)}")]
        public async Task S1()
        {
            Assert.True(this.state.Stack.IsEmpty);
            this.state.Stack.Push(nameof(S1));
        }

        [Fact(DisplayName = $"{nameof(S2)}")]
        public async Task S2()
        {
            Assert.True(this.state.Stack.elements.Count == 1);
            this.state.Stack.Push(nameof(S2));
        }

        [Fact(DisplayName = $"{nameof(S4)}")]
        public async Task S4()
        {
            Assert.True(this.state.Stack.elements.Count == 3);
            this.state.Stack.Push(nameof(S4));
        }

        [Fact(DisplayName = $"{nameof(S3)}")]
        public async Task S3()
        {
            Assert.True(this.state.Stack.elements.Count == 2);
            this.state.Stack.Push(nameof(S3));
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
