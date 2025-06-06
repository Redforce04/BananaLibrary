// -----------------------------------------------------------------------
// <copyright file="ConfigurationOptionTypeInspector.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Utils;

using System.Collections.Generic;
using System.Linq;

using BananaLibrary.API.Attributes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

/// <summary>
/// Used by YamlDotNet to serialize specific properties that include the <see cref="BananaConfigAttribute"/> Attribute.
/// </summary>
public class ConfigurationOptionTypeInspector : TypeInspectorSkeleton
{
    private readonly ITypeInspector innerTypeInspector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationOptionTypeInspector"/> class.
    /// </summary>
    /// <param name="innerTypeInspector">The Type Inspector passed through.</param>
    public ConfigurationOptionTypeInspector(ITypeInspector innerTypeInspector)
    {
        this.innerTypeInspector = innerTypeInspector;
    }

    /// <inheritdoc/>
    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
    {
        IEnumerable<IPropertyDescriptor> properties = innerTypeInspector.GetProperties(type, container);

        return properties.Where(prop =>
        {
            BananaConfigAttribute? memberInfo = prop.GetCustomAttribute<BananaConfigAttribute>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            return memberInfo != null;
        });
    }
}