// -----------------------------------------------------------------------
// <copyright file="DisabledOnServerAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using Features;

/// <summary>
/// Indicates that a feature should be disabled on a specific server.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DisabledOnServerAttribute : ServerFeatureTargetAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisabledOnServerAttribute"/> class.
    /// </summary>
    /// <param name="type">The type of the <see cref="BananaServer"/> to target.</param>
    public DisabledOnServerAttribute(Type type)
        : base(type, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisabledOnServerAttribute"/> class.
    /// </summary>
    /// <param name="serverId">The <see cref="BananaServer.ServerId"/> of the <see cref="BananaServer"/> to target.</param>
    public DisabledOnServerAttribute(string serverId)
        : base(serverId, false)
    {
    }
}