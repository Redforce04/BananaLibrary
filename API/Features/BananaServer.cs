// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaPlugin
//    Project:          BananaPlugin
//    FileName:         ServerInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    11/05/2023 2:53 PM
//    Created Date:     11/05/2023 2:53 PM
// -----------------------------------------

namespace BananaLibrary.API.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BananaPlugin.API.Utils;
using Collections;
using Interfaces;

/// <summary>
/// Defines an implementation of a single SL Server Instance.
/// </summary>
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
public abstract class BananaServer : IServerInfo, IPrefixableItem
{
    /// <summary>
    /// Gets a <see cref="ServerInfoCollection"/>.
    /// </summary>
    public static ServerInfoCollection? Servers { get; private set; }

    /// <summary>
    /// Gets the port that the server uses.
    /// </summary>
    public abstract int ServerPort { get; }

    /// <summary>
    /// Gets the name of the server.
    /// </summary>
    public abstract string ServerName { get; }

    /// <summary>
    /// Gets this can be defined via the config of the server.
    /// </summary>
    public abstract string ServerId { get; }

    /// <summary>
    /// Gets a value indicating the prefix for this server.
    /// </summary>
    public abstract string Prefix { get; }

    /// <summary>
    /// Initializes the server implementation for a plugin.
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown when the plugin doesn't have a valid Server.</exception>
    internal static void LoadBananaServers()
    {
        BPLogger.Debug($"Loading defined Banana Servers.");
        List<BananaServer> bananaServers = GetServerInfoInstances();
        BananaServer? primaryKey = bananaServers.FirstOrDefault(b => b.ServerId == Plugin.Instance!.Config!.CurrentBananaServerId);

        if (primaryKey is null)
        {
            Log.Error("Could not find a valid server profile, for this server. Try setting the Server Id.");
            throw new NullReferenceException("Could not find a valid server profile, for this server. Try setting the Server Id.");
        }

        Servers = new ServerInfoCollection(primaryKey, bananaServers);
        Servers.MarkAsLoaded();
    }

    /// <summary>
    /// Unloads the server implementation for a plugin.
    /// </summary>
    internal static void UnloadBananaServers()
    {
        BPLogger.Debug($"Unloading Banana Servers.");
        Servers = null;
    }

    private static List<BananaServer> GetServerInfoInstances()
    {
        List<BananaServer> serverInfos = new();

        foreach (Assembly pluginAssembly in LabApi.Loader.PluginLoader.Plugins.Values)
        {
            Type[] types = pluginAssembly.GetTypes();
            foreach (Type type in types)
            {
                if (!type.IsSubclassOf(typeof(BananaServer)) || type.IsAbstract)
                {
                    continue;
                }

                if (type.GetCustomAttribute<ObsoleteAttribute>() is not null)
                {
                    continue;
                }

                BananaServer info = (BananaServer)Activator.CreateInstance(type, nonPublic: true);
                serverInfos.Add(info);
                BPLogger.Debug($"Found Server Info {info.ServerName} [{info.ServerPort}] - {info.ServerId}");
            }
        }

        return serverInfos;
    }
}