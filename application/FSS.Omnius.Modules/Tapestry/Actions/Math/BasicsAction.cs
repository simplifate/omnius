using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using NCalc;
using FSS.Omnius.Modules.Tapestry.Actions.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions
{
    /// <summary>
    /// Evaluate any math problem
    /// input var names in []
    /// Use anything from NCalc - http://ncalc.codeplex.com/
    /// </summary>
    [MathRepository]
    public class BasicsAction : Action
    {
        public override int Id => 15;

        public override string[] InputVar => new string[] { "problem" };

        public override string[] OutputVar => new string[] { "result" };

        public override int? ReverseActionId => null;

        public override string Name => "Math: Basics operations";

        /// <summary>
        /// Evaluate any math problem, input var names in []
        /// </summary>
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string problem = (string)vars["problem"];

            // get vars names in problem
            List<string> varNames = new List<string>();
            int endI = 0;
            int startI = problem.IndexOf('[', endI) + 1;
            while (startI != 0)
            {
                endI = problem.IndexOf(']', startI);
                varNames.Add(problem.Substring(startI, endI - startI));

                startI = problem.IndexOf('[', endI) + 1;
            }

            // add params
            Expression expr = new Expression(problem);
            foreach (string varName in varNames)
            {
                expr.Parameters[varName] = vars[varName];
            }

            // solve
            outputVars["result"] = expr.Evaluate();
        }
    }
}
