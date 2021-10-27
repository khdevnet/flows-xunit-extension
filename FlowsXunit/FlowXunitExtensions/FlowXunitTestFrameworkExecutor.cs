using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlowsXunit.FlowXunitExtensions
{
    public class FlowXunitTestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        public FlowXunitTestFrameworkExecutor(AssemblyName assemblyName,
                                          ISourceInformationProvider sourceInformationProvider,
                                          IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
        }

        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            using (var assemblyRunner = new FlowXunitTestAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions))
                await assemblyRunner.RunAsync();
        }
    }
}