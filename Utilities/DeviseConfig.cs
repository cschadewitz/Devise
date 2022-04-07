using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Devise.Utilities.Diagnostics;

namespace Devise.Utilities
{
    public class DeviseConfig
    {
        public string? ConfigPath { get; set; }
        public string? DataProjectPath { get; set; }
        public DeviseProjectType ProjectType { get; set; }
        public string? ApiProjectName { get; set; }
        public string? BusinessProjectName { get; set; }
        public string? DataProjectName { get; set; }
        public string? WebProjectName { get; set; }
        public bool? Nullable { get; set; }
        private DeviseConfig() { }
        public static DeviseConfig LoadConfig(GeneratorExecutionContext context)
        {
            if(context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(DeviseBuildProperties.ProjectType, out string? projectType))
            {
                if(!Enum.TryParse<DeviseProjectType>(projectType, out DeviseProjectType type))
                {
                    //TODO: Add diagnostics notification that Invalid Project Type
                }
                string? dataProjectPath = LoadBuildProp(DeviseBuildProperties.DataProjectPath, context);
                string? apiProjectName = LoadBuildProp(DeviseBuildProperties.ApiProjectName, context);
                string? dataProjectName = LoadBuildProp(DeviseBuildProperties.DataProjectName, context);
                string? businessProjectName = LoadBuildProp(DeviseBuildProperties.BusinessProjectName, context);
                string? webProjectName = LoadBuildProp(DeviseBuildProperties.WebProjectName, context);
                bool? isNullable = LoadBuildProp<bool>(DeviseBuildProperties.IsNullable, context);
                DeviseConfig config = new() 
                { 
                    ConfigPath = null, 
                    ProjectType = type, 
                    DataProjectPath = dataProjectPath,
                    ApiProjectName = apiProjectName,
                    BusinessProjectName = businessProjectName,
                    DataProjectName = dataProjectName,
                    WebProjectName = webProjectName,
                    Nullable = isNullable

                };
                return config;

            }
            else
            {
                return FromJsonFile(context);
            }
        }

        private static DeviseConfig FromJsonFile(GeneratorExecutionContext context)
        {
            string jsonPath = "";
            try
            {
              jsonPath = context.AdditionalFiles.Single(f => f.Path.EndsWith("DeviseConfig.json")).Path;
            }
            catch (InvalidOperationException)
            {
              //TODO: Add diagnostics notification that more that one DeviseConfig Json file was found
              throw;
            }
            catch (NullReferenceException)
            {
              //TODO: Add diagnostics notification that no configfile was found
              throw;
            }
            //Add proxy config so that each projects config points to a single location to load from 
            //If proxy replace jsonPath with the path to the shared config file
            if (jsonPath == null)
            {
                //TODO: Add diagnostics notification 
                throw new ArgumentNullException(nameof(jsonPath));
            }
            DeviseConfig config = new() { ConfigPath = Path.GetDirectoryName(jsonPath) };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Error;
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(jsonPath), config, settings);
            }
            catch(JsonSerializationException)
            {
                //TODO: Add diagnostics notification that the json file does not contain all required members
                throw;
            }
            //JsonConvert.PopulateObject(File.ReadAllText(jsonPath), config);
            return config;
        }

        private static string LoadBuildProp(string deviseBuildProperty, GeneratorExecutionContext context)
        {

            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(deviseBuildProperty, out string? deviseConfig))
            {
                DeviseDiagnostics.Instance?.Report(DiagnosticDescriptors.BuildPropertyNotFound, null, deviseBuildProperty.Split('.')[1]);
                throw new KeyNotFoundException(deviseBuildProperty);
            }
            return deviseConfig;
        }

        private static T? LoadBuildProp<T>(string deviseBuildProperty, GeneratorExecutionContext context) where T : IConvertible
        {
            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(deviseBuildProperty, out string? deviseConfig))
            {
                DeviseDiagnostics.Instance?.Report(DiagnosticDescriptors.BuildPropertyNotFound, null, deviseBuildProperty.Split('.')[1]);
                throw new KeyNotFoundException(deviseBuildProperty);
            }
            return deviseConfig.ConvertTo<T>();
        }

        public static bool TryLoadBuildProp(string deviseBuildProperty, GeneratorExecutionContext context, out string? buildPropertyValue)
        {
          if (deviseBuildProperty == null)
          {
            buildPropertyValue = default(string);
            return false;
          }
          try
          {
            buildPropertyValue = LoadBuildProp(deviseBuildProperty, context);
            return true;
          }
          catch (KeyNotFoundException)
          {
            buildPropertyValue = default(string);
            return false;
          }
        }

        public static bool TryLoadBuildProp<T>(string deviseBuildProperty, GeneratorExecutionContext context, out T? buildPropertyValue) where T : IConvertible
        {
            if(deviseBuildProperty == null)
            {
              buildPropertyValue = default(T);
              return false;
            }
            try
            {
                buildPropertyValue = LoadBuildProp<T>(deviseBuildProperty, context);
                return true;
            }
            catch(KeyNotFoundException)
            {
                buildPropertyValue = default(T);
                return false;
            }
        }


    }
}