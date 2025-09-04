using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace YTDLP.Dotnet.GUI.Utilities;

[AttributeUsage(AttributeTargets.Property)]
public class SettingAttribute(string defaultValue) : Attribute
{
    public string DefaultValue { get; } = defaultValue;
}

public static class Configuration
{
    public static readonly string[] AvailableCultureNames = ["en-US", "ru-RU"];

    public enum SetResult
    {
        Success,
        NoSuchSettingError
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Setting("en-US")]       public static string CultureName    { get; private set; }
    [Setting("downloads")]   public static string DownloadPath   { get; private set; }
    [Setting("edits")]       public static string EditsPath      { get; private set; }
    [Setting("false")]       public static string IsProxyEnabled { get; private set; }
    [Setting("")]            public static string Proxy          { get; private set; }
    // Hidden
    [Setting("")]            public static string POToken        { get; private set; }
    [Setting("")]            public static string CookiesPath    { get; private set; }
    [Setting("yt-dlp.exe")]  public static string YTDLPPath      { get; private set; }
    [Setting("ffmpeg.exe")]  public static string FFmpegPath     { get; private set; }
    [Setting("ffprobe.exe")] public static string FFprobePath    { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static bool IsProxyEnabledAsBool
    {
        get => IsProxyEnabled.IsTrue();
        private set => IsProxyEnabled = value ? "true" : "false"; 
    }
    
    public static void LoadFromFileOrDefault()
    {
        try
        {
            var config = File.ReadAllLines(FileName)
                .Select(line => line.Split('='))
                .Select(split => (split[0], split[1]))
                .ToDictionary();

            var namesIntersect = config
                .Select(kvp => kvp.Key)
                .Intersect(SettingProperties.Select(p => p.Name))
                .ToImmutableArray();

            if (namesIntersect.Length != SettingProperties.Count())
            {
                throw new InvalidDataException();
            }

            var namesValues = namesIntersect.Join(
                config,
                name => name,
                kvp => kvp.Key,
                (name, kvp) => (name, kvp.Value));

            foreach (var (name, value) in namesValues)
            {
                Set(name, value);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());

            foreach (var property in SettingProperties)
            {
                Set(property.Name, property.GetCustomAttribute<SettingAttribute>()!.DefaultValue);
            }

            SaveToFile();
        }
    }

    public static void SaveToFile()
    {
        try
        {
            File.WriteAllLines(
                FileName,
                SettingProperties.Select(property => $"{property.Name}={property.GetValue(null)}"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static SetResult Set(string name, string value)
    {
        var property = SettingProperties.FirstOrDefault(p => p.Name == name);
        if (property == null)
        {
            return SetResult.NoSuchSettingError;
        }

        switch (name)
        {
            case nameof(CultureName):
                if (AvailableCultureNames.Contains(value))
                {
                    CultureName = value;
                    Assets.Resources.Culture = new CultureInfo(CultureName);
                }

                break;

            default:
                property.SetValue(null, value);
                break;
        }

        SaveToFile();
        return SetResult.Success;
    }

    private const string FileName = "config.ini";

    private static readonly IEnumerable<PropertyInfo> SettingProperties = typeof(Configuration)
        .GetProperties()
        .Where(p => p.GetCustomAttribute<SettingAttribute>() != null);
}