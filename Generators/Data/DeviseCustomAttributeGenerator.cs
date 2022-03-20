using Microsoft.CodeAnalysis;
using System;
using Devise.Utilities;

namespace Devise.Generators.Data
{
    public static class DeviseCustomAttributeGenerator
    {
        //Devise Attribute Definition
        private const string deviseCustomAttributeText = @"
using System;
namespace Devise
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class DeviseCustomAttribute : Attribute
    {
        /// <summary>
        /// Marks a Devise target for custom implementation
        /// </summary>
        /// <param name=" +"\"Target\"" + @">String name of the target to mark for custom implementation. Valid targets - Api, Business, Dto, Mapping.</param>
        /// <param name=" + "\"Create\"" + @">Marks Create (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name=" + "\"Read\"" + @">Marks Read or Get (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name=" + "\"Update\"" + @">Marks Update or Post (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name=" + "\"Delete\"" + @">Marks Delete or Remove (delete) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name=" + "\"List\"" + @">Marks List or Get (get) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        public DeviseCustomAttribute(string Target, bool Create = false, bool Read = false, bool Update = false, bool Delete = false, bool List = false)
        {
            this.Target = Target;
            this.Create = Create;
            this.Read = Read;
            this.Update = Update;
            this.Delete = Delete;
            this.List = List;
        }
        /// <summary>
        /// DeviseTarget to mark for custom implementation
        /// Valid targets include Api, Business, DTO, Mapping
        /// </summary>
        public string Target { get; }
        /// <summary>
        /// Marks Create (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Create { get; }
        /// <summary>
        /// Marks Read or Get (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Read { get; }
        /// <summary>
        /// Marks Update or Post (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Update { get; }
        /// <summary>
        /// Marks Delete or Remove (delete) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Delete { get; }
        /// <summary>
        /// Marks List or Get (get) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool List { get; }
    }
}
";

        public static void Generate(GeneratorExecutionContext context)
        {
            context.AddSource("DeviseCustomAttribute.g.cs", deviseCustomAttributeText);
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class DeviseCustomAttribute : Attribute
    {
        /// <summary>
        /// Marks a Devise target for custom implementation
        /// </summary>
        /// <param name="Target">String name of the target to mark for custom implementation. Valid targets - Api, Business, Dto, Mapping.</param>
        /// <param name="Create">Marks Create (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name="Read">Marks Read or Get (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name="Update">Marks Update or Post (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name="Delete">Marks Delete or Remove (delete) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name="List">Marks List or Get (get) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        public DeviseCustomAttribute(string Target, bool Create = false, bool Read = false, bool Update = false, bool Delete = false, bool List = false)
        {
            this.Target = Target;
            this.Create = Create;
            this.Read = Read;
            this.Update = Update;
            this.Delete = Delete;
            this.List = List;
        }
        /// <summary>
        /// DeviseTarget to mark for custom implementation
        /// Valid targets include Api, Business, DTO, Mapping
        /// </summary>
        public string Target { get; }
        /// <summary>
        /// Marks Create (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Create { get; }
        /// <summary>
        /// Marks Read or Get (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Read { get; }
        /// <summary>
        /// Marks Update or Post (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Update { get; }
        /// <summary>
        /// Marks Delete or Remove (delete) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Delete { get; }
        /// <summary>
        /// Marks List or Get (get) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool List { get; }
    }
}
