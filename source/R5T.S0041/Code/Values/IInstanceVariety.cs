using System;

using R5T.T0131;


namespace R5T.S0041
{
	[ValuesMarker]
	public interface IInstanceVariety : IValuesMarker
	{
		public string DraftMarkerAttribute => "Marker Attributes-Draft";
		public string MarkerAttribute => "Marker Attributes";

		public string DraftConstants => "Constants-Draft-OBSOLETE";
		public string DraftDataType => "Data Type-Draft";
		public string DraftDemonstrations => "Demonstrations-Draft";
		public string DraftExperiments => "Experiments-Draft";
		public string DraftExplorations => "Explorations-Draft";
		public string DraftFunctionality => "Functionality-Draft";
		public string DraftUtilityType => "Utility Type-Draft";
		public string DraftValues => "Values-Draft";

		public string Constants => "Constants-OBSOLETE";
		public string DataType => "Data Type";
		public string Demonstrations => "Demonstrations";
		public string Experiments => "Experiments";
		public string Explorations => "Explorations";
		public string Functionality => "Functionality";
		public string UtilityType => "Utility Type";
		public string Values => "Values";
	}
}