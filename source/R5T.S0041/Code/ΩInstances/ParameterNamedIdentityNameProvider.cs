﻿using System;


namespace R5T.S0041
{
    public class ParameterNamedIdentityNameProvider : IParameterNamedIdentityNameProvider
    {
        #region Infrastructure

        public static IParameterNamedIdentityNameProvider Instance { get; } = new ParameterNamedIdentityNameProvider();

        private ParameterNamedIdentityNameProvider()
        {
        }

        #endregion
    }
}
