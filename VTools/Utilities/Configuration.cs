using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace VTools.Utilities
{
    public class Configuration
    {
        public static CultureInfo[] AvailableCultures { get; } = [new("en-US"), new("ru-RU")];

        public CultureInfo Culture;

        public static Configuration Load()
        {
            Dictionary<string, string> config;
            try
            {
                config = File.ReadAllLines(configFileName)
                    .Select(s => s.Split('='))
                    .Select(arr => (arr[0], arr[1]))
                    .ToDictionary();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                config = defaultConfig;
            }

            return new Configuration(config);
        }

        public void Save()
        {
            var config = new Dictionary<string, string>() { { nameof(Culture), Culture.Name } };

            try
            {
                File.WriteAllLines("config", config.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Apply()
        {
            Assets.Resources.Culture = Culture;
        }

        private static readonly Dictionary<string, string> defaultConfig = new() { { "Culture", "en-US" } };
        private static readonly string configFileName = "config";

        private Configuration(Dictionary<string, string> config)
        {
            Culture = new CultureInfo(config[nameof(Culture)]);
        }
    }
}
