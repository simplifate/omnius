using System;
using System.Collections.Generic;
using Renci.SshNet;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Watchtower;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
	class RunSSHCommand : Action
	{
		public override int Id
		{
			get
			{
				return 3014;
			}
		}

		public override string[] InputVar
		{
			get
			{
				return new string[] { "s$Hostname", "i$Port", "s$Username", "s$Password", "s$Command" };
			}
		}

		public override string Name
		{
			get
			{
				return "Run SSH command";
			}
		}

		public override string[] OutputVar
		{
			get
			{
				return new string[]
				{
					"Result", "Error"
				};
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
            string hostname = (string)vars["Hostname"];
            int port = (int)vars["Port"];
            string userName = (string)vars["Username"];
            string password = (string)vars["Password"];
            string command = (string)vars["Command"];

			try
			{
                using (var client = new SshClient(hostname, port, userName, password)) {
                    client.Connect();
                    var result = client.RunCommand(command);
                    client.Disconnect();
                }
            }
			catch (Exception e)
            {
                COREobject core = COREobject.i;
                OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
				outputVars["Result"] = String.Empty;
				outputVars["Error"] = true;
			}
		}
	}
}
