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
using System.Linq;
using System.Reflection;
using Features;
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
    private static readonly Dictionary<string, bool> DebugModes = new();
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
    /// Logs an info message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Info(string message)
    {
        LogMessage($"{GetCallerString()}{message}", Assembly.GetCallingAssembly(), LogLevel.Info);
    }

    /// <summary>
    /// Logs a warn message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Warn(string message)
    {
        LogMessage($"{GetCallerString(true, true)}{message}", Assembly.GetCallingAssembly(), LogLevel.Warn);
    }

    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Error(string message)
    {
        LogMessage($"{GetCallerString()}{message}", Assembly.GetCallingAssembly(), LogLevel.Error);
    }

    /// <summary>
    /// Logs a debug message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="showTrace">Indicates whether the trace should be shown.</param>
    public static void Debug(string message, bool showTrace = true)
    {
        Assembly assembly = Assembly.GetCallingAssembly();
        if (!DebugModeEnabled(assembly))
        {
            return;
        }

        LogMessage($"{GetCallerString(true, showTrace)}{message}", assembly, LogLevel.Debug);
    }

    /// <summary>
    /// Logs an exception to the console.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    public static void Exception(Exception ex)
    {
        LogMessage($"{GetCallerString(true, true)}An error has occured while processing the method.", Assembly.GetCallingAssembly(), LogLevel.Error);
    }

    /// <summary>
    /// Logs an info message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void FeatureInfo(string message)
    {
        LogMessage($"&4{this.LogName}&7-{GetCallerString(false)} {message}", Assembly.GetCallingAssembly(), LogLevel.Info, this);
    }

    /// <summary>
    /// Logs a warn message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void FeatureWarn(string message)
    {
        LogMessage($"&4{this.LogName}&7-{GetCallerString(false, true)} {message}", Assembly.GetCallingAssembly(), LogLevel.Warn, this);
    }

    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void FeatureError(string message)
    {
        LogMessage($"&4{this.LogName}&7-{GetCallerString(false)} {message}", Assembly.GetCallingAssembly(), LogLevel.Error, this);
    }

    /// <summary>
    /// Logs a debug message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void FeatureDebug(string message)
    {
        Assembly assembly = Assembly.GetCallingAssembly();
        if (!DebugModeEnabled(assembly))
        {
            return;
        }

        LogMessage($"&4{this.LogName}&7-{GetCallerString(false, true)} {message}", assembly, LogLevel.Debug, this);
    }

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

    private static bool DebugModeEnabled(Assembly assembly)
    {
        if (DebugModes.ContainsKey(assembly.FullName))
        {
            return DebugModes[assembly.FullName];
        }

        if (BananaPlugin.BananaPlugins.FirstOrDefault(x => x.Assembly == assembly) is not { } plugin)
        {
            return true;
        }

        DebugModes.Add(assembly.FullName, plugin.Config.Debug);
        return true;
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
                    '0' => "[30m", // Black
                    '1' => "[31m", // Red
                    '2' => "[32m", // Green
                    '3' => "[33m", // Yellow
                    '4' => "[34m", // Blue
                    '5' => "[35m", // Purple
                    '6' => "[36m", // Cyan
                    '7' => "[37m", // Gray
                    'r' => "[0m",  // reset / White
                    'b' => "[1m",  // bold opener
                    'B' => "[22m", // bold closer
                    'o' => "[3m",  // italics opener
                    'O' => "[23m", // italics closer
                    'm' => "[4m",  // underline opener
                    'M' => "[24m", // underline closer
                    'n' => "[9m",  // strikeout opener
                    'N' => "[29m", // strikeout closer
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

    private static string GetCallerString(bool includeType = true, bool showTraces = false, bool showParamNames = false)
    {
        if (!showTraces)
        {
            return string.Empty;
        }

        MethodBase method = GetCallingMethod(1);
        string paramInfo = string.Empty;
        foreach (ParameterInfo parameter in method.GetParameters())
        {
            paramInfo += $"&5{parameter.ParameterType.GetTypeString()}{(showParamNames ? $" &r{parameter.Name}" : string.Empty)}&7, ";
        }

        if (paramInfo.Length > 0)
        {
            paramInfo = paramInfo.Substring(0, paramInfo.Length - 2);
        }

        string result = !Identifiers.TryGetValue(GetFullMethodName(method), out (string, string) identifier)
            ? includeType
                ? $"&7[&6{method.DeclaringType?.Name}&r.&2{method.Name}&r({paramInfo}&r)&7] "
                : $"&7[&2{method.Name}&r({paramInfo}&r)&7] "
            : includeType
                ? $"&7[&6{identifier.Item1}&r.&2{identifier.Item2}&7] "
                : $"&7[&2{identifier.Item2}&7] ";

        return result;
    }

    private static MethodBase GetCallingMethod(int skip = 0)
    {
        StackTrace stack = new (2 + skip);

        return stack.GetFrame(0).GetMethod();
    }

    private static string GetFullMethodName(MethodBase methodBase)
    {
        return methodBase.DeclaringType?.FullName + "." + methodBase.Name;
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
