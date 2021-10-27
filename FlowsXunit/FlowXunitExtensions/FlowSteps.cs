using Xunit;

namespace FlowsXunit.FlowXunitExtensions
{
    public abstract class FlowSteps<TFixture> : IClassFixture<TFixture> where TFixture : class
    {
        protected TFixture state;

        public FlowSteps(TFixture state)
        {
            this.state = state;
        }
    }
}
