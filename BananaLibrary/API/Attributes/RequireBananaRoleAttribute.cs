// -----------------------------------------------------------------------
// <copyright file="RequireBananaRoleAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using System.Linq;

using AdvancedCommandLibrary.Attributes;
using AdvancedCommandLibrary.Contexts;
using BananaLibrary.API.Features;
using BananaLibrary.API.Utils;
using BananaLibrary.Extensions;
using LabApi.Features.Wrappers;

/// <summary>
/// Used to indicate that a command should not execute if the player does not have the specified BananaRole.
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class RequireBananaRoleAttribute : RequirePermissionsAttribute
{
    private BananaRole? requiredRole;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBananaRoleAttribute"/> class.
    /// </summary>
    /// <param name="type">The type of the required BananaRole.</param>
    public RequireBananaRoleAttribute(Type type)
    {
        this.Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBananaRoleAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the required BananaRole Type.</param>
    public RequireBananaRoleAttribute(string name)
    {
        this.Name = name;
    }

    /// <summary>
    /// Gets the name of the required <see cref="BananaRole"/>.
    /// </summary>
    internal string? Name { get; }

    /// <summary>
    /// Gets the type of the required <see cref="BananaRole"/>.
    /// </summary>
    internal Type? Type { get; }

    private BananaRole? RequiredRole
    {
        get
        {
            if (this.requiredRole is null)
            {
                this.GetBananaRole();
            }

            return this.requiredRole;
        }

        // ReSharper disable once UnusedMember.Local
        set
        {
            this.requiredRole = value;
        }
    }

    /// <summary>
    /// Checks to see if the sender has the required permissions.
    /// </summary>
    /// <param name="args">The context arguments.</param>
    public override void CheckPermissions(ProcessPermissionsArgs args)
    {
        if (!args.IsAllowed)
        {
            return;
        }

        args.Response = "Not Allowed";
        if (this.RequiredRole is null)
        {
            args.IsAllowed = false;
            args.Response = "Not allowed due to a plugin error.";
            Log.Warn($"Role \"{this.Name ?? this.Type?.GetTypeString() ?? "Unknown"}\" could not be found.");
            return;
        }

        // Console Sender.
        if (args.Sender is ServerConsoleSender)
        {
            args.IsAllowed = true;
            args.Response = "Server Console Allowed.";
            return;
        }

        if (Player.Get(args.Sender) is not { } ply)
        {
            args.Response = "An error has occured.";
            args.IsAllowed = false;
            return;
        }

        args.Response = "You have permissions to execute this command.";
        args.IsAllowed = ply.HasBananaRolePermission(this.RequiredRole);
    }

    private void GetBananaRole()
    {
        foreach (BananaPlugin plugin in BananaPlugin.BananaPlugins)
        {
            if (plugin.Roles is null)
            {
                continue;
            }

            if (this.Type is not null)
            {
                this.requiredRole = plugin.Roles.FirstOrDefault(x => x.GetType() == this.Type);
            }

            if (this.requiredRole is null && this.Name is not null)
            {
                this.requiredRole = plugin.Roles.FirstOrDefault(x => x.Name == this.Name);
            }

            if (this.requiredRole is not null)
            {
                return;
            }
        }
    }
}