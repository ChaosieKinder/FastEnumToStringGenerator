using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FastEnumToStringGenerator
{
    [Generator]
    public class EnumToStringGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is MySytaxReceiver receiver))
                return;
            
            var code = new CodeWriter();
            code.AppendLine("using System;");
            code.AppendLine();
            foreach (var workItem in receiver.WorkItems)
            {
                using (code.BeginScope($"namespace {workItem.FullNamespace()}"))
                {
                    using (code.BeginScope($"internal static partial class EnumToStringExtensions"))
                    {
                        using (code.BeginScope($"public static string FastToString(this {workItem.FullName()} enumValue)"))
                        {
                            using (code.BeginScope("switch(enumValue)"))
                            {
                                foreach (var member in workItem.GetMembers().Where(m => m.Kind == SymbolKind.Field))
                                {
                                    code.AppendLine($"case {member.ToString()}:");
                                    code.AppendLine($"   return nameof({member.ToString()});");
                                }
                                code.AppendLine("default:");
                                code.AppendLine("    throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, null);");
                            }
                        }
                    }
                }
            }

            context.AddSource("Logs", SourceText.From($@"/*{ Environment.NewLine + string.Join(Environment.NewLine, receiver.Log) + Environment.NewLine}*/", Encoding.UTF8));
            context.AddSource("EnumToStringExtensions.g.cs", SourceText.From(code.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MySytaxReceiver());
        }
    }

    public class MySytaxReceiver : ISyntaxContextReceiver
    {
        public List<string> Log { get; } = new();
#pragma warning disable RS1024 // Ignore, we want nullables to be counted the same as their non-nullable type so this behavior is fine.
        public HashSet<INamedTypeSymbol> WorkItems { get; } = new();
#pragma warning restore RS1024
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            try
            {
                if (context.Node is EnumDeclarationSyntax enumDeclarationSyntax)
                {
                    var typeInfo = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!;
                    Log.Add($"Found a enum named {typeInfo.Name}");
                    WorkItems.Add(typeInfo);
                }
                else if (context.Node is ClassDeclarationSyntax csDeclarationSyntax)
                {
                    var typeInfo = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!;
                    Log.Add($"Found a class named {typeInfo.Name}");
                    var declaredClass = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!;
                    var makeFastEnum = declaredClass.GetAttributes().Where(att => att.AttributeClass.FullName() == "FastEnumToStringGenerator.ExtraFastEnumAttribute");
                    foreach(var attribute in makeFastEnum)
                    {
                        Log.Add($"Found a attribute");
                        var extraFastEnumType = (INamedTypeSymbol?)attribute.ConstructorArguments[0].Value;
                        if (extraFastEnumType != null)
                            WorkItems.Add(extraFastEnumType);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Add("Error parsing syntax: " + ex.ToString());
            }
        }
    }

    public static class Extensions
    {
        // Sourced:
        // https://github.com/Grauenwolf/Tortuga-TestMonkey/blob/main/Tortuga.TestMonkey/SemanticHelper.cs
        public static string FullNamespace(this ISymbol symbol)
        {
            var parts = new Stack<string>();
            INamespaceSymbol? iterator = (symbol as INamespaceSymbol) ?? symbol.ContainingNamespace;
            while (iterator != null)
            {
                if (!string.IsNullOrEmpty(iterator.Name))
                    parts.Push(iterator.Name);
                iterator = iterator.ContainingNamespace;
            }
            return string.Join(".", parts);
        }

        // Sourced:
        // https://github.com/Grauenwolf/Tortuga-TestMonkey/blob/main/Tortuga.TestMonkey/SemanticHelper.cs
        public static string? FullName(this INamedTypeSymbol? symbol)
        {
            if (symbol == null)
                return null;

            var prefix = FullNamespace(symbol);
            var suffix = "";
            if (symbol.Arity > 0)
            {
                suffix = "<" + string.Join(", ", symbol.TypeArguments.Select(targ => FullName((INamedTypeSymbol)targ))) + ">";
            }

            if (prefix != "")
                return prefix + "." + symbol.Name + suffix;
            else
                return symbol.Name + suffix;
        }
    }
}
