using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MyGenerator
{
    [Generator]
    public class ApiGenerator : ISourceGenerator
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
            context.RegisterForPostInitialization((i) => i.AddSource("DeviseAttribute.cs", attributeText));
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            DataNamespace = context.Compilation.AssemblyName;
            ApiNamespace = DataNamespace.Substring(0, DataNamespace.LastIndexOf("."));
            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("Devise.DeviseAttribute");
            foreach (IGrouping<INamedTypeSymbol, IPropertySymbol> devisablePropertyGroup in receiver.DevisableEntities)
            {
                string devisedDTOSource = GenerateDTO(devisablePropertyGroup.Key, devisablePropertyGroup.ToList());
                context.AddSource($"{devisablePropertyGroup.Key.Name}DTO.cs", SourceText.From(devisedDTOSource, Encoding.UTF8));
                //GenerateMapping();
                //GenerateControllerPartial(entityClass);
                //GenerateBusinessPartial(entityClass);
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

        

        private void GenerateControllerPartial(SyntaxTree entityClass)
        {

        }

        private void GenerateBusinessPartial(SyntaxTree entityClass)
        {

        }        

        //Devise Attribute Definition
        private const string attributeText = @"
using System;
namespace Devise
{
    [AttributeUsage(AttributeTargets.Class , Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""DeviseGenerator_DEBUG"")]
    sealed class DeviseAttribute : Attribute
    {
        public DeviseAttribute()
        {
        }
        public string ClassName { get; set; }
    }
}
";

        //Class Base StringBuilders
        private StringBuilder GetDTOBase()
        {

            return new StringBuilder(@"
using System;
using System.Collections.Generic;

namespace " + ApiNamespace + @".DTO
{
    public class ");

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