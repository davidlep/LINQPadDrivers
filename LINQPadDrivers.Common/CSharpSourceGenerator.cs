using Davidlep.LINQPadDrivers.Common;
using System.Collections.Generic;
using System.Linq;

namespace Davidlep.LINQPadDrivers.Common
{
    public class CSharpSourceGenerator
    {
        private CSharpSourceGeneratorInput input;

        public CSharpSourceGenerator(CSharpSourceGeneratorInput input)
        {
            this.input = input;
        }

        public string GenerateSource(string nameSpace, string typeName, string[] propertyNames)
        {
            return

            $"{GenerateImports(input.Imports)}" + @"

            namespace " + nameSpace + @"
            {
	            public class " + typeName + @"
	            {
		            " + "public IEnumerable<"+ input.DataContextTypeName + "> " + input.DataSourceMemberName + " => " + input.DataProviderFullIdentifier + "." + input.DataProviderMethod + "<"+ input.DataContextTypeName + ">(@\"" + input.DataSourceFilePath + "\"" + @");
	            }

	            public class "+ input.DataContextTypeName + @"    
	            {
                    " + GeneratePropertiesSource(propertyNames) + @"
	            }
            }";
        }

        private string GenerateImports(string[] imports)
        {
            return string.Join("\r\n", imports.Select(x => $"using {x};"));
        }

        private string GeneratePropertiesSource(string[] propertyNames)
        {
            var propertiesWithAttributeSources = new List<string>();

            for (int i = 0; i < propertyNames.Length; i++)
            {
                propertiesWithAttributeSources.Add(input.PropertyAttributeGenerator(propertyNames[i]));
                propertiesWithAttributeSources.Add("	public "+ input.DataContextTypePropertiesType + " " + CSharpSourceHelper.SanitizeStringForCSharpIdentifier(propertyNames[i], $"Property{i}") + " { get; set; }");
            }

            return string.Join("\r\n", propertiesWithAttributeSources);
        }
    }
}
