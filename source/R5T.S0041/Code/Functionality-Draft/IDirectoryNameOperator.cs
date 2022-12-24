using System;
using System.IO;

using R5T.T0132;


namespace R5T.S0041
{
    [DraftFunctionalityMarker]
    public partial interface IDirectoryNameOperator : IDraftFunctionalityMarker
    {
        public string GetDatedDirectoryName(DateTime dateTime)
        {
            var output = this.GetDirectoryName_YYYYMMDD(dateTime);
            return output;
        }

        public string GetDirectoryName_YYYYMMDD(DateTime dateTime)
        {
            var output = Instances.DateOperator.ToString_YYYYMMDD(dateTime);
            return output;
        }

        public bool IsNotBinariesOrObjectsDirectory(DirectoryInfo directoryInfo)
        {
            var output = true
                && directoryInfo.Name != Instances.DirectoryNames.bin
                && directoryInfo.Name != Instances.DirectoryNames.obj
                ;

            return output;
        }
    }
}
