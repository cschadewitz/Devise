using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Devise.Generators.Data
{

    public static class DeviseIgnoreAttributeGenerator
    {
        //Devise Attribute Definition
        private const string deviseIgnoreAttributeText = @"
using System;
namespace Devise
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    sealed class DeviseIgnoreAttribute : Attribute
    {
        public DeviseIgnoreAttribute()
        {
            DTO = true;
            Api = true;
            Business = true;
        }
        public bool DTO { get; set; }
        public bool Business { get; set; }
        public bool Api { get; set; }
    }
}
";

        public static void Generate(GeneratorExecutionContext context)
        {
            context.AddSource("DeviseIgnoreAttribute.g.cs", deviseIgnoreAttributeText);
        }
    }

}
