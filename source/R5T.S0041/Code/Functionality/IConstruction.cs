using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

using R5T.T0132;
using R5T.T0159.Extensions;


namespace R5T.S0041
{
    [FunctionalityMarker]
    public partial interface IConstruction : IFunctionalityMarker
    {
        public void CompareDates()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();
            var priorDate = Instances.Operations.GetPriorComparisonDate(date);


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var datedInstancesJsonFilePath = Instances.FilePathProvider.Get_InstancesJsonFilePath(datedOutputDirectoryPath);

            var priorDatedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(priorDate);

            var priorDatedInstancesJsonFilePath = Instances.FilePathProvider.Get_InstancesJsonFilePath(priorDatedOutputDirectoryPath);

            var datedInstances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(datedInstancesJsonFilePath);
            var priorDatedInstances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(priorDatedInstancesJsonFilePath);

            var newInstances = datedInstances.Except(
                priorDatedInstances,
                N002.InstanceDescriptorEqualityComparer.Instance)
                .Now();

            var oldInstances = priorDatedInstances.Except(
                datedInstances,
                N002.InstanceDescriptorEqualityComparer.Instance)
                .Now();

            var newInstancesJsonFilePath = Instances.FilePathProvider.Get_NewInstancesJsonFilePath(datedOutputDirectoryPath);
            var oldInstancesJsonFilePath = Instances.FilePathProvider.Get_OldInstancesJsonFilePath(datedOutputDirectoryPath);

            Instances.JsonOperator.Serialize(
                newInstancesJsonFilePath,
                newInstances);

            Instances.JsonOperator.Serialize(
                oldInstancesJsonFilePath,
                oldInstances);
        }

        public void OpenOutputFiles()
        {
            /// Inputs.


            /// Run.
            var instanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            var filePathsToShow = new List<string>(instanceVarietyNames.Length);

            foreach (var instanceVarietyName in instanceVarietyNames)
            {
                var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

                var cloudFilePath = Instances.PathOperator.GetFilePath(
                    DirectoryPaths.Instance.CloudOutputDirectoryPath,
                    fileName);

                filePathsToShow.Add(cloudFilePath);
            }

            Instances.NotepadPlusPlusOperator.Open(
                filePathsToShow);
        }

        public void CopyFilesToCloudSharedDirectory()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var instanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            foreach (var instanceVarietyName in instanceVarietyNames)
            {
                var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

                var sourceFilePath = Instances.PathOperator.GetFilePath(
                    datedOutputDirectoryPath,
                    fileName);

                var destinationFilePath = Instances.PathOperator.GetFilePath(
                    DirectoryPaths.Instance.CloudOutputDirectoryPath,
                    fileName);

                F0000.FileSystemOperator.Instance.CopyFile(
                    sourceFilePath,
                    destinationFilePath);
            }
        }

        public void OutputInstanceSpecificFiles()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var instancesJsonFilePath = Instances.FilePathProvider.Get_InstancesJsonFilePath(datedOutputDirectoryPath);

            var instances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(instancesJsonFilePath);
            var instancesByVariety = instances
                .GroupBy(x => x.InstanceVariety)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray());

            var instanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            foreach (var instanceVarietyName in instanceVarietyNames)
            {
                var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

                var outputFilePath = Instances.PathOperator.GetFilePath(
                    datedOutputDirectoryPath,
                    fileName);

                var instancesOfVariety = instancesByVariety.ContainsKey(instanceVarietyName)
                    ? instancesByVariety[instanceVarietyName]
                    : Array.Empty<N002.InstanceDescriptor>()
                    ;

                var title = instanceVarietyName;

                var lines = EnumerableHelper.From($"{title}, Count: {instancesOfVariety.Length}\n\n")
                    .Append(instancesOfVariety
                        .GroupBy(x => x.ProjectFilePath)
                        .OrderAlphabetically(x => x.Key)
                        .SelectMany(xGroup => EnumerableHelper.From($"{xGroup.Key}:")
                            .Append(xGroup
                                // Order by the identity name.
                                .OrderAlphabetically(x => x.IdentityName)
                                // But output the parameter named identity name.
                                .Select(x => $"\t{x.ParameterNamedIdentityName}")
                                .Append(Instances.Strings.Empty))));

                Instances.FileOperator.WriteAllLines_Synchronous(
                    outputFilePath,
                    lines);
            }
        }

        public void SummarizeProcessing()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var projectsListTextFilePath = Instances.FilePathProvider.Get_ProjectsListTextFilePath(datedOutputDirectoryPath);
            var buildProblemProjectsFilePath = Instances.FilePathProvider.Get_BuildProblemProjectsTextFilePath(datedOutputDirectoryPath);
            var processingProblemProjectsFilePath = Instances.FilePathProvider.Get_ProcessingProblemProjectsTextFilePath(datedOutputDirectoryPath);

            var instancesJsonFilePath = Instances.FilePathProvider.Get_InstancesJsonFilePath(datedOutputDirectoryPath);

            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(projectsListTextFilePath);
            var buildProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(buildProblemProjectsFilePath);
            var processingProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(processingProblemProjectsFilePath);

            var builtProjectFilePaths = projectFilePaths.Except(buildProblemProjectFilePaths).Now();
            var processedProjectFilePaths = builtProjectFilePaths.Except(processingProblemProjectFilePaths).Now();

            var instances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(instancesJsonFilePath);

            var now = Instances.NowOperator.GetNow();

            var summaryLines = F0000.EnumerableOperator.Instance.From($"{F0000.DateOperator.Instance.ToString_YYYYMMDD_HHMMSS(now)}: As-of")
                .Append(
                    $"({projectFilePaths.Length} / {builtProjectFilePaths.Length} / {processedProjectFilePaths.Length}): Projects, built projects, processed projects",
                    $"{instances.Length}: total instances");

            var processingSummaryFilePath = Instances.FilePathProvider.Get_ProcessingSummaryTextFilePath(datedOutputDirectoryPath);

            Instances.FileOperator.WriteLines_Synchronous(
                processingSummaryFilePath,
                summaryLines);

            Instances.NotepadPlusPlusOperator.Open(
                processingSummaryFilePath);
        }

        public void ProcessBuiltProjects()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var projectFileTuplesJsonFilePath = Instances.FilePathProvider.Get_ProjectFileTuplesJsonFilePath(datedOutputDirectoryPath);

            var projectFileTuples = Instances.JsonOperator.Deserialize_Synchronous<ProjectFilesTuple[]>(
                projectFileTuplesJsonFilePath);

            var processingProblemsFilePath = Instances.FilePathProvider.Get_ProcessingProblemsTextFilePath(datedOutputDirectoryPath);
            var processingProblemProjectsFilePath = Instances.FilePathProvider.Get_ProcessingProblemProjectsTextFilePath(datedOutputDirectoryPath);

            var instancesJsonFilePath = Instances.FilePathProvider.Get_InstancesJsonFilePath(datedOutputDirectoryPath);

            Instances.LoggingOperator.InConsoleLoggerContext_Synchronous(
                nameof(ProcessBuiltProjects),
                logger =>
                {
                    var instances = new List<N002.InstanceDescriptor>();

                    var processingProblemTextsByProjectFilePath = new Dictionary<string, string>();

                    var projectCounter = 1; // Start at 1.
                    var projectCount = projectFileTuples.Length;

                    foreach (var tuple in projectFileTuples)
                    {
                        logger.LogInformation("Processing project ({projectCounter} / {projectCount}):\n\t{projectFile}",
                            projectCounter++,
                            projectCount,
                            tuple.ProjectFilePath);

                        var assemblyFileExists = Instances.FileSystemOperator.FileExists(
                                tuple.AssemblyFilePath);

                        if (!assemblyFileExists)
                        {
                            processingProblemTextsByProjectFilePath.Add(
                                tuple.ProjectFilePath,
                                "No assembly file.");

                            logger.LogInformation("No assembly file to process, build failed.");

                            continue;
                        }

                        try
                        {
                            var currentInstances = Operations.Instance.ProcessAssemblyFile(
                                tuple.ProjectFilePath,
                                tuple.AssemblyFilePath,
                                tuple.DocumentationFilePath);

                            instances.AddRange(currentInstances);

                            logger.LogInformation("Processed project.");
                        }
                        catch (Exception ex)
                        {
                            processingProblemTextsByProjectFilePath.Add(
                                tuple.ProjectFilePath,
                                ex.Message);
                        }
                    }

                    Instances.Operations.WriteProblemProjectsFile(
                        processingProblemsFilePath,
                        processingProblemTextsByProjectFilePath);

                    Instances.FileOperator.WriteLines_Synchronous(
                        processingProblemProjectsFilePath,
                        processingProblemTextsByProjectFilePath.Keys
                            .OrderAlphabetically());

                    Instances.JsonOperator.Serialize(
                       instancesJsonFilePath,
                       instances);
                });

            Instances.NotepadPlusPlusOperator.Open(
                processingProblemsFilePath,
                processingProblemProjectsFilePath,
                instancesJsonFilePath);
        }

        public void CreateProjectFileTuples()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var projectsListTextFilePath = Instances.FilePathProvider.Get_ProjectsListTextFilePath(datedOutputDirectoryPath);
            var buildProblemProjectsFilePath = Instances.FilePathProvider.Get_BuildProblemProjectsTextFilePath(datedOutputDirectoryPath);

            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(projectsListTextFilePath);
            var buildProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(buildProblemProjectsFilePath);

            var buildSuccessProjectFilePaths = projectFilePaths.Except(buildProblemProjectFilePaths).Now();

            var projectFileTuples = buildSuccessProjectFilePaths
                .Select(projectFilePath =>
                {
                    var assemblyFilePath = Instances.FilePathProvider.Get_PublishDirectoryOutputAssemblyFilePath(projectFilePath);

                    var documentationFilePath = F0040.ProjectPathsOperator.Instance.GetDocumentationFilePath_ForAssemblyFilePath(assemblyFilePath);

                    var projectFilesTuple = new ProjectFilesTuple
                    {
                        ProjectFilePath = projectFilePath,
                        AssemblyFilePath = assemblyFilePath,
                        DocumentationFilePath = documentationFilePath,
                    };

                    return projectFilesTuple;
                })
                .Now();

            // Write project file tuples file.
            var projectFileTuplesJsonFilePath = Instances.FilePathProvider.Get_ProjectFileTuplesJsonFilePath(datedOutputDirectoryPath);

            Instances.JsonOperator.Serialize(
                projectFileTuplesJsonFilePath,
                projectFileTuples);

            Instances.NotepadPlusPlusOperator.Open(projectFileTuplesJsonFilePath);
        }

        /// <summary>
        /// Use the list of project file paths in the dated output directory (from <see cref="GetAllProjectFilePaths_OutputToDatedDirectory"/>), build all project files and output build problem projects to file.
        /// </summary>
        public void BuildProjectFilePaths()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();
            // True, if you want to spend the time to rebuild failed builds in order to collect build errors during this run.
            var rebuildFailedBuildsToCollectErrors = true;


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var projectsListTextFilePath = Instances.FilePathProvider.Get_ProjectsListTextFilePath(datedOutputDirectoryPath);

            var buildProblemsFilePath = Instances.FilePathProvider.Get_BuildProblemsTextFilePath(datedOutputDirectoryPath);
            var buildProblemProjectsFilePath = Instances.FilePathProvider.Get_BuildProblemProjectsTextFilePath(datedOutputDirectoryPath);

            F0035.LoggingOperator.Instance.InConsoleLoggerContext_Synchronous(
                nameof(GetAllProjectFilePaths),
                logger =>
                {
                    var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(projectsListTextFilePath);

                    var buildProblemTextsByProjectFilePath = new Dictionary<string, string>();

                    var projectCounter = 1; // Start at 1.
                    var projectCount = projectFilePaths.Length;

                    foreach (var projectFilePath in projectFilePaths)
                    {
                        logger.LogInformation("Building project ({projectCounter} / {projectCount}):\n\t{projectFilePath}",
                            projectCounter++,
                            projectCount,
                            projectFilePath);

                        var projectDirectoryPath = Instances.ProjectPathsOperator.GetProjectDirectoryPath(projectFilePath);

                        var shouldBuildProject = Instances.Operations.ShouldBuildProject(
                            projectFilePath,
                            rebuildFailedBuildsToCollectErrors,
                            logger);

                        if (shouldBuildProject)
                        {
                            // Clear the publish directory and publish (build), and not any problems.
                            Instances.Operations.BuildProject(
                                projectFilePath,
                                logger,
                                buildProblemTextsByProjectFilePath);
                        }
                        else
                        {
                            // See if a prior attempt to build the project failed, and not the failure.
                            var hasOutputAssembly = Instances.FileSystemOperator.Has_OutputAssembly(projectFilePath);
                            if (!hasOutputAssembly)
                            {
                                var buildProblemText = "Prior builds failed, and option to rebuild prior failed builds to collect errors was not true.";

                                buildProblemTextsByProjectFilePath.Add(
                                    projectFilePath,
                                    buildProblemText);
                            }
                        }
                    }

                    // Write build problems file.
                    Operations.Instance.WriteProblemProjectsFile(
                        buildProblemsFilePath,
                        buildProblemTextsByProjectFilePath);

                    // Write build problem projects file.
                    Instances.FileOperator.WriteLines(
                        buildProblemProjectsFilePath,
                        buildProblemTextsByProjectFilePath.Keys
                            .OrderAlphabetically());
                });

            Instances.NotepadPlusPlusOperator.Open(
                buildProblemsFilePath,
                buildProblemProjectsFilePath);
        }

        /// <summary>
        /// Determines the list of all project files, and outputs them to today's dated output directory path.
        /// </summary>
        public void GetAllProjectFilePaths_OutputToDatedDirectory()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            // Output project paths to current run date's directory.
            var projectsListTextFilePath = Instances.FilePathProvider.Get_ProjectsListTextFilePath(
                datedOutputDirectoryPath);

            F0035.LoggingOperator.Instance.InConsoleLoggerContext_Synchronous(
                nameof(GetAllProjectFilePaths),
                logger =>
                {
                    var repositoriesDirectoryPaths = Instances.RepositoriesDirectoryPaths.AllOfMine;

                    var projectFilePaths = Instances.FileSystemOperator.GetAllProjectFilePaths_FromRepositoriesDirectoryPaths(
                        repositoriesDirectoryPaths,
                        logger.ToTextOutput())
                        .OrderAlphabetically()
                        .Now();

                    Instances.FileOperator.WriteLines_Synchronous(
                        projectsListTextFilePath,
                        projectFilePaths);
                });

            Instances.NotepadPlusPlusOperator.Open(projectsListTextFilePath);
        }

        public void BuildAllProjectFilePaths_AndProcessAssemblies()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();
            // True, if you want to spend the time to rebuild failed builds in order to collect build errors during this run.
            var rebuildFailedBuildsToCollectErrors = true;


            /// Run.
            var outputDirectoryPath = Instances.DirectoryPaths.OutputDirectoryPath;

            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedChildDirectoryPath(
                outputDirectoryPath,
                date);

            F0035.LoggingOperator.Instance.InConsoleLoggerContext_Synchronous(
                nameof(GetAllProjectFilePaths),
                logger =>
                {
                    var repositoriesDirectoryPaths = Z0022.RepositoriesDirectoryPaths.Instance.AllOfMine;

                    var projectFilePaths = F0082.FileSystemOperator.Instance.GetAllProjectFilePaths_FromRepositoriesDirectoryPaths(
                        repositoriesDirectoryPaths,
                        logger.ToTextOutput())
                        .OrderAlphabetically()
                        .Now();

                    // Output project paths to current run's directory.
                    var projectsListTextFilePath = F0002.PathOperator.Instance.GetFilePath(
                        datedOutputDirectoryPath,
                        FileNames.Instance.ProjectsListTextFileName);

                    F0000.FileOperator.Instance.WriteLines_Synchronous(
                        projectsListTextFilePath,
                        F0000.EnumerableOperator.Instance.From($"{projectFilePaths.Length}: projects count")
                            .AppendRange(projectFilePaths));

                    var buildProblemTextsByProjectFilePath = new Dictionary<string, string>();
                    var projectFileTuples = new List<ProjectFilesTuple>();
                    var builtProjects = new List<string>();

                    var projectCounter = 1; // Start at 1.
                    var projectCount = projectFilePaths.Length;

                    foreach (var projectFilePath in projectFilePaths)
                    {
                        logger.LogInformation("Building project ({projectCounter} / {projectCount}):\n\t{projectFilePath}",
                            projectCounter++,
                            projectCount,
                            projectFilePath);

                        var projectDirectoryPath = Instances.ProjectPathsOperator.GetProjectDirectoryPath(projectFilePath);

                        var projectFilesLastModifiedTime = Instances.FileSystemOperator.GetLastModifiedTime_ForDirectory_Local(
                            projectDirectoryPath,
                            Instances.DirectoryNameOperator.IsNotBinariesOrObjectsDirectory);

                        var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

                        var shouldBuildProject = Instances.Operations.ShouldBuildProject(
                            projectFilePath,
                            rebuildFailedBuildsToCollectErrors,
                            logger);

                        if (shouldBuildProject)
                        {
                            // Clear the publish directory and publish (build), and not any problems.
                            Instances.Operations.BuildProject(
                                projectFilePath,
                                logger,
                                buildProblemTextsByProjectFilePath);
                        }
                        else
                        {
                            // See if a prior attempt to build the project failed, and not the failure.
                            var hasOutputAssembly = Instances.FileSystemOperator.Has_OutputAssembly(projectFilePath);
                            if (!hasOutputAssembly)
                            {
                                var buildProblemText = "Prior builds failed, and option to rebuild prior failed builds to collect errors was not true.";

                                buildProblemTextsByProjectFilePath.Add(
                                    projectFilePath,
                                    buildProblemText);
                            }
                        }

                        // Output the tuple of project file related paths.
                        var assemblyFilePath = Instances.FilePathProvider.Get_PublishDirectoryOutputAssemblyFilePath(projectFilePath);

                        var documentationFilePath = F0040.ProjectPathsOperator.Instance.GetDocumentationFilePath_ForAssemblyFilePath(
                            assemblyFilePath);

                        var projectFilesTuple = new ProjectFilesTuple
                        {
                            ProjectFilePath = projectFilePath,
                            AssemblyFilePath = assemblyFilePath,
                            DocumentationFilePath = documentationFilePath,
                        };

                        projectFileTuples.Add(projectFilesTuple);
                    }

                    // Write problem projects file.
                    var problemProjectsFilePath = Instances.PathOperator.GetFilePath(
                        datedOutputDirectoryPath,
                        FileNames.Instance.BuildProblemProjectsTextFileName);

                    Operations.Instance.WriteProblemProjectsFile(
                        problemProjectsFilePath,
                        buildProblemTextsByProjectFilePath);

                    // Write project file tuples file.
                    var projectFileTuplesJsonFilePath = Instances.PathOperator.GetFilePath(
                        datedOutputDirectoryPath,
                        FileNames.Instance.ProjectFileTuplesJsonFileName);

                    Instances.JsonOperator.Serialize(
                        projectFileTuplesJsonFilePath,
                        projectFileTuples);

                    /// Process all project assemblies.
                    var instances = new List<N002.InstanceDescriptor>();

                    var processedProjects = new List<string>();
                    var processingProblemProjects = new Dictionary<string, string>();

                    projectCounter = 1;

                    foreach (var tuple in projectFileTuples)
                    {
                        logger.LogInformation("Processing project ({projectCounter} / {projectCount}):\n\t{projectFile}",
                                projectCounter++,
                                projectCount,
                                tuple.ProjectFilePath);

                        if (!F0000.FileSystemOperator.Instance.FileExists(
                            tuple.AssemblyFilePath))
                        {
                            processingProblemProjects.Add(
                                tuple.ProjectFilePath,
                                "No assembly file.");

                            logger.LogInformation("No assembly file to process, build failed.");

                            continue;
                        }

                        try
                        {
                            var currentInstances = Operations.Instance.ProcessAssemblyFile(
                                tuple.ProjectFilePath,
                                tuple.AssemblyFilePath,
                                tuple.DocumentationFilePath);

                            instances.AddRange(currentInstances);

                            processedProjects.Add(tuple.ProjectFilePath);

                            logger.LogInformation("Processed project.");
                        }
                        catch (Exception ex)
                        {
                            processingProblemProjects.Add(
                                tuple.ProjectFilePath,
                                ex.Message);
                        }
                    }

                    // Output files.
                    var problemProcessingProjectsFilePath = F0002.PathOperator.Instance.GetFilePath(
                        datedOutputDirectoryPath,
                        FileNames.Instance.ProblemProcessingProjectsTextFileName);

                    Operations.Instance.WriteProblemProjectsFile(
                        problemProcessingProjectsFilePath,
                        processingProblemProjects);

                    var processedProjectFilePath = F0002.PathOperator.Instance.GetFilePath(
                        datedOutputDirectoryPath,
                        FileNames.Instance.ProcessedProjectsTextFileName);

                    F0000.FileOperator.Instance.WriteLines_Synchronous(
                        processedProjectFilePath,
                        processedProjects);

                    // Output the instances.
                    var instancesJsonFilePath = F0002.PathOperator.Instance.GetFilePath(
                        datedOutputDirectoryPath,
                        FileNames.Instance.InstancesJsonFileName);

                    F0032.JsonOperator.Instance.Serialize(
                       instancesJsonFilePath,
                       instances);

                    // Summarize.
                    var now = Instances.NowOperator.GetNow();

                    var summaryLines = F0000.EnumerableOperator.Instance.From($"{F0000.DateOperator.Instance.ToString_YYYYMMDD_HHMMSS(now)}: As-of")
                        .Append(
                            $"({projectFilePaths.Length} / {projectFileTuples.Count} / {processedProjects.Count}): Projects, built projects, processed projects",
                            $"{instances.Count}: total instances")
                        ;

                    var summaryFilePath = F0002.PathOperator.Instance.GetFilePath(
                        datedOutputDirectoryPath,
                        FileNames.Instance.SummaryTextFileName);

                    F0000.FileOperator.Instance.WriteLines_Synchronous(
                        summaryFilePath,
                        summaryLines);

                    /// Show output files.
                    F0033.NotepadPlusPlusOperator.Instance.Open(
                        projectsListTextFilePath,
                        problemProjectsFilePath,
                        projectFileTuplesJsonFilePath,
                        instancesJsonFilePath,
                        problemProcessingProjectsFilePath,
                        processedProjectFilePath,
                        summaryFilePath);
                });
        }

        public void GetAllProjectFilePaths()
        {
            F0035.LoggingOperator.Instance.InConsoleLoggerContext_Synchronous(
                nameof(GetAllProjectFilePaths),
                logger =>
                {
                    var repositoriesDirectoryPaths = Z0022.RepositoriesDirectoryPaths.Instance.AllOfMine;

                    var projectFilePaths = F0082.FileSystemOperator.Instance.GetAllProjectFilePaths_FromRepositoriesDirectoryPaths(
                        repositoriesDirectoryPaths,
                        logger.ToTextOutput())
                        .OrderAlphabetically()
                        .Now();

                    F0033.NotepadPlusPlusOperator.Instance.WriteLinesAndOpen(
                        Z0015.FilePaths.Instance.OutputTextFilePath,
                        projectFilePaths);
                });
        }

        public void FindLatestModifiedFile()
        {
            /// Inputs.
            var directoryPath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.Q0000\source\R5T.Q0000";


            /// Run.
            var orderedFiles = F0000.FileSystemOperator.Instance.EnumerateFiles(
                directoryPath,
                directoryInfo =>
                {
                    var output = directoryInfo.Name != "bin" && directoryInfo.Name != "obj";
                    return output;
                })
                .OrderByDescending(x => x.LastWriteTimeUtc)
                .Now();

            var filePath = orderedFiles
                .First()
                .FullName;

            Console.WriteLine(filePath);
        }

        public void ProcessAssembly()
        {
            /// Inputs.
            var assemblyFilePath = @"C:\Temp\Publish\R5T.F0074\R5T.F0074.dll";
            var outputJsonFilePath = Z0015.FilePaths.Instance.OutputJsonFilePath;


            /// Run.
            var instances = Operations.Instance.ProcessAssemblyFile(
                assemblyFilePath);

            F0032.JsonOperator.Instance.Serialize(
                outputJsonFilePath,
                instances);

            F0033.NotepadPlusPlusOperator.Instance.Open(outputJsonFilePath);
        }

        public void OutputMethodName_WithParameterNames()
        {
            /// Inputs.
            var assemblyFilePath = FilePathProvider.Instance.Get_ExampleAssemblyFilePath();
            var typeName = Z0025.NamespacedTypeNames.Instance.R5T_Z0025_IFunctionality;
            var methodName = Z0025.MethodNames.Instance.Functionality01;


            /// Run.
            var documentationForAssemblyFilePath = F0040.ProjectPathsOperator.Instance.GetDocumentationFilePath_ForAssemblyFilePath(
                assemblyFilePath);

            var descriptor = F0018.ReflectionOperator.Instance.InAssemblyContext(
                assemblyFilePath,
                assembly =>
                {
                    var typeInfo = F0000.AssemblyOperator.Instance.GetType(
                        assembly,
                        typeName);

                    var methodInfo = F0000.TypeOperator.Instance.GetMethod_Declarared(
                        typeInfo,
                        methodName);

                    var methodIdentityName = F0017.F002.IdentityNameProvider.Instance.GetIdentityName(methodInfo);

                    var methodIdentityNameWithParameterNames = ParameterNamedIdentityNameProvider.Instance.GetParameterNamedIdentityName(methodInfo);

                    var documentationForMethod = Operations.Instance.GetDocumentationForMemberIdentityName(
                        documentationForAssemblyFilePath,
                        methodIdentityName);

                    var descriptor = new N001.InstanceDescriptor
                    {
                        IdentityName = methodIdentityName,
                        ParameterNamedIdentityName = methodIdentityNameWithParameterNames,
                        DescriptionXML = documentationForMethod,
                    };

                    return descriptor;
                });

            var lines = new[]
            {
                $"For:\n\t{methodName}: method name\n\t{typeName}: type name\n",
                $"{descriptor.DescriptionXML}\n",
                descriptor.ParameterNamedIdentityName,
                descriptor.IdentityName
            };

            F0033.NotepadPlusPlusOperator.Instance.WriteLinesAndOpen(
                Z0015.FilePaths.Instance.OutputTextFilePath,
                lines);
        }
    }
}
