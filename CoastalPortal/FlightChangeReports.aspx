<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FlightChangeReports.aspx.vb" Inherits="CoastalPortal.FlightChangeReports"  EnableEventValidation="false"%>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Flight Change Reports</title>
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

	
	<div class="form__order2"  id="form_1" runat="server" >
		<div class="title">Review Flight Change Reports</div>
		<span class="description" style="width: 100%">
            <div class="form" style="width: 100%" runat="server" id="divHeading">
                <asp:Label ID="Label1" runat="server" Text="Trip number: " Font-Size="x-small"></asp:Label>
                <asp:TextBox ID="txtTripnumber" runat="server" Width="150px" Height="24px" Enabled="False"></asp:TextBox>
                <asp:Label ID="Label2" runat="server" Text="Weight class: " Font-Size="x-small"></asp:Label>
                <asp:TextBox ID="txtWeightclass" runat="server" Width="50px" Height="24px" Enabled="False"></asp:TextBox>
                <br />
                <asp:Label ID="Label3" runat="server" Text="origin: " Font-Size="x-small"></asp:Label>
                <asp:TextBox ID="txtorigin" runat="server" Width="50px" Height="24px" Enabled="False"></asp:TextBox>
                <asp:Label ID="Label4" runat="server" Text="destination: " Font-Size="x-small"></asp:Label>
                <asp:TextBox ID="txtdest" runat="server" Width="50px" Height="24px" Enabled="False"></asp:TextBox>
                <br />
                <asp:Label ID="Label5" runat="server" Text="departs: " Font-Size="x-small"></asp:Label>
                <asp:TextBox ID="txtdeparture" runat="server" Width="170px" Height="24px" Enabled="False"></asp:TextBox>
		            <%--<label id="lblHeader" runat="server" style="width: 100%">
			            <p class="sub_title" style="width: 100%">Trip number:
                            <asp:TextBox id="TextBox1" runat="server" CssClass="txt" placeholder="Trip number">Trip number</asp:TextBox>
                            Weight class requested:
                            <asp:TextBox id="TextBox2" runat="server" CssClass="txt" placeholder="Weight class">Weight class</asp:TextBox>
                            origin:
                            <asp:TextBox id="TextBox3" runat="server" CssClass="txt" placeholder="origin">origin</asp:TextBox>
                            destination:
                            <asp:TextBox id="TextBox4" runat="server" CssClass="txt" placeholder="destination">destination</asp:TextBox>
                            departure date/time:
                            <asp:TextBox id="TextBox5" runat="server" CssClass="txt" placeholder="departure date/time">departure date/time</asp:TextBox>
			            </p>
		            </label>--%>
            </div>
		</span>

		<div class="title"> <%--<asp:Label runat="server" ID="aircraft_type_txt_1" CssClass="title"></asp:Label>--%> </div>
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
				</ul> CssClass="table__tr" HeaderStyle-CssClass="table__h"
			</div> <div style="align-items:center; justify-content:center;margin-left:10px;"> 

		</div>	
		</div>--%>
       <div style="align-items:center; justify-content:center;margin-left:10px;">
           <asp:UpdatePanel EnableViewState="false" runat="server" ID="FCSummary">
               <ContentTemplate>
               
    <asp:GridView ID="gvFCDRList" runat="server"  BorderWidth="0" AutoGenerateColumns="False"  CssClass="fcdrlist__tr" HeaderStyle-CssClass="fcdrlist__h" 
            HeaderStyle-HorizontalAlign="Center"  ItemType="CoastalPortal.FCDRList" AllowPaging="True" PageSize="10" 
            PagerStyle-HorizontalAlign="Center" OnPageIndexChanging="gvFCDRList_PageIndexChanging" OnPreRender="gvFCDRList_PreRender">
        <PagerSettings FirstPageText="First Page" LastPageText="Last Page" visible="true"/>
        <PagerStyle Font-Size="Medium" />
            <Columns >
               <asp:TemplateField HeaderText ="Show Details">
                    <ItemTemplate>
                        <button name="btnselect" value='<%#Eval("keyid") %>' >Change Details</button>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField Text="See Schedule" DataNavigateUrlFields="PDFLink" HeaderText="Schedule" SortExpression="Key" ItemStyle-HorizontalAlign="Center" Target="_blank" />
                <asp:BoundField DataField="modelrun" HeaderText="Model Run" SortExpression="ModelRun" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="GMTStart" HeaderText="Model Start" SortExpression="Start" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="deltanonrevmiles" HeaderText="Delta NRM" SortExpression="NRM" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="#00936F"/>
                <asp:BoundField DataField="TotalSavings" HeaderText="TOT SAVE" SortExpression="TotalSavings" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center"  ItemStyle-ForeColor="#00936F"/>
                <asp:BoundField DataField="savingsday0" HeaderText="SAV D0" SortExpression="SaveNow" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center"  ItemStyle-ForeColor="#00936F"/>
                <asp:BoundField DataField="savingsday1" HeaderText="SAV D1" SortExpression="Save1" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="#00936F"/>
                <asp:BoundField DataField="savingsday2" HeaderText="SAV D2" SortExpression="Save2" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Center"  ItemStyle-ForeColor="#00936F"/>
                <asp:BoundField DataField="priortailnumber" HeaderText="Starting Tail" SortExpression="priortail" ItemStyle-HorizontalAlign="Center"/>
                <asp:BoundField DataField="carrieracceptstatus" HeaderText="Accept/Reject" SortExpression="Accept" ItemStyle-HorizontalAlign="Center"/>
               <asp:TemplateField HeaderText ="Accept/Reject">
                    <ItemTemplate>
                        <button name="btnacpt" value='<%# "accept" + " " + Eval("keyid") %>' >Accept</button>&nbsp;&nbsp; 
                        <button name="btnacpt" value='<%# "reject" + " " + Eval("keyid") %>' >Reject</button>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:BoundField DataField="keyid"  />
                  <asp:BoundField DataField="isTrade"  />
                </Columns>
            </asp:GridView>	
                   </ContentTemplate>
               </asp:UpdatePanel>
           <br />
           <br />
           <asp:UpdatePanel runat="server" ID="DetailPanel">
               <ContentTemplate>
                   <asp:GridView ID="gvFCDRDetail" runat="server" BorderWidth="0" AutoGenerateColumns="False"  CssClass="fcdrlist__tr" HeaderStyle-CssClass="fcdrlist__h" HeaderStyle-HorizontalAlign="Center"  
                       AllowPaging="True" PageSize="10" PagerStyle-HorizontalAlign="Center" OnPageIndexChanging="gvFCDRDetail_PageIndexChanging" ItemType="CoastalPortal.FCDRListDetail" OnDataBound="gvFCDRDetail_DataBound" 
                       EmptyDataText="No Revenue trips were added or changed in this Report" Caption="<b>Change Details for Revenue Flights</b>">
                               <PagerSettings FirstPageText="First Page" LastPageText="Last Page"  />
                             <PagerStyle Font-Size="Medium" />
                       <columns>
                           <asp:BoundField Datafield ="TripNumber" HeaderText="Trip Number" SortExpression="TripNumber" ItemStyle-HorizontalAlign="Center"/>
                           <asp:BoundField Datafield ="DepartDate" HeaderText="Departure Date(GMT)" SortExpression="DepartDate" ItemStyle-HorizontalAlign="Center"/>
                           <asp:BoundField Datafield ="From_ICAO" HeaderText="From" SortExpression="From" ItemStyle-HorizontalAlign="Center"/>
                           <asp:BoundField Datafield ="To_ICAO" HeaderText="To" SortExpression="To" ItemStyle-HorizontalAlign="Center"/>
                           <asp:BoundField Datafield ="AC" HeaderText="Prior Tail" SortExpression="OAC" ItemStyle-HorizontalAlign="Center"/>
                           <asp:BoundField Datafield ="Modification" HeaderText="New Tail" SortExpression="Result" ItemStyle-HorizontalAlign="Center"/>
                       </columns>
                   </asp:GridView>
                   </ContentTemplate>
               </asp:UpdatePanel>
           </div>
        	<%--<div class="form__order2"  id="Div1" runat="server" >
		        <div class="title">Change Details for Revenue Flights</div>
                <div style="padding-left:30px">
		        </div>
            </div>--%>

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
