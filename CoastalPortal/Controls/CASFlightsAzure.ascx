<%@ Control Language="vb" AutoEventWireup="false"   Inherits="CoastalPortal.CasFlightsAzure6" Codebehind="CASFlightsAzure.ascx.vb" %>
<%@ Import Namespace="System.Web.Services.Description" %>

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

    
        .auto-style1 {
            height: 27px;
        }

    
        .auto-style6 {
            width: 100%;
        }

    
        .auto-style9 {
            float: left;
            width: 142px;
        }
        .auto-style10 {
            width: 142px;
        }
        .auto-style11 {
            width: 146px;
        }

    
        .auto-style12 {
            height: 14px;
        }
        .auto-style14 {
            float: left;
            width: 151px;
            height: 14px;
        }
        .auto-style15 {
            float: left;
            width: 142px;
            height: 14px;
        }
        .auto-style16 {
            width: 146px;
            height: 14px;
        }
        .auto-style17 {
            width: 147px;
        }
        .auto-style18 {
            height: 14px;
            width: 147px;
        }
        .auto-style19 {
            float: left;
            width: 147px;
        }
        .auto-style20 {
            float: left;
            width: 146px;
        }
        .auto-style22 {
            float: left;
            width: 146px;
            height: 14px;
        }
        .auto-style23 {
            width: 151px;
        }
        .auto-style24 {
            float: left;
            width: 151px;
        }

    
        </style>


<telerik:RadCodeBlock ID="radCodeBlock" runat="server">
        <script type="text/javascript" language="javascript">
            var daaRecieved;

            function ShowHotel(EmpId) {
                var oManager = GetRadWindowManager();
                var oWnd = oManager.GetWindowByName("wndShowHotel");
                var url = 'Hotel.aspx?Id=' + EmpId + '&Rnd=' + Math.random();
                oWnd.SetUrl(url);
                oWnd.Show();
                return false;
            }

            function ShowDepart(EmpId) {
                var oManager = GetRadWindowManager();
                var oWnd = oManager.GetWindowByName("wndShowDepart");
                var url = 'AcceptRejectChange.aspx?Id=' + EmpId + '&Rnd=' + Math.random();
                oWnd.SetUrl(url);
                oWnd.Show();
                return false;
            }


            function ShowEmpty(EmpId) {
                var oManager = GetRadWindowManager();
                var oWnd = oManager.GetWindowByName("wndShowDepart");
                var url = 'PostForSale.aspx?Id=' + EmpId + '&Rnd=' + Math.random();
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



            function ShowPublish() {
                var url = 'PublishMe.aspx?Rnd=' + Math.random();
                var oWnd = window.radopen(url, "wndPublishMe", 450,600)
              //  oWnd.Show();
              //  return false;
            }

            function openCalendar(sender, args) {
                window.open(sender._navigateUrl,
                '_blank',
                'height=800,width=1000,location=no,' +
                'menubar=yes,resizable=yes,scrollbars=yes,' +
                'status=no,toolbar=no');
                args.set_cancel(true);
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
   

   

<script runat="server">

    Sub CustomersGridView_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)

        ' If multiple ButtonField column fields are used, use the
        ' CommandName property to determine which button was clicked.
        If e.CommandName = "Select" Then

            ' Convert the row index stored in the CommandArgument
            ' property to an Integer.
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)

            ' Get the last name of the selected author from the appropriate
            ' cell in the GridView control.
            Dim selectedRow As GridViewRow = GridViewTrips.Rows(index)
            Dim contactCell As TableCell = selectedRow.Cells(1)
            Dim contact As String = contactCell.Text

            ' Display the selected author.

            ' alert("You selected " & contact & ".")


        End If

    End Sub

</script>

   <div style="background-color: #669999; width: auto; font-family: Arial, Helvetica, sans-serif; font-size: 7px;">
             <div style="visibility: hidden" runat="server" id="TopStats">

    
   <table style="width:100%; font-family: Arial, Helvetica, sans-serif; font-size: 8px; font-weight: bold; color: #FFFFFF; ">

            <tr>  <td ></td> <td  style="float: left">CAS</td>  <td >
                <asp:Label ID="lblmodelid" runat="server" Text="Label"></asp:Label></td>  </tr>
            <tr>
                <td >Revenue Leg Expense</td>
                <td ><asp:Label ID="lblrevenueexpense" runat="server" Text=""></asp:Label></td>
                            

                 <td >Non Revenue Leg Expense</td>
                  <td > <asp:Label ID="lblnonrevenueexpense" runat="server" Text=""></asp:Label></td>
               
                <td >Average Empty Leg NM</td>
                <td > <asp:Label ID="lblAverageEmtpyNM" runat="server" Text=""></asp:Label></td>
                
                 <td >Total Expense</td>
                <td ><asp:Label ID="lbltotalrevenueexpense" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
              

                    <td >Revenue Legs</td>
                  <td  style="float: left"> <asp:Label ID="lblrevenuelegs" runat="server" Text=""></asp:Label></td>

               

                  <td >Non Revenue Legs</td>
                <td  style="float: left"><asp:Label ID="lblnonrevenuelegs" runat="server" Text=""></asp:Label></td>

                             
                 <td >Shortest Empty Leg NM</td>
                <td  style="float: left"><asp:Label ID="lblShortestEmtpyLeg" runat="server" Text=""></asp:Label></td>


                   <td >Line Breaks</td>
                <td ><asp:Label ID="lblLineBreaks" runat="server" Text=""></asp:Label></td>
                
            </tr>
            <tr>

                 <td >Revenue Miles</td>
                <td  style="float: left"><asp:Label ID="lblrevenuemiles" runat="server" Text=""></asp:Label></td>


                 <td >Non Revenue Miles</td>
                <td  style="float: left"><asp:Label ID="lblnonrevenuemiles" runat="server" Text=""></asp:Label></td>

              

                <td >Longest EmptyLeg NM</td>
                  <td  style="float: left"> <asp:Label ID="lblLongestEmptyLeg" runat="server" Text=""></asp:Label></td>

                

                  <td >Efficiency</td>
                <td style="float: left"><asp:Label ID="lblefficiency" runat="server" Text=""></asp:Label></td>
            </tr>
        </table>
                                                       
                                                 <table >

                                                    <tr style="font-size: 9px; color: #FFFFFF;" >
                                                            <td align="right" >
                                                                N Numbers<asp:DropDownList ID="DropDownNNumbers" runat="server" AutoPostBack="True">
                                                                </asp:DropDownList>     </td>
                                                                               
                                                                                 <td align="right" >
                                                                Trip Numbers<asp:DropDownList ID="DropDownTripNumbers" runat="server" AutoPostBack="True">
                                                                </asp:DropDownList>       </td>
                                                                                
                                                                                                                            
                                                             <td align="right" >
                                                                Origin Airports<asp:DropDownList ID="DropDownOriginAirports" runat="server" AutoPostBack="True">
                                                                </asp:DropDownList>       </td>
                                                               
                                                                                                                               <td align="right">
                                                                Dest Airports <asp:DropDownList ID="DropDownDestAirports" runat="server" AutoPostBack="True">
                                                                </asp:DropDownList>       </td>

                                                             <td align="right">
                                                                Fleet Type<asp:DropDownList ID="DropDownFleetType" runat="server" AutoPostBack="True">
                                                                </asp:DropDownList>     
                                                            </td>  

                                                               <td align="right">

                                                                 <telerik:RadButton ID="RBcalendar" runat="server"  Text="Calendar" BackColor="#FFFF66" Height="15px" Font-Size="9pt"
                                                                       ButtonType="LinkButton" Width="80px" Skin="MetroTouch" style="line-height: 15px; height: 15px; position: relative;" 
                                                                     Font-Bold="True" Font-Names="Arial"  ToolTip="Click Here to see Calendar" OnClientClicking="openCalendar" 
                                                                     AutoPostBack="False"  ViewStateMode="Disabled">
                                                                 </telerik:RadButton>                                                                         

                                                      </tr>



                                                                                                                                                       
                                         </table>
                                                       
                                                 <table   >
                                                      <tr>
                                                             <td class="auto-style1">
                                                                     <asp:CheckBox ID="chkLocal" runat="server" AutoPostBack="True" Font-Size="8pt" Height="15px"  />
                                                             <td class="auto-style1">
                                                                  <asp:LinkButton ID="LinkPending" runat="server" Font-Size="8pt">Local</asp:LinkButton>
                                                            </td>   <td class="auto-style1">
                                                                   <asp:CheckBox ID="chkGMT" runat="server" AutoPostBack="True" Font-Size="8pt" Checked="True"  />
                                                                   <td class="auto-style1">
                                                                    <asp:LinkButton ID="LinkGMT" runat="server" Font-Size="8pt">GMT</asp:LinkButton>
                                                                                                                                     </td>
<td class="auto-style1">
                                                         <asp:Button ID="cmdRefresh" runat="server" Text="Refresh" Visible="False" />
                                                          </td>  
                                                          <td class="auto-style1">
                                                                 <telerik:RadButton ID="radbtnAcceptAll" runat="server"  Text="Accept All" BackColor="#33CC33" Height="15px" Font-Size="10pt"
                                                                      OnClientClicked ="ShowPublish" ButtonType="LinkButton" Width="100px" Skin="MetroTouch" style="line-height: 15px; height: 15px; position: relative;" BorderWidth="2px" Font-Bold="True" Font-Names="Arial"  ToolTip="Click Here to Publish Schedule" AutoPostBack="False" >
                                                                 </telerik:RadButton>                                                                          
                                                        
                                                          </td>  

                                                            <td class="auto-style1">
                                                         <asp:Button ID="cmdSlideReport" runat="server" Text="SlideReport" Visible="True" BackColor="Aqua" Height="20px"   />
                                                          </td> 
                                                               <td class="auto-style1">
                                                         <asp:Button ID="cmdRecovery" runat="server" Text="Show Base Model" Visible="True" Height="20px" />
                                                          </td>  

                                                            <td class="auto-style1">
                                                         <asp:Button ID="cmdRemoveDH" runat="server" Text="Remove DH" Visible="True" Height="20px" />
                                                          </td>  

                                                          
                                                            <td class="auto-style1">
                                                         <asp:Button ID="cmdScrubDH" runat="server" Text="Scrub DH" Visible="True" Height="20px" />
                                                          </td>  


                                                           <td class="auto-style1">
                                                               <asp:Label ID="lblmsg" runat="server" Text=" "></asp:Label>
                                                          </td>
                                                                                                                  <td align="right">
                                                                 <telerik:RadLinearGauge ID="modelstatusguage" runat="server" Height="15px" Width="150px" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Skin="Office2010Black">
                                                                     <Pointer Color="Black" Size="5">
                                                                     </Pointer>
                                                                     <Scale Vertical="False">
                                                                         <Labels Color="Black" Position="Outside" />
                                                                         <MinorTicks Visible="False" />
                                                                         <MajorTicks Color="Black" Width="2" />
                                                                         <Ranges>
                                                                             <telerik:GaugeRange Color="Red" To="100" />
                                                                         </Ranges>
                                                                     </Scale>
                                                                 </telerik:RadLinearGauge>
                                                            </td>
                                                       <td>
                                                        <asp:Label runat="server" ID="lblFlightsinDemand" height="20px" width="150px" Text="" style="font-family: Arial, Helvetica, sans-serif; font-size: 10px; font-weight: bold; color: #FFFFFF"/>
                                                       </td>



                                                       <%--   <td><asp:LinkButton ID="ExportToExcel" runat="server">Export to Excel</asp:LinkButton></td>
                                                        --%> </tr>
                                                    </table>
                                                
                                                
                                                    
                                                    <div style="display:none;visibility:hidden;">
                                                        <asp:HiddenField ID="hddnSearchMembers" runat="server" Value="0" />
                                                    </div>
                        </div>




       <asp:GridView ID="GridViewTrips" runat="server" BackColor="White" onPreRender="GridViewTrips_PreRender1"
           BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
           Font-Names="Arial" Font-Size="X-Small" Width="933px" Style="margin-top: 0px"  >


           <Columns>
               <asp:BoundField DataField="From" HeaderText="From" SortExpression="From" />
               <asp:BoundField DataField="To" HeaderText="To" SortExpression="To" />
               <asp:BoundField DataField="From GMT" HeaderText="From GMT" SortExpression="From GMT" />
               <asp:BoundField DataField="To GMT" HeaderText="To GMT" />
               <asp:BoundField DataField="From Local" HeaderText="From Local" SortExpression="From Local" Visible="False"></asp:BoundField>
               <asp:BoundField DataField="To Local" HeaderText="To Local" Visible="False"></asp:BoundField>
               <asp:BoundField DataField="Minutes" HeaderText="Min." SortExpression="Minutes" />
               <asp:BoundField DataField="NM" HeaderText="NM" SortExpression="NM" />
               <asp:BoundField DataField="AC" HeaderText="AC" SortExpression="AC" />
               <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" />
              <%-- <asp:BoundField DataField="Cost" HeaderText="Cost" SortExpression="Cost" />--%>
                 <asp:TemplateField HeaderText="Cost">
                   <ItemTemplate>
                       <asp:LinkButton ID="lnkRateDetail" Text='<%#Eval("Cost")%>' DataFormatString="{0:C0}" OnClientClick='<%#Eval("id", "return ShowDetail({0});")%>' runat="server">
                       </asp:LinkButton>
                   </ItemTemplate>
               </asp:TemplateField>

               <asp:TemplateField HeaderText="FT">
                   <ItemTemplate>
                       <asp:LinkButton ID="lnkEmptyClientSide" Text='<%#Eval("FT")%>' OnClientClick='<%#Eval("id", "return ShowEmpty({0});")%>' runat="server">
                       </asp:LinkButton>
                   </ItemTemplate>
               </asp:TemplateField>
               <asp:BoundField DataField="TripNumber" HeaderText="Trip#"  SortExpression="TripNumber" />
               <%--      <asp:BoundField DataField="PriorTail" HeaderText="Prior" 
                                    SortExpression="PriorTail" />--%>

               <%--      <asp:TemplateField HeaderText="AR">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkDepartmentServer" DataField="PriorTail" Text='<%#Eval("Id")%>' runat="server"
                            CommandName="showPopUp">
                        </asp:LinkButton>
                    </ItemTemplate>
                   </asp:TemplateField>--%>

               <%--      <asp:BoundField DataField="PriorTail" HeaderText="Prior" 
                                    SortExpression="PriorTail" Visible="False" />--%>

               <asp:TemplateField HeaderText="Ind">
                   <ItemTemplate>
                       <asp:LinkButton ID="lnkPTSindividual" Text='<%#Eval("PriorTail")%>' OnClientClick='<%#Eval("id", "return ShowDepart({0});")%>' runat="server">
                       </asp:LinkButton>
                   </ItemTemplate>
               </asp:TemplateField>

               <asp:TemplateField HeaderText="Grp">
                   <ItemTemplate>
                       <asp:LinkButton ID="lnkDepartmentClientSide" Text='<%#Eval("PriorTail")%>'
                           runat="server" PostBackUrl='<%#String.Format("flightchangesNew.aspx?Key={0}", DataBinder.Eval(Container, "DataItem.ID"))%>' OnClientClick="form1.target='_blank'">
                       </asp:LinkButton>
                   </ItemTemplate>
               </asp:TemplateField>

               <%--  <asp:TemplateColumn HeaderText="Name">
           <ItemTemplate>
           <a href = "#" title="click to view detail" onclick="return jsPop('win_indvinfo11.aspx?ID=<%# DataBinder.Eval (Container.DataItem,"ID")%>', 'win01', 700, 600);">
           <%# DataBinder.Eval(Container.DataItem, "Name")%></a>
           </ItemTemplate>
           </asp:TemplateColumn>--%>

               <%-- <asp:TemplateField HeaderText="PriorTail" SortExpression="PriorTail">
                      <EditItemTemplate>
                          <asp:TextBox ID="TextBox2" runat="server"
Text='<%# Bind("PriorTail")%>'></asp:TextBox>
                      </EditItemTemplate>
                      <ItemTemplate>
                          <asp:Label ID="Label3" runat="server"
Text='<%# Bind("PriorTail")%>'></asp:Label>
                      </ItemTemplate>
                      <FooterTemplate>
                          <asp:TextBox ID="DescriptionTextBox" runat="server"></asp:TextBox>
                      </FooterTemplate>
                  </asp:TemplateField>
                  <asp:TemplateField>
                     <FooterTemplate>
                          <asp:LinkButton ID="btnNew" runat="server"
CommandName="New" Text="New" />
                      </FooterTemplate>
                  </asp:TemplateField>--%>

               <asp:BoundField DataField="LRC" HeaderText="LRC" SortExpression="LRC" />
               <asp:BoundField DataField="LPC" HeaderText="LPC" SortExpression="LPC" />
               <asp:BoundField DataField="LTC" HeaderText="LTC" SortExpression="LTC" />
               <%-- <asp:BoundField DataField="PIC" HeaderText="PIC" SortExpression="PIC" />--%>
               <asp:TemplateField HeaderText="PIC">
                   <ItemTemplate>
                       <asp:LinkButton ID="lnkPICClientSide" Text='<%#Eval("PIC")%>'
                           OnClientClick='<%#Eval("id", "return ShowHotel({0});")%>' runat="server">
                            
                       </asp:LinkButton>
                   </ItemTemplate>
               </asp:TemplateField>


               <asp:BoundField DataField="SIC" HeaderText="SIC" SortExpression="SIC" />
               <asp:BoundField DataField="CrewDutyFlightTime" HeaderText="CrewDutyFlightTime" SortExpression="CrewDutyFlightTime" Visible="False" />
               <asp:BoundField DataField="CrewDutyWindow" HeaderText="CrewDutyWindow" SortExpression="CrewDutyWindow" Visible="False" />
               <asp:BoundField DataField="CrewDutyComment" HeaderText="CrewDutyComment" SortExpression="CrewDutyComment" Visible="False" />
               <%--     <asp:ButtonField CommandName="Edit" HeaderText="PT" ShowHeader="True" Text="PT" />
               <asp:BoundField DataField="triprevenue" HeaderText="Revenue" SortExpression="triprevenue" Visible="false" DataFormatString="{0:C0}" />
               <asp:BoundField DataField="PandL" HeaderText="P&L" SortExpression="PandL" Visible="false" DataFormatString="{0:C0}" />
               <asp:BoundField DataField="base" HeaderText="Base" SortExpression="base" Visible="true" /> --%>
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
               <%-- %>  <asp:BoundField DataField="AWC" HeaderText="WC" SortExpression="AWC" Visible ="true"/> --%>
               <asp:BoundField DataField="id"
                   Visible="False" />

               <asp:ButtonField CommandName="X" HeaderText=" Pin  "
                   ShowHeader="True" Text="Pin" ItemStyle-ForeColor="#009999" ButtonType="Link">
                   <ControlStyle Width="30px" />
                   <HeaderStyle Width="30px" />
                   <ItemStyle Width="30px" ForeColor="#009999"></ItemStyle>
               </asp:ButtonField>

                <asp:BoundField DataField="Pinned" HeaderText="PR" SortExpression="Pinned" />

           </Columns>

           <FooterStyle BackColor="White" ForeColor="#000066" />
           <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
           <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
           <RowStyle ForeColor="#000066" />
           <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
           <SortedAscendingCellStyle BackColor="#F1F1F1" />
           <SortedAscendingHeaderStyle BackColor="#007DBB" />
           <SortedDescendingCellStyle BackColor="#CAC9C9" />
           <SortedDescendingHeaderStyle BackColor="#00547E" />
       </asp:GridView>










       <br />
                      
                   
                                                                              
                             


            </div>

           

<%-- <asp:BoundField DataField="Cost" HeaderText="Cost" SortExpression="Cost" />--%>




<telerik:RadWindowManager ID="radWinMgr" runat="server" Behaviors="Close, Move" Modal="True" Animation="Resize" AutoSize ="True" AutoSizeBehaviors ="Height, Width"
        VisibleStatusbar="False" ShowContentDuringLoad="False" Behavior="Close, Move" Skin="MetroTouch">
        <Windows>
            <telerik:RadWindow ID="wndPublishMe" runat="server"  AutoSize="True"  OnClientClose="onWindowClose" AutoSizeBehaviors ="Height, Width"
                     Title="Publish Schedule" Modal="true" Animation="Resize" Skin="MetroTouch">
            </telerik:RadWindow>
            <telerik:RadWindow ID="wndShowDepart" runat="server" autosize="true" Modal="true" 
                     Title="Accept/Reject" NavigateUrl="" OnClientClose="onWindowClose" Animation="Resize" Skin="MetroTouch">
            </telerik:RadWindow>
             <telerik:RadWindow ID="wndShowRate" runat="server" autosize="true" Modal="true" 
                     Title="Rate Detail" NavigateUrl="" OnClientClose="onWindowClose" Animation="Resize" Skin="MetroTouch">
            </telerik:RadWindow>
            <telerik:RadWindow runat="server" Behaviors="Close, Move" Behavior="Close, Move" Animation="Resize" Width="300px" Height="380px" 
                     Title="Book Hotels" ShowContentDuringLoad="False" VisibleStatusbar="False" Modal="True" OnClientClose="onWindowClose" Skin="MetroTouch" ID="wndShowHotel">

            </telerik:RadWindow>
        </Windows>


   
    </telerik:RadWindowManager>
                                 
