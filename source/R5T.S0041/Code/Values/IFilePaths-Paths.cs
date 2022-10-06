using System;

using R5T.T0131;


namespace R5T.S0041
{
	public partial interface IFilePaths
	{
		// Functionality
		public string FunctionalityOutputFilePath_Text => @"C:\Temp\Functionality.txt";
		public string FunctionalityOutputFilePath_Json => @"C:\Temp\Functionality.json";

		public string DepartedFunctionalitiesFilePath_Json => @"C:\Temp\Functionalities-Departed.json";
		public string NewFunctionalitiesFilePath_Json => @"C:\Temp\Functionalities-New.json";

		public string DraftFunctionalityOutputFilePath_Text => @"C:\Temp\Functionality-Draft.txt";
		public string DraftFunctionalityOutputFilePath_Json => @"C:\Temp\Functionality-Draft.json";

		public string DepartedDraftFunctionalitiesFilePath_Json => @"C:\Temp\Draft Functionalities-Departed.json";
		public string NewDraftFunctionalitiesFilePath_Json => @"C:\Temp\Draft Functionalities-New.json";

		// Values.
		public string ValuesOutputFilePath_Text => @"C:\Temp\Values.txt";
		public string ValuesOutputFilePath_Json => @"C:\Temp\Values.json";

		public string DepartedValuesFilePath_Json => @"C:\Temp\Values-Departed.json";
		public string NewValuesFilePath_Json => @"C:\Temp\Values-New.json";

		public string DraftValuesOutputFilePath_Text => @"C:\Temp\Values-Draft.txt";
		public string DraftValuesOutputFilePath_Json => @"C:\Temp\Values-Draft.json";

		public string DepartedDraftValuesFilePath_Json => @"C:\Temp\Draft Values-Departed.json";
		public string NewDraftValuesFilePath_Json => @"C:\Temp\Draft Values-New.json";

		// Explorations
		public string ExplorationsOutputFilePath_Text => @"C:\Temp\Explorations.txt";
		public string ExplorationsOutputFilePath_Json => @"C:\Temp\Explorations.json";

		public string DepartedExplorationsOutputFilePath_Json => @"C:\Temp\Explorations-Departed.json";
		public string NewExplorationsOutputFilePath_Json => @"C:\Temp\Explorations-New.json";

		public string DraftExplorationsOutputFilePath_Text => @"C:\Temp\Explorations-Draft.txt";
		public string DraftExplorationsOutputFilePath_Json => @"C:\Temp\Explorations-Draft.json";

		public string DepartedDraftExplorationsOutputFilePath_Json => @"C:\Temp\Explorations-Draft-Departed.json";
		public string NewDraftExplorationsOutputFilePath_Json => @"C:\Temp\Explorations-Draft-New.json";

		// Experiments
		public string ExperimentsOutputFilePath_Text => @"C:\Temp\Experiments.txt";
		public string ExperimentsOutputFilePath_Json => @"C:\Temp\Experiments.json";

		public string DepartedExperimentsOutputFilePath_Json => @"C:\Temp\Experiments-Departed.json";
		public string NewExperimentsOutputFilePath_Json => @"C:\Temp\Experiments-New.json";

		public string DraftExperimentsOutputFilePath_Text => @"C:\Temp\Experiments-Draft.txt";
		public string DraftExperimentsOutputFilePath_Json => @"C:\Temp\Experiments-Draft.json";

		public string DepartedDraftExperimentsOutputFilePath_Json => @"C:\Temp\Experiments-Draft-Departed.json";
		public string NewDraftExperimentsOutputFilePath_Json => @"C:\Temp\Experiments-Draft-New.json";

		// Demonstrations
		public string DemonstrationsOutputFilePath_Text => @"C:\Temp\Demonstrations.txt";
		public string DemonstrationsOutputFilePath_Json => @"C:\Temp\Demonstrations.json";

		public string DepartedDemonstrationsOutputFilePath_Json => @"C:\Temp\Demonstrations-Departed.json";
		public string NewDemonstrationsOutputFilePath_Json => @"C:\Temp\Demonstrations-New.json";

		public string DraftDemonstrationsOutputFilePath_Text => @"C:\Temp\Demonstrations-Draft.txt";
		public string DraftDemonstrationsOutputFilePath_Json => @"C:\Temp\Demonstrations-Draft.json";

		public string DepartedDraftDemonstrationsOutputFilePath_Json => @"C:\Temp\Demonstrations-Draft-Departed.json";
		public string NewDraftDemonstrationsOutputFilePath_Json => @"C:\Temp\Demonstrations-Draft-New.json";

		// Other files.
		public string ProblemProjectsFilePath => @"C:\Temp\Problem Projects.txt";
		public string SummaryFilePath => @"C:\Temp\Summary.txt";
		public string SummaryPresentationFilePath => @"C:\Temp\Instances Summary.txt";
	}
}