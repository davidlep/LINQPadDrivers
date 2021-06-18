using Davidlep.LINQPadDrivers.Common.SourceGeneration;

namespace Davidlep.LINQPadDrivers.SimpleJsonDriver
{
    public static class SourceGeneratorInputFactory
    {
        public static CSharpSourceGeneratorInput CreateInput(string jsonFilePath, string dataSourceName)
        {
            return new CSharpSourceGeneratorInput()
            {
                DataContextTypeName = "Record",
                DataContextTypeDefaultPropertiesType = "dynamic",
                DataProviderFullIdentifier = "Davidlep.LINQPadDrivers.SimpleJsonDriver.DataProvider",
                DataProviderMethod = "GetRecordsJson",
                DataSourceFilePath = jsonFilePath,
                DataSourceMemberName = dataSourceName,
                PropertyAttributeGenerator = (prop) => $"   [JsonProperty(\"{CSharpSourceHelper.SanitizeStringForCSharpString(prop)}\")]",
                Imports = new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.IO",
                    "System.Dynamic",
                    "Newtonsoft.Json",
                }
            };
        }
    }
}
