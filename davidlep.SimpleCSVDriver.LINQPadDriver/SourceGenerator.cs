using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace davidlep.SimpleCSVDriver.LINQPadDriver
{
    public class SourceGenerator
    {
        public string GenerateSource(string nameSpace, string typeName, string[] csvHeaderNames, string csvFilePath)
        {
            return

          @"using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Globalization;
            using System.IO;
            using CsvHelper;
            using CsvHelper.Configuration.Attributes;

            namespace " + nameSpace + @"
            {
	            public class " + typeName + @"
	            {
		          " + "public IEnumerable<Record> Records => GetRecordsCSV<Record>(@\"" + csvFilePath + "\"" + @");

                    private IEnumerable<TRecord> GetRecordsCSV<TRecord>(string filepath)
                    {
	                    using (var sr = new StreamReader(filepath))
	                    using (var reader = new CsvReader(sr, CultureInfo.InvariantCulture))
	                    {
                            reader.Configuration.BadDataFound = context => { };

		                    foreach (var record in reader.GetRecords<TRecord>())
		                    {
			                    yield return record;
		                    }
	                    }
                    }
	            }

	            public class Record    
	            {
                " + GeneratePropertiesSource(csvHeaderNames) + @"
	            }
            }";
        }

        private string GeneratePropertiesSource(string[] csvHeaderNames)
        {
            List<string> propertiesWithAttributeSources = new List<string>();

            for (int i = 0; i < csvHeaderNames.Length; i++)
            {
                propertiesWithAttributeSources.Add($"   [Name(\"{SanitizeStringForCsharpString(csvHeaderNames[i])}\")]");
                propertiesWithAttributeSources.Add("	public string " + SanitizeStringForCSharpIdentifier(csvHeaderNames[i], $"Header{i}") + " { get; set; }");
            }

            return string.Join("\r\n", propertiesWithAttributeSources);
        }

        private string SanitizeStringForCsharpString(string str, string fallback = "")
        {
            return str.Replace("\"",@"\" + "\"");
        }

        private string SanitizeStringForCSharpIdentifier(string str, string fallback = "")
        {
            if (SyntaxFacts.IsReservedKeyword(SyntaxFacts.GetKeywordKind(str)))
                return $"@{str}";

            if (SyntaxFacts.IsValidIdentifier(str))
                return str;

            if (string.IsNullOrWhiteSpace(str))
                return fallback;

            if (!SyntaxFacts.IsIdentifierStartCharacter(str[0]))
                str = $"_{str}";

            var sanitizedString = new string(str
                .Where(c => SyntaxFacts.IsIdentifierPartCharacter(c))
                .ToArray());

            return SyntaxFacts.IsValidIdentifier(sanitizedString) ? sanitizedString : fallback;
        }
    }
}
