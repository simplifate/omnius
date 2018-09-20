using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2
{
    public abstract class Block : MarshalByRefObject
    {
        /// AppDomain
        //public Block(ConstructTransaction core)
        //{
        //    Entitron.Entitron.DefaultConnectionString = core.efConnectionString;
        //    _core = TapestryCOREobject.Create(core.Username, core.ApplicationName);
        //    _core.ModelId = core.ModelId;
        //    _core.DeleteId = core.DeleteId;
        //    _core.Data = core.Data;
        //    _core.HttpRequest = core.httpRequest;
        //    _threads = new Dictionary<int, Thread>();
        //}
        //~Block()
        //{
        //    _core.Destroy();
        //}
        public Block(COREobject core)
        {
            _core = core;
            _parallel = new Dictionary<int, Task>();
        }

        protected COREobject _core;
        private Dictionary<int, Task> _parallel;

        public string TargetName { get; set; }

        ///// <summary>
        ///// Returns TargetBlock name
        ///// </summary>
        //public void _Run(string methodName, string[] roleNames, Action<int, Block> _symbolAction)
        //{
        //    MethodInfo method = GetType().GetMethod(methodName);
        //    if (method == null)
        //        throw new TapestryLoadOmniusException("Method not found", TapestryLoadOmniusException.LoadTarget.Rule, null);

        //    method.Invoke(this, new object[] { roleNames, _symbolAction });
        //}

        /// AppDomain
        //public DestructTransaction _GetDestructData()
        //{
        //    return new DestructTransaction
        //    {
        //        BlockAttribute = GetType().GetCustomAttribute<BlockAttribute>(),
        //        HttpResponse = _core.HttpResponse,
        //        Data = _core.Data,
        //        JsonResults = JObject.FromObject(_core.Results).ToString(),
        //        CrossBlockRegistry = _core.CrossBlockRegistry
        //    };
        //}

        public virtual void INIT(string[] roleNames, Action<int, Block> _symbolAction)
        {
        }
        
        protected void _ParallelRun(int gatewayId, Action action)
        {
            Task newTask = Task.Run(action);
            _parallel.Add(gatewayId, newTask);
        }

        protected void _WaitForParallel(int id)
        {
            Task task = _parallel[id];
            task.Wait();
        }
    }
}
