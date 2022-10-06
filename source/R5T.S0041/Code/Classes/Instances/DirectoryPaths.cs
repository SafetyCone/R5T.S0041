using System;


namespace R5T.S0041
{
	public class DirectoryPaths : IDirectoryPaths
	{
		#region Infrastructure

	    public static IDirectoryPaths Instance { get; } = new DirectoryPaths();

	    private DirectoryPaths()
	    {
        }

	    #endregion
	}
}