using System;

using R5T.F0017.F002;
using R5T.F0018;
using R5T.T0041;
using R5T.T0044;
using R5T.T0062;
using R5T.T0070;
using R5T.Z0006;


namespace R5T.S0041
{
    public static class Instances
    {
        public static IDateOperator DateOperator { get; } = S0041.DateOperator.Instance;
        public static IDirectoryNameOperator DirectoryNameOperator { get; } = S0041.DirectoryNameOperator.Instance;
        public static IFileNameOperator FileNameOperator { get; } = T0041.FileNameOperator.Instance;
        public static IFilePaths FilePaths { get; } = S0041.FilePaths.Instance;
        public static IFileSystemOperator FileSystemOperator { get; } = T0044.FileSystemOperator.Instance;
        public static IHost Host { get; } = T0070.Host.Instance;
        public static IIdentityNameProvider IdentityNameProvider { get; } = F0017.F002.IdentityNameProvider.Instance;
        public static IInstanceVariety InstanceVariety { get; } = S0041.InstanceVariety.Instance;
        public static INamespacedTypeNames NamespacedTypeNames { get; } = Z0006.NamespacedTypeNames.Instance;
        public static IParameterNamedIdentityNameProvider ParameterNamedIdentityNameProvider { get; } = S0041.ParameterNamedIdentityNameProvider.Instance;
        public static IPathOperator PathOperator { get; } = T0041.PathOperator.Instance;
        public static Functionalities.IOperations Operations { get; } = Functionalities.Operations.Instance;
        public static IReflectionOperator ReflectionOperator { get; } = F0018.ReflectionOperator.Instance;
        public static IServiceAction ServiceAction { get; } = T0062.ServiceAction.Instance;
        public static ITypeOperator TypeOperator { get; } = S0041.TypeOperator.Instance;
    }
}