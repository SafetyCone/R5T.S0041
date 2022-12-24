using System;


namespace R5T.S0041
{
    public class DocumentationOperator : IDocumentationOperator
    {
        #region Infrastructure

        public static IDocumentationOperator Instance { get; } = new DocumentationOperator();


        private DocumentationOperator()
        {
        }

        #endregion
    }
}
