using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForm1
{
    public partial class Membership : System.Web.UI.Page
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
                    // 根據角色返回對應的字串
                    return role;

                }
            }

        }

        // 綁定 GridView 中的資料
        private void BindUsers()
        {
            string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";
            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                string query = "SELECT * FROM Data";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    GridViewUsers.DataSource = dataTable;
                    GridViewUsers.DataBind();
                }
            }
        }

        protected List<UserInfo> GetAllUsers()
        {
            List<UserInfo> users = new List<UserInfo>();

            string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";

            //建立connection物件
            using (OleDbConnection objc = new OleDbConnection(Dbc))
            {
                //啟動資料庫連結
                objc.Open();

                //下資料庫指令
                string cmd = "SELECT * FROM Data;";
                using (OleDbCommand DbCommand = new OleDbCommand(cmd, objc))
                {
                    //建立reader物件
                    using (OleDbDataReader Reader = DbCommand.ExecuteReader())
                    {
                        // 確認讀取到的資料列數
                        while (Reader.Read())
                        {
                            // 將資料封裝到 UserInfo 物件中
                            UserInfo user = new UserInfo
                            {
                                Account = Reader["account"].ToString(),
                                Password = Reader["password"].ToString(),
                                Username = Reader["username"].ToString(),
                                Role = Reader["role"].ToString(),
                                Email = Reader["email"].ToString()
                            };

                            // 將使用者加入列表
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        // ...

        public class UserInfo
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
            public string Email { get; set; }
        }


        protected void GridViewUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewUsers.EditIndex = e.NewEditIndex;
            BindUsers();
        }

        protected void GridViewUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // 取得編輯行的索引
            int rowIndex = e.RowIndex;

            // 取得 GridView 中的控制項
            GridViewRow row = GridViewUsers.Rows[rowIndex];

            // 取得編輯後的值
            string account = GridViewUsers.DataKeys[e.RowIndex].Values["Account"].ToString();
            string password = ((TextBox)GridViewUsers.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
            string username = ((TextBox)GridViewUsers.Rows[e.RowIndex].Cells[2].Controls[0]).Text;
            string role = ((TextBox)GridViewUsers.Rows[e.RowIndex].Cells[3].Controls[0]).Text;
            string email = ((TextBox)GridViewUsers.Rows[e.RowIndex].Cells[4].Controls[0]).Text;

            // 更新資料庫中的相應資料，這裡需要你自行實作更新邏輯

            // 使用 SQL UPDATE 語句
            string updateQuery = "UPDATE Data SET [password] = @Password, username = @Username, role = @Role, email = @Email WHERE account = @Account";

            string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";

            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {

                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                {
                    // 填入更新的參數值
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Account", account);
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
                            // 行受影響，表示更新成功
                            string script = "showAlert('更新成功!');";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        }
                        else
                        {
                            // 行未受影響，表示更新失敗
                            // Session 值不存在或為空，進行適當的處理
                            string script = "showAlert('更新失敗!');";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 處理例外情況
                        // 這裡可以記錄錯誤或者顯示錯誤訊息
                        Response.Write("更新資料庫時發生錯誤：" + ex.Message);
                    }

                }
            }

            // 編輯完成後，取消編輯模式
            GridViewUsers.EditIndex = -1;

            // 重新綁定 GridView
            BindUsers();
        }


        protected void GridViewUsers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // 取消編輯模式
            GridViewUsers.EditIndex = -1;
            BindUsers();
        }

        protected void GridViewUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnDelete = (Button)e.Row.FindControl("GridViewUsers_DeleteButton");
                if (btnDelete != null)
                {
                    string account = DataBinder.Eval(e.Row.DataItem, "Account").ToString();
                    //btnDelete.OnClientClick = $"return confirm('確定要刪除帳號 {account} 嗎？');";
                }
            }
        }


        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string account = txtAccount.Text;
            string role = txtRole.Text;
            string email = txtEmail.Text;

            // 檢查資料庫中的記錄數量
            string countQuery = "SELECT MAX(Num) FROM Data";

            // 新增使用者到資料庫
            string insertQuery = "INSERT INTO Data (Num, account, [password], username, role, email) VALUES (@Num, @Account, @Password, @Username, @Role, @Email)";

            string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";

            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                // 打開資料庫連接
                connection.Open();
                OleDbCommand countCommand = new OleDbCommand(countQuery, connection);

                int recordCount = (int)countCommand.ExecuteScalar();

                // 設定新的Num值為記錄數量加1
                int newNum = recordCount + 1;

                // 檢查資料庫中是否已存在相同的帳號
                string checkUserQuery = "SELECT COUNT(*) FROM Data WHERE account = @account";
                OleDbCommand checkUserCommand = new OleDbCommand(checkUserQuery, connection);
                checkUserCommand.Parameters.AddWithValue("@account", account);

                int userCount = (int)checkUserCommand.ExecuteScalar();

                // 關閉資料庫連接
                connection.Close();

                if (userCount == 0)
                {

                    OleDbCommand command = new OleDbCommand(insertQuery, connection);
                    // 填入新增的參數值
                    command.Parameters.AddWithValue("@Num", newNum);
                    command.Parameters.AddWithValue("@Account", account);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Email", email);
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
                        txtUsername.Text = "";
                        txtPassword.Text = "";
                        txtAccount.Text = "";
                        txtRole.Text = "";
                        txtEmail.Text = "";
                    }
                    catch (Exception ex)
                    {
                        // 處理例外情況
                        // 這裡可以記錄錯誤或者顯示錯誤訊息
                        Response.Write("新增資料庫時發生錯誤：" + ex.Message);
                    }
                }
                else
                {
                    // 帳號已存在
                    string script = "showAlert('新增失敗! 帳號已存在，請使用其他帳號!');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                }

        }

            // 重新綁定 GridView
            BindUsers();
        }


        protected void GridViewUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // 取得被刪除行的索引
            int rowIndex = e.RowIndex;

            // 取得 GridView 中的控制項
            GridViewRow row = GridViewUsers.Rows[rowIndex];

            // 取得刪除的相關資料
            string account = GridViewUsers.DataKeys[e.RowIndex].Value.ToString();

            // 在這裡實作刪除資料的邏輯，可以使用 SQL DELETE 語句
            string deleteQuery = "DELETE FROM Data WHERE account = @Account";

            string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";

            using (OleDbConnection connection = new OleDbConnection(Dbc))
            {
                using (OleDbCommand command = new OleDbCommand(deleteQuery, connection))
                {
                    // 填入刪除的參數值
                    command.Parameters.AddWithValue("@Account", account);
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
                            //string script = "showConfirmation();";
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
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

    }
}
