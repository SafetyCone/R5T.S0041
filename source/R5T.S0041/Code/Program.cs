using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using R5T.Magyar;

using R5T.D0088;
using R5T.D0090;
using R5T.D0105;


namespace R5T.S0041
{
    class Program : ProgramAsAServiceBase
    {
        #region Static
        
        static async Task Main()
        {
            //OverridableProcessStartTimeProvider.Override("20211214 - 163052");
            //OverridableProcessStartTimeProvider.DoNotOverride();
        
            await Instances.Host.NewBuilder()
                .UseProgramAsAService<Program, T0075.IHostBuilder>()
                .UseHostStartup<HostStartup, T0075.IHostBuilder>(Instances.ServiceAction.AddHostStartupAction())
                .Build()
                .SerializeConfigurationAudit()
                .SerializeServiceCollectionAudit()
                .RunAsync();
        }

        #endregion


        private ILogger Logger { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }


        public Program(IServiceProvider serviceProvider,
            ILogger<Program> logger,
            INotepadPlusPlusOperator notepadPlusPlusOperator)
            : base(serviceProvider)
        {
            this.Logger = logger;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
        }

        protected override Task ServiceMain(CancellationToken stoppingToken)
        {
            //return this.RunOperation();
            return this.RunMethod();
        }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0051 // Remove unused private members

        private Task RunOperation()
        {
            return this.ServiceProvider.Run<O001_SurveyForFunctionality>();
            //return this.ServiceProvider.Run<O002_SurveyForDraftFunctionality>();
        }

        private Task RunMethod()
        {
            //return this.ListTypesAndAttributesInAssembly();
            //return this.ListTypesAndTheirImplementedInterfacesInAssembly();
            //return this.FindFunctionalityTypesInAssembly();
            //return this.GetAllProjectFilesTuples();
            //return this.GetAllDraftFunctionalityInterfaces();
            //return this.GetAllDraftFunctionalityMethods();
            //return this.GetDraftFunctionalityDescriptors();
            //return this.GetFunctionalityDescriptors();
            //return this.GetAllDescriptors();
            //return this.CreateDatedFiles();
            //return this.CompareFilesForTwoDates();
            return this.CreateSummaryFile();
        }

        private async Task CreateSummaryFile()
        {
            /// Inputs.
            var outputDirectoryPath = @"C:\Temp\Output\S0041\";

            //var date = Instances.DateOperator.GetToday();
            var date = Instances.DateOperator.From_YYYYMMDD("20220601");

            /// Run.
            var summaryOutputDirectoryPath = Instances.PathOperator.GetDirectoryPath(
                outputDirectoryPath,
                "Summaries");

            var dateString = Instances.DateOperator.ToString_YYYYMMDD(date);

            var datedSummaryOutputJsonFileName = $"Summary-{dateString}.json";

            var datedSummaryOutputJsonFilePath = Instances.PathOperator.GetFilePath(
                summaryOutputDirectoryPath,
                datedSummaryOutputJsonFileName);

            var datedOutputDirectoryPath = Instances.Operations.GetDatedOutputDirectoryPath(
                    outputDirectoryPath,
                    date);

            //// Non-ideality of file names with 
            //var datedFunctionalityFilePath = Instances.Operations.GetDatedOutputFilePath(
            //    date,
            //    Instances.FilePaths.FunctionalityOutputFilePath_Json,
            //    datedOutputDirectoryPath);
            //var datedValuesFilePath = Instances.Operations.GetDatedOutputFilePath(
            //    date,
            //    Instances.FilePaths.ValuesOutputFilePath_Json,
            //    datedOutputDirectoryPath);

            var summaryModifiersByFunctionalityFilePath = new Dictionary<string, Action<int, DatedFunctionalitySummary>>
            {
                // Functionality, new, departed.
                {
                    //datedFunctionalityFilePath,
                    Instances.FilePaths.FunctionalityOutputFilePath_Json,
                    (count, summary) => { summary.FunctionalityCount = count; }
                },
                {
                    Instances.FilePaths.NewFunctionalitiesFilePath_Json,
                    (count, summary) => { summary.NewFunctionalityCount = count; }
                },
                {
                    Instances.FilePaths.DepartedFunctionalitiesFilePath_Json,
                    (count, summary) => { summary.DepartedFunctionalityCount = count; }
                },
                // Draft functionality, new, departed.
                {
                    Instances.FilePaths.DraftFunctionalityOutputFilePath_Json,
                    (count, summary) => { summary.DraftFunctionalityCount = count; }
                },
                {
                    Instances.FilePaths.NewDraftFunctionalitiesFilePath_Json,
                    (count, summary) => { summary.NewDraftFunctionalityCount = count; }
                },
                {
                    Instances.FilePaths.DepartedDraftFunctionalitiesFilePath_Json,
                    (count, summary) => { summary.DepartedDraftFunctionalityCount = count; }
                },
                // Values, new, departed.
                {
                    //datedValuesFilePath,
                    Instances.FilePaths.ValuesOutputFilePath_Json,
                    (count, summary) => { summary.ValuesCount = count; }
                },
                {
                    Instances.FilePaths.NewValuesFilePath_Json,
                    (count, summary) => { summary.NewValuesCount = count; }
                },
                {
                    Instances.FilePaths.DepartedValuesFilePath_Json,
                    (count, summary) => { summary.DepartedValuesCount = count; }
                },
                // Draft values, new, departed.
                {
                    Instances.FilePaths.DraftValuesOutputFilePath_Json,
                    (count, summary) => { summary.DraftValuesCount = count; }
                },
                {
                    Instances.FilePaths.NewDraftValuesFilePath_Json,
                    (count, summary) => { summary.NewDraftValuesCount = count; }
                },
                {
                    Instances.FilePaths.DepartedDraftValuesFilePath_Json,
                    (count, summary) => { summary.DepartedDraftValuesCount = count; }
                },
                // Demonstrations, new, departed.
                {
                    Instances.FilePaths.DemonstrationsOutputFilePath_Json,
                    (count, summary) => { summary.DemonstrationsCount = count; }
                },
                {
                    Instances.FilePaths.NewDemonstrationsOutputFilePath_Json,
                    (count, summary) => { summary.NewDemonstrationsCount = count; }
                },
                {
                    Instances.FilePaths.DepartedDemonstrationsOutputFilePath_Json,
                    (count, summary) => { summary.DepartedDemonstrationsCount = count; }
                },
                // Draft demonstrations, new, departed.
                {
                    Instances.FilePaths.DraftDemonstrationsOutputFilePath_Json,
                    (count, summary) => { summary.DraftDemonstrationsCount = count; }
                },
                {
                    Instances.FilePaths.NewDraftDemonstrationsOutputFilePath_Json,
                    (count, summary) => { summary.NewDraftDemonstrationsCount = count; }
                },
                {
                    Instances.FilePaths.DepartedDraftDemonstrationsOutputFilePath_Json,
                    (count, summary) => { summary.DepartedDraftDemonstrationsCount = count; }
                },
                // Experiments, new, departed.
                {
                    Instances.FilePaths.ExperimentsOutputFilePath_Json,
                    (count, summary) => { summary.ExperimentsCount = count; }
                },
                {
                    Instances.FilePaths.NewExperimentsOutputFilePath_Json,
                    (count, summary) => { summary.NewExperimentsCount = count; }
                },
                {
                    Instances.FilePaths.DepartedExperimentsOutputFilePath_Json,
                    (count, summary) => { summary.DepartedExperimentsCount = count; }
                },
                // Draft experiments, new, departed.
                {
                    Instances.FilePaths.DraftExperimentsOutputFilePath_Json,
                    (count, summary) => { summary.DraftExperimentsCount = count; }
                },
                {
                    Instances.FilePaths.NewDraftExperimentsOutputFilePath_Json,
                    (count, summary) => { summary.NewDraftExperimentsCount = count; }
                },
                {
                    Instances.FilePaths.DepartedDraftExperimentsOutputFilePath_Json,
                    (count, summary) => { summary.DepartedDraftExperimentsCount = count; }
                },
                // Explorations, new, departed.
                {
                    Instances.FilePaths.ExplorationsOutputFilePath_Json,
                    (count, summary) => { summary.ExplorationsCount = count; }
                },
                {
                    Instances.FilePaths.NewExplorationsOutputFilePath_Json,
                    (count, summary) => { summary.NewExplorationsCount = count; }
                },
                {
                    Instances.FilePaths.DepartedExplorationsOutputFilePath_Json,
                    (count, summary) => { summary.DepartedExplorationsCount = count; }
                },
                // Draft explorations, new, departed.
                {
                    Instances.FilePaths.DraftExplorationsOutputFilePath_Json,
                    (count, summary) => { summary.DraftExplorationsCount = count; }
                },
                {
                    Instances.FilePaths.NewDraftExplorationsOutputFilePath_Json,
                    (count, summary) => { summary.NewDraftExplorationsCount = count; }
                },
                {
                    Instances.FilePaths.DepartedDraftExplorationsOutputFilePath_Json,
                    (count, summary) => { summary.DepartedDraftExplorationsCount = count; }
                },
            };

            var summary = new DatedFunctionalitySummary
            {
                AsOfDate = date,
            };

            foreach (var pair in summaryModifiersByFunctionalityFilePath)
            {
                // Find the file matching the file name stem.
                var datedFilePath = Instances.Operations.GetChildFilePathMatchingFileNameStem(
                    pair.Key,
                    datedOutputDirectoryPath);

                var descriptors = JsonFileHelper.LoadFromFile<FunctionalityDescriptor[]>(datedFilePath);

                var count = descriptors.Length;

                pair.Value(count, summary);
            }

            // Write summary.
            Instances.FileSystemOperator.CreateDirectoryOkIfExists(summaryOutputDirectoryPath);

            JsonFileHelper.WriteToFile(
                datedSummaryOutputJsonFilePath,
                summary);

            await this.NotepadPlusPlusOperator.OpenFilePath(datedSummaryOutputJsonFilePath);
        }

        private Task CompareFilesForTwoDates()
        {
            /// Inputs.
            var outputDirectoryPath = @"C:\Temp\Output\S0041\";

            //var firstDate = Instances.DateOperator.GetYesterday();
            //var firstDate = Instances.DateOperator.GetDefault();
            var firstDate = Instances.DateOperator.From_YYYYMMDD("20220531");
            //var secondDate = Instances.DateOperator.GetToday();
            var secondDate = Instances.DateOperator.From_YYYYMMDD("20220601");

            /// Run.
            var functionalityFilePathsByFunctionalityVariety = new Dictionary<string, string>
            {
                { Instances.InstanceVariety.Functionality, Instances.FilePaths.FunctionalityOutputFilePath_Json },
                { Instances.InstanceVariety.DraftFunctionality, Instances.FilePaths.DraftFunctionalityOutputFilePath_Json },
                { Instances.InstanceVariety.Values, Instances.FilePaths.ValuesOutputFilePath_Json },
                { Instances.InstanceVariety.DraftValues, Instances.FilePaths.DraftValuesOutputFilePath_Json },

                { Instances.InstanceVariety.Experiments, Instances.FilePaths.ExperimentsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftExperiments, Instances.FilePaths.DraftExperimentsOutputFilePath_Json },
                { Instances.InstanceVariety.Explorations, Instances.FilePaths.ExplorationsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftExplorations, Instances.FilePaths.DraftExplorationsOutputFilePath_Json },
                { Instances.InstanceVariety.Demonstrations, Instances.FilePaths.DemonstrationsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftDemonstrations, Instances.FilePaths.DraftDemonstrationsOutputFilePath_Json },
            };

            var newFunctionalityFilePathsByFunctionalityVariety = new Dictionary<string, string>
            {
                { Instances.InstanceVariety.Functionality, Instances.FilePaths.NewFunctionalitiesFilePath_Json },
                { Instances.InstanceVariety.DraftFunctionality, Instances.FilePaths.NewDraftFunctionalitiesFilePath_Json },
                { Instances.InstanceVariety.Values, Instances.FilePaths.NewValuesFilePath_Json },
                { Instances.InstanceVariety.DraftValues, Instances.FilePaths.NewDraftValuesFilePath_Json },

                { Instances.InstanceVariety.Experiments, Instances.FilePaths.NewExperimentsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftExperiments, Instances.FilePaths.NewDraftExperimentsOutputFilePath_Json },
                { Instances.InstanceVariety.Explorations, Instances.FilePaths.NewExplorationsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftExplorations, Instances.FilePaths.NewDraftExplorationsOutputFilePath_Json },
                { Instances.InstanceVariety.Demonstrations, Instances.FilePaths.NewDemonstrationsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftDemonstrations, Instances.FilePaths.NewDraftDemonstrationsOutputFilePath_Json },
            };

            var departedFunctionalityFilePathsByFunctionalityVariety = new Dictionary<string, string>
            {
                { Instances.InstanceVariety.Functionality, Instances.FilePaths.DepartedFunctionalitiesFilePath_Json },
                { Instances.InstanceVariety.DraftFunctionality, Instances.FilePaths.DepartedDraftFunctionalitiesFilePath_Json },
                { Instances.InstanceVariety.Values, Instances.FilePaths.DepartedValuesFilePath_Json },
                { Instances.InstanceVariety.DraftValues, Instances.FilePaths.DepartedDraftValuesFilePath_Json },

                { Instances.InstanceVariety.Experiments, Instances.FilePaths.DepartedExperimentsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftExperiments, Instances.FilePaths.DepartedDraftExperimentsOutputFilePath_Json },
                { Instances.InstanceVariety.Explorations, Instances.FilePaths.DepartedExplorationsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftExplorations, Instances.FilePaths.DepartedDraftExplorationsOutputFilePath_Json },
                { Instances.InstanceVariety.Demonstrations, Instances.FilePaths.DepartedDemonstrationsOutputFilePath_Json },
                { Instances.InstanceVariety.DraftDemonstrations, Instances.FilePaths.DepartedDraftDemonstrationsOutputFilePath_Json },
            };

            FunctionalityDescriptor[] GetFunctionalityForDate(
                DateTime date,
                string functionalityJsonFilePath)
            {
                var isDefault = Instances.DateOperator.IsDefault(date);
                if(isDefault)
                {
                    return Array.Empty<FunctionalityDescriptor>();
                }

                var datedOutputDirectoryPath = Instances.Operations.GetDatedOutputDirectoryPath(
                    outputDirectoryPath,
                    date);

                var datedFunctionalityJsonFilePath = Instances.Operations.GetDatedOutputFilePath(
                   date,
                   functionalityJsonFilePath,
                   datedOutputDirectoryPath);

                var fileExists = Instances.FileSystemOperator.FileExists(datedFunctionalityJsonFilePath);
                if (!fileExists)
                {
                    return Array.Empty<FunctionalityDescriptor>();
                }

                var output = JsonFileHelper.LoadFromFile<FunctionalityDescriptor[]>(datedFunctionalityJsonFilePath);
                return output;
            }

            var secondDatedOutputDirectoryPath = Instances.Operations.GetDatedOutputDirectoryPath(
                outputDirectoryPath,
                secondDate);

            foreach (var pair in functionalityFilePathsByFunctionalityVariety)
            {
                var functionalityFilePath = pair.Value;

                var firstDateFunctionalities = GetFunctionalityForDate(
                    firstDate,
                    functionalityFilePath);

                var secondDateFuntionalities = GetFunctionalityForDate(
                    secondDate,
                    functionalityFilePath);

                var newFunctionalities = secondDateFuntionalities.Except(firstDateFunctionalities,
                    FunctionalityDescriptorDataIdentityEqualityComparer.Instance)
                    .Now();

                var departedFuntionalities = firstDateFunctionalities.Except(secondDateFuntionalities,
                    FunctionalityDescriptorDataIdentityEqualityComparer.Instance);

                // Write out new.
                var newFunctionalityFilePath = newFunctionalityFilePathsByFunctionalityVariety[pair.Key];

                var newDatedFunctionalityFilePath = Instances.Operations.GetDateComparisonOutputFilePath(
                    firstDate,
                    secondDate,
                    newFunctionalityFilePath,
                    secondDatedOutputDirectoryPath);

                JsonFileHelper.WriteToFile(
                    newDatedFunctionalityFilePath,
                    newFunctionalities);

                // Write out departed.
                var departedFunctionalityFilePath = departedFunctionalityFilePathsByFunctionalityVariety[pair.Key];

                var departedDatedFunctionalityFilePath = Instances.Operations.GetDateComparisonOutputFilePath(
                    firstDate,
                    secondDate,
                    departedFunctionalityFilePath,
                    secondDatedOutputDirectoryPath);

                JsonFileHelper.WriteToFile(
                    departedDatedFunctionalityFilePath,
                    departedFuntionalities);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a dated directory, copies files to the dated directory as dated files, for functionality.
        /// </summary>
        private Task CreateDatedFiles()
        {
            /// Inputs.
            var outputDirectoryPath = @"C:\Temp\Output\S0041\";

            //var date = Instances.DateOperator.GetToday();
            var date = Instances.DateOperator.From_YYYYMMDD("20220601");

            /// Run.
            var filePaths = new[]
            {
                Instances.FilePaths.FunctionalityOutputFilePath_Json,
                Instances.FilePaths.DraftFunctionalityOutputFilePath_Json,
                Instances.FilePaths.ValuesOutputFilePath_Json,
                Instances.FilePaths.DraftValuesOutputFilePath_Json,
                Instances.FilePaths.ExperimentsOutputFilePath_Json,
                Instances.FilePaths.DraftExperimentsOutputFilePath_Json,
                Instances.FilePaths.ExplorationsOutputFilePath_Json,
                Instances.FilePaths.DraftExplorationsOutputFilePath_Json,
                Instances.FilePaths.DemonstrationsOutputFilePath_Json,
                Instances.FilePaths.DraftDemonstrationsOutputFilePath_Json,
            };

            var datedOutputDirectoryPath = Instances.Operations.GetDatedOutputDirectoryPath(
                outputDirectoryPath,
                date);

            var fileCopyPairs = filePaths
                .Select(filePath => (FilePath: filePath, DatedFilePath: Instances.Operations.GetDatedOutputFilePath(
                    date,
                    filePath,
                    datedOutputDirectoryPath)))
                .ToArray();

            // Copy.
            Instances.FileSystemOperator.CreateDirectory(datedOutputDirectoryPath);

            foreach (var (filePath, datedFilePath) in fileCopyPairs)
            {
                Instances.FileSystemOperator.CopyFile(
                    sourceFilePath: filePath,
                    destinationFilePath: datedFilePath);
            }

            return Task.CompletedTask;
        }

        private async Task GetAllDescriptors()
        {
            /// Inputs.
            var useProjectsCache = false;

            /// Run.
            var projectFilesTuples = Instances.Operations.GetProjectFilesTuples(useProjectsCache)
                //// For debugging.
                //.Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.F0004\source\R5T.F0004\R5T.F0004.csproj")
                //.Now()
                ;

            // Assembly actions.
            var methodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety = new Dictionary<string, string>
            {
                { Instances.InstanceVariety.Functionality, Instances.NamespacedTypeNames.FunctionalityMarkerAttribute },
                { Instances.InstanceVariety.DraftFunctionality, Instances.NamespacedTypeNames.DraftFunctionalityMarkerAttribute },
                { Instances.InstanceVariety.Explorations, Instances.NamespacedTypeNames.ExplorationsMarkerAttribute },
                { Instances.InstanceVariety.DraftExplorations, Instances.NamespacedTypeNames.DraftExplorationsMarkerAttribute },
                { Instances.InstanceVariety.Experiments, Instances.NamespacedTypeNames.ExperimentsMarkerAttribute },
                { Instances.InstanceVariety.DraftExperiments, Instances.NamespacedTypeNames.DraftExperimentsMarkerAttribute },
                { Instances.InstanceVariety.Demonstrations, Instances.NamespacedTypeNames.DemonstrationsMarkerAttribute },
                { Instances.InstanceVariety.DraftDemonstrations, Instances.NamespacedTypeNames.DraftDemonstrationsMarkerAttribute },
            };

            var propertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety = new Dictionary<string, string>
            {
                { Instances.InstanceVariety.Values, Instances.NamespacedTypeNames.ValuesMarkerAttribute },
                { Instances.InstanceVariety.DraftValues, Instances.NamespacedTypeNames.DraftValuesMarkerAttribute },
            };

            var getInstanceMethodsByInstanceVariety = methodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety
                .Select(xPair => (xPair.Key, Instances.Operations.GetInstanceMethodNamesProviderFunction(xPair.Value)))
                .Append(propertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety
                    .Select(xPair => (xPair.Key, Instances.Operations.GetInstancePropertyNamesProviderFunction(xPair.Value))))
                .ToDictionary(
                    x => x.Key,
                    x => x.Item2);

            var functionalityDescriptorsByFunctionalityVariety = new Dictionary<string, List<FunctionalityDescriptor>>();

            // Pre-add all lists in case there are no successfully processed projects.
            foreach (var pair in getInstanceMethodsByInstanceVariety)
            {
                functionalityDescriptorsByFunctionalityVariety.Add(pair.Key, new List<FunctionalityDescriptor>());
            }

            Instances.Operations.ProcessProjectFilesTuples(
                projectFilesTuples,
                this.Logger,
                tuple =>
                {
                    // Get project documentation file contents.
                    var documentationByMemberIdentityName = Instances.Operations.GetDocumentationByMemberIdentityName(
                        tuple.DocumentationFilePath);

                    var functionalityMethodNamesSetsByFunctionalityVariety = new Dictionary<string, FunctionalityMethodNames[]>();

                    // Perform all assembly actions on the assembly.
                    Instances.ReflectionOperator.InAssemblyContext(
                        tuple.AssemblyFilePath,
                        EnumerableHelper.From(@"C:\Users\David\Dropbox\Organizations\Rivet\Shared\Binaries\Nuget Assemblies\"),
                        assembly =>
                        {
                            getInstanceMethodsByInstanceVariety.ForEach(
                                pair =>
                                {
                                    var functionalityMethodNamesSet = pair.Value(assembly);

                                    functionalityMethodNamesSetsByFunctionalityVariety.Add(pair.Key, functionalityMethodNamesSet);
                                });
                        });

                    // Create all descriptors, and add to output.
                    functionalityMethodNamesSetsByFunctionalityVariety.ForEach(
                        pair =>
                        {
                            var descriptors = pair.Value
                                .Select(functionalityMethodNames => Instances.Operations.GetDescriptor(
                                    tuple.ProjectFilePath,
                                    functionalityMethodNames,
                                    documentationByMemberIdentityName))
                                ;

                            // Get descriptor set (added initially).
                            var allDescriptors = functionalityDescriptorsByFunctionalityVariety[pair.Key];

                            allDescriptors.AddRange(descriptors);
                        });
                },
                out var problemProjects);

            // Output problem projects.
            var problemProjectsFilePath = Instances.FilePaths.ProblemProjectsFilePath;

            await Instances.Operations.WriteProblemProjectsFile(
                problemProjectsFilePath,
                problemProjects);

            // Output data.
            var outputFunctionsByFunctionalityVariety = new Dictionary<string, Func<ICollection<FunctionalityDescriptor>, Task>>
            {
                { Instances.InstanceVariety.Functionality, Instances.Operations.OutputFunctionalityFiles },
                { Instances.InstanceVariety.DraftFunctionality, Instances.Operations.OutputDraftFunctionalityFiles },
                { Instances.InstanceVariety.Values, Instances.Operations.OutputValuesFiles },
                { Instances.InstanceVariety.DraftValues, Instances.Operations.OutputDraftValuesFiles },

                { Instances.InstanceVariety.Experiments, Instances.Operations.OutputExperimentsFiles },
                { Instances.InstanceVariety.DraftExperiments, Instances.Operations.OutputDraftExperimentsFiles },
                { Instances.InstanceVariety.Explorations, Instances.Operations.OutputExplorationsFiles },
                { Instances.InstanceVariety.DraftExplorations, Instances.Operations.OutputDraftExplorationsFiles },
                { Instances.InstanceVariety.Demonstrations, Instances.Operations.OutputDemonstrationsFiles },
                { Instances.InstanceVariety.DraftDemonstrations, Instances.Operations.OutputDraftDemonstrationsFiles },
            };

            // Iterate over initial functionality methods so we can choose not to run some.
            await getInstanceMethodsByInstanceVariety.ForEach(
                async pair =>
                {
                    var functionalityDescriptors = functionalityDescriptorsByFunctionalityVariety[pair.Key];
                    var outputFunction = outputFunctionsByFunctionalityVariety[pair.Key];

                    await outputFunction(functionalityDescriptors);
                });

            // Output summary.
            var countsByFunctionalityVariety = functionalityDescriptorsByFunctionalityVariety
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Count);

            var problemProjectReasonCounts = problemProjects
                .GroupBy(x => x.Message)
                .Select(x => (Reason: x.Key, Count: x.Count()))
                .Now();

            var lines = countsByFunctionalityVariety
                    .OrderAlphabetically(x => x.Key)
                    .Select(pair => $"{pair.Key}: {pair.Value}")
                    .Append($"\n\nProblem projects ({problemProjects.Count}). Reasons:")
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
            // Show project list.
            var projectsListFilePath = Instances.Operations.GetProjectsListTextFilePath();
            await this.NotepadPlusPlusOperator.OpenFilePath(projectsListFilePath);

            // Show project files tuples.
            var projectFilesTuplesJsonFilePath = Instances.Operations.GetProjectFilesTuplesJsonFilePath();
            await this.NotepadPlusPlusOperator.OpenFilePath(projectFilesTuplesJsonFilePath);

            // Show problem projects.
            await this.NotepadPlusPlusOperator.OpenFilePath(problemProjectsFilePath);

            // Show output data.
            await Instances.FilePaths.AllInstanceVarietyTextFiles
                // Show summary last.
                .Append(summaryFilePath)
                .ForEach(async x => await this.NotepadPlusPlusOperator.OpenFilePath(x));
        }

        private async Task GetFunctionalityDescriptors()
        {
            /// Inputs.
            var textOutputFilePath = @"C:\Temp\Functionality.txt";
            var jsonOutputFilePath = @"C:\Temp\Functionality.json";
            var useCache = false;

            var problemProjectsFilePath = @"C:\Temp\Problem Projects.txt";
            var projectFilesTuplesJsonFilePath = @"C:\Temp\Project Files Tuples.json";

            /// Run.
            var cacheFilePath = jsonOutputFilePath;

            var canUseCache = useCache && Instances.FileSystemOperator.FileExists(cacheFilePath);

            FunctionalityDescriptor[] LoadCache()
            {
                var output = JsonFileHelper.LoadFromFile<FunctionalityDescriptor[]>(cacheFilePath);
                return output;
            }

            async Task<FunctionalityDescriptor[]> QueryForFunctionalityDescriptors()
            {
                var projectFilesTuples = JsonFileHelper.LoadFromFile<ProjectFilesTuple[]>(
                    projectFilesTuplesJsonFilePath)
                    //// For debugging
                    //.Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.S0041\source\R5T.S0041\R5T.S0041.csproj")
                    //.ToArray()
                    ;

                var problemProjects = new List<Failure<string>>();

                var functionalityDescriptors = new List<FunctionalityDescriptor>();

                var projectCount = projectFilesTuples.Length;

                var counter = 1;
                foreach (var tuple in projectFilesTuples)
                {
                    try
                    {
                        this.Logger.LogInformation($"Processing {counter} of {projectCount}:\n{tuple.ProjectFilePath}");

                        var hasFunctionality = Instances.Operations.ProcessProjectFilesTuple(
                            tuple,
                            tuple =>
                            {
                                var hasDraftFunctionality = Instances.Operations.AssemblyHasFunctionality(
                                    tuple.AssemblyFilePath);

                                return hasDraftFunctionality;
                            });

                        if (hasFunctionality)
                        {
                            var documentationByMethodIdentityName = Instances.Operations.GetDocumentationForMemberIdentityNames(
                                hasFunctionality.Result
                                    .Select(x => x.IdentityName),
                                tuple.DocumentationFilePath);

                            var descriptors = hasFunctionality.Result
                                .Select(x =>
                                {
                                    var descriptionXml = documentationByMethodIdentityName[x.IdentityName].Exists
                                        ? documentationByMethodIdentityName[x.IdentityName]
                                        : String.Empty
                                        ;

                                    var output = new FunctionalityDescriptor
                                    {
                                        MethodIdentityName = x.IdentityName,
                                        MethodParameterNamedIdentityName = x.ParameterNamedIdentityName,
                                        ProjectFilePath = tuple.ProjectFilePath,
                                        DescriptionXML = descriptionXml
                                    };

                                    return output;
                                });

                            functionalityDescriptors.AddRange(descriptors);
                        }
                    }
                    catch (Exception ex)
                    {
                        problemProjects.Add(
                            Failure.Of(
                                tuple.ProjectFilePath,
                                ex.Message));
                    }

                    counter++;
                }

                // Write cache.
                JsonFileHelper.WriteToFile(
                    cacheFilePath,
                    functionalityDescriptors);

                // Output problems.
                await Instances.Operations.WriteProblemProjectsFile(
                    problemProjectsFilePath,
                    problemProjects);
                
                // Show problems.
                var notepadPlusPlus = this.ServiceProvider.GetRequiredService<D0105.INotepadPlusPlusOperator>();

                await notepadPlusPlus.OpenFilePath(problemProjectsFilePath);

                return functionalityDescriptors.ToArray();
            }

            var functionalityDescriptors = canUseCache
                ? LoadCache()
                : await QueryForFunctionalityDescriptors()
                ;

            // Format output.
            await Instances.Operations.WriteFunctionalityDescriptors(
                "Functionalities",
                textOutputFilePath,
                functionalityDescriptors);

            JsonFileHelper.WriteToFile(
                jsonOutputFilePath,
                functionalityDescriptors);

            // Show output.
            var notepadPlusPlus = this.ServiceProvider.GetRequiredService<D0105.INotepadPlusPlusOperator>();

            await notepadPlusPlus.OpenFilePath(jsonOutputFilePath);
            // Open text output last so that it's the first thing user sees in Notepad++.
            await notepadPlusPlus.OpenFilePath(textOutputFilePath);
        }

        private async Task GetDraftFunctionalityDescriptors()
        {
            /// Inputs.
            var textOutputFilePath = @"C:\Temp\Functionality-Draft.txt";
            var jsonOutputFilePath = @"C:\Temp\Functionality-Draft.json";
            var requestUseCache = false;

            var problemProjectsFilePath = @"C:\Temp\Problem Projects.txt";
            var projectFilesTuplesJsonFilePath = @"C:\Temp\Project File Tuples.json";

            /// Run.
            var cacheFilePath = @"C:\Temp\Functionality-Draft-Cached.json";

            var useCache = requestUseCache && Instances.FileSystemOperator.FileExists(cacheFilePath);

            FunctionalityDescriptor[] LoadCache()
            {
                var output = JsonFileHelper.LoadFromFile<FunctionalityDescriptor[]>(cacheFilePath);
                return output;
            }

            async Task<FunctionalityDescriptor[]> QueryForDraftFunctionalityMethods()
            {
                var projectFilesTuples = JsonFileHelper.LoadFromFile<ProjectFilesTuple[]>(
                    projectFilesTuplesJsonFilePath)
                    // For debugging
                    .Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.S0041\source\R5T.S0041\R5T.S0041.csproj")
                    .ToArray()
                    ;

                var problemProjects = new List<Failure<string>>();

                var draftFunctionalityDescriptors = new List<FunctionalityDescriptor>();

                var projectCount = projectFilesTuples.Length;

                var counter = 1;
                foreach (var tuple in projectFilesTuples)
                {
                    try
                    {
                        this.Logger.LogInformation($"Processing {counter} of {projectCount}:\n{tuple.ProjectFilePath}");

                        var hasDraftFunctionality = Instances.Operations.ProcessProjectFilesTuple(
                            tuple,
                            tuple =>
                            {
                                var hasDraftFunctionality = Instances.Operations.AssemblyHasDraftFunctionality(
                                    tuple.AssemblyFilePath);

                                return hasDraftFunctionality;
                            });

                        if (hasDraftFunctionality)
                        {
                            var documentationByMethodIdentityName = Instances.Operations.GetDocumentationForMemberIdentityNames(
                                hasDraftFunctionality.Result
                                    .Select(x => x.IdentityName),
                                tuple.DocumentationFilePath);

                            var descriptors = hasDraftFunctionality.Result
                                .Select(x =>
                                {
                                    var descriptionXml = documentationByMethodIdentityName[x.IdentityName].Exists
                                        ? documentationByMethodIdentityName[x.IdentityName]
                                        : String.Empty
                                        ;

                                    var output = new FunctionalityDescriptor
                                    {
                                        MethodIdentityName = x.IdentityName,
                                        MethodParameterNamedIdentityName = x.ParameterNamedIdentityName,
                                        ProjectFilePath = tuple.ProjectFilePath,
                                        DescriptionXML = descriptionXml
                                    };

                                    return output;
                                });
                        
                            draftFunctionalityDescriptors.AddRange(descriptors);
                        }
                    }
                    catch (Exception ex)
                    {
                        problemProjects.Add(
                            Failure.Of(
                                tuple.ProjectFilePath,
                                ex.Message));
                    }

                    counter++;
                }

                // Write cache.
                JsonFileHelper.WriteToFile(
                    cacheFilePath,
                    draftFunctionalityDescriptors);

                // Output problems.
                await Instances.Operations.WriteProblemProjectsFile(
                    problemProjectsFilePath,
                    problemProjects);

                // Show problems.
                var notepadPlusPlus = this.ServiceProvider.GetRequiredService<D0105.INotepadPlusPlusOperator>();

                await notepadPlusPlus.OpenFilePath(problemProjectsFilePath);

                return draftFunctionalityDescriptors.ToArray();
            }

            var draftFunctionalityDescriptors = useCache
                ? LoadCache()
                : await QueryForDraftFunctionalityMethods()
                ;

            // Format output.
            await Instances.Operations.WriteFunctionalityDescriptors(
                "Functionalities-Draft",
                textOutputFilePath,
                draftFunctionalityDescriptors);

            JsonFileHelper.WriteToFile(
                jsonOutputFilePath,
                draftFunctionalityDescriptors);

            // Show output.
            var notepadPlusPlus = this.ServiceProvider.GetRequiredService<D0105.INotepadPlusPlusOperator>();

            await notepadPlusPlus.OpenFilePath(jsonOutputFilePath);
            // Open text output last so that it's the first thing user sees in Notepad++.
            await notepadPlusPlus.OpenFilePath(textOutputFilePath);
        }

        private async Task GetAllDraftFunctionalityMethods()
        {
            /// Inputs.
            var outputFilePath = @"C:\Temp\Functionality-Draft List.txt";
            var requestUseCache = false;

            var problemProjectsFilePath = @"C:\Temp\Problem Projects.txt";
            var projectFilesTuplesJsonFilePath = @"C:\Temp\Project File Tuples.json";

            /// Run.
            var cacheFilePath = @"C:\Temp\Functionality-Draft List-Cached.json";

            var useCache = requestUseCache && Instances.FileSystemOperator.FileExists(cacheFilePath);

            Dictionary<string, string[]> LoadCache()
            {
                var output = JsonFileHelper.LoadFromFile<Dictionary<string, string[]>>(cacheFilePath);
                return output;
            }

            async Task<Dictionary<string, string[]>> QueryForDraftFunctionalityMethods()
            {
                var projectFilesTuples = JsonFileHelper.LoadFromFile<ProjectFilesTuple[]>(
                    projectFilesTuplesJsonFilePath)
                    //// For debugging
                    //.Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.S0041\source\R5T.S0041\R5T.S0041.csproj")
                    //.ToArray()
                    ;

                var problemProjects = new List<Failure<string>>();

                var draftFunctionalityMethodNamesByProjectFilePath = new Dictionary<string, string[]>();

                var projectCount = projectFilesTuples.Length;

                var counter = 1;
                foreach (var tuple in projectFilesTuples)
                {
                    try
                    {
                        this.Logger.LogInformation($"Processing {counter} of {projectCount}:\n{tuple.ProjectFilePath}");

                        var hasDraftFunctionality = Instances.Operations.ProcessProjectFilesTuple(
                            tuple,
                            tuple =>
                            {
                                var hasDraftFunctionality = Instances.Operations.AssemblyHasDraftFunctionality_GetMethodIdentityNames(
                                    tuple.AssemblyFilePath);

                                return hasDraftFunctionality;
                            });

                        if (hasDraftFunctionality)
                        {
                            draftFunctionalityMethodNamesByProjectFilePath.Add(
                                tuple.ProjectFilePath,
                                hasDraftFunctionality.Result);
                        }
                    }
                    catch (Exception ex)
                    {
                        problemProjects.Add(
                            Failure.Of(
                                tuple.ProjectFilePath,
                                ex.Message));
                    }

                    counter++;
                }

                // Write cache.
                JsonFileHelper.WriteToFile(
                    cacheFilePath,
                    draftFunctionalityMethodNamesByProjectFilePath);

                // Output problems.
                await FileHelper.WriteAllLines(
                    problemProjectsFilePath,
                    problemProjects
                        .GroupBy(x => x.Message)
                        .OrderAlphabetically(x => x.Key)
                        .SelectMany(xGrouping => EnumerableHelper.From($"{xGrouping.Key}:")
                            .Append(xGrouping
                                .Select(x => $"{x.Value}: {x.Message}"))
                            .Append("***\n")));

                // Show problems.
                var notepadPlusPlus = this.ServiceProvider.GetRequiredService<D0105.INotepadPlusPlusOperator>();

                await notepadPlusPlus.OpenFilePath(problemProjectsFilePath);

                return draftFunctionalityMethodNamesByProjectFilePath;
            }

            var draftFunctionalityMethodNamesByProjectFilePath = useCache
                ? LoadCache()
                : await QueryForDraftFunctionalityMethods()
                ;

            // Format output.
            var lines = draftFunctionalityMethodNamesByProjectFilePath
                .OrderAlphabetically(x => x.Key)
                .SelectMany(xPair => EnumerableHelper.From($"{xPair.Key}:")
                    .Append(xPair.Value
                        .OrderAlphabetically()
                        .Select(x => $"\t{x}"))
                    .Append(String.Empty))
                .Now();

            await FileHelper.WriteAllLines(
                outputFilePath,
                lines);

            // Show output.
            var notepadPlusPlus = this.ServiceProvider.GetRequiredService<D0105.INotepadPlusPlusOperator>();

            await notepadPlusPlus.OpenFilePath(outputFilePath);
        }

        private async Task GetAllDraftFunctionalityInterfaces()
        {
            /// Inputs.
            var outputFilePath = @"C:\Temp\Functionality-Draft Interfaces List.txt";
            var requestUseCache = true;

            var problemProjectsFilePath = @"C:\Temp\Problem Projects.txt";
            var projectFilesTuplesJsonFilePath = @"C:\Temp\Project File Tuples.json";

            /// Run.
            var cacheFilePath = @"C:\Temp\Functionality-Draft Interfaces List-Cached.json";

            var useCache = requestUseCache && Instances.FileSystemOperator.FileExists(cacheFilePath);

            Dictionary<string, string[]> LoadCache()
            {
                var output = JsonFileHelper.LoadFromFile<Dictionary<string, string[]>>(cacheFilePath);
                return output;
            }

            async Task<Dictionary<string, string[]>> QueryForDraftFunctionalityTypes()
            {
                var projectFilesTuples = JsonFileHelper.LoadFromFile<ProjectFilesTuple[]>(
                    projectFilesTuplesJsonFilePath);

                var problemProjects = new List<Failure<string>>();

                var draftFunctionalityTypeNamesByProjectFilePath = new Dictionary<string, string[]>();

                var projectCount = projectFilesTuples.Length;

                var counter = 1;
                foreach (var tuple in projectFilesTuples)
                {
                    try
                    {
                        this.Logger.LogInformation($"Processing {counter} of {projectCount}:\n{tuple.ProjectFilePath}");

                        var projectFileExists = Instances.FileSystemOperator.FileExists(tuple.ProjectFilePath);
                        if (!projectFileExists)
                        {
                            throw new Exception("Project file did not exist.");
                        }

                        var assemblyFileExists = Instances.FileSystemOperator.FileExists(tuple.AssemblyFilePath);
                        if (!assemblyFileExists)
                        {
                            throw new Exception("Assembly file did not exist.");
                        }

                        var hasDraftFunctionalityTypes = Instances.Operations.AssemblyHasDraftFunctionalityTypes(
                            tuple.AssemblyFilePath);

                        if (hasDraftFunctionalityTypes)
                        {
                            draftFunctionalityTypeNamesByProjectFilePath.Add(
                                tuple.ProjectFilePath,
                                hasDraftFunctionalityTypes.Result);
                        }
                    }
                    catch (Exception ex)
                    {
                        problemProjects.Add(
                            Failure.Of(
                                tuple.ProjectFilePath,
                                ex.Message));
                    }

                    counter++;
                }

                // Write cache.
                JsonFileHelper.WriteToFile(
                    cacheFilePath,
                    draftFunctionalityTypeNamesByProjectFilePath);

                // Output problems.
                await FileHelper.WriteAllLines(
                    problemProjectsFilePath,
                    problemProjects
                        .GroupBy(x => x.Message)
                        .OrderAlphabetically(x => x.Key)
                        .SelectMany(xGrouping => EnumerableHelper.From($"{xGrouping.Key}:")
                            .Append(xGrouping
                                .Select(x => $"{x.Value}: {x.Message}"))
                            .Append("***\n")));

                // Show problems.
                var notepadPlusPlus = this.ServiceProvider.GetRequiredService<D0105.INotepadPlusPlusOperator>();

                await notepadPlusPlus.OpenFilePath(problemProjectsFilePath);

                return draftFunctionalityTypeNamesByProjectFilePath;
            }

            var draftFunctionalityTypeNamesByProjectFilePath = useCache
                ? LoadCache()
                : await QueryForDraftFunctionalityTypes()
                ;

            // Format output.
            var lines = draftFunctionalityTypeNamesByProjectFilePath
                .OrderAlphabetically(x => x.Key)
                .SelectMany(xPair => EnumerableHelper.From($"{xPair.Key}:")
                    .Append(xPair.Value
                        .OrderAlphabetically()
                        .Select(x => $"\t{x}"))
                    .Append(String.Empty))
                .Now();

            await FileHelper.WriteAllLines(
                outputFilePath,
                lines);

            // Show output.
            var notepadPlusPlus = this.ServiceProvider.GetRequiredService<D0105.INotepadPlusPlusOperator>();

            await notepadPlusPlus.OpenFilePath(outputFilePath);
        }

        private Task GetAllProjectFilesTuples()
        {
            /// Inputs.
            var requestUseCache = true;

            var outputJsonFilePath = @"C:\Temp\Project File Tuples.json";

            /// Run.
            var cacheFilePath = @"C:\Temp\Projects List-Cached-txt";

            string[] LoadCache()
            {
                var output = FileHelper.ActuallyReadAllLines(cacheFilePath);
                return output;
            }

            string[] QueryProjectFilesAndSaveToCache()
            {
                var repositoriesDirectoryPath = Instances.Operations.GetRepositoriesDirectoryPath();
                
                var output = Instances.Operations.GetAllProjectFilePaths(repositoriesDirectoryPath);

                FileHelper.WriteAllLinesSynchronous(
                    cacheFilePath,
                    output);

                return output;
            }

            var useCache = requestUseCache && FileHelper.Exists(cacheFilePath);

            var allProjectFilePaths = useCache
                ? LoadCache()
                : QueryProjectFilesAndSaveToCache()
                ;

            var projectFileTuples = allProjectFilePaths
                .Select(xProjectFilePath =>
                {
                    var projectAssemblyFilePath = Instances.Operations.GetAssemblyFilePathForProjectFilePath(xProjectFilePath);
                    var documentationFilePath = Instances.Operations.GetDocumentationFilePathForProjectFilePath(xProjectFilePath);

                    var output = new ProjectFilesTuple
                    {
                        AssemblyFilePath = projectAssemblyFilePath,
                        DocumentationFilePath = documentationFilePath,
                        ProjectFilePath = xProjectFilePath,
                    };

                    return output;
                })
                .Now();

            JsonFileHelper.WriteToFile(
                outputJsonFilePath,
                projectFileTuples);

            return Task.CompletedTask;
        }

        private Task FindFunctionalityTypesInAssembly()
        {
            /// Inputs.
            var assemblyFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.S0040\source\R5T.S0040\bin\Debug\net5.0\R5T.S0040.dll";

            /// Run.
            var assembly = Assembly.LoadFrom(assemblyFilePath);

            var draftFunctionalityMarkerAttributeNamespacedTypeName = "R5T.T0132.DraftFunctionalityMarkerAttribute";
            var functionalityMarkerAttributeNamespacedTypeName = "R5T.T0132.FunctionalityMarkerAttribute";

            var attributeTypeNamesOfInterest = new HashSet<string>(new[]
            {
                draftFunctionalityMarkerAttributeNamespacedTypeName,
                functionalityMarkerAttributeNamespacedTypeName,
            });

            var draftFunctionalityMarkerInterfaceNamespacedTypeName = "R5T.T0132.IDraftFunctionalityMarker";
            var functionalityMarkerInterfaceNamespacedTypeName = "R5T.T0132.IFunctionalityMarker";

            var interfaceTypeNamesOfInterest = new HashSet<string>(new[]
            {
                draftFunctionalityMarkerInterfaceNamespacedTypeName,
                functionalityMarkerInterfaceNamespacedTypeName,
            });

            var typesOfInterest = assembly.DefinedTypes
                .Where(xType =>
                {
                    var hasMarkerAttribute = xType.CustomAttributes
                        .Where(xAttribute => attributeTypeNamesOfInterest.Contains(xAttribute.AttributeType.FullName))
                        .Any();

                    var hasMarkerInterface = xType.ImplementedInterfaces
                        .Where(xInterface => interfaceTypeNamesOfInterest.Contains(xInterface.FullName))
                        .Any();

                    var output = hasMarkerAttribute || hasMarkerInterface;
                    return output;
                })
                .Now();

            Console.WriteLine("Types of interest:\n");

            foreach (var typeOfInterest in typesOfInterest)
            {
                Console.WriteLine(typeOfInterest.FullName);
            }

            return Task.CompletedTask;
        }

        private Task ListTypesAndTheirImplementedInterfacesInAssembly()
        {
            /// Inputs.
            var assemblyFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.S0040\source\R5T.S0040\bin\Debug\net5.0\R5T.S0040.dll";

            /// Run.
            var assembly = Assembly.LoadFrom(assemblyFilePath);

            foreach (var type in assembly.DefinedTypes)
            {
                Console.WriteLine(type.FullName);

                foreach (var interfaceType in type.ImplementedInterfaces)
                {
                    Console.WriteLine($"\t{interfaceType.FullName}");
                }

                Console.WriteLine();
            }

            return Task.CompletedTask;
        }

        private Task ListTypesAndAttributesInAssembly()
        {
            // Output:
            //R5T.S0040.Operations
            //    R5T.T0132.DraftFunctionalityMarkerAttribute

            //R5T.S0040.XPathOperations
            //        R5T.T0132.DraftFunctionalityMarkerAttribute

            /// Inputs.
            var assemblyFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.S0040\source\R5T.S0040\bin\Debug\net5.0\R5T.S0040.dll";

            /// Run.
            //// Reflection-only loading is not supported on Windows 10...
            //var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFilePath);

            var assembly = Assembly.LoadFrom(assemblyFilePath);

            foreach (var type in assembly.DefinedTypes)
            // Same types.s
            //foreach (var type in assembly.GetTypes())
            {
                Console.WriteLine(type.FullName);

                foreach (var attribute in type.CustomAttributes)
                {
                    var attributeType = attribute.AttributeType;

                    Console.WriteLine($"\t{attributeType.FullName}");
                }

                Console.WriteLine();
            }

            return Task.CompletedTask;
        }
    }
}