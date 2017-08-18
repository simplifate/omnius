using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.SmartCoasters
{
    public class ParseValues : Action
    {
        public override int Id
        {
            get {
                return 7001;
            }
        }

        public override string[] InputVar
        {
            get {
                return new string[] { "v$ByteArray" };
            }
        }

        public override string[] OutputVar
        {
            get {
                return new string[] { "MacAddress", "Data" };
            }
        }

        public override string Name
        {
            get {
                return "SmartCoasters: Parse values";
            }
        }

        public override int? ReverseActionId
        {
            get {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            byte[] input = (byte[])vars["ByteArray"];
            List<int> result = new List<int>();

            if(input.Count() != 84) {
                throw new Exception($"{Name}: unexpected data length. 84 bytes expected.");
            }

            List<int> numbers = new List<int>();
            for (int i = 0; i <= 80; i += 4) {
                numbers.Add(BitConverter.ToInt32(input, i));
            }

            result.Add(numbers.Skip(4).Take(1).Single());
            result.Add(numbers.Skip(8).Take(1).Single());
            result.Add(numbers.Skip(12).Take(1).Single());
            result.Add(numbers.Skip(16).Take(1).Single());
            result.Add(numbers.Skip(20).Take(1).Single());

            outputVars["MacAddress"] = numbers.First();
            outputVars["Data"] = result;
        }
    }
}
