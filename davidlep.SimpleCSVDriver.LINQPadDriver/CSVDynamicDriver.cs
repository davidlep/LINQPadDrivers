using CsvHelper;
using CsvHelper.Configuration.Attributes;
using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace davidlep.SimpleCSVDriver.LINQPadDriver
{
    public class CSVDynamicDriver : DynamicDataContextDriver
	{
        #region Debug
        static CSVDynamicDriver()
        {
            // Uncomment the following code to attach to Visual Studio's debugger when an exception is thrown.
            //AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
            //{
            //	if (args.Exception.StackTrace.Contains (typeof (DynamicDemoDriver).Namespace))
            //		Debugger.Launch ();
            //};
        }
        #endregion

        public override string Name => "Simple CSV Driver";
        [Name]
        public override string Author => "David Lépine";

        public override string GetConnectionDescription(IConnectionInfo connectionInfo)
        {
            var filepath = new ConnectionProperties(connectionInfo).FilePath;
            return new FileInfo(filepath).Name;
        }

        public override bool ShowConnectionDialog(IConnectionInfo connectionInfo, ConnectionDialogOptions dialogOptions) => new ConnectionDialog(connectionInfo).ShowDialog() == true;

        public override List<ExplorerItem> GetSchemaAndBuildAssembly(
            IConnectionInfo connectionInfo,
            AssemblyName assemblyToBuild,
            ref string nameSpace,
            ref string typeName)
        {
            //Debugger.Launch();

            var connectionProperties = new ConnectionProperties(connectionInfo);
            var filePath = connectionProperties.FilePath;

            var headers = GetCSVHeaders(filePath);
            string source = new SourceGenerator().GenerateSource(nameSpace, typeName, headers, filePath);
            Compile(source, assemblyToBuild.CodeBase);

            // Tell LINQPad what to display in the TreeView on the left (Schema Explorer):
            var schema = new ExplorerItem("Records", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
            {
                IsEnumerable = true,
                DragText = "Records",
                Children = headers.Select(x => new ExplorerItem(x, ExplorerItemKind.Property, ExplorerIcon.Column)).ToList()
            };

            return new[] { schema }.ToList();
        }

        private string[] GetCSVHeaders(string filePath)
        {
            ExpandoObject headerNames;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.BadDataFound = context => { };
                csv.Configuration.HasHeaderRecord = false;
                headerNames = csv.GetRecords<object>().First() as ExpandoObject;
            }

            return headerNames.Select(x => x.Value.ToString()).ToArray();
        }

        static void Compile(string cSharpSourceCode, string outputFile)
        {
            var csvHelperAssembly = Assembly.GetAssembly(typeof(CsvReader)).Location;
            var microsoftCodeAnalysisAssembly = Assembly.GetAssembly(typeof(SyntaxFacts)).Location;

            string[] assembliesToReference = GetCoreFxReferenceAssemblies()
                .Concat(new[] { csvHelperAssembly, microsoftCodeAnalysisAssembly })
                .ToArray();

            var compileResult = CompileSource(new CompilationInput
            {
                FilePathsToReference = assembliesToReference.Concat(new[] { csvHelperAssembly }).ToArray(),
                OutputPath = outputFile,
                SourceCode = new[] { cSharpSourceCode }
            });

            if (compileResult.Errors.Length > 0)
                throw new Exception("Cannot compile typed context: " + compileResult.Errors[0]);
        }
    }
}