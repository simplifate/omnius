using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
	/// <summary>
	/// Akce pro nastaveni data odebrani uzivatele z Active Directory.
	/// Akce prijima tabulku puvodnich uzivatelu v AD a tabulku novych uzivatelu v AD.
	/// Vytridi nove vymazane uivatele a priradi jim aktualni cas do sloupce DeletedBySync v tabulce Entitron_Users.
	/// Sloupec sapid1 obsahuje uzivatelska jmena v AD. 
	/// </summary>
	[OtherRepository]
	class RecordDatesDeletedUsers : Action
	{
		public override int Id => 4111;
		public override string[] InputVar => new string[] { "OldData", "NewData" };
		public override string Name => "Persona: Record dates of deleted users";
		public override string[] OutputVar => new string[] { "Result" };
		public override int? ReverseActionId => null;

		public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
		{
			// Table of old users in AD
			List<DBItem> oldData = (List<DBItem>)vars["OldData"];

			// Table of new users in AD
			List<DBItem> newData = (List<DBItem>)vars["NewData"];

			// List of newly removed users from AD
			List<DBItem> removedUsers = new List<DBItem>();

			for (int i = 0; i < oldData.Count; i++)
			{
				DBItem oldUser = oldData[i];
				string sapid1 = (string)oldUser["sapid1"];

				// Remove old active users, so oldData contains only deleted users in the end
				if (!(newData.Any(c => (string)c["sapid1"] == sapid1)))
					removedUsers.Add(oldUser);
			}

			foreach (DBItem removedUser in removedUsers)
			{
				string sapid1 = (string)removedUser["sapid1"];
				var user = DBEntities.instance.Users.SingleOrDefault(c => c.UserName == sapid1 && c.DeletedBySync == null);

				if (user != null)
					user.DeletedBySync = DateTime.Now;
			}

			DBEntities.instance.SaveChanges();
			outputVars["Result"] = oldData;
		}
	}
}