using System;
using System.Linq;

using R5T.T0132;


namespace R5T.S0041
{
	[FunctionalityMarker]
	public partial interface IFileNameOperator : IFunctionalityMarker,
		F0000.IFileNameOperator
	{
		public string[] GetAllJsonOutputFileNames()
		{
			var allVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames();

			var output = allVarietyNames
				.Select(varietyName => this.GetJsonOutputFileName_ForInstanceVariety(varietyName))
				.ToArray();

			return output;
		}

		public string GetDatedComparisonFileName(
			string fileName,
			DateTime earlierDate,
			DateTime laterDate)
        {
			var earlierYyyymmdd = Instances.DateOperator.ToString_YYYYMMDD(earlierDate);
			var laterYyyymmdd = Instances.DateOperator.ToString_YYYYMMDD(laterDate);

			var appendix = $"-{earlierYyyymmdd} to {laterYyyymmdd}";

			var output = Instances.FileNameOperator.AppendToFileNameStem(
				fileName,
				appendix);

			return output;
        }

		public string GetDatedFileName(
			string fileName,
			DateTime date)
        {
			var yyyymmdd = Instances.DateOperator.ToString_YYYYMMDD(date);

			var appendix = $"-{yyyymmdd}";

			var output = Instances.FileNameOperator.AppendToFileNameStem(
				fileName,
				appendix);

			return output;
        }

		public string GetDepartedFileName_FromFileName(string fileName)
		{
			var departedFileName = Instances.FileNameOperator.AppendToFileNameStem(
				fileName,
				"-Departed");

			return departedFileName;
		}

		public string GetJsonOutputFileName_ForInstanceVariety(string instanceVarietyName)
		{
			var fileNameStem = this.GetOutputFileNameStem_ForInstanceVariety(instanceVarietyName);

			var output = this.GetFileName(
				fileNameStem,
				Instances.FileExtensions.Json);

			return output;
		}

		public string GetNewFileName_FromFileName(string fileName)
		{
			var departedFileName = Instances.FileNameOperator.AppendToFileNameStem(
				fileName,
				"-New");

			return departedFileName;
		}

		public string GetOutputFileNameStem_ForInstanceVariety(string instanceVarietyName)
		{
			// Just return the instance variety name.
			var output = instanceVarietyName;
			return output;
		}

		public string GetTextOutputFileName_ForInstanceVariety(string instanceVarietyName)
		{
			var fileNameStem = this.GetOutputFileNameStem_ForInstanceVariety(instanceVarietyName);

			var output = this.GetFileName(
				fileNameStem,
				Instances.FileExtensions.Text);

			return output;
		}
	}
}