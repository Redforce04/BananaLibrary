// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaLibrary
//    Project:          BananaLibrary
//    FileName:         Loader.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/23/2025 14:41
//    Created Date:     05/23/2025 14:05
// -----------------------------------------

// ReSharper disable InconsistentNaming
namespace BananaLibrary.Loader;

using API.Features;

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

        // Initialize the current Banana Servers
        BananaServer.LoadBananaServers();
        BananaRole.LoadBananaRoles();
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
        BananaServer.UnloadBananaServers();
        BananaRole.UnloadBananaRoles();
    }
}