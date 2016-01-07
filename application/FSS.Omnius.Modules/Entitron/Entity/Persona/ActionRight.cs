using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using FSS.Omnius.Modules.Tapestry;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_ActionRights")]
    public class ActionRight
    {
        [Key]
        [Column(Order = 1)]
        public int GroupId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ActionId { get; set; }

        public bool Readable { get; set; }
        public bool Executable { get; set; }

        public virtual Group Group { get; set; }

        public static List<string> GetSpecificRights(string groupName, string actionName)
        {
            DBEntities e = new DBEntities();
            List<string> rights = new List<string>();

            foreach (ActionRight act in e.ActionRights)
            {
                string name = Action.All[act.ActionId].Name;

                if ((act.Group.Name == groupName) && (name == actionName))
                {
                    if (act.Readable == true)
                    {
                        rights.Add("R");
                    }
                    else if (act.Executable == true)
                    {
                        rights.Add("E");
                    }
                    else if (act.Executable == false || act.Readable == false)
                    {
                        rights.Add("None");
                    }
                }
            }
            return rights;
        }
    }
}
