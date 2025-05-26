// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaLibrary
//    Project:          BananaLibrary
//    FileName:         IConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/23/2025 14:52
//    Created Date:     05/23/2025 14:05
// -----------------------------------------

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

    /// <summary>
    /// Gets or sets a value indicating whether the feature should output debugging logs.
    /// </summary>
    public bool Debug { get; set; }
}