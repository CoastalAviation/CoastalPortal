<%@ Control Language="vb" AutoEventWireup="false" Inherits="Optimizer.OD123" Codebehind="OptimizerRunDetail.ascx.vb" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<%@ Register TagPrefix="eo" Namespace="EO.Web" Assembly="EO.Web" %>

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
   



   <div style="background-color: #669999; width: 1228px;">

       
       <br />
       <asp:LinkButton ID="LinkRefresh" runat="server" Font-Names="Arial Black">Refresh Detail</asp:LinkButton><asp:Label ID="lblmsg" runat="server" Text=" "></asp:Label>
               <br />
       <br />
       
       


       <telerik:RadGrid ID="GridView3" runat="server" AllowSorting="True" AutoGenerateColumns="False" Skin="WebBlue">
<GroupingSettings CollapseAllTooltip="Collapse all groups"></GroupingSettings>

                    <ClientSettings>
               <Selecting AllowRowSelect="True" />
                        <Animation AllowColumnReorderAnimation="True" AllowColumnRevertAnimation="True" />
           </ClientSettings>
<MasterTableView>
<CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

<RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
<HeaderStyle Width="20px"></HeaderStyle>
</RowIndicatorColumn>

<ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
<HeaderStyle Width="20px"></HeaderStyle>
</ExpandCollapseColumn>

    <Columns>


     

        <telerik:GridBoundColumn DataField="CasTotalRevenueExpense" DataType="System.Double" FilterControlAltText="Filter CasTotalRevenueExpense column" HeaderText="Cas Total Revenue Expense" SortExpression="CasTotalRevenueExpense" UniqueName="CasTotalRevenueExpense">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="DeltaNRM" DataType="System.Double" FilterControlAltText="Filter DeltaNRM column" HeaderText="DeltaNRM" SortExpression="DeltaNRM" UniqueName="DeltaNRM">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="EffCAS1" FilterControlAltText="Filter EffCAS1 column" HeaderText="D1 CAS Eff" UniqueName="EffCAS1"  DataFormatString="{0:P}" >
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="Efffos1" FilterControlAltText="Filter Efffos1 column" HeaderText="D1 FOS Eff" UniqueName="Efffos1"  DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="EffCAS2" FilterControlAltText="Filter EffCAS2 column" HeaderText="D2 CAS Eff" UniqueName="EffCAS2" DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="Efffos2" FilterControlAltText="Filter Efffos2 column" HeaderText="D2 FOS Eff" UniqueName="Efffos2" DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="EffCAS3" FilterControlAltText="Filter EffCAS3 column" HeaderText="D3 CAS Eff" UniqueName="EffCAS3" DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
         <telerik:GridBoundColumn DataField="Efffos3" FilterControlAltText="Filter Efffos3 column" HeaderText="D3 FOS Eff" UniqueName="Efffos3" DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="EffCAS4" FilterControlAltText="Filter EffCAS4 column" HeaderText="D4 CAS Eff" UniqueName="EffCAS4" DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="Efffos4" FilterControlAltText="Filter Efffos4 column" HeaderText="D4 FOS Eff" UniqueName="Efffos4" DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="DeltaExpense1" DataType="System.Double" FilterControlAltText="Filter DeltaExpense1 column" HeaderText="Day 1 Expense" SortExpression="DeltaExpense1" UniqueName="DeltaExpense1">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="DeltaExpense2" DataType="System.Double" FilterControlAltText="Filter DeltaExpense2 column" HeaderText="Day 2 Expense" SortExpression="DeltaExpense2" UniqueName="DeltaExpense2">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="DeltaExpense3" DataType="System.Double" FilterControlAltText="Filter DeltaExpense3 column" HeaderText="Day 3 Expense" SortExpression="DeltaExpense3" UniqueName="DeltaExpense3">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="DeltaExpense4" DataType="System.Double" FilterControlAltText="Filter DeltaExpense4 column" HeaderText="Day 4 Expense" SortExpression="DeltaExpense4" UniqueName="DeltaExpense4">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="CASefficiency" DataType="System.Double" FilterControlAltText="Filter CASefficiency column" HeaderText="CASefficiency" SortExpression="CASefficiency" UniqueName="CASefficiency" DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn DataField="FOSefficiency" DataType="System.Double" FilterControlAltText="Filter FOSefficiency column" HeaderText="FOSefficiency" SortExpression="FOSefficiency" UniqueName="FOSefficiency" DataFormatString="{0:P}">
        </telerik:GridBoundColumn>
     
        <telerik:GridBoundColumn DataField="FOSlinebreaks" DataType="System.Int32" FilterControlAltText="Filter FOSlinebreaks column" HeaderText="FOSlinebreaks" SortExpression="FOSlinebreaks" UniqueName="FOSlinebreaks">
        </telerik:GridBoundColumn>
     
        <telerik:GridBoundColumn DataField="Viewed" FilterControlAltText="Filter Viewed column" HeaderText="Viewed" SortExpression="Viewed" UniqueName="Viewed">
        </telerik:GridBoundColumn>
     
        <telerik:GridHyperLinkColumn HeaderText="Model Number" DataTextField="ModelRunId" ItemStyle-HorizontalAlign="Left" UniqueName="ModelRunClick" HeaderStyle-Font-Bold="false" HeaderStyle-Wrap="false" DataNavigateUrlFields="ModelRunId" DataNavigateUrlFormatString="panel.aspx?modelrunid={0}" HeaderStyle-Width="150px" FilterControlAltText="Filter ModelRunClick column" AutoPostBackOnFilter="True">
<HeaderStyle Wrap="False" Font-Bold="False" Width="150px"></HeaderStyle>

<ItemStyle HorizontalAlign="Left"></ItemStyle>
        </telerik:GridHyperLinkColumn>

    </Columns>

<EditFormSettings>
<EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
</EditFormSettings>
</MasterTableView>

<FilterMenu EnableImageSprites="False"></FilterMenu>
       </telerik:RadGrid>

       
     
     
       
       <br />
       <br />
       
<br />
       <%--DataSourceID="SqlDataSource1--%>
       
       <telerik:RadHtmlChart ID="RadHtmlChart1" runat="server"  >
           <ChartTitle Text="Optimizer Runs">
           </ChartTitle>
       <PlotArea>

                <XAxis>
                    <Items>
                        <telerik:AxisItem LabelText="" />
                    <telerik:AxisItem LabelText="ModelrunId" /></Items>
                </XAxis>
                <Series>
                    <telerik:BarSeries DataFieldY="DeltaExpense" Name="Delta Expense">
                    </telerik:BarSeries>
                    
                </Series>

                </PlotArea>

            </telerik:RadHtmlChart>
     


       <br />


       <br />
                   
       <br />

       

							</div> <!-- end PanelProducts -->											  							

              

                                                
                              <table>              
                                                     
                   </table>                      
                                     
                                                    <br />
                      
                   
                                                   
                                                                              
                             


            


                                 

