// -----------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Utils;

using System;
using System.Collections.Generic;

/// <summary>
/// Helps with advanced type creation and loading.
/// </summary>
#pragma warning disable SA1201 // Interface should not follow a method.
internal class TypeConverter
{
    /// <summary>
    /// Used to return an object that has been converted to the specified type.
    /// </summary>
    /// <param name="value">The original value of the object.</param>
    /// <param name="targetType">The target type to convert it to.</param>
    /// <returns>The original value as the target type or null if it cannot be converted.</returns>
    internal static object? ConvertValue(object value, Type targetType)
    {
        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value.ToString());
        }

        if (targetType.GetInterface(nameof(IConvertible)) is not null)
        {
            return Convert.ChangeType(value, targetType);
        }

        if (targetType.IsGenericType)
        {
            if (targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                IDictHelper? helper = Activator.CreateInstance(typeof(DictHelper<,>).MakeGenericType(targetType.GenericTypeArguments[0], targetType.GenericTypeArguments[1])) as IDictHelper;
                if (helper is null)
                {
                    Log.Debug("Dictionary Helper could not be created.");
                    return null;
                }

                return helper.CreateDictionary(value);
            }

            if (targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                IListHelper? helper = Activator.CreateInstance(typeof(ListHelper<>).MakeGenericType(targetType.GenericTypeArguments[0])) as IListHelper;
                if (helper is null)
                {
                    Log.Debug("List Helper could not be created.");
                    return null;
                }

                return helper.CreateList(value);
            }
        }

        Log.Debug($"Could not find a converter for type {targetType.Name}");
        return null;
    }

    /// <summary>
    /// A helper interface for creating type-casted dictionaries.
    /// </summary>
    private interface IDictHelper
    {
        /// <summary>
        /// Creates a dictionary.
        /// </summary>
        /// <returns>An object that is a dictionary.</returns>
        /// <param name="obj">The original dictionary object to convert the object to.</param>
        internal object? CreateDictionary(object obj);
    }

    private class DictHelper<TKey, TValue> : IDictHelper
    {
        /// <inheritdoc />
        object? IDictHelper.CreateDictionary(object obj)
        {
            if (obj is not Dictionary<object, object> dict)
            {
                Log.Debug("Dictionary was not generic dictionary.");
                return null;
            }

            Dictionary<TKey, TValue> newDict = new();
            foreach (KeyValuePair<object, object> pair in dict)
            {
                if (ConvertValue(pair.Key, typeof(TKey)) is not TKey newKey || ConvertValue(pair.Value, typeof(TValue)) is not TValue newValue)
                {
                    Log.Debug($"Dictionary object could not be converted to Dictionary<{typeof(TKey).Name},{typeof(TValue).Name}>.");
                    return null;
                }

                newDict.Add(newKey, newValue);
            }

            return newDict;
        }
    }

    /// <summary>
    /// A helper interface for creating type-casted lists.
    /// </summary>
    private interface IListHelper
    {
        /// <summary>
        /// Creates a lists.
        /// </summary>
        /// <returns>An object that is a list.</returns>
        /// <param name="obj">The original list object to convert the object to.</param>
        internal object? CreateList(object obj);
    }

    private class ListHelper<TKey> : IListHelper
    {
        /// <inheritdoc />
        object? IListHelper.CreateList(object obj)
        {
            if (obj is not List<object> list)
            {
                Log.Debug("List was not generic list.");
                return null;
            }

            List<TKey> newList = new();
            foreach (KeyValuePair<object, object> pair in list)
            {
                if (ConvertValue(pair.Key, typeof(TKey)) is not TKey newKey)
                {
                    Log.Debug($"List object could not be converted to LIst<{typeof(TKey).Name}>.");
                    return null;
                }

                newList.Add(newKey);
            }

            return newList;
        }
    }
}
#pragma warning restore
