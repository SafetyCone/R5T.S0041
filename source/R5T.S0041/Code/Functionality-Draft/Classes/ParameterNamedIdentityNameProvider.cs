using System;


namespace R5T.S0041
{
    public class ParameterNamedIdentityNameProvider : IParameterNamedIdentityNameProvider
    {
        #region Infrastructure

        public static ParameterNamedIdentityNameProvider Instance { get; } = new();

        private ParameterNamedIdentityNameProvider()
        {
        }

        #endregion
    }
}
