using System;
using System.Collections.Generic;


namespace R5T.S0041
{
    public class DatedInstancesSummary
    {
        public DateTime AsOfDate { get; set; }
        public DateTime PriorComparisonDate { get; set; }

        public Dictionary<string, CountChange> InstanceVarietyCountsByVarietyName { get; set; }
    }
}
