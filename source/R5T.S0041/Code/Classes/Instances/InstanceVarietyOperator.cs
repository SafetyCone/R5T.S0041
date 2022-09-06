using System;


namespace R5T.S0041
{
	public class InstanceVarietyOperator : IInstanceVarietyOperator
	{
		#region Infrastructure

	    public static InstanceVarietyOperator Instance { get; } = new();

	    private InstanceVarietyOperator()
	    {
        }

	    #endregion
	}
}