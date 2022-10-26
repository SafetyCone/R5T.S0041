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

using R5T.F0000;
using R5T.T0132;


namespace R5T.S0041
{
    [DraftFunctionalityMarker]
    public partial interface IOperations : IDraftFunctionalityMarker
    {
        public DateTime GetPriorComparisonDate(DateTime date)
        {
            var outputDirectoryPath = Instances.DirectoryPaths.OutputDirectoryPath;

            // Look through the names of directories in the output directory.
            var directoryPaths = Instances.FileSystemOperator.EnumerateAllChildDirectoryPaths(outputDirectoryPath);

            // Find directories whose names are YYYYMMDDs,
            var directoryDates = directoryPaths
                .Select(directoryPath =>
                {
                    var directoryName = Instances.PathOperator.GetDirectoryNameOfDirectoryPath(directoryPath);

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
            var canUseProjectFilesTuplesCache = useProjectFilesTuplesCache && Instances.FileSystemOperator.FileExists(projectFilesTuplesJsonFilePath);

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
            var fileNameStem = Instances.PathOperator.GetFileNameStem(targetFilePath);

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
                    var projectFileExists = Instances.FileSystemOperator.FileExists(tuple.ProjectFilePath);
                    if (!projectFileExists)
                    {
                        throw new Exception("Project file did not exist.");
                    }

                    var assemblyFileExists = Instances.FileSystemOperator.FileExists(tuple.AssemblyFilePath);
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

            var fileName = Instances.PathOperator.GetFileName(filePath);

            var fileNameStem = Instances.FileNameOperator.GetFileNameStem(fileName);
            var fileExtension = Instances.FileNameOperator.GetFileExtension(fileName);

            var datedFileNameStem = $"{fileNameStem}-{firstDateString} to {secondDateString}";
            var datedFileName = Instances.FileNameOperator.GetFileName(
                datedFileNameStem,
                fileExtension);

            var datedFilePath = Instances.PathOperator.GetFilePath(
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

            var fileName = Instances.PathOperator.GetFileName(filePath);

            var fileNameStem = Instances.FileNameOperator.GetFileNameStem(fileName);
            var fileExtension = Instances.FileNameOperator.GetFileExtension(fileName);

            var datedFileNameStem = $"{fileNameStem}-{dateString}";
            var datedFileName = Instances.FileNameOperator.GetFileName(
                datedFileNameStem,
                fileExtension);

            var datedFilePath = Instances.PathOperator.GetFilePath(
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
                DescriptionXML = descriptionXml
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
                DescriptionXML = descriptionXml
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
            var fileExists = Instances.FileSystemOperator.FileExists(instanceDescriptorJsonFilePath);
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
                .Append(projectFilePaths
                    .OrderAlphabetically());

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
                .OrderAlphabetically_OnlyIfDebug()
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

                        output.Add(memberIdentityName, documentationForMember);
                    }
                }
            }

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
                            var wasFound = memberNodeExists
                                ? WasFound.Found(memberNode.FirstNode.ToString())
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

                    var output = WasFound.FromArray(functionaityMethodNames);
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

                    var output = WasFound.FromArray(draftFunctionaityMethodNames);
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

                    var output = WasFound.FromArray(draftFunctionaityMethodNames);
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

            var output = WasFound.FromArray(draftFunctionalityMethods);
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

            var output = WasFound.FromArray(draftFunctionalityMethods);
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

            var output = WasFound.FromArray(draftFunctionalityTypes);
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

            var output = WasFound.FromArray(functionalityTypes);
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

                    var output = WasFound.FromArray(draftFunctionalityTypeNames);
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
