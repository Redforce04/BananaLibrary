// -----------------------------------------------------------------------
// <copyright file="FeatureCollection.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Collections;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Features;

/// <summary>
/// Used to contain all <see cref="BananaFeature"/> for all plugins.
/// </summary>
public sealed class FeatureCollection : Collection<BananaFeature>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureCollection"/> class.
    /// </summary>
    /// <param name="features">The features to be added.</param>
    public FeatureCollection(List<BananaFeature> features)
    {
        foreach (BananaFeature feature in features)
        {
            this.TryAddItem(feature, out _);
        }
    }

    /// <inheritdoc cref="TryGetFeature"/>
    public new BananaFeature this[string prefix]
    {
        get
        {
            if (!this.TryGetFeature(prefix, out BananaFeature? result))
            {
                throw new ArgumentOutOfRangeException($"Feature {prefix} does not exist, and cannot be retrieved.");
            }

            return result;
        }
    }

    /// <summary>
    /// Attempts to get a feature by its prefix.
    /// </summary>
    /// <param name="prefix">The prefix to find.</param>
    /// <param name="feature">The feature, if found.</param>
    /// <returns>A value indicating whether the operation was a success.</returns>
    public bool TryGetFeature(string prefix, [NotNullWhen(true)] out BananaFeature? feature) =>
        this.TryGetItem(prefix, out feature);
}
