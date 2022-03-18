using Devise.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Devise.Generators.Api
{
    public static class MappingProfileGenerator
    {

        private static string ApiNamespace { get; set; }
        private static string DataNamespace { get; set; }
        private static bool _HasCustom = false;
        private static StringBuilder GetMappingBase()
        {

            return new StringBuilder(@"
using AutoMapper;
using " + DataNamespace + @";
using " + ApiNamespace + @".DTO;

namespace " + ApiNamespace + @"
{
    public " + (_HasCustom ? "partial " : "") + @"class MappingProfileApi : Profile
    {
        public MappingProfileApi()
        {");

        }
        public static void Generate(GeneratorExecutionContext context, IEnumerable<ClassDeclarationSyntax> devisableEntities)
        {
            if(devisableEntities is null)
                throw new ArgumentNullException(nameof(devisableEntities));
            ApiNamespace = context.Compilation.AssemblyName;
            DataNamespace = ApiNamespace.Substring(0, ApiNamespace.LastIndexOf(".")) + ".Data";
            StringBuilder sourceBuilder = GetMappingBase();
            foreach (ClassDeclarationSyntax entity in devisableEntities)
            {
                //List<AttributeArgumentSyntax>
                if (SyntaxParser.GetEntityAttributes(entity).Any(a => a.Name.ToString() == "DeviseCustom" && a.ArgumentList.Arguments.Any(r => r.Expression.ToString() == "\"Mapping\"")))
                {
                    _HasCustom = true;
                }
                else
                {
                    string entityName = entity.Identifier.Text;
                    string dtoName = entityName + "DTO";
                    sourceBuilder.Append($@"
            CreateMap<{entityName}, {dtoName}>();
            CreateMap<{dtoName}, {entityName}>();");
                }
            }
            if (_HasCustom)
            {
                sourceBuilder.Append($@"
            CustomMaps();");
            }
            sourceBuilder.Append(@"
        }");
            if (_HasCustom)
            {
                sourceBuilder.Append($@"
            partial void CustomMaps();");
            }
            sourceBuilder.Append(ClassGeneratorHelpers.GetClassEnd());

            context.AddSource($"MappingProfileApi.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public static void Generate(GeneratorExecutionContext context, List<IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax>> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            ApiNamespace = context.Compilation.AssemblyName;
            DataNamespace = ApiNamespace.Substring(0, ApiNamespace.LastIndexOf(".")) + ".Data";
            StringBuilder sourceBuilder = GetMappingBase();
            foreach (IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax> entity in entities)
            {
                string entityName = entity.Key.Identifier.Text;
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

        public static void GenerateCottle(GeneratorExecutionContext context, IEnumerable<ClassDeclarationSyntax> devisableEntities)
        {
            if (devisableEntities is null)
                throw new ArgumentNullException(nameof(devisableEntities));
            string renderedCode = CottleRenderer.Render(
                TemplateResourceReader.ReadTemplate("automapper"), 
                SyntaxParser.GetMappingCottleContext(devisableEntities)
                );

            context.AddSource($"MappingProfileApi.g.cs", SourceText.From(renderedCode, Encoding.UTF8));

        }
    }
}
