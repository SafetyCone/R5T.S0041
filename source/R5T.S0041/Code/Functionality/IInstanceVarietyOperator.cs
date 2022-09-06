using System;

using R5T.T0132;


namespace R5T.S0041
{
	[FunctionalityMarker]
	public partial interface IInstanceVarietyOperator : IFunctionalityMarker
	{
		public string[] GetAllInstanceVarietyNames()
        {
			var instanceVarietyValues = Instances.InstanceVariety;

			var output = new[]
			{
				instanceVarietyValues.Constants,
				instanceVarietyValues.DataType,
				instanceVarietyValues.Demonstrations,
				instanceVarietyValues.DraftConstants,
				instanceVarietyValues.DraftDataType,
				instanceVarietyValues.DraftDemonstrations,
				instanceVarietyValues.DraftExperiments,
				instanceVarietyValues.DraftExplorations,
				instanceVarietyValues.DraftFunctionality,
				instanceVarietyValues.DraftMarkerAttribute,
				instanceVarietyValues.DraftUtilityType,
				instanceVarietyValues.DraftValues,
				instanceVarietyValues.Experiments,
				instanceVarietyValues.Explorations,
				instanceVarietyValues.Functionality,
				instanceVarietyValues.MarkerAttribute,
				instanceVarietyValues.UtilityType,
				instanceVarietyValues.Values,
			};

			return output;
        }

		public string GetInstanceVarietyName(string markerAttributeNamespacedTypeName)
        {
			// Use the type name of the marker attribute.
			var instanceVarietyName = F0000.Instances.NamespacedTypeNameOperator.GetTypeName(markerAttributeNamespacedTypeName);
			return instanceVarietyName;
        }
	}
}