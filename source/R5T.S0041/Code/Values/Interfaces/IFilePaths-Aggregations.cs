using System;

using R5T.T0131;


namespace R5T.S0041
{
	public partial interface IFilePaths
    {
        public string[] AllInstanceVarietyTextFiles => new[]
        {
            this.FunctionalityOutputFilePath_Text,
            this.DraftFunctionalityOutputFilePath_Text,
            this.ValuesOutputFilePath_Text,
            this.DraftValuesOutputFilePath_Text,

            this.ExperimentsOutputFilePath_Text,
            this.DraftExperimentsOutputFilePath_Text,
            this.ExplorationsOutputFilePath_Text,
            this.DraftExplorationsOutputFilePath_Text,
            this.DemonstrationsOutputFilePath_Text,
            this.DraftDemonstrationsOutputFilePath_Text,
        };
    }
}