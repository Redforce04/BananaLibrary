// -----------------------------------------------------------------------
// <copyright file="BananaFeature.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using CommandSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Attributes;
using Collections;
using Interfaces;
using Extensions;
using MEC;

/// <summary>
/// The main feature implementation.
/// </summary>
public abstract class BananaFeature : IPrefixableItem
{
    // ReSharper disable InconsistentNaming
    private bool shouldEnable = false;
    private bool enabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="BananaFeature"/> class.
    /// </summary>
    protected BananaFeature()
    {
    }

    /// <summary>
    /// Gets a list of all registered <see cref="BananaFeature">BananaFeatures</see>.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public static FeatureCollection? Features { get; private set; }

    /// <summary>
    /// Gets the name of the feature.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the prefix of the feature.
    /// </summary>
    public string Prefix => this.Name;

    /// <summary>
    /// Gets or sets a value indicating whether the feature is enabled.
    /// </summary>
    public bool Enabled
    {
        get => this.enabled;
        set
        {
            // Check if setting same value.
            if (this.enabled == value)
            {
                return;
            }

            try
            {
                if (value)
                {
                    this.Enable();

                    Log.Info($"Feature '{this.Name}' was enabled!");
                }
                else
                {
                    this.Disable();

                    Log.Info($"Feature '{this.Name}' was disabled!");
                }

                this.enabled = value;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to {(value ? "disable" : "enable")} feature {this.Name}." + e);
                throw;
            }
        }
    }

    public static implicit operator bool([NotNullWhen(true)] BananaFeature? feature)
    {
        return feature is not null;
    }

    /// <summary>
    /// Loads all of the <see cref="BananaFeature">BananaFeatures</see>.
    /// </summary>
    internal static void LoadBananaFeatures()
    {
        Dictionary<string, BananaFeature> features = GetBananaFeatures();
        if (features.Count == 0)
        {
            Log.Debug($"No features were found!");
            return;
        }

        Features = new FeatureCollection(features.Values.ToList());
        Features.MarkAsLoaded();
        ExHandlers.ServerEvents.WaitingForPlayers += EnableFeatures;
    }

    /// <summary>
    /// Unloads all of the <see cref="BananaFeature">BananaFeatures</see>.
    /// </summary>
    internal static void UnloadBananaFeatures()
    {
        foreach (BananaFeature feature in Features!)
        {
            feature.Enabled = false;
        }

        Features = null;
    }

    /// <summary>
    /// Enables the feature.
    /// </summary>
    protected abstract void Enable();

    /// <summary>
    /// Disables the feature.
    /// </summary>
    protected abstract void Disable();

    /// <summary>
    /// Enables all features for this plugin.
    /// </summary>
    private static void EnableFeatures()
    {
        ExHandlers.ServerEvents.WaitingForPlayers -= EnableFeatures;
        Timing.RunCoroutine(EnableBananaFeatures());
    }

    private static IEnumerator<float> EnableBananaFeatures()
    {
        // Wait a quarter second between as an attempt to not overwhelm the game thread.
        foreach (BananaFeature feature in Features!)
        {
            yield return Timing.WaitForSeconds(.25f);
            try
            {
                if (feature is IConfigLoader configLoader)
                {
                    configLoader.LoadConfig();
                }

                if (feature.shouldEnable)
                {
                    feature.Enabled = true;
                    Log.Debug($"Feature '{feature.Name}' was enabled!");
                }
            }
            catch (Exception)
            {
                Log.Warn($"Could not load feature '{feature.Name}' due to an error!");
            }
        }
    }

    private static Dictionary<string, BananaFeature> GetBananaFeatures()
    {
        Dictionary<string, BananaFeature> features = new();

        foreach (Assembly pluginAssembly in LabApi.Loader.PluginLoader.Plugins.Values)
        {
            Type[] types = pluginAssembly.GetTypes();
            foreach (Type type in types)
            {
                if (!type.IsSubclassOf(typeof(BananaFeature)) || type.IsAbstract)
                {
                    continue;
                }

                if (type.GetCustomAttribute<ObsoleteAttribute>() is not null)
                {
                    continue;
                }

                bool def = true;
                Attribute[] attributes = Attribute.GetCustomAttributes(typeof(ServerFeatureTargetAttribute), type);
                foreach (Attribute atr in attributes)
                {
                    if (atr is not ServerFeatureTargetAttribute targetAtr)
                    {
                        continue;
                    }

                    if (targetAtr is { TargetsServer: false })
                    {
                        def = targetAtr.DefaultEnabledForServer;
                        continue;
                    }

                    BananaServer? server = targetAtr.GetServer();
                    if (server is null)
                    {
                        continue;
                    }

                    if (server != BananaServer.Servers?.PrimaryKey)
                    {
                        continue;
                    }

                    def = targetAtr.DefaultEnabledForServer;
                    break;
                }

                BananaFeature feature = (BananaFeature)Activator.CreateInstance(type, nonPublic: true);
                feature.shouldEnable = def;
                features.Add(type.FullName, feature);
                Log.Debug($"Found Banana Feature \'{feature.Name}\'.");
            }
        }

        return features;
    }
}
