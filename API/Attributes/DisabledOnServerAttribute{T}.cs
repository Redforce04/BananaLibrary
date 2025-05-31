// -----------------------------------------------------------------------
// <copyright file="DisabledOnServerAttribute{T}.cs" company="Redforce04">
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
/// <typeparam name="T">The type of the <see cref="BananaServer"/> to target.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DisabledOnServerAttribute<T> : ServerFeatureTargetAttribute
    where T : BananaServer, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisabledOnServerAttribute{T}"/> class.
    /// </summary>
    public DisabledOnServerAttribute()
        : base(typeof(T), false)
    {
    }
}