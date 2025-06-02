// -----------------------------------------------------------------------
// <copyright file="BananaEventAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using System.Collections.Generic;
using System.Reflection;
using LabApi.Events;

/// <summary>
/// Used to subscribe to events from within features.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class BananaEventAttribute : Attribute
{
    private static readonly Dictionary<Type, EventInfo> EventTypes = new();

    /// <summary>
    /// Initializes static members of the <see cref="BananaEventAttribute"/> class.
    /// </summary>
    static BananaEventAttribute()
    {
        RegisterAllEvents();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BananaEventAttribute"/> class.
    /// </summary>
    /// <param name="eventHandlerType">The type of the event handler to subscribe to.</param>
    /// <param name="eventHandlerName">The name of the event handler to subscribe to.</param>
    /// <param name="autoRegister">Indicates whether the plugin should autoregister the event.</param>
    public BananaEventAttribute(Type eventHandlerType, string eventHandlerName, bool autoRegister = true)
    {
        this.Type = eventHandlerType;
        this.Name = eventHandlerName;
        this.AutoRegister = autoRegister;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BananaEventAttribute"/> class.
    /// </summary>
    /// <param name="autoRegister">Indicates whether the plugin should autoregister the event.</param>
    public BananaEventAttribute(bool autoRegister = true)
    {
        this.AutoRegister = autoRegister;
    }

    /// <summary>
    /// Gets a value indicating whether the event should be auto-registered once the feature is loaded.
    /// </summary>
    internal bool AutoRegister { get; }

    private Type? Type { get; }

    private string? Name { get; }

    private EventInfo? SubscribedEvent { get; set; }

    private Dictionary<object, Delegate> Objects { get; set; } = new();

    private Delegate? StaticDelegate { get; set; }

    /// <summary>
    /// Registers the event.
    /// </summary>
    /// <param name="method">The method to hook onto the event.</param>
    /// <param name="eventArgsType">The event args to search for when finding the event. If null it will be searched using a manual lookup.</param>
    /// <param name="obj">The object of the method. If this is null it will be assumed the object is static.</param>
    internal void RegisterEvent(MethodInfo method, Type? eventArgsType, object? obj = null)
    {
        try
        {
            if ((obj is null && this.StaticDelegate is not null) || (obj is not null && Objects.ContainsKey(obj)))
            {
                Log.Warn($"An attempt to double subscribe an event was made.");
                return;
            }

            if (this.SubscribedEvent is null)
            {
                {
                    if (eventArgsType is null || !EventTypes.ContainsKey(eventArgsType))
                    {
                        goto checkViaTypeName;
                    }

                    this.SubscribedEvent = EventTypes[eventArgsType];
                    goto subscribeToEvent;
                }

                checkViaTypeName:
                {
                    if (this.Name is null || this.Type is null)
                    {
                        goto subscribeToEvent;
                    }

                    this.SubscribedEvent = Type.GetEvent(this.Name, BindingFlags.Public | BindingFlags.Static);
                }
            }

            subscribeToEvent:
            {
                if (this.SubscribedEvent is null)
                {
                    Log.Error($"Event '{this.Name}' was not found! {(eventArgsType?.Name is null ? string.Empty : $"(Args Type: {eventArgsType.Name})")}");
                    return;
                }

                Delegate triggerDelegate = Delegate.CreateDelegate(this.SubscribedEvent.EventHandlerType, obj, method);
                if (obj is not null)
                {
                    this.Objects.Add(obj, triggerDelegate);
                }
                else
                {
                    this.StaticDelegate = triggerDelegate;
                }

                // Assign the event handler. This corresponds with `this.OnExecute += command.Type.Method`.
                this.SubscribedEvent.AddEventHandler(this, obj is null ? this.StaticDelegate : this.Objects[obj]);
            }
        }
        catch (Exception e)
        {
            Log.Warn($"Could not register event '{this.Name}' due to an exception.");
            Log.Debug($"Exception: {e}.");
        }
    }

    /// <summary>
    /// Unsubscribes from the event which this method is registered to.
    /// </summary>
    /// <param name="method">The method to hook onto the event.</param>
    /// <param name="instance">The instance to unregister.</param>
    internal void UnregisterEvent(MethodInfo method, object? instance = null)
    {
        if (this.SubscribedEvent is null)
        {
            return;
        }

        if ((instance is null && this.StaticDelegate is null) || (instance is not null && !Objects.ContainsKey(instance)))
        {
            return;
        }

        try
        {
            this.SubscribedEvent.RemoveEventHandler(this, instance is null ? this.StaticDelegate : this.Objects[instance]);
            if (instance is null)
            {
                this.StaticDelegate = null;
            }
            else
            {
                this.Objects.Remove(instance);
            }
        }
        catch (Exception)
        {
            Log.Error($"Could not unregister event '{this.Name}' due to an exception.");
        }
    }

    private static void RegisterAllEvents()
    {
        int typeCount = 0;
        foreach (Type type in typeof(ExHandlers.ServerEvents).Assembly.GetTypes())
        {
            if (type.Namespace != "LabApi.Events.Handlers")
            {
                continue;
            }

            typeCount++;
            foreach (EventInfo ev in type.GetEvents(BindingFlags.Public | BindingFlags.Static))
            {
                if (!ev.EventHandlerType.IsGenericType || ev.EventHandlerType.GetGenericTypeDefinition() != typeof(LabEventHandler<>) || ev.EventHandlerType.GenericTypeArguments.Length == 0)
                {
                    continue;
                }

                EventTypes.Add(ev.EventHandlerType.GenericTypeArguments[0], ev);
            }
        }

        Log.Debug($"Registered {EventTypes.Count} events from {typeCount} types.");
    }
}