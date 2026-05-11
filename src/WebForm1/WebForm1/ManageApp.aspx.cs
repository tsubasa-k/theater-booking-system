using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Principal;
using WebForm1.Helpers;


namespace WebForm1
{
    public partial class ManageApp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // 使用者已登入，根據使用者角色顯示歡迎訊息
            // 使用者已登入，根據使用者角色顯示歡迎訊息
            string username = GetLoggedInUsername();
            string role = GetUserRole(username);

            bool isLoggedIn = Convert.ToBoolean(Session["LoggedIn"]);
            if (!IsPostBack)
            {
                if (isLoggedIn)
                {
                    // 根據角色顯示或隱藏相應的項目
                    if (role == "admin")
                    {
                        PlaceHolderAdmin.Visible = true; // 顯示管理者相關項目
                        BindUsers();
                    }
                    else
                    {
                        string script = "showAlert('此帳號並未有權限!');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        Response.Redirect("~/Login.aspx");

                    }
                }
                else
                {
                    // 使用者未登錄，可在此新增登入連結或重定向到登入頁面
                    Response.Redirect("~/Login.aspx");
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
                    // 根據角色返回對應的字串
                    return role;

                }
            }

        }

        // 綁定 GridView 中的資料
        private void BindUsers()
        {
            string Dbc = DbConfig.BookingDb;
            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                string query = "SELECT * FROM booking";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // 將 Date 欄位的型態轉換為 DateTime
                    dataTable.Columns["Date"].DataType = typeof(DateTime);

                    GridViewUsers.DataSource = dataTable;
                    GridViewUsers.DataBind();
                }
            }
        }

        protected List<DataInfo> GetAllUsers()
        {
            List<DataInfo> datas = new List<DataInfo>();

            string Dbc = DbConfig.BookingDb;

            //建立connection物件
            using (OleDbConnection objc = new OleDbConnection(Dbc))
            {
                //啟動資料庫連結
                objc.Open();

                //下資料庫指令
                string cmd = "SELECT * FROM booking;";
                using (OleDbCommand DbCommand = new OleDbCommand(cmd, objc))
                {
                    //建立reader物件
                    using (OleDbDataReader Reader = DbCommand.ExecuteReader())
                    {
                        // 確認讀取到的資料列數
                        while (Reader.Read())
                        {
                            // 將資料封裝到 DataInfo 物件中
                            DataInfo data = new DataInfo
                            {
                                Venue = Reader["Venue"].ToString(),
                                Date = Reader["Date"].ToString(),
                                Time = Reader["Time"].ToString()
                                
                            };

                            // 將使用者加入列表
                            datas.Add(data);
                        }
                    }
                }
            }

            return datas;
        }


        public class DataInfo
        {
            public string Venue { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
           
        }

        protected void GridViewUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnDelete = (Button)e.Row.FindControl("GridViewUsers_DeleteButton");
                //btnDelete.OnClientClick = $"return confirm('確定要刪除預約資訊嗎？');";
            }
        }

        // 綁定 GridView 中的資料
        private void BindUsers2()
        {
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
            string Dbc = DbConfig.BookingDb;
            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                string query = string.Format("SELECT * FROM {0}", venue);
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // 將 Date 欄位的型態轉換為 DateTime
                    dataTable.Columns["Date"].DataType = typeof(DateTime);

                    GridViewUsers2.DataSource = dataTable;
                    GridViewUsers2.DataBind();
                }
            }
        }

        protected List<DataInfo2> GetAllUsers2()
        {
            List<DataInfo2> datas = new List<DataInfo2>();

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

            string Dbc = DbConfig.BookingDb;

            //建立connection物件
            using (OleDbConnection objc = new OleDbConnection(Dbc))
            {
                //啟動資料庫連結
                objc.Open();

                //下資料庫指令
                string cmd = string.Format("SELECT * FROM {0}", venue);
                using (OleDbCommand DbCommand = new OleDbCommand(cmd, objc))
                {
                    //建立reader物件
                    using (OleDbDataReader Reader = DbCommand.ExecuteReader())
                    {
                        // 確認讀取到的資料列數
                        while (Reader.Read())
                        {
                            // 將資料封裝到 DataInfo 物件中
                            DataInfo2 data = new DataInfo2
                            {
                                
                                Date = Reader["Date"].ToString(),
                                SeatNo = Reader["SeatNo"].ToString()

                            };

                            // 將使用者加入列表
                            datas.Add(data);
                        }
                    }
                }
            }

            return datas;
        }

        public class DataInfo2
        {
            public string Date { get; set; }
            public string SeatNo { get; set; }

        }

        protected void GridViewUsers2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnDelete = (Button)e.Row.FindControl("GridViewUsers_DeleteButton");
                
            }
        }
        protected void GridViewUsers2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewUsers2.PageIndex = e.NewPageIndex;
            BindUsers2();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // 調用 BindUsers2() 以綁定 GridViewUsers2
            BindUsers2();
        }
        protected void btnAddData_Click(object sender, EventArgs e)
        {
            string venue = ddlVenue2.SelectedValue;
            
            string date = txtDate.Text;
            string time = txtTime.Text;

            // 檢查資料庫中的記錄數量
            string getMaxNumQuery = "SELECT MAX(Num) FROM booking";

            // 新增使用者到資料庫
            string insertQuery = "INSERT INTO booking VALUES (@Num, @Venue, @Date, @Time)";

            string Dbc = DbConfig.BookingDb;

            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                // 打開資料庫連接
                connection.Open();
                OleDbCommand countCommand = new OleDbCommand(getMaxNumQuery, connection);

                // 執行 SQL 語句
                var result = countCommand.ExecuteScalar();
                int num = Convert.ToInt32(result);
                int newNum = num + 1;
                // 關閉資料庫連接
                connection.Close();
   
                OleDbCommand command = new OleDbCommand(insertQuery, connection);
                // 填入新增的參數值
                command.Parameters.AddWithValue("@Num", newNum);
                command.Parameters.AddWithValue("@Venue", venue);
                command.Parameters.AddWithValue("@Date", date);
                command.Parameters.AddWithValue("@Time", time);
               
                try
                {
                    // 打開資料庫連接
                    connection.Open();

                    // 執行 SQL 語句
                    int rowsAffected = command.ExecuteNonQuery();

                    // 關閉資料庫連接
                    connection.Close();

                    // 檢查是否有行受影響
                    if (rowsAffected > 0)
                    {
                        // 行受影響，表示新增成功
                        string script = "showAlert('新增成功!');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                    }
                    else
                    {
                        // 行未受影響，表示新增失敗
                        // Session 值不存在或為空，進行適當的處理
                        string script = "showAlert('新增失敗!');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                    }

                    // 清空 TextBox 的資訊
                    ddlVenue2.ClearSelection();
                    txtDate.Text = "";
                    txtTime.Text = "";
                }
                catch (Exception ex)
                {
                    // 處理例外情況
                    // 這裡可以記錄錯誤或者顯示錯誤訊息
                    Response.Write("新增資料庫時發生錯誤：" + ex.Message);
                }
                
                

            }

            // 重新綁定 GridView
            BindUsers();
        }

        protected void btnAddData2_Click(object sender, EventArgs e)
        {
            string searchVenue = ddlVenue3.SelectedValue;
            string date = txtDate2.Text;
            string seatno = txtSeatNo.Text;

            string venue = "venue1";
            if (searchVenue == "場地2")
            {
                venue = "venue2";
            }
            else if (searchVenue == "場地3")
            {
                venue = "venue3";
            }

            // 檢查資料庫中的記錄數量
            string countQuery = string.Format("SELECT MAX(Num) FROM {0}", venue);

            // 新增使用者到資料庫
            string insertQuery = string.Format("INSERT INTO {0} VALUES (@Num, @Date, @Time)", venue);

            string Dbc = DbConfig.BookingDb;

            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                // 打開資料庫連接
                connection.Open();
                OleDbCommand countCommand = new OleDbCommand(countQuery, connection);

                int recordCount = (int)countCommand.ExecuteScalar();

                // 設定新的Num值為記錄數量加1
                int newNum = recordCount + 1;


                // 關閉資料庫連接
                connection.Close();


                OleDbCommand command = new OleDbCommand(insertQuery, connection);
                // 填入新增的參數值
                command.Parameters.AddWithValue("@Num", newNum);
                command.Parameters.AddWithValue("@Date", date);
                command.Parameters.AddWithValue("@SeatNo", seatno);

                try
                {
                    // 打開資料庫連接
                    connection.Open();

                    // 執行 SQL 語句
                    int rowsAffected = command.ExecuteNonQuery();

                    // 關閉資料庫連接
                    connection.Close();

                    // 檢查是否有行受影響
                    if (rowsAffected > 0)
                    {
                        // 行受影響，表示新增成功
                        string script = "showAlert('新增成功!');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                    }
                    else
                    {
                        // 行未受影響，表示新增失敗
                        // Session 值不存在或為空，進行適當的處理
                        string script = "showAlert('新增失敗!');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                    }

                    // 清空 TextBox 的資訊
                    ddlVenue3.ClearSelection();
                    txtDate2.Text = "";
                    txtSeatNo.Text = "";
                }
                catch (Exception ex)
                {
                    // 處理例外情況
                    // 這裡可以記錄錯誤或者顯示錯誤訊息
                    Response.Write("新增資料庫時發生錯誤：" + ex.Message);
                }

            }

            // 重新綁定 GridView
            BindUsers2();
        }

        protected void GridViewUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // 取得被刪除行的索引
            int rowIndex = e.RowIndex;

            // 取得 GridView 中的控制項
            GridViewRow row = GridViewUsers.Rows[rowIndex];

            // 取得刪除的相關資料
            string venue = GridViewUsers.Rows[e.RowIndex].Cells[0].Text;
            string date = GridViewUsers.Rows[e.RowIndex].Cells[1].Text;
            string time = GridViewUsers.Rows[e.RowIndex].Cells[2].Text;

            // 在這裡實作刪除資料的邏輯，可以使用 SQL DELETE 語句
            string deleteQuery = "DELETE FROM booking WHERE Venue = @Venue AND Date = @Date AND Time = @Time";

            string Dbc = DbConfig.BookingDb;

            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                using (OleDbCommand command = new OleDbCommand(deleteQuery, connection))
                {
                    // 填入刪除的參數值
                    command.Parameters.AddWithValue("@Venue", venue);
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@Time", time);
                    try
                    {
                        // 打開資料庫連接
                        connection.Open();

                        // 執行 SQL 語句
                        int rowsAffected = command.ExecuteNonQuery();

                        // 關閉資料庫連接
                        connection.Close();

                        // 檢查是否有行受影響
                        if (rowsAffected > 0)
                        {
                            // 行受影響，表示刪除成功
                            
                        }
                        else
                        {
                            // 行未受影響，表示刪除失敗
                            // Session 值不存在或為空，進行適當的處理
                            string script = "showAlert('刪除失敗!');";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        }

                    }
                    catch (Exception ex)
                    {
                        // 處理例外情況
                        // 這裡可以記錄錯誤或者顯示錯誤訊息
                        Response.Write("刪除資料庫時發生錯誤：" + ex.Message);
                    }
                }
            }

            // 重新綁定 GridView
            BindUsers();
        }


        protected void GridViewUsers2_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // 取得被刪除行的索引
            int rowIndex = e.RowIndex;

            // 取得 GridView 中的控制項
            GridViewRow row = GridViewUsers2.Rows[rowIndex];
            
            // 取得刪除的相關資料
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
            string date = GridViewUsers2.Rows[e.RowIndex].Cells[0].Text;
            string seatno = GridViewUsers2.Rows[e.RowIndex].Cells[1].Text;

            // 在這裡實作刪除資料的邏輯，可以使用 SQL DELETE 語句
            string deleteQuery = string.Format("DELETE FROM {0} WHERE Date = @Date AND SeatNo = @SeatNo", venue);

            string Dbc = DbConfig.BookingDb;

            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                using (OleDbCommand command = new OleDbCommand(deleteQuery, connection))
                {
                    // 填入刪除的參數值
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@SeatNo", seatno);
                    try
                    {
                        // 打開資料庫連接
                        connection.Open();

                        // 執行 SQL 語句
                        int rowsAffected = command.ExecuteNonQuery();

                        // 關閉資料庫連接
                        connection.Close();

                        // 檢查是否有行受影響
                        if (rowsAffected > 0)
                        {
                            // 行受影響，表示刪除成功
                           
                        }
                        else
                        {
                            // 行未受影響，表示刪除失敗
                            // Session 值不存在或為空，進行適當的處理
                            string script = "showAlert('刪除失敗!');";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        }

                    }
                    catch (Exception ex)
                    {
                        // 處理例外情況
                        // 這裡可以記錄錯誤或者顯示錯誤訊息
                        Response.Write("刪除資料庫時發生錯誤：" + ex.Message);
                    }
                }
            }

            // 重新綁定 GridView
            BindUsers2();
        }

    }
}
