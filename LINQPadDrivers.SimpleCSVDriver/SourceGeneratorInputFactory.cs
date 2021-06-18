using Davidlep.LINQPadDrivers.Common.SourceGeneration;

namespace Davidlep.LINQPadDrivers.SimpleCsvDriver
{
    public static class SourceGeneratorInputFactory
    {
        public static CSharpSourceGeneratorInput CreateInput(string csvFilePath, string dataSourceName)
        {
            return new CSharpSourceGeneratorInput()
            {
                DataContextTypeName = "Record",
                DataContextTypeDefaultPropertiesType = "string",
                DataProviderFullIdentifier = "Davidlep.LINQPadDrivers.SimpleCsvDriver.DataProvider",
                DataProviderMethod = "GetRecordsCsv",
                DataSourceFilePath = csvFilePath,//Genre pour parameter
                DataSourceMemberName = dataSourceName,
                PropertyAttributeGenerator = (prop) => $"   [Name(\"{CSharpSourceHelper.SanitizeStringForCSharpString(prop)}\")]",
                Imports = new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Globalization",
                    "System.IO",
                    "CsvHelper",
                    "CsvHelper.Configuration.Attributes",
                }
            };
        }
    }
}
