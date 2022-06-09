using System;

using R5T.T0062;
using R5T.T0063;


namespace R5T.S0041
{
    public static partial class IServiceActionExtensions
    {
        public static IServiceAction<HostStartup> AddHostStartupAction(this IServiceAction _)
        {
            var output = _.New<HostStartup>(services => services.AddHostStartup());
        
            return output;
        }
    }
}