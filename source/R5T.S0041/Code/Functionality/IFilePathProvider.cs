using System;

using R5T.T0132;


namespace R5T.S0041
{
    [FunctionalityMarker]
    public partial interface IFilePathProvider : IFunctionalityMarker
    {
        public string Get_NewAndOldSummaryTextFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.NewAndOldSummaryTextFilePath);

            return instancesJsonFilePath;
        }

        public string Get_DateComparisonSummaryTextFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.DateComparisonSummaryTextFilePath);

            return instancesJsonFilePath;
        }

        public string Get_NewInstancesJsonFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.NewInstancesJsonFileName);

            return instancesJsonFilePath;
        }

        public string Get_OldInstancesJsonFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.OldInstancesJsonFileName);

            return instancesJsonFilePath;
        }

        public string Get_ProcessingSummaryTextFilePath(
            string datedOutputDirectoryPath)
        {
            var summaryFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.ProcessingSummaryTextFileName);

            return summaryFilePath;
        }

        public string Get_InstancesJsonFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.InstancesJsonFileName);

            return instancesJsonFilePath;
        }

        public string Get_ProcessingProblemProjectsTextFilePath(
            string datedOutputDirectoryPath)
        {
            var buildProblemProjectsFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.ProcessingProblemProjectsTextFileName);

            return buildProblemProjectsFilePath;
        }

        public string Get_ProcessingProblemsTextFilePath(
            string datedOutputDirectoryPath)
        {
            var buildProblemsFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.ProcessingProblemsTextFileName);

            return buildProblemsFilePath;
        }

        public string Get_ProjectFileTuplesJsonFilePath(
            string datedOutputDirectoryPath)
        {
            var projectFileTuplesJsonFilePath = Instances.PathOperator.Get_FilePath(
                datedOutputDirectoryPath,
                Instances.FileNames.ProjectFileTuplesJsonFileName);

            return projectFileTuplesJsonFilePath;
        }

        public string Get_BuildProblemProjectsTextFilePath(
            string datedOutputDirectoryPath)
        {
            var buildProblemProjectsFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.BuildProblemProjectsTextFileName);

            return buildProblemProjectsFilePath;
        }

        public string Get_BuildProblemsTextFilePath(
            string datedOutputDirectoryPath)
        {
            var buildProblemsFilePath = F0002.PathOperator.Instance.Get_FilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.BuildProblemsTextFileName);

            return buildProblemsFilePath;
        }

        public string Get_BuildJsonFilePath(string projectFilePath)
        {
            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            var buildJsonFilePath = Instances.FilePathProvider.Get_BuildJsonFilePath_FromPublishDirectory(publishDirectoryPath);
            return buildJsonFilePath;
        }

        public string Get_BuildJsonFilePath_FromPublishDirectory(string publishDirectoryPath)
        {
            var buildJsonFilePath = F0002.PathOperator.Instance.Get_FilePath(
                publishDirectoryPath,
                FileNames.Instance.BuildJsonFileName);

            return buildJsonFilePath;
        }

        public string Get_ExampleAssemblyFilePath()
        {
            var exampleAssemblyFileNameStem = Instances.AssemblyNames.R5T_Z0025;

            var exampleAssemblyFileName = Instances.FileNameOperator.Get_FileName(
                exampleAssemblyFileNameStem,
                Instances.FileExtensions.Dll);

            var exampleAssemblyFilePath = Instances.ExecutablePathOperator.Get_Path_ExecutableDirectoryRelative(
                exampleAssemblyFileName);

            return exampleAssemblyFilePath;
        }

        public string Get_PublishDirectoryOutputAssemblyFilePath(
            string projectFilePath)
        {
            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            var projectName = Instances.ProjectPathsOperator.GetProjectName(projectFilePath);

            var outputAssemblyFileName = F0000.FileNameOperator.Instance.Get_FileName(
                projectName,
                Instances.FileExtensions.Dll);

            var assemblyFilePath = Instances.PathOperator.Get_FilePath(
                publishDirectoryPath,
                outputAssemblyFileName);

            return assemblyFilePath;
        }

        public string Get_ProjectsListTextFilePath(
            string datedOutputDirectoryPath)
        {
            var projectsListTextFilePath = Instances.PathOperator.Get_FilePath(
                datedOutputDirectoryPath,
                Instances.FileNames.ProjectsListTextFileName);

            return projectsListTextFilePath;
        }
    }
}
