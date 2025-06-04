// -----------------------------------------------------------------------
// <copyright file="Loader.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
// ReSharper disable InconsistentNaming
namespace BananaLibrary;

using API.Features;
using HarmonyLib;
using MEC;

/// <summary>
/// Contains the Initializer that is used to enable the features of this framework.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class Loader
{
    private static bool initialized;

    /// <summary>
    /// Gets or sets the primary <see cref="Harmony"/> Instance used to patch methods.
    /// </summary>
    /// <remarks>
    /// This Library uses the updated HarmonyX library rather than Lib.Harmony. It may conflict with other harmony installs.
    /// </remarks>
    internal static Harmony? Harmony { get; set; }

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
        LoadHarmony();
        BananaPlugin.LoadBananaPlugins();

        BananaServer.LoadBananaServers();
        LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += BananaRole.LoadBananaRoles;
        PermissionsProvider.LoadPermissionProvider();
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
        UnloadHarmony();
        LabApi.Events.Handlers.ServerEvents.WaitingForPlayers -= BananaRole.LoadBananaRoles;

        BananaServer.UnloadBananaServers();
        PermissionsProvider.UnloadPermissionProvider();
        BananaRole.UnloadBananaRoles();
        BananaFeature.UnloadBananaFeatures();
    }

    private static void OnWaitingForPlayer()
    {
        BananaRole.LoadBananaRoles();
    }

    private static void LoadHarmony()
    {
        Harmony = new Harmony("BananaLibrary");
        Harmony.PatchAll();
    }

    private static void UnloadHarmony()
    {
        Harmony.UnpatchAll();
        Harmony = null;
    }
}
