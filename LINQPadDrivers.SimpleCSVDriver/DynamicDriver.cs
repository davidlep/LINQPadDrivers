using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Davidlep.LINQPadDrivers.Common;
using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    public class DynamicDriver : DynamicDataContextDriver
	{
        #region Debug
        static DynamicDriver()
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
        public override string Author => "David L�pine";

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

            var dataSourceHeaders = GetCSVHeaders(filePath);
            var dataSourceName = "Records";
           
            var generatorInput = SourceGeneratorInputFactory.CreateInput(filePath, dataSourceName);
            var generator = new CSharpSourceGenerator(generatorInput);

            string source = generator.GenerateSource(nameSpace, typeName, dataSourceHeaders);

            var csvHelperAssembly               = Assembly.GetAssembly(typeof(CsvReader)).Location;
            var microsoftCodeAnalysisAssembly   = Assembly.GetAssembly(typeof(SyntaxFacts)).Location;
            var dataProviderAssembly            = Assembly.GetAssembly(typeof(DataProvider)).Location;

            var referencedAssemblies = new[] { csvHelperAssembly, microsoftCodeAnalysisAssembly, dataProviderAssembly };

            CSharpSourceCompiler.Compile(source, assemblyToBuild.CodeBase, referencedAssemblies);

            // Tell LINQPad what to display in the TreeView on the left (Schema Explorer):
            var schema = new ExplorerItem(dataSourceName, ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
            {
                IsEnumerable = true,
                DragText = dataSourceName,
                Children = dataSourceHeaders.Select(x => new ExplorerItem(x, ExplorerItemKind.Property, ExplorerIcon.Column)).ToList()
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
    }
}