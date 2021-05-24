using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Devise.Generators;
using Devise.Generators.Data;
using Devise.Generators.Business;
using Devise.Generators.Api;
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
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            // Load Config
            DeviseConfig config = DeviseConfig.LoadConfig(context);

            // Generate based on which assembly is running the generator
            string assemblyName = context.Compilation.AssemblyName;
            if (assemblyName.Contains(".Data"))
            {
                DeviseAttributeGenerator.Generate(context);
                return;
            }
            if (assemblyName.Contains(".Business"))
            {

            }
            if (assemblyName.Contains(".Api"))
            {
                IEnumerable<SyntaxTree> devisableEntities = ProjectLoader.LoadDataProject(config);
                DtoClassGenerator.Generate(context, devisableEntities);
                MappingProfileGenerator.Generate(context, devisableEntities);
            }

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