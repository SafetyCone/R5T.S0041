using System;
using System.Text.RegularExpressions;

using R5T.T0132;


namespace R5T.S0041
{
    [DraftFunctionalityMarker]
    public partial interface IDocumentationOperator : IDraftFunctionalityMarker
    {
        public string PrettyPrint(string documentationXml)
        {
            var regexPattern = "\n +";

            var output = Regex.Replace(
                documentationXml,
                regexPattern,
                "\n");

            return output;
        }
    }
}
