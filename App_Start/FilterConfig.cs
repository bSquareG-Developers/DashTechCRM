using DashTechCRM.Models.User;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
namespace DashTechCRM
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
        public class _AuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
        {
            public void OnAuthentication(AuthenticationContext filterContext)
            {
                if (filterContext.HttpContext.Session["userInfo"] == null)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
            {
                if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
                {
                    //Redirecting the user to the Login View of Account Controller  
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                     { "controller", "Login" },
                     { "action", "Index" },
                     {"area","User" }
                    });
                }
                else
                {
                    //ConfigurationManager.AppSettings.Set("userRoel", "");
                }
            }
        }

        public class _AuthorizeAttribute : AuthorizeAttribute
        {
            private readonly string[] allowedroles;
            public _AuthorizeAttribute(params string[] roles)
            {
                this.allowedroles = roles;
            }
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                bool authorize = false;
                var user = httpContext.Session["userInfo"] as UserObject;

                string _role = user.Role;
                if (user != null)
                {
                    foreach (var role in allowedroles)
                    {
                        if (role == _role) return true;
                    }

                }

                return authorize;
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary
                   {
                   { "controller", "Login" },
                     { "action", "Index" },
                     {"area","User" }
                   });
            }

        }
    }
}
