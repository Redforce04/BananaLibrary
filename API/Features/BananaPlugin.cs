// -----------------------------------------------------------------------
// <copyright file="BananaPlugin.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Attributes;
using Collections;
using HarmonyLib;
using Interfaces;
using LabApi.Features.Console;
using LabApi.Loader.Features.Yaml;
using Utils;
using YamlDotNet.Serialization.NamingConventions;
using Plugin = LabApi.Loader.Features.Plugins.Plugin;

/// <summary>
/// The base BananaPlugin class. Use this to access any features.
/// </summary>
public sealed class BananaPlugin : IPrefixableItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BananaPlugin"/> class.
    /// </summary>
    /// <param name="plugin">The plugin instance being tracked.</param>
    /// <param name="assembly">The assembly instance being tracked.</param>
    internal BananaPlugin(Plugin plugin, Assembly assembly)
    {
        this.Assembly = assembly;
        this.Plugin = plugin;
    }

    /// <summary>
    /// Gets the primary <see cref="BananaPluginCollection"/> that contains all <see cref="BananaPlugins"/>.
    /// </summary>
    public static BananaPluginCollection BananaPlugins { get; private set; } = null!;

    /// <summary>
    /// Gets the assembly that the BananaItems are a part of.
    /// </summary>
    public Assembly Assembly { get; private set; }

    /// <summary>
    /// Gets the <see cref="Plugin"/> that the BananaItems are a part of.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public Plugin Plugin { get; private set; }

    /// <inheritdoc />
    public string Prefix => Plugin.Name;

    /// <summary>
    /// Gets the <see cref="ServerInfoCollection"/> for all <see cref="BananaServer">BananaServers</see> defined in this plugin.
    /// </summary>
    public ServerInfoCollection? Servers { get; private set; }

    /// <summary>
    /// Gets the <see cref="FeatureCollection"/> for all <see cref="BananaFeature">BananaFeatures</see> defined in this plugin.
    /// </summary>
    public FeatureCollection? Features { get; private set; }

    /// <summary>
    /// Gets the <see cref="RoleCollection"/> for all <see cref="BananaRole">BananaRoles</see> defined in this plugin.
    /// </summary>
    public RoleCollection? Roles { get; private set; } = null;

    /// <summary>
    /// Gets the <see cref="BananaPluginConfig"/> for the plugin.
    /// </summary>
    public BananaPluginConfig Config { get; private set; } = null!;

    /// <summary>
    /// Loads all BananaPlugins.
    /// </summary>
    internal static void LoadBananaPlugins()
    {
        BananaPlugins = new();
        string bananaLibraryAssemblyName = typeof(BananaPlugin).Assembly.FullName;
        foreach (KeyValuePair<Plugin, Assembly> pluginKvp in LabApi.Loader.PluginLoader.Plugins)
        {
            if(!pluginKvp.Value.GetReferencedAssemblies().Any(x => x.FullName == bananaLibraryAssemblyName))
            {
                // Log.Debug($"{pluginKvp.Value.FullName} ({pluginKvp.Key.Name}) does not reference the banana library.");
                continue;
            }

            Log.Debug($"BananaPlugin: {pluginKvp.Value.FullName} ({pluginKvp.Key.Name}).");
            BananaPlugin plugin = new(pluginKvp.Key, pluginKvp.Value);
            Log.Debug($"Loading Configs for plugin \"{plugin.Prefix}\".");
            LoadConfigs(plugin, Attribute.GetCustomAttribute(pluginKvp.Key.GetType(), typeof(BananaPluginConfigDefaultsAttribute)) as BananaPluginConfigDefaultsAttribute);
            Log.Debug($"Loading Banana Servers for plugin \"{plugin.Prefix}\".");
            LoadBananaServers(plugin);
            Log.Debug($"Loading Banana Roles for plugin \"{plugin.Prefix}\".");

            // Todo: revise BananaRole System.
            Log.Debug($"Loading Banana Features for plugin \"{plugin.Prefix}\".");
            LoadBananaFeatures(plugin);

            if (!BananaPlugins.TryAddItem(plugin, out string? response))
            {
                Log.Error($"Could not add BananaPlugin \"{plugin.Plugin.Name}\" to the BananaPlugins Collection. {response}");
            }

            Log.Debug($"Fully loaded plugin \"{plugin.Prefix}\".");
        }

        BananaPlugins.MarkAsLoaded();
        Log.Info($"Loading all BananaFeature configs for all plugins.");
        LoadFeatureConfigs();
    }

    private static void LoadFeatureConfigs()
    {
        foreach (BananaPlugin plugin in BananaPlugins)
        {
            if (plugin.Features is null)
            {
                continue;
            }

            if (plugin.Features?.GetCount() == 0)
            {
                continue;
            }

            const string fileName = "BananaFeatures.yml";
            string pluginConfigPath = LabApi.Loader.ConfigurationLoader.GetConfigPath(plugin.Plugin, fileName);
            if (!File.Exists(pluginConfigPath))
            {
                goto generateNewConfigs;
            }

            if (!TryReadConfig(pluginConfigPath, out Dictionary<string, object>? config))
            {
                Log.Error($"Could not read configs. New configs will be generated.");
                goto generateNewConfigs;
            }

            foreach (KeyValuePair<string, object> kvp in config)
            {
                if (plugin.Features!.FirstOrDefault(x => x.Name == kvp.Key || UnderscoredNamingConvention.Instance.Apply(x.Name) == kvp.Key) is not { } feat)
                {
                    Logger.Debug($"Could not find feature {kvp.Key}.");
                    continue;
                }

                if (kvp.Value is not Dictionary<object, object> objects)
                {
                    continue;
                }

                foreach (KeyValuePair<object, object> obj in objects)
                {
                    CopyConfigValuesWithObject(ref feat, obj.Key.ToString(), obj.Value);
                }
            }

            continue;

            generateNewConfigs:
            {
                Log.Debug($"Creating feature config file for plugin {plugin.Prefix}.");
                if (CreateDefaultFeatureConfig(plugin, out Dictionary<string, BananaFeature> newFeatureConfig))
                {
                    foreach (KeyValuePair<string, BananaFeature> kvp in newFeatureConfig)
                    {
                        if (!plugin.Features!.TryGetFeature(kvp.Key, out BananaFeature? feature))
                        {
                            continue;
                        }

                        CopyConfigValuesTo(ref feature, kvp.Value);
                    }

                    if (!TrySaveConfig(newFeatureConfig, pluginConfigPath))
                    {
                        Log.Error($"An error occurred trying to save feature config file for plugin {plugin.Plugin.Name}.");
                    }

                    continue;
                }

                Logger.Error($"Could not create default configs.");
            }
        }
    }

    private static void CopyConfigValuesWithObject(ref BananaFeature feature, string key, object value)
    {
        foreach (PropertyInfo property in feature.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            try
            {
                string propertyName = UnderscoredNamingConvention.Instance.Apply(property.Name);
                if (key != propertyName)
                {
                    continue;
                }

                if (property.GetMethod is null || property.SetMethod is null)
                {
                    continue;
                }

                if (!Attribute.IsDefined(property, typeof(BananaConfigAttribute)))
                {
                    continue;
                }

                object? convertedValue = TypeConverter.ConvertValue(value, property.PropertyType);
                if (convertedValue is null)
                {
                    Log.Error($"Value of BananaConfig {property.Name} [{property.PropertyType.Name}] could not be converted. It seems to be an unsupported type.");
                    continue;
                }

                property.SetValue(feature, convertedValue);
                Logger.Debug($"{key} - {value} ({property.PropertyType.GetTypeString()})");
            }
            catch (Exception e)
            {
                Log.Error($"Could not copy value {property.Name} due to error [{feature.GetType().Name} <- {value}]: {e}");
            }
        }
    }

    private static void CopyConfigValuesTo(ref BananaFeature original, BananaFeature copy)
    {
        if (original.GetType() != copy.GetType())
        {
            return;
        }

        foreach (PropertyInfo property in original.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            try
            {
                if (property.GetMethod is null || property.SetMethod is null)
                {
                    continue;
                }

                if (!Attribute.IsDefined(property, typeof(BananaConfigAttribute)))
                {
                    continue;
                }

                if (property.Name == nameof(BananaFeature.ShouldEnable))
                {
                    // This value is already set by the loading module.
                    continue;
                }

                property.SetValue(original, property.GetValue(copy));
            }
            catch (Exception e)
            {
                Log.Error($"Could not copy value {property.Name} due to error [{original.GetType().Name} <- {copy.GetType().Namespace}]: {e}");
            }
        }
    }

    private static bool CreateDefaultFeatureConfig(BananaPlugin plugin, out Dictionary<string, BananaFeature> newFeatureConfig)
    {
        newFeatureConfig = new();
        foreach (BananaFeature feature in plugin.Features!)
        {
            BananaFeature featureClone = (BananaFeature)Activator.CreateInstance(feature.GetType(), nonPublic: true);
            featureClone.ShouldEnable = feature.ShouldEnable;
            foreach (PropertyInfo property in feature.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    if (property.GetMethod is null || property.SetMethod is null)
                    {
                        continue;
                    }

                    if (!Attribute.IsDefined(property, typeof(BananaConfigAttribute)))
                    {
                        continue;
                    }

                    if (property.Name == nameof(BananaFeature.ShouldEnable))
                    {
                        // This value is already set by the loading module.
                        continue;
                    }

                    BananaConfigDefaultAttribute[] configDefaults = Attribute.GetCustomAttributes(property, typeof(BananaConfigDefaultAttribute)).Cast<BananaConfigDefaultAttribute>().ToArray();
                    if (configDefaults.Length == 0)
                    {
                        continue;
                    }

                    foreach (BananaConfigDefaultAttribute def in configDefaults)
                    {
                        if (!def.IsCurrentServer(plugin.Assembly))
                        {
                            continue;
                        }

                        property.SetValue(featureClone, def.DefaultValue);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Could not set feature value due to error: {e}");
                }
            }

            newFeatureConfig.Add(feature.Name, featureClone);
        }

        return true;
    }

    private static void LoadConfigs(BananaPlugin plugin, BananaPluginConfigDefaultsAttribute? defaults = null)
    {
        const string fileName = "BananaSettings.yml";
        string pluginConfigPath = LabApi.Loader.ConfigurationLoader.GetConfigPath(plugin.Plugin, fileName);
        if (!File.Exists(pluginConfigPath))
        {
            goto createDefaults;
        }

        if (TryReadConfig(pluginConfigPath, out BananaPluginConfig? pluginConfig))
        {
            plugin.Config = pluginConfig;
            return;
        }

        Log.Error($"Could not read configs. Defaults will be generated.");
        createDefaults:
        {
            BananaPluginConfig? pluginConfig2;
            if (defaults is not null)
            {
                pluginConfig2 = new BananaPluginConfig()
                {
                    Debug = defaults.Debug,
                    IsEnabled = defaults.IsEnabled,
                    CurrentBananaServerId = defaults.CurrentBananaServerId,
                    LoggerPrefix = defaults.LoggerPrefix,
                };
            }
            else
            {
                TryCreateDefaultConfig(out pluginConfig2);
            }

            if (pluginConfig2 is not null)
            {
                plugin.Config = pluginConfig2;
                TrySaveConfig(pluginConfig2, pluginConfigPath);
                return;
            }

            Log.Error($"Could not generate default configs.");
        }
    }

    private static bool TryReadConfig<TConfig>(string fileName, [NotNullWhen(true)] out TConfig? config)
        where TConfig : class, new()
    {
        config = null;

        try
        {
            // If the configuration file doesn't exist, we return false to indicate that the configuration wasn't successfully read.
            if (!File.Exists(fileName))
            {
                return false;
            }

            // We read the configuration file.
            string serializedConfig = File.ReadAllText(fileName);

            // We deserialize the configuration and return whether it was successful.
            config = YamlConfigParser.Deserializer.Deserialize<TConfig>(serializedConfig);
            return true;
        }
        catch (Exception e)
        {
            // We log the error and return false to indicate that the configuration wasn't successfully read.
            Log.Error($"Couldn't read the configuration of the plugin.");
            Log.Error($"{e}");
            return false;
        }
    }

    private static void TryCreateDefaultConfig<TConfig>([NotNullWhen(true)] out TConfig? config)
        where TConfig : class, new()
    {
        config = null;

        try
        {
            // We create a default instance of the configuration and return true.
            config = Activator.CreateInstance<TConfig>();
        }
        catch (Exception)
        {
            // We log the error and return false to indicate that the configuration wasn't successfully loaded.
        }
    }

    private static bool TrySaveConfig<TConfig>(TConfig config, string configFilePath)
        where TConfig : class, new()
    {
        try
        {
            // We serialize the configuration.
            string serializedConfig = ConfigLoader.Serializer.Serialize(config);

            // We finally write the serialized configuration to the file and return whether it was successful.
            File.WriteAllText(configFilePath, serializedConfig);

            // We return true to indicate that the configuration was successfully saved.
            return true;
        }
        catch (Exception e)
        {
            // We log the error and return false to indicate that the configuration wasn't successfully saved.
            Log.Error($"Couldn't save the configuration of the plugin.");
            Log.Error($"{e}");
            return false;
        }
    }

    private static void LoadBananaServers(BananaPlugin plugin)
    {
        try
        {
            List<BananaServer> bananaServers = GetServerInfoInstances(plugin);
            if (bananaServers.Count == 0)
            {
                Log.Debug($"No BananaServers found. BananaServers will not be loaded.");
                return;
            }

            bool foundViaPort = false;
            BananaServer? primaryKey = bananaServers.FirstOrDefault(b => b.ServerId == plugin.Config.CurrentBananaServerId);
            if (primaryKey == null)
            {
                foundViaPort = true;
                primaryKey = bananaServers.FirstOrDefault(b => b.ServerPort == ServerStatic.ServerPort);
            }

            if (primaryKey is null)
            {
                Log.Error($"Could not find a valid server profile, for BananaPlugin \"{plugin.Prefix}\". Try setting the Server Id or updating the BananaServer port to match the server port.");
                return;
            }

            plugin.Servers = new ServerInfoCollection(primaryKey, bananaServers);
            plugin.Servers.MarkAsLoaded();
            Log.Info($"Plugin \"{plugin.Prefix}\" Current BananaServer: {plugin.Servers.PrimaryKey} [Found Via {(foundViaPort ? "Port" : "Config")}].");
        }
        catch (Exception e)
        {
            Log.Error($"Could not load BananaServers for plugin {plugin.Prefix}. Exception: {e}");
        }
    }

    private static List<BananaServer> GetServerInfoInstances(BananaPlugin plugin)
    {
        List<BananaServer> serverInfos = new();

        Type[] types = plugin.Assembly.GetTypes();
        foreach (Type type in types)
        {
            if (!type.IsSubclassOf(typeof(BananaServer)) || type.IsAbstract)
            {
                continue;
            }

            if (type.GetCustomAttribute<ObsoleteAttribute>() is not null)
            {
                continue;
            }

            BananaServer info = (BananaServer)Activator.CreateInstance(type, nonPublic: true);
            serverInfos.Add(info);
            Log.Debug($"Found Server Info for plugin \"{plugin.Prefix}\": {info.ServerName} [{info.ServerPort}] - {info.ServerId}");
        }

        return serverInfos;
    }

    private static void LoadBananaFeatures(BananaPlugin plugin)
    {
        Dictionary<string, BananaFeature> features = GetBananaFeatures(plugin);
        if (features.Count == 0)
        {
            Log.Debug($"No features were found!");
            return;
        }

        plugin.Features = new FeatureCollection(features.Values.ToList());
        plugin.Features.MarkAsLoaded();
    }

    private static Dictionary<string, BananaFeature> GetBananaFeatures(BananaPlugin plugin)
    {
        Dictionary<string, BananaFeature> features = new();

        Type[] types = plugin.Assembly.GetTypes();
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
            Attribute[] attributes = Attribute.GetCustomAttributes(type, typeof(ServerFeatureTargetAttribute));
            foreach (Attribute atr in attributes)
            {
                if (atr is not ServerFeatureTargetAttribute targetAtr)
                {
                    continue;
                }

                try
                {
                    if (targetAtr is { TargetsServer: false })
                    {
                        Log.Debug($"[Global] Feature flag detected. {targetAtr.GetType()} - {targetAtr.DefaultEnabledForServer}");
                        def = targetAtr.DefaultEnabledForServer;
                        continue;
                    }

                    BananaServer? server = targetAtr.GetServer(plugin);
                    if (server is null)
                    {
                        Log.Warn($"Feature Flag server is null. This likely shouldn't happen.");
                        continue;
                    }

                    if (server != plugin.Servers?.PrimaryKey)
                    {
                        continue;
                    }

                    def = targetAtr.DefaultEnabledForServer;
                    break;
                }
                catch (Exception)
                {
                    Log.Error($"An error has occured while processing a feature flag.");
                }
            }

            BananaFeature feature = (BananaFeature)Activator.CreateInstance(type, nonPublic: true);
            feature.Harmony = new Harmony($"BananaLibrary.{plugin.Prefix}.{feature.Name}");
            feature.ShouldEnable = def;
            features.Add(type.FullName, feature);
            Log.Debug($"Found Banana Feature \'{feature.Name}\'.", false);
        }

        return features;
    }
}