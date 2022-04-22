using Devise.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devise.Generators.Business
{
  public static class RepositoryInterefaceGenerator
  {
    //Devise Attribute Definition

    public static void GenerateCottle(GeneratorExecutionContext context, DeviseConfig config, IEnumerable<ClassDeclarationSyntax> devisableEntities)
    {
      if (devisableEntities is null)
        throw new ArgumentNullException(nameof(devisableEntities));
      string renderedCode = CottleRenderer.Render(
          TemplateResourceReader.ReadTemplate("irepository"),
          SyntaxParser.GetMappingCottleContext(config, devisableEntities)
          );

      context.AddSource($"IRepository.g.cs", SourceText.From(renderedCode, Encoding.UTF8));

    }
  }
}

