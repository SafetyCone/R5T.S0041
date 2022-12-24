using System;
using System.Extensions;

using R5T.T0131;


namespace R5T.S0041
{
    [ValuesMarker]
    public partial interface IFileExtensions : IValuesMarker,
        F0000.IFileExtensions,
        Z0010.IFileExtensions
    {
        public new string Dll => (this as Z0010.IFileExtensions).Dll;
    }
}
