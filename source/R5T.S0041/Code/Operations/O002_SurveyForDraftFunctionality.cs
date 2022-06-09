using System;
using System.Threading.Tasks;

using R5T.T0020;


namespace R5T.S0041
{
    public class O002_SurveyForDraftFunctionality : IActionOperation
    {
        private O001_SurveyForFunctionality O001_SurveyForFunctionality { get; }


        public O002_SurveyForDraftFunctionality(
            O001_SurveyForFunctionality o001_SurveyForFunctionality)
        {
            this.O001_SurveyForFunctionality = o001_SurveyForFunctionality;
        }

        public async Task Run()
        {
            /// Inputs.
            var useProjectsCache = false;

            /// Run.
            string title = "Draft Functionalities";

            var jsonOutputFilePath = Instances.FilePaths.DraftFunctionalityOutputFilePath_Json;
            var textOutputFilePath = Instances.FilePaths.DraftFunctionalityOutputFilePath_Text;

            await this.O001_SurveyForFunctionality.Run_Core(
                useProjectsCache,
                title,
                jsonOutputFilePath,
                textOutputFilePath,
                Instances.Operations.GetDraftFunctionalityDescriptors);
        }
    }
}
