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
    using FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap;
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

            // default app
            if (context.Applications.Count() < 2)
            {
                // master
                var app = new Application { Name = "tables", DisplayName = "Testing tables", IsSystem = false, IsAllowedForAll = true, EntitronChangedSinceLastBuild = true, MenuChangedSinceLastBuild = true, MozaicChangedSinceLastBuild = true, TapestryChangedSinceLastBuild = true, TitleFontSize = 12, Color = 1, TileHeight = 5, TileWidth = 5 };
                context.Applications.Add(app);

                // entitron
                var schemeCommit = new DbSchemeCommit { Application = app, IsComplete = true, Timestamp = DateTime.UtcNow };
                context.DBSchemeCommits.Add(schemeCommit);

                var table = new DbTable { Name = "Tables", PositionX = 387, PositionY = 200, DbSchemeCommit = schemeCommit };
                context.DbTables.Add(table);

                context.DbColumn.AddOrUpdate(
                    new DbColumn { Name = "id", PrimaryKey = true, Type = "integer", DbTable = table },
                    new DbColumn { Name = "Name", Unique = true, Type = "varchar", ColumnLength = 100, DbTable = table },
                    new DbColumn { Name = "Height", Type = "integer", DbTable = table },
                    new DbColumn { Name = "Width", Type = "integer", DbTable = table },
                    new DbColumn { Name = "Length", Type = "integer", DbTable = table }
                );
                context.ColumnMetadata.AddOrUpdate(
                    new ColumnMetadata { TableName = "Tables", ColumnName = "id", ColumnDisplayName = "id", Application = app },
                    new ColumnMetadata { TableName = "Tables", ColumnName = "Name", ColumnDisplayName = "Name", Application = app },
                    new ColumnMetadata { TableName = "Tables", ColumnName = "Height", ColumnDisplayName = "Height", Application = app },
                    new ColumnMetadata { TableName = "Tables", ColumnName = "Width", ColumnDisplayName = "Width", Application = app },
                    new ColumnMetadata { TableName = "Tables", ColumnName = "Length", ColumnDisplayName = "Length", Application = app }
                );

                // mozaic
                var indexPage = new MozaicBootstrapPage { Name = "Index", CompiledPageId = 0, Version = Modules.Entitron.Entity.Mozaic.VersionEnum.Bootstrap, IsDeleted = false, ParentApp = app, Content = "<h1 data-uic=\"text|heading\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">List</span></h1><form class=\"form-horizontal\" method=\"post\" data-uic=\"form|form\"><button type=\"submit\" class=\"btn btn-primary active\" name=\"button\" data-uic=\"controls|button\" value=\"new\" id=\"new\"><span class=\"mbe-text-node\" contenteditable=\"false\">New table</span></button></form><table class=\"table data-table dataTable no-footer mbe-active\" data-uic=\"ui|data-table\" data-dtpaging=\"1\" data-dtinfo=\"1\" data-dtfilter=\"1\" data-dtordering=\"1\" data-dtcolumnfilter=\"0\" data-dtserverside=\"0\" id=\"list\" role=\"grid\" aria-describedby=\"list_info\" data-actions=\"[{'icon':'fa fa-pencil','action':'edit','idParam':'modelId','title':'Edit','confirm':''},{'icon':'fa fa-remove','action':'remove','idParam':'deleteId','title':'Remove','confirm':''}]\"><thead><tr role=\"row\"><th class=\"sorting_asc\" tabindex=\"0\" aria-controls=\"list\" rowspan=\"1\" colspan=\"1\" aria-label=\"Column 1: activate to sort column descending\" style=\"width: 297px;\" aria-sort=\"ascending\"><span class=\"mbe-text-node\">Column 1</span></th><th class=\"sorting\" tabindex=\"0\" aria-controls=\"list\" rowspan=\"1\" colspan=\"1\" aria-label=\"Column 2: activate to sort column ascending\" style=\"width: 299px;\"><span class=\"mbe-text-node\">Column 2</span></th><th class=\"sorting\" tabindex=\"0\" aria-controls=\"list\" rowspan=\"1\" colspan=\"1\" aria-label=\"Column 3: activate to sort column ascending\" style=\"width: 300px;\"><span class=\"mbe-text-node\">Column 3</span></th><th class=\"actionHeader sorting\" tabindex=\"0\" aria-controls=\"list\" rowspan=\"1\" colspan=\"1\" aria-label=\"Akce: activate to sort column ascending\" style=\"width: 35px;\"><span class=\"mbe-text-node\">Akce</span></th></tr></thead><tbody><tr role=\"row\" class=\"odd\"><td class=\"sorting_1\"><span class=\"mbe-text-node\">Value 1</span></td><td><span class=\"mbe-text-node\">Value 2</span></td><td><span class=\"mbe-text-node\">Value 3</span></td><td class=\"actionIcons\"><i class=\"fa fa-pencil\" data-action=\"edit\" data-idparam=\"modelId\" data-confirm=\"\" title=\"Edit\"></i><i class=\"fa fa-remove\" data-action=\"remove\" data-idparam=\"deleteId\" data-confirm=\"\" title=\"Remove\"></i></td></tr><tr role=\"row\" class=\"even\"><td class=\"sorting_1\"><span class=\"mbe-text-node\">Value 2</span></td><td><span class=\"mbe-text-node\">Value 4</span></td><td><span class=\"mbe-text-node\">Value 6</span></td><td class=\"actionIcons\"><i class=\"fa fa-pencil\" data-action=\"edit\" data-idparam=\"modelId\" data-confirm=\"\" title=\"Edit\"></i><i class=\"fa fa-remove\" data-action=\"remove\" data-idparam=\"deleteId\" data-confirm=\"\" title=\"Remove\"></i></td></tr><tr role=\"row\" class=\"odd\"><td class=\"sorting_1\"><span class=\"mbe-text-node\">Value 3</span></td><td><span class=\"mbe-text-node\">Value 6</span></td><td><span class=\"mbe-text-node\">Value 9</span></td><td class=\"actionIcons\"><i class=\"fa fa-pencil\" data-action=\"edit\" data-idparam=\"modelId\" data-confirm=\"\" title=\"Edit\"></i><i class=\"fa fa-remove\" data-action=\"remove\" data-idparam=\"deleteId\" data-confirm=\"\" title=\"Remove\"></i></td></tr></tbody></table><span class=\"mbe-drag-handle\" draggable=\"true\" style=\"top: 133.883px; left: 1.25px;\"></span>" };
                var newPage = new MozaicBootstrapPage { Name = "New", CompiledPageId = 0, Version = Modules.Entitron.Entity.Mozaic.VersionEnum.Bootstrap, IsDeleted = false, ParentApp = app, Content = "<h1 data-uic=\"text|heading\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">New table</span></h1><form class=\"form-horizontal\" method=\"post\" data-uic=\"form|form\" _lpchecked=\"1\"><label for=\"name\" data-uic=\"form|label\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">Name<br></span></label><input type=\"text\" name=\"name\" value=\"\" class=\"form-control\" data-uic=\"form|input-text\" id=\"name\" autofocus=\"autofocus\" tabindex=\"1\"><label for=\"height\" data-uic=\"form|label\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">Height</span></label><input type=\"number\" name=\"height\" value=\"\" class=\"form-control\" data-uic=\"form|input-number\" tabindex=\"2\" id=\"height\"><label for=\"width\" data-uic=\"form|label\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">Width<br></span></label><input type=\"number\" name=\"width\" value=\"\" class=\"form-control\" data-uic=\"form|input-number\" id=\"width\" tabindex=\"3\"><label for=\"length\" data-uic=\"form|label\" class=\"mbe-active\"><span class=\"mbe-text-node\" contenteditable=\"false\">Length</span></label><input type=\"number\" name=\"length\" value=\"\" class=\"form-control\" data-uic=\"form|input-number\" id=\"length\" tabindex=\"5\"><button type=\"submit\" class=\"btn active btn-primary\" name=\"button\" data-uic=\"controls|button\" value=\"create\" id=\"create\"><span class=\"mbe-text-node\" contenteditable=\"false\">Create</span></button><button type=\"submit\" class=\"btn btn-default active\" name=\"button\" data-uic=\"controls|button\" value=\"back\" id=\"back\"><span class=\"mbe-text-node\" contenteditable=\"false\">Back</span></button></form><span class=\"mbe-drag-handle\" draggable=\"true\" style=\"top: 245.398px; left: 0.992188px;\"></span>" };
                var editPage = new MozaicBootstrapPage { Name = "Edit", CompiledPageId = 0, Version = Modules.Entitron.Entity.Mozaic.VersionEnum.Bootstrap, IsDeleted = false, ParentApp = app, Content = "<h1 data-uic=\"text|heading\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">Edit table</span></h1><form class=\"form-horizontal\" method=\"post\" data-uic=\"form|form\" _lpchecked=\"1\"><label for=\"name\" data-uic=\"form|label\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">Name<br></span></label><input type=\"text\" name=\"name\" value=\"\" class=\"form-control\" data-uic=\"form|input-text\" id=\"name\" autofocus=\"autofocus\" tabindex=\"1\"><label for=\"height\" data-uic=\"form|label\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">Height</span></label><input type=\"number\" name=\"height\" value=\"\" class=\"form-control\" data-uic=\"form|input-number\" tabindex=\"2\" id=\"height\"><label for=\"width\" data-uic=\"form|label\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">Width<br></span></label><input type=\"number\" name=\"width\" value=\"\" class=\"form-control\" data-uic=\"form|input-number\" id=\"width\" tabindex=\"3\"><label for=\"length\" data-uic=\"form|label\" class=\"\"><span class=\"mbe-text-node\" contenteditable=\"false\">Length</span></label><input type=\"number\" name=\"length\" value=\"\" class=\"form-control\" data-uic=\"form|input-number\" id=\"length\" tabindex=\"5\"><button type=\"submit\" class=\"btn active btn-primary mbe-active\" name=\"button\" data-uic=\"controls|button\" value=\"update\" id=\"update\"><span class=\"mbe-text-node\" contenteditable=\"false\">Update</span></button><button type=\"submit\" class=\"btn btn-default active\" name=\"button\" data-uic=\"controls|button\" value=\"back\" id=\"back\"><span class=\"mbe-text-node\" contenteditable=\"false\">Back</span></button></form><span class=\"mbe-drag-handle\" draggable=\"true\" style=\"top: 303.875px; left: 0.992188px;\"></span>" };
                context.MozaicBootstrapPages.AddOrUpdate(
                    indexPage,
                    newPage,
                    editPage
                );

                var indexFormComponent = new MozaicBootstrapComponent { Tag = "form", UIC = "form|form", Attributes = "[{\"name\":\"class\",\"value\":\"form-horizontal\"},{\"name\":\"method\",\"value\":\"post\"}]", Content = "__UIC_1__", MozaicBootstrapPage = indexPage, NumOrder = 2 };
                var newFormComponent = new MozaicBootstrapComponent { Tag = "form", UIC = "form|form", Attributes = "[{\"name\":\"class\",\"value\":\"form-horizontal\"},{\"name\":\"method\",\"value\":\"post\"},{\"name\":\"_lpchecked\",\"value\":\"1\"}]", Content = "__UIC_1____UIC_2____UIC_3____UIC_4____UIC_5____UIC_6____UIC_7____UIC_8____UIC_9____UIC_10__", MozaicBootstrapPage = newPage, NumOrder = 2 };
                var editFormComponent = new MozaicBootstrapComponent { Tag = "form", UIC = "form|form", Attributes = "[{\"name\":\"class\",\"value\":\"form-horizontal\"},{\"name\":\"method\",\"value\":\"post\"},{\"name\":\"_lpchecked\",\"value\":\"1\"}]", Content = "__UIC_1____UIC_2____UIC_3____UIC_4____UIC_5____UIC_6____UIC_7____UIC_8____UIC_9____UIC_10__", MozaicBootstrapPage = editPage, NumOrder = 2 };
                context.MozaicBootstrapComponents.AddOrUpdate(
                    indexFormComponent,
                    new MozaicBootstrapComponent { Tag = "h1", UIC = "text|heading", Attributes = "[{\"name\":\"class\",\"value\":\"\"}]", Content = "List", MozaicBootstrapPage = indexPage, NumOrder = 1 },
                    new MozaicBootstrapComponent { Tag = "button", UIC = "controls|button", Attributes = "[{\"name\":\"type\",\"value\":\"submit\"},{\"name\":\"class\",\"value\":\"btn btn-primary active\"},{\"name\":\"name\",\"value\":\"button\"},{\"name\":\"value\",\"value\":\"new\"},{\"name\":\"id\",\"value\":\"new\"}]", Content = "New table", ParentComponent = indexFormComponent, MozaicBootstrapPage = indexPage, ElmId = "new", NumOrder = 1 },
                    new MozaicBootstrapComponent { Tag = "table", UIC = "ui|data-table", Attributes = "[{\"name\":\"class\",\"value\":\"table data-table dataTable no-footer mbe-active\"},{\"name\":\"data-dtpaging\",\"value\":\"1\"},{\"name\":\"data-dtinfo\",\"value\":\"1\"},{\"name\":\"data-dtfilter\",\"value\":\"1\"},{\"name\":\"data-dtordering\",\"value\":\"1\"},{\"name\":\"data-dtcolumnfilter\",\"value\":\"0\"},{\"name\":\"data-dtserverside\",\"value\":\"0\"},{\"name\":\"id\",\"value\":\"list\"},{\"name\":\"role\",\"value\":\"grid\"},{\"name\":\"aria-describedby\",\"value\":\"list_info\"},{\"name\":\"data-actions\",\"value\":\"[{'icon':'fa fa-pencil','action':'edit','idParam':'modelId','title':'Edit','confirm':''},{'icon':'fa fa-remove','action':'remove','idParam':'deleteId','title':'Remove','confirm':''}]\"}]", Content = "<thead><tr role=\"row\"><th class=\"sorting_asc\" tabindex=\"0\" aria-controls=\"list\" rowspan=\"1\" colspan=\"1\" aria-label=\"Column 1: activate to sort column descending\" style=\"width: 297px;\" aria-sort=\"ascending\">Column 1</th><th class=\"sorting\" tabindex=\"0\" aria-controls=\"list\" rowspan=\"1\" colspan=\"1\" aria-label=\"Column 2: activate to sort column ascending\" style=\"width: 299px;\">Column 2</th><th class=\"sorting\" tabindex=\"0\" aria-controls=\"list\" rowspan=\"1\" colspan=\"1\" aria-label=\"Column 3: activate to sort column ascending\" style=\"width: 300px;\">Column 3</th><th class=\"actionHeader sorting\" tabindex=\"0\" aria-controls=\"list\" rowspan=\"1\" colspan=\"1\" aria-label=\"Akce: activate to sort column ascending\" style=\"width: 35px;\">Akce</th></tr></thead><tbody><tr role=\"row\" class=\"odd\"><td class=\"sorting_1\">Value 1</td><td>Value 2</td><td>Value 3</td><td class=\"actionIcons\"><i class=\"fa fa-pencil\" data-action=\"edit\" data-idparam=\"modelId\" data-confirm=\"\" title=\"Edit\"></i><i class=\"fa fa-remove\" data-action=\"remove\" data-idparam=\"deleteId\" data-confirm=\"\" title=\"Remove\"></i></td></tr><tr role=\"row\" class=\"even\"><td class=\"sorting_1\">Value 2</td><td>Value 4</td><td>Value 6</td><td class=\"actionIcons\"><i class=\"fa fa-pencil\" data-action=\"edit\" data-idparam=\"modelId\" data-confirm=\"\" title=\"Edit\"></i><i class=\"fa fa-remove\" data-action=\"remove\" data-idparam=\"deleteId\" data-confirm=\"\" title=\"Remove\"></i></td></tr><tr role=\"row\" class=\"odd\"><td class=\"sorting_1\">Value 3</td><td>Value 6</td><td>Value 9</td><td class=\"actionIcons\"><i class=\"fa fa-pencil\" data-action=\"edit\" data-idparam=\"modelId\" data-confirm=\"\" title=\"Edit\"></i><i class=\"fa fa-remove\" data-action=\"remove\" data-idparam=\"deleteId\" data-confirm=\"\" title=\"Remove\"></i></td></tr></tbody>", MozaicBootstrapPage = indexPage, ElmId = "list", NumOrder = 3 },
                    newFormComponent,
                    new MozaicBootstrapComponent { Tag = "h1", UIC = "text|heading", Attributes = "[{\"name\":\"class\",\"value\":\"\"}]", Content = "New table", MozaicBootstrapPage = newPage, NumOrder = 1 },
                    new MozaicBootstrapComponent { Tag = "label", UIC = "form|label", Attributes = "[{\"name\":\"for\",\"value\":\"name\"},{\"name\":\"class\",\"value\":\"\"}]", Content = "Name", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, NumOrder = 1 },
                    new MozaicBootstrapComponent { Tag = "input", UIC = "form|input-text", Attributes = "[{\"name\":\"type\",\"value\":\"text\"},{\"name\":\"name\",\"value\":\"name\"},{\"name\":\"value\",\"value\":\"\"},{\"name\":\"class\",\"value\":\"form-control\"},{\"name\":\"id\",\"value\":\"name\"},{\"name\":\"autofocus\",\"value\":\"autofocus\"},{\"name\":\"tabindex\",\"value\":\"1\"}]", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, ElmId = "name", NumOrder = 2 },
                    new MozaicBootstrapComponent { Tag = "label", UIC = "form|label", Attributes = "[{\"name\":\"for\",\"value\":\"height\"},{\"name\":\"class\",\"value\":\"\"}]", Content = "Height", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, NumOrder = 3 },
                    new MozaicBootstrapComponent { Tag = "input", UIC = "form|input-number", Attributes = "[{\"name\":\"type\",\"value\":\"number\"},{\"name\":\"name\",\"value\":\"height\"},{\"name\":\"value\",\"value\":\"\"},{\"name\":\"class\",\"value\":\"form-control\"},{\"name\":\"tabindex\",\"value\":\"2\"},{\"name\":\"id\",\"value\":\"height\"}]", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, ElmId = "height", NumOrder = 4 },
                    new MozaicBootstrapComponent { Tag = "label", UIC = "form|label", Attributes = "[{\"name\":\"for\",\"value\":\"width\"},{\"name\":\"class\",\"value\":\"\"}]", Content = "Width", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, NumOrder = 5 },
                    new MozaicBootstrapComponent { Tag = "input", UIC = "form|input-number", Attributes = "[{\"name\":\"type\",\"value\":\"number\"},{\"name\":\"name\",\"value\":\"width\"},{\"name\":\"value\",\"value\":\"\"},{\"name\":\"class\",\"value\":\"form-control\"},{\"name\":\"id\",\"value\":\"width\"},{\"name\":\"tabindex\",\"value\":\"3\"}]", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, ElmId = "width", NumOrder = 6 },
                    new MozaicBootstrapComponent { Tag = "label", UIC = "form|label", Attributes = "[{\"name\":\"for\",\"value\":\"length\"},{\"name\":\"class\",\"value\":\"mbe-active\"}]", Content = "Length", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, NumOrder = 7 },
                    new MozaicBootstrapComponent { Tag = "input", UIC = "form|input-number", Attributes = "[{\"name\":\"type\",\"value\":\"number\"},{\"name\":\"name\",\"value\":\"length\"},{\"name\":\"value\",\"value\":\"\"},{\"name\":\"class\",\"value\":\"form-control\"},{\"name\":\"id\",\"value\":\"length\"},{\"name\":\"tabindex\",\"value\":\"5\"}]", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, ElmId = "length", NumOrder = 8 },
                    new MozaicBootstrapComponent { Tag = "button", UIC = "controls|button", Attributes = "[{\"name\":\"type\",\"value\":\"submit\"},{\"name\":\"class\",\"value\":\"btn active btn-primary\"},{\"name\":\"name\",\"value\":\"button\"},{\"name\":\"value\",\"value\":\"create\"},{\"name\":\"id\",\"value\":\"create\"}]", Content = "Create", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, ElmId = "create", NumOrder = 9 },
                    new MozaicBootstrapComponent { Tag = "button", UIC = "controls|button", Attributes = "[{\"name\":\"type\",\"value\":\"submit\"},{\"name\":\"class\",\"value\":\"btn btn-default active\"},{\"name\":\"name\",\"value\":\"button\"},{\"name\":\"value\",\"value\":\"back\"},{\"name\":\"id\",\"value\":\"back\"}]", Content = "Back", ParentComponent = newFormComponent, MozaicBootstrapPage = newPage, ElmId = "back", NumOrder = 10 },
                    editFormComponent,
                    new MozaicBootstrapComponent { Tag = "h1", UIC = "text|heading", Attributes = "[{\"name\":\"class\",\"value\":\"\"}]", Content = "Edit table", MozaicBootstrapPage = editPage, NumOrder = 1 },
                    new MozaicBootstrapComponent { Tag = "label", UIC = "form|label", Attributes = "[{\"name\":\"for\",\"value\":\"name\"},{\"name\":\"class\",\"value\":\"\"}]", Content = "Name", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, NumOrder = 1 },
                    new MozaicBootstrapComponent { Tag = "input", UIC = "form|input-text", Attributes = "[{\"name\":\"type\",\"value\":\"text\"},{\"name\":\"name\",\"value\":\"name\"},{\"name\":\"value\",\"value\":\"\"},{\"name\":\"class\",\"value\":\"form-control\"},{\"name\":\"id\",\"value\":\"name\"},{\"name\":\"autofocus\",\"value\":\"autofocus\"},{\"name\":\"tabindex\",\"value\":\"1\"}]", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, ElmId = "name", NumOrder = 2 },
                    new MozaicBootstrapComponent { Tag = "label", UIC = "form|label", Attributes = "[{\"name\":\"for\",\"value\":\"height\"},{\"name\":\"class\",\"value\":\"\"}]", Content = "Height", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, NumOrder = 3 },
                    new MozaicBootstrapComponent { Tag = "input", UIC = "form|input-number", Attributes = "[{\"name\":\"type\",\"value\":\"number\"},{\"name\":\"name\",\"value\":\"height\"},{\"name\":\"value\",\"value\":\"\"},{\"name\":\"class\",\"value\":\"form-control\"},{\"name\":\"tabindex\",\"value\":\"2\"},{\"name\":\"id\",\"value\":\"height\"}]", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, ElmId = "height", NumOrder = 4 },
                    new MozaicBootstrapComponent { Tag = "label", UIC = "form|label", Attributes = "[{\"name\":\"for\",\"value\":\"width\"},{\"name\":\"class\",\"value\":\"\"}]", Content = "Width", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, NumOrder = 5 },
                    new MozaicBootstrapComponent { Tag = "input", UIC = "form|input-number", Attributes = "[{\"name\":\"type\",\"value\":\"number\"},{\"name\":\"name\",\"value\":\"width\"},{\"name\":\"value\",\"value\":\"\"},{\"name\":\"class\",\"value\":\"form-control\"},{\"name\":\"id\",\"value\":\"width\"},{\"name\":\"tabindex\",\"value\":\"3\"}]", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, ElmId = "width", NumOrder = 6 },
                    new MozaicBootstrapComponent { Tag = "label", UIC = "form|label", Attributes = "[{\"name\":\"for\",\"value\":\"length\"},{\"name\":\"class\",\"value\":\"\"}]", Content = "Length", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, NumOrder = 7 },
                    new MozaicBootstrapComponent { Tag = "input", UIC = "form|input-number", Attributes = "[{\"name\":\"type\",\"value\":\"number\"},{\"name\":\"name\",\"value\":\"length\"},{\"name\":\"value\",\"value\":\"\"},{\"name\":\"class\",\"value\":\"form-control\"},{\"name\":\"id\",\"value\":\"length\"},{\"name\":\"tabindex\",\"value\":\"5\"}]", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, ElmId = "length", NumOrder = 8 },
                    new MozaicBootstrapComponent { Tag = "button", UIC = "controls|button", Attributes = "[{\"name\":\"type\",\"value\":\"submit\"},{\"name\":\"class\",\"value\":\"btn active btn-primary mbe-active\"},{\"name\":\"name\",\"value\":\"button\"},{\"name\":\"value\",\"value\":\"update\"},{\"name\":\"id\",\"value\":\"update\"}]", Content = "Update", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, ElmId = "update", NumOrder = 9 },
                    new MozaicBootstrapComponent { Tag = "button", UIC = "controls|button", Attributes = "[{\"name\":\"type\",\"value\":\"submit\"},{\"name\":\"class\",\"value\":\"btn btn-default active\"},{\"name\":\"name\",\"value\":\"button\"},{\"name\":\"value\",\"value\":\"back\"},{\"name\":\"id\",\"value\":\"back\"}]", Content = "Back", ParentComponent = editFormComponent, MozaicBootstrapPage = editPage, ElmId = "back", NumOrder = 10 }
                );

                // tapestry
                var rootMeta = new TapestryDesignerMetablock { Name = "Root metablock", PositionX = 0, PositionY = 0, ParentApp = app, IsInitial = true };
                context.TapestryDesignerMetablocks.Add(rootMeta);

                var indexBlock = new TapestryDesignerBlock { Name = "Index", PositionX = 20, PositionY = 20, ParentMetablock = rootMeta, IsInitial = true, IsChanged = true };
                var newBlock = new TapestryDesignerBlock { Name = "New", PositionX = 200, PositionY = 20, ParentMetablock = rootMeta, IsChanged = true };
                var editBlock = new TapestryDesignerBlock { Name = "Edit", PositionX = 400, PositionY = 20, ParentMetablock = rootMeta, IsChanged = true };
                context.TapestryDesignerBlocks.AddOrUpdate(
                    indexBlock,
                    newBlock,
                    editBlock
                );

                context.SaveChanges();
                var indexBlockCommit = new TapestryDesignerBlockCommit { Name = "Index", AssociatedTableName = "Tables", Timestamp = DateTime.UtcNow, ParentBlock = indexBlock, AssociatedBootstrapPageIds = indexPage.Id.ToString() };
                var newBlockCommit = new TapestryDesignerBlockCommit { Name = "New", AssociatedTableName = "Tables", Timestamp = DateTime.UtcNow, ParentBlock = newBlock, AssociatedBootstrapPageIds = newPage.Id.ToString() };
                var editBlockCommit = new TapestryDesignerBlockCommit { Name = "Edit", AssociatedTableName = "Tables", Timestamp = DateTime.UtcNow, ParentBlock = editBlock, AssociatedBootstrapPageIds = editPage.Id.ToString() };
                context.TapestryDesignerBlockCommits.AddOrUpdate(
                    indexBlockCommit,
                    newBlockCommit,
                    editBlockCommit
                );

                var indexRRlist = new TapestryDesignerResourceRule { PositionX = 10, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = indexBlockCommit };

                var newRRname1 = new TapestryDesignerResourceRule { PositionX = 10, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = newBlockCommit };
                var newRRheight1 = new TapestryDesignerResourceRule { PositionX = 370, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = newBlockCommit };
                var newRRwidth1 = new TapestryDesignerResourceRule { PositionX = 730, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = newBlockCommit };
                var newRRlength1 = new TapestryDesignerResourceRule { PositionX = 1090, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = newBlockCommit };
                var newRRname2 = new TapestryDesignerResourceRule { PositionX = 10, PositionY = 80, Width = 350, Height = 60, ParentBlockCommit = newBlockCommit };
                var newRRheight2 = new TapestryDesignerResourceRule { PositionX = 370, PositionY = 80, Width = 350, Height = 60, ParentBlockCommit = newBlockCommit };
                var newRRwidth2 = new TapestryDesignerResourceRule { PositionX = 730, PositionY = 80, Width = 350, Height = 60, ParentBlockCommit = newBlockCommit };
                var newRRlength2 = new TapestryDesignerResourceRule { PositionX = 1090, PositionY = 80, Width = 350, Height = 60, ParentBlockCommit = newBlockCommit };

                var editRRname1 = new TapestryDesignerResourceRule { PositionX = 10, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = editBlockCommit };
                var editRRheight1 = new TapestryDesignerResourceRule { PositionX = 370, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = editBlockCommit };
                var editRRwidth1 = new TapestryDesignerResourceRule { PositionX = 730, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = editBlockCommit };
                var editRRlength1 = new TapestryDesignerResourceRule { PositionX = 1090, PositionY = 10, Width = 350, Height = 60, ParentBlockCommit = editBlockCommit };
                var editRRname2 = new TapestryDesignerResourceRule { PositionX = 10, PositionY = 80, Width = 350, Height = 60, ParentBlockCommit = editBlockCommit };
                var editRRheight2 = new TapestryDesignerResourceRule { PositionX = 370, PositionY = 80, Width = 350, Height = 60, ParentBlockCommit = editBlockCommit };
                var editRRwidth2 = new TapestryDesignerResourceRule { PositionX = 730, PositionY = 80, Width = 350, Height = 60, ParentBlockCommit = editBlockCommit };
                var editRRlength2 = new TapestryDesignerResourceRule { PositionX = 1090, PositionY = 80, Width = 350, Height = 60, ParentBlockCommit = editBlockCommit };
                context.TapestryDesignerResourceRules.AddOrUpdate(
                    indexRRlist,
                    newRRname1,
                    newRRheight1,
                    newRRwidth1,
                    newRRlength1,
                    newRRname2,
                    newRRheight2,
                    newRRwidth2,
                    newRRlength2,
                    editRRname1,
                    editRRheight1,
                    editRRwidth1,
                    editRRlength1,
                    editRRname2,
                    editRRheight2,
                    editRRwidth2,
                    editRRlength2
                );

                var resourceIndexUi = new TapestryDesignerResourceItem { Label = "list", TypeClass = "uiItem", PositionX = 172, PositionY = 32, ParentRule = indexRRlist, ComponentName = "list", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = indexPage };
                var resourceIndexDb = new TapestryDesignerResourceItem { Label = "Table: Tables", TypeClass = "attributeItem", PositionX = 23, PositionY = 32, ParentRule = indexRRlist, ComponentName = null, TableName = "Tables", ColumnName = null, IsBootstrap = false, BootstrapPage = null };

                var resourceNewNameUi1 = new TapestryDesignerResourceItem { Label = "name", TypeClass = "uiItem", PositionX = 191, PositionY = 11, ParentRule = newRRname1, ComponentName = "name", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = newPage };
                var resourceNewNameDb1 = new TapestryDesignerResourceItem { Label = "Tables.Name", TypeClass = "attributeItem", PositionX = 42, PositionY = 12, ParentRule = newRRname1, ComponentName = null, TableName = "Tables", ColumnName = "Name", IsBootstrap = false, BootstrapPage = null };
                var resourceNewHeightUi1 = new TapestryDesignerResourceItem { Label = "height", TypeClass = "uiItem", PositionX = 231, PositionY = 12, ParentRule = newRRheight1, ComponentName = "height", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = newPage };
                var resourceNewHeightDb1 = new TapestryDesignerResourceItem { Label = "Tables.Height", TypeClass = "attributeItem", PositionX = 40, PositionY = 11, ParentRule = newRRheight1, ComponentName = null, TableName = "Tables", ColumnName = "Height", IsBootstrap = false, BootstrapPage = null };
                var resourceNewWidthUi1 = new TapestryDesignerResourceItem { Label = "width", TypeClass = "uiItem", PositionX = 212, PositionY = 11, ParentRule = newRRwidth1, ComponentName = "width", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = newPage };
                var resourceNewWidthDb1 = new TapestryDesignerResourceItem { Label = "Tables.Width", TypeClass = "attributeItem", PositionX = 22, PositionY = 12, ParentRule = newRRwidth1, ComponentName = null, TableName = "Tables", ColumnName = "Width", IsBootstrap = false, BootstrapPage = null };
                var resourceNewLengthUi1 = new TapestryDesignerResourceItem { Label = "length", TypeClass = "uiItem", PositionX = 191, PositionY = 11, ParentRule = newRRlength1, ComponentName = "length", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = newPage };
                var resourceNewLengthDb1 = new TapestryDesignerResourceItem { Label = "Tables.Length", TypeClass = "attributeItem", PositionX = 0, PositionY = 11, ParentRule = newRRlength1, ComponentName = null, TableName = "Tables", ColumnName = "Length", IsBootstrap = false, BootstrapPage = null };
                var resourceNewNameUi2 = new TapestryDesignerResourceItem { Label = "name", TypeClass = "uiItem", PositionX = 32, PositionY = 32, ParentRule = newRRname2, ComponentName = "name", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = newPage };
                var resourceNewNameDb2 = new TapestryDesignerResourceItem { Label = "Tables.Name", TypeClass = "attributeItem", PositionX = 162, PositionY = 12, ParentRule = newRRname2, ComponentName = null, TableName = "Tables", ColumnName = "Name", IsBootstrap = false, BootstrapPage = null };
                var resourceNewHeightUi2 = new TapestryDesignerResourceItem { Label = "height", TypeClass = "uiItem", PositionX = 31, PositionY = 12, ParentRule = newRRheight2, ComponentName = "height", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = newPage };
                var resourceNewHeightDb2 = new TapestryDesignerResourceItem { Label = "Tables.Height", TypeClass = "attributeItem", PositionX = 161, PositionY = 12, ParentRule = newRRheight2, ComponentName = null, TableName = "Tables", ColumnName = "Height", IsBootstrap = false, BootstrapPage = null };
                var resourceNewWidthUi2 = new TapestryDesignerResourceItem { Label = "width", TypeClass = "uiItem", PositionX = 11, PositionY = 32, ParentRule = newRRwidth2, ComponentName = "width", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = newPage };
                var resourceNewWidthDb2 = new TapestryDesignerResourceItem { Label = "Tables.Width", TypeClass = "attributeItem", PositionX = 162, PositionY = 32, ParentRule = newRRwidth2, ComponentName = null, TableName = "Tables", ColumnName = "Width", IsBootstrap = false, BootstrapPage = null };
                var resourceNewLengthUi2 = new TapestryDesignerResourceItem { Label = "length", TypeClass = "uiItem", PositionX = 32, PositionY = 11, ParentRule = newRRlength2, ComponentName = "length", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = newPage };
                var resourceNewLengthDb2 = new TapestryDesignerResourceItem { Label = "Tables.Length", TypeClass = "attributeItem", PositionX = 179, PositionY = 31, ParentRule = newRRlength2, ComponentName = null, TableName = "Tables", ColumnName = "Length", IsBootstrap = false, BootstrapPage = null };

                var resourceEditNameUi1 = new TapestryDesignerResourceItem { Label = "name", TypeClass = "uiItem", PositionX = 32, PositionY = 32, ParentRule = editRRname1, ComponentName = "name", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = editPage };
                var resourceEditNameDb1 = new TapestryDesignerResourceItem { Label = "Tables.Name", TypeClass = "attributeItem", PositionX = 183, PositionY = 32, ParentRule = editRRname1, ComponentName = null, TableName = "Tables", ColumnName = "Name", IsBootstrap = false, BootstrapPage = null };
                var resourceEditHeightUi1 = new TapestryDesignerResourceItem { Label = "height", TypeClass = "uiItem", PositionX = 32, PositionY = 32, ParentRule = editRRheight1, ComponentName = "height", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = editPage };
                var resourceEditHeightDb1 = new TapestryDesignerResourceItem { Label = "Tables.Height", TypeClass = "attributeItem", PositionX = 161, PositionY = 11, ParentRule = editRRheight1, ComponentName = null, TableName = "Tables", ColumnName = "Height", IsBootstrap = false, BootstrapPage = null };
                var resourceEditWidthUi1 = new TapestryDesignerResourceItem { Label = "width", TypeClass = "uiItem", PositionX = 31, PositionY = 11, ParentRule = editRRwidth1, ComponentName = "width", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = editPage };
                var resourceEditWidthDb1 = new TapestryDesignerResourceItem { Label = "Tables.Width", TypeClass = "attributeItem", PositionX = 163, PositionY = 12, ParentRule = editRRwidth1, ComponentName = null, TableName = "Tables", ColumnName = "Width", IsBootstrap = false, BootstrapPage = null };
                var resourceEditLengthUi1 = new TapestryDesignerResourceItem { Label = "length", TypeClass = "uiItem", PositionX = 12, PositionY = 11, ParentRule = editRRlength1, ComponentName = "length", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = editPage };
                var resourceEditLengthDb1 = new TapestryDesignerResourceItem { Label = "Tables.Length", TypeClass = "attributeItem", PositionX = 139, PositionY = 12, ParentRule = editRRlength1, ComponentName = null, TableName = "Tables", ColumnName = "Length", IsBootstrap = false, BootstrapPage = null };
                var resourceEditNameUi2 = new TapestryDesignerResourceItem { Label = "name", TypeClass = "uiItem", PositionX = 191, PositionY = 11, ParentRule = editRRname2, ComponentName = "name", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = editPage };
                var resourceEditNameDb2 = new TapestryDesignerResourceItem { Label = "Tables.Name", TypeClass = "attributeItem", PositionX = 23, PositionY = 32, ParentRule = editRRname2, ComponentName = null, TableName = "Tables", ColumnName = "Name", IsBootstrap = false, BootstrapPage = null };
                var resourceEditHeightUi2 = new TapestryDesignerResourceItem { Label = "height", TypeClass = "uiItem", PositionX = 211, PositionY = 12, ParentRule = editRRheight2, ComponentName = "height", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = editPage };
                var resourceEditHeightDb2 = new TapestryDesignerResourceItem { Label = "Tables.Height", TypeClass = "attributeItem", PositionX = 41, PositionY = 12, ParentRule = editRRheight2, ComponentName = null, TableName = "Tables", ColumnName = "Height", IsBootstrap = false, BootstrapPage = null };
                var resourceEditWidthUi2 = new TapestryDesignerResourceItem { Label = "width", TypeClass = "uiItem", PositionX = 212, PositionY = 11, ParentRule = editRRwidth2, ComponentName = "width", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = editPage };
                var resourceEditWidthDb2 = new TapestryDesignerResourceItem { Label = "Tables.Width", TypeClass = "attributeItem", PositionX = 23, PositionY = 12, ParentRule = editRRwidth2, ComponentName = null, TableName = "Tables", ColumnName = "Width", IsBootstrap = false, BootstrapPage = null };
                var resourceEditLengthUi2 = new TapestryDesignerResourceItem { Label = "length", TypeClass = "uiItem", PositionX = 192, PositionY = 12, ParentRule = editRRlength2, ComponentName = "length", TableName = null, ColumnName = null, IsBootstrap = true, BootstrapPage = editPage };
                var resourceEditLengthDb2 = new TapestryDesignerResourceItem { Label = "Tables.Length", TypeClass = "attributeItem", PositionX = 19, PositionY = 32, ParentRule = editRRlength2, ComponentName = null, TableName = "Tables", ColumnName = "Length", IsBootstrap = false, BootstrapPage = null };

                context.TapestryDesignerResourceItems.AddOrUpdate(
                    resourceIndexUi,
                    resourceIndexDb,

                    resourceNewNameUi1,
                    resourceNewNameDb1,
                    resourceNewHeightUi1,
                    resourceNewHeightDb1,
                    resourceNewWidthUi1,
                    resourceNewWidthDb1,
                    resourceNewLengthUi1,
                    resourceNewLengthDb1,
                    resourceNewNameUi2,
                    resourceNewNameDb2,
                    resourceNewHeightUi2,
                    resourceNewHeightDb2,
                    resourceNewWidthUi2,
                    resourceNewWidthDb2,
                    resourceNewLengthUi2,
                    resourceNewLengthDb2,

                    resourceEditNameUi1,
                    resourceEditNameDb1,
                    resourceEditHeightUi1,
                    resourceEditHeightDb1,
                    resourceEditWidthUi1,
                    resourceEditWidthDb1,
                    resourceEditLengthUi1,
                    resourceEditLengthDb1,
                    resourceEditNameUi2,
                    resourceEditNameDb2,
                    resourceEditHeightUi2,
                    resourceEditHeightDb2,
                    resourceEditWidthUi2,
                    resourceEditWidthDb2,
                    resourceEditLengthUi2,
                    resourceEditLengthDb2
                );
                context.TapestryDesignerResourceConnections.AddOrUpdate(
                    new TapestryDesignerResourceConnection { Source = resourceIndexDb, SourceSlot = 0, Target = resourceIndexUi, TargetSlot = 0, ResourceRule = indexRRlist },

                    new TapestryDesignerResourceConnection { Source = resourceNewNameDb1, SourceSlot = 0, Target = resourceNewNameUi1, TargetSlot = 0, ResourceRule = newRRname1 },
                    new TapestryDesignerResourceConnection { Source = resourceNewHeightDb1, SourceSlot = 0, Target = resourceNewHeightUi1, TargetSlot = 0, ResourceRule = newRRheight1 },
                    new TapestryDesignerResourceConnection { Source = resourceNewWidthDb1, SourceSlot = 0, Target = resourceNewWidthUi1, TargetSlot = 0, ResourceRule = newRRwidth1 },
                    new TapestryDesignerResourceConnection { Source = resourceNewLengthDb1, SourceSlot = 0, Target = resourceNewLengthUi1, TargetSlot = 0, ResourceRule = newRRlength1 },
                    new TapestryDesignerResourceConnection { Source = resourceNewNameUi2, SourceSlot = 0, Target = resourceNewNameDb2, TargetSlot = 0, ResourceRule = newRRname2 },
                    new TapestryDesignerResourceConnection { Source = resourceNewHeightUi2, SourceSlot = 0, Target = resourceNewHeightDb2, TargetSlot = 0, ResourceRule = newRRheight2 },
                    new TapestryDesignerResourceConnection { Source = resourceNewWidthUi2, SourceSlot = 0, Target = resourceNewWidthDb2, TargetSlot = 0, ResourceRule = newRRwidth2 },
                    new TapestryDesignerResourceConnection { Source = resourceNewLengthUi2, SourceSlot = 0, Target = resourceNewLengthDb2, TargetSlot = 0, ResourceRule = newRRlength2 },

                    new TapestryDesignerResourceConnection { Source = resourceEditNameDb1, SourceSlot = 0, Target = resourceEditNameUi1, TargetSlot = 0, ResourceRule = editRRname1 },
                    new TapestryDesignerResourceConnection { Source = resourceEditHeightDb1, SourceSlot = 0, Target = resourceEditHeightUi1, TargetSlot = 0, ResourceRule = editRRheight1 },
                    new TapestryDesignerResourceConnection { Source = resourceEditWidthDb1, SourceSlot = 0, Target = resourceEditWidthUi1, TargetSlot = 0, ResourceRule = editRRwidth1 },
                    new TapestryDesignerResourceConnection { Source = resourceEditLengthDb1, SourceSlot = 0, Target = resourceEditLengthUi1, TargetSlot = 0, ResourceRule = editRRlength1 },
                    new TapestryDesignerResourceConnection { Source = resourceEditNameUi2, SourceSlot = 0, Target = resourceEditNameDb2, TargetSlot = 0, ResourceRule = editRRname2 },
                    new TapestryDesignerResourceConnection { Source = resourceEditHeightUi2, SourceSlot = 0, Target = resourceEditHeightDb2, TargetSlot = 0, ResourceRule = editRRheight2 },
                    new TapestryDesignerResourceConnection { Source = resourceEditWidthUi2, SourceSlot = 0, Target = resourceEditWidthDb2, TargetSlot = 0, ResourceRule = editRRwidth2 },
                    new TapestryDesignerResourceConnection { Source = resourceEditLengthUi2, SourceSlot = 0, Target = resourceEditLengthDb2, TargetSlot = 0, ResourceRule = editRRlength2 }
                );

                var indexWRnew = new TapestryDesignerWorkflowRule { Name = "New", PositionX = 40, PositionY = 20, Width = 760, Height = 200, ParentBlockCommit = indexBlockCommit };
                var indexWRedit = new TapestryDesignerWorkflowRule { Name = "Edit", PositionX = 40, PositionY = 240, Width = 760, Height = 200, ParentBlockCommit = indexBlockCommit };
                var indexWRremove = new TapestryDesignerWorkflowRule { Name = "Remove", PositionX = 40, PositionY = 460, Width = 760, Height = 200, ParentBlockCommit = indexBlockCommit };
                var newWRcreate = new TapestryDesignerWorkflowRule { Name = "Create", PositionX = 40, PositionY = 20, Width = 760, Height = 200, ParentBlockCommit = newBlockCommit };
                var newWRback = new TapestryDesignerWorkflowRule { Name = "Back", PositionX = 40, PositionY = 240, Width = 760, Height = 200, ParentBlockCommit = newBlockCommit };
                var editWRupdate = new TapestryDesignerWorkflowRule { Name = "Update", PositionX = 40, PositionY = 20, Width = 760, Height = 200, ParentBlockCommit = editBlockCommit };
                var editWRback = new TapestryDesignerWorkflowRule { Name = "Back", PositionX = 40, PositionY = 240, Width = 760, Height = 200, ParentBlockCommit = editBlockCommit };
                context.TapestryDesignerWorkflowRules.AddOrUpdate(
                    indexWRnew,
                    indexWRedit,
                    indexWRremove,
                    newWRcreate,
                    newWRback,
                    editWRupdate,
                    editWRback
                );

                var indexSwimlaneNew = new TapestryDesignerSwimlane { SwimlaneIndex = 0, Height = 200, Roles = "", ParentWorkflowRule = indexWRnew };
                var indexSwimlaneEdit = new TapestryDesignerSwimlane { SwimlaneIndex = 0, Height = 200, Roles = "", ParentWorkflowRule = indexWRedit };
                var indexSwimlaneRemove = new TapestryDesignerSwimlane { SwimlaneIndex = 0, Height = 200, Roles = "", ParentWorkflowRule = indexWRremove };
                var newSwimlaneCreate = new TapestryDesignerSwimlane { SwimlaneIndex = 0, Height = 200, Roles = "", ParentWorkflowRule = newWRcreate };
                var newSwimlaneBack = new TapestryDesignerSwimlane { SwimlaneIndex = 0, Height = 200, Roles = "", ParentWorkflowRule = newWRback };
                var editSwimlaneUpdate = new TapestryDesignerSwimlane { SwimlaneIndex = 0, Height = 200, Roles = "", ParentWorkflowRule = editWRupdate };
                var editSwimlaneBack = new TapestryDesignerSwimlane { SwimlaneIndex = 0, Height = 200, Roles = "", ParentWorkflowRule = editWRback };
                context.TapestryDesignerSwimlane.AddOrUpdate(
                    indexSwimlaneNew,
                    indexSwimlaneEdit,
                    indexSwimlaneRemove,
                    newSwimlaneCreate,
                    newSwimlaneBack,
                    editSwimlaneUpdate,
                    editSwimlaneBack
                );

                var wfitemCreate1 = new TapestryDesignerWorkflowItem { Label = "create", TypeClass = "uiItem", PositionX = 54, PositionY = 52, ParentSwimlane = newSwimlaneCreate, ActionId = null, InputVariables = null, ComponentName = "create", Target = null, PageId = newPage.Id, IsBootstrap = true };
                var wfitemCreate2 = new TapestryDesignerWorkflowItem { Label = "Create DB Item", TypeClass = "actionItem", PositionX = 234, PositionY = 52, ParentSwimlane = newSwimlaneCreate, ActionId = 1004, InputVariables = "TableName=s$Tables", ComponentName = null, Target = null, PageId = null, IsBootstrap = false };
                var wfitemCreate3 = new TapestryDesignerWorkflowItem { Label = "Index", TypeClass = "targetItem", PositionX = 394, PositionY = 52, ParentSwimlane = newSwimlaneCreate, ActionId = null, InputVariables = null, ComponentName = null, Target = indexBlock, PageId = null, IsBootstrap = false };
                var wfitemNewBack1 = new TapestryDesignerWorkflowItem { Label = "back", TypeClass = "uiItem", PositionX = 54, PositionY = 72, ParentSwimlane = newSwimlaneBack, ActionId = null, InputVariables = null, ComponentName = "back", Target = null, PageId = newPage.Id, IsBootstrap = true };
                var wfitemNewBack2 = new TapestryDesignerWorkflowItem { Label = "Index", TypeClass = "targetItem", PositionX = 355, PositionY = 72, ParentSwimlane = newSwimlaneBack, ActionId = null, InputVariables = null, ComponentName = null, Target = indexBlock, PageId = null, IsBootstrap = false };
                var wfitemUpdate1 = new TapestryDesignerWorkflowItem { Label = "update", TypeClass = "uiItem", PositionX = 35, PositionY = 52, ParentSwimlane = editSwimlaneUpdate, ActionId = null, InputVariables = null, ComponentName = "update", Target = null, PageId = editPage.Id, IsBootstrap = true };
                var wfitemUpdate2 = new TapestryDesignerWorkflowItem { Label = "Update DB Item", TypeClass = "actionItem", PositionX = 194, PositionY = 52, ParentSwimlane = editSwimlaneUpdate, ActionId = 1007, InputVariables = "TableName=s$Tables;Id=__ModelId__", ComponentName = null, Target = null, PageId = null, IsBootstrap = false };
                var wfitemUpdate3 = new TapestryDesignerWorkflowItem { Label = "Index", TypeClass = "targetItem", PositionX = 414, PositionY = 52, ParentSwimlane = editSwimlaneUpdate, ActionId = null, InputVariables = null, ComponentName = null, Target = indexBlock, PageId = null, IsBootstrap = false };
                var wfitemEditBack1 = new TapestryDesignerWorkflowItem { Label = "back", TypeClass = "uiItem", PositionX = 34, PositionY = 72, ParentSwimlane = editSwimlaneBack, ActionId = null, InputVariables = null, ComponentName = "back", Target = null, PageId = editPage.Id, IsBootstrap = true };
                var wfitemEditBack2 = new TapestryDesignerWorkflowItem { Label = "Index", TypeClass = "targetItem", PositionX = 255, PositionY = 72, ParentSwimlane = editSwimlaneBack, ActionId = null, InputVariables = null, ComponentName = null, Target = indexBlock, PageId = null, IsBootstrap = false };
                var wfitemNew1 = new TapestryDesignerWorkflowItem { Label = "new", TypeClass = "uiItem", PositionX = 35, PositionY = 52, ParentSwimlane = indexSwimlaneNew, ActionId = null, InputVariables = null, ComponentName = "new", Target = null, PageId = indexPage.Id, IsBootstrap = true };
                var wfitemNew2 = new TapestryDesignerWorkflowItem { Label = "New", TypeClass = "targetItem", PositionX = 275, PositionY = 52, ParentSwimlane = indexSwimlaneNew, ActionId = null, InputVariables = null, ComponentName = null, Target = newBlock, PageId = null, IsBootstrap = false };
                var wfitemEdit1 = new TapestryDesignerWorkflowItem { Label = "list_edit", TypeClass = "uiItem", PositionX = 35, PositionY = 52, ParentSwimlane = indexSwimlaneEdit, ActionId = null, InputVariables = null, ComponentName = "list_edit", Target = null, PageId = indexPage.Id, IsBootstrap = true };
                var wfitemEdit2 = new TapestryDesignerWorkflowItem { Label = "Edit", TypeClass = "targetItem", PositionX = 234, PositionY = 52, ParentSwimlane = indexSwimlaneEdit, ActionId = null, InputVariables = null, ComponentName = null, Target = editBlock, PageId = null, IsBootstrap = false };
                var wfitemRemove1 = new TapestryDesignerWorkflowItem { Label = "list_remove", TypeClass = "uiItem", PositionX = 35, PositionY = 52, ParentSwimlane = indexSwimlaneRemove, ActionId = null, InputVariables = null, ComponentName = "list_remove", Target = null, PageId = indexPage.Id, IsBootstrap = true };
                var wfitemRemove2 = new TapestryDesignerWorkflowItem { Label = "Delete Item", TypeClass = "actionItem", PositionX = 235, PositionY = 52, ParentSwimlane = indexSwimlaneRemove, ActionId = 1010, InputVariables = "ItemId=__DeleteId__;TableName=s$Tables", ComponentName = null, Target = null, PageId = null, IsBootstrap = false };
                context.TapestryDesignerWorkflowItems.AddOrUpdate(
                    wfitemCreate1,
                    wfitemCreate2,
                    wfitemCreate3,
                    wfitemNewBack1,
                    wfitemNewBack2,
                    wfitemUpdate1,
                    wfitemUpdate2,
                    wfitemUpdate3,
                    wfitemEditBack1,
                    wfitemEditBack2,
                    wfitemNew1,
                    wfitemNew2,
                    wfitemEdit1,
                    wfitemEdit2,
                    wfitemRemove1,
                    wfitemRemove2
                );

                context.TapestryDesignerWorkflowConnection.AddOrUpdate(
                    new TapestryDesignerWorkflowConnection { Source = wfitemCreate1, SourceSlot = 0, Target = wfitemCreate2, TargetSlot = 0, WorkflowRule = newWRcreate },
                    new TapestryDesignerWorkflowConnection { Source = wfitemCreate2, SourceSlot = 0, Target = wfitemCreate3, TargetSlot = 0, WorkflowRule = newWRcreate },
                    new TapestryDesignerWorkflowConnection { Source = wfitemNewBack1, SourceSlot = 0, Target = wfitemNewBack2, TargetSlot = 0, WorkflowRule = newWRback },
                    new TapestryDesignerWorkflowConnection { Source = wfitemUpdate1, SourceSlot = 0, Target = wfitemUpdate2, TargetSlot = 0, WorkflowRule = editWRupdate },
                    new TapestryDesignerWorkflowConnection { Source = wfitemUpdate2, SourceSlot = 0, Target = wfitemUpdate3, TargetSlot = 0, WorkflowRule = editWRupdate },
                    new TapestryDesignerWorkflowConnection { Source = wfitemEditBack1, SourceSlot = 0, Target = wfitemEditBack2, TargetSlot = 0, WorkflowRule = editWRback },
                    new TapestryDesignerWorkflowConnection { Source = wfitemNew1, SourceSlot = 0, Target = wfitemNew2, TargetSlot = 0, WorkflowRule = indexWRnew },
                    new TapestryDesignerWorkflowConnection { Source = wfitemEdit1, SourceSlot = 0, Target = wfitemEdit2, TargetSlot = 0, WorkflowRule = indexWRedit },
                    new TapestryDesignerWorkflowConnection { Source = wfitemRemove1, SourceSlot = 0, Target = wfitemRemove2, TargetSlot = 0, WorkflowRule = indexWRremove }
                );
            }
        }
    }
}
