using System;

using R5T.T0142;


namespace R5T.S0041
{
    [DraftDataTypeMarker]
    public class ProjectFilesTuple
    {
        public string ProjectFilePath { get; set; }
        public string AssemblyFilePath { get; set; }
        public string DocumentationFilePath { get; set; }
    }
}
