// -----------------------------------------------------------------------
// <copyright file="BananaFeature{IConfig}.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using System.Diagnostics.CodeAnalysis;
using BananaLibrary;
using Interfaces;

/// <summary>
/// The main feature implementation allowing for config usage.
/// </summary>
/// <typeparam name="T">The config type to use.</typeparam>
public abstract class BananaFeature<T> : BananaFeature, IConfigLoader
    where T : class, IConfig, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaFeature{T}"/> class.
    /// </summary>
    protected BananaFeature()
    {
        this.LocalConfig = null!;
        /* if (!base.Plugin.AssertEnabled())
        {
            throw new System.InvalidOperationException();
        }

        this.OnConfigUpdated(Plugin.Instance.Config);
        Config.FeatureConfigUpdated += this.OnConfigUpdated;*/
    }

    /// <summary>
    /// Gets the local feature config.
    /// </summary>
    public T LocalConfig { get; private set; }

    /// <inheritdoc />
    IConfig IConfigLoader.Config
    {
        get => this.LocalConfig;
        set => this.LocalConfig = (T)value;
    }

    /// <inheritdoc />
    void IConfigLoader.LoadConfig()
    {
    }

    /// <summary>
    /// Called when updating the local feature config.
    /// </summary>
    /// <param name="config">The config to apply.</param>
    [MemberNotNull(nameof(LocalConfig))]
    internal void OnConfigUpdated(T config)
    {
        this.ConfigUpdated(config);
        this.LocalConfig = config;
    }

    /// <summary>
    /// Called right before the config is updated.
    /// </summary>
    /// <param name="newConfig">The new configuration.</param>
    protected virtual void ConfigUpdated(T newConfig)
    {
    }
}
