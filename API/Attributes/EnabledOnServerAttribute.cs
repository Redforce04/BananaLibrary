// -----------------------------------------------------------------------
// <copyright file="EnabledOnServerAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using Features;

/// <summary>
/// Indicates that a feature should be enabled on a specific server.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EnabledOnServerAttribute : ServerFeatureTargetAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnabledOnServerAttribute"/> class.
    /// </summary>
    /// <param name="type">The type of the <see cref="BananaServer"/> to target.</param>
    public EnabledOnServerAttribute(Type type)
        : base(type, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnabledOnServerAttribute"/> class.
    /// </summary>
    /// <param name="serverId">The <see cref="BananaServer.ServerId"/> of the <see cref="BananaServer"/> to target.</param>
    public EnabledOnServerAttribute(string serverId)
        : base(serverId, true)
    {
    }
}