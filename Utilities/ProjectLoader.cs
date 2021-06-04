using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.NetworkInformation;

namespace Devise.Utilities
{
    public static class ProjectLoader
    {
        internal static List<ClassDeclarationSyntax> LoadDataProject(GeneratorExecutionContext context, DeviseConfig config)
        {
            List<ClassDeclarationSyntax> devisableEntities = new();
            string dataProjectDirectory = Path.GetFullPath(Path.Combine(config.ConfigPath, config.DataProject));
            foreach (var filePath in Directory.GetFiles(dataProjectDirectory, "*.cs", SearchOption.AllDirectories))
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
                devisableEntities.AddRange(SyntaxParser.GetDevisableEntities(syntaxTree));               
            }
            return devisableEntities;
        }
        internal static List<IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax>> LoadDataProjectParsed(GeneratorExecutionContext context, DeviseConfig config)
{
            string dataProjectDirectory = Path.GetFullPath(Path.Combine(config.ConfigPath, config.DataProject));
            List<IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax>> devisableEntities = new();
            foreach (var filePath in Directory.GetFiles(dataProjectDirectory, "*.cs", SearchOption.AllDirectories))
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
                SyntaxNode node = syntaxTree.GetRoot().DescendantNodes().FirstOrDefault(x => x.IsKind(SyntaxKind.ClassDeclaration));
                IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax> entity = SyntaxParser.GetDevisableEntityDetails(context, node);
                if (entity is not null)
                    devisableEntities.Add(entity);
            }
            return devisableEntities;
        }
    }
    
}
