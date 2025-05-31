// -----------------------------------------------------------------------
// <copyright file="BananaEventManager.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using System;
using System.Diagnostics;
using System.Reflection;
using Attributes;

/// <summary>
/// Manages <see cref="BananaEventAttribute"/> items such as registering and unregistering.
/// </summary>
// ReSharper disable MemberCanBePrivate.Global
public class BananaEventManager
{
    /// <summary>
    /// Manually registers all events in the calling type.
    /// </summary>
    /// <remarks>By default, all <see cref="BananaEventAttribute">BananaEvents</see> in a <see cref="BananaFeature">BananaFeatures</see> will be auto-registered unless it is not auto-registered.</remarks>
    /// <param name="instance">The object instance to register. If null, it will be assumed the methods are static.</param>
    public static void LoadEvents(object? instance = null) => LoadEvents(GetCallingMethod().GetType(), instance);

    /// <summary>
    /// Manually registers all events in the specified type.
    /// </summary>
    /// <remarks>By default, all <see cref="BananaEventAttribute">BananaEvents</see> in a <see cref="BananaFeature">BananaFeatures</see> will be auto-registered unless it is not auto-registered.</remarks>
    /// <param name="instance">The object instance to register. If null, it will be assumed the methods are static.</param>
    /// <typeparam name="T">The type that should be registered.</typeparam>
    public static void LoadEvents<T>(object? instance = null) => LoadEvents(typeof(T), instance);

    /// <summary>
    /// Manually registers all events in the specified type.
    /// </summary>
    /// <remarks>By default, all <see cref="BananaEventAttribute">BananaEvents</see> in a <see cref="BananaFeature">BananaFeatures</see> will be auto-registered unless it is not auto-registered.</remarks>
    /// <param name="type">The type that should be registered.</param>
    /// <param name="instance">The object instance to register. If null, it will be assumed the methods are static.</param>
    public static void LoadEvents(Type type, object? instance = null)
    {
        BananaFeature? bananaFeature = null;
        if (type.IsSubclassOf(typeof(BananaFeature)))
        {
            foreach (BananaPlugin plugin in BananaPlugin.BananaPlugins)
            {
                if (plugin.Features is null)
                {
                    continue;
                }

                bool found = false;
                foreach (BananaFeature banFeature in plugin.Features)
                {
                    if (banFeature.GetType() != type)
                    {
                        continue;
                    }

                    bananaFeature = banFeature;
                    found = true;
                    break;
                }

                if (found)
                {
                    break;
                }
            }
        }

        foreach (MethodInfo m in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | (instance is null ? BindingFlags.Static : BindingFlags.Instance)))
        {
            if (Attribute.GetCustomAttribute(m, typeof(BananaEventAttribute)) is not BananaEventAttribute ev)
            {
                continue;
            }

            ParameterInfo[] parameterInfos = m.GetParameters();
            ev.RegisterEvent(m, parameterInfos.Length == 1 ? parameterInfos[0].ParameterType : null, instance);

            bananaFeature?.SubscribedEvents.Add(m, ev);
        }
    }

    /// <summary>
    /// Manually unregisters all events in the calling type.
    /// </summary>
    /// <remarks>By default, all <see cref="BananaEventAttribute">BananaEvents</see> in a <see cref="BananaFeature">BananaFeatures</see> will be auto-unregistered unless it is not auto-registered.</remarks>
    /// <param name="instance">The object instance to unregister. If null, it will be assumed the methods are static.</param>
    public static void UnloadEvents(object? instance = null) => UnloadEvents(GetCallingMethod().GetType(), instance);

    /// <summary>
    /// Manually unregisters all events in the specified type.
    /// </summary>
    /// <remarks>By default, all <see cref="BananaEventAttribute">BananaEvents</see> in a <see cref="BananaFeature">BananaFeatures</see> will be auto-unregistered unless it is not auto-registered.</remarks>
    /// <param name="instance">The object instance to unregister. If null, it will be assumed the methods are static.</param>
    /// <typeparam name="T">The type that should be unregistered.</typeparam>
    public static void UnloadEvents<T>(object? instance = null) => UnloadEvents(typeof(T), instance);

    /// <summary>
    /// Manually unregisters all events in the specified type.
    /// </summary>
    /// <remarks>By default, all <see cref="BananaEventAttribute">BananaEvents</see> in a <see cref="BananaFeature">BananaFeatures</see> will be auto-unregistered unless it is not auto-registered.</remarks>
    /// <param name="type">The type that should be unregistered.</param>
    /// <param name="instance">The object instance to register. If null, it will be assumed the methods are static.</param>
    public static void UnloadEvents(Type type, object? instance = null)
    {
        BananaFeature? bananaFeature = null;
        if (type.IsSubclassOf(typeof(BananaFeature)))
        {
            foreach (BananaPlugin plugin in BananaPlugin.BananaPlugins)
            {
                if (plugin.Features is null)
                {
                    continue;
                }

                bool found = false;
                foreach (BananaFeature banFeature in plugin.Features)
                {
                    if (banFeature.GetType() != type)
                    {
                        continue;
                    }

                    bananaFeature = banFeature;
                    found = true;
                    break;
                }

                if (found)
                {
                    break;
                }
            }
        }

        foreach (MethodInfo m in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | (instance is null ? BindingFlags.Static : BindingFlags.Instance)))
        {
            if (Attribute.GetCustomAttribute(m, typeof(BananaEventAttribute)) is not BananaEventAttribute ev)
            {
                continue;
            }

            ev.UnregisterEvent(m, instance);

            bananaFeature?.SubscribedEvents.Remove(m);
        }
    }

    private static MethodBase GetCallingMethod(int skip = 0)
    {
        StackTrace stack = new (2 + skip);

        return stack.GetFrame(0).GetMethod();
    }
}