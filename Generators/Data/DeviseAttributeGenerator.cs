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

    public static class DeviseAttributeGenerator
    {
        //Devise Attribute Definition
        private const string deviseAttributeText = @"
using System;
namespace Devise
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class DeviseAttribute : Attribute
    {
        public DeviseAttribute()
        {
            DTO = true;
            Api = true;
            Business = true;
            Mapping = true;
        }
        public bool DTO { get; set; }
        public bool Business { get; set; }
        public bool Api { get; set; }
        public bool Mapping {get; set; }
    }
}
";

        public static void Generate(GeneratorExecutionContext context)
        {
            context.AddSource("DeviseAttribute.g.cs", deviseAttributeText);
        }
    }
    
}