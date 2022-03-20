using System;
using System.Collections.Generic;
using System.Text;

namespace Devise.Utilities
{
    internal static class DeviseBuildProperties
    {
        internal static string ProjectType { get => "build_property.Devise_ProjectType"; } 
        internal static string DataProjectPath { get => "build_property.Devise_DataProject"; }
        internal static string ApiProjectName { get => "build_property.Devise_ApiProject_Name"; }
        internal static string BusinessProjectName { get => "build_property.Devise_BusinessProject_Name"; }
        internal static string DataProjectName { get => "build_property.Devise_DataProject_Name"; }
        internal static string WebProjectName { get => "build_property.Devise_WebProject_Name"; }
        internal static string IsNullable { get => "build_property.Devise_IsNullable"; }
    }
}
