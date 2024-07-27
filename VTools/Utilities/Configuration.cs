using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace VTools.Utilities
{
    public static class Configuration
    {
        public static CultureInfo[] AvailableCultures { get; } = [new("en-US"), new("ru-RU")];

        public static CultureInfo Culture { get; set; }

        public static void LoadFromFileOrDefault()
        {
            Dictionary<string, string> config;
            try
            {
                config = File.ReadAllLines(fileName)
                    .Select(s => s.Split('='))
                    .Select(arr => (arr[0], arr[1]))
                    .ToDictionary();

                Culture = new CultureInfo(config[nameof(Culture)]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                config = defaultConfig;

                Culture = new CultureInfo(config[nameof(Culture)]);

                SaveToFile();
        }
        }

        public static void SaveToFile()
        {
            var config = new Dictionary<string, string>() {
                { nameof(Culture),           Culture.Name },
            };

            try
            {
                File.WriteAllLines("config", config.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Apply()
        {
            Assets.Resources.Culture = Culture;
        }

        private static readonly Dictionary<string, string> defaultConfig = new() {
            { nameof(Culture),            "en-US" },
        };
        private static readonly string fileName = "config";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static Configuration() => LoadFromFileOrDefault();
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
