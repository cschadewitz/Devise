using Devise.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devise.Generators.Api
{
  public static class ControllerGenerator
  {
    public static void GenerateCottle(GeneratorExecutionContext context, DeviseConfig config, IEnumerable<ClassDeclarationSyntax> devisableEntities)
    {
      if (devisableEntities is null)
        throw new ArgumentNullException(nameof(devisableEntities));
      foreach (ClassDeclarationSyntax entity in devisableEntities)
      {
        if (!entity.Parent.IsKind(SyntaxKind.NamespaceDeclaration))
        {
          //Throw Error to user that subclasses are not supported
        }
        string renderedCode = CottleRenderer.Render(
            TemplateResourceReader.ReadTemplate("controller"),
            SyntaxParser.GetEntityCottleContext(config, entity)
            );

        context.AddSource($"{entity.Identifier.Text}Controller.g.cs", SourceText.From(renderedCode, Encoding.UTF8));
      }

    }
  }
}
