// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaPlugin
//    Project:          BananaPlugin
//    FileName:         IServerInstancePlugin.cs
//    Author:           Redforce04#4091
//    Revision Date:    11/08/2023 2:32 PM
//    Created Date:     11/08/2023 2:32 PM
// -----------------------------------------

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
    /// Gets the prefix for the server.
    /// </summary>
    public abstract string Prefix { get; }
}