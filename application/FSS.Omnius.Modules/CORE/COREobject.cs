using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Tapestry2;
using System.IO;

namespace FSS.Omnius.Modules.CORE
{
    public class COREobject : IModule
    {
        private COREobject()
        {
            Context = new DBEntities(this);

            Message = new Message(this);
            Data = new Dictionary<string, object>();
            CrossBlockRegistry = new Dictionary<string, object>();
            Results = new Dictionary<string, object>();
        }

        public DateTime RequestStart { get; set; }
        public User User { get; set; }
        public Application Application { get; set; }
        public Application ApplicationShared => Application.SystemApp();
        public DBEntities Context { get; set; }
        public DBEntities AppContext => Context;
        private DBConnection _entitron;
        public DBConnection Entitron
        {
            get
            {
                if (_entitron == null)
                    _entitron = new DBConnection(this);

                return _entitron;
            }
        }
        private DBConnection _entitronShared { get; set; }
        public DBConnection EntitronShared
        {
            get
            {
                if (_entitronShared == null)
                    _entitronShared = new DBConnection(this, true);

                return _entitronShared;
            }
        }

        public BlockAttribute BlockAttribute { get; set; }
        public string Executor { get; set; }
        public int ModelId { get; set; }
        public int DeleteId { get; set; }
        public ITranslator Translator { get; set; }
        public Message Message { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public Dictionary<string, object> CrossBlockRegistry { get; set; }
        public Dictionary<string, object> Results { get; set; }
        public bool WfCreateHttpResponse { get; set; }

        private object _lockLock = new object();
        private Dictionary<int, object> _locks = new Dictionary<int, object>();

        public Dictionary<string, byte[]> GetRequestFiles()
        {
            HttpRequest request = HttpContext.Current.Request;
            var files = new Dictionary<string, byte[]>();
            for (int i = 0; i < request.Files.Count; i++)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    request.Files[i].InputStream.CopyTo(ms);

                    files.Add(request.Files[i].FileName, ms.ToArray());
                }
            }

            return files;
        }
        public void HttpResponse(string fileName, string fileType, byte[] fileBinary)
        {
            WfCreateHttpResponse = true;
            var httpResponse = HttpContext.Current.Response;
            httpResponse.Clear();
            httpResponse.StatusCode = 200;
            httpResponse.ContentType = fileType;
            httpResponse.AddHeader("content-disposition", $"attachment; filename={fileName}");
            httpResponse.BinaryWrite(fileBinary);
            httpResponse.Flush();
            httpResponse.Close();
            httpResponse.End();
        }
        public void HttpResponse(string redirectUrl)
        {
            WfCreateHttpResponse = true;
            HttpContext.Current.Response.Redirect(redirectUrl);
        }

        public object Lock(int itemId)
        {
            lock (_lockLock)
            {
                if (!_locks.ContainsKey(itemId))
                    _locks.Add(itemId, new object());

                return _locks[itemId];
            }
        }

        private static COREobject CreateINIT(int? rHash = null)
        {
            rHash = rHash ?? requestHash;
            lock (_lock)
            {
                if (!_instances.ContainsKey(rHash.Value))
                {
                    COREobject core = new COREobject();
                    core.RequestStart = DateTime.UtcNow;
                    _instances.Add(rHash.Value, core);
                }
            }

            return _instances[rHash.Value];
        }
        public static COREobject Create()
        {
            COREobject core = CreateINIT();
            return core;
        }
        public static COREobject Create(string userName, string appName)
        {
            COREobject core = CreateINIT();
            core.Application = core.Context.Applications.SingleOrDefault(a => a.Name == appName);
            core.User = Persona.Persona.GetUser(userName, core.Application?.IsAllowedGuests ?? false);
            return core;
        }
        public static COREobject Create(string userName, int appId)
        {
            COREobject core = CreateINIT();
            core.Application = core.Context.Applications.Find(appId);
            core.User = Persona.Persona.GetUser(userName, core.Application.IsAllowedGuests);
            return core;
        }
        public static void Destroy()
        {
            lock (_lock)
            {
                _instances[requestHash].Context.Dispose();
                _instances.Remove(requestHash);
            }
        }

        private static int requestHash
        {
            get
            {
                try
                {
                    if (HttpContext.Current == null)
                        return 0;    //pro přístup z jiného vlákna
                    return HttpContext.Current.Request.GetHashCode();
                }
                catch (HttpException)
                {
                    return 0;
                }
            }
        }
        private static Dictionary<int, COREobject> _instances = new Dictionary<int, COREobject>();
        private static object _lock = new object();
        public static COREobject i
        {
            get
            {
                if (!_instances.ContainsKey(requestHash))
                {
                    lock (_lock)
                    {
                        _instances.Add(requestHash, new COREobject());
                    }
                }

                return _instances[requestHash];
            }
        }
    }
}
