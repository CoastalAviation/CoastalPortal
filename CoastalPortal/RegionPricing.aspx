<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RegionPricing.aspx.vb" Inherits="CoastalPortal.special_pricing" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Special Pricing</title>
    <link rel="shortcut icon" href="Images/cat.ico" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link href="style.css" rel="stylesheet" type="text/css" />
	<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1"/>
	<!--[if lt IE 9]><script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script><![endif]-->
    <script type="text/javascript" id="telerikClientEvents1">
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
<body>
    <form id="form1" runat="server">
        <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
        </telerik:RadStyleSheetManager>
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
<main class="main">
<section class="content no-img">
	<header class="menu black">
		<div class="wrapper">
			<div class="menu__left">
				<ul>
					<li><a href="RunOptimizer.aspx">Run Optimizer</a></li>
					<%--<li><a href="AOGRecovery.aspx">AOG Recovery</a></li>--%>
					<li><a href="ModelRunHistory.aspx">Model Run History</a></li>
					<%--<li><a href="#">Model Run History</a></li>--%>
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
			
			<div class="header__title">
                <asp:Label ID="lblCarrier" runat="server" Text="TMC"></asp:Label>
				 &nbsp;ONLINE QUOTING SYSTEM
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
	<div class="form__order2">
		<div class="title">QUOTE SYSTEM ADMINISTRATION SPECIAL PRICING</div>
		
		<span class="description"><a class="openpopup" href="#addnew">add new special pricing</a></span>
                    <asp:Label ID="lblMsg" Text="" runat="server" forecolor="Red" />
        <span class="description find">  
        <asp:Panel ID="pnlEdit" runat="server" Visible="False" >
	        <span class="popup--title">Edit</span>
	        <div class="form">
		        <label>
			        <p class="sub_title" style="margin: 0 auto; width: 50%">from:</p>
                    <%--<asp:TextBox id="edit_from_1" runat="server" CssClass="txt " placeholder="Region name"/>--%>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 50%;" class="txt ">
                        <telerik:RadComboBox ID="ddlFrom" runat="server" EmptyMessage="Region From" EnableLoadOnDemand="True" 
                                OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                        </telerik:RadComboBox>
                        <%--<br />--%>
                            <%--<telerik:RadComboBox ID="OriginAddress2" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                            </telerik:RadComboBox>--%>
                    </div>
		        </label>
		        <label>
			        <p class="sub_title" style="margin: 0 auto; width: 50%">to:</p>
                    <%--<asp:TextBox id="edit_to_1" runat="server" CssClass="txt " placeholder="Region name"/>--%>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 50%;" class="txt ">
                        <telerik:RadComboBox ID="ddlTo" runat="server" EmptyMessage="Region To" EnableLoadOnDemand="True" 
                                OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                        </telerik:RadComboBox>
                    </div>
		        </label>
		        <label>
			        <p class="sub_title" style="margin: 0 auto; width: 25%">eff date:</p>
                    <%--<asp:TextBox id="edit_eff_1" runat="server" CssClass="txt " placeholder="eff"/>--%>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 25%;" class="txt ">
                        <telerik:RadDatePicker ID="start_date" runat="server" Culture="en-US"></telerik:RadDatePicker>
                    </div>
		        </label>
		        <label>
			        <p class="sub_title" style="margin: 0 auto; width: 25%">disc date:</p>
                    <%--<asp:TextBox id="edit_disc_1" runat="server" CssClass="txt " placeholder="disc"/>--%>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 25%;" class="txt ">
                        <telerik:RadDatePicker ID="end_date" runat="server" Culture="en-US"></telerik:RadDatePicker>
                    </div>
		        </label>
		        <label>
			        <p class="sub_title" style="margin: 0 auto; width: 50%">fleet:</p>
                    <%--<asp:TextBox id="edit_fleet_1" runat="server" CssClass="txt " placeholder="fleet"/>--%>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 50%;" class="txt ">
                        <telerik:RadComboBox ID="RadComboBoxACInclude" runat="server" EmptyMessage="Fleet Type" EnableLoadOnDemand="True" 
                                OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                            <Items>
                            </Items>
                        </telerik:RadComboBox>
                    </div>
		        </label>
		        <label>
			        <p class="sub_title">fixed price:</p>
                    <asp:TextBox id="edit_price_1" runat="server" CssClass="txt " placeholder="fixed price"/>
		        </label>
                <label>
			        <p class="sub_title">or-disc %:</p>
                    <asp:TextBox id="edit_or_sidc_1" runat="server" CssClass="txt" placeholder="or-disc %"/>
		        </label>
                <label>
			        <p class="sub_title">or-prem %:</p>
                    <asp:TextBox id="edit_or_prem_1" runat="server" CssClass="txt" placeholder="or-prem %"/>
		        </label>
                <label>
			        <p class="sub_title" style="margin: 0 auto; width: 50%">Optional day of week:</p>
                    <%--<asp:TextBox id="edit_dayweek_1" runat="server" CssClass="txt" placeholder="day of week"/>--%>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 25%;" class="txt ">
                        <telerik:RadComboBox ID="ddlDayofWeek" runat="server" EmptyMessage="Day of Week" EnableLoadOnDemand="True" 
                                OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" >
                            <Items>
                                <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Sunday" 
                                    ToolTip="Sunday" Value="Su" />
                                <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Monday" 
                                    ToolTip="Monday" Value="M" />
                                <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Tuesday" 
                                    ToolTip="Tuesday" Value="Tu" />
                                <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Wednesday" 
                                    ToolTip="Wednesday" Value="W" />
                                <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Thursday" 
                                    ToolTip="Thursday" Value="Th" />
                                <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Friday" 
                                    ToolTip="Friday" Value="F" />
                                <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Saturday" 
                                    ToolTip="Saturday" Value="Sa" />
                            </Items>
                        </telerik:RadComboBox>
                    </div>
		        <br />
		        </label>			
<%--                <label>
			        <p class="sub_title" style="margin: 0 auto; width: 50%">Optional time of day:</p>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 25%;" class="txt ">
                        <telerik:RadComboBox ID="ddlTimeFrom" runat="server" EmptyMessage="Time From" EnableLoadOnDemand="True" 
                                OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" 
                                OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                            <Items>
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="00:00" ToolTip="12 AM" Value="24:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="01:00" ToolTip="1 AM" Value="01:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="02:00" ToolTip="2 AM" Value="02:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="03:00" ToolTip="3 AM" Value="03:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="04:00" ToolTip="4 AM" Value="04:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="05:00" ToolTip="5 AM" Value="05:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="06:00" ToolTip="6 AM" Value="06:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="07:00" ToolTip="7 AM" Value="07:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="08:00" ToolTip="8 AM" Value="08:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="09:00" ToolTip="9 AM" Value="09:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="10:00" ToolTip="10 AM" Value="10:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="11:00" ToolTip="11 AM" Value="11:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="12:00" ToolTip="12 PM" Value="12:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="13:00" ToolTip="13 PM" Value="13:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="14:00" ToolTip="14 PM" Value="14:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="15:00" ToolTip="15 PM" Value="15:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="16:00" ToolTip="16 PM" Value="16:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="17:00" ToolTip="17 PM" Value="17:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="18:00" ToolTip="18 PM" Value="18:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="19:00" ToolTip="19 PM" Value="19:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="20:00" ToolTip="20 PM" Value="20:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="21:00" ToolTip="21 PM" Value="21:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="22:00" ToolTip="22 PM" Value="22:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeFrom" Text="23:00" ToolTip="23 PM" Value="23:00" />
                            </Items>
                        </telerik:RadComboBox>
                    </div>
		            <br />
		        </label>			
                <label>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 25%;" class="txt ">
                        <telerik:RadComboBox ID="ddlTimeTo" runat="server" EmptyMessage="Time To" EnableLoadOnDemand="True" 
                                OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" 
                                OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                            <Items>
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="00:00" ToolTip="12 AM" Value="24:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="01:00" ToolTip="1 AM" Value="01:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="02:00" ToolTip="2 AM" Value="02:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="03:00" ToolTip="3 AM" Value="03:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="04:00" ToolTip="4 AM" Value="04:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="05:00" ToolTip="5 AM" Value="05:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="06:00" ToolTip="6 AM" Value="06:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="07:00" ToolTip="7 AM" Value="07:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="08:00" ToolTip="8 AM" Value="08:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="09:00" ToolTip="9 AM" Value="09:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="10:00" ToolTip="10 AM" Value="10:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="11:00" ToolTip="11 AM" Value="11:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="12:00" ToolTip="12 PM" Value="12:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="13:00" ToolTip="13 PM" Value="13:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="14:00" ToolTip="14 PM" Value="14:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="15:00" ToolTip="15 PM" Value="15:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="16:00" ToolTip="16 PM" Value="16:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="17:00" ToolTip="17 PM" Value="17:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="18:00" ToolTip="18 PM" Value="18:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="19:00" ToolTip="19 PM" Value="19:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="20:00" ToolTip="20 PM" Value="20:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="21:00" ToolTip="21 PM" Value="21:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="22:00" ToolTip="22 PM" Value="22:00" />
                                <telerik:RadComboBoxItem Owner="ddlTimeTo" Text="23:00" ToolTip="23 PM" Value="23:00" />
                            </Items>
                        </telerik:RadComboBox>
                    </div>
                </label>--%>
                <label>
			        <p class="sub_title" style="margin: 0 auto; width: 50%">Optional time of day:</p>
                    <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 25%;" class="txt ">
                        <telerik:RadComboBox ID="ddlTimeofDay" runat="server" EmptyMessage="Time From" EnableLoadOnDemand="True" 
                                OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" 
                                OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                            <Items>
                                <telerik:RadComboBoxItem Owner="ddlTimeofDay" Text="Morning" ToolTip="Morning" Value="M" />
                                <telerik:RadComboBoxItem Owner="ddlTimeofDay" Text="Afternoon" ToolTip="Afternoon" Value="A" />
                                <telerik:RadComboBoxItem Owner="ddlTimeofDay" Text="Evening" ToolTip="Evening" Value="E" />
                            </Items>
                        </telerik:RadComboBox>
                    </div>
		            <br />
		        </label>			
                <label class="no">
                    <asp:Button ID="update_popup_ok" CssClass="button" Text="Update" runat="server" />
                    &nbsp;&nbsp;&nbsp;
                    <asp:Button ID="update_popup_delete" CssClass="button" Text="Delete" runat="server" />
                    &nbsp;&nbsp;&nbsp;
                    <asp:Button ID="update_popup_cancel" CssClass="button" Text="Cancel" runat="server" />
		        </label>
	        </div>
        </asp:Panel>
        </span>
        <span class="description find">  
            <label> Region FROM </label> 
            <asp:TextBox ID="find_region_txt" runat="server" CssClass="txt" />
            <label> And / or FLEET </label>
            <asp:TextBox ID="find_airport_txt" runat="server" CssClass="txt" />
            <asp:Button ID="find_button" runat="server" CssClass="button" Text="Search"/>
            <asp:HiddenField ID="hddnID" runat="server"></asp:HiddenField>
        </span>
	<div class="table">
<asp:SqlDataSource ID="get_defineregions" runat="server"
             ConnectionString="<%$ ConnectionStrings:PortalDB %>" 
             SelectCommand="SELECT Id, z1.ZoneName as ZoneIDFrom, z2.ZoneName as ZoneIDTo, StartDate, EndDate, name, Rate, DiscountPCT, PremiumPCT, [DayofWeek], case when TimeofDay = 'M' then 'Morning' when TimeofDay = 'A' then 'Afternoon' when TimeofDay = 'E' then 'Evening' else '' end as TimeofDay FROM PricingZonesRates r join PricingZones z1 on r.ZoneIDFrom = z1.ZoneID join PricingZones z2 on r.ZoneIDTo = z2.ZoneID join AircraftTypeServiceSpecs ss on r.CarrierID = ss.CarrierID and r.ACTypeID = ss.AircraftTypeServiceSpecID where r.carrierid = @carrierid order by z1.Zonename, z2.Zonename, StartDate, name" UpdateCommand="update PricingZones set ZoneName = @ZoneName, ZoneDescription = @ZoneDescription, BaseAirport = @BaseAirport, RadiusNM = @RadiusNM where carrierid = @carrierid and zoneID = @ZoneID" >

                <SelectParameters>
                    <asp:SessionParameter Name="carrierid" SessionField="carrierid" Type="Int32" />
                </SelectParameters>

                <UpdateParameters>
                            <asp:Parameter Name="ZoneID" Type="Int32" />
                            <asp:Parameter Name="ZoneName" Type="String" />
                            <asp:Parameter Name="ZoneDescription" Type="String" />
                            <asp:Parameter Name="BaseAirport" Type="String" />
                            <asp:Parameter Name="RadiusNM" Type="Int32" />
                            <asp:SessionParameter Name="CarrierID" SessionField="carrierid" Type="Int32" />
               </UpdateParameters>

</asp:SqlDataSource>

<asp:GridView ID="table_main_gridview" runat="server" HeaderStyle-CssClass="table__h" BorderWidth="0"
            DataSourceID="get_defineregions" AutoGenerateColumns="False" DataKeyNames="Id"  CssClass="table__tr" AutoGenerateEditButton="True">
            <Columns >
                <%--<asp:BoundField DataField="ZoneIDFrom"  HeaderText="From"  />--%>
                <%--<asp:BoundField DataField="ZoneIDTo"   HeaderText="To" />--%>
                <%--<asp:BoundField DataField="StartDate"   HeaderText="Eff" />--%>
                <%--<asp:BoundField DataField="EndDate"  HeaderText="Disc" />--%>
                <%--<asp:BoundField DataField="name"   HeaderText="FLEET" />--%>
                <%--<asp:BoundField DataField="Rate"   HeaderText="Fixed Price" />--%>
                <%--<asp:BoundField DataField="DiscountPCT"   HeaderText="OR-DISC %" />--%>
                <%--<asp:BoundField DataField="PremiumPCT"   HeaderText="OR-PREM %" />--%>
                <%--<asp:BoundField DataField="DayofWeek"   HeaderText="DAY OF WEEK" />--%>
                <asp:TemplateField ShowHeader="true" HeaderText="ID" Visible="False">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "ID")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="From">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "ZoneIDFrom")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="To">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "ZoneIDTo")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Eff">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "StartDate", "{0:d}")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Disc">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "EndDate", "{0:d}")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Fleet">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "name")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Fixed Price">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "Rate")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Or Discount %">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "DiscountPCT")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Or Premium %">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "PremiumPCT")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Day of Week">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "DayofWeek")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Time of Day">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "TimeofDay")%>
                    </ItemTemplate>
                </asp:TemplateField> 

                <%--<asp:HyperLinkField HeaderText="" Text="EDIT" DataNavigateUrlFormatString="#edit" DataNavigateUrlFields="Id" ControlStyle-CssClass="openpopup" />--%>
                <%--<asp:HyperLinkField HeaderText="" Text="DELETE" DataNavigateUrlFormatString="" DataNavigateUrlFields="Id" />--%>
            </Columns>
</asp:GridView>		
		</div>
	</div>
</section>
</main>
	
<footer class="normal">
	<div class="wrapper">
		<span class="copyring">
			<p>© 2016 CoastalAV LTD</p>
			<p>All Rights Reserved</p>
		</span>
		
		<span class="social">
			<ul>
				<li><a href="#"><i class="ico ico1"></i></a></li>
				<li><a href="#"><i class="ico ico2"></i></a></li>
				<li><a href="#"><i class="ico ico3"></i></a></li>
				<li><a href="#"><i class="ico ico4"></i></a></li>
			</ul>
		</span>
	</div>
</footer>


<div class="maska"></div>
<div class="popup" id="addnew">
	<span class="popup--title">add new special pricing</span>
	<div class="form">
		<label>
			<p class="sub_title">from:</p>
            <%--<asp:TextBox CssClass="txt" ID="popup_from_txt" runat="server" placeholder="from"/>--%>
            <telerik:RadComboBox ID="ddlFromAdd" runat="server" EmptyMessage="Region From" EnableLoadOnDemand="True" 
                    OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                    OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
            </telerik:RadComboBox>
		</label>
		<label>
			<p class="sub_title">to:</p>
            <%--<asp:TextBox CssClass="txt" ID="popup_to_txt" runat="server" placeholder="to"/>--%>
            <telerik:RadComboBox ID="ddlToAdd" runat="server" EmptyMessage="Region To" EnableLoadOnDemand="True" 
                    OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                    OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
            </telerik:RadComboBox>
		</label>
		<label>
			<p class="sub_title">eff:</p>
            <%--<asp:TextBox CssClass="txt" ID="popup_eff_txt" runat="server" placeholder="eff"/>--%>
            <telerik:RadDatePicker ID="start_date_add" runat="server" Culture="en-US"></telerik:RadDatePicker>
		</label>
		<label>
			<p class="sub_title">disc:</p>
            <%--<asp:TextBox CssClass="txt" ID="popup_disc_txt" runat="server" placeholder="disc"/>--%>
            <telerik:RadDatePicker ID="end_date_add" runat="server" Culture="en-US"></telerik:RadDatePicker>
		</label>
		<label>
			<p class="sub_title">fleet:</p>
            <%--<asp:TextBox CssClass="txt" ID="popup_fleet_txt" runat="server" placeholder="fleet:"/>--%>
            <telerik:RadComboBox ID="RadComboBoxfleet_add" runat="server" EmptyMessage="Fleet Type" EnableLoadOnDemand="True" 
                    OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                    OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                <Items>
                </Items>
            </telerik:RadComboBox>
		</label>
        <label>
			<p class="sub_title">fixed price:</p>
            <asp:TextBox CssClass="txt" ID="popup_fixedprice_txt" runat="server" placeholder="fixed price"/>
		</label>
        <label>
			<p class="sub_title">or-disc %:</p>
            <asp:TextBox CssClass="txt" ID="popup_ordisc_txt" runat="server" placeholder="or-disc %"/>
		</label>
        <label>
			<p class="sub_title">or-prem %:</p>
            <asp:TextBox CssClass="txt" ID="popup_orprem_txt" runat="server" placeholder="or-prem %"/>
		</label>
        <label>
			<p class="sub_title">day of week:</p>
            <%--<asp:TextBox CssClass="txt" ID="popup_dayofweek_txt" runat="server" placeholder="day of week"/>--%>
            <telerik:RadComboBox ID="ddlDayofWeekAdd" runat="server" EmptyMessage="Day of Week" EnableLoadOnDemand="True" 
                    OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                    OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" >
                <Items>
                    <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Sunday" 
                        ToolTip="Sunday" Value="Su" />
                    <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Monday" 
                        ToolTip="Monday" Value="M" />
                    <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Tuesday" 
                        ToolTip="Tuesday" Value="Tu" />
                    <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Wednesday" 
                        ToolTip="Wednesday" Value="W" />
                    <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Thursday" 
                        ToolTip="Thursday" Value="Th" />
                    <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Friday" 
                        ToolTip="Friday" Value="F" />
                    <telerik:RadComboBoxItem Owner="ddlDayofWeek" Text="Saturday" 
                        ToolTip="Saturday" Value="Sa" />
                </Items>
            </telerik:RadComboBox>
		</label>
        <%--<label>
			<p class="sub_title" style="margin: 0 auto; width: 50%">Optional time of day:</p>
            <telerik:RadComboBox ID="ddlTimeFromAdd" runat="server" EmptyMessage="Time From" EnableLoadOnDemand="True" 
                    OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%"
                    OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                <Items>
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="00:00" ToolTip="12 AM" Value="24:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="01:00" ToolTip="1 AM" Value="01:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="02:00" ToolTip="2 AM" Value="02:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="03:00" ToolTip="3 AM" Value="03:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="04:00" ToolTip="4 AM" Value="04:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="05:00" ToolTip="5 AM" Value="05:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="06:00" ToolTip="6 AM" Value="06:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="07:00" ToolTip="7 AM" Value="07:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="08:00" ToolTip="8 AM" Value="08:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="09:00" ToolTip="9 AM" Value="09:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="10:00" ToolTip="10 AM" Value="10:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="11:00" ToolTip="11 AM" Value="11:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="12:00" ToolTip="12 PM" Value="12:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="13:00" ToolTip="13 PM" Value="13:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="14:00" ToolTip="14 PM" Value="14:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="15:00" ToolTip="15 PM" Value="15:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="16:00" ToolTip="16 PM" Value="16:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="17:00" ToolTip="17 PM" Value="17:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="18:00" ToolTip="18 PM" Value="18:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="19:00" ToolTip="19 PM" Value="19:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="20:00" ToolTip="20 PM" Value="20:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="21:00" ToolTip="21 PM" Value="21:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="22:00" ToolTip="22 PM" Value="22:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeFromAdd" Text="23:00" ToolTip="23 PM" Value="23:00" />
                </Items>
            </telerik:RadComboBox>
		    <br />
		</label>			
        <label>
            <telerik:RadComboBox ID="ddlTimeToAdd" runat="server" EmptyMessage="Time To" EnableLoadOnDemand="True" 
                    OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%"
                    OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                <Items>
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="00:00" ToolTip="12 AM" Value="24:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="01:00" ToolTip="1 AM" Value="01:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="02:00" ToolTip="2 AM" Value="02:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="03:00" ToolTip="3 AM" Value="03:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="04:00" ToolTip="4 AM" Value="04:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="05:00" ToolTip="5 AM" Value="05:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="06:00" ToolTip="6 AM" Value="06:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="07:00" ToolTip="7 AM" Value="07:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="08:00" ToolTip="8 AM" Value="08:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="09:00" ToolTip="9 AM" Value="09:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="10:00" ToolTip="10 AM" Value="10:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="11:00" ToolTip="11 AM" Value="11:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="12:00" ToolTip="12 PM" Value="12:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="13:00" ToolTip="13 PM" Value="13:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="14:00" ToolTip="14 PM" Value="14:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="15:00" ToolTip="15 PM" Value="15:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="16:00" ToolTip="16 PM" Value="16:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="17:00" ToolTip="17 PM" Value="17:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="18:00" ToolTip="18 PM" Value="18:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="19:00" ToolTip="19 PM" Value="19:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="20:00" ToolTip="20 PM" Value="20:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="21:00" ToolTip="21 PM" Value="21:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="22:00" ToolTip="22 PM" Value="22:00" />
                    <telerik:RadComboBoxItem Owner="ddlTimeToAdd" Text="23:00" ToolTip="23 PM" Value="23:00" />
                </Items>
            </telerik:RadComboBox>
        </label>--%>
        <label>
			<p class="sub_title" style="margin: 0 auto; width: 50%">Optional time of day:</p>
            <div style="margin: 0 auto; border-bottom-style: solid; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 1%; width: 25%;" class="txt ">
                <telerik:RadComboBox ID="ddlTimeofDayAdd" runat="server" EmptyMessage="Time From" EnableLoadOnDemand="True" 
                        OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                        OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" 
                        OpenDropDownOnFocus="False" OpenDropDownOnLoad="False" ShowToggleImage="False" CssClass="txt" >
                    <Items>
                        <telerik:RadComboBoxItem Owner="ddlTimeofDayAdd" Text="Morning" ToolTip="Morning" Value="M" />
                        <telerik:RadComboBoxItem Owner="ddlTimeofDayAdd" Text="Afternoon" ToolTip="Afternoon" Value="A" />
                        <telerik:RadComboBoxItem Owner="ddlTimeofDayAdd" Text="Evening" ToolTip="Evening" Value="E" />
                    </Items>
                </telerik:RadComboBox>
            </div>
		    <br />
		</label>			
		<label class="no">
            <asp:Button ID="popup_add" CssClass="button" Text="Add" runat="server" />
            <asp:Button ID="popup_cancel" CssClass="button" Text="Cancel" runat="server" />
		</label>
</div>
    </div>
<%--<div class="popup " id="edit">
	<span class="popup--title">Edit</span>
	<div class="form">
		<label>
			<p class="sub_title">from:</p>
            <asp:TextBox id="edit_from_1" runat="server" CssClass="txt " placeholder="Region name"/>
		</label>
		<label>
			<p class="sub_title">to:</p>
            <asp:TextBox id="edit_to_1" runat="server" CssClass="txt " placeholder="Region name"/>
		</label>
		<label>
			<p class="sub_title">eff:</p>
            <asp:TextBox id="edit_eff_1" runat="server" CssClass="txt " placeholder="eff"/>
		</label>
		<label>
			<p class="sub_title">disc:</p>
            <asp:TextBox id="edit_disc_1" runat="server" CssClass="txt " placeholder="disc"/>
		</label>
		<label>
			<p class="sub_title">fleet:</p>
            <asp:TextBox id="edit_fleet_1" runat="server" CssClass="txt " placeholder="fleet"/>
		</label>
		<label>
			<p class="sub_title">fixed price:</p>
            <asp:TextBox id="edit_price_1" runat="server" CssClass="txt " placeholder="fixed price"/>
		</label>
        <label>
			<p class="sub_title">or-disc %:</p>
            <asp:TextBox id="edit_or_sidc_1" runat="server" CssClass="txt" placeholder="or-disc %"/>
		</label>
        <label>
			<p class="sub_title">or-prem %:</p>
            <asp:TextBox id="edit_or_prem_1" runat="server" CssClass="txt" placeholder="or-prem %"/>
		</label>
        <label>
			<p class="sub_title">day of week:</p>
            <asp:TextBox id="edit_dayweek_1" runat="server" CssClass="txt" placeholder="day of week"/>
		</label>			
        <label class="no">
            <asp:Button ID="update_popup_ok" CssClass="button" Text="Update" runat="server" />
            <asp:Button ID="update_popup_cancel" CssClass="button" Text="Cancel" runat="server" />
		</label>
	</div>
</div>--%>

<script type="text/javascript" src="js\jQuery_v1.11.js"></script> 
<script type="text/javascript" src="js\jquery.datetimepicker.min.js"></script> 
<script type="text/javascript" src="js\jquery.maskedinput.min.js"></script> 
<script type="text/javascript" src="js\script.js"></script> 
<script type="text/javascript" src="js\core.js"></script> 
    </form>
</body>
</html>
