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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Attributes;
using Collections;
using Interfaces;
using Extensions;
using LabApi.Features.Console;
using MEC;

/// <summary>
/// The main feature implementation.
/// </summary>
public abstract class BananaFeature : IPrefixableItem
{
    // ReSharper disable InconsistentNaming
    private bool enabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="BananaFeature"/> class.
    /// </summary>
    protected BananaFeature()
    {
    }

    /// <summary>
    /// gets or sets a value indicating whether this feature should be enabled.
    /// </summary>
    /// <remarks>
    /// When this config option is set, it overrides any plugin attributes such as DisabledByDefault.
    /// </remarks>
    [BananaConfig]
    [Description("Indicates whether or not this BananaFeature should be enabled. This option overrides DisabledOnServer Attributes.")]
    public bool ShouldEnable { get; set; } = true;

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
                    this.LoadFeatureEvents();
                    this.Enable();
                    Log.Info($"Feature '{this.Name}' was enabled!");
                }
                else
                {
                    this.UnloadFeatureEvents();
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

    /// <summary>
    /// Gets a value indicating what events are subscribed.
    /// </summary>
    internal Dictionary<MethodInfo, BananaEventAttribute> SubscribedEvents { get; } = new();

    public static implicit operator bool([NotNullWhen(true)] BananaFeature? feature)
    {
        return feature is not null;
    }

    /// <summary>
    /// Loads all of the <see cref="BananaFeature">BananaFeatures</see>.
    /// </summary>
    internal static void LoadBananaFeatures()
    {
    }

    /// <summary>
    /// Unloads all of the <see cref="BananaFeature">BananaFeatures</see>.
    /// </summary>
    internal static void UnloadBananaFeatures()
    {
    }

    /// <summary>
    /// Enables all features for this plugin.
    /// </summary>
    internal static void EnableFeatures()
    {
        Timing.RunCoroutine(EnableBananaFeatures());
    }

    /// <summary>
    /// Enables the feature.
    /// </summary>
    protected abstract void Enable();

    /// <summary>
    /// Disables the feature.
    /// </summary>
    protected abstract void Disable();

    private static IEnumerator<float> EnableBananaFeatures()
    {
        // Wait a quarter second between as an attempt to not overwhelm the game thread.
        foreach (BananaPlugin plugin in BananaPlugin.BananaPlugins)
        {
            Log.Debug($"Loading Features for Plugin '{plugin.Prefix}'.");
            if (plugin.Features is null)
            {
                continue;
            }

            foreach (BananaFeature feature in plugin.Features)
            {
                yield return Timing.WaitForSeconds(.25f);
                try
                {
                    if (feature.ShouldEnable)
                    {
                        feature.Enabled = true;
                        Log.Debug($"Feature '{feature.Name}' was enabled!");
                    }
                    else
                    {
                        Log.Debug($"Feature '{feature.Name}' will not be enabled due to configuration!");
                    }
                }
                catch (Exception)
                {
                    Log.Warn($"Could not load feature '{feature.Name}' due to an error!");
                }
            }
        }
    }

    private void LoadFeatureEvents()
    {
        foreach (MethodInfo m in this.GetType().GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (Attribute.GetCustomAttribute(m, typeof(BananaEventAttribute)) is not BananaEventAttribute ev)
            {
                continue;
            }

            if (!ev.AutoRegister)
            {
                continue;
            }

            ParameterInfo[] parameterInfos = m.GetParameters();
            ev.RegisterEvent(m, parameterInfos.Length == 1 ? parameterInfos[0].ParameterType : null, this);
            SubscribedEvents.Add(m, ev);
        }
    }

    private void UnloadFeatureEvents()
    {
        foreach (KeyValuePair<MethodInfo, BananaEventAttribute> kvp in SubscribedEvents)
        {
            kvp.Value.UnregisterEvent(kvp.Key);
        }

        SubscribedEvents.Clear();
    }
}
