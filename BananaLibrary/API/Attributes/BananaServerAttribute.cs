// -----------------------------------------------------------------------
// <copyright file="BananaServerAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;

using Features;

/// <summary>
/// Used to autogenerate BananaServers.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BananaServerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaServerAttribute"/> class.
    /// </summary>
    /// <param name="serverName">The name of the server.</param>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="serverPort">The port of the server.</param>
    public BananaServerAttribute(string serverName, string serverId, ushort serverPort)
    {
        this.ServerName = serverName;
        this.ServerId = serverId;
        this.ServerPort = serverPort;
    }

    /// <summary>
    /// Gets the <see cref="BananaServer.ServerName"/> of the server.
    /// </summary>
    public string ServerName { get; }

    /// <summary>
    /// Gets the <see cref="BananaServer.ServerId"/> of the server.
    /// </summary>
    public string ServerId { get; }

    /// <summary>
    /// Gets the <see cref="BananaServer.ServerPort"/> of the server.
    /// </summary>
    public ushort ServerPort { get; }
}