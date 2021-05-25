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
                INamedTypeSymbol classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
                if (!classSymbol.GetAttributes().Any(a => a.AttributeClass.Name == "Devise"))
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
                if (!classDeclaration.AttributeLists.Any(a => a.Attributes.Any(n => n.Name.ToString() == "Devise")))
                    return null;
                //Get symbols for each Property of the class
                foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)))
                {
                    propertyDeclarations.Add(propertyDeclaration);
                }
            }
            return propertyDeclarations.GroupBy(p => p.Parent as ClassDeclarationSyntax).FirstOrDefault();
        }
    }
}
