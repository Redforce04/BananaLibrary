// -----------------------------------------------------------------------
// <copyright file="BananaPluginCollection.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Collections;

using System.Collections.Generic;
using Features;

/// <summary>
/// Used to contain all <see cref="BananaPlugin"/> instances.
/// </summary>
public sealed class BananaPluginCollection : Collection<BananaPlugin>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaPluginCollection"/> class.
    /// </summary>
    /// <param name="plugins">The plugins to add.</param>
    public BananaPluginCollection(List<BananaPlugin> plugins)
    {
        foreach (BananaPlugin plugin in plugins)
        {
            if (plugin is null)
            {
                continue;
            }

            this.TryAddItem(plugin, out _);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BananaPluginCollection"/> class.
    /// </summary>
    public BananaPluginCollection()
    {
    }
}