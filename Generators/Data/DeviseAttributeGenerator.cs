using Microsoft.CodeAnalysis;
using System;

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
        /// <summary>
        /// Marks a Entity as a Devise target
        /// </summary>
        public DeviseAttribute()
        {
            DTO = true;
            Api = true;
            Business = true;
            Mapping = true;
        }
        public bool DTO { get; }
        public bool Business { get; }
        public bool Api { get; set; }
        public bool Mapping {get; }
    }
}
";

        public static void Generate(GeneratorExecutionContext context)
        {
            context.AddSource("DeviseAttribute.g.cs", deviseAttributeText);
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class DeviseAttribute : Attribute
    {
        /// <summary>
        /// Marks a Entity as a Devise target
        /// </summary>
        public DeviseAttribute()
        {
            DTO = true;
            Api = true;
            Business = true;
            Mapping = true;
        }
        public bool DTO { get; }
        public bool Business { get; }
        public bool Api { get; set; }
        public bool Mapping { get; }
    }

}