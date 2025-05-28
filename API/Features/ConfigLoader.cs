// -----------------------------------------------------------------------
// <copyright file="ConfigLoader.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

/// <summary>
/// Manages config loading for features.
/// </summary>
internal static class ConfigLoader
{
    /// <summary>
    /// Gets the deserializer used to deserialize all configs.
    /// </summary>
    public static IDeserializer Deserializer { get; } = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .IgnoreFields()
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Gets the serializer used to serialize all configs.
    /// </summary>
    public static ISerializer Serializer { get; } = new SerializerBuilder().Build();
}