// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaLibrary
//    Project:          BananaLibrary
//    FileName:         DefaultServerValue.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/23/2025 14:57
//    Created Date:     05/23/2025 14:05
// -----------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using Features;

/// <summary>
/// Sets a default value for a configuration based on the current server.
/// </summary>
/// <typeparam name="T">The <see cref="BananaServer"/> Instance to target for the config.</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class DefaultServerValueAttribute<T> : Attribute
    where T : BananaServer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultServerValueAttribute{T}"/> class.
    /// </summary>
    /// <param name="defaultValue">The default value of the configuration object.</param>
    public DefaultServerValueAttribute(object defaultValue)
    {
        this.DefaultValue = defaultValue;
    }

    /// <summary>
    /// Gets the default value of the configuration option for the provided server.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public object DefaultValue { get; }

    /// <summary>
    /// Checks to see if the server option for this config is the current running server instance.
    /// </summary>
    /// <returns>A value indicating whether the specified server is the current running server instance.</returns>
    internal bool IsCurrentServer()
    {
        return BananaServer.Servers?.PrimaryKey is T;
    }
}