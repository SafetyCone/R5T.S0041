using System;

using System.Collections.Generic;


namespace R5T.S0041
{
    public class FunctionalityDescriptorDataIdentityEqualityComparer : IEqualityComparer<FunctionalityDescriptor>
    {
        #region Static

        public static FunctionalityDescriptorDataIdentityEqualityComparer Instance { get; } = new();

        #endregion


        public bool Equals(FunctionalityDescriptor x, FunctionalityDescriptor y)
        {
            var output = true
                && x.MethodIdentityName == y.MethodIdentityName
                && x.ProjectFilePath == y.ProjectFilePath
                ;

            return output;
        }

        public int GetHashCode(FunctionalityDescriptor obj)
        {
            var output = HashCode.Combine(
                obj.MethodIdentityName,
                obj.ProjectFilePath);

            return output;
        }
    }
}
