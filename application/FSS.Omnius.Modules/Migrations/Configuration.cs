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
            //  This method will be called after migrating to the latest version.
            if (!context.Modules.Any())
                context.Modules.AddOrUpdate(
                    new Entitron.Entity.CORE.Module { Name = "CORE", Description = "Central Omnius Rellay Engine", IsEnabled = true },
                    new Entitron.Entity.CORE.Module { Name = "Entitron", Description = "Manage Database", IsEnabled = true },
                    new Entitron.Entity.CORE.Module { Name = "Mozaic", Description = "Rendering Html", IsEnabled = true },
                    new Entitron.Entity.CORE.Module { Name = "Tapestry", Description = "WorkFlow", IsEnabled = true },
                    new Entitron.Entity.CORE.Module { Name = "Persona", Description = "Manage Rights, Users, Group", IsEnabled = true }
                );

            if (!context.WorkFlowTypes.Any())
                context.WorkFlowTypes.AddOrUpdate(
                    new Entitron.Entity.Tapestry.WorkFlowType { Name = "Init" },
                    new Entitron.Entity.Tapestry.WorkFlowType { Name = "Partial" },
                    new Entitron.Entity.Tapestry.WorkFlowType { Name = "Preview" }
                );

            if (!context.Actors.Any())
                context.Actors.AddOrUpdate(
                    new Entitron.Entity.Tapestry.Actor { Name = "Manual" },
                    new Entitron.Entity.Tapestry.Actor { Name = "Time" },
                    new Entitron.Entity.Tapestry.Actor { Name = "Auto" }
                );

            if (!context.DataTypes.Any())
                context.DataTypes.AddOrUpdate(
                    new Entitron.Entity.CORE.DataType { CSharpName = "string", SqlName = "NVarChar", shortcut = "s", limited = true, DBColumnTypeName = "varchar" },
                    new Entitron.Entity.CORE.DataType { CSharpName = "bool", SqlName = "Bit", shortcut = "b", limited = false, DBColumnTypeName = "boolean" },
                    new Entitron.Entity.CORE.DataType { CSharpName = "int", SqlName = "Integer", shortcut = "i", limited = false, DBColumnTypeName = "integer" },
                    new Entitron.Entity.CORE.DataType { CSharpName = "float", SqlName = "Float", shortcut = "f", limited = false, DBColumnTypeName = "float" },
                    new Entitron.Entity.CORE.DataType { CSharpName = "DateTime", SqlName = "DateTime", shortcut = "d", limited = false, DBColumnTypeName = "datetime" },
                    new Entitron.Entity.CORE.DataType { CSharpName = "string", SqlName = "XML", shortcut = "x", limited = true, DBColumnTypeName = "xml" },
                    new Entitron.Entity.CORE.DataType { CSharpName = "blob", SqlName = "Blob", shortcut = "l", limited = true, DBColumnTypeName = "blob" }
                );

            if (!context.Applications.Any())
                context.Applications.AddOrUpdate(
                    new Entitron.Entity.Master.Application { Name = "System", DisplayName = "System", IsSystem = true, TitleFontSize = 12, Color = 1, TileHeight = 5, TileWidth = 5 }
                );

            if (!context.Applications.Any())
                context.Applications.AddOrUpdate(
                    new Entitron.Entity.Master.Application { Name = "System", DisplayName = "System", IsEnabled = true, IsPublished = false, IsSystem = true }
                );
        }
    }
}
