using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.D0088.I0002;
using R5T.T0063;


namespace R5T.S0041
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHostStartup(this IServiceCollection services)
        {
            var dependencyServiceActions = new DependencyServiceActionAggregation();

            services.AddHostStartup<HostStartup>(dependencyServiceActions)
                // Add services required by HostStartup, but not by HostStartupBase.
                ;

            return services;
        }
    }
}