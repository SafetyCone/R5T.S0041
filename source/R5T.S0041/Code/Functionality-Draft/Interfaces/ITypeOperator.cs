using System;
using System.Linq;
using System.Reflection;

using R5T.T0132;


namespace R5T.S0041
{
    [FunctionalityMarker]
    public interface ITypeOperator : IFunctionalityMarker
    {
        /// <summary>
        /// Determines whether the method is a property get or set method.
        /// </summary>
        public bool IsPropertyMethod(MethodInfo methodInfo)
        {
            var output = true
                // All property methods have special names.
                && methodInfo.IsSpecialName
                && methodInfo.DeclaringType.GetProperties()
                    .Any(property => false
                        || property.GetGetMethod() == methodInfo
                        || property.GetSetMethod() == methodInfo);

            return output;
        }
    }
}
