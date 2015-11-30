namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FSS.Omnius.Modules.Entitron.Entity.DBEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FSS.Omnius.Modules.Entitron.Entity.DBEntities context)
        {
            //  This method will be called after migrating to the latest version.

            context.Modules.AddOrUpdate(
                new Entitron.Entity.CORE.Module { Name = "CORE", Description = "Central Omnius Rellay Engine", IsEnabled = true },
                new Entitron.Entity.CORE.Module { Name = "Entitron", Description = "Manage Database", IsEnabled = true },
                new Entitron.Entity.CORE.Module { Name = "Mozaic", Description = "Rendering Html", IsEnabled = true },
                new Entitron.Entity.CORE.Module { Name = "Tapestry", Description = "WorkFlow", IsEnabled = true },
                new Entitron.Entity.CORE.Module { Name = "Persona", Description = "Manage Rights, Users, Group", IsEnabled = true }
            );
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
