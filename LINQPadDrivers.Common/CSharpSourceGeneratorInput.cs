using System;

namespace Davidlep.LINQPadDrivers.Common
{
    public class CSharpSourceGeneratorInput
    {
        public string[] Imports { get; set; }
        public string DataProviderFullIdentifier { get; set; }
        public string DataProviderMethod { get; set; }
        public string DataContextTypeName { get; set; }
        public string DataContextTypePropertiesType { get; set; }
        public string DataSourceMemberName { get; set; }
        public string DataSourceFilePath { get; set; }

        /// <summary>
        /// Takes a property name and generate a corresponding CSharp attribute for that property
        /// </summary>
        public Func<string, string> PropertyAttributeGenerator { get; set; }
    }
}
