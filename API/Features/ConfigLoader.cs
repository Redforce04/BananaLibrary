// -----------------------------------------------------------------------
// <copyright file="ConfigLoader.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using LabApi.Loader.Features.Yaml.CustomConverters;
using Serialization;
using Utils;
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
    internal static IDeserializer Deserializer { get; } = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithTypeConverter(new CustomVectorConverter())
        .WithTypeConverter(new CustomColor32Converter())
        .WithTypeConverter(new CustomColorConverter()).IgnoreFields()
        .IgnoreUnmatchedProperties()
        .IgnoreFields()
        .WithTypeInspector(inspector => new ConfigurationOptionTypeInspector(inspector))
        .Build();

    /// <summary>
    /// Gets the serializer used to serialize all configs.
    /// </summary>
    internal static ISerializer Serializer { get; } = new SerializerBuilder()
        .WithEmissionPhaseObjectGraphVisitor(visitor => new CommentsObjectGraphVisitor(visitor.InnerVisitor))
        .WithTypeInspector(typeInspector => new CommentGatheringTypeInspector(typeInspector))
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithTypeConverter(new CustomVectorConverter())
        .WithTypeConverter(new CustomColor32Converter())
        .WithTypeConverter(new CustomColorConverter())
        .WithTypeInspector(inspector => new ConfigurationOptionTypeInspector(inspector))
        .IgnoreFields()
        .DisableAliases()
        .Build();

    /// <summary>
    /// Loads the configs for all features.
    /// </summary>
    internal static void LoadConfigs()
    {
        // Dictionary<string, object> rawDeserializedConfigs = Deserializer.Deserialize<Dictionary<string, object>>()
    }
}