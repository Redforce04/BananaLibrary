// -----------------------------------------------------------------------
// <copyright file="BananaRoleGenerator.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaSourceGenerator;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// A generator for BananaRoleAttributes.
/// </summary>
[Generator]
public class BananaRoleGenerator : IIncrementalGenerator
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

        List<InheritedRole> inheritedRoles = new();
        PropertyInfo? property = null;
        foreach (AttributeData? attr in propertySymbol.GetAttributes())
        {
            string attrName = attr.AttributeClass?.ToDisplayString() ?? "unknown";
            if (!attrName.StartsWith("BananaLibrary.API.Attributes."))
            {
                continue;
            }
            int len = "BananaLibrary.API.Attributes.".Length;
            attrName = attrName.Substring(len, attrName.Length - len);

            int typeLen = "InheritBananaRoleAttribute".Length;
            if (attrName.Length > typeLen && attrName[typeLen] == '<')
            {
                string inheritedRoleType = attrName.Substring(typeLen + 1, attrName.Length - typeLen - 2);
                inheritedRoles.Add(new InheritedRoleT(propertySymbol.Name, propertySymbol.ContainingAssembly.ToDisplayString(), inheritedRoleType));
                continue;
            }
            switch (attrName)
            {
                case "InheritBananaRoleAttribute":
                    if (attr.ConstructorArguments.Length != 1)
                    {
                        continue;
                    }
                    string? inheritedRoleName = attr.ConstructorArguments[0].Value?.ToString();
                    if (inheritedRoleName is null)
                    {
                        continue;
                    }
                    inheritedRoles.Add(new InheritedRoleName(propertySymbol.Name, propertySymbol.ContainingAssembly.ToDisplayString(), inheritedRoleName));
                    break;
                case "BananaRoleAttribute":
                    if (property is not null || attr.ConstructorArguments.Length != 2)
                    {
                        continue;
                    }
                    string? roleName = attr.ConstructorArguments[0].Value?.ToString();
                    string? groupName = attr.ConstructorArguments[1].Value?.ToString();
                    if (roleName != null && groupName != null)
                    {
                        property = new PropertyInfo(
                            propertySymbol.Name,
                            propertySymbol.ContainingNamespace.ToDisplayString(),
                            roleName,
                            groupName);
                    }
                    break;
            }
            if (property is not null)
            {
                return new PropertyInfo(property.PropertyName, property.Namespace, property.RoleName, property.GroupName, inheritedRoles);
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
            INamedTypeSymbol? existing = compilation.GetTypeByMetadataName($"{prop.Namespace}.{prop.PropertyName}Role");

            if (existing is null)
            {
                context.AddSource($"{prop.PropertyName}Role.g.cs", source);
            }
        }
    }

    private string GenerateMetadataClass(PropertyInfo prop)
    {
        string inheritedRoles = "";
        if (prop.Roles is not null)
        {
            foreach (InheritedRole? x in prop.Roles)
            {
                if (x is InheritedRoleName name)
                {
                    inheritedRoles += $"\n[InheritBananaRole(\"{name.RoleName}\")]";
                }
                if (x is InheritedRoleT t)
                {
                    inheritedRoles += $"\n[InheritBananaRole<{t.RoleType}>]";
                }
            }
            
        }
        StringBuilder sb = new(
$@"namespace {prop.Namespace};

using System.Runtime.CompilerServices;
{(inheritedRoles.Length > 0 ? "\nusing BananaLibrary.API.Attributes;" : string.Empty)}
using BananaLibrary.API.Features;

/// <summary>
/// The <see cref=""BananaRole""/> instance for {prop.PropertyName}.
/// </summary>
[CompilerGenerated]{inheritedRoles}
public sealed class {prop.PropertyName}Role : BananaRole
{{
    /// <inheritdoc />
    public override string Name => ""{prop.RoleName}"";

    /// <inheritdoc />
    public override string GroupName => ""{prop.GroupName}"";
}}");
        return sb.ToString();
    }

    private record Info(string PropertyName, string Namespace);
    private record InheritedRole(string PropertyName, string Namespace) : Info(PropertyName, Namespace);
    private record PropertyInfo(string PropertyName, string Namespace, string RoleName, string GroupName, List<InheritedRole>? Roles = null) : Info(PropertyName, Namespace);
    private record InheritedRoleT(string PropertyName, string Namespace, string RoleType) : InheritedRole(PropertyName, Namespace);
    private record InheritedRoleName(string PropertyName, string Namespace, string RoleName) : InheritedRole(PropertyName, Namespace);
}
