using System;


namespace R5T.S0041
{
    public class TypeOperator : ITypeOperator
    {
        #region Infrastructure

        public static TypeOperator Instance { get; } = new();

        private TypeOperator()
        {
        }

        #endregion
    }
}
