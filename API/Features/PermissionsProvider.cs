// -----------------------------------------------------------------------
// <copyright file="PermissionsProvider.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using System.Linq;
using LabApi.Features.Permissions;

/// <summary>
/// The default permissions provider.
/// </summary>
public sealed class PermissionsProvider : IPermissionsProvider
{
    /// <inheritdoc/>
    public string[] GetPermissions(ExPlayer player)
    {
        return player.UserGroup is null ? [] : BananaRole.GroupPermissions[player.UserGroup.Name];
    }

    /// <inheritdoc/>
    public bool HasPermissions(ExPlayer player, params string[] permissions)
    {
        return player.UserGroup is not null && permissions.All(permission => BananaRole.GroupPermissions[player.UserGroup.Name].Contains(permission));
    }

    /// <inheritdoc/>
    public bool HasAnyPermission(ExPlayer player, params string[] permissions)
    {
        return player.UserGroup is not null && permissions.Any(permission => BananaRole.GroupPermissions[player.UserGroup.Name].Contains(permission));
    }

    /// <inheritdoc/>
    public void AddPermissions(ExPlayer player, params string[] permissions)
    {
        Log.Warn($"Cannot add BananaRole Permissions.");
    }

    /// <inheritdoc/>
    public void RemovePermissions(ExPlayer player, params string[] permissions)
    {
        Log.Warn($"Cannot remove BananaRole Permissions.");
    }

    /// <inheritdoc/>
    public void ReloadPermissions()
    {
        Log.Warn($"Cannot reload BananaRole Permissions.");
    }

    /// <summary>
    /// Loads the permission provider.
    /// </summary>
    internal static void LoadPermissionProvider()
    {
        PermissionsManager.RegisterProvider<PermissionsProvider>();
    }

    /// <summary>
    /// Unloads the permission provider.
    /// </summary>
    internal static void UnloadPermissionProvider()
    {
        PermissionsManager.UnregisterProvider<PermissionsProvider>();
    }
}
