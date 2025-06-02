// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary;

using System;
using API.Utils;
using LabApi.Features;
using LabApi.Loader.Features.Plugins.Enums;

/// <summary>
/// The main plugin for loading the banana api interface.
/// </summary>
// ReSharper disable ClassNeverInstantiated.Global
public sealed class Plugin : LabApi.Loader.Features.Plugins.Plugin<Config>
{
    static Plugin()
    {
        CosturaUtility.Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    public Plugin()
    {
    }

    /// <summary>
    /// Gets the primary instance of the Banana Library Plugin. This will be null if the plugin is not currently loaded.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc/>
    public override string Name => "BananaLibrary";

    /// <inheritdoc/>
    public override string Description => "An API for modularized server features.";

    /// <inheritdoc/>
    public override string Author => "O5Zereth and Redforce04";

    /// <inheritdoc/>
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

    /// <inheritdoc/>
    public override LoadPriority Priority => LoadPriority.Lowest;

    /// <inheritdoc/>
    public override Version Version => Version.Parse("1.0.0");

    /// <inheritdoc/>
    public override void Enable()
    {
        Instance = this;
        Loader.Initialize();
        Log.Debug($"Loaded BananaFramework v{this.Version}");
        try
        {
            ThrowException();
        }
        catch (Exception ex)
        {
            Log.Debug(ObjectLogger.GetExceptionString(ex));
        }
    }

    /// <inheritdoc/>
    public override void Disable()
    {
        Loader.Unload();
        Instance = null;
    }

    private void ThrowException()
    {
        throw new Exception($"Test exception.");
    }
}