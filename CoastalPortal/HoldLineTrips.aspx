﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="HoldLineTrips.aspx.vb" Inherits="CoastalPortal.HoldLineTrips" %>
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
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" ShowChooser="False" Skin="Outlook" />
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
		<div class="title">Hold Line Trips</div>
		<div class="title"> <%--<asp:Label runat="server" ID="aircraft_type_txt_1" CssClass="title"></asp:Label>--%> </div>
       <div style="align-items:center; justify-content:center;margin-left:10px;">
           <%--<asp:UpdatePanel EnableViewState="false" runat="server" ID="FCSummary">
               <ContentTemplate>
                </ContentTemplate>
            </asp:UpdatePanel>--%>
            <telerik:RadGrid RenderMode="Lightweight" ID="RadGrid1"  ShowStatusBar="true"
                DataSourceID="SqlDataSource1" runat="server" AutoGenerateColumns="False" PageSize="10"
                AllowSorting="True" AllowMultiRowSelection="False" AllowPaging="True" GridLines="None" Font-Size="Medium" Visible="False">
                <PagerStyle Mode="NumericPages"></PagerStyle>
                <MasterTableView EnableHierarchyExpandAll="true" DataSourceID="SqlDataSource1" DataKeyNames="modelrun" AllowMultiColumnSorting="True" Name="Parent">
                    <DetailTables>
                        <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="KeyId" DataSourceID="SqlDataSource2" Width="100%"
                            runat="server" Name="Child1">
                            <ParentTableRelation>
                                <telerik:GridRelationFields DetailKeyField="modelrun" MasterKeyField="modelrun"></telerik:GridRelationFields>
                            </ParentTableRelation>
                            <DetailTables>
                                <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="KeyId" DataSourceID="SqlDataSource3" Width="100%"
                                    runat="server" NoDetailRecordsText="No FCDRs to display." NoMasterRecordsText="No Hold Line Trips to display." Name="Child2">
                                    <ParentTableRelation>
                                        <telerik:GridRelationFields DetailKeyField="KeyId" MasterKeyField="KeyId"></telerik:GridRelationFields>
                                    </ParentTableRelation>
                                    <Columns>
                                        <telerik:GridBoundColumn SortExpression="TripNumber" HeaderText="<b>TRIP NUMBER</b>" HeaderButtonType="TextButton"
                                            DataField="TripNumber" UniqueName="TripNumber">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="DepartDate" HeaderText="<b>DEPARTURE DATE (GMT)</b>" HeaderButtonType="TextButton"
                                            DataField="DepartDate" UniqueName="DepartDate" DataFormatString="{0:g}">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="From_ICAO" HeaderText="<b>ORIGIN</b>" HeaderButtonType="TextButton"
                                            DataField="From_ICAO" UniqueName="From_ICAO">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="To_ICAO" HeaderText="<b>DESTINATION</b>" HeaderButtonType="TextButton"
                                            DataField="To_ICAO" UniqueName="To_ICAO">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="AC" HeaderText="<b>PRIOR TAIL</b>" HeaderButtonType="TextButton"
                                            DataField="AC" UniqueName="AC">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn SortExpression="Modification" HeaderText="<b>NEW TAIL</b>" HeaderButtonType="TextButton"
                                            DataField="Modification" UniqueName="Modification">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                    <SortExpressions>
                                        <telerik:GridSortExpression FieldName="DepartDate" SortOrder="Ascending"></telerik:GridSortExpression>
                                    </SortExpressions>
                                </telerik:GridTableView>
                            </DetailTables>
                            <Columns>
                                <telerik:GridBoundColumn SortExpression="modelrun" HeaderText="<b>MODEL RUN</b>" HeaderButtonType="TextButton"
                                    DataField="modelrun" UniqueName="modelrun">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="DeltaNonRevMiles" HeaderText="<b>DELTA NRM</b>" HeaderButtonType="TextButton"
                                    DataField="DeltaNonRevMiles" UniqueName="DeltaNonRevMiles">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="TotalSavings" HeaderText="<b>TOTAL SAVINGS</b>" HeaderButtonType="TextButton"
                                    DataField="TotalSavings" UniqueName="TotalSavings" DataFormatString="{0:c0}">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="SavingsDay0" HeaderText="<b>SAVINGS D0</b>" HeaderButtonType="TextButton"
                                    DataField="SavingsDay0" UniqueName="SavingsDay0" DataFormatString="{0:c0}">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="SavingsDay1" HeaderText="<b>SAVINGS D1</b>" HeaderButtonType="TextButton"
                                    DataField="SavingsDay1" UniqueName="SavingsDay1" DataFormatString="{0:c0}">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="SavingsDay2" HeaderText="<b>SAVINGS D2</b>" HeaderButtonType="TextButton"
                                    DataField="SavingsDay2" UniqueName="SavingsDay2" DataFormatString="{0:c0}">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="PriorTailNumber" HeaderText="<b>STARTING TAIL</b>" HeaderButtonType="TextButton"
                                    DataField="PriorTailNumber" UniqueName="PriorTailNumber">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Registration" HeaderText="<b>REGISTRATION</b>" HeaderButtonType="TextButton"
                                    DataField="Registration" UniqueName="Registration">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="CarrierID" HeaderText="<b>CARRIER ID</b>" HeaderButtonType="TextButton"
                                    DataField="CarrierID" UniqueName="CarrierID">
                                </telerik:GridBoundColumn>
                                <telerik:GridHyperLinkColumn Text="See Schedule" HeaderText="<b>Schedule</b>" 
                                    Target="_blank" DataNavigateUrlFields="PDFLink" ItemStyle-ForeColor="#0066FF">
                                </telerik:GridHyperLinkColumn>
                                <telerik:GridBoundColumn SortExpression="ReviewedDate" HeaderText="<b>DATE REVIEWED</b>" HeaderButtonType="TextButton"
                                    DataField="ReviewedDate" UniqueName="ReviewedDate" Visible="False">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="ReviewedByInit" HeaderText="<b>BY</b>" HeaderButtonType="TextButton"
                                    DataField="ReviewedByInit" UniqueName="ReviewedByInit" Visible="False">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Notes" HeaderText="<b>NOTES</b>" HeaderButtonType="TextButton"
                                    DataField="Notes" UniqueName="Notes" ItemStyle-Width="20%" Visible="False">
                                </telerik:GridBoundColumn>
                            </Columns>
                            <SortExpressions>
                                <telerik:GridSortExpression FieldName="TotalSavings" SortOrder="Descending"></telerik:GridSortExpression>
                            </SortExpressions>
                        </telerik:GridTableView>
                    </DetailTables>
                    <Columns>
                        <telerik:GridBoundColumn SortExpression="modelrun" HeaderText="<b>MODEL RUN</b>" HeaderButtonType="TextButton"
                            DataField="modelrun" UniqueName="modelrun">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn SortExpression="Description" HeaderText="<b>DESCRIPTION</b>" HeaderButtonType="TextButton"
                            DataField="Description" UniqueName="Description">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn SortExpression="GMTStart" HeaderText="<b>GMT START</b>" HeaderButtonType="TextButton"
                            DataField="GMTStart" UniqueName="GMTStart" DataFormatString="{0:g}">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn SortExpression="TripNumber" HeaderText="<b>TRIP NUMBER</b>" HeaderButtonType="TextButton"
                            DataField="TripNumber" UniqueName="TripNumber">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn SortExpression="Weightclass" HeaderText="<b>WEIGHT CLASS</b>" HeaderButtonType="TextButton"
                            DataField="Weightclass" UniqueName="Weightclass">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn SortExpression="DepartureAirport" HeaderText="<b>ORIGIN</b>" HeaderButtonType="TextButton"
                            DataField="DepartureAirport" UniqueName="DepartureAirport">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn SortExpression="ArrivalAirport" HeaderText="<b>DESTINATION</b>" HeaderButtonType="TextButton"
                            DataField="ArrivalAirport" UniqueName="ArrivalAirport">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn SortExpression="DEPARTS" HeaderText="<b>DEPARTURE DATE (GMT)</b>" HeaderButtonType="TextButton"
                            DataField="DEPARTS" UniqueName="DEPARTS" DataFormatString="{0:g}">
                        </telerik:GridBoundColumn>
                    </Columns>
                    <SortExpressions>
                        <telerik:GridSortExpression FieldName="modelrun" SortOrder="Descending"></telerik:GridSortExpression>
                    </SortExpressions>
                </MasterTableView>
            </telerik:RadGrid>
            <asp:SqlDataSource ID="SqlDataSource1" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
                ProviderName="System.Data.SqlClient" runat="server" 
                SelectCommand="SELECT distinct r.ID as modelrun,Description,replace(TripNumber,r.CarrierID + '-','') as TripNumber,Weightclass,DepartureAirport,ArrivalAirport,cast(DepartureDate + ' ' + DepartureTime as datetime) as DEPARTS,r.CarrierID,r.GMTStart FROM OptimizerRequest r join QuoteFlights q on r.ID = q.QuoteNumber join FCDRList l on r.ID = l.modelrun where ParentRequestNumber &gt; 0 and r.GMTStart &gt;= DATEADD(d,-7,getdate()) and r.CarrierID = @carrierid order by r.id desc">
                <SelectParameters>
                    <asp:Parameter Name="carrierid" />
                </SelectParameters>
           </asp:SqlDataSource>
            <asp:SqlDataSource ID="SqlDataSource2" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
                ProviderName="System.Data.SqlClient" runat="server"
                SelectCommand="SELECT max(keyid) as KeyId,modelrun,DeltaNonRevMiles,TotalSavings,SavingsDay0,SavingsDay1,SavingsDay2,PriorTailNumber,Registration,l.CarrierID,ReviewedDate,ReviewedByInit,Notes,'~/FCDRpages/' + KeyId + '.pdf' as PDFLink FROM FCDRList l join Aircraft a on l.CarrierID = a.CarrierID and l.PriorTailNumber = a.FOSAircraftID Where modelrun = @modelrun group by modelrun,DeltaNonRevMiles,TotalSavings,SavingsDay0,SavingsDay1,SavingsDay2,PriorTailNumber,Registration,l.CarrierID,ReviewedDate,ReviewedByInit,Notes,KeyId">
                <SelectParameters>
                    <asp:SessionParameter Name="modelrun" SessionField="modelrun" Type="Int32"></asp:SessionParameter>
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SqlDataSource3" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
                ProviderName="System.Data.SqlClient" runat="server"
                SelectCommand="SELECT d.KeyId,case when CHARINDEX('-', TripNumber) > 0 then SUBSTRING(TripNumber,CHARINDEX('-', TripNumber) + 1,LEN(TripNumber) - CHARINDEX('-', TripNumber)) else TripNumber end as TripNumber,DepartDate,From_ICAO,To_ICAO,AC,Modification FROM FCDRListDetail d join FCDRList l on d.KeyID = l.KeyId where d.KeyId = @KeyId">
                <SelectParameters>
                    <asp:SessionParameter Name="KeyId" SessionField="KeyId" Type="Int32"></asp:SessionParameter>
                </SelectParameters>
            </asp:SqlDataSource>

           <br />
           <br />
 
           <telerik:RadGrid RenderMode="Lightweight" ID="RadGrid2"  ShowStatusBar="true"
            DataSourceID="SqlDataSource4" runat="server" AutoGenerateColumns="False" PageSize="10"
            AllowSorting="True" AllowMultiRowSelection="False" AllowPaging="True" GridLines="None" Font-Size="Medium" Visible="True">
            <PagerStyle Mode="NumericPages"></PagerStyle>
            <MasterTableView EnableHierarchyExpandAll="true" DataSourceID="SqlDataSource4" DataKeyNames="modelrun" AllowMultiColumnSorting="True">
                <DetailTables>
                    <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="modelrun" DataSourceID="SqlDataSource5" Width="100%"
                        runat="server" Name="Child3">
                        <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="modelrun" MasterKeyField="modelrun"></telerik:GridRelationFields>
                        </ParentTableRelation>
                        <DetailTables>
                            <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="KeyId" DataSourceID="SqlDataSource6" Width="100%"
                                runat="server" Name="Child4">
                                <ParentTableRelation>
                                    <telerik:GridRelationFields DetailKeyField="modelrun" MasterKeyField="modelrun"></telerik:GridRelationFields>
                                </ParentTableRelation>
                                <DetailTables>
                                    <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="KeyId" DataSourceID="SqlDataSource7" Width="100%"
                                        runat="server" Name="Child5">
                                        <ParentTableRelation>
                                            <telerik:GridRelationFields DetailKeyField="KeyId" MasterKeyField="KeyId"></telerik:GridRelationFields>
                                        </ParentTableRelation>
                                        <Columns>
                                            <telerik:GridBoundColumn SortExpression="TripNumber" HeaderText="<b>TRIP NUMBER</b>" HeaderButtonType="TextButton"
                                                DataField="TripNumber" UniqueName="TripNumber">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn SortExpression="DepartDate" HeaderText="<b>DEPARTURE DATE (GMT)</b>" HeaderButtonType="TextButton"
                                                DataField="DepartDate" UniqueName="DepartDate" DataFormatString="{0:g}">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn SortExpression="From_ICAO" HeaderText="<b>ORIGIN</b>" HeaderButtonType="TextButton"
                                                DataField="From_ICAO" UniqueName="From_ICAO">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn SortExpression="To_ICAO" HeaderText="<b>DESTINATION</b>" HeaderButtonType="TextButton"
                                                DataField="To_ICAO" UniqueName="To_ICAO">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn SortExpression="AC" HeaderText="<b>PRIOR TAIL</b>" HeaderButtonType="TextButton"
                                                DataField="AC" UniqueName="AC">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn SortExpression="Modification" HeaderText="<b>NEW TAIL</b>" HeaderButtonType="TextButton"
                                                DataField="Modification" UniqueName="Modification">
                                            </telerik:GridBoundColumn>
                                        </Columns>
                                        <SortExpressions>
                                            <telerik:GridSortExpression FieldName="DepartDate" SortOrder="Ascending"></telerik:GridSortExpression>
                                        </SortExpressions>
                                    </telerik:GridTableView>
                                </DetailTables>
                                <Columns>
                                    <telerik:GridBoundColumn SortExpression="modelrun" HeaderText="<b>MODEL RUN</b>" HeaderButtonType="TextButton"
                                        DataField="modelrun" UniqueName="modelrun">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="DeltaNonRevMiles" HeaderText="<b>DELTA NRM</b>" HeaderButtonType="TextButton"
                                        DataField="DeltaNonRevMiles" UniqueName="DeltaNonRevMiles">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="TotalSavings" HeaderText="<b>TOTAL SAVINGS</b>" HeaderButtonType="TextButton"
                                        DataField="TotalSavings" UniqueName="TotalSavings" DataFormatString="{0:c0}">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="SavingsDay0" HeaderText="<b>SAVINGS D0</b>" HeaderButtonType="TextButton"
                                        DataField="SavingsDay0" UniqueName="SavingsDay0" DataFormatString="{0:c0}">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="SavingsDay1" HeaderText="<b>SAVINGS D1</b>" HeaderButtonType="TextButton"
                                        DataField="SavingsDay1" UniqueName="SavingsDay1" DataFormatString="{0:c0}">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="SavingsDay2" HeaderText="<b>SAVINGS D2</b>" HeaderButtonType="TextButton"
                                        DataField="SavingsDay2" UniqueName="SavingsDay2" DataFormatString="{0:c0}">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="PriorTailNumber" HeaderText="<b>STARTING TAIL</b>" HeaderButtonType="TextButton"
                                        DataField="PriorTailNumber" UniqueName="PriorTailNumber">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="Registration" HeaderText="<b>REGISTRATION</b>" HeaderButtonType="TextButton"
                                        DataField="Registration" UniqueName="Registration">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="CarrierID" HeaderText="<b>CARRIER ID</b>" HeaderButtonType="TextButton"
                                        DataField="CarrierID" UniqueName="CarrierID">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridHyperLinkColumn Text="See Schedule" HeaderText="<b>Schedule</b>" 
                                        Target="_blank" DataNavigateUrlFields="PDFLink" ItemStyle-ForeColor="#0066FF">
                                    </telerik:GridHyperLinkColumn>
                                    <telerik:GridBoundColumn SortExpression="ReviewedDate" HeaderText="<b>DATE REVIEWED</b>" HeaderButtonType="TextButton"
                                        DataField="ReviewedDate" UniqueName="ReviewedDate" Visible="False">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="ReviewedByInit" HeaderText="<b>BY</b>" HeaderButtonType="TextButton"
                                        DataField="ReviewedByInit" UniqueName="ReviewedByInit" Visible="False">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="Notes" HeaderText="<b>NOTES</b>" HeaderButtonType="TextButton"
                                        DataField="Notes" UniqueName="Notes" ItemStyle-Width="20%" Visible="False">
                                    </telerik:GridBoundColumn>
                                </Columns>
                                <SortExpressions>
                                    <telerik:GridSortExpression FieldName="TotalSavings" SortOrder="Descending"></telerik:GridSortExpression>
                                </SortExpressions>
                            </telerik:GridTableView>
                        </DetailTables>
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="modelrun" HeaderText="<b>MODEL RUN</b>" HeaderButtonType="TextButton"
                                DataField="modelrun" UniqueName="modelrun">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="Description" HeaderText="<b>DESCRIPTION</b>" HeaderButtonType="TextButton"
                                DataField="Description" UniqueName="Description">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="GMTStart" HeaderText="<b>GMT START</b>" HeaderButtonType="TextButton"
                                DataField="GMTStart" UniqueName="GMTStart" DataFormatString="{0:g}">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="TripNumber" HeaderText="<b>TRIP NUMBER</b>" HeaderButtonType="TextButton"
                                DataField="TripNumber" UniqueName="TripNumber">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="Weightclass" HeaderText="<b>WEIGHT CLASS</b>" HeaderButtonType="TextButton"
                                DataField="Weightclass" UniqueName="Weightclass">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="DepartureAirport" HeaderText="<b>ORIGIN</b>" HeaderButtonType="TextButton"
                                DataField="DepartureAirport" UniqueName="DepartureAirport">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="ArrivalAirport" HeaderText="<b>DESTINATION</b>" HeaderButtonType="TextButton"
                                DataField="ArrivalAirport" UniqueName="ArrivalAirport">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="DEPARTS" HeaderText="<b>DEPARTURE DATE (GMT)</b>" HeaderButtonType="TextButton"
                                DataField="DEPARTS" UniqueName="DEPARTS" DataFormatString="{0:g}">
                            </telerik:GridBoundColumn>
                            <%--<telerik:GridBoundColumn SortExpression="CarrierID" HeaderText="<b>CARRIER ID</b>" HeaderButtonType="TextButton"
                                DataField="CarrierID" UniqueName="CarrierID">
                            </telerik:GridBoundColumn>--%>
                        </Columns>
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="modelrun"></telerik:GridSortExpression>
                        </SortExpressions>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="modelrun" HeaderText="<b>MODEL RUN</b>" HeaderButtonType="TextButton"
                        DataField="modelrun" UniqueName="modelrun">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Description" HeaderText="<b>DESCRIPTION</b>" HeaderButtonType="TextButton"
                        DataField="Description" UniqueName="Description">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="RequestDate" HeaderText="<b>DATE REQUESTED</b>" HeaderButtonType="TextButton"
                        DataField="RequestDate" UniqueName="RequestDate" DataFormatString="{0:g}">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Complete" HeaderText="<b>COMPLETE</b>" HeaderButtonType="TextButton"
                        DataField="Complete" UniqueName="Complete">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="GMTStart" HeaderText="<b>GMT START</b>" HeaderButtonType="TextButton"
                        DataField="GMTStart" UniqueName="GMTStart" DataFormatString="{0:g}">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="GMTEnd" HeaderText="<b>GMT END</b>" HeaderButtonType="TextButton"
                        DataField="GMTEnd" UniqueName="GMTEnd" DataFormatString="{0:g}">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CarrierID" HeaderText="<b>CARRIER ID</b>" HeaderButtonType="TextButton"
                        DataField="CarrierID" UniqueName="CarrierID">
                    </telerik:GridBoundColumn>
                </Columns>
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="modelrun" SortOrder="Descending"></telerik:GridSortExpression>
                </SortExpressions>
            </MasterTableView>
        </telerik:RadGrid>
    <asp:SqlDataSource ID="SqlDataSource4" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" runat="server" 
        SelectCommand="SELECT distinct r.ID as modelrun,r.Description,r.RequestDate,r.declaredcomplete,r.CompleteDate,case when r.CompleteDate is not null then (case when r.CarrierID = 108 then 'Y' else format(r.CompleteDate,'g','en-US') end) else 'N' end as Complete,r.GMTStart,r.GMTEnd,r.CarrierID FROM OptimizerRequest r join OptimizerRequest r2 on r.id = r2.ParentRequestNumber where r.CarrierID = @carrierid and r.DemandFlights = 1 and r.GMTStart &gt;= DATEADD(d,-2,r.GMTStart) order by r.id desc">
        <SelectParameters>
            <asp:Parameter Name="carrierid" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource5" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" runat="server"
        SelectCommand="SELECT distinct r.ID as modelrun,Description,replace(TripNumber,r.CarrierID + '-','') as TripNumber,Weightclass,DepartureAirport,ArrivalAirport,cast(DepartureDate + ' ' + DepartureTime as datetime) as DEPARTS,r.CarrierID,r.GMTStart FROM OptimizerRequest r join QuoteFlights q on r.ID = q.QuoteNumber join FCDRList l on r.ID = l.modelrun where ParentRequestNumber = @modelrun and r.GMTStart &gt;= DATEADD(d,-2,r.GMTStart) order by r.id desc">
        <SelectParameters>
            <asp:SessionParameter Name="modelrun" SessionField="modelrun" Type="Int32"></asp:SessionParameter>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource6" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" runat="server"
        SelectCommand="SELECT max(keyid) as KeyId,modelrun,DeltaNonRevMiles,TotalSavings,SavingsDay0,SavingsDay1,SavingsDay2,PriorTailNumber,Registration,l.CarrierID,ReviewedDate,ReviewedByInit,Notes,'~/FCDRpages/' + KeyId + '.pdf' as PDFLink FROM FCDRList l join Aircraft a on l.CarrierID = a.CarrierID and l.PriorTailNumber = a.FOSAircraftID Where modelrun = @modelrun group by modelrun,DeltaNonRevMiles,TotalSavings,SavingsDay0,SavingsDay1,SavingsDay2,PriorTailNumber,Registration,l.CarrierID,ReviewedDate,ReviewedByInit,Notes,KeyId">
        <SelectParameters>
            <asp:SessionParameter Name="modelrun" SessionField="modelrun" Type="Int32"></asp:SessionParameter>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource7" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" runat="server"
        SelectCommand="SELECT d.KeyId,case when CHARINDEX('-', TripNumber) &gt; 0 then SUBSTRING(TripNumber,CHARINDEX('-', TripNumber) + 1,LEN(TripNumber) - CHARINDEX('-', TripNumber)) else TripNumber end as TripNumber,DepartDate,From_ICAO,To_ICAO,AC,Modification FROM FCDRListDetail d join FCDRList l on d.KeyID = l.KeyId where d.KeyId = @KeyId">
        <SelectParameters>
            <asp:SessionParameter Name="KeyId" SessionField="KeyId" Type="Int32"></asp:SessionParameter>
        </SelectParameters>
    </asp:SqlDataSource>

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
