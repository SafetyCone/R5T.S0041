using System;

using R5T.T0131;


namespace R5T.S0041
{
	[ValuesMarker]
	public interface IInstanceVariety : IValuesMarker
	{
		public string DraftDemonstrations => "Draft Demonstrations";
		public string DraftExperiments => "Draft Experiments";
		public string DraftExplorations => "Draft Explorations";
		public string DraftFunctionality => "Draft Functionality";
		public string DraftValues => "Draft Values";

		public string Demonstrations => "Demonstrations";
		public string Experiments => "Experiments";
		public string Explorations => "Explorations";
		public string Functionality => "Functionality";
		public string Values => "Values";
	}
}