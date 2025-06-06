// -----------------------------------------------------------------------
// <copyright file="ParentCommand.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.Commands;

using AdvancedCommandLibrary;
using AdvancedCommandLibrary.Attributes;
using AdvancedCommandLibrary.Contexts;

/// <summary>
/// The base command for all commands.
/// </summary>
public static class ParentCommand
{
    /// <summary>
    /// The base BananaCommand.
    /// </summary>
    /// <param name="context">The context.</param>
    [ParentCommand("banana", "The base command for all banana functions.", ["bl"])]
    public static void BananaCommand(ParentCommandContext context)
    {
        if (!context.CheckPermissions())
        {
            return;
        }

        context.RespondWithSubCommands();
    }
}