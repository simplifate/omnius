using System;

namespace FSS.Omnius.Modules.Tapestry2
{
    public class TapestrySyntacticOmniusException : Exception
    {
        public TapestrySyntacticOmniusException(string message) : base(message)
        {
        }
        public TapestrySyntacticOmniusException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public TapestrySyntacticOmniusException(string message, string applicationName, string blockName, string workflowName) : base(message)
        {
            ApplicationName = applicationName;
            BlockName = blockName;
            WorkflowName = workflowName;
        }
        public TapestrySyntacticOmniusException(string message, string applicationName, string blockName, string workflowName, Exception innerException) : base(message, innerException)
        {
            ApplicationName = applicationName;
            BlockName = blockName;
            WorkflowName = workflowName;
        }
        public TapestrySyntacticOmniusException(string message, int wfItemId) : base(message)
        {
            WFItemId = wfItemId;
        }
        public TapestrySyntacticOmniusException(string message, int? wfItemId, Exception innerException) : base(message, innerException)
        {
            WFItemId = wfItemId;
        }

        public string ApplicationName { get; set; }
        public string BlockName { get; set; }
        public string WorkflowName { get; set; }
        public int? WFItemId { get; }
    }
}
