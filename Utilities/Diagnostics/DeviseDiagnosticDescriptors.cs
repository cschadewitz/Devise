using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Devise.Generators.Data;

namespace Devise.Utilities.Diagnostics
{
  public static partial class DiagnosticDescriptors
  {
    //DV-1000 level diagnostics are for reporting diagnostic settings level information
    public static readonly DiagnosticDescriptor FileLoggingDisabled = new DiagnosticDescriptor(id: "DV1000",
                                                                                            title: "Diagnostic File Logging Disabled",
                                                                                            messageFormat: "Diagnostic File Logging Enabled build property (Devise_DiagnoticsLoggingFileEnabled) is either false or not present",
                                                                                            category: "DeviseGenerator_Diagnostics",
                                                                                            DiagnosticSeverity.Info,
                                                                                            isEnabledByDefault: true);
    //DV-2XXX level diagnostics are for reporting Devise configuration information
    //DV-20XX level diagnostics are for reporting Devise configuration information related to Build Property style config
    public static readonly DiagnosticDescriptor BuildPropertyNotFound = new DiagnosticDescriptor(id: "DV2000",
                                                                                              title: "{0} is missing",
                                                                                              messageFormat: "The build property {0} is missing",
                                                                                              category: "DeviseGenerator_Diagnostics",
                                                                                              DiagnosticSeverity.Warning,
                                                                                              isEnabledByDefault: true);
    public static readonly DiagnosticDescriptor BuildPropertySetting = new DiagnosticDescriptor(id: "DV2001",
                                                                                              title: "{0} set to {1}",
                                                                                              messageFormat: "The build property {0} is set to {1}",
                                                                                              category: "DeviseGenerator_Diagnostics",
                                                                                              DiagnosticSeverity.Info,
                                                                                              isEnabledByDefault: true);
    //DV-21XX level diagnostics are for reporting Devise configuration information related to Json style config
    public static readonly DiagnosticDescriptor ConfigurationNotFound = new DiagnosticDescriptor(id: "DV2100",
                                                                                              title: "{0} is missing",
                                                                                              messageFormat: "The configuration {0} is missing",
                                                                                              category: "DeviseGenerator_Diagnostics",
                                                                                              DiagnosticSeverity.Warning,
                                                                                              isEnabledByDefault: true);
    public static readonly DiagnosticDescriptor ConfigurationSetting = new DiagnosticDescriptor(id: "DV2101",
                                                                                              title: "{0} set to {1}",
                                                                                              messageFormat: "The configuration {0} is set to {1}",
                                                                                              category: "DeviseGenerator_Diagnostics",
                                                                                              DiagnosticSeverity.Info,
                                                                                              isEnabledByDefault: true);
  }
}
