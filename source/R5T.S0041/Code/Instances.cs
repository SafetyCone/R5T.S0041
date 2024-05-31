using System;

using R5T.F0017.F002;
using R5T.F0018;
using R5T.T0062;
using R5T.T0070;
using R5T.Z0006;


namespace R5T.S0041
{
    public static class Instances
    {
        public static Z0025.IAssemblyNames AssemblyNames => Z0025.AssemblyNames.Instance;
        public static Z0000.ICharacters Characters => Z0000.Characters.Instance;
        public static F0000.IDateOperator DateOperator => F0000.DateOperator.Instance;
        public static IDirectoryNameOperator DirectoryNameOperator => S0041.DirectoryNameOperator.Instance;
        public static Z0012.IDirectoryNames DirectoryNames => Z0012.DirectoryNames.Instance;
        public static IDirectoryPathOperator DirectoryPathOperator => S0041.DirectoryPathOperator.Instance;
        public static IDirectoryPaths DirectoryPaths => S0041.DirectoryPaths.Instance;
        public static F0027.IDotnetPublishOperator DotnetPublishOperator => F0027.DotnetPublishOperator.Instance;
        public static D8S.Z0003.IEmailAddresses EmailAddresses => D8S.Z0003.EmailAddresses.Instance;
        public static F0097.IEmailSender EmailSender => F0097.EmailSender.Instance;
        public static F0000.IEnumerableOperator EnumerableOperator => F0000.EnumerableOperator.Instance;
        public static F0002.IExecutablePathOperator ExecutablePathOperator => F0002.ExecutablePathOperator.Instance;
        public static Z0072.Z002.IFileExtensions FileExtensions => Z0072.Z002.FileExtensions.Instance;
        public static IFileNameOperator FileNameOperator => S0041.FileNameOperator.Instance;
        public static IFileNames FileNames => S0041.FileNames.Instance;
        public static F0000.IFileOperator FileOperator => F0000.FileOperator.Instance;
        public static IFilePathOperator FilePathOperator => S0041.FilePathOperator.Instance;
        public static IFilePathProvider FilePathProvider => S0041.FilePathProvider.Instance;
        public static IFilePaths FilePaths => S0041.FilePaths.Instance;
        public static IFileSystemOperator FileSystemOperator => S0041.FileSystemOperator.Instance;
        public static IHost Host => T0070.Host.Instance;
        public static IIdentityNameProvider IdentityNameProvider => F0017.F002.IdentityNameProvider.Instance;
        public static IInstanceVariety InstanceVariety => S0041.InstanceVariety.Instance;
        public static IInstanceVarietyOperator InstanceVarietyOperator => S0041.InstanceVarietyOperator.Instance;
        public static F0032.IJsonOperator JsonOperator => F0032.JsonOperator.Instance;
        public static F0035.ILoggingOperator LoggingOperator => F0035.LoggingOperator.Instance;
        public static F0000.INamespacedTypeNameOperator NamespacedTypeNameOperator => F0000.NamespacedTypeNameOperator.Instance;
        public static INamespacedTypeNames NamespacedTypeNames => Z0006.NamespacedTypeNames.Instance;
        public static F0033.INotepadPlusPlusOperator NotepadPlusPlusOperator => F0033.NotepadPlusPlusOperator.Instance;
        public static F0000.INowOperator NowOperator => F0000.NowOperator.Instance;
        public static IParameterNamedIdentityNameProvider ParameterNamedIdentityNameProvider => S0041.ParameterNamedIdentityNameProvider.Instance;
        public static F0002.IPathOperator PathOperator => F0002.PathOperator.Instance;
        public static F0052.IProjectPathsOperator ProjectPathsOperator => F0052.ProjectPathsOperator.Instance;
        public static Functionalities.IOperations Operations => Functionalities.Operations.Instance;
        public static IReflectionOperator ReflectionOperator => F0018.ReflectionOperator.Instance;
        public static Z0022.IRepositoriesDirectoryPathsSets RepositoriesDirectoryPathsSets => Z0022.RepositoriesDirectoryPathsSets.Instance;
        public static IServiceAction ServiceAction => T0062.ServiceAction.Instance;
        public static F0000.IStringOperator StringOperator => F0000.StringOperator.Instance;
        public static Z0000.IStrings Strings => Z0000.Strings.Instance;
        public static ITypeOperator TypeOperator => S0041.TypeOperator.Instance;
        public static L0089.F000.IWasFoundOperator WasFoundOperator => L0089.F000.WasFoundOperator.Instance;
    }
}