// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary;

using System.ComponentModel;
using API.Interfaces;

/// <summary>
/// The main instance of the config.
/// </summary>
public sealed class Config : IConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether Banana Library should be loaded.
    /// </summary>
    [Description("A boolean indicator that determines whether Banana Library should be loaded.")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether Banana Library should output debug logs.
    /// </summary>
    [Description("A boolean indicator that determines whether Banana Library should output debug logs.")]
    public bool Debug { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the currently active Banana Server.
    /// </summary>
    [Description("A string ID indicator that determines which Banana Server is currently running..")]
    public string CurrentBananaServerId { get; set; } = string.Empty;
}