<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="OptimizerCalendar.ascx.vb" Inherits="CoastalPortal.OptimizerCalendar" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register src="FOSFlightsCalendar.ascx" tagname="FOSFlights" tagprefix="uc1" %>

    <style type="text/css">
    body,td,th {
	    font-family: Arial, Helvetica, sans-serif;
	    
    }
    body
    {
        margin:0px;
        font-family: Arial, Helvetica, sans-serif;
        font-size: 12px;
        /*background-image:url(Images/SATSair/bg_repeat.gif);*/
        /*background-repeat:repeat-x;*/
        /*background-color:#FAFAFA;*/
    }
    </style>
    <link href="../StyleSheet.css" rel="stylesheet" type="text/css" />

<%--<telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server"></telerik:RadStyleSheetManager>--%>
<%--<telerik:RadScriptManager ID="RadScriptManager1" runat="server">
    <scripts>
        <asp:ScriptReference Assembly="Telerik.Web.UI" 
            Name="Telerik.Web.UI.Common.Core.js">
        </asp:ScriptReference>
        <asp:ScriptReference Assembly="Telerik.Web.UI" 
            Name="Telerik.Web.UI.Common.jQuery.js">
        </asp:ScriptReference>
        <asp:ScriptReference Assembly="Telerik.Web.UI" 
            Name="Telerik.Web.UI.Common.jQueryInclude.js">
        </asp:ScriptReference>
    </scripts>
</telerik:RadScriptManager>--%>
<%--<div style="width: 1575px; background-color: #FFFFFF;">--%>
<div style="width: 100%; background-color: #FFFFFF;">
    <table>
        <tr>
            <td style="width: 35%">
                <span class="description find">
                    <label> Model Run ID: </label>
                    <%--<asp:TextBox ID="find_region_txt" runat="server" CssClass="txt" />--%>
                    <asp:Label ID="lblModelRunID" runat="server" Font-Bold="True" Font-Size="Small" ></asp:Label>
                </span>
            </td>
            <td style="width: 30%">
                <div align="center">
                    <telerik:RadComboBox ID="rcbModelRun" runat="server" 
                        EmptyMessage="Select Model Run" Width="95%" AutoPostBack="True">
                    </telerik:RadComboBox>
                    <%--&nbsp;&nbsp;
                    <asp:LinkButton ID="LinkButtonLoadReport" runat="server">Load</asp:LinkButton>--%>
                </div>
            </td>
            <td style="width: 35%">
                <div align="right">
                    <span class="description find">
                        <asp:Label ID="lblDev" runat="server" Text="Use Dev "></asp:Label>
                        <asp:CheckBox ID="chkDev" runat="server" />
                    </span>
                </div>
            </td>
        </tr>
        <tr>
            <td style="width: 35%">
                <div align="left">
                    <span class="description find">
                        <asp:Label ID="lblModelDesc" runat="server" Font-Bold="True" 
                            Font-Size="Small"></asp:Label>
                    </span>
                </div>
            </td>
            <td style="width: 30%"></td>
            <td style="width: 35%"></td>
        </tr>
    </table>
    <br />
    <br />
    <table>
        <tr>
            <asp:Panel ID="pnlPrintFOS" runat="server">
                <td style="vertical-align: top"><uc1:FOSFlights ID="FOSFlights1" runat="server" /></td>
            </asp:Panel>
            <%--<asp:Panel ID="pnlPrintCAS" runat="server">
                <td style="vertical-align: top"><uc2:CASFlights ID="CASFlights1" runat="server" /></td>
            </asp:Panel>--%>
        </tr>
    </table>
    <%--<asp:HiddenField ID="hddnModelRunID" runat="server" />--%>
    <%--<asp:HiddenField ID="hddnModelStart" runat="server" />--%>
</div>
