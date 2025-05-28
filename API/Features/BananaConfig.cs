// -----------------------------------------------------------------------
// <copyright file="BananaConfig.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Features;

using System.ComponentModel;
using Interfaces;

#pragma warning disable CS8618

/// <summary>
/// The interface that plugins should use.
/// </summary>
public sealed class BananaConfig : IConfig
{
    /// <summary>
    /// Gets or sets the <see cref="BananaServer.ServerId"/> of this server.
    /// </summary>
    [Description("The id of the server that this plugin is being run on.")]
    public string ServerId { get; set; }

    /// <inheritdoc cref="IConfig.IsEnabled"/>
    [Description("The id of the server that this plugin is being run on.")]
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether debugging logs should be shown.
    /// </summary>
    [Description("Enables or Disables debugging logs for the library.")]
    public bool Debug { get; set; }
}