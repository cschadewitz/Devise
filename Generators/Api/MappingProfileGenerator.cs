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
    public static class MappingProfileGenerator
    {

        private static string ApiNamespace;
        private static string DataNamespace;
        private static StringBuilder GetMappingBase()
        {

            return new StringBuilder(@"
using AutoMapper;
using " + DataNamespace + @";
using " + ApiNamespace + @".DTO;

namespace " + ApiNamespace + @"
{
    public class MappingProfileApi : Profile
    {
        public MappingProfileApi()
        {");

        }
        public static void Generate(GeneratorExecutionContext context, IEnumerable<SyntaxTree> entities)
        {
            ApiNamespace = context.Compilation.AssemblyName;
            DataNamespace = ApiNamespace.Substring(0, ApiNamespace.LastIndexOf(".")) + ".Data";
            StringBuilder sourceBuilder = GetMappingBase();
            foreach (SyntaxTree entity in entities)
            {
                NamespaceDeclarationSyntax namespaceDeclaration = entity.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.NamespaceDeclaration)).FirstOrDefault() as NamespaceDeclarationSyntax;
                ClassDeclarationSyntax classDeclaration = entity.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.ClassDeclaration)).FirstOrDefault() as ClassDeclarationSyntax;
                INamedTypeSymbol classSymbol = context.Compilation.GetTypeByMetadataName(namespaceDeclaration.Name + "." + classDeclaration.Identifier.Text);
                string entityName = classSymbol.Name;
                string dtoName = entityName + "DTO";
                sourceBuilder.Append($@"
            CreateMap<{entityName}, {dtoName}>();
            CreateMap<{dtoName}, {entityName}>();");
            }
            sourceBuilder.Append(@"
        }");
            sourceBuilder.Append(ClassGeneratorHelpers.GetClassEnd());

            context.AddSource($"MappingProfileApi.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
