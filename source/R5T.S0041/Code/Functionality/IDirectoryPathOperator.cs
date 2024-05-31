using System;

using R5T.T0132;


namespace R5T.S0041
{
	[FunctionalityMarker]
	public partial interface IDirectoryPathOperator : IFunctionalityMarker
	{
        public string GetDatedOutputDirectoryPath(
            DateTime date)
        {
            var outputDirectoryPath = Instances.DirectoryPaths.OutputDirectoryPath;

            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedChildDirectoryPath(
                outputDirectoryPath,
                date);

            return datedOutputDirectoryPath;
        }

        public string GetDatedChildDirectoryPath(
            string parentDirectoryPath,
            DateTime date)
        {
            var datedDirectoryName = Instances.DirectoryNameOperator.GetDatedDirectoryName(date);

            var datedOutputDirectoryPath = Instances.PathOperator.Get_DirectoryPath(
                parentDirectoryPath,
                datedDirectoryName);

            return datedOutputDirectoryPath;
        }

        public string GetPublishDirectoryPath_ForProjectFilePath(string projectFilePath)
        {
            var projectDirectoryPath = F0052.ProjectPathsOperator.Instance.GetProjectDirectoryPath(projectFilePath);

            var publishDirectoryPath = F0002.PathOperator.Instance.Get_DirectoryPath(
                projectDirectoryPath,
                Instances.DirectoryNames.bin,
                Instances.DirectoryNames.Publish);

            return publishDirectoryPath;
        }
    }
}