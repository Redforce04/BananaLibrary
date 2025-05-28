// -----------------------------------------------------------------------
// <copyright file="IConfigLoader.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Interfaces;

/// <summary>
/// Helps manage config loading and management.
/// </summary>
public interface IConfigLoader
{
    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    IConfig Config { get; set; }

    /// <summary>
    /// Called when loading the config.
    /// </summary>
    void LoadConfig();
}