using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace FlowsXunit.FlowXunitExtensions
{
    public class FlowTestCaseOrderer
    {
        public IEnumerable<IXunitTestCase> OrderTestCases(IEnumerable<IXunitTestCase> testCases)
        {
            var result = testCases.ToList();
            result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            return result;
        }
    }
}
