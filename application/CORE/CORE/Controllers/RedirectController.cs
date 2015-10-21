using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CORE.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace CORE.Controllers
{
    public class RedirectController : Controller
    {
        public string Index(string modul, string app, string url, FormCollection fc)
        {
            DBEntities e = new DBEntities();
            Module module = e.Modules.SingleOrDefault(m => m.Name == modul);

            if (module == null || !module.IsEnabled)
                return null;

            using (var client = new HttpClient())
            {
                Task<string> responseString = null;
                string address = string.Format("{0}/{1}/{2}", module.Address, app, url);

                // POST
                if (Request.HttpMethod == "POST")
                {
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    foreach(string a in fc.Keys) { param.Add(a, fc[a]); }
                    var content = new FormUrlEncodedContent(param);
                    
                    var response = client.PostAsync(address, content);
                    responseString = response.Result.Content.ReadAsStringAsync();
                }
                // GET
                else if (Request.HttpMethod == "GET")
                {
                    responseString = client.GetStringAsync(address);
                }

                // get response
                try
                {
                    responseString.Wait();
                    return responseString.Result;
                }
                catch(HttpRequestException)
                {
                    // SERVICE NOT AVAILABLE
                    return "Service not Available";
                }
            }
        }

        public string Config(string modul, string url, FormCollection fc)
        {
            DBEntities e = new DBEntities();
            Module module = e.Modules.SingleOrDefault(m => m.Name == modul);

            if (module == null || !module.IsEnabled)
                return null;

            using (var client = new HttpClient())
            {
                Task<string> responseString = null;
                string address = string.Format("{0}/config/{1}", module.Address, url);

                // POST
                if (Request.HttpMethod == "POST")
                {
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    foreach (string a in fc.Keys) { param.Add(a, fc[a]); }
                    var content = new FormUrlEncodedContent(param);

                    var response = client.PostAsync(address, content);
                    responseString = response.Result.Content.ReadAsStringAsync();
                }
                // GET
                else if (Request.HttpMethod == "GET")
                {
                    responseString = client.GetStringAsync(address);
                }

                // get response
                try
                {
                    responseString.Wait();
                    return responseString.Result;
                }
                catch (AggregateException)
                {
                    // SERVICE NOT AVAILABLE
                    return "Service not Available";
                }
            }
        }
    }
}