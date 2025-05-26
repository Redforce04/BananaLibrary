// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         BananaPlugin
//    Project:          BananaPlugin
//    FileName:         RoleCollection.cs
//    Author:           Redforce04#4091
//    Revision Date:    11/09/2023 11:52 AM
//    Created Date:     11/09/2023 11:52 AM
// -----------------------------------------

namespace BananaLibrary.API.Collections;

using System.Collections.Generic;
using Features;

/// <inheritdoc />
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