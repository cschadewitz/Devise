using Devise.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Devise.Generators.Api
{
    public static class DtoGenerator
    {
        #region Pre-Cottle Gen Code
        /*

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
        public static void Generate(GeneratorExecutionContext context, List<ClassDeclarationSyntax> devisableEntities)
        {
            ApiNamespace = context.Compilation.AssemblyName;
            foreach (ClassDeclarationSyntax entity in devisableEntities)
            {
                IEnumerable<PropertyDeclarationSyntax> properties = SyntaxParser.GetEntityProperties(entity);
                if (!entity.Parent.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    //Throw Error to user subclasses not supported
                }
                if (!SyntaxParser.GetEntityAttributes(entity).Any(a => a.Name.ToString() == "DeviseCustom" && a.ArgumentList.Arguments.Any(r => r.Expression.ToString() == "DTO")))
                {
                    var namespaceDeclaration = entity.Parent as NamespaceDeclarationSyntax;
                    StringBuilder sourceBuilder = GetDTOBase();
                    sourceBuilder.Append(entity.Identifier + "DTO\n\t{");
                    foreach (PropertyDeclarationSyntax property in SyntaxParser.GetEntityProperties(entity))
                    {
                        string propertyType = property.Type.ToString();
                        string propertyName = property.Identifier.Text;
                        sourceBuilder.Append($@"
        public {propertyType} {propertyName} {{ get; set; }}");
                    }
                    sourceBuilder.Append(ClassGeneratorHelpers.GetClassEnd());
                    context.AddSource($"{entity.Identifier}DTO.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                }
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
        */
        #endregion
        public static void GenerateCottle(GeneratorExecutionContext context, IEnumerable<ClassDeclarationSyntax> devisableEntities)
        {
            if (devisableEntities is null)
                throw new ArgumentNullException(nameof(devisableEntities));
            string ApiNamespace = context.Compilation.AssemblyName;
            foreach (ClassDeclarationSyntax entity in devisableEntities)
            {
                if (!entity.Parent.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    //Throw Error to user that subclasses are not supported
                }
                string renderedCode = CottleRenderer.Render(
                    TemplateResourceReader.ReadTemplate("dto"), 
                    SyntaxParser.GetEntityCottleContext(entity)
                    );

                context.AddSource($"{entity.Identifier.Text}DTO.g.cs", SourceText.From(renderedCode, Encoding.UTF8));
            }

        }
    }
}
