using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using R5T.Magyar;

using R5T.D0105;
using R5T.T0020;



namespace R5T.S0041
{
    /// <summary>
    /// Surveys all repositories to find types marked with the functionality marker attribute.
    /// * Output results to a text file.
    /// </summary>
    public class O001_SurveyForFunctionality : IActionOperation
    {
        private ILogger Logger { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }


        public O001_SurveyForFunctionality(
            ILogger<O001_SurveyForFunctionality> logger,
            INotepadPlusPlusOperator notepadPlusPlusOperator)
        {
            this.Logger = logger;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
        }

        public async Task Run()
        {
            /// Inputs.
            var useProjectsCache = false;

            /// Run.
            string title = "Functionalities";

            var jsonOutputFilePath = Instances.FilePaths.FunctionalityOutputFilePath_Json;
            var textOutputFilePath = Instances.FilePaths.FunctionalityOutputFilePath_Text;

            await this.Run_Core(
                useProjectsCache,
                title,
                jsonOutputFilePath,
                textOutputFilePath,
                Instances.Operations.GetFunctionalityDescriptors);
        }

        public async Task Run_Core(
            bool useProjectsCache,
            string title,
            string jsonOutputFilePath,
            string textOutputFilePath,
            Func<ICollection<ProjectFilesTuple>, ILogger, (List<InstanceDescriptor> functionalityDescriptors, List<Failure<string>> problemProjects)> getDescriptors)
        {
            /// Run.
            // Survey projects, or use the existing projects cache.
            var projectFilesTuples = Instances.Operations.GetProjectFilesTuples(useProjectsCache);

            // Get functionalities.
            var (functionalityDescriptors, problemProjects) = getDescriptors(
                projectFilesTuples,
                this.Logger);

            // Output problems.
            var problemProjectsFilePath = Instances.FilePaths.ProblemProjectsFilePath;
            await Instances.Operations.WriteProblemProjectsFile(
                problemProjectsFilePath,
                problemProjects);

            // Output JSON format data.
            JsonFileHelper.WriteToFile(
                jsonOutputFilePath,
                functionalityDescriptors);

            // Output text format data.
            await Instances.Operations.WriteFunctionalityDescriptors(
                title,
                textOutputFilePath,
                functionalityDescriptors);

            // Output summary file.
            var problemProjectReasonCounts = problemProjects
                .GroupBy(x => x.Message)
                .Select(x => (Reason: x.Key, Count: x.Count()))
                .Now();

            var lines = EnumerableHelper.From($"{title}: {functionalityDescriptors.Count}\n\nProblem projects ({problemProjects.Count}). Reasons:")
                .Append(problemProjectReasonCounts
                    // Order by descending count, then alphabetically by reason.
                    .GroupBy(x => x.Count)
                    .OrderByDescending(x => x.Key)
                    .SelectMany(xGrouping => xGrouping
                        .OrderAlphabetically(x => x.Reason)
                        .Select(x => $"\n({x.Count}) {x.Reason}")));

            var summaryFilePath = Instances.FilePaths.SummaryFilePath;

            await FileHelper.WriteAllLines(
                summaryFilePath,
                lines);

            // Show outputs.
            var projectsListFilePath = Instances.Operations.GetProjectsListTextFilePath();
            var projectFilesTuplesJsonFilePath = Instances.Operations.GetProjectFilesTuplesJsonFilePath();

            await this.NotepadPlusPlusOperator.OpenFilePath(projectsListFilePath);
            await this.NotepadPlusPlusOperator.OpenFilePath(projectFilesTuplesJsonFilePath);

            await this.NotepadPlusPlusOperator.OpenFilePath(problemProjectsFilePath);

            await this.NotepadPlusPlusOperator.OpenFilePath(jsonOutputFilePath);
            await this.NotepadPlusPlusOperator.OpenFilePath(textOutputFilePath);

            // Open summary file last so that it's the first thing user sees in Notepad++.
            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);
        }
    }
}
