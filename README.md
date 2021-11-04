# xunit scenario extension
Extends xunit test runner, can be use for E2E testing
* Fail not runned tests if one fail
* Run tests in order

# Configuration
* Create the setup class.
* Add assembly attribute with XunitScenarioTestFramework setup to it
```csharp
using Xunit.Scenario;
using Xunit;

[assembly: TestFramework($"Xunit.Scenario.{nameof(XunitScenarioTestFramework)}", "Xunit.Scenario")]
```
* Look [Setup](https://github.com/khdevnet/xunit-scenario/blob/main/Xunit.Scenario.Examples/Setup.cs) from example project

