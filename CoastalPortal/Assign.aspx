<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Assign.aspx.vb" Inherits="CoastalPortal.Assign" %>
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
        <telerik:RadGrid RenderMode="Lightweight" ID="RadGrid1" OnPreRender="RadGrid1_PreRender" ShowStatusBar="true"
            DataSourceID="SqlDataSource1" runat="server" AutoGenerateColumns="False" PageSize="7"
            AllowSorting="True" AllowMultiRowSelection="False" AllowPaging="True" GridLines="None">
            <PagerStyle Mode="NumericPages"></PagerStyle>
            <MasterTableView EnableHierarchyExpandAll="true" DataSourceID="SqlDataSource1" DataKeyNames="CustomerID" AllowMultiColumnSorting="True">
                <DetailTables>
                    <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="OrderID" DataSourceID="SqlDataSource2" Width="100%"
                        runat="server">
                        <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="CustomerID" MasterKeyField="CustomerID"></telerik:GridRelationFields>
                        </ParentTableRelation>
                        <DetailTables>
                            <telerik:GridTableView EnableHierarchyExpandAll="true" DataKeyNames="OrderID" DataSourceID="SqlDataSource3" Width="100%"
                                runat="server">
                                <ParentTableRelation>
                                    <telerik:GridRelationFields DetailKeyField="OrderID" MasterKeyField="OrderID"></telerik:GridRelationFields>
                                </ParentTableRelation>
                                <Columns>
                                    <telerik:GridBoundColumn SortExpression="UnitPrice" HeaderText="Unit Price" HeaderButtonType="TextButton"
                                        DataField="UnitPrice" UniqueName="UnitPrice">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="Quantity" HeaderText="Quantity" HeaderButtonType="TextButton"
                                        DataField="Quantity" UniqueName="Quantity">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn SortExpression="Discount" HeaderText="Discount" HeaderButtonType="TextButton"
                                        DataField="Discount" UniqueName="Discount">
                                    </telerik:GridBoundColumn>
                                </Columns>
                                <SortExpressions>
                                    <telerik:GridSortExpression FieldName="Quantity" SortOrder="Descending"></telerik:GridSortExpression>
                                </SortExpressions>
                            </telerik:GridTableView>
                        </DetailTables>
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="OrderID" HeaderText="OrderID" HeaderButtonType="TextButton"
                                DataField="OrderID" UniqueName="OrderID">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="OrderDate" HeaderText="Date Ordered" HeaderButtonType="TextButton"
                                DataField="OrderDate" UniqueName="OrderDate" DataFormatString="{0:D}">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn SortExpression="Freight" HeaderText="Freight" HeaderButtonType="TextButton"
                                DataField="Freight" UniqueName="Freight">
                            </telerik:GridBoundColumn>
                        </Columns>
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="OrderDate"></telerik:GridSortExpression>
                        </SortExpressions>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn SortExpression="CustomerID" HeaderText="CustomerID" HeaderButtonType="TextButton"
                        DataField="CustomerID" UniqueName="CustomerID">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="ContactName" HeaderText="Contact Name" HeaderButtonType="TextButton"
                        DataField="ContactName" UniqueName="ContactName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="CompanyName" HeaderText="Company" HeaderButtonType="TextButton"
                        DataField="CompanyName" UniqueName="CompanyName">
                    </telerik:GridBoundColumn>
                </Columns>
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="CompanyName"></telerik:GridSortExpression>
                </SortExpressions>
            </MasterTableView>
        </telerik:RadGrid>
    <asp:SqlDataSource ID="SqlDataSource1" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT ID as CustomerID, description as ContactName, quotenumber as ContactName, status as CompanyName, * FROM OptimizerRequest where (DemandFlights = 1 or ParentRequestNumber > 0)"
        runat="server"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT modelrun as OrderID, PriorTailNumber as ContactName, * FROM FCDRList Where CustomerID = @CustomerID"
        runat="server">
        <SelectParameters>
            <asp:SessionParameter Name="CustomerID" SessionField="CustomerID" Type="string"></asp:SessionParameter>
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource3" ConnectionString="<%$ ConnectionStrings:OptimizerDB %>"
        ProviderName="System.Data.SqlClient" SelectCommand="SELECT modelrun as OrderID, TripNumber as UnitPrice, From_ICAO as Quantity, To_ICAO as Discount, * FROM FCDRListDetail where modelrun = @OrderID"
        runat="server">
        <SelectParameters>
            <asp:SessionParameter Name="OrderID" SessionField="OrderID" Type="Int32"></asp:SessionParameter>
        </SelectParameters>
    </asp:SqlDataSource>
    <%--<qsf:ConfiguratorPanel ID="ConfiguratorPanel1" runat="server" Title="Hierarchy Load Mode">
        <Views>
            <qsf:View>
                <qsf:RadioButtonList ID="hierarchyLoadMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="hierarchyLoadMode_SelectedIndexChanged">
                    <asp:ListItem Text="Server"></asp:ListItem>
                    <asp:ListItem Text="ServerOnDemand" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Conditional"></asp:ListItem>
                    <asp:ListItem Text="Client"></asp:ListItem>
                </qsf:RadioButtonList>
            </qsf:View>
        </Views>
    </qsf:ConfiguratorPanel>--%>
    </form>
</body>
</html>
