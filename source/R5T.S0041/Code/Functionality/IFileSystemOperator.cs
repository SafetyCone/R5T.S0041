using System;

using R5T.T0132;
using R5T.L0089.T000;


namespace R5T.S0041
{
    [FunctionalityMarker]
    public partial interface IFileSystemOperator : IFunctionalityMarker,
        F0000.IFileSystemOperator,
        F0002.IFileSystemOperator,
        F0082.IFileSystemOperator
    {
        public bool Has_OutputAssembly(
            string projectFilePath)
        {
            var assemblyFilePath = Instances.FilePathProvider.Get_PublishDirectoryOutputAssemblyFilePath(projectFilePath);

            var outputAssemblyExists = Instances.FileSystemOperator.Exists_File(assemblyFilePath);
            return outputAssemblyExists;
        }

        public bool Has_BuildResultFile(
            string projectFilePath)
        {
            var buildJsonFilePath = Instances.FilePathProvider.Get_BuildJsonFilePath(projectFilePath);

            var buildJsonFileExists = Instances.FileSystemOperator.Exists_File(buildJsonFilePath);
            return buildJsonFileExists;
        }

        public WasFound<BuildResult> Has_BuildResult(
            string projectFilePath)
        {
            var buildJsonFilePath = Instances.FilePathProvider.Get_BuildJsonFilePath(projectFilePath);

            var buildJsonFileExists = Instances.FileSystemOperator.Exists_File(buildJsonFilePath);

            var buildResultOrDefault = buildJsonFileExists
                ? Instances.JsonOperator.Deserialize_Synchronous<BuildResult>(buildJsonFilePath)
                : default
                ;

            var hasBuildResult = WasFound.From(buildResultOrDefault);
            return hasBuildResult;
        }
    }
}
