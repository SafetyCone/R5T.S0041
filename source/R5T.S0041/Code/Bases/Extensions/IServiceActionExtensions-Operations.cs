using System;

using R5T.D0105;
using R5T.L0017.D001;
using R5T.T0062;
using R5T.T0063;


namespace R5T.S0041
{
    public static partial class IServiceActionExtensions
    {
        /// <summary>
        /// Adds the <see cref="O002_SurveyForDraftFunctionality"/> operation as a <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O002_SurveyForDraftFunctionality> AddO002_SurveyForDraftFunctionalityAction(this IServiceAction _,
            IServiceAction<O001_SurveyForFunctionality> o001_SurveyForFunctionalityAction)
        {
            var serviceAction = _.New<O002_SurveyForDraftFunctionality>(services => services.AddO002_SurveyForDraftFunctionality(
                o001_SurveyForFunctionalityAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="O001_SurveyForFunctionality"/> operation as a <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O001_SurveyForFunctionality> AddO001_SurveyForFunctionalityAction(this IServiceAction _,
            IServiceAction<ILoggerUnbound> loggerUnboundAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction)
        {
            var serviceAction = _.New<O001_SurveyForFunctionality>(services => services.AddO001_SurveyForFunctionality(
                loggerUnboundAction,
                notepadPlusPlusOperatorAction));

            return serviceAction;
        }
    }
}