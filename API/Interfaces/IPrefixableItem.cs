// -----------------------------------------------------------------------
// <copyright file="IPrefixableItem.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Interfaces;

/// <summary>
/// Adds a prefix to an item. Used for collections.
/// </summary>
public interface IPrefixableItem
{
    /// <summary>
    /// Gets the prefix of the item.
    /// </summary>
    string Prefix { get; }
}
