using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Modules.Tapestry.Actions.AuctionSystem
{
    public class GetWSO_Users : Action
    {
        public override int Id
        {
            get
            {
                return 5000;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override string Name
        {
            get
            {
                return "GetWsoUsers";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
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
            NexusWSService webService = new NexusWSService();
            object[] parameters = new[] {"Auction_User" };
            JToken results = webService.CallWebService("RWE_WSO_SOAP", "getUserListOfRole", parameters);
            var x = results.Children().First().Value<String>();

            //get the list of users names and add it to the list.
            IEnumerable<String> usersNames = results.Children().Values().Select(y => y.Value<String>());

            List<User> NewUsers = new List<User>();
            //iterate list of usersNames and make USerObject
            foreach (String userName in usersNames) {
                object[] param = new[] { userName,null };
                JToken userClaim = webService.CallWebService("RWE_WSO_SOAP", "getUserClaimValues", param);
            }
        }




    }
}
