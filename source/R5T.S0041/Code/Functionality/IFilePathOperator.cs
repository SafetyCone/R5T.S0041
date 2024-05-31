using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;


namespace R5T.S0041
{
	[FunctionalityMarker]
	public partial interface IFilePathOperator : IFunctionalityMarker
	{
		public string[] GetAllCloudTextOutputFilePaths_InPresentationOrder()
        {
			var instanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

			var textOutputFilePaths = instanceVarietyNames
				.Select(instanceVarietyName => Instances.FilePathOperator.GetTextOutputFilePath_ForInstanceVariety(instanceVarietyName))
				;

			var cloudOutputTextFilePaths = textOutputFilePaths
				.Select(textOutputFilePath => Instances.PathOperator.Get_DestinationFilePath(
					textOutputFilePath,
					Instances.DirectoryPaths.CloudOutputDirectoryPath))
				.ToArray();

			return cloudOutputTextFilePaths;
		}

		public string[] GetAllCloudTextOutputFilePaths()
        {
			var allTextOutputFilePaths = this.GetAllTextOutputFilePaths();

			var allCloudTextOutputFilePaths = Instances.PathOperator.Get_DestinationFilePaths(
				allTextOutputFilePaths,
				Instances.DirectoryPaths.CloudOutputDirectoryPath)
				.ToArray();

			return allCloudTextOutputFilePaths;
        }

		public string[] GetAllTextOutputFilePaths()
        {
			var allInstanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames();

			var allInstanceVarietyTextOutputFilePaths = allInstanceVarietyNames
				.Select(varietyName => Instances.FilePathOperator.GetTextOutputFilePath_ForInstanceVariety(varietyName))
				.ToArray();

			return allInstanceVarietyTextOutputFilePaths;
		}

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

					var datedComparisonJsonOutputFilePath = Instances.PathOperator.Get_FilePath(
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

					var datedJsonOutputFilePath = Instances.PathOperator.Get_FilePath(
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

					var jsonOutputFilePath = Instances.PathOperator.Get_FilePath(
						outputDirectoryPath,
						fileName);

					return (instanceVarietyName, jsonOutputFilePath);
				});

			return output;
        }

		public string GetJsonOutputFilePath_ForInstanceVariety(string instanceVarietyName)
        {
			var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

			var output = Instances.PathOperator.Get_FilePath(
				Instances.DirectoryPaths.InitialOutputDirectorPath,
				fileName);

			return output;
		}

		public string GetTextOutputFilePath_ForInstanceVariety(string instanceVarietyName)
        {
			var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

			var output = Instances.PathOperator.Get_FilePath(
				Instances.DirectoryPaths.InitialOutputDirectorPath,
				fileName);

			return output;
		}
	}
}