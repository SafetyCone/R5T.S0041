using System;

using R5T.T0132;


namespace R5T.S0041
{
	[FunctionalityMarker]
	public partial interface IDirectoryPathOperator : IFunctionalityMarker
	{
        public string GetDatedChildDirectoryPath(
            string parentDirectoryPath,
            DateTime date)
        {
            var datedDirectoryName = Instances.DirectoryNameOperator.GetDatedDirectoryName(date);

            var datedOutputDirectoryPath = Instances.PathOperator.GetDirectoryPath(
                parentDirectoryPath,
                datedDirectoryName);

            return datedOutputDirectoryPath;
        }
    }
}