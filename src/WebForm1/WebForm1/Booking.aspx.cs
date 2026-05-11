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
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;
using System.Data.Common;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.Web.Services.Description;
using WebForm1.Helpers;


namespace WebForm1
{
    public partial class Booking : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 使用者已登入，根據使用者角色顯示歡迎訊息
            string username = GetLoggedInUsername();
            string role = GetUserRole(username);
            bool isLoggedIn = Convert.ToBoolean(Session["LoggedIn"]);
            if (!isLoggedIn)
            {
                // 使用者未登錄，可在此新增登入連結或重定向到登入頁面
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                // 使用者已登錄，顯示場地預訂表單
                // 根據角色顯示或隱藏相應的項目
                if (role == "admin")
                {
                    PlaceHolderAdmin.Visible = true; // 顯示管理者相關項目
                }
                else
                {
                    PlaceHolderAdmin.Visible = false; // 隱藏管理者相關項目
                }
                // 根據查詢參數判斷顯示哪個視圖
                string view = Request.QueryString["view"];
                if (!string.IsNullOrEmpty(view) && view.ToLower() == "bookings")
                {
                    // 顯示預訂查詢視圖
                    MultiView1.ActiveViewIndex = 1;
                }
                else
                {
                    // 顯示預訂表單視圖
                    MultiView1.ActiveViewIndex = 0;
                }
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
                    return role;

                }
            }

        }
        protected void SelectedIndexChanged(object sender, EventArgs e)
        {
            // 當場地名稱改變時，清空預訂時間的選項
            DropDownList2.Items.Clear();
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            // 當日期改變時，清空預訂時間的選項
            DropDownList2.Items.Clear();
        }


        protected void rblFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedFunction = int.Parse(rblFunction.SelectedValue);
            MultiView1.ActiveViewIndex = selectedFunction;
        }

        private void SearchBookings(string venue, DateTime startDate, DateTime endDate)
        {
            // 在這裡查詢資料庫，獲取指定場地和日期範圍內的預訂資訊
            // 使用你的連接字串和查詢語句
            string Dbc = DbConfig.BookingDb;

            using (OleDbConnection objConn = new OleDbConnection(Dbc))
            {
                string query = "SELECT Venue, Date, Time FROM booking WHERE Venue = @Venue AND Date BETWEEN @StartDate AND @EndDate ORDER BY Date, Time";
                using (OleDbCommand command = new OleDbCommand(query, objConn))
                {
                    // 參數化查詢，以避免 SQL 注入
                    command.Parameters.AddWithValue("@Venue", venue);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    objConn.Open();
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string bookedVenue = reader["Venue"].ToString();
                            string bookedDate = Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd");
                            string bookedTime = reader["Time"].ToString();

                            // 將查詢到的場地、日期和時間顯示在頁面上
                            DisplayBookingInfo(bookedVenue, bookedDate, bookedTime);
                        }
                    }
                }
            }
        }

        private void DisplayBookingInfo(string bookedVenue, string bookedDate, string bookedTime)
        {
            // 在這裡你可以動態建立 div，顯示同一天的預訂資訊
            // 這裡使用 LiteralControl 來動態生成 HTML，然後加到一個 div 中

            // 檢查是否已經有相同日期的 div，如果沒有，則新增一個
            string divId = $"div_{bookedDate}";
            HtmlGenericControl existingDiv = panelBookingInfo.FindControl(divId) as HtmlGenericControl;

            if (existingDiv == null)
            {
                // 新增一個 div
                HtmlGenericControl newDiv = new HtmlGenericControl("div");
                newDiv.ID = divId;
                newDiv.Attributes["class"] = "booking-info";
                // 加上 margin 屬性
                newDiv.Style.Add("margin", "10px");

                // 將新的 div 加入到 panelBookingInfo 中
                panelBookingInfo.Controls.Add(newDiv);

                // 新增日期標題
                LiteralControl dateLiteral = new LiteralControl($"<h3>{bookedDate}</h3>");
                newDiv.Controls.Add(dateLiteral);

                // 更新 existingDiv，以便後續加入預訂資訊
                existingDiv = newDiv;
            }

            // 將預訂資訊加入到對應的 div 中
            LiteralControl bookingInfoLiteral = new LiteralControl($"場地 {bookedVenue}，時間 {bookedTime}<br />");
            existingDiv.Controls.Add(bookingInfoLiteral);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // 獲取使用者輸入的場地和日期
            string searchVenue = ddlVenues.SelectedValue;
            DateTime startDate = DateTime.ParseExact(startdate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(enddate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // 清空之前的查詢結果
            panelBookingInfo.Controls.Clear();

            // 執行查詢
            SearchBookings(searchVenue, startDate, endDate);

            // 切換到預訂查詢視圖
            MultiView1.ActiveViewIndex = 1;
        }


        protected void btnGetAvailableTimes_Click(object sender, EventArgs e)
        {
            // 在這裡處理顯示目前剩下可以選擇的時間的邏輯

            // 獲取使用者選擇的場地和日期
            string selectedVenue = DropDownList1.SelectedValue;
            string selectedDate = txtDate.Text;

            // 獲取已經預訂的時間
            List<string> bookedTimes = GetBookedTimes(selectedVenue, selectedDate);

            // 初始化時間列表
            DropDownList2.Items.Clear();
            string time1;
            for (int i = 8; i <= 18; i++)
            {
                if(i < 10)
                {
                    time1 = $"0{i}:00";
                }
                else
                {
                    time1 = $"{i}:00";
                }

                // 將未預訂的時間加入到 DropDownList2
                if (!bookedTimes.Contains(time1))
                {
                    DropDownList2.Items.Add(new ListItem(time1));
                }
            }
        }

        private List<string> GetBookedTimes(string venue, string date)
        {
            List<string> bookedTimes = new List<string>();

            // 在這裡查詢資料庫，獲取已經預訂的時間
            // 使用你的連接字串和查詢語句
            string Dbc = DbConfig.BookingDb;
            using (OleDbConnection objConn = new OleDbConnection(Dbc))
            {
                string query = "SELECT Time FROM booking WHERE Venue = @Venue AND Date = @Date";
                using (OleDbCommand command = new OleDbCommand(query, objConn))
                {
                    command.Parameters.AddWithValue("@Venue", venue);
                    command.Parameters.AddWithValue("@Date", date);

                    objConn.Open();
                    //建立reader物件
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string bookedTime = reader["Time"].ToString();
                        bookedTimes.Add(bookedTime);
                    }
                }
            }

            return bookedTimes;
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // 在這裡處理提交預訂請求的邏輯
            string selectedVenue = DropDownList1.SelectedValue;
            string selectedDate = txtDate.Text;
            // 將時間字串轉換為 DateTime，僅抓取 hh:mm 部分
            DateTime selectedTime = DateTime.Parse(DropDownList2.SelectedValue);
            // 轉換成功，現在 selectedTime 包含了時間值
            string formattedTime = selectedTime.ToString("HH:mm");
            SaveBooking(selectedVenue, selectedDate, formattedTime);
        }

        private void LoadVenueList()
        {
            // 在這裡加載場地列表
            // 在這個範例中，我們使用 DropDownList 控制項，你可以從資料庫中載入場地列表
            // 這裡是一個簡單的例子
            DropDownList1.Items.Add(new ListItem("場地1", "場地1"));
            DropDownList1.Items.Add(new ListItem("場地2", "場地2"));
            DropDownList1.Items.Add(new ListItem("場地3", "場地3"));
        }

        private void SaveBooking(string venue, string date, string time)
        {
            string Dbc = DbConfig.BookingDb;

            // 建立connection對象
            OleDbConnection objConn = new OleDbConnection(Dbc);

            try
            {
                // 開啟資料庫連接
                objConn.Open();

                // 檢查資料庫中是否已存在相同場地、日期和時間的預訂
                string checkDuplicateQuery = "SELECT COUNT(*) FROM booking WHERE Venue = @Venue AND Date = @Date AND Time = @Time";
                OleDbCommand checkDuplicateCommand = new OleDbCommand(checkDuplicateQuery, objConn);
                checkDuplicateCommand.Parameters.AddWithValue("@Venue", venue);
                checkDuplicateCommand.Parameters.AddWithValue("@Date", date);
                checkDuplicateCommand.Parameters.AddWithValue("@Time", time);

                int duplicateCount = (int)checkDuplicateCommand.ExecuteScalar();

                if (duplicateCount > 0)
                {
                    // 重複預訂，顯示錯誤訊息或進行其他處理
                    Response.Write("已經存在相同場地、日期和時間的預訂，請重新選擇!");
                }
                else
                {
                    // 預訂可用，執行插入操作
                    // 設定新的Num值為記錄數量加1
                    string countQuery = "SELECT MAX(Num) FROM booking";
                    OleDbCommand countCommand = new OleDbCommand(countQuery, objConn);
                    int record = (int)countCommand.ExecuteScalar();

                    int newNum = record + 1;

                    // 插入新預訂
                    string insertUserQuery = "INSERT INTO booking VALUES (@Num, @Venue, @Date, @Time)";
                    OleDbCommand command = new OleDbCommand(insertUserQuery, objConn);
                    command.Parameters.AddWithValue("@num", newNum);
                    command.Parameters.AddWithValue("@Venue", venue);
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@Time", time);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // 在這裡插入 JavaScript 代碼
                        string script = "showConfirmation();";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showConfirmation", script, true);

                        if (Session["Account"] != null && Session["Username"] != null)
                        {
                            string account = Convert.ToString(Session["Account"]);
                            string username = Convert.ToString(Session["Username"]);
                            // 在適當的地方輸出日誌
                            System.Diagnostics.Debug.WriteLine($"Account: {account}, Username: {username}");

                            // 接下來使用 account 和 username
                            SendReservationEmail(venue, date, account, username);
                        }
                        else
                        {
                            // Session 值不存在或為空，進行適當的處理
                            string script2 = "alert('郵件發送失敗!');";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script2, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 處理例外
                Response.Write("出現錯誤：" + ex.Message);
            }
            finally
            {
                // 關閉資料庫連接
                objConn.Close();
            }
        }

        // 新增方法：發送預訂通知郵件
        private void SendReservationEmail(string venue, string date, string account, string username)
        {
            // 連接資料庫，讀取收件人的電子郵件地址
            string connectionString = DbConfig.DataDb;

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

                        message.Body = string.Format("親愛的 {0}，您已成功預訂了表演場地: {1} ，日期：{2}", username, venue, date);
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

    }
}
