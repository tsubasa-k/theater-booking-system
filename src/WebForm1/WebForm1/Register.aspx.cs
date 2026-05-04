using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForm1
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

        }

        protected void RegisterUser(object sender, EventArgs e)
        {
            // 這裡需要連接資料庫，執行插入使用者資訊的操作
            // 假設你有一個名為 "login" 的表，包含 "account"、"password"、 "name" 和 "Num" 欄

            string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";

            // 建立connection對象
            OleDbConnection objConn = new OleDbConnection(Dbc);

            try
            {
                // 開啟資料庫連接
                objConn.Open();

                string newAccount = txtNewAccount.Text;
                string newPassword = txtNewPassword.Text;
                string newName = txtNewUser.Text;
                string newEmail = txtNewEmail.Text;

                // 檢查資料庫中的記錄數量
                string countQuery = "SELECT COUNT(*) FROM Data";
                OleDbCommand countCommand = new OleDbCommand(countQuery, objConn);
                int recordCount = (int)countCommand.ExecuteScalar();

                // 設定新的Num值為記錄數量加1
                int newNum = recordCount + 1;

                // 檢查資料庫中是否已存在相同的帳號
                string checkUserQuery = "SELECT COUNT(*) FROM Data WHERE account = @account";
                OleDbCommand checkUserCommand = new OleDbCommand(checkUserQuery, objConn);
                checkUserCommand.Parameters.AddWithValue("@account", newAccount);

                int userCount = (int)checkUserCommand.ExecuteScalar();

                if (userCount == 0)
                {
                    // 帳號可用，執行插入操作，設定 "Num" 欄位的值為新的遞增值
                    string insertUserQuery = "INSERT INTO Data (Num, account, [password], username, role, email) VALUES (@Num, @Account, @Password, @Username, @Role, @Email)";
                    OleDbCommand command = new OleDbCommand(insertUserQuery, objConn);
                    command.Parameters.AddWithValue("@Num", newNum);
                    command.Parameters.AddWithValue("@Account", newAccount);
                    command.Parameters.AddWithValue("@Password", newPassword);
                    command.Parameters.AddWithValue("@Username", newName);
                    command.Parameters.AddWithValue("@Role", "user");
                    command.Parameters.AddWithValue("@Email", newEmail);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // 註冊成功
                        Response.Write("帳號已建立成功！");
                    }
                }
                else
                {
                    // 帳號已存在
                    Response.Write("帳號已存在，請使用其他帳號。");
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

            // 清空 TextBox 的資訊
            txtNewAccount.Text = "";
            txtNewPassword.Text = "";
            txtNewUser.Text = "";
            txtNewEmail.Text = "";
            checkPassword.Text = "";
        }
    }
}