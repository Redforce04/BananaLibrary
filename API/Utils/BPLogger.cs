// -----------------------------------------------------------------------
// <copyright file="BPLogger.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BananaLibrary.API.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BananaLibrary.API.Features;
using Discord;

/// <summary>
/// A class used to sort logs by their feature and type.
/// </summary>
public sealed class BPLogger
{
    /// <summary>
    /// Gets a list of loggers.
    /// </summary>
    public static readonly List<BPLogger> Loggers = new ();

    private static readonly HashSet<MethodBase> AlreadyIdentified = new();

    private static readonly Dictionary<string, (string, string)> Identifiers = new ();
    private static readonly Assembly CurrentAssembly;

#if !DEBUG
    private static readonly Assembly Assembly = typeof(BPLogger).Assembly;
#endif

    static BPLogger()
    {
        CurrentAssembly = Assembly.GetExecutingAssembly();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BPLogger"/> class.
    /// </summary>
    /// <param name="feature">The feature to use as a parent.</param>
    internal BPLogger(BananaFeature feature)
    {
        this.Feature = feature;
        Loggers.Add(this);
    }

    /// <summary>
    /// Gets the feature associated with this logger.
    /// </summary>
    public BananaFeature Feature { get; }

    /// <summary>
    /// Gets the log name associated with this logger.
    /// </summary>
    public string LogName => this.Feature.Name;

    /// <summary>
    /// Forces the logger to identify the calling method as the specified method and declaring type.
    /// </summary>
    /// <param name="typeName">The type name to identify as.</param>
    /// <param name="methodName">The method name to identify as.</param>
    /// <param name="force">Specifies whether to force the operation.</param>
    internal static void IdentifyMethodAs(string typeName, string methodName, bool force = false)
    {
        MethodBase method = GetCallingMethod();

        if (AlreadyIdentified.Add(method) || force)
        {
            Identifiers[GetFullMethodName(method)] = (typeName, methodName);
        }
    }

    /// <summary>
    /// Logs an info message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal static void Info(string message)
    {
        LogMessage($"[BP:{GetCallerString()}] {message}", Assembly.GetCallingAssembly(), LogLevel.Info);
    }

    /// <summary>
    /// Logs a warn message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal static void Warn(string message)
    {
        LogMessage($"[BP:{GetCallerString()}] {message}", Assembly.GetCallingAssembly(), LogLevel.Warn);
    }

    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal static void Error(string message)
    {
        LogMessage($"[BP:{GetCallerString()}] {message}", Assembly.GetCallingAssembly(), LogLevel.Error);
    }

    /// <summary>
    /// Logs a debug message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal static void Debug(string message)
    {
        LogMessage($"[BP:{GetCallerString()}] {message}", Assembly.GetCallingAssembly(), LogLevel.Debug);
    }

    /// <summary>
    /// Logs an info message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal void FeatureInfo(string message)
    {
        LogMessage($"[BP+{this.LogName}-{GetCallerString(false)}] {message}", Assembly.GetCallingAssembly(), LogLevel.Info, this);
    }

    /// <summary>
    /// Logs a warn message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal void FeatureWarn(string message)
    {
        LogMessage($"[BP+{this.LogName}-{GetCallerString(false)}] {message}", Assembly.GetCallingAssembly(), LogLevel.Warn, this);
    }

    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal void FeatureError(string message)
    {
        LogMessage($"[BP+{this.LogName}-{GetCallerString(false)}] {message}", Assembly.GetCallingAssembly(), LogLevel.Error, this);
    }

    /// <summary>
    /// Logs a debug message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    internal void FeatureDebug(string message)
    {
        LogMessage($"[BP+{this.LogName}-{GetCallerString(false)}] {message}", Assembly.GetCallingAssembly(), LogLevel.Debug, this);
    }

    // ReSharper disable once UnusedParameter.Local
    private static void LogMessage(string message, Assembly callingAssembly, LogLevel logType, BPLogger? feature = null)
    {
        // Info:    ServerConsole.AddLog(FormatText($"&7[&b&6{type}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7"), ConsoleColor.Cyan);
        // Warning: ServerConsole.AddLog(FormatText($"&7[&b&3{type}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7"), ConsoleColor.DarkYellow);
        // Error:   ServerConsole.AddLog(FormatText($"&7[&b&1{type}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7"), ConsoleColor.Red);
        // Debug:   ServerConsole.AddLog(FormatText($"&7[&b&5{type}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7"), ConsoleColor.Magenta);
        // Raw:     ServerConsole.AddLog(FormatText($"{(!string.IsNullOrEmpty(prefix) ? $"&7[&b&2{prefix}&B&7] " : "")}" + message, "7"));

        // Unity Logging:
        // Info:    UnityEngine.Debug.Log(FormatText($"&7[&b&6{type}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7", true));
        // Warning: UnityEngine.Debug.LogWarning(FormatText($"&7[&b&3{type}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7", true));
        // Error:   UnityEngine.Debug.LogError(FormatText($"&7[&b&1{type}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7", true));
        // Debug:   UnityEngine.Debug.Log(FormatText($"&7[&b&5{type}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7", true));
        // Raw:     UnityEngine.Debug.Log(FormatText($"{(!string.IsNullOrEmpty(prefix) ? $"&7[&b&2{prefix}&B&7] " : "")}" + message, "7", true));
#if !DEBUG
        if (logType != LogLevel.Debug || Log.DebugEnabled.Contains(Assembly))
        {
            Log.SendRaw(message, color);
        }
#else
        // Todo Fix PluginAPI.Core.Log.Raw(PluginAPI.Core.Log.FormatText(message, color));
        string prefix = callingAssembly == CurrentAssembly ? "BananaLibrary" : callingAssembly.GetName().Name;
        switch (logType)
        {
            // Raw:     ServerConsole.AddLog(FormatText($"{(!string.IsNullOrEmpty(prefix) ? $"&7[&b&2{prefix}&B&7] " : "")}" + message, "7"));
            case LogLevel.Info:
                ServerConsole.AddLog(FormatText($"&7[&b&6{logType}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7"), ConsoleColor.Cyan);
                break;
            case LogLevel.Warn:
                ServerConsole.AddLog(FormatText($"&7[&b&3{logType}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7"), ConsoleColor.DarkYellow);
                break;
            case LogLevel.Error:
                ServerConsole.AddLog(FormatText($"&7[&b&1{logType}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7"), ConsoleColor.Red);
                break;
            case LogLevel.Debug:
                ServerConsole.AddLog(FormatText($"&7[&b&5{logType}&B&7] &7[&b&2{prefix}&B&7]&r {message}", "7"), ConsoleColor.Magenta);
                break;
        }
#endif
    }

    /// <summary>
    /// Converts a raw message with color tags to formatted message.
    /// </summary>
    /// <param name="message">The raw message.</param>
    /// <param name="defaultColor">The default color of message.</param>
    /// <param name="unityRichText">If its unity richtext or ansi colors.</param>
    /// <returns>Formatted message with colors.</returns>
    private static string FormatText(string message, string defaultColor = "", bool unityRichText = false)
    {
        bool isPrefix = false;
        char escapeChar = (char)27;
        string newText = string.Empty;
        string lastTag = string.Empty;

        if (defaultColor != string.Empty)
        {
            defaultColor = FormatText($"&{defaultColor}", string.Empty, unityRichText);
        }

        for (int x = 0; x < message.Length; x++)
        {
            char currentChar = message[x];
            if (currentChar == '&' && !isPrefix)
            {
                isPrefix = true;
                continue;
            }

            if (isPrefix)
            {
                if (unityRichText && lastTag != string.Empty)
                {
                    switch (currentChar)
                    {
                        case 'r':
                            newText += EndTag(ref lastTag) + $"{defaultColor}";
                            lastTag = "color";
                            continue;
                        case 'm' or 'n' or 'M' or 'N':
                            continue;
                        default:
                            newText += EndTag(ref lastTag);
                            break;
                    }
                }

                newText += unityRichText ? currentChar switch
                {
                    '0' => "<color=black>",
                    '1' => "<color=red>",
                    '2' => "<color=green>",
                    '3' => "<color=yellow>",
                    '4' => "<color=blue>",
                    '5' => "<color=purple>",
                    '6' => "<color=cyan>",
                    '7' => "<color=white>",
                    'b' => "<b>",
                    'B' => "</b>",
                    'o' => "<i>",
                    'O' => "</i>",
                    _ => string.Empty,
                } : escapeChar + currentChar switch
                {
                    '0' => "[30m",
                    '1' => "[31m",
                    '2' => "[32m",
                    '3' => "[33m",
                    '4' => "[34m",
                    '5' => "[35m",
                    '6' => "[36m",
                    '7' => "[37m",
                    'r' => "[0m",
                    'b' => "[1m",
                    'B' => "[22m",
                    'o' => "[3m",
                    'O' => "[23m",
                    'm' => "[4m",
                    'M' => "[24m",
                    'n' => "[9m",
                    'N' => "[29m",
                    _ => string.Empty,
                };

                if (currentChar is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or 'r')
                {
                    lastTag = "color";
                }

                isPrefix = false;
                continue;
            }

            newText += message[x];

            if (unityRichText && x == message.Length - 1 && lastTag != string.Empty)
            {
                newText += EndTag(ref lastTag);
            }
        }

        return newText;
    }

    private static string EndTag(ref string currentTag)
    {
        string saveTag = currentTag;
        currentTag = string.Empty;

        return $"</{saveTag}>";
    }

    private static string GetCallerString(bool includeType = true)
    {
        MethodBase method = GetCallingMethod(1);

        string result = !Identifiers.TryGetValue(GetFullMethodName(method), out (string, string) identifier)
            ? includeType
                ? $"{method.DeclaringType?.Name}::{method.Name}"
                : $"{method.Name}]"
            : includeType
                ? $"{identifier.Item1}::{identifier.Item2}"
                : $"{identifier.Item2}";

        return result;
    }

    private static MethodBase GetCallingMethod(int skip = 0)
    {
        StackTrace stack = new (2 + skip);

        return stack.GetFrame(0).GetMethod();
    }

    private static string GetFullMethodName(MethodBase methodBase)
    {
        return methodBase.DeclaringType?.FullName + "::" + methodBase.Name;
    }

    /// <summary>
    /// A log struct containing basic log information.
    /// </summary>
    public readonly struct LogInfo
    {
        /// <summary>
        /// The log level of this log.
        /// </summary>
        public readonly byte Level;

        /// <summary>
        /// The message of this log.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// The feature name of this log.
        /// </summary>
        public readonly string? FeatureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogInfo"/> struct.
        /// </summary>
        /// <param name="level">The level of the log.</param>
        /// <param name="message">The message of the log.</param>
        /// <param name="featureName">The feature name of this log.</param>
        public LogInfo(LogLevel level, string message, string? featureName)
        {
            this.Level = (byte)level;
            this.Message = message;
            this.FeatureName = featureName;
        }
    }
}
