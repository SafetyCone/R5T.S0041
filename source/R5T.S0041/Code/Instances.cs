using System;

using R5T.T0062;
using R5T.T0070;


namespace R5T.S0041
{
    public static class Instances
    {
        public static IHost Host { get; } = T0070.Host.Instance;
        public static IServiceAction ServiceAction { get; } = T0062.ServiceAction.Instance;
    }
}