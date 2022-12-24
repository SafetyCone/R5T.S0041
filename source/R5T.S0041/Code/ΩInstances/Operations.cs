using System;


namespace R5T.S0041
{
    public class Operations : IOperations
    {
        #region Infrastructure

        public static IOperations Instance { get; } = new Operations();

        private Operations()
        {
        }

        #endregion
    }
}


namespace R5T.S0041.Functionalities
{
    public class Operations : IOperations
    {
        #region Infrastructure

        public static IOperations Instance { get; } = new Operations();

        private Operations()
        {
        }

        #endregion
    }
}
