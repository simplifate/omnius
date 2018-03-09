using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    using Master;

    [Table("Persona_Users")]
    public partial class User : IdentityUser<int, UserLogin, Iden_User_Role, UserClaim>, IEntity
    {
        public User()
        {
            ADgroup_Users = new HashSet<ADgroup_User>();
            UsersApplications = new HashSet<UsersApplications>();
            SecurityStamp = "b532ea85-8d2e-4ffb-8c64-86e8bfe363d7";
        }
        
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }
        [StringLength(100)]
        public string Company { get; set; }
        [StringLength(100)]
        public string Department { get; set; }
        [StringLength(100)]
        public string Team { get; set; }
        [StringLength(20)]
        public string WorkPhone { get; set; }
        [StringLength(20)]
        public string MobilPhone { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
        [StringLength(100)]
        public string Job { get; set; }
        public bool isLocalUser { get; set; }
        public int? LocaleId { get; set; }
        public DateTime CurrentLogin { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime? LastLogout { get; set; }
        public DateTime? LastAction { get; set; }
        public DateTime? DeletedBySync { get; set; }

        [StringLength(50)]
        public string LastIp { get; set; }
        public string LastAppCookie { get; set; }

        [Required]
        public DateTime? localExpiresAt { get; set; }

        public bool isActive { get; set; }

        public int? DesignAppId { get; set; }
        
        public virtual ICollection<ADgroup_User> ADgroup_Users { get; set; }
        public virtual ICollection<UsersApplications> UsersApplications { get; set; }
        public virtual ModuleAccessPermission ModuleAccessPermission { get; set; }
        public virtual Application DesignApp { get; set; }
        public virtual ICollection<User_Role> Users_Roles { get; set; }

        public void Update(User updateFrom)
        {
            if (updateFrom.DisplayName != null)
                DisplayName = updateFrom.DisplayName;

            if (updateFrom.Company != null)
                Company = updateFrom.Company;

            if (updateFrom.Department != null)
                Department = updateFrom.Department;

            if (updateFrom.Team != null)
                Team = updateFrom.Team;

            if (updateFrom.WorkPhone != null)
                WorkPhone = updateFrom.WorkPhone;

            if (updateFrom.MobilPhone != null)
                MobilPhone = updateFrom.MobilPhone;

            if (updateFrom.Address != null)
                Address = updateFrom.Address;

            if (updateFrom.Job != null)
                Job = updateFrom.Job;
            
            //isLocalUser = updateFrom.isLocalUser;

            //if (updateFrom.CurrentLogin != null)
            //    CurrentLogin = updateFrom.CurrentLogin;

            //if (updateFrom.LastLogin != null)
            //    LastLogin = updateFrom.LastLogin;

            //if (updateFrom.LastLogout != null)
            //    LastLogout = updateFrom.LastLogout;

            if (updateFrom.localExpiresAt != null)
                localExpiresAt = updateFrom.localExpiresAt;

            //if (updateFrom.isActive != null)
            //    isActive = updateFrom.isActive;
        }
    }
}
