<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm3.aspx.vb" Inherits="CoastalPortal.WebForm3" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%--<%@ Register TagPrefix="qsf" Namespace="Telerik.QuickStart" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns='http://www.w3.org/1999/xhtml'>
<head runat="server">
    <title>Telerik ASP.NET Example</title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" ShowChooser="False" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
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
                    <telerik:GridBoundColumn SortExpression="modelrun" HeaderText="model run" HeaderButtonType="TextButton"
                        DataField="modelrun" UniqueName="modelrun">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="Description" HeaderText="Description" HeaderButtonType="TextButton"
                        DataField="Description" UniqueName="Description">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="GMTStart" HeaderText="GMT Start" HeaderButtonType="TextButton"
                        DataField="GMTStart" UniqueName="GMTStart" DataFormatString="{0:D}">
                    </telerik:GridBoundColumn>
                </Columns>
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="GMTStart"></telerik:GridSortExpression>
                </SortExpressions>
            </MasterTableView>
        </telerik:RadGrid>
    <asp:SqlDataSource ID="SqlDataSource1" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT distinct ID as modelrun,Description,r.GMTStart FROM OptimizerRequest r join FCDRList l on r.ID = l.modelrun where ParentRequestNumber > 0 and r.GMTStart >= DATEADD(d,-2,r.GMTStart) order by id desc"
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
    </form>
</body>
</html>
