// -----------------------------------------------------------------------
// <copyright file="InheritBananaRoleAttribute{T}.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;

using BananaLibrary.API.Features;

/// <summary>
/// Indicates that the provided <see cref="BananaRole" /> should inherit the player permissions, permission nodes, kick power, and required kick power of another role.
/// </summary>
/// <typeparam name="T">The type of <see cref="BananaRole"/> that should be inherited.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public sealed class InheritBananaRoleAttribute<T> : InheritBananaRoleAttribute
    where T : BananaRole, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InheritBananaRoleAttribute{T}"/> class.
    /// </summary>
    public InheritBananaRoleAttribute()
        : base(typeof(T))
    {
    }
}