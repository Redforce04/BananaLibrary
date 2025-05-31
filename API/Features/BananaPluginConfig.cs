// -----------------------------------------------------------------------
// <copyright file="BananaPluginConfig.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using System.Collections.Generic;
using System.ComponentModel;
using Attributes;
using Interfaces;

/// <summary>
/// The base config for a banana plugin which manages per-plugin banana information.
/// </summary>
public sealed class BananaPluginConfig : IConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether Banana Library should be loaded.
    /// </summary>
    [Description("A boolean indicator that determines whether Banana Library should be loaded.")]
    [BananaConfig]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether Banana Library should output debug logs.
    /// </summary>
    [Description("A boolean indicator that determines whether Banana Library should output debug logs.")]
    [BananaConfig]
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating the currently active Banana Server.
    /// </summary>
    [Description("A string ID indicator that determines which Banana Server is currently running.")]
    [BananaConfig]
    public string CurrentBananaServerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value the prefix to show on BananaLogs for this plugin.
    /// </summary>
    [Description("A string prefix that indicates the prefix to show on BananaLogs for this plugin.")]
    [BananaConfig]
    public string LoggerPrefix { get; set; } = "BP";
}