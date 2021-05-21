using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Devise.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Devise
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        private string DataNamespace;
        private string ApiNamespace;
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif 
            //context.RegisterForPostInitialization((i) => i.AddSource("DeviseAttribute.g.cs", attributeText));
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            if (context.Compilation.AssemblyName.Contains(".Data"))
            {
                DeviseAttributeGenerator.Generate(context);
                return;
            }
            if (context.Compilation.AssemblyName.Contains(".Business"))
            {

            }
            if (context.Compilation.AssemblyName.Contains(".Api"))
            {
                string jsonPath = "";
                try
                {
                     jsonPath = context.AdditionalFiles.Single(f => Path.GetFileName(f.Path) == "AutoRouteConfig.Json").Path;
                }
                catch
                {
                    //TODO: Add notice that more that one DeviseConfig Json file was found
                }
                DeviseConfig config = DeviseConfig.FromJsonFile(jsonPath);
                
                //ApiNamespace = context.Compilation.AssemblyName;
                //DataNamespace = ApiNamespace.Substring(0, ApiNamespace.LastIndexOf(".")) + ".Data";
                INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("Devise.DeviseAttribute");
                IEnumerable<SyntaxTree> devisableEntities = ProjectLoader.LoadDataProject(config);
                foreach (IGrouping<INamedTypeSymbol, IPropertySymbol> devisablePropertyGroup in receiver.DevisableEntities)
                {
                    string devisedDTOSource = GenerateDTO(devisablePropertyGroup.Key, devisablePropertyGroup.ToList());
                    context.AddSource($"{devisablePropertyGroup.Key.Name}DTO.g.cs", SourceText.From(devisedDTOSource, Encoding.UTF8));
                    //GenerateControllerPartial(entityClass);
                    //GenerateBusinessPartial(entityClass);
                }
                string devisedMappingPartialSource = GenerateMapping(receiver.DevisableEntities);
                context.AddSource($"MappingProfileApi.g.cs", SourceText.From(devisedMappingPartialSource, Encoding.UTF8));
            }

        }
        private string GenerateDTO(INamedTypeSymbol classSymbol, List<IPropertySymbol> properties)
        {
            StringBuilder sourceBuilder = GetDTOBase();
            sourceBuilder.Append(classSymbol.Name + "DTO\n\t{");
            foreach(IPropertySymbol property in properties)
            {
                ITypeSymbol propertyType = property.Type;
                string propertyName = property.Name;

                sourceBuilder.Append($@"
        public {propertyType} {propertyName} {{ get; set; }}");

            }
            sourceBuilder.Append(GetClassEnd());
            return sourceBuilder.ToString();
        }

        private string GenerateMapping(List<IGrouping<INamedTypeSymbol, IPropertySymbol>> devisableEntities)
        {
            StringBuilder sourceBuilder = GetMappingBase();
            foreach (IGrouping<INamedTypeSymbol, IPropertySymbol> group in devisableEntities)
            {
                string entityName = group.Key.Name;
                string dtoName = group.Key.Name + "DTO";
                sourceBuilder.Append($@"
            CreateMap<{entityName}, {dtoName}>();
            CreateMap<{dtoName}, {entityName}>();");
            }
            sourceBuilder.Append(@"
        }");
            sourceBuilder.Append(GetClassEnd());
            return sourceBuilder.ToString();
        }
        

        private void GenerateControllerPartial(SyntaxTree entityClass)
        {

        }

        private void GenerateBusinessPartial(SyntaxTree entityClass)
        {

        }        

        //Class Base StringBuilders
        

        private StringBuilder GetMappingBase()
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

        //Class End
        private string GetClassEnd()
        {
            return "\n\t}\n}";
        }

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<IGrouping<INamedTypeSymbol, IPropertySymbol>> DevisableEntities = new List<IGrouping<INamedTypeSymbol, IPropertySymbol>>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // any class with at least one attribute is a candidate for property generation
                if (context.Node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0)
                {
                    INamedTypeSymbol classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                    //break if the class does not contain the Devise Attribute
                    if (!classSymbol.GetAttributes().Any(a => a.AttributeClass.ToDisplayString() == "Devise.DeviseAttribute"))
                        return;

                    List<IPropertySymbol> propertySymbols = new();
                    //Get symbols for each Property of the class
                    foreach (PropertyDeclarationSyntax property in classDeclarationSyntax.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)))
                    {
                        propertySymbols.Add(context.SemanticModel.GetDeclaredSymbol(property));
                    }
                    DevisableEntities.Add(propertySymbols.GroupBy(p => p.ContainingType).First());
                }
            }
        }

    }
}