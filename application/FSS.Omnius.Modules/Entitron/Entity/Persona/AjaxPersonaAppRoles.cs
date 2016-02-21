using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    public class AjaxPersonaAppRoles
    {
        public string AppName { get; set; }
        public List<AjaxPersonaAppRoles_User> Users { get; set; }
        public List<AjaxPersonaAppRoles_Role> Roles { get; set; }

        public AjaxPersonaAppRoles()
        {
            Users = new List<AjaxPersonaAppRoles_User>();
            Roles = new List<AjaxPersonaAppRoles_Role>();
        }
    }

    public class AjaxPersonaAppRoles_User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AjaxPersonaAppRoles_Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> MemberList { get; set; }

        public AjaxPersonaAppRoles_Role()
        {
            MemberList = new List<int>();
        }
    }
}