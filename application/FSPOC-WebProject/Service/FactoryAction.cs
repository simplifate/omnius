using System;
using FSPOC_WebProject.Actions;
using FSS.FSPOC.BussinesObjects.Common;
using FSS.FSPOC.BussinesObjects.Service;

namespace FSPOC_WebProject.Service
{
    public class FactoryAction : IFactoryAction
    {
        public FactoryAction(ISaveEntity saveEntity)
        {
            SaveEntity = saveEntity;
        }

        private ISaveEntity SaveEntity { get; set; }

        public IAction GetAction(int actionId)
        {
            if (actionId == 1)
            {
                return SaveEntity;
            }
            return new DefaultAction();
        }
    }

    /// <summary>
    /// POUZE ILUSTRACE!!!! 
    /// </summary>
    public class DefaultAction : IAction
    {
        public ResultAction Run(object paramActin = null)
        {
            return null;
        }
    }
}