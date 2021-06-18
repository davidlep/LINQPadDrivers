using Davidlep.LINQPadDrivers.Common;
using Davidlep.LINQPadDrivers.Common.SourceGeneration;
using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Davidlep.LINQPadDrivers.SimpleJsonDriver
{
    public class DynamicDriver : DynamicDataContextDriver
    {
        public override string Name => "Simple JSON Driver";
        public override string Author => "David Lépine";
        public JsonDataProvider<ExpandoObject> DataProvider = new JsonDataProvider<ExpandoObject>();

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

            var dataSourceProperties = GetJsonProperties(filePath).Select(x => new PropertyModel { RecordHeaderName = x }).ToArray();
            var dataSourceName = "Records";

            var generatorInput = SourceGeneratorInputFactory.CreateInput(filePath, dataSourceName);
            var generator = new CSharpSourceGenerator(generatorInput);

            string source = generator.GenerateSource(nameSpace, typeName, dataSourceProperties);

            var newtonsoftAssembly = Assembly.GetAssembly(typeof(JsonSerializer)).Location;
            var microsoftCodeAnalysisAssembly = Assembly.GetAssembly(typeof(SyntaxFacts)).Location;
            var dataProviderAssembly = Assembly.GetAssembly(typeof(JsonDataProvider<ExpandoObject>)).Location;

            var referencedAssemblies = new[] { newtonsoftAssembly, microsoftCodeAnalysisAssembly, dataProviderAssembly };

            CSharpSourceCompiler.Compile(source, assemblyToBuild.CodeBase, referencedAssemblies);

            // Tell LINQPad what to display in the TreeView on the left (Schema Explorer):
            var schema = new ExplorerItem(dataSourceName, ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
            {
                IsEnumerable = true,
                DragText = dataSourceName,
                Children = dataSourceProperties.Select(x => new ExplorerItem(x.RecordHeaderName, ExplorerItemKind.Property, ExplorerIcon.Column)).ToList()
            };

            return new[] { schema }.ToList();
        }

        private string[] GetJsonProperties(string filePath)
        {
            return DataProvider.GetRecordsFromFile(filePath)
                .SelectMany(x => (x as IDictionary<string, object>).Keys)
                .Distinct()
                .ToArray();
        }
    }
}