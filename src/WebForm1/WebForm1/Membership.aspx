<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Membership.aspx.cs" Inherits="WebForm1.Membership" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous"/>

    <!-- Bootstrap JS -->
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js" integrity="sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js" integrity="sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl" crossorigin="anonymous"></script>

    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <script type="text/javascript">
        function showConfirmation(account) {
            var result = confirm('確定要刪除這個預訂資訊嗎？');
            if (result) {
                // 如果確定，顯示 "劃位成功!" 訊息
                alert('刪除成功!');
                __doPostBack('GridViewUsers', 'Delete$' + account);
            }
        }

        function showAlert(message) {
            alert(message);
            // 更新 Label2 的文字內容
        }
    </script>

    <title>會員管理</title>
    <webopt:bundlereference runat="server" path="~/Content/css" />
    <style>
     
    .Membership {
        min-width: 100vh;
        background-color: antiquewhite;
        position: relative;
        display: flex;
        padding-top: 20px;
        padding-bottom: 30px;
        flex-direction: column;
        align-items: center;
        font-size: 20px;
    }
    .Membership h2{
        color: saddlebrown;
    }
    
    .gridViewItem {
        color: peru;
        padding-right: 20px; /* 設定右邊距 */
    }
    .gridViewHeader th {
        color: saddlebrown;
        padding-right: 20px; 
    }
    
</style>
</head>
<body>
    <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-dark bg-dark">
        <div class="container">
            <a class="navbar-brand" runat="server" href="~/">表演場地線上預約系統</a>
            <button type="button" class="navbar-toggler" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" title="切換導覽" aria-controls="navbarSupportedContent"
                aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse d-sm-inline-flex justify-content-between">
                <ul class="navbar-nav flex-grow-1">
                    <li class="nav-item">
                        <a class="nav-link" href="Main.aspx">Home</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="Booking.aspx">Booking</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="Appoint.aspx">Appoint</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="Login.aspx">Login</a>
                    </li>
                    <!-- 根據使用者角色動態顯示的項目 -->
                    <asp:PlaceHolder ID="PlaceHolderAdmin" runat="server" Visible = "false">
                        <li class="nav-item">
                            <a class="nav-link" href="Membership.aspx">Membership</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="ManageApp.aspx">Manage App</a>
                        </li>
                    </asp:PlaceHolder>
                </ul>
            </div>
        </div>
    </nav>
    <form id="form1" runat="server">
        <div class="Membership">
            <h2>會員管理</h2>
            <asp:GridView ID="GridViewUsers" runat="server" AutoGenerateColumns="False" DataKeyNames="Account" OnRowEditing="GridViewUsers_RowEditing" OnRowUpdating="GridViewUsers_RowUpdating" OnRowCancelingEdit="GridViewUsers_RowCancelingEdit" OnRowDeleting="GridViewUsers_RowDeleting" OnRowDataBound="GridViewUsers_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Account" HeaderText="Account" ReadOnly="True" SortExpression="Account" ItemStyle-CssClass="gridViewItem"/>
                    <asp:BoundField DataField="Password" HeaderText="Password" SortExpression="Password" ItemStyle-CssClass="gridViewItem"/>
                    <asp:BoundField DataField="Username" HeaderText="Username" SortExpression="Username" ItemStyle-CssClass="gridViewItem"/>
                    <asp:BoundField DataField="Role" HeaderText="Role" SortExpression="Role" ItemStyle-CssClass="gridViewItem"/>
                    <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ItemStyle-CssClass="gridViewItem"/>
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>
                            <asp:Button ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除"  OnClientClick='<%# Eval("Account", "return confirm(\"確定要刪除帳號 {0} 嗎？\");") %>'></asp:Button>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                 <HeaderStyle CssClass="gridViewHeader" />
            </asp:GridView>
        </div>
        <br />
        <div>
            <h2>新增會員</h2>
            <asp:TextBox ID="txtAccount" runat="server" placeholder="Account"></asp:TextBox>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
            <asp:TextBox ID="txtUsername" runat="server" placeholder="Username"></asp:TextBox>
            <asp:TextBox ID="txtRole" runat="server" placeholder="Role"></asp:TextBox>
            <asp:TextBox ID="txtEmail" runat="server" placeholder="Email"></asp:TextBox>
            <asp:Button ID="btnAddUser" runat="server" Text="新增會員" OnClick="btnAddUser_Click" />
        </div>
    </form>
</body>
</html>
