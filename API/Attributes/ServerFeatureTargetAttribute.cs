// -----------------------------------------------------------------------
// <copyright file="ServerFeatureTargetAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using System.Linq;
using Features;

/// <summary>
/// Used to indicate whether features should be default enabled or disabled or to target a server.
/// </summary>
// ReSharper disable InconsistentNaming
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ServerFeatureTargetAttribute : Attribute
{
    private Type? type;
    private string? serverId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerFeatureTargetAttribute"/> class.
    /// </summary>
    /// <param name="type">The type of the <see cref="BananaServer"/> to target.</param>
    /// <param name="defaultEnabledForServer">Indicates whether the default should be enabled or disabled for the server.</param>
    internal ServerFeatureTargetAttribute(Type type, bool defaultEnabledForServer)
    {
        this.type = type;
        this.DefaultEnabledForServer = defaultEnabledForServer;
        this.TargetsServer = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerFeatureTargetAttribute"/> class.
    /// </summary>
    /// <param name="serverId">The <see cref="BananaServer.ServerId"/> of the <see cref="BananaServer"/> to target.</param>
    /// <param name="defaultEnabledForServer">Indicates whether the default should be enabled or disabled for the server.</param>
    internal ServerFeatureTargetAttribute(string serverId, bool defaultEnabledForServer)
    {
        this.serverId = serverId;
        this.DefaultEnabledForServer = defaultEnabledForServer;
        this.TargetsServer = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerFeatureTargetAttribute"/> class.
    /// </summary>
    /// <param name="defaultEnabledForServer">Indicates whether the default should be enabled or disabled for the server.</param>
    internal ServerFeatureTargetAttribute(bool defaultEnabledForServer)
    {
        this.DefaultEnabledForServer = defaultEnabledForServer;
        this.TargetsServer = false;
    }

    /// <summary>
    /// Gets a value indicating whether this attribute is intended to target a specific server.
    /// </summary>
    internal bool TargetsServer { get; }

    /// <summary>
    /// Gets a value indicating whether the feature should be enabled or disabled by default.
    /// </summary>
    internal bool DefaultEnabledForServer { get; }

    /// <summary>
    /// Gets the targeted server.
    /// </summary>
    /// <param name="plugin">The BananaPlugin of the Feature target.</param>
    /// <returns>The server being targeted.</returns>
    internal BananaServer? GetServer(BananaPlugin plugin)
    {
        if (!this.TargetsServer)
        {
            Log.Warn($"Feature flag does not target a server. Calling GetServer() will be null (this flag indicates a global feature state)");
            return null;
        }

        BananaServer? server = null;
        if (plugin.Servers is null)
        {
            Log.Warn($"Plugin {plugin.Prefix} does not have a server collection.");
            return null;
        }

        if (this.type is not null)
        {
            server = plugin.Servers.FirstOrDefault(x => x.GetType() == this.type);
        }

        if (server is null && this.serverId is not null)
        {
            server = plugin.Servers.FirstOrDefault(x => x.ServerId == this.serverId);
        }

        if (server is null)
        {
            Log.Warn($"Could not find server with{(this.type is null ? string.Empty : $" [Type => {this.type.Name}]")}{(this.serverId is null ? string.Empty : $" [ServerId => {serverId}]")}");
        }

        return server;
    }
}