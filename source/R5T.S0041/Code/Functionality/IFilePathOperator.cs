using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;


namespace R5T.S0041
{
	[FunctionalityMarker]
	public partial interface IFilePathOperator : IFunctionalityMarker
	{
		public string[] GetAllJsonOutputFilePaths()
        {
			var allVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames();

			var output = allVarietyNames
				.Select(varietyName => this.GetJsonOutputFilePath_ForInstanceVariety(varietyName))
				.ToArray();

			return output;
        }

		public IEnumerable<(string InstanceVarietyName, string DatedComparisonJsonOutputFilePath)> GetDatedComparisonJsonOutputFilePaths_ForInstanceVarieties(
			IEnumerable<string> instanceVarietyNames,
			string outputDirectoryPath,
			DateTime earlierDate,
			DateTime laterDate)
		{
			var output = instanceVarietyNames
				.Select(instanceVarietyName =>
				{
					var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

					var datedComparisonFileName = Instances.FileNameOperator.GetDatedComparisonFileName(
						fileName,
						earlierDate,
						laterDate);

					var datedComparisonJsonOutputFilePath = Instances.PathOperator.GetFilePath(
						outputDirectoryPath,
						datedComparisonFileName);

					return (instanceVarietyName, datedComparisonJsonOutputFilePath);
				});

			return output;
		}

		public IEnumerable<(string InstanceVarietyName, string DatedJsonOutputFilePath)> GetDatedJsonOutputFilePaths_ForInstanceVarieties(
			IEnumerable<string> instanceVarietyNames,
			string outputDirectoryPath,
			DateTime date)
		{
			var output = instanceVarietyNames
				.Select(instanceVarietyName =>
				{
					var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

					var datedFileName = Instances.FileNameOperator.GetDatedFileName(
						fileName,
						date);

					var datedJsonOutputFilePath = Instances.PathOperator.GetFilePath(
						outputDirectoryPath,
						datedFileName);

					return (instanceVarietyName, datedJsonOutputFilePath);
				});

			return output;
		}

		public IEnumerable<(string InstanceVarietyName, string jsonOutputFilePath)> GetJsonOutputFilePaths_ForInstanceVarieties(
			IEnumerable<string> instanceVarietyNames,
			string outputDirectoryPath)
        {
			var output = instanceVarietyNames
				.Select(instanceVarietyName =>
				{
					var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

					var jsonOutputFilePath = Instances.PathOperator.GetFilePath(
						outputDirectoryPath,
						fileName);

					return (instanceVarietyName, jsonOutputFilePath);
				});

			return output;
        }

		public string GetJsonOutputFilePath_ForInstanceVariety(string instanceVarietyName)
        {
			var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

			var output = Instances.PathOperator.GetFilePath(
				Instances.DirectoryPaths.InitialOutputDirectorPath,
				fileName);

			return output;
		}

		public string GetTextOutputFilePath_ForInstanceVariety(string instanceVarietyName)
        {
			var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

			var output = Instances.PathOperator.GetFilePath(
				Instances.DirectoryPaths.InitialOutputDirectorPath,
				fileName);

			return output;
		}
	}
}