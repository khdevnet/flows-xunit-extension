using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ScenarioTestClassTemplate
{
    public ScenarioTestClassTemplate(ITypeSymbol targetType)
    {
        TargetType = targetType;
    }
    public string ClassName => TargetType.Name;

    public ITypeSymbol TargetType { get; }

    public string GetSource(string scenarioTextValue)
    {
        List<string> testCases = GetUniqueTestCase(scenarioTextValue);

        var allInterfaceMethods = TargetType.AllInterfaces
          .SelectMany(x => x.GetMembers())
          .Concat(TargetType.GetMembers())
          .OfType<IMethodSymbol>()
          .ToList();

        var sb = new StringBuilder();
        sb.Append($@"
using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace {TargetType.ContainingNamespace}
{{

  public partial class {ClassName}
  {{ ");
        for (int i = 0; i < testCases.Count; i++)
        {
            var testCase = $"S{i + 1} {testCases[i]}";
            var methodName = testCase.Replace(" ", "_");

            sb.Append(@$"
        [Step(""{testCase}"")]
        public partial Task {methodName}();
");
        }

        sb.Append(@"
  }
}");
        return sb.ToString();
    }

    private static List<string> GetUniqueTestCase(string scenarioTextValue)
    {
        var testCases = scenarioTextValue.Replace(System.Environment.NewLine, "").Split(new string[] { "* " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        var uniqueTestCases = new HashSet<string>(testCases);
        return uniqueTestCases.ToList();
    }
}
