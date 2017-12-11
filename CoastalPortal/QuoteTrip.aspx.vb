Imports CoastalPortal.AirTaxi
'Imports WebPortal.ServiceProviderMatrix
Imports System.Data
'Imports System.Data.Common
'Imports System.Data.SqlClient
'Imports System.Globalization
Imports Microsoft.Security.Application
Imports Telerik.Web.UI
Imports System.Data.SqlClient
Imports System.Drawing

'Imports PopulateLookups

'20120208 - pab - show/hide carrier logos under quotes
Imports System.IO

Public Class QuoteTrip
    Inherits System.Web.UI.Page

    '20110926 - pab - update web services
    'Public wscas As New CoastalAviation.Service 'connect_it_coastal.Service
    '20120201 - pab - changes for azure
    'Public wscas As New AviationService1_9.Service 'connect_it_coastal.Service
    '20120423 - pab - call azure web service
    'Public wscas As New AviationService1_10.Service 'connect_it_coastal.Service
    Public wscas As New AviationWebService1_10.WebService1 'connect_it_coastal.Service

    Public oMapping As New Mapping


    Private _pricefees As Double = 0

    Private _leaves, _returns As Date
    Private _segment_leaves, _segment_returns

    Public latLongO As LatLong = Nothing
    Public latLongD As LatLong = Nothing

    Private _originTable As DataTable = Nothing
    Private _destinationTable As DataTable = Nothing

    'rk 11/18/2010 import a banner
    Public mybanner As String = String.Empty
    Public myJS1 As String = String.Empty
    Public myJS0 As String = String.Empty

    Public myclick As String = String.Empty


    Private dtflights As New DataTable '= Nothing

    '20141121 - pab - rewrite quoteworker routine
    Private dtquotes As New DataTable '= Nothing
    Private dtflights2 As New DataTable '= Nothing

    '20141201 - pab - quoteworker rewrite
    Private dtquotedetail As New DataTable '= Nothing

    Private _mapData As String = String.Empty
    Private _mapData0 As String = String.Empty

    Private _arrCount As Integer = 0

    Private _arrCount0 As Integer = 0
    '/ Inherits System.Web.UI.Page

    Private _initializeMap As String = ""


    Private Const _setTimeoutInterval As Integer = 1500

    Private _originCount As Integer = 0

    Private _qs As String




    Private _origAddr As String = String.Empty
    Private _origCity As String = String.Empty
    Private _origState As String = String.Empty
    Private _origZip As String = String.Empty
    Private _origCountry As String = String.Empty

    Private _destAddr As String = String.Empty
    Private _destCity As String = String.Empty
    Private _destState As String = String.Empty
    Private _destZip As String = String.Empty
    Private _destCountry As String = String.Empty

    Private _origAirportCode As String = String.Empty
    Private _destAirportCode As String = String.Empty


    Private _addCoPilot As Boolean = False

    Private _roundTrip As Boolean = False
    Private _passengers As Integer = 0
    Private _leaveDateTime As DateTime
    Private _returnDateTime As DateTime



    '20090120 - pab - add additional fees
    Private _departPriceExplanationDetail As String = String.Empty
    Private _returnPriceExplanationDetail As String = String.Empty



    Private _departOrigAirportCode As String = String.Empty
    Private _departDestAirportCode As String = String.Empty

    Private _returnOrigAirportCode As String = String.Empty
    Private _returnDestAirportCode As String = String.Empty

    Private _departServiceProvider As String = String.Empty
    Private _returnServiceProvider As String = String.Empty

    Private _departPrice As Double = 0
    Private _returnPrice As Double = 0


    Private _departDateTime As DateTime


    'Private _pricePerSeat As Double = 0
    'Private _taxPerSeatMoney As Double = 0
    'Private _roletype As Enumerations.RoleTypes

    Private _userID As Integer = 0
    Private _userType As Enumerations.UserTypes = Enumerations.UserTypes.Guest

    Private _totalCost As Double = 0

    '20100602 - pab - fix pricing 
    Private _returntobase As String

    '20101102 - pab - fix intl fees
    Private _intl As Double = 0
    Private _customs As Double = 0

    '20130722 - pab - fix web service call returning before quotes finished
    Private _sleep As Integer

    '20131118 - pab - add more fields to aircraft
    Private ManufactureDate As Date

    '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
    Private TurnAroundMinutes As Integer

    '20161028 - drive time
    Private latLongorig As LatLong
    Private latLongdest As LatLong

    '20160117 - pab - quote multi-leg trips
    Private Sub CreateFlightsTableMulti(ByVal ds As DataSet)

        Dim dc As DataColumn = Nothing
        Dim dr As DataRow = Nothing

        Dim sortExp As String = "quoteID,ID"
        Dim drarray() As DataRow
        Dim dr2 As DataRow = Nothing
        Dim dr3 As DataRow = Nothing
        Dim prevquoteID As Integer = 0
        Dim prevPrice As String = String.Empty
        Dim prevPriceExplanationDetail As String = String.Empty
        Dim s As String = String.Empty
        Dim dt As DataTable = ds.Tables(0)
        Dim quoteID As Integer = dt.Rows(0).Item("quoteID")
        Dim worknumber As Integer = 0
        Dim da As New DataAccess
        Dim dtq As DataTable

        AirTaxi.post_timing("CreateFlightsTableMulti Start  " & Now.ToString)

        dtq = da.GetD2DQuoteQueue(quoteID)
        If isdtnullorempty(dtq) Then
            Exit Sub
        End If
        worknumber = dtq.Rows(0).Item("worknumber")
        dtq.Clear()
        dtq = da.GetD2DQuoteQueuebyWorkNumber(worknumber)
        If isdtnullorempty(dtq) Then
            Exit Sub
        End If

        If dtquotes.Columns.Count = 0 Then

            dtquotes = Create_dtflights()

            dc = New DataColumn("WeightClassTitle", System.Type.GetType("System.String"))
            dtquotes.Columns.Add(dc)

        End If

        Try

            For i As Integer = 0 To dtq.Rows.Count - 1

                dr3 = dtq.Rows(i)

                If dr3("totalprice") > 0 Then

                    If dr3("result") <> "" Then

                        Dim sr As New StringReader(dr3("result").ToString)
                        Dim dtr As New DataTable

                        dtr.Clear()
                        dtr.ReadXml(sr)

                        dr = dtquotes.NewRow

                        For n2 As Integer = 0 To dtr.Rows.Count - 1
                            dr2 = dtr.Rows(n2)

                            dr("Service Provider") = dr2("Service Provider")

                            dr("carrierlogo") = dr2("carrierlogo")

                            If dr("Origin").ToString.Trim = "" Then
                                dr("Origin") &= "<table valign=""center""><tr><td>" & dr2("Origin") & "</td></tr>"
                            Else
                                dr("Origin") &= "<tr><td>" & dr2("Origin") & "</td></tr>"
                            End If
                            s = dr2("OriginFacilityName")
                            If InStr(s, "/") > 0 Then s = Replace(s, "/", "/ ", 1, 1)
                            '20160912
                            'If Len(s) < 20 Then s &= "<br /><br />"
                            If dr("OriginFacilityName").ToString.Trim = "" Then
                                dr("OriginFacilityName") &= "<table valign=""center""><tr><td>" & s & "</td></tr>"
                            Else
                                dr("OriginFacilityName") &= "<tr><td>" & s & "</td></tr>"
                            End If
                            s = dr2("Departs")
                            'If Len(s) < 15 Then s &= "<br /><br />"
                            If dr("Departs").ToString.Trim = "" Then
                                dr("Departs") &= "<table valign=""center""><tr><td>" & s & "</td></tr>"
                            Else
                                dr("Departs") &= "<tr><td>" & s & "</td></tr>"
                            End If
                            If dr("Destination").ToString.Trim = "" Then
                                dr("Destination") &= "<table valign=""center""><tr><td>" & dr2("Destination") & "</td></tr>"
                            Else
                                dr("Destination") &= "<tr><td>" & dr2("Destination") & "</td></tr>"
                            End If
                            s = dr2("DestinationFacilityName")
                            If InStr(s, "/") > 0 Then s = Replace(s, "/", "/ ", 1, 1)
                            'If Len(s) < 20 Then s &= "<br /><br />"
                            If dr("DestinationFacilityName").ToString.Trim = "" Then
                                dr("DestinationFacilityName") &= "<table valign=""center""><tr><td>" & s & "</td></tr>"
                            Else
                                dr("DestinationFacilityName") &= "<tr><td>" & s & "</td></tr>"
                            End If
                            s = dr2("Arrives")
                            'If Len(s) < 15 Then s &= "<br /><br />"
                            If dr("Arrives").ToString.Trim = "" Then
                                dr("Arrives") &= "<table valign=""center""><tr><td>" & s & "</td></tr>"
                            Else
                                dr("Arrives") &= "<tr><td>" & s & "</td></tr>"
                            End If
                            If dr("Flight Duration").ToString.Trim = "" Then
                                'dr("Flight Duration") &= "<table valign=""center""><tr><td>" & dr2("Flight Duration") & "<br /><br /></td></tr>"
                                dr("Flight Duration") &= "<table valign=""center""><tr><td>" & dr2("Flight Duration") & "</td></tr>"
                            Else
                                'dr("Flight Duration") &= "<tr><td>" & dr2("Flight Duration") & "<br /><br /></td></tr>"
                                dr("Flight Duration") &= "<tr><td>" & dr2("Flight Duration") & "</td></tr>"
                            End If
                            If dr("minutes").ToString.Trim = "" Then
                                dr("minutes") &= "<table valign=""center""><tr><td>" & dr2("minutes") & "</td></tr>"
                            Else
                                dr("minutes") &= "<tr><td>" & dr2("minutes") & "</td></tr>"
                            End If
                            If dr2("Price").ToString.Trim <> "" Then
                                prevPrice = dr2("Price")
                            End If
                            dr("PriceEdit") = dr2("PriceEdit")
                            If dr2("PriceExplanationDetail").ToString.Trim <> "" Then prevPriceExplanationDetail = dr2("PriceExplanationDetail")
                            dr("ShowPriceExplanation") = dr2("ShowPriceExplanation")
                            If dr("EmptyLeg").ToString.Trim = "" Then
                                dr("EmptyLeg") &= "<table valign=""center""><tr><td>" & dr2("EmptyLeg") & "</td></tr>"
                            Else
                                dr("EmptyLeg") &= "<tr><td>" & dr2("EmptyLeg") & "</td></tr>"
                            End If
                            dr("AircraftType") = dr2("AircraftType")
                            dr("WeightClass") = dr2("WeightClass")
                            dr("dbname") = dr2("dbname")
                            dr("aircraftlogo") = dr2("aircraftlogo")
                            dr("Name") = dr2("Name")
                            dr("FAQPageURL") = dr2("FAQPageURL")

                            If dr("FuelStops").ToString.Trim = "" Then
                                'dr("FuelStops") &= "<table valign=""center""><tr><td>" & dr2("FuelStops") & "<br /><br /></td></tr>"
                                dr("FuelStops") &= "<table valign=""center""><tr><td>" & dr2("FuelStops") & "</td></tr>"
                            Else
                                'dr("FuelStops") &= "<tr><td>" & dr2("FuelStops") & "<br /><br /></td></tr>"
                                dr("FuelStops") &= "<tr><td>" & dr2("FuelStops") & "</td></tr>"
                            End If
                            dr("Pets") = dr2("Pets")
                            dr("Smoking") = dr2("Smoking")

                            dr("certifications") = dr2("certifications")

                            dr("wifi") = dr2("wifi")

                            dr("EnclosedLav") = dr2("EnclosedLav")
                            dr("PowerAvailable") = dr2("PowerAvailable")

                            dr("InflightEntertainment") = dr2("InflightEntertainment")
                            dr("ManufactureDate") = dr2("ManufactureDate")

                            dr("OwnerConfirmation") = dr2("OwnerConfirmation")

                            If dr("EmptyLegPricing").ToString.Trim = "" Then
                                dr("EmptyLegPricing") &= "<table valign=""center""><tr><td>" & dr2("EmptyLegPricing") & "</td></tr>"
                            Else
                                dr("EmptyLegPricing") &= "<tr><td>" & dr2("EmptyLegPricing") & "</td></tr>"
                            End If

                            If dr("legcode").ToString.Trim = "" Then
                                dr("legcode") &= "<table valign=""center""><tr><td>" & dr2("legcode") & "</td></tr>"
                            Else
                                dr("legcode") &= "<tr><td>" & dr2("legcode") & "</td></tr>"
                            End If

                            If InStr(dr2("Name").ToString, ":") > 0 Then
                                dr("WeightClassTitle") = Left(dr2("Name"), InStr(dr2("Name").ToString, ":") - 1)
                            Else
                                Select Case dr2("WeightClass").ToString
                                    Case "P"
                                        dr("WeightClassTitle") = "Single Piston"
                                    Case "V"
                                        dr("WeightClassTitle") = "VLJ"
                                    Case "L"
                                        dr("WeightClassTitle") = "Light"
                                    Case "M"
                                        dr("WeightClassTitle") = "Mid"
                                    Case "H"
                                        dr("WeightClassTitle") = "Heavy"
                                    Case "S"
                                        dr("WeightClassTitle") = "Super Heavy"
                                    Case "T"
                                        dr("WeightClassTitle") = "Twin Piston"
                                    Case "1"
                                        dr("WeightClassTitle") = "Single Turboprop"
                                    Case "2"
                                        dr("WeightClassTitle") = "Twin Turboprop"
                                    Case "U"
                                        dr("WeightClassTitle") = "SuperMid"
                                    Case Else
                                        dr("WeightClassTitle") = dr2("WeightClass").ToString
                                End Select
                            End If

                        Next

                        If Not IsNothing(dr) Then
                            If dr("Origin").ToString.Trim <> "" Then dr("Origin") &= "</table>"
                            If dr("OriginFacilityName").ToString.Trim <> "" Then dr("OriginFacilityName") &= "</table>"
                            If dr("Departs").ToString.Trim <> "" Then dr("Departs") &= "</table>"
                            If dr("Destination").ToString.Trim <> "" Then dr("Destination") &= "</table>"
                            If dr("DestinationFacilityName").ToString.Trim <> "" Then dr("DestinationFacilityName") &= "</table>"
                            If dr("Arrives").ToString.Trim <> "" Then dr("Arrives") &= "</table>"
                            If dr("Flight Duration").ToString.Trim <> "" Then dr("Flight Duration") &= "</table>"
                            If dr("minutes").ToString.Trim <> "" Then dr("minutes") &= "</table>"
                            If dr("EmptyLeg").ToString.Trim <> "" Then dr("EmptyLeg") &= "</table>"
                            If dr("FuelStops").ToString.Trim <> "" Then dr("FuelStops") &= "</table>"
                            If dr("EmptyLegPricing").ToString.Trim <> "" Then dr("EmptyLegPricing") &= "</table>"
                            If dr("legcode").ToString.Trim <> "" Then dr("legcode") &= "</table>"
                            dr("Price") = prevPrice
                            dr("PriceExplanationDetail") = prevPriceExplanationDetail

                            dr("quoteID") = prevquoteID

                            If prevPrice = "" Then
                                prevPrice = prevPrice
                            End If

                            dtquotes.Rows.Add(dr)
                        End If

                    End If

                End If

            Next

        Catch ex As Exception
            s = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbCr & vbLf & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace.ToString) Then s &= vbCr & vbLf & ex.StackTrace.ToString
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.Insertsys_log(Session("carrierid"), appName, s, "CreateFlightsTableMulti", "QuoteTrip.aspx.vb")
        End Try

        AirTaxi.post_timing("CreateFlightsTableMulti End  " & Now.ToString)

    End Sub

    Private Function pushpin(ByVal v As Double, ByVal h As Double, ByVal color As String, ByVal title As String, ByVal description As String, ByVal shapename As String) As String

        '20111229 - pab
        AirTaxi.post_timing("pushpin Start  " & Now.ToString)

        Dim colors As String
        colors = "198,239,206,1.0" 'blue
        If UCase(color) = "GREEN" Then colors = "0,150,100,1.0"



        'Dim latLong As LatLong = Session("origlat")
        '20120424 - pab - convert from mappoint to bing
        'Dim oMapping As New Mapping



        '20120424 - pab - convert from mappoint to bing
        'Dim routeInfo As MapPointService.Route
        'routeInfo = oMapping.GetRoute(latLong.Latitude, _
        '                           latLong.Longitude, CDbl(v), CDbl(h))
        'Dim time As Double = 0
        'Dim timeHrs As Integer = 0
        'Dim timeMins As Integer = 0
        'Dim timeDisplay As String = String.Empty
        'Dim distance As Double = 0

        '20120424 - pab - convert from mappoint to bing
        'If Not routeInfo Is Nothing Then
        '    time = routeInfo.Itinerary.TripTime
        '    distance = Math.Round(routeInfo.Itinerary.Distance, 2)
        'End If
        'If time / 60 >= 60 Then
        '    timeHrs = CInt(Math.Floor(time / 3600))
        '    timeMins = CInt(Math.Floor((time - timeHrs * 3600) / 60))
        '    timeDisplay = timeHrs.ToString & " hr " & timeMins.ToString & " min"
        'Else 'minutes only
        '    timeDisplay = Math.Floor(time / 60).ToString & " min"
        'End If

        '20101029 - pab - truncate lat and lon
        v = Math.Round(v, 2)
        h = Math.Round(h, 2)

        'add pushpin
        '_mapData &= "addPushpin('" & CStr(dr("LocationID")) & "_" & (i - 1).ToString & "', new VELatLong(" & CStr(dr("Latitude")) & _
        '                ", " & CStr(dr("Longitude")) & "), '" & CStr(dr("LocationID")) & " - " & CStr(dr("FacilityName")).Replace("'", "\'") & "', '" & _
        '"<table border=""0"" cellpadding=""0"" cellspacing=""1"">" '& _
        ''"<tr><td align=""right"" style=""padding-right:2px;padding-top:2px;"">" & _
        '"timex:</td><td align=""left"" style=""padding-top:2px;width:75%;"">" & timeDisplay & _
        '"</td></tr>" & _
        '"<tr><td align=""right"" style=""padding-top:2px;padding-right:2px;padding-bottom:5px;"">" & _
        '"Distance:</td><td align=""left"" style=""padding-top:2px;padding-bottom:5px;"">" & CStr(distance) & " mi " & _
        '"</td></tr>"

        'rk 112009 **
        '20120424 - pab - convert from mappoint to bing
        'Dim myHtml As String
        'myHtml = "<br><tr><td align=""left"" style=""padding-right:2px;padding-top:2px;"">" & _
        '"timex:</td><td align=""left"" style=""padding-top:2px;width:75%;"">" & timeDisplay & _
        '"</td></tr>" & _
        '"<br>" & _
        '"<tr><td align=""left"" style=""padding-top:2px;padding-right:2px;padding-bottom:5px;"">" & _
        '"Distance:</td><td align=""left"" style=""padding-top:2px;padding-bottom:5px;"">" & CStr(distance) & " mi " & _
        '"</td></tr>"

        'Dim myHtml As String = "</br></br>Click <a href=\'http://www.airnav.com/airport/KHTS\' target=\'_blank\'>here</a> for more info" & _
        '"<tr><td align=""left"" style=""border-top:solid 1px #C0C0C0;padding-top:2px;"""">" & _
        '"<table id=""rblFindNearby_0"" border=""0"" cellpadding=""0"" cellspacing=""0"">" & _
        ' "<tr>" & _
        ' "<td colspan=""3"">Find Nearby:</td>" & _
        ' "</tr><tr>" & _
        '  "<td><input id=""rblFindNearby_0_0"" type=""radio"" name=""rblFindNearby_0"" value=""Car_Rental"" /><label for=""rblFindNearby_0_0"">Car Rental</label></td>" & _
        '  "<td><input id=""rblFindNearby_0_2"" type=""radio"" name=""rblFindNearby_0"" value=""Limousine"" /><label for=""rblFindNearby_0_2"">Limousine</label></td>" & _
        '  "<td rowspan=""2"" valign=""top"" style=""padding-left:5px;padding-top:2px;"">" & _
        '  "<input type=""button"" value=""Find"" style=""font-size:0.9em;"" id=""bttnFindNearby_0"" " & _
        '   "<td style=""padding-bottom:3px;""><input id=""rblFindNearby_0_1"" type=""radio"" name=""rblFindNearby_0"" value=""Hotel"" /><label for=""rblFindNearby_0_1"">Hotel</label></td>" & _
        '  "<td style=""padding-bottom:3px;""><input id=""rblFindNearby_0_3"" type=""radio"" name=""rblFindNearby_0"" value=""Taxi"" /><label for=""rblFindNearby_0_3"">Taxi</label></td>" & _
        ' "</tr>" & _
        '"</table>" & _
        '"</td></tr>"





        'Dim myHtml As String = "</br></br>Click <a href=\'QuoteTrip.aspx?origAirportCode=KHTS\' target=\'_blank\'>here</a> for departure airport" & _
        '"</br></br>Click <a href=\'QuoteTrip.aspx?destairportcode=KHTS\' target=\'_blank\'>here</a> for destination airport"
        '' "<input type=""button"" value=""Fly From: KHTS"" style=""font-size:0.9em;"" id=""bttnFindNearby"" " & _
        ''"OnClick = " & "bttnNewFlightRequest_Click" & _
        ' "<input type=""button"" value=""Fly To: KHTS"" style=""font-size:0.9em;"" id=""bttnFlyTo"" "

        '"onclick=""LocateItemOnMap(title);" & _

        'chg3620 - 20100920 - bd - fix javascript error
        ' pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
        '"shape2.SetTitle('" & title & "');" & _
        '"shape2.SetDescription('" & description & "');" & _
        '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
        '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_blue.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & title & "</span></td></tr></table>"";" & _
        '"var spec = new VECustomIconSpecification();" & _
        '"spec.CustomHTML = icon;" & _
        '"shape2.SetCustomIcon(spec);" & _
        '"map.AddShape(shape2);"

        '20120502 - pab - get rid of the annoying single quotes surrounding airport code
        ' pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
        '"shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" & _
        '"shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" & _
        '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
        '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_blue.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & AntiXss.JavaScriptEncode(title) & "</span></td></tr></table>"";" & _
        '"var spec = new VECustomIconSpecification();" & _
        '"spec.CustomHTML = icon;" & _
        '"shape2.SetCustomIcon(spec);" & _
        '"map.AddShape(shape2);"
        pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" &
        "shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" &
        "shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" &
        "shape2.SetLineColor(new VEColor(" & colors & "));" &
        "var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_blue.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>"
        If InStr(title, "/") > 0 Or InStr(title, "\") > 0 Or InStr(title, ".") > 0 Or InStr(title, "'") > 0 Then
            pushpin &= AntiXss.JavaScriptEncode(title)
        Else
            pushpin &= title
        End If
        pushpin &= "</span></td></tr></table>"";" &
          "var spec = new VECustomIconSpecification();" &
          "spec.CustomHTML = icon;" &
          "shape2.SetCustomIcon(spec);" &
          "map.AddShape(shape2);"

        '   '   "shape2.SetDescription('" & description & myHtml & "');" & _
        If color = "end" Then
            'chg3620 - 20100920 - bd - fix javascript error
            'pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
            '"shape2.SetTitle('" & "" & "');" & _
            '"shape2.SetDescription('" & "" & "');" & _
            '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
            '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/starting_point_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & title & "</span></td></tr></table>"";" & _
            '"var spec = new VECustomIconSpecification();" & _
            '"spec.CustomHTML = icon;" & _
            '"shape2.SetCustomIcon(spec);" & _
            '"map.AddShape(shape2);"

            '20120502 - pab - get rid of the annoying single quotes surrounding airport code
            'pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
            '"shape2.SetTitle('" & "" & "');" & _
            '"shape2.SetDescription('" & "" & "');" & _
            '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
            '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/starting_point_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & AntiXss.JavaScriptEncode(title) & "</span></td></tr></table>"";" & _
            '"var spec = new VECustomIconSpecification();" & _
            '"spec.CustomHTML = icon;" & _
            '"shape2.SetCustomIcon(spec);" & _
            '"map.AddShape(shape2);"
            pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" &
            "shape2.SetTitle('" & "" & "');" &
            "shape2.SetDescription('" & "" & "');" &
            "shape2.SetLineColor(new VEColor(" & colors & "));" &
            "var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/starting_point_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>"
            If InStr(title, "/") > 0 Or InStr(title, "\") > 0 Or InStr(title, ".") > 0 Or InStr(title, "'") > 0 Then
                pushpin &= AntiXss.JavaScriptEncode(title)
            Else
                pushpin &= title
            End If
            pushpin &= "</span></td></tr></table>"";" &
                "var spec = new VECustomIconSpecification();" &
                "spec.CustomHTML = icon;" &
                "shape2.SetCustomIcon(spec);" &
                "map.AddShape(shape2);"
        End If

        If color = "selected" Then
            'chg3620 - 20100920 - bd - fix javascript error
            'pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
            '"shape2.SetTitle('" & title & "');" & _
            '"shape2.SetDescription('" & description & "');" & _
            '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
            '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & title & "</span></td></tr></table>"";" & _
            '"var spec = new VECustomIconSpecification();" & _
            '"spec.CustomHTML = icon;" & _
            '"shape2.SetCustomIcon(spec);" & _
            '"map.AddShape(shape2);"

            '20120502 - pab - get rid of the annoying single quotes surrounding airport code
            'pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
            '"shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" & _
            '"shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" & _
            '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
            '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & AntiXss.JavaScriptEncode(title) & "</span></td></tr></table>"";" & _
            '"var spec = new VECustomIconSpecification();" & _
            '"spec.CustomHTML = icon;" & _
            '"shape2.SetCustomIcon(spec);" & _
            '"map.AddShape(shape2);"
            pushpin = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" &
            "shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" &
            "shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" &
            "shape2.SetLineColor(new VEColor(" & colors & "));" &
            "var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>"
            If InStr(title, "/") > 0 Or InStr(title, "\") > 0 Or InStr(title, ".") > 0 Or InStr(title, "'") > 0 Then
                pushpin &= AntiXss.JavaScriptEncode(title)
            Else
                pushpin &= title
            End If
            pushpin &= "</span></td></tr></table>"";" &
                "var spec = new VECustomIconSpecification();" &
                "spec.CustomHTML = icon;" &
                "shape2.SetCustomIcon(spec);" &
                "map.AddShape(shape2);"
        End If

        pushpin = Replace(pushpin, "KHTS", title)
        pushpin = Replace(pushpin, "shape2", shapename)

        '"var icon = ""<div style='font-size:12px;font-weight:bold;border:solid 2px Black;background-color:Aqua;width:50px;'>Custom</div>"";" & _
        '        "var spec = new VECustomIconSpecification();" & _
        '        "spec.CustomHTML = icon;" & _
        '        "spec.Image = 'http://www.geocities.com/cmpoet/Airplane1.png'" & _
        '        "shape2.SetCustomIcon(spec);" & _

        '20111229 - pab
        AirTaxi.post_timing("pushpin End  " & Now.ToString)

    End Function


    Private Function pushpin0(ByVal v As Double, ByVal h As Double, ByVal color As String, ByVal title As String, ByVal description As String, ByVal shapename As String) As String

        '20111229 - pab
        AirTaxi.post_timing("pushpin0 Start  " & Now.ToString)

        Dim colors As String
        colors = "198,239,206,1.0" 'blue
        If UCase(color) = "GREEN" Then colors = "0,150,100,1.0"


        'Dim myHtml As String = "</br></br>Click <a href=\'http://www.airnav.com/airport/KHTS\' target=\'_blank\'>here</a> for more info" & _
        '"<tr><td align=""left"" style=""border-top:solid 1px #C0C0C0;padding-top:2px;"""">" & _
        '"<table id=""rblFindNearby_0"" border=""0"" cellpadding=""0"" cellspacing=""0"">" & _
        ' "<tr>" & _
        ' "<td colspan=""3"">Find Nearby:</td>" & _
        ' "</tr><tr>" & _
        '  "<td><input id=""rblFindNearby_0_0"" type=""radio"" name=""rblFindNearby_0"" value=""Car_Rental"" /><label for=""rblFindNearby_0_0"">Car Rental</label></td>" & _
        '  "<td><input id=""rblFindNearby_0_2"" type=""radio"" name=""rblFindNearby_0"" value=""Limousine"" /><label for=""rblFindNearby_0_2"">Limousine</label></td>" & _
        '  "<td rowspan=""2"" valign=""top"" style=""padding-left:5px;padding-top:2px;"">" & _
        '  "<input type=""button"" value=""Find"" style=""font-size:0.9em;"" id=""bttnFindNearby_0"" " & _
        '   "<td style=""padding-bottom:3px;""><input id=""rblFindNearby_0_1"" type=""radio"" name=""rblFindNearby_0"" value=""Hotel"" /><label for=""rblFindNearby_0_1"">Hotel</label></td>" & _
        '  "<td style=""padding-bottom:3px;""><input id=""rblFindNearby_0_3"" type=""radio"" name=""rblFindNearby_0"" value=""Taxi"" /><label for=""rblFindNearby_0_3"">Taxi</label></td>" & _
        ' "</tr>" & _
        '"</table>" & _
        '"</td></tr>"


        'Dim myHtml As String = "</br></br>Click <a href=\'QuoteTrip.aspx?origAirportCode=KHTS\' target=\'_blank\'>here</a> for departure airport" & _
        '"</br></br>Click <a href=\'QuoteTrip.aspx?destairportcode=KHTS\' target=\'_blank\'>here</a> for destination airport" & _
        ' "<input type=""button"" value=""Fly From: KHTS"" style=""font-size:0.9em;"" id=""bttnFindNearby"" " & _
        '"OnClick = " & "bttnNewFlightRequest_Click" & _
        ' "<input type=""button"" value=""Fly To: KHTS"" style=""font-size:0.9em;"" id=""bttnFlyTo"" "



        'Dim latLong As LatLong = Session("origlat")
        '20120424 - pab - convert from mappoint to bing
        'Dim oMapping As New Mapping



        '20120424 - pab - convert from mappoint to bing
        'Dim routeInfo As MapPointService.Route = oMapping.GetRoute(latLong.Latitude, _
        '                           latLong.Longitude, CDbl(v), CDbl(h))
        'Dim time As Double = 0
        'Dim timeHrs As Integer = 0
        'Dim timeMins As Integer = 0
        'Dim timeDisplay As String = String.Empty
        'Dim distance As Double = 0

        '20120424 - pab - convert from mappoint to bing
        'If Not routeInfo Is Nothing Then
        '    time = routeInfo.Itinerary.TripTime
        '    distance = Math.Round(routeInfo.Itinerary.Distance, 2)
        'End If
        'If time / 60 >= 60 Then
        '    timeHrs = CInt(Math.Floor(time / 3600))
        '    timeMins = CInt(Math.Floor((time - timeHrs * 3600) / 60))
        '    timeDisplay = timeHrs.ToString & " hr " & timeMins.ToString & " min"
        'Else 'minutes only
        '    timeDisplay = Math.Floor(time / 60).ToString & " min"
        'End If

        '20101029 - pab - truncate lat and lon
        v = Math.Round(v, 2)
        h = Math.Round(h, 2)

        'add pushpin
        '_mapData &= "addPushpin('" & CStr(dr("LocationID")) & "_" & (i - 1).ToString & "', new VELatLong(" & CStr(dr("Latitude")) & _
        '                ", " & CStr(dr("Longitude")) & "), '" & CStr(dr("LocationID")) & " - " & CStr(dr("FacilityName")).Replace("'", "\'") & "', '" & _
        '"<table border=""0"" cellpadding=""0"" cellspacing=""1"">" '& _
        ''"<tr><td align=""right"" style=""padding-right:2px;padding-top:2px;"">" & _
        '"timex:</td><td align=""left"" style=""padding-top:2px;width:75%;"">" & timeDisplay & _
        '"</td></tr>" & _
        '"<tr><td align=""right"" style=""padding-top:2px;padding-right:2px;padding-bottom:5px;"">" & _
        '"Distance:</td><td align=""left"" style=""padding-top:2px;padding-bottom:5px;"">" & CStr(distance) & " mi " & _
        '"</td></tr>"

        Dim myHtml As String

        'myHtml = "<br><tr><td align=""left"" style=""padding-right:2px;padding-top:2px;"">" & _
        '"timex:</td><td align=""left"" style=""padding-top:2px;width:75%;"">" & timeDisplay & _
        '"</td></tr>" & _
        '"<br>" & _
        '"<tr><td align=""left"" style=""padding-top:2px;padding-right:2px;padding-bottom:5px;"">" & _
        '"Distance:</td><td align=""left"" style=""padding-top:2px;padding-bottom:5px;"">" & CStr(distance) & " mi " & _
        '"</td></tr>"


        myHtml = ""

        '"<input type=""button"" value=""Fly To: KHTS"" style=""font-size:0.9em;"" id=""bttnFlyTo"" "





        '  Dim myHtml As String
        '  myHtml = "<input type=""button"" value=""Fly From: KHTS"" style=""font-size:0.9em;"" id=""bttnFindNearby"" " & _
        '"OnClick= {__doPostBack(\'bttnFindNearby_0\', rblValue }" & _
        ' "<input type=""button"" value=""Fly To: KHTS"" style=""font-size:0.9em;"" id=""bttnFlyTo"" "





        'Dim myHtml As String
        'myHtml = "<input type=""button"" value=""Find"" style=""font-size:0.9em;"" id=""bttnFindNearby_0"" " & _
        '                  "onclick=""var rblValue = 'KHTS'); {__doPostBack(\'bttnFindNearby_0\', rblValue };"">"




        'chg3620 - 20100920 - bd - fix javascript error
        'pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
        '"shape2.SetTitle('" & title & "');" & _
        '"shape2.SetDescription('" & description & "');" & _
        '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
        '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_blue.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & title & "</span></td></tr></table>"";" & _
        '"var spec = new VECustomIconSpecification();" & _
        '"spec.CustomHTML = icon;" & _
        '"shape2.SetCustomIcon(spec);" & _
        '"map0.AddShape(shape2);"

        '20120502 - pab - get rid of the annoying single quotes surrounding airport code
        'pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
        '"shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" & _
        '"shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" & _
        '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
        '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_blue.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & AntiXss.JavaScriptEncode(title) & "</span></td></tr></table>"";" & _
        '"var spec = new VECustomIconSpecification();" & _
        '"spec.CustomHTML = icon;" & _
        '"shape2.SetCustomIcon(spec);" & _
        '"map0.AddShape(shape2);"
        pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" &
        "shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" &
        "shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" &
        "shape2.SetLineColor(new VEColor(" & colors & "));" &
        "var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_blue.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>"
        If InStr(title, "/") > 0 Or InStr(title, "\") > 0 Or InStr(title, ".") > 0 Or InStr(title, "'") > 0 Then
            pushpin0 &= AntiXss.JavaScriptEncode(title)
        Else
            pushpin0 &= title
        End If
        pushpin0 &= "</span></td></tr></table>"";" &
        "var spec = new VECustomIconSpecification();" &
        "spec.CustomHTML = icon;" &
        "shape2.SetCustomIcon(spec);" &
        "map0.AddShape(shape2);"

        If color = "start" Then
            'chg3620 - 20100920 - bd - fix javascript error
            'pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
            '"shape2.SetTitle('" & "" & "');" & _
            '"shape2.SetDescription('" & "" & "');" & _
            '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
            '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/starting_point_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & title & "</span></td></tr></table>"";" & _
            '"var spec = new VECustomIconSpecification();" & _
            '"spec.CustomHTML = icon;" & _
            '"shape2.SetCustomIcon(spec);" & _
            '"map0.AddShape(shape2);"

            '20120502 - pab - get rid of the annoying single quotes surrounding airport code
            'pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
            '"shape2.SetTitle('" & "" & "');" & _
            '"shape2.SetDescription('" & "" & "');" & _
            '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
            '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/starting_point_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & AntiXss.JavaScriptEncode(title) & "</span></td></tr></table>"";" & _
            '"var spec = new VECustomIconSpecification();" & _
            '"spec.CustomHTML = icon;" & _
            '"shape2.SetCustomIcon(spec);" & _
            '"map0.AddShape(shape2);"
            pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" &
            "shape2.SetTitle('" & "" & "');" &
            "shape2.SetDescription('" & "" & "');" &
            "shape2.SetLineColor(new VEColor(" & colors & "));" &
            "var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/starting_point_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>"
            If InStr(title, "/") > 0 Or InStr(title, "\") > 0 Or InStr(title, ".") > 0 Or InStr(title, "'") > 0 Then
                pushpin0 &= AntiXss.JavaScriptEncode(title)
            Else
                pushpin0 &= title
            End If
            pushpin0 &= "</span></td></tr></table>"";" &
                "var spec = new VECustomIconSpecification();" &
                "spec.CustomHTML = icon;" &
                "shape2.SetCustomIcon(spec);" &
                "map0.AddShape(shape2);"
        End If

        If color = "selected" Then
            'chg3620 - 20100920 - bd - fix javascript error
            'pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
            '"shape2.SetTitle('" & title & "');" & _
            '"shape2.SetDescription('" & description & "');" & _
            '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
            '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & title & "</span></td></tr></table>"";" & _
            '"var spec = new VECustomIconSpecification();" & _
            '"spec.CustomHTML = icon;" & _
            '"shape2.SetCustomIcon(spec);" & _
            '"map0.AddShape(shape2);"

            '20120502 - pab - get rid of the annoying single quotes surrounding airport code
            'pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
            '"shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" & _
            '"shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" & _
            '"shape2.SetLineColor(new VEColor(" & colors & "));" & _
            '"var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>" & AntiXss.JavaScriptEncode(title) & "</span></td></tr></table>"";" & _
            '"var spec = new VECustomIconSpecification();" & _
            '"spec.CustomHTML = icon;" & _
            '"shape2.SetCustomIcon(spec);" & _
            '"map0.AddShape(shape2);"
            pushpin0 = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" &
            "shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" &
            "shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" &
            "shape2.SetLineColor(new VEColor(" & colors & "));" &
            "var icon = ""<table border='0' cellpadding='0' cellspacing='0'><tr><td><img src='Images/airport_red.gif' alt='' title=''/></td><td><span style='font-size:x-small;font-weight:bold;text-decoration:none;font-family:tahoma;color:Black;border:solid 0px Black;background-color:White;padding:1px;' onmouseover='this.style.cursor=&quot;pointer&quot;;' onmouseout='this.style.cursor=&quot;default&quot;;'>"
            If InStr(title, "/") > 0 Or InStr(title, "\") > 0 Or InStr(title, ".") > 0 Or InStr(title, "'") > 0 Then
                pushpin0 &= AntiXss.JavaScriptEncode(title)
            Else
                pushpin0 &= title
            End If
            pushpin0 &= "</span></td></tr></table>"";" &
                "var spec = new VECustomIconSpecification();" &
                "spec.CustomHTML = icon;" &
                "shape2.SetCustomIcon(spec);" &
                "map0.AddShape(shape2);"
        End If


        pushpin0 = Replace(pushpin0, "KHTS", title)
        pushpin0 = Replace(pushpin0, "shape2", shapename)

        '"var icon = ""<div style='font-size:12px;font-weight:bold;border:solid 2px Black;background-color:Aqua;width:50px;'>Custom</div>"";" & _
        '        "var spec = new VECustomIconSpecification();" & _
        '        "spec.CustomHTML = icon;" & _
        '        "spec.Image = 'http://www.geocities.com/cmpoet/Airplane1.png'" & _
        '        "shape2.SetCustomIcon(spec);" & _

        '20111229 - pab
        AirTaxi.post_timing("pushpin0 End  " & Now.ToString)

    End Function

    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function pushpinairport(ByVal airport As String, ByVal color As String, ByVal title As String, ByVal description As String,
                                   ByVal shapename As String, ByVal carrierid As Integer) As String

        '20111229 - pab
        AirTaxi.post_timing("pushpinairport Start  " & Now.ToString)

        '20110926 - pab - update web services
        'Dim ws As New CoastalAviation.Service 'connect_it_coastal.Service
        '20120201 - pab - changes for azure
        'Dim ws As New AviationService1_9.Service 'connect_it_coastal.Service
        '20120423 - pab - call azure web service
        'Dim ws As New AviationService1_10.Service
        Dim ws As New AviationWebService1_10.WebService1

        Dim fromairport As String
        fromairport = ws.AirportLongLat("pbaumgart@ctgi.com", "123", carrierid, airport)

        '20090706 - pab - fix airport locations 
        Dim oMapping As New Mapping
        Dim latLong As LatLong = Nothing
        Dim ds As DataSet = Nothing

        ds = oMapping.GetAirportInformationByAirportCode(airport)
        latLong = New LatLong
        If Not ds Is Nothing Then
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    If ds.Tables(0).Rows.Count > 0 Then
                        latLong.Latitude = CDbl(ds.Tables(0).Rows(0)("latitude"))
                        latLong.Longitude = CDbl(ds.Tables(0).Rows(0)("longitude"))
                    End If
                End If
            End If
        End If


        Dim v, h As String
        '20090706 - pab - fix airport locations 
        'v = InBetween(1, fromairport, "<Latitude>", "</Latitude>")
        'h = InBetween(1, fromairport, "<Longitude>", "</Longitude>")
        '20101029 - pab - truncate lat and lon
        v = Math.Round(latLong.Longitude, 2).ToString
        h = Math.Round(latLong.Latitude, 2).ToString


        Dim colors As String
        colors = "0,100,150,1.0" 'blue
        If UCase(color) = "GREEN" Then colors = "0,150,100,1.0"


        'chg3620 - 20100920 - bd - fix javascript error
        'pushpinairport = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" & _
        '"shape2.SetTitle('" & title & "');" & _
        '"shape2.SetDescription('" & description & "');" & _
        ' "shape2.SetLineColor(new VEColor(" & colors & "));" & _
        ' "map0.AddShape(shape2);"
        pushpinairport = "var shape2= new VEShape(VEShapeType.Pushpin, new VELatLong(" & v & ", " & h & "));" &
        "shape2.SetTitle(" & AntiXss.JavaScriptEncode(title) & ");" &
        "shape2.SetDescription(" & AntiXss.JavaScriptEncode(description) & ");" &
         "shape2.SetLineColor(new VEColor(" & colors & "));" &
         "map0.AddShape(shape2);"



        pushpinairport = Replace(pushpinairport, "shape2", shapename)


        '20111229 - pab
        AirTaxi.post_timing("pushpinairport End  " & Now.ToString)


    End Function


    Shared Function InBetween(ByVal Start As Integer, ByVal work As String, ByVal target As String, ByVal target2 As String) As String

        Dim pos As Integer
        Dim pos2 As Integer
        Dim work1 As String


        If Start = 0 Then
            InBetween = ""
            Exit Function
        End If

        pos = InStr(Start, work, target, vbTextCompare)

        pos = pos + Len(target)
        pos2 = InStr(pos, work, target2, vbTextCompare)

        If pos = 0 + Len(target) Or pos2 = 0 Then
            InBetween = ""
            Exit Function
        End If


        work1 = Mid(work, pos, pos2 - pos)

        InBetween = work1

    End Function


    '20120905 - pab - run time improvements
    Private Function graphpath(ByVal apt As String, ByVal color As String, ByVal type As String, ByVal latLong As LatLong, ByVal facilityname As String) As String

        AirTaxi.post_timing("Graph Path Start  " & Now.ToString)

        Dim fromairport As String

        fromairport = "<Latitude>" & latLong.Latitude.ToString & "</Latitude><Longitude>" & latLong.Longitude.ToString & "</Longitude>"

        latLong.Latitude = Math.Round(latLong.Latitude, 2)
        latLong.Longitude = Math.Round(latLong.Longitude, 2)

        Dim title As String
        Select Case type
            Case "D"
                title = "Empty Leg " & title
            Case "R"
                title = "Revenue Leg " & title
            Case "M"
                title = "Maint. Leg " & title
            Case "O"
                title = "Ops Leg " & title
        End Select

        graphpath = ""

        If InStr(fromairport, "<error>") = 0 Then

            graphpath = graphpath & pushpin(latLong.Latitude.ToString, latLong.Longitude.ToString, color, apt, facilityname, "point1") & vbCr & vbLf

            Dim lat As String = latLong.Latitude.ToString
            Dim lon As String = latLong.Longitude.ToString

            If lat.Trim.Length > 0 And lon.Trim.Length > 0 Then

                If _arrCount = 0 Then
                    _mapData &= "map.SetCenter(new VELatLong(""" & lat & """, """ & lon & """));"
                End If

                _arrCount += 1
            End If
        End If

        AirTaxi.post_timing("Graph Path End  " & Now.ToString)

    End Function

    '20120905 - pab - run time improvements
    Private Function graphpath0(ByVal apt As String, ByVal color As String, ByVal type As String, ByVal latlong As LatLong, ByVal facilityname As String) As String


        AirTaxi.post_timing("Graph Path 0 Start  " & Now.ToString)

        Dim fromairport As String = fromairport = "<Latitude>" & latlong.Latitude.ToString & "</Latitude><Longitude>" & latlong.Longitude.ToString & "</Longitude>"

        '20101029 - pab - truncate lat and lon
        latlong.Latitude = Math.Round(latlong.Latitude, 2)
        latlong.Longitude = Math.Round(latlong.Longitude, 2)

        Dim title As String
        Select Case type
            Case "D"
                title = "Empty Leg " & title
            Case "R"
                title = "Revenue Leg " & title
            Case "M"
                title = "Maint. Leg " & title
            Case "O"
                title = "Ops Leg " & title
        End Select

        graphpath0 = ""

        If InStr(fromairport, "<error>") = 0 Then

            graphpath0 = graphpath0 & pushpin0(latlong.Latitude.ToString, latlong.Longitude.ToString, color, apt, facilityname, "point1") & vbCr & vbLf

            Dim lat As String = latlong.Latitude.ToString
            Dim lon As String = latlong.Longitude.ToString

            If lat.Trim.Length > 0 And lon.Trim.Length > 0 Then

                If _arrCount0 = 0 Then
                    _mapData0 &= "map0.SetCenter(new VELatLong(""" & lat & """, """ & lon & """));"
                End If

                _arrCount0 += 1
            End If
        End If

        AirTaxi.post_timing("Graph Path 0 end  " & Now.ToString)

    End Function

    'Private Function graphpath(ByVal apt As String, ByVal color As String, ByVal type As String) As String


    '    AirTaxi.post_timing("Graph Path Start  " & Now.ToString)





    '    Dim fromairport As String
    '    '  fromairport = wscas.AirportLongLat("pbaumgart@ctgi.com", "123", apt)
    '    'Dim toairport As String
    '    'toairport = ws.AirportLongLat(ConfigurationManager.AppSettings("connectuserid"), "123", toapt)

    '    '20090706 - pab - fix airport locations 
    '    Dim oMapping As New Mapping
    '    Dim latLong As LatLong = Nothing
    '    Dim ds As DataSet = Nothing

    '    ds = oMapping.GetAirportInformationByAirportCode(apt)
    '    latLong = New LatLong
    '    If Not ds Is Nothing Then
    '        If ds.Tables.Count > 0 Then
    '            If ds.Tables(0).Rows.Count > 0 Then
    '                If ds.Tables(0).Rows.Count > 0 Then
    '                    latLong.Latitude = CDbl(ds.Tables(0).Rows(0)("latitude"))
    '                    latLong.Longitude = CDbl(ds.Tables(0).Rows(0)("longitude"))
    '                    '                 If InStr(fromairport, "<error>") > 0 Then
    '                    fromairport = "<Latitude>" & latLong.Latitude.ToString & "</Latitude><Longitude>" & latLong.Longitude.ToString & "</Longitude>"
    '                    'End If
    '                End If
    '            End If
    '        End If
    '    End If

    '    '20101029 - pab - truncate lat and lon
    '    latLong.Latitude = Math.Round(latLong.Latitude, 2)
    '    latLong.Longitude = Math.Round(latLong.Longitude, 2)

    '    'Dim distance As String
    '    'distance = ws.getdistance(ConfigurationManager.AppSettings("connectuserid"), ConfigurationManager.AppSettings("connectpassword"), fromapt, toapt)


    '    Dim title As String
    '    'title = (fromapt & "-" & toapt)
    '    If type = "D" Then title = "Empty Leg " & title
    '    If type = "R" Then title = "Revenue Leg " & title
    '    If type = "M" Then title = "Maint. Leg " & title
    '    If type = "O" Then title = "Ops Leg " & title


    '    graphpath = ""

    '    '<Latitude>53.86527777</Latitude><Longitude>-1.65472222</Longitude>

    '    'If InStr(fromairport, "<error>") = 0 And InStr(toairport, "<error>") = 0 Then
    '    '       graphpath = graphpath & polyline(InBetween(1, fromairport, "<Latitude>", "</Latitude>"), InBetween(1, fromairport, "<Longitude>", "</Longitude>"), InBetween(1, toairport, "<Latitude>", "</Latitude>"), InBetween(1, toairport, "<Longitude>", "</Longitude>"), color, title, distance & " miles", "line1") & vbCr & vbLf
    '    ' End If

    '    ' jscriptcode = jscriptcode & polyline(46.5, -122.5, 50.5, -112.3, "blue", "Line2", "Line1Test", "line2")


    '    If InStr(fromairport, "<error>") = 0 Then


    '        '20090706 - pab - fix airport locations 
    '        'graphpath = graphpath & pushpin(InBetween(1, fromairport, "<Latitude>", "</Latitude>"), InBetween(1, fromairport, "<Longitude>", "</Longitude>"), color, apt, Class1.fname(apt), "point1") & vbCr & vbLf
    '        graphpath = graphpath & pushpin(latLong.Latitude.ToString, latLong.Longitude.ToString, color, apt, Class1.fname(apt), "point1") & vbCr & vbLf
    '        'If InStr(toairport, "<error>") = 0 Then graphpath = graphpath & pushpin(InBetween(1, toairport, "<Latitude>", "</Latitude>"), InBetween(1, toairport, "<Longitude>", "</Longitude>"), color, toapt, Class1.fname(toapt), "point2") & vbCr & vbLf

    '        '20090706 - pab - fix airport locations 
    '        'Dim lat As String = InBetween(1, fromairport, "<Latitude>", "</Latitude>")
    '        'Dim lon As String = InBetween(1, fromairport, "<Longitude>", "</Longitude>")
    '        Dim lat As String = latLong.Latitude.ToString
    '        Dim lon As String = latLong.Longitude.ToString

    '        If lat.Trim.Length > 0 And lon.Trim.Length > 0 Then

    '            '_mapData &= "vll = new VELatLong(" & InBetween(1, toairport, "<Latitude>", "</Latitude>") & ", " & InBetween(1, toairport, "<Longitude>", "</Longitude>") & ");" & ControlChars.CrLf

    '            '20101201 - pab - fix bimini map issue - not centering
    '            '_mapData &= "vll = new VELatLong(" & lat & ", " & lon & ");" & ControlChars.CrLf
    '            '_mapData &= "latLongArr[" & _arrCount.ToString & "] = vll;" & ControlChars.CrLf
    '            If _arrCount = 0 Then
    '                _mapData &= "map.SetCenter(new VELatLong(""" & lat & """, """ & lon & """));"
    '            End If

    '            _arrCount += 1
    '        End If
    '    End If


    '    AirTaxi.post_timing("Graph Path End  " & Now.ToString)


    'End Function

    'Private Function graphpath0(ByVal apt As String, ByVal color As String, ByVal type As String) As String


    '    AirTaxi.post_timing("Graph Path 0 Start  " & Now.ToString)


    '    ' Dim ws As New CoastalAviation.Service 'connect_it_coastal.Service

    '    ' AirTaxi.post_timing("Graph Path 0 get latlong  " & Now.ToString)

    '    Dim fromairport As String
    '    '   fromairport = wscas.AirportLongLat(ConfigurationManager.AppSettings("connectuserid"), ConfigurationManager.AppSettings("connectpassword"), apt)

    '    'AirTaxi.post_timing("Graph Path 0 get latlong end " & Now.ToString)


    '    'Dim toairport As String
    '    'toairport = ws.AirportLongLat(ConfigurationManager.AppSettings("connectuserid"), ConfigurationManager.AppSettings("connectpassword"), toapt)

    '    '20090706 - pab - fix airport locations 
    '    Dim oMapping As New Mapping
    '    Dim latLong As LatLong = Nothing
    '    Dim ds As DataSet = Nothing

    '    ds = oMapping.GetAirportInformationByAirportCode(apt)
    '    latLong = New LatLong
    '    If Not ds Is Nothing Then
    '        If ds.Tables.Count > 0 Then
    '            If ds.Tables(0).Rows.Count > 0 Then
    '                If ds.Tables(0).Rows.Count > 0 Then
    '                    latLong.Latitude = CDbl(ds.Tables(0).Rows(0)("latitude"))
    '                    latLong.Longitude = CDbl(ds.Tables(0).Rows(0)("longitude"))
    '                    ' If InStr(fromairport, "<error>") > 0 Then
    '                    fromairport = "<Latitude>" & latLong.Latitude.ToString & "</Latitude><Longitude>" & latLong.Longitude.ToString & "</Longitude>"
    '                    'End If
    '                End If
    '            End If
    '        End If
    '    End If

    '    '20101029 - pab - truncate lat and lon
    '    latLong.Latitude = Math.Round(latLong.Latitude, 2)
    '    latLong.Longitude = Math.Round(latLong.Longitude, 2)

    '    'Dim distance As String
    '    'distance = ws.getdistance(ConfigurationManager.AppSettings("connectuserid"), ConfigurationManager.AppSettings("connectpassword"), fromapt, toapt)


    '    Dim title As String
    '    'title = (fromapt & "-" & toapt)
    '    If type = "D" Then title = "Empty Leg " & title
    '    If type = "R" Then title = "Revenue Leg " & title
    '    If type = "M" Then title = "Maint. Leg " & title
    '    If type = "O" Then title = "Ops Leg " & title


    '    graphpath0 = ""

    '    '<Latitude>53.86527777</Latitude><Longitude>-1.65472222</Longitude>

    '    'If InStr(fromairport, "<error>") = 0 And InStr(toairport, "<error>") = 0 Then
    '    '       graphpath = graphpath & polyline(InBetween(1, fromairport, "<Latitude>", "</Latitude>"), InBetween(1, fromairport, "<Longitude>", "</Longitude>"), InBetween(1, toairport, "<Latitude>", "</Latitude>"), InBetween(1, toairport, "<Longitude>", "</Longitude>"), color, title, distance & " miles", "line1") & vbCr & vbLf
    '    ' End If

    '    ' jscriptcode = jscriptcode & polyline(46.5, -122.5, 50.5, -112.3, "blue", "Line2", "Line1Test", "line2")


    '    If InStr(fromairport, "<error>") = 0 Then


    '        '20090706 - pab - fix airport locations 
    '        'graphpath = graphpath & pushpin(InBetween(1, fromairport, "<Latitude>", "</Latitude>"), InBetween(1, fromairport, "<Longitude>", "</Longitude>"), color, apt, Class1.fname(apt), "point1") & vbCr & vbLf
    '        graphpath0 = graphpath0 & pushpin0(latLong.Latitude.ToString, latLong.Longitude.ToString, color, apt, Class1.fname(apt), "point1") & vbCr & vbLf
    '        'If InStr(toairport, "<error>") = 0 Then graphpath = graphpath & pushpin(InBetween(1, toairport, "<Latitude>", "</Latitude>"), InBetween(1, toairport, "<Longitude>", "</Longitude>"), color, toapt, Class1.fname(toapt), "point2") & vbCr & vbLf

    '        '20090706 - pab - fix airport locations 
    '        'Dim lat As String = InBetween(1, fromairport, "<Latitude>", "</Latitude>")
    '        'Dim lon As String = InBetween(1, fromairport, "<Longitude>", "</Longitude>")
    '        Dim lat As String = latLong.Latitude.ToString
    '        Dim lon As String = latLong.Longitude.ToString

    '        If lat.Trim.Length > 0 And lon.Trim.Length > 0 Then

    '            '_mapData &= "vll = new VELatLong(" & InBetween(1, toairport, "<Latitude>", "</Latitude>") & ", " & InBetween(1, toairport, "<Longitude>", "</Longitude>") & ");" & ControlChars.CrLf

    '            '20101201 - pab - fix bimini map issue - not centering
    '            '_mapData0 &= "vll = new VELatLong(" & lat & ", " & lon & ");" & ControlChars.CrLf
    '            '_mapData0 &= "latLongArr[" & _arrCount0.ToString & "] = vll;" & ControlChars.CrLf
    '            If _arrCount0 = 0 Then
    '                _mapData0 &= "map0.SetCenter(new VELatLong(""" & lat & """, """ & lon & """));"
    '            End If

    '            _arrCount0 += 1
    '        End If
    '    End If


    '    AirTaxi.post_timing("Graph Path 0 end  " & Now.ToString)


    'End Function

    '--~~DHA 09/08/2010 - Functions needed to support multileg reimplementation
    Private Function MultiLegDetermineLatLong(ByVal airportcode As String, ByVal carrierid As Integer) As String
        'need to determine lat/long for each leg to make sure airports on each leg are not outside the service area

        '20111229 - pab
        AirTaxi.post_timing("MultiLegDetermineLatLong Start  " & Now.ToString)

        Dim retval As String = Nothing
        Dim ds As DataSet = New DataSet
        ds = oMapping.GetAirportInformationByAirportCode(airportcode)
        Dim latLong As New LatLong

        If ds.Tables(0).Rows.Count > 0 Then
            '20100622 - pab - fix floating point error
            'latLong.Latitude = CDbl(ds.Tables(0).Rows(0)("latitude"))
            'latLong.Longitude = CDbl(ds.Tables(0).Rows(0)("longitude"))
            latLong.Latitude = Math.Round(CDbl(ds.Tables(0).Rows(0)("latitude")), 13)
            latLong.Longitude = Math.Round(CDbl(ds.Tables(0).Rows(0)("longitude")), 13)

            Dim dt As New DataTable
            dt = GetAirportsLatLong(latLong, carrierid)


            If dt.Rows.Count = 0 Then
                retval = "X|Outside Service Area " & latLong.Latitude.ToString & " " & latLong.Longitude.ToString & "|" & "X"
            Else
                retval = "Y|" & latLong.Latitude & "|" & latLong.Longitude
            End If
        End If

        '20111229 - pab
        AirTaxi.post_timing("MultiLegDetermineLatLong End  " & Now.ToString)

        Return retval

    End Function

    '--~~DHA 09/08/2010 - Functions needed to support multileg reimplementation
    Private Function MultiLegErrors(ByVal carrierid As Integer) As String

        '20111229 - pab
        AirTaxi.post_timing("MultiLegErrors Start  " & Now.ToString)

        Dim retval As String = Nothing
        Dim aLegs As New ArrayList
        Dim strErr, strAreaErr As String

        Dim dt As DateTime
        Dim r As Integer = 0
        Dim dtArrive As DateTime
        Dim dtDepart As DateTime

        Dim totalLegs = dtflights.Rows.Count - 1

        'For multileg determine the initial origination and final destintation
        'Dim mOrig As String
        'Dim mDest As String
        'mOrig = dtflights.Rows(0).Item("Origin")
        'mDest = dtflights.Rows(totalLegs).Item("Destination")

        'determine destination - see if last leg destination = origin
        'If mOrig = mDest Then
        '    mDest = dtflights.Rows(totalLegs).Item("Origin")
        'End If

        'Session("destAirportCode") = mDest
        'Session("origAirportCode") = mOrig

        For Each d As DataRow In dtflights.Rows
            strErr = Nothing        'initialize string for each leg
            'org <> dest
            If d.Item("Origin") = d.Item("Destination") Then
                strErr = strErr & "Origin and Destination Airport Codes Must Be Different. <br/>"
            End If

            'dates not < now
            dt = CDate(d.Item("Departs"))

            If dt < Now Then
                strErr = strErr & "Departure Dates must be in the future. " & dt.ToString & "<br/>"
            End If

            'dates not overlapping --we may need to add 15 minutes in between flights...CHECK with Richard
            Dim oAccess As New DataAccess
            Dim waitminutes = oAccess.GetSetting(carrierid, "AutoAssignPilotMinutesPrior")

            'current row = r -- get value of depart time of next record compare to arrive of this leg
            dtArrive = dtflights.Rows(r).Item("Arrives")
            Dim dtArrivaltime As DateTime = dtArrive.AddMinutes(waitminutes)

            If r < totalLegs Then
                dtDepart = dtflights.Rows(r + 1).Item("Departs")
                If dtArrivaltime > dtDepart Then
                    strErr = strErr & "Legs " & (r + 1).ToString & " and " & (r + 2).ToString & " cannot overlap <br/>"
                End If
            End If

            'airports outside of service area. - check to make sure that airports in leg are not outside service area
            'we need this for multileg because we have only checked the origin of the first leg and the destination of the last leg ONLY

            'we have the aiport codes:
            Dim aResult() As String
            'Dim savelatlong As LatLong

            strAreaErr = MultiLegDetermineLatLong(d.Item("Origin"), carrierid)
            aResult = Split(strAreaErr, "|")

            If aResult(0) = "X" Then
                strErr = strErr & "Origination" & strAreaErr & "<br/>"
            End If
            'this is not for catching errors - it is for saving session variable
            'If d.Item("Origin") = mOrig Then
            '    If aResult(0) <> "X" Then
            '        'save lat long in Session("origlat")
            '        savelatlong = New LatLong
            '        savelatlong.Latitude = aResult(1).ToString
            '        savelatlong.Longitude = aResult(2).ToString
            '        Session("origlat") = savelatlong
            '    End If
            'End If


            strAreaErr = MultiLegDetermineLatLong(d.Item("Destination"), carrierid)
            aResult = Split(strAreaErr, "|")
            If aResult(0) = "X" Then
                strErr = strErr & "Destination" & strAreaErr & "<br/>"
            End If


            'this is not for catching errors - it is for saving session variable
            'If d.Item("Destination") = mDest Then
            '    'save lat-long in Session("destlat")
            '    If aResult(0) <> "X" Then
            '        savelatlong = New LatLong
            '        savelatlong.Latitude = aResult(1)
            '        savelatlong.Longitude = aResult(2)
            '        Session("destlat") = savelatlong
            '    End If
            'End If

            aLegs.Add(strErr)

            r = r + 1
        Next


        Dim iTotal As Integer = (aLegs.Count) - 1
        For i As Integer = 0 To iTotal
            If Not IsNothing(aLegs(i)) Then
                'retval = retval & "Leg #" & (i + 1).ToString & "->" & aLegs(i)
                retval = retval & "<u>" & "Leg#" & (i + 1).ToString & "</u><br/>" & aLegs(i)
                'retval = retval & "\n" & "-------------------------------\n"
            End If
        Next i

        '20111229 - pab
        AirTaxi.post_timing("MultiLegErrors End  " & Now.ToString)

        Return retval

    End Function

    Protected Sub cmdEURoutes(ByVal O As DataTable, ByVal d As DataTable, ByVal carrierid As Integer) 'Handles cmdEURoutes.Click

        '20131015 - pab - add more error handling to catch error - Object reference not set to an instance of an object
        Try

            '20111229 - pab
            AirTaxi.post_timing("cmdEURoutes Start  " & Now.ToString)

            If O.Rows.Count = 0 Or d.Rows.Count = 0 Then
                Exit Sub
            End If

            '20101201 - pab - fix bimini map issue - not centering
            '_mapData = "var latLongArr = new Array();" '(" & dt.Rows.Count.ToString & ");" & ControlChars.CrLf
            '_mapData0 = "var latLongArr = new Array();" '(" & dt.Rows.Count.ToString & ");" & ControlChars.CrLf



            ' Dim jscript_template As String = ""
            ''   If jscript_template = "" Then

            'Dim fs As New FileStream("C:\Inetpub\wwwroot\TrafficMap\jscripttemplate.html", FileMode.Open, FileAccess.Read)
            'Dim sw As New StreamReader(fs)

            'jscript_template = sw.ReadToEnd.ToString

            'sw.Close()
            'fs.Close()
            'sw.Dispose()
            'fs.Dispose()
            ''  End If

            '  Dim ws As New CoastalAviation.Service 'connect_it_coastal.Service



            'Dim cn As New ADODB.Connection
            'Dim rs As New ADODB.Recordset


            'If cn.State = 1 Then cn.Close()
            'If cn.State = 0 Then
            '    cn.ConnectionString = "PROVIDER=MSDASQL;driver={SQL Server};server=(local);uid=sa;password=CoastalPass1;database=;" '" "PROVIDER=MSDASQL;driver={SQL Server};server=localhost;uid=sa;password=ctgi;database=airtaxi"
            '    ' cn.ConnectionString = "server=(local);uid=cmp;password=coastal;database=airtaxi"""
            '    cn.Open()
            'End If

            'If rs.State = 1 Then rs.Close()


            'Dim req As String
            'req = "select * from serviceairports"

            ''        rs.Open(req)
            ''20090604 - pab - display messages in German
            ''rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockPessimistic)
            'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

            Dim jscriptcode As String = ""

            Dim jscriptcode0 As String = ""



            'Do While rs.EOF = False

            '    Dim apt As String
            '    apt = rs.Fields("locationid").Value


            '    jscriptcode = jscriptcode & graphpath(apt, "red", "R")

            '    rs.MoveNext()

            'Loop
            'Me.drpToAirport.Items.Clear()
            'Me.drpFromAirport.Items.Clear()

            'departOrigAirportCode As String = String.Empty
            'Private _departDestAirportCode


            AirTaxi.post_timing("EU Routes Add Orig  " & Now.ToString)

            Dim latlong As New LatLong

            '20100625 - pab - fix error - Conversion from type 'DBNull' to type 'String' is not valid
            If Not IsNothing(O) Then
                For Each dr As DataRow In O.Rows

                    Dim ac As String = String.Empty

                    If Not IsDBNull(dr("LocationID")) Then ac = dr("LocationID").ToString.Trim

                    '20120502 - pab - fix selectied airport not showing up red 
                    'If ac <> "" And ac = _origAirportCode And _origAirportCode <> "" Then
                    '20120905 - pab - run time improvements
                    Dim facilityname As String = String.Empty
                    If Not IsDBNull(dr("latitude")) Then latlong.Latitude = CDbl(dr("latitude"))
                    If Not IsDBNull(dr("longitude")) Then latlong.Longitude = CDbl(dr("longitude"))
                    If Not IsDBNull(dr("facilityname")) Then facilityname = dr("facilityname").ToString.Trim & " (" & ac & ")"
                    If ac <> "" And (ac = _origAirportCode Or _origAirportCode = "") Then
                        '20120425 - pab - cleanup code - occurs multiple times - move to airtaxi class
                        'jscriptcode0 = jscriptcode0 & graphpath(ac, "selected", "R")
                        jscriptcode0 = jscriptcode0 & graphpath0(ac, "selected", "R", latlong, facilityname)
                    Else
                        '20111221 - pab - fix error if ac blank
                        'jscriptcode0 = jscriptcode0 & graphpath0(ac, "red", "R")
                        '20120425 - pab - cleanup code - occurs multiple times - move to airtaxi class
                        'If ac <> "" Then jscriptcode0 = jscriptcode0 & graphpath(ac, "red", "R")
                        If ac <> "" Then jscriptcode0 = jscriptcode0 & graphpath0(ac, "red", "R", latlong, facilityname)
                    End If


                    '20111221 - pab - fix error if ac blank
                    If ac <> "" Then

                        Dim x As New WebControls.ListItem
                        x.Value = ac
                        'x.Text = Trim(fname(ac))
                        x.Text = facilityname
                        'Me.drpFromAirport.Items.Add(x)

                        ' If Not IsNothing(Session("clickairport")) Then
                        Dim ii As Long
                        ii = ii + 1

                        If _origAirportCode = "" Then
                            If ii = 1 Then
                                _origAirportCode = ac
                                Session("origAirportCode") = ac
                            End If

                        End If

                    End If

                    'End If

                Next
            End If


            AirTaxi.post_timing("EU Routes Add Dest  " & Now.ToString)


            '20100625 - pab - fix error - Conversion from type 'DBNull' to type 'String' is not valid
            If Not IsNothing(d) Then
                If d.Rows.Count > 0 Then
                    For Each dr As DataRow In d.Rows

                        Dim ac As String = String.Empty

                        If Not IsDBNull(dr("LocationID")) Then ac = dr("LocationID").ToString.Trim

                        '20120502 - pab - fix selectied airport not showing up red 
                        'If ac <> "" And ac = _destAirportCode And _destAirportCode <> "" Then
                        '20120905 - pab - run time improvements
                        Dim facilityname As String = String.Empty
                        If Not IsDBNull(dr("latitude")) Then latlong.Latitude = CDbl(dr("latitude"))
                        If Not IsDBNull(dr("longitude")) Then latlong.Longitude = CDbl(dr("longitude"))
                        If Not IsDBNull(dr("facilityname")) Then facilityname = dr("facilityname").ToString.Trim & " (" & ac & ")"
                        If ac <> "" And (ac = _destAirportCode Or _destAirportCode = "") Then
                            jscriptcode = jscriptcode & graphpath(ac, "selected", "R", latlong, facilityname)
                        Else
                            '20111221 - pab - fix error if ac blank
                            'jscriptcode = jscriptcode & graphpath(ac, "red", "R")
                            If ac <> "" Then jscriptcode = jscriptcode & graphpath(ac, "red", "R", latlong, facilityname)
                        End If


                        'If Not IsNothing(Session("clickairport")) Then

                        '20111221 - pab - fix error if ac blank
                        If ac <> "" Then

                            Dim x As New WebControls.ListItem
                            x.Value = ac
                            'x.Text = Trim(fname(ac))
                            x.Text = facilityname
                            'Me.drpToAirport.Items.Add(x)

                            Dim ii As Long
                            ii = ii + 1
                            If _destAirportCode = "" Then
                                If ii = 1 Then
                                    _destAirportCode = ac
                                    Session("destAirportCode") = _destAirportCode
                                End If


                            End If

                        End If

                    Next
                End If
            End If

            ' If IsNothing(Session("firstquote")) Then
            '20110926 - pab - fix error if dtflights is nothing
            If IsNothing(dtflights) Then
                dtflights = New DataTable
            End If

            If dtflights.Rows.Count = 0 Then

                ' _origAirportCode = Me.drpFromAirport.Items(1).Text
                ' _destAirportCode = Me.drpToAirport.Items(1).Text

                Session("firstquote") = Now

                '20110228 - pab - fix when passport fields are displayed
                Dim bIntl As Boolean = isflightintl(_origAirportCode, _destAirportCode)
                If bIntl Then
                    Session("international_type") = "intl"
                Else
                    Session("international_type") = "domestic"
                End If

                Dim x As String = Session("triptype").ToString


                If Session("triptype") = "R" Then
                    Call addleg("R", carrierid)
                    Call quote(carrierid)
                End If

                If Session("triptype") = "O" Then

                    Call addleg("L", carrierid)
                    Call quote(carrierid)

                End If

                '--------------------------------DHA ~~Multileg - 
                '20160117 - pab - quote multi-leg trips
                'If Session("triptype") = "M" Then
                '    Dim f As String
                '    Dim dMulti As DataTable = Session("MultiInfo")
                '    '20101027 - pab - fix error if dmulti is empty like when changing from round trip to multileg
                '    If dMulti Is Nothing Then
                '        f = ""
                '    Else
                '        For Each drMulti As DataRow In dMulti.Rows
                '            _origAirportCode = drMulti.Item("FromLocation")
                '            _destAirportCode = drMulti.Item("ToLocation")
                '            _departDateTime = CDate(CDate(drMulti.Item("DepartDate").ToString).ToString("d") & " " & drMulti.Item("DepartTime"))
                '            _returnDateTime = Nothing
                '            Call addleg("L", carrierid)
                '        Next
                '        f = MultiLegErrors(carrierid)
                '    End If
                '    If Not IsNothing(f) Then        'there is an error
                '        Me.lblItineraryError.Text = f
                '        'Me.lblMsg.Visible = True
                '        Session("reset") = "M"
                '        'Session("DoQuery") = "N"

                '        'Dim s As String = "alert('" & f & "')"
                '        'Dim sb As New StringBuilder

                '        'sb.Append("<script language='javascript'>")
                '        'sb.Append(s)
                '        'sb.Append("</script>")


                '        'ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "WO", sb.ToString, False)

                '        'clear variablees
                '        dtflights.Clear()

                '        '20111221 - pab multiple quotes
                '        dtflights2.Clear()

                '        'Me.gvServiceProviderMatrix.DataSource = dtflights
                '        'Me.gvServiceProviderMatrix.DataBind()
                '        '_origAirportCode = ""
                '        _destAirportCode = ""
                '        Session("ORIGAIRPORTCODE") = ""
                '        Session("DESTAIRPORTCODE") = ""

                '        'Session.Abandon()
                '        'Response.Redirect("QuoteTrip.aspx")


                '        Exit Sub
                '    End If
                '    Call quote(carrierid)

                '    '20140220 - pab - cleanup code
                '    'Me.gvMultiLeg.Visible = False
                '    'Me.pnlMultiLeg.Visible = False

                '    '20111216 - pab - use radio buttons for select per David
                '    'Me.gvServiceProviderMatrix.DataSource = dtflights
                '    'Me.gvServiceProviderMatrix.DataBind()
                '    Bind_gvServiceProviderMatrix()

                '    Me.gvServiceProviderMatrix.Visible = True

                '    '20111221 - pab - multiple quotes
                '    '20140220 - pab - cleanup code
                '    'Me.gvServiceProviderMatrixReturn.Visible = False

                '    'Response.Redirect("QuoteTrip.aspx")
                '    Session("DoQuery") = "Y"
                '    'Exit Sub



                'End If
                If Session("triptype") = "M" Then

                    Call addleg("M", carrierid)
                    Call quote(carrierid)

                End If
            End If



            'If Not Nothing(Session("origAirportCode")) Then


            'If _origAirportCode <> "" Then

            '    If Me.drpFromAirport.Items.FindByValue(_origAirportCode) IsNot Nothing Then
            '        Me.drpFromAirport.SelectedValue = _origAirportCode
            '    End If

            '    ' Me.drpFromAirport.Text = fname(_origAirportCode)
            'End If



            'If _destAirportCode <> "" Then

            '    If Me.drpToAirport.Items.FindByValue(_destAirportCode) IsNot Nothing Then Me.drpToAirport.SelectedValue = _destAirportCode
            'End If



            AirTaxi.post_timing("EU Routes Add OrigLat Push Pin  " & Now.ToString)


            latlong = Session("origlat")
            If latlong.Latitude <> 0 Then
                If Len(Session("origplace")) <> 3 Then
                    '20101029 - pab
                    'Dim a As String = pushpin0(latlong.Latitude.ToString, latlong.Longitude.ToString, "start", "", "", "point1") & vbCr & vbLf
                    Dim a As String = pushpin0(latlong.Latitude.ToString, latlong.Longitude.ToString, "start", "", "", "point6") & vbCr & vbLf
                    jscriptcode0 = jscriptcode0 & a
                End If
            End If



            AirTaxi.post_timing("EU Routes Add DestLat Push Pin  " & Now.ToString)


            latlong = Session("destlat")
            If latlong.Latitude <> 0 Then
                If Len(Session("destplace")) <> 3 Then
                    Dim a As String = pushpin(latlong.Latitude.ToString, latlong.Longitude.ToString, "end", "", "", "point6") & vbCr & vbLf
                    jscriptcode = jscriptcode & a
                End If
            End If





            latlong = Session("destlat")




            'Dim fromairport As String
            'fromairport = ws.AirportLongLat(ConfigurationManager.AppSettings("connectuserid"), ConfigurationManager.AppSettings("connectpassword"), Me.txtFrom.Text)
            'Dim toairport As String
            'toairport = ws.AirportLongLat(ConfigurationManager.AppSettings("connectuserid"), ConfigurationManager.AppSettings("connectpassword"), Me.txtTo.Text)




            'Dim distance As String
            'distance = ws.getdistance(ConfigurationManager.AppSettings("connectuserid"), ConfigurationManager.AppSettings("connectpassword"), Me.txtFrom.Text, Me.txtTo.Text)





            'If InStr(fromairport, "<error>") = 0 And InStr(toairport, "<error>") = 0 Then
            '    jscriptcode = jscriptcode & polyline(InBetween(1, fromairport, "<Latitude>", "</Latitude>"), InBetween(1, fromairport, "<Longitude>", "</Longitude>"), InBetween(1, toairport, "<Latitude>", "</Latitude>"), InBetween(1, toairport, "<Longitude>", "</Longitude>"), "green", Me.txtFrom.Text & "-" & Me.txtTo.Text, distance & " miles", "line1") & vbCr & vbLf
            'End If

            ' jscriptcode = jscriptcode & polyline(46.5, -122.5, 50.5, -112.3, "blue", "Line2", "Line1Test", "line2")


            'If InStr(fromairport, "<error>") = 0 Then jscriptcode = jscriptcode & pushpin(InBetween(1, fromairport, "<Latitude>", "</Latitude>"), InBetween(1, fromairport, "<Longitude>", "</Longitude>"), "green", Me.txtFrom.Text, fromairport, "point1") & vbCr & vbLf

            'If InStr(toairport, "<error>") = 0 Then jscriptcode = jscriptcode & pushpin(InBetween(1, toairport, "<Latitude>", "</Latitude>"), InBetween(1, toairport, "<Longitude>", "</Longitude>"), "green", Me.txtTo.Text, toairport, "point2") & vbCr & vbLf

            'jscript_template = Replace(jscript_template, "//Add Shapes Here", jscriptcode)

            'ensure all coords are shown
            '20101029 - pab - center map
            '_mapData = "map.SetCenter(new VELatLong(" & Math.Round(Session("destlat").Latitude, 2).ToString & _
            '                ", " & Math.Round(Session("destlat").Longitude, 2).ToString & "));" & ControlChars.CrLf
            '20101201 - pab - fix bimini map issue - not centering
            '_mapData &= "map.SetMapView(latLongArr);" & ControlChars.CrLf
            '_mapData &= "map.SetZoomLevel(8);;" & ControlChars.CrLf

            '20101029 - pab - center map
            '_mapData0 = "map0.SetCenter(new VELatLong(" & Math.Round(Session("origlat").Latitude, 2).ToString & _
            '                ", " & Math.Round(Session("origlat").Longitude, 2).ToString & "));" & ControlChars.CrLf
            '20101201 - pab - fix bimini map issue - not centering
            '_mapData0 &= "map0.SetMapView(latLongArr);" & ControlChars.CrLf
            '_mapData0 &= "map0.SetZoomLevel(8);;" & ControlChars.CrLf




            Me.myJS1 = jscriptcode + " " + _mapData

            '  Me.myJS0 = Me.myJS1 'pushpinairport("CGN", "red", "Origin", "My Origin", "pin") + " " + _mapData


            Me.myJS0 = jscriptcode0 + " " + _mapData0

            'If File.Exists("C:\Inetpub\wwwroot\TrafficMap\jscriptmap.html") Then Kill("C:\Inetpub\wwwroot\TrafficMap\jscriptmap.html")
            'Dim fs1 As New FileStream("C:\Inetpub\wwwroot\TrafficMap\jscriptmap.html", FileMode.CreateNew, FileAccess.Write)
            'Dim sw1 As New StreamWriter(fs1)

            'sw1.Write(jscript_template)

            'sw1.Close()
            'fs1.Close()
            'sw1.Dispose()
            'fs1.Dispose()


            'Response.Redirect("jscriptmap.html")

            AirTaxi.post_timing("EU Routes End Sub  " & Now.ToString)

        Catch ex As Exception
            '20131015 - pab - add more error handling to catch error - Object reference not set to an instance of an object
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "cmdEURoutes", "QuoteTrip.aspx.vb")
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.InsertEmailQueue(CInt(Session("carrierid")), "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                "QuoteTrip.aspx.vb cmdEURoutes error", s, False, "", "", "", False)

        End Try


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '20111229 - pab
        AirTaxi.post_timing("Page_Load Start  " & Now.ToString)

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        Dim ip As String = ""
        Dim internationalfees As Double = 0

        Try

            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            If Session("carrierid") Is Nothing Then
                Insertsys_log(0, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "QuoteTrip.aspx.vb")
            Else
                Insertsys_log(Session("carrierid"), appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - " & Session("carrierid").ToString, "QuoteTrip.aspx.vb")
            End If

            '20120830 - pab - try to fix telerik issue - ddls not working when round trip clicked
            Response.Cache.SetNoStore()

            '20160412 - pab - If Session("email") Is Nothing Then assume timeout and redirect to login page
            If Session("email") Is Nothing Then
                'Session.Abandon()
                Response.Redirect("CustomerLogin.aspx", True)

            End If

            '20111121 - pab - convert to single db
            Dim da As New DataAccess
            Dim dt As DataTable

            '20120503 - pab - run time improvements - execute on if not postback
            If Not IsPostBack Then

                Me.gvServiceProviderMatrix.Visible = True

                '20120123 - pab - fix passengers
                If Not IsNothing(Session("passengers")) Then
                    If IsNumeric(Session("passengers")) Then _passengers = CInt(Session("passengers"))
                End If

                '20111227 - pab - process call from external source
                If Not Request.QueryString("roundTrip") Is Nothing Then
                    If CBool(Request.QueryString("roundTrip")) = True Then
                        Session("triptype") = "R"
                        _roundTrip = True
                        Me.rblOneWayRoundTrip.SelectedValue = "RoundTrip"
                        'Me.r()
                    Else
                        '20160117 - pab - quote multi-leg trips
                        'Session("triptype") = "O"
                        _roundTrip = False
                        If Session("triptype").ToString = "O" Then
                            Me.rblOneWayRoundTrip.SelectedValue = "OneWay"
                        Else
                            Me.rblOneWayRoundTrip.SelectedValue = "MultiLeg"
                        End If
                    End If

                    '20171117 - pab - skip home page and go straight to quoting - do not pass go
                Else
                    Session("triptype") = "O"
                    _roundTrip = False
                    Me.rblOneWayRoundTrip.SelectedValue = "OneWay"

                End If
                '20140310 - pab - use table instead of parms
                'Dim dtquoterequest As DataTable
                'Dim qn As Integer = 0
                'If Not Request.QueryString("qn") Is Nothing Then
                '    If IsNumeric(Request.QueryString("qn")) Then
                '        qn = CInt(Request.QueryString("qn"))
                '        dtquoterequest = da.GetQuoteRequests(_carrierid, qn)
                '        If Not isdtnullorempty(dtquoterequest) Then
                '            If dtquoterequest.Rows(0).Item("roundTrip") = True Then
                '                Session("triptype") = "R"
                '                _roundTrip = True
                '            Else
                '                Session("triptype") = "O"
                '                _roundTrip = False
                '            End If
                '        End If
                '    End If
                'End If
                '20140107 - pab - where called from?
                'AirTaxi.Insertsys_log(0, appName, Left("Request.UrlReferrer " & Request.UrlReferrer.ToString, 500), "Page_Load", "QuoteTrip.aspx.vb")
                '20150317 - pab - remove acg branding
                'If Not Request.QueryString("acg") Is Nothing Then
                '    '_acg = Request.QueryString("acg")
                '    If Request.QueryString("acg") <> "" Then
                '        If Not Request.QueryString("actoinclude") Is Nothing Then
                '            Session("achourly") = Request.QueryString("actoinclude")
                '        End If
                '    End If
                'End If

                '20111221 - pab - multiple quotes
                '20120329 - pab - grids combined - do not show
                '20140220 - pab - cleanup code
                ''If Session("triptype") = "R" Then
                ''    Me.gvServiceProviderMatrixReturn.Visible = True
                ''Else
                'Me.gvServiceProviderMatrixReturn.Visible = False
                ''End If

                Dim ut As String = Session("usertype")

                If Session("usertype") = "A" Then
                    '      Me.lbladmin.Visible = True
                Else
                    '     Me.lbladmin.Visible = False
                End If

                '20160517 - pab - fix carrierid = 0 preventing quotes
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And CInt(Session("carrierid")) = 0 Then
                    Session("carrierid") = 65
                End If

                '20111121 - pab - convert to single db
                If IsNothing(CInt(Session("carrierid"))) Or CInt(Session("carrierid")) = 0 Then
                    '20160517 - pab - fix carrierid = 0 preventing quotes
                    AirTaxi.Insertsys_log(0, appName, Request.Url.Host & " carrierid null or 0 - user " & Session("email").ToString, "Page_Load", "QuoteTrip.aspx.vb")

                    '20160823 - pab - redirect to logon if carrierid lost
                    Response.Redirect("CustomerLogin.aspx", True)
                    'dt = da.GetProviderByAlias("Door2Door")
                    'If Not IsNothing(dt) Then
                    '    If dt.Rows.Count > 0 Then
                    '        _carrierid = CInt(dt.Rows(0).Item("carrierid").ToString)
                    '    End If
                    'End If
                End If

                '20130930 - pab - change email from
                '20171121 - pab - fix carriers changing midstream - change to Session variables
                If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""
                If Session("emailfrom").ToString = "" Then
                    Session("emailfrom") = da.GetSetting(CInt(Session("carrierid")), "emailsentfrom")
                End If
                If IsNothing(Session("emailfromquote")) Then Session("emailfromquote") = ""
                If Session("emailfromquote") = "" Then
                    Session("emailfromquote") = da.GetSetting(CInt(Session("carrierid")), "emailsentfromQuote")
                End If

                'rk 6.7.2010 load company branding
                '20171121 - pab - fix carriers changing midstream - change to Session variables
                'companyname = da.GetSetting(CInt(Session("carrierid")), "companyname")
                ''20150119 - pab - use blob storage for carrier logos and aircraft images instead of sql
                ''companylogo = da.GetSetting(_carrierid, "companylogo")
                'companysplash = da.GetSetting(CInt(Session("carrierid")), "companysplash")
                'companywelcomeleft = da.GetSetting(CInt(Session("carrierid")), "companywelcomeleft")
                'companywelcomeright = da.GetSetting(CInt(Session("carrierid")), "companywelcomeright")

                '20140220 - pab - cleanup code
                'lblwelcome.Text = companyname

                'rk 11/18/2010 pull in a banner and hide form names
                'companybanner = da.GetSetting(_carrierid, "companybanner")

                'If companybanner <> "" Then
                '    Me.mybanner = companybanner
                '    Me.imgSplash.Visible = False
                'End If

                Dim cc As String = da.GetSetting(CInt(Session("carrierid")), "framecolor")
                'If cc <> "" Then
                '    gvServiceProviderMatrix.BorderColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    gvServiceProviderMatrix.FooterStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    gvServiceProviderMatrix.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)

                '    '20140220 - pab - cleanup code
                '    'lblwelcome.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    'reserveback1.Style.Add("background-color", cc)
                '    'reserveback0.Style.Add("background-color", cc)
                '    'lblwelcomeright.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    'gvMultiLeg.BorderColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    ''default background-color: #1879C6

                '    ''20111220 - pab - add return flight grid
                '    'gvServiceProviderMatrixReturn.BorderColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    'gvServiceProviderMatrixReturn.FooterStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    'gvServiceProviderMatrixReturn.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                'End If

                cc = da.GetSetting(CInt(Session("carrierid")), "buttoncolor")
                '20140220 - pab - cleanup code
                'reserveback0.Visible = False
                'If cc <> "" Then
                '    '20130429 - pab - move buttons per David - change to telerik buttons
                '    'Me.cmdConfirm.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    Me.CmdEdit.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    Me.cmdjOIN.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    Me.cmdlOGINnOW.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    Me.cmdNextAirplane.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    'Me.cmdStartOver.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
                '    ' Me.lbladmin.ForeColor = System.Drawing.ColorTranslator.FromHtml(cc)
                'End If


                '20101104 - pab - remove hardcoded value
                '  Title = Title & da.GetSetting("CompanyName")


                'End If
                '20140220 - pab - cleanup code
                'lblwelcome.Text = companywelcomeleft
                'lblwelcomeright.Text = companywelcomeright
                'imgSplash.ImageUrl = companylogo
                '  Me.Title = companyname & " Select Flights"

                '20130319 - pab - add flight time message
                lblFlightTimeMsg.Text = da.GetSetting(CInt(Session("carrierid")), "FlightTimeMessage")
                If InStr(lblFlightTimeMsg.Text, "*") = 0 Or InStr(lblFlightTimeMsg.Text, "*") > 2 Then
                    lblFlightTimeMsg.Text = "*" & lblFlightTimeMsg.Text
                End If

                ip = Request.UserHostAddress
                Session("ip") = ip
                '20140414 - pab - add acg indicator for logging and tracking
                '20150317 - pab - remove acg branding
                'If _acg <> "" Then ip &= "/?acg=" & _acg

                '20130722 - pab - fix web service call returning before quotes finished
                _sleep = da.getsettingnumeric(CInt(Session("carrierid")), "wssleeptime")

                '--------------------------------DHA ~~Multileg - 
                If IsNothing(Session("triptype")) Then
                    Session("triptype") = "z"
                End If

                '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
                TurnAroundMinutes = CInt(da.GetSetting(CInt(Session("carrierid")), "TurnAroundMinutes"))

            Else
                '20131016 - pab - fix session timeout
                If IsNothing(Session("flights")) And IsNothing(Session("triptype")) Then
                    lblMsg.Text = da.GetSetting(CInt(Session("carrierid")), "TimeoutMessage")
                    gvServiceProviderMatrix.EmptyDataText = lblMsg.Text
                    dtflights.Clear()
                    Me.gvServiceProviderMatrix.DataSource = dtflights
                    Me.gvServiceProviderMatrix.DataBind()
                End If
            End If


            If Not IsPostBack Then

                Dim s As String = Session("triptype").ToString

                If Not IsNothing(Session("ShowMulti")) Then
                    If Session("ShowMulti") = "M" Then
                        Session("triptype") = "M"
                    End If
                End If


                '20160117 - pab - quote multi-leg trips
                'If Session("triptype") = "M" Then
                '    'create grid
                '    'DirectCast(FindControl("Header1"), UserControl).Visible = False
                '    'Me.pnlMultiPax.Visible = True
                '    Create_MultiLegDatatable()
                '    Bind_multiLeg(1, 7)
                '    '20140220 - pab - cleanup code
                '    'Me.pnlMultiLeg.Visible = True
                '    'Me.gvMultiLeg.Visible = True
                'Else
                '    'DirectCast(FindControl("Header1"), UserControl).Visible = True
                '    'Me.pnlMultiPax.Visible = False
                '    '20140220 - pab - cleanup code
                '    'Me.pnlMultiLeg.Visible = False
                '    'Me.gvMultiLeg.Visible = False
                'End If

                'rk 10/17/2010 ShowPriceDetail as an option in settings
                '20140220 - pab - cleanup code
                'If da.GetSetting(_carrierid, "ShowPriceDetailBox") = "Y" Then
                '    Me.TxtQuoteDetail.Visible = True
                '    If Trim(Me.TxtQuoteDetail.Text) = "" Then Me.TxtQuoteDetail.Text = globaldetail
                'Else
                '    Me.TxtQuoteDetail.Visible = False
                '    'rk 6.14.2012 leave this box open for testing timing purposes
                'End If


            End If



            'ChkCoPilot.Checked = False
            'If Session("INCLUDECOPILOT") = "True" Or _addCoPilot = True Then
            '    ChkCoPilot.Checked = True
            'End If



            If IsNothing(Session("currplane")) Then
                '20140220 - pab - cleanup code
                'Call cmdNextAirplane_Click(Nothing, Nothing)
            Else
                loadairplane(CInt(Session("carrierid")))
            End If

            '20100608 - pab - add logo to email
            Session("ApplicationPath") = Request.PhysicalApplicationPath



            AirTaxi.post_timing("start QuoteTrip load" & Now.ToString)

            '--------------------------------DHA ~~Multileg - 
            'Me.lblMsg.Text = ""
            Me.lblMsg.BackColor = Drawing.Color.White

            '20120119 - pab - force new selection if returning by user clicking home
            If Not (IsNothing(Session("flights"))) Then
                If Session("flights").Rows.Count > 2 Then
                    If Session("flights").Rows(Session("flights").Rows.Count - 1).Item("DestinationFacilityName").ToString = "Total Price:" Then
                        Session("newselection") = "T"
                    End If
                End If
            End If

            If Not (IsNothing(Session("flights"))) Then
                dtflights = Session("flights")
            Else
                'chg3641 - 20101008 - pab - fix clearing session variables when going back to request another flight
                dtflights.Clear()
            End If

            '20111221 - pab multiple quotes
            If Not (IsNothing(Session("flights2"))) Then
                dtflights2 = Session("flights2")
            Else
                dtflights2.Clear()
            End If


            'clear flight info if new selection of roundtrip, one way, multi
            If Not (IsNothing(Session("newselection"))) Then
                If Session("newselection") = "T" Then
                    dtflights.Clear()

                    '20111221 - pab multiple quotes
                    dtflights2.Clear()

                    Session("newselection") = "X"
                End If
            End If


            If Not (IsNothing(Session("destairportcode"))) Then
                _destAirportCode = Session("destairportcode")
            End If

            If Not (IsNothing(Session("origairportcode"))) Then
                _origAirportCode = Session("origairportcode")
            End If

            'rk 11/1/2010 this was preventing clicks from creating new flight record
            'If dtflights.Rows.Count = 0 Then
            '    _leaveDateTime = CDate(Session("departdate")) '  cdate(Session("leavetime"))
            '    _departDateTime = CDate(Session("departdate"))  'cdate(Session("leavetime")) ' cdate(_leaveDateTime) 
            'Else
            '    _leaveDateTime = CDate(Session("leavetime"))
            '    _departDateTime = CDate(Session("leavetime"))
            'End If




            'If Not Request.QueryString("origAirportCode") Is Nothing Then
            '    If Request.QueryString("origAirportCode").Trim <> "" Then
            '        Session("origAirportCode") = Request.QueryString("origAirportCode").Trim
            '        Dim a As String = Session("origAirportCode")
            '        Me.drpFromAirport.SelectedValue = Session("origAirportCode").ToString
            '        Me.drpFromAirport.Text = fname(Session("origAirportCode"))
            '        _origAirportCode = a
            '        ' Me.txtFromAirport.Text = fname(Session("origAirportCode"))
            '    End If
            'End If

            'If Not Request.QueryString("destAirportCode") Is Nothing Then
            '    If Request.QueryString("destAirportCode").Trim <> "" Then
            '        Session("destAirportCode") = Request.QueryString("destAirportCode")
            '        Me.drpToAirport.SelectedValue = Session("destAirportCode").ToString
            '        _destAirportCode = Session("destAirportCode")
            '    End If
            'End If

            'rk 8.16.2010 - make sure passengers field is et
            If Not Request.QueryString("passengers") Is Nothing Then
                _passengers = CInt(Request.QueryString("passengers"))
            End If

            '20111227 - pab - process call from external source
            If Not IsPostBack Then
                Dim latLong As New LatLong
                If Not Request.QueryString("origAirportCode") Is Nothing Then
                    If Request.QueryString("origAirportCode").ToString <> "" Then
                        _origAirportCode = Request.QueryString("origAirportCode").ToString.ToUpper
                        latLong = GetLatLong(_origAirportCode)
                        Session("origlat") = latLong
                        Session("origplace") = _origAirportCode
                        Session("origAirportCode") = _origAirportCode
                        dtflights.Clear()
                        dtflights2.Clear()
                        Session("flights") = Nothing
                        Session("flights2") = Nothing

                        '20120418 - pab - add contact information
                        Session("paymentType") = Nothing

                    End If
                End If
                If Not Request.QueryString("destAirportCode") Is Nothing Then
                    If Request.QueryString("destAirportCode").ToString <> "" Then
                        _destAirportCode = Request.QueryString("destAirportCode").ToString.ToUpper
                        latLong = GetLatLong(_destAirportCode)
                        Session("destlat") = latLong
                        Session("destplace") = _destAirportCode
                        Session("destAirportCode") = _destAirportCode
                    End If
                End If
                If Not Request.QueryString("leaveDate") Is Nothing And Not Request.QueryString("leaveTime") Is Nothing Then
                    If Request.QueryString("leaveDate").ToString <> "" And Request.QueryString("leaveTime").ToString <> "" Then
                        'If IsDate(CDate(Request.QueryString("leaveDate").ToString & " " & Request.QueryString("leaveTime").ToString)) Then
                        If IsDate(Request.QueryString("leaveDate").ToString & " " & Request.QueryString("leaveTime").ToString) Then
                            ''20120102 - pab - check time - default to 9 am if midnight
                            If Request.QueryString("leaveTime").ToString = "0:0" Then
                                Session("departdate") = CDate(Request.QueryString("leaveDate").ToString & " 09:00 AM")
                            Else
                                Session("departdate") = CDate(Request.QueryString("leaveDate").ToString & " " & Request.QueryString("leaveTime").ToString)
                            End If
                        End If
                    End If
                End If
                If Not Request.QueryString("returnDate") Is Nothing And Not Request.QueryString("returnTime") Is Nothing Then
                    If Request.QueryString("returnDate").ToString <> "" And Request.QueryString("returnTime").ToString <> "" Then
                        If IsDate(CDate(Request.QueryString("returnDate").ToString)) Then
                            Session("returndate") = CDate(Request.QueryString("returnDate").ToString)
                        End If
                        If IsDate(CDate(Request.QueryString("returnTime").ToString)) Then
                            '20120102 - pab - check time - default to 9 am if midnight
                            If Request.QueryString("returnTime").ToString = "0:0" Then
                                Session("Returntime") = "05:00 PM"
                            Else
                                Session("Returntime") = Request.QueryString("returnTime").ToString
                            End If
                        End If
                    End If
                End If


                '20111230 - pab - additional parm from external source
                Session("dqn") = ""
                Session("dqn2") = ""
                Session("weightclass") = ""
                Session("airboardprice") = 0
                If Not Request.QueryString("dqn") Is Nothing Then
                    Session("dqn") = Request.QueryString("dqn").ToString
                End If
                If Not Request.QueryString("weightclass") Is Nothing Then
                    Session("weightclass") = Request.QueryString("weightclass").ToString.ToUpper

                    'validate weightclass - sometimes garbage sent
                    Select Case Session("weightclass")
                        '20120507 - pab - add new weight classes - Twin Piston, Single Turboprop, Twin Turboprop, SuperMid Jet
                        'Case "P", "V", "L", "M", "H"
                        Case "P", "V", "L", "M", "H", "S", "T", "1", "2", "U"
                            'ok - do nothing
                        Case Else
                            Session("weightclass") = "L"
                            Dim dt1, dt2 As DataTable
                            Dim carrierid As Integer = 0
                            dt1 = da.GetProviderByAlias("delta")
                            If dt1.Rows.Count > 0 Then
                                carrierid = dt1.Rows(0).Item("carrierid")
                                dt2 = da.GetAircraftTypeServiceSpecsByWeightClass(carrierid, "L")
                                If dt2.Rows.Count > 0 Then
                                    If _passengers <= dt2.Rows(0).Item("totalpassengers") Then Session("weightclass") = dt2.Rows(0).Item("weightclass")
                                    Exit Select
                                End If

                                dt2 = da.GetAircraftTypeServiceSpecsByWeightClass(carrierid, "M")
                                If dt2.Rows.Count > 0 Then
                                    If _passengers <= dt2.Rows(0).Item("totalpassengers") Then Session("weightclass") = dt2.Rows(0).Item("weightclass")
                                    Exit Select
                                End If

                                Session("weightclass") = "H"
                            End If
                    End Select
                End If

                '20120103 - pab - add original quote price to click thru stats
                If Not Request.QueryString("price") Is Nothing Then
                    If IsNumeric(Request.QueryString("price").ToString) Then Session("airboardprice") = CDbl(Request.QueryString("price").ToString)
                End If

                If ddllBrokerCompanies.Items.Count = 0 Then
                    HydrateddllBrokerCompanies()
                    HydrateddllBrokers("")
                End If

                If (IsNothing(Me.depart_date.SelectedDate)) Then

                    Me.depart_date.Clear()
                    Me.depart_date2.Clear()
                    '20120830 - pab - default time to 9:00 per David
                    'Me.depart_date.SelectedDate = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy") & " " & Now.Hour & ":" & "00"
                    If DateDiff(DateInterval.Hour, Now, CDate(DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy") & " 9:00")) > 8 Then
                        Me.depart_date.SelectedDate = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy") & " 9:00"
                        Me.depart_date2.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy") & " 17:00"
                    Else
                        Me.depart_date.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy") & " 9:00"
                        Me.depart_date2.SelectedDate = DateAdd(DateInterval.Day, 3, Now).ToString("MM/dd/yyyy") & " 17:00"
                    End If

                End If

                Dim oLookup As New PopulateLookups
                'Dim dt As New Data.DataTable

                Me.departtime_combo.Items.Clear()
                dt = oLookup.TimeDD("All")
                Me.departtime_combo.DataSource = dt.DefaultView
                Me.departtime_combo.DataBind()

                'For i As Integer = 0 To dt.Rows.Count - 1
                '    Dim li As New ListItem
                '    li.Text = dt.Rows(i).Item("timestr")
                '    li.Value = dt.Rows(i).Item("timestr")
                '    Me.ddlTimeLeave.Items.Add(li)
                'Next

                Me.departtime_combo.SelectedValue = "09:00 AM"

                Me.departtime_combo2.Items.Clear()
                Me.departtime_combo2.DataSource = dt.DefaultView
                Me.departtime_combo2.DataBind()
                Me.departtime_combo2.SelectedValue = "09:00 AM"

                Me.departtime_combo3.Items.Clear()
                Me.departtime_combo3.DataSource = dt.DefaultView
                Me.departtime_combo3.DataBind()
                Me.departtime_combo3.SelectedValue = "09:00 AM"

                Me.departtime_combo4.Items.Clear()
                Me.departtime_combo4.DataSource = dt.DefaultView
                Me.departtime_combo4.DataBind()
                Me.departtime_combo4.SelectedValue = "09:00 AM"

                Me.departtime_combo5.Items.Clear()
                Me.departtime_combo5.DataSource = dt.DefaultView
                Me.departtime_combo5.DataBind()
                Me.departtime_combo5.SelectedValue = "09:00 AM"

                Me.departtime_combo6.Items.Clear()
                Me.departtime_combo6.DataSource = dt.DefaultView
                Me.departtime_combo6.DataBind()
                Me.departtime_combo6.SelectedValue = "09:00 AM"

                Me.departtime_combo7.Items.Clear()
                Me.departtime_combo7.DataSource = dt.DefaultView
                Me.departtime_combo7.DataBind()
                Me.departtime_combo7.SelectedValue = "09:00 AM"

                Me.departtime_combo8.Items.Clear()
                Me.departtime_combo8.DataSource = dt.DefaultView
                Me.departtime_combo8.DataBind()
                Me.departtime_combo8.SelectedValue = "09:00 AM"

                Me.departtime_combo9.Items.Clear()
                Me.departtime_combo9.DataSource = dt.DefaultView
                Me.departtime_combo9.DataBind()
                Me.departtime_combo9.SelectedValue = "09:00 AM"

                Me.departtime_combo10.Items.Clear()
                Me.departtime_combo10.DataSource = dt.DefaultView
                Me.departtime_combo10.DataBind()
                Me.departtime_combo10.SelectedValue = "09:00 AM"

                '20160117 - pab - quote multi-leg trips
                If Not Request.QueryString("legs") Is Nothing Then
                    Session("legs") = Request.QueryString("legs").ToString
                End If

            End If

            If Not Page.IsPostBack Then
                'Session("MapData") = Nothing
                'Session("OriginDataTable") = Nothing
                'Session("DestinationDataTable") = Nothing

                'HydrateddlAircraftServiceTypes(_carrierid)



                'If IsNothing(Session("currtype")) Then
                '    '20140221 - pab - use telerik controls
                '    'Session("currtype") = Me.ddlAircraftServiceTypes.SelectedValue
                '    'Session("currtype") = Me.ddlAircraftServiceTypes1.SelectedValue
                '    Call ddlAircraftServiceTypes_SelectedIndexChanged(Nothing, Nothing) 'rk 11/9/2010 force aircraft selection to default at launch

                'End If

                If Not Session("currtype") Is Nothing Then
                    Dim dt_priceTable As DataTable = da.GetAircraftTypeServiceSpecsByID(CInt(Session("carrierid")), CInt(Session("currtype").ToString))
                    If dt_priceTable.Rows.Count > 0 Then

                        'rk 11/9/2010 FAQ Page Link Update
                        '20140220 - pab - cleanup code
                        'Me.ImgPlane.ImageUrl = dt_priceTable.Rows(0)("LogoURL")
                        'Me.HyperLinkPlane.NavigateUrl = dt_priceTable.Rows(0)("FAQPageURL")
                        'Me.HyperLinkPlane.Text = dt_priceTable.Rows(0)("Name")

                        ''20101214 - pab - get service provider graphics
                        '_aircraftlogo = Me.ImgPlane.ImageUrl.ToString

                    End If
                End If

                '20120320 - pab - default ddl to "any"
                'If Not (IsNothing(Session("currtype"))) Then Me.ddlAircraftServiceTypes.SelectedValue = Session("currtype")


                'If Not IsNothing(Session("aircraft")) Then
                '    Me.ddlAircraftServiceTypes.SelectedValue = Session("aircraft").ToString
                'End If


                '20120619 - pab - add filters for pets, smoking, wifi
                '20120709 - pab - add lav, power
                'If Not Request.QueryString("origAddr") Is Nothing And _
                '    Not Request.QueryString("origCity") Is Nothing And _
                '    Not Request.QueryString("origState") Is Nothing And _
                '    Not Request.QueryString("origZip") Is Nothing And _
                '    Not Request.QueryString("origCountry") Is Nothing And _
                '    Not Request.QueryString("destAddr") Is Nothing And _
                '    Not Request.QueryString("destCity") Is Nothing And _
                '    Not Request.QueryString("destState") Is Nothing And _
                '    Not Request.QueryString("destZip") Is Nothing And _
                '    Not Request.QueryString("destCountry") Is Nothing And _
                '    Not Request.QueryString("roundTrip") Is Nothing And _
                '    Not Request.QueryString("passengers") Is Nothing And _
                '    Not Request.QueryString("origAirportCode") Is Nothing And _
                '    Not Request.QueryString("destAirportCode") Is Nothing And _
                '    Not Request.QueryString("leaveDate") Is Nothing And _
                '    Not Request.QueryString("returnDate") Is Nothing And _
                '    Not Request.QueryString("leaveTime") Is Nothing And _
                '    Not Request.QueryString("returnTime") Is Nothing Then
                '20131118 - pab - add more fields to aircraft
                If Not Request.QueryString("origAddr") Is Nothing And
                    Not Request.QueryString("destAddr") Is Nothing And
                    Not Request.QueryString("roundTrip") Is Nothing And
                    Not Request.QueryString("passengers") Is Nothing And
                    Not Request.QueryString("origAirportCode") Is Nothing And
                    Not Request.QueryString("destAirportCode") Is Nothing And
                    Not Request.QueryString("leaveDate") Is Nothing And
                    Not Request.QueryString("returnDate") Is Nothing And
                    Not Request.QueryString("leaveTime") Is Nothing And
                    Not Request.QueryString("returnTime") Is Nothing And
                    Not Request.QueryString("AllowPets") Is Nothing And
                    Not Request.QueryString("AllowSmoking") Is Nothing And
                    Not Request.QueryString("WiFi") Is Nothing And
                    Not Request.QueryString("EnclosedLav") Is Nothing And
                    Not Request.QueryString("PowerAvailable") Is Nothing And
                    Not Request.QueryString("InflightEntertainment") Is Nothing And
                    Not Request.QueryString("ManufactureDate") Is Nothing Then

                    '20150302 - pab - add optional crew
                    'If Not Request.QueryString("addCoPilot") Is Nothing Then
                    If Not Request.QueryString("copilot") Is Nothing Then
                        '_addCoPilot = Request.QueryString("addCoPilot")
                        _addCoPilot = Request.QueryString("copilot")
                    End If

                    _origAddr = Request.QueryString("origAddr")
                    '_origCity = Request.QueryString("origCity")
                    '_origState = Request.QueryString("origState")
                    '_origZip = Request.QueryString("origZip")
                    '_origCountry = Request.QueryString("origCountry")

                    _destAddr = Request.QueryString("destAddr")
                    '_destCity = Request.QueryString("destCity")
                    '_destState = Request.QueryString("destState")
                    '_destZip = Request.QueryString("destZip")
                    '_destCountry = Request.QueryString("destCountry")

                    ''_origAirportCode = Request.QueryString("origAirportCode")
                    ''_destAirportCode = Request.QueryString("destAirportCode")

                    _roundTrip = CBool(Request.QueryString("roundTrip"))
                    _passengers = CInt(Request.QueryString("passengers"))
                    _leaveDateTime = CDate(Request.QueryString("leaveDate") & " " & Request.QueryString("leaveTime"))
                    _returnDateTime = CDate(Request.QueryString("returnDate") & " " & Request.QueryString("returnTime"))

                    '20160430 - pab
                    'Me.OriginAddress.Text = Request.QueryString("origAirportCode").ToString
                    'Me.DestinationAddress.Text = Request.QueryString("destAirportCode").ToString
                    'Me.ddlPassengers.Text = Request.QueryString("passengers").ToString
                    'Me.depart_date.SelectedDate = CDate(Request.QueryString("leaveDate"))
                    'Me.departtime_combo.SelectedValue = Request.QueryString("leaveTime")
                    'If _roundTrip = True Then
                    '    Me.OriginAddress2.Text = Request.QueryString("destAirportCode").ToString
                    '    Me.DestinationAddress2.Text = Request.QueryString("origAirportCode").ToString
                    '    Me.ddlPassengers2.Text = Request.QueryString("passengers").ToString
                    '    Me.depart_date2.SelectedDate = CDate(Request.QueryString("returnDate"))
                    '    Me.departtime_combo2.SelectedValue = Request.QueryString("returnTime")
                    '    Me.pnlLeg2.Visible = True
                    'End If

                    '20120619 - pab - add filters for pets, smoking, wifi
                    '20140221 - pab - use telerik controls
                    'If Not Request.QueryString("AllowPets") Is Nothing Then chkPets.Checked = CBool(Request.QueryString("AllowPets"))
                    'If Not Request.QueryString("AllowSmoking") Is Nothing Then chkSmoking.Checked = CBool(Request.QueryString("AllowSmoking"))
                    'If Not Request.QueryString("WiFi") Is Nothing Then chkWiFi.Checked = CBool(Request.QueryString("WiFi"))

                    ''20120709 - pab - add lav, power
                    'If Not Request.QueryString("EnclosedLav") Is Nothing Then chkLav.Checked = CBool(Request.QueryString("EnclosedLav"))
                    'If Not Request.QueryString("PowerAvailable") Is Nothing Then chkPower.Checked = CBool(Request.QueryString("PowerAvailable"))

                    ''20131118 - pab - add more fields to aircraft
                    'If Not Request.QueryString("InflightEntertainment") Is Nothing Then chkInflightEntertainment.Checked = CBool(Request.QueryString("InflightEntertainment"))
                    'Dim collection As IList(Of Telerik.Web.UI.RadComboBoxItem) = RadComboBoxRequests.CheckedItems
                    Dim bAllowPets As Boolean = False
                    Dim bAllowSmoking As Boolean = False
                    Dim bWiFi As Boolean = False
                    Dim bLav As Boolean = False
                    Dim bPower As Boolean = False
                    Dim bInFlightEntertainment As Boolean = False
                    If Not Request.QueryString("AllowPets") Is Nothing Then bAllowPets = CBool(Request.QueryString("AllowPets"))
                    If Not Request.QueryString("AllowSmoking") Is Nothing Then bAllowSmoking = CBool(Request.QueryString("AllowSmoking"))
                    If Not Request.QueryString("WiFi") Is Nothing Then bWiFi = CBool(Request.QueryString("WiFi"))
                    If Not Request.QueryString("EnclosedLav") Is Nothing Then bLav = CBool(Request.QueryString("EnclosedLav"))
                    If Not Request.QueryString("PowerAvailable") Is Nothing Then bPower = CBool(Request.QueryString("PowerAvailable"))
                    If Not Request.QueryString("InflightEntertainment") Is Nothing Then bInFlightEntertainment = CBool(Request.QueryString("InflightEntertainment"))

                    'For Each item As Telerik.Web.UI.RadComboBoxItem In collection
                    '    Select Case item.Text
                    '        Case "Pets"
                    '            If bAllowPets = True Then
                    '                item.Checked = True
                    '            Else
                    '                item.Checked = False
                    '            End If

                    '        Case "Smoking"
                    '            If bAllowSmoking = True Then
                    '                item.Checked = True
                    '            Else
                    '                item.Checked = False
                    '            End If

                    '        Case "WiFi"
                    '            If bWiFi = True Then
                    '                item.Checked = True
                    '            Else
                    '                item.Checked = False
                    '            End If

                    '        Case "Enclosed Lav"
                    '            If bLav = True Then
                    '                item.Checked = True
                    '            Else
                    '                item.Checked = False
                    '            End If

                    '        Case "Power"
                    '            If bPower = True Then
                    '                item.Checked = True
                    '            Else
                    '                item.Checked = False
                    '            End If

                    '        Case "InFlight Entertainment"
                    '            If bInFlightEntertainment = True Then
                    '                item.Checked = True
                    '            Else
                    '                item.Checked = False
                    '            End If

                    '    End Select
                    'Next

                    If Not Request.QueryString("ManufactureDate") Is Nothing Then
                        If IsDate(Request.QueryString("ManufactureDate")) Then
                            ManufactureDate = CDate(Request.QueryString("ManufactureDate"))
                        End If
                    End If

                    'If CType(Session("OriginTable"), DataTable).Rows.Count = 0 Or _
                    '    CType(Session("DestinationTable"), DataTable).Rows.Count = 0 Then
                    '    Me.Visible = False
                    'Else
                    Me.Visible = True
                    ' MapNearbyAirports(_origAddr, _origCity, _origState, _origZip, _origCountry, True, CType(Session("OriginTable"), DataTable))
                    'MapNearbyAirports(_destAddr, _destCity, _destState, _destZip, _destCountry, False, CType(Session("DestinationTable"), DataTable))
                    'End If
                Else
                    'MapAirportsInState()
                End If
            Else
                '20120710 - pab - prevent fields from being reset with initial values when page IS postback
                ''20120619 - pab - add filters for pets, smoking, wifi
                ''20120709 - pab - add lav, power
                ''If Not Request.QueryString("origAddr") Is Nothing And _
                ''    Not Request.QueryString("origCity") Is Nothing And _
                ''    Not Request.QueryString("origState") Is Nothing And _
                ''    Not Request.QueryString("origZip") Is Nothing And _
                ''    Not Request.QueryString("origCountry") Is Nothing And _
                ''    Not Request.QueryString("destAddr") Is Nothing And _
                ''    Not Request.QueryString("destCity") Is Nothing And _
                ''    Not Request.QueryString("destState") Is Nothing And _
                ''    Not Request.QueryString("destZip") Is Nothing And _
                ''    Not Request.QueryString("destCountry") Is Nothing And _
                ''    Not Request.QueryString("roundTrip") Is Nothing And _
                ''    Not Request.QueryString("passengers") Is Nothing And _
                ''    Not Request.QueryString("origAirportCode") Is Nothing And _
                ''    Not Request.QueryString("destAirportCode") Is Nothing And _
                ''    Not Request.QueryString("leaveDate") Is Nothing And _
                ''    Not Request.QueryString("returnDate") Is Nothing And _
                ''    Not Request.QueryString("leaveTime") Is Nothing And _
                ''    Not Request.QueryString("returnTime") Is Nothing Then
                'If Not Request.QueryString("origAddr") Is Nothing And _
                '    Not Request.QueryString("destAddr") Is Nothing And _
                '    Not Request.QueryString("roundTrip") Is Nothing And _
                '    Not Request.QueryString("passengers") Is Nothing And _
                '    Not Request.QueryString("origAirportCode") Is Nothing And _
                '    Not Request.QueryString("destAirportCode") Is Nothing And _
                '    Not Request.QueryString("leaveDate") Is Nothing And _
                '    Not Request.QueryString("returnDate") Is Nothing And _
                '    Not Request.QueryString("leaveTime") Is Nothing And _
                '    Not Request.QueryString("returnTime") Is Nothing And _
                '    Not Request.QueryString("AllowPets") Is Nothing And _
                '    Not Request.QueryString("AllowSmoking") Is Nothing And _
                '    Not Request.QueryString("WiFi") Is Nothing And _
                '    Not Request.QueryString("EnclosedLav") Is Nothing And _
                '    Not Request.QueryString("PowerAvailable") Is Nothing Then


                '    If Not Request.QueryString("addCoPilot") Is Nothing Then
                '        _addCoPilot = Request.QueryString("addCoPilot")
                '    End If


                '    _origAddr = Request.QueryString("origAddr")
                '    '_origCity = Request.QueryString("origCity")
                '    '_origState = Request.QueryString("origState")
                '    '_origZip = Request.QueryString("origZip")
                '    '_origCountry = Request.QueryString("origCountry")

                '    _destAddr = Request.QueryString("destAddr")
                '    '_destCity = Request.QueryString("destCity")
                '    '_destState = Request.QueryString("destState")
                '    '_destZip = Request.QueryString("destZip")
                '    '_destCountry = Request.QueryString("destCountry")

                '    ''_origAirportCode = Request.QueryString("origAirportCode")
                '    ''_destAirportCode = Request.QueryString("destAirportCode")

                '    _roundTrip = CBool(Request.QueryString("roundTrip"))
                '    _passengers = CInt(Request.QueryString("passengers"))
                '    _leaveDateTime = CDate(Request.QueryString("leaveDate") & " " & Request.QueryString("leaveTime"))
                '    _returnDateTime = CDate(Request.QueryString("returnDate") & " " & Request.QueryString("returnTime"))


                '    _departDateTime = CDate(Request.QueryString("leaveDate") & " " & Request.QueryString("leaveTime"))
                '    _returnDateTime = CDate(Request.QueryString("returnDate") & " " & Request.QueryString("returnTime"))

                '    '20120619 - pab - add filters for pets, smoking, wifi
                '    If Not Request.QueryString("AllowPets") Is Nothing Then chkPets.Checked = CBool(Request.QueryString("AllowPets"))
                '    If Not Request.QueryString("AllowSmoking") Is Nothing Then chkSmoking.Checked = CBool(Request.QueryString("AllowSmoking"))
                '    If Not Request.QueryString("WiFi") Is Nothing Then chkWiFi.Checked = CBool(Request.QueryString("WiFi"))


                '    '20120709 - pab - add lav, power
                '    If Not Request.QueryString("EnclosedLav") Is Nothing Then chkLav.Checked = CBool(Request.QueryString("EnclosedLav"))
                '    If Not Request.QueryString("PowerAvailable") Is Nothing Then chkPower.Checked = CBool(Request.QueryString("PowerAvailable"))

                'End If

                'If Request.Params("__EVENTTARGET").Contains("bttnFindNearby") Then ' = Me.upMapping.UniqueID Then

                '    PlotPointsOfInterest()

                '    Me.pnlMainMapControls.Visible = False
                '    Me.bttnRedrawMap.Visible = True

                'End If
            End If





            'If Not Request.Params("ScriptManager1") Is Nothing Then
            '    If Request.Params("ScriptManager1").Contains("$bttnGetFlights") Or Request.Params("ScriptManager1").Contains("$bttnGetSpecials") Then 'rk 2.04.09
            '        ' BindData()
            '    End If
            'End If

            '   If Not Nothing(Request.Params("__EVENTTARGET").Contains("bttnFindNearby")) Then

            '20160430 - pab
            'If Not Request.Params("__EVENTTARGET") Is Nothing Then
            '    Dim a As String
            '    a = Request.Params("__EVENTTARGET")
            '    If a = "airport" Then


            '        Dim b As String
            '        b = Request.Params("__EVENTARGUMENT")
            '        a = a


            '        Dim selected_airport As String = Right(b, 2)


            '        If _destAirportCode = _origAirportCode Then
            '            If _destAirportCode <> "" Then
            '                Me.lblMsg.Text = "Origin and Destination Airport Codes Must Be Different"
            '            End If
            '        End If

            '        'rk 11/1/2010 round trip not being set and selecting airports inoperative.
            '        If Session("triptype") = "R" Then _roundTrip = True


            '        ' Me.lblMsgHello.Visible = True
            '        'an airport selection event has occured
            '        'for round trip
            '        If _destAirportCode <> "" Then
            '            If _origAirportCode <> "" Then
            '                If _destAirportCode <> _origAirportCode Then
            '                    ' Me.lblMsgHello.Visible = False
            '                    If _roundTrip Then
            '                        'Me.cmdNewDest.Visible = False
            '                        'Me.cmdQuote.Visible = False

            '                        'rk 11/1/2010 clicks should request new airports
            '                        _leaveDateTime = CDate(Session("departdate")) '  cdate(Session("leavetime"))
            '                        _departDateTime = CDate(Session("departdate"))  'c

            '                        Dim workdate As Date = Session("returndate")
            '                        _returnDateTime = CDate(Month(workdate) & "/" & Day(workdate) & "/" & Year(workdate) & " " & Session("Returntime"))

            '                        dtflights.Clear()

            '                        '20111221 - pab multiple quotes
            '                        dtflights2.Clear()

            '                        Call addleg("R", _carrierid)
            '                        Call quote(_carrierid)
            '                        Session("clickairport") = True

            '                        '20120130 - pab - add start over button at top for when no flights returned
            '                        'Me.cmdStartOver2.Visible = False
            '                        Me.cmdStartOver.Visible = False
            '                        '20130429 - pab - move buttons per David - change to telerik buttons
            '                        'Me.cmdStartOver.Visible = True
            '                        'Me.cmdStartOver1.Visible = True
            '                    End If

            '                End If
            '            End If
            '        End If



            '        'an airport selection event has occured
            '        'for one way
            '        If _destAirportCode <> "" Then
            '            If _origAirportCode <> "" Then
            '                If _destAirportCode <> _origAirportCode Then
            '                    If Session("triptype") = "O" Then
            '                        dtflights.Clear()

            '                        '20111221 - pab multiple quotes
            '                        dtflights2.Clear()

            '                        'rk 11/1/2010 clicks should request new airports
            '                        _leaveDateTime = CDate(Session("departdate")) '  cdate(Session("leavetime"))
            '                        _departDateTime = CDate(Session("departdate"))  'c

            '                        Dim workdate As Date = Session("returndate")
            '                        _returnDateTime = CDate(Month(workdate) & "/" & Day(workdate) & "/" & Year(workdate) & " " & Session("Returntime"))

            '                        Call addleg("L", _carrierid)
            '                        ' ''-- ~~DHA new implmentation for multileg 
            '                        'Me.cmdNewDest.Visible = False

            '                        '20140220 - pab - cleanup code
            '                        'Me.cmdQuote.Visible = False

            '                        Call quote(_carrierid)
            '                        Session("clickairport") = True

            '                        '20120130 - pab - add start over button at top for when no flights returned
            '                        'Me.cmdStartOver2.Visible = False
            '                        Me.cmdStartOver.Visible = False
            '                        '20130429 - pab - move buttons per David - change to telerik buttons
            '                        'Me.cmdStartOver.Visible = True
            '                        'Me.cmdStartOver1.Visible = True
            '                    End If
            '                End If
            '            End If
            '        End If

            '    End If
            'End If




            If Not IsPostBack Then


                '20100323 - pab - add airport pax fees
                Dim bIntl As Boolean = isflightintl(_origAirportCode, _destAirportCode)
                'chg3620  - 20100923 - pab - fix international flag
                If bIntl Then
                    Session("international_type") = "intl"
                Else
                    Session("international_type") = "domestic"
                End If

                'add error correction here to ensure on drop down list.
                If _destAirportCode <> "" Then
                    'If Me.drpToAirport.Items.FindByValue(_destAirportCode) IsNot Nothing Then Me.drpToAirport.SelectedValue = _destAirportCode

                    AirTaxi.post_timing("fees at dest  " & Now.ToString)
                    '20100323 - pab - add airport pax fees
                    'Dim x As String = feesatairport(_destAirportCode)
                    Dim x As String = feesatairport(_destAirportCode, True, bIntl, CInt(Session("carrierid")), internationalfees)
                    AirTaxi.post_timing("fees at dest complete " & Now.ToString)
                    If _pricefees > 100 Then
                        Me.lblMsg.Text = "Please Note!  Landing Fees at " & _destAirportCode & " are $" & _pricefees & " and may have long delays - consider another airport on price and time" & vbCr & vbLf
                    End If

                End If




                If _origAirportCode <> "" Then
                    'If Me.drpFromAirport.Items.FindByValue(_origAirportCode) IsNot Nothing Then Me.drpFromAirport.SelectedValue = _origAirportCode

                    AirTaxi.post_timing("fees at orig  " & Now.ToString)
                    '20100323 - pab - add airport pax fees
                    'Dim x As String = feesatairport(_origAirportCode)
                    Dim x As String = feesatairport(_origAirportCode, False, bIntl, CInt(Session("carrierid")), internationalfees)
                    AirTaxi.post_timing("fees at orig complete  " & Now.ToString)
                    If _pricefees > 100 Then
                        'chg3612 - 20100915 - pab - fix fees message
                        'Me.lblMsg.Text = Me.lblMsg.Text & "Fees at " & _origAirportCode & " are " & _pricefees & " which suggest delays - consider another airport on price and time"
                        Me.lblMsg.Text = Me.lblMsg.Text & "Additional fees at " & _origAirportCode & " are $" & _pricefees & " which suggest delays - consider another airport on price and time"
                    End If

                End If


                If Session("destAirportCode") <> "" Then
                    _destAirportCode = Session("destAirportCode")
                    '20140220 - pab - cleanup code
                    'lblDest.Text = "Destination: " & fname(_destAirportCode)
                    'lblDest.Text = fname(_destAirportCode)
                End If

                If Session("origAirportCode") <> "" Then
                    _origAirportCode = Session("origAirportCode")
                    '20140220 - pab - cleanup code
                    'lblOrigin.Text = "Origin: " & fname(_origAirportCode)
                    'lblOrigin.Text = fname(_origAirportCode)
                End If


                '20120130 - pab - fix error - Object reference not set to an instance of an object
                Dim dtflightsrows As Integer = 0
                If dtflights Is Nothing Then
                    dtflightsrows = 0
                ElseIf dtflights.Rows.Count > 0 Then
                    dtflightsrows = dtflights.Rows.Count
                End If
                'If dtflights.Rows.Count = 0 Then
                If dtflightsrows = 0 Then
                    ' ''-- ~~DHA new implmentation for multileg 
                    'Me.cmdNewDest.Visible = False
                    '20140220 - pab - cleanup code
                    'Me.cmdQuote.Visible = False
                    'Me.LblItin.Visible = False
                    Me.lblMsg.Text = ""
                    '20160117 - pab - quote multi-leg trips
                    'If Session("triptype") <> "M" And Session("triptype") <> "G" Then
                    If Session("triptype") <> "G" Then
                        lblMsg.Visible = True

                        '20131016 - pab - fix session timeout
                        If IsNothing(Session("flights")) And IsNothing(Session("triptype")) Then
                            lblMsg.Text = da.GetSetting(CInt(Session("carrierid")), "TimeoutMessage")
                            gvServiceProviderMatrix.EmptyDataText = lblMsg.Text
                        Else
                            'lblMsg.Text = "No flights available. Please make a new trip request or change aircraft type."
                        End If

                        '20120316 - pab - remove label - now in grid header
                        'lblTaxes.Visible = False

                        '20110906 - pab - don't display contine button if no flights
                        '20130429 - pab - move buttons per David - change to telerik buttons
                        'Me.cmdConfirm.Visible = False
                        'Me.cmdConfirm1.Visible = False

                        '20111220 - pab - add return flight grid
                        '20140220 - pab - cleanup code
                        'Me.lblItinReturn.Visible = False
                        'Me.gvServiceProviderMatrixReturn.Visible = False

                        '20120125 - pab -remove widget - go to index
                        '20120130 - pab - add start over button at top for when no flights returned
                        'Me.cmdStartOver2.Visible = True
                        Me.cmdStartOver.Visible = True
                        '20130429 - pab - move buttons per David - change to telerik buttons
                        'Me.cmdStartOver.Visible = False
                        'Me.cmdStartOver1.Visible = False

                    End If
                    'lblmsg.Visible = False
                Else

                    If Session("triptype") <> "M" Then

                        '20120130 - pab - add start over button at top for when no flights returned
                        'Me.cmdStartOver2.Visible = False
                        'Me.cmdStartOver.Visible = False
                        '20130429 - pab - move buttons per David - change to telerik buttons
                        'Me.cmdStartOver.Visible = True
                        'Me.cmdStartOver1.Visible = True

                    End If

                    '20120130 - pab - add start over button at top for when no flights returned
                    'Me.cmdStartOver2.Visible = False
                    'Me.cmdStartOver.Visible = False


                End If

            End If

            '20111229 - pab
            AirTaxi.post_timing("Page_Load End  " & Now.ToString)

            '20160517 - pab - fix carrierid = 0 preventing quotes
        Catch ex As Exception
            Dim s As String = ex.Message
            If s <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "Page_Load", "QuoteTrip.aspx.vb")
                AirTaxi.InsertEmailQueue(CInt(Session("carrierid")), "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "QuoteTrip.aspx.vb Page_Load error", s, False, "", "", "", False)
            End If

        End Try

    End Sub

    '20111227 - pab - process call from external source
    Function GetLatLong(ByVal airportcode As String) As LatLong

        Dim da As New DataAccess
        Dim ds As DataSet = da.GetAirportInformationByAirportCode(airportcode)
        Dim latLong As New LatLong

        If Not IsNothing(ds) Then
            If ds.Tables(0).Rows.Count > 0 Then
                latLong.Latitude = Math.Round(CDbl(ds.Tables(0).Rows(0)("latitude")), 13)
                latLong.Longitude = Math.Round(CDbl(ds.Tables(0).Rows(0)("longitude")), 13)
            End If
        End If

        Return latLong

    End Function

    Public Function traveltime(ByVal a As String, ByVal b As String, ByVal carrierid As Integer) As Long

        Try

            Dim d As Integer
            ' d = CInt(getdistance(_origAirportCode, _destAirportCode))
            Dim oMapping As New Mapping
            d = oMapping.GetRoundEarthDistanceBetweenLocations("pbaumgart@ctgi.com", "123", CInt(Session("carrierid")), a, b)

            'Session("distance") = d

            Dim _airSpeed As Double = 0
            Dim t As Long
            Dim da As New DataAccess
            ' _airSpeed = CDbl(da.GetAirSpeed) * 1.15077945



            Dim currtype As Integer = Session("currtype")

            If IsNothing(Session("weightclass")) Then
                Session("weightclass") = "L"
            ElseIf Session("weightclass").ToString = "" Then
                Session("weightclass") = "L"
            End If
            If currtype = 0 Then
                Dim dt As DataTable = da.GetAircraftTypeServiceSpecsByWeightClass(CInt(Session("carrierid")), Session("weightclass").ToString)
                If Not isdtnullorempty(dt) Then
                    currtype = dt.Rows(0).Item("currtype")
                    Session("currtype") = currtype
                End If
            End If

            Dim dt_priceTable As DataTable = da.GetAircraftTypeServiceSpecsByID(carrierid, currtype)

            Dim currentplane As Integer = 0

            Dim ma As Integer ', d As Double, t As Double

            '20171121 - pab - fix carriers changing midstream - change to Session variables
            Dim distance_text As String = ""

            '20160521 - pab - fix error - no row at position 0
            If Not isdtnullorempty(dt_priceTable) Then

                _airSpeed = CInt(dt_priceTable.Rows(currentplane)("Blockspeed")) * 1.15077945
                'Dim discountPricePerHour As Integer = CInt(dt_priceTable.Rows(currentplane)("DiscountPricePerHour"))
                'Dim CostPerHourFlightTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourFlightTime"))

                '20081218 - pab - add additional fees
                Dim perHourRepositionTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourRepositionTime"))
                Dim perNightOvernight As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerNightOvernight"))
                Dim perCycleFee As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerCycleFee"))
                Dim perDayCrewExpenses As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerDayCrewExpenses"))
                Dim perHourWaitTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourWaitTime"))

                '20100524 - pab - add new fees
                Dim perDayCabinAideExpenses As Double = dt_priceTable.Rows(currentplane)("CostPerDayCabinAideExpenses")

                Dim CycleFees As Integer

                '20090223 - pab - change hardcoded values to configurable
                Dim TaxiMinutes As Integer = CInt(dt_priceTable.Rows(currentplane)("TaxiMinutes"))
                Dim FlightTimeSwag As Double = dt_priceTable.Rows(currentplane)("FlightTimeSwag")


                ' Dim dt As DataTable = da.GetAircraftPerformanceRange(currtype)


                'If dt.Rows.Count <> 0 Then
                '    hasperformancematrix = True
                'End If

                'If hasperformancematrix = False Then
                '    dt = da.GetAircraftPerformanceFuelByHourAndID(serviceTypeID)
                '    If dt.Rows.Count <> 0 Then
                '        hasperformancematrix = True
                '    End If
                'End If


                'If hasperformancematrix = False Then
                '    dt = da.GetAircraftPerformanceFuelByDistanceAndID(serviceTypeID)
                '    If dt.Rows.Count <> 0 Then

                '20100723 - bd - Use new aircraft performance class
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                Dim dtAPD As DataTable = AircraftPerformance.FuelSpeedByDistance.Get(CInt(Session("carrierid")), currtype) 'da.GetAircraftPerformanceFuelByDistanceAndID(currtype)

                '        rangelow	rangehigh	speed	Gallons
                '1	50	150.00	0.00
                '51	75	160.00	0.00
                '76	2000	170.00	0.00

                If dtAPD.Rows.Count <> 0 Then
                    For i = 0 To dtAPD.Rows.Count - 1
                        If d >= dtAPD.Rows(i)("RangeLow") And d <= dtAPD.Rows(i)("RangeHigh") Then
                            _airSpeed = dtAPD.Rows(i)("speed") * 1.15077945
                            distance_text = distance_text & "Airspeed Adjusted by Performance Table to " & _airSpeed & " which is " & dtAPD.Rows(i)("speed") & " multiplied by 1.15077945" & vbCr & vbLf & vbCr & vbLf
                        End If
                    Next i
                End If


                t = CInt((d / _airSpeed) * 60)
                t = t + (t * FlightTimeSwag) 'add an optional percentage to each flight time calculation

                '20090223 - pab - change hardcoded values to configurable
                'ma = ma + CInt(t) + 12

                ma = CInt(t) + TaxiMinutes

            End If

            traveltime = ma

        Catch ex As Exception
            '20131015 - pab - add more error handling to catch error - Object reference not set to an instance of an object
            Dim s As String = "from " & a & " to " & b & vbCr & vbLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "traveltime", "QuoteTrip.aspx.vb")
            AirTaxi.InsertEmailQueue(CInt(Session("carrierid")), "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                "QuoteTrip.aspx.vb traveltime error", s, False, "", "", "", False)

        End Try

    End Function


    '20120503 - pab - move airport mapping to separate sub so it can be called from many places 
    Private Sub FindNearbyAirports()

        '20131015 - pab - add more error handling to catch error - Object reference not set to an instance of an object
        Try

            AirTaxi.post_timing("FindNearbyAirports start  " & Now.ToString)

            Dim o, d As DataTable
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            o = GetAirportsLatLong(Session("origlat"), CInt(Session("carrierid")))
            d = GetAirportsLatLong(Session("destlat"), CInt(Session("carrierid")))

            Dim a As LatLong = Session("origlat")
            Dim b As LatLong = Session("destlat")


            If o.Rows.Count = 0 Then
                If Session("triptype") = "M" Then
                    Me.lblMsg.Text = ""
                Else
                    Me.lblMsg.Text = "Origination Outside Service Area " & a.Latitude & " " & a.Longitude & "</br>"
                End If

                'Me.myMap0.Visible = False  'DHA
                'Me.pnlMyMap0.Visible = False
            Else
                'Me.myMap0.Visible = True
                ''Me.pnlMyMap0.Visible = True
            End If


            If d.Rows.Count = 0 Then
                If Session("triptype") = "M" Then
                    Me.lblMsg.Text = ""

                Else
                    Me.lblMsg.Text = Me.lblMsg.Text & "Destination Outside Service Area " & b.Latitude & " " & b.Longitude
                End If
                'Me.myMap.Visible = False 'DHA
                'Me.Panel7.Visible = False
            Else
                'Me.myMap.Visible = True
                ''Me.Panel7.Visible = True
            End If

            'chg3621 - rk 9/23/2010 - fix error where the time was being appended twice to the date
            If Not IsNothing(Session("departdate")) Then
                If Not IsNothing(Session("returndate")) Then
                    If IsDate(Session("returndate")) Then
                        If IsDate(Session("departdate")) Then

                            _leaveDateTime = CDate(Session("departdate")) '  cdate(Session("leavetime"))
                            _departDateTime = CDate(Session("departdate"))  'c

                            Dim workdate As Date = Session("returndate")
                            _returnDateTime = CDate(Month(workdate) & "/" & Day(workdate) & "/" & Year(workdate) & " " & Session("Returntime"))


                        End If
                    End If
                End If
            End If

            If d.Rows.Count <> 0 And o.Rows.Count <> 0 Then
                cmdEURoutes(o, d, CInt(Session("carrierid")))
                gvServiceProviderMatrix.Visible = True

                '20111221 - pab - multiple quotes
                '20120329 - pab - grids combined - do not show
                'If Session("triptype") = "R" Then
                '    Me.gvServiceProviderMatrixReturn.Visible = True
                'Else
                '20140220 - pab - cleanup code
                'Me.gvServiceProviderMatrixReturn.Visible = False
                'End If

            Else
                '20120316 - pab - remove label - now in grid header
                'lblTaxes.Visible = False

                '20111220 - pab - add return flight grid
                '20140220 - pab - cleanup code
                'Me.lblItinReturn.Visible = False
                'Me.gvServiceProviderMatrixReturn.Visible = False

            End If

            '20140315 - pab - show or hide maps (problems with maps caused by acg branding)
            Dim da As New DataAccess
            'If da.GetSetting(_carrierid, "ShowMaps") = "Y" Then
            '    lblOrigin.Visible = True
            '    lblDest.Visible = True
            '    lblMsg1.Visible = True
            '    myMap0.Visible = True
            '    myMap.Visible = True
            'Else
            '    lblOrigin.Visible = False
            '    lblDest.Visible = False
            '    lblMsg1.Visible = False
            '    myMap0.Visible = False
            '    myMap.Visible = False
            'End If

            AirTaxi.post_timing("FindNearbyAirports end  " & Now.ToString)

        Catch ex As Exception
            '20131015 - pab - add more error handling to catch error - Object reference not set to an instance of an object
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "FindNearbyAirports", "QuoteTrip.aspx.vb")
            AirTaxi.InsertEmailQueue(CInt(Session("carrierid")), "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                "QuoteTrip.aspx.vb FindNearbyAirports error", s, False, "", "", "", False)

        End Try

    End Sub

    Private Sub MapNearbyAirports(ByVal address As String, ByVal city As String,
         ByVal state As String, ByVal zip As String, ByVal country As String, ByVal isOrigin As Boolean,
         ByVal dt As DataTable)

        '20111229 - pab
        AirTaxi.post_timing("MapNearbyAirports Start  " & Now.ToString)

        If country = "" Or country = "USA" Then country = "United States of America" ' Me.tbCountry.Text.Trim

        Dim oMapping As New Mapping

        ''If state.Length = 0 Then Exit Sub
        'If isOrigin Then
        '    If state.Length = 0 And zip.Length = 0 And _origAirportCode.Length = 0 Then
        '        Exit Sub
        '    End If
        'Else 'destination
        '    If state.Length = 0 And zip.Length = 0 And _destAirportCode.Length = 0 Then
        '        Exit Sub
        '    End If
        'End If

        'get lat long from address, etc
        Dim latLong As LatLong = CType(IIf(isOrigin, Session("OriginLatLong"), Session("DestinationLatLong")), LatLong)
        'Dim latLong As LatLong = oMapping.GeocodeAddress(address, city, _
        '    state, zip, country)

        'get nearbly airports
        Dim minRunwayLength As Integer = 2500
        Dim miles As Integer = 100 '100 mi radius
        Dim count As Integer = 5 'return top 5
        'Dim ds As DataSet = oMapping.FindNearbyAirports(latLong.Latitude, latLong.Longitude, _
        '     minRunwayLength, miles, count)

        Dim addressText As String = address & " " & city & " " & state & " " & zip '& " " & country

        Dim imageUrl As String = String.Empty
        Dim maxRunwayLength As Integer = 0

        'Me.lblMinRunwayLength.Text = minRunwayLength.ToString

        If isOrigin Then
            'create map
            _initializeMap = "<script language='javascript' type='text/javascript'>" & ControlChars.CrLf
            _initializeMap &= "function showWaitDialog(){document.getElementById('pleaseWait').style.visibility='visible';document.getElementById('pleaseWait').style.display='inline';} window.onload = showWaitDialog;" & ControlChars.CrLf

            '_initializeMap &= "window.onload = createMainMap;" & ControlChars.CrLf
            'setTimeout allows the rest of the page to be shown in the browser before rendering map
            _initializeMap &= "setTimeout('createMainMap()', " & CStr(_setTimeoutInterval) & ");" & ControlChars.CrLf

            _initializeMap &= "</script>" & ControlChars.CrLf

            'center map
            '_mapData = "map.SetCenter(new VELatLong(" & latLong.Latitude.ToString & _
            '                ", " & latLong.Longitude.ToString & "));" & ControlChars.CrLf
        End If

        'add starting point
        _mapData &= "addPushpin('home_" & isOrigin.GetHashCode.ToString & "', new VELatLong(" & latLong.Latitude.ToString &
                        ", " & latLong.Longitude.ToString & "), 'Starting Point', '" &
                        "<table border=""0"" cellpadding=""0"" cellspacing=""1"" width=""100%"">" &
                        "<tr><td align=""left"" style=""padding-top:2px;padding-bottom:5px;"">" & addressText.Trim.Replace("'", "\'") &
                        "</td></tr>"

        ''find nearby points of interest
        '_mapData &= "<tr><td align=""left"" style=""border-top:solid 1px #C0C0C0;padding-top:2px;"""">" & _
        '"<table id=""rblFindNearby_0"" border=""0"" cellpadding=""0"" cellspacing=""0"">" & _
        ' "<tr>" & _
        ' "<td colspan=""3"">Find Nearby:</td>" & _
        ' "</tr><tr>" & _
        '  "<td><input id=""rblFindNearby_0_0"" type=""radio"" name=""rblFindNearby_0"" value=""Car_Rental"" /><label for=""rblFindNearby_0_0"">Car Rental</label></td>" & _
        '  "<td><input id=""rblFindNearby_0_2"" type=""radio"" name=""rblFindNearby_0"" value=""Limousine"" /><label for=""rblFindNearby_0_2"">Limousine</label></td>" & _
        '  "<td rowspan=""2"" valign=""top"" style=""padding-left:5px;padding-top:2px;"">" & _
        '  "<input type=""button"" value=""Find"" style=""font-size:0.9em;"" id=""bttnFindNearby_0"" " & _
        '  "onclick=""var rblValue = getFindNearbyCategory(\'rblFindNearby_0\'); if (rblValue != \'\') {__doPostBack(\'bttnFindNearby_0\', rblValue + \',home," & latLong.Latitude.ToString & "," & latLong.Longitude.ToString & "," & addressText.Replace(",", " ").Replace("'", "\'") & "\')};""></td>" & _
        '  "</tr><tr>" & _
        '  "<td style=""padding-bottom:3px;""><input id=""rblFindNearby_0_1"" type=""radio"" name=""rblFindNearby_0"" value=""Hotel"" /><label for=""rblFindNearby_0_1"">Hotel</label></td>" & _
        '  "<td style=""padding-bottom:3px;""><input id=""rblFindNearby_0_3"" type=""radio"" name=""rblFindNearby_0"" value=""Taxi"" /><label for=""rblFindNearby_0_3"">Taxi</label></td>" & _
        ' "</tr>" & _
        '"</table>" & _
        '"</td></tr>"

        _mapData &=
            "<tr><td align=""left"" style=""border-top:solid 1px #C0C0C0;padding-top:2px;text-decoration:underline;"">" &
           "<span onmouseover=""this.style.cursor=\'pointer\';"" " &
            " onmouseout=""this.style.cursor=\'default\';"" onclick=""centerItemOnMap(" & latLong.Latitude.ToString & "," & latLong.Longitude.ToString & ");"">Center on map</span>" &
            "</td></tr>" &
            "</table>" &
            "', 'Images/starting_point_red.gif');" & ControlChars.CrLf

        'parse the dataset and show results in label


        If isOrigin Then

            _originTable = dt

            _originCount = dt.Rows.Count

            _mapData &= "map.SetZoomLevel(14);" & ControlChars.CrLf

            _mapData &= "latLongArr = new Array();" '(" & dt.Rows.Count.ToString & ");" & ControlChars.CrLf

            _mapData &= "vll = new VELatLong(" & latLong.Latitude.ToString &
                            ", " & latLong.Longitude.ToString & ");" & ControlChars.CrLf

            '_mapData &= "latLongArr[0] = vll;" & ControlChars.CrLf

            _mapData &= "latLongOriginArr = new Array();"
        Else

            _destinationTable = dt

            _mapData &= "vll = new VELatLong(" & latLong.Latitude.ToString &
                                                ", " & latLong.Longitude.ToString & ");" & ControlChars.CrLf

            _mapData &= "latLongDestArr = new Array();"
            '_mapData &= "latLongArr[" & _originCount.ToString & "] = vll;" & ControlChars.CrLf
        End If

        If isOrigin Then
            _mapData &= "latLongArr[0] = vll;" & ControlChars.CrLf
            _mapData &= "latLongOriginArr[0] = vll;" & ControlChars.CrLf
        Else
            _mapData &= "latLongDestArr[0] = vll;" & ControlChars.CrLf
            _mapData &= "latLongArr[" & _originCount.ToString & "] = vll;" & ControlChars.CrLf
        End If

        Dim i As Integer = CInt(IIf(isOrigin, 0, _originCount))

        For Each dr As DataRow In dt.Rows

            '  maxRunwayLength = CInt(dr("MaxRunwayLength"))

            ''set airport image
            'If maxRunwayLength >= minRunwayLength And maxRunwayLength < 3000 Then
            '    imageUrl = "Images/airport_green.gif" '#00CC00
            'ElseIf maxRunwayLength >= 3000 And maxRunwayLength < 3500 Then
            '    imageUrl = "Images/airport_black.gif" '#000000
            'ElseIf maxRunwayLength >= 3500 And maxRunwayLength < 4000 Then
            '    imageUrl = "Images/airport_yellow.gif" '#CCCC00
            'ElseIf maxRunwayLength >= 4000 And maxRunwayLength < 4500 Then
            '    imageUrl = "Images/airport_blue.gif" '#0000CC
            'ElseIf maxRunwayLength >= 4500 And maxRunwayLength < 5000 Then
            '    imageUrl = "Images/airport_aqua.gif" '#00CCCC
            'ElseIf maxRunwayLength >= 5000 And maxRunwayLength < 5500 Then
            '    imageUrl = "Images/airport_purple.gif" '#CC00CC
            'ElseIf maxRunwayLength >= 5500 And maxRunwayLength < 6000 Then
            '    imageUrl = "Images/airport_orange.gif" '#FF8000
            'ElseIf maxRunwayLength >= 6000 Then
            '    imageUrl = "Images/airport_red.gif" '#CC0000
            'End If

            imageUrl = "Images/airport_purple.gif" '#CC00CC

            'Me.Label1.Text &= "Location ID: " & CStr(dr("LocationID")) & " Facility Name: " & CStr(dr("FacilityName")) & _
            '                    " Latitude: " & CStr(dr("Latitude")) & " Longitude: " & CStr(dr("Longitude")) & _
            '                    " Miles: " & CStr(dr("Miles")) & "</br>"

            i += 1

            'get route info and add pushpin

            'coordinate array used to set map view to ensure all coords are shown in map
            _mapData &= "vll = new VELatLong(" & CStr(dr("Latitude")) &
                            ", " & CStr(dr("Longitude")) & ");" & ControlChars.CrLf

            _mapData &= "latLongArr[" & i.ToString & "] = vll;" & ControlChars.CrLf

            If isOrigin Then
                _mapData &= "latLongOriginArr[" & i.ToString & "] = vll;" & ControlChars.CrLf
            Else
                _mapData &= "latLongDestArr[" & (i - _originCount).ToString & "] = vll;" & ControlChars.CrLf
            End If

            '20120424 - pab - convert from mappoint to bing
            'Dim routeInfo As MapPointService.Route = oMapping.GetRoute(latLong.Latitude, _
            '                        latLong.Longitude, CDbl(dr("Latitude")), CDbl(dr("Longitude")))
            'Dim time As Double = 0
            'Dim timeHrs As Integer = 0
            'Dim timeMins As Integer = 0
            'Dim timeDisplay As String = String.Empty
            'Dim distance As Double = 0

            '20120424 - pab - convert from mappoint to bing
            'If Not routeInfo Is Nothing Then
            '    time = routeInfo.Itinerary.TripTime
            '    distance = Math.Round(routeInfo.Itinerary.Distance, 2)
            'End If
            'If time / 60 >= 60 Then
            '    timeHrs = CInt(Math.Floor(time / 3600))
            '    timeMins = CInt(Math.Floor((time - timeHrs * 3600) / 60))
            '    timeDisplay = timeHrs.ToString & " hr " & timeMins.ToString & " min"
            'Else 'minutes only
            '    timeDisplay = Math.Floor(time / 60).ToString & " min"
            'End If

            'add pushpin
            _mapData &= "addPushpin('" & CStr(dr("LocationID")) & "_" & (i - 1).ToString & "', new VELatLong(" & CStr(dr("Latitude")) &
                            ", " & CStr(dr("Longitude")) & "), '" & CStr(dr("LocationID")) & " - " & CStr(dr("FacilityName")).Replace("'", "\'") & "', '" &
            "<table border=""0"" cellpadding=""0"" cellspacing=""1"">" '& _
            '"<tr><td align=""right"" style=""padding-right:2px;padding-top:2px;"">" & _
            '"timex:</td><td align=""left"" style=""padding-top:2px;width:75%;"">" & timeDisplay & _
            '"</td></tr>" & _
            '"<tr><td align=""right"" style=""padding-top:2px;padding-right:2px;padding-bottom:5px;"">" & _
            '"Distance:</td><td align=""left"" style=""padding-top:2px;padding-bottom:5px;"">" & CStr(distance) & " mi " & _
            '"</td></tr>"

            ''find nearby points of interest
            '_mapData &= "<tr><td align=""left"" colspan=""2"" style=""border-top:solid 1px #C0C0C0;padding-top:2px;"""">" & _
            '"<table id=""rblFindNearby_" & CStr(i) & """ border=""0"" cellpadding=""0"" cellspacing=""0"">" & _
            ' "<tr>" & _
            ' "<td colspan=""3"">Find Nearby:</td>" & _
            ' "</tr><tr>" & _
            '  "<td><input id=""rblFindNearby_" & CStr(i) & "_0"" type=""radio"" name=""rblFindNearby_" & CStr(i) & """ value=""Car_Rental"" /><label for=""rblFindNearby_" & CStr(i) & "_0"">Car Rental</label></td>" & _
            '  "<td><input id=""rblFindNearby_" & CStr(i) & "_2"" type=""radio"" name=""rblFindNearby_" & CStr(i) & """ value=""Limousine"" /><label for=""rblFindNearby_" & CStr(i) & "_2"">Limousine</label></td>" & _
            '  "<td rowspan=""2"" valign=""top"" style=""padding-left:5px;padding-top:2px;"">" & _
            '  "<input type=""button"" value=""Find"" style=""font-size:0.9em;"" id=""bttnFindNearby_" & CStr(i) & """ " & _
            '  "onclick=""var rblValue = getFindNearbyCategory(\'rblFindNearby_" & CStr(i) & "\'); if (rblValue != \'\') {__doPostBack(\'bttnFindNearby_" & CStr(i) & "\', rblValue + \',airport," & CStr(dr("Latitude")) & "," & CStr(dr("Longitude")) & "," & (CStr(dr("LocationID")) & " - " & CStr(dr("FacilityName"))).Replace(",", " ").Replace("'", "\'") & "\')};""></td>" & _
            '  "</tr><tr>" & _
            '  "<td style=""padding-bottom:3px;""><input id=""rblFindNearby_" & CStr(i) & "_1"" type=""radio"" name=""rblFindNearby_" & CStr(i) & """ value=""Hotel"" /><label for=""rblFindNearby_" & CStr(i) & "_1"">Hotel</label></td>" & _
            '  "<td style=""padding-bottom:3px;""><input id=""rblFindNearby_" & CStr(i) & "_3"" type=""radio"" name=""rblFindNearby_" & CStr(i) & """ value=""Taxi"" /><label for=""rblFindNearby_" & CStr(i) & "_3"">Taxi</label></td>" & _
            ' "</tr>" & _
            '"</table>" & _
            '"</td></tr>"

            _mapData &=
            "<tr><td align=""left"" colspan=""2"" style=""border-top:solid 1px #C0C0C0;padding-top:2px;text-decoration:underline;"">" &
            "<span onmouseover=""this.style.cursor=\'pointer\';"" " &
            " onmouseout=""this.style.cursor=\'default\';"" onclick=""centerItemOnMap(" & CStr(dr("Latitude")) & "," & CStr(dr("Longitude")) & ");"">Center on map</span>" &
            "</td></tr>" &
            "</table>" &
            "', '" & imageUrl & "');" & ControlChars.CrLf

            '& _
            '"Click <font onclick=""airportSelected('' + pushPinArr[i][4] + '', '' + pushPinArr[i][5] + '', '' + route.Itinerary.Time + '');"" class=""pushPinLink"" onmouseover=""this.style.cursor='hand';"">here</font> to select.');"

            'Me.Label1.Text = oMapping.FindAddressByLatLong(CDbl(dr("Latitude")), CDbl(dr("Longitude"))) & "</br>"
        Next
        If Not isOrigin Then
            'ensure all coords are shown
            _mapData &= "map.SetMapView(latLongArr);" & ControlChars.CrLf
        End If

        '20111229 - pab
        AirTaxi.post_timing("MapNearbyAirports End  " & Now.ToString)

    End Sub



    Private Function GetAirportsLatLong(ByVal latlong As LatLong, ByVal carrierid As Integer) As DataTable

        AirTaxi.post_timing("getairports start  " & Now.ToString)

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        Dim dt As New DataTable

        Try

            Dim oMapping As New Mapping

            'get lat long from address, etc
            ' Dim latLong As LatLong = Nothing

            'get nearbly airports
            Dim minRunwayLength As Integer = 2500
            Dim miles As Integer = 100 ' CInt(Me.tbDisplayAirportsWithin.Text) '100 mi radius
            Dim count As Integer = 5 '5 'return top 5
            Dim ds As DataSet = Nothing

            Dim maxRunwayLength As Integer = 0

            'get lat long for this airport
            'ds = oMapping.GetAirportInformationByAirportCode(Me.tbAirport.Text.Trim)

            ' latLong = New LatLong

            ' If ds.Tables(0).Rows.Count > 0 Then
            'latlong.Latitude = CDbl(ds.Tables(0).Rows(0)("latitude"))
            'latlong.Longitude = CDbl(ds.Tables(0).Rows(0)("longitude"))
            'End If

            '20111221 - pab - check min runway length
            '20160429 - pab - don't have currtype
            Dim da As New DataAccess
            'dt = da.GetAircraftTypeServiceSpecsByID(carrierid, CInt(Session("currtype").ToString))
            'If dt.Rows.Count > 0 Then
            '    minRunwayLength = CInt(dt.Rows(0).Item("minrunwaylength").ToString)
            'End If

            AirTaxi.post_timing("FindNearbyAirports start - " & latlong.Latitude & " - " & latlong.Longitude & " - " & Now.ToString)

            'find nearby airports
            ds = oMapping.FindNearbyAirports(latlong.Latitude, latlong.Longitude,
                                                minRunwayLength, miles, count)

            AirTaxi.post_timing("FindNearbyAirports end  " & Now.ToString)

            'Dim da As New DataAccess
            Dim dt_serviceAirports As DataTable = da.GetServiceAirports(carrierid)
            Dim dv_serviceAirports As DataView = dt_serviceAirports.DefaultView

            'create a table to bind to the gridview
            'Dim dt As New DataTable

            Dim dc As DataColumn = Nothing
            Dim dr As DataRow = Nothing

            'dc = New DataColumn("ID", System.Type.GetType("System.Int32"))
            'dc.AutoIncrement = True
            'dc.AutoIncrementSeed = 0
            'dc.AutoIncrementStep = 1
            'dt.Columns.Add(dc)

            dc = New DataColumn("LocationID", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("FacilityName", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("IsServiceAirport", System.Type.GetType("System.Boolean"))
            dt.Columns.Add(dc)

            '20090518 - pab - changes requested by Darrell on 5/14/09
            dc = New DataColumn("DisplayName", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            '20081205 - pab - use string for euro symbol
            'dc = New DataColumn("LandingFees", System.Type.GetType("System.Double"))
            dc = New DataColumn("LandingFees", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            '20081205 - pab - use string for euro symbol
            'dc = New DataColumn("RepositionFees", System.Type.GetType("System.Double"))
            dc = New DataColumn("RepositionFees", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            '20090418 - pab - add additional fees
            dc = New DataColumn("HangarCharges", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("ParkingCost", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("HandlingFeeGPU", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("SecurityFee", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("DeIcingPerCall", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("DeIcingPerUnit", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("FuelDifferential", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)

            dc = New DataColumn("VolumeAirport", System.Type.GetType("System.Boolean"))
            dt.Columns.Add(dc)

            dc = New DataColumn("Notes", System.Type.GetType("System.String"))
            dt.Columns.Add(dc)
            '20090418 - pab - add additional fees - end

            '20120905 - pab -run time improvements
            dc = New DataColumn("latitude", System.Type.GetType("System.Double"))
            dt.Columns.Add(dc)
            dc = New DataColumn("longitude", System.Type.GetType("System.Double"))
            dt.Columns.Add(dc)

            '20100525 - pab - fix error - System.NullReferenceException: Object reference not set to an instance of an object.
            If ds Is Nothing Then
                dr = dt.NewRow
                dr("IsServiceAirport") = False
                dr("LandingFees") = "0"
                dr("RepositionFees") = "0"
                dt.Rows.Add(dr)
            Else

                For Each dro As DataRow In ds.Tables(0).Rows

                    dr = dt.NewRow

                    dr("LocationID") = dro("LocationID").ToString

                    dr("FacilityName") = dro("FacilityName").ToString

                    Dim fn As String = dro("FacilityName").ToString

                    dv_serviceAirports.RowFilter = "LocationID = '" & dro("LocationID").ToString & "'"

                    If dv_serviceAirports.Count > 0 Then
                        dr("IsServiceAirport") = True
                        '20081205 - pab - euro symbol "€"
                        'dr("LandingFees") = CDbl(dv_serviceAirports(0)("LandingFees"))
                        'dr("RepositionFees") = CDbl(dv_serviceAirports(0)("RepositionFees"))
                        dr("LandingFees") = "$" & CStr(dv_serviceAirports(0)("LandingFees"))
                        dr("RepositionFees") = "$" & CStr(dv_serviceAirports(0)("RepositionFees"))
                        '20090418 - pab - add additional fees
                        'dr("HangarCharges") = "$" & CStr(dv_serviceAirports(0)("HangarCharges"))
                        'dr("ParkingCost") = "$" & CStr(dv_serviceAirports(0)("ParkingCost"))
                        'dr("HandlingFeeGPU") = "$" & CStr(dv_serviceAirports(0)("HandlingFeeGPU"))
                        'dr("SecurityFee") = "$" & CStr(dv_serviceAirports(0)("SecurityFee"))
                        'dr("DeIcingPerCall") = "$" & CStr(dv_serviceAirports(0)("DeIcingPerCall"))
                        'dr("DeIcingPerUnit") = "$" & CStr(dv_serviceAirports(0)("DeIcingPerUnit"))
                        'dr("FuelDifferential") = "$" & CStr(dv_serviceAirports(0)("FuelDifferential"))
                        ''20090420 - pab - add additional fees
                        'dr("VolumeAirport") = CBool(dv_serviceAirports(0)("VolumeAirport"))
                        'dr("Notes") = CStr(dv_serviceAirports(0)("Notes"))
                        ''20090518 - pab - changes requested by Darrell on 5/14/09
                        'dr("DisplayName") = CStr(dv_serviceAirports(0)("DisplayName"))
                    Else
                        dr("IsServiceAirport") = False
                        '20081205 - pab - euro symbol "€"
                        'dr("LandingFees") = 0
                        'dr("RepositionFees") = 0
                        dr("LandingFees") = "$0.00"
                        dr("RepositionFees") = "$0.00"
                        '20090418 - pab - add additional fees
                        'dr("HangarCharges") = "$0.00"
                        'dr("ParkingCost") = "$0.00"
                        'dr("HandlingFeeGPU") = "$0.00"
                        'dr("SecurityFee") = "$0.00"
                        'dr("DeIcingPerCall") = "$0.00"
                        'dr("DeIcingPerUnit") = "$0.00"
                        'dr("FuelDifferential") = "$0.00"
                        ''20090420 - pab - add additional fees
                        'dr("VolumeAirport") = False
                        'dr("Notes") = ""
                        ''20090518 - pab - changes requested by Darrell on 5/14/09
                        'dr("DisplayName") = ""
                    End If

                    '20120905 - pab -run time improvements
                    dr("latitude") = CDbl(dro("latitude"))
                    dr("longitude") = CDbl(dro("longitude"))

                    dt.Rows.Add(dr)
                Next
            End If

            AirTaxi.post_timing("getairports end  " & Now.ToString)

        Catch ex As Exception
            Dim s = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "QuoteTrip.aspx.vb GetAirportsLatLong", "")
            '20130930 - pab - change email from
            'SendEmail(_carrierid, "info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " QuoteTrip.aspx.vb GetAirportsLatLong error", s, False, "", False)
            '20131024 - pab - fix duplicate emails
            'SendEmail(_carrierid, _emailfrom, "rkane@coastalaviationsoftware.com", appName & " QuoteTrip.aspx.vb GetAirportsLatLong error", s, False, "", False)
            SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "", appName & " QuoteTrip.aspx.vb GetAirportsLatLong error", s, CInt(Session("carrierid")))

        End Try

        Return dt

    End Function

    Protected Sub bttnNewFlightRequest_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim a As String = ""
        a = a


    End Sub



    'Protected Sub drpFromAirport_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpFromAirport.SelectedIndexChanged


    '    _origAirportCode = drpFromAirport.SelectedItem.Value
    '    Session("origAirportCode") = drpFromAirport.SelectedValue
    '    '     Me.TextBox1.Text = drpFromAirport.SelectedValue
    'End Sub

    'Protected Sub drpToAirport_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpToAirport.SelectedIndexChanged
    '    _destAirportCode = drpToAirport.SelectedItem.Value
    '    Session("destAirportCode") = drpToAirport.SelectedValue
    'End Sub






    Protected Sub addleg(ByVal type As String, ByVal carrierid As Integer)

        '20131015 - pab - add more error handling to catch error - Object reference not set to an instance of an object
        Try

            '20111229 - pab
            AirTaxi.post_timing("addleg Start  " & Now.ToString)


            Dim da As New DataAccess


            Dim dc As DataColumn = Nothing
            Dim dr As DataRow = Nothing

            'Dim dt_priceTable As DataTable = da.GetFlightPricesPerHour(carrierid)

            'Dim currentplane As Integer = Session("currplane") - 1
            ''20120329 - pab - any added to ddl - change code to handle it
            ''If currentplane < 0 Then currentplane = 0
            'If currentplane < 1 Then currentplane = 1


            '' Dim discountPricePerHour As Integer = CInt(dt_priceTable.Rows(currentplane)("DiscountPricePerHour"))
            '' Dim CostPerHourFlightTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourFlightTime"))

            ''20081218 - pab - add additional fees
            'Dim perHourRepositionTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourRepositionTime"))
            'Dim perNightOvernight As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerNightOvernight"))
            'Dim perCycleFee As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerCycleFee"))
            'Dim perDayCrewExpenses As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerDayCrewExpenses"))
            'Dim perHourWaitTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourWaitTime"))

            ''20100524 - pab - add new fees
            'Dim perDayCabinAideExpenses As Double = dt_priceTable.Rows(currentplane)("CostPerDayCabinAideExpenses")

            'Dim CycleFees As Integer

            ''20090223 - pab - change hardcoded values to configurable
            'Dim TaxiMinutes As Integer = CInt(dt_priceTable.Rows(currentplane)("TaxiMinutes"))
            'Dim MinFlightDuration As Integer = CInt(dt_priceTable.Rows(currentplane)("MinFlightDuration"))

            'Dim airspeed As Double
            'airspeed = CDbl((currentplandt_priceTable.Rows(currentplane)("BlockSpeed")) * 1.15077945

            'setup columns if not already done

            If dtflights.Columns.Count = 0 Then

                '20140620 - pab - quote from admin portal
                dtflights = Create_dtflights()
                'dc = New DataColumn("ID", System.Type.GetType("System.Int32"))
                'dc.AutoIncrement = True
                'dc.AutoIncrementSeed = 0
                'dc.AutoIncrementStep = 1
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Service Provider", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Origin", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("OriginFacilityName", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Departs", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Destination", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("DestinationFacilityName", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Arrives", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Flight Duration", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Minutes", System.Type.GetType("System.Int32"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Price", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                '' rk 10/20/2010 add quote editor
                'dc = New DataColumn("PriceEdit", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)


                ''20081218 - pab - add additional fees
                'dc = New DataColumn("PriceExplanationDetail", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("ShowPriceExplanation", System.Type.GetType("System.Boolean"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("EmptyLeg", System.Type.GetType("System.Int32"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("AircraftType", System.Type.GetType("System.Int32"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("WeightClass", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("dbname", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("aircraftlogo", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Name", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("FAQPageURL", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                ' ''20111212 - pab - return table for itinerary
                ''dc = New DataColumn("Itinerary", System.Type.GetType("System.String"))
                ''dtflights.Columns.Add(dc)

                ''20120125 - pab - add carrier logo
                'dc = New DataColumn("carrierlogo", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                ''20120403 - pab - add fuel stops, pets, smoking
                'dc = New DataColumn("FuelStops", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Pets", System.Type.GetType("System.Boolean"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("Smoking", System.Type.GetType("System.Boolean"))
                'dtflights.Columns.Add(dc)

                ''20120525 - pab - add certifications
                'dc = New DataColumn("certifications", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                ''20120619 - pab - add wifi
                'dc = New DataColumn("wifi", System.Type.GetType("System.Boolean"))
                'dtflights.Columns.Add(dc)

                ''20120709 - pab - add lav, power
                'dc = New DataColumn("EnclosedLav", System.Type.GetType("System.Boolean"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("PowerAvailable", System.Type.GetType("System.Boolean"))
                'dtflights.Columns.Add(dc)

                ''20131118 - pab - add more fields to aircraft
                'dc = New DataColumn("InflightEntertainment", System.Type.GetType("System.Boolean"))
                'dtflights.Columns.Add(dc)

                'dc = New DataColumn("ManufactureDate", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                ''20140219 - pab - owner confirmation
                'dc = New DataColumn("OwnerConfirmation", System.Type.GetType("System.String"))
                'dtflights.Columns.Add(dc)

                '20130204 - pab - show weight class under aircraft picture
                dc = New DataColumn("WeightClassTitle", System.Type.GetType("System.String"))
                dtflights.Columns.Add(dc)

            Else

                '20140539 - pab - fix error - Column 'WeightClassTitle' does not belong to table 
                Dim bWeightClassTitle As Boolean = False
                For i As Integer = 0 To dtflights.Columns.Count - 1
                    If dtflights.Columns(i).Caption.ToString.Trim = "WeightClassTitle" Then
                        bWeightClassTitle = True
                        Exit For
                    End If
                Next
                If bWeightClassTitle = False Then
                    dc = New DataColumn("WeightClassTitle", System.Type.GetType("System.String"))
                    dtflights.Columns.Add(dc)
                End If

            End If



            ' Dim milesinair As Long = traveltime(_origAirportCode, _departDestAirportCode)
            Dim minutesinair As Long = traveltime(_origAirportCode, _destAirportCode, carrierid)


            'If _roundtrip Then
            '    milesInAir += milesInAir
            'End If

            '        minutesInAir = CInt(milesInAir / (_airSpeed / 60))

            dr = dtflights.NewRow

            dr("Origin") = _origAirportCode
            dr("Destination") = _destAirportCode

            dr("OriginFacilityName") = fname(_origAirportCode)
            dr("DestinationFacilityName") = fname(_destAirportCode)

            '20101104 - pab - remove hardcoded value
            'dr("Service Provider") = "Portal"
            dr("Service Provider") = da.GetSetting(carrierid, "CompanyName")
            'departure time
            ' dr("Departs") = _departDateTime ' cdate(_leaveDateTime) ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)

            dr("Departs") = Format(CDate(_departDateTime), "M/d/yy ") & Format(CDate(_departDateTime), "t")


            '20081218 - pab - add taxi time
            'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))


            '20100617 - pab - remove taxi minutes - it's included in traveltime
            'Dim adate As Date = DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
            Dim adate As Date = DateAdd(DateInterval.Minute, minutesinair, _departDateTime)

            dr("Arrives") = Format(CDate(adate), "M/d/yy ") & Format(CDate(adate), "t")



            dr("minutes") = minutesinair

            dr("Flight Duration") = formatmins(minutesinair)

            '20120403 - pab - add fuel stops, pets, smoking
            '20140221 - pab - use telerik controls
            'If ddlFuelStops.SelectedValue <> "" Then
            'dr("FuelStops") = ddlFuelStops.SelectedValue
            'If ddlFuelStops1.SelectedValue <> "" Then
            '    dr("FuelStops") = ddlFuelStops1.SelectedValue
            'Else
            dr("FuelStops") = "1"
            'End If

            If IsNothing(Session("certifications")) Then Session("certifications") = ""
            dr("certifications") = Session("certifications").ToString

            '20140221 - pab - use telerik controls
            'dr("Pets") = CBool(chkPets.Checked)
            'dr("Smoking") = CBool(chkSmoking.Checked)
            ''20120525 - pab - add certifications
            ''20140111 - pab - fix error - Object reference not set to an instance of an object
            'dr("certifications") = Session("certifications").ToString
            ''20120619 - pab - add wifi
            'dr("wifi") = CBool(chkWiFi.Checked)
            ''20120709 - pab - add lav, power
            'dr("EnclosedLav") = CBool(chkLav.Checked)
            'dr("PowerAvailable") = CBool(chkPower.Checked)
            ''20131118 - pab - add more fields to aircraft
            'dr("InflightEntertainment") = CBool(chkInflightEntertainment.Checked)
            'Dim collection As IList(Of Telerik.Web.UI.RadComboBoxItem) = RadComboBoxRequests.CheckedItems
            Dim bAllowPets As Boolean = False
            Dim bAllowSmoking As Boolean = False
            Dim bWiFi As Boolean = False
            Dim bLav As Boolean = False
            Dim bPower As Boolean = False
            Dim bInFlightEntertainment As Boolean = False
            'For Each item As Telerik.Web.UI.RadComboBoxItem In collection
            '    Select Case item.Text
            '        Case "Pets"
            '            If item.Checked = True Then bAllowPets = True

            '        Case "Smoking"
            '            If item.Checked = True Then bAllowSmoking = True

            '        Case "WiFi"
            '            If item.Checked = True Then bWiFi = True

            '        Case "Enclosed Lav"
            '            If item.Checked = True Then bLav = True

            '        Case "Power"
            '            If item.Checked = True Then bPower = True

            '        Case "InFlight Entertainment"
            '            If item.Checked = True Then bInFlightEntertainment = True

            '    End Select
            'Next
            dr("Pets") = bAllowPets
            dr("Smoking") = bAllowSmoking
            dr("wifi") = bWiFi
            dr("EnclosedLav") = bLav
            dr("PowerAvailable") = bPower
            dr("InflightEntertainment") = bInFlightEntertainment

            Dim ManufactureDate As Date
            'If Not (IsNothing(RadComboBoxMfcDates.SelectedItem)) Then
            '    Select Case RadComboBoxMfcDates.SelectedItem.Text
            '        Case "Any"
            '            'ok - do nothing
            '        Case "< 5 Years"
            '            ManufactureDate = DateAdd(DateInterval.Year, -5, Now)
            '        Case "< 10 Years"
            '            ManufactureDate = DateAdd(DateInterval.Year, -10, Now)
            '        Case "< 15 Years"
            '            ManufactureDate = DateAdd(DateInterval.Year, -15, Now)
            '        Case "< 20 Years"
            '            ManufactureDate = DateAdd(DateInterval.Year, -20, Now)
            '    End Select
            'End If
            dr("ManufactureDate") = ManufactureDate

            '20130204 - pab - show weight class under aircraft picture
            dr("WeightClassTitle") = ""

            dr("legcode") = "R"

            dtflights.Rows.Add(dr)

            '20160407 - add short runway warning
            'lblOwnerConfirm.Visible = False
            'lblOwnerConfirm.Text = ""
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Dim ShortRunwayMsg As Integer = da.getsettingnumeric(CInt(Session("carrierid")), "ShortRunwayMsg")
            Dim ShortRunwayMsgText As String = da.GetSetting(CInt(Session("carrierid")), "ShortRunwayMsgText")
            Dim dt_orig As DataSet = da.GetAirportInformationByAirportCode(_origAirportCode)
            Dim dt_dest As DataSet = da.GetAirportInformationByAirportCode(_destAirportCode)
            If ShortRunwayMsg = 0 Then ShortRunwayMsg = 4400
            If ShortRunwayMsgText = "" Then ShortRunwayMsgText = "Short Runway Warning"
            If Not IsNothing(dt_orig) And Not IsNothing(dt_dest) Then
                If Not isdtnullorempty(dt_orig.Tables(0)) And Not isdtnullorempty(dt_dest.Tables(0)) Then
                    If dt_orig.Tables(0).Rows(0).Item("maxrunwaylength") < ShortRunwayMsg Or
                                    dt_dest.Tables(0).Rows(0).Item("maxrunwaylength") < ShortRunwayMsg Then
                        If ShortRunwayMsgText <> "" And InStr(lblFlightTimeMsg.Text, ShortRunwayMsgText) = 0 Then
                            lblFlightTimeMsg.Text &= "<br /><b>**" & ShortRunwayMsgText & "</b>"
                            lblFlightTimeMsg.Visible = True
                        End If
                    End If
                End If
            End If

            'add return flight as well
            If type = "R" Then

                dr = dtflights.NewRow

                dr("Origin") = _destAirportCode
                dr("Destination") = _origAirportCode

                dr("OriginFacilityName") = fname(_destAirportCode)
                dr("DestinationFacilityName") = fname(_origAirportCode)

                '   dr("Departs") = CDate(_returnDateTime) ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)



                dr("Departs") = Format(CDate(_returnDateTime), "M/d/yy ") & Format(CDate(_returnDateTime), "t")


                '20081218 - pab - add taxi time
                'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))

                '20100617 - pab - remove taxi minutes - it's included in traveltime
                'Dim adate2 As Date = DateAdd(DateInterval.Minute, minutesinair + 12, _returnDateTime)
                Dim adate2 As Date = DateAdd(DateInterval.Minute, minutesinair, _returnDateTime)

                dr("Arrives") = Format(CDate(adate2), "M/d/yy ") & Format(CDate(adate2), "t")


                '20101104 - pab - remove hardcoded value
                'dr("Service Provider") = "Portal"
                dr("Service Provider") = da.GetSetting(carrierid, "CompanyName")

                dr("minutes") = minutesinair
                dr("Flight Duration") = formatmins(minutesinair)

                '20120403 - pab - add fuel stops, pets, smoking
                '20140221 - pab - use telerik controls
                'If ddlFuelStops.SelectedValue <> "" Then
                'dr("FuelStops") = ddlFuelStops.SelectedValue
                'If ddlFuelStops1.SelectedValue <> "" Then
                '    dr("FuelStops") = ddlFuelStops1.SelectedValue
                'Else
                dr("FuelStops") = "1"
                'End If

                '20120525 - pab - add certifications
                dr("certifications") = Session("certifications").ToString

                '20140221 - pab - use telerik controls
                'dr("Pets") = CBool(chkPets.Checked)
                'dr("Smoking") = CBool(chkSmoking.Checked)
                ''20120619 - pab - add wifi
                'dr("wifi") = CBool(chkWiFi.Checked)
                ''20120709 - pab - add lav, power
                'dr("EnclosedLav") = CBool(chkLav.Checked)
                'dr("PowerAvailable") = CBool(chkPower.Checked)
                ''20131118 - pab - add more fields to aircraft
                'dr("InflightEntertainment") = CBool(chkInflightEntertainment.Checked)
                dr("Pets") = bAllowPets
                dr("Smoking") = bAllowSmoking
                dr("wifi") = bWiFi
                dr("EnclosedLav") = bLav
                dr("PowerAvailable") = bPower
                dr("InflightEntertainment") = bInFlightEntertainment

                dr("ManufactureDate") = ManufactureDate

                '20140219 - pab - owner confirmation
                dr("OwnerConfirmation") = ""

                '20140620 - pab - quote from admin portal
                dr("EmptyLegPricing") = ""

                '20141020 - pab - rewrite quote routine
                dr("legcode") = "R"

                '20130204 - pab - show weight class under aircraft picture
                dr("WeightClassTitle") = ""

                dtflights.Rows.Add(dr)

                '20160117 - pab - quote multi-leg trips
            ElseIf type = "M" Then
                Dim legs As New ArrayList
                Dim s1, s2 As String

                s1 = ""
                s2 = ""

                If Not IsNothing(Session("legs")) Then
                    s1 = Session("legs").ToString
                    Dim n As Integer
                    n = InStr(s1, ";")
                    Do While n > 0
                        s2 = Left(s1, n - 1)
                        legs.Add(s2)
                        s1 = Mid(s1, n + 1)
                        n = InStr(s1, ";")
                    Loop
                End If

                Dim adate2 As Date
                For i As Integer = 1 To legs.Count - 1

                    Dim fromloc As String = ""
                    Dim toloc As String = ""
                    Dim departdate As String = ""
                    Dim departtime As String = ""
                    Dim arrivedate As String = ""
                    Dim arrivetime As String = ""

                    s1 = legs.Item(i).ToString
                    fromloc = Left(s1, InStr(s1, " to ") - 1)
                    toloc = Mid(s1, InStr(s1, " to ") + 4, 4).Trim
                    s2 = Mid(s1, InStr(s1, " at ") + 4).Trim
                    If IsDate(s2) Then
                        departdate = CDate(s2).ToString("d")
                        departtime = CDate(s2).ToString("t")
                    End If

                    minutesinair = traveltime(fromloc, toloc, carrierid)

                    dr = dtflights.NewRow

                    dr("Origin") = fromloc
                    dr("Destination") = toloc
                    dr("OriginFacilityName") = fname(fromloc)
                    dr("DestinationFacilityName") = fname(toloc)
                    dr("Departs") = departdate & " " & departtime
                    adate2 = DateAdd(DateInterval.Minute, minutesinair, CDate(departdate & " " & departtime))
                    dr("Arrives") = adate2
                    dr("Service Provider") = da.GetSetting(carrierid, "CompanyName")
                    dr("minutes") = minutesinair
                    dr("Flight Duration") = formatmins(minutesinair)
                    'If ddlFuelStops1.SelectedValue <> "" Then
                    '    dr("FuelStops") = ddlFuelStops1.SelectedValue
                    'Else
                    dr("FuelStops") = "1"
                    'End If
                    dr("certifications") = Session("certifications").ToString
                    dr("Pets") = bAllowPets
                    dr("Smoking") = bAllowSmoking
                    dr("wifi") = bWiFi
                    dr("EnclosedLav") = bLav
                    dr("PowerAvailable") = bPower
                    dr("InflightEntertainment") = bInFlightEntertainment
                    dr("ManufactureDate") = ManufactureDate
                    dr("OwnerConfirmation") = ""
                    dr("EmptyLegPricing") = ""
                    dr("legcode") = "R"
                    dr("WeightClassTitle") = ""

                    dtflights.Rows.Add(dr)

                    dt_dest = da.GetAirportInformationByAirportCode(toloc)
                    If Not IsNothing(dt_dest) Then
                        If Not isdtnullorempty(dt_dest.Tables(0)) Then
                            If dt_dest.Tables(0).Rows(0).Item("maxrunwaylength") < ShortRunwayMsg Then
                                If ShortRunwayMsgText <> "" And InStr(lblFlightTimeMsg.Text, ShortRunwayMsgText) = 0 Then
                                    lblFlightTimeMsg.Text &= "<br /><b>**" & ShortRunwayMsgText & "</b>"
                                    lblFlightTimeMsg.Visible = True
                                End If
                            End If
                        End If
                    End If

                Next

            End If


            Session("flights") = dtflights

            ' ''-- ~~DHA new implmentation for multileg 
            '20160117 - pab - quote multi-leg trips
            'If Session("triptype") <> "M" Then
            '    '20111216 - pab - use radio buttons for select per David
            '    'Me.gvServiceProviderMatrix.DataSource = dtflights
            '    'Me.gvServiceProviderMatrix.DataBind()
            Bind_gvServiceProviderMatrix()
            'End If

            '20111229 - pab
            AirTaxi.post_timing("addleg End  " & Now.ToString)

        Catch ex As Exception
            '20131015 - pab - add more error handling to catch error - Object reference not set to an instance of an object
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(0, appName, Left(Now & " " & s, 500), "addleg", "QuoteTrip.aspx.vb")
            AirTaxi.InsertEmailQueue(CInt(Session("carrierid")), "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                "QuoteTrip.aspx.vb addleg error", s, False, "", "", "", False)

        End Try

    End Sub

    '20111216 - pab - use radio buttons for select per David
    'Protected Sub gvServiceProviderMatrix_RowCreated(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvServiceProviderMatrix.RowCreated

    '    If e.Row.RowType = DataControlRowType.DataRow Then
    '        ' Grab a reference to the Literal control 
    '        Dim output As Literal = CType(e.Row.FindControl("RadioButtonMarkup"), Literal)
    '        ' Output the markup except for the "checked" attribute 
    '        output.Text = String.Format("<input type=""radio"" name=""Select"" id=""RowSelector{0}"" value=""{0}"" />", e.Row.RowIndex)
    '    End If

    'End Sub

    '20111216 - pab - use radio buttons for select per David

    Private Sub GetSelectedRecord()

        For i As Integer = 0 To gvServiceProviderMatrix.Rows.Count - 1
            Dim rb As RadioButton = DirectCast(gvServiceProviderMatrix.Rows(i).Cells(0).FindControl("RadioButton1"), RadioButton)
            If rb IsNot Nothing Then
                If rb.Checked Then
                    Dim hf As HiddenField = DirectCast(gvServiceProviderMatrix.Rows(i).Cells(0).FindControl("HiddenField1"), HiddenField)
                    If hf IsNot Nothing Then
                        'ViewState("SelectedFlight") = hf.Value
                        ViewState("SelectedFlight") = i
                    End If

                    Exit For
                End If
            End If
        Next

    End Sub


    '20111216 - pab - use radio buttons for select per David
    Private Sub GetSelectedRecord2()

        '20140220 - pab - cleanup code
        'For i As Integer = 0 To gvServiceProviderMatrixReturn.Rows.Count - 1
        '    Dim rb As RadioButton = DirectCast(gvServiceProviderMatrixReturn.Rows(i).Cells(0).FindControl("RadioButton2"), RadioButton)
        '    If rb IsNot Nothing Then
        '        If rb.Checked Then
        '            Dim hf As HiddenField = DirectCast(gvServiceProviderMatrixReturn.Rows(i).Cells(0).FindControl("HiddenField2"), HiddenField)
        '            If hf IsNot Nothing Then
        '                'ViewState("SelectedFlight2") = hf.Value
        '                ViewState("SelectedFlight2") = i
        '            End If

        '            Exit For
        '        End If
        '    End If
        'Next

    End Sub


    '20111216 - pab - use radio buttons for select per David
    Private Sub SetSelectedRecord()

        For i As Integer = 0 To gvServiceProviderMatrix.Rows.Count - 1
            Dim rb As RadioButton = DirectCast(gvServiceProviderMatrix.Rows(i).Cells(0).FindControl("RadioButton1"), RadioButton)
            If rb IsNot Nothing Then
                Dim hf As HiddenField = DirectCast(gvServiceProviderMatrix.Rows(i).Cells(0).FindControl("HiddenField1"), HiddenField)
                If hf IsNot Nothing And ViewState("SelectedFlight") IsNot Nothing Then
                    If hf.Value.Equals(ViewState("SelectedFlight").ToString()) Then
                        rb.Checked = True
                        Exit For
                    End If
                End If
            End If
        Next

    End Sub


    '20111216 - pab - use radio buttons for select per David
    Private Sub SetSelectedRecord2()

        '20140220 - pab - cleanup code
        'For i As Integer = 0 To gvServiceProviderMatrixReturn.Rows.Count - 1
        '    Dim rb As RadioButton = DirectCast(gvServiceProviderMatrixReturn.Rows(i).Cells(0).FindControl("RadioButton2"), RadioButton)
        '    If rb IsNot Nothing Then
        '        Dim hf As HiddenField = DirectCast(gvServiceProviderMatrixReturn.Rows(i).Cells(0).FindControl("HiddenField2"), HiddenField)
        '        If hf IsNot Nothing And ViewState("SelectedFlight2") IsNot Nothing Then
        '            If hf.Value.Equals(ViewState("SelectedFlight2").ToString()) Then
        '                rb.Checked = True
        '                Exit For
        '            End If
        '        End If
        '    End If
        'Next

    End Sub


    '20111216 - pab - use radio buttons for select per David
    Sub Bind_gvServiceProviderMatrix()

        GetSelectedRecord()

        '20120125 - pab - show/hide carrier logo on quote based on setting
        Dim da As New DataAccess
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If da.GetSetting(CInt(Session("carrierid")), "ShowCarrierOnQuote") = "Y" Then
            Me.gvServiceProviderMatrix.Columns(1).Visible = True
        Else
            Me.gvServiceProviderMatrix.Columns(1).Visible = False
        End If

        Me.gvServiceProviderMatrix.DataSource = dtflights
        Me.gvServiceProviderMatrix.DataBind()

    End Sub

    '20111216 - pab - use radio buttons for select per David
    Sub Bind_gvServiceProviderMatrixReturn()

        GetSelectedRecord2()

        '20120125 - pab - show/hide carrier logo on quote based on setting
        Dim da As New DataAccess
        '20140220 - pab - cleanup code
        'If da.GetSetting(_carrierid, "ShowCarrierOnQuote") = "Y" Then
        '    Me.gvServiceProviderMatrixReturn.Columns(1).Visible = True
        'Else
        '    Me.gvServiceProviderMatrixReturn.Columns(1).Visible = False
        'End If

        'Me.gvServiceProviderMatrixReturn.DataSource = dtflights2
        'Me.gvServiceProviderMatrixReturn.DataBind()

    End Sub

    '20120402 - pab - shrink aircraft pic size
    'Protected Sub gvServiceProviderMatrix_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvServiceProviderMatrix.RowDataBound

    '    If e.Row.RowIndex >= 0 Then
    '        'e.Row.Cells(0).Visible = False
    '        'e.Row.Cells(1).Visible = False

    '        'Dim dr As GridViewRow
    '        'dr = gvServiceProviderMatrix.Rows(e.Row.RowIndex)
    '        'For i As Integer = 0 To dr.Cells(0).Controls.Count - 1
    '        '    Dim ctl As Control = dr.Cells(0).Controls(i)

    '        '    If TypeOf ctl Is DataBoundLiteralControl Then
    '        '        Dim s As String = CType(ctl, DataBoundLiteralControl).Text.Trim
    '        '    End If
    '        'Next
    '        Dim nHeight As Integer = 0
    '        Dim nWidth As Integer = 0
    '        Dim dr As DataRow
    '        dr = dtflights(e.Row.RowIndex)
    '        Dim path As String = Replace(Session("ApplicationPath").ToString, "/", "\")
    '        Dim sfile As String = Replace(dr.Item(18).ToString, "/", "\")
    '        If File.Exists(path & sfile) Then
    '            Using objImage As System.Drawing.Image = System.Drawing.Image.FromFile(path & sfile)
    '                ' Display its Height and Width
    '                Dim s As String = "Width: " & objImage.Width & "<br />Height: " & objImage.Height
    '                nHeight = objImage.Height / 2
    '                nWidth = objImage.Width / 2
    '            End Using

    '            e.Row.Cells(1).Height = System.Web.UI.WebControls.Unit.Pixel(nHeight)
    '            e.Row.Cells(1).Width = System.Web.UI.WebControls.Unit.Pixel(nWidth)
    '        End If
    '    End If

    'End Sub

    Protected Sub gvServiceProviderMatrix_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvServiceProviderMatrix.SelectedIndexChanged

        Dim r As Long = Me.gvServiceProviderMatrix.SelectedRow.RowIndex

        '20111206 - pab - multiple quotes
        'dtflights.Rows.Item(r).Delete()

        ''dtflights.Rows(1).Delete()
        'Me.gvServiceProviderMatrix.DataSource = dtflights
        'Me.gvServiceProviderMatrix.DataBind()

    End Sub


    '20130429 - pab - move buttons per David - change to telerik buttons
    'Protected Sub cmdStartOver_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdStartOver.Click

    '    cmdStartOverClicked(sender, e)

    'End Sub

    '20130429 - pab - move buttons per David - change to telerik buttons
    Protected Sub cmdStartOverClicked(ByVal sender As Object, ByVal e As System.EventArgs)

        '20111229 - pab
        AirTaxi.post_timing("cmdStartOverClicked Start  " & Now.ToString)

        dtflights.Clear()

        '20111221 - pab multiple quotes
        dtflights2.Clear()

        '20111216 - pab - use radio buttons for select per David
        'Me.gvServiceProviderMatrix.DataSource = dtflights
        'Me.gvServiceProviderMatrix.DataBind()
        Bind_gvServiceProviderMatrix()

        _origAirportCode = ""
        _destAirportCode = ""
        Session("origairportcode") = ""
        Session("destairportcode") = ""

        '20120507 - pab - prevent confirmation email from going out more than once if back button pressed
        Session("confirmemailsent") = "N"

        Dim saveusername, saveusertype As String
        saveusername = ""
        saveusertype = ""
        If Not IsNothing(Session("username")) Then
            If Not IsNothing(Session("usertype")) Then
                saveusername = Session("username")
                saveusertype = Session("usertype")
            End If
        End If
        '20101101 - pab - fix admin start over
        '20150826 - pab - don't abandon session or clear all of the session variables - losing login info
        'If Session("usertype") <> "A" Then
        '    Session.Abandon()
        'Else
        '    For i As Integer = 0 To Session.Count - 1
        '        Session(i) = Nothing
        '    Next
        '    If saveusername <> "" Then Session("username") = saveusername
        '    If saveusertype <> "" Then Session("usertype") = saveusertype
        'End If

        '20111229 - pab
        AirTaxi.post_timing("cmdStartOverClicked End  " & Now.ToString)

        '20120125 - pab -remove widget - go to index
        'Response.Redirect("QuoteTrip.aspx")
        '20140310 - pab - acg background image
        '20150317 - pab - remove acg branding
        'Response.Redirect("default.aspx?acg=" & _acg)
        Response.Redirect("home.aspx", True)


    End Sub

    '20120130 - pab - add start over button at top for when no flights returned
    'Protected Sub cmdStartOver2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdStartOver2.Click
    Protected Sub cmdStartOver_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdStartOver.Click

        '20130429 - pab - move buttons per David - change to telerik buttons
        'cmdStartOver_Click(sender, e)
        cmdStartOverClicked(sender, e)

    End Sub


    Function formatmins(ByVal m As Long) As String

        Dim h As Long = Int(m / 60)

        Dim mm As Long = m - h * 60

        Dim mms As String = CStr(mm)
        If mm < 10 Then mms = "0" & CStr(mm)

        Dim hhs As String = CStr(h)
        If h < 10 Then hhs = "0" & CStr(h)


        formatmins = hhs & ":" & mms


    End Function

    '20140220 - pab - cleanup code
    'Protected Sub cmdQuote_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdQuote.Click


    '    '20120706 - pab - fix error when session times out - Object reference not set to an instance of an object
    '    If IsNothing(Session("triptype")) Then
    '        '20120130 - pab - add start over button at top for when no flights returned
    '        'cmdStartOver_Click(sender, e)
    '        cmdStartOverClicked(sender, e)
    '        Exit Sub
    '    End If

    '    AirTaxi.Insertsys_log(0, appName, Left(Now & "Quote Start  ", 500), "cmdQuote_Click", "")

    '    Call quote(_carrierid)

    '    AirTaxi.Insertsys_log(0, appName, Left(Now & "Quote End  ", 500), "cmdQuote_Click", "")

    '    If dtflights.Rows.Count > 3 Then

    '        txtGeoOrig.Visible = False
    '        txtGeoDest.Visible = False
    '        cmdGeoCodeOrig.Visible = False
    '        cmdGeoCodeDest.Visible = False

    '    Else

    '        txtGeoOrig.Visible = True
    '        txtGeoDest.Visible = True
    '        cmdGeoCodeOrig.Visible = True
    '        cmdGeoCodeDest.Visible = True

    '    End If

    '    '20130429 - pab - move buttons per David - change to telerik buttons
    '    'Me.cmdConfirm.Visible = True
    '    Me.cmdConfirm1.Visible = True

    '    ''20131014 - pab - beta mode - do not allow purchase
    '    'Me.cmdConfirm1.Enabled = False
    '    'Me.lblmsg.Text = DataAccess.GetSetting(_carrierid, "BetaMessage")
    '    'Me.lblmsg.Visible = True

    '    If Not IsNothing(Session("usertype")) Then
    '        If Session("usertype") = "A" Then Me.CmdEdit.Visible = True ' rk 10/20/2010 add quote editor
    '    End If

    '    AirTaxi.Insertsys_log(0, appName, Left(Now & "Quote End  ", 500), "Sub quote(ByVal carrierid As Integer)", "")


    'End Sub

    Protected Sub quote(ByVal carrierid As Integer)

        '20111229 - pab
        AirTaxi.post_timing("Quote Start  " & Now.ToString)
        AirTaxi.Insertsys_log(0, appName, Left(Now & "Quote Start  ", 500), "Sub quote(ByVal carrierid As Integer)", "")

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""


        '20110926 - pab - update web services
        'Dim ws As New optimizelowestquote.Service
        '20120201 - pab - changes for azure
        'Dim ws As New AviationService1_9.Service
        '20120423 - pab - call azure web service
        'Dim ws As New AviationService1_10.Service
        Dim ws As New AviationWebService1_10.WebService1

        Dim dsflights, dsquotes As New DataSet
        Dim dr As DataRow
        Dim weightclass As String = ""
        Dim da As New DataAccess
        Dim dt As DataTable

        'clear price rows from dtflights table before calling ws
        '20111220 - pab - multiple quotes
        Dim i As Integer = dtflights.Rows.Count - 1
        'Do While i >= 0
        '    dr = dtflights.Rows(i)
        '    If dr("Origin") = "" Then
        '        dtflights.Rows(i).Delete()
        '    Else
        '        dtflights.Rows(i).Item("service provider") = ""
        '        dtflights.Rows(i).Item("Arrives") = ""
        '        dtflights.Rows(i).Item("Flight Duration") = ""
        '        dtflights.Rows(i).Item("Price") = ""
        '    End If
        '    i = i - 1
        'Loop

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If Not IsNothing("ip") Then Session("ip") = ""
        Dim ip As String = Session("ip").ToString

        '20101214 - pab - fix error when dtflights has no rows
        If dtflights.Rows.Count = 0 Then
            '20140220 - pab - cleanup code
            'Me.cmdQuote.Visible = False
            'Me.LblItin.Visible = False
            Me.lblMsg.Text = ""
            If Session("triptype") <> "M" And Session("triptype") <> "G" Then
                lblMsg.Visible = True

                '20131016 - pab - fix session timeout
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                If IsNothing(Session("flights")) And IsNothing(Session("triptype")) Then
                    lblMsg.Text = da.GetSetting(CInt(Session("carrierid")), "TimeoutMessage")
                    gvServiceProviderMatrix.EmptyDataText = lblMsg.Text
                Else
                    'lblmsg.Text = "No flights available. Please make a new trip request or change aircraft type."
                End If

                '20110906 - pab - don't display contine button if no flights
                '20130429 - pab - move buttons per David - change to telerik buttons
                'Me.cmdConfirm.Visible = False
                'Me.cmdConfirm1.Visible = False

                '20120316 - pab - remove label - now in grid header
                'lblTaxes.Visible = False

                '20111220 - pab - add return flight grid
                '20140220 - pab - cleanup code
                'Me.lblItinReturn.Visible = False
                'Me.gvServiceProviderMatrixReturn.Visible = False

                '20120125 - pab -remove widget - go to index
                '20120130 - pab - add start over button at top for when no flights returned
                'Me.cmdStartOver2.Visible = True
                Me.cmdStartOver.Visible = True
                '20130429 - pab - move buttons per David - change to telerik buttons
                'Me.cmdStartOver.Visible = False
                'Me.cmdStartOver1.Visible = False

            End If
            '20150831 - pab - add more logging to find out why sometimes no quotes generated
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "dtflights.Rows.Count = 0 - exit sub", "QuoteTrip.aspx.vb quote", "")
            Exit Sub
        End If

        Dim sdtflights As String = dtflightstoxml(dtflights)
        dsflights.Tables.Add(dtflights.Copy())

        weightclass = "L"
        ''20140221 - pab - use telerik controls
        ''If IsNumeric(ddlAircraftServiceTypes.SelectedValue) Then
        ''dt = da.GetAircraftTypeServiceSpecsByID(carrierid, ddlAircraftServiceTypes.SelectedValue)
        'If IsNumeric(ddlAircraftServiceTypes1.SelectedValue) Then
        '    dt = da.GetAircraftTypeServiceSpecsByID(carrierid, ddlAircraftServiceTypes1.SelectedValue)
        '    If Not dt Is Nothing Then
        '        If dt.Rows.Count > 0 Then
        '            weightclass = dt.Rows(0).Item("weightclass").ToString
        '        End If
        '    End If
        'End If

        '20120123 - pab - fix passengers being reset
        'If _passengers = 0 Then _passengers = 1
        If _passengers = 0 Then
            If IsNothing(Session("passengers")) Then
                _passengers = 1
            ElseIf IsNumeric(Session("passengers")) Then
                _passengers = CInt(Session("passengers"))
            Else
                _passengers = 1
            End If
            'Else
            '    _passengers = 1
        End If
        If Session("usertype") Is Nothing Then Session("usertype") = "M"
        If Session("plantype") Is Nothing Then Session("plantype") = ""

        '20120316 - pab - remove label - now in grid header
        'lblTaxes.Visible = False

        '20111220 - pab - add return flight grid
        '20140220 - pab - cleanup code
        'Me.lblItinReturn.Visible = False
        'Me.gvServiceProviderMatrixReturn.Visible = False

        '20131206 - pab - make quote email configurable
        Dim emailsenttoQuote As String = da.GetSetting(CInt(Session("carrierid")), "emailsenttoQuote")
        If emailsenttoQuote = "" Then emailsenttoQuote = "personifyquote@gmail.com"

        '20140111 - pab - fix error - Object reference not set to an instance of an object
        If IsNothing(Session("achourly")) Then Session("achourly") = ""


        '20160711 - pab - populate quote fields on bottom
        hydratesearchfields()


        '20140221 - pab - use telerik controls
        'Dim collection As IList(Of Telerik.Web.UI.RadComboBoxItem) = RadComboBoxRequests.CheckedItems
        Dim bAllowPets As Boolean = False
        Dim bAllowSmoking As Boolean = False
        Dim bWiFi As Boolean = False
        Dim bLav As Boolean = False
        Dim bPower As Boolean = False
        Dim bInFlightEntertainment As Boolean = False
        'For Each item As Telerik.Web.UI.RadComboBoxItem In collection
        '    Select Case item.Text
        '        Case "Pets"
        '            If item.Checked = True Then bAllowPets = True

        '        Case "Smoking"
        '            If item.Checked = True Then bAllowSmoking = True

        '        Case "WiFi"
        '            If item.Checked = True Then bWiFi = True

        '        Case "Enclosed Lav"
        '            If item.Checked = True Then bLav = True

        '        Case "Power"
        '            If item.Checked = True Then bPower = True

        '        Case "InFlight Entertainment"
        '            If item.Checked = True Then bInFlightEntertainment = True

        '    End Select
        'Next

        '20111221 - pab - get lowest quote for all weight classes
        '20140414 - pab - add acg indicator for logging and tracking
        'If Session("triptype") = "R" Or Session("triptype") = "O" Or Session("triptype") = "L" Then
        If Session("triptype") = "R" Or Session("triptype") = "O" Or Session("triptype") = "L" Or Session("triptype") = "M" Then
            Try
                dtquotes.Clear()

                '20140414 - pab - add acg indicator for logging and tracking
                Dim s As String = "Personifly Quote requested - "
                Select Case Session("triptype").ToString
                    Case "R"
                        s &= "Round trip "
                    Case "O"
                        s &= "One way "
                        '20160117 - pab - quote multi-leg trips
                    Case "L", "M"
                        s &= "Multi-leg "
                End Select
                For i2 As Integer = 0 To dsflights.Tables(0).Rows.Count - 1
                    s &= dsflights.Tables(0).Rows(i2).Item("Origin").ToString.Trim & " to " & dsflights.Tables(0).Rows(i2).Item("Destination").ToString.Trim &
                        " departing " & dsflights.Tables(0).Rows(i2).Item("Departs").ToString.Trim & "; "
                Next
                'do not insert if debugging or Paula's IP
                If InStr(ip, "127.0.0.1") = 0 And InStr(ip, "173.162.122.246") = 0 Then AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, Left(s, 500), "quote", ip)

                'Dim sdtflights As String = dtflightstoxml(dtflights)

                'chkTesting.Checked = True

                '20120104 - pab - pass name and email for delta
                Dim firstname As String = "Personifly"
                Dim lastname As String = "CAS"
                Dim email As String = "Flights@CoastalAviationSoftware.com"

                '20150821 - pab - force login before getting quotes to track users
                Dim MemberID As Integer = 0
                Dim MemberIDCarrier As Integer = 0

                '20140423 - pab - quote empty legs
                If Not IsNothing(Session("FirstName")) Then
                    If Session("FirstName").ToString.Trim <> "" Then firstname = Session("FirstName").ToString.Trim
                End If
                If Not IsNothing(Session("LastName")) Then
                    If Session("LastName").ToString.Trim <> "" Then lastname = Session("LastName").ToString.Trim
                End If
                If Not IsNothing(Session("email")) Then
                    'If Session("email").ToString.Trim <> "" Then email = Session("email").ToString.Trim
                End If

                '20150821 - pab - force login before getting quotes to track users
                If Not IsNothing(Session("MemberID")) Then
                    If IsNumeric(Session("MemberID").ToString.Trim) Then MemberID = CInt(Session("MemberID").ToString.Trim)
                End If
                If Not IsNothing(Session("MemberIDCarrier")) Then
                    If IsNumeric(Session("MemberIDCarrier").ToString.Trim) Then MemberIDCarrier = CInt(Session("MemberIDCarrier").ToString.Trim)
                End If

                '20111230 - pab - get only one quote if from external link
                '20120111 - pab - or weightclass selected
                'If Session("dqn").ToString <> "" And Session("weightclass").ToString <> "" Then
                '20120208 - pab - return multiple quotes if weight class selected
                'If Session("dqn").ToString <> "" Or Session("weightclass").ToString <> "" Then
                If Session("dqn").ToString <> "" And Session("weightclass").ToString <> "" Then

                    '20111229 - pab
                    AirTaxi.post_timing("Quote " & Session("weightclass").ToString & "  " & Now.ToString)

                    AirTaxi.Insertsys_log(0, appName, Left(Now & "GetLowestQuoteByWeightClass  ", 500), " GetLowestQuoteByWeightClass 4158 Start", "")

                    '20140221 - pab - use telerik controls
                    '20150821 - pab - force login before getting quotes to track users
                    GetLowestQuoteByWeightClass(Session("weightclass").ToString, _passengers, Session("triptype"), Session("usertype"), Session("plantype"),
                        dsflights, False, ip, firstname, lastname, email, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, bInFlightEntertainment, MemberID,
                        MemberIDCarrier)

                    AirTaxi.Insertsys_log(0, appName, Left(Now & "GetLowestQuoteByWeightClass  ", 500), " GetLowestQuoteByWeightClass 4158 End", "")

                    '20121204 - pab - implement aircraft to include
                ElseIf Session("achourly").ToString <> "" Then
                    AirTaxi.post_timing("Quote " & Session("achourly").ToString & "  " & Now.ToString)

                    AirTaxi.Insertsys_log(0, appName, Left(Now & " GetLowestQuoteByWeightClassPartial  ", 500), " GetLowestQuoteByWeightClassPartial 4173 Start", "")

                    '20131118 - pab - add more fields to aircraft
                    '20140221 - pab - use telerik controls
                    '20150821 - pab - force login before getting quotes to track users
                    GetLowestQuoteByWeightClassPartial("A", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip,
                        firstname, lastname, email, Session("achourly").ToString, bInFlightEntertainment, ManufactureDate, bAllowPets, bAllowSmoking, bWiFi,
                        bLav, bPower, bInFlightEntertainment, MemberID, MemberIDCarrier)
                    AirTaxi.Insertsys_log(0, appName, Left(Now & " GetLowestQuoteByWeightClassPartial  ", 500), " GetLowestQuoteByWeightClassPartial 4173 End", "")


                    '20120208 - pab - return multiple quotes if weight class selected
                ElseIf Session("weightclass").ToString <> "" Then

                    '20111229 - pab
                    AirTaxi.post_timing("Quote " & Session("weightclass").ToString & "  " & Now.ToString)

                    AirTaxi.Insertsys_log(0, appName, Left(Now & " GetQuotesByWeightClassAllCarriers  ", 500), " GetLowestQuoteByWeightClass 4191 Start", "")

                    '20140221 - pab - use telerik controls
                    '20150821 - pab - force login before getting quotes to track users
                    GetQuotesByWeightClassAllCarriers(Session("weightclass").ToString, _passengers, Session("triptype"), Session("usertype"), Session("plantype"),
                        dsflights, False, ip, firstname, lastname, email, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, bInFlightEntertainment, MemberID,
                        MemberIDCarrier)
                    AirTaxi.Insertsys_log(0, appName, Left(Now & " GetQuotesByWeightClassAllCarriers  ", 500), " GetLowestQuoteByWeightClass 4191 End", "")


                Else



                    '20120206 - pab - and configurable setting to ShowMultipleQuotes y/n
                    Dim ShowMultipleQuotes As Boolean = False
                    If da.GetSetting(CInt(Session("carrierid")), "ShowMultipleQuotes") = "Y" Then ShowMultipleQuotes = True

                    '20120206 - pab - and configurable setting to ShowMultipleQuotes y/n
                    ''20120111 - pab - run time improvements - quote medium first, only quote light or heavy if no quote previously returned
                    'AirTaxi.post_timing("Quote Medium  " & Now.ToString)
                    ''Medium
                    'GetLowestQuoteByWeightClass("M", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                    'rk 6/13/2012
                    'AirTaxi.Insertsys_log(0, "selectapt4023", Left(Now, 500), "start lowest quote A ", "")
                    AirTaxi.Insertsys_log(0, appName, Left(Now & " GetQuotesByWeightClass A  ", 500), " GetQuotesByWeightClass 4216 Start", "")

                    'rk 6.13.2012 call only once for all types
                    If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote All " & Now.ToString)
                    '20140221 - pab - use telerik controls
                    '20150821 - pab - force login before getting quotes to track users
                    If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("A", _passengers, Session("triptype"),
                        Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email, bAllowPets, bAllowSmoking, bWiFi,
                        bLav, bPower, bInFlightEntertainment, MemberID, MemberIDCarrier)
                    If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote All Complete " & Now.ToString)
                    AirTaxi.Insertsys_log(0, appName, Left(Now & " GetQuotesByWeightClass A  ", 500), " GetQuotseByWeightClass 4216 End", "")


                    ''Light
                    ''20120206 - pab - and configurable setting to ShowMultipleQuotes y/n
                    ''If dtquotes.Rows.Count = 0 Then AirTaxi.post_timing("Quote Light  " & Now.ToString)
                    ''If dtquotes.Rows.Count = 0 Then GetLowestQuoteByWeightClass("L", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Light  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("L", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                    ''Medium
                    ''AirTaxi.post_timing("Quote Medium  " & Now.ToString)
                    ''GetLowestQuoteByWeightClass("M", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Medium  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("M", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                    ''20120507 - pab - add new weight classes - Twin Piston, Single Turboprop, Twin Turboprop, SuperMid Jet
                    ''SuperMid
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote SuperMid Jet  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("U", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                    ''Heavy
                    ''If dtquotes.Rows.Count = 0 Then AirTaxi.post_timing("Quote Heavy  " & Now.ToString)
                    ''If dtquotes.Rows.Count = 0 Then GetLowestQuoteByWeightClass("H", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Heavy  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("H", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                    ''VLJ
                    ''If dtquotes.Rows.Count = 0 Then AirTaxi.post_timing("Quote VLJ  " & Now.ToString)
                    ''If dtquotes.Rows.Count = 0 Then GetLowestQuoteByWeightClass("V", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote VLJ  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("V", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                    ''20111229 - pab
                    ''Propeller
                    ''If dtquotes.Rows.Count = 0 Then AirTaxi.post_timing("Quote Propeller  " & Now.ToString)
                    ''If dtquotes.Rows.Count = 0 Then GetLowestQuoteByWeightClass("P", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Single Piston  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("P", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)


                    ''20120507 - pab - add new weight classes - Twin Piston, Single Turboprop, Twin Turboprop, SuperMid Jet
                    ''Twin Piston
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Twin Piston  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("T", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                    ''Single Turboprop
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Single Turboprop  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("1", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                    ''Twin Turboprop
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Twin Turboprop  " & Now.ToString)
                    'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("2", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

                End If

                '20160603 - pab - add code to do additional direct lookup if no quotes returned by web service
                If dtquotes.Rows.Count = 0 Then
                    dtquotes.Clear()
                    For j As Integer = 1 To 5
                        System.Threading.Thread.Sleep(1000)

                        '20171025 - pab - fix dbnull conversion error
                        'dtquotes = da.GetD2DQuoteQueuebyRequestParms(_carrierid, Session("triptype"), ip, _origAirportCode, _destAirportCode)
                        dtquotes = da.GetD2DQuoteQueuebyRequestParms(CInt(Session("carrierid")), Session("triptype"), ip, _origAirportCode,
                            _destAirportCode, DateAdd(DateInterval.Minute, -5, Now))
                        If Not isdtnullorempty(dtquotes) Then
                            If Session("triptype") = "M" Then
                                dsquotes.Clear()
                                dsquotes.Tables.Add(dtquotes)
                                CreateFlightsTableMulti(dsquotes)
                            Else
                                CreateFlightsTable(dtquotes)
                            End If

                            Exit For
                        End If
                    Next

                    If isdtnullorempty(dtquotes) Then
                        AirTaxi.post_timing("Quote no flights - lookup by request parms " & Now.ToString)

                        lblMsg.Visible = True
                        lblMsg.Text = da.GetSetting(carrierid, "NotAvailable")
                        gvServiceProviderMatrix.EmptyDataText = "<br />" & lblMsg.Text & "<br />"
                        lblMsg.Text = ""
                        dtflights = Nothing
                        dtflights2 = Nothing
                        Session("flights") = dtflights
                        Session("flights2") = dtflights2
                        Bind_gvServiceProviderMatrixReturn()

                        Me.cmdStartOver.Visible = True
                    End If
                End If

                If dtquotes.Rows.Count > 0 Then

                    dtflights.Clear()

                    '20111221 - pab multiple quotes
                    '20160512 - pab - fix object not set
                    If Not IsNothing(dtflights2) Then dtflights2.Clear()

                    dtflights = dtquotes.Copy

                    '20111229 - pab
                    AirTaxi.post_timing("Quote Email and Tweet  " & Now.ToString)

                    '20120125 - pab - save dqn2 for purchase
                    Dim dqn2 As String = InBetween(1, dtflights.Rows(0).Item("PriceExplanationDetail"), "; dqn ", ";")
                    Session("dqn2") = dqn2
                    Session("quotenumber") = 0

                    '20120103 - pab - save click thru info if from airboard
                    If Session("dqn").ToString <> "" Then
                        AirTaxi.post_timing("Start dqn " & Now.ToString)

                        Dim dt1 As DataTable
                        Dim carrierid1 As Integer = 0
                        dt1 = da.GetProviderByAlias(dtflights.Rows(0).Item("service provider"))
                        If dt1.Rows.Count > 0 Then
                            carrierid1 = dt1.Rows(0).Item("carrierid")
                        End If

                        dtflights.TableName = "dtflights"

                        '20120125 - pab - save dqn2 for purchase
                        'Dim dqn2 As String = InBetween(1, dtflights.Rows(0).Item("PriceExplanationDetail"), "; dqn ", ";")
                        AirTaxi.Insertsys_log(0, appName, Left(Now & "recordquotexml  ", 500), " recordquotexmlStart", "")

                        Dim qn As Integer = recordquotexml(dtflights, "", "", 0, "", 9, "", "", "", "", "", "", carrierid)
                        Session("quotenumber") = qn
                        'Session("dqn2") = dqn2
                        AirTaxi.Insertsys_log(0, appName, Left(Now & "recordquotexml  ", 500), " recordquotexmlEnd", "")

                        da.InsertAirBoardClickThru(carrierid1, Session("dqn").ToString, dqn2, qn, Now, 0, CDbl(Session("airboardprice").ToString))
                        AirTaxi.post_timing("Stop dqn " & Now.ToString)

                    End If



                    AirTaxi.post_timing("start emails " & Now.ToString)

                    '20131118 - pab - add more fields to aircraft
                    Dim bownerconfirm As Boolean = False

                    '20140620 - pab - quote from admin portal
                    Dim bemptyleg As Boolean = False

                    '20141106 - pab - rewrite quote routine
                    'Dim sortExp As String = "Price,WeightClass,quoteID"
                    Dim sortExp As String = "WeightClass,quoteID"
                    Dim drarray() As DataRow
                    drarray = dtflights.Select(Nothing, sortExp, DataViewRowState.CurrentRows)

                    '20111208 - pab - email all quotes per Richard
                    '20141106 - pab - rewrite quote routine
                    'For i = 0 To dtflights.Rows.Count - 1
                    For i = 0 To drarray.Count - 1

                        '20141106 - pab - rewrite quote routine
                        dr = drarray(i)

                        '20120530 - pab - fix error - Conversion from type 'DBNull' to type 'String' is not valid.
                        'Dim scerts As String = dtflights.Rows(i).Item("certifications")
                        Dim scerts As String = ""
                        If Not IsDBNull(dr.Item("certifications")) Then
                            scerts = dr.Item("certifications").ToString
                        End If
                        '20120530 - pab - for some reason certifications coming back null
                        If scerts = "" Then
                            Dim dt1 As DataTable = da.GetProviderByAlias(dr.Item("service provider"))
                            If Not isdtnullorempty(dt1) Then
                                scerts = "Ratings: "
                                If Not IsDBNull(dt1.Rows(0)("ARGUSlevel")) Then
                                    If dt1.Rows(0)("ARGUSlevel").ToString <> "" Then scerts &= dt1.Rows(0)("ARGUSlevel").ToString
                                End If
                                If Not IsDBNull(dt1.Rows(0)("WYVERNlevel")) Then
                                    If dt1.Rows(0)("WYVERNlevel").ToString <> "" Then scerts &= vbNewLine & dt1.Rows(0)("WYVERNlevel").ToString
                                End If
                                If Not IsDBNull(dt1.Rows(0)("Sentientlevel")) Then
                                    If dt1.Rows(0)("Sentientlevel").ToString <> "" Then scerts &= vbNewLine & dt1.Rows(0)("Sentientlevel").ToString
                                End If
                                scerts &= vbNewLine & vbNewLine
                            End If
                            scerts &= "Certifications: " & da.GetSetting(carrierid, "Certification")
                            dr.Item("certifications") = scerts
                        End If

                        '20120503 = pab - create aircraft type link if blank
                        Dim carrierid1 As Integer = 0
                        If dr.Item("FAQPageURL").ToString = "" Then
                            Dim dt1 As DataTable
                            dt1 = da.GetProviderByAlias(dr.Item("service provider"))
                            If dt1.Rows.Count > 0 Then
                                carrierid1 = dt1.Rows(0).Item("carrierid")
                            Else
                                dt1 = da.GetProviderByCompanyName(dr.Item("service provider"))
                                If dt1.Rows.Count > 0 Then
                                    carrierid1 = dt1.Rows(0).Item("carrierid")
                                End If
                            End If
                            '20160603 - pab - fix error for data from direct lookup - Column 'FAQPageURL' is read only.
                            'If IsNumeric(dr.Item("AircraftType").ToString) Then
                            '    dr.Item("FAQPageURL") = "AircraftTypeDetails.aspx?carrierid=" & carrierid1 & "&atssid=" & CInt(dr.Item("AircraftType").ToString)
                            'Else
                            '    dr.Item("FAQPageURL") = "AircraftTypeDetails.aspx?carrierid=" & carrierid1 & "&atssid=" & 0
                            'End If

                            '20150127 - pab - upload and display pdfs
                        Else
                            'check if pdf from blob
                            If InStr(dr.Item("FAQPageURL").ToString.ToLower, ".pdf") > 0 And InStr(dr.Item("FAQPageURL").ToString, "/") = 0 Then
                                'not a full url so must be blob pdf
                                dr.Item("FAQPageURL") = "AircraftTypePDF.aspx?acpdf=" & dr.Item("FAQPageURL").ToString
                            End If

                        End If

                        '20131118 - pab - add more fields to aircraft
                        If dr.Item("OwnerConfirmation").ToString <> "" Then
                            '20141209 - pab - fix footnotes
                            s = dr.Item("OwnerConfirmation").ToString
                            Do While InStr(s, "<strong>") > 0
                                If InBetween(1, s, "<strong>", "</strong>") <> "" Then
                                    bownerconfirm = True
                                    Exit Do
                                End If
                                s = Mid(s, InStr(s, "</strong>") + 1)
                            Loop
                        End If

                        '20140620 - pab - quote from admin portal
                        If dr.Item("EmptyLegPricing").ToString <> "" Then
                            '20141209 - pab - fix footnotes
                            s = dr.Item("EmptyLegPricing").ToString
                            Do While InStr(s, "<td>") > 0
                                If InBetween(1, s, "<td>", "</td>") <> "" Then
                                    bemptyleg = True
                                    Exit Do
                                End If
                                s = Mid(s, InStr(s, "</td>") + 1)
                            Loop
                        End If

                        '20111208 - pab - tweet all quotes per Richard
                        '20120111 - pab make tweeting configurable
                        'If da.GetSetting(_carrierid, "SendTweet") = "Y" Then
                        '    AirTaxi.Insertsys_log(0, appName, Left(Now & "tweetstart  ", 500), " recordquotexmlEnd", "")


                        '    If IsNumeric(InBetween(1, dtflights.Rows(i).Item("price"), "<strong>", "</strong>")) And Not chkTesting.Checked Then
                        '        Dim owrt As String = String.Empty
                        '        If Session("triptype") = "R" Then owrt = "Round Trip"
                        '        If Session("triptype") = "O" Then owrt = "One Way"

                        '        Dim sTweet As String = String.Empty

                        '        Try
                        '            'twitter
                        '            If owrt <> "" Then
                        '                '20111208 - pab - calculate price per seat
                        '                Dim priceperseat As Double = CDbl(InBetween(1, dtflights.Rows(i).Item("price"), "<strong>", "</strong>"))
                        '                Dim dtcarrier As DataTable = da.GetSettingCarrierIDBySetting(dtflights.Rows(i).Item("Service Provider").ToString, "CompanyName")
                        '                Dim dtat As DataTable

                        '                If Not IsNothing(dtcarrier) Then
                        '                    If dtcarrier.Rows.Count > 0 Then
                        '                        dtat = da.GetAircraftTypeServiceSpecsByID(CInt(dtcarrier.Rows(0).Item("carrierid").ToString), CInt(dtflights.Rows(i).Item("AircraftType").ToString))
                        '                        If Not IsNothing(dtat) Then
                        '                            If dtat.Rows.Count > 0 Then
                        '                                If IsNumeric(dtat.Rows(0).Item("totalpassengers").ToString) Then priceperseat = priceperseat / CInt(dtat.Rows(0).Item("totalpassengers").ToString)
                        '                            End If
                        '                        End If
                        '                    End If
                        '                End If
                        '                Dim stattweet As String = ("Fly " & owrt & " from " & fname(Session("origairportcode")) & " To " & fname(Session("destairportcode")) & " for " & Format(priceperseat, "$###,##0.00"))
                        '                If Len(stattweet) > 144 Then stattweet = Left(stattweet, 144)

                        '                sTweet = wscas.Tweet("pbaumgart@ctgi.com", "123", _carrierid, stattweet)
                        '            End If
                        '        Catch
                        '            'oh well tweet off
                        '        End Try
                        '    End If
                        '    AirTaxi.Insertsys_log(0, appName, Left(Now & "tweetstart  ", 500), " tweet end", "")

                        'End If
                    Next

                    '20131118 - pab - add more fields to aircraft
                    'lblOwnerConfirm.Visible = False
                    'lblOwnerConfirm.Text = ""
                    If bownerconfirm = True Then
                        '20140227 - pab - emphasize owner confirmation 
                        'If InStr(lblFlightTimeMsg.Text, "<br />+") = 0 Then lblFlightTimeMsg.Text &= "<br />+" & da.GetSetting(_carrierid, "operator/owner confirmation")
                        '20140528 - pab - use diamond instead of plus sign
                        'If InStr(lblOwnerConfirm.Text, "+") = 0 Then
                        '   lblOwnerConfirm.Text = "+" & da.GetSetting(_carrierid, "operator/owner confirmation")
                        '20140620 - pab - quote from admin portal
                        'If InStr(lblOwnerConfirm.Text, "&diams;") = 0 Then
                        '    lblOwnerConfirm.Text = "&diams; " & da.GetSetting(_carrierid, "operator/owner confirmation")
                        '    lblOwnerConfirm.Visible = True
                        'End If
                        'lblOwnerConfirm.Text = da.GetSetting(_carrierid, "ownerconfirmationSpecialChar") & " " & da.GetSetting(_carrierid, "operator/owner confirmation")
                        'lblOwnerConfirm.Visible = True
                    End If

                    '20140620 - pab - quote from admin portal
                    If bemptyleg = True Then
                        'If lblOwnerConfirm.Text <> "" Then lblOwnerConfirm.Text &= "   "
                        'lblOwnerConfirm.Text &= da.GetSetting(_carrierid, "EmptyLegSpecialCharacter") & " " & da.GetSetting(_carrierid, "EmptyLegMessage")
                        'lblOwnerConfirm.Visible = True
                    End If

                    AirTaxi.post_timing("stop emails " & Now.ToString)

                    '20111220 - pab - add return flight grid
                    '20141110 - PAB - rewrite quote routine
                    'If Session("triptype") = "R" Then

                    '    '20111229 - pab
                    '    AirTaxi.post_timing("Quote CreateReturnTable  " & Now.ToString)

                    '    '20120215 - pab - combine return flights with outbound
                    '    'CreateReturnTable(dtflights)
                    '    CreateCombinedTable(dtflights)
                    '    'Session("flights2") = dtflights2
                    '    'Bind_gvServiceProviderMatrixReturn()
                    '    'Me.lblItinReturn.Visible = True
                    '    'Me.gvServiceProviderMatrixReturn.Visible = True
                    'End If

                    '20111230 - pab - fix message if flight available
                    If dtflights.Rows.Count > 0 Then

                        'rk 6.15.2012 remove email as delay source
                        For i2 As Integer = 0 To dtflights.Rows.Count - 1
                            If dtflights.Rows(i2).Item("PriceExplanationDetail").ToString <> "" Then
                                'If Not chkTesting.Checked And dtflights.Rows(i2).Item("service provider").ToString.ToUpper <> "DELTA" Then
                                If dtflights.Rows(i2).Item("service provider").ToString.ToUpper <> "DELTA" Then
                                    Dim emailbody As String = dtflights.Rows(i2).Item("PriceExplanationDetail").ToString
                                    Dim carrierid1 As Integer = 0
                                    Dim dt1 As DataTable = da.GetProviderByCompanyName(dtflights.Rows(i2).Item("service provider").ToString)
                                    If Not isdtnullorempty(dt1) Then
                                        carrierid1 = dt1.Rows(0).Item("carrierid")
                                    End If
                                    '20121130 - pab - display from and to airports in email subject
                                    '20141117 - pab - fix email headings after quoteworker rewrite
                                    Dim emailsubject As String = "Personifly Quote for " & dtflights.Rows(i2).Item("service provider").ToString
                                    If Session("triptype") = "R" Then
                                        emailsubject &= " Round Trip "
                                        'emailsubject &= Replace(dtflights.Rows(i2).Item("Origin").ToString, "<br />", " to ")

                                        '20160316 - pab - multi-leg
                                    ElseIf Session("triptype") = "M" Then
                                        emailsubject &= " Multi-Leg "

                                    Else
                                        emailsubject &= " One Way "
                                        'emailsubject &= dtflights.Rows(i2).Item("Origin").ToString & " to " & dtflights.Rows(i2).Item("destination").ToString
                                    End If
                                    emailsubject &= _origAirportCode & " to " & _destAirportCode

                                    '20130507 - pab - make from email configurable
                                    'SendEmail(carrierid1, "noreply@personifly.com", "dhackett@coastalaviationsoftware.com; rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com", _
                                    '    emailsubject, emailbody, False, "", False)
                                    Dim emailaddrquote As String = String.Empty
                                    emailaddrquote = da.GetSetting(CInt(Session("carrierid")), "emailsentfromQuote")
                                    '20130930 - pab - change email from
                                    'If emailaddrquote = "" Then emailaddrquote = "noreply@personifly.com"
                                    If emailaddrquote = "" Then emailaddrquote = "CharterSales@coastalavtech.com"
                                    '20131024 - pab - fix duplicate emails
                                    'SendEmail(carrierid1, emailaddrquote, "dfhairmail@gmail.com; rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com", _
                                    '    emailsubject, emailbody, False, "", False)
                                    '20131206 - pab - make quote email configurable
                                    Dim sendto As String = da.GetSetting(carrierid1, "emailsenttoQuote")
                                    If sendto = "" Then sendto = emailsenttoQuote
                                    'SendEmail(emailaddrquote, sendto, "rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com; personifyquoteuat@gmail.com",
                                    '    emailsubject, emailbody, carrierid1, True)
                                    '20170428 - pab - fix email format error 
                                    If carrierid1 = 65 Then
                                        'tmc - html
                                        SendEmailLogo(sendto, "rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com; personifyquoteuat@gmail.com",
                                        "", emailaddrquote, emailsubject, emailbody, Nothing, True, "", carrierid1, "", "", False, "")
                                    Else
                                        'other carriers - not html
                                        SendEmailLogo(sendto, "rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com; personifyquoteuat@gmail.com",
                                        "", emailaddrquote, emailsubject, emailbody, Nothing, False, "", carrierid1, "", "", False, "")
                                    End If
                                End If
                                '20111208 - pab - email all quotes per Richard
                                'bEmailSent = True
                                ''Exit For
                            End If
                        Next

                        If InStr(dtflights.Rows(0).Item("Price").ToString, "$") > 0 Then
                            lblMsg.Visible = True
                            If lblMsg.Text <> "Please select a flight" Then
                                '20120329 - pab - move this text to right side between maps
                                'lblmsg.Text = "Airports closest to your address are selected, as shown in red.  Click on blue airports to select alternatives and possibly lower costs."
                                lblMsg.Visible = False
                                'lblMsg1.Visible = True
                                'lblMsg1.Text = "Airports closest to your address are selected, as shown in red.  Click on blue airports to select alternatives and possibly lower costs."
                            End If

                            '20120316 - pab - remove label - now in grid header
                            'lblTaxes.Visible = True

                            '20130429 - pab - move buttons per David - change to telerik buttons
                            'cmdConfirm.Enabled = True
                            'cmdConfirm1.Enabled = True

                            '20120130 - pab - add start over button at top for when no flights returned
                            'Me.cmdStartOver2.Visible = False
                            'Me.cmdStartOver.Visible = False
                            '20130429 - pab - move buttons per David - change to telerik buttons
                            'Me.cmdStartOver.Visible = True
                            'Me.cmdStartOver1.Visible = True
                        End If
                    End If

                    Session("flights") = dtflights

                    '20120316 - pab - remove label - now in grid header
                    'lblTaxes.Visible = True

                Else

                    '20111229 - pab
                    AirTaxi.post_timing("Quote no flights  " & Now.ToString)

                    lblMsg.Visible = True
                    '20121105 - pab - use setting
                    'lblmsg.Text = "This Aircraft Not Available - Try another time or aircraft"
                    lblMsg.Text = da.GetSetting(carrierid, "NotAvailable")
                    gvServiceProviderMatrix.EmptyDataText = "<br />" & lblMsg.Text & "<br />"
                    '20130924 - pab - fix error message showing up twice on page
                    lblMsg.Text = ""
                    '20130429 - pab - move buttons per David - change to telerik buttons
                    'cmdConfirm.Enabled = False
                    'cmdConfirm1.Enabled = False
                    '20110531 - pab - fix confirm button displaying if no flight available
                    dtflights = Nothing
                    dtflights2 = Nothing
                    Session("flights") = dtflights
                    Session("flights2") = dtflights2
                    Bind_gvServiceProviderMatrixReturn()
                    '20110906 - pab - don't display contine button if no flights
                    '20130429 - pab - move buttons per David - change to telerik buttons
                    'Me.cmdConfirm.Visible = False
                    'Me.cmdConfirm1.Visible = False

                    '20120130 - pab - add start over button at top for when no flights returned
                    'Me.cmdStartOver2.Visible = True
                    Me.cmdStartOver.Visible = True
                    '20130429 - pab - move buttons per David - change to telerik buttons
                    'Me.cmdStartOver.Visible = False
                    'Me.cmdStartOver1.Visible = False
                End If

                '20111216 - pab - use radio buttons for select per David
                'Me.gvServiceProviderMatrix.DataSource = dtflights
                'Me.gvServiceProviderMatrix.DataBind()
                Bind_gvServiceProviderMatrix()

                '20110906 - pab - don't display contine button if no flights
                '20130429 - pab - move buttons per David - change to telerik buttons
                'Me.cmdConfirm.Visible = True
                'Me.cmdConfirm1.Visible = True

            Catch ex As Exception
                Dim s As String = ex.Message
                If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
                AirTaxi.Insertsys_log(carrierid, appName, s, "quote", "QuoteTrip.aspx.vb")
                SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "", appName & " QuoteTrip.aspx.vb quote error", s, CInt(Session("carrierid")))
            End Try

        Else
            '20150831 - pab - add more logging to find out why sometimes no quotes generated
            AirTaxi.Insertsys_log(carrierid, appName, "Session(""triptype"") not R, O or L - value " & Session("triptype").ToString, "quote", "QuoteTrip.aspx.vb")
        End If

        '20111229 - pab
        AirTaxi.post_timing("Quote End  " & Now.ToString)
        AirTaxi.Insertsys_log(0, appName, Left(Now & "quote end  ", 500), " recordquotexmlEnd", "")

    End Sub

    '20160711 - pab - populate quote fields on bottom
    Sub hydratesearchfields()

        Dim sDeparts As String = ""
        Dim sTime As String = ""

        Try
            'OriginAddress2.Visible = False
            'OriginAddress3.Visible = False
            'OriginAddress4.Visible = False
            'OriginAddress5.Visible = False
            'OriginAddress6.Visible = False
            'OriginAddress7.Visible = False
            'OriginAddress8.Visible = False
            'OriginAddress9.Visible = False
            'OriginAddress10.Visible = False

            If Not IsNothing(Session("triptype")) Then
                Select Case Session("triptype")
                    Case "O"
                        rblOneWayRoundTrip.SelectedIndex = 0

                    Case "R"
                        rblOneWayRoundTrip.SelectedIndex = 1

                    Case "M"
                        rblOneWayRoundTrip.SelectedIndex = 2

                    Case Else
                        rblOneWayRoundTrip.SelectedIndex = 0

                End Select
            End If

            pnlLeg2.Visible = False
            pnlLeg3.Visible = False
            pnlLeg4.Visible = False
            pnlLeg5.Visible = False
            pnlLeg6.Visible = False
            pnlLeg7.Visible = False
            pnlLeg8.Visible = False
            pnlLeg9.Visible = False
            pnlLeg10.Visible = False

            If dtflights.Rows.Count > 0 Then
                Dim dr As DataRow

                For i As Integer = 0 To dtflights.Rows.Count - 1
                    dr = dtflights.Rows(i)

                    'If i > 0 Then
                    '    bttnAddLeg_Click(Nothing, Nothing)
                    'End If

                    sDeparts = dr("Departs")
                    Select Case i
                        Case 0
                            OriginAddress.Text = dr("Origin")
                            DestinationAddress.Text = dr("Destination")
                            depart_date.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo.SelectedValue = sTime
                            ddlPassengers.Text = Session("passengers")
                        Case 1
                            OriginAddress2.Text = dr("Origin")
                            DestinationAddress2.Text = dr("Destination")
                            depart_date2.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo2.SelectedValue = sTime
                            pnlLeg2.Visible = True
                        Case 2
                            OriginAddress3.Text = dr("Origin")
                            DestinationAddress3.Text = dr("Destination")
                            depart_date3.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo3.SelectedValue = sTime
                            pnlLeg3.Visible = True
                        Case 3
                            OriginAddress4.Text = dr("Origin")
                            DestinationAddress4.Text = dr("Destination")
                            depart_date4.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo4.SelectedValue = sTime
                            pnlLeg4.Visible = True
                        Case 4
                            OriginAddress5.Text = dr("Origin")
                            DestinationAddress5.Text = dr("Destination")
                            depart_date5.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo5.SelectedValue = sTime
                            pnlLeg5.Visible = True
                        Case 5
                            OriginAddress6.Text = dr("Origin")
                            DestinationAddress6.Text = dr("Destination")
                            depart_date6.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo6.SelectedValue = sTime
                            pnlLeg6.Visible = True
                        Case 6
                            OriginAddress7.Text = dr("Origin")
                            DestinationAddress7.Text = dr("Destination")
                            depart_date7.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo7.SelectedValue = sTime
                            pnlLeg7.Visible = True
                        Case 7
                            OriginAddress8.Text = dr("Origin")
                            DestinationAddress8.Text = dr("Destination")
                            depart_date8.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo8.SelectedValue = sTime
                            pnlLeg8.Visible = True
                        Case 8
                            OriginAddress9.Text = dr("Origin")
                            DestinationAddress9.Text = dr("Destination")
                            depart_date9.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo9.SelectedValue = sTime
                            pnlLeg9.Visible = True
                        Case 9
                            OriginAddress10.Text = dr("Origin")
                            DestinationAddress10.Text = dr("Destination")
                            depart_date10.SelectedDate = CDate(sDeparts).ToShortDateString
                            sTime = CDate(sDeparts).ToShortTimeString
                            If Len(sTime) < 8 Then sTime = "0" & sTime
                            departtime_combo10.SelectedValue = sTime
                            pnlLeg10.Visible = True
                    End Select

                Next

                'Dim sDeparts2 As String
                'Dim n As Integer = InStr(sDeparts, "<td>")
                'Do While n > 0
                '    sDeparts2 = InBetween(1, sDeparts, "<td>", "</td>")
                '    If IsDate(sDeparts2) Then 
                '    End If
                '    n = InStr(n + 1, sDeparts, "")
                '    sDeparts = Mid(sDeparts, n)

                '    i += 1
                'Loop
            Else

            End If

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbCr & vbLf & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbCr & vbLf & ex.StackTrace.ToString

            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "hydratesearchfields", "QuoteTrip.aspx.vb")
        End Try


    End Sub

    '20101208 - pab - get lowest quote accross carriers
    'Protected Sub quote(ByVal carrierid As Integer)

    '    '20111229 - pab
    '    AirTaxi.post_timing("Quote Start  " & Now.ToString)
    '    AirTaxi.Insertsys_log(0, appName, Left(Now & "Quote Start  ", 500), "Sub quote(ByVal carrierid As Integer)", "")


    '    '20110926 - pab - update web services
    '    'Dim ws As New optimizelowestquote.Service
    '    '20120201 - pab - changes for azure
    '    'Dim ws As New AviationService1_9.Service
    '    '20120423 - pab - call azure web service
    '    'Dim ws As New AviationService1_10.Service
    '    Dim ws As New AviationWebService1_10.WebService1

    '    Dim dsflights, dsquotes As New DataSet
    '    Dim dr As DataRow
    '    Dim weightclass As String = ""
    '    Dim da As New DataAccess
    '    Dim dt As DataTable

    '    'clear price rows from dtflights table before calling ws
    '    '20111220 - pab - multiple quotes
    '    Dim i As Integer = dtflights.Rows.Count - 1
    '    'Do While i >= 0
    '    '    dr = dtflights.Rows(i)
    '    '    If dr("Origin") = "" Then
    '    '        dtflights.Rows(i).Delete()
    '    '    Else
    '    '        dtflights.Rows(i).Item("service provider") = ""
    '    '        dtflights.Rows(i).Item("Arrives") = ""
    '    '        dtflights.Rows(i).Item("Flight Duration") = ""
    '    '        dtflights.Rows(i).Item("Price") = ""
    '    '    End If
    '    '    i = i - 1
    '    'Loop

    '    '20101214 - pab - fix error when dtflights has no rows
    '    If dtflights.Rows.Count = 0 Then
    '        '20140220 - pab - cleanup code
    '        'Me.cmdQuote.Visible = False
    '        'Me.LblItin.Visible = False
    '        Me.lblMsg.Text = ""
    '        If Session("triptype") <> "M" And Session("triptype") <> "G" Then
    '            lblMsg.Visible = True

    '            '20131016 - pab - fix session timeout
    '            If IsNothing(Session("flights")) And IsNothing(Session("triptype")) Then
    '                lblMsg.Text = da.GetSetting(_carrierid, "TimeoutMessage")
    '                gvServiceProviderMatrix.EmptyDataText = lblMsg.Text
    '            Else
    '                'lblMsg.Text = "No flights available. Please make a new trip request or change aircraft type."
    '            End If

    '            '20110906 - pab - don't display contine button if no flights
    '            '20130429 - pab - move buttons per David - change to telerik buttons
    '            'Me.cmdConfirm.Visible = False
    '            'Me.cmdConfirm1.Visible = False

    '            '20120316 - pab - remove label - now in grid header
    '            'lblTaxes.Visible = False

    '            '20111220 - pab - add return flight grid
    '            '20140220 - pab - cleanup code
    '            'Me.lblItinReturn.Visible = False
    '            'Me.gvServiceProviderMatrixReturn.Visible = False

    '            '20120125 - pab -remove widget - go to index
    '            '20120130 - pab - add start over button at top for when no flights returned
    '            'Me.cmdStartOver2.Visible = True
    '            Me.cmdStartOver.Visible = True
    '            '20130429 - pab - move buttons per David - change to telerik buttons
    '            'Me.cmdStartOver.Visible = False
    '            'Me.cmdStartOver1.Visible = False

    '        End If
    '        '20150831 - pab - add more logging to find out why sometimes no quotes generated
    '        AirTaxi.Insertsys_log(_carrierid, appName, "dtflights.Rows.Count = 0 - exit sub", "QuoteTrip.aspx.vb quote", "")
    '        Exit Sub
    '    End If

    '    Dim sdtflights As String = dtflightstoxml(dtflights)
    '    dsflights.Tables.Add(dtflights.Copy())

    '    weightclass = "L"
    '    '20140221 - pab - use telerik controls
    '    'If IsNumeric(ddlAircraftServiceTypes.SelectedValue) Then
    '    'dt = da.GetAircraftTypeServiceSpecsByID(carrierid, ddlAircraftServiceTypes.SelectedValue)
    '    'If IsNumeric(ddlAircraftServiceTypes1.SelectedValue) Then
    '    '    dt = da.GetAircraftTypeServiceSpecsByID(carrierid, ddlAircraftServiceTypes1.SelectedValue)
    '    '    If Not dt Is Nothing Then
    '    '        If dt.Rows.Count > 0 Then
    '    '            weightclass = dt.Rows(0).Item("weightclass").ToString
    '    '        End If
    '    '    End If
    '    'End If

    '    '20120123 - pab - fix passengers being reset
    '    'If _passengers = 0 Then _passengers = 1
    '    If _passengers = 0 Then
    '        If IsNothing(Session("passengers")) Then
    '            _passengers = 1
    '        ElseIf IsNumeric(Session("passengers")) Then
    '            _passengers = CInt(Session("passengers"))
    '        Else
    '            _passengers = 1
    '        End If
    '        'Else
    '        '    _passengers = 1
    '    End If
    '    If Session("usertype") Is Nothing Then Session("usertype") = "M"
    '    If Session("plantype") Is Nothing Then Session("plantype") = ""

    '    '20120316 - pab - remove label - now in grid header
    '    'lblTaxes.Visible = False

    '    '20111220 - pab - add return flight grid
    '    '20140220 - pab - cleanup code
    '    'Me.lblItinReturn.Visible = False
    '    'Me.gvServiceProviderMatrixReturn.Visible = False

    '    '20131206 - pab - make quote email configurable
    '    Dim emailsenttoQuote As String = da.GetSetting(_carrierid, "emailsenttoQuote")
    '    If emailsenttoQuote = "" Then emailsenttoQuote = "personifyquote@gmail.com"

    '    '20140111 - pab - fix error - Object reference not set to an instance of an object
    '    If IsNothing(Session("achourly")) Then Session("achourly") = ""

    '    '20140221 - pab - use telerik controls
    '    'Dim collection As IList(Of Telerik.Web.UI.RadComboBoxItem) = RadComboBoxRequests.CheckedItems
    '    Dim bAllowPets As Boolean = False
    '    Dim bAllowSmoking As Boolean = False
    '    Dim bWiFi As Boolean = False
    '    Dim bLav As Boolean = False
    '    Dim bPower As Boolean = False
    '    Dim bInFlightEntertainment As Boolean = False
    '    'For Each item As Telerik.Web.UI.RadComboBoxItem In collection
    '    '    Select Case item.Text
    '    '        Case "Pets"
    '    '            If item.Checked = True Then bAllowPets = True

    '    '        Case "Smoking"
    '    '            If item.Checked = True Then bAllowSmoking = True

    '    '        Case "WiFi"
    '    '            If item.Checked = True Then bWiFi = True

    '    '        Case "Enclosed Lav"
    '    '            If item.Checked = True Then bLav = True

    '    '        Case "Power"
    '    '            If item.Checked = True Then bPower = True

    '    '        Case "InFlight Entertainment"
    '    '            If item.Checked = True Then bInFlightEntertainment = True

    '    '    End Select
    '    'Next

    '    '20111221 - pab - get lowest quote for all weight classes
    '    '20140414 - pab - add acg indicator for logging and tracking
    '    'If Session("triptype") = "R" Or Session("triptype") = "O" Or Session("triptype") = "L" Then
    '    If Session("triptype") = "R" Or Session("triptype") = "O" Or Session("triptype") = "L" Or Session("triptype") = "M" Then
    '        Try
    '            dtquotes.Clear()

    '            '20140414 - pab - add acg indicator for logging and tracking
    '            Dim s As String = "Personifly Quote requested - "
    '            Select Case Session("triptype").ToString
    '                Case "R"
    '                    s &= "Round trip "
    '                Case "O"
    '                    s &= "One way "
    '                    '20160117 - pab - quote multi-leg trips
    '                Case "L", "M"
    '                    s &= "Multi-leg "
    '            End Select
    '            For i2 As Integer = 0 To dsflights.Tables(0).Rows.Count - 1
    '                s &= dsflights.Tables(0).Rows(i2).Item("Origin").ToString.Trim & " to " & dsflights.Tables(0).Rows(i2).Item("Destination").ToString.Trim & _
    '                    " departing " & dsflights.Tables(0).Rows(i2).Item("Departs").ToString.Trim & "; "
    '            Next
    '            'do not insert if debugging or Paula's IP
    '            If InStr(ip, "127.0.0.1") = 0 And InStr(ip, "173.162.122.246") = 0 Then AirTaxi.Insertsys_log(_carrierid, appName, Left(s, 500), "quote", ip)

    '            'Dim sdtflights As String = dtflightstoxml(dtflights)

    '            'chkTesting.Checked = True

    '            '20120104 - pab - pass name and email for delta
    '            Dim firstname As String = "Personifly"
    '            Dim lastname As String = "CAS"
    '            Dim email As String = "Flights@CoastalAviationSoftware.com"

    '            '20150821 - pab - force login before getting quotes to track users
    '            Dim MemberID As Integer = 0
    '            Dim MemberIDCarrier As Integer = 0

    '            '20140423 - pab - quote empty legs
    '            If Not IsNothing(Session("FirstName")) Then
    '                If Session("FirstName").ToString.Trim <> "" Then firstname = Session("FirstName").ToString.Trim
    '            End If
    '            If Not IsNothing(Session("LastName")) Then
    '                If Session("LastName").ToString.Trim <> "" Then lastname = Session("LastName").ToString.Trim
    '            End If
    '            If Not IsNothing(Session("email")) Then
    '                'If Session("email").ToString.Trim <> "" Then email = Session("email").ToString.Trim
    '            End If

    '            '20150821 - pab - force login before getting quotes to track users
    '            If Not IsNothing(Session("MemberID")) Then
    '                If IsNumeric(Session("MemberID").ToString.Trim) Then MemberID = CInt(Session("MemberID").ToString.Trim)
    '            End If
    '            If Not IsNothing(Session("MemberIDCarrier")) Then
    '                If IsNumeric(Session("MemberIDCarrier").ToString.Trim) Then MemberIDCarrier = CInt(Session("MemberIDCarrier").ToString.Trim)
    '            End If

    '            '20111230 - pab - get only one quote if from external link
    '            '20120111 - pab - or weightclass selected
    '            'If Session("dqn").ToString <> "" And Session("weightclass").ToString <> "" Then
    '            '20120208 - pab - return multiple quotes if weight class selected
    '            'If Session("dqn").ToString <> "" Or Session("weightclass").ToString <> "" Then
    '            If Session("dqn").ToString <> "" And Session("weightclass").ToString <> "" Then

    '                '20111229 - pab
    '                AirTaxi.post_timing("Quote " & Session("weightclass").ToString & "  " & Now.ToString)

    '                AirTaxi.Insertsys_log(0, appName, Left(Now & "GetLowestQuoteByWeightClass  ", 500), " GetLowestQuoteByWeightClass 4158 Start", "")

    '                '20140221 - pab - use telerik controls
    '                '20150821 - pab - force login before getting quotes to track users
    '                GetLowestQuoteByWeightClass(Session("weightclass").ToString, _passengers, Session("triptype"), Session("usertype"), Session("plantype"), _
    '                    dsflights, False, ip, firstname, lastname, email, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, bInFlightEntertainment, MemberID, _
    '                    MemberIDCarrier)

    '                AirTaxi.Insertsys_log(0, appName, Left(Now & "GetLowestQuoteByWeightClass  ", 500), " GetLowestQuoteByWeightClass 4158 End", "")

    '                '20121204 - pab - implement aircraft to include
    '            ElseIf Session("achourly").ToString <> "" Then
    '                AirTaxi.post_timing("Quote " & Session("achourly").ToString & "  " & Now.ToString)

    '                AirTaxi.Insertsys_log(0, appName, Left(Now & " GetLowestQuoteByWeightClassPartial  ", 500), " GetLowestQuoteByWeightClassPartial 4173 Start", "")

    '                '20131118 - pab - add more fields to aircraft
    '                '20140221 - pab - use telerik controls
    '                '20150821 - pab - force login before getting quotes to track users
    '                GetLowestQuoteByWeightClassPartial("A", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, _
    '                    firstname, lastname, email, Session("achourly").ToString, bInFlightEntertainment, ManufactureDate, bAllowPets, bAllowSmoking, bWiFi, _
    '                    bLav, bPower, bInFlightEntertainment, MemberID, MemberIDCarrier)
    '                AirTaxi.Insertsys_log(0, appName, Left(Now & " GetLowestQuoteByWeightClassPartial  ", 500), " GetLowestQuoteByWeightClassPartial 4173 End", "")


    '                '20120208 - pab - return multiple quotes if weight class selected
    '            ElseIf Session("weightclass").ToString <> "" Then

    '                '20111229 - pab
    '                AirTaxi.post_timing("Quote " & Session("weightclass").ToString & "  " & Now.ToString)

    '                AirTaxi.Insertsys_log(0, appName, Left(Now & " GetQuotesByWeightClassAllCarriers  ", 500), " GetLowestQuoteByWeightClass 4191 Start", "")

    '                '20140221 - pab - use telerik controls
    '                '20150821 - pab - force login before getting quotes to track users
    '                GetQuotesByWeightClassAllCarriers(Session("weightclass").ToString, _passengers, Session("triptype"), Session("usertype"), Session("plantype"), _
    '                    dsflights, False, ip, firstname, lastname, email, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, bInFlightEntertainment, MemberID, _
    '                    MemberIDCarrier)
    '                AirTaxi.Insertsys_log(0, appName, Left(Now & " GetQuotesByWeightClassAllCarriers  ", 500), " GetLowestQuoteByWeightClass 4191 End", "")


    '            Else



    '                '20120206 - pab - and configurable setting to ShowMultipleQuotes y/n
    '                Dim ShowMultipleQuotes As Boolean = False
    '                If da.GetSetting(_carrierid, "ShowMultipleQuotes") = "Y" Then ShowMultipleQuotes = True

    '                '20120206 - pab - and configurable setting to ShowMultipleQuotes y/n
    '                ''20120111 - pab - run time improvements - quote medium first, only quote light or heavy if no quote previously returned
    '                'AirTaxi.post_timing("Quote Medium  " & Now.ToString)
    '                ''Medium
    '                'GetLowestQuoteByWeightClass("M", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '                'rk 6/13/2012
    '                'AirTaxi.Insertsys_log(0, "selectapt4023", Left(Now, 500), "start lowest quote A ", "")
    '                AirTaxi.Insertsys_log(0, appName, Left(Now & " GetQuotesByWeightClass A  ", 500), " GetQuotesByWeightClass 4216 Start", "")

    '                'rk 6.13.2012 call only once for all types
    '                If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote All " & Now.ToString)
    '                '20140221 - pab - use telerik controls
    '                '20150821 - pab - force login before getting quotes to track users
    '                If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("A", _passengers, Session("triptype"), _
    '                    Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email, bAllowPets, bAllowSmoking, bWiFi, _
    '                    bLav, bPower, bInFlightEntertainment, MemberID, MemberIDCarrier)
    '                If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote All Complete " & Now.ToString)
    '                AirTaxi.Insertsys_log(0, appName, Left(Now & " GetQuotesByWeightClass A  ", 500), " GetQuotseByWeightClass 4216 End", "")


    '                ''Light
    '                ''20120206 - pab - and configurable setting to ShowMultipleQuotes y/n
    '                ''If dtquotes.Rows.Count = 0 Then AirTaxi.post_timing("Quote Light  " & Now.ToString)
    '                ''If dtquotes.Rows.Count = 0 Then GetLowestQuoteByWeightClass("L", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Light  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("L", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '                ''Medium
    '                ''AirTaxi.post_timing("Quote Medium  " & Now.ToString)
    '                ''GetLowestQuoteByWeightClass("M", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Medium  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("M", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '                ''20120507 - pab - add new weight classes - Twin Piston, Single Turboprop, Twin Turboprop, SuperMid Jet
    '                ''SuperMid
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote SuperMid Jet  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("U", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '                ''Heavy
    '                ''If dtquotes.Rows.Count = 0 Then AirTaxi.post_timing("Quote Heavy  " & Now.ToString)
    '                ''If dtquotes.Rows.Count = 0 Then GetLowestQuoteByWeightClass("H", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Heavy  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("H", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '                ''VLJ
    '                ''If dtquotes.Rows.Count = 0 Then AirTaxi.post_timing("Quote VLJ  " & Now.ToString)
    '                ''If dtquotes.Rows.Count = 0 Then GetLowestQuoteByWeightClass("V", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote VLJ  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("V", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '                ''20111229 - pab
    '                ''Propeller
    '                ''If dtquotes.Rows.Count = 0 Then AirTaxi.post_timing("Quote Propeller  " & Now.ToString)
    '                ''If dtquotes.Rows.Count = 0 Then GetLowestQuoteByWeightClass("P", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Single Piston  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("P", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)


    '                ''20120507 - pab - add new weight classes - Twin Piston, Single Turboprop, Twin Turboprop, SuperMid Jet
    '                ''Twin Piston
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Twin Piston  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("T", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '                ''Single Turboprop
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Single Turboprop  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("1", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '                ''Twin Turboprop
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then AirTaxi.post_timing("Quote Twin Turboprop  " & Now.ToString)
    '                'If dtquotes.Rows.Count = 0 Or ShowMultipleQuotes = True Then GetLowestQuoteByWeightClass("2", _passengers, Session("triptype"), Session("usertype"), Session("plantype"), dsflights, False, ip, firstname, lastname, email)

    '            End If

    '            If dtquotes.Rows.Count > 0 Then

    '                dtflights.Clear()

    '                '20111221 - pab multiple quotes
    '                dtflights2.Clear()

    '                dtflights = dtquotes.Copy

    '                '20111229 - pab
    '                AirTaxi.post_timing("Quote Email and Tweet  " & Now.ToString)

    '                '20120125 - pab - save dqn2 for purchase
    '                Dim dqn2 As String = InBetween(1, dtflights.Rows(0).Item("PriceExplanationDetail"), "; dqn ", ";")
    '                Session("dqn2") = dqn2
    '                Session("quotenumber") = 0

    '                '20120103 - pab - save click thru info if from airboard
    '                If Session("dqn").ToString <> "" Then
    '                    AirTaxi.post_timing("Start dqn " & Now.ToString)

    '                    Dim dt1 As DataTable
    '                    Dim carrierid1 As Integer = 0
    '                    dt1 = da.GetProviderByAlias(dtflights.Rows(0).Item("service provider"))
    '                    If dt1.Rows.Count > 0 Then
    '                        carrierid1 = dt1.Rows(0).Item("carrierid")
    '                    End If

    '                    dtflights.TableName = "dtflights"

    '                    '20120125 - pab - save dqn2 for purchase
    '                    'Dim dqn2 As String = InBetween(1, dtflights.Rows(0).Item("PriceExplanationDetail"), "; dqn ", ";")
    '                    AirTaxi.Insertsys_log(0, appName, Left(Now & "recordquotexml  ", 500), " recordquotexmlStart", "")

    '                    Dim qn As Integer = recordquotexml(dtflights, "", "", 0, "", 9, "", "", "", "", "", "")
    '                    Session("quotenumber") = qn
    '                    'Session("dqn2") = dqn2
    '                    AirTaxi.Insertsys_log(0, appName, Left(Now & "recordquotexml  ", 500), " recordquotexmlEnd", "")

    '                    da.InsertAirBoardClickThru(carrierid1, Session("dqn").ToString, dqn2, qn, Now, 0, CDbl(Session("airboardprice").ToString))
    '                    AirTaxi.post_timing("Stop dqn " & Now.ToString)

    '                End If



    '                AirTaxi.post_timing("start emails " & Now.ToString)

    '                '20131118 - pab - add more fields to aircraft
    '                Dim bownerconfirm As Boolean = False

    '                '20140620 - pab - quote from admin portal
    '                Dim bemptyleg As Boolean = False

    '                '20141106 - pab - rewrite quote routine
    '                'Dim sortExp As String = "Price,WeightClass,quoteID"
    '                Dim sortExp As String = "WeightClass,quoteID"
    '                Dim drarray() As DataRow
    '                drarray = dtflights.Select(Nothing, sortExp, DataViewRowState.CurrentRows)

    '                '20111208 - pab - email all quotes per Richard
    '                '20141106 - pab - rewrite quote routine
    '                'For i = 0 To dtflights.Rows.Count - 1
    '                For i = 0 To drarray.Count - 1

    '                    '20141106 - pab - rewrite quote routine
    '                    dr = drarray(i)

    '                    '20120530 - pab - fix error - Conversion from type 'DBNull' to type 'String' is not valid.
    '                    'Dim scerts As String = dtflights.Rows(i).Item("certifications")
    '                    Dim scerts As String = ""
    '                    If Not IsDBNull(dr.Item("certifications")) Then
    '                        scerts = dr.Item("certifications").ToString
    '                    End If
    '                    '20120530 - pab - for some reason certifications coming back null
    '                    If scerts = "" Then
    '                        Dim dt1 As DataTable = da.GetProviderByAlias(dr.Item("service provider"))
    '                        If Not isdtnullorempty(dt1) Then
    '                            scerts = "Ratings: "
    '                            If Not IsDBNull(dt1.Rows(0)("ARGUSlevel")) Then
    '                                If dt1.Rows(0)("ARGUSlevel").ToString <> "" Then scerts &= dt1.Rows(0)("ARGUSlevel").ToString
    '                            End If
    '                            If Not IsDBNull(dt1.Rows(0)("WYVERNlevel")) Then
    '                                If dt1.Rows(0)("WYVERNlevel").ToString <> "" Then scerts &= vbNewLine & dt1.Rows(0)("WYVERNlevel").ToString
    '                            End If
    '                            If Not IsDBNull(dt1.Rows(0)("Sentientlevel")) Then
    '                                If dt1.Rows(0)("Sentientlevel").ToString <> "" Then scerts &= vbNewLine & dt1.Rows(0)("Sentientlevel").ToString
    '                            End If
    '                            scerts &= vbNewLine & vbNewLine
    '                        End If
    '                        scerts &= "Certifications: " & da.GetSetting(carrierid, "Certification")
    '                        dr.Item("certifications") = scerts
    '                    End If

    '                    '20120503 = pab - create aircraft type link if blank
    '                    Dim carrierid1 As Integer = 0
    '                    If dr.Item("FAQPageURL").ToString = "" Then
    '                        Dim dt1 As DataTable
    '                        dt1 = da.GetProviderByAlias(dr.Item("service provider"))
    '                        If dt1.Rows.Count > 0 Then
    '                            carrierid1 = dt1.Rows(0).Item("carrierid")
    '                        Else
    '                            dt1 = da.GetProviderByCompanyName(dr.Item("service provider"))
    '                            If dt1.Rows.Count > 0 Then
    '                                carrierid1 = dt1.Rows(0).Item("carrierid")
    '                            End If
    '                        End If
    '                        If IsNumeric(dr.Item("AircraftType").ToString) Then
    '                            dr.Item("FAQPageURL") = "AircraftTypeDetails.aspx?carrierid=" & carrierid1 & "&atssid=" & CInt(dr.Item("AircraftType").ToString)
    '                        Else
    '                            dr.Item("FAQPageURL") = "AircraftTypeDetails.aspx?carrierid=" & carrierid1 & "&atssid=" & 0
    '                        End If

    '                        '20150127 - pab - upload and display pdfs
    '                    Else
    '                        'check if pdf from blob
    '                        If InStr(dr.Item("FAQPageURL").ToString.ToLower, ".pdf") > 0 And InStr(dr.Item("FAQPageURL").ToString, "/") = 0 Then
    '                            'not a full url so must be blob pdf
    '                            dr.Item("FAQPageURL") = "AircraftTypePDF.aspx?acpdf=" & dr.Item("FAQPageURL").ToString
    '                        End If

    '                    End If

    '                    '20131118 - pab - add more fields to aircraft
    '                    If dr.Item("OwnerConfirmation").ToString <> "" Then
    '                        '20141209 - pab - fix footnotes
    '                        s = dr.Item("OwnerConfirmation").ToString
    '                        Do While InStr(s, "<strong>") > 0
    '                            If InBetween(1, s, "<strong>", "</strong>") <> "" Then
    '                                bownerconfirm = True
    '                                Exit Do
    '                            End If
    '                            s = Mid(s, InStr(s, "</strong>") + 1)
    '                        Loop
    '                    End If

    '                    '20140620 - pab - quote from admin portal
    '                    If dr.Item("EmptyLegPricing").ToString <> "" Then
    '                        '20141209 - pab - fix footnotes
    '                        s = dr.Item("EmptyLegPricing").ToString
    '                        Do While InStr(s, "<td>") > 0
    '                            If InBetween(1, s, "<td>", "</td>") <> "" Then
    '                                bemptyleg = True
    '                                Exit Do
    '                            End If
    '                            s = Mid(s, InStr(s, "</td>") + 1)
    '                        Loop
    '                    End If

    '                    '20111208 - pab - tweet all quotes per Richard
    '                    '20120111 - pab make tweeting configurable
    '                    'If da.GetSetting(_carrierid, "SendTweet") = "Y" Then
    '                    '    AirTaxi.Insertsys_log(0, appName, Left(Now & "tweetstart  ", 500), " recordquotexmlEnd", "")


    '                    '    If IsNumeric(InBetween(1, dtflights.Rows(i).Item("price"), "<strong>", "</strong>")) And Not chkTesting.Checked Then
    '                    '        Dim owrt As String = String.Empty
    '                    '        If Session("triptype") = "R" Then owrt = "Round Trip"
    '                    '        If Session("triptype") = "O" Then owrt = "One Way"

    '                    '        Dim sTweet As String = String.Empty

    '                    '        Try
    '                    '            'twitter
    '                    '            If owrt <> "" Then
    '                    '                '20111208 - pab - calculate price per seat
    '                    '                Dim priceperseat As Double = CDbl(InBetween(1, dtflights.Rows(i).Item("price"), "<strong>", "</strong>"))
    '                    '                Dim dtcarrier As DataTable = da.GetSettingCarrierIDBySetting(dtflights.Rows(i).Item("Service Provider").ToString, "CompanyName")
    '                    '                Dim dtat As DataTable

    '                    '                If Not IsNothing(dtcarrier) Then
    '                    '                    If dtcarrier.Rows.Count > 0 Then
    '                    '                        dtat = da.GetAircraftTypeServiceSpecsByID(CInt(dtcarrier.Rows(0).Item("carrierid").ToString), CInt(dtflights.Rows(i).Item("AircraftType").ToString))
    '                    '                        If Not IsNothing(dtat) Then
    '                    '                            If dtat.Rows.Count > 0 Then
    '                    '                                If IsNumeric(dtat.Rows(0).Item("totalpassengers").ToString) Then priceperseat = priceperseat / CInt(dtat.Rows(0).Item("totalpassengers").ToString)
    '                    '                            End If
    '                    '                        End If
    '                    '                    End If
    '                    '                End If
    '                    '                Dim stattweet As String = ("Fly " & owrt & " from " & fname(Session("origairportcode")) & " To " & fname(Session("destairportcode")) & " for " & Format(priceperseat, "$###,##0.00"))
    '                    '                If Len(stattweet) > 144 Then stattweet = Left(stattweet, 144)

    '                    '                sTweet = wscas.Tweet("pbaumgart@ctgi.com", "123", _carrierid, stattweet)
    '                    '            End If
    '                    '        Catch
    '                    '            'oh well tweet off
    '                    '        End Try
    '                    '    End If
    '                    '    AirTaxi.Insertsys_log(0, appName, Left(Now & "tweetstart  ", 500), " tweet end", "")

    '                    'End If
    '                Next

    '                '20131118 - pab - add more fields to aircraft
    '                'lblOwnerConfirm.Visible = False
    '                'lblOwnerConfirm.Text = ""
    '                If bownerconfirm = True Then
    '                    '20140227 - pab - emphasize owner confirmation 
    '                    'If InStr(lblFlightTimeMsg.Text, "<br />+") = 0 Then lblFlightTimeMsg.Text &= "<br />+" & da.GetSetting(_carrierid, "operator/owner confirmation")
    '                    '20140528 - pab - use diamond instead of plus sign
    '                    'If InStr(lblOwnerConfirm.Text, "+") = 0 Then
    '                    '   lblOwnerConfirm.Text = "+" & da.GetSetting(_carrierid, "operator/owner confirmation")
    '                    '20140620 - pab - quote from admin portal
    '                    'If InStr(lblOwnerConfirm.Text, "&diams;") = 0 Then
    '                    '    lblOwnerConfirm.Text = "&diams; " & da.GetSetting(_carrierid, "operator/owner confirmation")
    '                    '    lblOwnerConfirm.Visible = True
    '                    'End If
    '                    lblMsg.Text = da.GetSetting(_carrierid, "ownerconfirmationSpecialChar") & " " & da.GetSetting(_carrierid, "operator/owner confirmation")
    '                    lblMsg.Visible = True
    '                End If

    '                '20140620 - pab - quote from admin portal
    '                If bemptyleg = True Then
    '                    If lblMsg.Text <> "" Then lblMsg.Text &= "   "
    '                    lblMsg.Text &= da.GetSetting(_carrierid, "EmptyLegSpecialCharacter") & " " & da.GetSetting(_carrierid, "EmptyLegMessage")
    '                    lblMsg.Visible = True
    '                End If

    '                AirTaxi.post_timing("stop emails " & Now.ToString)

    '                '20111220 - pab - add return flight grid
    '                '20141110 - PAB - rewrite quote routine
    '                'If Session("triptype") = "R" Then

    '                '    '20111229 - pab
    '                '    AirTaxi.post_timing("Quote CreateReturnTable  " & Now.ToString)

    '                '    '20120215 - pab - combine return flights with outbound
    '                '    'CreateReturnTable(dtflights)
    '                '    CreateCombinedTable(dtflights)
    '                '    'Session("flights2") = dtflights2
    '                '    'Bind_gvServiceProviderMatrixReturn()
    '                '    'Me.lblItinReturn.Visible = True
    '                '    'Me.gvServiceProviderMatrixReturn.Visible = True
    '                'End If

    '                '20111230 - pab - fix message if flight available
    '                If dtflights.Rows.Count > 0 Then

    '                    'rk 6.15.2012 remove email as delay source
    '                    For i2 As Integer = 0 To dtflights.Rows.Count - 1
    '                        If dtflights.Rows(i2).Item("PriceExplanationDetail").ToString <> "" Then
    '                            'If Not chkTesting.Checked And dtflights.Rows(i2).Item("service provider").ToString.ToUpper <> "DELTA" Then
    '                            If dtflights.Rows(i2).Item("service provider").ToString.ToUpper <> "DELTA" Then
    '                                Dim emailbody As String = dtflights.Rows(i2).Item("PriceExplanationDetail").ToString
    '                                Dim carrierid1 As Integer = 0
    '                                Dim dt1 As DataTable = da.GetProviderByCompanyName(dtflights.Rows(i2).Item("service provider").ToString)
    '                                If Not isdtnullorempty(dt1) Then
    '                                    carrierid1 = dt1.Rows(0).Item("carrierid")
    '                                End If
    '                                '20121130 - pab - display from and to airports in email subject
    '                                '20141117 - pab - fix email headings after quoteworker rewrite
    '                                Dim emailsubject As String = "Personifly Quote for " & dtflights.Rows(i2).Item("service provider").ToString
    '                                If Session("triptype") = "R" Then
    '                                    emailsubject &= " Round Trip "
    '                                    'emailsubject &= Replace(dtflights.Rows(i2).Item("Origin").ToString, "<br />", " to ")

    '                                    '20160316 - pab - multi-leg
    '                                ElseIf Session("triptype") = "M" Then
    '                                    emailsubject &= " Multi-Leg "

    '                                Else
    '                                    emailsubject &= " One Way "
    '                                    'emailsubject &= dtflights.Rows(i2).Item("Origin").ToString & " to " & dtflights.Rows(i2).Item("destination").ToString
    '                                End If
    '                                emailsubject &= _origAirportCode & " to " & _destAirportCode

    '                                '20130507 - pab - make from email configurable
    '                                'SendEmail(carrierid1, "noreply@personifly.com", "dhackett@coastalaviationsoftware.com; rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com", _
    '                                '    emailsubject, emailbody, False, "", False)
    '                                Dim emailaddrquote As String = String.Empty
    '                                emailaddrquote = da.GetSetting(_carrierid, "emailsentfromQuote")
    '                                '20130930 - pab - change email from
    '                                'If emailaddrquote = "" Then emailaddrquote = "noreply@personifly.com"
    '                                If emailaddrquote = "" Then emailaddrquote = "CharterSales@coastalavtech.com"
    '                                '20131024 - pab - fix duplicate emails
    '                                'SendEmail(carrierid1, emailaddrquote, "dfhairmail@gmail.com; rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com", _
    '                                '    emailsubject, emailbody, False, "", False)
    '                                '20131206 - pab - make quote email configurable
    '                                Dim sendto As String = da.GetSysSettingByCarrier(carrierid1, "emailsenttoQuote", "")
    '                                If sendto = "" Then sendto = emailsenttoQuote
    '                                SendEmail(emailaddrquote, sendto, "rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com", _
    '                                    emailsubject, emailbody, carrierid1)
    '                            End If
    '                            '20111208 - pab - email all quotes per Richard
    '                            'bEmailSent = True
    '                            ''Exit For
    '                        End If
    '                    Next

    '                    If InStr(dtflights.Rows(0).Item("Price").ToString, "$") > 0 Then
    '                        lblMsg.Visible = True
    '                        If lblMsg.Text <> "Please select a flight" Then
    '                            '20120329 - pab - move this text to right side between maps
    '                            'lblmsg.Text = "Airports closest to your address are selected, as shown in red.  Click on blue airports to select alternatives and possibly lower costs."
    '                            lblMsg.Visible = False
    '                            'lblMsg1.Visible = True
    '                            'lblMsg1.Text = "Airports closest to your address are selected, as shown in red.  Click on blue airports to select alternatives and possibly lower costs."
    '                        End If

    '                        '20120316 - pab - remove label - now in grid header
    '                        'lblTaxes.Visible = True

    '                        '20130429 - pab - move buttons per David - change to telerik buttons
    '                        'cmdConfirm.Enabled = True
    '                        'cmdConfirm1.Enabled = True

    '                        '20120130 - pab - add start over button at top for when no flights returned
    '                        'Me.cmdStartOver2.Visible = False
    '                        Me.cmdStartOver.Visible = False
    '                        '20130429 - pab - move buttons per David - change to telerik buttons
    '                        'Me.cmdStartOver.Visible = True
    '                        'Me.cmdStartOver1.Visible = True
    '                    End If
    '                End If

    '                Session("flights") = dtflights

    '                '20120316 - pab - remove label - now in grid header
    '                'lblTaxes.Visible = True

    '            Else

    '                '20111229 - pab
    '                AirTaxi.post_timing("Quote no flights  " & Now.ToString)

    '                lblMsg.Visible = True
    '                '20121105 - pab - use setting
    '                'lblmsg.Text = "This Aircraft Not Available - Try another time or aircraft"
    '                lblMsg.Text = da.GetSetting(carrierid, "NotAvailable")
    '                gvServiceProviderMatrix.EmptyDataText = "<br />" & lblMsg.Text & "<br />"
    '                '20130924 - pab - fix error message showing up twice on page
    '                lblMsg.Text = ""
    '                '20130429 - pab - move buttons per David - change to telerik buttons
    '                'cmdConfirm.Enabled = False
    '                'cmdConfirm1.Enabled = False
    '                '20110531 - pab - fix confirm button displaying if no flight available
    '                dtflights = Nothing
    '                dtflights2 = Nothing
    '                Session("flights") = dtflights
    '                Session("flights2") = dtflights2
    '                Bind_gvServiceProviderMatrixReturn()
    '                '20110906 - pab - don't display contine button if no flights
    '                '20130429 - pab - move buttons per David - change to telerik buttons
    '                'Me.cmdConfirm.Visible = False
    '                'Me.cmdConfirm1.Visible = False

    '                '20120130 - pab - add start over button at top for when no flights returned
    '                'Me.cmdStartOver2.Visible = True
    '                Me.cmdStartOver.Visible = True
    '                '20130429 - pab - move buttons per David - change to telerik buttons
    '                'Me.cmdStartOver.Visible = False
    '                'Me.cmdStartOver1.Visible = False
    '            End If

    '            '20111216 - pab - use radio buttons for select per David
    '            'Me.gvServiceProviderMatrix.DataSource = dtflights
    '            'Me.gvServiceProviderMatrix.DataBind()
    '            Bind_gvServiceProviderMatrix()

    '            '20110906 - pab - don't display contine button if no flights
    '            '20130429 - pab - move buttons per David - change to telerik buttons
    '            'Me.cmdConfirm.Visible = True
    '            'Me.cmdConfirm1.Visible = True

    '        Catch ex As Exception
    '            Dim s As String = ex.Message
    '            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
    '            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
    '            AirTaxi.Insertsys_log(carrierid, appName, s, "quote", "QuoteTrip.aspx.vb")
    '            SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "", appName & " QuoteTrip.aspx.vb quote error", s, _carrierid)
    '        End Try

    '    Else
    '        '20150831 - pab - add more logging to find out why sometimes no quotes generated
    '        AirTaxi.Insertsys_log(carrierid, appName, "Session(""triptype"") not R, O or L - value " & Session("triptype").ToString, "quote", "QuoteTrip.aspx.vb")
    '    End If

    '    '20111229 - pab
    '    AirTaxi.post_timing("Quote End  " & Now.ToString)
    '    AirTaxi.Insertsys_log(0, appName, Left(Now & "quote end  ", 500), " recordquotexmlEnd", "")

    'End Sub

    '20121204 - pab - implement aircraft to include
    '20131118 - pab - add more fields to aircraft
    '20140221 - pab - use telerik controls
    '20150821 - pab - force login before getting quotes to track users
    Private Sub GetLowestQuoteByWeightClassPartial(ByVal weightclass As String, ByVal passengers As Integer, ByVal triptype As String, ByVal usertype As String,
                                            ByVal plantype As String, ByVal dsflights As DataSet, ByVal chkCoPilot As Boolean, ByVal ip As String,
                                            ByVal firstname As String, ByVal lastname As String, ByVal email As String, ByVal actoinclude As String,
                                            ByVal InflightEntertainment As Boolean, ByVal ManufactureDate As Date, ByRef bAllowPets As Boolean,
                                            ByRef bAllowSmoking As Boolean, ByRef bWiFi As Boolean, ByRef bLav As Boolean, ByRef bPower As Boolean,
                                            ByRef bInFlightEntertainment As Boolean, ByRef MemberID As Integer, ByRef MemberIDCarrier As Integer)

        Dim ws As New AviationWebService1_10.WebService1

        '20130722 - pab - fix web service call returning before quotes finished
        ws.Timeout = _sleep + 95000

        Dim dsquotes As New DataSet

        AirTaxi.post_timing("GetLowestQuoteByWeightClassPartial Start  " & Now.ToString)

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        Dim BrokerID As Integer = 0
        If Not IsNothing(Session("BrokerID")) Then
            If IsNumeric(Session("BrokerID")) Then BrokerID = CInt(Session("BrokerID"))
        End If

        Try
            Dim sdtflights As String = dtflightstoxml(dtflights)

            AirTaxi.post_timing("GetLowestQuoteByWeightClassPartial ws call  " & Now.ToString)

            '20150831 - pab - add more logging to find out why sometimes no quotes generated
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClassPartial ws call", "QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial", "")

            '20131118 - pab - add more fields to aircraft
            '20131223 - pab - add carrier specific quotes for admin portal
            '20140221 - pab - use telerik controls
            '20150821 - pab - force login before getting quotes to track users
            dsquotes = ws.GetLowestQuoteByClassPartial("pbaumgart@ctgi.com", "123", weightclass, passengers, triptype, usertype, plantype, dsflights,
               chkCoPilot, ip, firstname, lastname, email, False, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, BrokerID, actoinclude,
               InflightEntertainment, ManufactureDate, CInt(Session("carrierid")), MemberID, MemberIDCarrier)
            'dsquotes = ws.GetLowestQuoteByClassPartial("pbaumgart@ctgi.com", "123", weightclass, passengers, triptype, usertype, plantype, dsflights,
            '   chkCoPilot, ip, firstname, lastname, email, False, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, BrokerID, actoinclude,
            '   InflightEntertainment, ManufactureDate, 0, MemberID, MemberIDCarrier)

            If Not IsNothing(dsquotes) Then
                If dsquotes.Tables.Count > 0 Then
                    If dsquotes.Tables(0).Rows.Count > 0 Then
                        '20150831 - pab - add more logging to find out why sometimes no quotes generated
                        AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClassPartial ws call end - " & dsquotes.Tables(0).Rows.Count.ToString &
                            " rows returned", "QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial", "")
                        '20160117 - pab - quote multi-leg trips
                        If Session("triptype") = "M" Then
                            CreateFlightsTableMulti(dsquotes)
                        Else
                            CreateFlightsTable(dsquotes.Tables(0))
                        End If

                    Else
                        '20150831 - pab - add more logging to find out why sometimes no quotes generated
                        AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClassPartial ws call end - empty table returned in dataset",
                            "QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial", "")
                    End If

                Else
                    '20150831 - pab - add more logging to find out why sometimes no quotes generated
                    AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClassPartial ws call end - no tables returned in dataset",
                        "QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial", "")
                End If

            Else
                '20150831 - pab - add more logging to find out why sometimes no quotes generated
                AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClassPartial ws call end - nothing returned",
                    "QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial", "")
            End If

        Catch ex As Exception
            AirTaxi.post_timing("GetLowestQuoteByWeightClassPartial Failed" & Now.ToString)
            Dim s = ex.Message
            If Not IsNothing(ex.InnerException) Then
                s &= " - " & ex.InnerException.ToString
            End If
            If Not IsNothing(ex.StackTrace) Then
                s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            End If
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, Left(Now & " " & s, 500), "QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial", "")
            If ex.Message <> "The operation has timed out" Then
                '20130930 - pab - change email from
                'SendEmail(_carrierid, "info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial error", s, False, "", False)
                '20131024 - pab - fix duplicate emails
                'SendEmail(_carrierid, _emailfrom, "rkane@coastalaviationsoftware.com", appName & " QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial error", s, False, "", False)
                SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "", appName & " QuoteTrip.aspx.vb GetLowestQuoteByWeightClassPartial error", s, CInt(Session("carrierid")))
            End If

        End Try

        AirTaxi.post_timing("GetLowestQuoteByWeightClassPartial End  " & Now.ToString)

    End Sub

    '20111221 - pab - get lowest quote for all weight classes
    '20140221 - pab - use telerik controls
    '20150821 - pab - force login before getting quotes to track users
    Private Sub GetLowestQuoteByWeightClass(ByVal weightclass As String, ByVal passengers As Integer, ByVal triptype As String, ByVal usertype As String,
                                            ByVal plantype As String, ByVal dsflights As DataSet, ByVal chkCoPilot As Boolean, ByVal ip As String,
                                            ByVal firstname As String, ByVal lastname As String, ByVal email As String, ByRef bAllowPets As Boolean,
                                            ByRef bAllowSmoking As Boolean, ByRef bWiFi As Boolean, ByRef bLav As Boolean, ByRef bPower As Boolean,
                                            ByRef bInFlightEntertainment As Boolean, ByRef MemberID As Integer, ByRef MemberIDCarrier As Integer)

        '20120201 - pab - changes for azure
        'Dim ws As New AviationService1_9.Service
        '20120423 - pab - call azure web service
        'Dim ws As New AviationService1_10.Service
        Dim ws As New AviationWebService1_10.WebService1

        '20130722 - pab - fix web service call returning before quotes finished
        ws.Timeout = _sleep + 35000

        Dim dsquotes As New DataSet

        '20111229 - pab
        AirTaxi.post_timing("GetLowestQuoteByWeightClass Start  " & Now.ToString)

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        '20120919 - pab - add broker quote
        Dim BrokerID As Integer = 0
        If Not IsNothing(Session("BrokerID")) Then
            If IsNumeric(Session("BrokerID")) Then BrokerID = CInt(Session("BrokerID"))
        End If

        Try
            Dim sdtflights As String = dtflightstoxml(dtflights)

            '20111229 - pab
            AirTaxi.post_timing("GetLowestQuoteByWeightClass ws call start " & Now.ToString)

            '20150831 - pab - add more logging to find out why sometimes no quotes generated
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClass ws call", "QuoteTrip.aspx.vb GetLowestQuoteByWeightClass", "")

            '20120619 - pab - add filters for pets, smoking, wifi
            'dsquotes = ws.GetLowestQuoteByClass("pbaumgart@ctgi.com", "123", _
            '    weightclass, passengers, triptype, usertype, plantype, dsflights, chkCoPilot, ip, firstname, lastname, email, False)
            '20120709 - pab - add lav, power
            'dsquotes = ws.GetLowestQuoteByClass("pbaumgart@ctgi.com", "123", weightclass, passengers, triptype, usertype, plantype, dsflights, _
            '    chkCoPilot, ip, firstname, lastname, email, False, chkPets.Checked, chkSmoking.Checked, chkWiFi.Checked)
            '20120919 - pab - add broker quote
            '20131016 - pab - fix timeout error
            ws.Timeout = 6500000
            '20131118 - pab - add more fields to aircraft
            '20131223 - pab - add carrier specific quotes for admin portal
            '20140221 - pab - use telerik controls
            '20150821 - pab - force login before getting quotes to track users
            dsquotes = ws.GetLowestQuoteByClass("pbaumgart@ctgi.com", "123", weightclass, passengers, triptype, usertype, plantype, dsflights, chkCoPilot, ip,
                firstname, lastname, email, False, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, BrokerID, bInFlightEntertainment, ManufactureDate,
                CInt(Session("carrierid")), MemberID, MemberIDCarrier)
            'AirTaxi.post_timing("Optimize End" & Now.ToString)

            If Not IsNothing(dsquotes) Then
                If dsquotes.Tables.Count > 0 Then
                    If dsquotes.Tables(0).Rows.Count > 0 Then
                        '20150831 - pab - add more logging to find out why sometimes no quotes generated
                        AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClass ws call end - " & dsquotes.Tables(0).Rows.Count.ToString &
                            " rows returned", "QuoteTrip.aspx.vb GetLowestQuoteByWeightClass", "")
                        CreateFlightsTable(dsquotes.Tables(0))

                    Else
                        '20150831 - pab - add more logging to find out why sometimes no quotes generated
                        AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClass ws call end - empty table returned", "QuoteTrip.aspx.vb GetLowestQuoteByWeightClass", "")
                    End If

                Else
                    '20150831 - pab - add more logging to find out why sometimes no quotes generated
                    AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClass ws call end - no tables returned", "QuoteTrip.aspx.vb GetLowestQuoteByWeightClass", "")
                End If

            Else
                '20150831 - pab - add more logging to find out why sometimes no quotes generated
                AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetLowestQuoteByWeightClass ws call end - nothing returned", "QuoteTrip.aspx.vb GetLowestQuoteByWeightClass", "")
            End If

        Catch ex As Exception
            AirTaxi.post_timing("GetLowestQuoteByWeightClass Failed" & Now.ToString)
            '20120606 - pab - write to email queue
            'SendEmail("rkane@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", "GetLowestQuoteByClass failed - round trip", ex.Message)
            'SendEmail("pbaumgart@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "GetLowestQuoteByClass failed - round trip", ex.Message)
            Dim s = ex.Message
            If Not IsNothing(ex.InnerException) Then
                s &= " - " & ex.InnerException.ToString
            End If
            If Not IsNothing(ex.StackTrace) Then
                s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            End If
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, Left(Now & " " & s, 500), "QuoteTrip.aspx.vb GetLowestQuoteByWeightClass", "")
            If ex.Message <> "The operation has timed out" Then
                '20130930 - pab - change email from
                'SendEmail(_carrierid, "info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " QuoteTrip.aspx.vb GetLowestQuoteByWeightClass error", s, False, "", False)
                '20131024 - pab - fix duplicate emails
                'SendEmail(_carrierid, _emailfrom, "rkane@coastalaviationsoftware.com", appName & " QuoteTrip.aspx.vb GetLowestQuoteByWeightClass error", s, False, "", False)
                SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "", appName & " QuoteTrip.aspx.vb GetLowestQuoteByWeightClass error", s, CInt(Session("carrierid")))
            End If

        End Try

        '20111229 - pab
        AirTaxi.post_timing("GetLowestQuoteByWeightClass End  " & Now.ToString)

    End Sub

    '20111221 - pab - get lowest quote for all weight classes
    '20140221 - pab - use telerik controls
    '20150821 - pab - force login before getting quotes to track users
    Private Sub GetQuotesByWeightClassAllCarriers(ByVal weightclass As String, ByVal passengers As Integer, ByVal triptype As String, ByVal usertype As String,
                                            ByVal plantype As String, ByVal dsflights As DataSet, ByVal chkCoPilot As Boolean, ByVal ip As String,
                                            ByVal firstname As String, ByVal lastname As String, ByVal email As String, ByRef bAllowPets As Boolean,
                                            ByRef bAllowSmoking As Boolean, ByRef bWiFi As Boolean, ByRef bLav As Boolean, ByRef bPower As Boolean,
                                            ByRef bInFlightEntertainment As Boolean, ByRef MemberID As Integer, ByRef MemberIDCarrier As Integer)

        '20120423 - pab - call azure web service
        'Dim ws As New AviationService1_10.Service
        Dim ws As New AviationWebService1_10.WebService1
        Dim dsquotes As New DataSet

        '20111229 - pab
        AirTaxi.post_timing("GetQuotesByWeightClassAllCarriers Start  " & Now.ToString)

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        '20130722 - pab - fix web service call returning before quotes finished
        ws.Timeout = _sleep + 35000

        '20120919 - pab - add broker quote
        Dim BrokerID As Integer = 0
        If Not IsNothing(Session("BrokerID")) Then
            If IsNumeric(Session("BrokerID")) Then BrokerID = CInt(Session("BrokerID"))
        End If

        Try
            Dim sdtflights As String = dtflightstoxml(dtflights)

            '20111229 - pab
            AirTaxi.post_timing("GetQuotesByWeightClassAllCarriers ws call  " & Now.ToString)

            '20150831 - pab - add more logging to find out why sometimes no quotes generated
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetQuotesByWeightClassAllCarriers ws call", "QuoteTrip.aspx.vb GetQuotesByWeightClassAllCarriers", "")

            '20120619 - pab - add filters for pets, smoking, wifi
            'dsquotes = ws.GetD2DQuotesByClass("pbaumgart@ctgi.com", "123", _
            '    weightclass, passengers, triptype, usertype, plantype, dsflights, chkCoPilot, ip, firstname, lastname, email, False)
            '20120709 - pab - add lav, power
            'dsquotes = ws.GetD2DQuotesByClass("pbaumgart@ctgi.com", "123", weightclass, passengers, triptype, usertype, plantype, dsflights, _
            '    chkCoPilot, ip, firstname, lastname, email, False, chkPets.Checked, chkSmoking.Checked, chkWiFi.Checked)
            '20120919 - pab - add broker quote
            '20131118 - pab - add more fields to aircraft
            '20131223 - pab - add carrier specific quotes for admin portal
            '20140221 - pab - use telerik controls
            '20150821 - pab - force login before getting quotes to track users
            dsquotes = ws.GetD2DQuotesByClass("pbaumgart@ctgi.com", "123", weightclass, passengers, triptype, usertype, plantype, dsflights, chkCoPilot, ip,
                firstname, lastname, email, False, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, BrokerID, bInFlightEntertainment, ManufactureDate,
                CInt(Session("carrierid")), MemberID, MemberIDCarrier)
            'AirTaxi.post_timing("Optimize End" & Now.ToString)

            If dsquotes.Tables.Count > 0 Then
                If dsquotes.Tables(0).Rows.Count > 0 Then
                    '20150831 - pab - add more logging to find out why sometimes no quotes generated
                    AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetQuotesByWeightClassAllCarriers ws call end - " & dsquotes.Tables(0).Rows.Count.ToString &
                        " rows returned", "QuoteTrip.aspx.vb GetQuotesByWeightClassAllCarriers", "")
                    CreateFlightsTable(dsquotes.Tables(0))
                End If

            Else
                '20150831 - pab - add more logging to find out why sometimes no quotes generated
                AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, "GetQuotesByWeightClassAllCarriers ws call end - no tables returned",
                    "QuoteTrip.aspx.vb GetQuotesByWeightClassAllCarriers", "")
            End If

        Catch ex As Exception
            AirTaxi.post_timing("GetQuotesByWeightClassAllCarriers Failed" & Now.ToString)
            '20120606 - pab - write to email queue
            'SendEmail("rkane@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", "GetLowestQuoteByClass failed - round trip", ex.Message)
            'SendEmail("pbaumgart@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "GetLowestQuoteByClass failed - round trip", ex.Message)
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then
                s &= "" & ex.InnerException.ToString
            End If
            If Not IsNothing(ex.StackTrace) Then
                s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            End If
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, Left(Now & " " & s, 500), "QuoteTrip.aspx.vb GetQuotesByWeightClassAllCarriers", "")
            If ex.Message <> "The operation has timed out" Then
                '20130930 - pab - change email from
                'SendEmail(_carrierid, "info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", _
                '          appName & " QuoteTrip.aspx.vb GetQuotesByWeightClassAllCarriers error", s, False, "", False)
                '20131024 - pab - fix duplicate emails
                'SendEmail(_carrierid, _emailfrom, "rkane@coastalaviationsoftware.com", _
                '          appName & " QuoteTrip.aspx.vb GetQuotesByWeightClassAllCarriers error", s, False, "", False)
                SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "",
                          appName & " QuoteTrip.aspx.vb GetQuotesByWeightClassAllCarriers error", s, CInt(Session("carrierid")))
            End If

        End Try

        '20111229 - pab
        AirTaxi.post_timing("GetQuotesByWeightClassAllCarriers End  " & Now.ToString)

    End Sub

    '20111221 - pab - get lowest quote for all weight classes
    Private Sub CreateFlightsTable(ByVal dt As DataTable)

        Dim dc As DataColumn = Nothing
        Dim dr As DataRow = Nothing

        '20141106 - pab - rewrite quote routine
        Dim sortExp As String = "quoteID,ID"
        Dim drarray() As DataRow
        Dim dr2 As DataRow = Nothing
        Dim prevquoteID As Integer = 0
        Dim prevPrice As String = String.Empty
        Dim prevPriceExplanationDetail As String = String.Empty
        Dim s As String = String.Empty

        '20111229 - pab
        AirTaxi.post_timing("CreateFlightsTable Start  " & Now.ToString)

        If dtquotes.Columns.Count = 0 Then

            '20140620 - pab - quote from admin portal
            dtquotes = Create_dtflights()
            'dc = New DataColumn("ID", System.Type.GetType("System.Int32"))
            'dc.AutoIncrement = True
            'dc.AutoIncrementSeed = 0
            'dc.AutoIncrementStep = 1
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Service Provider", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Origin", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("OriginFacilityName", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Departs", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Destination", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("DestinationFacilityName", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Arrives", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Flight Duration", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Minutes", System.Type.GetType("System.Int32"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Price", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("PriceEdit", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("PriceExplanationDetail", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("ShowPriceExplanation", System.Type.GetType("System.Boolean"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("EmptyLeg", System.Type.GetType("System.Int32"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("AircraftType", System.Type.GetType("System.Int32"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("WeightClass", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("dbname", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("aircraftlogo", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Name", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("FAQPageURL", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            ''20120125 - pab - add carrier logo
            'dc = New DataColumn("carrierlogo", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            ''20120403 - pab - add fuel stops, pets, smoking
            'dc = New DataColumn("FuelStops", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Pets", System.Type.GetType("System.Boolean"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("Smoking", System.Type.GetType("System.Boolean"))
            'dtquotes.Columns.Add(dc)

            ''20120525 - pab - add certifications
            'dc = New DataColumn("certifications", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            ''20120619 - pab - add wifi
            'dc = New DataColumn("wifi", System.Type.GetType("System.Boolean"))
            'dtquotes.Columns.Add(dc)

            ''20120709 - pab - add lav, power
            'dc = New DataColumn("EnclosedLav", System.Type.GetType("System.Boolean"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("PowerAvailable", System.Type.GetType("System.Boolean"))
            'dtquotes.Columns.Add(dc)

            ''20131118 - pab - add more fields to aircraft
            'dc = New DataColumn("InflightEntertainment", System.Type.GetType("System.Boolean"))
            'dtquotes.Columns.Add(dc)

            'dc = New DataColumn("ManufactureDate", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            ''20140219 - pab - owner confirmation
            'dc = New DataColumn("OwnerConfirmation", System.Type.GetType("System.String"))
            'dtquotes.Columns.Add(dc)

            '20130204 - pab - show weight class under aircraft picture
            dc = New DataColumn("WeightClassTitle", System.Type.GetType("System.String"))
            dtquotes.Columns.Add(dc)

        End If

        Try
            'drarray = dt.Select(Nothing, sortExp, DataViewRowState.CurrentRows)

            '20160117 - pab - quote multi-leg trips
            If Session("triptype") = "Lx" Or Session("triptype") = "Mx" Then
                'multi-leg
                For i As Integer = 0 To dt.Rows.Count - 1
                    dr2 = dt.Rows(i)
                    If dr2("quoteID") <> prevquoteID Then

                    End If
                Next

            Else

                'one-way or round trip
                'For i As Integer = 0 To drarray.Length - 1
                For i As Integer = 0 To dt.Rows.Count - 1

                    '20141106 - pab - rewrite quote routine
                    dr2 = dt.Rows(i)
                    'dr2 = drarray(i)

                    If dr2("quoteID") <> prevquoteID Then
                        If Not IsNothing(dr) Then
                            If dr("Origin").ToString.Trim <> "" Then dr("Origin") &= "</table>"
                            If dr("OriginFacilityName").ToString.Trim <> "" Then dr("OriginFacilityName") &= "</table>"
                            If dr("Departs").ToString.Trim <> "" Then dr("Departs") &= "</table>"
                            If dr("Destination").ToString.Trim <> "" Then dr("Destination") &= "</table>"
                            If dr("DestinationFacilityName").ToString.Trim <> "" Then dr("DestinationFacilityName") &= "</table>"
                            If dr("Arrives").ToString.Trim <> "" Then dr("Arrives") &= "</table>"
                            If dr("Flight Duration").ToString.Trim <> "" Then dr("Flight Duration") &= "</table>"
                            If dr("minutes").ToString.Trim <> "" Then dr("minutes") &= "</table>"
                            If dr("EmptyLeg").ToString.Trim <> "" Then dr("EmptyLeg") &= "</table>"
                            If dr("FuelStops").ToString.Trim <> "" Then dr("FuelStops") &= "</table>"
                            If dr("EmptyLegPricing").ToString.Trim <> "" Then dr("EmptyLegPricing") &= "</table>"
                            If dr("legcode").ToString.Trim <> "" Then dr("legcode") &= "</table>"
                            dr("Price") = prevPrice
                            dr("PriceExplanationDetail") = prevPriceExplanationDetail

                            '20141201 - pab - quoteworker rewrite
                            dr("quoteID") = prevquoteID

                            If prevPrice = "" Then
                                prevPrice = prevPrice
                            End If

                            dtquotes.Rows.Add(dr)
                        End If

                        prevquoteID = dr2("quoteID")
                        prevPrice = ""
                        prevPriceExplanationDetail = ""
                        If dr2("Price").ToString.Trim <> "" Then prevPrice = dr2("Price")
                        If dr2("PriceExplanationDetail").ToString.Trim <> "" Then prevPriceExplanationDetail = dr2("PriceExplanationDetail")
                        dr = dtquotes.NewRow
                    End If

                    If dr2("legcode") <> "D" And dr2("legcode") <> "W" Then

                        'dr("ID") = dr2("ID")
                        dr("Service Provider") = dr2("Service Provider")

                        '20120125 - pab - add carrier logo
                        dr("carrierlogo") = dr2("carrierlogo")

                        If dr("Origin").ToString.Trim = "" Then
                            'dr("Origin") = dr2("Origin")
                            dr("Origin") &= "<table valign=""center""><tr><td>" & dr2("Origin") & "</td></tr>"
                        Else
                            'dr("Origin") &= "<br />" & dr2("Origin")
                            dr("Origin") &= "<tr><td>" & dr2("Origin") & "</td></tr>"
                        End If
                        s = dr2("OriginFacilityName")
                        If InStr(s, "/") > 0 Then s = Replace(s, "/", "/ ", 1, 1)
                        'If Len(s) < 20 Then s &= "<br /><br />"
                        If dr("OriginFacilityName").ToString.Trim = "" Then
                            'dr("OriginFacilityName") = s
                            dr("OriginFacilityName") &= "<table valign=""center""><tr><td>" & s & "</td></tr>"
                        Else
                            'dr("OriginFacilityName") &= "<br />" & s
                            dr("OriginFacilityName") &= "<tr><td>" & s & "</td></tr>"
                        End If
                        s = dr2("Departs")
                        'If Len(s) < 15 Then s &= "<br /><br />"
                        If dr("Departs").ToString.Trim = "" Then
                            'dr("Departs") = dr2("Departs")
                            'dr("Departs") &= "<table valign=""center""><tr><td style=""font-size: 11px;"">" & dr2("Departs") & "<br /><br /></td></tr>"
                            dr("Departs") &= "<table valign=""center""><tr><td>" & s & "</td></tr>"
                        Else
                            'dr("Departs") &= "<br />" & dr2("Departs")
                            'dr("Departs") &= "<tr><td style=""font-size: 11px;"">" & dr2("Departs") & "<br /><br /></td></tr>"
                            dr("Departs") &= "<tr><td>" & s & "</td></tr>"
                        End If
                        If dr("Destination").ToString.Trim = "" Then
                            'dr("Destination") = dr2("Destination")
                            dr("Destination") &= "<table valign=""center""><tr><td>" & dr2("Destination") & "</td></tr>"
                        Else
                            'dr("Destination") &= "<br />" & dr2("Destination")
                            dr("Destination") &= "<tr><td>" & dr2("Destination") & "</td></tr>"
                        End If
                        s = dr2("DestinationFacilityName")
                        If InStr(s, "/") > 0 Then s = Replace(s, "/", "/ ", 1, 1)
                        'If Len(s) < 20 Then s &= "<br /><br />"
                        If dr("DestinationFacilityName").ToString.Trim = "" Then
                            'dr("DestinationFacilityName") = s
                            dr("DestinationFacilityName") &= "<table valign=""center""><tr><td>" & s & "</td></tr>"
                        Else
                            'dr("DestinationFacilityName") &= "<br />" & s
                            dr("DestinationFacilityName") &= "<tr><td>" & s & "</td></tr>"
                        End If
                        s = dr2("Arrives")
                        'If Len(s) < 15 Then s &= "<br /><br />"
                        If dr("Arrives").ToString.Trim = "" Then
                            'dr("Arrives") = dr2("Arrives")
                            'dr("Arrives") &= "<table valign=""center""><tr><td style=""font-size: 11px;"">" & dr2("Arrives") & "<br /><br /></td></tr>"
                            dr("Arrives") &= "<table valign=""center""><tr><td>" & s & "</td></tr>"
                        Else
                            'dr("Arrives") &= "<br />" & dr2("Arrives")
                            'dr("Arrives") &= "<tr><td style=""font-size: 11px;"">" & dr2("Arrives") & "<br /><br /></td></tr>"
                            dr("Arrives") &= "<tr><td>" & s & "</td></tr>"
                        End If
                        If dr("Flight Duration").ToString.Trim = "" Then
                            'dr("Flight Duration") = dr2("Flight Duration")
                            'dr("Flight Duration") &= "<table valign=""center""><tr><td>" & dr2("Flight Duration") & "<br /><br /></td></tr>"
                            dr("Flight Duration") &= "<table valign=""center""><tr><td>" & dr2("Flight Duration") & "</td></tr>"
                        Else
                            'dr("Flight Duration") &= "<br />" & dr2("Flight Duration")
                            'dr("Flight Duration") &= "<tr><td>" & dr2("Flight Duration") & "<br /><br /></td></tr>"
                            dr("Flight Duration") &= "<tr><td>" & dr2("Flight Duration") & "</td></tr>"
                        End If
                        If dr("minutes").ToString.Trim = "" Then
                            'dr("minutes") = dr2("minutes")
                            dr("minutes") &= "<table valign=""center""><tr><td>" & dr2("minutes") & "</td></tr>"
                        Else
                            'dr("minutes") &= "<br />" & dr2("minutes")
                            dr("minutes") &= "<tr><td>" & dr2("minutes") & "</td></tr>"
                        End If
                        If dr2("Price").ToString.Trim <> "" Then
                            prevPrice = dr2("Price")
                        End If
                        dr("PriceEdit") = dr2("PriceEdit")
                        If dr2("PriceExplanationDetail").ToString.Trim <> "" Then prevPriceExplanationDetail = dr2("PriceExplanationDetail")
                        dr("ShowPriceExplanation") = dr2("ShowPriceExplanation")
                        If dr("EmptyLeg").ToString.Trim = "" Then
                            'dr("EmptyLeg") = dr2("EmptyLeg")
                            dr("EmptyLeg") &= "<table valign=""center""><tr><td>" & dr2("EmptyLeg") & "</td></tr>"
                        Else
                            'dr("EmptyLeg") &= "<br />" & dr2("EmptyLeg")
                            dr("EmptyLeg") &= "<tr><td>" & dr2("EmptyLeg") & "</td></tr>"
                        End If
                        dr("AircraftType") = dr2("AircraftType")
                        dr("WeightClass") = dr2("WeightClass")
                        dr("dbname") = dr2("dbname")
                        dr("aircraftlogo") = dr2("aircraftlogo")
                        '20160429 - pab - remove pax count from name
                        'dr("Name") = dr2("Name")
                        Dim s2 As String = dr2("Name")
                        Dim n As Integer = InStr(s2, "Up to ")
                        If n > 0 Then
                            s2 = Mid(s2, 1, n - 1)
                        End If
                        dr("Name") = s2
                        dr("FAQPageURL") = dr2("FAQPageURL")

                        '20120403 - pab - add fuel stops, pets, smoking
                        If dr("FuelStops").ToString.Trim = "" Then
                            'dr("FuelStops") = dr2("FuelStops")
                            'dr("FuelStops") &= "<table valign=""center""><tr><td>" & dr2("FuelStops") & "<br /><br /></td></tr>"
                            dr("FuelStops") &= "<table valign=""center""><tr><td>" & dr2("FuelStops") & "</td></tr>"
                        Else
                            'dr("FuelStops") &= "<br />" & dr2("FuelStops")
                            'dr("FuelStops") &= "<tr><td>" & dr2("FuelStops") & "<br /><br /></td></tr>"
                            dr("FuelStops") &= "<tr><td>" & dr2("FuelStops") & "</td></tr>"
                        End If
                        dr("Pets") = dr2("Pets")
                        dr("Smoking") = dr2("Smoking")

                        '20120525 - pab - add certifications
                        dr("certifications") = dr2("certifications")

                        '20120619 - pab - add wifi
                        dr("wifi") = dr2("wifi")

                        '20120709 - pab - add lav, power
                        dr("EnclosedLav") = dr2("EnclosedLav")
                        dr("PowerAvailable") = dr2("PowerAvailable")

                        '20131118 - pab - add more fields to aircraft
                        dr("InflightEntertainment") = dr2("InflightEntertainment")
                        dr("ManufactureDate") = dr2("ManufactureDate")

                        '20140219 - pab - owner confirmation
                        dr("OwnerConfirmation") = dr2("OwnerConfirmation")

                        '20140620 - pab - quote from admin portal
                        If dr("EmptyLegPricing").ToString.Trim = "" Then
                            'dr("EmptyLegPricing") = dr2("EmptyLegPricing")
                            dr("EmptyLegPricing") &= "<table valign=""center""><tr><td>" & dr2("EmptyLegPricing") & "</td></tr>"
                        Else
                            'dr("EmptyLegPricing") &= "<br />" & dr2("EmptyLegPricing")
                            dr("EmptyLegPricing") &= "<tr><td>" & dr2("EmptyLegPricing") & "</td></tr>"
                        End If

                        '20141020 - pab - rewrite quote routine
                        If dr("legcode").ToString.Trim = "" Then
                            'dr("legcode") = dr2("legcode")
                            dr("legcode") &= "<table valign=""center""><tr><td>" & dr2("legcode") & "</td></tr>"
                        Else
                            'dr("legcode") &= "<br />" & dr2("legcode")
                            dr("legcode") &= "<tr><td>" & dr2("legcode") & "</td></tr>"
                        End If

                        '20130204 - pab - show weight class under aircraft picture
                        If InStr(dr2("Name").ToString, ":") > 0 Then
                            dr("WeightClassTitle") = Left(dr2("Name"), InStr(dr2("Name").ToString, ":") - 1)
                        Else
                            Select Case dr2("WeightClass").ToString
                                Case "P"
                                    dr("WeightClassTitle") = "Single Piston"
                                Case "V"
                                    dr("WeightClassTitle") = "VLJ"
                                Case "L"
                                    dr("WeightClassTitle") = "Light"
                                Case "M"
                                    dr("WeightClassTitle") = "Mid"
                                Case "H"
                                    dr("WeightClassTitle") = "Heavy"
                                Case "S"
                                    dr("WeightClassTitle") = "Super Heavy"
                                Case "T"
                                    dr("WeightClassTitle") = "Twin Piston"
                                Case "1"
                                    dr("WeightClassTitle") = "Single Turboprop"
                                Case "2"
                                    dr("WeightClassTitle") = "Twin Turboprop"
                                Case "U"
                                    dr("WeightClassTitle") = "SuperMid"
                                Case Else
                                    dr("WeightClassTitle") = dr2("WeightClass").ToString
                            End Select
                        End If

                    Else

                        If dr2("Price").ToString.Trim <> "" Then
                            prevPrice = dr2("Price")
                        End If
                        If dr2("PriceExplanationDetail").ToString.Trim <> "" Then prevPriceExplanationDetail = dr2("PriceExplanationDetail")

                    End If

                Next

                If Not IsNothing(dr) Then
                    If dr("Origin").ToString.Trim <> "" Then dr("Origin") &= "</table>"
                    If dr("OriginFacilityName").ToString.Trim <> "" Then dr("OriginFacilityName") &= "</table>"
                    If dr("Departs").ToString.Trim <> "" Then dr("Departs") &= "</table>"
                    If dr("Destination").ToString.Trim <> "" Then dr("Destination") &= "</table>"
                    If dr("DestinationFacilityName").ToString.Trim <> "" Then dr("DestinationFacilityName") &= "</table>"
                    If dr("Arrives").ToString.Trim <> "" Then dr("Arrives") &= "</table>"
                    If dr("Flight Duration").ToString.Trim <> "" Then dr("Flight Duration") &= "</table>"
                    If dr("minutes").ToString.Trim <> "" Then dr("minutes") &= "</table>"
                    If dr("EmptyLeg").ToString.Trim <> "" Then dr("EmptyLeg") &= "</table>"
                    If dr("FuelStops").ToString.Trim <> "" Then dr("FuelStops") &= "</table>"
                    If dr("EmptyLegPricing").ToString.Trim <> "" Then dr("EmptyLegPricing") &= "</table>"
                    If dr("legcode").ToString.Trim <> "" Then dr("legcode") &= "</table>"
                    dr("Price") = prevPrice
                    dr("PriceExplanationDetail") = prevPriceExplanationDetail

                    '20141201 - pab - quoteworker rewrite
                    dr("quoteID") = prevquoteID

                    If prevPrice = "" Then
                        prevPrice = prevPrice
                    End If

                    dtquotes.Rows.Add(dr)
                End If

            End If

        Catch ex As Exception
            s = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbCr & vbLf & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace.ToString) Then s &= vbCr & vbLf & ex.StackTrace.ToString
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "CreateFlightsTable", "QuoteTrip.aspx.vb")
        End Try

        '20111229 - pab
        AirTaxi.post_timing("CreateFlightsTable End  " & Now.ToString)

    End Sub

    '20120215 - pab - combine return flights with outbound
    '20141110 - PAB - rewrite quote routine
    'Private Sub CreateCombinedTable(ByVal dtflights As DataTable)

    '    '20111229 - pab
    '    AirTaxi.post_timing("CreateCombinedTable Start  " & Now.ToString)

    '    'Dim dc As DataColumn = Nothing
    '    'Dim dr As DataRow = Nothing
    '    Dim n As Integer = 0
    '    Dim price As Double = 0

    '    CreateReturnTable(dtflights)

    '    Try
    '        For i As Integer = 0 To dtflights.Rows.Count - 1
    '            'dtflights.Rows(i).Item("Origin") = dr("Origin") & "<br />" & dtflights2.Rows(i).Item("Origin")
    '            'dtflights.Rows(i).Item("OriginFacilityName") = dr("Origin") & "<br />" & dtflights2.Rows(i).Item("OriginFacilityName")
    '            'dtflights.Rows(i).Item("Departs") = dr("Origin") & "<br />" & dtflights2.Rows(i).Item("Departs")
    '            'dtflights.Rows(i).Item("Destination") = dr("Origin") & "<br />" & dtflights2.Rows(i).Item("Destination")
    '            'dtflights.Rows(i).Item("DestinationFacilityName") = dr("Origin") & "<br />" & dtflights2.Rows(i).Item("DestinationFacilityName")
    '            'dtflights.Rows(i).Item("Arrives") = dr("Origin") & "<br />" & dtflights2.Rows(i).Item("Arrives")
    '            'dtflights.Rows(i).Item("Price") = dr("Origin") & "<br />" & dtflights2.Rows(i).Item("Price")

    '            If dtflights.Rows(i).Item("Origin") <> dtflights2.Rows(n).Item("Origin") _
    '                    And dtflights.Rows(i).Item("service provider") = dtflights2.Rows(n).Item("service provider") Then
    '                dtflights.Rows(i).Item("Origin") &= "<br />" & dtflights2.Rows(n).Item("Origin")
    '                dtflights.Rows(i).Item("Departs") &= "<br />" & "<br />" & dtflights2.Rows(n).Item("Departs") & "<br />" & "<br />"
    '                dtflights.Rows(i).Item("Destination") &= "<br />" & dtflights2.Rows(n).Item("Destination")
    '                '20120403 - pab - make grid more readable
    '                '20120705 - pab - more formatting for facility names
    '                Dim orig As String = dtflights.Rows(i).Item("OriginFacilityName")
    '                Dim orig2 As String = dtflights2.Rows(n).Item("OriginFacilityName")
    '                Dim dest As String = dtflights.Rows(i).Item("DestinationFacilityName")
    '                Dim dest2 As String = dtflights2.Rows(n).Item("DestinationFacilityName")
    '                orig = Replace(orig, "/", "/ ")
    '                orig2 = Replace(orig2, "/", "/ ")
    '                dest = Replace(dest, "/", "/ ")
    '                dest2 = Replace(dest2, "/", "/ ")
    '                'If Len(dtflights.Rows(i).Item("OriginFacilityName")) < 25 Then
    '                '    dtflights.Rows(i).Item("OriginFacilityName") &= "<br />" & "<br />" & dtflights2.Rows(n).Item("OriginFacilityName") & "<br />"
    '                'Else
    '                '    dtflights.Rows(i).Item("OriginFacilityName") &= "<br />" & dtflights2.Rows(n).Item("OriginFacilityName") & "<br />"
    '                'End If
    '                'If Len(dtflights2.Rows(i).Item("OriginFacilityName")) < 25 Then
    '                '    dtflights.Rows(i).Item("OriginFacilityName") &= "<br />"
    '                'End If
    '                'dtflights.Rows(i).Item("OriginFacilityName") &= "<br />"
    '                'If Len(dtflights.Rows(i).Item("DestinationFacilityName")) < 25 Then
    '                '    dtflights.Rows(i).Item("DestinationFacilityName") &= "<br />" & "<br />" & dtflights2.Rows(n).Item("DestinationFacilityName") & "<br />"
    '                'Else
    '                '    dtflights.Rows(i).Item("DestinationFacilityName") &= "<br />" & dtflights2.Rows(n).Item("DestinationFacilityName") & "<br />"
    '                'End If
    '                'If Len(dtflights2.Rows(i).Item("DestinationFacilityName")) < 25 Then
    '                '    dtflights.Rows(i).Item("DestinationFacilityName") &= "<br />"
    '                'End If
    '                If Len(orig) < 25 Then
    '                    orig &= "<br />" & "<br />" & orig2 & "<br />"
    '                Else
    '                    orig &= "<br />" & orig2 & "<br />"
    '                End If
    '                If Len(dtflights2.Rows(i).Item("OriginFacilityName")) < 25 Then
    '                    orig &= "<br />"
    '                End If
    '                dtflights.Rows(i).Item("OriginFacilityName") = orig & "<br />"
    '                If Len(dest) < 25 Then
    '                    dest &= "<br />" & "<br />" & dest2 & "<br />"
    '                Else
    '                    dest &= "<br />" & dest2 & "<br />"
    '                End If
    '                If Len(dtflights2.Rows(i).Item("DestinationFacilityName")) < 25 Then
    '                    dest &= "<br />"
    '                End If
    '                dtflights.Rows(i).Item("DestinationFacilityName") = dest & "<br />"
    '                '20120418 - pab - only show total price if not one way
    '                'dtflights.Rows(i).Item("DestinationFacilityName") &= "<b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Total Price:</b>"
    '                dtflights.Rows(i).Item("Arrives") &= "<br />" & "<br />" & dtflights2.Rows(n).Item("Arrives") & "<br />"
    '                dtflights.Rows(i).Item("Flight Duration") &= "<br />" & "<br />" & dtflights2.Rows(n).Item("Flight Duration") & "<br />" & "<br />"
    '                price = 0
    '                If IsNumeric(InBetween(1, dtflights.Rows(i).Item("price"), "<strong>", "</strong>")) Then
    '                    price = CDbl(InBetween(1, dtflights.Rows(i).Item("price"), "<strong>", "</strong>"))
    '                End If
    '                dtflights.Rows(i).Item("Price") = "<br />" & dtflights.Rows(i).Item("Price") & "<br />" & dtflights2.Rows(n).Item("Price")
    '                If IsNumeric(InBetween(1, dtflights2.Rows(n).Item("price"), "<strong>", "</strong>")) Then
    '                    price = price + CDbl(InBetween(1, dtflights2.Rows(n).Item("price"), "<strong>", "</strong>"))
    '                End If
    '                '20120418 - pab - only show total price if not one way
    '                'dtflights.Rows(i).Item("Price") &= "<br /><b>" & Format(price, "$###,##0.00") & "</b>"
    '                dtflights.Rows(i).Item("Price") = "<b>" & Format(price, "$###,##0.00") & "</b>"

    '                dtflights.Rows(i).Item("PriceExplanationDetail") = dtflights2.Rows(n).Item("PriceExplanationDetail")

    '                n = n + 1
    '            End If
    '        Next

    '        dtflights2.Clear()

    '    Catch ex As Exception
    '        Dim s As String = ex.Message
    '    End Try

    '    '20111229 - pab
    '    AirTaxi.post_timing("CreateCombinedTable End  " & Now.ToString)

    'End Sub

    '20111220 - pab - add return flight grid
    Private Sub CreateReturnTable(ByVal dtflights As DataTable)

        '20111229 - pab
        AirTaxi.post_timing("CreateReturnTable Start  " & Now.ToString)

        Dim dc As DataColumn = Nothing
        Dim dr As DataRow = Nothing

        If dtflights2.Columns.Count = 0 Then

            '20140620 - pab - quote from admin portal
            dtflights2 = Create_dtflights()
            'dc = New DataColumn("ID", System.Type.GetType("System.Int32"))
            ''dc.AutoIncrement = True
            ''dc.AutoIncrementSeed = 0
            ''dc.AutoIncrementStep = 1
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Service Provider", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Origin", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("OriginFacilityName", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Departs", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Destination", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("DestinationFacilityName", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Arrives", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Flight Duration", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Minutes", System.Type.GetType("System.Int32"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Price", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("PriceEdit", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("PriceExplanationDetail", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("ShowPriceExplanation", System.Type.GetType("System.Boolean"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("EmptyLeg", System.Type.GetType("System.Int32"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("AircraftType", System.Type.GetType("System.Int32"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("WeightClass", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("dbname", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("aircraftlogo", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Name", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("FAQPageURL", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            ''20120125 - pab - add carrier logo
            'dc = New DataColumn("carrierlogo", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            ''20120403 - pab - add fuel stops, pets, smoking
            'dc = New DataColumn("FuelStops", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Pets", System.Type.GetType("System.Boolean"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("Smoking", System.Type.GetType("System.Boolean"))
            'dtflights2.Columns.Add(dc)

            ''20120525 - pab - add certifications
            'dc = New DataColumn("certifications", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            ''20120619 - pab - add wifi
            'dc = New DataColumn("wifi", System.Type.GetType("System.Boolean"))
            'dtflights2.Columns.Add(dc)

            ''20120709 - pab - add lav, power
            'dc = New DataColumn("EnclosedLav", System.Type.GetType("System.Boolean"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("PowerAvailable", System.Type.GetType("System.Boolean"))
            'dtflights2.Columns.Add(dc)

            ''20131118 - pab - add more fields to aircraft
            'dc = New DataColumn("InflightEntertainment", System.Type.GetType("System.Boolean"))
            'dtflights2.Columns.Add(dc)

            'dc = New DataColumn("ManufactureDate", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            ''20140219 - pab - owner confirmation
            'dc = New DataColumn("OwnerConfirmation", System.Type.GetType("System.String"))
            'dtflights2.Columns.Add(dc)

            '20130204 - pab - show weight class under aircraft picture
            dc = New DataColumn("WeightClassTitle", System.Type.GetType("System.String"))
            dtflights2.Columns.Add(dc)

        End If

        Try
            For i As Integer = 0 To dtflights.Rows.Count - 1
                If dtflights.Rows(i).Item("Origin") <> Session("origAirportCode").ToString Then

                    dr = dtflights2.NewRow

                    dr("ID") = dtflights.Rows(i).Item("ID")
                    dr("Service Provider") = dtflights.Rows(i).Item("Service Provider")

                    '20120125 - pab - add carrier logo
                    dr("carrierlogo") = dtflights.Rows(i).Item("carrierlogo")

                    dr("Origin") = dtflights.Rows(i).Item("Origin")
                    dr("OriginFacilityName") = dtflights.Rows(i).Item("OriginFacilityName")
                    dr("Departs") = dtflights.Rows(i).Item("Departs")
                    dr("Destination") = dtflights.Rows(i).Item("Destination")
                    dr("DestinationFacilityName") = dtflights.Rows(i).Item("DestinationFacilityName")
                    dr("Arrives") = dtflights.Rows(i).Item("Arrives")
                    dr("Flight Duration") = dtflights.Rows(i).Item("Flight Duration")
                    dr("minutes") = dtflights.Rows(i).Item("minutes")
                    dr("Price") = dtflights.Rows(i).Item("Price")
                    dr("PriceEdit") = dtflights.Rows(i).Item("PriceEdit")
                    dr("PriceExplanationDetail") = dtflights.Rows(i).Item("PriceExplanationDetail")
                    dr("ShowPriceExplanation") = dtflights.Rows(i).Item("ShowPriceExplanation")
                    dr("EmptyLeg") = dtflights.Rows(i).Item("EmptyLeg")
                    dr("AircraftType") = dtflights.Rows(i).Item("AircraftType")
                    dr("WeightClass") = dtflights.Rows(i).Item("WeightClass")
                    dr("dbname") = dtflights.Rows(i).Item("dbname")
                    dr("aircraftlogo") = dtflights.Rows(i).Item("aircraftlogo")
                    dr("Name") = dtflights.Rows(i).Item("Name")
                    dr("FAQPageURL") = dtflights.Rows(i).Item("FAQPageURL")

                    '20120403 - pab - add fuel stops, pets, smoking
                    dr("FuelStops") = dtflights.Rows(i).Item("FuelStops")
                    dr("Pets") = dtflights.Rows(i).Item("Pets")
                    dr("Smoking") = dtflights.Rows(i).Item("Smoking")

                    '20120525 - pab - add certifications
                    dr("certifications") = dtflights.Rows(i).Item("certifications")

                    '20120619 - pab - add wifi
                    dr("wifi") = dtflights.Rows(i).Item("wifi")

                    '20120709 - pab - add lav, power
                    dr("EnclosedLav") = dtflights.Rows(i).Item("EnclosedLav")
                    dr("PowerAvailable") = dtflights.Rows(i).Item("PowerAvailable")

                    '20131118 - pab - add more fields to aircraft
                    dr("InflightEntertainment") = dtflights.Rows(i).Item("InflightEntertainment")
                    dr("ManufactureDate") = dtflights.Rows(i).Item("ManufactureDate")

                    '20140219 - pab - owner confirmation
                    dr("OwnerConfirmation") = dtflights.Rows(i).Item("OwnerConfirmation")

                    '20140620 - pab - quote from admin portal
                    dr("EmptyLegPricing") = dtflights.Rows(i).Item("EmptyLegPricing")

                    '20141020 - pab - rewrite quote routine
                    dr("legcode") = dtflights.Rows(i).Item("legcode")

                    '20130204 - pab - show weight class under aircraft picture
                    If InStr(dtflights.Rows(i).Item("Name").ToString, ":") > 0 Then
                        dr("WeightClassTitle") = Left(dtflights.Rows(i).Item("Name"), InStr(dtflights.Rows(i).Item("Name").ToString, ":") - 1)
                    Else
                        Select Case dtflights.Rows(i).Item("WeightClass").ToString
                            Case "P"
                                dr("WeightClassTitle") = "Single Piston"
                            Case "V"
                                dr("WeightClassTitle") = "VLJ"
                            Case "L"
                                dr("WeightClassTitle") = "Light"
                            Case "M"
                                dr("WeightClassTitle") = "Mid"
                            Case "H"
                                dr("WeightClassTitle") = "Heavy"
                            Case "S"
                                dr("WeightClassTitle") = "Super Heavy"
                            Case "T"
                                dr("WeightClassTitle") = "Twin Piston"
                            Case "1"
                                dr("WeightClassTitle") = "Single Turboprop"
                            Case "2"
                                dr("WeightClassTitle") = "Twin Turboprop"
                            Case "U"
                                dr("WeightClassTitle") = "SuperMid"
                            Case Else
                                dr("WeightClassTitle") = dtflights.Rows(i).Item("WeightClass").ToString
                        End Select
                    End If

                    dtflights2.Rows.Add(dr)

                End If
            Next

            Dim rowfound As Boolean = True
            Do While rowfound = True
                For i As Integer = 0 To dtflights.Rows.Count - 1
                    rowfound = False
                    If dtflights.Rows(i).Item("Origin") <> Session("origAirportCode").ToString Then
                        dtflights.Rows(i).Delete()
                        rowfound = True
                        Exit For
                    End If
                Next
            Loop

        Catch ex As Exception
            Dim s As String = ex.Message
        End Try

        '20111229 - pab
        AirTaxi.post_timing("CreateReturnTable End  " & Now.ToString)

    End Sub

    '20101208 - pab - get lowest quote accross carriers
    'Protected Sub quote()

    '    Dim priceoverride As Boolean = False

    '    'rlk 11/1/2010 trap an optimizer failure
    '    Dim optmizerfailmsg As String = ""

    '    AirTaxi.post_timing("Start Quote  " & Now.ToString)



    '    Dim dc As DataColumn = Nothing
    '    Dim dr As DataRow = Nothing
    '    Dim dr1 As DataRow = Nothing


    '    _waittime = 0
    '    _landingfees = 0
    '    _taxes = 0
    '    _internationalfees = 0


    '    _fuelsurcharges = 0
    '    _crewovernight = 0
    '    _totalprice = 0
    '    'rk 10302010 - add taxes to quote editor
    '    _segmentfee = 0

    '    '20101027 - pab - add return to base bucket
    '    _rtbcost = 0

    '    '20101102 - pab - fix intl fees
    '    _intl = 0
    '    _customs = 0

    '    '20101027 - pab - fix totals on grid
    '    Dim TotalWaitTime As Double = 0
    '    Dim TotalLandingFees As Double = 0
    '    Dim TotalIntlFees As Double = 0
    '    Dim TotalFuelSurcharges As Double = 0
    '    Dim TotalCrewOvernight As Double = 0
    '    Dim TotalAddCharges As Double = 0
    '    Dim TotalTaxes As Double = 0
    '    Dim GrandTotalPrice As Double = 0

    '    Dim prevdest As String

    '    Me.lblMsg.Text = ""
    '    If dtflights.Rows.Count = 0 Then
    '        Me.lblMsg.Text = "Please add a flight segment first"
    '        Exit Sub
    '    End If

    '    '20101102 - pab - fix intl fees
    '    Dim intl As Double = 0
    '    Dim customs As Double = 0

    '    'rk 8.16.2010 - make sure passengers field is et
    '    'If Not Request.QueryString("passengers") Is Nothing Then
    '    '    _passengers = CInt(Request.QueryString("passengers"))
    '    'End If

    '    'If _passengers = 0 Then
    '    '_passengers = Session("pax")
    '    '20101027 - pab - use correct session variable
    '    _passengers = Session("passengers")
    '    'End If

    '    'rk 10.17.2010 don't allow more than six passengers on king air with optional copiot
    '    If ChkCoPilot.Checked = True Then
    '        If _passengers > 6 Then
    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("DestinationFacilityName") = "<strong>" & "King Air Only Six Passengers with Co-Pilot" & vbCr & vbLf
    '            Session("optimize") = "FAIL"
    '            dtflights.Rows.Add(dr)
    '            Me.gvServiceProviderMatrix.DataSource = dtflights
    '            Me.gvServiceProviderMatrix.DataBind()
    '            Me.lblMsg.Text = "If a co-pilot is selected, there is a 6-passenger maximum.  Please select another aircraft or reduce no. of passengers."
    '            Exit Sub
    '        End If
    '    End If




    '    Dim fwh As Double = CDbl(da.GetSetting("FreeWaitHours"))

    '    Dim pedetail As String = ""
    '    Dim ma, ma1 As Double
    '    Dim d As Double
    '    Dim minutesinair As Double = 0
    '    Dim closest_base As String
    '    Dim totalprice As Double
    '    Dim dispatch_base As String
    '    Dim _airSpeed As Double = 0
    '    Dim t As Long
    '    Dim da As New DataAccess
    '    ' _airSpeed = CDbl(da.GetAirSpeed) * 1.15077945  'don't use this, get blockspeed
    '    Dim currentplane As Integer = 0
    '    Dim currtype As Integer = Session("currtype")

    '    '20101027 - pab - add return to base bucket
    '    Dim rtbcost As Double = 0



    '    'RK 6.17.2010 This is a work around to return a home base by type - will have to be driven to specific aircraft at a later date in
    '    'larger carriers
    '    Dim homebaseairportcode As String = ""        '  sp_GetAircraftHomeBaseByType()
    '    Dim dt_homebase As DataTable = da.GetHomeBaseByType(currtype)
    '    homebaseairportcode = dt_homebase.Rows(0)("HomebaseAirportCode")

    '    If Len(homebaseairportcode) = 4 And Left(homebaseairportcode, 1) = "K" Then homebaseairportcode = Right(homebaseairportcode, 3) 'rk 11/9/2010 adjust for K airport names


    '    Dim dt_priceTable As DataTable = da.GetAircraftTypeServiceSpecsByID(currtype)


    '    '20101029 - pab - change cost fields from int to lng
    '    'Dim discountPricePerHour As Integer = CInt(dt_priceTable.Rows(currentplane)("DiscountPricePerHour"))
    '    'Dim CostPerHourFlightTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourFlightTime"))
    '    ''20081218 - pab - add additional fees
    '    'Dim TotalWeighT As Integer = CInt(dt_priceTable.Rows(currentplane)("TotalWeighT"))
    '    'Dim perHourRepositionTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourRepositionTime"))
    '    'Dim perNightOvernight As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerNightOvernight"))
    '    'Dim perCycleFee As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerCycleFee"))
    '    'Dim perDayCrewExpenses As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerDayCrewExpenses"))
    '    'Dim perHourWaitTime As Integer = CInt(dt_priceTable.Rows(currentplane)("CostPerHourWaitTime"))
    '    'Dim MinFlightDuration As Integer = CInt(dt_priceTable.Rows(currentplane)("MinFlightDuration"))
    '    'Dim TotalPassengers As Integer = CInt(dt_priceTable.Rows(currentplane)("TotalPassengers"))
    '    '  Dim HomeBaseAirportCode As String = (dt_priceTable.Rows(currentplane)("HomeBaseAirportCode"))
    '    Dim CostPerHourFlightTime As Long = CInt(dt_priceTable.Rows(currentplane)("CostPerHourFlightTime"))
    '    '20081218 - pab - add additional fees
    '    Dim TotalWeighT As Double = dt_priceTable.Rows(currentplane)("TotalWeighT")
    '    _totalweight = TotalWeighT
    '    Dim perHourRepositionTime As Double = dt_priceTable.Rows(currentplane)("CostPerHourRepositionTime")
    '    Dim perNightOvernight As Double = dt_priceTable.Rows(currentplane)("CostPerNightOvernight")
    '    Dim perCycleFee As Double = dt_priceTable.Rows(currentplane)("CostPerCycleFee")
    '    Dim perDayCrewExpenses As Double = dt_priceTable.Rows(currentplane)("CostPerDayCrewExpenses")
    '    Dim perHourWaitTime As Double = dt_priceTable.Rows(currentplane)("CostPerHourWaitTime")
    '    Dim MinFlightDuration As Double = dt_priceTable.Rows(currentplane)("MinFlightDuration")
    '    Dim TotalPassengers As Double = dt_priceTable.Rows(currentplane)("TotalPassengers")
    '    Dim PerDayCoPilotExpenses As Double
    '    If Not IsDBNull(dt_priceTable.Rows(currentplane)("CostPerDayCoPilotExpenses")) Then
    '        PerDayCoPilotExpenses = dt_priceTable.Rows(currentplane)("CostPerDayCoPilotExpenses")
    '    End If
    '    Dim FlightTimeSwag As Double = dt_priceTable.Rows(currentplane)("FlightTimeSwag")
    '    Dim AwayLandingFee As Double = dt_priceTable.Rows(currentplane)("AwayLandingFee")
    '    Dim AircraftTypeServiceSpecID As Integer = currtype
    '    Dim bQuoteAtCapacity As Boolean = CBool(dt_priceTable.Rows(currentplane)("ContinueToQuoteAtCapacity"))


    '    If _addCoPilot = False Then PerDayCoPilotExpenses = 0

    '    Dim name As String = dt_priceTable.Rows(currentplane)("name")
    '    Me.LblItin.Text = "Aircraft: " & name
    '    _aircraftname = name
    '    _aircraftlogo = dt_priceTable.Rows(currentplane)("logourl")

    '    _airSpeed = CInt(dt_priceTable.Rows(currentplane)("Blockspeed")) * 1.15077945

    '    '20100812 - rlk - waive handling fees if take fuel
    '    Dim longrangedistance As Integer = CInt(dt_priceTable.Rows(0)("LongRangeDistance"))



    '    '20100723 - bd - Use new aircraft performance class
    '    Dim dtAPD As DataTable = AircraftPerformance.FuelSpeedByDistance.Get(currtype) 'da.GetAircraftPerformanceFuelByDistanceAndID(currtype)

    '    '20100524 - pab - add new fees
    '    Dim perDayCabinAideExpenses As Double = dt_priceTable.Rows(currentplane)("CostPerDayCabinAideExpenses")

    '    Dim FuelSurcharge As Double = 0
    '    If IsNumeric(da.GetSetting("fuelsurcharge")) Then
    '        FuelSurcharge = CDbl(da.GetSetting("fuelsurcharge"))
    '    End If


    '    Dim CycleFees As Integer
    '    '20090223 - pab - change hardcoded values to configurable
    '    Dim TaxiMinutes As Integer = CInt(dt_priceTable.Rows(currentplane)("TaxiMinutes"))
    '    Dim pilotcost As Long = 0
    '    Dim waitcost As Long = 0
    '    Dim sameday As Boolean = False

    '    Dim flightcharge As Decimal

    '    Dim _price As Double

    '    '20101029 - pab - make ChargeCabinAide configurable
    '    Dim bChargeCabinAide As Boolean = False
    '    If da.GetSetting("ChargeCabinAide") = "Y" Then bChargeCabinAide = True
    '    If Not bChargeCabinAide Then
    '        perDayCabinAideExpenses = 0
    '    End If

    '    _leaves = CDate(dtflights.Rows(0).Item("departs"))
    '    _returns = cdatesafe(dtflights.Rows(dtflights.Rows.Count - 1).Item("arrives"))

    '    Me.lblmsg.Text = ""
    '    '  If _leaves < Now Or _returns < Now Then
    '    If _leaves < Now Then 'rk 10/20/2010 this is the line that keeps a new airplane from being quoted.
    '        'aircraft not avaialble had added an extra row!
    '        Me.lblmsg.Text = "Please hit start over and begin again."
    '        Exit Sub
    '    End If


    '    If Month(_leaves) = Month(_returns) And Day(_leaves) = Day(_returns) And Year(_leaves) = Year(_returns) Then sameday = True

    '    '20090120 - pab - add additional fees
    '    Dim days As Integer = 0
    '    Dim minutes As Integer = 0

    '    '20101103 - pab - fix pedetail
    '    Dim bRTB As Boolean = False

    '    pedetail = ""

    '    '-- ~DHA
    '    Dim oQuote As New SaveQuoteInfo '--This code puts each quote detail into table: for analysis.

    '    If Session("triptype") = "M" Then
    '        pedetail = pedetail & "This is a quote for a multi leg trip with " & dtflights.Rows.Count & " legs." & vbCr & vbLf & vbCr & vbLf
    '        oQuote.QuoteInfoArray("1-1|Multileg with legs", dtflights.Rows.Count)
    '    End If

    '    pedetail = pedetail & "This is a quote for a " & name & " created on " & Now & "." & vbCr & vbLf & vbCr & vbLf
    '    oQuote.QuoteInfoArray("2-1|Quote For", name)
    '    oQuote.QuoteInfoArray("2-2|Quoted On", Now)

    '    pedetail = pedetail & "This is an aircraft type " & currtype & "." & vbCr & vbLf & vbCr & vbLf
    '    oQuote.QuoteInfoArray("3-1|AircraftType", currtype)

    '    '20100608 - pab - rig city pair for internal credit card testing
    '    Dim bAdminLogin As Boolean = False
    '    If Session("usertype") = "A" Then bAdminLogin = True
    '    'rk change for user type


    '    Dim orig As String

    '    Dim airplaneid = typeforplane(currtype)
    '    Dim arrives, departs As Date

    '    Dim taxes As Decimal


    '    Dim arrival, departure As Date

    '    Dim optimizerfail As Boolean = False
    '    optimizerfail = False
    '    Dim optimizerwarning As String = ""
    '    Session("optimize") = "OK"

    '    Dim dest As String
    '    For i = 0 To dtflights.Rows.Count - 1

    '        _internationalfees = 0


    '        AirTaxi.post_timing("Quote leg:  " & i.ToString & "  " & Now.ToString)


    '        oQuote.QuoteInfoArray("0|-----BEGIN LEG: ", i.ToString)
    '        pilotcost = 0
    '        waitcost = 0
    '        ma1 = 0
    '        ma = 0

    '        '20100602 - pab - fix pricing
    '        _price = 0
    '        CycleFees = 0

    '        '20101027 - pab - add return to base bucket
    '        rtbcost = 0

    '        '20101102 - pab - fix intl fees and pedetail
    '        intl = 0
    '        customs = 0
    '        bRTB = False

    '        dr = dtflights.Rows(i)            'dr("Price") = i
    '        orig = dr("Origin").ToString
    '        dest = dr("Destination").ToString
    '        minutesinair = dr("minutes")






    '        If IsDate(dtflights.Rows(i).Item("arrives")) Then
    '            arrival = CDate(dtflights.Rows(i).Item("arrives"))
    '        End If
    '        If IsDate(dtflights.Rows(i).Item("departs")) Then
    '            departure = CDate(dtflights.Rows(i).Item("departs"))
    '        End If



    '        pedetail = pedetail & "______________________________________________" & dest & vbCr & vbLf & vbCr & vbLf
    '        pedetail = pedetail & "For Leg " & i + 1 & " from " & orig & " to " & dest & " on " & departure.ToShortDateString & " at " & departure.ToShortTimeString & vbCr & vbLf & vbCr & vbLf
    '        pedetail = pedetail & "______________________________________________" & dest & vbCr & vbLf & vbCr & vbLf
    '        'if i = 0 that's the first leg
    '        ' pe = "For up to 3 Passengers" & vbCr & vbLf
    '        pedetail = pedetail & "The selected origin airport is " & fname(orig) & vbCr & vbLf
    '        pedetail = pedetail & vbCr & vbLf
    '        pedetail = pedetail & "The selected destination airport is " & fname(dest) & vbCr & vbLf
    '        pedetail = pedetail & vbCr & vbLf

    '        d = oMapping.GetRoundEarthDistanceBetweenLocations("pbaumgart@ctgi.com", "123", orig, dest)




    '        Dim milesInAir As Double = d

    '        pedetail = pedetail & "The distance between " & orig & " and " & dest & " is " & d & " statute miles" & vbCr & vbLf
    '        pedetail = pedetail & vbCr & vbLf

    '        '-- ~DHA
    '        oQuote.QuoteInfoArray("4-1|Leg" & (i + 1).ToString & "|Origin", fname(orig))
    '        oQuote.QuoteInfoArray("4-2|Leg" & (i + 1).ToString & "|Destination", fname(dest))
    '        oQuote.QuoteInfoArray("4-3|Leg" & (i + 1).ToString & "Distance|" & orig & "|" & dest & "|statute miles", d)

    '        Dim sSaveString As String = ""

    '        Dim fees As Double = 0
    '        '20100323 - pab - add airport pax fees
    '        Dim bIntl As Boolean = isflightintl(orig, dest)

    '        'chg3620  - 20100923 - pab - fix international flag
    '        If bIntl Then
    '            Session("international_type") = "intl"
    '        Else
    '            Session("international_type") = "domestic"
    '        End If

    '        '20100323 - pab - add airport pax fees
    '        sSaveString = feesatairport(orig, False, bIntl)
    '        'pedetail = pedetail & feesatairport(orig) & vbCr & vbLf
    '        'pedetail = pedetail & feesatairport(orig, False, bIntl) & vbCr & vbLf
    '        pedetail = pedetail & sSaveString & vbCr & vbLf
    '        sSaveString = sSaveString.Replace(vbCr, "*").Replace(vbLf, "*")
    '        oQuote.QuoteInfoArray("5-1|FeesAtAirport|Origin", sSaveString)
    '        fees = fees + _pricefees

    '        '20101102 - pab - fix intl fees
    '        intl = intl + _intl
    '        customs = customs + _customs

    '        '20100323 - pab - add airport pax fees
    '        'pedetail = pedetail & feesatairport(dest) & vbCr & vbLf
    '        'pedetail = pedetail & feesatairport(dest, True, bIntl) & vbCr & vbLf

    '        sSaveString = feesatairport(dest, True, bIntl)
    '        pedetail = pedetail & sSaveString & vbCr & vbLf
    '        sSaveString = sSaveString.Replace(vbCr, "*").Replace(vbLf, "*")
    '        oQuote.QuoteInfoArray("5-1|FeesAtAirport|Destination", sSaveString)

    '        fees = fees + _pricefees

    '        '20101102 - pab - fix intl fees
    '        intl = intl + _intl
    '        customs = customs + _customs


    '        'check if capacity available

    '        '20100812 - rlk - waiving handling fee at destination if taking fuel
    '        'get handlign fee on destination from above
    '        Dim handlingfeeatdest As String = CInt(InBetween(1, sSaveString, "<", ">")) ' return handling fee at destination
    '        'pedetail = pedetail & feedest & vbCr & vbLf
    '        If IsNumeric(da.GetSetting("FuelRangePercent")) Then 'give a discount on hanlding fees if a % set.
    '            Dim FuelRangePercent As Double = CDbl(da.GetSetting("FuelRangePercent"))
    '            If FuelRangePercent <> 0 Then
    '                If IsNumeric(handlingfeeatdest) Then
    '                    If d > longrangedistance * FuelRangePercent Then 'we are going to take fuel
    '                        fees = fees - handlingfeeatdest
    '                        pedetail = pedetail & "We will take fuel at " & dest & " eliminating  the handling fee of  " & handlingfeeatdest & vbCr & vbLf & vbCr & vbLf
    '                    End If
    '                End If
    '            End If
    '        End If






    '        _landingfees = _landingfees + _pricefees



    '        '20101027 - pab - fix totals on grid
    '        TotalIntlFees = TotalIntlFees + _internationalfees

    '        '20101102 - pab - add intl fees to grand total
    '        GrandTotalPrice = GrandTotalPrice + _internationalfees

    '        'orig = dri("LocationID").ToString
    '        'dest = dro("LocationID").ToString

    '        ma1 = 0
    '        If i = 0 Then 'origination of itinerary
    '            '8/25/09 Kane pick closest base
    '            'reposition back to RDU

    '            '20100727 - pab - fix pricing
    '            closest_base = homebaseairportcode 'closesttobase("DXR", "ORH", orig)
    '            dispatch_base = closest_base

    '            If orig <> homebaseairportcode Then
    '                '   d = getdistance("RDU", dest23)
    '                '  Dim closest_base As String
    '                '20100727 - pab - fix pricing
    '                'closest_base = homebaseairportcode 'closesttobase("DXR", "ORH", orig)
    '                'dispatch_base = closest_base
    '                d = oMapping.GetRoundEarthDistanceBetweenLocations("pbaumgart@ctgi.com", "123", closest_base, orig)

    '                Dim ii As Integer
    '                If dtAPD.Rows.Count <> 0 Then
    '                    For ii = 0 To dtAPD.Rows.Count - 1
    '                        If d >= dtAPD.Rows(ii)("RangeLow") And d <= dtAPD.Rows(ii)("RangeHigh") Then
    '                            _airSpeed = dtAPD.Rows(ii)("speed") * 1.15077945
    '                            pedetail = pedetail & "Airspeed Adjusted by Performance Table to " & _airSpeed & " which is " & dtAPD.Rows(ii)("speed") & " multipled by 1.15077945" & vbCr & vbLf & vbCr & vbLf
    '                            oQuote.QuoteInfoArray("6-1." & ii.ToString & "|Airspeed Adjusted by Performance Table to ", _airSpeed)
    '                            oQuote.QuoteInfoArray("6-2." & ii.ToString & "|Airspeed is " & dtAPD.Rows(ii)("speed") & "(* by 1.15077945)", dtAPD.Rows(ii)("speed"))
    '                        End If
    '                    Next ii
    '                End If

    '                t = CInt((d / _airSpeed) * 60)
    '                t = t + (t * FlightTimeSwag) 'add an optional percentage to each flight time calculation

    '                '20090223 - pab - change hardcoded values to configurable
    '                'ma1 = CInt(t) + 12
    '                ma1 = CInt(t) + TaxiMinutes

    '                '20081218 - pab - add additional fees
    '                'pedetail = pedetail & "We will reposition this flight from ADS to " & orig & " at a cost of $" & (ma1 * 3385 / 60) & _
    '                '    " for " & d & " miles and " & ma1 & " minutes. " & vbCr & vbLf & vbCr & vbLf
    '                If perHourRepositionTime <> 0 Then
    '                    pedetail = pedetail & "We will reposition this flight from " & closest_base & " to " & orig & " at a cost of $" & Math.Round((ma1 * perHourRepositionTime / 60), 2) & _
    '                        " for " & d & " miles and " & ma1 & " minutes. " & vbCr & vbLf & vbCr & vbLf

    '                    oQuote.QuoteInfoArray("7-1|Reposition from|" & closest_base & "|to|" & orig & "|cost", (ma1 * perHourRepositionTime / 60))
    '                    oQuote.QuoteInfoArray("7-2|Reposition", d & "miles")
    '                    oQuote.QuoteInfoArray("7-3|Reposition", ma1 & "minutes")
    '                End If
    '            End If
    '        End If

    '        ma = 0
    '        '8/25/09 Kane pick closest base
    '        If i = dtflights.Rows.Count - 1 Then 'last row of itinerary
    '            'reposition from RDU
    '            If dest <> homebaseairportcode Then
    '                ' d = getdistance("RDU", orig1)
    '                '  closest_base = closesttobase("DXR", "ORH", dest)
    '                d = oMapping.GetRoundEarthDistanceBetweenLocations("pbaumgart@ctgi.com", "123", dispatch_base, dest)

    '                Dim ii As Integer
    '                If dtAPD.Rows.Count <> 0 Then
    '                    For ii = 0 To dtAPD.Rows.Count - 1
    '                        If d >= dtAPD.Rows(ii)("RangeLow") And d <= dtAPD.Rows(ii)("RangeHigh") Then
    '                            _airSpeed = dtAPD.Rows(ii)("speed") * 1.15077945
    '                            pedetail = pedetail & "Airspeed Adjusted by Performance Table to " & _airSpeed & " which is " & dtAPD.Rows(ii)("speed") & " multipled by 1.15077945" & vbCr & vbLf & vbCr & vbLf

    '                            oQuote.QuoteInfoArray("8-1|." & ii.ToString & "|Airspeed Adjusted by Performance Table to ", _airSpeed)
    '                            oQuote.QuoteInfoArray("8-2|." & ii.ToString & "|Airspeed is " & dtAPD.Rows(ii)("speed") & "(* by 1.15077945)", dtAPD.Rows(ii)("speed"))

    '                        End If
    '                    Next ii
    '                End If

    '                t = CInt((d / _airSpeed) * 60)
    '                t = t + (t * FlightTimeSwag) 'add an optional percentage to each flight time calculation

    '                '20090223 - pab - change hardcoded values to configurable
    '                'ma = CInt(t) + 12
    '                ma = CInt(t) + TaxiMinutes

    '                '20081218 - pab - add additional fees
    '                'pedetail = pedetail & "We will return the aircraft from " & dest & " to ADS at a cost of $" & (ma * 3385 / 60) & _
    '                '" for " & d & " miles and " & ma & " minutes. " & vbCr & vbLf & vbCr & vbLf
    '                pedetail = pedetail & "We will return the aircraft from " & dest & " to " & closest_base & " at a cost of $" & Math.Round((ma * perHourRepositionTime / 60), 2) & _
    '                " for " & d & " miles and " & ma & " minutes. " & vbCr & vbLf & vbCr & vbLf

    '                oQuote.QuoteInfoArray("9-1|Return aircraft|" & dest & "|to|" & closest_base & "|cost", (ma1 * perHourRepositionTime / 60))
    '                oQuote.QuoteInfoArray("9-2|Reposition", d & "miles")
    '                oQuote.QuoteInfoArray("9-3|Reposition", ma1 & "minutes")
    '            End If
    '        End If

    '        'only compute 
    '        ' If dtflights.Rows.Count < 3 Then

    '        'compute the wait time between legs
    '        Dim minuteswaitbetweensegments As Double = 0

    '        days = 0
    '        minutes = 0

    '        If i > 0 Then 'not the first row

    '            If IsDate(dtflights.Rows(i - 1).Item("arrives")) Then
    '                arrives = CDate(dtflights.Rows(i - 1).Item("arrives"))
    '            End If
    '            If IsDate(dtflights.Rows(i).Item("departs")) Then
    '                departs = CDate(dtflights.Rows(i).Item("departs"))
    '            End If
    '            minuteswaitbetweensegments = DateDiff(DateInterval.Minute, arrives, departs)
    '            If minuteswaitbetweensegments > 60 * 24 * 10 Then minuteswaitbetweensegments = 0
    '            If Not sameday Then
    '                days = CInt(DateDiff(DateInterval.Day, CDate(Format(arrives, "d")), CDate(Format(departs, "d"))))
    '            Else
    '                minutes = CInt(DateDiff(DateInterval.Minute, CDate(Format(arrives, "t")), CDate(Format(departs, "t"))))
    '            End If

    '            dr1 = dtflights.Rows(i - 1)
    '            ' Dim orig As String = dr("Origin").ToString
    '            prevdest = dr1("Destination").ToString
    '        End If
    '        Dim extrawaitcosts As Double
    '        extrawaitcosts = 0

    '        '20101027 - pab - make charging waitcosts on overnight flights configurable
    '        Dim bChargeWaitTimeOvernight As Boolean = False
    '        If da.GetSetting("ChargeWaitTimeOvernight") = "Y" Then bChargeWaitTimeOvernight = True

    '        If Not sameday Then ' And i > 0 Then
    '            'if they arrive before noon on an overnight, charge an additional
    '            'four hours of wait time

    '            '20101027 - pab - make charging waitcosts on overnight flights configurable

    '            '****************  carolyn said NOT to put this in!!!!!!!!!!!
    '            'If orig <> homebaseairportcode And bChargeWaitTimeOvernight Then 'dont charge for waiting at home base

    '            If orig <> homebaseairportcode Then 'dont charge for waiting at home base
    '                waitcost = CLng((perHourWaitTime * (minuteswaitbetweensegments - (fwh * 60)) / 60))
    '                If waitcost < 0 Then waitcost = 0
    '            End If
    '            If da.GetSetting("ChargeBeforeNoon") = "Y" Then
    '                If i = 0 Then 'only on first leg
    '                    If Hour(_leaves) + minutes / 60 < 12 Then
    '                        extrawaitcosts = perHourWaitTime * 4
    '                        pedetail = pedetail & "This includes extra wait time of departure before noon of " & Math.Round(perHourWaitTime * 4, 2) & vbCr & vbLf & vbCr & vbLf
    '                        oQuote.QuoteInfoArray("10-1|Extra Wait time of departure before noon", perHourWaitTime * 4)
    '                        _waittime = _waittime + perHourWaitTime * 4
    '                    End If
    '                End If
    '            End If
    '            If da.GetSetting("ChargeAfterNoon") = "Y" Then
    '                If i = dtflights.Rows.Count - 1 Then 'last row of itinerary
    '                    If Hour(_returns) > 12 Then
    '                        extrawaitcosts = extrawaitcosts + perHourWaitTime * 4
    '                        pedetail = pedetail & "This includes extra wait time of return after noon of " & Math.Round(perHourWaitTime * 4, 2) & vbCr & vbLf & vbCr & vbLf
    '                        oQuote.QuoteInfoArray("10-2|Extra Wait time of return after noon", perHourWaitTime * 4)
    '                        _waittime = _waittime + perHourWaitTime * 4
    '                    End If
    '                End If
    '            End If

    '            Dim pc, cr As Double
    '            '20100524 - pab - add new fees
    '            'pc = (days * (perDayCrewExpenses + perNightOvernight)) + extrawaitcosts
    '            If orig <> homebaseairportcode Then
    '                pc = (days * (PerDayCoPilotExpenses + perDayCrewExpenses + perDayCabinAideExpenses + perNightOvernight)) + extrawaitcosts
    '            Else
    '                pc = 0
    '            End If

    '            'chg3640 - 20101014 - pab - fix prevdest used before value assigned
    '            prevdest = orig

    '            cr = waitorreturn(prevdest, minuteswaitbetweensegments, _airSpeed, perHourRepositionTime, TaxiMinutes)

    '            '20101027 - fix waitcost detail display
    '            If i > 0 Then
    '                pedetail = pedetail & "Total Overnight Crew Costs are $" & Math.Round(pc, 2) & vbCr & vbLf
    '                If bChargeWaitTimeOvernight Then pedetail = pedetail & "Wait Costs are $" & Math.Round(waitcost, 2) & vbCr & vbLf
    '                pedetail = pedetail & "Return to base roundtrip is $" & Math.Round(cr, 0) & vbCr & vbLf
    '                'pedetail = pedetail & "Return to base roundtrip is $" & Math.Round(rtbcost, 0) & vbCr & vbLf
    '            End If

    '            oQuote.QuoteInfoArray("11-1|Overnight Crew Costs $", pc)
    '            If bChargeWaitTimeOvernight Then oQuote.QuoteInfoArray("11-1|Wait Costs $", waitcost)
    '            oQuote.QuoteInfoArray("12-1|Return to base roundtrip $", Math.Round(cr, 0))

    '            '20101027 - pab - make charging waitcosts on overnight flights configurable
    '            If i > 0 Then
    '                Dim pc2 As Double = 0
    '                If bChargeWaitTimeOvernight Then
    '                    pc2 = pc + waitcost
    '                Else
    '                    pc2 = pc
    '                End If
    '                'If cr > pc Then
    '                If cr > pc2 Then
    '                    pilotcost = pilotcost + pc
    '                    '20100524 - pab - add new fees
    '                    'pedetail = pedetail & "Overnight costs of " & (days * (perDayCrewExpenses + perNightOvernight)) & vbCr & vbLf
    '                    '20101027 - pab - make charging waitcosts on overnight flights configurable
    '                    If bChargeWaitTimeOvernight Then
    '                        pedetail = pedetail & "Overnight costs of $" & Math.Round((days * (perDayCrewExpenses + perDayCabinAideExpenses + perNightOvernight)) + waitcost, 2) & vbCr & vbLf
    '                    Else
    '                        pedetail = pedetail & "Overnight costs of S" & Math.Round((days * (perDayCrewExpenses + perDayCabinAideExpenses + perNightOvernight)), 2) & vbCr & vbLf
    '                    End If
    '                    pedetail = pedetail & "This includes " & days & " days of wait time " & vbCr & vbLf
    '                    pedetail = pedetail & "This includes $" & Math.Round(perDayCrewExpenses, 2) & " of per day crew expense " & vbCr & vbLf & vbCr & vbLf
    '                    '20100524 - pab - add new fees
    '                    '20101029 - pab - make ChargeCabinAide configurable
    '                    If days > 0 Then
    '                        If bChargeCabinAide Then pedetail = pedetail & "This includes $" & Math.Round(perDayCabinAideExpenses, 2) & " of per day cabin aide expense " & vbCr & vbLf & vbCr & vbLf
    '                        pedetail = pedetail & "This includes $" & Math.Round(perNightOvernight, 2) & " of per night overnight fees " & vbCr & vbLf & vbCr & vbLf & vbCr & vbLf
    '                        pedetail = pedetail & "This includes $" & Math.Round(PerDayCoPilotExpenses, 2) & " of per day co pilot expense " & vbCr & vbLf & vbCr & vbLf
    '                    End If
    '                    '20101027 - pab - make charging waitcosts on overnight flights configurable
    '                    If bChargeWaitTimeOvernight Then pedetail = pedetail & "This includes wait costs of $" & Math.Round(waitcost, 2) & vbCr & vbLf & vbCr & vbLf

    '                    oQuote.QuoteInfoArray("13-1|Overnight costs", (days * (perDayCrewExpenses + perDayCabinAideExpenses + perNightOvernight)))
    '                    oQuote.QuoteInfoArray("13-2|Includes (days wait time)", days)
    '                    oQuote.QuoteInfoArray("13-3|Includes (per day crew expense)", perDayCrewExpenses)
    '                    '20101029 - pab - make ChargeCabinAide configurable
    '                    If bChargeCabinAide Then oQuote.QuoteInfoArray("13-4|Includes (per day cabin aide expense)", perDayCabinAideExpenses)
    '                    oQuote.QuoteInfoArray("13-5|Includes (per night overnight)", perNightOvernight)
    '                    oQuote.QuoteInfoArray("13-6|Includes (per day copilot expenses)", PerDayCoPilotExpenses)


    '                    _crewovernight = (days * (perDayCrewExpenses + perDayCabinAideExpenses + perNightOvernight))

    '                Else



    '                    '20101029 - pab
    '                    'extrawaitcosts = 0
    '                    'pilotcost = pilotcost + cr
    '                    waitcost = 0
    '                    rtbcost = rtbcost + cr
    '                    pedetail = pedetail & "We will return to base instead of waiting for a cost of $" & Math.Round(cr, 0) & vbCr & vbLf & vbCr & vbLf

    '                    oQuote.QuoteInfoArray("14-1|We will return to base instead of waiting for a cost of", cr)

    '                    '20101103 - pab - fix pedetail
    '                    bRTB = True
    '                End If
    '            End If
    '        End If


    '        If sameday And i > 0 Then

    '            ' Dim hrs As Double = ((minutes - minutesinair) / 60) - fwh
    '            '   If hrs > 0 Then
    '            If orig <> homebaseairportcode Then
    '                waitcost = CLng((perHourWaitTime * (minuteswaitbetweensegments - (fwh * 60)) / 60))
    '                If waitcost < 0 Then waitcost = 0

    '            End If


    '            Dim cr As Double
    '            cr = waitorreturn(prevdest, minuteswaitbetweensegments, _airSpeed, perHourRepositionTime, TaxiMinutes)

    '            pedetail = pedetail & "Waitcost is $" & Math.Round(waitcost, 2) & vbCr & vbLf
    '            pedetail = pedetail & "Return to base roundtrip is $" & Math.Round(cr, 0) & vbCr & vbLf

    '            oQuote.QuoteInfoArray("15-1|Waitcost $", waitcost)
    '            oQuote.QuoteInfoArray("16-1|Return to base roundrip $", Math.Round(cr, 0))

    '            If cr > waitcost Then

    '                pedetail = pedetail & "Minutes between takeoff and return   " & minuteswaitbetweensegments & vbCr & vbLf

    '                pedetail = pedetail & "Flight Duration  " & minutesinair & vbCr & vbLf

    '                pedetail = pedetail & "Total Wait minutes as  " & minuteswaitbetweensegments & vbCr & vbLf
    '                ' " for " & d & " miles and " & ma & " minutes. " & vbCr & vbLf & vbCr & vbLf

    '                oQuote.QuoteInfoArray("17-1|-Minutes between takeoff and return", minuteswaitbetweensegments)
    '                oQuote.QuoteInfoArray("18-1|-Flight Duration", minutesinair)
    '                oQuote.QuoteInfoArray("19-1|Total Wat minutes", minuteswaitbetweensegments)

    '                '20101027 - pab - fix waitcost detail line
    '                rtbcost = 0
    '                _waittime = _waittime + waitcost

    '            Else

    '                '20101027 - pab - fix waitcost detail line
    '                'waitcost = cr
    '                '_waittime = _waittime + waitcost

    '                waitcost = 0
    '                rtbcost = rtbcost + cr  'rk 11.2.2010 RTB cost using _rtbcost

    '                pedetail = pedetail & "Return to base instead of waiting $" & Math.Round(cr, 0) & vbCr & vbLf & vbCr & vbLf & vbCr & vbLf
    '                oQuote.QuoteInfoArray("20-1|Return to base instead of waiting", Math.Round(cr, 0))

    '            End If
    '            'End If
    '        End If

    '        If minutesinair < MinFlightDuration Then
    '            '20081218 - pab - add additional fees
    '            'dr("Price") = Math.Round((3685 * (60 * (7 / 10) + 12) / 60), 2) + Math.Round((3385 * (ma) / 60), 2) + pilotcost '+ waitcost '* _passengers
    '            '20090223 - pab - change hardcoded values to configurable
    '            'dr("Price") = Math.Round((CostPerHourFlightTime * (60 * (7 / 10) + 12) / 60), 2) + _
    '            '    Math.Round((perHourRepositionTime * (ma) / 60), 2) + pilotcost + perCycleFee '+ waitcost '* _passengers
    '            'pedetail = pedetail & "We will fly from " & orig & " to " & dest & " at a cost of $" & Math.Round(CostPerHourFlightTime * (60 * 7 / 10 + 12) / 60, 2) + perCycleFee & _
    '            '    " for " & milesInAir & " miles and " & (60 * (7 / 10)) + 12 & " minutes and cycle fees of $" & perCycleFee & ". " & vbCr & vbLf & vbCr & vbLf

    '            '20101029 - pab
    '            '_price = CInt(Math.Round((CostPerHourFlightTime * (MinFlightDuration + TaxiMinutes) / 60), 2) + _
    '            '    CInt(Math.Round((perHourRepositionTime * (ma + ma1) / 60), 2)) + pilotcost + perCycleFee) + _
    '            '    CInt(waitcost) + fees
    '            '20101103 - pab - fix rtb
    '            If bRTB = True Then
    '                pilotcost = 0
    '            End If
    '            _price = CInt(Math.Round((CostPerHourFlightTime * (MinFlightDuration + TaxiMinutes) / 60), 2) + _
    '                CInt(Math.Round((perHourRepositionTime * (ma + ma1) / 60), 2)) + CInt(rtbcost) + pilotcost + perCycleFee) + fees + _internationalfees + FuelSurcharge

    '            'rk 11/1/2010 update per line information
    '            flightcharge = flightcharge + CInt(Math.Round((CostPerHourFlightTime * (MinFlightDuration + TaxiMinutes) / 60)))
    '            dtflights.Rows(i).Item("price") = Format(CInt(Math.Round((CostPerHourFlightTime * (MinFlightDuration + TaxiMinutes) / 60))), "$##,##0.00")



    '            If bChargeWaitTimeOvernight Or sameday Then
    '                _price = _price + CInt(waitcost)
    '                TotalWaitTime = TotalWaitTime + CInt(waitcost)
    '            End If

    '            '20101027 - pab - fix totals on grid
    '            TotalAddCharges = TotalAddCharges + CInt(Math.Round((perHourRepositionTime * (ma + ma1) / 60), 2)) + perCycleFee + Math.Round(rtbcost, 2)
    '            TotalCrewOvernight = TotalCrewOvernight + pilotcost
    '            TotalLandingFees = TotalLandingFees + fees
    '            GrandTotalPrice = GrandTotalPrice + _price

    '            '20101027 - pab - fix waitcost detail line - add to bucket when used in price calculation
    '            _price = _price + extrawaitcosts
    '            _waittime = _waittime + waitcost

    '            '               dr("Price") = Format(_price, "##,##0.00") '* _passengers
    '            If dest <> homebaseairportcode Then
    '                _price = _price + AwayLandingFee
    '                pedetail = pedetail & "This flight has an away landing fee of $" & Math.Round(AwayLandingFee, 2) & "." & vbCr & vbLf
    '                _landingfees = _landingfees + AwayLandingFee
    '                oQuote.QuoteInfoArray("21-1|Flight away landing fee", AwayLandingFee)
    '                '20101027 - pab - fix totals on grid
    '                GrandTotalPrice = GrandTotalPrice + AwayLandingFee
    '                TotalLandingFees = TotalLandingFees + AwayLandingFee
    '            End If


    '            If sameday And i = 1 Then 'only add per day copilot to one leg if same day
    '                If PerDayCoPilotExpenses <> 0 Then
    '                    pedetail = pedetail & "This flight has copilot costs of $" & Math.Round(PerDayCoPilotExpenses, 2) & "." & vbCr & vbLf
    '                    _price = _price + PerDayCoPilotExpenses

    '                    oQuote.QuoteInfoArray("22-1|Copilot costs of", PerDayCoPilotExpenses)
    '                    '20101027 - pab - fix totals on grid
    '                    GrandTotalPrice = GrandTotalPrice + PerDayCoPilotExpenses
    '                    TotalCrewOvernight = TotalCrewOvernight + PerDayCoPilotExpenses
    '                End If
    '            End If

    '            '  totalprice = totalprice + _price

    '            'pedetail = pedetail & "We will fly from " & dr("Origin").ToString & " to " & dr("Destination").ToString & " at a cost of $" & Math.Round((CostPerHourFlightTime * (MinFlightDuration + TaxiMinutes) / 60) + perCycleFee + CInt(waitcost), 2) & _
    '            '    " for " & milesInAir & " miles and the minimum " & MinFlightDuration + TaxiMinutes & " minutes. " & vbCr & vbLf
    '            '20101111 - pab - fix price on pedetail for minimum flight duration
    '            'pedetail = pedetail & "We will fly from " & dr("Origin").ToString & " to " & dr("Destination").ToString & " at a cost of $" & Math.Round((CostPerHourFlightTime * (MinFlightDuration + TaxiMinutes) / 60) + CInt(waitcost), 2) & _
    '            '    " for " & milesInAir & " miles and the minimum " & MinFlightDuration + TaxiMinutes & " minutes. " & vbCr & vbLf
    '            pedetail = pedetail & "We will fly from " & dr("Origin").ToString & " to " & dr("Destination").ToString & " at a cost of $" & _
    '                Math.Round((CostPerHourFlightTime * (MinFlightDuration + TaxiMinutes) / 60), 2) & _
    '                " for " & milesInAir & " miles and the minimum " & MinFlightDuration + TaxiMinutes & " minutes. " & vbCr & vbLf

    '            '20101111 - pab - fix price on pedetail for minimum flight duration
    '            ''20101029 - pab
    '            'If waitcost <> 0 Then
    '            '    If bChargeWaitTimeOvernight Or sameday Then
    '            '        pedetail &= "This includes wait time fees of $" & waitcost & "." & vbCr & vbLf
    '            '    End If
    '            'End If
    '            'If perCycleFee <> 0 Then pedetail &= "This includes cycle fees of $" & perCycleFee & "." & vbCr & vbLf
    '            If perCycleFee <> 0 Then pedetail &= "Cycle fees are $" & perCycleFee & "." & vbCr & vbLf
    '            pedetail &= vbCr & vbLf


    '            If FuelSurcharge <> 0 Then pedetail &= "This includes a fuel surcharge of $" & FuelSurcharge & "." & vbCr & vbLf


    '            oQuote.QuoteInfoArray("23-1|Fly from" & dr("Origin").ToString & " to " & dr("Destination").ToString & " at a cost of $", Math.Round((CostPerHourFlightTime * (MinFlightDuration + TaxiMinutes) / 60) + perCycleFee + CInt(waitcost), 2))
    '            oQuote.QuoteInfoArray("23-2|Miles in Air (miles)", milesInAir)
    '            oQuote.QuoteInfoArray("23-3|Minimum (minutes)", MinFlightDuration + TaxiMinutes)
    '            oQuote.QuoteInfoArray("23-4|Waitcost", waitcost)
    '            oQuote.QuoteInfoArray("23-5|Cycle Fee", perCycleFee)
    '        Else

    '            '20100614 - taxi minutes already included in price calculation ... in travel time and taken from grid
    '            'taxi minutes already in price.
    '            '20101027 - pab - fix waitcost detail line 
    '            ' _price = CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60), 2) + _
    '            'CInt(Math.Round((perHourRepositionTime * (ma + ma1) / 60), 2)) + pilotcost + perCycleFee) + _
    '            'CInt(waitcost) + fees

    '            '20101029 - pab
    '            '_price = CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60), 2) + _
    '            '   CInt(Math.Round((perHourRepositionTime * (ma + ma1) / 60), 2)) + pilotcost + perCycleFee) + _
    '            '   CInt(waitcost) + CInt(rtbcost) + fees + extrawaitcosts
    '            _price = CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60), 2) + _
    '                CInt(Math.Round((perHourRepositionTime * (ma + ma1) / 60), 2)) + pilotcost + perCycleFee) + _
    '                CInt(rtbcost) + fees + extrawaitcosts + _internationalfees + FuelSurcharge  'rk 11/5/2010 add fuel surcharge on both sides
    '            If bChargeWaitTimeOvernight Or sameday Then
    '                _price = _price + waitcost
    '                TotalWaitTime = TotalWaitTime + waitcost
    '            End If

    '            'rk 11/1/2010 update per line information
    '            flightcharge = flightcharge + CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60)))
    '            dtflights.Rows(i).Item("price") = Format(CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60))), "$##,##0.00")


    '            '20101027 - pab - fix totals on grid
    '            TotalAddCharges = TotalAddCharges + CInt(Math.Round((perHourRepositionTime * (ma + ma1) / 60), 2)) + perCycleFee + CInt(rtbcost)
    '            TotalCrewOvernight = TotalCrewOvernight + pilotcost
    '            TotalWaitTime = TotalWaitTime + extrawaitcosts
    '            TotalLandingFees = TotalLandingFees + fees
    '            GrandTotalPrice = GrandTotalPrice + _price

    '            '                dr("Price") = Format(_price, "##,##0.00")

    '            '               dr("Price") = Format(_price, "##,##0.00") '* _passengers
    '            If dest <> homebaseairportcode Then
    '                _price = _price + AwayLandingFee
    '                _landingfees = _landingfees + AwayLandingFee
    '                pedetail = pedetail & "This flight has an away landing fee of $" & Math.Round(AwayLandingFee, 2) & "." & vbCr & vbLf
    '                oQuote.QuoteInfoArray("24-1|Flight Away Lander Fee", AwayLandingFee)
    '                '20101027 - pab - fix totals on grid
    '                GrandTotalPrice = GrandTotalPrice + AwayLandingFee
    '                TotalLandingFees = TotalLandingFees + AwayLandingFee
    '            End If

    '            If sameday And i = 1 Then 'only add per day copilot to one leg if same day
    '                If PerDayCoPilotExpenses <> 0 Then
    '                    pedetail = pedetail & "This flight has copilot costs of $" & Math.Round(PerDayCoPilotExpenses, 2) & "." & vbCr & vbLf
    '                    oQuote.QuoteInfoArray("25-1|Copilot costs of", PerDayCoPilotExpenses)
    '                    _price = _price + PerDayCoPilotExpenses
    '                    '20101027 - pab - fix totals on grid
    '                    GrandTotalPrice = GrandTotalPrice + PerDayCoPilotExpenses
    '                    TotalCrewOvernight = TotalCrewOvernight + fees
    '                End If
    '            End If

    '            '    totalprice = totalprice + _price

    '            pedetail = pedetail & "This flight will take approximately " & minutesinair & " minutes." & vbCr & vbLf
    '            pedetail = pedetail & "This flight time is charged at $" & Math.Round((CostPerHourFlightTime * (minutesinair) / 60), 2) & vbCr & vbLf
    '            pedetail = pedetail & "This includes taxi minutes of  " & TaxiMinutes & vbCr & vbLf

    '            '20101111 - pab - fix price on pedetail for minimum flight duration
    '            'pedetail = pedetail & "This includes cycle fees of $" & Math.Round(perCycleFee, 2) & "." & vbCr & vbLf
    '            pedetail = pedetail & "Cycle fees are $" & Math.Round(perCycleFee, 2) & "." & vbCr & vbLf
    '            '  pedetail = pedetail & "We will fly this flight from " & orig & " to " & dest & " at a cost of $" & CStr(dr("Price")) & vbCr & vbLf & vbCr & vbLf


    '            oQuote.QuoteInfoArray("26-1|Flight Time", minutesinair)
    '            oQuote.QuoteInfoArray("26-2|Flight Charge", CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60))))
    '            oQuote.QuoteInfoArray("26-3|Includes Taxi Minutes", TaxiMinutes)
    '            oQuote.QuoteInfoArray("26-4|Cycle Fee", perCycleFee)


    '            flightcharge = flightcharge + CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60)))
    '            dtflights.Rows(i).Item("price") = Format(CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60))), "$##,##0.00")

    '            '20090223 - pab - change hardcoded values to configurable
    '            'pedetail = pedetail & "We will fly from " & orig & " to " & dest & " at a cost of $" & Math.Round(CostPerHourFlightTime * (minutesInAir + 12) / 60, 2) + perCycleFee & _
    '            '    " for " & milesInAir & " miles and " & minutesInAir + 12 & " minutes and cycle fees of $" & perCycleFee & ". " & vbCr & vbLf & vbCr & vbLf
    '            'pedetail = pedetail & "We will fly from " & dr("Origin").ToString & " to " & dr("Destination").ToString & " at a cost of $" & CInt(Math.Round(CostPerHourFlightTime * (minutesinair + TaxiMinutes) / 60, 2)) + perCycleFee & _
    '            '    " for " & milesInAir & " miles and " & minutesinair + TaxiMinutes & " minutes. " & vbCr & vbLf

    '            '20101029 - pab
    '            If waitcost <> 0 Then
    '                If bChargeWaitTimeOvernight Or sameday Then
    '                    pedetail &= "This includes wait time fees of $" & waitcost & "." & vbCr & vbLf
    '                End If
    '            End If


    '            If FuelSurcharge <> 0 Then pedetail &= "This includes a fuel surcharge of $" & FuelSurcharge & "." & vbCr & vbLf

    '            '20101027 - pab - fix waitcost detail line 
    '            If rtbcost <> 0 Then pedetail &= "This includes return to base fees of $" & Math.Round(rtbcost, 0) & "." & vbCr & vbLf
    '            '20101027 - pab - cycle fees already written to pedetail
    '            'If perCycleFee <> 0 Then pedetail &= "This includes cycle fees of $" & perCycleFee & "." & vbCr & vbLf
    '            pedetail &= vbCr & vbLf
    '            oQuote.QuoteInfoArray("26-5|Wait Cost", waitcost)
    '            oQuote.QuoteInfoArray("26-6|Cycle Fee", perCycleFee)
    '            oQuote.QuoteInfoArray("26-6|RTB Fee", rtbcost)
    '            oQuote.QuoteInfoArray("26-6|extrawaitcosts", extrawaitcosts)
    '        End If


    '        'If ma1 <> 0 And perHourRepositionTime <> 0 Then
    '        '    pedetail = pedetail & "We will reposition this flight from " & dr("Destination").ToString & " to " & closest_base & "  at a cost of $" & CInt(Math.Round((ma1 * perHourRepositionTime / 60), 2)) & _
    '        '        " for " & d & " miles and " & ma1 & " minutes. The cycle fees for repositioning are $" & perCycleFee & ". " & vbCr & vbLf & vbCr & vbLf
    '        'End If

    '        If pilotcost > 0 Then
    '            pedetail = pedetail & "The additional costs for pilots' expenses are $" & Math.Round(pilotcost, 2) & vbCr & vbLf & vbCr & vbLf
    '            oQuote.QuoteInfoArray("27-1|Additonal costs for pilots expenses", pilotcost)
    '        End If
    '        pedetail = pedetail & "The total for this flight is $" & Math.Round(_price, 2) & "." & vbCr & vbLf & vbCr & vbLf
    '        oQuote.QuoteInfoArray("28-1|Total for Flight", _price)



    '        'rk 7.13.2010 only charge if FET flag set to yes per customer
    '        'rk 8.16.2010 charge FET per passenger
    '        'da.GetSetting("MinAircraftWeightForFET")
    '        '20101027 - pab - make fet aircraft weight configurable
    '        'If TotalWeighT > 6000 Then 'rk 10.18.2010 only apply fet if weight > 6000
    '        If da.GetSetting("ChargeFET") = "Y" Then
    '            If TotalWeighT > CInt(da.GetSetting("MinAircraftWeightForFET")) Then
    '                '20100614 - add FET and segment fees
    '                'chg3631 - 20100929 - pab - FET for domestic flights only

    '                _segmentfee = _segmentfee + (7.4 / 2) * _passengers

    '                If Not bIntl Then

    '                    pedetail = pedetail & "Add Passenger FET of $" & Math.Round(_price * 0.075, 2) & "." & vbCr & vbLf & vbCr & vbLf
    '                    pedetail = pedetail & "Add FET Segment Fees of $" & Math.Round((7.4 / 2) * _passengers, 2) & " for " & _passengers & " passengers." & vbCr & vbLf & vbCr & vbLf
    '                    '  _price = _price + _price * 0.075 + 7.4 / 2

    '                    '20101027 - pab - fix totals on grid
    '                    Dim tax As Double = _price * 0.075 + (7.4 / 2) * _passengers

    '                    _taxes = _taxes + _price * 0.075 + (7.4 / 2) * _passengers

    '                    '      totalprice = totalprice + _price * 0.075 + (7.4 / 2) * _passengers
    '                    _price = _price + _price * 0.075 + (7.4 / 2) * _passengers
    '                    '   dr("price") = i


    '                    '20101027 - pab - fix totals on grid
    '                    GrandTotalPrice = GrandTotalPrice + tax
    '                    TotalTaxes = TotalTaxes + tax


    '                    oQuote.QuoteInfoArray("29-1|Passenger FET", _price * 0.075)
    '                    oQuote.QuoteInfoArray("29-2|FET Segment Fees", (7.4 / 2) * _passengers)

    '                Else

    '                    'chg3631 - 20100929 - pab - FET for domestic flights only
    '                    oQuote.QuoteInfoArray("29-1|Passenger FET", 0)
    '                    oQuote.QuoteInfoArray("29-2|FET Segment Fees", 0)

    '                End If

    '            End If
    '        End If
    '        'End If



    '        'If TotalWeighT > 6000 Then  '-- ~DHA changed to 6000 will need to double check...
    '        '    '20100614 - add FET and segment fees
    '        '    pedetail = pedetail & "Add Passenger FET of $" & _price * 0.075 & "." & vbCr & vbLf & vbCr & vbLf
    '        '    pedetail = pedetail & "Add FET Segment Fees of $" & 7.4 / 2 & "." & vbCr & vbLf & vbCr & vbLf
    '        '    '  _price = _price + _price * 0.075 + 7.4 / 2
    '        '    totalprice = totalprice + _price * 0.075 + 7.4 / 2
    '        '    _price = _price + _price * 0.075 + 7.4 / 2
    '        '    '   dr("price") = i

    '        '    oQuote.QuoteInfoArray("29-1|Passenger FET", _price * 0.075)
    '        '    oQuote.QuoteInfoArray("29-2|FET Segment Fees", 7.4 / 2)

    '        'End If



    '        'rk 10/11/2010 use a price override function
    '        Dim req2 As String
    '        Dim rs2 As New ADODB.Recordset

    '        req2 = "SELECT * "
    '        req2 = req2 & "FROM flights WHERE departureairport = '123' and arrivalairport = '456' and flighttype = 'D' and allowsale = 'true' "
    '        req2 = Replace(req2, "123", orig)
    '        req2 = Replace(req2, "456", dest)
    '        req2 = Replace(req2, "789", currtype)
    '        rs2.Open(req2, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

    '        Dim holdprice As Decimal
    '        Dim foskey As String = ""
    '        Dim overrideprice As Boolean = False

    '        Dim aircraftidflights As Integer
    '        Dim servicetypeaircraft As Integer

    '        dtflights.Rows(i).Item("EmptyLeg") = 0
    '        If Not (rs2.EOF) Then
    '            If rs2.Fields("price").Value <> 0 Then
    '                '  totalprice = rs2.Fields("price").Value

    '                dtflights.Rows(i).Item("EmptyLeg") = rs2.Fields("id").Value
    '                holdprice = rs2.Fields("price").Value
    '                foskey = rs2.Fields("modelrun").Value 'legacy field from flights table used for math modeling - indicates where flight is written from
    '                'overrideprice = True
    '                'pedetail = pedetail & "Price Override For This Flight $" & _price & "." & vbCr & vbLf & vbCr & vbLf
    '                'priceoverride = True

    '                aircraftidflights = rs2.Fields("AircraftID").Value

    '            End If
    '        End If
    '        If rs2.State = 1 Then rs2.Close()


    '        'rk 11/9/2010 restrict flights to those that match currtype
    '        req2 = "SELECT * "
    '        req2 = req2 & "FROM aircraft WHERE id = '" & aircraftidflights & "'"
    '        rs2.Open(req2, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

    '        servicetypeaircraft = 0
    '        If Not (rs2.EOF) Then
    '            servicetypeaircraft = rs2.Fields("AircraftTypeServiceSpecID").Value
    '        End If
    '        If rs2.State = 1 Then rs2.Close()

    '        Dim dhdatebegins, dhdateends As Date
    '        If currtype = servicetypeaircraft Then

    '            req2 = "SELECT * "
    '            req2 = req2 & "FROM FOSflights WHERE foskey = '" & foskey & "'"
    '            rs2.Open(req2, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


    '            If Not (rs2.EOF) Then

    '                dhdatebegins = CDate("1/1/2030")
    '                dhdateends = CDate("1/2/2030")

    '                'use the flight date, a full 24 hour window, if no dh information
    '                If rs2.Fields("departuredate").Value <> "" Then
    '                    If IsDate(rs2.Fields("departuredate").Value) Then
    '                        dhdatebegins = CDate(rs2.Fields("departuredate").Value & " 00:00")
    '                        dhdateends = CDate(rs2.Fields("departuredate").Value & " 23:59")
    '                    End If
    '                End If

    '                Dim skipdh As Boolean = False
    '                If IsDBNull(rs2.Fields("dhdatebegin").Value) Then skipdh = True

    '                If Not IsDBNull(rs2.Fields("dhdatebegin").Value) Then
    '                    If IsDate(rs2.Fields("dhdatebegin").Value) Then
    '                        If CDate(rs2.Fields("dhdatebegin").Value) < CDate("12/01/2009") Then
    '                            skipdh = True
    '                        End If
    '                    End If
    '                End If



    '                If Not skipdh Then
    '                    'use the large dh window from FOS if valid
    '                    If Not IsDBNull(rs2.Fields("dhdatebegin").Value) Then
    '                        If rs2.Fields("dhdatebegin").Value <> "" Then
    '                            If IsDate(rs2.Fields("dhdatebegin").Value & " " & rs2.Fields("dhFlexBegin").Value) Then dhdatebegins = rs2.Fields("dhdatebegin").Value & " " & rs2.Fields("dhFlexBegin").Value
    '                            If IsDate(rs2.Fields("dhdateend").Value & " " & rs2.Fields("dhFlexEnd").Value) Then dhdateends = rs2.Fields("dhdateend").Value & " " & rs2.Fields("dhFlexEnd").Value
    '                        End If
    '                    End If
    '                End If



    '                If _leaves > dhdatebegins And _leaves < dhdateends Then

    '                    overrideprice = True
    '                    _price = holdprice
    '                    pedetail = pedetail & "Price Override For This Flight $" & _price & "." & vbCr & vbLf & vbCr & vbLf
    '                    priceoverride = True


    '                    flightcharge = flightcharge + CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60)))
    '                    dtflights.Rows(i).Item("price") = Format(CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60))), "$##,##0.00")


    '                    If da.GetSetting("ShowSpecialPricingPerLeg") = "Y" Then
    '                        dtflights.Rows(i).Item("price") = "Empty Leg Special " & Format(CInt(Math.Round((_price))), "$##,##0.00")
    '                    Else
    '                        Me.lblMsg.Text = "Empty Leg Special " & Format(CInt(Math.Round((_price))), "$##,##0.00")
    '                        dtflights.Rows(i).Item("price") = Format(CInt(Math.Round((_price))), "$##,##0.00") 'rk 11/9/2010 show new pricing detail
    '                    End If



    '                Else
    '                    dtflights.Rows(i).Item("EmptyLeg") = 0 'not a price override based on empty leg
    '                End If



    '            End If


    '        End If 'make sure same service type

    '        If rs2.State = 1 Then rs2.Close()

    '        'rk 11/1/2010 add per leg flight charges
    '        'flightcharge = flightcharge + CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60)))
    '        'dtflights.Rows(i).Item("price") = Format(CInt(Math.Round((CostPerHourFlightTime * (minutesinair) / 60))), "$##,##0.00")



    '        'rk 9/30/2010 use a price override function

    '        req2 = "SELECT * "
    '        req2 = req2 & "FROM citypairspecials WHERE DepartureAirport = '123' and ArrivalAirport = '456' and AircraftServiceType = '789' "
    '        req2 = Replace(req2, "123", orig)
    '        req2 = Replace(req2, "456", dest)
    '        req2 = Replace(req2, "789", currtype)
    '        rs2.Open(req2, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

    '        dhdatebegins = CDate("1/1/2030")
    '        dhdateends = CDate("1/2/2030")



    '        Do While Not rs2.EOF

    '            If rs2.Fields("price").Value <> 0 Then

    '                'use the flight date, a full 24 hour window, if no dh information
    '                If IsDate(rs2.Fields("datebegin").Value) Then
    '                    If IsDate(rs2.Fields("dateend").Value) Then
    '                        dhdatebegins = CDate(rs2.Fields("datebegin").Value & " 00:00")
    '                        dhdateends = CDate(rs2.Fields("dateend").Value & " 23:59")
    '                    End If
    '                End If


    '                If _leaves > dhdatebegins And _leaves < dhdateends Then
    '                    '   totalprice = rs2.Fields("price").Value
    '                    _price = rs2.Fields("price").Value

    '                    pedetail = pedetail & "Price Override For This Flight $" & _price & "." & vbCr & vbLf & vbCr & vbLf
    '                    priceoverride = True

    '                    'rk 11/9/2010 special pricing on City Pair labeling
    '                    If da.GetSetting("ShowSpecialPricingPerLeg") = "Y" Then
    '                        dtflights.Rows(i).Item("price") = Format(CInt(Math.Round((_price))), "$##,##0.00")
    '                    Else
    '                        dtflights.Rows(i).Item("price") = ""
    '                    End If

    '                End If
    '                rs2.MoveNext()


    '            End If

    '        Loop

    '        If rs2.State = 1 Then rs2.Close()


    '        If da.GetSetting("CheckBothDirections") = "Y" Then

    '            req2 = "SELECT * "
    '            req2 = req2 & "FROM citypairspecials WHERE DepartureAirport = '123' and ArrivalAirport = '456' and AircraftServiceType = '789' "
    '            req2 = Replace(req2, "123", dest)
    '            req2 = Replace(req2, "456", orig)
    '            req2 = Replace(req2, "789", currtype)
    '            rs2.Open(req2, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

    '            Do While Not rs2.EOF

    '                If rs2.Fields("price").Value <> 0 Then

    '                    'use the flight date, a full 24 hour window, if no dh information
    '                    If IsDate(rs2.Fields("datebegin").Value) Then
    '                        If IsDate(rs2.Fields("dateend").Value) Then
    '                            dhdatebegins = CDate(rs2.Fields("datebegin").Value & " 00:00")
    '                            dhdateends = CDate(rs2.Fields("dateend").Value & " 23:59")
    '                        End If
    '                    End If


    '                    If _leaves > dhdatebegins And _leaves < dhdateends Then
    '                        '   totalprice = rs2.Fields("price").Value
    '                        _price = rs2.Fields("price").Value

    '                        pedetail = pedetail & "Price Override For This Flight $" & _price & "." & vbCr & vbLf & vbCr & vbLf
    '                        priceoverride = True

    '                        'rk 11/9/2010 special pricing on City Pair labeling
    '                        If da.GetSetting("ShowSpecialPricingPerLeg") = "Y" Then
    '                            dtflights.Rows(i).Item("price") = Format(CInt(Math.Round((_price))), "$##,##0.00")
    '                        Else
    '                            dtflights.Rows(i).Item("price") = ""
    '                        End If

    '                    End If
    '                    rs2.MoveNext()


    '                End If

    '            Loop



    '            If rs2.State = 1 Then rs2.Close()
    '        End If


    '        TotalFuelSurcharges = TotalFuelSurcharges + FuelSurcharge 'rk 11.4/2010 add total fuel surcharges
    '        totalprice = totalprice + _price



    '        '20090609 - rlk - record price data
    '        ' recordquote(origin, dest, _price, outret, _price, pedetail, "Portal", "")
    '        recordquote(orig, dest, _price, "R", _price, pedetail, da.GetSetting("CompanyName"), "", CStr(dr("Departs")), da.GetSetting("CompanyName"), ip)
    '        oQuote.InsertQuote()



    '        '20100111 - pab - call optimization
    '        Dim s As String = "0"
    '        If ConfigurationManager.AppSettings("Optimize") = "Y" Then
    '            AirTaxi.post_timing("Optimize Start" & Now.ToString)

    '            Dim ws As New optimizesmall.Service

    '            'rlk 11/1/2010 trap an optimizer failure
    '            Try
    '                s = ws.OptimizeCASWebSmall("N", airplaneid, orig, dest, departure, arrival, "R", 0, ConfigurationManager.AppSettings("optimizerDatabase"))
    '                If s <> "0" Then
    '                    optimizerfail = True
    '                    optimizerwarning = s
    '                End If
    '                AirTaxi.post_timing("Optimize End" & Now.ToString)
    '            Catch ex As Exception
    '                optimizerfail = True
    '                optimizerwarning = "Optimizer Failed at " & Now
    '                AirTaxi.post_timing("Optimize Failed" & Now.ToString)
    '                optmizerfailmsg = "OPT1"
    '                SendEmail("rkane@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", "Optimizer Error 1", ex.Message)
    '                SendEmail("pbaumgart@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", "Optimizer Error 1", ex.Message)
    '                SendEmail("pbaumgart@coastalaviationsoftware.com", "5612397068@txt.att.net", "Optimizer Error 1", ex.Message)

    '            End Try



    '        End If




    '    Next

    '    'rk 11/3/2010 make continue to quote at capacity specific to model being tested.
    '    ''20100111 - pab - call optimization - nothing available
    '    'Dim da1 As New DataAccess
    '    'Dim bQuoteAtCapacity As Boolean = da1.QuoteAtCapacity

    '    If optimizerfail And Not bQuoteAtCapacity Then
    '        totalprice = 0
    '        Dim cea As New EmailAgent
    '        Dim emailSubject As String = "Capacity Warning "
    '        Dim emailBody As String = "Capacity Warning generated when generating quote." & vbCr & vbLf
    '        emailBody = emailBody & "We are at capacity on " & optimizerwarning & "." & vbCr & vbLf
    '        cea.SendEmail(ConfigurationManager.AppSettings("capacityalert"), "rkane@coastalaviationsoftware.com;pbaumgart@coastalaviationsoftware.com;supportquotes@kmjunlimited.com", ConfigurationManager.AppSettings("capacityalert"), emailSubject, emailBody, Nothing, True)
    '    End If


    '    Me.TxtQuoteDetail.Text = distance_text & pedetail
    '    globaldetail = distance_text & pedetail

    '    AirTaxi.post_timing("Email Start" & Now.ToString)


    '    SendEmail("info@coastalaviationsoftware.com", "casquoter@coastalaviationsoftware.com", "Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), pedetail)

    '    SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), pedetail)

    '    SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", "Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), pedetail)


    '    AirTaxi.post_timing("Email End" & Now.ToString)



    '    AirTaxi.post_timing("Twitter Start" & Now.ToString)


    '    Dim user, pw As String

    '    Dim owrt As String
    '    If Session("triptype") = "R" Then owrt = "Round Trip"
    '    If Session("triptype") = "O" Then owrt = "One Way"

    '    Try
    '        'twitter
    '        user = "CASUpdates"
    '        pw = "n621kf"
    '        Dim tweet As New Twitterizer.Framework.Twitter(user, pw)
    '        Dim stattweet As String = ("Fly " & owrt & " from " & fname(orig) & " To " & fname(dest) & " for " & Format(totalprice / TotalPassengers, "$##,##0.00"))
    '        If Len(stattweet) > 144 Then stattweet = Left(stattweet, 144)
    '        tweet.Status.Update(stattweet)
    '    Catch
    '        'oh well tweet off
    '    End Try


    '    Try
    '        'twitter
    '        user = "PrivateAirFares"
    '        pw = "n621kf"
    '        Dim tweet As New Twitterizer.Framework.Twitter(user, pw)
    '        Dim stattweet As String = ("Fly " & owrt & " from " & fname(orig) & " To " & fname(dest) & " for " & Format(totalprice / TotalPassengers, "$##,##0.00"))
    '        If Len(stattweet) > 144 Then stattweet = Left(stattweet, 144)
    '        tweet.Status.Update(stattweet)
    '    Catch
    '        'oh well tweet off
    '    End Try


    '    AirTaxi.post_timing("Twitter End" & Now.ToString)


    '    ' Session("PlanType") = rs.Fields("PlanType").Value
    '    'Session("FirstName") = rs.Fields("PlanType").Value

    '    Dim discount As Decimal = 0
    '    If UCase(Session("PlanType")) = "SILVER" Then
    '        discount = totalprice * 0.05
    '        totalprice = totalprice - discount
    '    End If

    '    _totalprice = totalprice

    '    If Not (optimizerfail And Not bQuoteAtCapacity) Then



    '        If da.GetSetting("ShowPriceDetail") = "Y" And priceoverride = False Then


    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("Destination") = ""
    '            dr("OriginFacilityName") = ""
    '            dr("DestinationFacilityName") = "Wait Time:"
    '            '20101104 - pab - remove hardcoded value
    '            'dr("Service Provider") = "Portal"
    '            dr("Service Provider") = da.GetSetting("CompanyName")
    '            'departure time
    '            dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '            '20081218 - pab - add taxi time
    '            'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '            dr("Arrives") = "" '"<strong>Total: " & Format(totalprice, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("minutes") = minutesinair
    '            '20101027 - pab - fix totals on grid
    '            'dr("price") = "" & Format(_waittime, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("price") = "" & Format(TotalWaitTime, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dtflights.Rows.Add(dr)


    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("Destination") = ""
    '            dr("OriginFacilityName") = ""
    '            dr("DestinationFacilityName") = "Landing Fees:"
    '            '20101104 - pab - remove hardcoded value
    '            'dr("Service Provider") = "Portal"
    '            dr("Service Provider") = da.GetSetting("CompanyName")
    '            'departure time
    '            dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '            '20081218 - pab - add taxi time
    '            'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '            dr("Arrives") = "" '"<strong>Total: " & Format(totalprice, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("minutes") = minutesinair
    '            '20101027 - pab - fix totals on grid = intl fees part of landing fees so subtract out here
    '            'dr("price") = Format(_landingfees, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            '   dr("price") = Format(TotalLandingFees - TotalIntlFees, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)

    '            'rk 11/1/2010 landing fees go negative if subtracting TotalIntlFees
    '            dr("price") = Format(TotalLandingFees, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)

    '            dtflights.Rows.Add(dr)



    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("Destination") = ""
    '            dr("OriginFacilityName") = ""
    '            dr("DestinationFacilityName") = "International Fees:"
    '            '20101104 - pab - remove hardcoded value
    '            'dr("Service Provider") = "Portal"
    '            dr("Service Provider") = da.GetSetting("CompanyName")
    '            'departure time
    '            dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '            '20081218 - pab - add taxi time
    '            'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '            dr("Arrives") = "" '"<strong>Total: " & Format(totalprice, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("minutes") = minutesinair
    '            '20101027 - pab - fix totals on grid
    '            'dr("price") = Format(_internationalfees, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("price") = Format(TotalIntlFees, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dtflights.Rows.Add(dr)




    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("Destination") = ""
    '            dr("OriginFacilityName") = ""
    '            dr("DestinationFacilityName") = "Fuel Surcharges:"
    '            '20101104 - pab - remove hardcoded value
    '            'dr("Service Provider") = "Portal"
    '            dr("Service Provider") = da.GetSetting("CompanyName")
    '            'departure time
    '            dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '            '20081218 - pab - add taxi time
    '            'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '            dr("Arrives") = "" '"<strong>Total: " & Format(totalprice, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("minutes") = minutesinair
    '            '20101027 - pab - fix totals on grid
    '            'dr("price") = Format(_fuelsurcharges, "$##,##0.00")   '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("price") = Format(TotalFuelSurcharges, "$##,##0.00")   '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dtflights.Rows.Add(dr)





    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("Destination") = ""
    '            dr("OriginFacilityName") = ""
    '            dr("DestinationFacilityName") = "Crew Overnight:"
    '            '20101104 - pab - remove hardcoded value
    '            'dr("Service Provider") = "Portal"
    '            dr("Service Provider") = da.GetSetting("CompanyName")
    '            'departure time
    '            dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '            '20081218 - pab - add taxi time
    '            'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '            dr("Arrives") = "" '"<strong>Total: " & Format(totalprice, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("minutes") = minutesinair
    '            '20101027 - pab - fix totals on grid
    '            'dr("price") = Format(_crewovernight, "$##,##0.00")   '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("price") = Format(TotalCrewOvernight, "$##,##0.00")   '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dtflights.Rows.Add(dr)



    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("Destination") = ""
    '            dr("OriginFacilityName") = ""
    '            dr("DestinationFacilityName") = "Additional Charges:"
    '            '20101104 - pab - remove hardcoded value
    '            'dr("Service Provider") = "Portal"
    '            dr("Service Provider") = da.GetSetting("CompanyName")
    '            'departure time
    '            dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '            '20081218 - pab - add taxi time
    '            'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '            dr("Arrives") = "" '"<strong>Total: " & Format(totalprice, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("minutes") = minutesinair
    '            '20101027 - pab - fix totals on grid
    '            'dr("price") = Format(totalprice - flightcharge - _taxes - _landingfees - _waittime - _crewovernight - _fuelsurcharges - _internationalfees, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("price") = Format(TotalAddCharges, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dtflights.Rows.Add(dr)


    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("Destination") = ""
    '            dr("OriginFacilityName") = ""
    '            dr("DestinationFacilityName") = "Taxes:"
    '            '20101104 - pab - remove hardcoded value
    '            'dr("Service Provider") = "Portal"
    '            dr("Service Provider") = da.GetSetting("CompanyName")
    '            'departure time
    '            dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '            '20081218 - pab - add taxi time
    '            'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '            dr("Arrives") = "" '"<strong>Total: " & Format(totalprice, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("minutes") = minutesinair
    '            '20101027 - pab - fix totals on grid
    '            'dr("price") = Format(_taxes, "$##,##0.00")  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dr("price") = Format(TotalTaxes, "$##,##0.00")  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '            dtflights.Rows.Add(dr)

    '        End If


    '        'rk 11.3.2010
    '        If priceoverride Then
    '            dr = dtflights.NewRow
    '            dr("Origin") = ""
    '            dr("Destination") = ""
    '            dr("OriginFacilityName") = ""


    '            dr("DestinationFacilityName") = "<strong>" & da.GetSetting("SpecialPricingText") & vbCr & vbLf & " </strong>"
    '            dtflights.Rows.Add(dr)
    '        End If



    '        dr = dtflights.NewRow
    '        dr("Origin") = ""
    '        dr("Destination") = ""
    '        dr("OriginFacilityName") = ""
    '        dr("DestinationFacilityName") = "<strong>Total Price:</strong>"
    '        '20101104 - pab - remove hardcoded value
    '        'dr("Service Provider") = "Portal"
    '        dr("Service Provider") = da.GetSetting("CompanyName")
    '        'departure time
    '        dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '        '20081218 - pab - add taxi time
    '        'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '        dr("Arrives") = "" '"<strong>Total: " & Format(totalprice, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '        dr("minutes") = minutesinair
    '        '20101027 - pab - fix totals on grid
    '        'dr("price") = "<strong> " & Format(totalprice, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '        dr("price") = "<strong> " & Format(totalprice, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '        dtflights.Rows.Add(dr)
    '        Session("totalprice") = totalprice


    '    End If 'optimizer fail check



    '    dr = dtflights.NewRow
    '    dr("Origin") = ""
    '    dr("Destination") = ""
    '    dr("OriginFacilityName") = ""
    '    dr("DestinationFacilityName") = "<strong>Price Per Available Seat" & vbCr & vbLf & " (Up to " & TotalPassengers & " PAX):</strong>"
    '    '20101104 - pab - remove hardcoded value
    '    'dr("Service Provider") = "Portal"
    '    dr("Service Provider") = da.GetSetting("CompanyName")
    '    'departure time
    '    dr("Departs") = "" ') ' cdate(dv_oneWay(i)("DepartureTime")) 'DateAdd(DateInterval.Hour, -1, _leaves)
    '    '20081218 - pab - add taxi time
    '    'dr("Arrives") = DateAdd(DateInterval.Minute, minutesInAir, cdate(dv_oneWay(i)("DepartureTime")))
    '    dr("Arrives") = "" '"<strong>Total: " & Format(totalprice / 3, "##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '    dr("minutes") = minutesinair
    '    'rk 11/1/2010 GrandTotalPrice has replaced totalprice ... ?
    '    '  dr("price") = "<strong> " & Format(totalprice / TotalPassengers, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)
    '    dr("price") = "<strong> " & Format(totalprice / TotalPassengers, "$##,##0.00") & "</strong>"

    '    If discount <> 0 Then
    '        dr("DestinationFacilityName") = "<strong>" & Session("Plantype") & " level discount  applied for " & Session("firstname") & vbCr & vbLf & " :</strong>"
    '        dr("price") = "<strong> " & Format(discount, "$##,##0.00") & "</strong>"  '"" 'DateAdd(DateInterval.Minute, minutesinair + 12, _departDateTime)

    '    End If




    '    If optimizerfail And Not bQuoteAtCapacity Then
    '        Dim failmsg As String = da.GetSetting("NotAvailable") & " " & optmizerfailmsg   'rlk 11/1/2010 trap an optimizer failure
    '        If failmsg = "" Then failmsg = "This Aircraft Not Available - Try another time or aircraft " & optmizerfailmsg 'rlk 11/1/2010 trap an optimizer failure
    '        dr("DestinationFacilityName") = "<strong>" & failmsg & vbCr & vbLf
    '        Session("optimize") = "FAIL"
    '    End If


    '    dtflights.Rows.Add(dr)


    '    Me.gvServiceProviderMatrix.DataSource = dtflights
    '    Me.gvServiceProviderMatrix.DataBind()



    '    SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", "Timing Email ! Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), timing)
    '    SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "Timing Email ! Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), timing)



    'End Sub

    '20100323 - pab - add dest airport pax fees
    'Function feesatairport(ByVal airport As String) As String
    '20171121 - pab - fix carriers changing midstream - change to Session variables
    Function feesatairport(ByVal airport As String, ByVal bdest As Boolean, ByVal bintl As Boolean, ByVal carrierid As Integer, ByVal internationalfees As Double) As String

        If airport = "" Then

            feesatairport = "Null"
            _pricefees = 0
            Exit Function


        End If


        Dim dt_airportfeesPAX As DataTable = Nothing
        Dim dt_airportfees As DataTable = Nothing

        Dim pedetail As String
        _pricefees = 0
        '20101108 - pab
        '_internationalfees = 0

        Dim LandingFees As Double = 0 'CDbl(dt_airportfees.Rows(0)("LandingFees").ToString)
        Dim RepositionFees As Double = 0 'CDbl(dt_airportfees.Rows(0)("RepositionFees").ToString)
        Dim HangarCharges As Double = 0 'CDbl(dt_airportfees.Rows(0)("HangarCharges").ToString)
        Dim ParkingCost As Double = 0 'CDbl(dt_airportfees.Rows(0)("ParkingCost").ToString)
        Dim HandlingFeeGPU As Double = 0 'CDbl(dt_airportfees.Rows(0)("HandlingFeeGPU").ToString)
        Dim SecurityFee As Double = 0 'CDbl(dt_airportfees.Rows(0)("SecurityFee").ToString)
        Dim DeIcingPerCall As Double = 0 'CDbl(dt_airportfees.Rows(0)("DeIcingPerCall").ToString)
        Dim DeIcingPerUnit As Double = 0 'CDbl(dt_airportfees.Rows(0)("DeIcingPerUnit").ToString)
        Dim FuelDifferential As Double = 0 'CDbl(dt_airportfees.Rows(0)("FuelDifferential").ToString)
        Dim customsfee As Double = 0 'CDbl(dt_airportfees.Rows(0)("customsfee").ToString)

        '20100323 - pab - add airport pax fees
        Dim EU_EEA_CH As Double = 0 'CDbl(dt_airportfees.Rows(0)("EU_EEA_CH").ToString)
        Dim DOM As Double = 0 'CDbl(dt_airportfees.Rows(0)("DOM").ToString)
        Dim nonEU As Double = 0 'CDbl(dt_airportfees.Rows(0)("nonEU").ToString)
        Dim SecurityperPax As Double = 0 'CDbl(dt_airportfees.Rows(0)("SecurityperPax").ToString)
        Dim SegmentFeeDomestic As Double = 0 'CDbl(dt_airportfees.Rows(0)("SegmentFeeDomestic").ToString)
        Dim SegmentFeeIntl As Double = 0 'CDbl(dt_airportfees.Rows(0)("SegmentFeeIntl").ToString)

        Dim da As New DataAccess
        dt_airportfees = da.GetAirportServiceByCode(carrierid, airport)

        'System.IndexOutOfRangeException: There is no row at position 0.
        'rlk 1.4.10
        If dt_airportfees.Rows.Count <> 0 Then

            LandingFees = CDbl(dt_airportfees.Rows(0)("LandingFees").ToString)
            RepositionFees = CDbl(dt_airportfees.Rows(0)("RepositionFees").ToString)
            HangarCharges = CDbl(dt_airportfees.Rows(0)("HangarCharges").ToString)
            ParkingCost = CDbl(dt_airportfees.Rows(0)("ParkingCost").ToString)
            HandlingFeeGPU = CDbl(dt_airportfees.Rows(0)("HandlingFeeGPU").ToString)
            SecurityFee = CDbl(dt_airportfees.Rows(0)("SecurityFee").ToString)
            DeIcingPerCall = CDbl(dt_airportfees.Rows(0)("DeIcingPerCall").ToString)
            DeIcingPerUnit = CDbl(dt_airportfees.Rows(0)("DeIcingPerUnit").ToString)
            FuelDifferential = CDbl(dt_airportfees.Rows(0)("FuelDifferential").ToString)
            customsfee = CDbl(dt_airportfees.Rows(0)("customsfee").ToString)

            '20100323 - pab - add airport pax fees
            If Not IsDBNull(dt_airportfees.Rows(0)("EU_EEA_CH")) Then EU_EEA_CH = CDbl(dt_airportfees.Rows(0)("EU_EEA_CH").ToString)
            If Not IsDBNull(dt_airportfees.Rows(0)("DOM")) Then DOM = CDbl(dt_airportfees.Rows(0)("DOM").ToString)
            If Not IsDBNull(dt_airportfees.Rows(0)("nonEU")) Then nonEU = CDbl(dt_airportfees.Rows(0)("nonEU").ToString)
            If Not IsDBNull(dt_airportfees.Rows(0)("SecurityperPax")) Then SecurityperPax = CDbl(dt_airportfees.Rows(0)("SecurityperPax").ToString)
            If Not IsDBNull(dt_airportfees.Rows(0)("SegmentFeeDomestic")) Then SegmentFeeDomestic = CDbl(dt_airportfees.Rows(0)("SegmentFeeDomestic").ToString)
            If Not IsDBNull(dt_airportfees.Rows(0)("SegmentFeeIntl")) Then SegmentFeeIntl = CDbl(dt_airportfees.Rows(0)("SegmentFeeIntl").ToString)

        End If

        Dim airportname As String = fname(airport)

        '20100617 RK
        If bdest = True Then 'only charge this fee on landing, not takeoff
            pedetail = pedetail & "The Landing & ATC Fees Per FC at " & fname(airport) & " are " & Math.Round(LandingFees, 2) & vbCr & vbLf
            pedetail = pedetail & vbCr & vbLf
            _pricefees = _pricefees + LandingFees

            '20101029 - pab - only charge customs on landing
            pedetail = pedetail & "Custom Fees " & airportname & " are " & Math.Round(customsfee, 2) & vbCr & vbLf
            pedetail = pedetail & vbCr & vbLf
            '   _pricefees = _pricefees + customsfee
            internationalfees = internationalfees + customsfee

            'rlk 11/4/2010 add security fee on destination to destination bucket
            pedetail = pedetail & "Security Fees " & airportname & " are " & Math.Round(SecurityFee, 2) & vbCr & vbLf
            pedetail = pedetail & vbCr & vbLf
            '   _pricefees = _pricefees + customsfee
            internationalfees = internationalfees + SecurityFee

            '20101102 - pab - finx intl fees
            _customs = customsfee
        End If



        pedetail = pedetail & "The Reposition Fees at " & airportname & " are " & Math.Round(RepositionFees, 2) & vbCr & vbLf
        pedetail = pedetail & vbCr & vbLf
        _pricefees = _pricefees + RepositionFees

        'airfare

        '20101029 - pab - only charge customs on landing
        'pedetail = pedetail & "Custom Fees " & airportname & " are " & customsfee & vbCr & vbLf
        'pedetail = pedetail & vbCr & vbLf
        '_pricefees = _pricefees + customsfee

        'rk 8.18.2010 add support for handling fee credits
        pedetail = pedetail & "Handling Fee & GPU per FC " & airportname & " are <" & Math.Round(HandlingFeeGPU, 2) & ">" & vbCr & vbLf
        pedetail = pedetail & vbCr & vbLf
        _pricefees = _pricefees + HandlingFeeGPU






        Dim sameday As Boolean = False
        If Month(_leaves) = Month(_returns) And Day(_leaves) = Day(_returns) And Year(_leaves) = Year(_returns) Then sameday = True

        Dim overnight As Boolean = False
        If sameday = False Then overnight = True 'for now assume overnight if not return same day
        If overnight = True Then
            pedetail = pedetail & "The HangarCharges at " & airportname & " are " & Math.Round(HangarCharges, 2) & vbCr & vbLf
            pedetail = pedetail & vbCr & vbLf
            pedetail = pedetail & "The ParkingCost Fees at " & airportname & " are " & Math.Round(ParkingCost, 2) & vbCr & vbLf
            pedetail = pedetail & vbCr & vbLf
            '20101029 - pab - customs fee already added if needed
            '_pricefees = _pricefees + customsfee + HangarCharges + ParkingCost
            _pricefees = _pricefees + HangarCharges + ParkingCost

        End If

        '20100323 - pab - add dest airport pax fees
        '20101109 - pab - SegmentFeeIntl is per pax and charged on both origination and destination on international flights per Carolyn
        'If bdest = True Then
        'pedetail = pedetail & "The EU_EEA_CH Fees at " & airportname & " are " & EU_EEA_CH & vbCr & vbLf
        'pedetail = pedetail & vbCr & vbLf
        'pedetail = pedetail & "The DOM Fees at " & airportname & " are " & DOM & vbCr & vbLf
        'pedetail = pedetail & vbCr & vbLf
        'pedetail = pedetail & "The nonEU Fees at " & airportname & " are " & nonEU & vbCr & vbLf
        'pedetail = pedetail & vbCr & vbLf
        'pedetail = pedetail & "The SecurityperPax Fees at " & airportname & " are " & SecurityperPax & vbCr & vbLf
        'pedetail = pedetail & vbCr & vbLf
        '_pricefees = _pricefees + EU_EEA_CH + DOM + nonEU + SecurityperPax
        If bintl Then
            '20101109 - pab - SegmentFeeIntl is per pax
            'pedetail = pedetail & "The International Segment Fees at " & airportname & " are " & Math.Round(SegmentFeeIntl, 2) & vbCr & vbLf
            pedetail = pedetail & "The International Segment Fees at " & airportname & " are " & Math.Round(SegmentFeeIntl * _passengers, 2) & vbCr & vbLf
            pedetail = pedetail & vbCr & vbLf

            '20101102 - pab - SegmentFeeIntl being double counted - do not add to _pricefees
            '_pricefees = _pricefees + SegmentFeeIntl

            '20101109 - pab - SegmentFeeIntl is per pax
            '_internationalfees = _internationalfees + SegmentFeeIntl
            internationalfees = internationalfees + (SegmentFeeIntl * _passengers)

            '20101102 - pab - finx intl fees
            '20101109 - pab - SegmentFeeIntl is per pax
            '_intl = SegmentFeeIntl
            _intl = SegmentFeeIntl * _passengers

            'rk 10.18.2010 don't double count segment fees
            'Else
            '    pedetail = pedetail & "The Domestic Segment Fees at " & airportname & " are " & SegmentFeeDomestic & vbCr & vbLf
            '    pedetail = pedetail & vbCr & vbLf
            '    _pricefees = _pricefees + SegmentFeeDomestic
        End If
        'End If

        feesatairport = pedetail

    End Function


    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        '20120809 - pab - try to catch error causing Object reference not set to an instance of an object
        Try

            '20111229 - pab
            AirTaxi.post_timing("Page_PreRender Start  " & Now.ToString)

            Dim da As New DataAccess

            '20140310 - pab - acg background image
            '20150317 - pab - remove acg branding
            'If IsNothing(_acg) Then _acg = ""
            'If Not Request.QueryString("acg") Is Nothing Then
            '    _acg = Request.QueryString("acg")
            'End If

            'content.Style.Remove("class")
            'content.Style.Remove("background-image")
            'imgSplash.Visible = True
            'pflybody.Style.Remove("background")
            'pflybody.Style.Remove("filter")
            'gvServiceProviderMatrix.ForeColor = Drawing.Color.White
            ''LblItin.ForeColor = Drawing.Color.White
            ''topnavlogo.Visible = False
            ''imgSplash.Visible = True
            ''20150317 - pab - remove acg branding
            ''imgACGLogo.Visible = False
            'bottomnavlogo.Visible = False
            ''If _acg = "1" Or _acg = "2" Then
            ''    pflybody.Style.Remove("background-image")
            ''    content.Style.Add("class", "home_acg")

            ''    bimage.Src = "images/Background_1500x1000.jpg"
            ''    bimage.Visible = True

            ''    'content.Style.Add("class", "home_acg")
            ''    'content.Style.Add("background-image", "url('../images/Background_1500x1000.jpg')")
            ''    'imgSplash.Visible = False
            ''    'pflybody.Style.Add("background", "#e6e1d6;-moz-linear-gradient(top,  #e6e1d6 0%, #d8d4ca 46%, #d8d4ca 100%);-webkit-gradient(linear, left top, left bottom, color-stop(0%,#e6e1d6), color-stop(46%,#d8d4ca), color-stop(100%,#d8d4ca));-webkit-linear-gradient(top,  #e6e1d6 0%,#d8d4ca 46%,#d8d4ca 100%);-o-linear-gradient(top,  #e6e1d6 0%,#d8d4ca 46%,#d8d4ca 100%);-ms-linear-gradient(top,  #e6e1d6 0%,#d8d4ca 46%,#d8d4ca 100%);linear-gradient(to bottom,  #e6e1d6 0%,#d8d4ca 46%,#d8d4ca 100%);")
            ''    'pflybody.Style.Add("filter", "progid:DXImageTransform.Microsoft.gradient( startColorstr='#e6e1d6', endColorstr='#d8d4ca',GradientType=0 );")
            ''    'gvServiceProviderMatrix.ForeColor = Drawing.Color.Black
            ''    ''LblItin.ForeColor = Drawing.Color.Black
            ''    ''topnavlogo.Visible = True
            ''    lblFlightTimeMsg.ForeColor = Drawing.Color.White
            ''    lblmsg.ForeColor = Drawing.Color.White
            ''    imgSplash.Visible = False
            ''    imgACGLogo.Visible = True
            ''    bottomnavlogo.Visible = True
            ''Else
            ''content.Style.Add("class", "home")
            ''content.Style.Add("background-image", "url('../images/personifly_CAT_logo_MinimalClouds.png')")
            'pflybody.Style.Remove("class")
            'content.Style.Remove("background-image")
            'content.Style.Add("background-image", "url('images/personifly_CAT_logo_MinimalClouds.png')")
            'bimage.Visible = False
            ''End If

            ''20140315 - pab - show or hide maps (problems with maps caused by acg branding)
            'If da.GetSetting(_carrierid, "ShowMaps") = "Y" Then
            '    lblOrigin.Visible = True
            '    lblDest.Visible = True
            '    lblMsg1.Visible = True
            '    myMap0.Visible = True
            '    myMap.Visible = True
            'Else
            '    lblOrigin.Visible = False
            '    lblDest.Visible = False
            '    lblMsg1.Visible = False
            '    myMap0.Visible = False
            '    myMap.Visible = False
            'End If

            'Session("INCLUDECOPILOT") = "False"
            'If ChkCoPilot.Checked = True Then
            '    Session("INCLUDECOPILOT") = "True"
            'End If

            '20101027 - pab - don't clear msg - prerender executing after ddl edit and error was being cleared
            'Me.lblMsg.Text = ""

            '20131212 - pab - different background if from acg

            '20161227 - pab - dynamic carrier images
            If Not IsPostBack Then
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                Me.imglogo.Src = GetImageURLByATSSID(CInt(Session("carrierid")), 0, "logo")
                Me.lblCarrier.Text = Session("urlalias").ToString.ToUpper

                '20171017 - pab - demoair branding
                If CInt(Session("carrierid")) = DEMOAIR Then
                    imglogo.Width = 56
                    imglogo.Style.Remove("position")
                    imglogo.Style.Add("position", "absolute;top:16px;lefT:50%;margin:0 0 0 -23px;width:56px;z-index:1;")
                End If
            End If

            If Not (IsNothing(Session("reset"))) Then
                Dim aa As String = Session("reset").ToString
                If Session("reset").ToString = "T" Then
                    dtflights.Clear()

                    '20111221 - pab multiple quotes
                    dtflights2.Clear()

                    Session("reset") = "x"
                    Session("flights") = Nothing


                    _destAirportCode = ""
                    Session("destairportcode") = Nothing


                    Session("origairportcode") = Nothing
                    _origAirportCode = ""

                    '20120418 - pab - add contact information
                    Session("paymentType") = Nothing

                End If


            End If
            '--------------------------------DHA ~~Multileg -     
            If Not IsNothing(Session("reset")) Then
                If Session("reset").ToString = "M" Then
                    Session("newselection") = "M"
                    Exit Sub 'this will get set to M if there is an error    
                End If
            End If

            'Me.myMap0.Visible = False
            'Me.myMap.Visible = False


            'Me.LblItin.Visible = False

            '20140220 - pab - cleanup code
            'If Session("triptype") <> "M" Then
            '    Me.txtGeoOrig.Text = Session("origplace")
            '    Me.txtGeoDest.Text = Session("destplace")
            'End If


            ''Me.cmdNewDest.Visible = False

            'If Session("triptype") = "R" Then
            '    'Me.cmdNewDest.Visible = False
            '    Me.cmdQuote.Visible = False
            '    'Me.ddlTimeLeave.Visible = False
            'End If

            'If Session("triptype") = "O" Then
            '    'Me.cmdNewDest.Visible = True
            '    Me.cmdQuote.Visible = False
            '    'Me.ddlTimeLeave.Visible = False
            'End If

            'If Session("triptype") = "M" Then
            '    'Me.cmdNewDest.Visible = True
            '    Me.lblMsg.Visible = False
            '    Me.cmdQuote.Visible = False
            '    If Not IsNothing(Session("destAirportCode")) Then
            '        If IsNothing(Session("destlat")) Then
            '            Dim oAddr As New DetermineAddress
            '            'Session("destlat") = oAddr.AddressInfo(Session("destAirportCode"))
            '            Session("triptype") = "G"
            '        End If
            '    End If
            'End If

            If Not (IsNothing(Session("origlat"))) Then
                If Not (IsNothing(Session("destlat"))) Then
                    'list the available airports on each side

                    '20100608 - pab - add select airplane type
                    '20140315 - pab - make heading configurable
                    'Me.LblItin.Text = "Selected Itinerary "
                    'Me.LblItin.Text = da.GetSetting(_carrierid, "QuotePageHeading")

                    'If Not (IsNothing(Session("aircraftname"))) Then
                    '    Me.LblItin.Text = "Selected Itinerary " & Session("aircraftname").ToString
                    'End If


                    AirTaxi.post_timing("FindNearbyAirports start  " & Now.ToString)

                    '20101214 - pab fix error when lat/long too long - An invalid floating point operation occurred.
                    Session("origlat").Latitude = Math.Round(Session("origlat").Latitude, 10)
                    Session("origlat").Longitude = Math.Round(Session("origlat").Longitude, 10)
                    Session("destlat").Latitude = Math.Round(Session("destlat").Latitude, 10)
                    Session("destlat").Longitude = Math.Round(Session("destlat").Longitude, 10)

                    '20120503 - pab - move airport mapping to separate sub so it can be called from many places 
                    FindNearbyAirports()

                    'If Session("triptype") = "M" Then Me.cmdNewDest.Visible = True
                    '20160624 - pab - rti - cmdeuroutes is called in findnearbyairports - don't execute again here
                    'cmdEURoutes(GetAirportsLatLong(Session("origlat"), _carrierid), GetAirportsLatLong(Session("destlat"), _carrierid), _carrierid)
                    ' displayleg()

                    AirTaxi.post_timing("FindNearbyAirports complete  " & Now.ToString)
                End If
            End If


            'Me.calLeave.Visible = False
            'Me.txtdateleave.Visible = False

            If Not (IsDate(_departDateTime)) Or _departDateTime < Now Then
                If Request.QueryString("leaveDate") <> "" Then
                    _departDateTime = CDate(Request.QueryString("leaveDate"))
                End If

                '-- ~~DHA multileg
                'Me.ddlTimeLeave.SelectedIndex = _
                ' Me.ddlTimeLeave.Items.IndexOf( _
                '    Me.ddlTimeLeave.Items.FindByText(Request.QueryString("leaveTime")))

            End If


            '-- ~~DHA multileg
            'If Not (IsNothing(Session("leavetimedropvalue"))) Then
            '    Me.ddlTimeLeave.SelectedValue = Session("leavetimedropvalue")
            'End If


            'If Session("triptype") = "M" Then
            '    '  Me.calLeave.SelectedDate = _departDateTime = cdate(Me.txtdateleave.Text & Me.ddlTimeLeave.SelectedValue)
            '    '     _departDateTime()
            '    '
            '    Me.txtdateleave.Text = Month(_departDateTime) & "/" & Day(_departDateTime) & "/" & Year(_departDateTime)


            '    'If Request.QueryString("leaveDate") <> "" Then

            '    '    Me.calLeave.SelectedDate = cdate(Request.QueryString("leaveDate"))
            '    '    ' Me.calReturn.SelectedDate = cdate(Request.QueryString("returnDate"))


            '    ' Me.ddlTimeLeave.SelectedValue

            '    '' End If


            '    Me.txtdateleave.Visible = True
            '    Me.ddlTimeLeave.Visible = True


            '    ' Me.Header1.Visible = False

            '    If Me.ddlTimeLeave.BackColor <> Drawing.Color.Red Then
            '        Me.cmdNewDest.Visible = True
            '        If _origAirportCode = "" Or _destAirportCode = "" Then
            '            Me.cmdNewDest.Visible = False
            '            Me.lblMsg.Text = "Please click on desired departure and arrival airports"

            '            'Please click on desired departure and arrival airports

            '        End If



            '        If dtflights.Rows.Count > 0 Then
            '            LblItin.Visible = True
            '            Me.lblmsg.Text = "1. Click ADD ANOTHER DESTINATION, 2. Enter next destination, 3. Enter Departure Time, 4. Select airport on right map."
            '        End If
            '    End If


            'End If

            'Me.LblItin.Visible = True
            '20130429 - pab - move buttons per David - change to telerik buttons
            'Me.cmdStartOver.Visible = True
            'Me.cmdStartOver1.Visible = True
            If Not IsNothing(dtflights) Then
                If dtflights.Rows.Count = 0 Then
                    '20160117 - pab - quote multi-leg trips
                    'If Session("triptype") = "R" Or Session("triptype") = "O" Then
                    If Session("triptype") = "R" Or Session("triptype") = "O" Or Session("triptype") = "M" Then
                        '20130429 - pab - move buttons per David - change to telerik buttons
                        'Me.cmdStartOver.Visible = False
                        'Me.cmdStartOver1.Visible = False
                    End If
                    '20140220 - pab - cleanup code
                    'Me.cmdQuote.Visible = False
                    'Me.CmdEdit.Visible = False ' rk 10/20/2010 add quote editor
                    '20130429 - pab - move buttons per David - change to telerik buttons
                    'Me.cmdConfirm.Visible = False
                    'Me.cmdConfirm1.Visible = False
                    'Me.LblItin.Visible = False
                    '   Me.cmdNewDest.Visible = True

                End If

                If Not IsNothing(Session("triptype")) Then
                    If Session("triptype") = "G" Then
                        'If Not IsNothing(Me.drpFromAirport.SelectedItem) Then
                        '    Me.lblOrigin.Text = "Suggested Origin " & Me.drpFromAirport.SelectedItem.Text
                        'End If
                        'If Not IsNothing(Me.drpToAirport.SelectedItem) Then
                        '    Me.lblDest.Text = "Suggested Destination " & Me.drpToAirport.SelectedItem.Text
                        'End If
                    End If
                End If


                If Session("triptype") = "R" Or Session("triptype") = "O" Then
                    If dtflights.Rows.Count <> 0 Then
                        '20110906 - pab - don't display contine button if no flights
                        If dtflights.Rows(dtflights.Rows.Count - 1).Item("Price").ToString = "" Then
                            '20130429 - pab - move buttons per David - change to telerik buttons
                            'Me.cmdConfirm.Visible = False
                            'Me.cmdConfirm1.Visible = False
                        Else
                            '20130429 - pab - move buttons per David - change to telerik buttons
                            'Me.cmdConfirm.Visible = True
                            'Me.cmdConfirm1.Visible = True
                        End If
                        '20140220 - pab - cleanup code
                        'If Not IsNothing(Session("usertype")) Then
                        '    If Session("usertype") = "A" Then Me.CmdEdit.Visible = True ' rk 10/20/2010 add quote editor
                        'End If
                    End If
                End If

                ' Me.cmdNewDest.Visible = True
                If dtflights.Rows.Count <> 0 Then
                    '20110906 - pab - don't display contine button if no flights
                    If dtflights.Rows(dtflights.Rows.Count - 1).Item("Price").ToString = "" Then
                        '20130429 - pab - move buttons per David - change to telerik buttons
                        'Me.cmdConfirm.Visible = False
                        'Me.cmdConfirm1.Visible = False
                    Else
                        '20130429 - pab - move buttons per David - change to telerik buttons
                        'Me.cmdConfirm.Visible = True
                        'Me.cmdConfirm1.Visible = True
                    End If
                    '20140220 - pab - cleanup code
                    'If Not IsNothing(Session("usertype")) Then   'rk 11/1/2010 make quote editor useable on multi leg
                    '    If Session("usertype") = "A" Then Me.CmdEdit.Visible = True ' rk 10/20/2010 add quote editor
                    'End If
                    Me.gvServiceProviderMatrix.Visible = True

                    '20111216 - pab - use radio buttons for select per David
                    'Me.gvServiceProviderMatrix.DataSource = dtflights
                    'Me.gvServiceProviderMatrix.DataBind()
                    Bind_gvServiceProviderMatrix()

                    '20141017 - pab - add short runway message
                    '20160407 - pab - move to addleg so all legs are checked
                    'Dim ShortRunwayMsg As Integer = da.getsettingnumeric(_carrierid, "ShortRunwayMsg")
                    'Dim ShortRunwayMsgText As String = da.GetSetting(_carrierid, "ShortRunwayMsgText")
                    'Dim dt_orig As DataSet = da.GetAirportInformationByAirportCode(_origAirportCode)
                    'Dim dt_dest As DataSet = da.GetAirportInformationByAirportCode(_destAirportCode)
                    'If ShortRunwayMsg = 0 Then ShortRunwayMsg = 4400
                    'If Not IsNothing(dt_orig) And Not IsNothing(dt_dest) Then
                    '    If Not isdtnullorempty(dt_orig.Tables(0)) And Not isdtnullorempty(dt_dest.Tables(0)) Then
                    '        If dt_orig.Tables(0).Rows(0).Item("maxrunwaylength") < ShortRunwayMsg Or _
                    '                dt_dest.Tables(0).Rows(0).Item("maxrunwaylength") < ShortRunwayMsg Then
                    '            If ShortRunwayMsgText <> "" And InStr(lblOwnerConfirm.Text, ShortRunwayMsgText) = 0 Then
                    '                lblOwnerConfirm.Text &= "  <b>**" & ShortRunwayMsgText & "</b>"
                    '            End If
                    '        End If
                    '    End If
                    'End If

                End If

                '20111221 - pab - multiple quotes
                If Not IsNothing(dtflights2) Then
                    If dtflights2.Rows.Count <> 0 Then
                        '20120329 - pab - grids combined - do not show
                        'Me.gvServiceProviderMatrixReturn.Visible = True

                        'Bind_gvServiceProviderMatrixReturn()

                    End If
                End If

            End If

            '20120130 - pab - add start over button at top for when no flights returned
            'If Me.cmdStartOver2.Visible = True Then
            If Me.cmdStartOver.Visible = True Then
                '20130429 - pab - move buttons per David - change to telerik buttons
                'Me.cmdStartOver.Visible = False
                'Me.cmdStartOver1.Visible = False
            End If

            ' lblAdmin.Visible = False
            ' If Session("usertype") = "A" Then lblAdmin.Visible = True

            ''20131014 - pab - beta mode - do not allow purchase
            'Me.cmdConfirm1.Enabled = False
            'Me.lblmsg.Text = DataAccess.GetSetting(_carrierid, "BetaMessage")
            'Me.lblmsg.Visible = True

            Dim oQuote As New SaveQuoteInfo '--This code puts each quote detail into table: for analysis.
            oQuote.QuoteInfoArray("Door2Door Timing ! Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), timing)
            oQuote.InsertQuote()

            '20111229 - pab - response slow - check timing
            AirTaxi.post_timing("Page_PreRender End  " & Now.ToString)

            '20120111 - pab make timing email configurable
            If da.GetSetting(CInt(Session("carrierid")), "SendTimingEmail") = "Y" Then
                '20120606 - pab - write to email queue
                'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", "Door2Door Timing Email ! Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), timing)
                '20131024 - pab - fix duplicate emails
                'SendEmail(_carrierid, _emailfrom, "rkane@coastalaviationsoftware.com", "Door2Door Timing Email ! Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), timing, False, "", False)
                SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "", "Door2Door Timing Email ! Price Request from " & Session("origairportcode") & " to " & Session("destairportcode"), timing, CInt(Session("carrierid")))
            End If

            timing = ""

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then
                s &= "" & ex.InnerException.ToString
            End If
            If Not IsNothing(ex.StackTrace) Then
                s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            End If
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, Left(Now & " " & s, 500), "QuoteTrip.aspx.vb Page_PreRender", "")
            '20130930 - pab - change email from
            'SendEmail(_carrierid, "info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", _
            '          appName & " QuoteTrip.aspx.vb Page_PreRender error", s, False, "", False)
            '20131024 - pab - fix duplicate emails
            'SendEmail(_carrierid, _emailfrom, "rkane@coastalaviationsoftware.com", _
            '          appName & " QuoteTrip.aspx.vb Page_PreRender error", s, False, "", False)
            SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " QuoteTrip.aspx.vb Page_PreRender error", s, CInt(Session("carrierid")))

        End Try

    End Sub


    '20140220 - pab - cleanup code
    'Protected Sub cmdGeoCodeOrig_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdGeoCodeOrig.Click
    '    Dim o As New Mapping
    '    '20120424 - pab - convert from mappoint to bing
    '    'Dim a As LatLong = o.GeocodePlace(Me.txtGeoOrig.Text)
    '    Dim a As LatLong = o.geocodebing(Me.txtGeoOrig.Text)

    '    If a.Latitude = 0 Then
    '        Me.txtGeoOrig.BackColor = Drawing.Color.Crimson
    '        Me.lblMsg.Text = "Please check originating address"
    '    Else
    '        '20101214 - pab fix error when lat/long too long - An invalid floating point operation occurred.
    '        a.Latitude = Math.Round(a.Latitude, 10)
    '        a.Longitude = Math.Round(a.Longitude, 10)
    '        Session("origlat") = a
    '        Me.txtGeoOrig.BackColor = Drawing.Color.White
    '    End If



    'End Sub

    '20140220 - pab - cleanup code
    'Protected Sub cmdGeoCodeDest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdGeoCodeDest.Click

    '    Dim o As New Mapping
    '    '20120424 - pab - convert from mappoint to bing
    '    'Dim a As LatLong = o.GeocodePlace(Me.txtGeoDest.Text)
    '    Dim a As LatLong = o.geocodebing(Me.txtGeoDest.Text)

    '    If a.Latitude = 0 Then
    '        Me.txtGeoDest.BackColor = Drawing.Color.Crimson
    '        Me.lblMsg.Text = "Please check destination address"
    '    Else
    '        '20101214 - pab fix error when lat/long too long - An invalid floating point operation occurred.
    '        a.Latitude = Math.Round(a.Latitude, 10)
    '        a.Longitude = Math.Round(a.Longitude, 10)
    '        Session("destlat") = a
    '        Me.txtGeoDest.BackColor = Drawing.Color.White
    '    End If






    'End Sub




    '20140220 - pab - cleanup code
    'Protected Sub cmdSwitch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSwitch.Click





    '    Session("saveplace") = Session("origplace")
    '    Session("origplace") = Session("destplace")
    '    Session("destplace") = Session("saveplace")

    '    Me.txtGeoDest.Text = Session("destplace")
    '    Me.txtGeoOrig.Text = Session("origplace")


    '    'switch orig and dest lat and long
    '    Session("savelat") = Session("origlat")
    '    Session("origlat") = Session("destlat")
    '    Session("destlat") = Session("savelat")

    '    'switch orig and dest airport codes in both
    '    'private variables and session information
    '    Dim saveairportcode As String
    '    saveairportcode = _origAirportCode
    '    _origAirportCode = _destAirportCode
    '    _destAirportCode = saveairportcode
    '    Session("origAirportCode") = _destAirportCode
    '    Session("destAirportCode") = _origAirportCode





    'End Sub

    '--------------------------------DHA ~~Multileg - multileg error checking will be done on grid
    'Protected Sub ddlTimeLeave_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTimeLeave.SelectedIndexChanged

    '    If dtflights.Rows.Count > 0 Then
    '        Dim workingdate As Date
    '        'arrival time of last segment
    '        If dtflights.Rows(dtflights.Rows.Count - 1).Item("arrives") <> "" Then
    '            workingdate = CDate(dtflights.Rows(dtflights.Rows.Count - 1).Item("arrives"))

    '            ' Me.lblMsg.BackColor = Drawing.Color.White
    '            Me.ddlTimeLeave.BackColor = Drawing.Color.White
    '            If workingdate > CDate(Me.txtdateleave.Text & " " & Me.ddlTimeLeave.SelectedValue) Then

    '                Me.lblMsg.Text = "Please select a later departure time"
    '                Me.ddlTimeLeave.BackColor = Drawing.Color.Red
    '                Exit Sub
    '            End If
    '        End If

    '    End If

    '    _departDateTime = CDate(Me.txtdateleave.Text & " " & Me.ddlTimeLeave.SelectedValue)
    '    Session("leavtime") = _departDateTime
    '    Session("leavetimedropvalue") = Me.ddlTimeLeave.SelectedValue
    '    Session("departdate") = _departDateTime

    'End Sub





    'Protected Sub txtdateleave_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtdateleave.PreRender

    'End Sub

    'Protected Sub txtdateleave_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtdateleave.TextChanged
    '    If dtflights.Rows.Count > 0 Then
    '        Dim workingdate As Date
    '        'arrival time of last segment
    '        Me.lblMsg.BackColor = Drawing.Color.White
    '        workingdate = CDate(dtflights.Rows(dtflights.Rows.Count - 1).Item("arrives"))
    '        If workingdate > CDate(Me.txtdateleave.Text & " " & Me.ddlTimeLeave.SelectedValue) Then
    '            Me.lblMsg.Text = "Please select a later departure time"
    '            Me.lblMsg.BackColor = Drawing.Color.Red
    '            Exit Sub
    '        End If
    '    End If



    '    _departDateTime = CDate(Me.txtdateleave.Text & " " & Me.ddlTimeLeave.SelectedValue)
    '    Session("leavtime") = _departDateTime
    '    Session("leavetimedropvalue") = Me.ddlTimeLeave.SelectedValue

    'End Sub

    '20140220 - pab - cleanup code
    'Protected Sub txtGeoOrig_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGeoOrig.TextChanged
    '    Session("origplace") = Me.txtGeoOrig.Text
    'End Sub

    '20140220 - pab - cleanup code
    'Protected Sub txtGeoDest_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGeoDest.TextChanged
    '    Session("destplace") = Me.txtGeoDest.Text
    'End Sub

    '20130429 - pab - move buttons per David - change to telerik buttons
    'Protected Sub cmdConfirm_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdConfirm.Click

    '    cmdConfirmClicked(sender, e)

    'End Sub

    '20130429 - pab - move buttons per David - change to telerik buttons
    Protected Sub cmdConfirmClicked(ByVal sender As Object, ByVal e As System.EventArgs)

        '20120706 - pab - fix error when session times out - Object reference not set to an instance of an object
        If IsNothing(Session("triptype")) Then
            '20120130 - pab - add start over button at top for when no flights returned
            'cmdStartOver_Click(sender, e)
            cmdStartOverClicked(sender, e)
            Exit Sub
        End If

        '20111229 - pab
        AirTaxi.post_timing("cmdConfirmClicked Start  " & Now.ToString)

        Me.lblMsg.Visible = False
        Me.lblMsg.ForeColor = Drawing.Color.Black
        Me.lblMsg.Font.Bold = False
        If Session("optimize") = "FAIL" Then
            Me.lblMsg.Text = "Please try another aircraft or time"
            Me.lblMsg.Visible = True
            Me.lblMsg.ForeColor = Drawing.Color.Red
            Me.lblMsg.Font.Bold = True
            Exit Sub
        End If

        '20111206 - pab - multiple quotes
        ''If _leg1 = 0 Then
        'If Me.gvServiceProviderMatrix.SelectedIndex < 0 Then
        '    Me.lblmsg.Text = "Please select a flight"
        '    Exit Sub
        'End If
        'If Session("triptype") = "R" Then
        '    If Me.gvServiceProviderMatrix.SelectedIndex < 0 Then
        '        Me.lblmsg.Text = "Please select a return flight"
        '        Exit Sub
        '    End If
        'End If

        '20111216 - pab - use radio buttons for select per David
        Dim idx As Integer = -1
        GetSelectedRecord()
        '20111230 - pab - fix error when nothing selected - Object reference not set to an instance of an object
        If ViewState("SelectedFlight") Is Nothing Then
            If gvServiceProviderMatrix.Rows.Count = 1 Then
                idx = 0
            Else
                Me.lblMsg.Text = "Please select a flight"
                Me.lblMsg.Visible = True
                Me.lblMsg.ForeColor = Drawing.Color.Red
                Me.lblMsg.Font.Bold = True
                Exit Sub
            End If
        ElseIf ViewState("SelectedFlight").ToString = "" Or Not IsNumeric(ViewState("SelectedFlight").ToString) Then
            Me.lblMsg.Text = "Please select a flight"
            Me.lblMsg.Visible = True
            Me.lblMsg.ForeColor = Drawing.Color.Red
            Me.lblMsg.Font.Bold = True
            Exit Sub
        Else
            idx = CInt(ViewState("SelectedFlight"))
        End If

        '20120501 - pab - round trip in one row - no longer using second grid
        'Dim idx2 As Integer = -1
        'If Session("triptype") = "R" Then
        '    GetSelectedRecord2()
        '    If IsNothing(ViewState("SelectedFlight2")) Then
        '        idx2 = idx
        '    ElseIf ViewState("SelectedFlight2").ToString = "" Or Not IsNumeric(ViewState("SelectedFlight2").ToString) Then
        '        'Me.lblmsg.Text = "Please select a return flight"
        '        'Exit Sub
        '        idx2 = idx
        '    Else
        '        'idx2 = CInt(ViewState("SelectedFlight2"))
        '        idx2 = idx
        '    End If
        'End If

        Dim dr As GridViewRow
        'For i As Integer = 0 To gvServiceProviderMatrix.Rows.Count - 1
        '    dr = gvServiceProviderMatrix.Rows(i)
        '    For i2 As Integer = 0 To dr.Cells(0).Controls.Count - 1
        '        Dim ctl As Control = dr.Cells(0).Controls(i2)
        '        If TypeOf ctl Is RadioButton Then
        '            If CType(ctl, RadioButton).Text.Trim = "1" Then
        '                idx = i
        '            End If
        '        End If
        '    Next
        'Next

        Dim serviceprovider As String = String.Empty
        'Dim dr As GridViewRow = gvServiceProviderMatrix.Rows(Me.gvServiceProviderMatrix.SelectedIndex)
        dr = gvServiceProviderMatrix.Rows(idx)
        'Dim n As Integer = dr.Cells.Count

        '20120125 - pab - column added to grid
        'For i As Integer = 0 To dr.Cells(8).Controls.Count - 1
        'Dim ctl As Control = dr.Cells(8).Controls(i)
        '20120411 - pab - fuel stop column added to grid
        'For i As Integer = 0 To dr.Cells(9).Controls.Count - 1
        'Dim ctl As Control = dr.Cells(9).Controls(i)
        For i As Integer = 0 To dr.Cells(11).Controls.Count - 1
            Dim ctl As Control = dr.Cells(11).Controls(i)

            If TypeOf ctl Is DataBoundLiteralControl Then
                serviceprovider = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next
        Session("service provider") = serviceprovider
        Session("service provider") = Replace(Session("service provider"), "Fly ", "")

        ' ''-- ~~DHA new implmentation for multileg 
        If Session("triptype") = "G" Then
            '_passengers = Me.ddlPassengers.SelectedValue
            Session("passengers") = _passengers
        End If


        '20140220 - pab - cleanup code
        'Session("origAddr") = Me.txtGeoOrig.Text
        'Session("destAddr") = Me.txtGeoDest.Text
        Session("passengers") = _passengers
        Session("leaveDate") = Month(_leaveDateTime) & "/" & Day(_leaveDateTime) & "/" & Year(_leaveDateTime)
        Session("returnDate") = Month(_returnDateTime) & "/" & Day(_returnDateTime) & "/" & Year(_returnDateTime)


        Session("origAirportCodeReturn") = _origAirportCode
        Session("destAirportCodeReturn") = _destAirportCode

        '20120507 - pab - prevent confirmation email from going out more than once if back button pressed
        Session("confirmemailsent") = "N"

        '20120501 - pab - fix bug if one row selected, the user goes to the next page and then returns here and selects another row. the original row remains selected.
        Dim dtflightsselected As New DataTable
        Dim dr2 As DataRow = Nothing
        Dim dc As DataColumn

        '20140620 - pab - quote from admin portal
        dtflightsselected = Create_dtflights()
        'dc = New DataColumn("ID", System.Type.GetType("System.Int32"))
        'dc.AutoIncrement = True
        'dc.AutoIncrementSeed = 0
        'dc.AutoIncrementStep = 1
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Service Provider", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Origin", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("OriginFacilityName", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Departs", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Destination", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("DestinationFacilityName", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Arrives", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Flight Duration", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Minutes", System.Type.GetType("System.Int32"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Price", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("PriceEdit", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("PriceExplanationDetail", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("ShowPriceExplanation", System.Type.GetType("System.Boolean"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("EmptyLeg", System.Type.GetType("System.Int32"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("AircraftType", System.Type.GetType("System.Int32"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("WeightClass", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("dbname", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("aircraftlogo", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Name", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("FAQPageURL", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("carrierlogo", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        ''20120403 - pab - add fuel stops, pets, smoking
        'dc = New DataColumn("FuelStops", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Pets", System.Type.GetType("System.Boolean"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("Smoking", System.Type.GetType("System.Boolean"))
        'dtflightsselected.Columns.Add(dc)

        ''20120525 - pab - add certifications
        'dc = New DataColumn("certifications", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        ''20120619 - pab - add wifi
        'dc = New DataColumn("wifi", System.Type.GetType("System.Boolean"))
        'dtflightsselected.Columns.Add(dc)

        ''20120709 - pab - add lav, power
        'dc = New DataColumn("EnclosedLav", System.Type.GetType("System.Boolean"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("PowerAvailable", System.Type.GetType("System.Boolean"))
        'dtflightsselected.Columns.Add(dc)

        ''20131118 - pab - add more fields to aircraft
        'dc = New DataColumn("InflightEntertainment", System.Type.GetType("System.Boolean"))
        'dtflightsselected.Columns.Add(dc)

        'dc = New DataColumn("ManufactureDate", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        ''20140219 - pab - owner confirmation
        'dc = New DataColumn("OwnerConfirmation", System.Type.GetType("System.String"))
        'dtflightsselected.Columns.Add(dc)

        dr2 = dtflightsselected.NewRow

        For i As Integer = 0 To dr.Cells(1).Controls.Count - 1
            Dim ctl As Control = dr.Cells(1).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("carrierlogo") = InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "<img src=""", """ alt=")
                '20140620 - pab - populate certifications
                '20141201 - pab - fix error - Object reference not set to an instance of an object
                'dr2("certifications") = Replace(InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "Certifications: ", " title="), """", "").Trim
                Dim s As String = CType(ctl, DataBoundLiteralControl).Text.Trim
                If InStr(s, "Certifications: ") > 0 And InStr(s, " title=") > 0 Then
                    dr2("certifications") = Replace(InBetween(1, s, "Certifications: ", " title="), """", "").Trim
                Else
                    dr2("certifications") = InBetween(1, s, "title=""", """ style=")
                End If
            End If
        Next

        For i As Integer = 0 To dr.Cells(2).Controls.Count - 1
            Dim ctl As Control = dr.Cells(2).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("FAQPageURL") = InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "href=""", """  target=")
                dr2("aircraftlogo") = InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "<img src=""", """ alt=")
                dr2("Name") = InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "title=""", """ style=")
            End If
        Next

        For i As Integer = 0 To dr.Cells(3).Controls.Count - 1
            Dim ctl As Control = dr.Cells(3).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("OriginFacilityName") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(4).Controls.Count - 1
            Dim ctl As Control = dr.Cells(4).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("Departs") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(5).Controls.Count - 1
            Dim ctl As Control = dr.Cells(5).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("DestinationFacilityName") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(6).Controls.Count - 1
            Dim ctl As Control = dr.Cells(6).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("Arrives") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(7).Controls.Count - 1
            Dim ctl As Control = dr.Cells(7).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("Flight Duration") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(8).Controls.Count - 1
            Dim ctl As Control = dr.Cells(8).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("FuelStops") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        '20140219 - pab - owner confirmation
        For i As Integer = 0 To dr.Cells(9).Controls.Count - 1
            Dim ctl As Control = dr.Cells(9).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("OwnerConfirmation") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(10).Controls.Count - 1
            Dim ctl As Control = dr.Cells(10).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("Price") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        dr2("Service Provider") = serviceprovider

        For i As Integer = 0 To dr.Cells(12).Controls.Count - 1
            Dim ctl As Control = dr.Cells(12).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("PriceExplanationDetail") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        'Cells(13) is ID - ignore

        For i As Integer = 0 To dr.Cells(14).Controls.Count - 1
            Dim ctl As Control = dr.Cells(14).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("Origin") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(15).Controls.Count - 1
            Dim ctl As Control = dr.Cells(15).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("Destination") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(16).Controls.Count - 1
            Dim ctl As Control = dr.Cells(16).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("minutes") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(17).Controls.Count - 1
            Dim ctl As Control = dr.Cells(17).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("PriceEdit") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(18).Controls.Count - 1
            Dim ctl As Control = dr.Cells(18).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("ShowPriceExplanation") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("ShowPriceExplanation") = False
                End If
            End If
        Next

        For i As Integer = 0 To dr.Cells(19).Controls.Count - 1
            Dim ctl As Control = dr.Cells(19).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsNumeric(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    '20141106 - pab - rewrite quote routine
                    'dr2("EmptyLeg") = CInt(CType(ctl, DataBoundLiteralControl).Text.Trim)
                    dr2("EmptyLeg") = CType(ctl, DataBoundLiteralControl).Text.Trim
                Else
                    '20141106 - pab - rewrite quote routine
                    'dr2("EmptyLeg") = 0
                    dr2("EmptyLeg") = "0"
                End If
            End If
        Next

        For i As Integer = 0 To dr.Cells(20).Controls.Count - 1
            Dim ctl As Control = dr.Cells(20).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsNumeric(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("AircraftType") = CInt(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("AircraftType") = 0
                End If
            End If
        Next

        For i As Integer = 0 To dr.Cells(21).Controls.Count - 1
            Dim ctl As Control = dr.Cells(21).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("WeightClass") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(22).Controls.Count - 1
            Dim ctl As Control = dr.Cells(22).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("dbname") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        For i As Integer = 0 To dr.Cells(23).Controls.Count - 1
            Dim ctl As Control = dr.Cells(23).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("Pets") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("Pets") = False
                End If
            End If
        Next

        For i As Integer = 0 To dr.Cells(24).Controls.Count - 1
            Dim ctl As Control = dr.Cells(24).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("Smoking") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("Smoking") = False
                End If
            End If
        Next

        '20120619 - pab - add wifi
        For i As Integer = 0 To dr.Cells(25).Controls.Count - 1
            Dim ctl As Control = dr.Cells(25).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("wifi") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("wifi") = False
                End If
            End If
        Next

        '20120709 - pab - add lav, power
        For i As Integer = 0 To dr.Cells(26).Controls.Count - 1
            Dim ctl As Control = dr.Cells(26).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("EnclosedLav") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("EnclosedLav") = False
                End If
            End If
        Next

        For i As Integer = 0 To dr.Cells(27).Controls.Count - 1
            Dim ctl As Control = dr.Cells(27).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("PowerAvailable") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("PowerAvailable") = False
                End If
            End If
        Next

        '20131118 - pab - add more fields to aircraft
        For i As Integer = 0 To dr.Cells(28).Controls.Count - 1
            Dim ctl As Control = dr.Cells(28).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("InflightEntertainment") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("InflightEntertainment") = False
                End If
            End If
        Next

        For i As Integer = 0 To dr.Cells(29).Controls.Count - 1
            Dim ctl As Control = dr.Cells(29).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("ManufactureDate") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        '20140620 - pab - quote from admin portal
        For i As Integer = 0 To dr.Cells(30).Controls.Count - 1
            Dim ctl As Control = dr.Cells(30).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                dr2("EmptyLegPricing") = CType(ctl, DataBoundLiteralControl).Text.Trim
            End If
        Next

        '20141201 - pab - quoteworker rewrite
        For i As Integer = 0 To dr.Cells(31).Controls.Count - 1
            Dim ctl As Control = dr.Cells(31).Controls(i)
            If TypeOf ctl Is DataBoundLiteralControl Then
                If IsNumeric(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
                    dr2("quoteID") = CInt(CType(ctl, DataBoundLiteralControl).Text.Trim)
                Else
                    dr2("quoteID") = 0
                End If
            End If
        Next

        dtflightsselected.Rows.Add(dr2)

        '20120104 - pab - save quote if delta so contract can be pulled at purchase time
        If Session("dqn").ToString = "" And serviceprovider.ToUpper = "DELTA" Then

            If dtflightsselected.TableName = "" Then dtflightsselected.TableName = "dtflights"

            Dim carrierid As Integer = 0
            Dim dt As DataTable
            Dim da As New DataAccess
            dt = da.GetProviderByAlias(serviceprovider)
            If dt.Rows.Count > 0 Then
                carrierid = dt.Rows(0).Item("carrierid")
            End If

            Dim dqn2 As String = InBetween(1, dtflightsselected.Rows(0).Item("PriceExplanationDetail"), "; dqn ", ";")
            Dim qn As Integer = recordquotexml(dtflightsselected, "", "", 0, "", 9, "", "", "", "", "", "", carrierid)
            Session("quotenumber") = qn
            Session("dqn") = dqn2
            Session("dqn2") = dqn2

        Else
            '20120411 - pab - fix contract error on reservefilghts3 page
            If serviceprovider.ToUpper <> "DELTA" Then
                Session("dqn") = ""
                Session("dqn2") = ""
            End If

        End If

        '20111229 - pab
        AirTaxi.post_timing("cmdConfirmClicked End  " & Now.ToString)

        Session("flights") = dtflightsselected
        '20140310 - pab - acg background image
        '20150317 - pab - remove acg branding
        'Response.Redirect("reserveflight.aspx?acg=" & _acg)
        Response.Redirect("reserveflight.aspx", True)

        '20120501 - pab - fix bug if one row selected, the user goes to the next page and then returns here and selects another row. the original row remains selected.
        ''20111206 - pab - remove unselected quotes
        'If dtflights.Rows.Count > 1 Then
        '    'For n As Integer = gvServiceProviderMatrix.SelectedIndex + 1 To dtflights.Rows.Count - 1
        '    For n As Integer = idx + 1 To dtflights.Rows.Count - 1
        '        'dtflights.Rows(n).Delete()
        '        dtflights.Rows(dtflights.Rows.Count - 1).Delete()
        '    Next
        '    Do While dtflights.Rows.Count > 1
        '        dtflights.Rows(0).Delete()
        '    Loop
        'End If

        'If Session("triptype") = "R" Then
        '    If dtflights2.Rows.Count > 1 Then
        '        For n As Integer = idx2 + 1 To dtflights2.Rows.Count - 1
        '            dtflights2.Rows(dtflights2.Rows.Count - 1).Delete()
        '        Next
        '        Do While dtflights2.Rows.Count > 1
        '            dtflights2.Rows(0).Delete()
        '        Loop
        '    End If

        '    Dim dr2 As DataRow = Nothing
        '    For i As Integer = 0 To dtflights2.Rows.Count - 1
        '        dr2 = dtflights.NewRow

        '        'dr2("ID") = dtflights2.Rows(i).Item("ID")
        '        dr2("Service Provider") = dtflights2.Rows(i).Item("Service Provider")

        '        '20120125 - pab - add carrier logo
        '        dr2("carrierlogo") = dtflights2.Rows(i).Item("carrierlogo")

        '        dr2("Origin") = dtflights2.Rows(i).Item("Origin")
        '        dr2("OriginFacilityName") = dtflights2.Rows(i).Item("OriginFacilityName")
        '        dr2("Departs") = dtflights2.Rows(i).Item("Departs")
        '        dr2("Destination") = dtflights2.Rows(i).Item("Destination")
        '        dr2("DestinationFacilityName") = dtflights2.Rows(i).Item("DestinationFacilityName")
        '        dr2("Arrives") = dtflights2.Rows(i).Item("Arrives")
        '        dr2("Flight Duration") = dtflights2.Rows(i).Item("Flight Duration")
        '        dr2("minutes") = dtflights2.Rows(i).Item("minutes")
        '        dr2("Price") = dtflights2.Rows(i).Item("Price")
        '        dr2("PriceEdit") = dtflights2.Rows(i).Item("PriceEdit")
        '        dr2("PriceExplanationDetail") = dtflights2.Rows(i).Item("PriceExplanationDetail")
        '        dr2("ShowPriceExplanation") = dtflights2.Rows(i).Item("ShowPriceExplanation")
        '        dr2("EmptyLeg") = dtflights2.Rows(i).Item("EmptyLeg")
        '        dr2("AircraftType") = dtflights2.Rows(i).Item("AircraftType")
        '        dr2("WeightClass") = dtflights2.Rows(i).Item("WeightClass")
        '        dr2("dbname") = dtflights2.Rows(i).Item("dbname")
        '        dr2("aircraftlogo") = dtflights2.Rows(i).Item("aircraftlogo")
        '        dr2("Name") = dtflights2.Rows(i).Item("Name")
        '        dr2("FAQPageURL") = dtflights2.Rows(i).Item("FAQPageURL")

        '        '20120403 - pab - add fuel stops, pets, smoking
        '        dr2("FuelStops") = dtflights2.Rows(i).Item("FuelStops")
        '        dr2("Pets") = dtflights2.Rows(i).Item("Pets")
        '        dr2("Smoking") = dtflights2.Rows(i).Item("Smoking")

        '        dtflights.Rows.Add(dr2)

        '    Next

        'End If

        ''20120104 - pab - save quote if delta so contract can be pulled at purchase time
        'If Session("dqn").ToString = "" And serviceprovider.ToUpper = "DELTA" Then

        '    If dtflights.TableName = "" Then dtflights.TableName = "dtflights"

        '    Dim carrierid As Integer = 0
        '    Dim dt As DataTable
        '    Dim da As New DataAccess
        '    dt = da.GetProviderByAlias(serviceprovider)
        '    If dt.Rows.Count > 0 Then
        '        carrierid = dt.Rows(0).Item("carrierid")
        '    End If

        '    Dim dqn2 As String = InBetween(1, dtflights.Rows(0).Item("PriceExplanationDetail"), "; dqn ", ";")
        '    Dim qn As Integer = recordquotexml(dtflights, "", "", 0, "", 9, "", "", "", "", "", "", carrierid)
        '    Session("quotenumber") = qn
        '    Session("dqn") = dqn2
        '    Session("dqn2") = dqn2

        'Else
        '    '20120411 - pab - fix contract error on reservefilghts3 page
        '    If serviceprovider.ToUpper <> "DELTA" Then
        '        Session("dqn") = ""
        '        Session("dqn2") = ""
        '    End If

        'End If

        ''20111229 - pab
        'AirTaxi.post_timing("cmdConfirmClicked End  " & Now.ToString)

        'Session("flights") = dtflights
        'Response.Redirect("reserveflight.aspx")

    End Sub

    '20101029 - pab - make taxi minutes configurable
    'Function waitorreturn(ByVal waitingat As String, ByVal layoverminutes As Integer, ByVal airspeed As Double, ByVal repositioncostperhour As Double) As Double
    Function waitorreturn(ByVal CarrierID As Integer, ByVal waitingat As String, ByVal layoverminutes As Integer, ByVal airspeed As Double,
                          ByVal repositioncostperhour As Double, ByVal taximinutes As Double) As Double

        Dim closest_base As String
        Dim d As Double
        Dim t As Double
        Dim ma As Double


        'chg3640 - 20101014 - pab - remove hardcoded bases
        'closest_base = closesttobase("DXR", "ORH", waitingat)
        Dim da As New DataAccess
        Dim dt_homebase As DataTable = da.GetHomeBaseByType(CarrierID, Session("currtype"))
        Dim homebaseairportcode As String = ""
        homebaseairportcode = dt_homebase.Rows(0)("HomebaseAirportCode")


        If Len(homebaseairportcode) = 4 And Left(homebaseairportcode, 1) = "K" Then homebaseairportcode = Right(homebaseairportcode, 3) 'rk 11/9/2010 adjust for K airport names


        If homebaseairportcode <> waitingat Then
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            closest_base = closesttobase(CInt(Session("carrierid")), homebaseairportcode, homebaseairportcode, waitingat)
        Else
            closest_base = homebaseairportcode
        End If


        d = oMapping.GetRoundEarthDistanceBetweenLocations("pbaumgart@ctgi.com", "123", CInt(Session("carrierid")), closest_base, waitingat)


        t = CInt((d / airspeed) * 60)

        '20090223 - pab - change hardcoded values to configurable
        'ma1 = CInt(t) + 12
        '20101029 - pab - make taxi minutes configurable
        'ma = (CInt(t) + 12) * 2 'round trip
        ma = (CInt(t) + taximinutes) * 2

        'not enough time to reposition
        If ma > layoverminutes Then
            waitorreturn = 9999999999
            Exit Function
        End If

        '20081218 - pab - add additional fees
        'pedetail = pedetail & "We will reposition this flight from ADS to " & orig & " at a cost of $" & (ma1 * 3385 / 60) & _
        '    " for " & d & " miles and " & ma1 & " minutes. " & vbCr & vbLf & vbCr & vbLf
        If repositioncostperhour <> 0 Then
            waitorreturn = (ma * repositioncostperhour / 60)
            'pedetail = pedetail & "We will reposition this flight from " & closest_base & " to " & orig & " at a cost of $" & (ma1 * perHourRepositionTime / 60) & _
            '    " for " & d & " miles and " & ma1 & " minutes. " & vbCr & vbLf & vbCr & vbLf
        End If


    End Function


    'Protected Sub drpFromAirport_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpFromAirport.TextChanged
    '    Dim i As Long
    '    i = i

    'End Sub



    '--------------------------------DHA ~~Multileg - 
    'Protected Sub txtNewDest_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNewDest.TextChanged
    '    Call cmdNewDest_Click(Nothing, Nothing)
    '    Session("destAirportCode") = ""
    '    lblDest.Text = "Destination: " & txtNewDest.Text


    'End Sub

    '20140220 - pab - cleanup code
    'Protected Sub cmdReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdReset.Click
    '    Dim saveusername, saveusertype As String
    '    saveusername = ""
    '    saveusertype = ""
    '    If Not IsNothing(Session("username")) Then
    '        If Not IsNothing(Session("usertype")) Then
    '            saveusername = Session("username")
    '            saveusertype = Session("usertype")
    '        End If
    '    End If
    '    '20101101 - pab - fix admin start over
    '    If Session("usertype") <> "A" Then
    '        Session.Abandon()
    '    Else
    '        For i As Integer = 0 To Session.Count - 1
    '            Session(i) = Nothing
    '        Next
    '        If saveusername <> "" Then Session("username") = saveusername
    '        If saveusertype <> "" Then Session("usertype") = saveusertype
    '    End If

    '    '20120125 - pab -remove widget - go to index
    '    'Response.Redirect("QuoteTrip.aspx")
    '    '20140109 - pab - acg background image
    '    Response.Redirect("default.aspx?Referrer=" & _Referrer)

    'End Sub

    'Protected Sub cmdConfirm1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdlOGINnOW.Click
    '    Response.Redirect("customerlogin.aspx")
    'End Sub


    '20140220 - pab - cleanup code
    'Protected Sub cmdjOIN_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdjOIN.Click
    '    '20140109 - pab - acg background image
    '    Response.Redirect("customerlogin.aspx?Referrer=" & _Referrer)
    'End Sub




    '20140220 - pab - cleanup code
    'Protected Sub cmdNextAirplane_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdNextAirplane.Click

    '    '20111229 - pab
    '    AirTaxi.post_timing("cmdNextAirplane_Click Start  " & Now.ToString)

    '    distance_text = ""

    '    Dim da As New DataAccess


    '    Dim dt_priceTable As DataTable = da.GetFlightPricesPerHour(_carrierid)

    '    'Dim currentplane As Integer = Session("currplane")
    '    'Dim availplane As Integer = dt_priceTable.Rows.Count

    '    'currentplane = currentplane + 1
    '    'If currentplane > availplane Then
    '    '    Session("currplane") = 1
    '    '    currentplane = 1
    '    'Else
    '    '    Session("currplane") = currentplane
    '    '    'currentplane = 1
    '    'End If

    '    Dim currentplane As Integer = Session("currplane")
    '    Dim currenttype As Integer = Session("currtype")
    '    Dim availplane As Integer = dt_priceTable.Rows.Count
    '    Dim i As Integer = 0

    '    If availplane = 0 Then Exit Sub 'rk 11/4/2010 handle no aircraft initial setup


    '    currentplane = currentplane + 1
    '    If currentplane > availplane Then
    '        Session("currplane") = 1

    '        Session("aircraft") = 1
    '        currentplane = 1
    '        Session("currtype") = dt_priceTable.Rows(0)("id")
    '        Session("aircraftname") = dt_priceTable.Rows(0)("name").ToString
    '    Else
    '        For i = currentplane - 1 To availplane - 1
    '            If dt_priceTable.Rows(i)("AircraftTypeServiceSpecID") <> currenttype Then
    '                currentplane = i + 1
    '                Session("currplane") = currentplane

    '                Session("aircraft") = currentplane
    '                Session("currtype") = dt_priceTable.Rows(i)("AircraftTypeServiceSpecID")
    '                Session("aircraftname") = dt_priceTable.Rows(i)("name").ToString
    '                'currentplane = 1
    '                Exit For
    '            End If
    '        Next i
    '    End If

    '    loadairplane(_carrierid)

    '    'Dim discountPricePerHour As Integer = CInt((currentplandt_priceTable.Rows(currentplane)("DiscountPricePerHour"))

    '    'dont clear beetween selecting aircraft
    '    'dtflights.Clear()


    '    ' Me.ddlAircraftServiceTypes.SelectedIndex = Session("currtype")

    '    Dim c As Integer = dtflights.Rows.Count

    '    '20100624 - BD - As per Paula, check variable "c" in order to prevent a Richard bug.
    '    '20101214 - pab - fix error when dtflights has no rows
    '    'If c > 0 Then

    '    '    '20100813 - RK - change flight time between flight requests
    '    '    dtflights.Clear()

    '    '    '   dtflights.Rows(c - 1).Delete()
    '    '    '  dtflights.Rows(c - 2).Delete()
    '    'End If

    '    Call quote(_carrierid)

    '    '20111229 - pab
    '    AirTaxi.post_timing("cmdNextAirplane_Click End  " & Now.ToString)


    'End Sub

    Public Function loadairplane(ByVal carrierid As Integer) As String

        Dim da As New DataAccess


        Dim dt_priceTable As DataTable = da.GetFlightPricesPerHour(carrierid)
        Dim currentplane As Integer = Session("currplane")

        If dt_priceTable.Rows.Count < currentplane - 1 Then Exit Function

        If dt_priceTable.Rows.Count = 0 Then Exit Function 'rk 11/4/2010 handle no aircraft condition

        '20111227 - pab - process call from external source
        If currentplane > 0 Then

            '20140220 - pab - cleanup code
            'Me.ImgPlane.ImageUrl = dt_priceTable.Rows(currentplane - 1)("LogoURL")
            ''rk 11/9/2010 FAQ Page Link Update
            ''Me.HyperLinkPlane.NavigateUrl = dt_priceTable.Rows(currentplane - 1)("FAQPageURL")
            ''Me.HyperLinkPlane.Text = dt_priceTable.Rows(currentplane - 1)("Name")

            ''20101214 - pab - get service provider graphics
            '_aircraftlogo = Me.ImgPlane.ImageUrl.ToString


            If Not IsDBNull(dt_priceTable.Rows(currentplane - 1)("CostPerDayCoPilotExpenses")) Then
                ocp = dt_priceTable.Rows(currentplane - 1)("CostPerDayCoPilotExpenses")
            Else
                ocp = 0
            End If

        End If


        '20140220 - pab - cleanup code
        'Me.ChkCoPilot.Visible = False

        '20120507 - pab - do not show
        'If ocp <> 0 Then
        '    Me.ChkCoPilot.Visible = True
        '    Me.ChkCoPilot.Text = "Add Optional Co-Pilot $" & CInt(ocp) & " per day."
        'End If

    End Function



    '20140220 - pab - cleanup code
    'Protected Sub ChkCoPilot_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkCoPilot.CheckedChanged

    '    If ChkCoPilot.Checked = True Then
    '        Session("INCLUDECOPILOT") = "True"
    '        _addCoPilot = True
    '    Else
    '        Session("INCLUDECOPILOT") = "False"
    '        _addCoPilot = False
    '    End If

    '    '20101214 - pab - fix error when dtflights has no rows
    '    'dtflights.Clear()
    '    Call quote(_carrierid)

    'End Sub

    'Private Sub HydrateddlAircraftServiceTypes(ByVal carrierid As Integer)

    '    '20111229 - pab
    '    AirTaxi.post_timing("HydrateddlAircraftServiceTypes Start  " & Now.ToString)

    '    '20120402 - pab - replace inline query
    '    Dim da As New DataAccess
    '    Dim dt As DataTable = da.ListAircraftTypeServiceSpecsActive(carrierid)



    '    '20120402 - pab - replace inline query
    '    'If cnsetting.State = 0 Then
    '    '    cnsetting.ConnectionString = connectstring
    '    '    cnsetting.Open()
    '    'End If

    '    'Dim rs, rs1 As New ADODB.Recordset

    '    'Dim req As String
    '    ''req = "Select distinct AircraftTypeServiceSpecID FROM [Aircraft] where Active = '1'"
    '    ''req &= " and carrierid = " & _carrierid
    '    'req = "Select distinct ss.AircraftTypeServiceSpecID, name, weightclass from AircraftTypeServiceSpecs ss "
    '    'req &= "join aircraft ac on ss.AircraftTypeServiceSpecID = ac.AircraftTypeServiceSpecID and ss.carrierid = ac.carrierid "
    '    'req &= "where ss. carrierid = " & _carrierid & " and active = '1' group by ss.AircraftTypeServiceSpecID, ss.name, weightclass "
    '    'req &= "order by ss.AircraftTypeServiceSpecID"
    '    'If rs.State = 1 Then rs.Close()

    '    'rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

    '    ''Dim ac(250, 1) As String

    '    ''Dim i As Long
    '    ''While Not rs.EOF
    '    ''    i = i + 1

    '    ''    Dim AircraftTypeServiceSpecID As String = rs.Fields("AircraftTypeServiceSpecID").Value

    '    ''    req = "Select * from aircraft where active = '1' and AircraftTypeServiceSpecID = '" & AircraftTypeServiceSpecID & "'"
    '    ''    req &= " and carrierid = " & _carrierid
    '    ''    If rs1.State = 1 Then rs1.Close()
    '    ''    rs1.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
    '    ''    ac(i, 0) = rs1.Fields("AircraftTypeServiceSpecID").Value

    '    ''    req = "Select * from AircraftTypeServiceSpecs where  AircraftTypeServiceSpecID = '" & AircraftTypeServiceSpecID & "'"
    '    ''    req &= " and carrierid = " & _carrierid
    '    ''    If rs1.State = 1 Then rs1.Close()
    '    ''    rs1.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
    '    ''    ac(i, 1) = rs1.Fields("name").Value



    '    ''    rs.MoveNext()
    '    ''End While





    '    '20140221 - pab - use telerik controls
    '    'Dim li As ListItem = Nothing
    '    Dim li As New Telerik.Web.UI.RadComboBoxItem
    '    'Me.ddlAircraftServiceTypes.Items.Clear()
    '    Me.ddlAircraftServiceTypes1.Items.Clear()

    '    '20120319 - pab - show any on initial display
    '    'li = New ListItem("Any", 0)
    '    li.Text = "Any"
    '    li.Value = 0
    '    'Me.ddlAircraftServiceTypes.Items.Add(li)
    '    Me.ddlAircraftServiceTypes1.Items.Add(li)

    '    ''20120402 - pab - replace inline query
    '    ''For ii = 1 To i

    '    ''    Dim a, b As String
    '    ''    a = ac(ii, 1)
    '    ''    b = ac(ii, 0)

    '    ''    If a <> "" Then li = New ListItem(a, b)

    '    ''    Me.ddlAircraftServiceTypes.Items.Add(li)

    '    ''Next ii

    '    ''20111221 - pab - get lowest quotes for all weight classes
    '    ''li = New ListItem("All", 0)
    '    ''Me.ddlAircraftServiceTypes.Items.Add(li)

    '    'Do While Not rs.EOF
    '    '    li = New ListItem(rs.Fields("name").Value, rs.Fields("AircraftTypeServiceSpecID").Value)
    '    '    Me.ddlAircraftServiceTypes.Items.Add(li)

    '    '    '20120112 - pab - default to medium
    '    '    If rs.Fields("weightclass").Value = "M" Then
    '    '        Session("currtype") = rs.Fields("AircraftTypeServiceSpecID").Value
    '    '        Session("aircraftname") = rs.Fields("name").Value
    '    '        Session("currplane") = Me.ddlAircraftServiceTypes.Items.Count
    '    '    End If

    '    '    rs.MoveNext()
    '    'Loop

    '    '20120402 - pab - replace inline query
    '    If Not IsNothing(dt) Then
    '        If dt.Rows.Count > 0 Then
    '            For i As Integer = 0 To dt.Rows.Count - 1
    '                '20140221 - pab - use telerik controls
    '                'li = New ListItem(dt.Rows(i).Item("name"), dt.Rows(i).Item("AircraftTypeServiceSpecID").ToString)
    '                'Me.ddlAircraftServiceTypes.Items.Add(li)
    '                li = New Telerik.Web.UI.RadComboBoxItem
    '                li.Text = Trim(dt.Rows(i).Item("name").ToString)
    '                li.Value = Trim(dt.Rows(i).Item("AircraftTypeServiceSpecID").ToString)
    '                Me.ddlAircraftServiceTypes1.Items.Add(li)

    '                '20120112 - pab - default to medium
    '                If dt.Rows(i).Item("weightclass") = "M" Then
    '                    Session("currtype") = dt.Rows(i).Item("AircraftTypeServiceSpecID")
    '                    Session("aircraftname") = dt.Rows(i).Item("name")
    '                    'Session("currplane") = Me.ddlAircraftServiceTypes.Items.Count
    '                    Session("currplane") = Me.ddlAircraftServiceTypes1.Items.Count
    '                End If

    '            Next
    '        End If
    '    End If

    '    '20120214 - pab - add specific aircraft type ddl
    '    '20140220 - pab - cleanup code
    '    'HydrateddlAircraftTypes()


    '    '20120112 - pab - default to medium
    '    ''20101129 - pab - fix index out of range error if no aircraft set up
    '    'If Me.ddlAircraftServiceTypes.Items.Count > 0 Then

    '    '    If IsNothing(Session("currtype")) Then
    '    '        'Me.ddlAircraftServiceTypes.SelectedIndex = 0
    '    '        Session("currtype") = Me.ddlAircraftServiceTypes.Items(0).Value
    '    '        Session("aircraftname") = Me.ddlAircraftServiceTypes.Items(0)
    '    '        Session("currplane") = 1
    '    '    End If

    '    '    Dim aaa As Integer = CInt(Session("currtype"))
    '    '    If aaa = 0 Then
    '    '        Session("currtype") = Me.ddlAircraftServiceTypes.Items(0).Value
    '    '        Session("aircraftname") = Me.ddlAircraftServiceTypes.Items(0)
    '    '        Session("currplane") = 1
    '    '    End If

    '    '    'check if this is a valid service type
    '    '    Dim da As New DataAccess
    '    '    Dim dt_priceTable As DataTable = da.GetAircraftTypeServiceSpecsByID(carrierid, aaa)
    '    '    If dt_priceTable.Rows.Count = 0 Then

    '    '        Session("currtype") = Me.ddlAircraftServiceTypes.Items(0).Value
    '    '        Session("aircraftname") = Me.ddlAircraftServiceTypes.Items(0)
    '    '        Session("currplane") = 1
    '    '    End If

    '    '    Dim si As Integer
    '    '    si = CInt(Session("currplane"))

    '    '    '  Me.ddlAircraftServiceTypes.SelectedIndex = si

    '    'End If

    '    '20111229 - pab
    '    AirTaxi.post_timing("HydrateddlAircraftServiceTypes End  " & Now.ToString)

    'End Sub


    '20120214 - pab - add specific aircraft type ddl
    '20140220 - pab - cleanup code
    'Private Sub HydrateddlAircraftTypes()

    '    '20111229 - pab
    '    AirTaxi.post_timing("HydrateddlAircraftTypes Start  " & Now.ToString)

    '    Dim da As New DataAccess
    '    Dim dt As DataTable = da.GetNonGenericAircraftTypesForD2D()

    '    Dim li As ListItem = Nothing

    '    Me.ddlAircraftTypes.Items.Clear()

    '    '20120319 - pab - show none on initial display
    '    li = New ListItem("None", "0||0")
    '    Me.ddlAircraftTypes.Items.Add(li)

    '    If Not IsNothing(dt) Then
    '        If dt.Rows.Count > 0 Then
    '            For n As Integer = 0 To dt.Rows.Count - 1
    '                li = New ListItem(dt.Rows(n).Item("name").ToString, dt.Rows(n).Item("CarrierID").ToString & "||" & dt.Rows(n).Item("AircraftTypeServiceSpecID").ToString)
    '                Me.ddlAircraftTypes.Items.Add(li)
    '            Next
    '        End If
    '    End If

    '    '20111229 - pab
    '    AirTaxi.post_timing("HydrateddlAircraftTypes End  " & Now.ToString)

    'End Sub

    ''20140221 - pab - use telerik controls
    ''Protected Sub ddlAircraftServiceTypes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAircraftServiceTypes.SelectedIndexChanged
    'Protected Sub ddlAircraftServiceTypes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAircraftServiceTypes1.SelectedIndexChanged

    '    'Exit Sub

    '    '20120501 - pab - do not requote when aircraft type changed - used may want to select other additional options
    '    'code moved to AircraftServiceTypesChanged()


    'End Sub

    ''20120501 - pab - do not requote when aircraft type changed - used may want to select other additional options
    'Protected Sub AircraftServiceTypesChanged()

    '    '20111229 - pab
    '    AirTaxi.post_timing("AircraftServiceTypesChanged Start  " & Now.ToString)

    '    '20121204 - pab - implement aircraft to include
    '    Session("achourly") = ""

    '    'Session("currtype") = dt_priceTable.Rows(i)("id")
    '    'Session("aircraftname") = dt_priceTable.Rows(i)("name").ToString

    '    '20140221 - pab - use telerik controls
    '    'Session("currtype") = Me.ddlAircraftServiceTypes.SelectedValue
    '    'Session("aircraftname") = Me.ddlAircraftServiceTypes.SelectedItem
    '    Session("currtype") = Me.ddlAircraftServiceTypes1.SelectedValue
    '    Session("aircraftname") = Me.ddlAircraftServiceTypes1.SelectedItem

    '    '20120111 - pab - update weightclass
    '    Dim da As New DataAccess
    '    Dim dt As DataTable
    '    'dt = da.GetAircraftTypeServiceSpecsByID(_carrierid, Me.ddlAircraftServiceTypes.SelectedValue)
    '    '20140421 - pab - fix error - Input string was not in a correct format
    '    'dt = da.GetAircraftTypeServiceSpecsByID(_carrierid, Me.ddlAircraftServiceTypes1.SelectedValue)
    '    'If dt.Rows.Count > 0 Then
    '    If Me.ddlAircraftServiceTypes1.SelectedValue <> "" Then dt = da.GetAircraftTypeServiceSpecsByID(_carrierid, Me.ddlAircraftServiceTypes1.SelectedValue)
    '    If Not isdtnullorempty(dt) Then
    '        Session("weightclass") = dt.Rows(0).Item("weightclass")

    '    Else
    '        '20120320 - pab - service types - any
    '        Session("weightclass") = ""
    '        'Session("currtype") = Me.ddlAircraftServiceTypes.Items(1).Value
    '        'Session("aircraftname") = Me.ddlAircraftServiceTypes.Items(1).Text
    '        Session("currtype") = Me.ddlAircraftServiceTypes1.Items(1).Value
    '        Session("aircraftname") = Me.ddlAircraftServiceTypes1.Items(1).Text
    '    End If


    '    '20111227 - pab - process call from external source
    '    'Session("currplane") = Me.ddlAircraftServiceTypes.SelectedIndex + 1
    '    '20120330 - pab - add any to ddl
    '    'If Me.ddlAircraftServiceTypes.SelectedIndex = 0 Then
    '    '    Session("currplane") = 0
    '    'Else
    '    '20120430 - pab - fix no row at position error
    '    'Session("currplane") = Me.ddlAircraftServiceTypes.SelectedIndex + 1
    '    'Session("currplane") = Me.ddlAircraftServiceTypes.SelectedIndex
    '    Session("currplane") = Me.ddlAircraftServiceTypes1.SelectedIndex
    '    'End If

    '    '20120320 - pab - handle when 'any' selected
    '    '20120430 - pab - fix no row at position error
    '    'If Session("currplane") = 0 Then
    '    'If Me.ddlAircraftServiceTypes.SelectedIndex = 0 Then
    '    If Me.ddlAircraftServiceTypes1.SelectedIndex = 0 Then
    '        Session("currplane") = Nothing
    '        '20120803 - pab - run time improvements - quotes being called twice
    '        'Call cmdNextAirplane_Click(Nothing, Nothing)
    '    Else
    '        '20120803 - pab - run time improvements - quotes being called twice
    '        'loadairplane(_carrierid)
    '    End If

    '    '20120503 - pab - reset from and to airport codes to revert back to original request values
    '    _origAirportCode = ""
    '    _destAirportCode = ""
    '    Session("origairportcode") = ""
    '    Session("destairportcode") = ""
    '    '20120803 - pab - run time improvements - quotes being called twice
    '    'FindNearbyAirports()

    '    Dim c As Integer = 0
    '    If Not IsNothing(dtflights) Then
    '        If dtflights.Rows.Count > 0 Then c = dtflights.Rows.Count
    '    End If


    '    'Dim a As String = Session("currtype").ToString
    '    'Dim b As String = Me.ddlAircraftServiceTypes.SelectedValue.ToString
    '    'Dim d As String = Session("currplane").ToString

    '    'chg3642 - 20101019 - pab - fix "Please add a flight segment first" when the user selects a plane from the ddl before getting the first quote
    '    ''20100624 - BD - As per Paula, check variable "c" in order to prevent a Richard bug.
    '    'If c > 0 Then

    '    '    '20100813 - RK - change flight time between flight requests
    '    '    dtflights.Clear()

    '    '    '   dtflights.Rows(c - 1).Delete()
    '    '    '  dtflights.Rows(c - 2).Delete()
    '    'End If
    '    '20101027 - pab - check capacity
    '    Me.lblMsg.Text = ""
    '    Me.lblMsg.Visible = False
    '    If Not Session("currtype") Is Nothing Then
    '        'Dim da As New DataAccess
    '        Dim dt_priceTable As DataTable = da.GetAircraftTypeServiceSpecsByID(_carrierid, CInt(Session("currtype").ToString))
    '        If dt_priceTable.Rows.Count > 0 Then

    '            'rk 11/9/2010 FAQ Page Link Update
    '            '20140220 - pab - cleanup code
    '            'Me.ImgPlane.ImageUrl = dt_priceTable.Rows(0)("LogoURL")
    '            'Me.HyperLinkPlane.NavigateUrl = dt_priceTable.Rows(0)("FAQPageURL")
    '            'Me.HyperLinkPlane.Text = dt_priceTable.Rows(0)("Name")

    '            ''20101214 - pab - get service provider graphics
    '            '_aircraftlogo = Me.ImgPlane.ImageUrl.ToString

    '            Dim TotalPassengers As Integer = CInt(dt_priceTable.Rows(0)("TotalPassengers"))
    '            If _passengers > TotalPassengers Then
    '                Me.lblMsg.Text = "Pax Capacity on " & dt_priceTable.Rows(0)("Name").ToString & " is " & dt_priceTable.Rows(0)("TotalPassengers").ToString & ". Select different plane."
    '                Me.lblMsg.Visible = True
    '                Exit Sub
    '            End If
    '        End If
    '    End If

    '    '20111219 - pab - multiple quotes
    '    'Dim dr As DataRow
    '    'If dtflights.Rows.Count > 0 Then
    '    '    'dtflights.Clear()
    '    '    Dim i As Integer = dtflights.Rows.Count - 1
    '    '    Do While i > 0
    '    '        dr = dtflights.Rows(i)
    '    '        If dr("Origin") = "" Then
    '    '            dtflights.Rows(i).Delete()
    '    '        End If
    '    '        i = i - 1
    '    '    Loop
    '    '    Call quote(_carrierid)
    '    'End If

    '    '20120502 - pab - clear flights even if orig airport populated
    '    'If _origAirportCode <> "" Then
    '    If Not IsNothing(dtflights) Then dtflights.Clear()

    '    '    '20111221 - pab multiple quotes
    '    If Not IsNothing(dtflights2) Then dtflights2.Clear()

    '    '    '20120112 - pab - runt time improvement - this is done in page_prerender - don't need to do twice
    '    '    'addleg(Session("triptype"), _carrierid)
    '    '    'Call quote(_carrierid)
    '    'End If

    '    '20111229 - pab
    '    AirTaxi.post_timing("AircraftServiceTypesChanged End  " & Now.ToString)

    'End Sub

    '20140220 - pab - cleanup code
    Protected Sub CmdEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEdit.Click

        '20111216 - pab - use radio buttons for select per David
        Dim idx As Integer = -1
        GetSelectedRecord()
        '20111230 - pab - fix error when nothing selected - Object reference not set to an instance of an object
        If ViewState("SelectedFlight") Is Nothing Then
            If gvServiceProviderMatrix.Rows.Count = 1 Then
                idx = 0
            Else
                Me.lblMsg.Text = "Please select a flight"
                Me.lblMsg.Visible = True
                Me.lblMsg.ForeColor = Drawing.Color.Red
                Me.lblMsg.Font.Bold = True
                Exit Sub
            End If
        ElseIf ViewState("SelectedFlight").ToString = "" Or Not IsNumeric(ViewState("SelectedFlight").ToString) Then
            Me.lblMsg.Text = "Please select a flight"
            Me.lblMsg.Visible = True
            Me.lblMsg.ForeColor = Drawing.Color.Red
            Me.lblMsg.Font.Bold = True
            Exit Sub
        Else
            idx = CInt(ViewState("SelectedFlight"))
        End If

        Dim dt As New DataTable
        Dim dr As DataRow = dtflights.Rows(idx)
        Dim dr2 As DataRow = Nothing
        Dim dc As DataColumn

        dt = Create_dtflights()
        dc = New DataColumn("WeightClassTitle", System.Type.GetType("System.String"))
        dt.Columns.Add(dc)

        dr2 = dt.NewRow
        For i As Integer = 0 To dt.Columns.Count - 1
            dr2(i) = dr(i)
        Next
        dt.Rows.Add(dr2)

        Session("flights") = dt
        '20140109 - pab - acg background image
        'Session("lookupquote") = 0
        'Response.Redirect("EditFlight.aspx?Referrer=" & Session("lookupquote").ToString) ' rk 10/20/2010 add quote editor
        Response.Redirect("EditFlight.aspx", True) ' rk 10/20/2010 add quote editor

    End Sub

    '--------------------------------DHA ~~Multileg - 
    Public Function isintl(ByVal airport As String) As Boolean

        Dim ds As New DataSet
        ds = oMapping.GetAirportInformationByAirportCode(airport)
        Dim region As String = ""

        If Not ds Is Nothing Then
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    If ds.Tables(0).Rows.Count > 0 Then
                        region = (ds.Tables(0).Rows(0)("REGION"))
                    End If
                End If
            End If
        End If

        If region = "INTL" Then
            isintl = True
        Else
            isintl = False
        End If




    End Function

    Public Function GetTimes() As DataTable
        Dim retDT As New Data.DataTable

        If Not IsPostBack Then
            Dim oLookup As New PopulateLookups
            'Dim ds As DataSet

            retDT = oLookup.TimeDD("All")
            'ds = retDT.DataSet
            'Return ds
        End If
        Return retDT
    End Function
    '--------------------------------DHA ~~Multileg - 
    '~~CHECK THIS we may not need this
    Protected Function DetermineOrigin(ByVal strCurrentLeg As String) As String
        Dim strOrigin As String = Nothing

        Dim iCurrentLeg As Integer
        iCurrentLeg = CInt(strCurrentLeg)


        'iLeg - current record
        If iCurrentLeg = 0 Or iCurrentLeg = -1 Then
            strOrigin = ""
        Else
            '20140220 - pab - cleanup code
            'Select Case DirectCast(Me.gvMultiLeg.Rows(iCurrentLeg - 1).FindControl("txtMultiTo"), TextBox).Text
            '    Case Nothing
            '        strOrigin = ""
            '    Case "From"
            '        strOrigin = ""
            '    Case Else
            '        strOrigin = DirectCast(Me.gvMultiLeg.Rows(iCurrentLeg - 1).FindControl("txtMultiTo"), TextBox).Text
            'End Select
        End If

        Return strOrigin
    End Function
    '--------------------------------DHA ~~Multileg -
    Private Sub Bind_multiLeg(ByVal iStart As Integer, ByVal iEnd As Integer)

        Dim oLookup As New PopulateLookups
        Dim retDT As New DataTable
        'Dim ds As DataSet

        retDT = oLookup.TimeDD("All")

        Dim dt As DataTable = Create_MultiLegDatatable()

        Dim dr As DataRow
        For i As Integer = iStart To iEnd

            dr = dt.NewRow()
            dr("LID") = (i - 1).ToString
            dr(1) = i.ToString          '--Leg
            dr(2) = ""                  '--From
            dr(3) = ""                  '--To
            dr(4) = ""                  '--Depart Date
            dr(5) = ""                  '--Depart Time
            dr(6) = ""
            dr(7) = ""
            dr(8) = ""                  'save latlong.lat  orig
            dr(9) = ""                  'save latlong.long orig
            dr(10) = ""                 'save latlong.lat  dest
            dr(11) = ""                 'save latlong.long dest
            dt.Rows.Add(dr)
        Next i

        '20140220 - pab - cleanup code
        'gvMultiLeg.DataSource = dt
        'gvMultiLeg.DataBind()

        'For Each d As GridViewRow In gvMultiLeg.Rows
        '    DirectCast(d.FindControl("ddlMultiTime"), DropDownList).DataSource = retDT.DefaultView
        '    DirectCast(d.FindControl("ddlMultiTime"), DropDownList).DataBind()
        'Next

        'DirectCast(Me.gvMultiLeg.Rows(0).FindControl("txtMultiFrom"), TextBox).ReadOnly = False
        ''DirectCast(Me.gvMultiLeg.Rows(0).FindControl("txtMultiFrom"), TextBox).Visible = True


    End Sub

    '--------------------------------DHA ~~Multileg - 
    Function Create_MultiLegDatatable() As DataTable

        Dim dt As New DataTable
        Dim dc As DataColumn

        dc = New DataColumn("LID")
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "Leg"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "FromLocation"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "ToLocation"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "DepartDate"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "DepartTime"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "SaveDate"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "SaveTime"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "LatOrig"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "LongOrig"
        dt.Columns.Add(dc)
        dc = New DataColumn
        dc.ColumnName = "LatDest"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "LongDest"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "SaveOrigAC"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.ColumnName = "SaveDestAC"
        dt.Columns.Add(dc)


        Return dt
    End Function

    '--------------------------------DHA ~~Multileg - 
    Private Function DetermineLegs(ByRef dr As GridViewRow) As ArrayList
        'row - airportcode/address From -  airportcode/address To - departure date - departure time
        'The entire row = each fields needs to have values or none of them do
        Dim aSaveLegInfo As New ArrayList
        'Dim ac As UserControl
        '1-4 values user inputted. 

        aSaveLegInfo.Add("X")
        aSaveLegInfo.Add(DirectCast(dr.FindControl("txtMultiFrom"), TextBox).Text)
        aSaveLegInfo.Add(DirectCast(dr.FindControl("txtMultiTo"), TextBox).Text)
        aSaveLegInfo.Add(DirectCast(dr.FindControl("txtCalMulti"), TextBox).Text)
        aSaveLegInfo.Add(DirectCast(dr.FindControl("ddlMultiTime"), DropDownList).SelectedValue)

        'If CDate(aSaveLegInfo(3)) <= Now Then
        'aSaveLegInfo(0) = "Flight can not be in the past"
        'End If
        If Not IsNothing(aSaveLegInfo(3)) Then
            If IsDate(aSaveLegInfo(3)) Then
                If aSaveLegInfo(3) <> "" Then
                    aSaveLegInfo.Add(CDate(CDate(aSaveLegInfo(3)).ToString("d") & " " & aSaveLegInfo(4).ToString))
                End If
            End If
        End If
        'CDate(aSaveLegInfo(3)) & " " & aSaveLegInfo(4)

        Dim ctr As Integer = 0
        For i As Integer = 1 To 4
            If (aSaveLegInfo(i) Is Nothing) Or (aSaveLegInfo(i).ToString = "") Then
                ctr = ctr + 1
            End If
        Next i

        Select Case ctr
            Case 0      'leg is valid
                aSaveLegInfo(0) = "0"
            Case 4      'leg is not filled in at all 
                aSaveLegInfo(0) = "4"
            Case Else   'not all filled in
                aSaveLegInfo(0) = aSaveLegInfo(0).ToString & "<br/>" & "Please fill in all fields before submitting the request"
                'Me.lblItineraryError.Text = "All vales must be filled in"
        End Select

        Return aSaveLegInfo
    End Function

    'Public Sub MultiCal_DateChanged(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim SaveDateSelect As Date = DirectCast(DirectCast(sender, System.Web.UI.WebControls.Calendar).Parent.Parent, calMulti).SelectedDate
    '    Dim index As Integer = DirectCast(DirectCast(sender, System.Web.UI.WebControls.Calendar).Parent.Parent.Parent.Parent, GridViewRow).RowIndex

    '    DirectCast(Me.gvMultiLeg.Rows(index).FindControl("txtCalMulti"), TextBox).Text = SaveDateSelect.ToString("d")
    '    DirectCast(DirectCast(Me.gvMultiLeg.Rows(index).FindControl("calMulti"), ASP.controls_calendar_ascx).FindControl("tbDate"), TextBox).Visible = False

    'End Sub

    '--------------------------------DHA ~~Multileg - 
    'Protected Sub gvMultiLeg_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvMultiLeg.RowCommand

    '    '20111229 - pab
    '    AirTaxi.post_timing("gvMultiLeg_RowCommand Start  " & Now.ToString)

    '    Dim eSave As Object = e.CommandArgument


    '    'Dim r As Integer
    '    'Dim oPricing As New Pricing

    '    'get selected calendar date for each row
    '    'Dim index As Integer = Convert.ToInt32(e.CommandArgument)   'current row

    '    'Dim DateSelectSave As Date
    '    'DateSelectSave = DirectCast(DirectCast(Me.gvMultiLeg.Rows(index).FindControl("calMulti"), UserControl), ASP.controls_calendar_ascx).SelectedDate
    '    'DirectCast(DirectCast(Me.gvMultiLeg.Rows(index).FindControl("calMulti"), UserControl), ASP.controls_calendar_ascx).SelectedDate = DateSelectSave
    '    Me.lblItineraryError.Text = ""
    '    'DirectCast(gvMultiLeg.FindControl("btnGetFlightsMulti"), Button).Enabled = False

    '    Select Case e.CommandName
    '        Case "MultiTesting"
    '            DirectCast(Me.gvMultiLeg.Rows(0).FindControl("txtMultiFrom"), TextBox).Text = "TEB" 'first leg from32
    '            DirectCast(Me.gvMultiLeg.Rows(0).FindControl("txtMultiTo"), TextBox).Text = "HPN" 'first leg to
    '            DirectCast(DirectCast(DirectCast(Me.gvMultiLeg.Rows(0).FindControl("calMulti"), UserControl), System.Web.UI.UserControl), ASP.controls_calendar_ascx).SelectedDate = CDate("12/1/2010")
    '            DirectCast(Me.gvMultiLeg.Rows(0).FindControl("txtCalMulti"), TextBox).Text = CDate("12/1/2010").ToString("d")
    '            DirectCast(Me.gvMultiLeg.Rows(0).FindControl("ddlMultiTime"), DropDownList).SelectedValue = "06:00 AM"

    '            DirectCast(Me.gvMultiLeg.Rows(1).FindControl("txtMultiFrom"), TextBox).Text = "HPN" 'first leg from
    '            DirectCast(Me.gvMultiLeg.Rows(1).FindControl("txtMultiTo"), TextBox).Text = "ISP" 'first leg to
    '            DirectCast(DirectCast(Me.gvMultiLeg.Rows(1).FindControl("calMulti"), UserControl), ASP.controls_calendar_ascx).SelectedDate = CDate("12/1/2010")
    '            DirectCast(Me.gvMultiLeg.Rows(1).FindControl("txtCalMulti"), TextBox).Text = CDate("12/1/2010").ToString("d")
    '            DirectCast(Me.gvMultiLeg.Rows(1).FindControl("ddlMultiTime"), DropDownList).SelectedValue = "09:00 AM"

    '            DirectCast(Me.gvMultiLeg.Rows(2).FindControl("txtMultiFrom"), TextBox).Text = "ISP" 'first leg from
    '            DirectCast(Me.gvMultiLeg.Rows(2).FindControl("txtMultiTo"), TextBox).Text = "TEB" 'first leg to
    '            DirectCast(DirectCast(DirectCast(Me.gvMultiLeg.Rows(2).FindControl("calMulti"), UserControl), System.Web.UI.UserControl), ASP.controls_calendar_ascx).SelectedDate = CDate("12/1/2010")
    '            DirectCast(Me.gvMultiLeg.Rows(2).FindControl("txtCalMulti"), TextBox).Text = CDate("12/1/2010").ToString("d")
    '            DirectCast(Me.gvMultiLeg.Rows(2).FindControl("ddlMultiTime"), DropDownList).SelectedValue = "11:00 AM"

    '        Case "MoreLegs"
    '            Dim oDT As DataTable = Create_MultiLegDatatable()
    '            Dim dr As DataRow = Nothing
    '            Dim rTotal As Integer = (Me.gvMultiLeg.Rows.Count) - 1

    '            'capture existing leg info
    '            '<%DetermineOrigin(strCurrentLeg:=0)%>

    '            For j As Integer = 0 To rTotal
    '                dr = oDT.NewRow()
    '                dr("LID") = DirectCast(Me.gvMultiLeg.Rows(j).FindControl("LID"), TextBox).Text
    '                dr("Leg") = DirectCast(Me.gvMultiLeg.Rows(j).FindControl("txLeg"), TextBox).Text
    '                dr("FromLocation") = DirectCast(Me.gvMultiLeg.Rows(j).FindControl("txtMultiFrom"), TextBox).Text
    '                dr("ToLocation") = DirectCast(Me.gvMultiLeg.Rows(j).FindControl("txtMultiTo"), TextBox).Text
    '                dr("DepartDate") = DirectCast(DirectCast(DirectCast(Me.gvMultiLeg.Rows(j).FindControl("calMulti"), UserControl), System.Web.UI.UserControl), ASP.controls_calendar_ascx).SelectedDate
    '                dr("DepartTime") = DirectCast(Me.gvMultiLeg.Rows(j).FindControl("ddlMultiTime"), DropDownList).SelectedValue
    '                dr("Savedate") = dr("DepartDate")
    '                dr("SaveTime") = dr("DepartTime")
    '                oDT.Rows.Add(dr)
    '            Next j
    '            Session("InMulti") = "Y"
    '            'now add the new legs
    '            Dim iStart As Integer = rTotal + 2
    '            Dim iEnd As Integer = (rTotal + 2) + 3 'add three legs later can be database driven

    '            For i As Integer = iStart To iEnd
    '                dr = oDT.NewRow
    '                dr("LID") = (i - 1).ToString
    '                dr("Leg") = i.ToString
    '                oDT.Rows.Add(dr)
    '            Next i

    '            'Me.gvMultiLeg.DataSource = oDT
    '            'Me.gvMultiLeg.DataBind()
    '            'Session("reset") = "M"

    '            Dim ct As Integer = 0
    '            For Each od As DataRow In oDT.Rows

    '                DirectCast(Me.gvMultiLeg.Rows(ct).FindControl("txtMultiFrom"), TextBox).Text = od.Item("FromLocation").ToString
    '                DirectCast(Me.gvMultiLeg.Rows(ct).FindControl("txtMultiTo"), TextBox).Text = od.Item("ToLocation").ToString
    '                DirectCast(Me.gvMultiLeg.Rows(ct).FindControl("txtCalMulti"), TextBox).Text = od.Item("time").ToString
    '                'DirectCast(DirectCast(DirectCast(Me.gvMultiLeg.Rows(0).FindControl("calMulti"), UserControl), System.Web.UI.UserControl), ASP.controls_calendar_ascx).SelectedDate = CDate("12/1/2010")
    '                DirectCast(Me.gvMultiLeg.Rows(ct).FindControl("ddlMultiTime"), DropDownList).SelectedValue = od.Item("DepartTime").ToString
    '                ct = ct + 1
    '            Next
    '        Case "btnGetFlightsMulti"
    '            'DirectCast(Me.gvMultiLeg.FindControl("btnGetFlightsMulti"), Button).Enabled = False
    '            Session("triptype") = "M"
    '            Me.lblItineraryError.Text = ""
    '            Session("dMulti") = Nothing

    '            '20101029 - pab - fix number of passengers for quote sub
    '            'Session("passengers") = ddlPassengers.SelectedValue

    '            Dim currentLeg As String = Nothing
    '            Dim SaveLocation As String = Nothing
    '            Dim aRetval As ArrayList
    '            Dim dtMulti As New DataTable
    '            Dim drMulti As DataRow
    '            Dim iNumLegs As Integer = 0
    '            Dim strLeg As String

    '            Dim dsOrig As DataTable
    '            Dim dsDest As DataTable

    '            Dim bContinue As Boolean = True
    '            Dim bDateMistake As Boolean = False

    '            Session("international_type") = "domestic"  'initialize this variable

    '            'create datatable
    '            dtMulti = Create_MultiLegDatatable()
    '            For Each d As GridViewRow In gvMultiLeg.Rows

    '                currentLeg = DirectCast(d.FindControl("LID"), TextBox).Text
    '                strLeg = " (Leg  " & (d.RowIndex + 1).ToString & ")<br/>"

    '                If currentLeg <> "0" Then
    '                    'update origin with destination from last record
    '                    DirectCast(d.FindControl("txtMultiFrom"), TextBox).Text = SaveLocation
    '                End If

    '                SaveLocation = DirectCast(d.FindControl("txtMultiTo"), TextBox).Text
    '                aRetval = DetermineLegs(d)



    '                'get lat/long for each leg
    '                Dim oAddr As New DetermineAddress
    '                Dim oRetval1 As New LATLONG
    '                Dim oRetval2 As New LATLONG

    '                oRetval1 = oAddr.AddressInfo(aRetval(1))
    '                oRetval2 = oAddr.AddressInfo(aRetval(2))

    '                'Me.AddressCaptureVertical1.OriginAddress = aRetval(1).ToString
    '                'Me.AddressCaptureVertical1.DestinationAddress = aRetval(2).ToS)tring
    '                'Me.calLeave.SelectedDate = CDate(aRetval(3))
    '                'Me.ddlTimeLeave.SelectedValue = aRetval(4).ToString

    '                'Me.calReturn.SelectedDate = Nothing
    '                'Me.ddlTimeReturn.SelectedValue = Nothing


    '                'Session("triptype") = "A"
    '                'Me.lblItineraryError.Text = ""
    '                If aRetval(0).ToString = "0" Then
    '                    'bttnGetFlights_Click(sender, e)  'called in case user has entered address and not airport code

    '                    drMulti = dtMulti.NewRow

    '                    iNumLegs = iNumLegs + 1
    '                    drMulti = dtMulti.NewRow()
    '                    drMulti("LID") = d.RowIndex
    '                    drMulti("Leg") = d.RowIndex + 1
    '                    'drMulti("FromLocation") = Me.AddressCaptureVertical1.OriginAddress.ToString
    '                    'drMulti("ToLocation") = Me.AddressCaptureVertical1.DestinationAddress.ToString

    '                    '--------------------

    '                    dsOrig = New DataTable
    '                    dsDest = New DataTable

    '                    'If Not IsNothing(oRetval1) Then
    '                    'dsOrig = GetAirportsLatLong(oRetval1)
    '                    '    If dsOrig.Rows.Count > 0 Then
    '                    '        _origAirportCode = dsOrig.Rows(0).Item("LocationID")
    '                    '        Session("origAirportCode") = _origAirportCode
    '                    '    Else
    '                    '        Me.lblItineraryError.Text = "Origination Outside Service Area: " & strLeg
    '                    '        bContinue = False
    '                    '    End If
    '                    'Else
    '                    '    Me.lblItineraryError.Text = "Is the destination city/state spelled correctly? " & strLeg
    '                    'End If


    '                    'If Not IsNothing(oRetval2) Then
    '                    ' dsDest = GetAirportsLatLong(oRetval2)
    '                    ' End If

    '                    '-------------------------
    '                    drMulti("FromLocation") = aRetval(1)
    '                    drMulti("ToLocation") = aRetval(2)
    '                    drMulti("DepartDate") = aRetval(3)
    '                    drMulti("DepartTime") = aRetval(4)
    '                    drMulti("SaveDate") = aRetval(3).ToString
    '                    drMulti("SaveTime") = aRetval(4)

    '                    If Not IsNothing(oRetval1) Then
    '                        drMulti("LatOrig") = oRetval1.Latitude
    '                        drMulti("LongOrig") = oRetval1.Longitude
    '                        dsOrig = GetAirportsLatLong(oRetval1, _carrierid)
    '                        If dsOrig.Rows.Count > 0 Then
    '                            _origAirportCode = dsOrig.Rows(0).Item("LocationID")
    '                            Session("origAirportCode") = _origAirportCode
    '                            drMulti("SaveOrigAC") = drMulti("FromLocation")
    '                            drMulti("FromLocation") = _origAirportCode
    '                            If isintl(_origAirportCode) Then
    '                                Session("international_type") = "intl" 'if any leg is intl will mark flight as such
    '                            End If
    '                        Else
    '                            Me.lblItineraryError.Text += "Origination Outside Service Area:" & strLeg
    '                            DirectCast(d.FindControl("txtMultiFrom"), TextBox).BackColor = Drawing.Color.Red
    '                            bContinue = False
    '                        End If
    '                    Else
    '                        Me.lblItineraryError.Text += "Is the origination city/state spelled correctly?" & strLeg
    '                        DirectCast(d.FindControl("txtMultiFrom"), TextBox).BackColor = Drawing.Color.Red
    '                        bContinue = False
    '                    End If

    '                    If Not IsNothing(oRetval2) Then
    '                        drMulti("LatDest") = oRetval2.Latitude
    '                        drMulti("LongDest") = oRetval2.Longitude
    '                        dsDest = GetAirportsLatLong(oRetval2, _carrierid)
    '                        If dsDest.Rows.Count > 0 Then
    '                            _destAirportCode = dsDest.Rows(0).Item("LocationID")
    '                            Session("destAirportCode") = _destAirportCode
    '                            drMulti("SaveDestAC") = drMulti("ToLocation")
    '                            drMulti("ToLocation") = _destAirportCode
    '                            If isintl(_destAirportCode) Then
    '                                Session("international_type") = "intl" 'if any leg is intl will mark flight as such
    '                            End If
    '                        Else
    '                            Me.lblItineraryError.Text += "Destination Outside Service Area: " & strLeg
    '                            DirectCast(d.FindControl("txtMultiTo"), TextBox).BackColor = Drawing.Color.Red
    '                            bContinue = False
    '                        End If
    '                    Else
    '                        Me.lblItineraryError.Text += "Is the destination city/state spelled correctly?" & strLeg
    '                        DirectCast(d.FindControl("txtMultiTo"), TextBox).BackColor = Drawing.Color.Red
    '                        bContinue = False
    '                    End If
    '                    dtMulti.Rows.Add(drMulti)
    '                    DirectCast(d.FindControl("tbSaveDate"), TextBox).Text = aRetval(3).ToString
    '                    DirectCast(d.FindControl("tbSaveTime"), TextBox).Text = aRetval(4).ToString


    '                    If Not IsDate(aRetval(3)) Then
    '                        If aRetval(3) <> "" Then
    '                            'If Not bDateMistake Then
    '                            Me.lblItineraryError.Text += "Invalid Date " & strLeg
    '                            bDateMistake = True
    '                            'End If
    '                            DirectCast(d.FindControl("txtCalMulti"), TextBox).BackColor = Drawing.Color.Red
    '                            bContinue = False
    '                        End If
    '                    End If

    '                ElseIf aRetval(0).ToString = "4" Then
    '                    ''Dim index = d.RowIndex
    '                    'DirectCast(Me.gvMultiLeg.Rows(index).FindControl("Leg"), TextBox).Text = "---"
    '                    'whole leg not leg
    '                Else
    '                    'problem with this lieg
    '                    'Me.lblItineraryError.Text = aRetval(0)
    '                    'bContinue = False
    '                End If

    '            Next


    '            Session("reset") = Nothing
    '            'deterine orignation and destination 
    '            If iNumLegs = 0 Then
    '                'user entered no data - display msg 
    '                Me.lblItineraryError.Text = "No complete legs entered"
    '            Else
    '                If Not bContinue Then
    '                    Exit Sub
    '                End If

    '                'Session("destairportcode") = DirectCast(Me.gvMultiLeg.Rows(iNumLegs - 1).FindControl("txtMultiTo"), TextBox).Text
    '                'Session("origairportcode") = DirectCast(Me.gvMultiLeg.Rows(0).FindControl("txtMultiFrom"), TextBox).Text

    '                Session("tripype") = "M"
    '                Session("MultiInfo") = dtMulti

    '                Dim mOrig As String
    '                Dim mDest As String
    '                Dim totalLegs As Integer = dtMulti.Rows.Count - 1

    '                mOrig = dtMulti.Rows(0).Item("FromLocation")
    '                mDest = dtMulti.Rows(totalLegs).Item("ToLocation")

    '                Dim oLatLong As New LATLONG
    '                oLatLong.Latitude = dtMulti.Rows(0).Item("LatOrig")
    '                oLatLong.Longitude = dtMulti.Rows(0).Item("LongOrig")
    '                Session("origlat") = oLatLong
    '                Session("origplace") = mOrig

    '                Dim dLatLong As New LATLONG

    '                'determine destination - see if last leg destination = origin
    '                If mOrig = mDest Then
    '                    mDest = dtMulti.Rows(totalLegs).Item("FromLocation")
    '                    dLatLong.Latitude = dtMulti.Rows(totalLegs).Item("LatOrig")
    '                    dLatLong.Longitude = dtMulti.Rows(totalLegs).Item("LongOrig")
    '                    Session("destlat") = dLatLong
    '                    Session("destplace") = mDest
    '                Else
    '                    dLatLong.Latitude = dtMulti.Rows(totalLegs).Item("LatDest")
    '                    dLatLong.Longitude = dtMulti.Rows(totalLegs).Item("LongDest")
    '                    Session("destlat") = dLatLong
    '                    Session("destplace") = mDest
    '                End If
    '                'oMapping.GetAirportInformationByAirportCode(mOrig)
    '                'Dim dsOrig, dsDest As DataTable
    '                dsOrig = GetAirportsLatLong(Session("origlat"), _carrierid)
    '                dsDest = GetAirportsLatLong(Session("destlat"), _carrierid)

    '                _origAirportCode = dsOrig.Rows(0).Item("LocationID")
    '                _destAirportCode = dsDest.Rows(0).Item("LocationID")

    '                Session("origAirportCode") = _origAirportCode
    '                Session("destAirportCode") = _destAirportCode


    '                cmdEURoutes(dsOrig, dsDest, _carrierid)

    '                'Session("origlat") = Nothing
    '                Session("destlat") = Nothing
    '                Session("triptype") = "M"

    '            End If
    '        Case "MapLeg"
    '            'get origin/dest send to cmdEURRoutes to show on maps
    '            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
    '            Me.lblSaveLeg.Text = index
    '            If index <> 0 Then
    '                Dim LastLeg As String = DirectCast(gvMultiLeg.Rows(index - 1).FindControl("txtMultiTo"), TextBox).Text
    '                DirectCast(gvMultiLeg.Rows(index).FindControl("txtMultiFrom"), TextBox).Text = LastLeg
    '            End If


    '            'get lat/long for each leg
    '            Dim oAddr As New DetermineAddress
    '            Dim oRetval1 As New LATLONG
    '            Dim oRetval2 As New LATLONG

    '            Dim a1 As String = (DirectCast(gvMultiLeg.Rows(index).FindControl("txtMultiFrom"), TextBox).Text)
    '            Dim a2 As String = (DirectCast(gvMultiLeg.Rows(index).FindControl("txtMultiTo"), TextBox).Text)

    '            oRetval1 = oAddr.AddressInfo(a1)
    '            oRetval2 = oAddr.AddressInfo(a2)


    '            Session("origlat") = oRetval1
    '            Session("destlat") = oRetval2


    '            Dim dsOrig, dsDest As DataTable
    '            dsOrig = New DataTable
    '            dsDest = New DataTable
    '            Dim strLeg As String = "(Leg  " & (index + 1).ToString & ")<br/>"


    '            Dim bContinue As Boolean = True
    '            If Not IsNothing(oRetval1) Then
    '                dsOrig = GetAirportsLatLong(oRetval1, _carrierid)
    '                If dsOrig.Rows.Count > 0 Then
    '                    _origAirportCode = dsOrig.Rows(0).Item("LocationID")
    '                    Session("origAirportCode") = _origAirportCode
    '                Else
    '                    Me.lblItineraryError.Text = "Origination Outside Service Area"
    '                    bContinue = False
    '                End If
    '            Else
    '                Me.lblItineraryError.Text = "Is the origination city/state spelled correctly?"
    '                bContinue = False
    '            End If

    '            If Not IsNothing(oRetval2) Then
    '                dsDest = GetAirportsLatLong(oRetval2, _carrierid)
    '                If dsDest.Rows.Count > 0 Then

    '                    _destAirportCode = dsDest.Rows(0).Item("LocationID")
    '                    Session("destAirportCode") = _destAirportCode

    '                Else
    '                    Me.lblItineraryError.Text = "Destination Outside Service Area"
    '                    bContinue = False
    '                End If
    '            Else
    '                Me.lblItineraryError.Text = "Is the destination city/state spelled correctly?"
    '                bContinue = False
    '            End If


    '            If bContinue Then
    '                Me.lblmsg.Text = "Displaying Leg: " & (index + 1).ToString
    '                Me.lblmsg.Visible = True
    '                If Session("triptype") = "M" Then
    '                    Session("triptype") = "G"
    '                    cmdEURoutes(dsOrig, dsDest, _carrierid)
    '                    'Session("origlat") = Nothing
    '                    Session("destlat") = Nothing
    '                    'Session("triptype") = "M"
    '                End If
    '            End If

    '    End Select

    '    '20111229 - pab
    '    AirTaxi.post_timing("gvMultiLeg_RowCommand End  " & Now.ToString)

    'End Sub
    '20120214 - pab - add specific aircraft type ddl
    '20140220 - pab - cleanup code
    'Protected Sub ddlAircraftTypes_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlAircraftTypes.SelectedIndexChanged

    '    Exit Sub

    '    AirTaxi.post_timing("ddlAircraftTypes_SelectedIndexChanged Start  " & Now.ToString)

    '    Session("currtype") = Me.ddlAircraftTypes.SelectedValue
    '    Session("aircraftname") = Me.ddlAircraftTypes.SelectedItem

    '    Dim da As New DataAccess
    '    Dim dt As DataTable
    '    dt = da.GetAircraftTypeServiceSpecsByID(_carrierid, Me.ddlAircraftTypes.SelectedValue)
    '    If dt.Rows.Count > 0 Then
    '        Session("weightclass") = dt.Rows(0).Item("weightclass")
    '    End If


    '    If Me.ddlAircraftTypes.SelectedIndex = 0 Then
    '        Session("currplane") = 1
    '    Else
    '        Session("currplane") = Me.ddlAircraftTypes.SelectedIndex + 1
    '    End If

    '    loadairplane(_carrierid)

    '    Dim c As Integer = dtflights.Rows.Count


    '    Me.lblMsg.Text = ""
    '    Me.lblMsg.Visible = False
    '    If Not Session("currtype") Is Nothing Then
    '        Dim dt_priceTable As DataTable = da.GetAircraftTypeServiceSpecsByID(_carrierid, CInt(Session("currtype").ToString))
    '        If dt_priceTable.Rows.Count > 0 Then

    '            '20140220 - pab - cleanup code
    '            'Me.ImgPlane.ImageUrl = dt_priceTable.Rows(0)("LogoURL")
    '            'Me.HyperLinkPlane.NavigateUrl = dt_priceTable.Rows(0)("FAQPageURL")
    '            'Me.HyperLinkPlane.Text = dt_priceTable.Rows(0)("Name")

    '            '20140220 - pab - cleanup code
    '            '_aircraftlogo = Me.ImgPlane.ImageUrl.ToString

    '            Dim TotalPassengers As Integer = CInt(dt_priceTable.Rows(0)("TotalPassengers"))
    '            If _passengers > TotalPassengers Then
    '                Me.lblMsg.Text = "Pax Capacity on " & dt_priceTable.Rows(0)("Name").ToString & " is " & dt_priceTable.Rows(0)("TotalPassengers").ToString & ". Select different plane."
    '                Me.lblMsg.Visible = True
    '                Exit Sub
    '            End If
    '        End If
    '    End If

    '    If _origAirportCode <> "" Then
    '        dtflights.Clear()

    '        dtflights2.Clear()

    '    End If

    '    AirTaxi.post_timing("ddlAircraftTypes_SelectedIndexChanged End  " & Now.ToString)

    'End Sub

    '20120501 - pab - do not requote when aircraft type changed - used may want to select other additional options
    'Protected Sub cmdUpdateQuote_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdateQuote.Click

    '    AircraftServiceTypesChanged()

    'End Sub

    ''20130429 - pab - move buttons per David - change to telerik buttons
    'Protected Sub cmdConfirm1_Click(sender As Object, e As EventArgs) Handles cmdConfirm1.Click

    '    '20131016 - pab - fix session timeout
    '    If IsNothing(Session("flights")) And IsNothing(Session("triptype")) Then
    '        Dim da As New DataAccess
    '        lblMsg.Text = da.GetSetting(_carrierid, "TimeoutMessage")
    '        lblMsg.Visible = True
    '        gvServiceProviderMatrix.EmptyDataText = lblMsg.Text
    '        dtflights.Clear()
    '        Me.gvServiceProviderMatrix.DataSource = dtflights
    '        Me.gvServiceProviderMatrix.DataBind()
    '    Else
    '        cmdConfirmClicked(sender, e)
    '    End If

    'End Sub

    ''20130429 - pab - move buttons per David - change to telerik buttons
    'Protected Sub cmdStartOver1_Click(sender As Object, e As EventArgs) Handles cmdStartOver1.Click

    '    cmdStartOverClicked(sender, e)

    'End Sub

    ''20130429 - pab - move buttons per David - change to telerik buttons
    'Protected Sub cmdUpdateQuote1_Click(sender As Object, e As EventArgs) Handles cmdUpdateQuote1.Click

    '    '20131016 - pab - fix session timeout
    '    If IsNothing(Session("flights")) And IsNothing(Session("triptype")) Then
    '        Dim da As New DataAccess
    '        lblMsg.Text = da.GetSetting(_carrierid, "TimeoutMessage")
    '        lblMsg.Visible = True
    '        gvServiceProviderMatrix.EmptyDataText = lblMsg.Text
    '        dtflights.Clear()
    '        Me.gvServiceProviderMatrix.DataSource = dtflights
    '        Me.gvServiceProviderMatrix.DataBind()
    '    Else
    '        AircraftServiceTypesChanged()
    '    End If

    'End Sub

    Private Sub RadComboBoxACInclude_SelectedIndexChanged(sender As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles RadComboBoxACInclude.SelectedIndexChanged

        Dim da As New DataAccess
        Dim dt As DataTable

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("weightclass").ToString = "" Then
            da.GetAircraftTypeServiceSpecsByWeightClass(CInt(Session("carrierid")), "L")
        Else
            da.GetAircraftTypeServiceSpecsByWeightClass(CInt(Session("carrierid")), Session("weightclass").ToString)
        End If
        If Not isdtnullorempty(dt) Then
            If Not IsDBNull(dt.Rows(0).Item("currtype")) Then
                Session("currtype") = dt.Rows(0).Item("currtype")
            End If
        End If

    End Sub

    Protected Sub Address_ItemsRequested(ByVal o As Object, ByVal e As RadComboBoxItemsRequestedEventArgs)

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        Try

            AirTaxi.post_timing("QuoteTrip.aspx.vb OriginAddress_ItemsRequested start  " & Now.ToString)

            '20160617 - pab - fix screen hanging up after sitting idle
            If Session("email") Is Nothing Then
                Response.Redirect("CustomerLogin.aspx", True)
            End If

            Dim id As String = o.id

            loadairports(e.Text.Trim, id)

            Exit Sub

        Catch ex As Exception
            Dim serr As String = ex.Message
            If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(0, AirTaxi.appName, serr, "QuoteTrip.aspx OriginAddress_ItemsRequested", "")
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " QuoteTrip.aspx OriginAddress_ItemsRequested Error",
                              serr, CInt(Session("carrierid")))

        End Try

        'Me.OriginAddress.DataTextField = "airport"
        'Me.OriginAddress.DataValueField = "airport"
        'Me.OriginAddress.DataSource = dt
        'Me.OriginAddress.DataBind()
        ' Insert the first item.
        Me.OriginAddress.Items.Insert(0, New RadComboBoxItem("- Select airport below -"))

        AirTaxi.post_timing("QuoteTrip.aspx.vb OriginAddress_ItemsRequested end  " & Now.ToString)

    End Sub

    '20160125 - pab - fix airport dropdowns
    Sub loadairports(ByVal airport As String, ByVal comboname As String)

        Try
            Dim miles As Integer = 0
            Dim airportcount As Integer = 0
            Dim MinRunwayLength As Integer = 2800
            Dim fromloc As String = String.Empty
            Dim dt As New DataTable()
            Dim da As New DataAccess
            Dim ds As New DataSet
            Dim slocations As String = String.Empty

            timing = ""
            AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports start  " & Now.ToString)

            '20171121 - pab - fix carriers changing midstream - change to Session variables
            If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            miles = DataAccess.getsettingnumeric(CInt(Session("carrierid")), "AirportDistance")
            airportcount = DataAccess.getsettingnumeric(CInt(Session("carrierid")), "AirportListCount")
            MinRunwayLength = DataAccess.getsettingnumeric(CInt(Session("carrierid")), "MinRunwayLength")

            If miles = 0 Then miles = 50 '25 mi radius
            If airportcount = 0 Then airportcount = 25
            If MinRunwayLength = 0 Then MinRunwayLength = 2800

            If comboname = "OriginAddress" Then
                Me.OriginAddress.Items.Clear()
            ElseIf comboname = "DestinationAddress" Then
                Me.DestinationAddress.Items.Clear()
            End If

            Dim adapter As New SqlDataAdapter("", ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            fromloc = airport.ToUpper

            Select Case Len(airport)
                Case 1, 2
                    'too short to look up
                    Exit Sub

                Case 3
                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports Case 3 start  " & Now.ToString)
                    '20161031 - pab - drive time
                    adapter.SelectCommand.CommandText = ("SELECT  rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, " &
                        "rtrim(icao) as icao, rtrim(iata) as locationid , 0 as miles,cast(lat as float) as latitude,cast(long as float) as longitude from ICAO_IATA_2 i WHERE iata LIKE @text + '%'")

                    adapter.SelectCommand.Parameters.AddWithValue("@text", fromloc)
                    adapter.Fill(dt)
                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports Case 3 end  " & Now.ToString)

                Case 4
                    '                   	SELECT rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, rtrim(icao) as icao, rtrim(iata) as locationid , 0 as miles 
                    'from ICAO_IATA_2 i 
                    'WHERE icao = @text 
                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports Case 4 start  " & Now.ToString)
                    If Left(airport, 1).ToUpper = "K" Then
                        '20161031 - pab - drive time
                        adapter.SelectCommand.CommandText = ("SELECT  rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, " &
                        "rtrim(icao) as icao, rtrim(iata) as locationid , 0 as miles,cast(lat as float) as latitude,cast(long as float) as longitude from ICAO_IATA_2 WHERE icao  = @text")
                    Else
                        '20160228 - pab - add more caribbean countries
                        '20161025 - pab - add bermuda
                        '20161031 - pab - drive time
                        adapter.SelectCommand.CommandText = ("SELECT  rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, " &
                          "rtrim(icao) as icao, rtrim(iata) as locationid , 0 as miles,cast(lat as float) as latitude,cast(long as float) as longitude from ICAO_IATA_2 i left join NfdcFacilities nf on i.iata = nf.locationid " &
                          "WHERE (icao  LIKE '%' + @text + '%' OR iata LIKE @text + '%' OR name LIKE @text + '%') and iata <> ''  and (region <> 'INTL' or " &
                          "country in ('bahamas','VG','VI','BS','Jamaica','AN','AG','PR','CAN','CA','DO','HN','HT','JM','KY','TC','AG','AB','AN','GP','KN','LC','TT','VC','VG','MX','BM')) ")
                    End If

                    adapter.SelectCommand.Parameters.AddWithValue("@text", fromloc)
                    adapter.Fill(dt)
                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports Case 4 end  " & Now.ToString)

                Case Else
                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports Case Else start  " & Now.ToString)
                    'if this looks like a geographic place instead of an airport name ... 
                    '20161028 - drive time
                    dt = findairports(airport, MinRunwayLength, miles, airportcount, comboname)

                    If Not (IsNothing(dt)) Then

                        '20161031 - pab - fix double loading - done below
                        'additemradcombobox(dt, "", "", airportcount, comboname)
                        'If dt.Rows.Count < 6 Then Insertsys_log(_carrierid, appName, "findairports returned < 6 - from loc " & airport & _
                        '    "; MinRunwayLength " & MinRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "loadairports", _
                        '    "QuoteTrip.aspx.vb")

                    Else
                        If IsNumeric(airport.Trim) And Len(airport.Trim) = 5 Then
                            Dim ti As New Telerik.Web.UI.RadComboBoxItem
                            ti.Text = "Zip Code not found"
                            ti.Value = ""
                            Me.OriginAddress.Items.Add(ti)
                        End If
                    End If
                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports Case Else end  " & Now.ToString)

            End Select

            If Not isdtnullorempty(dt) Then
                If dt.Rows.Count > 0 Then
                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports dt.Rows.Count > 0 start  " & Now.ToString)

                    '20161031 - pab - drive time
                    '20170523 - pab - fix ddl not populating - different name
                    'If comboname = "RadComboBox1" Then
                    If comboname = "RadComboBox1" Or comboname = "OriginAddress" Then
                        If IsNothing(latLongorig) Then
                            Dim ll As New LatLong
                            ll.Latitude = dt.Rows(0).Item("latitude")
                            ll.Longitude = dt.Rows(0).Item("Longitude")
                            latLongorig = ll
                        End If
                    Else
                        If IsNothing(latLongdest) Then
                            Dim ll As New LatLong
                            ll.Latitude = dt.Rows(0).Item("latitude")
                            ll.Longitude = dt.Rows(0).Item("Longitude")
                            latLongdest = ll
                        End If
                    End If
                    If dt.Rows.Count = 1 Then
                        ds = da.GetAirportInformationByAirportCode(dt.Rows(0).Item("locationid"))
                        If Not IsNothing(ds) Then
                            If Not isdtnullorempty(ds.Tables(0)) Then
                                dt = da.GetMajorAirportsByLatitudeLongitude(ds.Tables(0).Rows(0).Item("Latitude"), ds.Tables(0).Rows(0).Item("Longitude"),
                                    MinRunwayLength, miles, airportcount)
                                If dt.Rows.Count < 6 Then Insertsys_log(CInt(Session("carrierid")), appName, "hardcoded script returned < 6 - to loc " & fromloc & "; lat " &
                                    ds.Tables(0).Rows(0).Item("Latitude") & "; long " & ds.Tables(0).Rows(0).Item("Longitude"), "loadairports",
                                    "QuoteTrip.aspx.vb")
                            End If
                        End If
                    End If

                    If dt.Rows.Count > 0 Then slocations = additemradcombobox(dt, slocations, fromloc, airportcount, comboname)

                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports dt.Rows.Count > 0 end  " & Now.ToString)

                End If

            Else
                AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports GetLocationLookupFail start  " & Now.ToString)

                '20160126 - pab - checked failed lookups, add if not found
                dt = da.GetLocationLookupFail(airport)
                If Not isdtnullorempty(dt) Then
                    If dt.Rows.Count > 0 Then
                        For i As Integer = 0 To dt.Rows.Count - 1
                            If dt.Rows(i).Item("latitude") <> 0 And dt.Rows(i).Item("longitude") <> 0 Then
                                dt = da.GetMajorAirportsByLatitudeLongitude(dt.Rows(i).Item("Latitude"), dt.Rows(i).Item("Longitude"),
                                    MinRunwayLength, miles, airportcount)
                                slocations = additemradcombobox(dt, slocations, fromloc, airportcount, comboname)
                                Exit For
                            End If
                        Next
                    Else
                        AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports InsertLocationLookupFail start  " & Now.ToString)
                        da.InsertLocationLookupFail(airport)
                    End If

                Else
                    AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports InsertLocationLookupFail start  " & Now.ToString)
                    da.InsertLocationLookupFail(airport)
                End If
                AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports GetLocationLookupFail end  " & Now.ToString)

            End If

            AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports end  " & Now.ToString)
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), AirTaxi.appName, timing, "QuoteTrip.aspx loadairports", "")

        Catch ex As Exception
            Dim serr As String = ex.Message
            If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), AirTaxi.appName, serr, "QuoteTrip.aspx loadairports", "")
            AirTaxi.SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " QuoteTrip.aspx loadairports Error",
                serr, CInt(Session("carrierid")))

        End Try

        If comboname = "OriginAddress" Then
            Me.OriginAddress.Items.Insert(0, New RadComboBoxItem("- Select airport below -"))
        ElseIf comboname = "DestinationAddress" Then
            Me.DestinationAddress.Items.Insert(0, New RadComboBoxItem("- Select airport below -"))
        End If

        AirTaxi.post_timing("QuoteTrip.aspx.vb loadairports end  " & Now.ToString)

    End Sub

    '20150511 - pab - fix airport list too short
    '20161028 - drive time
    Function findairports(ByVal a As String, ByVal minRunwayLength As Integer, ByVal miles As Integer, ByVal airportcount As Integer,
            ByVal comboname As String) As DataTable

        AirTaxi.post_timing("QuoteTrip.aspx.vb findairports start  " & Now.ToString)

        Dim omapping As New CoastalPortal.Mapping

        'System.Diagnostics.Debug.WriteLine("Check Orig Lat  " & Now.ToString)

        Dim latLong2 As New LatLong

        '20120621 - pab - default country to USA if not iata or icao
        'Dim b As String = String.Empty
        '20150427 - pab - fix spaces sometimes being converted - geocode mapping location invalid - boca%20raton
        If InStr(a, "%20") > 0 Then a = Replace(a, "%20", " ")
        Dim da As New DataAccess
        If Len(a.Trim) = 3 Then
            '20120831 - pab - fix APA (Centennial CO) not being found
            'b = fname(a)
            ''text passed in is not airport iata
            'If a = b Then a &= ", US"
            Dim ds As DataSet
            ds = da.GetAirportInformationByAirportCode(a.Trim.ToUpper)
            If Not IsNothing(ds) Then
                If Not isdtnullorempty(ds.Tables(0)) Then
                    latLong2.Latitude = CDbl(ds.Tables(0).Rows(0).Item("latitude"))
                    latLong2.Longitude = CDbl(ds.Tables(0).Rows(0).Item("longitude"))
                End If
            End If
        ElseIf Len(a.Trim) = 4 Then
            'If Left(a.Trim.ToUpper, 1) = "K" Then
            '    Dim da As New DataAccess
            '    Dim ds As DataSet = da.GetIATAcodebyICAO(a.Trim)
            '    If ds.Tables.Count > 0 Then
            '        If ds.Tables(0).Rows.Count > 0 Then
            '            'text passed in is icao
            '        Else
            '            a &= ", US"
            '        End If
            '    Else
            '        a &= ", US"
            '    End If
            'End If

            '20120831 - pab - fix APA (Centennial CO) not being found
            Dim dt As DataTable
            dt = da.GetAirportInformationByICAO(a.Trim.ToUpper)
            If Not isdtnullorempty(dt) Then
                latLong2.Latitude = CDbl(dt.Rows(0).Item("latitude"))
                latLong2.Longitude = CDbl(dt.Rows(0).Item("longitude"))
            End If

            '20150625 - pab - add zip code table lookup to reduce bing mapping issues
        ElseIf Len(a.Trim) = 5 And IsNumeric(a.Trim) Then
            Dim dt As DataTable
            dt = da.GetZIPCodes(a.Trim.ToUpper)
            If Not isdtnullorempty(dt) Then
                latLong2.Latitude = CDbl(dt.Rows(0).Item("latitude"))
                latLong2.Longitude = CDbl(dt.Rows(0).Item("longitude"))
            End If

        Else
            '20150424 - pab - fix locations not being found 
            If InStr(a.Trim, ",") > 0 Then
                'check for incomplete address entry
                Dim s As String = Mid(a, InStr(a.Trim, ",") + 1).Trim
                If Len(s) < 2 Then
                    Return Nothing
                End If

            ElseIf InStr(a.Trim, ",") = 0 And InStr(a.Trim, " ") = 0 Then
                '20120823 - pab - oakland, ca not showing up in ddl when US appended to text. location set to somewhere in IL
                'a &= ", US"
            End If
        End If


        '20120831 - pab - fix APA (Centennial CO) not being found
        'latLong2 = omapping.GeoCodeText(a)    'oMapping.GeocodeAddress(a.Trim, AddressCaptureVertical1.OriginCity.Trim, Me.AddressCaptureVertical1.OriginState, Me.AddressCaptureVertical1.OriginZip.Trim, Me.AddressCaptureVertical1.OriginCountry.Trim)
        If IsNothing(latLong2) Then
            latLong2 = omapping.GeoCodeText(a) 'oMapping.GeocodeAddress(a.Trim, AddressCaptureVertical1.OriginCity.Trim, Me.AddressCaptureVertical1.OriginState, Me.AddressCaptureVertical1.OriginZip.Trim, Me.AddressCaptureVertical1.OriginCountry.Trim)
        ElseIf latLong2.Latitude = 0 Then
            '20150625 - pab - add zip code table lookup to reduce bing mapping issues
            If Len(a.Trim) = 5 And IsNumeric(a.Trim) Then
                'assume invalid zip code and exit
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                Insertsys_log(CInt(Session("carrierid")), appName, "invalid zip code - " & a, "findairports", "QuoteTrip.aspx.vb")
                Return Nothing
            Else
                latLong2 = omapping.GeoCodeText(a) 'oMapping.GeocodeAddress(a.Trim, AddressCaptureVertical1.OriginCity.Trim, Me.AddressCaptureVertical1.OriginState, Me.AddressCaptureVertical1.OriginZip.Trim, Me.AddressCaptureVertical1.OriginCountry.Trim)
            End If
        End If

        '20090107 - pab - mappoint subscription error - don't return value
        If latLong2 Is Nothing Then
            '20150422 - pab - add logging - lookup is flaky
            Insertsys_log(CInt(Session("carrierid")), appName, "geocode mapping location not found - " & a &
                "; MinRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "findairports", "QuoteTrip.aspx.vb")

        Else
            If latLong2.Latitude = 0 Then
                '  Me.bttnGetFlights.Enabled = True
                '  Me.lblmsg.Text = "Is the originating city/state spelled correctly?"
                ' Me.lblmsg.Visible = True
                '  Me.AddressCaptureVertical1.Focus()

                '20150422 - pab - add logging - lookup is flaky
                Insertsys_log(CInt(Session("carrierid")), appName, "geocode mapping location invalid - " & a &
                    "; MinRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "findairports", "QuoteTrip.aspx.vb")

                Return Nothing 'Exit Function
            Else
                '20100622 - pab - fix floating point error
                latLong2.Latitude = Math.Round(latLong2.Latitude, 13)
                latLong2.Longitude = Math.Round(latLong2.Longitude, 13)
                'Session("origlat") = latLong2
                'Session("origplace") = a
                'System.Diagnostics.Debug.WriteLine(latLong2.ToString & "Orig Lat  " & Now.ToString)

            End If
        End If

        '20161028 - drive time
        '20170523 - pab - fix ddl not populating - different name
        'If comboname = "RadComboBox1" Then
        If comboname = "RadComboBox1" Or comboname = "OriginAddress" Then
            latLongorig = latLong2
        Else
            latLongdest = latLong2
        End If



        Dim o As New DataTable
        '20150511 - pab - fix airport list too short
        '20170623 - pab - fix object not set
        If Not IsNothing(latLong2) Then
            o = GetAirportsLatLong(latLong2, minRunwayLength, miles, airportcount)

            If isdtnullorempty(o) Then
                Insertsys_log(CInt(Session("carrierid")), appName, "GetAirportsLatLong returned no records - lat " & latLong2.Latitude.ToString & "; long " &
                latLong2.Longitude.ToString & "; MinRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount,
                "findairports", "QuoteTrip.aspx.vb")
            End If

        End If

        AirTaxi.post_timing("QuoteTrip.aspx.vb findairports end  " & Now.ToString)

        Return o


    End Function

    '20160125 - pab - fix airport dropdowns
    Function additemradcombobox(dt As DataTable, items As String, fromloc As String, airportcount As Integer, ByVal comboname As String) As String

        additemradcombobox = items

        '20161031 - pab - drive time
        Dim latlongto As New LatLong
        Dim drivetime As Integer = 0
        Dim wp As Integer = 0
        Dim dc As New DataColumn("drivetime", System.Type.GetType("System.Int32"))
        dt.Columns.Add(dc)
        For i = 0 To dt.Rows.Count - 1
            drivetime = 0
            '20161108 - tmc doesn't need drive time per David
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            If CInt(Session("carrierid")) <> 65 Then
                latlongto.Latitude = dt.Rows(i).Item("latitude")
                latlongto.Longitude = dt.Rows(i).Item("Longitude")
                '20170523 - pab - fix ddl not populating - different name
                'If comboname = "RadComboBox1" Then
                If comboname = "RadComboBox1" Or comboname = "OriginAddress" Then
                    If latLongorig.Latitude <> latlongto.Latitude Or latLongorig.Longitude <> latlongto.Longitude Then
                        '20170523 - pab - fix error - Operator '<>' is not defined for type 'DBNull' and string "".
                        If IsDBNull(dt.Rows(i).Item("locationid")) Then
                            drivetime = Mapping.GetDriveTime(latLongorig, latlongto, "")
                            '20161101 - pab - travel time too high per David
                        ElseIf dt.Rows(i).Item("locationid") <> "" Then
                            '20170901 - pab - fix distance if airport code entered
                            If dt.Rows(i).Item("locationid").ToString.ToUpper <> fromloc.ToUpper And
                                        dt.Rows(i).Item("icao").ToString.Trim.ToUpper <> fromloc.ToUpper Then
                                drivetime = Mapping.GetDriveTime(latLongorig, latlongto, dt.Rows(i).Item("locationid"))
                                '20161107 - pab - fix bad durations - ida airport code not found - returns ida, iowa
                                'If drivetime >= 9999999 Then
                                If drivetime >= 300 Then
                                    drivetime = Mapping.GetDriveTime(latLongorig, latlongto, "")
                                End If
                            End If
                        Else
                            drivetime = Mapping.GetDriveTime(latLongorig, latlongto, "")
                        End If
                    End If
                Else
                    If latLongdest.Latitude <> latlongto.Latitude Or latLongdest.Longitude <> latlongto.Longitude Then
                        '20170523 - pab - fix error - Operator '<>' is not defined for type 'DBNull' and string "".
                        If IsDBNull(dt.Rows(i).Item("locationid")) Then
                            drivetime = Mapping.GetDriveTime(latLongorig, latlongto, "")
                            '20161101 - pab - travel time too high per David
                        ElseIf dt.Rows(i).Item("locationid") <> "" Then
                            '20170901 - pab - fix distance if airport code entered
                            If dt.Rows(i).Item("locationid").ToString.ToUpper <> fromloc.ToUpper And
                                        dt.Rows(i).Item("icao").ToString.Trim.ToUpper <> fromloc.ToUpper Then
                                drivetime = Mapping.GetDriveTime(latLongdest, latlongto, dt.Rows(i).Item("locationid"))
                                '20161107 - pab - fix bad durations - ida airport code not found - returns ida, iowa
                                'If drivetime >= 9999999 Then
                                If drivetime >= 300 Then
                                    drivetime = Mapping.GetDriveTime(latLongdest, latlongto, "")
                                End If
                            End If
                        Else
                            drivetime = Mapping.GetDriveTime(latLongdest, latlongto, "")
                        End If
                    End If
                End If
            End If
            dt.Rows(i).Item("drivetime") = drivetime
        Next
        Dim sortExp As String = "drivetime,miles"
        Dim drarray() As DataRow
        drarray = dt.Select(Nothing, sortExp, DataViewRowState.CurrentRows)

        'For i = 0 To dt.Rows.Count - 1
        For i = 0 To drarray.Count - 1
            Dim dr As DataRow = drarray(i)
            Dim ti As New Telerik.Web.UI.RadComboBoxItem

            'If Trim(dt.Rows(i).Item("locationid").ToString).ToUpper = "07FA" Then
            '    ti.Text = Trim(dt.Rows(i).Item("facilityname").ToString) & " (" & Trim(dt.Rows(i).Item("locationid").ToString) & ")"
            '    ti.Value = "OCA"
            'ElseIf Trim(dt.Rows(i).Item("icao").ToString) = "" Then
            '    ti.Text = Trim(dt.Rows(i).Item("facilityname").ToString) & " (" & Trim(dt.Rows(i).Item("locationid").ToString) & ")"
            '    ti.Value = Trim(dt.Rows(i).Item("locationid").ToString)
            'Else
            '    ti.Text = Trim(dt.Rows(i).Item("facilityname").ToString) & " (" & Trim(dt.Rows(i).Item("icao").ToString) & ")"
            '    ti.Value = Trim(dt.Rows(i).Item("locationid").ToString)
            'End If
            If Trim(dr("locationid").ToString).ToUpper = "07FA" Then
                ti.Text = Trim(dr("facilityname").ToString) & " (" & Trim(dr("locationid").ToString) & ")"
                ti.Value = "OCA"
            ElseIf Trim(dr("icao").ToString) = "" Then
                ti.Text = Trim(dr("facilityname").ToString) & " (" & Trim(dr("locationid").ToString) & ")"
                ti.Value = Trim(dr("locationid").ToString)
            Else
                ti.Text = Trim(dr("facilityname").ToString) & " (" & Trim(dr("icao").ToString) & ")"
                ti.Value = Trim(dr("locationid").ToString)
            End If

            If dr("drivetime") > 6 Then
                ti.Text &= "; " & dr("drivetime") & " minutes drive time"
            End If

            If dt.Rows(i).Item("miles") > 3 Then
                ti.Text &= "; " & dt.Rows(i).Item("miles") & " air miles"
            End If
            ti.ForeColor = Drawing.Color.Black

            If InStr(additemradcombobox, ti.Value) = 0 Then
                '20160426 - pab - make like door2door
                'If comboname = "OriginAddress" Then
                '    If OriginAddress.Items.Count >= airportcount Then Exit For
                'ElseIf comboname = "DestinationAddress" Then
                '    If DestinationAddress.Items.Count >= airportcount Then Exit For
                'End If
                Select Case comboname
                    Case "OriginAddress"
                        If OriginAddress.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress2"
                        If OriginAddress2.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress3"
                        If OriginAddress3.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress4"
                        If OriginAddress4.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress5"
                        If OriginAddress5.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress6"
                        If OriginAddress6.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress7"
                        If OriginAddress7.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress8"
                        If OriginAddress8.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress9"
                        If OriginAddress9.Items.Count >= airportcount Then Exit For
                    Case "OriginAddress10"
                        If OriginAddress10.Items.Count >= airportcount Then Exit For

                    Case "DestinationAddress"
                        If DestinationAddress.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress2"
                        If DestinationAddress2.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress3"
                        If DestinationAddress3.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress4"
                        If DestinationAddress4.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress5"
                        If DestinationAddress5.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress6"
                        If DestinationAddress6.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress7"
                        If DestinationAddress7.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress8"
                        If DestinationAddress8.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress9"
                        If DestinationAddress9.Items.Count >= airportcount Then Exit For
                    Case "DestinationAddress10"
                        If DestinationAddress10.Items.Count >= airportcount Then Exit For
                End Select

                'If (InStr(dt.Rows(i).Item("icao").ToString, fromloc) > 0 Or InStr("(" & ti.Text & ")", fromloc) > 0) And fromloc <> "" Then
                '    ti.ForeColor = Drawing.Color.Red
                'End If
                If (InStr(dr("icao").ToString, fromloc) > 0 Or InStr("(" & ti.Text & ")", fromloc) > 0) And fromloc <> "" Then
                    ti.ForeColor = Drawing.Color.Red
                End If
                '20160426 - pab - make like door2door
                'If comboname = "OriginAddress" Then
                '    OriginAddress.Items.Add(ti)
                'ElseIf comboname = "DestinationAddress" Then
                '    DestinationAddress.Items.Add(ti)
                'End If
                Select Case comboname
                    Case "OriginAddress"
                        OriginAddress.Items.Add(ti)
                    Case "OriginAddress2"
                        OriginAddress2.Items.Add(ti)
                    Case "OriginAddress3"
                        OriginAddress3.Items.Add(ti)
                    Case "OriginAddress4"
                        OriginAddress4.Items.Add(ti)
                    Case "OriginAddress5"
                        OriginAddress5.Items.Add(ti)
                    Case "OriginAddress6"
                        OriginAddress6.Items.Add(ti)
                    Case "OriginAddress7"
                        OriginAddress7.Items.Add(ti)
                    Case "OriginAddress8"
                        OriginAddress8.Items.Add(ti)
                    Case "OriginAddress9"
                        OriginAddress9.Items.Add(ti)
                    Case "OriginAddress10"
                        OriginAddress10.Items.Add(ti)

                    Case "DestinationAddress"
                        DestinationAddress.Items.Add(ti)
                    Case "DestinationAddress2"
                        DestinationAddress2.Items.Add(ti)
                    Case "DestinationAddress3"
                        DestinationAddress3.Items.Add(ti)
                    Case "DestinationAddress4"
                        DestinationAddress4.Items.Add(ti)
                    Case "DestinationAddress5"
                        DestinationAddress5.Items.Add(ti)
                    Case "DestinationAddress6"
                        DestinationAddress6.Items.Add(ti)
                    Case "DestinationAddress7"
                        DestinationAddress7.Items.Add(ti)
                    Case "DestinationAddress8"
                        DestinationAddress8.Items.Add(ti)
                    Case "DestinationAddress9"
                        DestinationAddress9.Items.Add(ti)
                    Case "DestinationAddress10"
                        DestinationAddress10.Items.Add(ti)
                End Select

                additemradcombobox &= ti.Value & ";"
            End If
        Next

    End Function

    '20150511 - pab - fix airport list too short
    Private Function GetAirportsLatLong(ByVal latlong As LatLong, ByVal minRunwayLength As Integer, ByVal miles As Integer,
            ByVal airportcount As Integer) As DataTable

        AirTaxi.post_timing("QuoteTrip.aspx.vb getairports start  " & Now.ToString)

        Dim oMapping As New Mapping

        Dim ds As DataSet = Nothing

        Dim maxRunwayLength As Integer = 0

        Dim da As New DataAccess
        'find nearby airports
        '20150511 - pab - fix airport list too short
        ds = da.GetNearestAirportsByLatitudeLongitudeWithinDistance(latlong.Latitude, latlong.Longitude, minRunwayLength, miles, airportcount)

        Dim dt As New DataTable
        dt = ds.Tables(0)

        '20150507 - pab - add more logging for lookup issue
        If Not isdtnullorempty(dt) Then
            If dt.Rows.Count < 6 Then
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                Insertsys_log(CInt(Session("carrierid")), appName, "lookup returned  < 6 - lat " & latlong.Latitude & "; long " & latlong.Longitude &
                    "; minRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "GetAirportsLatLong", "QuoteTrip.aspx.vb")
            End If
        Else
            Insertsys_log(CInt(Session("carrierid")), appName, "lookup returned no results - lat " & latlong.Latitude & "; long " & latlong.Longitude &
                "; minRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "GetAirportsLatLong", "QuoteTrip.aspx.vb")
        End If

        AirTaxi.post_timing("QuoteTrip.aspx.vb getairports end  " & Now.ToString)

        Return dt

    End Function

    Protected Sub bttnRemoveLeg2_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg2.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg3_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg3.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg4_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg4.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg5_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg5.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg6_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg6.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg7_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg7.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg8_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg8.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg9_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg9.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg10_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg10.Click

        removeleg(sender, e, "")

    End Sub

    Sub removeleg(sender As Object, e As EventArgs, button As String)

        lblMsg.Text = ""

        Dim s As String
        If Not IsNothing(sender) Then
            s = sender.id.ToString
        Else
            s = button
        End If
        Select Case s
            Case "bttnRemoveLeg9"
                move10to9()
            Case "bttnRemoveLeg8"
                move9to8()
                move10to9()
            Case "bttnRemoveLeg7"
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg6"
                move7to6()
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg5"
                move6to5()
                move7to6()
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg4"
                move5to4()
                move6to5()
                move7to6()
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg3"
                move4to3()
                move5to4()
                move6to5()
                move7to6()
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg2"
                move3to2()
                move4to3()
                move5to4()
                move6to5()
                move7to6()
                move8to7()
                move9to8()
                move10to9()
        End Select

        If Me.pnlLeg10.Visible = True Then
            Me.pnlLeg10.Visible = False
        ElseIf Me.pnlLeg9.Visible = True Then
            Me.pnlLeg9.Visible = False
        ElseIf Me.pnlLeg8.Visible = True Then
            Me.pnlLeg8.Visible = False
        ElseIf Me.pnlLeg7.Visible = True Then
            Me.pnlLeg7.Visible = False
        ElseIf Me.pnlLeg6.Visible = True Then
            Me.pnlLeg6.Visible = False
        ElseIf Me.pnlLeg5.Visible = True Then
            Me.pnlLeg5.Visible = False
        ElseIf Me.pnlLeg4.Visible = True Then
            Me.pnlLeg4.Visible = False
        ElseIf Me.pnlLeg3.Visible = True Then
            Me.pnlLeg3.Visible = False
        ElseIf Me.pnlLeg2.Visible = True Then
            Me.pnlLeg2.Visible = False

            '20160429 - pab - change trip type if round trip or multi-leg
            If Session("triptype") <> "O" Then
                Session("triptype") = "O"
                Session("showcal") = "Y"

                Me.rblOneWayRoundTrip.SelectedIndex = 0
            End If

        End If

    End Sub

    Sub move10to9()

        Me.OriginAddress9.Text = Me.OriginAddress10.Text
        Me.DestinationAddress9.Text = Me.DestinationAddress9.Text
        Me.ddlPassengers9.Text = Me.ddlPassengers10.Text
        Me.depart_date9.SelectedDate = Me.depart_date10.SelectedDate
        Me.departtime_combo9.Text = Me.departtime_combo10.Text
        Me.OriginAddress10.Text = ""
        Me.DestinationAddress10.Text = ""
        Me.ddlPassengers10.Text = ""
        If Not IsNothing(Me.depart_date10.SelectedDate) Then
            Me.depart_date10.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date9.SelectedDate))
        Else
            Me.depart_date10.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo10.Text = "09:00 AM"

    End Sub

    Sub move9to8()

        Me.OriginAddress8.Text = Me.OriginAddress9.Text
        Me.DestinationAddress8.Text = Me.DestinationAddress9.Text
        Me.ddlPassengers8.Text = Me.ddlPassengers9.Text
        Me.depart_date8.SelectedDate = Me.depart_date9.SelectedDate
        Me.departtime_combo9.Text = Me.departtime_combo8.Text
        Me.OriginAddress9.Text = ""
        Me.DestinationAddress9.Text = ""
        Me.ddlPassengers9.Text = ""
        If Not IsNothing(Me.depart_date9.SelectedDate) Then
            Me.depart_date9.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date9.SelectedDate))
        Else
            Me.depart_date9.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo9.Text = "09:00 AM"

    End Sub

    Sub move8to7()

        Me.OriginAddress7.Text = Me.OriginAddress8.Text
        Me.DestinationAddress7.Text = Me.DestinationAddress8.Text
        Me.ddlPassengers7.Text = Me.ddlPassengers8.Text
        Me.depart_date7.SelectedDate = Me.depart_date8.SelectedDate
        Me.departtime_combo8.Text = Me.departtime_combo7.Text
        Me.OriginAddress8.Text = ""
        Me.DestinationAddress8.Text = ""
        Me.ddlPassengers8.Text = ""
        If Not IsNothing(Me.depart_date8.SelectedDate) Then
            Me.depart_date8.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date8.SelectedDate))
        Else
            Me.depart_date8.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo8.Text = "09:00 AM"

    End Sub

    Sub move7to6()

        Me.OriginAddress6.Text = Me.OriginAddress7.Text
        Me.DestinationAddress6.Text = Me.DestinationAddress7.Text
        Me.ddlPassengers6.Text = Me.ddlPassengers7.Text
        Me.depart_date6.SelectedDate = Me.depart_date7.SelectedDate
        Me.departtime_combo7.Text = Me.departtime_combo6.Text
        Me.OriginAddress7.Text = ""
        Me.DestinationAddress7.Text = ""
        Me.ddlPassengers7.Text = ""
        If Not IsNothing(Me.depart_date7.SelectedDate) Then
            Me.depart_date7.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date7.SelectedDate))
        Else
            Me.depart_date7.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo7.Text = "09:00 AM"

    End Sub

    Sub move6to5()

        Me.OriginAddress5.Text = Me.OriginAddress6.Text
        Me.DestinationAddress5.Text = Me.DestinationAddress6.Text
        Me.ddlPassengers5.Text = Me.ddlPassengers6.Text
        Me.depart_date5.SelectedDate = Me.depart_date6.SelectedDate
        Me.departtime_combo6.Text = Me.departtime_combo5.Text
        Me.OriginAddress6.Text = ""
        Me.DestinationAddress6.Text = ""
        Me.ddlPassengers6.Text = ""
        If Not IsNothing(Me.depart_date6.SelectedDate) Then
            Me.depart_date6.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date6.SelectedDate))
        Else
            Me.depart_date6.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo6.Text = "09:00 AM"

    End Sub

    Sub move5to4()

        Me.OriginAddress4.Text = Me.OriginAddress5.Text
        Me.DestinationAddress4.Text = Me.DestinationAddress5.Text
        Me.ddlPassengers4.Text = Me.ddlPassengers5.Text
        Me.depart_date4.SelectedDate = Me.depart_date5.SelectedDate
        Me.departtime_combo5.Text = Me.departtime_combo4.Text
        Me.OriginAddress5.Text = ""
        Me.DestinationAddress5.Text = ""
        Me.ddlPassengers5.Text = ""
        If Not IsNothing(Me.depart_date5.SelectedDate) Then
            Me.depart_date5.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date5.SelectedDate))
        Else
            Me.depart_date5.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo5.Text = "09:00 AM"

    End Sub

    Sub move4to3()

        Me.OriginAddress3.Text = Me.OriginAddress4.Text
        Me.DestinationAddress3.Text = Me.DestinationAddress4.Text
        Me.ddlPassengers3.Text = Me.ddlPassengers4.Text
        Me.depart_date3.SelectedDate = Me.depart_date4.SelectedDate
        Me.departtime_combo4.Text = Me.departtime_combo3.Text
        Me.OriginAddress4.Text = ""
        Me.DestinationAddress4.Text = ""
        Me.ddlPassengers4.Text = ""
        If Not IsNothing(Me.depart_date4.SelectedDate) Then
            Me.depart_date4.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date4.SelectedDate))
        Else
            Me.depart_date4.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo4.Text = "03:00 AM"

    End Sub

    Sub move3to2()

        Me.OriginAddress2.Text = Me.OriginAddress3.Text
        Me.DestinationAddress2.Text = Me.DestinationAddress3.Text
        Me.ddlPassengers2.Text = Me.ddlPassengers3.Text
        Me.depart_date2.SelectedDate = Me.depart_date3.SelectedDate
        Me.departtime_combo3.Text = Me.departtime_combo2.Text
        Me.OriginAddress3.Text = ""
        Me.DestinationAddress3.Text = ""
        Me.ddlPassengers3.Text = ""
        If Not IsNothing(Me.depart_date3.SelectedDate) Then
            Me.depart_date3.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date3.SelectedDate))
        Else
            Me.depart_date3.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo3.Text = "09:00 AM"

    End Sub

    Protected Sub bttnAddLeg_Click(sender As Object, e As EventArgs) Handles bttnAddLeg.Click

        Dim broundtrip As Boolean = False

        lblMsg.Text = ""

        If Me.pnlLeg10.Visible = True Then
            lblMsg.Text = "Maximum number of legs added"
            Exit Sub
        End If

        Dim sdate As Date
        Dim n As Integer = 0
        If Me.pnlLeg2.Visible = False Then
            If Me.DestinationAddress.SelectedValue.Trim <> "" Then
                Me.OriginAddress2.Text = Me.DestinationAddress.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress.Text.Trim <> "" Then
                Me.OriginAddress2.Text = Me.DestinationAddress.Text.Trim
            End If
            If Me.OriginAddress2.Text.Trim <> "" Then loadairports(Me.OriginAddress2.Text.Trim, "OriginAddress2")
            If Not IsNothing(Me.depart_date.SelectedDate) Then
                n = Me.departtime_combo.SelectedIndex
                sdate = nextlegdate(Me.depart_date.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date2.SelectedDate = sdate
            Me.departtime_combo2.SelectedIndex = n
            Me.pnlLeg2.Visible = True
            '20160913 - pab - fix radio button
            'broundtrip = True

        ElseIf Me.pnlLeg3.Visible = False Then
            If Me.DestinationAddress2.SelectedValue.Trim <> "" Then
                Me.OriginAddress3.Text = Me.DestinationAddress2.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress2.Text.Trim <> "" Then
                Me.OriginAddress3.Text = Me.DestinationAddress2.Text.Trim
            End If
            If Me.OriginAddress3.Text.Trim <> "" Then loadairports(Me.OriginAddress3.Text.Trim, "OriginAddress3")
            If Not IsNothing(Me.depart_date2.SelectedDate) Then
                n = Me.departtime_combo2.SelectedIndex
                sdate = nextlegdate(Me.depart_date2.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date3.SelectedDate = sdate
            Me.departtime_combo3.SelectedIndex = n
            Me.pnlLeg3.Visible = True

        ElseIf Me.pnlLeg4.Visible = False Then
            If Me.DestinationAddress3.SelectedValue.Trim <> "" Then
                Me.OriginAddress4.Text = Me.DestinationAddress3.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress3.Text.Trim <> "" Then
                Me.OriginAddress4.Text = Me.DestinationAddress3.Text.Trim
            End If
            If Me.OriginAddress4.Text.Trim <> "" Then loadairports(Me.OriginAddress4.Text.Trim, "OriginAddress4")
            If Not IsNothing(Me.depart_date3.SelectedDate) Then
                n = Me.departtime_combo3.SelectedIndex
                sdate = nextlegdate(Me.depart_date3.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date4.SelectedDate = sdate
            Me.departtime_combo4.SelectedIndex = n
            Me.pnlLeg4.Visible = True

        ElseIf Me.pnlLeg5.Visible = False Then
            If Me.DestinationAddress4.SelectedValue.Trim <> "" Then
                Me.OriginAddress5.Text = Me.DestinationAddress4.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress4.Text.Trim <> "" Then
                Me.OriginAddress5.Text = Me.DestinationAddress4.Text.Trim
            End If
            If Me.OriginAddress5.Text.Trim <> "" Then loadairports(Me.OriginAddress5.Text.Trim, "OriginAddress5")
            If Not IsNothing(Me.depart_date4.SelectedDate) Then
                n = Me.departtime_combo4.SelectedIndex
                sdate = nextlegdate(Me.depart_date4.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date5.SelectedDate = sdate
            Me.departtime_combo5.SelectedIndex = n
            Me.pnlLeg5.Visible = True

        ElseIf Me.pnlLeg6.Visible = False Then
            If Me.DestinationAddress5.SelectedValue.Trim <> "" Then
                Me.OriginAddress6.Text = Me.DestinationAddress5.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress5.Text.Trim <> "" Then
                Me.OriginAddress6.Text = Me.DestinationAddress5.Text.Trim
            End If
            If Me.OriginAddress6.Text.Trim <> "" Then loadairports(Me.OriginAddress6.Text.Trim, "OriginAddress6")
            If Not IsNothing(Me.depart_date5.SelectedDate) Then
                n = Me.departtime_combo5.SelectedIndex
                sdate = nextlegdate(Me.depart_date5.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date6.SelectedDate = sdate
            Me.departtime_combo6.SelectedIndex = n
            Me.pnlLeg6.Visible = True

        ElseIf Me.pnlLeg7.Visible = False Then
            If Me.DestinationAddress6.SelectedValue.Trim <> "" Then
                Me.OriginAddress7.Text = Me.DestinationAddress6.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress6.Text.Trim <> "" Then
                Me.OriginAddress7.Text = Me.DestinationAddress6.Text.Trim
            End If
            If Me.OriginAddress7.Text.Trim <> "" Then loadairports(Me.OriginAddress7.Text.Trim, "OriginAddress7")
            If Not IsNothing(Me.depart_date6.SelectedDate) Then
                n = Me.departtime_combo6.SelectedIndex
                sdate = nextlegdate(Me.depart_date6.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date7.SelectedDate = sdate
            Me.departtime_combo7.SelectedIndex = n
            Me.pnlLeg7.Visible = True

        ElseIf Me.pnlLeg8.Visible = False Then
            If Me.DestinationAddress7.SelectedValue.Trim <> "" Then
                Me.OriginAddress8.Text = Me.DestinationAddress7.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress7.Text.Trim <> "" Then
                Me.OriginAddress8.Text = Me.DestinationAddress7.Text.Trim
            End If
            If Me.OriginAddress8.Text.Trim <> "" Then loadairports(Me.OriginAddress8.Text.Trim, "OriginAddress8")
            If Not IsNothing(Me.depart_date7.SelectedDate) Then
                n = Me.departtime_combo7.SelectedIndex
                sdate = nextlegdate(Me.depart_date7.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date8.SelectedDate = sdate
            Me.departtime_combo8.SelectedIndex = n
            Me.pnlLeg8.Visible = True

        ElseIf Me.pnlLeg9.Visible = False Then
            If Me.DestinationAddress8.SelectedValue.Trim <> "" Then
                Me.OriginAddress9.Text = Me.DestinationAddress8.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress8.Text.Trim <> "" Then
                Me.OriginAddress9.Text = Me.DestinationAddress8.Text.Trim
            End If
            If Me.OriginAddress9.Text.Trim <> "" Then loadairports(Me.OriginAddress9.Text.Trim, "OriginAddress9")
            If Not IsNothing(Me.depart_date8.SelectedDate) Then
                n = Me.departtime_combo8.SelectedIndex
                sdate = nextlegdate(Me.depart_date8.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date9.SelectedDate = sdate
            Me.departtime_combo9.SelectedIndex = n
            Me.pnlLeg9.Visible = True

        ElseIf Me.pnlLeg10.Visible = False Then
            If Me.DestinationAddress9.SelectedValue.Trim <> "" Then
                Me.OriginAddress10.Text = Me.DestinationAddress9.SelectedValue.ToString.Trim
            ElseIf Me.DestinationAddress9.Text.Trim <> "" Then
                Me.OriginAddress10.Text = Me.DestinationAddress9.Text.Trim
            End If
            If Me.OriginAddress10.Text.Trim <> "" Then loadairports(Me.OriginAddress10.Text.Trim, "OriginAddress10")
            If Not IsNothing(Me.depart_date9.SelectedDate) Then
                n = Me.departtime_combo9.SelectedIndex
                sdate = nextlegdate(Me.depart_date9.SelectedDate, n)
            Else
                sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
                n = 37 ' "09:00 AM"
            End If
            Me.depart_date10.SelectedDate = sdate
            Me.departtime_combo10.SelectedIndex = n
            Me.pnlLeg10.Visible = True

        End If

        '20160429 - pab - change trip type if one way or round trip
        '20160913 - pab - fix radio button
        'If Session("triptype") <> "M" And broundtrip = False Then
        If Session("triptype") <> "M" Then
            Session("triptype") = "M"
            Session("showcal") = "Y"

            Me.rblOneWayRoundTrip.SelectedIndex = 2
        End If

    End Sub

    '20160808 - pab - fix next depart date
    Function nextlegdate(ByVal departdate As Date, ByRef timeindex As Integer) As Date

        nextlegdate = departdate

        If Not IsNothing(timeindex) Then
            'sdate = DateAdd(DateInterval.Hour, 8, CDate(Me.depart_date.SelectedDate & " " & Me.departtime_combo.SelectedValue))
            If timeindex + 32 <= 85 Then
                nextlegdate = departdate
                timeindex = timeindex + 32
            Else
                nextlegdate = DateAdd(DateInterval.Day, 1, CDate(departdate))
                timeindex = 37
            End If
        Else
            nextlegdate = DateAdd(DateInterval.Day, 1, CDate(departdate))
            timeindex = 37
        End If

    End Function

    Protected Sub cmdQuote_Click(sender As Object, e As EventArgs) Handles cmdQuote.Click

        lblMsg.Text = ""
        lblMsg.Visible = False

        Me.pnlQuote.Visible = True

        findflights()

    End Sub

    Protected Sub findflights()

        Dim fromairport As String
        Dim toairport As String
        '20120621 - pab - fix if iata code entered (did not selct from ddl)
        Dim fromaddr As String = String.Empty
        Dim toaddr As String = String.Empty
        Dim da As New DataAccess
        Dim ds As New DataSet
        Dim ld As String = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy")
        Dim rd As String = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy")
        Dim lt As String = "09:00 AM"
        Dim rt As String = "05:00 PM"
        Dim s As String = ""

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        Try

            '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            TurnAroundMinutes = CInt(da.GetSetting(CInt(Session("carrierid")), "TurnAroundMinutes"))

            '20160117 - pab - quote multi-leg trips
            lblMsg.Text = ""
            lblMsg.ForeColor = Color.Red
            lblMsg.Font.Bold = True
            lblMsg.Visible = False

            '20160622 - pab - do not show other messages pre David
            lblMsg1.Text = ""
            lblMsg1.ForeColor = Color.Red
            lblMsg1.Font.Bold = True
            lblMsg1.Visible = False

            '20160117 - pab - quote multi-leg trips
            'If Session("triptype") = "M" And lbLegs.Items.Count = 1 Then
            '    Session("triptype") = "O"
            'End If
            If Session("triptype") = "M" Then
                s = editflightsmultileg(TurnAroundMinutes)
                If s <> "" Then s = "&Legs=" & s
                's = ""
                'If lbLegs.Items.Count > 0 Then
                '    s = "&legs="
                '    For i As Integer = 0 To lbLegs.Items.Count - 1
                '        If i = 0 Then
                '            Dim s1 As String = lbLegs.Items(0).Value
                '            Session("A") = Left(s1, InStr(s1, " to ") - 1).Trim
                '            Session("B") = Mid(s1, InStr(s1, " to ") + 4, 4).Trim
                '            s1 = Mid(s1, InStr(s1, " at ") + 4).Trim
                '            If IsDate(s1) Then
                '                ld = CDate(s1).ToString("d")
                '                lt = CDate(s1).ToString("t")
                '                rd = DateAdd(DateInterval.Day, 1, CDate(ld)).ToString("d")
                '                rt = DateAdd(DateInterval.Hour, 8, CDate(rd)).ToString("t")
                '            End If
                '            fromairport = Session("A")
                '            toairport = Session("B")
                '        End If
                '        s &= lbLegs.Items(i).Value & ";"
                '    Next
                'End If
            Else
                Session("A") = Me.OriginAddress.Text
                Session("B") = Me.DestinationAddress.Text

                editflights(TurnAroundMinutes)

            End If

            If lblMsg.Visible = True Or lblMsg1.Visible = True Then
                Exit Sub
            End If

            If Me.OriginAddress.Text <> "" Then fromaddr = Me.OriginAddress.Text
            If Me.OriginAddress.SelectedValue <> "" Then
                fromairport = Me.OriginAddress.SelectedValue
            ElseIf Me.OriginAddress.Text <> "" Then
                fromairport = Me.OriginAddress.Text.ToUpper
            End If
            If Me.DestinationAddress.Text <> "" Then toaddr = Me.DestinationAddress.Text
            If Me.DestinationAddress.SelectedValue <> "" Then
                toairport = Me.DestinationAddress.SelectedValue
            ElseIf Me.DestinationAddress.Text <> "" Then
                toairport = Me.DestinationAddress.Text.ToUpper
            End If

            '20121204 - pab - implement aircraft to include
            'Session("achourly") = RadComboBoxACInclude.SelectedValue
            Dim actoinclude As String = String.Empty
            For i As Integer = 0 To RadComboBoxACInclude.CheckedItems.Count - 1
                If actoinclude <> "" Then actoinclude &= ","
                actoinclude &= RadComboBoxACInclude.CheckedItems(i).Value
            Next

            '20160117 - pab - suppress all but light, mid and heavy per David 1/15/2016
            '20171025 - pab - put turboprops and supermids back in
            If actoinclude = "" Then actoinclude &= "2,L,M,U,H"

            Session("achourly") = actoinclude

            Session("actype") = RadComboBoxACInclude.Text
            'Session("actypeexclude") = RadComboBoxACExclude.Text
            'Session("certifications") = RadComboBoxCertifications.Text

            'Session("flexto") = RadComboBoxFlexTo.Text
            'Session("flexfrom") = RadComboFlexFrom.Text
            Session("outdate") = Me.depart_date.SelectedDate
            Session("returndate") = Me.depart_date2.SelectedDate


            Dim pax As String = 1


            If Not (IsNothing(Me.ddlPassengers.Text)) Then
                If IsNumeric(Me.ddlPassengers.Text) Then
                    pax = CInt(Me.ddlPassengers.Text)
                End If
            End If


            '20120619 - pab - add filters for pets, smoking, wifi
            'Dim script As String = "QuoteTrip.aspx?" & _
            '                   "origAddr=" & fromairport & _
            '                   "&origCity=" & "" & _
            '                   "&origState=" & "" & _
            '                   "&origZip=" & "" & _
            '                   "&origCountry=" & "" & _
            '                   "&destAddr=" & toairport & _
            '                   "&destCity=" & "" & _
            '                   "&destState=" & "" & _
            '                   "&destZip=" & "" & _
            '                   "&destCountry=" & "" & _
            '                   "&roundTrip=" & "False" & _
            '                   "&passengers=" & pax & _
            '                   "&leaveDate=" & ld & _
            '                   "&returnDate=" & rd & _
            '                   "&leaveTime=" & lt & _
            '                   "&returnTime=" & rt & _
            '                   "&origAirportCode=" & fromairport & _
            '                   "&destAirportCode=" & toairport
            Dim sb As New StringBuilder()
            'Dim collection As IList(Of RadComboBoxItem) = RadComboBoxRequests.CheckedItems
            Dim bAllowPets As Boolean = False
            Dim bAllowSmoking As Boolean = False
            Dim bWiFi As Boolean = False

            '20120709 - pab - add lav, power
            Dim bLav As Boolean = False
            Dim bPower As Boolean = False

            '20131118 - pab - add more fields to aircraft
            Dim bInFlightEntertainment As Boolean = False

            '20130626 - pab - check parms before running quote
            Dim options As String = " and ("

            'For Each item As RadComboBoxItem In collection
            '    sb.Append(item.Text + "<br />")
            '    Select Case item.Text
            '        Case "Pets"
            '            bAllowPets = True

            '            '20130626 - pab - check parms before running quote
            '            If options = " and (" Then
            '                options &= " AllowPets = 1 "
            '            Else
            '                options &= " and AllowPets = 1 "
            '            End If

            '        Case "Smoking"
            '            bAllowSmoking = True

            '            '20130626 - pab - check parms before running quote
            '            If options = " and (" Then
            '                options &= " AllowSmoking = 1 "
            '            Else
            '                options &= " and AllowSmoking = 1 "
            '            End If

            '        Case "WiFi"
            '            bWiFi = True

            '            '20130626 - pab - check parms before running quote
            '            If options = " and (" Then
            '                options &= " WiFi = 1 "
            '            Else
            '                options &= " and WiFi = 1 "
            '            End If

            '            '20120709 - pab - add lav, power
            '        Case "Enclosed Lav"
            '            bLav = True

            '            '20130626 - pab - check parms before running quote
            '            If options = " and (" Then
            '                options &= " EnclosedLav = 1 "
            '            Else
            '                options &= " and EnclosedLav = 1 "
            '            End If

            '        Case "Power"
            '            bPower = True

            '            '20130626 - pab - check parms before running quote
            '            If options = " and (" Then
            '                options &= " PowerAvailable = 1 "
            '            Else
            '                options &= " and PowerAvailable = 1 "
            '            End If

            '            '20131118 - pab - add more fields to aircraft
            '        Case "InFlight Entertainment"
            '            bInFlightEntertainment = True
            '            If options = " and (" Then
            '                options &= " InflightEntertainment = 1 "
            '            Else
            '                options &= " and InflightEntertainment = 1 "
            '            End If

            '    End Select
            'Next

            '20130626 - pab - check parms before running quote
            If options = " and (" Then
                options = ""
            Else
                options &= ")"
            End If

            Dim sb2 As New StringBuilder()
            Dim collection2 As IList(Of RadComboBoxItem) = RadComboBoxACInclude.CheckedItems
            Dim weightclass As String = " and WeightClass in ("
            Dim ratings As String = ""

            For Each item As RadComboBoxItem In collection2
                sb2.Append(item.Text + "<br />")

                If weightclass = " and WeightClass in (" Then
                    weightclass &= " '" & item.Value & "'"
                Else
                    weightclass &= ", '" & item.Value & "'"
                End If

            Next
            If weightclass = " and WeightClass in (" Then
                weightclass = ""
            Else
                weightclass &= ")"
            End If

            'If Not (IsNothing(RadComboBoxCertifications.SelectedItem)) Then
            '    Select Case RadComboBoxCertifications.SelectedItem.Text
            '        Case "ARGUS Gold/Wyvern Registered/IS-BAO Stage I"
            '            ratings = " and (ARGUSlevel <> ''  or WYVERNlevel <> '' or Sentientlevel <> '' or ISBAOlevel <> '')"
            '        Case "ARGUS Gold Plus/IS-BAO Stage II"
            '            ratings = " and (ARGUSlevel in ('ARG/US Gold Plus', 'ARG/US Platinum') or WYVERNlevel <> '' or Sentientlevel <> '' or ISBAOlevel in ('IS-BAO Stage Two', 'IS-BAO Stage Three'))"
            '        Case "ARGUS Platinum/Wyvern Wingman/IS-BAO Stage III/Sentient Approved"
            '            ratings = " and (ARGUSlevel in ('ARG/US Platinum') or WYVERNlevel in ('WYVERN Wingman') or Sentientlevel in ('Sentient Certified') or ISBAOlevel in ('IS-BAO Stage Three'))"
            '        Case Else
            '            ratings = ""
            '    End Select
            'End If

            '20131118 - pab - add more fields to aircraft
            Dim ManufactureDate As Date
            'If Not (IsNothing(RadComboBoxMfcDates.SelectedItem)) Then
            '    Select Case RadComboBoxMfcDates.SelectedItem.Text
            '        Case "Any"
            '            'ok - do nothing
            '        Case "< 5 Years"
            '            ManufactureDate = DateAdd(DateInterval.Year, -5, Now)
            '        Case "< 10 Years"
            '            ManufactureDate = DateAdd(DateInterval.Year, -10, Now)
            '        Case "< 15 Years"
            '            ManufactureDate = DateAdd(DateInterval.Year, -15, Now)
            '        Case "< 20 Years"
            '            ManufactureDate = DateAdd(DateInterval.Year, -20, Now)
            '    End Select
            'End If

            '20120626 - pab - enable round trips - was originaly hardcoded one-way only
            Dim roundTrip As String = "False"
            '20120628 - pab - add one way/round trip radio buttons
            'If minuteswaitbetweensegments > 0 Then
            If Me.rblOneWayRoundTrip.SelectedValue = "RoundTrip" Then
                roundTrip = "True"
            End If


            '20130626 - pab - check parms before running quote
            '20131118 - pab - add more fields to aircraft
            Dim dsparms As DataSet = da.CheckQuoteParms(fromairport, toairport, CInt(pax), weightclass, options, ratings, ManufactureDate)
            '20151214 - pab - fix intermittant sql errors
            If IsNothing(dsparms) Then
                Insertsys_log(CInt(Session("carrierid")), appName, "findflights CheckQuoteParms first lookup returned empty dataset - fromairport " & fromairport &
                    "; toairport " & toairport & "; pax " & pax.ToString & "; weightclass " & weightclass & "; options " & options & "; ratings " & ratings &
                    "; ManufactureDate " & ManufactureDate, "findflights", "QuoteTrip.aspx.vb")
                dsparms = da.CheckQuoteParms(fromairport, toairport, CInt(pax), weightclass, options, ratings, CDate("0001-01-01"))
            ElseIf dsparms.Tables.Count < 6 Then
                Insertsys_log(CInt(Session("carrierid")), appName, "findflights CheckQuoteParms first lookup returned partial dataset - fromairport " & fromairport &
                    "; toairport " & toairport & "; pax " & pax.ToString & "; weightclass " & weightclass & "; options " & options & "; ratings " & ratings &
                    "; ManufactureDate " & ManufactureDate & "; table count " & dsparms.Tables.Count.ToString, "findflights", "QuoteTrip.aspx.vb")
                dsparms = da.CheckQuoteParms(fromairport, toairport, CInt(pax), weightclass, options, ratings, CDate("0001-01-01"))
            End If
            If IsNothing(dsparms) Then
                If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                '20151214 - pab - fix intermittant sql errors
                'lblmsg.Text &= "The from or to airport is not serviced. Please select other airports."
                lblMsg.Text &= "Please update search criteria and try again."
                Insertsys_log(CInt(Session("carrierid")), appName, "findflights CheckQuoteParms second lookup returned empty dataset - fromairport " & fromairport &
                    "; toairport " & toairport & "; pax " & pax.ToString & "; weightclass " & weightclass & "; options " & options & "; ratings " & ratings &
                    "; ManufactureDate " & ManufactureDate, "findflights", "QuoteTrip.aspx.vb")
            ElseIf dsparms.Tables.Count < 6 Then
                '20151214 - pab - fix intermittant sql errors
                If lblMsg.Text <> "" Then
                    If InStr(lblMsg.Text, "update search criteria") = 0 Then
                        lblMsg.Text &= "<br />"
                        lblMsg.Text &= "Please update search criteria and try again."
                        Insertsys_log(CInt(Session("carrierid")), appName, "findflights CheckQuoteParms second lookup returned partial dataset - fromairport " & fromairport &
                            "; toairport " & toairport & "; pax " & pax.ToString & "; weightclass " & weightclass & "; options " & options & "; ratings " & ratings &
                            "; ManufactureDate " & ManufactureDate & "; table count " & dsparms.Tables.Count.ToString, "findflights", "QuoteTrip.aspx.vb")
                    End If
                End If
            Else
                'check if service airports
                '20151214 - pab - fix intermittant sql errors
                'If dsparms.Tables(0).Rows.Count <= 0 Then
                If dsparms.Tables(0).Rows.Count <= 0 Or dsparms.Tables(0).Rows(0).Item("TableType").ToString.ToLower <> "Airports".ToLower Then
                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                    lblMsg.Text &= "The from and to airports are not serviced. Please select other airports."
                Else
                    Dim bfrom As Boolean = False
                    Dim bto As Boolean = False
                    For i = 0 To dsparms.Tables(0).Rows.Count - 1
                        If dsparms.Tables(0).Rows(i).Item("locationid").ToString.Trim = fromairport Then
                            bfrom = True
                            Exit For
                        End If
                    Next
                    For i = 0 To dsparms.Tables(0).Rows.Count - 1
                        If dsparms.Tables(0).Rows(i).Item("locationid").ToString.Trim = toairport Then
                            bto = True
                            Exit For
                        End If
                    Next
                    If bfrom = False And bto = False Then
                        If lblMsg1.Text <> "" Then lblMsg.Text &= "<br />"
                        lblMsg1.Text &= "The from and to airports are not serviced. Please select other airports."
                        '20160622 - pab - do not show other messages pre David
                        lblMsg1.Visible = True
                        Exit Sub

                    ElseIf bfrom = False Then
                        If lblMsg1.Text <> "" Then lblMsg.Text &= "<br />"
                        lblMsg1.Text &= "The from airport is not serviced. Please select another departure airport."
                        '20160622 - pab - do not show other messages pre David
                        lblMsg1.Visible = True
                        Exit Sub

                    ElseIf bto = False Then
                        If lblMsg1.Text <> "" Then lblMsg.Text &= "<br />"
                        lblMsg1.Text &= "The to airport is not serviced. Please select another arrival airport."
                        '20160622 - pab - do not show other messages pre David
                        lblMsg1.Visible = True
                        Exit Sub

                    End If
                End If

                'check runway lengths
                '20151214 - pab - fix intermittant sql errors
                'If dsparms.Tables(1).Rows.Count <= 0 Then
                If dsparms.Tables(1).Rows.Count <= 0 Or dsparms.Tables(1).Rows(0).Item("TableType").ToString.ToLower <> "Runways".ToLower Then
                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                    lblMsg.Text &= "The runways at the from and to airports are too short. Please select other airports."
                Else
                    Dim bfrom As Boolean = False
                    Dim bto As Boolean = False
                    For i = 0 To dsparms.Tables(1).Rows.Count - 1
                        If dsparms.Tables(1).Rows(i).Item("locationid").ToString.Trim = fromairport Then
                            bfrom = True
                            Exit For
                        End If
                    Next
                    For i = 0 To dsparms.Tables(1).Rows.Count - 1
                        If dsparms.Tables(1).Rows(i).Item("locationid").ToString.Trim = toairport Then
                            bto = True
                            Exit For
                        End If
                    Next
                    If bfrom = False And bto = False Then
                        If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                        lblMsg.Text &= "The runways at the from and to airports are too short. Please select other airports."
                    ElseIf bfrom = False Then
                        If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                        lblMsg.Text &= "The runway at the from airport is too short. Please select another departure airport."
                    ElseIf bto = False Then
                        If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                        lblMsg.Text &= "The runway at the to airport is too short. Please select another arrival airport."
                    End If
                End If

                'check passengers
                '20140416 - pab - remove this message 
                'If dsparms.Tables(2).Rows.Count <= 0 Then
                '    If lblmsg.Text <> "" Then lblmsg.Text &= "<br />"
                '    If weightclass <> "" Then
                '        lblmsg.Text &= "Passenger count exceeds passenger capacity of selected aircraft types. Please adjust passenger count or select larger aircraft types."
                '    Else
                '        lblmsg.Text &= "Passenger count exceeds passenger capacity of fleet. Please adjust passenger count."
                '    End If
                'End If

                'check weight classes
                '20151214 - pab - fix intermittant sql errors
                'If dsparms.Tables(3).Rows.Count <= 0 Then
                If dsparms.Tables(3).Rows.Count <= 0 Or dsparms.Tables(3).Rows(0).Item("TableType").ToString.ToLower <> "WeightClass".ToLower Then
                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                    If weightclass <> "" Then
                        lblMsg.Text &= "There are no aircraft of selected types available. Please change optional aircraft types to include."
                    Else
                        lblMsg.Text &= "There are no aircraft of any type available."
                    End If
                End If

                'check ratings
                '20151214 - pab - fix intermittant sql errors
                'If dsparms.Tables(5).Rows.Count <= 0 Then
                If dsparms.Tables(5).Rows.Count <= 0 Or dsparms.Tables(5).Rows(0).Item("TableType").ToString.ToLower <> "ratings".ToLower Then
                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                    If options <> "" Then
                        lblMsg.Text &= "There are no aircraft with selected certifications available. Please change certifications."
                    Else
                        lblMsg.Text &= "There are no aircraft with any certifications available."
                    End If
                End If

                'check options
                '20151214 - pab - fix intermittant sql errors
                'If dsparms.Tables(4).Rows.Count <= 0 Then
                If dsparms.Tables(4).Rows.Count <= 0 Or dsparms.Tables(4).Rows(0).Item("TableType").ToString.ToLower <> "services".ToLower Then
                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
                    If options <> "" Then
                        lblMsg.Text &= "There are no aircraft with selected options available. Please change optional services."
                    Else
                        lblMsg.Text &= "There are no aircraft with any options available."
                    End If
                End If

            End If

            If lblMsg.Text <> "" Then
                lblMsg.Visible = True
                Exit Sub
            End If

            '20160490 - pab - set session variables
            Session("reset") = "x"
            Session("passengers") = pax
            Session("origairportcode") = fromairport
            Session("destairportcode") = toairport
            Session("weightclass") = weightclass

            Dim dt As DataTable
            If weightclass = "" Then
                da.GetAircraftTypeServiceSpecsByWeightClass(CInt(Session("carrierid")), "L")
            Else
                da.GetAircraftTypeServiceSpecsByWeightClass(CInt(Session("carrierid")), weightclass)
            End If
            If Not isdtnullorempty(dt) Then
                If Not IsDBNull(dt.Rows(0).Item("currtype")) Then
                    Session("currtype") = dt.Rows(0).Item("currtype")
                End If
            End If

            'for testing only 
            '_acg = "2"

            '20140310 - pab - use table instead of parms
            '20150317 - pab - remove acg branding
            'Dim bacg As Boolean = False
            'If _acg = "1" Or _acg = "2" Then bacg = True
            'Dim qn As Integer = da.InsertQuoteRequests(_carrierid, CInt(pax), fromaddr, toaddr, CBool(roundTrip), CDate(ld), CDate(rd), _
            '    CDate(lt).ToLongTimeString, CDate(rt).ToLongTimeString, fromairport, toairport, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, _
            '    bInFlightEntertainment, ManufactureDate, Nothing, bacg)
            'If qn > 0 Then

            '20160511 - pab - fix date reverting back to tomorrow
            ld = CDate(Me.depart_date.SelectedDate).ToShortDateString
            lt = departtime_combo.SelectedValue
            '20160716 - pab - fix error - Nullable object must have a value
            If Not IsNothing(Me.depart_date2.SelectedDate) Then
                rd = CDate(Me.depart_date2.SelectedDate).ToShortDateString
                rt = departtime_combo2.SelectedValue
            Else
                rd = CDate(DateAdd(DateInterval.Day, 1, CDate(ld))).ToShortDateString
                rt = "05:00 PM"
            End If

            '20120709 - pab - add lav, power
            '20131118 - pab - add more fields to aircraft - InflightEntertainment, ManufactureDate
            '20140310 - pab - acg background image
            '20140310 - pab - use table instead of parms
            '20150317 - pab - remove acg branding
            '20160117 - pab - quote multi-leg trips
            Dim script As String = "QuoteTrip.aspx?" &
                               "origAddr=" & fromaddr &
                               "&destAddr=" & toaddr &
                               "&roundTrip=" & roundTrip &
                               "&passengers=" & pax &
                               "&leaveDate=" & ld &
                               "&returnDate=" & rd &
                               "&leaveTime=" & lt &
                               "&returnTime=" & rt &
                               "&origAirportCode=" & fromairport &
                               "&destAirportCode=" & toairport &
                               "&AllowPets=" & bAllowPets.GetHashCode &
                               "&AllowSmoking=" & bAllowSmoking.GetHashCode &
                               "&WiFi=" & bWiFi.GetHashCode &
                               "&EnclosedLav=" & bLav.GetHashCode &
                               "&PowerAvailable=" & bPower.GetHashCode &
                               "&InflightEntertainment=" & bInFlightEntertainment.GetHashCode &
                               "&ManufactureDate=" & ManufactureDate &
                                s
            '"&acg=" & _acg
            '    Dim script As String = "QuoteTrip.aspx?" & _
            '            "qn=" & qn & _
            '            "&acg=" & _acg

            '20150330 - pab - force logon before getting quotes
            Dim bIsLoggedIn As Boolean = False
            Dim email As String = String.Empty
            If Not IsNothing(Session("IsLoggedIn")) Then bIsLoggedIn = CBool(Session("IsLoggedIn"))
            If Not IsNothing(Session("email")) Then email = Session("email").ToString
            If bIsLoggedIn = True And email <> "" Then
                Response.Redirect(script)
            Else
                script = Replace(script, "&", "||")
                Response.Redirect("customerLogin.aspx?ReturnUrl=" & script)
            End If

            'End If

        Catch ex As Exception
            Dim serr As String = ex.Message
            If serr <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.Insertsys_log(0, AirTaxi.appName, serr, "QuoteTrip.aspx findflights", "")
                AirTaxi.SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " QuoteTrip.aspx findflights Error",
                                  serr, CInt(Session("carrierid")))
            End If

        End Try

    End Sub

    '20160117 - pab - quote multi-leg trips
    Sub editflights(ByRef TurnAroundMinutes As Integer)

        Dim ld As String = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy")
        Dim rd As String = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy")
        Dim lt As String = "09:00 AM"
        Dim rt As String = "05:00 PM"

        Dim ma As Integer = 120
        Dim d As Integer = 0
        Dim bintl As Boolean = False
        Dim leavedate As DateTime
        Dim returndate As DateTime

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        If Not (IsNothing(Me.depart_date.SelectedDate)) Then
            If IsDate(Me.depart_date.SelectedDate.Value) Then
                leavedate = CDate(Me.depart_date.SelectedDate.Value & " " & Me.departtime_combo.SelectedValue)
                ld = leavedate.ToString("MM/dd/yyyy")
                lt = Me.departtime_combo.SelectedValue

                '20141114 - pab - do not allow date less than current date
                If leavedate <= Now Then
                    lblMsg.Text = "Depart Date must be in the future"
                    lblMsg.Visible = True

                    Exit Sub
                End If
            End If
        End If

        '20120626 - pab - edit return date
        Dim minuteswaitbetweensegments As Long = 0

        If Not (IsNothing(Me.depart_date2.SelectedDate)) Then
            If IsDate(Me.depart_date2.SelectedDate.Value) Then
                returndate = CDate(Me.depart_date2.SelectedDate.Value & " " & Me.departtime_combo2.SelectedValue)
                rd = returndate.ToString("MM/dd/yyyy")
                rt = Me.departtime_combo2.SelectedValue

                '20120626 - pab - edit return date
                If Session("triptype") <> "O" Then
                    If IsDate(rd & " " & rt) Then
                        minuteswaitbetweensegments = DateDiff(DateInterval.Minute, CDate(ld & " " & lt), CDate(rd & " " & rt))
                        If minuteswaitbetweensegments <= 0 Then
                            lblMsg.Text = "Return Date must be after Depart Date"
                            lblMsg.Visible = True

                            Exit Sub
                        End If
                    End If
                End If

            End If
        End If

        Dim fromairport As String
        Dim toairport As String
        '20120621 - pab - fix if iata code entered (did not selct from ddl)
        Dim fromaddr As String = String.Empty
        Dim toaddr As String = String.Empty
        Dim da As New DataAccess
        Dim ds As New DataSet

        '20120830 - pab - fix error when back button on QuoteTrip clicked and ueser returns here
        If Me.OriginAddress.Text.Trim <> "" Then
            If InStr(Me.OriginAddress.Text, "(") = 0 Then
                fromaddr = Me.OriginAddress.Text
                '20120824 - pab - fix if airport not selected from ddl
                If fname(fromaddr) <> fromaddr Then
                    fromairport = fromaddr.ToUpper
                Else
                    ds = da.GetIATAcodebyICAO(fromaddr)
                    If Not IsNothing(ds) Then
                        If Not isdtnullorempty(ds.Tables(0)) Then
                            fromairport = ds.Tables(0).Rows(0).Item("iata").ToString.Trim
                        Else
                            lblMsg.Text &= "Please select from airport. "
                            lblMsg.Visible = True

                        End If
                    End If
                End If
            Else
                'fromairport = InBetween(1, Me.OriginAddress.Text, "(K", ")")
                fromairport = Me.OriginAddress.SelectedValue
                fromaddr = Me.OriginAddress.Text
            End If
        Else
            lblMsg.Text &= "Enter from location. "
            lblMsg.Visible = True

        End If

        '20120830 - pab - fix error when back button on QuoteTrip clicked and ueser returns here
        If Me.DestinationAddress.Text.Trim <> "" Then
            If InStr(Me.DestinationAddress.Text, "(") = 0 Then
                toaddr = Me.DestinationAddress.Text
                '20120824 - pab - fix if airport not selected from ddl
                If fname(toaddr) <> toaddr Then
                    toairport = toaddr.ToUpper
                Else
                    ds = da.GetIATAcodebyICAO(toaddr)
                    If Not IsNothing(ds) Then
                        If Not isdtnullorempty(ds.Tables(0)) Then
                            toairport = ds.Tables(0).Rows(0).Item("iata").ToString.Trim
                        Else
                            lblMsg.Text &= "Please select to airport. "
                            lblMsg.Visible = True

                        End If
                    End If
                End If
            Else
                'toairport = InBetween(1, Me.DestinationAddress.Text, "(K", ")")
                toairport = Me.DestinationAddress.SelectedValue
                toaddr = Me.DestinationAddress.Text
            End If
        Else
            lblMsg.Text &= "Enter to location. "
            lblMsg.Visible = True

        End If

        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
        ma += TurnAroundMinutes    'add turn time
        If Session("triptype") <> "O" Then
            If returndate < DateAdd(DateInterval.Minute, ma, leavedate) Then
                '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
                'If Session("triptype") = "R" Then
                '    lblMsg.Text = "Round trip cannot be performed in the requested time frame"
                'Else
                '    lblMsg.Text = "Multi-leg trip cannot be performed in the requested time frame"
                'End If
                'lblMsg.Visible = True
                'Exit Sub
                Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leavedate)
                If DateDiff(DateInterval.Day, newdate, returndate) > 0 Then
                    Me.depart_date2.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, returndate),
                        CDate(Me.depart_date2.SelectedDate))
                End If
                For i As Integer = 0 To departtime_combo2.Items.Count - 1
                    If IsDate(departtime_combo2.Items(i).Value) Then
                        If CDate(CDate(departtime_combo2.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                            departtime_combo2.SelectedValue = departtime_combo2.Items(i).Value
                            Exit For
                        End If
                    End If
                Next
            End If
        End If

        '20150424 - pab - fix locations not being found 
        If lblMsg.Text <> "" Then
            lblMsg.Visible = True
            Exit Sub
        End If

    End Sub

    '20160117 - pab - quote multi-leg trips
    Function editflightsmultileg(ByRef TurnAroundMinutes As Integer) As String

        Dim prevairport As String
        Dim fromairport As String
        Dim toairport As String
        Dim leave1 As DateTime
        Dim leave2 As DateTime
        Dim s As String = ""

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        editflightsmultileg = ""

        editflights(TurnAroundMinutes)
        If lblMsg.Text <> "" Then Exit Function

        leave1 = CDate(depart_date.SelectedDate & " " & departtime_combo.SelectedValue)

        If Me.OriginAddress.SelectedValue.Trim <> "" Then
            editflightsmultileg = Me.OriginAddress.SelectedValue.Trim
        Else
            editflightsmultileg = Me.OriginAddress.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress.Text.Trim
        End If
        editflightsmultileg &= " at " & leave1 & ";"

        If Me.pnlLeg2.Visible = False Then Exit Function

        If Me.OriginAddress2.Text.Trim = "" And Me.OriginAddress2.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress2.Text.Trim = "" And Me.DestinationAddress2.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date2.SelectedDate & " " & departtime_combo2.SelectedValue)

        '20120626 - pab - edit return date
        Dim minuteswaitbetweensegments As Long = 0
        '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress2.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress2.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress2.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress2.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress2.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress2.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        If Me.pnlLeg3.Visible = False Then Exit Function

        If Me.OriginAddress3.Text.Trim = "" And Me.OriginAddress3.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress3.Text.Trim = "" And Me.DestinationAddress3.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date3.SelectedDate & " " & departtime_combo3.SelectedValue)
        Dim d As Double = 0
        Dim ma As Integer = 0
        Dim bintl As Boolean = False
        Dim da As New DataAccess
        If Me.OriginAddress3.SelectedValue.Trim = "" Then
            fromairport = Me.OriginAddress3.Text.Trim
        Else
            fromairport = Me.OriginAddress3.SelectedValue.Trim
        End If
        If Me.DestinationAddress3.SelectedValue.Trim = "" Then
            toairport = Me.DestinationAddress3.Text.Trim
        Else
            toairport = Me.DestinationAddress3.SelectedValue.Trim
        End If
        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        ma += TurnAroundMinutes    'add turn time
        If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
            If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
                Me.depart_date3.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2),
                    CDate(Me.depart_date3.SelectedDate))
            End If
            For i As Integer = 0 To departtime_combo3.Items.Count - 1
                If IsDate(departtime_combo3.Items(i).Value) Then
                    If CDate(CDate(departtime_combo3.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                        departtime_combo3.SelectedValue = departtime_combo3.Items(i).Value
                        Exit For
                    End If
                End If
            Next

            leave2 = CDate(Me.depart_date3.SelectedDate & " " & departtime_combo3.SelectedValue)
        End If

        'minuteswaitbetweensegments = 0
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress3.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress3.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress3.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress3.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress3.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress3.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        If Me.pnlLeg4.Visible = False Then Exit Function

        If Me.OriginAddress4.Text.Trim = "" And Me.OriginAddress4.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress4.Text.Trim = "" And Me.DestinationAddress4.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date4.SelectedDate & " " & departtime_combo4.SelectedValue)
        If Me.OriginAddress4.SelectedValue.Trim = "" Then
            fromairport = Me.OriginAddress4.Text.Trim
        Else
            fromairport = Me.OriginAddress4.SelectedValue.Trim
        End If
        If Me.DestinationAddress4.SelectedValue.Trim = "" Then
            toairport = Me.DestinationAddress4.Text.Trim
        Else
            toairport = Me.DestinationAddress4.SelectedValue.Trim
        End If
        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        ma += TurnAroundMinutes    'add turn time
        If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
            If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
                Me.depart_date4.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2),
                    CDate(Me.depart_date4.SelectedDate))
            End If
            For i As Integer = 0 To departtime_combo4.Items.Count - 1
                If IsDate(departtime_combo4.Items(i).Value) Then
                    If CDate(CDate(departtime_combo4.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                        departtime_combo4.SelectedValue = departtime_combo4.Items(i).Value
                        Exit For
                    End If
                End If
            Next

            leave2 = CDate(Me.depart_date4.SelectedDate & " " & departtime_combo4.SelectedValue)
        End If

        'minuteswaitbetweensegments = 0
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress4.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress4.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress4.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress4.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress4.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress4.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        If Me.pnlLeg5.Visible = False Then Exit Function

        If Me.OriginAddress5.Text.Trim = "" And Me.OriginAddress5.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress5.Text.Trim = "" And Me.DestinationAddress5.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date5.SelectedDate & " " & departtime_combo5.SelectedValue)
        If Me.OriginAddress5.SelectedValue.Trim = "" Then
            fromairport = Me.OriginAddress5.Text.Trim
        Else
            fromairport = Me.OriginAddress5.SelectedValue.Trim
        End If
        If Me.DestinationAddress5.SelectedValue.Trim = "" Then
            toairport = Me.DestinationAddress5.Text.Trim
        Else
            toairport = Me.DestinationAddress5.SelectedValue.Trim
        End If
        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        ma += TurnAroundMinutes    'add turn time
        If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
            If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
                Me.depart_date5.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2),
                    CDate(Me.depart_date5.SelectedDate))
            End If
            For i As Integer = 0 To departtime_combo5.Items.Count - 1
                If IsDate(departtime_combo5.Items(i).Value) Then
                    If CDate(CDate(departtime_combo5.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                        departtime_combo5.SelectedValue = departtime_combo5.Items(i).Value
                        Exit For
                    End If
                End If
            Next

            leave2 = CDate(Me.depart_date5.SelectedDate & " " & departtime_combo5.SelectedValue)
        End If

        'minuteswaitbetweensegments = 0
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress5.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress5.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress5.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress5.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress5.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress5.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        If Me.pnlLeg6.Visible = False Then Exit Function

        If Me.OriginAddress6.Text.Trim = "" And Me.OriginAddress6.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress6.Text.Trim = "" And Me.DestinationAddress6.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date6.SelectedDate & " " & departtime_combo6.SelectedValue)
        If Me.OriginAddress6.SelectedValue.Trim = "" Then
            fromairport = Me.OriginAddress6.Text.Trim
        Else
            fromairport = Me.OriginAddress6.SelectedValue.Trim
        End If
        If Me.DestinationAddress6.SelectedValue.Trim = "" Then
            toairport = Me.DestinationAddress6.Text.Trim
        Else
            toairport = Me.DestinationAddress6.SelectedValue.Trim
        End If
        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        ma += TurnAroundMinutes    'add turn time
        If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
            If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
                Me.depart_date6.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2),
                    CDate(Me.depart_date6.SelectedDate))
            End If
            For i As Integer = 0 To departtime_combo6.Items.Count - 1
                If IsDate(departtime_combo6.Items(i).Value) Then
                    If CDate(CDate(departtime_combo6.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                        departtime_combo6.SelectedValue = departtime_combo6.Items(i).Value
                        Exit For
                    End If
                End If
            Next

            leave2 = CDate(Me.depart_date6.SelectedDate & " " & departtime_combo6.SelectedValue)
        End If

        'minuteswaitbetweensegments = 0
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress6.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress6.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress6.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress6.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress6.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress6.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        If Me.pnlLeg7.Visible = False Then Exit Function

        If Me.OriginAddress7.Text.Trim = "" And Me.OriginAddress7.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress7.Text.Trim = "" And Me.DestinationAddress7.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date7.SelectedDate & " " & departtime_combo7.SelectedValue)
        If Me.OriginAddress7.SelectedValue.Trim = "" Then
            fromairport = Me.OriginAddress7.Text.Trim
        Else
            fromairport = Me.OriginAddress7.SelectedValue.Trim
        End If
        If Me.DestinationAddress7.SelectedValue.Trim = "" Then
            toairport = Me.DestinationAddress7.Text.Trim
        Else
            toairport = Me.DestinationAddress7.SelectedValue.Trim
        End If
        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        ma += TurnAroundMinutes    'add turn time
        If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
            If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
                Me.depart_date7.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2),
                    CDate(Me.depart_date7.SelectedDate))
            End If
            For i As Integer = 0 To departtime_combo7.Items.Count - 1
                If IsDate(departtime_combo7.Items(i).Value) Then
                    If CDate(CDate(departtime_combo7.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                        departtime_combo7.SelectedValue = departtime_combo7.Items(i).Value
                        Exit For
                    End If
                End If
            Next

            leave2 = CDate(Me.depart_date7.SelectedDate & " " & departtime_combo7.SelectedValue)
        End If

        'minuteswaitbetweensegments = 0
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress7.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress7.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress7.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress7.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress7.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress7.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        If Me.pnlLeg8.Visible = False Then Exit Function

        If Me.OriginAddress8.Text.Trim = "" And Me.OriginAddress8.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress8.Text.Trim = "" And Me.DestinationAddress8.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date8.SelectedDate & " " & departtime_combo8.SelectedValue)
        If Me.OriginAddress8.SelectedValue.Trim = "" Then
            fromairport = Me.OriginAddress8.Text.Trim
        Else
            fromairport = Me.OriginAddress8.SelectedValue.Trim
        End If
        If Me.DestinationAddress8.SelectedValue.Trim = "" Then
            toairport = Me.DestinationAddress8.Text.Trim
        Else
            toairport = Me.DestinationAddress8.SelectedValue.Trim
        End If
        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        ma += TurnAroundMinutes    'add turn time
        If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
            If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
                Me.depart_date8.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2),
                    CDate(Me.depart_date8.SelectedDate))
            End If
            For i As Integer = 0 To departtime_combo8.Items.Count - 1
                If IsDate(departtime_combo8.Items(i).Value) Then
                    If CDate(CDate(departtime_combo8.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                        departtime_combo8.SelectedValue = departtime_combo8.Items(i).Value
                        Exit For
                    End If
                End If
            Next

            leave2 = CDate(Me.depart_date8.SelectedDate & " " & departtime_combo8.SelectedValue)
        End If

        'minuteswaitbetweensegments = 0
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress8.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress8.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress8.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress8.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress8.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress8.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        If Me.pnlLeg9.Visible = False Then Exit Function

        If Me.OriginAddress9.Text.Trim = "" And Me.OriginAddress9.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress9.Text.Trim = "" And Me.DestinationAddress9.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date9.SelectedDate & " " & departtime_combo9.SelectedValue)
        If Me.OriginAddress9.SelectedValue.Trim = "" Then
            fromairport = Me.OriginAddress9.Text.Trim
        Else
            fromairport = Me.OriginAddress9.SelectedValue.Trim
        End If
        If Me.DestinationAddress9.SelectedValue.Trim = "" Then
            toairport = Me.DestinationAddress9.Text.Trim
        Else
            toairport = Me.DestinationAddress9.SelectedValue.Trim
        End If
        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        ma += TurnAroundMinutes    'add turn time
        If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
            If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
                Me.depart_date9.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2),
                    CDate(Me.depart_date9.SelectedDate))
            End If
            For i As Integer = 0 To departtime_combo9.Items.Count - 1
                If IsDate(departtime_combo9.Items(i).Value) Then
                    If CDate(CDate(departtime_combo9.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                        departtime_combo9.SelectedValue = departtime_combo9.Items(i).Value
                        Exit For
                    End If
                End If
            Next

            leave2 = CDate(Me.depart_date9.SelectedDate & " " & departtime_combo9.SelectedValue)
        End If

        'minuteswaitbetweensegments = 0
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress9.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress9.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress9.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress9.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress9.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress9.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        If Me.pnlLeg10.Visible = False Then Exit Function

        If Me.OriginAddress10.Text.Trim = "" And Me.OriginAddress10.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select from airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        If Me.DestinationAddress10.Text.Trim = "" And Me.DestinationAddress10.SelectedValue.Trim = "" Then
            lblMsg.Text &= "Please select to airport. "
            lblMsg.Visible = True
            editflightsmultileg = ""
            Exit Function
        End If

        leave2 = CDate(depart_date10.SelectedDate & " " & departtime_combo10.SelectedValue)
        If Me.OriginAddress10.SelectedValue.Trim = "" Then
            fromairport = Me.OriginAddress10.Text.Trim
        Else
            fromairport = Me.OriginAddress10.SelectedValue.Trim
        End If
        If Me.DestinationAddress10.SelectedValue.Trim = "" Then
            toairport = Me.DestinationAddress10.Text.Trim
        Else
            toairport = Me.DestinationAddress10.SelectedValue.Trim
        End If
        d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
        bintl = isflightintl(fromairport, toairport)
        If d > 0 Then
            ma = AirTaxi.traveltime(d, bintl, carrierid)
        End If
        ma += TurnAroundMinutes    'add turn time
        If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
            If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
                Me.depart_date10.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2),
                    CDate(Me.depart_date10.SelectedDate))
            End If
            For i As Integer = 0 To departtime_combo10.Items.Count - 1
                If IsDate(departtime_combo10.Items(i).Value) Then
                    If CDate(CDate(departtime_combo10.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) Then
                        departtime_combo10.SelectedValue = departtime_combo10.Items(i).Value
                        Exit For
                    End If
                End If
            Next

            leave2 = CDate(Me.depart_date10.SelectedDate & " " & departtime_combo10.SelectedValue)
        End If

        'minuteswaitbetweensegments = 0
        'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        'If minuteswaitbetweensegments <= 0 Then
        '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '    lblMsg.Visible = True
        '    Exit Function
        'ElseIf minuteswaitbetweensegments < 30 then
        '    lblMsg.Text = "Time between segments too short"
        '    lblMsg.Visible = True
        '    Exit Function
        'End If
        leave1 = leave2

        If Me.OriginAddress10.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.OriginAddress10.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.OriginAddress10.Text.Trim
        End If
        editflightsmultileg &= " to "
        If Me.DestinationAddress10.SelectedValue.Trim <> "" Then
            editflightsmultileg &= Me.DestinationAddress10.SelectedValue.Trim
        Else
            editflightsmultileg &= Me.DestinationAddress10.Text.Trim
        End If
        editflightsmultileg &= " at " & leave2.ToString & ";"

        'If Not IsNothing(lbLegs) Then
        '    For i As Integer = 0 To lbLegs.Items.Count - 1
        '        s = lbLegs.Items(i).Value
        '        fromairport = Left(s, InStr(s, " to ") - 1)
        '        toairport = Mid(s, InStr(s, " to ") + 4, 4).Trim
        '        s = Mid(s, InStr(s, " at ") + 4)
        '        If IsDate(s) Then
        '            leave2 = CDate(s)
        '            If 1 > 0 Then
        '                minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
        '                If minuteswaitbetweensegments <= 0 Then
        '                    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
        '                    lblMsg.Visible = True

        '                    Exit Sub
        '                End If
        '            End If
        '            leave1 = leave2
        '            fromairport = toairport
        '            toairport = ""
        '        End If
        '    Next

        'End If

    End Function

    'Private Sub CmdReview_Click(sender As Object, e As EventArgs) Handles CmdReview.Click

    '    '20111229 - pab
    '    AirTaxi.post_timing("cmdConfirmClicked Start  " & Now.ToString)

    '    '20120706 - pab - fix error when session times out - Object reference not set to an instance of an object
    '    If IsNothing(Session("triptype")) Then
    '        '20120130 - pab - add start over button at top for when no flights returned
    '        'cmdStartOver_Click(sender, e)
    '        cmdStartOverClicked(sender, e)
    '        Exit Sub
    '    End If

    '    '20131016 - pab - fix session timeout
    '    Dim da As New DataAccess
    '    If IsNothing(Session("flights")) And IsNothing(Session("triptype")) Then
    '        lblMsg.Text = da.GetSetting(_carrierid, "TimeoutMessage")
    '        lblMsg.Visible = True
    '        gvServiceProviderMatrix.EmptyDataText = lblMsg.Text
    '        dtflights.Clear()
    '        Me.gvServiceProviderMatrix.DataSource = dtflights
    '        Me.gvServiceProviderMatrix.DataBind()
    '    Else
    '        Me.lblMsg.Visible = False
    '        Me.lblMsg.ForeColor = Drawing.Color.Black
    '        Me.lblMsg.Font.Bold = False
    '        If Session("optimize") = "FAIL" Then
    '            Me.lblMsg.Text = "Please try another aircraft or time"
    '            Me.lblMsg.Visible = True
    '            Me.lblMsg.ForeColor = Drawing.Color.Red
    '            Me.lblMsg.Font.Bold = True
    '            Exit Sub
    '        End If

    '        '20111216 - pab - use radio buttons for select per David
    '        Dim idx As Integer = -1
    '        GetSelectedRecord()
    '        '20111230 - pab - fix error when nothing selected - Object reference not set to an instance of an object
    '        If ViewState("SelectedFlight") Is Nothing Then
    '            If gvServiceProviderMatrix.Rows.Count = 1 Then
    '                idx = 0
    '            Else
    '                Me.lblMsg.Text = "Please select a flight"
    '                Me.lblMsg.Visible = True
    '                Me.lblMsg.ForeColor = Drawing.Color.Red
    '                Me.lblMsg.Font.Bold = True
    '                Exit Sub
    '            End If
    '        ElseIf ViewState("SelectedFlight").ToString = "" Or Not IsNumeric(ViewState("SelectedFlight").ToString) Then
    '            Me.lblMsg.Text = "Please select a flight"
    '            Me.lblMsg.Visible = True
    '            Me.lblMsg.ForeColor = Drawing.Color.Red
    '            Me.lblMsg.Font.Bold = True
    '            Exit Sub
    '        Else
    '            idx = CInt(ViewState("SelectedFlight"))
    '        End If

    '        Dim dr As GridViewRow
    '        Dim serviceprovider As String = String.Empty
    '        'Dim dr As GridViewRow = gvServiceProviderMatrix.Rows(Me.gvServiceProviderMatrix.SelectedIndex)
    '        dr = gvServiceProviderMatrix.Rows(idx)

    '        For i As Integer = 0 To dr.Cells(11).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(11).Controls(i)

    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                serviceprovider = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next
    '        Session("service provider") = serviceprovider
    '        Session("service provider") = Replace(Session("service provider"), "Fly ", "")

    '        ' ''-- ~~DHA new implmentation for multileg 
    '        If Session("triptype") = "G" Then
    '            '_passengers = Me.ddlPassengers.SelectedValue
    '            Session("passengers") = _passengers
    '        End If


    '        '20140220 - pab - cleanup code
    '        'Session("origAddr") = Me.txtGeoOrig.Text
    '        'Session("destAddr") = Me.txtGeoDest.Text
    '        Session("passengers") = _passengers
    '        Session("leaveDate") = Month(_leaveDateTime) & "/" & Day(_leaveDateTime) & "/" & Year(_leaveDateTime)
    '        Session("returnDate") = Month(_returnDateTime) & "/" & Day(_returnDateTime) & "/" & Year(_returnDateTime)


    '        Session("origAirportCodeReturn") = _origAirportCode
    '        Session("destAirportCodeReturn") = _destAirportCode

    '        '20120507 - pab - prevent confirmation email from going out more than once if back button pressed
    '        Session("confirmemailsent") = "N"

    '        '20120501 - pab - fix bug if one row selected, the user goes to the next page and then returns here and selects another row. the original row remains selected.
    '        Dim dtflightsselected As New DataTable
    '        Dim dr2 As DataRow = Nothing
    '        Dim dc As DataColumn

    '        '20140620 - pab - quote from admin portal
    '        dtflightsselected = Create_dtflights()

    '        dr2 = dtflightsselected.NewRow

    '        For i As Integer = 0 To dr.Cells(1).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(1).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("carrierlogo") = InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "<img src=""", """ alt=")
    '                '20140620 - pab - populate certifications
    '                '20141201 - pab - fix error - Object reference not set to an instance of an object
    '                'dr2("certifications") = Replace(InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "Certifications: ", " title="), """", "").Trim
    '                Dim s As String = CType(ctl, DataBoundLiteralControl).Text.Trim
    '                If InStr(s, "Certifications: ") > 0 And InStr(s, " title=") > 0 Then
    '                    dr2("certifications") = Replace(InBetween(1, s, "Certifications: ", " title="), """", "").Trim
    '                Else
    '                    dr2("certifications") = InBetween(1, s, "title=""", """ style=")
    '                End If
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(2).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(2).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("FAQPageURL") = InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "href=""", """  target=")
    '                dr2("aircraftlogo") = InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "<img src=""", """ alt=")
    '                dr2("Name") = InBetween(1, CType(ctl, DataBoundLiteralControl).Text.Trim, "title=""", """ style=")
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(3).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(3).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("OriginFacilityName") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(4).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(4).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("Departs") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(5).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(5).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("DestinationFacilityName") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(6).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(6).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("Arrives") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(7).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(7).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("Flight Duration") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(8).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(8).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("FuelStops") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        '20140219 - pab - owner confirmation
    '        For i As Integer = 0 To dr.Cells(9).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(9).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("OwnerConfirmation") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(10).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(10).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("Price") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        dr2("Service Provider") = serviceprovider

    '        For i As Integer = 0 To dr.Cells(12).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(12).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("PriceExplanationDetail") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        'Cells(13) is ID - ignore

    '        For i As Integer = 0 To dr.Cells(14).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(14).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("Origin") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(15).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(15).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("Destination") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(16).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(16).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("minutes") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(17).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(17).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("PriceEdit") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(18).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(18).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("ShowPriceExplanation") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("ShowPriceExplanation") = False
    '                End If
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(19).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(19).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsNumeric(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    '20141106 - pab - rewrite quote routine
    '                    'dr2("EmptyLeg") = CInt(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                    dr2("EmptyLeg") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '                Else
    '                    '20141106 - pab - rewrite quote routine
    '                    'dr2("EmptyLeg") = 0
    '                    dr2("EmptyLeg") = "0"
    '                End If
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(20).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(20).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsNumeric(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("AircraftType") = CInt(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("AircraftType") = 0
    '                End If
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(21).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(21).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("WeightClass") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(22).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(22).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("dbname") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(23).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(23).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("Pets") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("Pets") = False
    '                End If
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(24).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(24).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("Smoking") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("Smoking") = False
    '                End If
    '            End If
    '        Next

    '        '20120619 - pab - add wifi
    '        For i As Integer = 0 To dr.Cells(25).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(25).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("wifi") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("wifi") = False
    '                End If
    '            End If
    '        Next

    '        '20120709 - pab - add lav, power
    '        For i As Integer = 0 To dr.Cells(26).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(26).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("EnclosedLav") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("EnclosedLav") = False
    '                End If
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(27).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(27).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("PowerAvailable") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("PowerAvailable") = False
    '                End If
    '            End If
    '        Next

    '        '20131118 - pab - add more fields to aircraft
    '        For i As Integer = 0 To dr.Cells(28).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(28).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsBool(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("InflightEntertainment") = CBool(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("InflightEntertainment") = False
    '                End If
    '            End If
    '        Next

    '        For i As Integer = 0 To dr.Cells(29).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(29).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("ManufactureDate") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        '20140620 - pab - quote from admin portal
    '        For i As Integer = 0 To dr.Cells(30).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(30).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                dr2("EmptyLegPricing") = CType(ctl, DataBoundLiteralControl).Text.Trim
    '            End If
    '        Next

    '        '20141201 - pab - quoteworker rewrite
    '        For i As Integer = 0 To dr.Cells(31).Controls.Count - 1
    '            Dim ctl As Control = dr.Cells(31).Controls(i)
    '            If TypeOf ctl Is DataBoundLiteralControl Then
    '                If IsNumeric(CType(ctl, DataBoundLiteralControl).Text.Trim) Then
    '                    dr2("quoteID") = CInt(CType(ctl, DataBoundLiteralControl).Text.Trim)
    '                Else
    '                    dr2("quoteID") = 0
    '                End If
    '            End If
    '        Next

    '        dtflightsselected.Rows.Add(dr2)

    '        '20120104 - pab - save quote if delta so contract can be pulled at purchase time
    '        If Session("dqn").ToString = "" And serviceprovider.ToUpper = "DELTA" Then

    '            If dtflightsselected.TableName = "" Then dtflightsselected.TableName = "dtflights"

    '            Dim carrierid As Integer = 0
    '            Dim dt As DataTable
    '            dt = da.GetProviderByAlias(serviceprovider)
    '            If dt.Rows.Count > 0 Then
    '                carrierid = dt.Rows(0).Item("carrierid")
    '            End If

    '            Dim dqn2 As String = InBetween(1, dtflightsselected.Rows(0).Item("PriceExplanationDetail"), "; dqn ", ";")
    '            Dim qn As Integer = recordquotexml(dtflightsselected, "", "", 0, "", 9, "", "", "", "", "", "")
    '            Session("quotenumber") = qn
    '            Session("dqn") = dqn2
    '            Session("dqn2") = dqn2

    '        Else
    '            '20120411 - pab - fix contract error on reservefilghts3 page
    '            If serviceprovider.ToUpper <> "DELTA" Then
    '                Session("dqn") = ""
    '                Session("dqn2") = ""
    '            End If

    '        End If

    '        '20111229 - pab
    '        AirTaxi.post_timing("cmdConfirmClicked End  " & Now.ToString)

    '        Session("flights") = dtflightsselected
    '        '20140310 - pab - acg background image
    '        '20150317 - pab - remove acg branding
    '        'Response.Redirect("reserveflight.aspx?acg=" & _acg)
    '        Response.Redirect("reserveflight.aspx")

    '    End If

    'End Sub

    Private Sub ddllBrokerCompanies_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddllBrokerCompanies.SelectedIndexChanged

        HydrateddllBrokers(Me.ddllBrokerCompanies.SelectedValue.ToString)

    End Sub

    Private Sub ddllBrokers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddllBrokers.SelectedIndexChanged

        If Me.ddllBrokers.SelectedIndex < 0 Then
            Session("BrokerID") = 0
        Else
            Session("BrokerID") = ddllBrokers.SelectedValue
        End If

    End Sub

    '20160304 - pab - add broker info
    Sub HydrateddllBrokerCompanies()

        Dim da As New DataAccess
        Dim dt As DataTable = da.GetBrokerCompanies
        Dim li As ListItem = Nothing

        Me.ddllBrokerCompanies.Items.Clear()

        li = New ListItem("", "")
        Me.ddllBrokerCompanies.Items.Add(li)

        If Not isdtnullorempty(dt) Then
            For n As Integer = 0 To dt.Rows.Count - 1
                li = New ListItem(dt.Rows(n).Item("companyname"), dt.Rows(n).Item("companyname"))

                Me.ddllBrokerCompanies.Items.Add(li)

            Next

        End If

    End Sub

    Sub HydrateddllBrokers(ByVal companyname As String)

        Dim da As New DataAccess
        Dim dt As DataTable = da.GetBrokersByCompany(companyname)
        Dim li As ListItem = Nothing

        Me.ddllBrokers.Items.Clear()

        li = New ListItem("", 0)
        Me.ddllBrokers.Items.Add(li)

        If Not isdtnullorempty(dt) Then
            For n As Integer = 0 To dt.Rows.Count - 1
                li = New ListItem(dt.Rows(n).Item("lastname") & ", " & dt.Rows(n).Item("firstname"), dt.Rows(n).Item("brokerid"))

                Me.ddllBrokers.Items.Add(li)

            Next

        End If

    End Sub

    '20160912 - pab - add start over
    Protected Sub cmdStartOver1_Click(sender As Object, e As EventArgs) Handles cmdStartOver1.Click

        cmdStartOverClicked(sender, e)

    End Sub

    Private Sub rblOneWayRoundTrip_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblOneWayRoundTrip.SelectedIndexChanged

        DisplayBoxBasedRadioBtns()

    End Sub

    '20120628 - pab - add one way/round trip radio buttons
    Private Sub DisplayBoxBasedRadioBtns()

        If Request.QueryString.Count = 0 Then
            'Me.depart_txt.Text = DateAdd(DateInterval.Day, 1, Now).ToShortDateString
            Me.depart_date.SelectedDate = CDate(DateAdd(DateInterval.Day, 1, Now).ToShortDateString)
            Me.departtime_combo.SelectedIndex = Me.departtime_combo.Items.IndexOf(Me.departtime_combo.Items.FindByText("09:00 AM"))

        Else
            If Not IsNothing(Request.QueryString("legs")) Then
                'Me.depart_txt.Text = DateAdd(DateInterval.Day, 1, Now).ToShortDateString
                Me.depart_date.SelectedDate = CDate(DateAdd(DateInterval.Day, 1, Now).ToShortDateString)
                Me.departtime_combo.SelectedIndex = Me.departtime_combo.Items.IndexOf(Me.departtime_combo.Items.FindByText("09:00 AM"))

            Else

                If Not IsNothing(Request.QueryString("leaveDate")) Then
                    'Me.depart_txt.Text = Request.QueryString("leaveDate").ToString
                    Me.depart_date.SelectedDate = Request.QueryString("leaveDate").ToString
                Else
                    'Me.depart_txt.Text = DateAdd(DateInterval.Day, 1, Now).ToShortDateString
                    Me.depart_date.SelectedDate = CDate(DateAdd(DateInterval.Day, 1, Now).ToShortDateString)
                End If

                'If Not IsNothing(Request.QueryString("returnDate")) Then
                '    Me.calReturn.SelectedDate = CDate(Request.QueryString("returnDate"))
                'Else
                '    Me.calReturn.SelectedDate = CDate(DateAdd(DateInterval.Day, 2, Now).ToShortDateString)
                'End If

                If Not IsNothing(Request.QueryString("leaveTime")) Then
                    Me.departtime_combo.SelectedIndex =
                    Me.departtime_combo.Items.IndexOf(
                        Me.departtime_combo.Items.FindByText(Request.QueryString("returnTime"))) + 1
                Else
                    Me.departtime_combo.SelectedIndex = Me.departtime_combo.Items.IndexOf(Me.departtime_combo.Items.FindByText("09:00 AM"))
                End If

                'If Not IsNothing(Request.QueryString("leaveTime")) Then
                '    Me.ddlTimeReturn.SelectedIndex = _
                '    Me.ddlTimeReturn.Items.IndexOf( _
                '        Me.ddlTimeReturn.Items.FindByText(Request.QueryString("returnTime")))
                'Else
                '    Me.ddlTimeReturn.SelectedIndex = Me.ddlTimeReturn.Items.IndexOf(Me.ddlTimeReturn.Items.FindByText("05:00 PM"))
                'End If

                ''20160310 - pab - fix empty fields after multileg quote
                'If CDate(Me.calReturn.SelectedDate.ToShortDateString & " " & ddlTimeReturn.Text) < DateAdd(DateInterval.Minute, 90, _
                '        CDate(Me.calLeave.SelectedDate.ToShortDateString & " " & ddlTimeLeave.Text)) Then
                '    Me.calReturn.SelectedDate = CDate(DateAdd(DateInterval.Day, 1, Me.calLeave.SelectedDate).ToShortDateString)
                'End If

            End If

        End If

        Select Case Me.rblOneWayRoundTrip.SelectedValue
            Case "RoundTrip"
                Dim ssender As String

                If Me.pnlLeg10.Visible = True Then
                    ssender = "bttnRemoveLeg10"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg9.Visible = True Then
                    ssender = "bttnRemoveLeg9"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg8.Visible = True Then
                    ssender = "bttnRemoveLeg8"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg7.Visible = True Then
                    ssender = "bttnRemoveLeg7"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg6.Visible = True Then
                    ssender = "bttnRemoveLeg6"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg5.Visible = True Then
                    ssender = "bttnRemoveLeg5"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg4.Visible = True Then
                    ssender = "bttnRemoveLeg4"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg3.Visible = True Then
                    ssender = "bttnRemoveLeg3"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg2.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)
                End If

                If Me.DestinationAddress.SelectedValue.ToString <> "" Then
                    Me.OriginAddress2.Text = Me.DestinationAddress.SelectedValue.ToString
                End If

                If Me.OriginAddress.SelectedValue.ToString <> "" Then
                    Me.DestinationAddress2.Text = Me.OriginAddress.SelectedValue.ToString
                End If

                Session("triptype") = "R"
                Session("showcal") = "Y"

                '20160117 - pab - quote multi-leg trips
            Case "MultiLeg"
                Session("triptype") = "M"
                Session("showcal") = "N"
                'pnlOneWayRT.Visible = False
                'pnlMultiLeg.Visible = True

                If Me.pnlLeg10.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress9.Text <> "" Then
                        Me.OriginAddress10.Text = Me.DestinationAddress9.SelectedValue.ToString
                    End If

                ElseIf Me.pnlLeg9.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress8.Text <> "" Then
                        Me.OriginAddress9.Text = Me.DestinationAddress8.SelectedValue.ToString
                    End If

                ElseIf Me.pnlLeg8.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress7.Text <> "" Then
                        Me.OriginAddress8.Text = Me.DestinationAddress7.SelectedValue.ToString
                    End If

                ElseIf Me.pnlLeg7.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress6.Text <> "" Then
                        Me.OriginAddress7.Text = Me.DestinationAddress6.SelectedValue.ToString
                    End If

                ElseIf Me.pnlLeg6.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress5.Text <> "" Then
                        Me.OriginAddress6.Text = Me.DestinationAddress5.SelectedValue.ToString
                    End If

                ElseIf Me.pnlLeg5.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress4.Text <> "" Then
                        Me.OriginAddress5.Text = Me.DestinationAddress4.SelectedValue.ToString
                    End If

                ElseIf Me.pnlLeg4.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress3.Text <> "" Then
                        Me.OriginAddress4.Text = Me.DestinationAddress3.SelectedValue.ToString
                    End If

                ElseIf Me.pnlLeg3.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress2.Text <> "" Then
                        Me.OriginAddress3.Text = Me.DestinationAddress2.SelectedValue.ToString
                    End If

                ElseIf Me.pnlLeg2.Visible = False Then
                    bttnAddLeg_Click(Nothing, Nothing)

                    If Me.DestinationAddress.SelectedValue.ToString <> "" Then
                        Me.OriginAddress2.Text = Me.DestinationAddress.SelectedValue.ToString
                    End If

                End If

            Case "OneWay"
                Session("triptype") = "O"
                Session("showcal") = "Y"

                Dim ssender As String

                If Me.pnlLeg10.Visible = True Then
                    ssender = "bttnRemoveLeg10"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg9.Visible = True Then
                    ssender = "bttnRemoveLeg9"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg8.Visible = True Then
                    ssender = "bttnRemoveLeg8"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg7.Visible = True Then
                    ssender = "bttnRemoveLeg7"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg6.Visible = True Then
                    ssender = "bttnRemoveLeg6"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg5.Visible = True Then
                    ssender = "bttnRemoveLeg5"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg4.Visible = True Then
                    ssender = "bttnRemoveLeg4"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg3.Visible = True Then
                    ssender = "bttnRemoveLeg3"
                    removeleg(Nothing, Nothing, ssender)
                End If

                If Me.pnlLeg2.Visible = True Then
                    ssender = "bttnRemoveLeg2"
                    removeleg(Nothing, Nothing, ssender)
                End If

            Case Else

        End Select

    End Sub

End Class
