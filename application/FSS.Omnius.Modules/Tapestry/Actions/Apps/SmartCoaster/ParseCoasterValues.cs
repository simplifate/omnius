using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.SmartCoaster
{
    public class ParseCoasterValues : Action
    {
        public override int Id
        {
            get
            {
                return 5020;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "ByteArray" };
            }
        }

        public override string Name
        {
            get
            {
                return "Parse coaster values";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "MacId","Weight" };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            byte[] data = (byte[])vars["ByteArray"];
            Stream stream = new MemoryStream(data);
            BinaryReader br = new BinaryReader(stream);
            int MacID = br.ReadInt32();
            int Value = br.ReadInt32();
            int Offset = br.ReadInt32();
            int Unit = br.ReadInt32();
            int Weight = br.ReadInt32();

            outputVars["MacId"] = MacID;
            outputVars["Weight"] = Weight;

        }
    }
}
