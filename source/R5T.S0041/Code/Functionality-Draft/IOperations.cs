using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using R5T.Magyar.Extensions;

using R5T.F0000;
using R5T.L0089.T000;
using R5T.T0132;


namespace R5T.S0041
{
    [DraftFunctionalityMarker]
    public partial interface IOperations : IDraftFunctionalityMarker
    {
        public void BuildProject(
            string projectFilePath,
            ILogger logger,
            Dictionary<string, string> buildProblemTextsByProjectFilePath)
        {
            try
            {
                Instances.Operations.BuildProject(
                    projectFilePath,
                    logger);
            }
            catch (AggregateException aggregateException)
            {
                logger.LogWarning("Failed to build project.");

                // Combine aggregate exception messages to text.
                var buildProblemText = Instances.StringOperator.Join(
                    Instances.Characters.NewLine,
                    aggregateException.InnerExceptions
                        .Select(exception => exception.Message));

                buildProblemTextsByProjectFilePath.Add(
                    projectFilePath,
                    buildProblemText);
            }

            // Output an S0041-specific file (R5T.S0041.Build.json) containing build time to publish directory.
            // Output this regardless of build success so that projects are not rebuilt in either case until project files change.
            var buildTime = Instances.NowOperator.Get_Now_Local();

            var buildResult = new BuildResult
            {
                Timestamp = buildTime,
            };

            var buildJsonFilePath = Instances.FilePathProvider.Get_BuildJsonFilePath(projectFilePath);

            Instances.JsonOperator.Serialize_Synchronous(
                buildJsonFilePath,
                buildResult);
        }

        public void BuildProject(
            string projectFilePath,
            ILogger logger)
        {
            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            Instances.FileSystemOperator.ClearDirectory_Synchronous(publishDirectoryPath);

            Instances.DotnetPublishOperator.Publish(
                projectFilePath,
                publishDirectoryPath);

            logger.LogInformation("Built project.");
        }

        public bool ShouldBuildProject(
            string projectFilePath,
            bool rebuildFailedBuildsToCollectErrors,
            ILogger logger)
        {
            logger.LogInformation("Determining whether the project should be built:\n\t{projectFilePath}", projectFilePath);

            var neverBuiltBefore = this.ShouldBuildProject_NeverBuiltBefore(
                projectFilePath,
                logger);

            if(neverBuiltBefore)
            {
                logger.LogInformation("Build project: never built (as part of this process).");

                return true;
            }

            var anyChangesSinceLastBuild = this.ShouldBuildProject_AnyChangesSinceLastBuild(
                projectFilePath,
                logger);

            if(anyChangesSinceLastBuild)
            {
                logger.LogInformation("Build project: changes found since last build.");

                return true;
            }

            // At this point, we know *an attempt* to build project has been tried before, and that there were no changes since the last attempt.

            // If the output assembly was not found, we should re-build the project.
            var outputAssemblyNotFound = this.ShouldBuildProject_OutputAssemblyNotFound(
                projectFilePath,
                logger);

            // But only if we want to wait to rebuild projects for which prior build attempts have failed.
            var rebuildProjectAfterPriorFailedBuilds = outputAssemblyNotFound && rebuildFailedBuildsToCollectErrors;

            if(rebuildProjectAfterPriorFailedBuilds)
            {
                logger.LogInformation("Build project: rebuild project after prior failure.");

                return true;
            }

            logger.LogInformation("Do not build project.");

            return false;
        }

        /// <summary>
        /// If the project has never been built before (as part of this process), it should be built.
        /// </summary>
        public bool ShouldBuildProject_NeverBuiltBefore(
            string projectFilePath,
            ILogger logger)
        {
            // Determine whether the project has been built before as part of this process by testing for the existence of the output build file specific to this process.
            var hasBuildResultFile = Instances.FileSystemOperator.Has_BuildResultFile(projectFilePath);

            if (hasBuildResultFile)
            {
                logger.LogInformation("Should not build: already built (as part of this process).");
                return false;
            }
            else
            {
                logger.LogInformation("Should build: never built (as part of this process).");
                return true;
            }
        }

        /// <summary>
        /// If a project has not been built (built during a prior run of this process, then it should be built).
        /// </summary>
        public bool ShouldBuildProject_OutputAssemblyNotFound(
            string projectFilePath,
            ILogger logger)
        {
            var outputAssemblyExists = Instances.FileSystemOperator.Has_OutputAssembly(projectFilePath);
            if (outputAssemblyExists)
            {
                logger.LogInformation("Should not build: output assembly already exists.");
                return false;
            }
            else
            {
                logger.LogInformation("Should build: output assembly does not exist.");
                return true;
            }
        }

        /// <summary>
        /// If a project has not changed since the last time it was built, then it should not be built (re-built).
        /// For the specific application of determining instances within a project, we only need to rebuild a project if files within that project have changed.
        /// Note: but for the general case of determining whether a project should be rebuilt, an examination of all files in the full recursive project references hierarchy should be performed (even including NuGet package reference update rules evaluation).
        /// </summary>
        public bool ShouldBuildProject_AnyChangesSinceLastBuild(
            string projectFilePath,
            ILogger logger)
        {
            // Assume that a project should be built.
            var shouldBuildProject = true;

            // Check latest file write time in project directory against build time in publish directory (R5T.S0041.Build.json).
            var projectDirectoryPath = Instances.ProjectPathsOperator.GetProjectDirectoryPath(projectFilePath);

            var projectFilesLastModifiedTime = F0000.FileSystemOperator.Instance.GetLastModifiedTime_ForDirectory_Local(
                projectDirectoryPath,
                Instances.DirectoryNameOperator.IsNotBinariesOrObjectsDirectory);

            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            var buildJsonFilePath = Instances.FilePathProvider.Get_BuildJsonFilePath_FromPublishDirectory(publishDirectoryPath);

            var buildJsonFileExists = Instances.FileSystemOperator.Exists_File(buildJsonFilePath);
            if(buildJsonFileExists)
            {
                var buildResult = Instances.JsonOperator.Deserialize_Synchronous<BuildResult>(
                    buildJsonFilePath);

                var lastBuildTime = buildResult.Timestamp;

                // If the last build time is greater than latest file write time, skip building project.
                var skipRepublishProject = lastBuildTime > projectFilesLastModifiedTime;
                if (skipRepublishProject)
                {
                    logger.LogInformation("Skip building project. (Project files last modified time: {projectFilesLastModifiedTime}, last build time: {lastBuildTime}", projectFilesLastModifiedTime, lastBuildTime);

                    shouldBuildProject = false;
                }
            }

            return shouldBuildProject;
        }

        public Dictionary<string, Func<Assembly, InstanceIdentityNames[]>> GetInstanceIdentityNamesProvidersByInstanceVariety()
        {
            // Type name data of types for which we want method names.
            var methodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety = Values.Instance.MethodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety;

            // Type name data of types for which we want property names.
            var propertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety = Values.Instance.PropertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety;

            // Type name data of types for which we want type names.
            var instanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName = Values.Instance.InstanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName;

            // Build the closures that will perform Assembly => InstancesIdentityNames for each type of code element (method or property), for each variety of instance (functionality, explorations, etc.).
            var intanceIdentityNamesProvidersByInstanceVariety = methodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety
                    .Select(xPair => (xPair.Key, Instances.Operations.GetInstanceMethodNamesProviderFunction(xPair.Value)))
                .Append(propertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety
                    .Select(xPair => (xPair.Key, Instances.Operations.GetInstancePropertyNamesProviderFunction(xPair.Value))))
                .Append(instanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName
                    .Select(xPair => (xPair.Key, Instances.Operations.GetInstanceTypeNamesProviderFunction(xPair.Value))))
                //// For debugging.
                //.Where(x => x.Key == Instances.InstanceVariety.MarkerAttribute)
                ////
                .ToDictionary(
                    x => x.Key,
                    x => x.Item2);

            return intanceIdentityNamesProvidersByInstanceVariety;
        }

        public DateTime GetPriorComparisonDate(DateTime date)
        {
            var outputDirectoryPath = Instances.DirectoryPaths.OutputDirectoryPath;

            // Look through the names of directories in the output directory.
            var directoryPaths = Instances.FileSystemOperator.EnumerateAllChildDirectoryPaths(outputDirectoryPath);

            // Find directories whose names are YYYYMMDDs,
            var directoryDates = directoryPaths
                .Select(directoryPath =>
                {
                    var directoryName = Instances.PathOperator.Get_DirectoryName_OfDirectoryPath(directoryPath);

                    var isYYYYMMDD = Instances.DateOperator.IsYYYYMMDD(directoryName);
                    return isYYYYMMDD;
                })
                .Where(x => x.Exists)
                // Only prior dates.
                .Where(x => x.Result < date)
                .Select(x => x.Result)
                .Now();

            // If none, return the default comparison date. Note: the date-to-date survey comparision has been implemented such that if a dated file path does not exist, an empty set of descriptors is returned.
            if(directoryDates.None())
            {
                return SpecialDates.Instance.DefaultPriorComparisonDate;
            }

            // Sort those directories in reverse chronological order, and choose the most recent.
            var mostRecentDate = directoryDates
                .OrderReverseChronologically()
                .First();

            return mostRecentDate;
        }

        public Func<Assembly, InstanceIdentityNames[]> GetInstancePropertyNamesProviderFunction(
            string markerAttributeNamespacedTypeName)
        {
            InstanceIdentityNames[] Internal(Assembly assembly)
            {
                var functionalityMethodNamesSet = new List<InstanceIdentityNames>();

                Instances.Operations.ForPropertiesOnTypes(
                    assembly,
                    Instances.Operations.GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName),
                    Instances.Operations.IsValuesProperty,
                    (typeInfo, propertyInfo) =>
                    {
                        var functionalityMethodNames = Instances.Operations.GetValuePropertyNames(propertyInfo);

                        functionalityMethodNamesSet.Add(functionalityMethodNames);
                    });

                return functionalityMethodNamesSet.ToArray();
            }

            return Internal;
        }

        public bool IsValuesProperty(PropertyInfo propertyInfo)
        {
            var output = true
                // Only properties with get methods.
                && propertyInfo.GetMethod is not null
                // Only properties with public get methods.
                && propertyInfo.GetMethod.IsPublic
                // Only properties *without* set methods.
                && propertyInfo.SetMethod is null
                // Only properties that are *not* indexers (which is tested by seeing if the property has any index parameters).
                && propertyInfo.GetIndexParameters().None()
                ;

            return output;
        }

        public InstanceIdentityNames GetValuePropertyNames(PropertyInfo propertyInfo)
        {
            var methodIdentityName = Instances.IdentityNameProvider.GetIdentityName(propertyInfo);

            var methodParameterNamedIdentityName = methodIdentityName; // TODO Instances.ParameterNamedIdentityNameProvider.GetParameterNamedIdentityName(methodInfo);

            var output = new InstanceIdentityNames
            {
                IdentityName = methodIdentityName,
                ParameterNamedIdentityName = methodParameterNamedIdentityName,
            };

            return output;
        }

        public async Task OutputSummaryFile(
            Dictionary<string, List<InstanceDescriptor>> functionalityDescriptorsByFunctionalityVariety,
            List<Failure<string>> problemProjects,
            string summaryFilePath)
        {
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

            await FileHelper.WriteAllLines(
                summaryFilePath,
                lines);
        }

        public Task OutputSummaryFile(
            Dictionary<string, List<InstanceDescriptor>> functionalityDescriptorsByFunctionalityVariety,
            List<Failure<string>> problemProjects)
        {
            var summaryFilePath = Instances.FilePaths.SummaryFilePath;

            return this.OutputSummaryFile(
                functionalityDescriptorsByFunctionalityVariety,
                problemProjects,
                summaryFilePath);
        }

        public async Task OutputFunctionalityFiles(
            ICollection<InstanceDescriptor> functionalityDescriptors,
            string title,
            string jsonOutputFilePath,
            string textOutputFilePath)
        {
            // Output JSON format data.
            JsonFileHelper.WriteToFile(
                jsonOutputFilePath,
                functionalityDescriptors);

            // Output text format data.
            await this.WriteFunctionalityDescriptors(
                title,
                textOutputFilePath,
                functionalityDescriptors);
        }

        public ProjectFilesTuple[] GetProjectFilesTuples(
            bool useProjectFilesTuplesCache)
        {
            var projectFilesTuplesJsonFilePath = this.GetProjectFilesTuplesJsonFilePath();

            return this.GetProjectFilesTuples(
                useProjectFilesTuplesCache,
                projectFilesTuplesJsonFilePath);
        }

        public ProjectFilesTuple[] GetProjectFilesTuples(
            bool useProjectFilesTuplesCache,
            string projectFilesTuplesJsonFilePath)
        {
            var canUseProjectFilesTuplesCache = useProjectFilesTuplesCache && Instances.FileSystemOperator.Exists_File(projectFilesTuplesJsonFilePath);

            var output = canUseProjectFilesTuplesCache
                ? this.LoadProjectFilesTuples(projectFilesTuplesJsonFilePath)
                : this.QueryProjectFilesTuples_AndWriteToFile(projectFilesTuplesJsonFilePath)
                ;

            return output;
        }

        public ProjectFilesTuple[] LoadProjectFilesTuples(
            string projectFilesTuplesJsonFilePath)
        {
            var projectFilesTuples = JsonFileHelper.LoadFromFile<ProjectFilesTuple[]>(
                projectFilesTuplesJsonFilePath);

            return projectFilesTuples;
        }

        /// <inheritdoc cref="QueryProjectFilesTuples_AndWriteToFile(string)"/>
        public void QueryProjectFilesTuples_AndWriteToFile()
        {
            var projectFilesTuplesJsonFilePath = this.GetProjectFilesTuplesJsonFilePath();

            this.QueryProjectFilesTuples_AndWriteToFile(projectFilesTuplesJsonFilePath);
        }

        /// <summary>
        /// Very quick operation.
        /// </summary>
        public ProjectFilesTuple[] QueryProjectFilesTuples_AndWriteToFile(
            string projectFilesTuplesJsonFilePath)
        {
            // Get project files.
            var projectFilePaths = Instances.Operations.QueryProjectFiles_AndWriteToFile();

            // Get project files tuples.
            var projectFilesTuples = Instances.Operations.GetProjectFilesTuples(projectFilePaths);

            JsonFileHelper.WriteToFile(
                projectFilesTuplesJsonFilePath,
                projectFilesTuples);

            return projectFilesTuples;
        }

        public string GetChildFilePathMatchingFileNameStem(
            string targetFilePath,
            string searchDirectoryPath)
        {
            var fileNameStem = Instances.PathOperator.Get_FileNameStem(targetFilePath);

            // Begins with the file name stem, followed by a dash, then eight (8) numeric digits.
            var regexPattern = $@"^{fileNameStem}-\d{{8}}";;

            var output = Instances.FileSystemOperator.FindChildFilesInDirectoryByRegexOnFileName(searchDirectoryPath, regexPattern)
                .Single();

            return output;
        }

        public (List<InstanceDescriptor> functionalityDescriptors, List<Failure<string>> problemProjects) GetFunctionalityDescriptors(
            ICollection<ProjectFilesTuple> projectFilesTuples,
            ILogger logger)
        {
            var output = this.GetFunctionalityDescriptors(
                projectFilesTuples,
                logger,
                tuple => Instances.Operations.AssemblyHasFunctionality(
                                tuple.AssemblyFilePath));

            return output;
        }

        public (List<InstanceDescriptor> functionalityDescriptors, List<Failure<string>> problemProjects) GetDraftFunctionalityDescriptors(
            ICollection<ProjectFilesTuple> projectFilesTuples,
            ILogger logger)
        {
            var output = this.GetFunctionalityDescriptors(
                projectFilesTuples,
                logger,
                tuple => Instances.Operations.AssemblyHasDraftFunctionality(
                                tuple.AssemblyFilePath));

            return output;
        }

        /// <summary>
        /// Processes project files tuples without checking whether the project and assembly files exist.
        /// </summary>
        public void ProcessProjectFilesTuples_NoFileExistenceChecks(
            ICollection<ProjectFilesTuple> projectFilesTuples,
            ILogger logger,
            Action<ProjectFilesTuple> projectFilesTupleAction,
            out List<Failure<string>> problemProjects)
        {
            // Use temp to allow capture inside closure.
            var tempProblemProjects = new List<Failure<string>>();

            var projectCount = projectFilesTuples.Count;

            projectFilesTuples.ForEach_WithCounter(
                (tuple, counter) =>
                {
                    try
                    {
                        logger.LogInformation($"Processing {counter} of {projectCount}:\n{tuple.ProjectFilePath}");

                        projectFilesTupleAction(tuple);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Processing failed:\n{tuple.ProjectFilePath}");

                        tempProblemProjects.Add(
                            Failure.Of(
                                tuple.ProjectFilePath,
                                ex.Message));
                    }
                });

            problemProjects = tempProblemProjects;
        }

        /// <summary>
        /// Checks that the project and assembly files exist for each tuple.
        /// </summary>
        public void ProcessProjectFilesTuples(
            ICollection<ProjectFilesTuple> projectFilesTuples,
            ILogger logger,
            Action<ProjectFilesTuple> projectFilesTupleAction,
            out List<Failure<string>> problemProjects)
        {
            this.ProcessProjectFilesTuples_NoFileExistenceChecks(
                projectFilesTuples,
                logger,
                tuple =>
                {
                    var projectFileExists = Instances.FileSystemOperator.Exists_File(tuple.ProjectFilePath);
                    if (!projectFileExists)
                    {
                        throw new Exception("Project file did not exist.");
                    }

                    var assemblyFileExists = Instances.FileSystemOperator.Exists_File(tuple.AssemblyFilePath);
                    if (!assemblyFileExists)
                    {
                        //throw new FileNotFoundException($"Assembly file did not exist: {tuple.AssemblyFilePath}", tuple.AssemblyFilePath);
                        throw new FileNotFoundException($"Assembly file did not exist.");
                    }

                    // Else, run the provided action.
                    projectFilesTupleAction(tuple);
                },
                out problemProjects);
        }

        public (List<InstanceDescriptor> functionalityDescriptors, List<Failure<string>> problemProjects) GetFunctionalityDescriptors(
            ICollection<ProjectFilesTuple> projectFilesTuples,
            ILogger logger,
            Func<ProjectFilesTuple, WasFound<InstanceIdentityNames[]>> getFunctionalityMethodNames)
        {
            var problemProjects = new List<Failure<string>>();

            var functionalityDescriptors = new List<InstanceDescriptor>();

            var projectCount = projectFilesTuples.Count;

            var counter = 1;
            foreach (var tuple in projectFilesTuples)
            {
                try
                {
                    logger.LogInformation($"Processing {counter} of {projectCount}:\n{tuple.ProjectFilePath}");

                    var hasFunctionality = Instances.Operations.ProcessProjectFilesTuple(
                        tuple,
                        getFunctionalityMethodNames);

                    if (hasFunctionality)
                    {
                        var documentationByMethodIdentityName = Instances.Operations.GetDocumentationForMemberIdentityNames(
                            hasFunctionality.Result
                                .Select(x => x.IdentityName),
                            tuple.DocumentationFilePath);

                        var descriptors = this.GetDescriptors(
                            tuple.ProjectFilePath,
                            hasFunctionality.Result,
                            documentationByMethodIdentityName);

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

            return (functionalityDescriptors, problemProjects);
        }

        public string GetDateComparisonOutputFilePath(
            DateTime firstDate,
            DateTime secondDate,
            string filePath,
            string outputDirectoryPath)
        {
            var firstDateString = Instances.DateOperator.ToString_YYYYMMDD(firstDate);
            var secondDateString = Instances.DateOperator.ToString_YYYYMMDD(secondDate);

            var fileName = Instances.PathOperator.Get_FileName(filePath);

            var fileNameStem = Instances.FileNameOperator.Get_FileNameStem(fileName);
            var fileExtension = Instances.FileNameOperator.Get_FileExtension(fileName);

            var datedFileNameStem = $"{fileNameStem}-{firstDateString} to {secondDateString}";
            var datedFileName = Instances.FileNameOperator.Get_FileName(
                datedFileNameStem,
                fileExtension);

            var datedFilePath = Instances.PathOperator.Get_FilePath(
                outputDirectoryPath,
                datedFileName);

            return datedFilePath;
        }

        public string GetDatedOutputFilePath(
            DateTime date,
            string filePath,
            string outputDirectoryPath)
        {
            var dateString = Instances.DateOperator.ToString_YYYYMMDD(date);

            var fileName = Instances.PathOperator.Get_FileName(filePath);

            var fileNameStem = Instances.FileNameOperator.Get_FileNameStem(fileName);
            var fileExtension = Instances.FileNameOperator.Get_FileExtension(fileName);

            var datedFileNameStem = $"{fileNameStem}-{dateString}";
            var datedFileName = Instances.FileNameOperator.Get_FileName(
                datedFileNameStem,
                fileExtension);

            var datedFilePath = Instances.PathOperator.Get_FilePath(
                outputDirectoryPath,
                datedFileName);

            return datedFilePath;
        }

        public InstanceDescriptor GetDescriptor(
            string projectFilePath,
            InstanceIdentityNames functionalityMethodNames,
            Dictionary<string, string> documentationByMemberIdentityName)
        {
            var identityName = functionalityMethodNames.IdentityName;

            var descriptionXml = documentationByMemberIdentityName.ValueOrDefault(
                identityName,
                Instances.Strings.Empty);

            var output = new InstanceDescriptor
            {
                IdentityName = identityName,
                ParameterNamedIdentityName = functionalityMethodNames.ParameterNamedIdentityName,
                ProjectFilePath = projectFilePath,
                DescriptionXml = descriptionXml
            };

            return output;
        }

        public InstanceDescriptor GetDescriptor(
            string projectFilePath,
            InstanceIdentityNames functionalityMethodNames,
            Dictionary<string, WasFound<string>> documentationByMethodIdentityName)
        {
            var identityName = functionalityMethodNames.IdentityName;

            var descriptionXml = documentationByMethodIdentityName[identityName].Exists
                ? documentationByMethodIdentityName[identityName]
                : Instances.Strings.Empty
                ;

            var output = new InstanceDescriptor
            {
                IdentityName = identityName,
                ParameterNamedIdentityName = functionalityMethodNames.ParameterNamedIdentityName,
                ProjectFilePath = projectFilePath,
                DescriptionXml = descriptionXml
            };

            return output;
        }

        public IEnumerable<InstanceDescriptor> GetDescriptors(
            string projectFilePath,
            IEnumerable<InstanceIdentityNames> functionalityMethodNamesSet,
            Dictionary<string, WasFound<string>> documentationByMethodIdentityName)
        {
            var output = functionalityMethodNamesSet
                .Select(functionalityMethodNames => this.GetDescriptor(
                    projectFilePath,
                    functionalityMethodNames,
                    documentationByMethodIdentityName))
                ;

            return output;
        }

        public string[] GetProjectFiles(
            bool requeryProjectFiles)
        {
            var output = requeryProjectFiles
                ? this.QueryProjectFiles_AndWriteToFile()
                : this.LoadProjectsListFile()
                ;

            return output;
        }

        /// <summary>
        /// Returns empty if the file path does not exist.
        /// </summary>
        public InstanceDescriptor[] LoadFunctionalityDescriptors(
            string instanceDescriptorJsonFilePath)
        {
            var fileExists = Instances.FileSystemOperator.Exists_File(instanceDescriptorJsonFilePath);
            if (!fileExists)
            {
                return Array.Empty<InstanceDescriptor>();
            }

            var output = JsonFileHelper.LoadFromFile<InstanceDescriptor[]>(instanceDescriptorJsonFilePath);
            return output;
        }

        public string[] LoadProjectsListFile()
        {
            var output = this.LoadProjectsListFile(
                this.GetProjectsListTextFilePath());

            return output;
        }

        public string[] LoadProjectsListFile(
            string projectsListTextFilePath)
        {
            var output = FileHelper.ActuallyReadAllLines(projectsListTextFilePath)
                // Skip the title lines.
                .Skip(2)
                .ToArray();

            return output;
        }

        public string[] QueryProjectFiles_AndWriteToFile()
        {
            var output = this.QueryProjectFiles_InRepositoriesDirectories_AndWriteToListTextFile(
                Instances.Operations.GetRepositoriesDirectoryPaths(),
                this.GetProjectsListTextFilePath());

            return output;
        }

        public string[] QueryProjectFiles_InRepositoriesDirectories_AndWriteToListTextFile(
            IEnumerable<string> repositoriesDirectoryPaths,
            string projectsListTextFilePath)
        {
            var output = this.QueryProjectFiles_InRepositoriesDirectories(
                repositoriesDirectoryPaths);

            this.WriteProjectsToListTextFile(
                output,
                projectsListTextFilePath);

            return output;
        }

        public void WriteProjectsToListTextFile(
            ICollection<string> projectFilePaths,
            string projectsListTextFilePath)
        {
            var lines = EnumerableHelper.From($"Projects, Count: {projectFilePaths.Count}\n\n")
                .Append(projectFilePaths.OrderAlphabetically2());
                    //.OrderAlphabetically());

            FileHelper.WriteAllLines_Synchronous(
                    projectsListTextFilePath,
                    lines);
        }

        public string[] QueryProjectFiles_InRepositoriesDirectories(
            IEnumerable<string> repositoriesDirectoryPaths)
        {
            var output = repositoriesDirectoryPaths
                .SelectMany(repositoriesDirectoryPath =>
                    Instances.Operations.GetAllProjectFilePaths(repositoriesDirectoryPath))
                //.OrderAlphabetically_OnlyIfDebug()
                .ToArray();

            return output;
        }

        public ProjectFilesTuple[] GetProjectFilesTuples(
            IEnumerable<string> projectFilePaths)
        {
            var output = projectFilePaths
                .Select(projectFilePath =>
                {
                    var projectAssemblyFilePath = F0040.ProjectPathsOperator.Instance.GetAssemblyFilePathForProjectFilePath(projectFilePath);
                    var documentationFilePath = F0040.ProjectPathsOperator.Instance.GetDocumentationFilePathForProjectFilePath(projectFilePath);

                    var output = new ProjectFilesTuple
                    {
                        AssemblyFilePath = projectAssemblyFilePath,
                        DocumentationFilePath = documentationFilePath,
                        ProjectFilePath = projectFilePath,
                    };

                    return output;
                })
                .Now();

            return output;
        }

        public string GetProjectFilesTuplesJsonFilePath()
        {
            return @"C:\Temp\Project File Tuples.json";
        }

        public string GetProjectsListTextFilePath()
        {
            return @"C:\Temp\Projects List.txt";
        }

        public Task WriteFunctionalityDescriptors(
            string title,
            string outputFilePath,
            ICollection<InstanceDescriptor> functionalityDescriptors)
        {
            var lines = EnumerableHelper.From($"{title}, Count: {functionalityDescriptors.Count}\n\n")
                .Append(functionalityDescriptors
                    .GroupBy(x => x.ProjectFilePath)
                    .OrderAlphabetically(x => x.Key)
                    .SelectMany(xGroup => EnumerableHelper.From($"{xGroup.Key}:")
                        .Append(xGroup
                            .OrderAlphabetically(x => x.IdentityName)
                            .Select(x => $"\t{x.IdentityName}")
                            .Append(Instances.Strings.Empty))));

            return FileHelper.WriteAllLines(
                outputFilePath,
                lines);
        }

        public Task WriteProblemProjectsFile(
            string problemProjectsFilePath,
            ICollection<Failure<string>> problemProjects)
        {
            return FileHelper.WriteAllLines(
                problemProjectsFilePath,
                EnumerableHelper.From($"Problem Projects, Count: {problemProjects.Count}\n\n")
                    .Append(problemProjects
                        .GroupBy(x => x.Message)
                        .OrderAlphabetically(x => x.Key)
                        .SelectMany(xGrouping => EnumerableHelper.From($"{xGrouping.Key}:")
                            .Append(xGrouping
                                .Select(x => $"{x.Value}: {x.Message}"))
                            .Append("***\n"))));
        }

        public void WriteProblemProjectsFile(
            string problemProjectsFilePath,
            Dictionary<string, string> problemProjects)
        {
            FileHelper.WriteAllLines_Synchronous(
                problemProjectsFilePath,
                EnumerableHelper.From($"Problem Projects, Count: {problemProjects.Count}\n\n")
                    .Append(problemProjects
                        .OrderAlphabetically(pair => pair.Key)
                        .SelectMany(pair => EnumerableHelper.From($"{pair.Key}:")
                            .Append(pair.Value)
                            .Append("***\n"))));
        }

        public Dictionary<string, string> GetDocumentationByMemberIdentityName(
            string documentationXmlFilePath)
        {
            var output = new Dictionary<string, string>();

            var documentationFileExists = File.Exists(documentationXmlFilePath);
            if (documentationFileExists)
            {
                var documentation = XDocumentHelper.LoadXDocument(documentationXmlFilePath);

                var membersNode = documentation.XPathSelectElement("//doc/members");

                var membersNodeExists = membersNode is not null;
                if (membersNodeExists)
                {
                    var memberNodes = membersNode.XPathSelectElements("member");
                    foreach (var memberNode in memberNodes)
                    {
                        var memberIdentityName = memberNode.Attribute("name").Value;
                        var documentationForMember = memberNode.FirstNode.ToString();

                        var prettyPrintedDocumentationForMember = DocumentationOperator.Instance.PrettyPrint(documentationForMember);

                        output.Add(memberIdentityName, prettyPrintedDocumentationForMember);
                    }
                }
            }

            return output;
        }

        public string GetDocumentationForMemberIdentityName(
            string documentationXmlFilePath,
            string memberIdentityName)
        {
            var wasFound = this.HasDocumentationForMemberIdentityName(
                documentationXmlFilePath,
                memberIdentityName);

            var output = Instances.WasFoundOperator.Get_Result_OrExceptionIfNotFound(
                wasFound,
                $"{memberIdentityName}: No documentation for member.");

            return output;
        }

        public WasFound<string> HasDocumentationForMemberIdentityName(
            string documentationXmlFilePath,
            string memberIdentityName)
        {
            var documentationForMemberIdentityNames = this.GetDocumentationForMemberIdentityNames(
                documentationXmlFilePath,
                memberIdentityName);

            var wasFound = documentationForMemberIdentityNames[memberIdentityName];
            return wasFound;
        }

        public Dictionary<string, WasFound<string>> GetDocumentationForMemberIdentityNames(
            string documentationXmlFilePath,
            params string[] memberIdentityNames)
        {
            var output = this.GetDocumentationForMemberIdentityNames(
                memberIdentityNames.AsEnumerable(),
                documentationXmlFilePath);

            return output;
        }

        public Dictionary<string, WasFound<string>> GetDocumentationForMemberIdentityNames(
            IEnumerable<string> memberIdentityNames,
            string documentationXmlFilePath)
        {
            var documentationFileExists = File.Exists(documentationXmlFilePath);
            if(documentationFileExists)
            {
                var documentation = XDocumentHelper.LoadXDocument(documentationXmlFilePath);

                var membersNode = documentation.XPathSelectElement("//doc/members");

                var membersNodeExists = membersNode is not null;
                if(membersNodeExists)
                {
                    var output = memberIdentityNames
                        .Select(memberIdentityName =>
                        {
                            var query = $"member[@name='{memberIdentityName}']";

                            var memberNode = membersNode.XPathSelectElement(query);

                            var memberNodeExists = memberNode is not null;

                            var documentation = memberNodeExists
                                ? memberNode.FirstNode.ToString()
                                : null
                                ;

                            var prettyPrintedDocumentation = memberNodeExists
                                ? DocumentationOperator.Instance.PrettyPrint(documentation)
                                : null
                                ;

                            var wasFound = memberNodeExists
                                ? WasFound.Found(prettyPrintedDocumentation)
                                : WasFound.NotFound<string>()
                                ;

                            var output = (memberIdentityName, wasFound);
                            return output;
                        })
                        .ToDictionary(
                            x => x.memberIdentityName,
                            x => x.wasFound);

                    return output;
                }
            }

            var defaultOutput = memberIdentityNames
                .Distinct()
                .ToDictionary(
                    x => x,
                    x => WasFound.NotFound<string>());

            return defaultOutput;
        }

        public N002.InstanceDescriptor[] ProcessAssemblyFile(
            string projectFilePath,
            string assemblyFilePath,
            string documentationForAssemblyFilePath)
        {
            var instanceDescriptorsWithoutProjectFile = this.ProcessAssemblyFile(
                assemblyFilePath,
                documentationForAssemblyFilePath);

            var output = instanceDescriptorsWithoutProjectFile
                .Select(x =>
                {
                    var instanceDescriptor = new N002.InstanceDescriptor
                    {
                        ProjectFilePath = projectFilePath,
                        InstanceVariety = x.InstanceVariety,
                        IdentityName = x.IdentityName,
                        ParameterNamedIdentityName = x.ParameterNamedIdentityName,
                        DescriptionXml = x.DescriptionXml,
                    };

                    return instanceDescriptor;
                })
                .Now();

            return output;
        }

        public N003.InstanceDescriptor[] ProcessAssemblyFile(
            string assemblyFilePath)
        {
            var documentationForAssemblyFilePath = F0040.ProjectPathsOperator.Instance.GetDocumentationFilePath_ForAssemblyFilePath(
                assemblyFilePath);

            return this.ProcessAssemblyFile(
                assemblyFilePath,
                documentationForAssemblyFilePath);
        }

        public N003.InstanceDescriptor[] ProcessAssemblyFile(
            string assemblyFilePath,
            string documentationForAssemblyFilePath)
        {
            var documentationByMemberIdentityName = Operations.Instance.GetDocumentationByMemberIdentityName(
                documentationForAssemblyFilePath);

            var intanceIdentityNamesProvidersByInstanceVariety = Operations.Instance.GetInstanceIdentityNamesProvidersByInstanceVariety();

            var instanceIdentityNameSetsByVarietyType = Instances.ReflectionOperator.InAssemblyContext(
                assemblyFilePath,
                assembly =>
                {
                    var output = intanceIdentityNamesProvidersByInstanceVariety
                        .Select(pair =>
                        {
                            var instanceIdentityNamesSet = pair.Value(assembly);

                            return (pair.Key, instanceIdentityNamesSet);
                        })
                        .ToDictionary(
                            x => x.Key,
                            x => x.instanceIdentityNamesSet);

                    return output;
                });

            var output = instanceIdentityNameSetsByVarietyType
                .SelectMany(pair =>
                {
                    var instanceVariety = pair.Key;

                    var output = pair.Value
                        .Select(x =>
                        {
                            var documentationXml = documentationByMemberIdentityName.HasValue(x.IdentityName)
                                ? documentationByMemberIdentityName[x.IdentityName]
                                : default
                                ;

                            var output = new N003.InstanceDescriptor
                            {
                                InstanceVariety = instanceVariety,
                                IdentityName = x.IdentityName,
                                ParameterNamedIdentityName = x.ParameterNamedIdentityName,
                                DescriptionXml = documentationXml,
                            };

                            return output;
                        });

                    return output;
                })
                .Now();

            return output;
        }

        public void ProcessProjectFilesTuple(
            ProjectFilesTuple tuple,
            Dictionary<string, Func<Assembly, InstanceIdentityNames[]>> getInstanceIdentityNamesByInstanceVariety,
            Dictionary<string, List<InstanceDescriptor>> functionalityDescriptorsByFunctionalityVariety)
        {
            // Get project documentation file contents.
            var documentationByMemberIdentityName = Instances.Operations.GetDocumentationByMemberIdentityName(
                tuple.DocumentationFilePath);

            var instanceIdentityNamesSetsByFunctionalityVariety = new Dictionary<string, InstanceIdentityNames[]>();

            // Perform all assembly actions on the assembly.
            Instances.ReflectionOperator.InAssemblyContext(
                tuple.AssemblyFilePath,
                EnumerableOperator.Instance.From(Instances.DirectoryPaths.NuGetAssemblies),
                assembly =>
                {
                    getInstanceIdentityNamesByInstanceVariety.ForEach(
                        pair =>
                        {
                            var instanceIdentityNamesSet = pair.Value(assembly);

                            instanceIdentityNamesSetsByFunctionalityVariety.Add(pair.Key, instanceIdentityNamesSet);
                        });
                });

            // Create all descriptors, and add to output.
            instanceIdentityNamesSetsByFunctionalityVariety.ForEach(
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
        }

        /// <summary>
        /// Check that the project file and assembly files exist, then run the project files tuple action.
        /// </summary>
        public TOut ProcessProjectFilesTuple<TOut>(
            ProjectFilesTuple tuple,
            Func<ProjectFilesTuple, TOut> projectFilesTupleFunction)
        {
            var projectFileExists = Instances.FileSystemOperator.Exists_File(tuple.ProjectFilePath);
            if (!projectFileExists)
            {
                throw new Exception("Project file did not exist.");
            }

            var assemblyFileExists = Instances.FileSystemOperator.Exists_File(tuple.AssemblyFilePath);
            if (!assemblyFileExists)
            {
                throw new Exception("Assembly file did not exist.");
            }

            var output = projectFilesTupleFunction(tuple);
            return output;
        }

        public IEnumerable<(TypeInfo TypeInfo, MethodInfo MethodInfo)> SelectMethodsOnTypes(
            Assembly assembly,
            Func<TypeInfo, bool> typeSelector,
            Func<MethodInfo, bool> methodSelector)
        {
            var output = AssemblyOperator.Instance.SelectTypes(assembly, typeSelector)
                .SelectMany(typeInfo => typeInfo.DeclaredMethods
                    .Where(methodSelector)
                    .Select(methodInfo => (typeInfo, methodInfo)));

            return output;
        }

        public IEnumerable<(TypeInfo TypeInfo, PropertyInfo PropertyInfo)> SelectPropertiesOnTypes(
            Assembly assembly,
            Func<TypeInfo, bool> typeSelector,
            Func<PropertyInfo, bool> propertySelector)
        {
            var output = assembly.DefinedTypes
                .Where(typeSelector)
                .SelectMany(typeInfo => typeInfo.DeclaredProperties
                    .Where(propertySelector)
                    .Select(propertyInfo => (typeInfo, propertyInfo)));

            return output;
        }

        public void ForMethodsOnTypes(
            Assembly assembly,
            Func<TypeInfo, bool> typeSelector,
            Func<MethodInfo, bool> methodSelector,
            Action<TypeInfo, MethodInfo> action)
        {
            var methodsOnTypes = this.SelectMethodsOnTypes(
                assembly,
                typeSelector,
                methodSelector);

            methodsOnTypes.ForEach(tuple => action(tuple.TypeInfo, tuple.MethodInfo));
        }

        public void ForPropertiesOnTypes(
            Assembly assembly,
            Func<TypeInfo, bool> typeSelector,
            Func<PropertyInfo, bool> propertySelector,
            Action<TypeInfo, PropertyInfo> action)
        {
            var propertiesOnTypes = this.SelectPropertiesOnTypes(
                assembly,
                typeSelector,
                propertySelector);

            propertiesOnTypes.ForEach(tuple => action(tuple.TypeInfo, tuple.PropertyInfo));
        }

        public Func<Assembly, InstanceIdentityNames[]> GetInstanceTypeNamesProviderFunction(
            string markerAttributeNamespacedTypeName)
        {
            InstanceIdentityNames[] Internal(Assembly assembly)
            {
                var functionalityMethodNamesSet = new List<InstanceIdentityNames>();

                AssemblyOperator.Instance.ForTypes(
                    assembly,
                    Instances.Operations.GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName),
                    typeInfo =>
                    {
                        var functionalityMethodNames = Instances.Operations.GetTypeInstanceIdentityNames(typeInfo);

                        functionalityMethodNamesSet.Add(functionalityMethodNames);
                    });

                return functionalityMethodNamesSet.ToArray();
            }

            return Internal;
        }

        public Func<Assembly, InstanceIdentityNames[]> GetInstanceMethodNamesProviderFunction(
            string markerAttributeNamespacedTypeName)
        {
            InstanceIdentityNames[] Internal(Assembly assembly)
            {
                var typeInstanceIdentityNamesSet = new List<InstanceIdentityNames>();

                Instances.Operations.ForMethodsOnTypes(
                    assembly,
                    Instances.Operations.GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName),
                    Instances.Operations.IsInstanceMethod,
                    (typeInfo, methodInfo) =>
                    {
                        var functionalityMethodNames = Instances.Operations.GetMethodInstanceIdentityNames(methodInfo);

                        typeInstanceIdentityNamesSet.Add(functionalityMethodNames);
                    });

                return typeInstanceIdentityNamesSet.ToArray();
            }

            return Internal;
        }

        public InstanceIdentityNames GetTypeInstanceIdentityNames(TypeInfo typeInfo)
        {
            var typeIdentityName = Instances.IdentityNameProvider.GetIdentityName(typeInfo);

            var typeParameterNamedIdentityName = typeIdentityName; // Does not exist yet: Instances.ParameterNamedIdentityNameProvider.GetParameterNamedIdentityName(typeInfo);

            var output = new InstanceIdentityNames
            {
                IdentityName = typeIdentityName,
                ParameterNamedIdentityName = typeParameterNamedIdentityName,
            };

            return output;
        }

        public InstanceIdentityNames GetMethodInstanceIdentityNames(MethodInfo methodInfo)
        {
            var methodIdentityName = Instances.IdentityNameProvider.GetIdentityName(methodInfo);

            var methodParameterNamedIdentityName = Instances.ParameterNamedIdentityNameProvider.GetParameterNamedIdentityName(methodInfo);

            var output = new InstanceIdentityNames
            {
                IdentityName = methodIdentityName,
                ParameterNamedIdentityName = methodParameterNamedIdentityName,
            };

            return output;
        }

        public WasFound<InstanceIdentityNames[]> AssemblyHasFunctionality(
            string assemblyFilePath)
        {
            var output = Instances.ReflectionOperator.InAssemblyContext(
                assemblyFilePath,
                assembly =>
                {
                    var hasFunctionalityTypes = this.AssemblyHasFunctionalityTypes(assembly);

                    var functionalityMethods = hasFunctionalityTypes
                        ? this.GetFunctionalityMethods(hasFunctionalityTypes.Result)
                        : Array.Empty<MethodInfo>()
                        ;

                    var functionaityMethodNames = functionalityMethods
                        .Select(this.GetMethodInstanceIdentityNames)
                        .ToArray();

                    var output = WasFound.From_Array(functionaityMethodNames);
                    return output;
                });

            return output;
        }

        public WasFound<InstanceIdentityNames[]> AssemblyHasDraftFunctionality(
            string assemblyFilePath)
        {
            var output = Instances.ReflectionOperator.InAssemblyContext(
                assemblyFilePath,
                assembly =>
                {
                    var hasDraftFunctionalityTypes = this.AssemblyHasDraftFunctionalityTypes(assembly);

                    var draftFunctionalityMethods = hasDraftFunctionalityTypes
                        ? this.GetFunctionalityMethods(hasDraftFunctionalityTypes.Result)
                        : Array.Empty<MethodInfo>()
                        ;

                    var draftFunctionaityMethodNames = draftFunctionalityMethods
                        .Select(xMethodInfo =>
                        {
                            var methodIdentityName = Instances.IdentityNameProvider.GetIdentityName(xMethodInfo);

                            var methodParameterNamedIdentityName = Instances.ParameterNamedIdentityNameProvider.GetParameterNamedIdentityName(xMethodInfo);

                            var output = new InstanceIdentityNames
                            {
                                IdentityName = methodIdentityName,
                                ParameterNamedIdentityName = methodParameterNamedIdentityName,
                            };

                            return output;
                        })
                        .ToArray();

                    var output = WasFound.From_Array(draftFunctionaityMethodNames);
                    return output;
                });

            return output;
        }

        /// <summary>
        /// Find all draft functionality in an assembly file and return a list of method identity names.
        /// </summary>
        public WasFound<string[]> AssemblyHasDraftFunctionality_GetMethodIdentityNames(string assemblyFilePath)
        {
            var output = Instances.ReflectionOperator.InAssemblyContext(
                assemblyFilePath,
                assembly =>
                {
                    var hasDraftFunctionalityTypes = this.AssemblyHasDraftFunctionalityTypes(assembly);

                    var draftFunctionalityMethods = hasDraftFunctionalityTypes

                        ? this.GetFunctionalityMethods(hasDraftFunctionalityTypes.Result)
                        : Array.Empty<MethodInfo>()
                        ;

                    var draftFunctionaityMethodNames = draftFunctionalityMethods
                        .Select(xMethodInfo => Instances.IdentityNameProvider.GetIdentityName(xMethodInfo))
                        .ToArray();

                    var output = WasFound.From_Array(draftFunctionaityMethodNames);
                    return output;
                });

            return output;
        }

        public bool IsInstanceMethod(MethodInfo methodInfo)
        {
            var output = true
                // Only public methods.
                && methodInfo.IsPublic
                // Must not be a property.
                && !Instances.TypeOperator.IsPropertyMethod(methodInfo)
                ;

            return output;
        }

        /// <summary>
        /// Get all functionality methods in an enumerable of draft functionality types (which is just all methods in the types).
        /// Note: draft functionality methods are selected the same way as functionality methods.
        /// </summary>
        public MethodInfo[] GetFunctionalityMethods(IEnumerable<TypeInfo> draftFunctionalityTypes)
        {
            var draftFunctionalityMethods = draftFunctionalityTypes
                .SelectMany(x => x.DeclaredMethods)
                .Where(this.IsInstanceMethod)
                .ToArray();

            var output = WasFound.From_Array(draftFunctionalityMethods);
            return output;
        }

        /// <summary>
        /// Get all draft functionality methods in an enumerable of draft functionality types (which is just all methods in the types).
        /// </summary>
        public MethodInfo[] GetunctionalityMethods(IEnumerable<TypeInfo> draftFunctionalityTypes)
        {
            var draftFunctionalityMethods = draftFunctionalityTypes
                .SelectMany(x => x.DeclaredMethods)
                .Where(xMethod =>
                {
                    // Only public methods.
                    var output = xMethod.IsPublic;
                    return output;
                })
                .ToArray();

            var output = WasFound.From_Array(draftFunctionalityMethods);
            return output;
        }

        /// <summary>
        /// Get the draft functionality types in an assembly.
        /// </summary>
        public WasFound<TypeInfo[]> AssemblyHasDraftFunctionalityTypes(
            Assembly assembly)
        {
            var draftFunctionalityTypes = this.AssemblyHasTypesWhere(
                assembly,
                this.GetDraftFunctionalityPredicate());

            var output = WasFound.From_Array(draftFunctionalityTypes);
            return output;
        }

        /// <summary>
        /// Get the draft functionality types in an assembly.
        /// </summary>
        public WasFound<TypeInfo[]> AssemblyHasFunctionalityTypes(
            Assembly assembly)
        {
            var functionalityTypes = this.AssemblyHasTypesWhere(
                assembly,
                this.GetFunctionalityPredicate());

            var output = WasFound.From_Array(functionalityTypes);
            return output;
        }

        /// <summary>
        /// Get all draft functionality types in an assembly and return the type full names. (TODO: should return identity names)
        /// </summary>
        public WasFound<string[]> AssemblyHasDraftFunctionalityTypes(string assemblyFilePath)
        {
            var output = Instances.ReflectionOperator.InAssemblyContext(
                assemblyFilePath,
                assembly =>
                {
                    var draftFunctionalityTypes = this.AssemblyHasTypesWhere(
                        assembly,
                        this.GetDraftFunctionalityPredicate());

                    // Need to query for full names while the metadata context is alive (since full name apparently depends on that).
                    var draftFunctionalityTypeNames = draftFunctionalityTypes
                        .Select(xType => xType.FullName)
                        .Now();

                    var output = WasFound.From_Array(draftFunctionalityTypeNames);
                    return output;
                });

            return output;
        }

        /// <summary>
        /// Gets the predicate to determine if a type is a draft functionality type.
        /// </summary>
        public Func<TypeInfo, bool> GetDraftFunctionalityPredicate()
        {
            var draftFunctionalityMarkerAttributeNamespacedTypeName = "R5T.T0132.DraftFunctionalityMarkerAttribute";

            var attributeTypeNamesOfInterest = new HashSet<string>(new[]
            {
                draftFunctionalityMarkerAttributeNamespacedTypeName,
            });

            // Only want attributed types, not interfaced.

            bool Internal(TypeInfo type)
            {
                var output = type.CustomAttributes
                        .Where(xAttribute => attributeTypeNamesOfInterest.Contains(xAttribute.AttributeType.FullName))
                        .Any();

                return output;
            }

            return Internal;
        }

        public Func<TypeInfo, bool> GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(
            string markerAttributeNamespacedTypeName)
        {
            return F0018.TypeOperator.Instance.GetTypeByHasAttributeOfNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName);
        }

        /// <summary>
        /// Gets the predicate to determine if a type is a draft functionality type.
        /// </summary>
        public Func<TypeInfo, bool> GetFunctionalityPredicate()
        {
            var functionalityMarkerAttributeNamespacedTypeName = "R5T.T0132.FunctionalityMarkerAttribute";

            var attributeTypeNamesOfInterest = new HashSet<string>(new[]
            {
                functionalityMarkerAttributeNamespacedTypeName,
            });

            // Only want attributed types, not interfaced.

            bool Internal(TypeInfo type)
            {
                var output = type.CustomAttributes
                    .Where(xAttribute => attributeTypeNamesOfInterest.Contains(xAttribute.AttributeType.FullName))
                    .Any();

                return output;
            }

            return Internal;
        }

        /// <summary>
        /// Applies a predicate to the types of an assembly.
        /// </summary>
        public TypeInfo[] AssemblyHasTypesWhere(
            Assembly assembly,
            Func<TypeInfo, bool> predicate)
        {
            var output = assembly.DefinedTypes
                .Where(predicate)
                .Now();

            return output;
        }
    }
}
