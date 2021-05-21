using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Devise.Generators
{
    public static class DeviseAttributeGenerator
    {
        //Devise Attribute Definition
        private const string deviseAttributeText = @"
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

        public static void Generate(GeneratorExecutionContext context)
        {
            context.AddSource("DeviseAttribute.g.cs", deviseAttributeText);
        }
    }
    
}