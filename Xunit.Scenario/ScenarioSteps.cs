using System;
using Xunit;

namespace Xunit.Scenario
{
    public abstract class ScenarioSteps<TFlowState> : IClassFixture<TFlowState> where TFlowState : class
    {
        protected TFlowState state;

        public ScenarioSteps(TFlowState state)
        {
            this.state = state;
        }
    }

    public abstract class ScenarioState<TScenarioState> : ICollectionFixture<TScenarioState> where TScenarioState : class
    {
        protected TScenarioState state;

        public ScenarioState(TScenarioState state)
        {
            this.state = state;
        }
    }
}
