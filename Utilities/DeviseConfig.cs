using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Devise.Utilities
{
    public class DeviseConfig
    {
        public string ConfigPath { get; set; }
        public string DataProject { get; set; }
        private DeviseConfig() { }
        public static DeviseConfig FromJsonFile(string jsonPath)
        {
            if (jsonPath == null)
                return null;
            DeviseConfig config = new DeviseConfig() { ConfigPath = Path.GetDirectoryName(jsonPath) };
            JsonConvert.PopulateObject(File.ReadAllText(jsonPath), config);
            return config;
        }
    }
}
