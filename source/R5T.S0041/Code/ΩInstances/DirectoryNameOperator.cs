using System;


namespace R5T.S0041
{
    public class DirectoryNameOperator : IDirectoryNameOperator
    {
        #region Infrastructure

        public static IDirectoryNameOperator Instance { get; } = new DirectoryNameOperator();


        private DirectoryNameOperator()
        {
        }

        #endregion
    }
}
