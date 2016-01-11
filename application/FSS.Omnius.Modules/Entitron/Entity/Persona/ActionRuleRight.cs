using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_ActionRuleRights")]
    public partial class ActionRuleRight
    {
        [Key]
        [Column(Order = 1)]
        public int GroupId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ActionRuleId { get; set; }

        public bool Readable { get; set; }
        public bool Executable { get; set; }

        public virtual Group Group { get; set; }
        public virtual ActionRule ActionRule { get; set; }

        public string GetShort()
        {
            if (Executable)
                return "E";
            if (Readable)
                return "R";

            return "None";
        }

        public static List<string> GetSpecificRights(string groupName, string actionName)
        {
            DBEntities e = new DBEntities();
            return e.ActionRuleRights
                .Where(arr => arr.Group.Name == groupName && arr.ActionRule.Name == actionName)
                .Select(arr => arr.GetShort())
                .ToList();
        }
    }
}
