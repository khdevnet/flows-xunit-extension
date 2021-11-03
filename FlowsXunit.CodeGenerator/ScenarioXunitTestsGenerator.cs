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
#if DEBUG
            if (!Debugger.IsAttached)
            {

                Debugger.Launch();

            }

#endif

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
                var aa = testClassScenarioAttribute.ConstructorArguments[0];
                var iconExpr = aa.Value as string;



                var r = testClassScenarioAttribute.AttributeClass.OriginalDefinition.GetType();

                var textField = testClassScenarioAttribute.AttributeClass.GetMembers().OfType<IFieldSymbol>()
                    .Where(x => x.Name.EndsWith(BackingFieldSuffix))
                    .FirstOrDefault(fieldSymbol => GetFieldName(fieldSymbol) == ScenarioAttributeClassProvider.TextFieldName);

                if (textField != null)
                {
                    var v = textField.ConstantValue;
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

                var proxySource = GenerateProxy(targetType);
                context.AddSource($"{targetType.Name}.scenario.cs", proxySource);
            }
        }

        private static string GetFieldName(IFieldSymbol fieldSymbol)
        {
            return fieldSymbol.OriginalDefinition.AssociatedSymbol.Name;
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
