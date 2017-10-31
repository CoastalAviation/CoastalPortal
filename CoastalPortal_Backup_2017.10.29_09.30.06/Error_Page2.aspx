<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Error_Page2.aspx.vb" Inherits="CoastalPortal.Error_Page2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Communication Link Failure</title>
    <link rel="shortcut icon" href="Images/cat.ico" />
    <link rel="stylesheet" type="text/css" href="StyleSheet.css" />
    <style type="text/css">
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <table width="100%" 
        
        style="font-family: Tahoma; font-size: medium; color: #FFFFFF; font-weight: bold;" >
        <tr align="center">
            <td align="center" style="padding-top: 120px;padding-bottom: 20px;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/aw snap.PNG" />
            </td>
        </tr>
        <tr align="center">
            <td align="center" style="color: #FF0000">
                Unknown Carrier ID<br /><br />Please enter the complete URL 
                starting with your carrier name and try again.            
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
