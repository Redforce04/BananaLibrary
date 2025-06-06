// -----------------------------------------------------------------------
// <copyright file="RoleCollection.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Collections;

using System.Collections.Generic;

using BananaLibrary.API.Features;

/// <summary>
/// Used to contain all <see cref="BananaRole"/> instances for a <see cref="BananaPlugin"/>.
/// </summary>
public sealed class RoleCollection : Collection<BananaRole>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleCollection"/> class.
    /// </summary>
    /// <param name="roles">The roles to add.</param>
    public RoleCollection(List<BananaRole> roles)
    {
        foreach (BananaRole role in roles)
        {
            if (roles is null)
            {
                continue;
            }

            this.TryAddItem(role, out _);
        }
    }
}