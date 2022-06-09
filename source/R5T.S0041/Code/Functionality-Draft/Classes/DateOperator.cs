using System;


namespace R5T.S0041
{
    public class DateOperator : IDateOperator
    {
        #region Infrastructure

        public static DateOperator Instance { get; } = new();

        private DateOperator()
        {
        }

        #endregion
    }
}
