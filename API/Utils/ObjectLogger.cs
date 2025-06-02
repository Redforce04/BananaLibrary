// -----------------------------------------------------------------------
// <copyright file="ObjectLogger.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Utils;

using System;
using StackCleaner;

/// <summary>
/// Provides helpers for logging objects, exceptions, and type names.
/// </summary>
internal static class ObjectLogger
{
    /// <summary>
    /// Gets a formatted string from an exception.
    /// </summary>
    /// <param name="e">The exception.</param>
    /// <returns>The formatted exception.</returns>
    public static string GetExceptionString(Exception e)
    {
        // Todo: Mess around with cleaning up exception logs.
        StackTraceCleaner cleaner = new(new StackCleanerConfiguration
        {
            ColorFormatting = StackColorFormatType.ExtendedANSIColor,
            IncludeFileData = true,
            IncludeILOffset = true,
            IncludeNamespaces = false,
            Colors = Color32Config.Default,
        });

        return e.GetType().Name + " - " + e.Message;
    }

    /// <summary>
    /// Gets a string of the name of a type.
    /// </summary>
    /// <param name="type">The type that the string should represent.</param>
    /// <returns>The name of the type.</returns>
    public static string GetTypeString(this Type type)
    {
        string result = type.Name;
        if (type.IsGenericType)
        {
            result = type.GetGenericTypeDefinition().Name;
            result += '<';
            foreach (Type genericType in type.GenericTypeArguments)
            {
                result += genericType.Name + ", ";
            }

            if (type.GenericTypeArguments.Length > 0)
            {
                result = result.Substring(result.Length - 2, 0) + '>';
            }
        }

        return result;
    }
}