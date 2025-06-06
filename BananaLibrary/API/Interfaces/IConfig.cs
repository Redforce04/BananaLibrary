// -----------------------------------------------------------------------
// <copyright file="IConfig.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Interfaces;

/// <summary>
/// An interface encapsulating generic configuration options which all configuration features must include.
/// </summary>
public interface IConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether the feature should be enabled.
    /// </summary>
    public bool IsEnabled { get; set; }
}