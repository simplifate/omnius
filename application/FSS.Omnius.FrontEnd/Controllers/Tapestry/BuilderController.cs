using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.CORE;
using Microsoft.Web.WebSockets;
using System.Net;
using T2 = FSS.Omnius.Modules.Tapestry2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Tapestry")]
    public class BuilderController : Controller
    {
        public ActionResult Index(FormCollection formParams)
        {
            JArray blockTree = JArray.Parse("[]");
            COREobject core = COREobject.i;

            if (Request.HttpMethod == "POST")
            {
                var context = core.Context;
                int blockId = int.Parse(formParams["blockId"]);
                var parentMetablock = context.TapestryDesignerBlocks.Include("ParentMetablock")
                    .First(c => c.Id == blockId).ParentMetablock;
                ViewData["blockId"] = blockId;
                if (parentMetablock == null)
                    ViewData["parentMetablockId"] = 0;
                else
                    ViewData["parentMetablockId"] = parentMetablock.Id;

                int appId = 0;
                int rootMetablockId = parentMetablock.Id;
                parentMetablock = context.TapestryDesignerMetablocks.Include("ParentMetablock").Include("ParentApp")
                                                                    .Where(c => c.Id == parentMetablock.Id).First();
                while (parentMetablock != null)
                {
                    rootMetablockId = parentMetablock.Id;
                    parentMetablock = context.TapestryDesignerMetablocks.Include("ParentMetablock").Include("ParentApp")
                        .Where(c => c.Id == parentMetablock.Id).First().ParentMetablock;
                }
                Application app = context.Applications.SingleOrDefault(a => a.TapestryDesignerMetablocks.Any(mb => mb.Id == rootMetablockId));

                ViewData["appId"] = app != null ? app.Id : appId;
                //ViewData["screenCount"] = context.TapestryDesignerBlocks.Find(blockId).Pages.Count();

                ViewData["currentUserId"] = core.User.Id;

                GetBlockTree(context.TapestryDesignerMetablocks.FirstOrDefault(m => m.Id == rootMetablockId), ref blockTree, 0);
            }
            else // TODO: remove after switching to real IDs
            {
                var context = COREobject.i.Context;
                ViewData["appId"] = 1;
                ViewData["blockId"] = 1;
                ViewData["parentMetablockId"] = 1;
                ViewData["currentUserId"] = core.User.Id;

                GetBlockTree(context.TapestryDesignerMetablocks.FirstOrDefault(m => m.Id == 1), ref blockTree, 0);
            }

            ViewData["blockTree"] = blockTree;
            return View();
        }

        private void GetBlockTree(TapestryDesignerMetablock metablock, ref JArray blockTree, int level)
        {
            JObject item = new JObject();
            item["Id"] = "";
            item["Name"] = metablock.Name;
            item["IsMetablock"] = true;
            item["Level"] = level;
            item["Items"] = JArray.Parse("[]");

            blockTree.Add(item);

            foreach (TapestryDesignerBlock block in metablock.Blocks)
            {
                JObject bi = new JObject();
                bi["Id"] = block.Id;
                bi["Name"] = block.Name;
                bi["IsMetablock"] = false;
                bi["Level"] = level;

                ((JArray)item["Items"]).Add(bi);
            }

            foreach (TapestryDesignerMetablock mb in metablock.Metablocks)
            {
                GetBlockTree(mb, ref blockTree, level + 1);
            }
        }

        public ActionResult RunDebug(int id, string blockName, string executor)
        {
            string menuPath = Server.MapPath("~/Views/Shared/_ApplicationMenu.cshtml");

            if (HttpContext.IsWebSocketRequest)
            {
                HttpContext.AcceptWebSocketRequest(new RunWebSocketHandler(COREobject.i.User.UserName, id, blockName.RemoveDiacritics(), executor));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.SwitchingProtocols;
                return null;
            }

            return RedirectToAction("Index");
        }
    }

    class RunWebSocketHandler : WebSocketHandler
    {
        private COREobject _core;
        private string _username;
        private int _applicationId;
        private string _blockName;
        private string _executor;
        
        private int _lastWfItemId;
        private bool _runAll;
        private bool _watchVars;
        private bool _step;
        private bool _stop;

        private Dictionary<string, string> _vars;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="blockName"></param>
        /// <param name="executor">null for INIT</param>
        public RunWebSocketHandler(string username, int appId, string blockName, string executor)
        {
            _username = username;
            _applicationId = appId;
            _blockName = blockName;
            _executor = executor;

            _lastWfItemId = -1;
            _runAll = false;
            _watchVars = true;
            _step = false;
            _stop = false;
        }

        public override void OnOpen()
        {
            Task task = Task.Run(() => {
                _core = COREobject.Create(_username, _applicationId);
                Build();
                Run();
                Close();
            });
        }

        public override void OnClose()
        {
            _stop = true;
        }

        public override void OnError()
        {
            base.OnError();
        }

        public override void OnMessage(string message)
        {
            var jMessage = JObject.Parse(message);

            switch(jMessage["action"].ToObject<string>())
            {
                // run
                case "runAll":
                    _runAll = true;
                    _watchVars = false;
                    _step = true;
                    return;

                // run with watching
                case "fastForward":
                    _runAll = true;
                    _watchVars = true;
                    _step = true;
                    return;

                // next
                case "step":
                    _step = true;
                    return;

                // stop
                case "stop":
                    _stop = true;
                    return;

                // set var
                case "addVars":
                    foreach (JProperty item in jMessage["data"])
                    {
                        string value = item.Value.ToObject<string>();
                        object realValue = null;

                        // static
                        if (value.Length >= 2 && value[1] == '$')
                        {
                            char identify = value[0];
                            value = value.Substring(2);

                            switch (identify)
                            {
                                case 's':
                                    realValue = value;
                                    break;
                                case 'b':
                                    realValue = Convert.ToBoolean(value);
                                    break;
                                case 'd':
                                    realValue = DateTime.Parse(value);
                                    break;
                                case 'f':
                                    realValue = Convert.ToDouble(value);
                                    break;
                                case 'c':
                                    realValue = Convert.ToDecimal(value);
                                    break;
                                // i, l
                                default:
                                    realValue = Convert.ToInt32(value);
                                    break;
                            }
                        }
                        // var
                        else
                        {
                            var splitted = value.Split('.');
                            if (_core.Data.ContainsKey(splitted[0]))
                                realValue = T2.Extend.GetChained(_core.Data[splitted[0]], splitted.Skip(1).ToArray());
                        }

                        _core.Data[item.Name] = realValue;
                    }
                    return;

                // get var
                case "getVar":
                    return;

                default:
                    throw new Exception("Unknown action");
            }
        }

        private void SendObject(object item)
        {
            if (item.GetType().Namespace == null)
            {
                JObject result = new JObject();
                foreach(var property in item.GetType().GetProperties())
                {
                    if (property.PropertyType.GenericTypeArguments.Length > 0)
                        result.Add(property.Name, JObject.FromObject(property.GetValue(item)));
                    else
                        result.Add(property.Name, new JValue(property.GetValue(item)));
                }

                Send(result.ToString());
                return;
            }

            throw new ArgumentException("Unknown type");
        }

        private void Build()
        {
            if (_core.Application.TapestryChangedSinceLastBuild)
            {
                SendObject(new { @action = "building" });

                var progressHandler = new ModalProgressHandler<EModule>((s) => { });
                progressHandler.Section(EModule.Tapestry, "Generating");

                var generator = new T2.Services.TapestryGenerateService(_core.Context, _core.Application, progressHandler, false);
                generator.Generate();

                _core.Application.TapestryChangedSinceLastBuild = false;
                _core.Context.SaveChanges();
            }
        }

        private void Run()
        {
            try
            {
                var tapestry = new T2.Tapestry(_core);
                var target = tapestry.innerRun(_blockName, _executor, handleDebugStep);

                SendObject(new { @action = "end", target });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                SendObject(new { @action = "error", @wfItemId = _lastWfItemId, @message = ex.Message });
            }
        }

        private void handleDebugStep(int wfItemId, T2.Block block)
        {
            _lastWfItemId = wfItemId;
            
            if (_watchVars)
            {
                // get vars
                string methodName = T2.Tapestry.MethodName(_executor);
                var vars = block.GetType().GetProperties()
                    .Where(p => p.Name.StartsWith(methodName) && p.GetCustomAttributes(typeof(T2.IsVariableAttribute), false) != null)
                    .ToDictionary(p => p.Name.Split(new string[] { "__" }, StringSplitOptions.None)[1], p => p.GetValue(block))
                    .Union(_core.Data)
                    .Union(_core.Results)
                    .ToDictionary(p => p.Key, p => p.Value?.ToString());

                // compare
                if (_vars == null)
                    _vars = vars;
                else
                {

                    vars = vars.Where(p => _vars.ContainsKey(p.Key) && _vars[p.Key] != p.Value).ToDictionary(p => p.Key, p => p.Value);
                    foreach(var pair in vars)
                    {
                        _vars[pair.Key] = pair.Value;
                    }
                }

                // waiting for user
                SendObject(new { @action = "wait", wfItemId, @data = vars });
            }
            if (!_runAll)
            {
                wait();
            }
            
            // continue
            SendObject(new { @action = "running", wfItemId });
        }

        private void wait()
        {
            while (!_step && !_stop)
            {
                Thread.Sleep(200);
            }

            if (_step)
                _step = false;
            if (_stop)
                throw new Exception("stopped");
        }
    }
}
