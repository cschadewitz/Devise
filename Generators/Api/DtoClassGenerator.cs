using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Devise.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Devise.Generators.Api
{
    public static class DtoClassGenerator
    {

        private static string ApiNamespace;
        private static StringBuilder GetDTOBase()
        {

            return new StringBuilder(@"
using System;
using System.Collections.Generic;

namespace " + ApiNamespace + @".DTO
{
    public class ");

        }
        private static IEnumerable<PropertyDeclarationSyntax> GetEntityProperties(SyntaxTree entity)
        {
            IEnumerable<SyntaxNode> propertyNodes = entity.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.PropertyDeclaration));
            foreach(SyntaxNode propertyNode in propertyNodes)
                yield return propertyNode as PropertyDeclarationSyntax;
        }
        public static void Generate(GeneratorExecutionContext context, IEnumerable<SyntaxTree> entities)
        {
            ApiNamespace = context.Compilation.AssemblyName;
            foreach(SyntaxTree entity in entities)
            {
                IEnumerable<PropertyDeclarationSyntax> properties = GetEntityProperties(entity);
                NamespaceDeclarationSyntax namespaceDeclaration = entity.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.NamespaceDeclaration)).FirstOrDefault() as NamespaceDeclarationSyntax;
                ClassDeclarationSyntax classDeclaration = entity.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.ClassDeclaration)).FirstOrDefault() as ClassDeclarationSyntax;
                INamedTypeSymbol classSymbol = context.Compilation.GetTypeByMetadataName(namespaceDeclaration.Name + "." + classDeclaration.Identifier.Text);
                StringBuilder sourceBuilder = GetDTOBase();
                sourceBuilder.Append(classSymbol.Name + "DTO\n\t{");
                foreach (PropertyDeclarationSyntax property in properties)
                {
                    string propertyType = property.Type.ToString();
                    string propertyName = property.Identifier.Text;
                    sourceBuilder.Append($@"
        public {propertyType} {propertyName} {{ get; set; }}");
                }
                sourceBuilder.Append(ClassGeneratorHelpers.GetClassEnd());
                context.AddSource($"{classDeclaration.Identifier.Text}DTO.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));

            }
        }
        public static void Generate(GeneratorExecutionContext context, List<IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax>> entities)
        {
            ApiNamespace = context.Compilation.AssemblyName;
            foreach (IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax> entity in entities)
            {
                NamespaceDeclarationSyntax namespaceDeclaration = entity.Key.Parent as NamespaceDeclarationSyntax;
                StringBuilder sourceBuilder = GetDTOBase();
                sourceBuilder.Append(entity.Key.Identifier + "DTO\n\t{");
                foreach (PropertyDeclarationSyntax property in entity.ToList())
                {
                    string propertyType = property.Type.ToString();
                    string propertyName = property.Identifier.Text;
                    sourceBuilder.Append($@"
        public {propertyType} {propertyName} {{ get; set; }}");
                }
                sourceBuilder.Append(ClassGeneratorHelpers.GetClassEnd());
                context.AddSource($"{entity.Key.Identifier}DTO.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }
    }
}
