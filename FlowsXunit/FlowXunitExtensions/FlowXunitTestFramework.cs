using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlowsXunit.FlowXunitExtensions
{
    public class FlowXunitTestFramework : XunitTestFramework
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XunitTestFramework"/> class.
        /// </summary>
        /// <param name="messageSink">The message sink used to send diagnostic messages</param>
        public FlowXunitTestFramework(IMessageSink messageSink) : base(messageSink) { }

        /// <inheritdoc/>
        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new FlowXunitTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        }
    }
}
