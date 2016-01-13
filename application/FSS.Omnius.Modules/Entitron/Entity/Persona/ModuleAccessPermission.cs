﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_ModuleAccessPermissions")]
    public class ModuleAccessPermission
    {
        public int Id { get; set; }

        public virtual User User { get; set; }

        public bool Core { get; set; }
        public bool Master { get; set; }
        public bool Tapestry { get; set; }
        public bool Entitron { get; set; }
        public bool Mozaic { get; set; }
        public bool Persona { get; set; }
        public bool Nexus { get; set; }
        public bool Sentry { get; set; }
        public bool Hermes { get; set; }
        public bool Athena { get; set; }
        public bool Watchtower { get; set; }
        public bool Cortex { get; set; }
    }

    public class AjaxModuleAccessPermission
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public bool Core { get; set; }
        public bool Master { get; set; }
        public bool Tapestry { get; set; }
        public bool Entitron { get; set; }
        public bool Mozaic { get; set; }
        public bool Persona { get; set; }
        public bool Nexus { get; set; }
        public bool Sentry { get; set; }
        public bool Hermes { get; set; }
        public bool Athena { get; set; }
        public bool Watchtower { get; set; }
        public bool Cortex { get; set; }
    }

    public class AjaxModuleAccessPermissionSettings
    {
        public List<AjaxModuleAccessPermission> PermissionList { get; set; }
    }
}