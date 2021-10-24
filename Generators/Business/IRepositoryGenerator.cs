using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Devise.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Devise.Generators.Business
{
    public static class IRepositoryGenerator
    {

        private static string BusinessNamespace;
        private static StringBuilder GetIRepositoryBase()
        {

            return new StringBuilder($@"
using System.Collections.Generic;

namespace {BusinessNamespace}
{{
    public interface IRepository<T>
    {{
        ICollection<T> List();
        T? GetItem(int id);
        bool Remove(int id);
        T Create(T item);
        void Save(T item);
    }}
}}");
        }
        public static void Generate(GeneratorExecutionContext context)
        {
            StringBuilder sourceBuilder = GetIRepositoryBase();
            context.AddSource($"IRepository.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
