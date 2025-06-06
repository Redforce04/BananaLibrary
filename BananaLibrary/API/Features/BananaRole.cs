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

using BananaLibrary.API.Interfaces;
using JetBrains.Annotations;

/// <summary>
/// The base role class used to define any roles.
/// </summary>
// ReSharper disable ArrangeModifiersOrder MemberCanBeProtected.Global
public abstract class BananaRole : IPrefixableItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaRole"/> class.
    /// </summary>
    protected BananaRole()
    {
    }

    /// <inheritdoc />
    public string Prefix => this.Name;

    /// <summary>
    /// Gets a list of all registered group permissions.
    /// </summary>
    internal static Dictionary<string, string[]> GroupPermissions { get; } = new();

    /// <summary>
    /// Gets the UserGroup representing this role.
    /// </summary>
    [PublicAPI]
    protected internal UserGroup? UserGroup { get; private set; }

    /// <summary>
    /// Gets a value indicating the name of the role.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets a value indicating the name of the group in the permission manager.
    /// </summary>
    public abstract string GroupName { get; }

    /// <summary>
    /// Gets a list of role nodes that the role has.
    /// </summary>
    /// <remarks>
    /// Format: $"BananaLibrary.{plugin.Prefix}.Role.{role.Prefix}".
    /// </remarks>
    protected internal List<string> RoleNodes { get; internal set; } = null!;

    /// <summary>
    /// Gets the primary role node for this BananaRole.
    /// </summary>
    /// <remarks>
    /// Format: $"BananaLibrary.{plugin.Prefix}.Role.{role.Prefix}".
    /// </remarks>
    protected internal string PrimaryRoleNode { get; internal set; } = null!;

    /// <summary>
    /// Loads the Banana Roles.
    /// </summary>
    internal static void LoadBananaRoles()
    {
        ExHandlers.ServerEvents.WaitingForPlayers -= LoadBananaRoles;

        foreach (BananaPlugin plugin in BananaPlugin.BananaPlugins)
        {
            if (plugin.Roles is null)
            {
                Log.Debug($"No roles loaded for plugin {plugin.Prefix}.");
                continue;
            }

            foreach (BananaRole bananaRole in plugin.Roles)
            {
                FindUserGroup(bananaRole);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (bananaRole.UserGroup is null)
                {
                    Log.Error($"Could not find User Group \"{bananaRole.GroupName}\" for BananaRole \"{bananaRole.Prefix}\". It will not be registered.");
                    continue;
                }

                if (!GroupPermissions.TryGetValue(bananaRole.GroupName, out string[] existingGroupPerms))
                {
                    GroupPermissions.Add(bananaRole.GroupName, bananaRole.RoleNodes.ToArray());
                    Log.Debug($"Registered BananaRole \"{bananaRole.Prefix}\" ({bananaRole.UserGroup?.Name ?? "Unknown Group"}) with {bananaRole.RoleNodes.Count} permissions. [Plugin: {plugin.Prefix}]");
                    continue;
                }

                GroupPermissions[bananaRole.GroupName] = existingGroupPerms.Union(bananaRole.RoleNodes.ToArray()).ToArray();
                Log.Debug($"Registered BananaRole \"{bananaRole.Prefix}\" ({bananaRole.UserGroup?.Name ?? "Unknown Group"}) with {bananaRole.RoleNodes.Count} permissions. [Plugin: {plugin.Prefix}]");
            }
        }
    }

    /// <summary>
    /// Unloads the Banana Roles.
    /// </summary>
    internal static void UnloadBananaRoles()
    {
        GroupPermissions.Clear();
    }

    /// <summary>
    /// Finds and sets the user group.
    /// </summary>
    /// <param name="role">The BananaRole to update.</param>
    private static void FindUserGroup(BananaRole role)
    {
        try
        {
            role.UserGroup = ServerStatic.PermissionsHandler.GetGroup(role.GroupName);
        }
        catch (Exception)
        {
            // Unused.
        }
    }
}
