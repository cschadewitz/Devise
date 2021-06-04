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
        private static bool HasCustom = false;
        private static StringBuilder GetMappingBase()
        {

            return new StringBuilder(@"
using AutoMapper;
using " + DataNamespace + @";
using " + ApiNamespace + @".DTO;

namespace " + ApiNamespace + @"
{
    public " + (HasCustom? "partial " : "") + @"class MappingProfileApi : Profile
    {
        public MappingProfileApi()
        {");

        }
        public static void Generate(GeneratorExecutionContext context, List<ClassDeclarationSyntax> devisableEntities)
        {
            ApiNamespace = context.Compilation.AssemblyName;
            DataNamespace = ApiNamespace.Substring(0, ApiNamespace.LastIndexOf(".")) + ".Data";
            StringBuilder sourceBuilder = GetMappingBase();
            foreach (ClassDeclarationSyntax entity in devisableEntities)
            {
                List<AttributeArgumentSyntax>
                if (SyntaxParser.GetEntityAttributes(entity).Any(a => a.Name.ToString() == "DeviseCustom" && a.ArgumentList.Arguments.Any(r => r.Expression.ToString() == "\"MappingProfile\"")))
                {
                    HasCustom = true;
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
            if (HasCustom)
            {
                sourceBuilder.Append($@"
            CustomMaps();");
            }
            sourceBuilder.Append(@"
        }");
            if(HasCustom)
            {
                sourceBuilder.Append($@"
            public partial static void CustomMaps();");
            }
            sourceBuilder.Append(ClassGeneratorHelpers.GetClassEnd());

            context.AddSource($"MappingProfileApi.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public static void Generate(GeneratorExecutionContext context, List<IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax>> entities)
        {
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
    }
}
