<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FlightSchedule.aspx.vb" Inherits="CoastalPortal.FlightSchedule" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="Controls/OptimizerCalendar.ascx" tagname="OptimizerCalendar" tagprefix="uc3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Flight Schedule</title>
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
    <%--<script language="javascript" type="text/javascript">
        function showFlightDetailPopout(flightID) {
            var elem = document.getElementById('flightDetailContainer');

            elem.style.visibility = 'visible';
            elem.style.display = 'inline';

            elem.style.visibility = 'visible';
            elem.style.display = 'inline';

            elem.style.top = (parseInt(mouseY) + 20) + 'px'; //(parseInt(mouseY) - 100) + 'px';
            //elem.style.left = (parseInt(mouseX) - 100) + 'px';

            if (parseInt(mouseX) - parseInt(elem.style.width) > 0) {
                //too far to the right
                if (parseInt(mouseX) +
                    parseInt(elem.style.width) >= parseInt(winW)) {
                    elem.style.left =
                        parseInt(mouseX) -
                        parseInt(elem.style.width) + 20;
                }
                else //just right
                {
                    elem.style.left = (parseInt(mouseX) - 100
                        - parseInt(elem.style.width) / 2) + 'px';
                }
            }
            else //to far to the left
            {
                elem.style.left = '5px';
            }

            // rk 10.30.2010 add flight editor and random number fix to refresh iframe
            // document.getElementById('flightDetailFrame').src = 'FlightDetailPopout.aspx?flightID=' + flightID;
            document.getElementById('flightDetailFrame').src = 'FlightEditorPopout.aspx?flightID=' + flightID + '&junk=' + (new Date()).valueOf();

        }

        function hideFlightDetailPopout() {
            var elem = document.getElementById('flightDetailContainer');

            //rk 10.30.2010 refresh main page after editing flight
            parent.location.reload()

            elem.style.visibility = 'hidden';
            elem.style.display = 'none';
        }
    </script>--%>
    <%--<script language="javascript" type="text/javascript">
        function showAircraftDetailPopout(aircraftID) {
            var elem = document.getElementById('flightDetailContainer');

            elem.style.visibility = 'visible';
            elem.style.display = 'inline';

            elem.style.visibility = 'visible';
            elem.style.display = 'inline';

            elem.style.top = (parseInt(mouseY) + 45) + 'px'; //(parseInt(mouseY) - 100) + 'px';
            //elem.style.left = (parseInt(mouseX) - 100) + 'px';

            if (parseInt(mouseX) - parseInt(elem.style.width) > 0) {
                //too far to the right
                if (parseInt(mouseX) +
                    parseInt(elem.style.width) >= parseInt(winW)) {
                    elem.style.left =
                        parseInt(mouseX) -
                        parseInt(elem.style.width) + 20;
                }
                else //just right
                {
                    elem.style.left = (parseInt(mouseX) - 100
                        - parseInt(elem.style.width) / 2) + 'px';
                }
            }
            else //to far to the left
            {
                elem.style.left = '145px';
            }

            // rk 10.30.2010 add flight editor and random number fix to refresh iframe
            // document.getElementById('flightDetailFrame').src = 'FlightDetailPopout.aspx?flightID=' + flightID;

            //  document.getElementById('flightDetailFrame').src = 'AircraftDetailPopout.aspx?aircraftID=' + aircraftID;
            document.getElementById('flightDetailFrame').src = 'AircraftDetailPopout.aspx?aircraftID=' + aircraftID + '&junk=' + (new Date()).valueOf();

        }

        function hideAircraftDetailPopout() {
            var elem = document.getElementById('flightDetailContainer');

            //rk 10.30.2010 refresh main page after making changes
            parent.location.reload()

            elem.style.visibility = 'hidden';
            elem.style.display = 'none';
        }
    </script>--%>
    
<script language=javascript>

        function hideOrShow(obj) {
            if (obj.style.visibility == 'visible') {
                hide(obj);
            }
            else {
                show(obj);
            }
        }

        function show(obj) {
            obj.style.visibility = 'visible';
            obj.style.position = 'relative';
        }

        function hide(obj) {
            obj.style.visibility = 'hidden';
            obj.style.position = 'absolute';
        }

</script>   
 
<script type="text/javascript">
    //<!--

    var flightID = 0;
    var orig = "";
    var dest = "";
    var flightDetail = 0;
    var apppath = "";

    function buildMenu(e1, e2, e3, e4) {
        flightID = e1;
        orig = e2;
        dest = e3;
        flightDetail = e4;
        displayMenu();
    }
    function displayMenu() {
        whichDiv = event.srcElement;
        menu1.style.leftPos += 10;
        menu1.style.posLeft = event.clientX;
        menu1.style.posTop = event.clientY;
        menu1.style.width = 120;
        menu1.style.display = "";
        menu1.setCapture();
    }
    function switchMenu() {
        el = event.srcElement;
        if (el.className == "menuItem") {
            el.className = "highlightItem";
        } else if (el.className == "highlightItem") {
            el.className = "menuItem";
        }
    }
    function clickMenu() {
        menu1.releaseCapture();
        menu1.style.display = "none";
        el = event.srcElement;
        if (el.id == "mnuFlightDetail") {
            showFlightDetailPopout(flightID);
        } else if (el.id == "mnuTripBuilder") {
            window.open("flighteditor.aspx?tripnumber=" + flightDetail);
            //        } else if (el.id == "mnuOrigAirport") {
            //            whichDiv.style.backgroundColor = "green";
            //        } else if (el.id == "mnuDestAirport") {
            //            whichDiv.style.backgroundColor = "blue";
            //        } else if (el.id == "mnuItem4") {
            //            whichDiv.style.backgroundColor = "yellow";
            //        if (el.id == "mnuPrintCBPForm") {
            //            showFlightDetailPopout(flightID);
        }
    }

    //-->
</script>
   </head>
<body>
    <script type="text/javascript" src="Scripts/wz_tooltip.js"></script>
    <script type="text/javascript" src="tip_balloon/tip_balloon.js"></script>
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
		<div class="title">Flight Schedule</div>

		<div> 
            <uc3:OptimizerCalendar ID="OptimizerCalendar1" runat="server" /> 
		</div>
	
	</div>
			
</section>
	
<footer class="normal">
	<div class="wrapper">
		<span class="copyring">
			<p>© 2017 CoastalAVTech LTD</p>
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
