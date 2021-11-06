using System;
using Xunit;

namespace Xunit.Scenario.Extension
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StepOrderAttribute : Attribute
    {
        public StepOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; private set; }
    }
}
