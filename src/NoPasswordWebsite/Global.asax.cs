using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using NoPasswordMembershipService;
using NoPasswordWebsite.Entities;

namespace NoPasswordWebsite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null) return;
            var decryptedCookie = FormsAuthentication.Decrypt(cookie.Value);
            if (decryptedCookie == null) return;
            using (var context = new UserContext())
            {
                var userRepository = new UserRepository(context);
                var identity = userRepository.Find(decryptedCookie.Name);
                var loginService = new LoginService(userRepository);
                if (loginService.CheckDate())
                {
                    var principal = new GenericPrincipal(identity, new string[] {"Member"});
                    Thread.CurrentPrincipal = HttpContext.Current.User = principal;
                }
                else
                {
                    FormsAuthentication.SignOut();
                }
            }
        }
    }
}