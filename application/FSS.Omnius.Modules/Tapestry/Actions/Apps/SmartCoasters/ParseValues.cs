using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.SmartCoasters
{
    public class ParseValues : Action
    {
        public override int Id => 7001;

        public override string[] InputVar => new string[] { "v$ByteArray" };

        public override string[] OutputVar => new string[] { "MacAddress", "Data" };

        public override string Name => "SmartCoasters: Parse values";

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            byte[] input = (byte[])vars["ByteArray"];
            List<int> result = new List<int>();

            if (input.Count() != 84)
            {
                throw new Exception($"{Name}: unexpected data length. 84 bytes expected.");
            }

            List<int> numbers = new List<int>();
            for (int i = 0; i <= 80; i += 4)
            {
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
