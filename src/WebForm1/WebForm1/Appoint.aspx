<%@ Page Language="C#" Debug="true" AutoEventWireup="true" CodeBehind="Appoint.aspx.cs" Inherits="WebForm1.Appoint" %>

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
        function showConfirmation(seatIndex) {
            var result = confirm('確定要預訂這個座位嗎？');
            if (result) {
                // 如果確定，顯示 "劃位成功!" 訊息
                alert('劃位成功!');

                // 取得 Label1 的元素並修改內容
                var label = document.getElementById('Label1');
                label.innerHTML = '劃位成功！座位編號：' + (seatIndex + 1);

                // 設置 Label1 的可見性為 true
                label.style.display = 'block';
                showAlert("已發送預訂資訊到您的信箱!")
            }
            return result;
        }
        function showAlert(message) {
            alert(message);
            // 更新 Label2 的文字內容
            var label = document.getElementById('Label2');
            label.innerHTML = "已發送預訂資訊到您的信箱!";
            label.style.display = 'block';
        }
    </script>
    <title>線上預約劃位</title>
    <webopt:bundlereference runat="server" path="~/Content/css" />
    <style>
        .Appoint {
            min-width: 100vh;
            min-height: 100vh;
            background-color: antiquewhite;
            position: relative;  
            padding-top: 20px;
            padding-bottom: 20px;
            display: flex;
            flex-direction: column;
            align-items: center;
        }
        .Appoint h2{
            color: saddlebrown;
        }
        .Appoint p{
            padding: 0;
            margin: 5px;
        }
        .Appoint-page{
            color: peru;
            display: flex;
            flex-direction: column;
            margin: 10px;
        }
        .Place-page{
            display: flex;
            height: 100%;
        }
        
        .seat-container {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }
        .seat {
            width: calc(5% + 4px);
            height: 40px;
            margin: 2px;
            text-align: center;
            cursor: pointer;

        }
        .reserved {
            width: calc(5% + 4px);
            height: 40px;
            margin: 2px;
            text-align: center;
            background-color: red;
            color: white;
        }
        #Label1 {
            display: none;
            color:indianred;
            font-size: 25px;
        }
        #Label2 {
            display: none;
            color:indianred;
            font-size: 25px;
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
    <form id="form1" runat="server" enctype="multipart/form-data">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
        <div class="Appoint">
            <h2>線上預約劃位</h2>
            <div class="Appoint-page">
                <!-- 日期選擇器 -->
                <asp:TextBox ID="txtDate" runat="server" placeholder="選擇日期"></asp:TextBox>
                <asp:Calendar ID="calDate" runat="server" OnSelectionChanged="calDate_SelectionChanged"></asp:Calendar>
                
                <!-- 場地選擇器 -->
                <div class="Place-page">
                    <p>選擇場地</p>
                    <asp:DropDownList ID="ddlVenue" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="場地1" Value="場地1"></asp:ListItem>
                        <asp:ListItem Text="場地2" Value="場地2"></asp:ListItem>
                        <asp:ListItem Text="場地3" Value="場地3"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <!-- 預約按鈕 -->
                <asp:Button ID="btnReserve" runat="server" Text="預約" OnClick="btnReserve_Click" />
                <asp:Label ID="Label1" runat="server" Text="Label" ></asp:Label>
                <asp:Label ID="Label2" runat="server" Text="Label" ></asp:Label>
            </div>
            <div class="seat-container">
                <!-- 顯示場地狀態的 Repeater -->
                <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                       <asp:Panel ID="pnlSeatMap" runat="server">
                            <asp:Repeater ID="repeaterSeatMap" runat="server" OnItemCommand="repeaterSeatMap_ItemCommand">
                                <ItemTemplate>
                                    <asp:Button ID="btnSeat" runat="server" CssClass='<%# GetSeatCssClass(Container.ItemIndex) %>'
                                        CommandName="SelectSeat" CommandArgument='<%# Container.ItemIndex %>'
                                        Text='<%# (Container.ItemIndex + 1) %>' OnClientClick='<%# "return showConfirmation(" + Container.ItemIndex + ");" %>'/>
                                </ItemTemplate>
                          
                            </asp:Repeater>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </form>
</body>
</html>
