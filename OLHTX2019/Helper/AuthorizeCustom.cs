using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using OLHTX2019.Models.EntityManager;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeCustom : AuthorizeAttribute 
    {
        public string[] Code { get; set; }

        public AuthorizeCustom() : base()
        {
            
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var currentUser = OLHTX2019.Helper.Security.GetCurrentUser();
            if (currentUser == null)
            {
                HttpContext.Current.Response.Redirect("~/Admin20191008/login");

                return false;
            }

            return OLHTX2019.Helper.Security.IsAllow(Code);
        }
        
    }
}