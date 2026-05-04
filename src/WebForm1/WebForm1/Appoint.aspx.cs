using System;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Input;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;
using System.Data.Common;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.Web.Services.Description;
using System.Web.Optimization;
using System.Web.Security;

namespace WebForm1
{
    public partial class Appoint : System.Web.UI.Page
    {
        // 在這裡可以設定座位圖的行數和列數
        const int totalSeats = 100;

        // 產生座位圖的數據，初始狀態都是 "Avaliable"
        List<string> seatStatus = Enumerable.Repeat("Avaliable", totalSeats).ToList();

        protected void Page_Load(object sender, EventArgs e)
        {
            // 使用者已登入，根據使用者角色顯示歡迎訊息
            string username = GetLoggedInUsername();
            string role = GetUserRole(username);
            bool isLoggedIn = Convert.ToBoolean(Session["LoggedIn"]);
            if (isLoggedIn)
            {
                // 根據角色顯示或隱藏相應的項目
                if (role == "admin")
                {
                    PlaceHolderAdmin.Visible = true; // 顯示管理者相關項目
                }
                else
                {
                    PlaceHolderAdmin.Visible = false; // 隱藏管理者相關項目
                }
            }
            else
            {
                // Redirect or handle the case when the user is not logged in
                Response.Redirect("~/Login.aspx", false);
            }
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

            string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";

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
                    return role;

                }
            }

        }

        protected void calDate_SelectionChanged(object sender, EventArgs e)
        {
            // 將選擇的日期顯示在 TextBox 中
            txtDate.Text = calDate.SelectedDate.ToShortDateString();
            
        }
        
        protected void btnReserve_Click(object sender, EventArgs e)
        {
            string searchVenue = ddlVenue.SelectedValue;

            // 判斷是否已經選擇了場地
            if (!string.IsNullOrEmpty(searchVenue))
            {

                // 最後，重新載入座位圖
                LoadSeatMap(searchVenue);   
            }
            else
            {
                // 如果未選擇場地，顯示提示或者不執行預訂邏輯
                string script = "alert('請先選擇場地！');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
        }


        // 新增方法：載入座位圖
        private void LoadSeatMap(string searchVenue)
        {
            // 將座位圖綁定到 Repeater
            // 先清空座位狀態列表
            seatStatus.Clear();

            int totalSeats = 49;
            string venue = "venue1";
            if (searchVenue == "場地2")
            {
                totalSeats = 80;
                venue = "venue2";

            }
            else if (searchVenue == "場地3")
            {
                totalSeats = 64;
                venue = "venue3";
            }
            seatStatus = Enumerable.Repeat("Avaliable", totalSeats).ToList();

            // 連接資料庫，讀取已預訂的座位編號
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Booking.accdb";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {

                string query = string.Format("SELECT SeatNo FROM {0} WHERE Date = @Date", venue);
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", txtDate.Text);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        int seatNo;
                        if (int.TryParse(reader["SeatNo"].ToString(), out seatNo))
                        {
                            // 更新座位狀態列表，將已預訂的座位狀態設為 "Reserved"
                            seatStatus[seatNo - 1] = "Reserved";
                        }
                    }
                    
                }
            }

            repeaterSeatMap.DataSource = seatStatus;
            repeaterSeatMap.DataBind();
            
        }

        protected void repeaterSeatMap_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string searchVenue = ddlVenue.SelectedValue;
            if (e.CommandName == "SelectSeat")
            {
                int selectedSeatIndex = Convert.ToInt32(e.CommandArgument);

                // 在這裡處理座位圖中的按鈕點擊事件
                ReserveSeat(selectedSeatIndex);

                // 使用 UpdatePanel 異步重新載入座位圖
                LoadSeatMap(searchVenue);
            }
        }


        // 新增方法：保留座位
        private void ReserveSeat(int seatIndex)
        {
            // 在座位圖中將選定的座位狀態設為 "Reserved"

            seatStatus[seatIndex] = "Reserved";

            string searchVenue = ddlVenue.SelectedValue;

            string venue = "venue1";
            if (searchVenue == "場地2")
            { 
                venue = "venue2";
            }
            else if (searchVenue == "場地3")
            { 
                venue = "venue3";
            }

            string date = txtDate.Text;
            string seatNo = (seatIndex + 1).ToString();    

            // 將預訂信息保存到資料庫
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Booking.accdb";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // 獲取現有記錄數量，生成新的預訂編號
                string countQuery = string.Format("SELECT MAX(Num) FROM {0}", venue);
                using (OleDbCommand countCommand = new OleDbCommand(countQuery, connection))
                {
                    int record = (int)countCommand.ExecuteScalar();
                    int newNum = record + 1;

                    string sql = string.Format("INSERT INTO {0} VALUES (@Num, @Date, @SeatNo)", venue);


                    OleDbCommand command = new OleDbCommand(sql, connection);
                    
                    command.Parameters.AddWithValue("@Num", newNum);
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@SeatNo", seatIndex + 1); // SeatNumber 從 1 開始

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // 發送郵件
                        // 取得使用者資訊，包括電子郵件地址和使用者名稱
                        if (Session["Account"] != null && Session["Username"] != null)
                        {
                            string account = Convert.ToString(Session["Account"]);
                            string username = Convert.ToString(Session["Username"]);
                            // 在適當的地方輸出日誌
                            System.Diagnostics.Debug.WriteLine($"Account: {account}, Username: {username}");

                            // 接下來使用 account 和 username
                            SendReservationEmail(venue, seatNo, date, account, username);
                        }
                        else
                        {
                            // Session 值不存在或為空，進行適當的處理
                            string script = "showAlert('郵件發送失敗!');";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        }

                        // 刷新頁面，重新載入座位圖
                        LoadSeatMap(searchVenue);
                    }
                    
                }
                
            }
            
        }

        // 新增方法：發送預訂通知郵件
        private void SendReservationEmail(string venue, string seatNo, string date, string account, string username)
        {
            // 連接資料庫，讀取收件人的電子郵件地址
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT email FROM Data WHERE account = @account AND username = @username";

                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account", account);
                    command.Parameters.AddWithValue("@username", username);

                    OleDbDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string recipientEmail = reader["email"].ToString();
                        // TODO: 填入你自己的 Gmail 寄件帳號與「應用程式密碼」(App Password, 16 字元)
                        // 申請步驟見 README.md 的「Gmail 寄信設定」章節
                        string fromEmail = "";
                        string fromPassword = "";

                        // 設置寄件人和收件人
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(fromEmail);
                        // 設置郵件標題和內容
                        message.Subject = "預訂通知";
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.To.Add(recipientEmail);

                        message.Body = string.Format("親愛的 {0}，您已成功預訂 {1} 的座位，座位號碼: {2}，日期：{3}", username, venue, seatNo, date);
                        message.IsBodyHtml = true;
                        message.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼

                        var smtpClient = new SmtpClient("smtp.gmail.com")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential(fromEmail, fromPassword),
                            EnableSsl = true,
                        };


                        try
                        {
                            // 發送郵件
                            smtpClient.Send(message);
                            
                        }
                        catch (Exception ex)
                        {
                            // 處理郵件發送失敗的例外
                            // 可以記錄錯誤，或者顯示錯誤訊息給使用者
                            Console.WriteLine("郵件發送失敗：" + ex.Message);
                            // 加入其他日誌輸出方式，例如 Debug.WriteLine
                            Debug.WriteLine("郵件發送失敗：" + ex.Message);
                            // 或者使用 ASP.NET 的 Trace
                            System.Web.HttpContext.Current.Trace.Write("郵件發送失敗：" + ex.Message);
                        }
                    }
                }
            }
        }


        // 修改 GetSeatCssClass 方法，根據座位狀態返回對應的 CSS 類別
        protected string GetSeatCssClass(int index)
        {
            // 根據座位狀態返回不同的 CSS 類別
            if (seatStatus[index] == "Avaliable")
            {
                return "seat";
            }
            else if (seatStatus[index] == "Reserved")
            {
                return "reserved";
            }
            // 其他狀態的處理

            return "";
        }
    }
}