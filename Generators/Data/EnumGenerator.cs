using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devise.Generators.Data
{
    internal class EnumGenerator
    {
        private const string deviseTargetText = @"
namespace Devise
{
    enum DeviseTarget
    {
        Api,
        Business,
        DTO,
        Mapping
    }
}
";
        private const string deviseOperationText = @"
namespace Devise
{
    enum Operation
    {
        Create,
        Read,
        Update,
        Delete,
        List
    }
}
";

        public static void Generate(GeneratorExecutionContext context)
        {
            context.AddSource("DeviseTarget.g.cs", deviseTargetText);
            context.AddSource("DeviseOperation.g.cs", deviseOperationText);
        }
    }
}
