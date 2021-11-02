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
        private const string ScenarioAttributeClassName = "ScenarioAttribute";

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

            var scenarioSrc = ScenarioAttributeClassProvider.Get();
            compilation = AddSourceClass(context, compilation, ScenarioAttributeClassName, scenarioSrc);
            var stepAttributeSrc = StepAttributeClassProvider.Get();
            compilation = AddSourceClass(context, compilation, "StepAttribute", stepAttributeSrc);


            var scenarioAttribute = compilation.GetTypeByMetadataName(ScenarioAttributeClassName);

            var targetTypes = new HashSet<ITypeSymbol>();
            foreach (var targetTypeSyntax in xunitTestsTargets)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var semanticModel = compilation.GetSemanticModel(targetTypeSyntax.SyntaxTree);
                var targetType = semanticModel.GetDeclaredSymbol(targetTypeSyntax);
                var hasScenarioAttribute = targetType.GetAttributes()
                  .Any(x => x.AttributeClass.Equals(scenarioAttribute));

                if (!hasScenarioAttribute)
                    continue;

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

                var proxySource = GenerateProxy(targetType);
                context.AddSource($"{targetType.Name}.scenario.cs", proxySource);
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

        private string GenerateProxy(ITypeSymbol targetType)
        {
            var allInterfaceMethods = targetType.AllInterfaces
              .SelectMany(x => x.GetMembers())
              .Concat(targetType.GetMembers())
              .OfType<IMethodSymbol>()
              .ToList();

            var fullQualifiedName = GetFullQualifiedName(targetType);

            var sb = new StringBuilder();
            var proxyName = targetType.Name;
            sb.Append($@"
using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using FlowsXunit.FlowXunitExtensions;

namespace {targetType.ContainingNamespace}
{{

  public partial class {proxyName}
  {{ ");


            sb.Append(@"
        [Step]
        public partial Task Test1();
");

            sb.Append(@"
  }
}");
            return sb.ToString();
        }

        private static string GetFullQualifiedName(ISymbol symbol)
        {
            var containingNamespace = symbol.ContainingNamespace;
            if (!containingNamespace.IsGlobalNamespace)
                return containingNamespace.ToDisplayString() + "." + symbol.Name;

            return symbol.Name;
        }
    }
}
