using System.Collections.Generic;
using System.Text.RegularExpressions;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    /// <summary>
    /// Akce prijima vstupni string v escapovanem tvaru (zaescapovana odradkovani, tabulatory, ...) 
    /// napr. "This is indented:\r\n\tHello World".
    /// Na vystupu vrati odescapovany string puvodniho vstupniho stringu.
    /// </summary>
    [OtherRepository]
    class UnescapeStringAction : Action
    {
        public override int Id => 218;
        public override string[] InputVar => new string[] { "EscapedString" };
        public override string Name => "Unescape String";
        public override string[] OutputVar => new string[] { "Result" };
        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string input = (string)vars["EscapedString"];

            var output = Regex.Replace(input, @"\\[rnt]", m =>
            {
                switch (m.Value)
                {
                    case @"\r": return "\r";
                    case @"\n": return "\n";
                    case @"\t": return "\t";
                    default: return m.Value;
                }
            });

            outputVars["Result"] = output;
        }
    }
}
