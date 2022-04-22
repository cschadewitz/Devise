using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;

namespace Devise.Utilities
{
  public static class ProjectLoader
  {
    public static IEnumerable<ClassDeclarationSyntax> LoadDataProject(DeviseConfig config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));
      List<ClassDeclarationSyntax> devisableEntities = new();
      string dataProjectDirectory = "";
      if (config.ConfigPath is null)
        dataProjectDirectory = Path.GetFullPath(config.DataProjectPath);
      else
        dataProjectDirectory = Path.GetFullPath(Path.Combine(config.ConfigPath, config.DataProjectPath));
      foreach (var filePath in Directory.GetFiles(dataProjectDirectory, "*.cs", SearchOption.AllDirectories))
      {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
        devisableEntities.AddRange(SyntaxParser.GetDevisableEntities(syntaxTree));
      }
      return devisableEntities;
    }
  }

}
