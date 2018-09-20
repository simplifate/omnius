using System;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace FSS.Omnius.Modules.Persona
{
    [Serializable]
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException()
        {
        }

        public NotAuthorizedException(string message) : base(message)
        {
        }

        public NotAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class LoggedOff : Exception
    {
        public LoggedOff(string message) : base(message)
        {
        }
    }
    
    public class Http403Result : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = 403;
        }
    }
}