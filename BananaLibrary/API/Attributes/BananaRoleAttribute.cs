// -----------------------------------------------------------------------
// <copyright file="BananaRoleAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;

/// <summary>
/// Used to autogenerate BananaRoles.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class BananaRoleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaRoleAttribute"/> class.
    /// </summary>
    /// <param name="roleName">The name of the role.</param>
    /// <param name="groupName">The group name for permissions.</param>
    public BananaRoleAttribute(string roleName, string groupName)
    {
        this.RoleName = roleName;
        this.GroupName = groupName;
    }

    /// <summary>
    /// Gets the name of the role.
    /// </summary>
    public string RoleName { get; init; }

    /// <summary>
    /// Gets the name of the group.
    /// </summary>
    public string GroupName { get; init; }
}