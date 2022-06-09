using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using R5T.A0003;
using R5T.D0048.Default;
using R5T.D0081.I001;
using R5T.D0088.I0002;
using R5T.D0094.I001;
using R5T.D0095.I001;
using R5T.D0105.I001;
using R5T.Magyar;
using R5T.Ostrogothia.Rivet;
using R5T.T0063;

using IProvidedServiceActionAggregation = R5T.D0088.I0002.IProvidedServiceActionAggregation;
using IRequiredServiceActionAggregation = R5T.D0088.I0002.IRequiredServiceActionAggregation;
using ServicesPlatformRequiredServiceActionAggregation = R5T.A0003.RequiredServiceActionAggregation;


namespace R5T.S0041
{
    public class HostStartup : HostStartupBase
    {
        public override Task ConfigureConfiguration(IConfigurationBuilder configurationBuilder)
        {
            // Do nothing.
        
            return Task.CompletedTask;
        }

        protected override Task ConfigureServices(IServiceCollection services, IProvidedServiceActionAggregation providedServicesAggregation)
        {
            // Inputs.
            var executionSynchronicityProviderAction = Instances.ServiceAction.AddConstructorBasedExecutionSynchronicityProviderAction(Synchronicity.Synchronous);

            var organizationProviderAction = Instances.ServiceAction.AddOrganizationProviderAction(); // Rivet organization.

            var rootOutputDirectoryPathProviderAction = Instances.ServiceAction.AddConstructorBasedRootOutputDirectoryPathProviderAction(@"C:\Temp\Output");

            // Services platform.
            var servicesPlatformRequiredServiceActionAggregation = new ServicesPlatformRequiredServiceActionAggregation
            {
                ConfigurationAction = providedServicesAggregation.ConfigurationAction,
                ExecutionSynchronicityProviderAction = executionSynchronicityProviderAction,
                LoggerAction = providedServicesAggregation.LoggerAction,
                LoggerFactoryAction = providedServicesAggregation.LoggerFactoryAction,
                MachineMessageOutputSinkProviderActions = EnumerableHelper.Empty<IServiceAction<D0099.D002.IMachineMessageOutputSinkProvider>>(),
                MachineMessageTypeJsonSerializationHandlerActions = EnumerableHelper.Empty<IServiceAction<D0098.IMachineMessageTypeJsonSerializationHandler>>(),
                OrganizationProviderAction = organizationProviderAction,
                RootOutputDirectoryPathProviderAction = rootOutputDirectoryPathProviderAction,
            };

            var loggerUnboundAction = providedServicesAggregation.LoggerAction;

            var servicesPlatform = Instances.ServiceAction.AddProvidedServiceActionAggregation(
                servicesPlatformRequiredServiceActionAggregation);

            // Logging.
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddConsole(
                        servicesPlatform.LoggerSynchronicityProviderAction)
                    .AddFile(
                        servicesPlatform.LogFilePathProviderAction,
                        servicesPlatform.LoggerSynchronicityProviderAction)
                    ;
            });

            // Notepad++
            var notepadPlusPlusExecutableFilePathProviderAction = Instances.ServiceAction.AddHardCodedNotepadPlusPlusExecutableFilePathProviderAction();

            var notepadPlusPlusOperatorAction = Instances.ServiceAction.AddNotepadPlusPlusOperatorAction(
                //commandLineOperatorAction,
                servicesPlatform.BaseCommandLineOperatorAction,
                notepadPlusPlusExecutableFilePathProviderAction);


            // Add services here.
            //var serviceAction = Instances.ServiceAction.AddXAction();

            // Operations.
            // Level 00.
            var o001_SurveyForFunctionalityAction = Instances.ServiceAction.AddO001_SurveyForFunctionalityAction(
                loggerUnboundAction,
                notepadPlusPlusOperatorAction);

            // Level 01.
            var o002_SurveyForDraftFunctionalityAction = Instances.ServiceAction.AddO002_SurveyForDraftFunctionalityAction(
                o001_SurveyForFunctionalityAction);

            // Run.
            services.MarkAsServiceCollectonConfigurationStatement()
                .Run(servicesPlatform.ConfigurationAuditSerializerAction)
                .Run(servicesPlatform.ServiceCollectionAuditSerializerAction)
                // Operations.
                .Run(o001_SurveyForFunctionalityAction)
                .Run(o002_SurveyForDraftFunctionalityAction)
                ;

            return Task.CompletedTask;
        }

        protected override Task FillRequiredServiceActions(IRequiredServiceActionAggregation requiredServiceActions)
        {
            // Do nothing since none are required.
        
            return Task.CompletedTask;
        }
    }
}