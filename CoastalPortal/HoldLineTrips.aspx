<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="HoldLineTrips.aspx.vb" Inherits="CoastalPortal.HoldLineTrips" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Hold Line Trips</title>
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
    <asp:SqlDataSource ID="SqlDataSource1" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT distinct r.ID as modelrun,Description,r.GMTStart FROM OptimizerRequest r join FCDRList l on r.ID = l.modelrun where ParentRequestNumber > 0 and r.GMTStart >= DATEADD(d,-2,r.GMTStart) order by r.id desc"
        runat="server"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT * FROM FCDRList Where modelrun = @modelrun"
        runat="server">
        <SelectParameters>
            <asp:SessionParameter Name="modelrun" SessionField="modelrun" Type="Int32"></asp:SessionParameter>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource3" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT * FROM FCDRListDetail where KeyId = @KeyId"
        runat="server">
        <SelectParameters>
            <asp:SessionParameter Name="KeyId" SessionField="KeyId" Type="Int32"></asp:SessionParameter>
        </SelectParameters>
    </asp:SqlDataSource>

	
	<div class="form__order2"  id="form_1" runat="server" >
		<div class="title">Hold Line Trips</div>
		<div class="title"> <%--<asp:Label runat="server" ID="aircraft_type_txt_1" CssClass="title"></asp:Label>--%> </div>
       <div style="align-items:center; justify-content:center;margin-left:10px;">
           <asp:UpdatePanel EnableViewState="false" runat="server" ID="FCSummary">
               <ContentTemplate>

                    <telerik:RadGrid RenderMode="Lightweight" ID="RadGrid1"  ShowStatusBar="true"
                        DataSourceID="SqlDataSource1" runat="server" AutoGenerateColumns="False" PageSize="10"
                        AllowSorting="True" AllowMultiRowSelection="False" AllowPaging="True" GridLines="None">
                        <PagerStyle Mode="NumericPages"></PagerStyle>
                        <MasterTableView EnableHierarchyExpandAll="true" DataSourceID="SqlDataSource1" DataKeyNames="modelrun" AllowMultiColumnSorting="True">
                            <DetailTables>
                                <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="KeyId" DataSourceID="SqlDataSource2" Width="100%"
                                    runat="server">
                                    <ParentTableRelation>
                                        <telerik:GridRelationFields DetailKeyField="modelrun" MasterKeyField="modelrun"></telerik:GridRelationFields>
                                    </ParentTableRelation>
                                    <DetailTables>
                                        <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="KeyId" DataSourceID="SqlDataSource3" Width="100%"
                                            runat="server">
                                            <ParentTableRelation>
                                                <telerik:GridRelationFields DetailKeyField="KeyId" MasterKeyField="KeyId"></telerik:GridRelationFields>
                                            </ParentTableRelation>
                                            <Columns>
                                                <telerik:GridBoundColumn SortExpression="TripNumber" HeaderText="Trip Number" HeaderButtonType="TextButton"
                                                    DataField="TripNumber" UniqueName="TripNumber">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn SortExpression="AC" HeaderText="AC" HeaderButtonType="TextButton"
                                                    DataField="AC" UniqueName="AC">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn SortExpression="From_ICAO" HeaderText="From" HeaderButtonType="TextButton"
                                                    DataField="From_ICAO" UniqueName="From_ICAO">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                            <SortExpressions>
                                                <telerik:GridSortExpression FieldName="AC" SortOrder="Descending"></telerik:GridSortExpression>
                                            </SortExpressions>
                                        </telerik:GridTableView>
                                    </DetailTables>
                                    <Columns>
                                        <telerik:GridBoundColumn SortExpression="KeyId" HeaderText="KeyId" HeaderButtonType="TextButton"
                                            DataField="KeyId" UniqueName="KeyId">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="PriorTailNumber" HeaderText="Tail" HeaderButtonType="TextButton"
                                            DataField="PriorTailNumber" UniqueName="PriorTailNumber">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="TotalSavings" HeaderText="Total Savings" HeaderButtonType="TextButton"
                                            DataField="TotalSavings" UniqueName="TotalSavings">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                    <SortExpressions>
                                        <telerik:GridSortExpression FieldName="PriorTailNumber"></telerik:GridSortExpression>
                                    </SortExpressions>
                                </telerik:GridTableView>
                            </DetailTables>
                            <Columns>
                                <telerik:GridBoundColumn SortExpression="modelrun" HeaderText="MODEL RUN" HeaderButtonType="TextButton"
                                    DataField="modelrun" UniqueName="modelrun">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="GMTStart" HeaderText="GMT Start" HeaderButtonType="TextButton"
                                    DataField="GMTStart" UniqueName="GMTStart" DataFormatString="{0:D}">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="TripNumber" HeaderText="Trip Number" HeaderButtonType="TextButton"
                                    DataField="TripNumber" UniqueName="TripNumber">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Weightclass" HeaderText="Weight class" HeaderButtonType="TextButton"
                                    DataField="Weightclass" UniqueName="Weightclass">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton"
                                    DataField="Description" UniqueName="Description">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="DepartureAirport" HeaderText="From" HeaderButtonType="TextButton"
                                    DataField="DepartureAirport" UniqueName="DepartureAirport">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="ArrivalAirport" HeaderText="To" HeaderButtonType="TextButton"
                                    DataField="ArrivalAirport" UniqueName="ArrivalAirport">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Departs" HeaderText="Departs" HeaderButtonType="TextButton"
                                    DataField="Departs" UniqueName="Departs" DataFormatString="{0:D}">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="CarrierID" HeaderText="CarrierID" HeaderButtonType="TextButton"
                                    DataField="CarrierID" UniqueName="CarrierID">
                                </telerik:GridBoundColumn>
                            </Columns>
                            <SortExpressions>
                                <telerik:GridSortExpression FieldName="modelrun"></telerik:GridSortExpression>
                            </SortExpressions>
                        </MasterTableView>
                    </telerik:RadGrid>
               
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
                <asp:BoundField DataField="carrierid" HeaderText="carrier id" SortExpression="carrierid" ItemStyle-HorizontalAlign="Center"/>
                </Columns>
            </asp:GridView>	
                   </ContentTemplate>
               </asp:UpdatePanel>
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
