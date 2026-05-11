using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace WebForm1
{
    /// <summary>
    /// 登出處理：清除 Session、撤銷 FormsAuthentication Cookie，導回登入頁。
    /// </summary>
    public partial class Logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1) 清掉所有 Session（LoggedIn / Account / Username）
            if (Session != null)
            {
                Session.Clear();
                Session.Abandon();
            }

            // 2) 撤銷 FormsAuthentication Cookie
            FormsAuthentication.SignOut();

            // 3) 主動讓瀏覽器端的 cookie 立刻失效
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddDays(-1),
                HttpOnly = true
            };
            Response.Cookies.Add(cookie);

            // 4) 導回登入頁
            Response.Redirect("~/Login.aspx");
        }
    }
}
