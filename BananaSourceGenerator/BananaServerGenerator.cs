// -----------------------------------------------------------------------
// <copyright file="BananaRoleGenerator.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaSourceGenerator;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// A generator for BananaServerAttributes.
/// </summary>
[Generator]
public class BananaServerGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Gather all properties with attributes
        IncrementalValuesProvider<PropertyInfo?> propertyDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is PropertyDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (ctx, _) =>
                {
                    // Debugger.Launch();
                    return GetSemanticTargetForGeneration(ctx);
                })
            .Where(static m => m is not null);
        

        // Step 2: Combine with Compilation
        IncrementalValueProvider<(Compilation Left, ImmutableArray<PropertyInfo?> Right)> compilationAndProperties = context.CompilationProvider.Combine(propertyDeclarations.Collect());

        // Step 3: Register the source output
        context.RegisterSourceOutput(compilationAndProperties, (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static PropertyInfo? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        PropertyDeclarationSyntax propertySyntax = (PropertyDeclarationSyntax)context.Node;
        IPropertySymbol? propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertySyntax) as IPropertySymbol;

        if (propertySymbol == null)
        {
            return null;
        }

        foreach (AttributeData? attr in propertySymbol.GetAttributes())
        {
            if (attr.AttributeClass?.ToDisplayString() is "BananaLibrary.API.Attributes.BananaServerAttribute" && attr.ConstructorArguments.Length == 3)
            {
                string? serverName = attr.ConstructorArguments[0].Value?.ToString();
                string? serverId = attr.ConstructorArguments[1].Value?.ToString();
                ushort? serverPort = (ushort?)attr.ConstructorArguments[2].Value;
                if (serverName != null && serverId != null && serverPort != null)
                {
                    return new PropertyInfo(
                        propertySymbol.Name,
                        propertySymbol.ContainingNamespace.ToDisplayString(),
                        serverName,
                        serverId,
                        serverPort.Value);
                }
            }
        }

        return null;
    }

    // ReSharper disable once UnusedParameter.Local
    private void Execute(Compilation compilation, ImmutableArray<PropertyInfo?> properties, SourceProductionContext context)
    {
        foreach (PropertyInfo? prop in properties.Where(p => p != null).Cast<PropertyInfo>())
        {
            string source = GenerateMetadataClass(prop);
            INamedTypeSymbol? existing = compilation.GetTypeByMetadataName($"{prop.Namespace}.{prop.PropertyName}Server");

            if (existing is null)
            {
                context.AddSource($"{prop.PropertyName}Server.g.cs", source);
            }
        }
    }

    private string GenerateMetadataClass(PropertyInfo prop)
    {
        StringBuilder sb = new(
$@"namespace {prop.Namespace};

using System.Runtime.CompilerServices;

using BananaLibrary.API.Features;

/// <summary>
/// The <see cref=""BananaServer""/> instance for {prop.PropertyName}.
/// </summary>
[CompilerGenerated]
public sealed class {prop.PropertyName}Server : BananaServer
{{
    /// <inheritdoc />
    public override string ServerName => ""{prop.ServerName}"";

    /// <inheritdoc />
    public override string ServerId => ""{prop.ServerId}"";

    /// <inheritdoc />
    public override ushort ServerPort => {prop.ServerPort};
}}");
        return sb.ToString();
    }

    private record PropertyInfo(string PropertyName, string Namespace, string ServerName, string ServerId, ushort ServerPort);
}
