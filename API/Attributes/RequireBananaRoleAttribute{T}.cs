// -----------------------------------------------------------------------
// <copyright file="RequireBananaRoleAttribute{T}.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedCommandLibrary.Attributes;
using AdvancedCommandLibrary.Contexts;
using Extensions;
using Features;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using Utils;

/// <summary>
/// Used to indicate that a command should not execute if the player does not have the specified BananaRole.
/// </summary>
/// <typeparam name="T">The BananaRole that is required.</typeparam>
[AttributeUsage(validOn: AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public sealed class RequireBananaRoleAttribute<T> : RequireBananaRoleAttribute
    where T : BananaRole, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBananaRoleAttribute{T}"/> class.
    /// </summary>
    public RequireBananaRoleAttribute()
        : base(typeof(T))
    {
    }
}