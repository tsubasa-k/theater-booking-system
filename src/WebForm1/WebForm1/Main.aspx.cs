using System;
using System.Collections;
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
using WebForm1.Helpers;

namespace WebForm1
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // 使用者已登入，根據使用者角色顯示歡迎訊息
            string username = GetLoggedInUsername();
            string role = GetUserRole(username);

            bool isLoggedIn = Convert.ToBoolean(Session["LoggedIn"]);
            if (isLoggedIn)
            {
                // 根據角色顯示或隱藏相應的項目
                if (role == "管理員")
                {
                    PlaceHolderAdmin.Visible = true; // 顯示管理者相關項目
                }
                else
                {
                    PlaceHolderAdmin.Visible = false; // 隱藏管理者相關項目
                }
                lblLoginStatus.Text = "成功登入!";
                lblWelcome.Text = "歡迎，" + username + " (" + role + ")";
                lblLoginStatus.Visible = true;
                lblWelcome.Visible = true;
                Button bookingButton = new Button();
                bookingButton.Text = "進入預約系統";
                bookingButton.PostBackUrl = "~/Booking.aspx";
                bookingButton.CssClass = "booking-button btn btn-secondary"; // 可以加上 CSS 類別進行樣式設定
                bookingButton.Click += BookingButton_Click; // 設定按鈕點擊事件
                                                            // 將 Button 加入至頁面
                PlaceHolderLogin.Controls.Add(bookingButton);

                Button appointButton = new Button();
                appointButton.Text = "進入劃位系統";
                appointButton.PostBackUrl = "~/Appoint.aspx";
                appointButton.CssClass = "appoint-button btn btn-secondary"; // 可以加上 CSS 類別進行樣式設定
                appointButton.Click += AppointButton_Click; // 設定按鈕點擊事件
                                                        // 將 Button 加入至頁面
                PlaceHolderLogin.Controls.Add(appointButton);
                // 使用 CSS 設定兩個按鈕之間的間距
                PlaceHolderLogin.Controls.Add(new LiteralControl("<style>.booking-button, .appoint-button { margin-top: 20px; }</style>"));
            }
            else
            {
                // 使用者未登入，新增登入按鈕
                Button loginButton = new Button();
                loginButton.Text = "點此登入";
                loginButton.PostBackUrl = "~/Login.aspx"; // 使用 PostBackUrl 屬性設定導向的頁面
                loginButton.CssClass = "login-button btn btn-secondary"; // 可以加上 CSS 類別進行樣式設定
                loginButton.Click += LoginButton_Click; // 設定按鈕點擊事件
                                                        // 將 Button 加入至頁面
                PlaceHolderLogin.Controls.Add(loginButton);
            }
        }

        protected void BookingButton_Click(object sender, EventArgs e)
        {
            // 在這裡處理按鈕點擊事件的邏輯，例如導向登入頁面的操作
            Response.Redirect("~/Booking.aspx");
        }

        protected void AppointButton_Click(object sender, EventArgs e)
        {
            // 在這裡處理按鈕點擊事件的邏輯，例如導向登入頁面的操作
            Response.Redirect("~/Appoint.aspx");
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            // 在這裡處理按鈕點擊事件的邏輯，例如導向登入頁面的操作
            Response.Redirect("~/Login.aspx");
        }


        private string GetLoggedInUsername()
        {
            if (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                if (ticket != null && !string.IsNullOrEmpty(ticket.Name))
                {
                    return ticket.Name;
                }
            }

            // 使用者未登入，可能需要處理一些邏輯或返回預設值
            return "未登入使用者";
        }

        private string GetUserRole(string username)
        {
            string role = "";
            // 新增使用者到資料庫
            string SQL = "SELECT role FROM Data WHERE username = @Username";

            string Dbc = DbConfig.DataDb;

            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                using (OleDbCommand command = new OleDbCommand(SQL, connection))
                {
                    // 填入新增的參數值
                    command.Parameters.AddWithValue("@Username", username);

                    // 打開資料庫連接
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();

                    // 檢查是否有資料列可讀取
                    if (reader.Read())
                    {
                        // 取得使用者角色資訊
                        role = reader["role"].ToString();
                    }
                    // 根據角色返回對應的字串
                    if (role == "admin")
                    {
                        return "管理員";
                    }
                    else
                    {
                        return "一般使用者";
                    }

                }
            }
            
        }

    }
}
