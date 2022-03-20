using Devise.Generators.Api;
using Devise.Generators.Data;
using Devise.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Devise
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            //context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            //if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
            //    return;

            // Load Config
            DeviseConfig config = DeviseConfig.LoadConfig(context);

            //Pull props from .target file or from .csproj of consuming assembly
            //context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.Devise_DataProject", out string dataProjectPath);
            //
            //context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.Devise_ApiProject_Name", out string apiProjectName);
            //
            //context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.Devise_BusinessProject_Name", out string businessProjectName);
            //
            //context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.Devise_DataProject_Name", out string dataProjectName);
            //
            //context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.Devise_WebProject_Name", out string webProjectName);

            // Generate based on which assembly is running the generator
            //string assemblyName = context.Compilation.AssemblyName;
            switch(config.ProjectType)
            {
                case DeviseProjectType.Data:
                    DeviseAttributeGenerator.Generate(context);
                    DeviseCustomAttributeGenerator.Generate(context);
                    //EnumGenerator.Generate(context);
                    break;
                case DeviseProjectType.Business:
                    break;
                case DeviseProjectType.Api:
                    IEnumerable<ClassDeclarationSyntax> devisableEntities = ProjectLoader.LoadDataProject(config);
                    DtoGenerator.GenerateCottle(context, config, devisableEntities);
                    MappingProfileGenerator.GenerateCottle(context, config, devisableEntities);
                    break;
            }
            //if (assemblyName.EndsWith(dataProjectName))
            //{
            //    DeviseAttributeGenerator.Generate(context);
            //    DeviseCustomAttributeGenerator.Generate(context);
            //    //EnumGenerator.Generate(context);
            //    return;
            //}
            //if (assemblyName.EndsWith(businessProjectName))
            //{
            //
            //}
            //if (assemblyName.EndsWith(apiProjectName))
            //{
            //    IEnumerable<ClassDeclarationSyntax> devisableEntities = ProjectLoader.LoadDataProject(config);
            //    DtoGenerator.GenerateCottle(context, devisableEntities);
            //    MappingProfileGenerator.GenerateCottle(context, devisableEntities);
            //
            //}

        }

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        //class SyntaxReceiver : ISyntaxContextReceiver
        //{
        //    public List<IGrouping<INamedTypeSymbol, IPropertySymbol>> DevisableEntities = new();
        //
        //    /// <summary>
        //    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        //    /// </summary>
        //    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        //    {
        //        IGrouping<INamedTypeSymbol, IPropertySymbol> entity = SyntaxParser.GetDevisableEntityDetails(context);
        //        if (entity is not null)
        //            DevisableEntities.Add(entity);
        //    }
        //}

    }
}