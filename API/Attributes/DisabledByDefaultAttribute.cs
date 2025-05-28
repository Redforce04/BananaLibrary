// -----------------------------------------------------------------------
// <copyright file="DisabledByDefaultAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;

/// <summary>
/// Indicates that a feature should be disabled by default.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DisabledByDefaultAttribute : ServerFeatureTargetAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisabledByDefaultAttribute"/> class.
    /// </summary>
    public DisabledByDefaultAttribute()
        : base(false)
    {
    }
}