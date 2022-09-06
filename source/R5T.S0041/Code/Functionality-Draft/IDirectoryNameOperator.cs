using System;

using R5T.T0132;


namespace R5T.S0041
{
    [DraftFunctionalityMarker]
    public interface IDirectoryNameOperator : IDraftFunctionalityMarker
    {
        public string GetDatedDirectoryName(DateTime dateTime)
        {
            var output = this.GetDirectoryName_YYYYMMDD(dateTime);
            return output;
        }

        public string GetDirectoryName_YYYYMMDD(DateTime dateTime)
        {
            var output = Instances.DateOperator.ToString_YYYYMMDD(dateTime);
            return output;
        }
    }
}
