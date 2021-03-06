﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="QuoteTrip.aspx.vb" Inherits="CoastalPortal.QuoteTrip" %>

<%--<%@ Register Src="Controls/Header.ascx" TagName="Header" TagPrefix="uc1" %>--%>
<%--<%@ Register src="Controls/Calendar.ascx" tagname="Calendar" tagprefix="uc2" %>--%>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Get Quotes</title>
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
			
			<div class="header__title small__padding">
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

	<asp:Panel ID="pnlQuote" runat="server" Visible="False">

	    <div class="form__order2"  id="form_1" runat="server" >
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
	
	</asp:Panel>

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
                                <asp:ListItem Value="OneWay" Selected="True">One Way</asp:ListItem>
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
                                <telerik:RadDatePicker ID="depart_date3" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
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
                                <telerik:RadDatePicker ID="depart_date4" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
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
                                <telerik:RadDatePicker ID="depart_date5" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
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
                                <telerik:RadDatePicker ID="depart_date6" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
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
                                <telerik:RadDatePicker ID="depart_date7" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
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
                                <telerik:RadDatePicker ID="depart_date8" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
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
                                <telerik:RadDatePicker ID="depart_date9" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
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
                                <telerik:RadDatePicker ID="depart_date10" runat="server" Culture="en-US" Width="100%"></telerik:RadDatePicker>
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
                <asp:Label ID="lblMsg1" runat="server" ForeColor="Red"></asp:Label>
                <br />
                <br />
                <asp:Button CssClass="button" text="Generate Quote" runat="server" ID="cmdQuote" />
                <asp:Button CssClass="button__secont" text="Start Over" runat="server" ID="cmdStartOver" />
            </div>
		</div>
	</div>
	
	<%--<div class="forms_maps">
		<div class="forms_order">
			<span class="title">Selected Itinerary</span>

			<div class="left__maps full__width">	
				<div class="map" id="map_canvas" runat="server"></div>
				<span class="bottom">
					<span class="bottom__wrap">
						<p class="sub_title">Origin:</p>
                        <asp:Label runat="server" CssClass="title" ID="map_origin_label" Text="">  </asp:Label>
					</span>
					<span class="bottom__wrap">
						<p class="sub_title">Destination:</p>
                        <asp:Label runat="server" CssClass="title" ID="map_destination_label" Text="">  </asp:Label>
					</span>
				</span>
			
			</div>
			
			<script src="https://maps.googleapis.com/maps/api/js?v=3.exp"></script>
			<script src="https://google-maps-utility-library-v3.googlecode.com/svn/tags/markerwithlabel/1.1.9/src/markerwithlabel_packed.js"></script>
			<script>
                var styleArray = [
                    {
                        "featureType": "water",
                        "elementType": "geometry",
                        "stylers": [
                            {
                                "color": "#e9e9e9"
                            },
                            {
                                "lightness": 17
                            }
                        ]
                    },
                    {
                        "featureType": "landscape",
                        "elementType": "geometry",
                        "stylers": [
                            {
                                "color": "#f5f5f5"
                            },
                            {
                                "lightness": 20
                            }
                        ]
                    },
                    {
                        "featureType": "road.highway",
                        "elementType": "geometry.fill",
                        "stylers": [
                            {
                                "color": "#ffffff"
                            },
                            {
                                "lightness": 17
                            }
                        ]
                    },
                    {
                        "featureType": "road.highway",
                        "elementType": "geometry.stroke",
                        "stylers": [
                            {
                                "color": "#ffffff"
                            },
                            {
                                "lightness": 29
                            },
                            {
                                "weight": 0.2
                            }
                        ]
                    },
                    {
                        "featureType": "road.arterial",
                        "elementType": "geometry",
                        "stylers": [
                            {
                                "color": "#ffffff"
                            },
                            {
                                "lightness": 18
                            }
                        ]
                    },
                    {
                        "featureType": "road.local",
                        "elementType": "geometry",
                        "stylers": [
                            {
                                "color": "#ffffff"
                            },
                            {
                                "lightness": 16
                            }
                        ]
                    },
                    {
                        "featureType": "poi",
                        "elementType": "geometry",
                        "stylers": [
                            {
                                "color": "#f5f5f5"
                            },
                            {
                                "lightness": 21
                            }
                        ]
                    },
                    {
                        "featureType": "poi.park",
                        "elementType": "geometry",
                        "stylers": [
                            {
                                "color": "#dedede"
                            },
                            {
                                "lightness": 21
                            }
                        ]
                    },
                    {
                        "elementType": "labels.text.stroke",
                        "stylers": [
                            {
                                "visibility": "on"
                            },
                            {
                                "color": "#ffffff"
                            },
                            {
                                "lightness": 16
                            }
                        ]
                    },
                    {
                        "elementType": "labels.text.fill",
                        "stylers": [
                            {
                                "saturation": 36
                            },
                            {
                                "color": "#333333"
                            },
                            {
                                "lightness": 40
                            }
                        ]
                    },
                    {
                        "elementType": "labels.icon",
                        "stylers": [
                            {
                                "visibility": "off"
                            }
                        ]
                    },
                    {
                        "featureType": "transit",
                        "elementType": "geometry",
                        "stylers": [
                            {
                                "color": "#f2f2f2"
                            },
                            {
                                "lightness": 19
                            }
                        ]
                    },
                    {
                        "featureType": "administrative",
                        "elementType": "geometry.fill",
                        "stylers": [
                            {
                                "color": "#fefefe"
                            },
                            {
                                "lightness": 20
                            }
                        ]
                    },
                    {
                        "featureType": "administrative",
                        "elementType": "geometry.stroke",
                        "stylers": [
                            {
                                "color": "#fefefe"
                            },
                            {
                                "lightness": 17
                            },
                            {
                                "weight": 1.2
                            }
                        ]
                    }
                ];


                var latLng = new google.maps.LatLng(49.47805, -123.84716);
                var map = new google.maps.Map(document.getElementById('map_canvas'), {
                    zoom: 12,
                    center: latLng,
                    styles: styleArray,
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    zoomControlOptions: {
                        style: google.maps.ZoomControlStyle.LARGE,
                        position: google.maps.ControlPosition.TOP_RIGHT
                    },
                });

                var homeLatLng1 = new google.maps.LatLng(49.47805, -123.84816);
                var markImg1 = new google.maps.MarkerImage('images/marker1.png', new google.maps.Size(52, 18));
                var marker1 = new MarkerWithLabel({
                    position: homeLatLng1,
                    map: map,
                    icon: markImg1,
                    labelContent: "jfk",
                    labelAnchor: new google.maps.Point(22, 0),
                    labelClass: "labels", // the CSS class for the label
                });

                var homeLatLng2 = new google.maps.LatLng(49.47405, -123.89016);
                var markImg2 = new google.maps.MarkerImage('images/marker2.png', new google.maps.Size(52, 18));
                var marker2 = new MarkerWithLabel({
                    position: homeLatLng2,
                    map: map,
                    icon: markImg2,
                    labelContent: "HPN",
                    labelAnchor: new google.maps.Point(22, 0),
                    labelClass: "labels", // the CSS class for the label
                });

                var homeLatLng3 = new google.maps.LatLng(49.47005, -123.84016);
                var markImg3 = new google.maps.MarkerImage('images/marker1.png', new google.maps.Size(52, 18));
                var marker3 = new MarkerWithLabel({
                    position: homeLatLng3,
                    map: map,
                    icon: markImg3,
                    labelContent: "HPN",
                    labelAnchor: new google.maps.Point(22, 0),
                    labelClass: "labels", // the CSS class for the label
                });
			</script>			
			
			
			
		
		
		</div>
	</div>--%>
	
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
