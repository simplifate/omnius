using System;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.CORE
{
    public class Message
    {
        private COREobject _core;
        private MessageType? _type;

        public List<string> Errors { get; set; }
        public List<string> Success { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Info { get; set; }
        public MessageType Type
        {
            get
            {
                if (_type == null)
                {
                    refreshType();
                }

                return _type.Value;
            }
        }

        public Message(COREobject core)
        {
            Errors = new List<string>();
            Warnings = new List<string>();
            Info = new List<string>();
            Success = new List<string>();

            _core = core;
            _type = null;
        }

        public void Join(Message message)
        {
            Errors.AddRange(message.Errors);
            Success.AddRange(message.Success);
            Warnings.AddRange(message.Warnings);
            Info.AddRange(message.Info);

            refreshType();
        }

        public string ToUser()
        {
            if (Errors.Count > 0)
            {
                _type = MessageType.Error;
                return string.Join("<br/>", Errors.Select(m => _core.Translator._(m)));
            }

            if (Success.Count > 0)
            {
                _type = MessageType.Success;
                return string.Join("<br/>", Success.Select(m => _core.Translator._(m)));
            }

            if (Warnings.Count > 0)
            {
                _type = MessageType.Warning;
                return string.Join("<br/>", Warnings.Select(m => _core.Translator._(m)));
            }

            _type = MessageType.Info;
            return string.Join("<br/>", Info.Select(m => _core.Translator._(m)));
        }
        public Tuple<MessageType, string> All()
        {
            throw new NotImplementedException();
        }

        private void refreshType()
        {
            if (Errors.Count > 0)
                _type = MessageType.Error;
            else if (Success.Count > 0)
                _type = MessageType.Success;
            else if (Warnings.Count > 0)
                _type = MessageType.Warning;
            else
                _type = MessageType.Info;
        }
    }

    public enum MessageType
    {
        Success,
        Info,
        Warning,
        Error,
        InProgress
    }
}
