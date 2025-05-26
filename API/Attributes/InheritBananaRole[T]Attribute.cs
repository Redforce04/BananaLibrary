// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaLibrary
//    Project:          BananaLibrary
//    FileName:         InheritBananaRole[T]Attribute.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/24/2025 16:47
//    Created Date:     05/24/2025 16:05
// -----------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using Features;

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