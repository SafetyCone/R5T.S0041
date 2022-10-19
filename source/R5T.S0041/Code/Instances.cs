using System;

using R5T.F0000;
using R5T.F0002;
using R5T.F0017.F002;
using R5T.F0018;
using R5T.T0062;
using R5T.T0070;
using R5T.Z0000;
using R5T.Z0006;
using R5T.Z0010;


namespace R5T.S0041
{
    public static class Instances
    {
        public static IDateOperator DateOperator { get; } = F0000.DateOperator.Instance;
        public static IDirectoryNameOperator DirectoryNameOperator { get; } = S0041.DirectoryNameOperator.Instance;
        public static IDirectoryPathOperator DirectoryPathOperator { get; } = S0041.DirectoryPathOperator.Instance;
        public static IDirectoryPaths DirectoryPaths { get; } = S0041.DirectoryPaths.Instance;
        public static Z0010.IFileExtensions FileExtensions { get; } = Z0010.FileExtensions.Instance;
        public static IFileNameOperator FileNameOperator { get; } = S0041.FileNameOperator.Instance;
        public static IFilePathOperator FilePathOperator { get; } = S0041.FilePathOperator.Instance;
        public static IFilePaths FilePaths { get; } = S0041.FilePaths.Instance;
        public static F0000.IFileSystemOperator FileSystemOperator { get; } = F0000.FileSystemOperator.Instance;
        public static IHost Host { get; } = T0070.Host.Instance;
        public static IIdentityNameProvider IdentityNameProvider { get; } = F0017.F002.IdentityNameProvider.Instance;
        public static IInstanceVariety InstanceVariety { get; } = S0041.InstanceVariety.Instance;
        public static IInstanceVarietyOperator InstanceVarietyOperator { get; } = S0041.InstanceVarietyOperator.Instance;
        public static INamespacedTypeNames NamespacedTypeNames { get; } = Z0006.NamespacedTypeNames.Instance;
        public static IParameterNamedIdentityNameProvider ParameterNamedIdentityNameProvider { get; } = S0041.ParameterNamedIdentityNameProvider.Instance;
        public static F0002.IPathOperator PathOperator { get; } = F0002.PathOperator.Instance;
        public static Functionalities.IOperations Operations { get; } = Functionalities.Operations.Instance;
        public static IReflectionOperator ReflectionOperator { get; } = F0018.ReflectionOperator.Instance;
        public static IServiceAction ServiceAction { get; } = T0062.ServiceAction.Instance;
        public static IStrings Strings { get; } = Z0000.Strings.Instance;
        public static ITypeOperator TypeOperator { get; } = S0041.TypeOperator.Instance;
    }
}