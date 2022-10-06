using System;

using R5T.T0131;


namespace R5T.S0041
{
	[ValuesMarker]
	public partial interface IDirectoryPaths : IValuesMarker
	{
		public string CloudOutputDirectoryPath => @"C:\Users\David\Dropbox\Organizations\Rivet\Shared\Data\Instances";
		public string InitialOutputDirectorPath => @"C:\Temp";
		/// Also see: R5T.S0046.IDirectoryPaths.NuGetAssemblies.
		public string NuGetAssemblies => @"C:\Users\David\Dropbox\Organizations\Rivet\Shared\Binaries\Nuget Assemblies\";
		public string OutputDirectoryPath => @"C:\Temp\Output\S0041\";
	}
}