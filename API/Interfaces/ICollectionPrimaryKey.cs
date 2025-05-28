// -----------------------------------------------------------------------
// <copyright file="ICollectionPrimaryKey.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Interfaces;

/// <summary>
/// Adds a primary instance of an item to a dictionary for caching or other purposes.
/// </summary>
/// <typeparam name="T">The type of the primary key.</typeparam>
public interface ICollectionPrimaryKey<T>
{
    /// <summary>
    /// Gets the primary item of the collection.
    /// </summary>
    public T PrimaryKey { get; }
}