<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CustomerLogin.aspx.vb" Inherits="CoastalPortal.loginpage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> User Login</title>
    <link rel="shortcut icon" href="Images/cat.ico" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link href="css/style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="style.css" type="text/css" media="screen" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <!--[if lt IE 9]><script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script><![endif]-->
    <style type="text/css">
        body,td,th {
	        font-family: Arial, Helvetica, sans-serif;
	    
        }
        body
        {
            margin:0px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }
    </style>
</head>
<body>

    <section class="login" id="login" runat="server">
	<header class="menu">
		<div class="wrapper">
			<div class="menu__left">
				<ul>
					<li><a href="#">Run Optimizer</a></li>
					<%--<li><a href="#">AOG Recovery</a></li>--%>
					<li><a href="#">Model Run History</a></li>
				</ul>
			</div>
			<div class="logo">
				<a href="#">
					<img src="~/Images/logo.png" alt="" id="imglogo" runat="server" width="112" />
				</a>
			</div>
			<div class="menu__right">
				<ul>
					<li><a href="#">Review Flight Change Reports</a></li>
					<li><a href="#">Flight Schedule </a></li>
					<li><a href="##registration__form">New User</a></li>
					<%--<li><a href="#">Operations Dashboard</a></li>--%>
				</ul>
			</div>
			
			<div class="trigger" id="trigger">
				<i></i>
				<i></i>
				<i></i>
			</div>
			
			<div class="header__title">
                <asp:Label ID="lblCarrier" runat="server" Text="TMC"></asp:Label>
				 <%--&nbsp;OPTIMIZER PORTAL BY COASTAL--%> 
			</div>
	
			<div class="menu__mobile" id="mainmenu">
				<ul>
					<li><a href="#">Run Optimizer</a></li>
					<%--<li><a href="#">AOG Recovery</a></li>--%>
					<li><a href="#">Model Run History</a></li>
					<li><a href="#">Review Flight Change Reports</a></li>
					<li><a href="#">Flight Schedule </a></li>
					<li><a href="##registration__form">New User</a></li>
					<%--<li><a href="#">Operations Dashboard</a></li>--%>
				</ul>
			</div>	
		</div>	
	</header>
<form runat="server">
    <section class="login__form">
		<div class="login__boxwrap">
            <div id="login__form" class="wraps__form">
				<div class="login__box">
					<span class="form_title">log in</span>
                    <div class="form">
				<label>
						<p class="sub_title ico1">User ID:</p>
                        <asp:TextBox ID="txtEmail" runat="server" TabIndex="1" CssClass="txt" placeholder="User ID:"   ToolTip="Please enter your email address." ></asp:TextBox>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmail" CssClass="field-validation-error" ErrorMessage="User ID required" Font-Bold="True" ForeColor="#CC0000" />
					</label>
					<label>
						<p class="sub_title ico2">Password:</p>
						<asp:TextBox ID="txtPin" runat="server" CssClass="txt" placeholder="Password:"   TabIndex="2" TextMode="Password"  ToolTip="Please enter your pin."></asp:TextBox>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPin" CssClass="field-validation-error" ErrorMessage="Password required" Font-Bold="True" ForeColor="#CC0000" />
					</label>
					<label class="nom">
                        <asp:Button CssClass="button" Text="LOG IN" runat="server" ToolTip="Log In" ID="imgLogin" />
					</label>
					<label class="nom">
						<%--<input type="checkbox" class="checkbox" id="chkbox" />--%>
                        <asp:CheckBox ID="chkbox" runat="server" class="checkbox" Checked="True" Visible="False"></asp:CheckBox>
						<label class="labeltxt" for="checkbox" style="visibility: hidden">Keep Me Logged In</label>
					</label>
					<label class="nom no">
                        	<%--<a href="#login__password" class="forgot">forgot password</a>--%>
							<%--<a href="#registration__form" class="forgot sign_form">Sign Up</a>--%>
                         <%--<asp:Literal ID="password_control" Text="" runat="server" Visible="false"/>--%>
                            
                        <asp:TextBox CssClass="txt" ID="txtmsg" runat="server" Visible="false"> </asp:TextBox>

					</label>  
                    </div>
				</div>
			</div>


   <%--     			<div id="login__password" class="wraps__form">
				<div class="login__box">
					<span class="form_title">forgot password</span>
                    <div class="form">
						<label>
							<p class="sub_title ico3">enter your email:</p>
                             <asp:TextBox ID="txtEmailrestore" runat="server" TabIndex="1" CssClass="txt" placeholder="User ID:"   ToolTip="Please enter your email address." ></asp:TextBox>
						</label>
						<label class="nom">
                            <asp:Button CssClass="button" Text="EMAIL PASSWORD" runat="server" ToolTip="Email Password" ID="but_Forgot" />
						</label>
						<label class="nom no">
							<a href="#login__form" class="forgot login_form">login</a>
						</label>
		            </div>
				</div>
			</div>--%>

<%--        			<div id="registration__form" class="wraps__form">
				<div class="login__box">
					<span class="form_title">Registration</span>
					<div class="form">
						<label class="col-2">
							<p class="sub_title">First Name:</p>
							  <asp:TextBox class="txt" ID="txtFirstName" runat="server"></asp:TextBox>
						</label>
						<label class="col-2">
							<p class="sub_title">Last Name:</p>
							<asp:TextBox class="txt" ID="txtLastName" runat="server"></asp:TextBox>
						</label>
						<label class="col-0">
							<p class="sub_title">Address:</p>
							<asp:TextBox class="txt" ID="txtAddr1" runat="server"></asp:TextBox>
						</label>
						<label class="col-2">
							<p class="sub_title">City:</p>
							<asp:TextBox class="txt" ID="txtCity" runat="server"></asp:TextBox>
						</label>
						<label class="col-3">
							<p class="sub_title">State:</p>
                              <asp:DropDownList ID="ddlState" runat="server" DataTextField="description" DataValueField="value"></asp:DropDownList>
						</label>
						<label class="col-3">
							<p class="sub_title">Zip:</p>
							<asp:TextBox class="txt" ID="txtZip" runat="server"></asp:TextBox>
						</label>
						<label class="col-2">
							<p class="sub_title">Country of Residence:</p>
                              <asp:DropDownList ID="ddlCountryofResidence" runat="server"  DataTextField="description" DataValueField="value"></asp:DropDownList>
						</label>
						<label class="col-2">
							<p class="sub_title">Cell Phone:</p>
							<asp:TextBox class="txt" ID="txtCellPhone" runat="server"></asp:TextBox>
						</label>
						
						<label class="col-2">
							<p class="sub_title">Day Phone:</p>
							<asp:TextBox class="txt" ID="txtDayPhone" runat="server"></asp:TextBox>
						</label>
						<label class="col-2">
							<p class="sub_title">Eve Phone:</p>
							<asp:TextBox class="txt" ID="txtNightPhone" runat="server"></asp:TextBox>
						</label>
						<label class="col-2">
							<p class="sub_title">Email Address:</p>
							<asp:TextBox class="txt" ID="txtEmailreg" runat="server"></asp:TextBox>
						</label>
						<label class="col-2">
							<p class="sub_title">Confirm Email Address:</p>
							<asp:TextBox class="txt" ID="txtEmailConfirm" runat="server"></asp:TextBox>
						</label>

                        <label class="col-2">
							<p class="sub_title">Password:</p>
							<asp:TextBox class="txt" ID="txtPinReg"  TextMode="Password" runat="server"></asp:TextBox>
						</label>
                        	<label class="col-2">
							<p class="sub_title">Confirm Password:</p>
							<asp:TextBox class="txt" ID="txtPinRegConfirm"  TextMode="Password" runat="server"></asp:TextBox>
						</label>

						<label class="col-2">
							<p class="sub_title"> Date of Birth:</p>
							<asp:TextBox class="txt" ID="txtDateOfBirth" runat="server"></asp:TextBox> 
						</label>

						<label class="col-2">
							<p class="sub_title">City of Birth:</p>
							<asp:TextBox class="txt" ID="txtCityofBirth" runat="server"></asp:TextBox>
						</label>
						<label class="col-2">
							<p class="sub_title">State or Province of Birth:</p>
							<asp:TextBox class="txt" ID="txtStateofBirth" runat="server"></asp:TextBox>
						</label>
						<label class="col-2">
							<p class="sub_title">Citizenship:</p>
                              <asp:DropDownList ID="ddlCitizenship" runat="server" DataTextField="description" DataValueField="value"></asp:DropDownList>
						</label>
			
						<label class="nom">
                            <asp:Button CssClass="button" Text="REGISTRATION" runat="server" ToolTip="Log In" ID="but_signup" />
						</label>
						<label class="nom no">
							<a href="#login__form" class="forgot">login</a>
						</label>
					</div>
				</div>
               </div>--%>
			</div>
	</section>
    </form>

        <footer>
		<div class="wrapper">
			<span class="copyring">
				<p>© 2017 CoastalAVTech LTD</p>
				<p>All Rights Reserved</p>
			</span>
			
			<span class="social">
				<ul>
					<li><a href="#">Support </a></li>
					<li><a href="#">Contact Us </a></li>
					<li><a href="#"><i class="ico ico1"></i></a></li>
					<li><a href="#"><i class="ico ico2"></i></a></li>
					<li><a href="#"><i class="ico ico3"></i></a></li>
					<li><a href="#"><i class="ico ico4"></i></a></li>
				</ul>
			</span>
		</div>
	</footer>
</section>





    <script type="text/javascript" src="js/jQuery_v1.11.js"></script>
    <script type="text/javascript" src="js/script.js"></script>
    <script type="text/javascript" src="js/core.js"></script>
    <script type="text/javascript" src="js/jquery.maskedinput.min.js"></script>
</body>
</html>
