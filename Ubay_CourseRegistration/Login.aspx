<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Ubay_CourseRegistration.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
	<!--===============================================================================================-->	
	<link rel="icon" type="image/png" href="images/icons/favicon.ico"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="vendor/bootstrap/css/bootstrap.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="fonts/font-awesome-4.7.0/css/font-awesome.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="fonts/iconic/css/material-design-iconic-font.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="vendor/animate/animate.css" />
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="vendor/css-hamburgers/hamburgers.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="vendor/animsition/css/animsition.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="vendor/select2/select2.min.css" />
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="vendor/daterangepicker/daterangepicker.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="css/util.css" />
	<link rel="stylesheet" type="text/css" href="css/main.css" />
<!--===============================================================================================-->
</head>
<body>
    <form id="form1" runat="server" class="login100-form validate-form">
	<div class="limiter">
		<div class="container-login100">
			<div class="wrap-login100">
					<span class="login100-form-title p-b-26">
                        <a href="Login.aspx"><img src="images/LOGO-removebg-preview.png" width="100" /></a>
					</span>

					<div class="wrap-input100 validate-input" data-validate = "請輸入帳號">
						<asp:TextBox runat="server" class="input100" type="text"  name="帳號" ID="txtAccount" />
						<span class="focus-input100" data-placeholder="帳號"></span>
					</div>
					<div class="wrap-input100 validate-input" data-validate="請輸入密碼">
						<span class="btn-show-pass">
							<i class="zmdi zmdi-eye"></i>
						</span>
						<asp:TextBox ID="txtPassword" runat="server" class="input100" type="password" name="密碼"></asp:TextBox>
						<span class="focus-input100" data-placeholder="密碼"></span>
					</div><br />

                <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                    <asp:Literal ID="ltMessage" runat="server"></asp:Literal>
                </asp:PlaceHolder>

					<div class="container-login100-form-btn">
						<div class="wrap-login100-form-btn">
							<div class="login100-form-bgbtn"></div>
							<button runat="server" class="login100-form-btn" onserverclick="Button1_Click" id="Login1" type="submit" name="btn1" >
								登入
							</button>
						</div>
					</div>

					<div class="text-center p-t-115">

						<a class="txt2" href="../Students/StudentSighUP.aspx">
							註冊
						</a>
					</div>
			</div>
		</div>
	</div>
	

	<div id="dropDownSelect1"></div>
	
<!--===============================================================================================-->
	<script src="vendor/jquery/jquery-3.2.1.min.js"></script>
<!--===============================================================================================-->
	<script src="vendor/animsition/js/animsition.min.js"></script>
<!--===============================================================================================-->
	<script src="vendor/bootstrap/js/popper.js"></script>
	<script src="vendor/bootstrap/js/bootstrap.min.js"></script>
<!--===============================================================================================-->
	<script src="vendor/select2/select2.min.js"></script>
<!--===============================================================================================-->
	<script src="vendor/daterangepicker/moment.min.js"></script>
	<script src="vendor/daterangepicker/daterangepicker.js"></script>
<!--===============================================================================================-->
	<script src="vendor/countdowntime/countdowntime.js"></script>
<!--===============================================================================================-->
	<script src="js/main.js"></script>
    </form>
</body>
</html>
