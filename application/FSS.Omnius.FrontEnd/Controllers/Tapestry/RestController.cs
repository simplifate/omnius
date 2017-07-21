using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.FrontEnd;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using C = FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Controllers.Tapestry
{
    public class RestController : Controller
    {
        public static DateTime requestStart;
        public static DateTime startTime;
        public static DateTime prepareEnd;

        private C.CORE core;
        private DBEntities context;

        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        private static Dictionary<int, List<PathDef>> urlList = new Dictionary<int, List<PathDef>>();

        public ApplicationUserManager UserManager
        {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set {
                _signInManager = value;
            }
        }

        public ActionResult Index(string path)
        {
            RunController.startTime = DateTime.Now;

            JToken response = new JObject();
            List<string> fragments = path.Split('/').ToList();
            string appName = fragments[0];
            string apiName = fragments[1];
            NameValueCollection fc = new NameValueCollection();
            fc.Add(Request.Form);
            
            core = HttpContext.GetCORE();
            core.Entitron.AppName = appName;
            core.User = User.GetLogged(core);
            context = DBEntities.appInstance(core.Entitron.Application);
            var masterContext = DBEntities.instance;

            if(core.User == null && !TryBasicAuth()) {
                Response.StatusCode = 401;
                Response.Headers.Remove("WWW-Authenticate");
                Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Omnius\"");
                return new EmptyResult();
            }

            // Zkusme najít vyhovující api
            API api = masterContext.APIs.Where(a => a.Name == apiName).FirstOrDefault();
            if(api == null) {
                Response.StatusCode = 404;
                return new EmptyResult();
            }

            // Máme api - rozparsujeme definici
            Deserializer deserializer = new Deserializer();
            var defYaml = deserializer.Deserialize(new StringReader(api.Definition));
            JToken def = JToken.FromObject(defYaml);
            
            // sestavíme regulární výrazy pro cesty, pokud nejsou
            //if(!urlList.ContainsKey((int)api.Id)) {
                BuildUrlList((int)api.Id, def);
            //}

            // Vyhledáme správnou cestu
            if(!path.EndsWith("/")) {
                path += "/";
            }
            path = path.Substring(path.IndexOf('/'));

            bool isMatch = false;
            foreach(PathDef pd in urlList[(int)api.Id]) {
                if(pd.rx.IsMatch(path)) {
                    // Předáme proměnné do form collection
                    if (pd.vars.Count() > 0) {
                        Match m = pd.rx.Match(path);

                        foreach(KeyValuePair<int, string> var in pd.vars) {
                            fc.Add(var.Value, m.Groups[var.Key].Value);
                        }
                    }
                    isMatch = true;
                    break;
                }
            }

            if(!isMatch) {
                Response.StatusCode = 404;
                return new EmptyResult();
            }

            // Zpracujeme GET proměnné
            if(Request.QueryString.Count > 0) {
                foreach(string key in Request.QueryString.AllKeys) {
                    fc.Add(key, Request.QueryString[key]);
                }
            }

            // Zpracujeme body
           
                Stream req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string jsonString = new StreamReader(req).ReadToEnd();
                fc.Add("__RequestBody__", jsonString);
            
          
            
             string wfName = Request.HttpMethod.ToUpper();

            Application app = core.Entitron.Application.similarApp;
            Block block = getBlockWithResource(app.Id, apiName);

            if(block == null) {
                Response.StatusCode = 404;
                return new EmptyResult();
            }

            // Check if user has one of allowed roles, otherwise return 403
            if (!String.IsNullOrEmpty(block.RoleWhitelist)) {
                bool userIsAllowed = false;
                foreach (var role in block.RoleWhitelist.Split(',').ToList()) {
                    if (core.User.HasRole(role, core.Entitron.AppId))
                        userIsAllowed = true;
                }
                if (!userIsAllowed) {
                    Response.StatusCode = 403;
                    response["status"] = "failed";
                    response["message"] = new JArray("You are not allowed to use this endpoint");
                }
            }

            if(block.SourceTo_ActionRules.Where(r => r.ExecutedBy == wfName).Count() == 0) { // Neimplementovaná metoda
                Response.StatusCode = 405;
                response["status"] = "failed";
                response["message"] = new JArray("Method not allowed");
            }

            if (Response.StatusCode == 200) {
                Message message = new Message();
                var result = core.Tapestry.jsonRun(core.User, block, wfName, -1, fc, out message);
                if(message.Type == Modules.Tapestry.ActionResultType.Error) {
                    Response.StatusCode = 500;
                    response["status"] = "failed";
                    response["message"] = JArray.FromObject(message.Errors);
                }
                else {
                    response = result;
                }
            }

            return Content(response.ToString(), "application/json");
        }
        
        private bool TryBasicAuth()
        {
            var authHeader = Request.Headers["Authorization"];
            if (authHeader != null) {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                if (authHeaderVal.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && authHeaderVal.Parameter != null) {
                    return AuthenticateUser(authHeaderVal.Parameter);
                }
            }
            return false;
        }

        private bool AuthenticateUser(string credentials)
        {
            try {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                int separator = credentials.IndexOf(':');
                string name = credentials.Substring(0, separator);
                string password = credentials.Substring(separator + 1);

                if (CheckUser(name, password)) {
                    core.User = core.Persona.AuthenticateUser(name);
                    return true;
                }
                else {
                    return false;
                }
            }
            catch (FormatException) {
                return false;
            }
        }

        private bool CheckUser(string name, string password)
        {
            var result = SignInManager.PasswordSignIn(name, password, false, shouldLockout: false);
            if (result == SignInStatus.Success) {
                return true;
            }
            return false;
        }

        private Block getBlockWithResource(int appId, string blockName)
        {
            return context.Blocks
                    .Include(b => b.SourceTo_ActionRules)
                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == appId && b.Name == blockName);
        }

        private void BuildUrlList(int apiId, JToken def)
        {
            List<PathDef> urls = new List<PathDef>();
            foreach(JProperty path in def["paths"]) {
                string url = path.Name;
                PathDef pd = new PathDef(url);

                if(path.Value["parameters"] != null) {
                    int i = 1;
                    foreach(JToken param in path.Value["parameters"]) {
                        if((string)param["in"] == "path") {
                            string paramName = (string)param["name"];
                            string rx = "";

                            switch((string)param["type"]) {
                                case "string": rx = "([^/]+)"; break;
                                case "integer": rx = "(\\d+)"; break;
                            }
                            if (!string.IsNullOrEmpty(rx)) {
                                url = url.Replace($"{{{paramName}}}", rx);
                                pd.vars.Add(i, paramName);
                                i++;
                            }
                        }
                    }
                }
                url += !url.EndsWith("/") ? "/$" : "$";
                pd.rx = new Regex(url);

                urls.Add(pd);
            }
            if (urlList.ContainsKey(apiId)) {
                urlList[apiId] = urls;
            }
            else {
                urlList.Add(apiId, urls);
            }
        }
    }

    class PathDef
    {
        public string route;
        public Regex rx;
        public Dictionary<int, string> vars;

        public PathDef(string route) {
            this.route = route;
            this.vars = new Dictionary<int, string>();
        }
    }
}