//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FSS.Omnius.Modules.Entitron.Entity;
//using FSS.Omnius.Modules.Entitron.Entity.Persona;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

//namespace FSS.Omnius.Modules.Persona
//{
//    class ApplicationUserManager : UserManager<User, int>
//    {

//        public ApplicationUserManager(IUserStore<User, int> store)
//            : base(store)
//        {
//        }

//        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
//        {
//            var manager = new ApplicationUserManager(new UserStore<User, Iden_Role, int, UserLogin, Iden_User_Role, UserClaim>(context.Get<DBEntities>()));
//            // Configure validation logic for usernames
//            manager.UserValidator = new UserValidator<User, int>(manager)
//            {
//                AllowOnlyAlphanumericUserNames = false,
//                RequireUniqueEmail = false
//            };

//            // Configure validation logic for passwords
//            manager.PasswordValidator = new PasswordValidator
//            {
//                RequiredLength = 6,
//                RequireNonLetterOrDigit = false,
//                RequireDigit = true,
//                RequireLowercase = true,
//                RequireUppercase = false,
//            };

//            // Configure user lockout defaults
//            manager.UserLockoutEnabledByDefault = true;
//            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
//            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

//            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
//            // You can write your own provider and plug it in here.
//            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User, int>
//            {
//                MessageFormat = "Your security code is {0}"
//            });
//            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User, int>
//            {
//                Subject = "Security Code",
//                BodyFormat = "Your security code is {0}"
//            });
//            //manager.EmailService = new EmailService();
//            //manager.SmsService = new SmsService();
//            var dataProtectionProvider = options.DataProtectionProvider;
//            //if (dataProtectionProvider != null)
//            //{
//            //    manager.UserTokenProvider =
//            //        new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ASP.NET Identity"));
//            //}
//            return manager;
//        }
//    }
//}
