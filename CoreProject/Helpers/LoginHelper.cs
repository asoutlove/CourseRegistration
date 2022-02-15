using System.Web;
using System.Data;
using CoreProject.Helpers;
using CoreProject.Models;

namespace Ubay_CourseRegistration
{
    public class LoginHelper
    {
        private const string _sessionKey = "IsLogined";
        private const string _sessionKey_Account = "Account";
        public static bool HasLogined()
        {
            bool? val = HttpContext.Current.Session[_sessionKey] as bool?;

            if (val.HasValue && val.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 嘗試登入
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool TryLogin(string account, string pwd)
        {
            if (LoginHelper.HasLogined())
                return true;

            //Get user account from DB

            LoginInfo Logmodel = new LoginInfo();
            Account_summaryModel asmodel = new Account_summaryModel();

            DataTable dt = DBAccountManager.GetUserAccount(account);

            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            string dbPwd = dt.Rows[0].Field<string>("password");
            bool isPasswordRight = string.Compare(dbPwd, pwd) == 0;

            //if (isAccountRight && isPasswordRight)
            if (isPasswordRight)

            {
                HttpContext.Current.Session[_sessionKey_Account] = account;
                HttpContext.Current.Session[_sessionKey] = true;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 登出目前使用者，如果還沒登入就不執行
        /// </summary>
        public static void Logout()
        {
            if (!LoginHelper.HasLogined())
                return;
            HttpContext.Current.Session.RemoveAll();

        }

        /// <summary>
        /// 取得已登入者的資訊，如果還沒登入回傳空字串
        /// </summary>
        /// <returns></returns>
        public static LoginInfo GetCurrentUserInfo()
        {
            if (!LoginHelper.HasLogined())
                return null;

            return HttpContext.Current.Session[_sessionKey_Account] as LoginInfo;
        }
    }
}