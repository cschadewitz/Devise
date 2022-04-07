using Microsoft.CodeAnalysis;
using System;
using System.IO;

namespace Devise.Utilities.Diagnostics
{
  public class DeviseDiagnostics : IDisposable
  {
    #region poor mans singleton, may replace with DI
    public static DeviseDiagnostics? Instance 
    { 
      get
      {
        if (Instance is null)
          throw new InvalidOperationException();
        else
          return Instance;
      }
      private set
      {
        Instance = value;
      }
    }
    public static DeviseDiagnostics CreateInstance(GeneratorExecutionContext context)
    {
      if (Instance is null)
        Instance = new DeviseDiagnostics(context);
      else
        throw new InvalidOperationException();
      return Instance;
    }
    #endregion

    protected bool Disposed { get; set; }

    protected GeneratorExecutionContext Context { get; }
    protected StreamWriter? LoggerFile { get; }

    protected delegate void ReportDelegate(DiagnosticDescriptor diagnostic, Location location, params object?[]? messageArgs);
    protected ReportDelegate ReportHandler { get; }

    #region constructors
    private DeviseDiagnostics(GeneratorExecutionContext context)
    {
      Context = context;
      if (DeviseConfig.TryLoadBuildProp(DeviseBuildProperties.LoggingFilePath, context, out string? filePath))
      {
        LoggerFile = File.AppendText(filePath);
        ReportHandler = ReportWithFile;
      }
      else
      {
        Report(context, DiagnosticDescriptors.FileLoggingDisabled);
        ReportHandler = ReportWithoutFile;
      }

    }
    #endregion


    private static void Report(GeneratorExecutionContext context, DiagnosticDescriptor diagnostic)
    {
      context.ReportDiagnostic(Diagnostic.Create(diagnostic, Location.None, (object[]?)null));
    }

    public void Report(DiagnosticDescriptor diagnostic, Location? location, params object?[]? messageArgs)
    {
      if (diagnostic is null)
        throw new ArgumentNullException(nameof(diagnostic));
      if(location is null)
        location = Location.None;
      ReportHandler(diagnostic, location, (object[]?)null);
    }

    private void ReportWithoutFile(DiagnosticDescriptor diagnostic, Location location, params object?[]? messageArgs)
    {
      Context.ReportDiagnostic(Diagnostic.Create(diagnostic, location, messageArgs));
    }

    private void ReportWithFile(DiagnosticDescriptor diagnostic, Location location, params object?[]? messageArgs)
    {
      Context.ReportDiagnostic(Diagnostic.Create(diagnostic, location, messageArgs));
      ReportToFile(diagnostic, location, messageArgs);
    }

    private void ReportToFile(DiagnosticDescriptor diagnostic, Location location, params object?[]? messageArgs)
    {
      LoggerFile?.WriteLine();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          LoggerFile?.Dispose();
        }
        Disposed=true;
      }
    }
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}
