﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Devise.Utilities
{
    public static class ProjectLoader
    {
        public static IEnumerable<SyntaxTree> LoadDataProject(DeviseConfig config)
{
            string dataProjectDirectory = Path.GetFullPath(Path.Combine(config.ConfigPath, config.DataProject));
            foreach (var filePath in Directory.GetFiles(dataProjectDirectory, "*.cs", SearchOption.AllDirectories))
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
                SyntaxNode classDeclaration = syntaxTree.GetRoot().DescendantNodes().FirstOrDefault(x => x.IsKind(SyntaxKind.ClassDeclaration));
                if (classDeclaration is ClassDeclarationSyntax classDeclarationSyntax &&
                    classDeclarationSyntax.AttributeLists.Count > 0 &&
                    classDeclarationSyntax.DescendantNodes().Any(n => n.IsKind(SyntaxKind.Attribute) && ((AttributeSyntax)n).Name.ToString() == "Devise"))
                {
                    yield return syntaxTree;
                }
            }
        }
    }
    
}