using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    public class AjaxUsers
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Team { get; set; }
        public string WorkPhone { get; set; }
        public string Job { get; set; }
        public string Email { get; set; }

        public static List<AjaxUsers> converToAjaxUsers(List<User> users)
        {
            var ajaxUsers = new List<AjaxUsers>();
            foreach (var u in users)
            {
                AjaxUsers user = new AjaxUsers();
                user.Id = u.Id;
                user.DisplayName = u.DisplayName;
                user.UserName = u.UserName;
                user.Company = u.Company;
                user.Department = u.Department;
                user.Team = u.Team;
                user.Job = u.Job;
                user.Email = u.Email;
                user.WorkPhone = u.WorkPhone;
                ajaxUsers.Add(user);
            }
            return ajaxUsers;
        }
    }

}
