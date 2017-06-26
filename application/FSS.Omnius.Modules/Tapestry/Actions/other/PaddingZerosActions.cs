using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    /// <summary>
    /// Action which is showed in Tapestry and can be used in workflows
    /// </summary>
    [EntitronRepository]
    public class PaddingZeroAction : Action
    {
        public override int Id => 216;

        /// <summary>
        /// Id of the action which revereses consequences of this action.
        /// </summary>
        public override int? ReverseActionId => null;

        /// <summary>
        /// Shows which parameters user can insert in UI.
        /// </summary>
        public override string[] InputVar => new string[] { "s$InputString", "i$Length" };

        /// <summary>
        /// Header in tapestry
        /// </summary>
        public override string Name => "Pad by zeros";
        
        /// <summary>
        /// Specifies where the action should store its results.
        /// </summary>
        public override string[] OutputVar => new string[] { "Result" };

        /// <summary>
        /// Kód co se spustí, když dojde k dané akci.
        /// </summary>
        /// <param name="inputVars">Input variables of the action - will be provided by user in UI.</param>
        /// <param name="outputVars">Output variables - user can specify where the results will be placed.</param>
        /// <param name="invertedInputVars"></param>
        /// <param name="message">Message for user.</param>
        public override void InnerRun(Dictionary<string, object> inputVars, Dictionary<string, object> outputVars,
            Dictionary<string, object> invertedInputVars, Message message)
        {
            string inputString = (string)inputVars["InputString"];
            int cipherCount = (int)inputVars["Length"];
            
            outputVars["Result"] = inputString.PadLeft(cipherCount, '0');
        }
    }
}
