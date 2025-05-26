// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaLibrary
//    Project:          BananaLibrary
//    FileName:         BananaAttributeRole.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/24/2025 16:49
//    Created Date:     05/24/2025 16:05
// -----------------------------------------

namespace BananaLibrary.API.Features;

using System.Reflection;
using Attributes;
using Enums;

/// <summary>
/// Utilized to create <see cref="BananaRole">BananaRoles</see> through the usage of attributes.
/// </summary>
// ReSharper disable ConvertToAutoProperty FieldCanBeMadeReadOnly.Local InconsistentNaming ConvertToConstant.Local
internal sealed class BananaAttributeRole : BananaRole
{
    /// <summary>
    /// The property which is accessible to plugins.
    /// </summary>
#pragma warning disable SA1401 // cant be made private because access is needed ?!?!?
    internal readonly PropertyInfo Property;
#pragma warning restore SA1401
    private string name;
    private ushort roleHierarchyId;
    private bool overrideExistingRole;
    private string[] permissionsNodes;
    private PlayerPermissions playerPermissions;
    private byte? requiredKickPower;
    private byte? kickPower;
    private RoleColor? badgeColor;
    private string? displayName;
    private bool? autoHide;
    private bool? cover;

    /// <summary>
    /// Initializes a new instance of the <see cref="BananaAttributeRole"/> class.
    /// </summary>
    /// <param name="attribute">The <see cref="BananaRoleAttribute"/> to create values from.</param>
    /// <param name="property">The property where this instance should coexist so other plugins can access it.</param>
    internal BananaAttributeRole(BananaRoleAttribute attribute, PropertyInfo property)
    {
        this.name = attribute.Name;
        this.roleHierarchyId = attribute.HeirarchyId;
        this.overrideExistingRole = attribute.OverrideExistingRole;
        this.permissionsNodes = attribute.PermissionNodes;
        this.playerPermissions = attribute.PlayerPermissions;
        this.requiredKickPower = attribute.RequiredKickPower;
        this.kickPower = attribute.KickPower;
        this.badgeColor = attribute.BadgeColor;
        this.displayName = attribute.DisplayName;
        this.autoHide = attribute.AutoHide;
        this.cover = attribute.Cover;
        this.Property = property;
    }

    /// <inheritdoc/>
    protected override string Name => this.name;

    /// <inheritdoc/>
    protected override ushort RoleHierarchyId => this.roleHierarchyId;

    /// <inheritdoc/>
    protected override bool OverrideExistingRole => this.overrideExistingRole;

    /// <inheritdoc/>
    protected override string[] PermissionNodes => this.permissionsNodes;

    /// <inheritdoc/>
    protected override byte? RequiredKickPower => this.requiredKickPower;

    /// <inheritdoc/>
    protected override PlayerPermissions PlayerPermissions => this.playerPermissions;

    /// <inheritdoc/>
    protected override RoleColor? BadgeColor => this.badgeColor;

    /// <inheritdoc/>
    protected override string? DisplayName => this.displayName;

    /// <inheritdoc/>
    protected override bool? AutoHide => this.autoHide;

    /// <inheritdoc/>
    protected override byte? KickPower => this.kickPower;

    /// <inheritdoc/>
    protected override bool? Cover => this.cover;
}