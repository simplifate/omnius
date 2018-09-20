using System;

namespace FSS.Omnius.Modules.Tapestry2
{
    public class TapestryAuthenticationOmniusException : Exception
    {
        public TapestryAuthenticationOmniusException(string message, string requiredRoles, string userRoles) : base(message)
        {
            RequiredRoles = requiredRoles;
            UserRoles = userRoles;
        }

        public string RequiredRoles { get; }
        public string UserRoles { get; }
    }
}
