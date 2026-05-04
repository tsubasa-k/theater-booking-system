<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebForm1.Login" %>

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
    <title>登入頁面</title>    
    <webopt:bundlereference runat="server" path="~/Content/css" />
    <style>
         
        .Login {
            min-width: 100vh;
            min-height: 100vh;
            background-color: antiquewhite;
            position: relative;
            padding: 20px;
            display: flex;
            flex-direction: column;
            align-items: center;
            font-size: 20px;
        }
        .Login h2{
            color: saddlebrown;
        }
        .Login p{
            padding: 0;
            margin: 5px;
        }
        .Login-page{
            color: peru;
            display: flex;
            flex-direction: column;
        }
        #txtAccount{
            height: 30px;
        }
        #txtPassword{
            height: 30px;
            margin-bottom: 10px;
        }
        #btnLogin {
            width: 230px;
            height: 50px;
            font-size: 20px;
            background-color: rosybrown;
            border: none; 
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
                </ul>
            </div>
        </div>
    </nav>
    <form id="formLogin" runat="server">
        <div class="Login">
            <h2>使用者登入</h2>
            <div class="Login-page">
                <asp:Label ID="lblResult" runat="server" Visible="false" ></asp:Label>  
                <p>帳號</p>
                <asp:TextBox ID="txtAccount" runat="server" placeholder="帳號" ></asp:TextBox>             
                <p>密碼</p>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="密碼"  ></asp:TextBox>
                <asp:Button ID="btnLogin" runat="server" Text="登入" OnClick="LoginUser"></asp:Button>
                <asp:Label ID="Label1" runat="server" Text="Label" Visible="false"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>

