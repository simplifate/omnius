using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.CORE
{
    public class OmniusMultipleException : Exception
    {
        public OmniusMultipleException(params Exception[] exceptions) : base(string.Join("<br/>", exceptions.Select(ex => ex.Message)))
        {
            AllExceptions = exceptions.ToList();
        }
        public OmniusMultipleException(List<Exception> exceptions) : base(string.Join("<br/>", exceptions.Select(ex => ex.Message)))
        {
            AllExceptions = exceptions;
        }

        public List<Exception> AllExceptions { get; set; }
    }
}
