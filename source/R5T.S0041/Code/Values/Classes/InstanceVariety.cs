using System;


namespace R5T.S0041
{
	public class InstanceVariety : IInstanceVariety
	{
		#region Infrastructure

	    public static InstanceVariety Instance { get; } = new();

	    private InstanceVariety()
	    {
	    }

	    #endregion
	}
}