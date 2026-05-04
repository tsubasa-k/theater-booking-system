<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Booking.aspx.cs" Inherits="WebForm1.Booking" %>

<!DOCTYPE html>
<html>
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
            var result = confirm('確定要預訂這個場地嗎？');
            if (result) {
                // 如果確定，顯示 "劃位成功!" 訊息
                alert('預訂成功!');

                showAlert("已發送預訂資訊到您的信箱!")
            }
           
            return result;
        }
        function showAlert(message) {
            alert(message);
            // 更新 Label1 的文字內容
            var label = document.getElementById('Label1');
            label.innerHTML = "已發送預訂資訊到您的信箱!";
            label.style.display = 'block';
        }
    </script>
    <title>場地預約</title>
    <webopt:bundlereference runat="server" path="~/Content/css" />
    <style>
      
     .Booking {
         min-width: 100vh;
         min-height: 100vh;
         color: saddlebrown;
         background-color: antiquewhite;
         position: relative;
         padding-top: 20px;
         padding-bottom: 20px;
         display: flex;
         flex-direction: column;
         align-items: center;
         font-size: 20px;
     }
     .Booking p{
         padding: 0;
         margin: 5px;
     }
     .flex-container {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-evenly;
        color: saddlebrown;
        margin-bottom: 10px;
     }
     
     .Booking-page #Label1 {
        display: none;
        color:indianred;
        font-size: 25px;
     }
     .panel {
         padding: 10px;
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
    <form id="formBooking" runat="server">
        <div class="Booking">
            <h2>場地預約</h2>
            <div class="Booking-page">
                <div class="Func-page">
                    <asp:RadioButtonList ID="rblFunction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblFunction_SelectedIndexChanged">
                        <asp:ListItem Text="預約功能" Value="0" Selected="True" />
                        <asp:ListItem Text="查詢功能" Value="1" />
                    </asp:RadioButtonList>
                    <asp:Label ID="Label1" runat="server" Text="Label" ></asp:Label>

                    <asp:MultiView ID="MultiView1" runat="server">
                        <!-- View 1: 預訂表單視圖 -->
                        <asp:View ID="View1" runat="server">

                            <!-- 顯示場地列表 -->
                            <asp:Label ID="lblVenue" runat="server" Text="選擇場地："></asp:Label>
                            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnTextChanged="SelectedIndexChanged">
                                <asp:ListItem Text="場地1" Value="場地1"></asp:ListItem>
                                <asp:ListItem Text="場地2" Value="場地2"></asp:ListItem>
                                <asp:ListItem Text="場地3" Value="場地3"></asp:ListItem>
                            </asp:DropDownList>

                            <!-- 選擇日期和時間 -->
                            <label for="txtDate">預訂日期：</label>
                            <asp:TextBox ID="txtDate" runat="server" type="date" OnTextChanged="txtDate_TextChanged"></asp:TextBox>

                            <asp:Button ID="btnGetAvailableTimes" runat="server" Text="預訂時間" OnClick="btnGetAvailableTimes_Click" />
                            <label for="txtTime">預訂時間：</label>
                            <asp:DropDownList ID="DropDownList2" runat="server"></asp:DropDownList>


                            <!-- 提交預訂請求 -->
                            <asp:Button ID="Button1" runat="server" Text="提交預訂請求" OnClick="btnSubmit_Click" OnClientClick='<%# "return showConfirmation();" %>'/>
                        </asp:View>

                        <!-- View 2: 預訂查詢視圖 -->
                        <asp:View ID="View2" runat="server">
                            <!-- 加入場地列表等元件 -->
                            <asp:DropDownList ID="ddlVenues" runat="server">
                                <asp:ListItem Text="場地1" Value="場地1"></asp:ListItem>
                                <asp:ListItem Text="場地2" Value="場地2"></asp:ListItem>
                                <asp:ListItem Text="場地3" Value="場地3"></asp:ListItem>
                            </asp:DropDownList>
                            <!-- 選擇日期和時間 -->
                            <label for="txtDate">開始日期：</label>
                            <asp:TextBox ID="startdate" runat="server" type="date"></asp:TextBox>
                            <label for="txtDate">結束日期：</label>
                            <asp:TextBox ID="enddate" runat="server" type="date"></asp:TextBox>
                            <!-- 查詢按鈕 -->
                            <asp:Button ID="Button2" runat="server" Text="查詢預訂" OnClick="btnSearch_Click" />
                            <!-- 顯示預訂查詢結果 -->
                            <div class="panel">
                                <asp:Panel ID="panelBookingInfo" runat="server" CssClass="flex-container"></asp:Panel>
                            </div>
                        </asp:View>
                </asp:MultiView>
                </div>
             </div>
        </div>
    </form>
</body>
</html>

