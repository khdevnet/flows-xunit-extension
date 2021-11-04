using Xunit.Scenario.CodeGenerator;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Xunit.Scenario.CodeGenerator.Test
{
    [UsesVerify]
    public class ScenarioXunitTestsGeneratorTests
    {
        [Fact]
        public Task CodeGeneratorTests()
        {
            var input = @"
using System.Threading.Tasks;

namespace DemoTests
{
   [Xunit.Scenario.Scenario(@$""
* step 1
"")]
   public partial class ProductCategory
   {
   }
}
";

            var settings = new VerifySettings();
            settings.UseDirectory("ScenarioXunitTestsGeneratorTests");

            return Verifier.Verify(GetGeneratedOutput(input), settings);
        }

        private static string GetGeneratedOutput(string sourceCode)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                                      .Where(assembly => !assembly.IsDynamic)
                                      .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                                      .Cast<MetadataReference>();

            var compilation = CSharpCompilation.Create("SourceGeneratorTests",
                                                       new[] { syntaxTree },
                                                       references,
                                                       new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new ScenarioXunitTestsGenerator();
            CSharpGeneratorDriver.Create(generator)
                                 .RunGeneratorsAndUpdateCompilation(compilation,
                                                                    out var outputCompilation,
                                                                    out var diagnostics);

            // optional
            diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)
                       .Should().BeEmpty();

            return outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString();
        }
    }
}
