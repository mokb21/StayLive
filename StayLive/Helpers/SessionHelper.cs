using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StayLive.Helpers
{
    public class SessionHelper
    {
        private const string _IsSessionInitializedSessionKey = "IsSessionInitialized";
        private const string _AccountIdSessionKey = "AccountId";
        private const string _AccountNameSessionKey = "AccountName";
        private const string _AccountEmailSessionKey = "AccountEmail";
        private const string _AccountRoleSessionKey = "AccountRole";
        private const string _LanguageSessionKey = "Language";
        private const string _CompanyIdSessionKey = "CompanyId";
        private const string _CompanyNameSessionKey = "CompanyName";

        public static bool IsSessionInitialized
        {
            get
            {
                if (HttpContext.Current.Session[_IsSessionInitializedSessionKey] == null)
                {
                    return false;
                }
                var isInitialized = HttpContext.Current.Session[_IsSessionInitializedSessionKey].ToString();
                if (string.IsNullOrEmpty(isInitialized))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                HttpContext.Current.Session[_IsSessionInitializedSessionKey] = value;
            }
        }

        public static int? AccountId
        {
            get
            {
                return (int?)HttpContext.Current.Session[_AccountIdSessionKey];
            }
            set
            {
                HttpContext.Current.Session[_AccountIdSessionKey] = value;
            }
        }
        public static string AccountName
        {
            get
            {
                //if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
                return (string)HttpContext.Current.Session[_AccountNameSessionKey];
                //return string.Empty;
            }
            set
            {
                HttpContext.Current.Session[_AccountNameSessionKey] = value;
            }
        }
        public static string AccountEmail
        {
            get
            {
                return (string)HttpContext.Current.Session[_AccountEmailSessionKey];
            }
            set
            {
                HttpContext.Current.Session[_AccountEmailSessionKey] = value;
            }
        }
        public static byte? AccountRole
        {
            get
            {
                return (byte?)HttpContext.Current.Session[_AccountRoleSessionKey];
            }
            set
            {
                HttpContext.Current.Session[_AccountRoleSessionKey] = value;
            }
        }

        public static string Language
        {
            get
            {
                string lanugae = (string)HttpContext.Current.Session[_LanguageSessionKey];
                if (string.IsNullOrWhiteSpace(lanugae))
                    return "en";
                return lanugae;

            }
            set
            {
                HttpContext.Current.Session[_LanguageSessionKey] = value;
            }
        }

        public static int? CompanyId
        {
            get
            {
                return (int?)HttpContext.Current.Session[_CompanyIdSessionKey];
            }
            set
            {
                HttpContext.Current.Session[_CompanyIdSessionKey] = value;
            }
        }

        public static string CompanyName
        {
            get
            {
                return (string)HttpContext.Current.Session[_CompanyNameSessionKey];
            }
            set
            {
                HttpContext.Current.Session[_CompanyNameSessionKey] = value;
            }
        }

        public static void InitializeSession()
        {
            SessionHelper.IsSessionInitialized = true;
        }
        public static void Destroy()
        {
            HttpContext.Current.Session.Abandon();
        }

    }
}