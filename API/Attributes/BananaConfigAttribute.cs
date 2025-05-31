// -----------------------------------------------------------------------
// <copyright file="BananaConfigAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;

/// <summary>
/// Used to indicate that a property should be parsed as a configuration option.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BananaConfigAttribute : Attribute
{
}