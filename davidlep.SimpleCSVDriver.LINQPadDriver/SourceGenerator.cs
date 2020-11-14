using Davidlep.LINQPadDriver.Common;
using System.Collections.Generic;
using System.Linq;

namespace Davidlep.LINQPadDrivers.SimpleCSVDriver
{
    public static class SourceGenerator
    {
        private static string DataProviderIdentifier = "Davidlep.LINQPadDrivers.SimpleCSVDriver.DataProvider";
        private static string[] Imports = new[]
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.Globalization",
            "System.IO",
            "CsvHelper",
            "CsvHelper.Configuration.Attributes",
        };

        public static string GenerateSource(string nameSpace, string typeName, string[] csvHeaderNames, string csvFilePath)
        {
            return

            $"{GenerateImports(Imports)}" + @"

            namespace " + nameSpace + @"
            {
	            public class " + typeName + @"
	            {
		            " + "public IEnumerable<Record> Records => " + DataProviderIdentifier + ".GetRecordsCSV<Record>(@\"" + csvFilePath + "\"" + @");
	            }

	            public class Record    
	            {
                    " + GeneratePropertiesSource(csvHeaderNames) + @"
	            }
            }";
        }

        private static string GenerateImports(string[] imports)
        {
            return string.Join("\r\n", imports.Select(x => $"using {x};"));
        }

        private static string GeneratePropertiesSource(string[] csvHeaderNames)
        {
            var propertiesWithAttributeSources = new List<string>();

            for (int i = 0; i < csvHeaderNames.Length; i++)
            {
                propertiesWithAttributeSources.Add($"   [Name(\"{CSharpSourceHelper.SanitizeStringForCsharpString(csvHeaderNames[i])}\")]");
                propertiesWithAttributeSources.Add("	public string " + CSharpSourceHelper.SanitizeStringForCSharpIdentifier(csvHeaderNames[i], $"Header{i}") + " { get; set; }");
            }

            return string.Join("\r\n", propertiesWithAttributeSources);
        }
    }
}
