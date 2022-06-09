using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.D0105;
using R5T.L0017.D001;
using R5T.T0063;


namespace R5T.S0041
{
    public static partial class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="O002_SurveyForDraftFunctionality"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO002_SurveyForDraftFunctionality(this IServiceCollection services,
            IServiceAction<O001_SurveyForFunctionality> o001_SurveyForFunctionalityAction)
        {
            services
                .Run(o001_SurveyForFunctionalityAction)
                .AddSingleton<O002_SurveyForDraftFunctionality>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="O001_SurveyForFunctionality"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO001_SurveyForFunctionality(this IServiceCollection services,
            IServiceAction<ILoggerUnbound> loggerUnboundAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction)
        {
            services
                .Run(loggerUnboundAction)
                .Run(notepadPlusPlusOperatorAction)
                .AddSingleton<O001_SurveyForFunctionality>();

            return services;
        }
    }
}