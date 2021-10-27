using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlowsXunit.FlowXunitExtensions
{
    public class FlowXunitTestClassRunner : XunitTestClassRunner
    {
        private readonly ITestCaseOrderer _testCaseOrderer;
        private readonly IEnumerable<IXunitTestCase> _testCases;

        public FlowXunitTestClassRunner(ITestClass testClass,
                                    IReflectionTypeInfo @class,
                                    IEnumerable<IXunitTestCase> testCases,
                                    IMessageSink diagnosticMessageSink,
                                    IMessageBus messageBus,
                                    ITestCaseOrderer testCaseOrderer,
                                    ExceptionAggregator aggregator,
                                    CancellationTokenSource cancellationTokenSource,
                                    IDictionary<Type, object> collectionFixtureMappings)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
        {
            _testCaseOrderer = testCaseOrderer;
            _testCases = testCases;
        }

        protected override Task<RunSummary> RunTestMethodsAsync()
        {
            var shouldFailFast = Class.ToRuntimeType().GetInterfaces().Contains(typeof(IFlowFailNotRunnedSteps));

            return shouldFailFast ? RunTestMethodsWithFailFastAsync() : RunTestMethodsOriginalAsync();
        }

        private async Task<RunSummary> RunTestMethodsOriginalAsync()
        {
            var summary = new RunSummary();
            IEnumerable<IXunitTestCase> orderedTestCases;
            try
            {
                orderedTestCases = _testCaseOrderer.OrderTestCases(TestCases);
            }
            catch (Exception ex)
            {
                var innerEx = ex.Unwrap();
                DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Test case orderer '{TestCaseOrderer.GetType().FullName}' threw '{innerEx.GetType().FullName}' during ordering: {innerEx.Message}{Environment.NewLine}{innerEx.StackTrace}"));
                orderedTestCases = _testCases.ToList();
            }

            var constructorArguments = CreateTestClassConstructorArguments();

            foreach (var method in orderedTestCases.GroupBy(tc => tc.TestMethod, TestMethodComparer.Instance))
            {
                summary.Aggregate(await RunTestMethodAsync(method.Key, (IReflectionMethodInfo)method.Key.Method, method, constructorArguments));
                if (CancellationTokenSource.IsCancellationRequested)
                    break;
            }

            return summary;
        }

        private async Task<RunSummary> RunTestMethodsWithFailFastAsync()
        {
            IEnumerable<IXunitTestCase> orderedTestCases;
            try
            {
                orderedTestCases = new StepTestCaseOrderer().OrderTestCases(TestCases);
            }
            catch (Exception ex)
            {
                var innerEx = ex.Unwrap();
                DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Test case orderer '{TestCaseOrderer.GetType().FullName}' threw '{innerEx.GetType().FullName}' during ordering: {innerEx.Message}{Environment.NewLine}{innerEx.StackTrace}"));
                orderedTestCases = TestCases.ToList();
            }

            var summary = new RunSummary();
            var constructorArguments = CreateTestClassConstructorArguments();

            bool hasFailedTest = false;

            foreach (IGrouping<ITestMethod, IXunitTestCase> method in orderedTestCases.GroupBy(tc => tc.TestMethod, TestMethodComparer.Instance))
            {
                if (hasFailedTest)
                {
                    Aggregator.Add(new TestClassException("Single test case fail. Fail all test cases in the class."));
                    method.ToList().ForEach(testCase =>
                    {
                        MessageBus.QueueMessage(new TestFailed(new XunitTest(testCase, testCase.DisplayName), 0, "Single test case fail. Fail all test cases in class", Aggregator.ToException()));
                        summary.Aggregate(new RunSummary() { Failed = 1, Total = 1 });
                    });
                }
                else
                {
                    var testMethodRunSummary = await RunTestMethodAsync(method.Key, (IReflectionMethodInfo)method.Key.Method, method, constructorArguments);
                    hasFailedTest = testMethodRunSummary.Failed > 0;
                    summary.Aggregate(testMethodRunSummary);
                }

                if (CancellationTokenSource.IsCancellationRequested)
                    break;
            }

            return summary;
        }
    }
}
