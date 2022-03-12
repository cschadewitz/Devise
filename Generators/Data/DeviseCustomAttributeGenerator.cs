using Microsoft.CodeAnalysis;
using System;

namespace Devise.Generators.Data
{
    [DeviseCustom("Api", read: true)]
    [DeviseCustom("Business", read: true, create: true)]
    [DeviseCustom("Mapping")]
    [DeviseCustom("DTO")]
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
        /// Marks a Devise generated component for custom implementation
        /// </summary>
        /// <param name=" + "\"target\"" + @">String name of the target to mark for custom implementation. Valid targets - Api, Business, DTO, Mapping.</param>
        /// <param name=" + "\"create\"" + @">Marks Create (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name=" + "\"read\"" + @">Marks Read or Get (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name=" + "\"update\"" + @">Marks Update or Post (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name=" + "\"delete\"" + @">Marks Delete or Remove (delete) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name=" + "\"list\"" + @">Marks List or Get (get) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        public DeviseCustomAttribute(string target, bool create = false, bool read = false, bool update = false, bool delete = false, bool list = false )
        {
            Target = target;
            Create = create;
            Read = read;
            Update = update;
            Delete = delete;
            List = list;
        }
        /// <summary>
        /// Name of the generated target to mark for custom implementation
        /// Valid targets include Api, Business, DTO, Mapping
        /// </summary>
        public string Target { get; }
        /// <summary>
        /// Marks Create (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Create { get;}
        /// <summary>
        /// Marks Read or Get (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Read { get;}
        /// <summary>
        /// Marks Update or Post (post) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Update { get;}
        /// <summary>
        /// Marks Delete or Remove (delete) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool Delete { get;}
        /// <summary>
        /// Marks List or Get (get) for custom implementation
        /// Only valid if the attribute target is Api or Business
        /// </summary>
        public bool List { get;}
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
        /// <param name="target">String name of the target to mark for custom implementation. Valid targets - Api, Business, DTO, Mapping.</param>
        /// <param name="create">Marks Create (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name="read">Marks Read or Get (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name="update">Marks Update or Post (post) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name="delete">Marks Delete or Remove (delete) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        /// <param name="list">Marks List or Get (get) for custom implementation. Only valid if the attribute target is Api or Business.</param>
        public DeviseCustomAttribute(string target, bool create = false, bool read = false, bool update = false, bool delete = false, bool list = false)
        {
            Target = target;
            Create = create;
            Read = read;
            Update = update;
            Delete = delete;
            List = list;
        }
        /// <summary>
        /// String name of the target to mark for custom implementation
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
