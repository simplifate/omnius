using FSS.Omnius.Modules.CORE;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
	[EntitronRepository]
	public class RigPlacementEditSaveAction : Action
	{
		private const string RIG_PLACEMENT_TABLE = "RigPacementTable";
		private const string NAME = "Update rig placements";

		public override int Id
		{
			get
			{
				return 185;
			}
		}
		public override int? ReverseActionId
		{
			get
			{
				return null;
			}
		}
		public override string[] InputVar
		{
			get
			{
				return new string[] { RIG_PLACEMENT_TABLE };
			}
		}

		public override string Name
		{
			get
			{
				return NAME;
			}
		}

		public override string[] OutputVar
		{
			get
			{
				return new string[] { "Result" };
			}
		}

		public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
		{
			var table = vars[RIG_PLACEMENT_TABLE];
		}
	}
}
