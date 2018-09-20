using System;

namespace FSS.Omnius.Modules.Tapestry2
{
    public class TapestryLoadOmniusException : Exception
    {
        public TapestryLoadOmniusException(string message, LoadTarget target, Exception innerException) : base(message, innerException)
        {
            Target = target;
        }

        public LoadTarget Target { get; }

        public enum LoadTarget
        {
            Assembly,
            Block,
            Rule
        }
    }
}
