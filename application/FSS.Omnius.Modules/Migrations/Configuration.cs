namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity;
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
            if (!context.ADgroups.Any())
                context.ADgroups.AddOrUpdate(
                    new Entitron.Entity.Persona.ADgroup { Name = "System", Application = systemApp, isAdmin = true }
                );

            // empty shared dbscheme
            if (!context.DBSchemeCommits.Any())
                context.DBSchemeCommits.AddOrUpdate(
                    new Entitron.Entity.Entitron.DbSchemeCommit { Application = systemApp, CommitMessage = "Empty", IsComplete = true, Timestamp = DateTime.UtcNow }
                );
        }
    }
}
