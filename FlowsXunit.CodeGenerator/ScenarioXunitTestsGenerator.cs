using System;
using System.Collections.Generic;
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
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public HashSet<TypeDeclarationSyntax> TypeDeclarationsWithAttributes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax declaration
                && declaration.AttributeLists.Any())
            {
                TypeDeclarationsWithAttributes.Add(declaration);
            }
        }
    }

    [Generator]
    public class ScenarioXunitTestsGenerator : ISourceGenerator
    {
        private const string BackingFieldSuffix = "BackingField";

        public void Initialize(GeneratorInitializationContext context)
        {
            //#if DEBUG
            //            if (!Debugger.IsAttached)
            //            {
            //
            //                Debugger.Launch();
            //
            //            }

            //#endif

            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            var syntaxReceiver = (SyntaxReceiver)context.SyntaxReceiver;
            var xunitTestsTargets = syntaxReceiver.TypeDeclarationsWithAttributes;

            compilation = AddSourceClass(context, compilation, ScenarioAttributeClassProvider.ClassName, ScenarioAttributeClassProvider.Get());
            compilation = AddSourceClass(context, compilation, StepAttributeClassProvider.ClassName, StepAttributeClassProvider.Get());


            var scenarioAttributeType = compilation.GetTypeByMetadataName(ScenarioAttributeClassProvider.ClassName);

            var targetTypes = new HashSet<ITypeSymbol>();
            foreach (var targetTypeSyntax in xunitTestsTargets)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var semanticModel = compilation.GetSemanticModel(targetTypeSyntax.SyntaxTree);
                var targetType = semanticModel.GetDeclaredSymbol(targetTypeSyntax);

                var testClassScenarioAttribute = targetType.GetAttributes().SingleOrDefault(attr => attr.AttributeClass.Equals(scenarioAttributeType));


                if (testClassScenarioAttribute == null)
                {
                    continue;
                }

                if (testClassScenarioAttribute.ConstructorArguments.Length == 0)
                {
                    continue;
                }

                var scenarioText = testClassScenarioAttribute.ConstructorArguments[0];
                var scenarioTextValue = scenarioText.Value as string;

                if (string.IsNullOrEmpty(scenarioTextValue))
                {
                    continue;
                }

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
                    continue;
                }

                targetTypes.Add(targetType);
            }

            foreach (var targetType in targetTypes)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var testClassScenarioAttribute = targetType.GetAttributes().SingleOrDefault(attr => attr.AttributeClass.Equals(scenarioAttributeType));
                var scenarioText = testClassScenarioAttribute.ConstructorArguments[0];
                var scenarioTextValue = scenarioText.Value as string;


                var scenraioTestClassProvider = new ScenarioTestClassProvider(targetType);
                var proxySource = scenraioTestClassProvider.GetSource(scenarioTextValue);
                context.AddSource($"{scenraioTestClassProvider.ClassName}.scenario.cs", proxySource);
            }
        }

        private static Compilation AddSourceClass(
            GeneratorExecutionContext context,
            Compilation compilation,
            string className,
            string scenarioSrc)
        {
            context.AddSource($"{className}.cs", scenarioSrc);

            var options = (CSharpParseOptions)compilation.SyntaxTrees.First().Options;
            var logSyntaxTree = CSharpSyntaxTree.ParseText(scenarioSrc, options);
            compilation = compilation.AddSyntaxTrees(logSyntaxTree);
            return compilation;
        }
    }
}
