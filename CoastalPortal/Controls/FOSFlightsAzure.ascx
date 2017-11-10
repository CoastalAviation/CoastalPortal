<%@ Control Language="vb" AutoEventWireup="false" Inherits="CoastalPortal.FOSFlightsAzureabc123" Codebehind="FOSFlightsAzure.ascx.vb" %>

<%--<%@ Register TagPrefix="eo" Namespace="EO.Web" Assembly="EO.Web" %>--%>

 <META HTTP-EQUIV="Pragma" CONTENT="no-cache"> 

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
           
            font-size: 10px;
            color: #004b91;
        }
    
a:link, a:visited
{
}

    
    
        </style>
   



<telerik:RadCodeBlock ID="radCodeBlock" runat="server">
        <script type="text/javascript" language="javascript">
            var daaRecieved;

            function ShowAOG(EmpId) {
                var oManager = GetRadWindowManager();
                var oWnd = oManager.GetWindowByName("wndShowDepart");
                var url = 'AOGReason.aspx?Id=' + EmpId + '&Rnd=' + Math.random();
                oWnd.SetUrl(url);
                oWnd.Show();
                return false;
            }



            function ShowPIN(EmpId) {
                var oManager = GetRadWindowManager();
                var oWnd = oManager.GetWindowByName("wndShowDepart");
                var url = 'PinReason.aspx?Id=' + EmpId + '&Rnd=' + Math.random();
                oWnd.SetUrl(url);
                oWnd.Show();
                return false;
            }

            function ShowDetail(EmpId) {
                var oManager = GetRadWindowManager();
                var oWnd = oManager.GetWindowByName("wndShowDepart");
                var url = 'RateDetail.aspx?cid=' + EmpId + '&Rnd=' + Math.random();
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
        </script>
    </telerik:RadCodeBlock>


<div style="background-color: #669999; width: auto; font-family: Arial, Helvetica, sans-serif; font-size: 7px; height: 251px;">

    <div style="visibility: hidden" runat="server" id="TopStats">

        <%--    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
                        <scripts>
                            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                                Name="Telerik.Web.UI.Common.Core.js">
                            </asp:ScriptReference>
                            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                                Name="Telerik.Web.UI.Common.jQuery.js">
                            </asp:ScriptReference>
                            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                                Name="Telerik.Web.UI.Common.jQueryInclude.js">
                            </asp:ScriptReference>
                        </scripts>
                    </telerik:RadScriptManager>--%>


        <table style="width: 100%; font-family: Arial, Helvetica, sans-serif; font-size: 8px; font-weight: bold; color: #FFFFFF;">

            <tr>
                <td></td>
                <td style="float: left">AirOps:<asp:Label ID="lblcarrier" runat="server" Text=""></asp:Label></td>
                <td></td>
            </tr>
            <tr>
                <td>Revenue Leg Expense</td>
                <td>
                    <asp:Label ID="lblrevenueexpense" runat="server" Text=""></asp:Label></td>


                <td>Non Revenue Leg Expense</td>
                <td>
                    <asp:Label ID="lblnonrevenueexpense" runat="server" Text=""></asp:Label></td>

                <td>Average Empty Leg NM</td>
                <td>
                    <asp:Label ID="lblAverageEmtpyNM" runat="server" Text=""></asp:Label></td>

                <td>Total Expense</td>
                <td>
                    <asp:Label ID="lbltotalrevenueexpense" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>


                <td>Revenue Legs</td>
                <td style="float: left">
                    <asp:Label ID="lblrevenuelegs" runat="server" Text=""></asp:Label></td>



                <td>Non Revenue Legs</td>
                <td style="float: left">
                    <asp:Label ID="lblnonrevenuelegs" runat="server" Text=""></asp:Label></td>


                <td>Shortest Empty Leg NM</td>
                <td style="float: left">
                    <asp:Label ID="lblShortestEmtpyLeg" runat="server" Text=""></asp:Label></td>


                <td>Line Breaks</td>
                <td>
                    <asp:Label ID="lblLineBreaks" runat="server" Text=""></asp:Label></td>

            </tr>
            <tr>

                <td>Revenue Miles</td>
                <td style="float: left">
                    <asp:Label ID="lblrevenuemiles" runat="server" Text=""></asp:Label></td>


                <td>Non Revenue Miles</td>
                <td style="float: left">
                    <asp:Label ID="lblnonrevenuemiles" runat="server" Text=""></asp:Label></td>



                <td>Longest EmptyLeg NM</td>
                <td style="float: left">
                    <asp:Label ID="lblLongestEmptyLeg" runat="server" Text=""></asp:Label></td>



                <td>Efficiency</td>
                <td style="float: left">
                    <asp:Label ID="lblefficiency" runat="server" Text=""></asp:Label></td>
            </tr>
        </table>




        <table>

            <tr style="font-size: 9px; color: #FFFFFF;">
                <td align="right">N Numbers 
                    <asp:DropDownList ID="DropDownNNumbers" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>

                <td align="right">Trip Numbers 
                    <asp:DropDownList ID="DropDownTripNumbers" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>


                <td align="right">Origin Airports 
                    <asp:DropDownList ID="DropDownOriginAirports" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>

                <td align="right">Dest Airports 
                    <asp:DropDownList ID="DropDownDestAirports" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>

                <td align="right">Fleet Type 
                    <asp:DropDownList ID="DropDownFleetType" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </td>
            </tr>


        </table>
        <table>
            <tr>
                <td>
                    <asp:CheckBox ID="chkLocal" runat="server" AutoPostBack="True" Font-Size="8pt" />
                <td>
                    <asp:LinkButton Font-Size="8pt" ID="LinkPending" runat="server">Show Local Time</asp:LinkButton>
                </td>




                <td>
                    <asp:CheckBox ID="chkGMT" runat="server" AutoPostBack="True" Checked="True" Font-Size="8pt" />




                <td>
                    <asp:LinkButton ID="LinkGMT" runat="server" Font-Size="8pt">Show GMT Time</asp:LinkButton>
                </td>
                <td>
                    <asp:Button ID="cmdRefresh" runat="server" Text="Refresh" Visible="False" Font-Size="8pt" />
                </td>
            </tr>

        </table>

    </div>

    <asp:GridView ID="GridViewTrips" runat="server" BackColor="White"
        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
        Font-Names="Arial" Font-Size="X-Small"
        Width="851px" Height="120px" Style="margin-top: 8px">
        <Columns>
            <asp:BoundField DataField="From" HeaderText="From" SortExpression="From" />
            <asp:BoundField DataField="To" HeaderText="To" SortExpression="To" />
            <asp:BoundField DataField="DateTimeGMT" HeaderText="From GMT" SortExpression="DateTimeGMT" />
            <asp:BoundField DataField="ToDateTimeGMT" HeaderText="To GMT"  SortExpression="ToDateTimeGMT" />
            <asp:BoundField DataField="DepartureDate" HeaderText="From Local"  SortExpression="DepartureDate" Visible="false" />
            <asp:BoundField DataField="ArrivalDate" HeaderText="To Local" Visible="False"   SortExpression="ArrivalDate" />
            <asp:BoundField DataField="Minutes" HeaderText="Min." SortExpression="Minutes" Visible="false" />
            <asp:BoundField DataField="NM" HeaderText="NM" SortExpression="NM" />
            <asp:BoundField DataField="AC" HeaderText="AC" SortExpression="AC" />
            <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" />
                  <asp:TemplateField HeaderText="Cost">
                   <ItemTemplate>
                       <asp:LinkButton ID="lnkRateDetail" Text='<%#Eval("Cost")%>' DataFormatString="{0:C0}" OnClientClick='<%#Eval("id", "return ShowDetail({0});")%>' runat="server">
                       </asp:LinkButton>
                   </ItemTemplate>
               </asp:TemplateField>

        <%-- %>    <asp:BoundField DataField="Cost" HeaderText="Cost" SortExpression="Cost" /> --%>
            <asp:BoundField DataField="DeadHead" HeaderText="FT" SortExpression="DeadHead" />
            <asp:BoundField DataField="TripNumber" HeaderText="Trip#" SortExpression="TripNumber" />
            <asp:TemplateField HeaderText="Pin">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkDepartmentClientSide" Text='PIN'
                        OnClientClick='<%#Eval("id", "return ShowPIN({0});")%>' runat="server">
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="AOG">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkDepartmentClientSide" Text='AOG'
                        OnClientClick='<%#Eval("id", "return ShowAOG({0});")%>' runat="server">
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="LRC" HeaderText="LRC" SortExpression="LRC" />
            <asp:BoundField DataField="LPC" HeaderText="LPC" SortExpression="LPC" />
            <asp:BoundField DataField="LTC" HeaderText="LTC" SortExpression="LTC" />
            <asp:BoundField DataField="PIC" HeaderText="PIC" SortExpression="PIC" />
            <asp:BoundField DataField="SIC" HeaderText="SIC" SortExpression="SIC" />
      <%--       <asp:BoundField DataField="triprevenue" HeaderText="Revenue" SortExpression="triprevenue" Visible="false" DataFormatString="{0:C0}"  />
            <asp:BoundField DataField="PandL" HeaderText="P&L" SortExpression="PandL" Visible="false" DataFormatString="{0:C0}" /> 
            <asp:BoundField DataField="base" HeaderText="Base" SortExpression="base" Visible="true"/>
            <asp:BoundField DataField="AWC" HeaderText="WC" SortExpression="AWC" Visible ="true"/>
            <asp:BoundField DataField="HA" HeaderText="HA" SortExpression="HA" Visible ="true"/> --%>
                <asp:TemplateField HeaderText="Revenue" SortExpression="triprevenue" Visible="false"> 
                   <ItemTemplate>
                       <asp:Label ID="revenue" runat="server" Text='<%# Eval("triprevenue")%>'></asp:Label>
                   </ItemTemplate>
               </asp:TemplateField>
               <asp:TemplateField HeaderText="P&L" SortExpression="PandL" Visible="false">
                   <ItemTemplate>
                       <asp:Label ID="pandl" runat="server" Text='<%# Eval("pandl")%>'></asp:Label>
                   </ItemTemplate>
               </asp:TemplateField>
               <asp:TemplateField HeaderText="Base" SortExpression="base" Visible="false">
                   <ItemTemplate>
                       <asp:Label ID="Base" runat="server" Text='<%# Eval("base")%>'></asp:Label>
                   </ItemTemplate>
               </asp:TemplateField>
               <asp:TemplateField HeaderText="WC" SortExpression="AWC" Visible="false">
                   <ItemTemplate>
                       <asp:Label ID="WC" runat="server" Text='<%# Eval("AWC")%>'></asp:Label>
                   </ItemTemplate>
               </asp:TemplateField>
                 <asp:TemplateField HeaderText="HA" SortExpression="HA" Visible="false">
                   <ItemTemplate>
                       <asp:Label ID="HA" runat="server" Text='<%# If(((Eval("HA").ToString() Is Nothing Or Eval("HA").ToString() = "") And Eval("base").ToString() <> "BKR"), "FLOAT", Eval("HA")) %>'></asp:Label>
                   </ItemTemplate>
               </asp:TemplateField>
                             <asp:TemplateField HeaderText="OE" SortExpression="OE" Visible="false">
                   <ItemTemplate>
                       <asp:Label ID="OE" runat="server" Text='<%# Eval("OE")%>'></asp:Label>
                   </ItemTemplate>
               </asp:TemplateField>
  </Columns>



        <FooterStyle BackColor="White" ForeColor="#000066" />
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
        <RowStyle ForeColor="#000066" Font-Size="X-Small" />
        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" />
        <SortedAscendingHeaderStyle BackColor="#007DBB" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#00547E" />
    </asp:GridView>

    <br />

</div>



<telerik:RadWindowManager ID="radWinMgr" runat="server" Behaviors="Close, Move"
    VisibleStatusbar="False" ShowContentDuringLoad="False" Behavior="Close, Move" Skin="MetroTouch">
    <Windows>
        <telerik:RadWindow ID="wndShowDepart" runat="server" Height="380px" Width="300px" Modal="true"
            Title="Accept/Reject" NavigateUrl="" OnClientClose="onWindowClose" Animation="Resize" Skin="MetroTouch">
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>


