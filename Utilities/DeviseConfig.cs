using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace Devise.Utilities
{
    internal sealed class DeviseConfig
    {
        public string ConfigPath { get; set; }
        public string DataProject { get; set; }
        public string ApiProject { get; set; }
        public string BusinessProject { get; set; }
        private DeviseConfig() { }
        public static DeviseConfig LoadConfig(GeneratorExecutionContext context)
        {
            string jsonPath = "";
            try
            {
                jsonPath = context.AdditionalFiles.Single(f => f.Path.EndsWith("DeviseConfig.json")).Path;
            }
            catch
            {
                //TODO: Add notice that more that one DeviseConfig Json file was found
            }
            return FromJsonFile(jsonPath);
        }
        private static DeviseConfig FromJsonFile(string jsonPath)
        {
            //Add proxy config so that each projects config points to a single location to load from 
            //If proxy replace jsonPath with the path to the shared config file
            if (jsonPath == null)
                return null;
            DeviseConfig config = new DeviseConfig() { ConfigPath = Path.GetDirectoryName(jsonPath) };
            JsonConvert.PopulateObject(File.ReadAllText(jsonPath), config);
            return config;
        }
    }
}
