namespace Tapestry.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Tapestry.Models.DBEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Tapestry.Models.DBEntities context)
        {
            context.WorkFlow_Types.AddOrUpdate(
                new WorkFlowType { Name = "Base" },
                new WorkFlowType { Name = "Overview" },
                new WorkFlowType { Name = "Partial" }
                );

            context.Actors.AddOrUpdate(
                new Actor { Name = "User" },
                new Actor { Name = "Auto" },
                new Actor { Name = "Time" }
                );

            //  This method will be called after migrating to the latest version.

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
