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

        public string GenerateSource(string nameSpace, string typeName, PropertyModel[] propertyModels)
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
                    " + GeneratePropertiesSource(propertyModels) + @"
	            }
            }";
        }

        private string GenerateImports(string[] imports)
        {
            return string.Join("\r\n", imports.Select(x => $"using {x};"));
        }

        private string GeneratePropertiesSource(PropertyModel[] propertyModels)
        {
            var propertiesWithAttributeSources = new List<string>();

            for (int i = 0; i < propertyModels.Length; i++)
            {
                propertiesWithAttributeSources.Add(input.PropertyAttributeGenerator(propertyModels[i].RecordHeaderName));
                propertiesWithAttributeSources.Add("	public "+ (propertyModels[i].CSharpType ?? input.DataContextTypeDefaultPropertiesType) + " " + CSharpSourceHelper.SanitizeStringForCSharpIdentifier(propertyModels[i].RecordHeaderName, $"Property{i}") + " { get; set; }");
            }

            return string.Join("\r\n", propertiesWithAttributeSources);
        }
    }
}
