// -----------------------------------------------------------------------
// <copyright file="BananaRole.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Attributes;
using Collections;
using Enums;
using Extensions;
using Interfaces;

/// <summary>
/// The base role class used to define any roles.
/// </summary>
public abstract class BananaRole : IPrefixableItem
{
    // ReSharper disable InconsistentNaming
    private PlayerPermissions realPlayerPermissions;
    private byte? realKickPower;
    private byte? realRequiredKickPower;
    private List<InheritBananaRoleAttribute> inheritedRoleAttributes = new();

    /// <summary>
    /// Gets a <see cref="RoleCollection"/> containing all registered <see cref="BananaRole">BananaRoles</see>.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public static RoleCollection? BananaRoles { get; private set; }

    /// <inheritdoc />
    public string Prefix => this.Name;

    /// <summary>
    /// Gets a value indicating the name of the role.
    /// </summary>
    protected abstract string Name { get; }

    /// <summary>
    /// Gets a value indicating where the role stands in importance compared to other roles. The highest role takes priority.
    /// </summary>
    protected abstract ushort RoleHierarchyId { get; }

    /// <summary>
    /// Gets a value indicating whether this role will update permissions of an identically registered role.
    /// </summary>
    protected abstract bool OverrideExistingRole { get; }

    /// <summary>
    /// Gets a value indicating the permissions nodes which the role is allowed to execute.
    /// </summary>
    /// <remarks>
    /// Default is empty.
    /// </remarks>
    protected virtual string[] PermissionNodes => [];

    /// <summary>
    /// Gets a value indicating the player permissions which the role is allowed to execute.
    /// </summary>
    /// <remarks>
    /// Default is no permissions.
    /// </remarks>
    protected virtual PlayerPermissions PlayerPermissions => 0;

    /// <summary>
    /// Gets a value indicating the Display Name of the role.
    /// </summary>
    /// <remarks>
    /// If left null, the display name will inherit whatever existing role information can be found. If left null, the display name will be inherited from any matching existing roles. If no existing roles are found it will use the default. The current default is <see cref="Name">BananaRole.Name</see>.
    /// </remarks>
    protected virtual string? DisplayName => null;

    /// <summary>
    /// Gets a value indicating the display color of the role.
    /// </summary>
    /// <remarks>
    /// If left null, the display name will inherit whatever existing role information can be found. If left null, the badge color will be inherited from any matching existing roles. If no existing roles are found it will use the default. The current default is <see cref="RoleColor.White"/>.
    /// </remarks>
    protected virtual RoleColor? BadgeColor => null;

    /// <summary>
    /// Gets a value indicating the kick power that this role has. If left null, the kick power will be inherited from any matching existing roles. If no existing roles are found it will use the default. The current default is 0.
    /// </summary>
    protected virtual byte? KickPower => null;

    /// <summary>
    /// Gets a value indicating the kick power required to kick a player with this role. If left null, the required kick power will be inherited from any matching existing roles. If no existing roles are found it will use the default. The current default is 0.
    /// </summary>
    protected virtual byte? RequiredKickPower => null;

    /// <summary>
    /// Gets a value indicating whether the role should cover other NorthWood / global roles. If left null, the cover value will be inherited from any matching existing roles. If no existing roles are found it will use the default. The current default is true.
    /// </summary>
    protected virtual bool? Cover => null;

    /// <summary>
    /// Gets a value indicating whether the role should be turned hidden after it is applied. If left null, the auto hide value will be inherited from any matching existing roles. If no existing roles are found it will use the default. The current default is false.
    /// </summary>
    protected virtual bool? AutoHide => null;

    private static PermissionsHandler Handler => ServerStatic.PermissionsHandler;

    /// <summary>
    /// Gets or sets the game's UserGroup generated from this role.
    /// </summary>
    private UserGroup? UserGroup { get; set; }

    private List<BananaRole> InheritedRoles { get; set; } = new();

    /// <summary>
    /// Loads the Banana Roles.
    /// </summary>
    internal static void LoadBananaRoles()
    {
        Log.Debug($"Loading defined Banana Roles.");
        List<BananaRole> roles = GetBananaRoles();
        if (roles.Count == 0)
        {
            Log.Debug($"No BananaRoles found. BananaRoles will not be loaded.");
            return;
        }

        BananaRoles = new RoleCollection(roles);
        BananaRoles.MarkAsLoaded();
        foreach (BananaRole role in BananaRoles.ToList())
        {
            if (role is not BananaAttributeRole attributeRole)
            {
                continue;
            }

            attributeRole.Property.SetMethod.Invoke(null, [BananaRoles[role.Name]]);
        }
    }

    /// <summary>
    /// Unloads the Banana Roles.
    /// </summary>
    internal static void UnloadBananaRoles()
    {
        Log.Debug($"Unloading Banana Roles.");
        BananaRoles = null;
    }

    private static List<BananaRole> GetBananaRoles()
    {
        Dictionary<string, BananaRole> roles = new();

        foreach (Assembly pluginAssembly in LabApi.Loader.PluginLoader.Plugins.Values)
        {
            Type[] types = pluginAssembly.GetTypes();
            foreach (Type type in types)
            {
                // Used to make instances of roles via attributes.
                if (type == typeof(BananaAttributeRole))
                {
                    continue;
                }

                if (!type.IsSubclassOf(typeof(BananaRole)) || type.IsAbstract)
                {
                    goto searchForRolesViaAttributes;
                }

                if (type.GetCustomAttribute<ObsoleteAttribute>() is not null)
                {
                    continue;
                }

                {
                    BananaRole role = (BananaRole)Activator.CreateInstance(type, nonPublic: true);
                    role.realPlayerPermissions = role.PlayerPermissions;
                    role.realKickPower = role.KickPower ?? 0;
                    role.realRequiredKickPower = role.RequiredKickPower ?? 0;
                    role.inheritedRoleAttributes = Attribute.GetCustomAttributes(type, typeof(InheritBananaRoleAttribute)).Cast<InheritBananaRoleAttribute>().ToList();
                    roles.Add(role.Name, role);
                    Log.Debug($"Found Banana Role {role.Name} [{role.RoleHierarchyId}] - (Kick Powers: {role.KickPower}/{role.RequiredKickPower})");
                    continue;
                }

                searchForRolesViaAttributes:
                {
                    foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (Attribute.GetCustomAttribute(property, typeof(BananaRoleAttribute)) is not BananaRoleAttribute roleAttribute)
                        {
                            continue;
                        }

                        if (property.SetMethod is null || !property.SetMethod.IsPublic)
                        {
                            continue;
                        }

                        BananaAttributeRole role = new(roleAttribute, property)
                        {
                            inheritedRoleAttributes = Attribute.GetCustomAttributes(type, typeof(InheritBananaRoleAttribute)).Cast<InheritBananaRoleAttribute>().ToList(),
                        };
                        roles.Add(role.Name, role);
                    }
                }
            }
        }

        foreach (BananaRole role in roles.Values.ToList())
        {
            List<BananaRole> rolesToInherit = new();
            foreach (InheritBananaRoleAttribute? attr in role.inheritedRoleAttributes)
            {
                if (attr.RoleName is not null && roles.Values.FirstOrDefault(x => x.Name == attr.RoleName) is { } existingRole)
                {
                    rolesToInherit.Add(existingRole);
                    continue;
                }

                if (attr.RoleType is not null && roles.Values.FirstOrDefault(x => x.GetType() == attr.RoleType) is { } existingRoleType)
                {
                    rolesToInherit.Add(existingRoleType);
                }
            }

            roles[role.Name].InheritedRoles = rolesToInherit;
        }

        foreach (BananaRole role in roles.Values.OrderBy(x => x.RoleHierarchyId).ToList())
        {
            if (role.InheritedRoles.Count <= 0)
            {
                continue;
            }

            foreach (BananaRole inheritedRole in role.InheritedRoles)
            {
                role.realRequiredKickPower = inheritedRole.realRequiredKickPower > role.realRequiredKickPower ? inheritedRole.realRequiredKickPower : role.realRequiredKickPower;
                role.realKickPower = inheritedRole.realKickPower > role.realKickPower ? inheritedRole.realKickPower : role.realKickPower;
                role.realPlayerPermissions = role.PlayerPermissions.Include(inheritedRole.PlayerPermissions);

                inheritedRole.PermissionNodes.ForEach(x =>
                {
                    if (!role.PermissionNodes.Contains(x))
                    {
                        role.PermissionNodes[role.PermissionNodes.Length] = x;
                    }
                });
            }

            roles[role.Name] = role;
        }

        foreach (BananaRole role in roles.Values.ToList())
        {
            role.CreateOrUpdateExistingRole();
        }

        return roles.Values.ToList();
    }

    private void CreateOrUpdateExistingRole()
    {
        KeyValuePair<string, UserGroup> existingRole = Handler.Groups.FirstOrDefault(role => role.Value.Name == this.Name);
        if (existingRole.Value is null)
        {
            this.UserGroup = new()
            {
                Permissions = (ulong)this.realPlayerPermissions, BadgeColor = (this.BadgeColor ?? RoleColor.White).ToString().ToLower(), BadgeText = this.DisplayName ?? this.Name, Cover = this.Cover ?? true,
                HiddenByDefault = this.AutoHide ?? false, KickPower = this.realKickPower ?? 0, RequiredKickPower = this.realRequiredKickPower ?? 0, Name = this.Name,
            };
            Handler.Groups.Add(this.Name, this.UserGroup);
            return;
        }

        // Cannot Change since it is the key value and must already be identical - existingRole.Value.Name = this.Name;
        // I don't know what this does. I assume it has something to do with global vs local server configs. - existingRole.Value.Shared = ;
        existingRole.Value.Permissions = (ulong)((PlayerPermissions)existingRole.Value.Permissions).Include(this.PlayerPermissions);
        existingRole.Value.BadgeColor = this.BadgeColor.HasValue ? this.BadgeColor.Value.ToString().ToLower() : existingRole.Value.BadgeColor;
        existingRole.Value.BadgeText = this.DisplayName ?? existingRole.Value.BadgeText;
        existingRole.Value.Cover = this.Cover ?? existingRole.Value.Cover;
        existingRole.Value.HiddenByDefault = this.AutoHide ?? existingRole.Value.HiddenByDefault;
        existingRole.Value.KickPower = this.realKickPower ?? existingRole.Value.KickPower;
        existingRole.Value.Permissions = (ulong)((PlayerPermissions)existingRole.Value.Permissions).Include(this.realPlayerPermissions);
        existingRole.Value.RequiredKickPower = this.realRequiredKickPower ?? existingRole.Value.RequiredKickPower;
        Handler.Groups[existingRole.Key] = existingRole.Value;
        this.UserGroup = Handler.Groups[existingRole.Key];
    }
}
