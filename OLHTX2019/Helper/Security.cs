using OLHTX2019.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OLHTX2019.Helper
{
    public static class Security
    {
        public static string GetCurrentUser()
        {
            return (string)HttpContext.Current.Session["User"];
        }
        public static void SetCurrentUser(string username)
        {
            HttpContext.Current.Session["User"] = username;
        }
        public static void ClearLoginSession()
        {

            HttpContext.Current.Session["User"] = null;
        }

        public static bool IsAdminLogin()
        {
            return HttpContext.Current.Session["User"] != null;
        }

        public static bool IsAllow(string[] inputCode)
        {
            return true;
        }
        
    }
}