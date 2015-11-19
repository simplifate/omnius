using FSPOC_WebProject.Controllers;
using FSS.Omnius.BussinesObjects.Common;
using FSS.Omnius.BussinesObjects.DAL;
using FSS.Omnius.Entitron.Entity.Tapestry;
using Newtonsoft.Json;

namespace FSPOC_WebProject.Actions
{
    /// <summary>
    ///     POUZE PRO ILUSTRACI (SMAZAT)!!!!!
    /// </summary>
    internal class SaveEntity : ISaveEntity
    {
        public SaveEntity(IRepository<Workflow> repositoryWorkflow)
        {
            RepositoryWorkflow = repositoryWorkflow;
        }
        /// <summary>
        /// POUZE PRO ILUSTRACI !!!!
        /// </summary>
        private IRepository<Workflow> RepositoryWorkflow { get; set; }

        public ResultAction Run(object paramActin = null)
        {
            var testConnection = new TestConnection();
            if (paramActin != null)
            {
                testConnection = JsonConvert.DeserializeObject<TestConnection>(paramActin.ToString());
            }

            return new ResultAction
            {
                TypeResult    = TypeResult.Message,
                ResultMessage = testConnection != null ? $"Entita TestConnection byla ulozena s hodnotou Name property {testConnection.Name}" : "Entita byla ulozena"
            };
        }
    }
}