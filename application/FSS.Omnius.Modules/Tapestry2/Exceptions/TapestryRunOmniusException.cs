using System;

namespace FSS.Omnius.Modules.Tapestry2
{
    public class TapestryRunOmniusException : Exception
    {
        public TapestryRunOmniusException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
