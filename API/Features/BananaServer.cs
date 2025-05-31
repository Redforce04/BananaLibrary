// -----------------------------------------------------------------------
// <copyright file="BananaServer.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Gets the port that the server uses.
    /// </summary>
    public abstract ushort ServerPort { get; }

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
    public string Prefix => this.ServerId;

    /// <summary>
    /// Initializes the server implementation for a plugin.
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown when the plugin doesn't have a valid Server.</exception>
    internal static void LoadBananaServers()
    {
    }

    /// <summary>
    /// Unloads the server implementation for a plugin.
    /// </summary>
    internal static void UnloadBananaServers()
    {
    }
}