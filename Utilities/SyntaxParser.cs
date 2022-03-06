using Cottle;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Devise.Utilities
{
    internal static class SyntaxParser
    {
        //internal static IGrouping<INamedTypeSymbol, IPropertySymbol> GetDevisableEntityDetails(GeneratorSyntaxContext context)
        //{
        //    List<IPropertySymbol> propertySymbols = new();
        //    if (context.Node is ClassDeclarationSyntax classDeclaration &&
        //        classDeclaration.AttributeLists.Count > 0)
        //    {
        //        if (!GetEntityAttributes(classDeclaration).Any(a => a.Name.ToString() == "Devise"))
        //            return null;
        //        //Get symbols for each Property of the class
        //        foreach (PropertyDeclarationSyntax property in classDeclaration.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)))
        //        {
        //            propertySymbols.Add(context.SemanticModel.GetDeclaredSymbol(property));
        //
        //        }
        //    }
        //    return propertySymbols.GroupBy(p => p.ContainingType).FirstOrDefault();
        //}
        //internal static IGrouping<ClassDeclarationSyntax, PropertyDeclarationSyntax> GetDevisableEntityDetails(GeneratorExecutionContext context, SyntaxNode node)
        //{
        //    List<PropertyDeclarationSyntax> propertyDeclarations = new();
        //    if (node is ClassDeclarationSyntax classDeclaration &&
        //        classDeclaration.AttributeLists.Count > 0)
        //    {
        //        if (!GetEntityAttributes(classDeclaration).Any(a => a.Name.ToString() == "Devise"))
        //            return null;
        //        //Get symbols for each Property of the class
        //        foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)))
        //        {
        //            propertyDeclarations.Add(propertyDeclaration);
        //        }
        //    }
        //    return propertyDeclarations.GroupBy(p => p.Parent as ClassDeclarationSyntax).FirstOrDefault();
        //}

        //internal static List<ClassDeclarationSyntax> GetDevisableEntities(GeneratorExecutionContext context)
        //{
        //    List<ClassDeclarationSyntax> devisableEntities = new();
        //    foreach (SyntaxTree tree in context.Compilation.SyntaxTrees.ToList())
        //    {
        //        devisableEntities.AddRange(GetDevisableEntities(tree));
        //    }
        //    return devisableEntities;
        //}

        internal static List<ClassDeclarationSyntax> GetDevisableEntities(SyntaxTree syntaxTree)
        {
            List<ClassDeclarationSyntax> devisableEntities = new();
            foreach (SyntaxNode classNode in syntaxTree.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.ClassDeclaration)))
            {
                if (classNode is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.AttributeLists.Count > 0 &&
                GetEntityAttributes(classDeclaration).Any(a => a.Name.ToString() == "Devise"))
                {
                    devisableEntities.Add(classDeclaration);
                }
            }
            return devisableEntities;
        }

        internal static IContext GetEntityCottleContext(ClassDeclarationSyntax classDeclaration)
        {
            var entityAttributes = GetEntityAttributes(classDeclaration);
            var entityProperties = GetEntityProperties(classDeclaration);

            IEnumerable<KeyValuePair<Value, Value>> properties = GetEntityCottleProperties(entityProperties);
            Dictionary<string, IEnumerable<KeyValuePair<Value, Value>>> customAttributes = GetEntityCottleAttributes(entityAttributes);

            var namespaceName = (classDeclaration.Parent as NamespaceDeclarationSyntax).Name.ToString();
            var baseNamespace = namespaceName.Substring(0, namespaceName.LastIndexOf('.'));

            var entityContext = Context.CreateBuiltin(new Dictionary<Value, Value>
            {
                ["ApiNamespace"] = baseNamespace + ".Api",
                ["BusinessNamespace"] = baseNamespace + ".Business",
                ["DataNamespace"] = baseNamespace + ".Data",
                ["EntityName"] = classDeclaration.Identifier.ToString(),
                ["ApiCustom"] = Value.FromEnumerable(customAttributes["Api"]),
                ["BusinessCustom"] = Value.FromEnumerable(customAttributes["Business"]),
                ["Properties"] = Value.FromEnumerable(properties)
            });
            return entityContext;
        }

        internal static IEnumerable<AttributeSyntax> GetEntityAttributes(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.AttributeLists.SelectMany(l => l.Attributes).ToList();
        }

        internal static IEnumerable<PropertyDeclarationSyntax> GetEntityProperties(ClassDeclarationSyntax classDeclaration)
        {
            foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)))
            {
                yield return propertyDeclaration;
            }
        }

        internal static Dictionary<string, IEnumerable<KeyValuePair<Value, Value>>> GetEntityCottleAttributes(IEnumerable<AttributeSyntax> entityAttributes)
        {
            Dictionary<string, IEnumerable<KeyValuePair<Value, Value>>> customAttributes = new Dictionary<string, IEnumerable<KeyValuePair<Value, Value>>>();
            foreach (AttributeSyntax attribute in entityAttributes)
            {
                if (attribute.Name.ToString() == "DeviseCustom")
                {
                    List<AttributeArgumentSyntax> arguments = attribute.ArgumentList.Arguments.ToList();
                    string customAttributeTarget = arguments[0].Expression.ToString().Capitalize();
                    Dictionary<Value, Value> customAttributeArgs = new Dictionary<Value, Value>();
                    for (int i = 1; i < arguments.Count; i++)
                    {
                        customAttributeArgs.Add(arguments[i].NameColon.Name.ToString().Capitalize(), bool.Parse(arguments[i].Expression.ToString()));
                    }
                    foreach (string operation in (string[])Enum.GetNames(typeof(Operation)))
                    {
                        if (!customAttributeArgs.ContainsKey(operation.Capitalize()))
                        {
                            customAttributeArgs.Add(operation.Capitalize(), false);
                        }
                    }
                    customAttributes.Add(customAttributeTarget, customAttributeArgs.ToList());
                }
            }
            foreach (string target in (string[])Enum.GetNames(typeof(DeviseTarget)))
            {
                if (!customAttributes.ContainsKey(target))
                {
                    List<KeyValuePair<Value, Value>> customAttributeArgs = new List<KeyValuePair<Value, Value>>();
                    foreach (string operation in (string[])Enum.GetNames(typeof(Operation)))
                    {
                        customAttributeArgs.Add(new KeyValuePair<Value, Value>(operation.Capitalize(), false));
                    }
                    customAttributes.Add(target.Capitalize(), customAttributeArgs);
                }
            }
            return customAttributes;
        }
        private static string Capitalize(this string name)
        {
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        internal static IEnumerable<KeyValuePair<Value, Value>> GetEntityCottleProperties(IEnumerable<PropertyDeclarationSyntax> entityProperties)
        {
            var properties = new List<KeyValuePair<Value, Value>>();
            foreach (PropertyDeclarationSyntax prop in entityProperties)
            {
                properties.Add(new KeyValuePair<Value, Value>(prop.Identifier.ToString(), prop.Type.ToString()));
            }
            return properties;
        }
    }
}
