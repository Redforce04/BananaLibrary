// -----------------------------------------------------------------------
// <copyright file="BananaConfigDefaultAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using System.Linq;
using System.Reflection;

using BananaLibrary.API.Features;

/// <summary>
/// Sets a default value for a configuration based on the current server.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class BananaConfigDefaultAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaConfigDefaultAttribute"/> class.
    /// </summary>
    /// <param name="defaultValue">The default value of the configuration object.</param>
    /// <param name="serverType">The type of the server to evaluate.</param>
    // ReSharper disable once MemberCanBeProtected.Global
    public BananaConfigDefaultAttribute(object defaultValue, Type serverType)
    {
        this.DefaultValue = defaultValue;
        this.Type = serverType;
    }

    /// <summary>
    /// Gets the default value of the configuration option for the provided server.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    internal object DefaultValue { get; }

    /// <summary>
    /// Gets the type of the <see cref="BananaServer"/> to find.
    /// </summary>
    private Type Type { get; }

    /// <summary>
    /// Checks to see if the server option for this config is the current running server instance.
    /// </summary>
    /// <param name="assembly">The assembly to search.</param>
    /// <returns>A value indicating whether the specified server is the current running server instance.</returns>
    internal bool IsCurrentServer(Assembly assembly)
    {
        return BananaPlugin.BananaPlugins.FirstOrDefault(x => x.Assembly == assembly)?.Servers?.PrimaryKey.GetType() == Type;
    }
}