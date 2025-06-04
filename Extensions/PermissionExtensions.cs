// -----------------------------------------------------------------------
// <copyright file="PermissionExtensions.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.Extensions;

using API.Features;
using System.Linq;

/// <summary>
/// Consists of permission extensions for checking banana plugin based permissions.
/// </summary>
public static class PermissionExtensions
{
    /// <summary>
    /// Checks to see if a player has all the required permissions.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <param name="permissions">The permissions to check for.</param>
    /// <returns>True if the player has all required permissions. False if the player is missing any permissions.</returns>
    public static bool HasRolePermissions(this ExPlayer player, params string[] permissions)
    {
        return player.UserGroup is not null && permissions.All(permission => BananaRole.GroupPermissions[player.UserGroup.Name].Contains(permission));
    }

    /// <summary>
    /// Checks to see if a player has any of the required permissions.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <param name="permissions">The permissions to check for.</param>
    /// <returns>True if the player has any of the required permissions. False if the player is missing all permissions.</returns>
    public static bool HasAnyRolePermission(this ExPlayer player, params string[] permissions)
    {
        return player.UserGroup is not null && permissions.Any(permission => BananaRole.GroupPermissions[player.UserGroup.Name].Contains(permission));
    }

    /// <summary>
    /// Checks to see if a player has a role.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <param name="bananaRole">The <see cref="BananaRole"/> to check for.</param>
    /// <returns>True if the player has the BananaRole. False if the player is missing the BananaRole.</returns>
    public static bool HasBananaRolePermission(this ExPlayer player, BananaRole bananaRole)
    {
        return player.HasRolePermissions(bananaRole.PrimaryRoleNode);
    }

    /// <summary>
    /// Checks to see if a player has a role.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <param name="bananaRole">The <see cref="BananaRole"/> to check for.</param>
    /// <returns>True if the player has the BananaRole. False if the player is missing the BananaRole.</returns>
    public static bool HasBananaRolePermission(this ExPlayer player, Type bananaRole)
    {
        BananaRole? role = BananaPlugin.BananaPlugins.FirstOrDefault(x => x.Roles is not null && x.Roles.Any(y => y.GetType() == bananaRole))?.Roles?.FirstOrDefault(z => z.GetType() == bananaRole);
        if (role is null)
        {
            return false;
        }

        return player.HasRolePermissions(role.PrimaryRoleNode);
    }

    /// <summary>
    /// Checks to see if a player has a role.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <param name="bananaRole">The <see cref="BananaRole"/> to check for.</param>
    /// <returns>True if the player has the BananaRole. False if the player is missing the BananaRole.</returns>
    public static bool HasBananaRolePermission(this ExPlayer player, string bananaRole)
    {
        BananaRole? role = BananaPlugin.BananaPlugins.FirstOrDefault(x => x.Roles is not null && x.Roles.Any(y => y.Name == bananaRole))?.Roles?.FirstOrDefault(z => z.Name == bananaRole);
        if (role is null)
        {
            return false;
        }

        return player.HasRolePermissions(role.PrimaryRoleNode);
    }

    /// <summary>
    /// Checks to see if a player has a role.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <typeparam name="T">The <see cref="BananaRole"/> to check for.</typeparam>
    /// <returns>True if the player has the BananaRole. False if the player is missing the BananaRole.</returns>
    public static bool HasBananaRolePermission<T>(this ExPlayer player)
        where T : BananaRole, new()
    {
        return player.HasBananaRolePermission(typeof(T));
    }

    /*
    /// <summary>
    /// Checks if a player has the specified banana bungalow staff rank.
    /// </summary>
    /// <param name="sender">The command sender.</param>
    /// <param name="rank">The rank to check for.</param>
    /// <param name="response">The error response.</param>
    /// <returns>A value indicating whether the sender has that rank permission.</returns>
    public static bool CheckPermission(this ICommandSender sender, BRank rank, [NotNullWhen(false)] out string? response)
    {
        if (sender is CommandSender cSender && cSender.FullPermissions)
        {
            response = null;
            return true;
        }

        if (rank == 0)
        {
            response = null;
            return true;
        }

        if (rank == BRank.Developer)
        {
            if (sender is not PlayerCommandSender pSender)
            {
                response = "You must be a player to use this command.";
                return false;
            }

            if (!DeveloperUtils.IsDeveloper(pSender.SenderId))
            {
                response = $"You dont have access to this command. Missing rank: Developer";
                return false;
            }

            response = null;
            return true;
        }
        else if (sender is PlayerCommandSender pSender && DeveloperUtils.IsDeveloper(pSender.SenderId))
        {
            response = null;
            return true;
        }

        foreach (string perm in rank.GetPossiblePermissions())
        {
            if (sender.CheckPermission(perm))
            {
                response = null;
                return true;
            }
        }

        response = $"You dont have access to this command. Missing rank: {rank}";
        return false;
    }

    /// <summary>
    /// Gets all possible string permissions for the specified banana bungalow staff rank.
    /// </summary>
    /// <param name="rank">The rank to get string permissions for.</param>
    /// <returns>An enumerable containing the collection of string permissions.</returns>
    public static IEnumerable<string> GetPossiblePermissions(this BRank rank)
    {
        BRank[] values = (BRank[])Enum.GetValues(typeof(BRank));

        // If the rank is below or equal to the
        // listed one, the listed permission will be added.
        //
        // This allows for higher ranked staff to
        // have access to lower rank permissions.
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == 0 || values[i] == BRank.Developer)
            {
                continue;
            }

            if (rank <= values[i])
            {
                yield return GetPermission(values[i]);
            }
        }
    }

    /// <summary>
    /// Gets the exact permission string of the specified rank.
    /// </summary>
    /// <param name="rank">The rank to retrieve.</param>
    /// <returns>A string representing the rank's permission.</returns>
    /// <exception cref="ArgumentOutOfRangeException">ArgumentOutOfRangeException.</exception>
    public static string GetPermission(this BRank rank)
    {
        return rank switch
        {
            BRank.JuniorModerator => "bananaplugin.juniormod",
            BRank.Moderator => "bananaplugin.moderator",
            BRank.SeniorModerator => "bananaplugin.seniormod",
            BRank.JuniorAdministrator => "bananaplugin.jradmin",
            BRank.Administrator => "bananaplugin.admin",
            BRank.HeadAdministrator => "bananaplugin.headadmin",
            BRank.SeniorAdministrator => "bananaplugin.senioradmin",
            _ => throw new ArgumentOutOfRangeException(nameof(rank)),
        };
    }
    */
}
