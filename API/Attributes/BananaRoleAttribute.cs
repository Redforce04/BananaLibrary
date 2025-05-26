// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaLibrary
//    Project:          BananaLibrary
//    FileName:         DefaultServerValue.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/23/2025 14:57
//    Created Date:     05/23/2025 14:05
// -----------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using Enums;
using Features;

/// <summary>
/// Creates a BananaRole and sets the value of the property.
/// </summary>
#pragma warning disable SA1011 // Closing bracket preceded by space.
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class BananaRoleAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BananaRoleAttribute"/> class.
    /// </summary>
    /// <param name="name">Indicates the name of the role.</param>
    /// <param name="heirarchyId">Indicates where the role stands in importance compared to other roles. The highest role takes priority.</param>
    /// <param name="overrideExistingRole">Indicates whether this role will update permissions of an identically registered role.</param>
    /// <param name="permissionNodes">Indicates the permissions nodes which the role is allowed to execute.</param>
    /// <param name="playerPermissions">Indicates the player permissions which the role is allowed to execute.</param>
    /// <param name="requiredKickPower">Indicates the kick power required to kick a player with this role. The current default is 0.</param>
    /// <param name="kickPower">Indicates the kick power that this role has. The current default is 0.</param>
    /// <param name="displayName">Indicates the Display Name of the role. The current default is the Name property.</param>
    /// <param name="badgeColor">Indicates the display color of the role. The current default is white.</param>
    /// <param name="coverOtherBadges">Indicates whether the role should cover other NorthWood / global roles. The current default is true.</param>
    /// <param name="autoHide">Indicates whether the role should be turned hidden after it is applied. The current default is false.</param>
    /// <remarks>
    ///     If nullable parameters are left null, their value will be inherited from any matching existing roles. If no existing roles are found it will use the default.
    /// </remarks>
    public BananaRoleAttribute(
        string name,
        ushort heirarchyId,
        bool overrideExistingRole,
        string[]? permissionNodes = null,
        PlayerPermissions playerPermissions = 0,
        byte? kickPower = null,
        byte? requiredKickPower = null,
        string? displayName = null,
        RoleColor? badgeColor = null,
        bool? coverOtherBadges = null,
        bool? autoHide = null)
    {
        this.Name = name;
        this.HeirarchyId = heirarchyId;
        this.OverrideExistingRole = overrideExistingRole;
        this.PermissionNodes = permissionNodes ?? [];
        this.PlayerPermissions = playerPermissions;
        this.KickPower = kickPower;
        this.RequiredKickPower = requiredKickPower;
        this.DisplayName = displayName;
        this.BadgeColor = badgeColor;
        this.Cover = coverOtherBadges;
        this.AutoHide = autoHide;
    }

    /// <inheritdoc cref="BananaRole.Name"/>
    internal string Name { get; }

    /// <inheritdoc cref="BananaRole.RoleHierarchyId"/>
    internal ushort HeirarchyId { get; }

    /// <inheritdoc cref="BananaRole.OverrideExistingRole"/>
    internal bool OverrideExistingRole { get; }

    /// <inheritdoc cref="BananaRole.PermissionNodes"/>
    internal string[] PermissionNodes { get; }

    /// <inheritdoc cref="BananaRole.PlayerPermissions"/>
    internal PlayerPermissions PlayerPermissions { get; }

    /// <inheritdoc cref="BananaRole.KickPower"/>
    internal byte? KickPower { get; }

    /// <inheritdoc cref="BananaRole.RequiredKickPower"/>
    internal byte? RequiredKickPower { get; }

    /// <inheritdoc cref="BananaRole.DisplayName"/>
    internal string? DisplayName { get; }

    /// <inheritdoc cref="BananaRole.BadgeColor"/>
    internal RoleColor? BadgeColor { get; }

    /// <inheritdoc cref="BananaRole.Cover"/>
    internal bool? Cover { get; }

    /// <inheritdoc cref="BananaRole.AutoHide"/>
    internal bool? AutoHide { get; }
}
#pragma warning restore SA1011
