﻿using Cottle;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;


namespace Devise.Utilities
{
    public static class SyntaxParser
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

        public static IEnumerable<ClassDeclarationSyntax> GetDevisableEntities(SyntaxTree syntaxTree)
        {
            if(syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));
            //List<ClassDeclarationSyntax> devisableEntities = new();
            foreach (SyntaxNode classNode in syntaxTree.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.ClassDeclaration)))
            {
                if (classNode is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.AttributeLists.Count > 0 &&
                GetEntityAttributes(classDeclaration).Any(a => a.Name.ToString() == "Devise"))
                {
                    yield return classDeclaration;
                }
            }
            //return devisableEntities;
        }

        public static IContext GetEntityCottleContext(DeviseConfig config, ClassDeclarationSyntax classDeclaration)
        {
            if(classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));
            if(config == null)
                throw new ArgumentNullException(nameof(config));
            var entityAttributes = GetEntityAttributes(classDeclaration);
            var entityProperties = GetEntityProperties(classDeclaration);

            IEnumerable<KeyValuePair<Value, Value>> properties = GetEntityCottleProperties(entityProperties);
            Dictionary<string, IEnumerable<KeyValuePair<Value, Value>>> customAttributes = GetEntityCottleAttributes(entityAttributes);

            //if(classDeclaration)
            //var namespaceName = classDeclaration.Parent is not null ? 
            //    (classDeclaration.Parent as NamespaceDeclarationSyntax).Name.ToString() :
            //    ;
            //var baseNamespace = namespaceName.Substring(0, namespaceName.LastIndexOf('.'));

            var entityContext = Context.CreateBuiltin(new Dictionary<Value, Value>
            {
                ["NullableTypes"] = Value.True,
                ["ApiNamespace"] = config.ApiProjectName,
                ["BusinessNamespace"] = config.BusinessProjectName,
                ["DataNamespace"] = config.DataProjectName,
                ["EntityName"] = classDeclaration.Identifier.ToString(),
                ["ApiCustom"] = Value.FromEnumerable(customAttributes["Api"]),
                ["BusinessCustom"] = Value.FromEnumerable(customAttributes["Business"]),
                ["DTOCustom"] = customAttributes.ContainsKey("DTO") ? Value.True : Value.False,
                ["MappingCustom"] = customAttributes.ContainsKey("Mapping") ? Value.True : Value.False,
                ["EntityProperties"] = Value.FromEnumerable(properties)
            });
            return entityContext;
        }

        public static IContext GetMappingCottleContext(DeviseConfig config, IEnumerable<ClassDeclarationSyntax> classDeclarations)
        {
            var entityNames = classDeclarations.Select(entity => Value.FromString(entity.Identifier.ToString()));
            //TODO: Use LINQ to check if a custom mapping is needed
            //var attributes = classDeclarations.Select(e => e.AttributeLists)
            var hasCustomMapping = false;

            //var namespaceName = (classDeclarations.First().Parent as NamespaceDeclarationSyntax).Name.ToString();
            //var baseNamespace = namespaceName.Substring(0, namespaceName.LastIndexOf('.'));
            var mappingContext = Context.CreateBuiltin(new Dictionary<Value, Value>
            {
                ["NullableTypes"] = Value.True,
                ["ApiNamespace"] = config.ApiProjectName,
                ["BusinessNamespace"] = config.BusinessProjectName,
                ["DataNamespace"] = config.DataProjectName,
                ["EntityNames"] = Value.FromEnumerable(entityNames),
                ["MappingHasCustom"] = Value.FromBoolean(hasCustomMapping)
            });
            return mappingContext;
        }

        internal static IEnumerable<AttributeSyntax> GetEntityAttributes(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.AttributeLists.SelectMany(l => l.Attributes);
        }

        internal static IEnumerable<PropertyDeclarationSyntax> GetEntityProperties(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.Members.Where(m => m.IsKind(SyntaxKind.PropertyDeclaration)).Select(p => p as PropertyDeclarationSyntax);
        }

        internal static Dictionary<string, IEnumerable<KeyValuePair<Value, Value>>> GetEntityCottleAttributes(IEnumerable<AttributeSyntax> entityAttributes)
        {
            Dictionary<string, IEnumerable<KeyValuePair<Value, Value>>> customAttributes = new Dictionary<string, IEnumerable<KeyValuePair<Value, Value>>>();
            foreach (AttributeSyntax attribute in entityAttributes)
            {
                if (attribute.Name.ToString() == "DeviseCustom")
                {
                    List<AttributeArgumentSyntax> arguments = attribute.ArgumentList.Arguments.ToList();
                    string customAttributeTarget = arguments[0].Expression.ToString().TrimQuotes();
                    Dictionary<Value, Value> customAttributeArgs = new Dictionary<Value, Value>();

                    for (int i = 1; i < arguments.Count; i++)
                    {
                        customAttributeArgs.Add(arguments[i].NameColon.Name.ToString().Capitalize(), bool.Parse(arguments[i].Expression.ToString()));
                    }
                    //If target is Api or Business, fill with false for unstated operations
                    if(customAttributeTarget.Equals("Api") || customAttributeTarget.Equals("Business"))
                        foreach (string operation in (string[])Enum.GetNames(typeof(DeviseOperation)))
                        {
                            if (!customAttributeArgs.ContainsKey(operation.Capitalize()))
                            {
                                customAttributeArgs.Add(operation.Capitalize(), false);
                            }
                        }

                    customAttributes.Add(customAttributeTarget, customAttributeArgs.ToList());
                }
            }

            //Add Api and/or Business, fill with false for operations
            foreach (string target in new string[] {"Api", "Business"})
            {
                if (!customAttributes.ContainsKey(target))
                {
                    List<KeyValuePair<Value, Value>> customAttributeArgs = new List<KeyValuePair<Value, Value>>();
                    foreach (string operation in (string[])Enum.GetNames(typeof(DeviseOperation)))
                    {
                        customAttributeArgs.Add(new KeyValuePair<Value, Value>(operation.Capitalize(), false));
                    }
                    customAttributes.Add(target.Capitalize(), customAttributeArgs);
                }
            }
            return customAttributes;
        }
        private static string TrimQuotes(this string name)
        {
            return name.Trim(new char[] { '"' });
        }
        private static string Capitalize(this string name)
        {
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        internal static IEnumerable<KeyValuePair<Value, Value>> GetEntityCottleProperties(IEnumerable<PropertyDeclarationSyntax> entityProperties)
        {
            //var properties = new List<KeyValuePair<Value, Value>>();
            foreach (PropertyDeclarationSyntax prop in entityProperties)
            {
                //properties.Add(new KeyValuePair<Value, Value>(prop.Identifier.Text, prop.Type.ToString()));
                yield return new KeyValuePair<Value, Value>(prop.Identifier.Text, prop.Type.ToString());
            }
            //return properties;
        }
    }
}
