namespace FSS.Omnius.Modules.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using FSS.Omnius.Modules.Entitron;
    using FSS.Omnius.Modules.Entitron.Entity;
    using FSS.Omnius.Modules.Entitron.Entity.Persona;
    using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
    using FSS.Omnius.Modules.Entitron.Entity.Master;
    using FSS.Omnius.Modules.Entitron.Entity.Entitron;

    internal sealed class Configuration : DbMigrationsConfiguration<DBEntities>
    {
        public Configuration()
        {
            /// common
            AutomaticMigrationsEnabled = false;
            
            /// provider-specific attrs
            /// - Migration directory
            Entitron.ParseConnectionString("DefaultConnection");
            switch (Entitron.DefaultDBType)
            {
                case Modules.Entitron.DB.ESqlType.MSSQL:
                    MigrationsDirectory = "Migrations\\MSSQL";
                    MigrationsNamespace = "FSS.Omnius.Modules.Migrations.MSSQL";
                    break;
                case Modules.Entitron.DB.ESqlType.MySQL:
                    MigrationsDirectory = "Migrations\\MySQL";
                    MigrationsNamespace = "FSS.Omnius.Modules.Migrations.MySQL";
                    break;
                default:
                    throw new InvalidOperationException("Unknown DB provider");
            }

            /// MySQL
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
        }

        protected override void Seed(DBEntities context)
        {
            // Enum: Workflow types
            if (!context.WorkFlowTypes.Any())
                context.WorkFlowTypes.AddOrUpdate(
                    new WorkFlowType { Name = "Init" },
                    new WorkFlowType { Name = "Partial" },
                    new WorkFlowType { Name = "Preview" }
                );
            // Enum: Actor types
            if (!context.Actors.Any())
                context.Actors.AddOrUpdate(
                    new Actor { Name = "Manual" },
                    new Actor { Name = "Time" },
                    new Actor { Name = "Auto" }
                );

            // system default application
            var systemApp = context.Applications.SingleOrDefault(a => a.IsSystem) ?? new Application { Name = "System", DisplayName = "System", IsSystem = true, IsPublished = false, IsEnabled = true, TitleFontSize = 12, Color = 1, TileHeight = 5, TileWidth = 5 };
            if (!context.Applications.Any())
                context.Applications.AddOrUpdate(systemApp);
            // system default AD group
            var systemAD = context.ADgroups.SingleOrDefault(ad => ad.Name == "System") ?? new ADgroup { Name = "System", Application = systemApp, isAdmin = true };
            if (!context.ADgroups.Any())
                context.ADgroups.AddOrUpdate(systemAD);

            // empty shared dbscheme
            if (!context.DBSchemeCommits.Any())
                context.DBSchemeCommits.AddOrUpdate(
                    new DbSchemeCommit { Application = systemApp, CommitMessage = "Empty", IsComplete = true, Timestamp = DateTime.UtcNow }
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
