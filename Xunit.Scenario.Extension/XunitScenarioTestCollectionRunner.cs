using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Scenario.Extension
{
    public class FlowsXunitTestCollectionRunner : XunitTestCollectionRunner
    {
       
        public FlowsXunitTestCollectionRunner(ITestCollection testCollection,
                                         IEnumerable<IXunitTestCase> testCases,
                                         IMessageSink diagnosticMessageSink,
                                         IMessageBus messageBus,
                                         ITestCaseOrderer testCaseOrderer,
                                         ExceptionAggregator aggregator,
                                         CancellationTokenSource cancellationTokenSource)
            : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
        }

        /// <inheritdoc/>
        protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
            => new XunitScenarioTestClassRunner(testClass, @class, testCases, DiagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, CollectionFixtureMappings).RunAsync();
    }
}
