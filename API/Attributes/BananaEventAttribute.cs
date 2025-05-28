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
    public BananaEventAttribute(Type eventHandlerType, string eventHandlerName)
    {
        this.Type = eventHandlerType;
        this.Name = eventHandlerName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BananaEventAttribute"/> class.
    /// </summary>
    public BananaEventAttribute()
    {
    }

    private Type? Type { get; }

    private string? Name { get; }

    /// <summary>
    /// Registers the event.
    /// </summary>
    /// <param name="method">The method to hook onto the event.</param>
    /// <param name="eventArgsType">The event args to search for when finding the event. If null it will be searched using a manual lookup.</param>
    internal void RegisterEvent(MethodInfo method, Type? eventArgsType)
    {
        try
        {
            EventInfo? info = null;
            {
                if (eventArgsType is null || !EventTypes.ContainsKey(eventArgsType))
                {
                    goto checkViaTypeName;
                }

                info = EventTypes[eventArgsType];
                goto subscribeToEvent;
            }

            checkViaTypeName:
            {
                if (this.Name is null || this.Type is null)
                {
                    goto subscribeToEvent;
                }

                info = Type.GetEvent(this.Name, BindingFlags.Public | BindingFlags.Static);
            }

            subscribeToEvent:
            {
                if (info is null)
                {
                    Log.Error($"Event '{this.Name}' was not found! {(eventArgsType?.Name is null ? string.Empty : $"(Args Type: {eventArgsType.Name})")}");
                    return;
                }

                Delegate handler = Delegate.CreateDelegate(info.EventHandlerType, method);

                // Assign the event handler. This corresponds with `this.OnExecute += command.Type.Method`.
                info.AddEventHandler(this, handler);
            }
        }
        catch (Exception e)
        {
            Log.Warn($"Could not register event '{this.Name}' due to an exception.");
            Log.Debug($"Exception: {e}.");
        }
    }

    private static void RegisterAllEvents()
    {
        foreach (Type type in typeof(ExHandlers.ServerEvents).Assembly.GetTypes())
        {
            if (type.Namespace != "Testing.Events.Handlers")
            {
                continue;
            }

            foreach (EventInfo ev in type.GetEvents(BindingFlags.Public | BindingFlags.Static))
            {
                if (!ev.EventHandlerType.IsGenericType || ev.EventHandlerType.GetGenericTypeDefinition() != typeof(LabEventHandler<>) || ev.EventHandlerType.GenericTypeArguments.Length == 0)
                {
                    continue;
                }

                EventTypes.Add(ev.EventHandlerType.GenericTypeArguments[0], ev);
            }
        }
    }
}