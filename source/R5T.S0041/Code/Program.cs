using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5T.D0088;
using R5T.D0090;
using R5T.D0105;
using R5T.Magyar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


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

        private Task RunMethod()
        {
            //return Task.Run(() => Instances.Operations.QueryProjectFiles_AndWriteToFile());
            //return Task.Run(() => Instances.Operations.QueryProjectFilesTuples_AndWriteToFile());

            //return this.GetAllTypesWithMarkerAttributeMarkerAttribute();

            //return this.ListTypesAndAttributesInAssembly();
            //return this.ListTypesAndTheirImplementedInterfacesInAssembly();
            //return this.FindFunctionalityTypesInAssembly();
            //return this.GetAllProjectFilesTuples();
            //return this.GetAllDraftFunctionalityInterfaces();
            //return this.GetAllDraftFunctionalityMethods();
            //return this.GetDraftFunctionalityDescriptors();
            //return this.GetFunctionalityDescriptors();

            /// Run these in order:
            // To add marker attributes, see project plan file.
            //return this.GetAllDescriptors();
            //return this.CreateDatedFiles();
            //return this.CompareFilesForTwoDates();
            //return this.CreateSummaryFile();
            return this.CreateSummaryPresentationFile();
        }

        private async Task CreateSummaryPresentationFile()
        {
            /// Inputs.
            var processDirectoryPath = @"C:\Temp\Output\S0041\";
            var outputFilePath = @"C:\Temp\Instances Summary.txt";

            //var olderDate = Instances.DateOperator.GetYesterday();
            //var olderDate = Instances.DateOperator.GetYesterday();
            var olderDate = Instances.DateOperator.From_YYYYMMDD("20220820");
            var newerDate = Instances.DateOperator.GetToday();
            //var newerDate = Instances.DateOperator.GetDefault();
            //var newerDate = Instances.DateOperator.From_YYYYMMDD("20220819");


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
                    $"{olderDateString}: prior comparision date",
                    "",
                }
                .Append(
                    new[]
                    {
                        Instances.InstanceVariety.Functionality,
                        Instances.InstanceVariety.Values,
                        Instances.InstanceVariety.DataType,
                        Instances.InstanceVariety.UtilityType,
                        Instances.InstanceVariety.DraftFunctionality,
                        Instances.InstanceVariety.DraftValues,
                        Instances.InstanceVariety.DraftDataType,
                        Instances.InstanceVariety.DraftUtilityType,
                        Instances.InstanceVariety.MarkerAttribute,
                        Instances.InstanceVariety.DraftMarkerAttribute,
                        Instances.InstanceVariety.Explorations,
                        Instances.InstanceVariety.DraftExplorations,
                        Instances.InstanceVariety.Experiments,
                        Instances.InstanceVariety.DraftExperiments,
                        Instances.InstanceVariety.Demonstrations,
                        Instances.InstanceVariety.DraftDemonstrations,
                        Instances.InstanceVariety.Constants,
                        Instances.InstanceVariety.DraftConstants,
                    }
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

        private async Task CreateSummaryFile()
        {
            /// Inputs.
            var outputDirectoryPath = @"C:\Temp\Output\S0041\";

            //var olderDate = Instances.DateOperator.GetYesterday();
            //var olderDate = Instances.DateOperator.GetYesterday();
            var olderDate = Instances.DateOperator.From_YYYYMMDD("20220820");
            var newerDate = Instances.DateOperator.GetToday();
            //var newerDate = Instances.DateOperator.GetDefault();
            //var newerDate = Instances.DateOperator.From_YYYYMMDD("20220819");


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
            /// Inputs.
            var outputDirectoryPath = @"C:\Temp\Output\S0041\";

            //var olderDate = Instances.DateOperator.GetYesterday();
            //var olderDate = Instances.DateOperator.GetYesterday();
            var olderDate = Instances.DateOperator.From_YYYYMMDD("20220820");
            var newerDate = Instances.DateOperator.GetToday();
            //var newerDate = Instances.DateOperator.GetDefault();
            //var newerDate = Instances.DateOperator.From_YYYYMMDD("20220819");

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
            var outputDirectoryPath = @"C:\Temp\Output\S0041\";

            var date = Instances.DateOperator.GetToday();
            //var date = Instances.DateOperator.GetYesterday();
            //var date = Instances.DateOperator.From_YYYYMMDD("20220727");

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
                //.Where(x => x.ProjectFilePath == @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.T0142\source\R5T.T0142\R5T.T0142.csproj")
                //.Now()
                ;

            // Assembly actions.
            // Start with type name data.
            // Type name data of types for which we want method names.
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

            // Type name data of types for which we want property names.
            var propertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety = new Dictionary<string, string>
            {
                { Instances.InstanceVariety.Values, Instances.NamespacedTypeNames.ValuesMarkerAttribute },
                { Instances.InstanceVariety.DraftValues, Instances.NamespacedTypeNames.DraftValuesMarkerAttribute },
                { Instances.InstanceVariety.Constants, Instances.NamespacedTypeNames.ConstantsMarkerAttribute },
                { Instances.InstanceVariety.DraftConstants, Instances.NamespacedTypeNames.DraftsConstantsMarkerInterface },
            };

            // Type name data of types for which we want type names.
            var instanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName = new Dictionary<string, string>
            {
                { Instances.InstanceVariety.MarkerAttribute, F0000.Instances.TypeOperator.GetNamespacedTypeName<T0143.MarkerAttributeMarkerAttribute>() },
                { Instances.InstanceVariety.DraftMarkerAttribute, F0000.Instances.TypeOperator.GetNamespacedTypeName<T0143.DraftMarkerAttributeMarkerAttribute>() },
                { Instances.InstanceVariety.DataType, Instances.NamespacedTypeNames.DataTypeMarkerAttribute },
                { Instances.InstanceVariety.DraftDataType, Instances.NamespacedTypeNames.DraftDataTypeMarkerAttribute },
                { Instances.InstanceVariety.UtilityType, Instances.NamespacedTypeNames.UtilityTypeMarkerAttribute },
                { Instances.InstanceVariety.DraftUtilityType, Instances.NamespacedTypeNames.DraftUtilityTypeMarkerAttribute },
            };

            // Build the closures that will perform Assembly => InstancesIdentityNames for each type of code element (method or property), for each variety of instance (functionality, explorations, etc.).
            var getInstanceIdentityNamesByInstanceVariety = methodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety
                    .Select(xPair => (xPair.Key, Instances.Operations.GetInstanceMethodNamesProviderFunction(xPair.Value)))
                .Append(propertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety
                    .Select(xPair => (xPair.Key, Instances.Operations.GetInstancePropertyNamesProviderFunction(xPair.Value))))
                .Append(instanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName
                    .Select(xPair => (xPair.Key, Instances.Operations.GetInstanceTypeNamesProviderFunction(xPair.Value))))
                .ToDictionary(
                    x => x.Key,
                    x => x.Item2);

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
            // Keep track of the output file paths we will want to later show.
            var textOutputFilePaths = new List<string>();

            // Iterate over initial functionality methods so we can choose not to run some.
            await getInstanceIdentityNamesByInstanceVariety.ForEach(
                async pair =>
                {
                    var functionalityDescriptors = functionalityDescriptorsByFunctionalityVariety[pair.Key];

                    var varietyName = pair.Key;

                    var jsonOutputFilePath = Instances.FilePathOperator.GetJsonOutputFilePath_ForInstanceVariety(varietyName);
                    var textOutputFilePath = Instances.FilePathOperator.GetTextOutputFilePath_ForInstanceVariety(varietyName);

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
                                        : String.Empty
                                        ;

                                    var output = new InstanceDescriptor
                                    {
                                        IdentityName = x.IdentityName,
                                        ParameterNamedIdentityName = x.ParameterNamedIdentityName,
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
                                        : String.Empty
                                        ;

                                    var output = new InstanceDescriptor
                                    {
                                        IdentityName = x.IdentityName,
                                        ParameterNamedIdentityName = x.ParameterNamedIdentityName,
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