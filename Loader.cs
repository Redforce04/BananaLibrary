// -----------------------------------------------------------------------
// <copyright file="Loader.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
// ReSharper disable InconsistentNaming
namespace BananaLibrary;

using BananaLibrary.API.Features;
using MEC;

/// <summary>
/// Contains the Initializer that is used to enable the features of this framework.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class Loader
{
    private static bool initialized;

    /// <summary>
    /// Initializes the features of this framework.
    /// </summary>
    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        CosturaUtility.Initialize();

        BananaPlugin.LoadBananaPlugins();

        BananaServer.LoadBananaServers();
        BananaRole.LoadBananaRoles();
        BananaFeature.LoadBananaFeatures();

        Timing.CallDelayed(.5f, BananaFeature.EnableFeatures);
    }

    /// <summary>
    /// Unloads all BananaPlugin Items.
    /// </summary>
    public static void Unload()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;

        // Todo make an unloading system.
        BananaServer.UnloadBananaServers();
        BananaRole.UnloadBananaRoles();
        BananaFeature.UnloadBananaFeatures();
    }
}
