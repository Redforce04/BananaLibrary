// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaLibrary
//    Project:          BananaLibrary
//    FileName:         InheritBananaRoleAttribute.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/24/2025 16:36
//    Created Date:     05/24/2025 16:05
// -----------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using Features;

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