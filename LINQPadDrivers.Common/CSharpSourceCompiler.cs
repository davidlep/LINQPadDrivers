using LINQPad.Extensibility.DataContext;
using System;
using System.Linq;

namespace Davidlep.LINQPadDrivers.Common
{
    public class CSharpSourceCompiler
    {
        public static void Compile(string cSharpSourceCode, string outputFile, string[] referencedAssemblies)
        {
            var compileResult = DataContextDriver.CompileSource(new CompilationInput
            {
                FilePathsToReference = DataContextDriver.GetCoreFxReferenceAssemblies().Concat(referencedAssemblies).ToArray(),
                OutputPath = outputFile,
                SourceCode = new[] { cSharpSourceCode }
            });

            if (compileResult.Errors.Length > 0)
                throw new Exception("Cannot compile typed context: " + compileResult.Errors[0]);
        }
    }
}
