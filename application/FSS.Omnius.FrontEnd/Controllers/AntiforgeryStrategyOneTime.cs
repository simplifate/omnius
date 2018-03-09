using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;

namespace FSS.Omnius.FrontEnd.Controllers
{
    public class AntiforgeryStrategyOneTime : IAntiForgeryAdditionalDataProvider
    {
        public string GetAdditionalData(HttpContextBase context)
        {
            if (context.Error == null) {
                if (context.Session["ValidAntiforgeryTokens"] == null) {
                    context.Session.Add("ValidAntiforgeryTokens", new List<string>());
                }

                using (RandomNumberGenerator rng = new RNGCryptoServiceProvider()) {
                    byte[] tokenData = new byte[32];
                    rng.GetBytes(tokenData);

                    var token = Convert.ToBase64String(tokenData);
                    ((List<string>)context.Session["ValidAntiforgeryTokens"]).Add(token);

                    return token;
                }
            }
            return "";
        }

        public bool ValidateAdditionalData(HttpContextBase context, string additionalData)
        {
            if (context.Session["ValidAntiforgeryTokens"] == null) {
                return false;
            }

            if (((List<string>)context.Session["ValidAntiforgeryTokens"]).Contains(additionalData)) {
                ((List<string>)context.Session["ValidAntiforgeryTokens"]).Remove(additionalData);
                return true;
            }

            return false;
        }
    }
}