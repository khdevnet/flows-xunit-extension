using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlowsXunit.CodeGenerator
{
    [Generator]
    public class ScenarioXunitTestsGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {

//                Debugger.Launch();

//            }

//#endif

            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            var syntaxReceiver = (SyntaxReceiver)context.SyntaxReceiver;
            var xunitTestsTargets = syntaxReceiver.TypeDeclarationsWithAttributes;

            var scenarioAttributeType = compilation.GetTypeByMetadataName(ScenarioAttributeTemplate.FullQualifiedName);

            foreach (var targetTypeSyntax in xunitTestsTargets)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var semanticModel = compilation.GetSemanticModel(targetTypeSyntax.SyntaxTree);
                var targetType = semanticModel.GetDeclaredSymbol(targetTypeSyntax);
                var testClassScenarioAttribute = targetType.GetAttributes()
                    .SingleOrDefault(attr => attr.AttributeClass.Equals(scenarioAttributeType));

                if (!IsScenarioType(targetType, testClassScenarioAttribute)) continue;
                if (!IsAppliedToClass(context, targetTypeSyntax)) continue;
                if (!TryGetScenarioText(testClassScenarioAttribute, out string scenarioText)) continue;

                var scenraioTestClassProvider = new ScenarioTestClassTemplate(targetType);
                var proxySource = scenraioTestClassProvider.GetSource(scenarioText);
                context.AddSource($"{scenraioTestClassProvider.ClassName}.scenario.cs", proxySource);
            }
        }

        private static bool TryGetScenarioText(AttributeData testClassScenarioAttribute, out string scenarioTextValue)
        {
            scenarioTextValue = "";
            if (testClassScenarioAttribute.ConstructorArguments.Length == 0)
            {
                return false;
            }

            var scenarioTextArg = testClassScenarioAttribute.ConstructorArguments[0];
            scenarioTextValue = scenarioTextArg.Value as string;

            return !string.IsNullOrEmpty(scenarioTextValue);
        }

        private static bool IsAppliedToClass(GeneratorExecutionContext context, TypeDeclarationSyntax targetTypeSyntax)
        {
            if (targetTypeSyntax is not ClassDeclarationSyntax)
            {
                context.ReportDiagnostic(
                  Diagnostic.Create(
                    "SC01",
                    "Scenario generator",
                    "[Scenario] must be applied to an class",
                    defaultSeverity: DiagnosticSeverity.Error,
                    severity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    warningLevel: 0,
                    location: targetTypeSyntax.GetLocation()));
                return false;
            }
            return true;
        }

        private static bool IsScenarioType(INamedTypeSymbol targetType, AttributeData testClassScenarioAttribute)
            => testClassScenarioAttribute != null && testClassScenarioAttribute.ConstructorArguments.Length > 0;
    }
}
