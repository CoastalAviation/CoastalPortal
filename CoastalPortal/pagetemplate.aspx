﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="pagetemplate.aspx.vb" Inherits="CoastalPortal.pagetemplate" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Model Run History</title>
    <link rel="shortcut icon" href="Images/cat.ico" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link href="style.css" rel="stylesheet" type="text/css" />
	<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1"/>
	<!--[if lt IE 9]><script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script><![endif]-->
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
					<li><a href="FlightSchedule.aspx?r0=0">Flight Schedule</a></li>
					<%--<li><a href="#">Log Off</a></li>--%>
                    <li><asp:LinkButton ID="LinkLogOut" runat="server">Log Off</asp:LinkButton></li>
					<%--<li><a href="Dashboard.aspx">Operations Dashboard</a></li>--%>
					<li><a href="HoldLineTrips.aspx">Review Hold Line Trips</a></li>
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
					<li><a href="HoldLineTrips.aspx">Review Hold Line Trips</a></li>
					<li><a href="FlightSchedule.aspx?r0=0">Flight Schedule</a></li>
					<%--<li><a href="#">Log Off</a></li>--%>
                    <li><asp:LinkButton ID="LinkLogOut2" runat="server">Log Off</asp:LinkButton></li>
					<%--<li><a href="Dashboard.aspx">Operations Dashboard</a></li>--%>
				</ul>
			</div>	
		</div>	
	</header>
	
	
	
</section>
	
<section class="article nopadding">

	
	<div class="form__order2"  id="form_1" runat="server" >
		<div class="title">Model Run History</div>

		<div class="title"> <asp:Label runat="server" ID="aircraft_type_txt_1" CssClass="title"></asp:Label> </div>
		<%--<div class="table">
			<div class="table__scroll">
			<div class="table_h">
				<span class="h">Origin</span>
				<span class="h">Departs</span>
				<span class="h">Destination</span>
				<span class="h">Arrives</span>
				<span class="h col-small">Flight Duration</span>
				<span class="h col-small">Price</span>
			</div>

			<div class="table__item">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="origin_one_1" Text="">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="origin_two_1" Text="BEVERLY MUNI (BVY)">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="departs_one_1" Text="">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="departs_two_1" Text="2/23/16 9:00 AM">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="destination_one_1" Text="">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="destination_two_1" Text="WESTCHESTER COUNTY (HPN)">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="arrives_one_1" Text="">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="arrives_two_1" Text="2/23/16 9:29 AM">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item col-small">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="flight_one_1" Text="">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="flight_two_1" Text="00:29">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item col-small">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="price_one_1" Text="$5200" >  </asp:Label></li>
					<li> <asp:Label runat="server" ID="price_two_1" Text="$5200" >  </asp:Label></li>
				</ul>
			</div>
		</div>	
		</div>--%>
    	<div class="table grid">
            <asp:GridView ID="gvServiceProviderMatrix" runat="server" HeaderStyle-CssClass="table__h" BorderWidth="0"
            AutoGenerateColumns="False" DataKeyNames="Origin,Departs,Destination,Arrives,Flight Duration,Price"  CssClass="table__tr">
            <Columns >
                <%--<asp:BoundField DataField="origin"  HeaderText="origin"  />--%>
                <%--<asp:BoundField DataField="departs"   HeaderText="departs" />--%>
                <%--<asp:BoundField DataField="destination"   HeaderText="destination" />--%>
                <%--<asp:BoundField DataField="arrives"  HeaderText="arrives" />--%>
                <%--<asp:BoundField DataField="flight_duration"   HeaderText="flight duration" />--%>
                <%--<asp:BoundField DataField="price"   HeaderText="price" />--%>
                <asp:TemplateField HeaderText="Select">
                    <ItemTemplate>
                        <asp:RadioButton ID="RadioButton1" runat="server" />
                        <asp:HiddenField ID="HiddenField1" runat="server" Value = '<%#Eval("ID")%>' />
                    </ItemTemplate>
                    <ControlStyle Width="5%" />
                    <ItemStyle Width="5%" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Aircraft Type" >
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "name")%>
                    </ItemTemplate>
                    <%--<ControlStyle Width="80px" />--%>
                    <%--<ItemStyle Width="80px" />--%>
                </asp:TemplateField>
                <asp:TemplateField  HeaderText="Origin">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "OriginFacilityName")%>
                    </ItemTemplate>
                    <%--<ControlStyle Width="200px" />--%>
                    <%--<ItemStyle Width="200px" />--%>
                </asp:TemplateField> 
                <asp:TemplateField HeaderText="Departs" >
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "Departs", "{0:G}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField  HeaderText="Destination">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "DestinationFacilityName")%>
                    </ItemTemplate>
                    <%--<ControlStyle Width="200px" />--%>
                    <%--<ItemStyle Width="200px" />--%>
                </asp:TemplateField> 
                <asp:TemplateField HeaderText="Arrives" >
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "Arrives", "{0:g}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Flight Duration" >
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "Flight Duration", "{0:g}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Fuel Stops">
                    <ItemTemplate> 
                        <%#DataBinder.Eval(Container.DataItem, "FuelStops")%> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Price<br />(w/o Tax)">
                    <ItemTemplate> <%#DataBinder.Eval(Container.DataItem, "Price", "{0:c}")%> </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <%--<ControlStyle Width="40px" />--%>
                    <%--<ItemStyle Width="40px" />--%>
                </asp:TemplateField>
            </Columns>
            </asp:GridView>		
		</div>
		<div class="form__buttons">
                <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label>
                <br />
                <asp:Label ID="lblFlightTimeMsg" runat="server" ForeColor="Red"></asp:Label>
			    <%--<br />--%>
                <%--<br />--%>
            <%--<asp:Button CssClass="button" Text="Review Qoute" runat="server" ID="CmdReview" />--%>
            <%--&nbsp;&nbsp;&nbsp;--%>
            <%--<div class="button_boxing order_box">
            </div>--%>
			<p class="price"> <asp:Label runat="server" ID="price_summary_1" Text="">  </asp:Label> </p>
		</div>
	
	</div>

    <div class="form__order">
        <div class="form">
            <div class="button_boxing order_box">
                <asp:Button ID="cmdEdit" CssClass="button" Text="Edit/Email Quote" runat="server" />
                <asp:Button CssClass="button__secont" text="Start Over" runat="server" ID="cmdStartOver1" />
            </div>
        </div>
    </div>
	
<%--<div class="form__order2" id="form_2" runat="server">
		<div class="title"> <asp:Label runat="server" ID="aircraft_type_txt_2" CssClass="title" Text="Cirrus SR22"> </asp:Label> </div>
		<div class="table">
			<div class="table__scroll">
			<div class="table_h">
				<span class="h">Origin</span>
				<span class="h">Departs</span>
				<span class="h">Destination</span>
				<span class="h">Arrives</span>
				<span class="h col-small">Flight Duration</span>
				<span class="h col-small">Price</span>
			</div>

			<div class="table__item">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="origin_one_2" Text="WESTCHESTER COUNTY (HPN)">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="origin_two_2" Text="BEVERLY MUNI (BVY)">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="departs_one_2" Text="2/23/16 9:00 AM">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="departs_two_2" Text="2/23/16 9:00 AM">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="destination_one_2" Text="BEVERLY MUNI (BVY)">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="destination_two_2" Text="WESTCHESTER COUNTY (HPN)">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="arrives_one_2" Text="2/23/16 9:29 AM">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="arrives_two_2" Text="2/23/16 9:29 AM">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item col-small">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="flight_one_2" Text="00:29">  </asp:Label></li>
					<li> <asp:Label runat="server" ID="flight_two_2" Text="00:29">  </asp:Label></li>
				</ul>
			</div>
			<div class="table__item col-small">
				<ul class="table__list">
					<li> <asp:Label runat="server" ID="price_one_2" Text="$5200" >  </asp:Label></li>
					<li> <asp:Label runat="server" ID="price_two_2" Text="$5200" >  </asp:Label></li>
				</ul>
			</div>
		</div>	
		</div>
		<div class="form__buttons">
            <asp:Button ID="cmdQuote" CssClass="button" Text="Generate Qoute" runat="server" />
            <asp:Button ID="cmdStartOver" CssClass="button no__color" Text="New Quote" runat="server" />
			<p class="price"> <asp:Label runat="server" ID="price_summary_2" Text="$5700">  </asp:Label> </p>
		</div>
	</div>--%>
	
	<div class="form__order">
        <span class="title">EDIT Itinerary</span>
		<div class="form">
			<div id="orderform">
				<span class="order_boxing first">
					<div class="box__checkboxes">
						<%--<label class="box__checkboxes--check">
							 <asp:RadioButton Text="one way" id="checkbox1" CssClass="checkbox" runat="server" GroupName="fl_type" AutoPostBack="true" />
							<label class="labeltxt" for="checkbox1">one way</label>
						</label>
						<label class="box__checkboxes--check">
							 <asp:RadioButton text="round trip" id="checkbox2" CssClass="checkbox" runat="server" GroupName="fl_type" autopostback="true" />
							<label class="labeltxt" for="checkbox2">ROUND TRIP</label>
						</label>
						<label class="box__checkboxes--check">
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
                            <asp:TextBox ID="ddlPassengers" CssClass="txt center" runat="server" value="1" placeholder="Airport Code, City" /> 
						</label>
					</span>
					<span class="order_box col-1">
						<label>
							<p class="sub_title ico3">Depart Date:</p>
                            <%--<asp:TextBox ID="calLeave" type="date" CssClass="txt" runat="server" placeholder="Airport Code, City" />--%> 
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
						<label>
							<%--<p class="add" id="addLeg">+  Add Leg</p>--%>
                            <asp:Button CssClass="button__secont" text="+  Add Leg" runat="server" ID="bttnAddLeg" Width="100%" />
						</label>
					</span>
				</span>
			</div>	
			<div class="button_boxing order_box">
                <asp:Label ID="lblMsg1" runat="server" ForeColor="Red"></asp:Label>
                <br />
                <br />
                <asp:Button CssClass="button" text="Generate Quote" runat="server" ID="cmdQuote" />
                <asp:Button CssClass="button__secont" text="Start Over" runat="server" ID="cmdStartOver" />
            </div>
		</div>
	</div>
		
</section>
	
<footer class="normal">
	<div class="wrapper">
		<span class="copyring">
			<p>© 2017 Coastal Aviation Software, Inc</p>
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


<script type="text/javascript" src="js\jQuery_v1.11.js"></script> 
<script type="text/javascript" src="js\jquery.datetimepicker.min.js"></script> 
<script type="text/javascript" src="js\jquery.maskedinput.min.js"></script> 
<script type="text/javascript" src="js\script.js"></script> 
<script type="text/javascript" src="js\core.js"></script> 


    </form>
</body>
</html>
