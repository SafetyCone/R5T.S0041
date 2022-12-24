using System;


namespace R5T.S0041
{
    public class FilePathProvider : IFilePathProvider
    {
        #region Infrastructure

        public static IFilePathProvider Instance { get; } = new FilePathProvider();


        private FilePathProvider()
        {
        }

        #endregion
    }
}
