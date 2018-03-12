using System;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FSS.Omnius.FrontEnd.Models;
using FSS.Omnius.Modules;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using PagedList;
using PagedList.Mvc;
using OneLogin.Saml;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Nexus.Service;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Configuration;
using reCAPTCHA.MVC;

namespace FSS.Omnius.FrontEnd.Controllers.Persona
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public static bool watchBadLogins = bool.Parse(WebConfigurationManager.AppSettings["PersonaWatchBadLoginAtempts"]);
        public static int captchaLimit = int.Parse(WebConfigurationManager.AppSettings["PersonaBadLoginCaptchaLimit"]);
        public static int waitInterval = int.Parse(WebConfigurationManager.AppSettings["PersonaBadLoginWaitInterval"]);
        public static int waitTimeout = int.Parse(WebConfigurationManager.AppSettings["PersonaBadLoginWaitTimeout"]);
        public static bool allowRegistration = bool.Parse(WebConfigurationManager.AppSettings["PersonaAllowRegistration"]);

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewData["showCaptcha"] = false;
            ViewData["allowRegistration"] = allowRegistration;

            if(watchBadLogins) {
                BadLoginCount record = DBEntities.instance.BadLoginCounts.SingleOrDefault(a => a.IP == HttpContext.Request.UserHostAddress);
                if(record != null) {
                    ViewData["showCaptcha"] = record.AttemptsCount >= captchaLimit;
                    if(record.AttemptsCount >= waitInterval) {
                        int timeout = (int)Math.Floor((double)record.AttemptsCount / waitInterval) * waitTimeout;
                        if (record.LastAtempt >= DateTime.Now.AddSeconds(-timeout)) {
                            ModelState.AddModelError("", string.Format("Please wait {0} seconds before next attempt", timeout));
                        }
                    }
                }
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LoginSaml(string SAMLResponse)
        {
            if (string.IsNullOrEmpty(SAMLResponse))
            {
                return RedirectToAction("RedirectToDefaultApp", "Home");
            }
            // Load configs and encode response
            AccountSettings accountSettings = new AccountSettings();
            Response response = new Response(accountSettings);
            response.LoadXmlFromBase64(SAMLResponse);

            // If is valid
            //if (response.IsValid())
            //{
            // Login user
            CORE core = HttpContext.GetCORE();
            core.User = core.Persona.GetUserByEmail(response.GetNameID());
            // If user is not found, send error
            if (core.User == null)
            {
                var ex = new OmniusException($"User with email {response.GetNameID()} not found.")
                {
                    SourceModule = OmniusLogSource.Persona
                };
                ex.Save();
                throw ex;
            }
            // Otherwise, login & redirect user home
            SignInManager.SignIn(core.User, true, true);
            return RedirectToAction("RedirectToDefaultApp", "Home");
            /*}
            else
            {
                WatchtowerLogger.Instance.LogEvent("Server recieved invalid response from saml endpoint.", 0);
                throw new UnauthorizedAccessException();
            }*/
        }

        //Overloading Login for system user 
        //GET Action
        [HttpGet]
        public async Task<ActionResult> LoginSystem(string userName, string password, string returnUrl)
        {
            var result = await SignInManager.PasswordSignInAsync(userName, password,false,false);
            if (result == SignInStatus.Success)
                    return RedirectToLocal(returnUrl);
            else
                return View();
        }



        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomCaptchaValidator]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            // Security checks
            string userIp = HttpContext.Request.UserHostAddress;
            DBEntities context = DBEntities.instance;
            BadLoginCount record = null;

            ViewData["showCaptcha"] = false;
            ViewData["allowRegistration"] = allowRegistration;
                
            // Check if bruteforce countermeasures are enabled
            if (watchBadLogins) {
                record = context.BadLoginCounts.SingleOrDefault(a => a.IP == userIp) ?? new BadLoginCount() { AttemptsCount = 0, IP = userIp };
                if(record.AttemptsCount == 0) { context.BadLoginCounts.Add(record); }
                
                // Show captcha?
                ViewData["showCaptcha"] = record.AttemptsCount >= captchaLimit;
                
                // Too fast attempts?
                if (record.AttemptsCount >= waitInterval) {
                    int timeout = (int)Math.Floor((double)record.AttemptsCount / waitInterval) * waitTimeout;
                    if(record.LastAtempt >= DateTime.Now.AddSeconds(-timeout)) {
                        ModelState.AddModelError("", string.Format("Please wait {0} seconds before next attempt", timeout));
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                if (watchBadLogins) {
                    record.LastAtempt = DateTime.Now;
                    record.AttemptsCount++;
                    context.SaveChanges();
                }

                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success: {
                        context.BadLoginCounts.Remove(record);
                        context.SaveChanges();

                        return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default: {
                        // Check if bruteforce countermeasures are enabled
                        if (watchBadLogins) {
                            record.AttemptsCount++;
                            record.LastAtempt = DateTime.Now;
                            context.SaveChanges();
                        
                            // Show captcha?
                            ViewData["showCaptcha"] = record.AttemptsCount >= captchaLimit;
                            
                            // Warn before too fast attempts?
                            if (record.AttemptsCount >= waitInterval) {
                                int timeout = (int)Math.Floor((double)record.AttemptsCount / waitInterval) * waitTimeout;
                                ModelState.AddModelError("", string.Format("Please wait {0} seconds before next attempt", timeout));
                            }
                        }

                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
        }

        //
        // GET: /Account/VerifyCode
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            if(!allowRegistration) {
                return new HttpNotFoundResult();
            }

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!allowRegistration) {
                return new HttpNotFoundResult();
            }

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    DisplayName = model.UserName,
                    Email = model.Email,
                    isLocalUser = true,
                    localExpiresAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    LastLogout = DateTime.UtcNow,
                    CurrentLogin = DateTime.UtcNow,
                    SecurityStamp = "b532ea85-8d2e-4ffb-8c64-86e8bfe363d7"
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // module Access permissions
                    DBEntities context = DBEntities.instance;
                    context.ModuleAccessPermissions.Add(new ModuleAccessPermission { User = context.Users.Single(u => u.UserName == user.UserName) });
                    context.SaveChanges();

                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // TODO: Email
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            IdentityResult result = UserManager.ChangePassword(ControllerContext.HttpContext.GetLoggedUser().Id, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
                return new RedirectToRouteResult("Master", new RouteValueDictionary(new { Controller = "Home", Action = "Details" }));

            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            if (userId == default(int) || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // TODO: Email
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == default(int))
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new User { Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [PersonaAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            
            CORE core = HttpContext.GetCORE();

            // Remove last app cookie and ip form database
            core.User.LastIp = "";
            core.User.LastAppCookie = "";
            DBEntities.instance.SaveChanges();
            
            core.Persona.LogOff(User.Identity.Name);
            
            Session.Clear();
            Session.Abandon();
            for(var i = 0; i < Response.Cookies.Count; i++) {
                Response.Cookies[i].Expires = DateTime.Now.AddDays(-1);
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [PersonaAuthorize]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, default(int))
            {
            }

            public ChallengeResult(string provider, string redirectUri, int userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public int UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != default(int))
                {
                    properties.Dictionary[XsrfKey] = UserId.ToString();
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        // GET: Users
        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
            
                return PartialView("AjaxIndex");
            }
            else
            {
              
                return View();
            }
        }

        //getUser JSON (for dataTable)
        public JsonResult loadData()
        {
            DBEntities e = DBEntities.instance;
            //var data = e.Users.OrderBy(a => a.UserName).ToList();
            var data = AjaxUsers.converToAjaxUsers(e.Users.ToList()).OrderBy(a => a.UserName);
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Detail(int id)
        {
            DBEntities e = DBEntities.instance;
            return View(e.Users.SingleOrDefault(x => x.Id == id));
        }

        public ActionResult Create()
        {
            SetPasswordViewModel model = new SetPasswordViewModel();
            User user = new User();
            model.User = user;

            //user je lokalní, není řešeno přes AD
            model.User.isLocalUser = true;

            //datumy se nastavují protože datetime v databázi a v c# mají rozdílné min.hodnoty
            model.User.localExpiresAt = DateTime.UtcNow;
            model.User.LastLogin = DateTime.UtcNow;
            model.User.CurrentLogin = DateTime.UtcNow;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateLocalUser(SetPasswordViewModel model)
        {
            IdentityResult result = null;

            model.User.isLocalUser = true;

            if (ModelState.IsValid)
            {
                result = await UserManager.CreateAsync(model.User, model.NewPassword);
                if (result.Succeeded)
                    return RedirectToAction("Index");
            }

            if (result != null)
                AddErrors(result);

            return View("Create", model);
        }

        public ActionResult Update(int id)
        {
            DBEntities e = DBEntities.instance;
            User u = e.Users.SingleOrDefault(x => x.Id == id);
            return View(u);
        }

        [HttpPost]
        public ActionResult Edit(User model)
        {
            if (ModelState.IsValid)
            {
                DBEntities e = DBEntities.instance;
                User user = e.Users.SingleOrDefault(x => x.Id == model.Id);
                e.Users.AddOrUpdate(user, model);
                e.SaveChanges();
                ViewBag.ShowTable = false;
                return RedirectToAction("Index");
            }

            ViewBag.ShowTable = true;
            return PartialView("Update", model);
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = DBEntities.instance;
            e.Users.Remove(e.Users.SingleOrDefault(x => x.Id == id));
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult SyncFromAD()
        {
            try
            {
                DBEntities e = DBEntities.instance;
                NexusLdapService service = new NexusLdapService();

                JToken ldapUsers = service.GetUsers();
                foreach (JToken ldapUser in ldapUsers)
                {
                    string username = (string)ldapUser["samaccountname"];
                    if (ldapUser["samaccountname"] == null)
                        continue;
                    if (e.Users.Any(u => u.UserName == username))
                    {
                        Logger.Log.Info($"SyncAD: skipping user {username}");
                        continue;
                    }

                    try
                    {
                        User user = new User
                        {
                            UserName = username,
                            DisplayName = string.IsNullOrWhiteSpace((string)ldapUser["displayname"]) ? username : (string)ldapUser["displayname"],
                            Email = (string)ldapUser["mail"],
                            Address = "",
                            Company = "",
                            Department = "",
                            Team = "",
                            Job = "",
                            WorkPhone = "",
                            MobilPhone = "",
                            LastLogin = (long)ldapUser["lastlogon"] != 0 ? DateTime.FromFileTime((long)ldapUser["lastlogon"]) : new DateTime(1970, 1, 1),
                            CurrentLogin = DateTime.UtcNow,

                            ModuleAccessPermission = new ModuleAccessPermission(),

                            isLocalUser = false,
                            localExpiresAt = DateTime.UtcNow.AddMonths(1)
                        };
                        e.Users.Add(user);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception($"LDAP: error in creating user '{username}'", ex);
                    }
                }
                e.SaveChanges();
            }
            catch(Exception ex)
            {
                OmniusException.Log(ex, OmniusLogSource.Persona);
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult SyncFromWSO()
        {
            var core = new CORE();
            var db = DBEntities.instance;

            NexusWSService webService = new NexusWSService();
            object[] parameters = new[] { "Auction_User" };
            JToken results = webService.CallWebService("RWE_WSO_SOAP", "getUserListOfRole", parameters);
            var x = results.Children().First().Value<String>();

            //get the list of users names and add it to the list.
            IEnumerable<String> usersNames = results.Children().Values().Select(y => y.Value<String>());

            List<User> listUsers = new List<User>();

            //iterate list of usersNames and make USerObject
            foreach (string userName in usersNames)
            {
                object[] param = new[] { userName, null };
                JToken userClaim = webService.CallWebService("RWE_WSO_SOAP", "getUserClaimValues", param);
                User newUser = new User();
                newUser.isLocalUser = false;
                newUser.localExpiresAt = DateTime.Today;//for test
                newUser.LastLogin = DateTime.Today;
                newUser.CurrentLogin = DateTime.Today;
                newUser.EmailConfirmed = false;
                newUser.PhoneNumberConfirmed = false;
                newUser.TwoFactorEnabled = false;
                newUser.LockoutEnabled = false;
                newUser.AccessFailedCount = 0;
                newUser.isActive = true;
                foreach (JToken property in userClaim.Children())
                {
                    var a = (property.Children().Single(c => (c as JProperty).Name == "claimUri") as JProperty).Value.ToString();

                    switch (a)
                    {
                        case "email":
                            var email = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
                            newUser.Email = email;
                            newUser.UserName = email;
                            break;

                        case "http://wso2.org/claims/mobile":
                            var mobile = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
                            newUser.MobilPhone = mobile;
                            break;

                        case "http://wso2.org/claims/organization":
                            var organization = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
                            newUser.Company = organization;
                            break;

                        case "fullname":
                            var fullname = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
                            newUser.DisplayName = fullname;
                            break;
                        //SET ROLES FOR this newly created USER
                        case "http://wso2.org/claims/role":
                            var roles = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString().Split(',').Where(r => r.Substring(0, 8) == "Auction_").Select(e => e.Remove(0, 8));
                            foreach (string role in roles)
                            {
                                PersonaAppRole approle = db.AppRoles.SingleOrDefault(r => r.Name == role && r.ApplicationId == core.Application.Id);
                                if (approle == null)
                                {
                                    db.AppRoles.Add(new PersonaAppRole() { Name = role, Application = core.Application, Priority = 0 });
                                    db.SaveChanges();
                                }
                                //User_Role userRole = newUser.Roles.SingleOrDefault(ur => ur.AppRole == approle && ur.User == newUser);
                                if (approle != null && !newUser.Users_Roles.Contains(new User_Role { RoleName = approle.Name, Application = approle.Application, User = newUser }))
                                {
                                    newUser.Users_Roles.Add(new User_Role { RoleName = approle.Name, Application = approle.Application, User = newUser });
                                }
                            }
                            break;
                    }
                }
                listUsers.Add(newUser);
            }

            //Now we can cal the resfresh method from persona
            Modules.Persona.Persona.RefreshUsersFromWSO(listUsers, core);

            return new HttpStatusCodeResult(200);
        }
    }
}
