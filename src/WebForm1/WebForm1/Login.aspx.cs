using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForm1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Write("後端 -- Hello");
            Label1.Text = "";

        }

        protected void LoginUser(object sender, EventArgs e)
        {
            // 這裡需要連接資料庫，查詢使用者訊息，然後檢查使用者名稱和密碼是否匹配
            // 假設你有一個名為 "login" 的表，包含 "account" 和 "password" 欄

            string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";

            //建立connection物件
            OleDbConnection objc = new OleDbConnection(Dbc);
            //啟動資料庫連結
            objc.Open();
            //建立reader物件
            OleDbDataReader Reader;

            string AC = txtAccount.Text;//獲得account
            string PS = txtPassword.Text;//獲得password
            //下資料庫指令（參數化查詢，避免 SQL Injection）
            //  Access OleDb 用 '?' 占位符，參數順序對應 SQL 中 '?' 出現的順序
            string cmd = "SELECT * FROM Data WHERE account = ? AND [password] = ?";
            OleDbCommand DbCommand = new OleDbCommand(cmd, objc);
            DbCommand.Parameters.AddWithValue("@account", AC);
            DbCommand.Parameters.AddWithValue("@password", PS);

            //reader接收執行結果
            Reader = DbCommand.ExecuteReader();

            if (Reader.Read())//如果有找到對應的帳密
            {
                object usernameObject = Reader["username"]; //獲得username欄位
                object accountObject = Reader["account"];
                string name = Convert.ToString(usernameObject);
                string account = Convert.ToString(accountObject);

                // 設置 ASP.NET 的身份驗證 Cookie
                FormsAuthentication.SetAuthCookie(name, false);

                Label1.Text = String.Format
                ("{0}您好! 登入時間{1}", name, DateTime.Now.ToString());
                Label1.ForeColor = System.Drawing.Color.Blue;
                Label1.Visible = true; // 使Label可見
                // 登入成功，顯示 "Correct!" 訊息
                lblResult.Text = "Correct!";
                lblResult.ForeColor = System.Drawing.Color.Green;
                lblResult.Visible = true; // 使Label可見

                // 使用JavaScript在前端弹出消息框
                string script = "alert('Correct!');";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CorrectMessage", script, true);

                // 設定Session變數表示已經成功登入
                if (Session != null)
                {
                    Session["LoggedIn"] = true;
                    Session["Account"] = account; // 替換為實際的使用者帳號
                    Session["Username"] = name; // 替換為實際的使用者名稱
                }

                // 登入成功後導向到主頁
                Response.Redirect("~/Main.aspx");
            }
            else
            {
                // 登入失敗，顯示 "Wrong!" 訊息
                lblResult.Text = "Wrong!";
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Visible = true; // 使Label可見

                Label1.Text = String.Format
               ("登入失敗! 請重新登入!");
                Label1.ForeColor = System.Drawing.Color.Red;
                Label1.Visible = true; // 使Label可見
                // 使用JavaScript在前端弹出消息框
                string failureScript = "alert('Wrong!');";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "WrongMessage", failureScript, true);
            }

        }

    }
}