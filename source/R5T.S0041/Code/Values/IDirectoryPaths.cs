using System;

using R5T.T0131;


namespace R5T.S0041
{
	[ValuesMarker]
	public partial interface IDirectoryPaths : IValuesMarker
	{
		public string InitialOutputDirectorPath => @"C:\Temp";
	}
}