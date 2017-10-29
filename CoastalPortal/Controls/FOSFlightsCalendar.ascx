<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="FOSFlightsCalendar.ascx.vb" Inherits="CoastalPortal.FOSFlightsCalendar" %>

    <script language="javascript" type="text/javascript" >
        var mouseX;
        var mouseY;
        var winW;
        var winH;
        var isIE = document.all ? true : false;

        //GET MOUSE POSITION	
        if (!isIE) document.captureEvents(Event.MOUSEMOVE);

        document.onmousemove = getMousePosition;

        function getMousePosition(mp) {
            var _x;
            var _y;

            var _w;
            var _h;

            if (!isIE) {
                _x = mp.pageX;
                _y = mp.pageY;

                _w = window.innerWidth;
                _h = window.innerHeight;
            }
            if (isIE) {
                try {
                    _x = event.clientX + document.body.scrollLeft;
                    _y = event.clientY + document.body.scrollTop;
                }
                catch (e) {
                    _x = event.clientX;
                    _y = event.clientY;
                }
                try {
                    _w = document.body.clientWidth;
                    _h = document.body.clientHeight;
                }
                catch (e) {

                }
            }
            mouseX = _x;
            mouseY = _y;

            winW = _w;
            winH = _h;

            //window.status = winW + ' ' + winH;
            //window.status = mouseX + ' ' + mouseY;
            return true;
        }
        function getScrollXY() {
            var scrOfX = 0, scrOfY = 0;
            if (typeof (window.pageYOffset) == 'number') {
                //Netscape compliant
                scrOfY = window.pageYOffset;
                scrOfX = window.pageXOffset;
            } else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
                //DOM compliant
                scrOfY = document.body.scrollTop;
                scrOfX = document.body.scrollLeft;
            } else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
                //IE6 standards compliant mode
                scrOfY = document.documentElement.scrollTop;
                scrOfX = document.documentElement.scrollLeft;
            }
            mouseX = scrOfX;
            mouseY = scrOfY;
            return true; //[ scrOfX, scrOfY ];
        }
        function cancelBubbleUp() {
            var e = window.event;
            // handle event
            e.cancelBubble = true;
            if (e.stopPropagation) e.stopPropagation();
        }
    </script>

    <style type="text/css">
    body,td,th {
	    font-family: Arial, Helvetica, sans-serif;
	    
    }
    body
    {
        margin:0px;
        font-family: Arial, Helvetica, sans-serif;
        font-size: 12px;
        /*background-image:url(Images/SATSair/bg_repeat.gif);*/
        /*background-repeat:repeat-x;*/
        /*background-color:#FAFAFA;*/
    }
    </style>
    <link href="../StyleSheet.css" rel="stylesheet" type="text/css" />
    <link href="../Style.css" rel="stylesheet" type="text/css" />

<%--<div style="background-color: #669999; width: 1575px;">--%>
<div style="background-color: #669999; width: 100%;">
    <table>
        <tr>
            <td align="center" style="width: 25%">
                Time Zone:&nbsp;&nbsp;
                <%--<asp:DropDownList ID="ddlTimeZone" runat="server" AutoPostBack="True"></asp:DropDownList>--%>
                <telerik:RadComboBox ID="rcTimeZone" runat="server" EmptyMessage="Select Time Zone">
                    <%--<Items>
                        <telerik:RadComboBoxItem Owner="rcTimeZone" Text="GMT" ToolTip="GMT" Value="GMT" />
                    </Items>--%>
                </telerik:RadComboBox>
                <%--<br />
                <br />
                Calendar Style:&nbsp;&nbsp;
                <asp:DropDownList ID="ddlStyle" runat="server">
                    <asp:ListItem Selected="True">Text</asp:ListItem>
                    <asp:ListItem>Graphic</asp:ListItem>
                </asp:DropDownList>--%>
            </td>
            <td align="center" style="width: 50%;font-size: large; font-weight: bold; color: #FFFFFF">
                <%--<asp:Label ID="lblCarrier" runat="server" Visible="False"></asp:Label>--%>
                <%--<br />--%>
                <%--<asp:LinkButton ID="lbPreviousDay" runat="server" Font-Underline="False" ToolTip="View Previous Day" Visible="False" Font-Bold="False" Font-Size="Small" ><<&nbsp;Previous Day&nbsp;</asp:LinkButton>--%>
                <%--&nbsp;--%>
                <%--<asp:DropDownList ID="ddlView" runat="server" AutoPostBack="True" Visible="False">
                    <asp:ListItem Value="3 Days" Selected="True">3 Days</asp:ListItem>
                    <asp:ListItem Value="5">5 Days</asp:ListItem>
                    <asp:ListItem Value="7">7 Days</asp:ListItem>
                </asp:DropDownList>--%>
                From:
                &nbsp;
                <telerik:RadDatePicker ID="from_date" runat="server" Culture="en-US" ></telerik:RadDatePicker>
                <br />
                To:
                &nbsp;
                <telerik:RadDatePicker ID="to_date" runat="server" Culture="en-US" ></telerik:RadDatePicker>
                <br />
                <asp:Button ID="bttnGo" CssClass="button" runat="server" Text="Go" Width="150px" />
                <%--<asp:LinkButton ID="lbNextDay" runat="server" Font-Underline="False" ToolTip="View Next Day" Visible="False" Font-Bold="False" Font-Size="Small" >&nbsp;Next Day&nbsp;>></asp:LinkButton>--%>
                <%--&nbsp;--%>
                <br />
                <br />
                <%--<asp:DropDownList ID="ddlStyle" runat="server" AutoPostBack="True">
                    <asp:ListItem Value="Graphic">Graphical</asp:ListItem>
                    <asp:ListItem Value="Text">Text</asp:ListItem>
                </asp:DropDownList>--%>
                <telerik:RadComboBox ID="rcStyle" runat="server" EmptyMessage="Calendar Style">
                    <Items>
                        <telerik:RadComboBoxItem Owner="rcStyle" Text="Graphical" ToolTip="Graphical" Value="Graphic" />
                        <telerik:RadComboBoxItem Owner="rcStyle" Text="Text" ToolTip="Text" Value="Text" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td align="center" style="width: 25%">
                Aircraft Type:&nbsp;&nbsp;
                <%--<asp:DropDownList ID="ddlACType" runat="server" AutoPostBack="True"></asp:DropDownList>--%>
                <telerik:RadComboBox ID="rcACType" runat="server" EmptyMessage="Aircraft type">
                    <%--<Items>
                        <telerik:RadComboBoxItem Owner="rcACType" Text="All" ToolTip="All" Value="All" />
                    </Items>--%>
                </telerik:RadComboBox>
            </td>
            <%--<td></td>--%>
            <%--<td align="right" style="font-size: large; font-weight: bold; color: #FFFFFF">
                Model Run ID&nbsp;
                <asp:Label ID="lblModelRunID" runat="server" Text=""></asp:Label>
            </td>--%>
        </tr>
    </table>
    <br />
    <br />
    <div id="legendTop" runat="server">
        <table border="0" cellpadding="3" cellspacing="0" width="100%">
            <tr>
                <td align="right" style="padding-right:5px;">
                    Available
                </td>
                <td align="left">
                    <div class="planningCellFlightPlanningActive" 
                        style="border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Revenue
                </td>
                <td align="left">
                    <%--<div class="planningCellCustomerFlight" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <div class="planningCellFlightPlanningCustomerFlight" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Restricted
                </td>
                <td align="left">
                    <div class="planningCellFlightPlanningRestricted" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Pinnned
                </td>
                <td align="left">
                    <div class="planningCellFlightPlanningPinned" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Reposition
                </td>
                <td align="left">
                    <%--<div class="planningCellCustomerDeadLeg" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <div class="planningCellFlightPlanningCustomerDeadLeg" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Maintenance
                </td>
                <td align="left">
                    <%--<div class="planningCellMaintenance" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <div class="planningCellFlightPlanningMaintenance" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    AOG
                </td>
                <td align="left">
                    <div class="planningCellFlightPlanningMaintenanceAOG" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    <%--Quick Flight--%>
                    <asp:Label ID="lblCustomType3" runat="server" Text="SWAP"></asp:Label>
                </td>
                <td align="left">
                    <%--<div class="planningCellCustomerFlight" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <%--<div class="planningCellFlightPlanningQuickFlight" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <div class="planningCellFlightPlanningOwner" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>                    
                <td align="right" style="padding-right:5px;">
                    <asp:Label ID="lblCustomType1" runat="server" Text="Bull Pen"></asp:Label>
                </td>
                <td align="left">
                    <%--<div class="planningCellOnGround" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>           
                    <div class="planningCellFlightPlanningOnGround" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>           
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>                    
                <td align="right" style="padding-right:5px;">
                    Training
                </td>
                <td align="left">
                    <div class="planningCellTraining" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>           
                </td>
            </tr>
        </table>
    </div>
    <div id="legendTopWC" runat="server">
        <table border="0" cellpadding="3" cellspacing="0" width="100%">
            <tr>
                <td align="right" style="padding-right:5px;">
                    Single Prop
                </td>
                <td align="left">
                    <div style="background-color: #FFFFCC; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Twin Prop
                </td>
                <td align="left">
                    <div style="background-color: #FFCC66; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Twin Turbo
                </td>
                <td align="left">
                    <div style="background-color: #9999FF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    VLJ
                </td>
                <td align="left">
                    <div style="background-color: #CCFF99; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Light
                </td>
                <td align="left">
                    <div style="background-color: #CCCCFF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Mid
                </td>
                <td align="left">
                    <div style="background-color: #CCFFFF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Super Mid
                </td>
                <td align="left">
                    <div style="background-color: #99FFCC; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Heavy
                </td>
                <td align="left">
                    <div style="background-color: #FFCCFF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Other
                </td>
                <td align="left">
                    <div style="background-color: #FFFFFF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
            </tr>
        </table>
    </div>
    <div id="legendTopText" runat="server">
        <table border="0" cellpadding="3" cellspacing="0" width="100%">
            <tr>
                <td align="right" style="padding-right:5px;">
                    Revenue
                </td>
                <td align="left">
                    <div style="font-size:8px;width:20px;height:10px; background-color: blue;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Reposition
                </td>
                <td align="left">
                    <div style="font-size:8px;width:20px;height:10px; background-color: gray;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Maintenance
                </td>
                <td align="left">
                    <div style="font-size:8px;width:20px;height:10px; background-color: red;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    AOG
                </td>
                <td align="left">
                    <div class="planningCellFlightPlanningMaintenanceAOG" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    <asp:Label ID="Label1" runat="server" Text="SWAP"></asp:Label>
                </td>
                <td align="left">
                    <div style="font-size:8px;width:20px;height:10px; background-color: purple;">&nbsp;&nbsp;&nbsp;</div>
                </td>
            </tr>
        </table>
    </div>
    <%--<div runat="server" id="div1" style="width:1575px;overflow:auto;background-color:Gray;position:relative; font-size: small;">--%>
    <div runat="server" id="div2" style="width:100%;overflow:auto;background-color:Gray;position:relative; font-size: small;">
        <asp:Table ID="tblFlightPlanningWeekly" runat="server" CellPadding="0" CellSpacing="0" CssClass="planningTable">
        </asp:Table>
    </div>
    <div id="legendBottom" runat="server">
        <table border="0" cellpadding="3" cellspacing="0" width="100%">
            <tr>
                <td align="right" style="padding-right:5px;">
                    Available
                </td>
                <td align="left">
                    <div class="planningCellFlightPlanningActive" 
                        style="border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Revenue
                </td>
                <td align="left">
                    <%--<div class="planningCellCustomerFlight" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <div class="planningCellFlightPlanningCustomerFlight" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                            <td align="right" style="padding-right:5px;">
                                Restricted
                            </td>
                            <td align="left">
                                <div class="planningCellFlightPlanningRestricted" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                            </td>
                            <td style="width:20px;">
                                &nbsp;
                            </td>
                            <td align="right" style="padding-right:5px;">
                                Pinnned
                            </td>
                            <td align="left">
                                <div class="planningCellFlightPlanningPinned" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                            </td>
                            <td style="width:20px;">
                                &nbsp;
                            </td>
                <td align="right" style="padding-right:5px;">
                    Reposition
                </td>
                <td align="left">
                    <%--<div class="planningCellCustomerDeadLeg" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <div class="planningCellFlightPlanningCustomerDeadLeg" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Maintenance
                </td>
                <td align="left">
                    <%--<div class="planningCellMaintenance" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <div class="planningCellFlightPlanningMaintenance" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    AOG
                </td>
                <td align="left">
                    <div class="planningCellFlightPlanningMaintenanceAOG" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    <%--Quick Flight--%>
                    <asp:Label ID="lblCustomType4" runat="server" Text="SWAP"></asp:Label>
                </td>
                <td align="left">
                    <%--<div class="planningCellCustomerFlight" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <%--<div class="planningCellFlightPlanningQuickFlight" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>
                    <div class="planningCellFlightPlanningOwner" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>                    
                <td align="right" style="padding-right:5px;">
                    <asp:Label ID="lblCustomType2" runat="server" Text="Bull Pen"></asp:Label>
                </td>
                <td align="left">
                    <%--<div class="planningCellOnGround" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>--%>           
                    <div class="planningCellFlightPlanningOnGround" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>           
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>                    
                <td align="right" style="padding-right:5px;">
                    Training
                </td>
                <td align="left">
                    <div class="planningCellTraining" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>           
                </td>
            </tr>
        </table>
    </div>
    <div id="legendBottomWC" runat="server">
        <table border="0" cellpadding="3" cellspacing="0" width="100%">
            <tr>
                <td align="right" style="padding-right:5px;">
                    Single Prop
                </td>
                <td align="left">
                    <div style="background-color: #FFFFCC; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Twin Prop
                </td>
                <td align="left">
                    <div style="background-color: #FFCC66; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Twin Turbo
                </td>
                <td align="left">
                    <div style="background-color: #9999FF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    VLJ
                </td>
                <td align="left">
                    <div style="background-color: #CCFF99; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Light
                </td>
                <td align="left">
                    <div style="background-color: #CCCCFF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Mid
                </td>
                <td align="left">
                    <div style="background-color: #CCFFFF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Super Mid
                </td>
                <td align="left">
                    <div style="background-color: #99FFCC; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Heavy
                </td>
                <td align="left">
                    <div style="background-color: #FFCCFF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Other
                </td>
                <td align="left">
                    <div style="background-color: #FFFFFF; border: thin solid #000000; font-size:8px; width:20px; height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
            </tr>
        </table>
    </div>
    <div id="legendBottomText" runat="server">
        <table border="0" cellpadding="3" cellspacing="0" width="100%">
            <tr>
                <td align="right" style="padding-right:5px;">
                    Revenue
                </td>
                <td align="left">
                    <div style="font-size:8px;width:20px;height:10px; background-color: blue;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Reposition
                </td>
                <td align="left">
                    <div style="font-size:8px;width:20px;height:10px; background-color: gray;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    Maintenance
                </td>
                <td align="left">
                    <div style="font-size:8px;width:20px;height:10px; background-color: red;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    AOG
                </td>
                <td align="left">
                    <div class="planningCellFlightPlanningMaintenanceAOG" style="font-size:8px;width:20px;height:10px;">&nbsp;&nbsp;&nbsp;</div>
                </td>
                <td style="width:20px;">
                    &nbsp;
                </td>
                <td align="right" style="padding-right:5px;">
                    <asp:Label ID="Label2" runat="server" Text="SWAP"></asp:Label>
                </td>
                <td align="left">
                    <div style="font-size:8px;width:20px;height:10px; background-color: purple;">&nbsp;&nbsp;&nbsp;</div>
                </td>
            </tr>
        </table>
    </div>
</div>