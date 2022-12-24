using System;

using System.Collections.Generic;


namespace R5T.S0041
{
    public class FunctionalityDescriptorDataIdentityEqualityComparer : IEqualityComparer<InstanceDescriptor>
    {
        #region Static

        public static FunctionalityDescriptorDataIdentityEqualityComparer Instance { get; } = new();

        #endregion


        public bool Equals(InstanceDescriptor x, InstanceDescriptor y)
        {
            var output = true
                && x.IdentityName == y.IdentityName
                && x.ProjectFilePath == y.ProjectFilePath
                ;

            return output;
        }

        public int GetHashCode(InstanceDescriptor obj)
        {
            var output = HashCode.Combine(
                obj.IdentityName,
                obj.ProjectFilePath);

            return output;
        }
    }
}
