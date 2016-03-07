using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.CORE
{
    public class Message
    {
        private ActionResultType? _type;

        public List<string> Errors { get; set; }
        public List<string> Success { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Info { get; set; }
        public ActionResultType Type
        {
            get
            {
                if (_type == null)
                {
                    if (Errors.Count > 0)
                        _type = ActionResultType.Error;
                    else if (Success.Count > 0)
                        _type = ActionResultType.Success;
                    else if (Warnings.Count > 0)
                        _type = ActionResultType.Warning;
                    else
                        _type = ActionResultType.Info;
                }

                return _type.Value;
            }
        }

        public Message()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
            Info = new List<string>();
            Success = new List<string>();

            _type = null;
        }

        public string ToUser()
        {
            if (Errors.Count > 0)
            {
                _type = ActionResultType.Error;
                return string.Join("<br/>", Errors);
            }

            if (Success.Count > 0)
            {
                _type = ActionResultType.Success;
                return string.Join("<br/>", Success);
            }

            if (Warnings.Count > 0)
            {
                _type = ActionResultType.Warning;
                return string.Join("<br/>", Warnings);
            }
            
            _type = ActionResultType.Info;
            return string.Join("<br/>", Info);
        }

        public Tuple<ActionResultType, string> All()
        {
            throw new NotImplementedException();
        }
    }
}
