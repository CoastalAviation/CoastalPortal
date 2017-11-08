﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ModelDetails.aspx.vb" Inherits="CoastalPortal.ModelDetails" %>
<%@ Register src="Controls/OptimizerRunDetail.ascx" tagname="OptimizerDetailc12" tagprefix="uc6" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Model Run Details</title>
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
    <style type="text/css">
        .image_cell
        {
            padding-left:10px;
            padding-right: 15px;
            padding-top: 3px;
            padding-bottom: 3px;
        }
    
        .demo_item_title
        {
         margin-left: 5px;
         margin-top: 4px;
         font-family: Arial;
         font-size: 13px;
         font-weight: bold;
         color: #004b91;         
        }
        
        .demo_item
        {
            font-family: Verdana;
            font-size: 11px;
            color: #004b91;
        }
    
a:link, a:visited
{
}

        .HideArrow
        {}

        .style1
        {
            width: 142px;
        }


         div.RadGrid_WebBlue .rgRow a,
    div.RadGrid_WebBlue .rgAltRow a
    {
      color: BLUE;
    }
    div.RadGrid_WebBlue .rgRow a:hover,
    div.RadGrid_WebBlue .rgRow a:visited,
    div.RadGrid_WebBlue .rgAltRow a:hover,
    div.RadGrid_WebBlue .rgAltRow a:visited
    {
      color: orange;
      /*font-size: 15px;*/
    }

        </style>
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
					<%--<li><a href="ModelRunHistory.aspx">Model Run History</a></li>--%>
					<li><a href="#">Model Run History</a></li>
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
					<%--<li><a href="ModelRunHistory.aspx">Model Run History</a></li>--%>
					<li><a href="#">Model Run History</a></li>
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

	
	<div class="form__order2"  id="form_1" runat="server" >
		<div class="title">Model <asp:Label runat="server" ID="lblModelRunID" CssClass="title"></asp:Label>&nbsp;Details</div>

		<div class="title">
            <uc6:OptimizerDetailc12 ID="OptimizerDetailabc123unm" runat="server" />
		</div>
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
		</div>
	
	</div>
		
</section>
	
<footer class="normal">
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


<script type="text/javascript" src="js\jQuery_v1.11.js"></script> 
<script type="text/javascript" src="js\jquery.datetimepicker.min.js"></script> 
<script type="text/javascript" src="js\jquery.maskedinput.min.js"></script> 
<script type="text/javascript" src="js\script.js"></script> 
<script type="text/javascript" src="js\core.js"></script> 


    </form>
</body>
</html>