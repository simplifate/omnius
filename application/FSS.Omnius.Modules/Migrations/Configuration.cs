namespace FSS.Omnius.Modules.Migrations
{
    using FSS.Omnius.Modules.Entitron.Entity.Persona;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Entitron.Entity.DBEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Entitron.Entity.DBEntities context)
        {
            // Enum: Workflow types
            if (!context.WorkFlowTypes.Any())
                context.WorkFlowTypes.AddOrUpdate(
                    new Entitron.Entity.Tapestry.WorkFlowType { Name = "Init" },
                    new Entitron.Entity.Tapestry.WorkFlowType { Name = "Partial" },
                    new Entitron.Entity.Tapestry.WorkFlowType { Name = "Preview" }
                );
            // Enum: Actor types
            if (!context.Actors.Any())
                context.Actors.AddOrUpdate(
                    new Entitron.Entity.Tapestry.Actor { Name = "Manual" },
                    new Entitron.Entity.Tapestry.Actor { Name = "Time" },
                    new Entitron.Entity.Tapestry.Actor { Name = "Auto" }
                );

            // system default application
            var systemApp = context.Applications.SingleOrDefault(a => a.IsSystem) ?? new Entitron.Entity.Master.Application { Name = "System", DisplayName = "System", IsSystem = true, IsPublished = false, IsEnabled = true, TitleFontSize = 12, Color = 1, TileHeight = 5, TileWidth = 5 };
            if (!context.Applications.Any())
                context.Applications.AddOrUpdate(systemApp);
            // system default AD group
            var systemAD = context.ADgroups.SingleOrDefault(ad => ad.Name == "System") ?? new ADgroup { Name = "System", Application = systemApp, isAdmin = true };
            if (!context.ADgroups.Any())
                context.ADgroups.AddOrUpdate(systemAD);

            // empty shared dbscheme
            if (!context.DBSchemeCommits.Any())
                context.DBSchemeCommits.AddOrUpdate(
                    new Entitron.Entity.Entitron.DbSchemeCommit { Application = systemApp, CommitMessage = "Empty", IsComplete = true, Timestamp = DateTime.UtcNow }
                );

            // admin account
            if (!context.Users.Any())
            {
                var store = new UserStore<User, Iden_Role, int, UserLogin, Iden_User_Role, UserClaim>(context);
                var manager = new UserManager<User, int>(store);
                var user = new User { UserName = "admin", DisplayName = "Admin", Email = "admin@example.com", isLocalUser = true, localExpiresAt = DateTime.UtcNow, LastLogin = DateTime.UtcNow, LastLogout = DateTime.UtcNow, CurrentLogin = DateTime.UtcNow };

                manager.Create(user, "pass132");

                user.ModuleAccessPermission = new ModuleAccessPermission { Athena = true, Babylon = true, Core = true, Cortex = true, Entitron = true, Hermes = true, Master = true, Mozaic = true, Nexus = true, Persona = true, Sentry = true, Tapestry = true, Watchtower = true, User = user };
                user.ADgroup_Users.Add(new ADgroup_User { ADgroup = systemAD, User = user });
            }
        }
    }
}
