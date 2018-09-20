using System;
using System.Collections.Generic;
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
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Persona;
using FSS.Omnius.Modules.Tapestry2;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace FSS.Omnius.Controllers.Tapestry2
{
    public class RestController : Controller
    {
        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

        public ActionResult Index(string path)
        {
            COREobject core = COREobject.i;
            DBEntities masterContext = core.Context;
            JToken response = new JObject();
            (string appName, string apiName) = path.Split('/');
            core.Application = masterContext.Applications.SingleOrDefault(a => a.Name == appName);

            // authentication
            if (core.User == null && !TryBasicAuth())
            {
                Response.StatusCode = 401;
                Response.Headers.Remove("WWW-Authenticate");
                Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Omnius\"");
                return new EmptyResult();
            }

            // Zkusme najít vyhovující api
            API api = masterContext.APIs.Where(a => a.Name == apiName).FirstOrDefault();
            if (api == null)
            {
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
            if (!path.EndsWith("/"))
                path += "/";
            path = path.Substring(path.IndexOf('/'));

            bool isMatch = false;
            foreach (PathDef pd in urlList[(int)api.Id])
            {
                if (pd.rx.IsMatch(path))
                {
                    // Předáme proměnné do form collection
                    if (pd.vars.Count() > 0)
                    {
                        Match m = pd.rx.Match(path);

                        foreach (KeyValuePair<int, string> var in pd.vars)
                        {
                            core.Data.Add(var.Value, m.Groups[var.Key].Value);
                        }
                    }
                    isMatch = true;
                    break;
                }
            }

            if (!isMatch)
            {
                Response.StatusCode = 404;
                return new EmptyResult();
            }

            // Zpracujeme GET proměnné
            foreach (string key in Request.QueryString.AllKeys)
            {
                core.Data.Add(key, Request.QueryString[key]);
            }

            // Zpracujeme POST proměnné
            foreach (string key in Request.Form)
            {
                core.Data.Add(key, Request.Form[key]);
            }

            // Zpracujeme body
            Stream req = Request.InputStream;
            req.Seek(0, SeekOrigin.Begin);
            string jsonString = new StreamReader(req).ReadToEnd();
            core.Data.Add("__RequestBody__", jsonString);

            // JSON-RPC?
            JToken rpc = null;
            if (!string.IsNullOrEmpty(jsonString))
            {
                try
                {
                    rpc = JToken.Parse(jsonString);
                    if (string.IsNullOrEmpty(rpc["jsonrpc"].ToObject<string>()))
                        rpc = null;
                }
                catch (Exception) { }
            }

            string wfName = Request.HttpMethod.ToUpper();
            if (rpc != null)
                wfName += $"_{(string)rpc["method"]}";

            try
            {
                var tapestry = new Modules.Tapestry2.Tapestry(core);
                JObject result = tapestry.jsonRun(apiName, wfName);

                if (rpc != null)
                {
                    response["jsonrpc"] = (string)rpc["jsonrpc"];
                    response["result"] = result;
                    response["id"] = (string)rpc["id"];
                }
                else
                {
                    response = result;
                }
            }
            catch (TapestryLoadOmniusException ex)
            {
                switch (ex.Target)
                {
                    case TapestryLoadOmniusException.LoadTarget.Assembly:
                        Response.StatusCode = 404;
                        response = BuildErrorResponse("Application not found", -32604, rpc);
                        break;
                    case TapestryLoadOmniusException.LoadTarget.Block:
                        Response.StatusCode = 404;
                        response = BuildErrorResponse("Api not found", -32601, rpc);
                        break;
                    case TapestryLoadOmniusException.LoadTarget.Rule:
                        Response.StatusCode = 405;
                        response = BuildErrorResponse("Method not found", -32601, rpc);
                        break;
                }
            }
            catch (TapestryAuthenticationOmniusException)
            {
                Response.StatusCode = 403;
                response = BuildErrorResponse("You are not allowed to use this endpoint and method", -32600, rpc);
            }
            catch (TapestryRunOmniusException ex)
            {
                Response.StatusCode = 500;
                BuildErrorResponse(ex.Message, -32603, rpc);
            }

            return Content(response.ToString(), "application/json");
        }

        private bool TryBasicAuth()
        {
            var authHeader = Request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                if (authHeaderVal.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && authHeaderVal.Parameter != null)
                {
                    return AuthenticateUser(authHeaderVal.Parameter);
                }
            }
            return false;
        }

        private bool AuthenticateUser(string credentials)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                int separator = credentials.IndexOf(':');
                string name = credentials.Substring(0, separator);
                string password = credentials.Substring(separator + 1);

                if (CheckUser(name, password))
                {
                    COREobject.i.User = Persona.GetAuthenticatedUser(name, false, Request);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private bool CheckUser(string name, string password)
        {
            var result = SignInManager.PasswordSignIn(name, password, false, shouldLockout: false);
            if (result == SignInStatus.Success)
            {
                return true;
            }
            return false;
        }

        private void BuildUrlList(int apiId, JToken def)
        {
            List<PathDef> urls = new List<PathDef>();
            foreach (JProperty path in def["paths"])
            {
                string url = path.Name;
                PathDef pd = new PathDef(url);

                if (path.Value["parameters"] != null)
                {
                    int i = 1;
                    foreach (JToken param in path.Value["parameters"])
                    {
                        if ((string)param["in"] == "path")
                        {
                            string paramName = (string)param["name"];
                            string rx = "";

                            switch ((string)param["type"])
                            {
                                case "string": rx = "([^/]+)"; break;
                                case "integer": rx = "(\\d+)"; break;
                            }
                            if (!string.IsNullOrEmpty(rx))
                            {
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

            urlList[apiId] = urls;
        }

        private JToken BuildErrorResponse(string message, int errorCode, JToken rpc)
        {
            JToken response = new JObject();

            if (rpc != null)
            {
                response["jsonrpc"] = (string)rpc["jsonrpc"];
                response["error"] = new JObject();
                response["error"]["code"] = errorCode;
                response["error"]["message"] = message;
                response["id"] = (string)rpc["id"];
            }
            else
            {
                response["status"] = "failed";
                response["message"] = JArray.FromObject(message);
            }

            return response;
        }

        private static Dictionary<int, List<PathDef>> urlList = new Dictionary<int, List<PathDef>>();
    }

    class PathDef
    {
        public string route;
        public Regex rx;
        public Dictionary<int, string> vars;

        public PathDef(string route)
        {
            this.route = route;
            this.vars = new Dictionary<int, string>();
        }
    }
}