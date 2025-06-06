// -----------------------------------------------------------------------
// <copyright file="IServerInfo.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Interfaces;

/// <summary>
/// Used to define settings relevant to the server instancing system.
/// </summary>
public interface IServerInfo
{
    /*/// <summary>
    /// Gets the method of indexing that plugin authors can use to find the <see cref="BananaServer"/> profile that this server instance is.
    /// </summary>
    public SearchIndex SearchMethod { get; }*/

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
}