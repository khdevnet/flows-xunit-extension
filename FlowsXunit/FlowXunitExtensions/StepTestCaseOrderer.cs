using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlowsXunit.FlowXunitExtensions
{
    public class StepTestCaseOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            return HasOrderAttribute(testCases)
                ? new OrderAttributeTestCaseOrderer().OrderTestCases(testCases)
                : new DisplayNameTestCaseOrderer().OrderTestCases(testCases);
        }

        private static bool HasOrderAttribute<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            return testCases
                    .SelectMany(t => t.TestMethod.Method.GetCustomAttributes((typeof(StepOrderAttribute).AssemblyQualifiedName)))
                    .Any(attr => attr.GetNamedArgument<int>(nameof(StepOrderAttribute.Order)) > 0);
        }
    }
}
