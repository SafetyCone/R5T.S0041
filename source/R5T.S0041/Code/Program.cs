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

using R5T.D0088;
using R5T.D0090;
using R5T.D0105;
using R5T.F0000;

namespace R5T.S0041
{
    partial class Program : ProgramAsAServiceBase
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

        private async Task RunMethod()
        {
            // To add marker attributes, see project plan file.

            /// Main
            //await this.Run();

            ///// Or run these in order:
            //await this.GetAllDescriptors();
            //await this.CopyOutputFilesToCloudDirectory();
            //await this.CreateDatedFiles();
            //await this.CompareFilesForTwoDates();
            //await this.CreateSummaryFile();
            //await this.CreateSummaryPresentationFile();

            /// Useful.
            //await this.OpenOutputDirectory();
            //await this.OpenCloudOutputDirectory();
            //await this.OpenAllCloudOutputFiles();
            //await this.OpenNugetAssembliesDirectory();

            /// Construction
            //Construction.Instance.OutputMethodName_WithParameterNames();
            //Construction.Instance.ProcessAssembly();
            //Construction.Instance.FindLatestModifiedFile();
            //Construction.Instance.BuildAllProjectFilePaths_AndProcessAssemblies();

            // Construction operation series for run.
            //Construction.Instance.GetAllProjectFilePaths_OutputToDatedDirectory();
            //Construction.Instance.BuildProjectFilePaths();
            //Construction.Instance.CreateProjectFileTuples();
            //Construction.Instance.ProcessBuiltProjects();
            //Construction.Instance.SummarizeProcessing();
            //Construction.Instance.CompareDates();
            //Construction.Instance.SummarizeDatesComparison();
            //Construction.Instance.SummarizeNewAndOldInstances();
            //Construction.Instance.SendResultsEmail();

            //Construction.Instance.OutputInstanceSpecificFiles();
            Construction.Instance.CopyFilesToCloudSharedDirectory();
            //Construction.Instance.OpenOutputFiles();

            //await Task.Run(() => Instances.Operations.QueryProjectFiles_AndWriteToFile());
            //await Task.Run(() => Instances.Operations.QueryProjectFilesTuples_AndWriteToFile());

            //await this.GetAllTypesWithMarkerAttributeMarkerAttribute();

            //await this.ListTypesAndAttributesInAssembly();
            //await this.ListTypesAndTheirImplementedInterfacesInAssembly();
            //await this.FindFunctionalityTypesInAssembly();
            //await this.GetAllProjectFilesTuples();
            //await this.GetAllDraftFunctionalityInterfaces();
            //await this.GetAllDraftFunctionalityMethods();
            //await this.GetDraftFunctionalityDescriptors();
            //await this.GetFunctionalityDescriptors();

        }

        private async Task Run()
        {
            /// Inputs.
            var date = Instances.DateOperator.GetToday();
            //var date = Instances.DateOperator.GetDefault();
            //var date = Instances.DateOperator.From_YYYYMMDD("20220819");

            //var olderDate = Instances.DateOperator.GetYesterday();
            //var olderDate = Instances.DateOperator.From_YYYYMMDD("20221010");
            var olderDate = Instances.Operations.GetPriorComparisonDate(date);


            /// Run.
            await this.GetAllDescriptors();
            await this.CopyOutputFilesToCloudDirectory();
            await this.CreateDatedFiles(date);
            await this.CompareFilesForTwoDates(olderDate, date);
            await this.CreateSummaryFile(olderDate, date);
            await this.CreateSummaryPresentationFile(olderDate, date);
        }

        private async Task CopyOutputFilesToCloudDirectory()
        {
            /// Inputs.
            var openFiles = true;

            /// Run.
            var allInstanceVarietyTextOutputFilePaths = Instances.FilePathOperator.GetAllTextOutputFilePaths();

            var cloudOutputDestinationDirectoryFileCopyPairs = Instances.PathOperator.GetDestinationFileCopyPairs(
                allInstanceVarietyTextOutputFilePaths,
                Instances.DirectoryPaths.CloudOutputDirectoryPath);

            Instances.FileSystemOperator.CopyFiles(cloudOutputDestinationDirectoryFileCopyPairs);

            // Open files if desired.
            if (openFiles)
            {
                // Use presentation-ordered instance varietys.
                var cloudOutputTextFilePaths = Instances.FilePathOperator.GetAllCloudTextOutputFilePaths_InPresentationOrder();

                await cloudOutputTextFilePaths
                    .ForEach(async textFilePath => await this.NotepadPlusPlusOperator.OpenFilePath(textFilePath));
            }
        }

        private Task OpenCloudOutputDirectory()
        {
            F0034.WindowsExplorerOperator.Instance.OpenDirectoryInExplorer(
                Instances.DirectoryPaths.CloudOutputDirectoryPath);

            return Task.CompletedTask;
        }

        private Task OpenOutputDirectory()
        {
            F0034.WindowsExplorerOperator.Instance.OpenDirectoryInExplorer(
                Instances.DirectoryPaths.OutputDirectoryPath);

            return Task.CompletedTask;
        }

        private async Task OpenAllCloudOutputFiles()
        {
            var cloudOutputTextFilePaths = Instances.FilePathOperator.GetAllCloudTextOutputFilePaths_InPresentationOrder();

            await cloudOutputTextFilePaths
                .ForEach(async textOutputFilePath => await this.NotepadPlusPlusOperator.OpenFilePath(textOutputFilePath));
        }

        private Task OpenNugetAssembliesDirectory()
        {
            var nugetAssembliesDirectoryPath = DirectoryPaths.Instance.NuGetAssemblies;

            F0034.WindowsExplorerOperator.Instance.OpenDirectoryInExplorer(nugetAssembliesDirectoryPath);

            return Task.CompletedTask;
        }

        private Task CreateSummaryPresentationFile()
        {
            /// Inputs.
            //var olderDate = Instances.DateOperator.GetYesterday();
            //var olderDate = Instances.DateOperator.GetYesterday();
            var olderDate = Instances.DateOperator.From_YYYYMMDD("20220920");
            var newerDate = Instances.DateOperator.GetToday();
            //var newerDate = Instances.DateOperator.GetDefault();
            //var newerDate = Instances.DateOperator.From_YYYYMMDD("20220819");

            return this.CreateSummaryPresentationFile(
                olderDate,
                newerDate);
        }

        private async Task CreateSummaryPresentationFile(
            DateTime olderDate,
            DateTime newerDate)
        {
            /// Inputs.
            var processDirectoryPath = Instances.DirectoryPaths.OutputDirectoryPath;
            var outputFilePath = Instances.FilePaths.SummaryPresentationFilePath;


            /// Run.
            // Get summary data file path.
            var summaryOutputDirectoryPath = Instances.PathOperator.GetDirectoryPath(
                processDirectoryPath,
                "Summaries");

            var olderDateString = Instances.DateOperator.ToString_YYYYMMDD(olderDate);
            var newerDateString = Instances.DateOperator.ToString_YYYYMMDD(newerDate);

            var datedSummaryOutputJsonFileName = $"Summary-{olderDateString} to {newerDateString}.json";

            var datedSummaryOutputJsonFilePath = Instances.PathOperator.GetFilePath(
                summaryOutputDirectoryPath,
                datedSummaryOutputJsonFileName);

            var datedInstancesSummary = JsonFileHelper.LoadFromFile<DatedInstancesSummary>(datedSummaryOutputJsonFilePath);

            var lines =
                new[]
                {
                    "Instances Summary",
                    $"\n{newerDateString}: as-of date",
                    $"{olderDateString}: prior comparison date",
                    "",
                }
                .Append(
                    Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder()
                    .SelectMany(x =>
                    {
                        var counts = datedInstancesSummary.InstanceVarietyCountsByVarietyName[x];

                        var output = new[]
                        {
                            $"{counts.Count,5}: {x}, (+{counts.New}, -{counts.Departed})"
                        };

                        return output;
                    }));

            await FileHelper.WriteAllLines(
                outputFilePath,
                lines);

            await this.NotepadPlusPlusOperator.OpenFilePath(outputFilePath);
        }

        private Task CreateSummaryFile()
        {
            /// Inputs.
            //var olderDate = Instances.DateOperator.GetYesterday();
            //var olderDate = Instances.DateOperator.GetYesterday();
            var olderDate = Instances.DateOperator.From_YYYYMMDD("20220920");
            var newerDate = Instances.DateOperator.GetToday();
            //var newerDate = Instances.DateOperator.GetDefault();
            //var newerDate = Instances.DateOperator.From_YYYYMMDD("20220819");

            return this.CreateSummaryFile(
                olderDate,
                newerDate);
        }

        private async Task CreateSummaryFile(
            DateTime olderDate,
            DateTime newerDate)
        {
            /// Inputs.
            var outputDirectoryPath = Instances.DirectoryPaths.OutputDirectoryPath;


            /// Run.
            var allInstanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames();

            // Get data file paths.
            var newerDatedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedChildDirectoryPath(
                outputDirectoryPath,
                newerDate);

            var newerDatedFilePathsByInstanceName = Instances.FilePathOperator.GetDatedJsonOutputFilePaths_ForInstanceVarieties(
                allInstanceVarietyNames,
                newerDatedOutputDirectoryPath,
                newerDate)
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.DatedJsonOutputFilePath);

            var comparisonDatedNewPathsByInstanceName = allInstanceVarietyNames
                .Select(instanceVarietyName =>
                {
                    var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

                    var newFileName = Instances.FileNameOperator.GetNewFileName_FromFileName(fileName);

                    var comparisonDatedNewFileName = Instances.FileNameOperator.GetDatedComparisonFileName(
                        newFileName,
                        olderDate,
                        newerDate);

                    var comparisonDatedNewFilePath = Instances.PathOperator.GetFilePath(
                        newerDatedOutputDirectoryPath,
                        comparisonDatedNewFileName);

                    return (InstanceVarietyName: instanceVarietyName, ComparisonDatedNewFilePath: comparisonDatedNewFilePath);
                })
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.ComparisonDatedNewFilePath);

            var comparisonDatedDepartedPathsByInstanceName = allInstanceVarietyNames
                .Select(instanceVarietyName =>
                {
                    var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

                    var departedFileName = Instances.FileNameOperator.GetDepartedFileName_FromFileName(fileName);

                    var comparisonDatedDepartedFileName = Instances.FileNameOperator.GetDatedComparisonFileName(
                        departedFileName,
                        olderDate,
                        newerDate);

                    var comparisonDatedDepartedFilePath = Instances.PathOperator.GetFilePath(
                        newerDatedOutputDirectoryPath,
                        comparisonDatedDepartedFileName);

                    return (InstanceVarietyName: instanceVarietyName, ComparisonDatedDepartedFilePath: comparisonDatedDepartedFilePath);
                })
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.ComparisonDatedDepartedFilePath);

            // Create the summary, and pre-fill with a count for each variety to allow zero (but still present) counts for varietys that don't have any instances.
            var summary = new DatedInstancesSummary
            {
                AsOfDate = newerDate,
                PriorComparisonDate = olderDate,
                InstanceVarietyCountsByVarietyName = allInstanceVarietyNames
                    .ToDictionary(
                        x => x,
                        x => new CountChange()),
            };

            // Now compute counts, and modify the summary.
            foreach (var instanceVarietyName in allInstanceVarietyNames)
            {
                var countFilePath = newerDatedFilePathsByInstanceName[instanceVarietyName];
                var newCountFilePath = comparisonDatedNewPathsByInstanceName[instanceVarietyName];
                var departedCountFilePath = comparisonDatedDepartedPathsByInstanceName[instanceVarietyName];

                var countInstances = Instances.Operations.LoadFunctionalityDescriptors(countFilePath);
                var newCountInstances = Instances.Operations.LoadFunctionalityDescriptors(newCountFilePath);
                var departedCountInstances = Instances.Operations.LoadFunctionalityDescriptors(departedCountFilePath);

                var countChanges = summary.InstanceVarietyCountsByVarietyName[instanceVarietyName];

                countChanges.Count = countInstances.Length;
                countChanges.New = newCountInstances.Length;
                countChanges.Departed = departedCountInstances.Length;
            }

            var summaryOutputDirectoryPath = Instances.PathOperator.GetDirectoryPath(
                outputDirectoryPath,
                "Summaries");

            var olderDateString = Instances.DateOperator.ToString_YYYYMMDD(olderDate);
            var newerDateString = Instances.DateOperator.ToString_YYYYMMDD(newerDate);

            var datedSummaryOutputJsonFileName = $"Summary-{olderDateString} to {newerDateString}.json";

            var datedSummaryOutputJsonFilePath = Instances.PathOperator.GetFilePath(
                summaryOutputDirectoryPath,
                datedSummaryOutputJsonFileName);

            // Write summary.
            Instances.FileSystemOperator.CreateDirectory_OkIfAlreadyExists(summaryOutputDirectoryPath);

            JsonFileHelper.WriteToFile(
                datedSummaryOutputJsonFilePath,
                summary);

            await this.NotepadPlusPlusOperator.OpenFilePath(datedSummaryOutputJsonFilePath);
        }

        private Task CompareFilesForTwoDates()
        {
            //var olderDate = Instances.DateOperator.GetYesterday();
            //var olderDate = Instances.DateOperator.GetYesterday();
            var olderDate = Instances.DateOperator.From_YYYYMMDD("20220920");
            var newerDate = Instances.DateOperator.GetToday();
            //var newerDate = Instances.DateOperator.GetDefault();
            //var newerDate = Instances.DateOperator.From_YYYYMMDD("20220819");

            return this.CompareFilesForTwoDates(
                olderDate,
                newerDate);
        }

        private Task CompareFilesForTwoDates(
            DateTime olderDate,
            DateTime newerDate)
        {
            /// Inputs.
            var outputDirectoryPath = Instances.DirectoryPaths.OutputDirectoryPath;

            /// Run.
            // First create all file paths.
            var allInstanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames();

            var olderDatedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedChildDirectoryPath(
                outputDirectoryPath,
                olderDate);

            var olderDatedFilePathsByInstanceName = Instances.FilePathOperator.GetDatedJsonOutputFilePaths_ForInstanceVarieties(
                allInstanceVarietyNames,
                olderDatedOutputDirectoryPath,
                olderDate)
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.DatedJsonOutputFilePath);

            var newerDatedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedChildDirectoryPath(
                outputDirectoryPath,
                newerDate);

            var newerDatedFilePathsByInstanceName = Instances.FilePathOperator.GetDatedJsonOutputFilePaths_ForInstanceVarieties(
                allInstanceVarietyNames,
                newerDatedOutputDirectoryPath,
                newerDate)
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.DatedJsonOutputFilePath);

            var comparisonDatedNewPathsByInstanceName = allInstanceVarietyNames
                .Select(instanceVarietyName =>
                {
                    var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

                    var newFileName = Instances.FileNameOperator.GetNewFileName_FromFileName(fileName);

                    var comparisonDatedNewFileName = Instances.FileNameOperator.GetDatedComparisonFileName(
                        newFileName,
                        olderDate,
                        newerDate);

                    var comparisonDatedNewFilePath = Instances.PathOperator.GetFilePath(
                        newerDatedOutputDirectoryPath,
                        comparisonDatedNewFileName);

                    return (InstanceVarietyName: instanceVarietyName, ComparisonDatedNewFilePath: comparisonDatedNewFilePath);
                })
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.ComparisonDatedNewFilePath);

            var comparisonDatedDepartedPathsByInstanceName = allInstanceVarietyNames
                .Select(instanceVarietyName =>
                {
                    var fileName = Instances.FileNameOperator.GetJsonOutputFileName_ForInstanceVariety(instanceVarietyName);

                    var departedFileName = Instances.FileNameOperator.GetDepartedFileName_FromFileName(fileName);

                    var comparisonDatedDepartedFileName = Instances.FileNameOperator.GetDatedComparisonFileName(
                        departedFileName,
                        olderDate,
                        newerDate);

                    var comparisonDatedDepartedFilePath = Instances.PathOperator.GetFilePath(
                        newerDatedOutputDirectoryPath,
                        comparisonDatedDepartedFileName);

                    return (InstanceVarietyName: instanceVarietyName, ComparisonDatedDepartedFilePath: comparisonDatedDepartedFilePath);
                })
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.ComparisonDatedDepartedFilePath);

            // Foreach variety, load, compare, then write out results.
            foreach (var instanceVarietyName in allInstanceVarietyNames)
            {
                var olderFilePath = olderDatedFilePathsByInstanceName[instanceVarietyName];
                var newerFilePath = newerDatedFilePathsByInstanceName[instanceVarietyName];

                var olderInstances = Instances.Operations.LoadFunctionalityDescriptors(
                    olderFilePath);

                var newerInstance = Instances.Operations.LoadFunctionalityDescriptors(
                    newerFilePath);

                var newInstances = newerInstance.Except(olderInstances,
                    FunctionalityDescriptorDataIdentityEqualityComparer.Instance)
                    .Now();

                var departedInstances = olderInstances.Except(newerInstance,
                    FunctionalityDescriptorDataIdentityEqualityComparer.Instance)
                    .Now();

                // Write out new.
                var comparisonDatedNewFilePath = comparisonDatedNewPathsByInstanceName[instanceVarietyName];

                JsonFileHelper.WriteToFile(
                    comparisonDatedNewFilePath,
                    newInstances);

                // Write out departed.
                var comparisonDatedDepartedFileName = comparisonDatedDepartedPathsByInstanceName[instanceVarietyName];

                JsonFileHelper.WriteToFile(
                    comparisonDatedDepartedFileName,
                    departedInstances);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a dated directory, copies files to the dated directory as dated files, for functionality.
        /// </summary>
        private Task CreateDatedFiles()
        {
            /// Inputs.
            var date = Instances.DateOperator.GetToday();
            //var date = Instances.DateOperator.GetYesterday();
            //var date = Instances.DateOperator.From_YYYYMMDD("20220727");

            return this.CreateDatedFiles(date);
        }
            
        private Task CreateDatedFiles(DateTime date)
        {
            /// Inputs.
            var outputDirectoryPath = Instances.DirectoryPaths.OutputDirectoryPath;


            /// Run.
            // Get source and destination paths for all varieties.
            var allVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames();

            var initialOutputDirectoryPath = Instances.DirectoryPaths.InitialOutputDirectorPath;

            var sourceFilePathsByVarietyName = Instances.FilePathOperator.GetJsonOutputFilePaths_ForInstanceVarieties(
                allVarietyNames,
                Instances.DirectoryPaths.InitialOutputDirectorPath)
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.jsonOutputFilePath);

            var datedDestinationDirectoryPath = Instances.DirectoryPathOperator.GetDatedChildDirectoryPath(
                outputDirectoryPath,
                date);

            var destinationFilePathsByVarietyName = Instances.FilePathOperator.GetDatedJsonOutputFilePaths_ForInstanceVarieties(
                allVarietyNames,
                datedDestinationDirectoryPath,
                date)
                .ToDictionary(
                    x => x.InstanceVarietyName,
                    x => x.DatedJsonOutputFilePath);

            // Copy.
            Instances.FileSystemOperator.CreateDirectory(datedDestinationDirectoryPath);

            var fileCopyPairs = sourceFilePathsByVarietyName
                .Join(destinationFilePathsByVarietyName,
                    outer => outer.Key,
                    inner => inner.Key,
                    (outer, inner) => (SourceFilePath: outer.Value, DestinationFilePath: inner.Value))
                .Now();

            foreach (var (sourceFilePath, destinationFilePath) in fileCopyPairs)
            {
                Instances.FileSystemOperator.CopyFile(
                    sourceFilePath: sourceFilePath,
                    destinationFilePath: destinationFilePath);
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
                //.Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.T0151\source\R5T.T0151\R5T.T0151.csproj")
                //.Now()
                ////
                ;

            // Assembly actions.
            // Build the closures that will perform Assembly => InstancesIdentityNames for each type of code element (method or property), for each variety of instance (functionality, explorations, etc.).
            var intanceIdentityNamesProvidersByInstanceVariety = Operations.Instance.GetInstanceIdentityNamesProvidersByInstanceVariety();

            var functionalityDescriptorsByFunctionalityVariety = new Dictionary<string, List<InstanceDescriptor>>();

            // Pre-add all lists in case there are no successfully processed projects. This way, there will be zero elements for each variety.
            foreach (var pair in intanceIdentityNamesProvidersByInstanceVariety)
            {
                functionalityDescriptorsByFunctionalityVariety.Add(pair.Key, new List<InstanceDescriptor>());
            }

            Instances.Operations.ProcessProjectFilesTuples(
                projectFilesTuples,
                this.Logger,
                tuple => Instances.Operations.ProcessProjectFilesTuple(
                    tuple,
                    intanceIdentityNamesProvidersByInstanceVariety,
                    functionalityDescriptorsByFunctionalityVariety),
                out var problemProjects);

            // Output problem projects.
            var problemProjectsFilePath = Instances.FilePaths.ProblemProjectsFilePath;

            await Instances.Operations.WriteProblemProjectsFile(
                problemProjectsFilePath,
                problemProjects);

            // Output data.
            // Iterate over initial functionality methods so we can choose not to run some.
            await intanceIdentityNamesProvidersByInstanceVariety.ForEach(
                async pair =>
                {
                    var functionalityDescriptors = functionalityDescriptorsByFunctionalityVariety[pair.Key];

                    var varietyName = pair.Key;

                    var jsonOutputFilePath = Instances.FilePathOperator.GetJsonOutputFilePath_ForInstanceVariety(varietyName);
                    var textOutputFilePath = Instances.FilePathOperator.GetTextOutputFilePath_ForInstanceVariety(varietyName);

                    await Instances.Operations.OutputFunctionalityFiles(
                        functionalityDescriptors,
                        varietyName,
                        jsonOutputFilePath,
                        textOutputFilePath);
                });

            // Output one single file.
            var instances = intanceIdentityNamesProvidersByInstanceVariety
                .SelectMany(pair =>
                {
                    var instanceVariety = pair.Key;

                    var functionalityDescriptors = functionalityDescriptorsByFunctionalityVariety[instanceVariety];

                    var instances = functionalityDescriptors
                        .Select(functionalityDescriptor =>
                        {
                            var instanceDescriptor = new N002.InstanceDescriptor
                            {
                                InstanceVariety = instanceVariety,
                                ProjectFilePath = functionalityDescriptor.ProjectFilePath,
                                IdentityName = functionalityDescriptor.IdentityName,
                                ParameterNamedIdentityName = functionalityDescriptor.ParameterNamedIdentityName,
                                DescriptionXml = functionalityDescriptor.DescriptionXml,
                            };

                            return instanceDescriptor;
                        });

                    return instances;
                })
                .Now();

            F0032.JsonOperator.Instance.Serialize_Synchronous(
                FilePaths.Instance.InstancesJsonFilePath,
                instances);

            // Output summary.
            var summaryFilePath = Instances.FilePaths.SummaryFilePath;

            await Instances.Operations.OutputSummaryFile(
                functionalityDescriptorsByFunctionalityVariety,
                problemProjects,
                summaryFilePath);

            // Show outputs.
            // Show project list.
            var projectsListFilePath = Instances.Operations.GetProjectsListTextFilePath();
            await this.NotepadPlusPlusOperator.OpenFilePath(projectsListFilePath);

            // Show project files tuples.
            var projectFilesTuplesJsonFilePath = Instances.Operations.GetProjectFilesTuplesJsonFilePath();
            await this.NotepadPlusPlusOperator.OpenFilePath(projectFilesTuplesJsonFilePath);

            // Show problem projects.
            await this.NotepadPlusPlusOperator.OpenFilePath(problemProjectsFilePath);

            // Show summary file.
            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);
        }

        /// <summary>
        /// Search all project files tuples (project compiled assemblies) for types marked with the MarkerAttributeMarkerAttribute (the marker attribute that indicates an attribute is a marker attribute).
        /// </summary>
        private async Task GetAllTypesWithMarkerAttributeMarkerAttribute()
        {
            /// Inputs.
            var useProjectsCache = false;


            /// Run.
            // Setup per-variety operations.
            var instanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName = new Dictionary<string, string>
            {
                { Instances.InstanceVariety.MarkerAttribute, F0000.Instances.TypeOperator.GetNamespacedTypeName<T0143.MarkerAttributeMarkerAttribute>() },
            };

            var getInstanceIdentityNamesByInstanceVariety = instanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName
                .Select(xPair => (xPair.Key, Instances.Operations.GetInstanceTypeNamesProviderFunction(xPair.Value)))
                .ToDictionary(
                    x => x.Key,
                    x => x.Item2);

            // Query projects.
            var projectFilesTuples = Instances.Operations.GetProjectFilesTuples(useProjectsCache)
                // For debugging.
                .Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.T0131\source\R5T.T0131\R5T.T0131.csproj")
                .ToArray()
                ;

            var functionalityDescriptorsByFunctionalityVariety = new Dictionary<string, List<InstanceDescriptor>>();

            // Pre-add all lists in case there are no successfully processed projects. This way, there will be zero elements for each variety.
            foreach (var pair in getInstanceIdentityNamesByInstanceVariety)
            {
                functionalityDescriptorsByFunctionalityVariety.Add(pair.Key, new List<InstanceDescriptor>());
            }

            Instances.Operations.ProcessProjectFilesTuples(
                projectFilesTuples,
                this.Logger,
                tuple => Instances.Operations.ProcessProjectFilesTuple(
                    tuple,
                    getInstanceIdentityNamesByInstanceVariety,
                    functionalityDescriptorsByFunctionalityVariety),
                out var problemProjects);

            // Output problem projects.
            var problemProjectsFilePath = Instances.FilePaths.ProblemProjectsFilePath;

            await Instances.Operations.WriteProblemProjectsFile(
                problemProjectsFilePath,
                problemProjects);

            // Output data.
            // Iterate over the available instance-variety methods so we can choose not to run some.
            var textOutputFilePaths = new List<string>();

            await getInstanceIdentityNamesByInstanceVariety.ForEach(
                async pair =>
                {
                    var functionalityDescriptors = functionalityDescriptorsByFunctionalityVariety[pair.Key];

                    var varietyName = pair.Key;

                    var jsonOutputFilePath = @"C:\Temp\" + varietyName + ".json";
                    var textOutputFilePath = @"C:\Temp\" + varietyName + ".txt";

                    textOutputFilePaths.Add(textOutputFilePath);

                    await Instances.Operations.OutputFunctionalityFiles(
                        functionalityDescriptors,
                        varietyName,
                        jsonOutputFilePath,
                        textOutputFilePath);
                });

            // Output summary.
            var summaryFilePath = Instances.FilePaths.SummaryFilePath;

            await Instances.Operations.OutputSummaryFile(
                functionalityDescriptorsByFunctionalityVariety,
                problemProjects,
                summaryFilePath);

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
            await textOutputFilePaths
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

            InstanceDescriptor[] LoadCache()
            {
                var output = JsonFileHelper.LoadFromFile<InstanceDescriptor[]>(cacheFilePath);
                return output;
            }

            async Task<InstanceDescriptor[]> QueryForFunctionalityDescriptors()
            {
                var projectFilesTuples = JsonFileHelper.LoadFromFile<ProjectFilesTuple[]>(
                    projectFilesTuplesJsonFilePath)
                    //// For debugging
                    //.Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.S0041\source\R5T.S0041\R5T.S0041.csproj")
                    //.ToArray()
                    ;

                var problemProjects = new List<Failure<string>>();

                var functionalityDescriptors = new List<InstanceDescriptor>();

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
                                        : Instances.Strings.Empty
                                        ;

                                    var output = new InstanceDescriptor
                                    {
                                        IdentityName = x.IdentityName,
                                        ParameterNamedIdentityName = x.ParameterNamedIdentityName,
                                        ProjectFilePath = tuple.ProjectFilePath,
                                        DescriptionXml = descriptionXml
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

            InstanceDescriptor[] LoadCache()
            {
                var output = JsonFileHelper.LoadFromFile<InstanceDescriptor[]>(cacheFilePath);
                return output;
            }

            async Task<InstanceDescriptor[]> QueryForDraftFunctionalityMethods()
            {
                var projectFilesTuples = JsonFileHelper.LoadFromFile<ProjectFilesTuple[]>(
                    projectFilesTuplesJsonFilePath)
                    // For debugging
                    .Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.S0041\source\R5T.S0041\R5T.S0041.csproj")
                    .ToArray()
                    ;

                var problemProjects = new List<Failure<string>>();

                var draftFunctionalityDescriptors = new List<InstanceDescriptor>();

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
                                        : Instances.Strings.Empty
                                        ;

                                    var output = new InstanceDescriptor
                                    {
                                        IdentityName = x.IdentityName,
                                        ParameterNamedIdentityName = x.ParameterNamedIdentityName,
                                        ProjectFilePath = tuple.ProjectFilePath,
                                        DescriptionXml = descriptionXml
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
                    .Append(Instances.Strings.Empty))
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
                            //throw new FileNotFoundException("Assembly file did not exist.", tuple.AssemblyFilePath);
                            throw new FileNotFoundException("Assembly file did not exist.");
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
                var notepadPlusPlus = this.ServiceProvider.GetRequiredService<INotepadPlusPlusOperator>();

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
                    .Append(Instances.Strings.Empty))
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

                FileHelper.WriteAllLines_Synchronous(
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
                    var projectAssemblyFilePath = F0040.ProjectPathsOperator.Instance.GetAssemblyFilePathForProjectFilePath(xProjectFilePath);
                    var documentationFilePath = F0040.ProjectPathsOperator.Instance.GetDocumentationFilePathForProjectFilePath(xProjectFilePath);

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