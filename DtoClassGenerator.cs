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

namespace Devise
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
        public static void Generate(GeneratorExecutionContext context, SyntaxTree entity)
        {
            ApiNamespace = context.Compilation.AssemblyName;
            StringBuilder sourceBuilder = GetDTOBase();
            GetEntityDetails
            sourceBuilder.Append(classSymbol.Name + "DTO\n\t{");
            foreach (IPropertySymbol property in properties)
            {
                ITypeSymbol propertyType = property.Type;
                string propertyName = property.Name;

                sourceBuilder.Append($@"
        public {propertyType} {propertyName} {{ get; set; }}");

            }
            sourceBuilder.Append(GetClassEnd());
            context.AddSource($"{devisablePropertyGroup.Key.Name}DTO.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
