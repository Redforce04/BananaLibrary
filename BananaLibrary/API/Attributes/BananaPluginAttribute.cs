// -----------------------------------------------------------------------
// <copyright file="BananaPluginAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;

/// <summary>
/// Used to set default BananaPlugin Configs for a server if they are not yet generated. Use on the main plugin class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class BananaPluginAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaPluginAttribute"/> class.
    /// </summary>
    /// <param name="isEnabled">Indicates whether this BananaPlugin should be enabled.</param>
    /// <param name="debug">Indicates whether debug logs should be shown on the console.</param>
    /// <param name="currentBananaServerId">Indicates the current BananaServerId for this plugin.</param>
    /// <param name="loggerPrefix">Indicates the logger prefix for this plugin.</param>
    public BananaPluginAttribute(bool isEnabled = true, bool debug = false, string currentBananaServerId = "", string loggerPrefix = "BP")
    {
        this.IsEnabled = isEnabled;
        this.Debug = debug;
        this.CurrentBananaServerId = currentBananaServerId;
        this.LoggerPrefix = loggerPrefix;
    }

    /// <summary>
    /// Gets a value indicating whether this BananaPlugin should be enabled.
    /// </summary>
    internal bool IsEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether debug logs should be shown on the console.
    /// </summary>
    internal bool Debug { get; }

    /// <summary>
    /// Gets a value indicating the current BananaServerId for this plugin.
    /// </summary>
    internal string CurrentBananaServerId { get; }

    /// <summary>
    /// Gets a value indicating the logger prefix for this plugin.
    /// </summary>
    internal string LoggerPrefix { get; }
}