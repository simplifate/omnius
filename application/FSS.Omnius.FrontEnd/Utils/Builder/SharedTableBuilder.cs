using FSS.Utils;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Utils.Builder
{

    public class SharedTableBuilder : BasicDispatchableObject
    {
        private Application App;
        private DBEntities DbContext;
        private CORE Core;
        
        /// <summary>
        /// Used for building shared tables
        /// </summary>
        /// <param name="core">CORE module instance</param>
        /// <param name="dbContext">Database context</param>
        /// <param name="app">Application to be builded</param>
        public SharedTableBuilder(CORE core, DBEntities dbContext, Application app)
        {
            this.Core = core;
            this.DbContext = dbContext;
            this.App = app;
        }

        /// <summary>
        /// Builds scheme (only if required)
        /// </summary>
        public void Build(bool force = false)
        {
            // Check if scheme is updated in editor
            if (force || this.App.EntitronChangedSinceLastBuild)
            {
                // Inform user that you're going to update scheme
                this.Dispatch(new { id = "entitron", type = "info", message = "proběhne aktualizace sdílených tabulek" });

                // If is updated, update scheme on server
                this.UpdateScheme();
            }
            else
            {
                // Inform user that you're not going to update scheme
                this.Dispatch(new { id = "entitron", type = "success", message = "databázi není potřeba aktualizovat" });
            }
        }

        /// <summary>
        /// Locks scheme (requires saving db context)
        /// </summary>
        private void LockScheme()
        {
            this.App.DbSchemeLocked = true;
        }

        /// <summary>
        /// Unlocks scheme (requires saving db context)
        /// </summary>
        private void UnlockScheme()
        {
            this.App.DbSchemeLocked = false;
        }

        /// <summary>
        /// Updates scheme
        /// </summary>
        private void UpdateScheme()
        {
            try
            {
                // Lock scheme
                LockScheme();
                DbContext.SaveChanges();

                // Give entitron id of app that we working on
                Entitron.Create(App);

                // Get commits
                var dbSchemeCommit = this.App.DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).FirstOrDefault();

                // If there are no commits
                if (dbSchemeCommit == null)
                    // Create new one?
                    dbSchemeCommit = new DbSchemeCommit();

                // If commit is uncomplete
                if (!dbSchemeCommit.IsComplete)
                    // Throw exception
                    throw new Exception("Pozor! Databázové schéma je špatně uložené, build nemůže pokračovat, protože by způsobil ztrátu dat!");

                // Generate database
                new DatabaseGenerateService().GenerateDatabase(dbSchemeCommit, this.Core, x => this.Dispatch(x));

                // Set that scheme is updated
                this.App.EntitronChangedSinceLastBuild = false;

                // Dispatch that it's builded successfully
                this.Dispatch(new { id = "entitron", type = "success", message = "proběhla aktualizace databáze" });
            }
            catch (Exception ex)
            {
                // If there was error dispatch it
                this.Dispatch(new { id = "entitron", type = "error", message = ex.Message, abort = true });
                // And also rethrow
                throw ex;
            }
            finally
            {
                // Unlock scheme
                this.UnlockScheme();

                // Update database
                this.DbContext.SaveChanges();
            }
        }
    }
}