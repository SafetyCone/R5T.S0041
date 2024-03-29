using System;

using R5T.T0131;


namespace R5T.S0041
{
    [ValuesMarker]
    public partial interface IFileNames : IValuesMarker
    {
        public string BuildJsonFileName => "R5T.S0041.Build.json";
        public string BuildProblemsTextFileName => "Build Problems.txt";
        public string BuildProblemProjectsTextFileName => "Build Problem Projects.txt";
        public string InstancesJsonFileName => "Instances.json";
        public string NewInstancesJsonFileName => "Instances-New.json";
        public string OldInstancesJsonFileName => "Instances-Old.json";
        public string ProblemProcessingProjectsTextFileName => "Problem Processing Projects.txt";
        public string ProcessingProblemsTextFileName => "Processing Problems.txt";
        public string ProcessingProblemProjectsTextFileName => "Processing Problem Projects.txt";
        public string ProcessedProjectsTextFileName => "Processed Projects.txt";
        public string ProjectFileTuplesJsonFileName => "Project File Tuples.json";
        public string ProjectsListTextFileName => "Projects.txt";
        public string SummaryTextFileName => "Summary.txt";
        public string ProcessingSummaryTextFileName => "Summary-Processing.txt";
        public string DateComparisonSummaryTextFilePath => "Summary-Date Comparison.txt";
        public string NewAndOldSummaryTextFilePath => "Summary-New and Old.txt";
    }
}
