using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

using Microsoft.Extensions.Logging;


namespace R5T.S0041
{
    public partial interface IConstruction
    {
        public void SendResultsEmail()
        {
            /// Inputs.
            var date = Instances.NowOperator.Get_Today();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var newAndOldSummaryFilePath = Instances.FilePathProvider.Get_NewAndOldSummaryTextFilePath(datedOutputDirectoryPath);
            var dateComparisonSummaryFilePath = Instances.FilePathProvider.Get_DateComparisonSummaryTextFilePath(datedOutputDirectoryPath);
            var processingSummaryFilePath = Instances.FilePathProvider.Get_ProcessingSummaryTextFilePath(datedOutputDirectoryPath);

            var toAddresses = new[]
            {
                Instances.EmailAddresses.David_Gmail,
                Instances.EmailAddresses.Vedika_Gmail,
            };

            var today = Instances.DateOperator.GetToday();

            var subject = $"Instances Summary {Instances.DateOperator.ToString_YYYY_MM_DD_Dashed(today)}";

            var newAndOldSummaryLines = Instances.FileOperator.ActuallyReadAllLines_Synchronous(newAndOldSummaryFilePath);
            var dateComparisonSummaryLines = Instances.FileOperator.ActuallyReadAllLines_Synchronous(dateComparisonSummaryFilePath);
            var processingSummaryLines = Instances.FileOperator.ActuallyReadAllLines_Synchronous(processingSummaryFilePath);

            var bodyLines = Instances.EnumerableOperator.Empty<string>()
                .AppendRange(dateComparisonSummaryLines)
                .Append(Instances.Strings.Empty)
                .AppendRange(newAndOldSummaryLines)
                .Append(Instances.Strings.Empty)
                .AppendRange(processingSummaryLines)
                .Append($"\n\nSent by machine: {F0000.MachineNameOperator.Instance.GetMachineName()}");

            var body = Instances.StringOperator.Join(
                Environment.NewLine,
                bodyLines);

            var emailMessage = new MailMessage
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };

            toAddresses.ForEach(toAddress => emailMessage.To.Add(new MailAddress(toAddress)));

            Instances.EmailSender.SendEmail(emailMessage);
        }

        public void SummarizeNewAndOldInstances()
        {
            /// Inputs.
            var date = Instances.NowOperator.Get_Today();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var newInstancesJsonFilePath = Instances.FilePathProvider.Get_NewInstancesJsonFilePath(datedOutputDirectoryPath);
            var oldInstancesJsonFilePath = Instances.FilePathProvider.Get_OldInstancesJsonFilePath(datedOutputDirectoryPath);

            var newInstances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(newInstancesJsonFilePath);
            var oldInstances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(oldInstancesJsonFilePath);

            var varietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            var newInstanceNamesByVarietyName = newInstances
                .GroupBy(x => x.InstanceVariety)
                .OrderByNames(x => x.Key, varietyNames)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(x => x.ParameterNamedIdentityName).OrderAlphabetically().ToArray());

            var oldInstanceNamesByVarietyName = oldInstances
                .GroupBy(x => x.InstanceVariety)
                .OrderByNames(x => x.Key, varietyNames)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(x => x.ParameterNamedIdentityName).OrderAlphabetically().ToArray());

            var addedLine = newInstanceNamesByVarietyName.Any()
                ? Instances.Strings.Empty
                : " <none>"
                ;

            var removedLine = oldInstanceNamesByVarietyName.Any()
                ? Instances.Strings.Empty
                : " <none>"
                ;

            static IEnumerable<string> GetVarietyLines(KeyValuePair<string, string[]> xPair)
            {
                var lines = Instances.EnumerableOperator.From($"{xPair.Key} ({xPair.Value.Length}):")
                    .AppendRange(xPair.Value
                        .Select(x => $"\t{x}"))
                    .Append(Instances.Strings.Empty)
                    ;

                return lines;
            }

            var lines = Instances.EnumerableOperator.From("Changes:\n")
                .AppendRange(Instances.EnumerableOperator.From($"Added:{addedLine}\n")
                    .AppendRange(newInstanceNamesByVarietyName
                        .SelectMany(xPair => GetVarietyLines(xPair))))
                .AppendRange(Instances.EnumerableOperator.From($"Removed:{removedLine}\n")
                    .AppendRange(oldInstanceNamesByVarietyName
                        .SelectMany(xPair => GetVarietyLines(xPair))))
                ;

            var outputFilePath = Instances.FilePathProvider.Get_NewAndOldSummaryTextFilePath(datedOutputDirectoryPath);

            FileHelper.WriteAllLines_Synchronous(
                outputFilePath,
                lines);

            Instances.NotepadPlusPlusOperator.Open(outputFilePath);
        }

        public void SummarizeDatesComparison()
        {
            /// Inputs.
            var date = Instances.NowOperator.Get_Today();
            var priorDate = Instances.Operations.GetPriorComparisonDate(date);


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.GetDatedOutputDirectoryPath(date);

            var instancesJsonFilePath = Instances.FilePathProvider.Get_InstancesJsonFilePath(datedOutputDirectoryPath);
            var newInstancesJsonFilePath = Instances.FilePathProvider.Get_NewInstancesJsonFilePath(datedOutputDirectoryPath);
            var oldInstancesJsonFilePath = Instances.FilePathProvider.Get_OldInstancesJsonFilePath(datedOutputDirectoryPath);

            var instances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(instancesJsonFilePath);
            var newInstances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(newInstancesJsonFilePath);
            var oldInstances = Instances.JsonOperator.Deserialize_Synchronous<N002.InstanceDescriptor[]>(oldInstancesJsonFilePath);

            var varietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            Dictionary<string, int> GetCountsByVarietyNames(
                IEnumerable<string> varietyNames,
                IEnumerable<N002.InstanceDescriptor> instances)
            {
                var instanceCountsByVarietyName = instances
                    .GroupBy(x => x.InstanceVariety)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Count());

                var output = varietyNames
                    .Select(varietyName =>
                    {
                        var count = instanceCountsByVarietyName.ContainsKey(varietyName)
                            ? instanceCountsByVarietyName[varietyName]
                            : 0
                            ;

                        return (varietyName, count);
                    })
                    .ToDictionary(
                        x => x.varietyName,
                        x => x.count);

                return output;
            }

            Dictionary<string, (int instanceCount, int newInstanceCount, int oldInstanceCount)> GetAllCountsByVarietyNames(
                IEnumerable<string> varietyNames,
                IEnumerable<N002.InstanceDescriptor> instances,
                IEnumerable<N002.InstanceDescriptor> newInstances,
                IEnumerable<N002.InstanceDescriptor> oldInstances)
            {
                var instanceCounts = GetCountsByVarietyNames(
                    varietyNames,
                    instances);

                var newInstanceCounts = GetCountsByVarietyNames(
                    varietyNames,
                    newInstances);

                var oldInstanceCounts = GetCountsByVarietyNames(
                    varietyNames,
                    oldInstances);

                var output = varietyNames
                    .ToDictionary(
                        x => x,
                        x => (instanceCounts[x], newInstanceCounts[x], oldInstanceCounts[x]));

                return output;
            }

            var allCountsByVarietyName = GetAllCountsByVarietyNames(
                varietyNames,
                instances,
                newInstances,
                oldInstances);

            var lines =
                new[]
                {
                    "Instances Summary",
                    $"\n{Instances.DateOperator.ToString_YYYYMMDD(date)}: as-of date",
                    $"{Instances.DateOperator.ToString_YYYYMMDD(priorDate)}: prior comparison date",
                    "",
                }
                .Append(varietyNames
                    .SelectMany(x =>
                    {
                        var (instanceCount, newInstanceCount, oldInstanceCount) = allCountsByVarietyName[x];

                        var output = new[]
                        {
                            $"{instanceCount,5}: {x}, (+{newInstanceCount}, -{oldInstanceCount})"
                        };

                        return output;
                    }));

            var outputFilePath = Instances.FilePathProvider.Get_DateComparisonSummaryTextFilePath(datedOutputDirectoryPath);

            FileHelper.WriteAllLines_Synchronous(
                outputFilePath,
                lines);

            Instances.NotepadPlusPlusOperator.Open(outputFilePath);
        }
    }
}
