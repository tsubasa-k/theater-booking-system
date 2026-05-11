using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForm1
{
    public partial class SiteMaster : MasterPage
    {
        // designer 端的 PlaceHolder / Label 控制項
        protected global::System.Web.UI.WebControls.PlaceHolder phLoggedOut;
        protected global::System.Web.UI.WebControls.PlaceHolder phLoggedIn;
        protected global::System.Web.UI.WebControls.Label lblUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 依登入狀態切換 Login/Register 與 Logout
            bool isLoggedIn = false;
            string username = null;
            if (Session != null && Session["LoggedIn"] != null)
            {
                isLoggedIn = Convert.ToBoolean(Session["LoggedIn"]);
                username = Convert.ToString(Session["Username"]);
            }

            if (phLoggedIn != null && phLoggedOut != null)
            {
                phLoggedIn.Visible = isLoggedIn;
                phLoggedOut.Visible = !isLoggedIn;
                if (isLoggedIn && lblUser != null && !string.IsNullOrEmpty(username))
                {
                    lblUser.Text = "👤 " + username;
                }
            }
        }
    }
}