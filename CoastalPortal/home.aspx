<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="home.aspx.vb" Inherits="CoastalPortal.home2" EnableViewStateMac="false" EnableSessionState="true" EnableEventValidation="false" ValidateRequest="false" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="shortcut icon" href="Images/cat.ico" />
    <link rel="stylesheet" href="style.css" type="text/css" media="screen">
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
	<!--[if lt IE 9]><script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script><![endif]-->
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title> Home</title>
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
<body>
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
    <div>
<section class="content" id="content" runat="server">
	<header class="menu">
		<div class="wrapper">
			<div class="menu__left">
				<ul>
					<li><a href="EditFlight.aspx">Retrieve/Edit Prior Quote</a></li>
					<%--<li><a href="#">System Administration</a></li>--%>
					<li><a href="#">Dashboard</a></li>
					<li><a href="AircraftTypeServiceSpecs.aspx">Fleet Setup</a></li>
					<li><a href="http://www.wuoptimizer.com/login.aspx">Optimizer</a></li>
				</ul>
			</div>
			<div class="logo">
				<a href="home.aspx">
					<img src="~/Images/logo_blue.png" alt="" id="imglogo" runat="server" width="112" />
				</a>
			</div>
			<div class="menu__right">
				<ul>
					<li><a href="CustomerMaintance.aspx">CRM</a></li>
					<li><a href="Regions.aspx">Define Regions</a></li>
					<%--<li><a href="CrewTravel.aspx">Crew Travel</a></li>--%>
					<li><a href="#">Log Off</a></li>
					<li><a href="#">pricing</a></li>
					<li><a href="RegionPricing.aspx">special pricing</a></li>
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
					<li><a href="EditFlight.aspx">Retrieve/Edit Prior Quote</a></li>
					<%--<li><a href="#">System Administration</a></li>--%>
					<li><a href="AircraftTypeServiceSpecs.aspx">Fleet Setup</a></li>
					<li><a href="Regions.aspx">Define Regions</a></li>
					<li><a href="CustomerMaintance.aspx">CRM</a></li>
					<li><a href="#">New User</a></li>
				</ul>
			</div>	
		</div>	
	</header>
	
	
	<div class="form__order">
		<div class="form">
			<div id="orderform">
				<span class="order_boxing first">
					<div class="box__checkboxes">
						<%--<label class="box__checkboxes--check">
							 <asp:RadioButton Text="one way" id="checkbox1" CssClass="checkbox" runat="server" GroupName="fl_type" AutoPostBack="true" />
							<label class="labeltxt" for="checkbox1">one way</label>
						</label>--%>
						<%--<label class="box__checkboxes--check">
							 <asp:RadioButton text="round trip" id="checkbox2" CssClass="checkbox" runat="server" GroupName="fl_type" autopostback="true" />
							<label class="labeltxt" for="checkbox2">ROUND TRIP</label>
						</label>--%>
						<%--<label class="box__checkboxes--check">
                            <asp:RadioButton Text="multi leg" id="checkbox3" CssClass="checkbox" runat="server" GroupName="fl_type" AutoPostBack="true" />
							<label class="labeltxt" for="checkbox3">multi Leg</label>
						</label>--%>
                        <div>
                            <asp:RadioButtonList ID="rblOneWayRoundTrip" runat="server" RepeatDirection="Horizontal" AutoPostBack="True">
                                <asp:ListItem Value="OneWay">One Way</asp:ListItem>
                                <asp:ListItem Value="RoundTrip">Round Trip</asp:ListItem>
                                <asp:ListItem Value="MultiLeg">Multi-Leg</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
					</div>
					
					<div class="boxes__select">
						<span class="order_box col-3">
							<label>
								<p class="sub_title ico8">Select Company</p>
                                <asp:DropDownList ID="ddllBrokerCompanies" runat="server" AutoPostBack="True" >
                                    </asp:DropDownList>
							</label>	
							
						</span>
						<span class="order_box col-3">
							<label>
								<p class="sub_title ico2">Select contact</p>
                              <asp:DropDownList ID="ddllBrokers" runat="server" >
                                  </asp:DropDownList>
							</label>	
							
						</span>
						<span class="order_box col-3">
							<label>
								<p class="sub_title ico8">Aircraft Types </p>
                                <%--<asp:DropDownList ID="ddlAircraftServiceTypes" runat="server">
                                    <asp:ListItem Text="Aircraft types"  Value="Aircraft types"></asp:ListItem>
                                    <asp:ListItem Text="CAS"  Value="CAS"></asp:ListItem>
                                </asp:DropDownList>--%>
                                <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                    <telerik:RadComboBox ID="RadComboBoxACInclude" runat="server" CheckBoxes="True" 
                                        EmptyMessage="Optional Aircraft types to INCLUDE" 
                                        EnableCheckAllItemsCheckBox="True" Width="100%">
                                        <Items>
                                            <%-- suppress all but light, mid and heavy per David 1/15/2016 --%>
                                            <%--<telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="Single Piston" 
                                                ToolTip="Piston Propeller" Value="P" />--%>
                                            <%--<telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="Twin Piston" 
                                                ToolTip="Twin Piston Propeller" Value="T" />--%>
                                            <%--<telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="Single Turboprop" 
                                                ToolTip="TurboProp (PC12, TBM, Caravan)" Value="1" />--%>
                                            <telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="Twin Turboprop" 
                                                ToolTip="TurboProp (King Air)" Value="2" />
                                            <%--<telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="Very Light Jet" 
                                                ToolTip="Very Light Jet (Phenom 100, Mustang, Eclipse)" Value="V" />--%>
                                            <telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="Light Jet" 
                                                ToolTip="Light Jet (Hawker 400)" Value="L" />
                                            <telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="Mid Jet" 
                                                ToolTip="Midsize Jet (Hawker 800, Hawker 800XP)" Value="M" />
                                            <telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="SuperMid Jet" 
                                                ToolTip="Super Midsize Jet (Citation X, Challenger)" Value="U" />
                                            <telerik:RadComboBoxItem Owner="RadComboBoxACInclude" Text="Heavy Jet" 
                                                ToolTip="Heavy Jet (Challenger 604)" Value="H" />
                                        </Items>
                                    </telerik:RadComboBox>
                                </div>
							</label>	
							
						</span>
					</div>
				
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" 
                                    Width="100%">
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%>
                            <%--<asp:Calendar ID="departdate" runat="server" CssClass="txt" BackColor="White" BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399" Height="200px" Width="220px">
                                <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                                <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                                <OtherMonthDayStyle ForeColor="#999999" />
                                <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                                <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                                <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                                <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                                <WeekendDayStyle BackColor="#CCCCFF" />
                            </asp:Calendar>--%>
                            <div style="border-bottom-style: solid; padding-top: 8%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">+  Add Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="+  Add Leg" runat="server" ID="bttnAddLeg" Width="100%" />
					</span>
				</span>

                <asp:Panel ID="pnlLeg2" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress2" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress2" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress2" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress2" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers2" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt2" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date2" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo2" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg2" Width="100%" />
					</span>
				</span>
                </asp:Panel>
                <asp:Panel ID="pnlLeg3" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress3" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress3" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress3" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress3" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers3" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt3" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date3" runat="server" Culture="en-US"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo3" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg3" Width="100%" />
					</span>
				</span>
                </asp:Panel>
                <asp:Panel ID="pnlLeg4" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress4" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress4" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress4" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress4" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers4" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt4" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date4" runat="server" Culture="en-US"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo4" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg4" Width="100%" />
					</span>
				</span>
                </asp:Panel>
                <asp:Panel ID="pnlLeg5" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress5" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress5" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress5" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress5" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers5" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt5" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date5" runat="server" Culture="en-US"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo5" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg5" Width="100%" />
					</span>
				</span>
                </asp:Panel>
                <asp:Panel ID="pnlLeg6" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress6" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress6" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress6" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress6" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers6" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt6" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date6" runat="server" Culture="en-US"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo6" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg6" Width="100%" />
					</span>
				</span>
                </asp:Panel>
                <asp:Panel ID="pnlLeg7" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress7" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress7" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress7" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress7" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers7" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt7" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date7" runat="server" Culture="en-US"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo7" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg7" Width="100%" />
					</span>
				</span>
                </asp:Panel>
                <asp:Panel ID="pnlLeg8" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress8" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress8" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress8" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress8" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers8" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt8" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date8" runat="server" Culture="en-US"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo8" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg8" Width="100%" />
					</span>
				</span>
                </asp:Panel>
                <asp:Panel ID="pnlLeg9" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress9" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress9" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress9" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress9" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers9" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt9" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date9" runat="server" Culture="en-US"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo9" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg9" Width="100%" />
					</span>
				</span>
                </asp:Panel>
                <asp:Panel ID="pnlLeg10" runat="server" Visible="False">
                <span class="order_boxing first">
					<span class="order_box">
						<label>
							<p class="sub_title ico1">
								Departure Point:
								<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="OriginAddress10" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="OriginAddress10" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box">
						<label class="ico__fly">
							<p class="sub_title ico1">Destination:
							<i class="helps"></i>
								<span class="formshover">
									Enter an Airport Code
									[KXXX], City, Street Address,
									or Landmark Name
								</span>
							</p>
                            <%--<asp:TextBox ID="DestinationAddress10" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadComboBox ID="DestinationAddress10" runat="server" EmptyMessage="Airport Code [KXXX], City" EnableLoadOnDemand="True" 
                                    Font-Size="10pt" Height="225px" OnClientDropDownOpening="OnClientDropDownOpening1" OnClientItemsRequested="ItemsLoaded" 
                                    OnClientItemsRequesting="OnClientItemsRequesting" OnClientSelectedIndexChanged="ProdSearch" Width="100%" 
                                    OnItemsRequested="Address_ItemsRequested" OpenDropDownOnFocus="False" OpenDropDownOnLoad="true" ShowToggleImage="False" >
                                </telerik:RadComboBox>
                            </div>
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico2">Passengers:</p>
							<span class="plusminus">
								<i class="plus">+</i>
								<i class="minus">-</i>
							</span>
                            <asp:TextBox ID="ddlPassengers10" CssClass="txt center" runat="server" value="1" placeholder="Passengers" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="depart_txt10" type="date" CssClass="txt" runat="server" placeholder="Depart Date" />--%> 
                            <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                                <telerik:RadDatePicker ID="depart_date10" runat="server" Culture="en-US"></telerik:RadDatePicker>
                            </div>
						</label>
					</span>
					<span class="order_box col-2">
						<label>
							<p class="sub_title ico4">Depart time:</p>
                              <asp:DropDownList ID="departtime_combo10" runat="server" DataTextField="TimeStr" DataValueField="TimeStr" >
                                  <asp:ListItem Text="9:45 AM"  Value="9:45 AM"></asp:ListItem>
                                  </asp:DropDownList>
						</label>	
						
					</span>
					<span class="order_box col-1 leggg">
						<%--<label>
							<p class="add" id="addLeg">-  Remove Leg</p>
						</label>--%>
                        <asp:Button CssClass="button__secont" text="-  Remove Leg" runat="server" ID="bttnRemoveLeg10" Width="100%" />
					</span>
				</span>
                </asp:Panel>
			</div>	
			<div class="button_boxing order_box">
                <asp:Button CssClass="button" text="Generate Quote" runat="server" ID="cmdQuote" />
                <asp:Button CssClass="button__secont" text="Start Over" runat="server" ID="cmdStartOver" />
                    			<br />
                <br />
                <asp:Label ID="lblMsg" runat="server" ForeColor="#CC0000" Text=""></asp:Label>
                    			</div>
		</div>
	</div>
</section>
	
<section class="article">
	<span class="article__title">About</span>
	<span class="article__txt">
		Wherever you are in the world, at any given moment, 
		Rapid response speed enables us to arrange charters within 90 minutes. 
	</span>
	
</section>
	
<footer class="normal nomargin">
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



<script type="text/javascript" src="js\jQuery_v1.11.js"></script> 
<script type="text/javascript" src="js\jquery.datetimepicker.min.js"></script> 
<script type="text/javascript" src="js\jquery.maskedinput.min.js"></script> 
<script type="text/javascript" src="js\script.js"></script> 
<script type="text/javascript" src="js\core.js"></script> 

    </div>
    </form>
</body>
</html>
