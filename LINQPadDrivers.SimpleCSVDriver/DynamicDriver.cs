using CsvHelper;
using Davidlep.LINQPadDrivers.Common;
using Davidlep.LINQPadDrivers.Common.SourceGeneration;
using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    public class DynamicDriver : DynamicDataContextDriver
	{
        public override string Name => "Simple CSV Driver";
        public override string Author => "David Lépine";

        public override string GetConnectionDescription(IConnectionInfo connectionInfo)
        {
            var filepath = new ConnectionProperties(connectionInfo).FilePath;
            return new FileInfo(filepath).Name;
        }

        public override bool ShowConnectionDialog(IConnectionInfo connectionInfo, ConnectionDialogOptions dialogOptions) => new DataSourcesConnectionDialog(connectionInfo).ShowDialog() == true;

        public override List<ExplorerItem> GetSchemaAndBuildAssembly(
            IConnectionInfo connectionInfo,
            AssemblyName assemblyToBuild,
            ref string nameSpace,
            ref string typeName)
        {
            //Debugger.Launch();

            var connectionProperties = new ConnectionProperties(connectionInfo);
            var filePath = connectionProperties.FilePath;

            var dataSourceProperties = GetPropertyModels(filePath, connectionProperties.UseTypeInference);
            var dataSourceName = "Records";
           
            var generatorInput = SourceGeneratorInputFactory.CreateInput(filePath, dataSourceName);
            var generator = new CSharpSourceGenerator(generatorInput);

            string source = generator.GenerateSource(nameSpace, typeName, dataSourceProperties);

            var csvHelperAssembly               = Assembly.GetAssembly(typeof(CsvReader)).Location;
            var microsoftCodeAnalysisAssembly   = Assembly.GetAssembly(typeof(SyntaxFacts)).Location;
            var dataProviderAssembly            = Assembly.GetAssembly(typeof(CsvDataProvider<ExpandoObject>)).Location;

            var referencedAssemblies = new[] { csvHelperAssembly, microsoftCodeAnalysisAssembly, dataProviderAssembly };

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

        private PropertyModel[] GetPropertyModels(string filePath, bool useTypeInference)
        {
            ExpandoObject headerNames;

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            csv.Configuration.BadDataFound = context => { };
            csv.Configuration.HasHeaderRecord = false;
            headerNames = csv.GetRecords<object>().First() as ExpandoObject;

            var propertiesModels = headerNames.Select(x => new PropertyModel { RecordHeaderName = x.Value.ToString() }).ToArray();

            if(!useTypeInference)
                return propertiesModels;

            var records = csv.GetRecords<object>().Cast<IDictionary<string, object>>().ToArray();

            foreach (var record in records)
            {
                for (int i = 0; i < record.Count; i++)
                {
                    if (propertiesModels[i].TypeInferenceState == TypeInferenceStates.MultipleTypes)
                        continue;

                    var previousState = propertiesModels[i].TypeInferenceState;
                    var previousType = propertiesModels[i].CSharpType;

                    var inferredType = CSharpSourceHelper.TryInferredCSharpType(record.ElementAt(i).Value);

                    if (inferredType == null)
                        continue;

                    if (previousState == TypeInferenceStates.Inferred && previousType != inferredType)
                    {
                        propertiesModels[i].TypeInferenceState = TypeInferenceStates.MultipleTypes;
                        propertiesModels[i].CSharpType = null;
                        continue;
                    }

                    propertiesModels[i].CSharpType = inferredType;
                    propertiesModels[i].TypeInferenceState = TypeInferenceStates.Inferred;
                }
            }

            return propertiesModels;
        }
    }
}