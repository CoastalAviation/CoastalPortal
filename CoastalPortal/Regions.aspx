<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Regions.aspx.vb" Inherits="CoastalPortal.define_regions" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Define Regions</title>
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
<form id="form" runat="server">
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
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server"></telerik:RadAjaxManager>
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
		<div class="title">QUOTE SYSTEM ADMINISTRATION DEFINE REGIONS</div>
		
		<span class="description"><a class="openpopup" href="#addnew">add new region</a></span>
                    <asp:Label ID="lblMsg" Text="" runat="server" forecolor="Red" />
        <span class="description find">  
            <asp:Panel ID="pnlEdit" runat="server" Visible="False">
	            <span class="popup--title">Edit</span>
	            <div class="form">
		            <label>
			            <p class="sub_title">region name:</p>
                                    <asp:TextBox id="edit_region_name_1" runat="server" CssClass="txt" placeholder="Region name"/>
		            </label>
		            <label>
			            <p class="sub_title">description:</p>
                                    <asp:TextBox id="edit_description_1" runat="server" CssClass="txt" placeholder="Region description"/>
		            </label>
		            <label>
			            <p class="sub_title">airport code:</p>
                                    <asp:TextBox id="edit_airport_code_1" runat="server" CssClass="txt" placeholder="Airport code"/>
		            </label>
		            <%--<label>
			            <p class="sub_title">airport name:</p>
                                    <asp:TextBox id="edit_airport_name_1" runat="server" CssClass="txt" placeholder="Airport name"/>
		            </label>--%>
		            <label>
			            <p class="sub_title">radius (nm):</p>
                                     <asp:TextBox id="edit_radius_1" runat="server" CssClass="txt" placeholder="Radius (nm)"/>
		            <br />
		            </label>
		            <label>
			            <p class="sub_title"><b>-- OR --</b></p>
		            </label>
                    <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
                        <telerik:RadComboBox ID="RadComboBoxStates1" runat="server" CheckBoxes="True" 
                            EmptyMessage="States" 
                            EnableCheckAllItemsCheckBox="True" Width="100%">
                            <Items>
                                <telerik:RadComboBoxItem Owner="RadComboBoxStates" Text="FLORIDA" 
                                    ToolTip="FLORIDA" Value="FL" />
                                <telerik:RadComboBoxItem Owner="RadComboBoxStates" Text="NEW YORK" 
                                    ToolTip="NEW YORK" Value="NY" />
                                <telerik:RadComboBoxItem Owner="RadComboBoxStates" Text="CALIFORNIA" 
                                    ToolTip="CALIFORNIA" Value="CA" />
                            </Items>
                        </telerik:RadComboBox>
                    </div>
		            <label class="no">
                        <asp:Button ID="update_popup_ok" CssClass="button" Text="Update" runat="server" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:Button ID="update_popup_delete" CssClass="button" Text="Delete" runat="server" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:Button ID="update_popup_cancel" CssClass="button" Text="Cancel" runat="server" />
		            </label>
                    <asp:HiddenField ID="hddnZoneID" runat="server"></asp:HiddenField>
	            </div>
            </asp:Panel>
            <label> Region </label> 
            <asp:TextBox ID="find_region_txt" runat="server" CssClass="txt" />
            <label> And / or Airport </label>
            <asp:TextBox ID="find_airport_txt" runat="server" CssClass="txt" />
            <asp:Button ID="find_button" runat="server" CssClass="button" Text="Search"/>
        </span>
	<div class="table">


<asp:SqlDataSource ID="get_defineregions" runat="server"
             ConnectionString="<%$ ConnectionStrings:PortalDB %>"
             SelectCommand="SELECT ZoneID, ZoneName, ZoneDescription, BaseAirport, facilityname, RadiusNM FROM PricingZones z left join NfdcFacilities f on z.baseairport = f.locationid where  carrierid = @carrierid order by ZoneName " 
             UpdateCommand="update PricingZones set ZoneName = @ZoneName, ZoneDescription = @ZoneDescription, BaseAirport = @BaseAirport, RadiusNM = @RadiusNM where  carrierid = @carrierid and ZoneID = @ZoneID" >

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
            DataSourceID="get_defineregions" AutoGenerateColumns="False" DataKeyNames="ZoneID,ZoneName,ZoneDescription,BaseAirport,RadiusNM"  
            CssClass="table__tr" AutoGenerateEditButton="True">
            <Columns >
                <%--<asp:BoundField DataField="ZoneID"  HeaderText="Zone ID"  Visible="False" />--%>
                <%--<asp:BoundField DataField="ZoneName"  HeaderText="Region Name"  />--%>
                <%--<asp:BoundField DataField="ZoneDescription"   HeaderText="Description" />--%>
                <%--<asp:BoundField DataField="BaseAirport"   HeaderText="Airport Code" />--%>
                <%--<asp:BoundField DataField="facilityname"  HeaderText="Airport Name" />--%>
                <%--<asp:BoundField DataField="RadiusNM"   HeaderText="Radius (NM)" />--%>
                <asp:TemplateField ShowHeader="true" HeaderText="ZoneID" Visible="False">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "ZoneID")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Zone Name">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "ZoneName")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Zone Description">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "ZoneDescription")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Airport Code">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "BaseAirport")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Airport Name">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "facilityname")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField ShowHeader="true" HeaderText="Radius (NM)">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "RadiusNM")%>
                    </ItemTemplate>
                </asp:TemplateField> 

                <%--<asp:HyperLinkField HeaderText="" Text="EDIT" DataNavigateUrlFormatString="#edit" DataNavigateUrlFields="ZoneID" ControlStyle-CssClass="openpopup" />--%>
                <%--<asp:HyperLinkField HeaderText="" Text="DELETE" DataNavigateUrlFormatString="" DataNavigateUrlFields="ZoneID" />--%>
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
	<span class="popup--title">add new region</span>
	<div class="form">
		<label>
			<p class="sub_title">region name:</p>
            <asp:TextBox CssClass="txt" ID="popup_regionname_txt" runat="server" placeholder="Region name"/>
		</label>
		<label>
			<p class="sub_title">description:</p>
            <asp:TextBox CssClass="txt" ID="popup_rdescription_txt" runat="server" placeholder="Region description"/>
		</label>
		<label>
			<p class="sub_title">airport code:</p>
            <asp:TextBox CssClass="txt" ID="popup_airportcode_txt" runat="server" placeholder="Airport code"/>
		</label>
		<%--<label>
			<p class="sub_title">airport name:</p>
            <asp:TextBox CssClass="txt" ID="popup_airportname_txt" runat="server" placeholder="Airport name"/>
		</label>--%>
		<label>
			<p class="sub_title">radius (nm):</p>
            <asp:TextBox CssClass="txt" ID="popup_radius_txt" runat="server" placeholder="Radius (nm):"/>
		</label>
		<label>
			<p class="sub_title"><b>-- OR --</b></p>
		</label>
        <div style="border-bottom-style: solid; padding-top: 6%; border-bottom-color: #0556a8; border-bottom-width: 1px; padding-bottom: 3%;">
            <telerik:RadComboBox ID="RadComboBoxStates" runat="server" CheckBoxes="True" 
                EmptyMessage="States" 
                EnableCheckAllItemsCheckBox="True" Width="100%">
                <Items>
                    <telerik:RadComboBoxItem Owner="RadComboBoxStates" Text="FLORIDA" 
                        ToolTip="FLORIDA" Value="FL" />
                    <telerik:RadComboBoxItem Owner="RadComboBoxStates" Text="NEW YORK" 
                        ToolTip="NEW YORK" Value="NY" />
                    <telerik:RadComboBoxItem Owner="RadComboBoxStates" Text="CALIFORNIA" 
                        ToolTip="CALIFORNIA" Value="CA" />
                </Items>
            </telerik:RadComboBox>
        </div>
		<label class="no">
            <br />
            <asp:Button ID="popup_add" CssClass="button" Text="Add" runat="server" />
            <asp:Button ID="popup_cancel" CssClass="button" Text="Cancel" runat="server" />
		</label>
	</div>
</div>
<%--<div class="popup " id="edit">
	<span class="popup--title">Edit</span>
	<div class="form">
		<label>
			<p class="sub_title">region name:</p>
                        <asp:TextBox id="edit_region_name_1" runat="server" CssClass="txt" placeholder="Region name"/>
		</label>
		<label>
			<p class="sub_title">description:</p>
                        <asp:TextBox id="edit_description_1" runat="server" CssClass="txt" placeholder="Region description"/>
		</label>
		<label>
			<p class="sub_title">airport code:</p>
                        <asp:TextBox id="edit_airport_code_1" runat="server" CssClass="txt" placeholder="Airport code"/>
		</label>
		<label>
			<p class="sub_title">airport name:</p>
                        <asp:TextBox id="edit_airport_name_1" runat="server" CssClass="txt" placeholder="Airport name"/>
		</label>
		<label>
			<p class="sub_title">radius (nm):</p>
                         <asp:TextBox id="edit_radius_1" runat="server" CssClass="txt" placeholder="Radius (nm)"/>
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
