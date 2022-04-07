using Devise.Generators.Api;
using Devise.Generators.Data;
using Devise.Utilities;
using Devise.Utilities.Diagnostics;
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
      DeviseDiagnostics.CreateInstance(context);
      DeviseConfig config = DeviseConfig.LoadConfig(context);

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
              ControllerGenerator.GenerateCottle(context, config, devisableEntities);
              break;
      }

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