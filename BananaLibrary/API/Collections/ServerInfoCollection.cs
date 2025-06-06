// -----------------------------------------------------------------------
// <copyright file="ServerInfoCollection.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Collections;

using System.Collections.Generic;

using BananaLibrary.API.Features;
using BananaLibrary.API.Interfaces;

/// <summary>
/// Used to contain all <see cref="BananaServer"/> instances for a <see cref="BananaPlugin"/>.
/// </summary>
// ReSharper disable UnusedParameter.Local
public sealed class ServerInfoCollection : Collection<BananaServer>, ICollectionPrimaryKey<BananaServer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerInfoCollection"/> class.
    /// </summary>
    /// <param name="primaryKey">The primary server.</param>
    /// <param name="servers">The servers to add.</param>
    public ServerInfoCollection(BananaServer primaryKey, List<BananaServer> servers)
    {
        foreach (BananaServer server in servers)
        {
            if (server is null)
            {
                continue;
            }

            this.TryAddItem(server, out _);
        }

        this.PrimaryKey = primaryKey;
    }

    /// <inheritdoc />
    public BananaServer PrimaryKey { get; }
}
