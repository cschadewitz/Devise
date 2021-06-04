using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Devise.Utilities;

namespace Devise.Generators.Business
{
    public static class RepositoryGenerator
    {
        private static string BusinessNamespace;
        private static string DataNamespace;
        private static StringBuilder GetRepositoryBase()
        {

            return new StringBuilder($@"
using System.Collections.Generic;
using {DataNamespace};

namespace {BusinessNamespace}
{{
    public partial class ");

        }
        private static IEnumerable<PropertyDeclarationSyntax> GetEntityProperties(SyntaxTree entity)
        {
            IEnumerable<SyntaxNode> propertyNodes = entity.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.PropertyDeclaration));
            foreach (SyntaxNode propertyNode in propertyNodes)
                yield return propertyNode as PropertyDeclarationSyntax;
        }
        public static void Generate(GeneratorExecutionContext context, IEnumerable<ClassDeclarationSyntax> devisableEntities)
        {
            BusinessNamespace = context.Compilation.AssemblyName;
            DataNamespace = BusinessNamespace.Substring(0, BusinessNamespace.LastIndexOf(".")) + ".Data";
            foreach (ClassDeclarationSyntax entity in devisableEntities)
            {
                StringBuilder sourceBuilder = GetRepositoryBase();
                sourceBuilder.Append($"{entity.Identifier}Repository : IRepository<{entity.Identifier}>\n\t{{");

                sourceBuilder.Append(ClassGeneratorHelpers.GetClassEnd());
            }
        }
    }
}
