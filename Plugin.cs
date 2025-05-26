// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaPlugin
//    Project:          BananaPlugin
//    FileName:         Plugin.cs
//    Author:           Redforce04#4091
//    Revision Date:    11/09/2023 10:48 AM
//    Created Date:     11/09/2023 10:48 AM
// -----------------------------------------

namespace BananaLibrary;

using System;
using BananaPlugin.API.Utils;
using LabApi.Features;
using LabApi.Loader.Features.Plugins.Enums;

/// <summary>
/// The main plugin for loading the banana api interface.
/// </summary>
// ReSharper disable ClassNeverInstantiated.Global
public sealed class Plugin : LabApi.Loader.Features.Plugins.Plugin<Config>
{
    /// <summary>
    /// Gets the primary instance of the Banana Library Plugin. This will be null if the plugin is not currently loaded.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc/>
    public override string Name => "BananaFramework";

    /// <inheritdoc/>
    public override string Description => "An API for modularized server features.";

    /// <inheritdoc/>
    public override string Author => "O5Zereth and Redforce04";

    /// <inheritdoc/>
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

    /// <inheritdoc/>
    public override LoadPriority Priority => LoadPriority.Medium;

    /// <inheritdoc/>
    public override Version Version => Version.Parse("1.0.0");

    /// <inheritdoc/>
    public override void Enable()
    {
        Loader.Loader.Initialize();
        BPLogger.Debug($"Loaded BananaFramework v{this.Version}");
    }

    /// <inheritdoc/>
    public override void Disable()
    {
        Loader.Loader.Unload();
    }
}