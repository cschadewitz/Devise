using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devise.Utilities.Diagnostics
{
  public class UserDiagnostics : DeviseDiagnostics
  {
    public UserDiagnostics(GeneratorExecutionContext context, DeviseConfig config) : base(context, config)
    {
    }
  }
}
