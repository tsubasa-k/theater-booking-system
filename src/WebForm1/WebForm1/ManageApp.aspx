<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageApp.aspx.cs" Inherits="WebForm1.ManageApp" %>

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
        function showConfirmation() {
            var result = confirm('確定要刪除這個預約資訊嗎？');
            if (result) {
                // 如果確定，顯示 "劃位成功!" 訊息
                alert('刪除成功!');
            }
            return result;
        }

        function showAlert(message) {
            alert(message);
            // 更新 Label2 的文字內容
        }
    </script>
    <title>會員管理</title>
    <webopt:bundlereference runat="server" path="~/Content/css" />
    <style>
        .ManageApp {
            min-width: 100vh;
            min-height: 100vh;
            background-color: antiquewhite;
            position: relative;
            padding-top: 20px;
            padding-bottom: 20px;
            display: flex;
            flex-direction: column;
            align-items: center;
            font-size: 20px;
        }
        .ManageApp h2{
            color: saddlebrown;
        }
        .ManageApp h4{
            color: saddlebrown;
            padding-top: 20px;
        }
        .Place{
            display: flex;
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
        <div class="ManageApp">
            <h2>預約管理</h2>
            <h4>場地預約管理</h4>
            <asp:GridView ID="GridViewUsers" runat="server" AutoGenerateColumns="False" OnRowDeleting="GridViewUsers_RowDeleting" OnRowDataBound="GridViewUsers_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Venue" HeaderText="Venue" SortExpression="Venue" ItemStyle-CssClass="gridViewItem"/>
                    <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-CssClass="gridViewItem"/>
                    <asp:BoundField DataField="Time" HeaderText="Time" SortExpression="Time" ItemStyle-CssClass="gridViewItem"/>
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>
                            <asp:Button ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" OnClientClick='<%# "return showConfirmation();" %>'></asp:Button>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                 <HeaderStyle CssClass="gridViewHeader" />
            </asp:GridView>
            <h4>劃位預約管理</h4>
            <div class="Place">
                <asp:DropDownList ID="ddlVenue" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="場地1" Value="場地1"></asp:ListItem>
                    <asp:ListItem Text="場地2" Value="場地2"></asp:ListItem>
                    <asp:ListItem Text="場地3" Value="場地3"></asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" />
            </div>
            <asp:GridView ID="GridViewUsers2" runat="server" AutoGenerateColumns="False" OnRowDeleting="GridViewUsers2_RowDeleting"
                AllowPaging="True" PageSize="10" OnPageIndexChanging="GridViewUsers2_PageIndexChanging" OnRowDataBound="GridViewUsers2_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-CssClass="gridViewItem"/>
                    <asp:BoundField DataField="SeatNo" HeaderText="SeatNo" SortExpression="SeatNo" ItemStyle-CssClass="gridViewItem"/>
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>
                            <asp:Button ID="lnkDelete2" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除"  OnClientClick='<%# "return confirm(\"確定要刪除預訂資訊嗎？\");" %>'></asp:Button>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="gridViewHeader" />
            </asp:GridView>
           
            <br />
            <div>
            <h2>新增場地預約</h2>
            <asp:DropDownList ID="ddlVenue2" runat="server" AutoPostBack="true">
                <asp:ListItem Text="場地1" Value="場地1"></asp:ListItem>
                <asp:ListItem Text="場地2" Value="場地2"></asp:ListItem>
                <asp:ListItem Text="場地3" Value="場地3"></asp:ListItem>
            </asp:DropDownList>
            <asp:TextBox ID="txtDate" runat="server" placeholder="Date"></asp:TextBox>
            <asp:TextBox ID="txtTime" runat="server" placeholder="Time"></asp:TextBox>
            <asp:Button ID="btnAddData" runat="server" Text="新增場地預約" OnClick="btnAddData_Click" />
            </div>
            <br />
            <div>
            <h2>新增劃位預約</h2>
            <asp:DropDownList ID="ddlVenue3" runat="server" AutoPostBack="true">
                <asp:ListItem Text="場地1" Value="場地1"></asp:ListItem>
                <asp:ListItem Text="場地2" Value="場地2"></asp:ListItem>
                <asp:ListItem Text="場地3" Value="場地3"></asp:ListItem>
            </asp:DropDownList>
            <asp:TextBox ID="txtDate2" runat="server" placeholder="Date"></asp:TextBox>
            <asp:TextBox ID="txtSeatNo" runat="server" placeholder="SeatNo"></asp:TextBox>
            <asp:Button ID="btnAddData2" runat="server" Text="新增劃位預約" OnClick="btnAddData2_Click" />
            </div>
        </div>
   </form>
</body>
</html>

