using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace FSS.Omnius.Modules.CORE
{
    public class ModalProgressHandler<TSection>
    {
        const int MaxErrors = 10;
        const int MaxWarnings = 10;

        public ModalProgressHandler(Action<string> sendAction, string newLine = "<br/>")
        {
            _ajaxSendAction = sendAction;
            _newLine = newLine;
            _subSections = new Dictionary<TSection, Dictionary<string, SubSection>>();
        }

        private Action<string> _ajaxSendAction;

        private TSection _activeSection;
        private string _activeSubSection;

        private Dictionary<TSection, Dictionary<string, SubSection>> _subSections { get; set; }

        public void SetActiveSection(TSection section)
        {
            _activeSection = section;
            _activeSubSection = "";
        }
        public void SetActiveSubSection(string subSection)
        {
            _activeSubSection = subSection;
        }

        public void Section(TSection section, string message, MessageType? type = null)
        {
            _activeSection = section;
            _activeSubSection = "";

            SubSection sub;
            // create
            if (!_subSections.ContainsKey(_activeSection))
            {
                sub = new SubSection { Section = _activeSection, Name = _activeSubSection, Type = type ?? MessageType.Info, Message = message };
                _subSections[_activeSection] = new Dictionary<string, SubSection>();
                _subSections[_activeSection].Add(_activeSubSection, sub);
            }
            // replace
            else
            {
                sub = _subSections[_activeSection][_activeSubSection];
                sub.Message = message;
                sub.Type = type ?? sub.Type;
            }

            _send(sub);
        }
        public void SetMessage(string subSection = null, string message = null, MessageType? type = null, int? progressSteps = null)
        {
            _activeSubSection = subSection ?? _activeSubSection;

            SubSection sub;
            // create
            if (!_subSections[_activeSection].ContainsKey(_activeSubSection))
            {
                sub = new SubSection { Section = _activeSection, Name = _activeSubSection, Type = type ?? MessageType.Info, Message = message, MaxProgress = progressSteps ?? 0 };
                _subSections[_activeSection].Add(_activeSubSection, sub);
            }
            // replace
            else
            {
                sub = _subSections[_activeSection][_activeSubSection];
                if (message != null)
                    sub.Message = message;
                if (type != null)
                    sub.Type = type.Value;
                if (progressSteps != null)
                {
                    sub.MaxProgress = progressSteps.Value;
                    sub.CurrentProgress = 0;
                }
            }

            _send(sub);
        }
        public void Error(string message)
        {
            /// limit
            SubSection sub = _subSections[_activeSection][_activeSubSection];
            if (sub.ErrorCount == MaxErrors)
            {
                sub.Errors.Add("More errors hidden...");
                _send(sub);

                sub.HiddenErrors.Add(message);

                return;
            }
            else if (sub.Errors.Count > MaxErrors)
            {
                sub.HiddenErrors.Add(message);
                return;
            }

            /// change section
            if (_activeSubSection != "")
            {
                // section
                SubSection section = _subSections[_activeSection][""];
                section.Type = MessageType.Error;
                _send(section);
            }

            /// sub
            sub.Type = MessageType.Error;
            sub.Errors.Add(message);
            _send(sub);
        }
        public void Warning(string message)
        {
            SubSection sub = _subSections[_activeSection][_activeSubSection];
            /// limit
            if (sub.WarningCount == MaxWarnings)
            {
                sub.WarningCount++;
                sub.Errors.Add("More warnings hidden...");
                _send(sub);
                return;
            }
            else if (sub.WarningCount > MaxWarnings)
                return;

            /// sub
            sub.WarningCount++;
            sub.Errors.Add(message);
            _send(sub);
        }
        public void IncrementProgress(string subSection = null)
        {
            _activeSubSection = subSection ?? _activeSubSection;

            SubSection sub = _subSections[_activeSection][_activeSubSection];
            sub.CurrentProgress++;
            _send(sub);
        }

        private void _send(TSection section, string subSection, MessageType type, string message)
        {
            _ajaxSendAction(Json.Encode(new { section = section.ToString(), subSection = subSection, type = type.ToString().ToLower(), message = message }));
        }
        private void _send(SubSection subSection)
        {
            _send(subSection.Section, subSection.Name, subSection.Type, subSection.ToString());
        }

        internal static string _newLine;

        internal class SubSection
        {
            public SubSection()
            {
                WarningCount = 0;
                Errors = new List<string>();
                HiddenErrors = new List<string>();
            }

            public TSection Section { get; set; }
            public string Name { get; set; }

            public string Message { get; set; }
            public MessageType Type { get; set; }

            public int MaxProgress { get; set; }
            public int CurrentProgress { get; set; }

            public List<string> Errors { get; }
            public int WarningCount { get; set; }
            public int ErrorCount => Errors.Count - WarningCount;

            public List<string> HiddenErrors { get; set; }

            public override string ToString()
            {
                if (MaxProgress == default(int))
                {
                    return Errors.Any()
                        ? $"{Message}{_newLine}{string.Join(_newLine, Errors)}"
                        : Message;
                }

                return
                    $"{Message} <span class='build-progress'>{CurrentProgress}/{MaxProgress} <progress value={CurrentProgress} max={MaxProgress}>({100.0 * CurrentProgress / MaxProgress}%)</progress></span>{string.Join("", Errors.Select(e => $"{_newLine}{e}"))}";
            }
        }
    }
}
