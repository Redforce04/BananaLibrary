// -----------------------------------------------------------------------
// <copyright file="BananaConfigDefaultAttribute{T}.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;

using BananaLibrary.API.Features;

/// <summary>
/// Sets a default value for a configuration based on the current server.
/// </summary>
/// <typeparam name="T">The <see cref="BananaServer"/> Instance to target for the config.</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class BananaConfigDefaultAttribute<T> : BananaConfigDefaultAttribute
    where T : BananaServer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaConfigDefaultAttribute{T}"/> class.
    /// </summary>
    /// <param name="defaultValue">The default value of the configuration object.</param>
    public BananaConfigDefaultAttribute(object defaultValue)
        : base(defaultValue, typeof(T))
    {
    }
}