// -----------------------------------------------------------------------
// <copyright file="InheritBananaRoleAttribute.cs" company="Redforce04">
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
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class InheritBananaRoleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InheritBananaRoleAttribute"/> class.
    /// </summary>
    /// <param name="roleName">The name of the role to inherit.</param>
    public InheritBananaRoleAttribute(string roleName)
    {
        this.RoleName = roleName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InheritBananaRoleAttribute"/> class.
    /// </summary>
    /// <param name="roleType">The type of the <see cref="BananaRole"/> to inherit.</param>
    protected InheritBananaRoleAttribute(Type roleType)
    {
        this.RoleType = roleType;
    }

    /// <summary>
    /// Gets the name of the role to inherit.
    /// </summary>
    public string? RoleName { get; }

    /// <summary>
    /// Gets the type of the role to inherit.
    /// </summary>
    public Type? RoleType { get; }
}