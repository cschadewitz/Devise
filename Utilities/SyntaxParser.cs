using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devise.Utilities
{
    internal static class SyntaxParser
    {
        internal static IGrouping<INamedTypeSymbol, IPropertySymbol> GetDevisableEntityDetails(GeneratorSyntaxContext context)
        {
            List<IPropertySymbol> propertySymbols = new();
            if (context.Node is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.AttributeLists.Count > 0)
            {
                if (!GetEntityAttributes(classDeclaration).Any(a => a.Name.ToString() == "Devise"))
                    return null;
                //Get symbols for each Property of the class
                foreach (PropertyDeclarationSyntax property in classDeclaration.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)))
                {
                    propertySymbols.Add(context.SemanticModel.GetDeclaredSymbol(property));

                }
            }
            return propertySymbols.GroupBy(p => p.ContainingType).FirstOrDefault();
        }
        internal static IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax> GetDevisableEntityDetails(GeneratorExecutionContext context, SyntaxNode node)
        {
            List<PropertyDeclarationSyntax> propertyDeclarations = new();
            if (node is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.AttributeLists.Count > 0)
            {
                if (!GetEntityAttributes(classDeclaration).Any(a => a.Name.ToString() == "Devise"))
                    return null;
                //Get symbols for each Property of the class
                foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)))
                {
                    propertyDeclarations.Add(propertyDeclaration);
                }
            }
            return propertyDeclarations.GroupBy(p => p.Parent as ClassDeclarationSyntax).FirstOrDefault();
        }

        internal static List<ClassDeclarationSyntax> GetDevisableEntities(GeneratorExecutionContext context)
        {
            List<ClassDeclarationSyntax> devisableEntities = new();
            foreach(SyntaxTree tree in context.Compilation.SyntaxTrees.ToList())
            {
                devisableEntities.AddRange(GetDevisableEntities(tree));
            }
            return devisableEntities;
        }

        internal static List<ClassDeclarationSyntax> GetDevisableEntities(SyntaxTree syntaxTree)
        {
            List<ClassDeclarationSyntax> devisableEntities = new();
            foreach (SyntaxNode classNode in syntaxTree.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.ClassDeclaration)))
            {
                if(classNode is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.AttributeLists.Count > 0 && 
                GetEntityAttributes(classDeclaration).Any(a => a.Name.ToString() == "Devise"))
                { 
                    devisableEntities.Add(classDeclaration);
                }
            }
            return devisableEntities;
        }

        internal static List<AttributeSyntax> GetEntityAttributes(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.AttributeLists.SelectMany(l => l.Attributes).ToList();
        }

        internal static IEnumerable<PropertyDeclarationSyntax> GetEntityProperties(ClassDeclarationSyntax classDeclaration)
        {
            foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)))
            {
                yield return propertyDeclaration;
            }
        }
    }
}
