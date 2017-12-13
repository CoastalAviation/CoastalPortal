<%@ Page Title="Flight Change Detail Report" Language="vb" AutoEventWireup="false" CodeBehind="FlightChangeDetail.aspx.vb" Inherits="CoastalPortal.FlightChangeDetail" EnableEventValidation="false" %>
<%@ Import Namespace="CoastalPortal.AirTaxi" %>
<%@ Import Namespace="System.Web.Services.Description" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Flight Change Detail Reports</title>
    <link rel="shortcut icon" href="Images/cat.ico" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link href="style.css" rel="stylesheet" type="text/css" />
	<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1"/>
	<!--[if lt IE 9]><script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script><![endif]-->

    <style type="text/css">
        /*
        .auto-style15 {
           
        }
        */
            .image_cell {
        padding-left: 10px;
        padding-right: 15px;
        padding-top: 3px;
        padding-bottom: 3px;
    }

    .demo_item_title {
        margin-left: 5px;
        margin-top: 4px;
        font-family: Arial;
        font-size: 13px;
        font-weight: bold;
        color: #004b91;
    }

    .demo_item {
        font-size: 10px;
        color: #004b91;
    }

    a:link, a:visited {
    }

    .borderbreak {
        border-left: 8px solid white;
    }

    .auto-style1 {
        height: 27px;
    }


    .auto-style6 {
        width: 100%;
    }


    .auto-style9 {
        float: left;
        width: 142px;
    }

    .auto-style10 {
        width: 142px;
    }

    .auto-style11 {
        width: 146px;
    }


    .auto-style12 {
        height: 14px;
    }

    .auto-style14 {
        float: left;
        width: 151px;
        height: 14px;
    }

    .auto-style15 {
        float: left;
        width: 142px;
        height: 14px;
    }

    .auto-style16 {
        width: 146px;
        height: 14px;
    }

    .auto-style17 {
        width: 147px;
    }

    .auto-style18 {
        height: 14px;
        width: 147px;
    }

    .auto-style19 {
        float: left;
        width: 147px;
    }

    .auto-style20 {
        float: left;
        width: 146px;
    }

    .auto-style22 {
        float: left;
        width: 146px;
        height: 14px;
    }

    .auto-style23 {
        width: 151px;
    }

    .auto-style24 {
        float: left;
        width: 151px;
    }

    </style>
        <base target="_blank" />
        <script type="text/javascript" id="telerikClientEvents1">
            function cmdFindFlights_Clicking(sender, args) {
                document.getElementById("PleaseWait").style.visibility = "visible";
            }
            function onLoad(sender) {
                var div = sender.get_element();

                $telerik.$(div).bind('mouseenter', function () {
                    if (!sender.get_dropDownVisible())
                        sender.showDropDown();
                });

                $telerik.$(".RadComboBoxDropDown").mouseleave(function (e) {
                    hideDropDown("#" + sender.get_id(), sender, e);
                });

                $telerik.$(div).mouseleave(function (e) {
                    hideDropDown(".RadComboBoxDropDown", sender, e);
                });
            }

            function hideDropDown(selector, combo, e) {
                var tgt = e.relatedTarget;
                var parent = $telerik.$(selector)[0];
                var parents = $telerik.$(tgt).parents(selector);

                if (tgt != parent && parents.length == 0) {
                    if (combo.get_dropDownVisible())
                        combo.hideDropDown();
                }
                combo.get_inputDomElement().blur();
                combo._raiseClientBlur(e);
                combo._focused = false;
            }
    </script>

</head>

     <telerik:RadCodeBlock ID="radCodeBlock" runat="server">
        <script type="text/javascript" language="javascript">
            var daaRecieved;


            function ShowDepart(EmpId) {
                var oManager = GetRadWindowManager();
                var oWnd = oManager.GetWindowByName("wndShowDepart");
                var url = 'AcceptRejectChange.aspx?Id=' + EmpId + '&Rnd=' + Math.random();
                oWnd.SetUrl(url);
                oWnd.Show();
                return false;
            }

            function ShowEmpty(EmpId) {
                var oManager = GetRadWindowManager();
                var oWnd = oManager.GetWindowByName("wndShowDepart");
                var url = 'PostForSale.aspx?Id=' + EmpId + '&Rnd=' + Math.random();
                oWnd.SetUrl(url);
                oWnd.Show();
                return false;

            }

            function OnDataAdded(data) {
                daaRecieved = data;
            }

            function onWindowClose(sender, eventArgs) {
                var args;
                if (daaRecieved != '') {
                    args = "data!" + daaRecieved;
                    <%--window["<%= RadAjaxManager1.ClientID%>"].ajaxRequest(args);--%>
                }
            }

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) {
                    oWindow = window.radWindow;
                }
                else if (window.frameElement.radWindow) {
                    oWindow = window.frameElement.radWindow
                }

                return oWindow;
            }
            function CloseWindow(arg) {
                var oWnd = GetRadWindow();
                oWnd.Close(arg);
            }
           
           function SizeToFit()
           {
               window.setTimeout(
                   function()
                   {
                       var oWnd = GetRadWindow();
                       oWnd.SetWidth(document.body.scrollWidth + 4);
                       oWnd.SetHeight(document.body.scrollHeight + 70);
                
                   }, 400);
           }
        </script>
    </telerik:RadCodeBlock>
<body ><%-- onload="if (typeof window.opener != 'undefined') window.opener.location.href = this.href; return false;"> --%>
    <form id="form1" runat="server">
        <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server"></telerik:RadStyleSheetManager>
        <telerik:RadScriptManager ID="ScriptManager1" runat="server" EnableTheming="True">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
                </asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
                </asp:ScriptReference>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
                </asp:ScriptReference>
            </Scripts>
        </telerik:RadScriptManager>
        <script type="text/javascript">
            //global variables for the countries and cities comboboxes
            var countriesCombo;
            var citiesCombo;
            var continentCombo;

            function pageLoad() {
                // initialize the global variables
                // in this event all client objects 
                // are already created and initialized

                //                            toggleoptions();

            }

            function HandleKeyPress(sender, eventArgs) {
                if (eventArgs.get_domEvent().keyCode == 13) {
                    sender.raise_SelectedIndexChanging();
                }
            }

            //                        function toggleoptions(sender, eventArgs) {
            //                            $('#RadComboFlexFrom').toggle();
            //                            $('#RadComboFlexto').toggle();
            //                            $('#RadComboBoxFlexMiles').toggle();
            //                            $('#RadComboBoxACInclude').toggle();
            //                            $('#RadComboBoxACExclude').toggle();
            //                            $('#RadComboBoxCertifications').toggle();
            //                            $('#RadComboBoxFlexTo').toggle();
            //                            $('#RadComboBoxRequests').toggle();

            //                        }

            function ProdSearch(sender, eventArgs) {

                //  document.getElementById("PleaseWait").style.visibility = "visible";
            }

            function OnClientItemsRequesting(sender, eventArgs) {
                if (eventArgs.get_text().length < 1)
                    eventArgs.set_cancel(true)
                else
                    eventArgs.set_cancel(false);
            }

            function OnClientDropDownOpening1(sender, eventArgs) {

                if (sender.get_items().get_count() == 0)
                    eventArgs.set_cancel(true);
            }

            function ItemsLoaded(sender, eventArgs) {

                if (sender.get_items().get_count() > 0) {
                    sender.showDropDown();
                    //window.setTimeout(function () { sender.ShowDropDown(); }, 100);
                }

            }

            function onKeyPressing(sender, eventArgs) {
                var keyCode = eventArgs.get_domEvent().keyCode;

                if (keyCode == 13) {
                    var item = findItemByText2(sender.get_text());
                    if (item) item.select();
                }
            }

            function HandleOpen(combobox) {
                if (combobox.get_items().get_count() > 0) {
                    return true;
                }
                { return false; }
            }

        </script>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>

<section class="content no-img">
	<header class="menu black">
		<div class="wrapper">
			<div class="menu__left">
				<ul>
					<li><a href="RunOptimizer.aspx">Run Optimizer</a></li>
					<%--<li><a href="AOGRecovery.aspx">AOG Recovery</a></li>--%>
					<li><a href="ModelRunHistory.aspx">Model Run History</a></li>
					<%--<li><a href="#">Model Run History</a></li>--%>
					<li><asp:LinkButton ID="LinkQuoting" runat="server" Visible="False">Rev Mgmt</asp:LinkButton></li>
				</ul>
			</div>
			<div class="logo">
				<a href="RunOptimizer.aspx">
					<img src="~/Images/logo_blue.png" alt="" id="imglogo" runat="server" width="112" />
				</a>
			</div>
			<div class="menu__right">
				<ul>
					<li><a href="FlightChangeReports.aspx">Review Flight Change Reports</a></li>
					<li><a href="FlightSchedule.aspx">Flight Schedule</a></li>
					<%--<li><a href="#">Log Off</a></li>--%>
                    <li><asp:LinkButton ID="LinkLogOut" runat="server">Log Off</asp:LinkButton></li>
					<%--<li><a href="Dashboard.aspx">Operations Dashboard</a></li>--%>
				</ul>
			</div>
			
			<div class="trigger" id="trigger">
				<i></i>
				<i></i>
				<i></i>
			</div>
			
			<div class="header__title small__padding">
                <asp:Label ID="lblCarrier" runat="server" Text="TMC"></asp:Label>
				 <%--&nbsp;OPTIMIZER PORTAL BY COASTAL--%> 
			</div>
			
			<div class="menu__mobile" id="mainmenu">
				<ul>
					<li><a href="RunOptimizer.aspx">Run Optimizer</a></li>
					<%--<li><a href="AOGRecovery.aspx">AOG Recovery</a></li>--%>
					<li><a href="ModelRunHistory.aspx">Model Run History</a></li>
					<%--<li><a href="#">Model Run History</a></li>--%>
					<li><a href="FlightChangeReports.aspx">Review Flight Change Reports</a></li>
					<li><a href="FlightSchedule.aspx">Flight Schedule</a></li>
					<%--<li><a href="#">Log Off</a></li>--%>
                    <li><asp:LinkButton ID="LinkLogOut2" runat="server">Log Off</asp:LinkButton></li>
					<%--<li><a href="Dashboard.aspx">Operations Dashboard</a></li>--%>
				</ul>
			</div>	
		</div>	
	</header>	
</section>

<section class="article nopadding">
     <%-- %>    <div class="form__order2" id="form_1" runat="server"> --%>
                <div class="title">Flight Change Detail Report</div>
                <div class="title">
                    <asp:Label runat="server" ID="aircraft_type_txt_1" CssClass="title"></asp:Label>
                </div>
    <div style="align-items:center; padding-left:30px">
        <table id="tryme" runat="server">
            <tr style="vertical-align: top;">
                <td style="vertical-align: top; padding-right: 4px;" >
               <%-- %>     <div style="background-color: #ffff; width: auto; font-family: Arial, Helvetica, sans-serif">class="auto-style15"
                        <div>--%>
                            <asp:ListView ID="lvflightlist" runat="server" ItemType="CoastalPortal.PanelDisplay" >
                                <LayoutTemplate>
                                    <table  style="padding-left: 10px; color: #00ff00">
                                        <thead>
                                            <th></th>
                                        </thead>
                                        <tbody>
                                            <tr runat="server" id="itemPlaceholder"></tr>
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <table style="vertical-align: top;">
                                        <tr>
                                            <td>
                                                <asp:Label runat="server" Font-Size="Medium" Text="Change in Non Revenue Miles:">
                                                    <asp:Label ID="lblNonRevDeltab" runat="server" Font-Size="Medium" Text='<%#Eval("NRM", "{0:N0}") %>' ForeColor="#00936F"></asp:Label></asp:Label>
                                                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" Font-Size="Medium" Text="Cost Savings:">
                                                    <asp:Label ID="lblCostSavings" runat="server" Font-Size="Medium" Text='<%#Eval("TotalSavings", "{0:C0}") %>' ForeColor="#00936F"></asp:Label></asp:Label>
                                                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" Font-Size="Medium" Text="Total Cost Day 0:">
                                                    <asp:Label ID="lblCostDay0b" runat="server" Font-Size="Medium" Text='<%#Eval("dcostday0", "{0:C0}") %>' ForeColor="#00936F"></asp:Label></asp:Label>
                                                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" Font-Size="Medium" Text="Total Cost Day 1:">
                                                    <asp:Label ID="lblCostDay1b" runat="server" Font-Size="Medium" Text='<%#Eval("dcostday1", "{0:C0}") %>' ForeColor="#00936F"></asp:Label></asp:Label>
                                                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" Font-Size="Medium" Text="Total Cost Day 2:">
                                                    <asp:Label ID="lblCostDay2b" runat="server" Font-Size="Medium" Text='<%#Eval("dcostday2", "{0:C0}") %>' ForeColor="#00936F"></asp:Label></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="PnlFCDR" runat="server" Visible="false" Text="<%#Item.FCDR_Key %>"></asp:Label>
                                                <asp:Label runat="server" Font-Size="Medium" Text='<%# "Starting Tail Number: " + Eval("TailNumber") %>'> </asp:Label>
                                                <asp:Label runat="server" Style="padding-left: 10px; font-size: small">ModelNumber: <a href='http://optimizerpanel.com/Panel.aspx?modelrunid=<%#Eval("ModelNumber")%>'> <%#Item.ModelNumber  %> </a></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="GVGridViewTrips" runat="server" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                                                    Font-Names="Arial" Font-Size="X-Small" Width="1500px" Style="margin-top: 0px;" AutoGenerateColumns="false"
                                                    DataSource="<%#Item.PanelRecord %>" ItemType="CoastalPortal.PanelRecord" OnPreRender="GVGridViewTrips_PreRender">
                                                    <Columns>
                                                        <asp:BoundField DataField="FOSRecord.DepartureAirportICAO" HeaderText="From" SortExpression="From" />
                                                        <asp:BoundField DataField="FOSRecord.ArrivalAirportICAO" HeaderText="To" SortExpression="To" />
                                                        <asp:TemplateField HeaderText="From GMT" SortExpression="From GMT">
                                                            <ItemTemplate>
                                                                <asp:Label ID="fosFromGMT" runat="server" Text='<%#Eval("FOSRecord.DepartureDategmt") + " " + Eval("FOSRecord.DepartureTimegmt") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="To GMT" SortExpression="From GMT">
                                                            <ItemTemplate>
                                                                <asp:Label ID="fosToGMT" runat="server" Text='<%#Eval("FOSRecord.ArrivalDateGMT") + " " + Eval("FOSRecord.ArrivalTimeGMT") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="FOSRecord.NauticalMiles" HeaderText="NM" SortExpression="NM" />
                                                        <asp:BoundField DataField="FOSRecord.AC" HeaderText="AC" SortExpression="AC" />
                                                        <asp:BoundField DataField="FOSRecord.AircraftType" HeaderText="Type" SortExpression="Type" />
                                                        <asp:BoundField DataField="FOSRecord.PAXCount" HeaderText="PAX" SortExpression="PAX" ItemStyle-HorizontalAlign="Center" />
                                                        <asp:BoundField DataField="FOSRecord.DeadHead" HeaderText="DH" SortExpression="DeadHead" />
                                                        <asp:BoundField DataField="FOSRecord.TripNumber" HeaderText="Trip#" SortExpression="Trip#" />
                                                        <asp:BoundField DataField="FOSRecord.LegTypeCode" HeaderText="LTC" SortExpression="LTC" />
                                                        <asp:BoundField DataField="FOSRecord.SIC" HeaderText="SIC" SortExpression="SIC" Visible="false" />
                                                        <asp:BoundField DataField="FOSRecord.PIC" HeaderText="PIC" SortExpression="PIC" Visible="false" />
                                                        <asp:BoundField DataField="FOSRecord.triprevenue" HeaderText="Revenue" SortExpression="Revenue" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:c0}" />
                                                        <asp:BoundField DataField="FOSRecord.DHCost" HeaderText="Cost" SortExpression="Cost" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:c0}" />
                                                        <asp:BoundField DataField="FOSRecord.PandL" HeaderText="P&L" SortExpression="P&L" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center" />
                                                        <asp:BoundField DataField="FOSRecord.BaseCode" HeaderText="Base" SortExpression="Base" />
                                                        <asp:BoundField DataField="FOSRecord.QuotedEquipType" HeaderText="QE" SortExpression="QE" />
                                                        <asp:BoundField DataField="CASModification" HeaderText="NewTail" SortExpression="NewTail" Visible="True" />
                                                        <asp:BoundField DataField="CASRecord.DepartureAirport" HeaderText="From" SortExpression="From" />
                                                        <asp:BoundField DataField="CASRecord.ArrivalAirport" HeaderText="To" SortExpression="To" />
                                                        <asp:BoundField DataField="CASRecord.DepartureTime" HeaderText="From GMT" SortExpression="From GMT" />
                                                        <asp:BoundField DataField="CASRecord.ArrivalTime" HeaderText="To GMT" SortExpression="From GMT" />
                                                        <asp:BoundField DataField="CASRecord.Distance" HeaderText="NM" SortExpression="NM" ItemStyle-HorizontalAlign="Center" />
                                                        <asp:BoundField DataField="CASRecord.AircraftRegistration" HeaderText="AC" SortExpression="AC" />
                                                        <asp:BoundField DataField="CASRecord.aircrafttype" HeaderText="Type" SortExpression="Type" />
                                                        <asp:BoundField DataField="CASRecord.FlightType" HeaderText="Rev" SortExpression="rev" />
                                                        <asp:BoundField DataField="CASRecord.TripNumber" HeaderText="Trip#" SortExpression="Trip#" />
                                                        <asp:TemplateField HeaderText="Ind">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkPTSindividual" runat="server" Text='<%# Eval("CasRecord.PriorTail") %>' OnClientClick='<%#Eval("CASRecord.id", "return ShowDepart({0});")%>'>
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="CASRecord.LegTypeCode" HeaderText="LTC" SortExpression="LTC" />
                                                        <asp:BoundField DataField="CASRecord.SIC" HeaderText="SIC" SortExpression="SIC" Visible="false" />
                                                        <asp:BoundField DataField="CASRecord.PIC" HeaderText="PIC" SortExpression="PIC" Visible="false" />
                                                        <asp:BoundField DataField="CASRecord.triprevenue" HeaderText="Revenue" SortExpression="Revenue" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:c0}" />
                                                        <asp:BoundField DataField="CASRecord.Cost" HeaderText="Cost" SortExpression="Cost" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center" />
                                                        <asp:BoundField DataField="CASRecord.PandL" HeaderText="P&L" SortExpression="P&L" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center" />
                                                        <asp:BoundField DataField="CASRecord.BaseCode" HeaderText="Base" SortExpression="Base" />
                                                        <asp:ButtonField CommandName="X" HeaderText=" Pin  " ShowHeader="True" Text="Pin" ItemStyle-ForeColor="#009999" ButtonType="Link">
                                                            <ControlStyle Width="30px" />
                                                            <HeaderStyle Width="30px" />
                                                            <ItemStyle Width="30px" ForeColor="#009999"></ItemStyle>
                                                        </asp:ButtonField>
                                                        <asp:BoundField DataField="CASRecord.PriorTail" HeaderText="PT" SortExpression="PT" />
                                                        <asp:BoundField DataField="PanelKey" HeaderText="ID" SortExpression="ID" />
                                                    </Columns>
                                                    <FooterStyle BackColor="White" ForeColor="#000066" />
                                                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                                    <RowStyle ForeColor="#000066" />
                                                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                                    <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                                    <SortedDescendingHeaderStyle BackColor="#00547E" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></br></br></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="GVbaseRevenue" runat="server" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                                                    Font-Names="Arial" Font-Size="Small" Width="700px" Style="margin-top: 0px;" AutoGenerateColumns="false"
                                                    DataSource="<%#Item.RevenueRecords %>" ItemType="Optimizer.RevenueRecords">
                                                    <Columns>
                                                        <asp:BoundField DataField="basecode" HeaderText="Base" SortExpression="Base" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center" />
                                                        <asp:BoundField DataField="FosRevenue" HeaderText="FOS P&L" SortExpression="FosRevenue" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center" />
                                                        <asp:BoundField DataField="CasRevenue" HeaderText="CAS P&L" SortExpression="CasRevenue" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center" />
                                                        <asp:BoundField DataField="GrossProfitChange" HeaderText="Gross Profit Change" SortExpression="DelatRev" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center" />
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:ListView>
                <%-- %>        </div>
                 </div>--%>
                </td>
            </tr>
        </table>
          </div>

        </section>
    </form>
       <input id="hidIsClose" type="hidden" value="0" runat="server" />
    <input id="hidData" type="hidden" runat="server" value="" />
    <script language="javascript" type="text/javascript">
           function CloseSelect() {
               var hidIsClose = document.getElementById('<%=hidIsClose.ClientID %>');
            var hidData = document.getElementById('<%=hidData.ClientID %>');

            if (hidIsClose.value != "0") {
                if (hidData.value != '') {
                    var oWnd = GetRadWindow();
                    oWnd.BrowserWindow.OnDataAdded(hidData.value);
                    CloseWindow(hidData.value);
                }
            }
           }
           CloseSelect();
    </script> 
    </body>
    </html>

