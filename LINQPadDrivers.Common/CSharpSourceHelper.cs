using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;

namespace Davidlep.LINQPadDrivers.Common
{
    public static class CSharpSourceHelper
    {
        public static string SanitizeStringForCSharpString(string str, string fallback = "")
        {
            return str.Replace("\"", @"\" + "\"");
        }

        public static string SanitizeStringForCSharpIdentifier(string str, string fallback = "")
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

        public static string TryInferredCSharpType(object value)
        {
            var str = value.ToString();

            if (double.TryParse(str, out _))
                return "double";

            if (Guid.TryParse(str, out _))
                return "Guid";

            if (DateTime.TryParse(str, out _))
                return "DateTime";

            return null;
        }
    }
}
