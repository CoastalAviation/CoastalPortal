Option Strict On
Option Explicit On

Imports CoastalPortal.AirTaxi

Public Class PlanningFOS

    '20140416 - pab - add select time zone
    '20161227 - pab - fix calendar - wu flight detail not numeric
    '20170106 - pab - no crew flagged as m
    '20170721 - pab - jlx - show bkr
    '20170913 - pab - add cost and leg type code to mouseover
    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function FormatRowWeekly(ByVal startDateRange As DateTime, ByVal endDateRange As DateTime, ByVal FlightID As String, ByVal AircraftID As Integer,
                       ByVal Registration As String, ByRef DepartureAirport As String, ByVal DepartureTime As DateTime, ByRef ArrivalAirport As String,
                       ByVal ArrivalTime As DateTime, ByVal FlightType As String, ByRef previousArrivalAirport As String, ByVal passengers As Integer,
                       ByVal seatsavailable As Integer, ByVal rownumber As Integer, ByVal PilotStatus As String, ByRef FOSCalendarDays As Integer,
                       ByRef timezone As String, ByVal legtypecode As String, ByVal basecode As String, ByRef cost As Integer, ByVal _carrierid As Integer) As ArrayList

        AirTaxi.post_timing("PlanningFOS FormatRowWeekly start  " & Now.ToString)

        Dim da As New DataAccess
        Dim bShowBothEnds As Boolean = False
        Dim i As Integer = 0
        Dim s As String = String.Empty

        's = da.GetSetting(_carrierid, "FlightPlanningShowAirportsBothEnds")
        'If s = "Y" Then
        '    bShowBothEnds = True
        'End If

        Dim block(7) As DateTime
        For i = 0 To block.Length - 1
            block(i) = DateAdd(DateInterval.Day, i, startDateRange)
        Next

        Dim td(7) As String
        For i = 0 To td.Length - 1
            td(i) = ""
        Next

        Dim blockstart As DateTime = Nothing
        Dim previousblockstart As DateTime = Nothing
        Dim minutesinblock As Integer = 1440
        'Dim textblockwidth As Long = 15
        Dim textblockwidth As Long = 4

        'Dim p As New Planning
        'Dim CssClass As String = p.GetCssClass(FlightType, PilotStatus)
        '20170106 - pab - no crew flagged as m
        Dim CssClass As String = GetCssClass(FlightType, PilotStatus, legtypecode, _carrierid)
        Dim mouseover As String = String.Empty
        Dim alignment As String = String.Empty

        Dim n As Long = 0
        Dim n2 As Double = 0

        Dim bairportdisplayed As Boolean = False

        Dim FlightRow As New ArrayList

        Try

            '20140204 - pab - calendar format changes
            '20170102 - pab - fix calendar - show icao instead of iata
            'If Len(DepartureAirport.Trim) = 4 And Left(DepartureAirport, 1) = "K" Then DepartureAirport = Mid(DepartureAirport, 2)
            'If Len(ArrivalAirport.Trim) = 4 And Left(ArrivalAirport, 1) = "K" Then ArrivalAirport = Mid(ArrivalAirport, 2)
            'If Len(previousArrivalAirport.Trim) = 4 And Left(previousArrivalAirport, 1) = "K" Then previousArrivalAirport = Mid(previousArrivalAirport, 2)

            mouseover = ""

            '20110120 - pab - calendar changes
            Dim title As String = ""
            If FlightType <> "" Then
                'title &= " title=" & Chr(34) & "<B>Scheduled</B><BR><BR><B>" & DepartureAirport & "</B> " & DepartureTime & _
                '    " GMT<BR>to<BR><B>" & ArrivalAirport & "</B> " & ArrivalTime & " GMT"""   '& "<br />"
                'mouseover = " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"""
                mouseover = "<a onmouseover=""Tip('<b>"
                Select Case FlightType
                    Case "R"
                        mouseover &= "Scheduled"
                    Case "D"
                        '20170104 - pab - change label per David
                        'mouseover &= "Empty Leg"
                        mouseover &= "Reposition"
                    Case "M"
                        '20170106 - pab - no crew flagged as m
                        If legtypecode = "NC" Then
                            mouseover &= "No Crew"
                            '20170323 - pab - use legtypecode for swap instead of airport per Richard
                        ElseIf legtypecode = "SWAP" Then
                            mouseover &= "SWAP"
                            '20170327 - pab - show AOG in different color
                        ElseIf legtypecode = "AOG" Then
                            mouseover &= "AOG"
                        Else
                            mouseover &= "Maintenance"
                        End If
                    Case "O"
                        mouseover &= "Owner Trip"
                    Case "Q"
                        mouseover &= "Quick Trip"
                    Case "T"
                        mouseover &= "Training"
                    Case "B"    'tmc
                        '20170102 - pab - fix calendar
                        If _carrierid = 100 Then
                            mouseover &= "SWAP"
                        Else
                            mouseover &= "Bull Pen"
                        End If
                    Case "C"    'xo
                        mouseover &= "Crew"

                    '20140806 - pab - add transient - available for sale
                    Case "S"    'transient
                        mouseover &= "Available For Sale"

                    Case ""
                        'active but not flying - no tip balloon
                    Case Else
                        mouseover &= "Other"
                End Select

                '20170721 - pab - jlx - show bkr
                If basecode = "BKR" Then
                    mouseover &= "</b><br /><br /><b>" & basecode
                End If

                '20170402 - pab - add trip number to pop-up
                If FlightID <> "" Then
                    mouseover &= "</b><br /><br />Trip Number <b>" & FlightID
                End If
                '20170913 - pab - add cost and leg type code to mouseover
                If cost > 0 Then
                    mouseover &= "</b><br /><br />Cost <b>$" & cost
                End If
                If legtypecode <> "" Then
                    mouseover &= "</b><br /><br />Leg Type Code <b>" & legtypecode
                End If
                '20140416 - pab - add select time zone
                mouseover &= "</b><br /><br /><b>" & DepartureAirport & "</b> " & Format(DepartureTime, "MM/dd/yy HH:mm") & " " & timezone & "<br />to<br /><b>" & ArrivalAirport &
                "</b> " & Format(ArrivalTime, "MM/dd/yy HH:mm") & " " & timezone & "', ABOVE, true, WIDTH, 200, OFFSETX, -20, TEXTALIGN, 'center', FADEIN, 500, FADEOUT, 500, " &
                "OPACITY, 100, PADDING, 5, BALLOON, true, BALLOONIMGPATH, '../tip_balloon/')"" onmouseout=""UnTip()"">"
            End If

            title = " title=" & Chr(34)
            If DepartureAirport <> "" Then
                title &= DepartureAirport & " - "
            End If
            title &= ArrivalAirport & Chr(34)

            'If FlightID <> 0 Then
            '    mouseover &= " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"""
            '    'mouseover &= " ondblclick=""showFlightDetailPopout(" & FlightID & ");"""
            '    'mouseover &= " onclick=""hideOrShow(trw" & rownumber + 1 & ");"""
            '    'mouseover &= " oncontextmenu=""buildMenu(" & FlightID & ",'" & DepartureAirport & "','" & ArrivalAirport & "','" & FlightID & "');return false;"""
            'End If

            alignment = ">"

            endDateRange = CDate(endDateRange.ToString("d") & " 11:59 PM")

            Dim divwidth As Long = 100
            If Registration = "404T" Then
                Registration = Registration
            End If

            If FOSCalendarDays < 3 Then textblockwidth = CLng(textblockwidth / 2)

            For i = 0 To block.Length - 1
                If Not IsNothing(block(i)) Then
                    If DepartureTime <= startDateRange And ArrivalTime >= endDateRange Then
                        '20140418 - pab - don't show if maintenance or bull pen per kyle at tmc
                        '20170105 - pab - show maintenance and bull pen per david
                        'If FlightType <> "M" And FlightType <> "B" Then
                        '20140204 - pab - calendar format changes
                        'alignment = " align=""left"">&nbsp;" & DepartureAirport
                        alignment = " align=""left"">&nbsp;" & DepartureAirport & "<br />&nbsp;" & ArrivalAirport
                        If mouseover <> "" Then
                            td(i) = td(i) & mouseover & "<div " & " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                        Else
                            td(i) = td(i) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" &
                            " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                        End If
                        alignment = ">"
                        Dim n3 As Integer
                        For n3 = 1 To td.Length - 2
                            If mouseover <> "" Then
                                td(n3) = td(n3) & mouseover & "<div " & " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                            Else
                                td(n3) = td(n3) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" &
                                " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                            End If
                        Next
                        '20140204 - pab - calendar format changes
                        'alignment = " align=""right"">" & ArrivalAirport & "&nbsp;"
                        If mouseover <> "" Then
                            td(n3) = td(n3) & mouseover & "<div " & " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                        Else
                            td(n3) = td(n3) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" &
                            " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                        End If
                        'End If
                        Exit For
                    End If
                    If i = 0 And DepartureTime < startDateRange Then
                        DepartureTime = startDateRange
                    End If
                    If DepartureTime < block(i) Then
                        n = DateDiff(DateInterval.Minute, DepartureTime, ArrivalTime)
                        If (DepartureTime = block(i - 1) And ArrivalTime <= block(i)) Or (DepartureTime > block(i - 1) And ArrivalTime <= block(i)) Then
                            n2 = n / minutesinblock * 100
                            alignment = ">"
                            'If n2 / 2 > textblockwidth Then
                            If n2 > textblockwidth Then
                                'If bShowBothEnds Then alignment = " align=""left"">" & DepartureAirport
                                '20140204 - pab - calendar format changes
                                'alignment = " align=""left"">&nbsp;" & DepartureAirport
                                alignment = " align=""left"">&nbsp;" & DepartureAirport & "<br />&nbsp;" & ArrivalAirport
                            End If
                            If mouseover <> "" Then
                                'td(i - 1) = td(i - 1) & mouseover & "<div " & " style=""float:left;width:" & n2 / 2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                                td(i - 1) = td(i - 1) & mouseover & "<div " & " style=""float:left;width:" & n2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                            Else
                                'td(i - 1) = td(i - 1) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" & _
                                '    " style=""float:left;width:" & n2 / 2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                                td(i - 1) = td(i - 1) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" &
                                " style=""float:left;width:" & n2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                            End If
                            ''If n2 / 2 > textblockwidth Then
                            ''    alignment = " align=""right"">" & ArrivalAirport
                            ''Else
                            ''    alignment = ">"
                            ''End If
                            'alignment = ">"
                            'If n2 / 2 > textblockwidth Then
                            '    If bShowBothEnds Then alignment = " align=""right"">" & ArrivalAirport & "&nbsp;"
                            'End If
                            'If mouseover <> "" Then
                            '    td(i - 1) = td(i - 1) & mouseover & "<div " & " style=""float:left;width:" & n2 / 2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                            'Else
                            '    td(i - 1) = td(i - 1) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" & _
                            '        " style=""float:left;width:" & n2 / 2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                            'End If
                            i = i + 1

                        Else
                            If DepartureTime = block(i - 1) And ArrivalTime > block(i) Then
                                'alignment = ">"
                                'If bShowBothEnds Then alignment = " align=""left"">" & DepartureAirport
                                '20140204 - pab - calendar format changes
                                'alignment = " align=""left"">&nbsp;" & DepartureAirport
                                alignment = " align=""left"">&nbsp;" & DepartureAirport & "<br />&nbsp;" & DepartureAirport
                                If mouseover <> "" Then
                                    td(i - 1) = td(i - 1) & mouseover & "<div " & " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                                Else
                                    td(i - 1) = td(i - 1) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" &
                                    " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                                End If
                                n = n - minutesinblock

                            End If

                            If DepartureTime > block(i - 1) And ArrivalTime > block(i) Then
                                n2 = DateDiff(DateInterval.Minute, DepartureTime, block(i))
                                n = n - CLng(n2)
                                n2 = n2 / minutesinblock * 100
                                alignment = ">"
                                If n2 > textblockwidth Then
                                    'If bShowBothEnds Then alignment = " align=""left"">" & DepartureAirport
                                    '20140204 - pab - calendar format changes
                                    'alignment = " align=""left"">&nbsp;" & DepartureAirport
                                    alignment = " align=""left"">&nbsp;" & DepartureAirport & "<br />&nbsp;" & ArrivalAirport
                                End If
                                If mouseover <> "" Then
                                    td(i - 1) = td(i - 1) & mouseover & "<div " & " style=""float:left;width:" & n2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                                Else
                                    td(i - 1) = td(i - 1) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" &
                                    " style=""float:left;width:" & n2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                                End If

                            End If

                            alignment = ">"
                            Do While n > minutesinblock And i < block.Length - 1
                                If mouseover <> "" Then
                                    td(i) = td(i) & mouseover & "<div " & " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                                Else
                                    td(i) = td(i) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" &
                                    " style=""float:left;width:100%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                                End If
                                n = n - minutesinblock
                                i = i + 1

                            Loop

                            n2 = n / minutesinblock * 100
                            'If n2 > textblockwidth Then
                            '    alignment = " align=""right"">" & ArrivalAirport
                            'Else
                            '    alignment = ">"
                            'End If
                            alignment = ">"
                            If n2 > textblockwidth Then
                                If bShowBothEnds Then alignment = " align=""right"">" & ArrivalAirport & "&nbsp;"
                            End If
                            If mouseover <> "" Then
                                td(i) = td(i) & mouseover & "<div " & " style=""float:left;width:" & n2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>" & "</a>"
                            Else
                                td(i) = td(i) & "<div " & title & " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';""" &
                                " style=""float:left;width:" & n2 & "%;""" & " class=""" & CssClass & """" & alignment & "</div>"
                            End If

                        End If

                        Exit For

                    End If

                End If

            Next

            For i = 0 To td.Length - 1
                FlightRow.Add(td(i))
            Next

            FormatRowWeekly = FlightRow

            AirTaxi.post_timing("PlanningFOS FormatRowWeekly end  " & Now.ToString)

        Catch ex As Exception
            s = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbCr & vbLf & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbCr & vbLf & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "FormatRowWeekly", "PlanningFOS.vb")

        End Try

    End Function

    '20140818 - pab - make text display more like fos
    '20161227 - pab - fix calendar - wu flight detail not numeric
    '20170106 - pab - no crew flagged as m
    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function FormatRowWeeklyText(ByVal startDateRange As DateTime, ByVal endDateRange As DateTime, ByVal FlightID As String, ByVal AircraftID As Integer,
                        ByVal Registration As String, ByVal DepartureAirport As String, ByVal DepartureTime As DateTime, ByVal ArrivalAirport As String,
                        ByVal ArrivalTime As DateTime, ByVal FlightType As String, ByVal previousArrivalAirport As String, ByVal passengers As Integer,
                        ByVal seatsavailable As Integer, ByVal rownumber As Integer, ByVal flightdtl As String, ByVal PilotStatus As String,
                        ByRef RequesterName As String, ByRef bFlightInfoOnly As Boolean, ByVal legtypecode As String, ByVal _carrierid As Integer) As ArrayList

        AirTaxi.post_timing("PlanningFOS FormatRowWeeklyText start  " & Now.ToString)

        Dim da As New DataAccess
        Dim bShowBothEnds As Boolean = False
        Dim i As Integer = 0
        Dim s As String = String.Empty

        s = da.GetSetting(_carrierid, "FlightPlanningShowAirportsBothEnds")
        If s = "Y" Then
            bShowBothEnds = True
        End If

        Dim block(7) As DateTime
        For i = 0 To block.Length - 1
            block(i) = DateAdd(DateInterval.Day, i, startDateRange)
        Next

        Dim td(7) As String
        For i = 0 To td.Length - 1
            td(i) = ""
        Next

        Dim blockstart As DateTime = Nothing
        Dim previousblockstart As DateTime = Nothing
        Dim minutesinblock As Integer = 1440
        Dim textblockwidth As Long = 15

        'Dim p As New Planning
        'Dim CssClass As String = p.GetCssClass(FlightType, PilotStatus)
        '20170106 - pab - no crew flagged as m
        Dim CssClass As String = GetCssClass(FlightType, PilotStatus, legtypecode, _carrierid)
        'Dim alignment As String = String.Empty

        Dim n As Long = 0
        Dim n2 As Double = 0

        Dim bairportdisplayed As Boolean = False

        Dim FlightRow As New ArrayList

        '20110120 - pab - calendar changes
        Dim title As String = ""
        'If FlightID > 0 Then
        If FlightID <> "" Then
            title = "Trip: " & FlightID & " - "
            title &= CDate(DepartureTime).ToString("t") & " " & DepartureAirport & " - " & ArrivalAirport & " " & CDate(ArrivalTime).ToString("t") '& "<br />"
        Else
            '20110208 - pab - pilot calendar changes 
            If DepartureAirport <> "" And ArrivalAirport <> "" Then title = DepartureAirport & " - " & ArrivalAirport
        End If

        'alignment = ">"

        endDateRange = CDate(endDateRange.ToString("d") & " 11:59 PM")

        '20170214 - pab - fix swap overnight but less than 24 hours
        'Dim days As Long = DateDiff(DateInterval.Day, DepartureTime, ArrivalTime)
        Dim days As Long = 0
        If DepartureTime < startDateRange Then
            days = DateDiff(DateInterval.Day, CDate(startDateRange.ToString("d")), CDate(ArrivalTime.ToString("d")))
        Else
            days = DateDiff(DateInterval.Day, CDate(DepartureTime.ToString("d")), CDate(ArrivalTime.ToString("d")))
        End If

        Dim flightsummary As String = ""

        endDateRange = CDate(endDateRange.ToString("d") & " 11:59 PM")

        For i = 0 To block.Length - 1
            If DepartureTime < block(i + 1) Then
                If days = 0 Then
                    '20110208 - pab - pilot calendar changes 
                    '20140818 - pab - make text display more like fos
                    '20170323 - pab - use legtypecode for swap instead of airport per Richard
                    flightsummary = FormatFlightSummaryText(DepartureAirport, DepartureTime, ArrivalAirport, ArrivalTime, FlightID,
                        flightdtl, seatsavailable, Registration, FlightType, rownumber, i, PilotStatus, RequesterName, bFlightInfoOnly,
                        legtypecode, _carrierid)
                    td(i) = td(i) & flightsummary
                Else
                    Do While days > 0
                        '20140818 - pab - make text display more like fos
                        '20170323 - pab - use legtypecode for swap instead of airport per Richard
                        flightsummary = FormatFlightSummaryText(DepartureAirport, DepartureTime, ArrivalAirport, DateAdd(DateInterval.Minute, -1,
                            block(i + 1)), FlightID, flightdtl, seatsavailable, Registration, FlightType, rownumber, i, PilotStatus, RequesterName,
                            bFlightInfoOnly, legtypecode, _carrierid)
                        td(i) = td(i) & flightsummary
                        DepartureTime = block(i + 1)
                        i = i + 1
                        If i >= block.Length - 1 Then Exit Do
                        days = days - 1
                    Loop
                    If i >= block.Length - 1 Then Exit For
                    '20140818 - pab - make text display more like fos
                    '20170323 - pab - use legtypecode for swap instead of airport per Richard
                    flightsummary = FormatFlightSummaryText(DepartureAirport, DepartureTime, ArrivalAirport, ArrivalTime, FlightID,
                        flightdtl, seatsavailable, Registration, FlightType, rownumber, i, PilotStatus, RequesterName, bFlightInfoOnly,
                        legtypecode, _carrierid)
                    td(i) = td(i) & flightsummary
                End If
                Exit For
            End If
        Next

        For i = 0 To td.Length - 1
            FlightRow.Add(td(i))
        Next

        FormatRowWeeklyText = FlightRow

        AirTaxi.post_timing("PlanningFOS FormatRowWeeklyText end  " & Now.ToString)

    End Function

    '20140818 - pab - make text display more like fos
    '20161227 - pab - fix calendar - wu flight detail not numeric
    '20170323 - pab - use legtypecode for swap instead of airport per Richard
    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function FormatFlightSummaryText(ByVal DepartureAirport As String, ByVal DepartureTime As DateTime, ByVal ArrivalAirport As String,
            ByVal ArrivalTime As DateTime, ByVal FlightID As String, ByVal flightdtl As String, ByVal seatsavailable As Integer,
            ByVal Registration As String, ByVal FlightType As String, ByVal rownumber As Integer, ByVal cell As Integer, ByVal PilotStatus As String,
            ByRef RequesterName As String, ByRef bFlightInfoOnly As Boolean, ByVal legtypecode As String, ByVal _carrierid As Integer) As String

        AirTaxi.post_timing("PlanningFOS FormatFlightSummaryText start  " & Now.ToString)

        Dim da As New DataAccess
        Dim ds As DataSet
        Dim dt As DataTable
        Dim orig As String = ""
        Dim dest As String = ""
        Dim purpose As String = ""
        Dim passengers As Integer = 0
        Dim username As String = ""
        Dim flightdetail As String = ""

        '20110208 - pab - pilot calendar changes 
        If DepartureAirport <> "" And ArrivalAirport <> "" Then
            orig = da.GetICAOcodebyIATA(DepartureAirport)
            dest = da.GetICAOcodebyIATA(ArrivalAirport)
            If orig = "" Then orig = DepartureAirport
            If dest = "" Then dest = ArrivalAirport
        End If

        '20110208 - pab - pilot calendar changes 
        'If Registration = "" Then
        'If Registration = "" And FlightID <> 0 Then
        If Registration = "" And FlightID <> "" Then
            If IsNumeric(FlightID) Then ds = da.GetFlightDetails(_carrierid, CInt(FlightID))
            If Not ds Is Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Registration = ds.Tables(0).Rows(0).Item("Registration").ToString
                End If
            End If
        End If

        '20110208 - pab - pilot calendar changes 
        '20161227 - pab - fix calendar - wu flight detail not numeric
        'If flightdtl <> 0 Then
        If flightdtl <> "" Then
            If IsNumeric(flightdtl) Then dt = da.GetSysFlightRequestPAXInfo(_carrierid, CInt(flightdtl))
            '20170130 - pab - fix error - object not set
            If Not isdtnullorempty(dt) Then
                If dt.Rows.Count > 0 Then
                    For i = 0 To dt.Rows.Count - 1
                        'If CInt(dt.Rows(i).Item("flightid").ToString) = FlightID Then
                        If dt.Rows(i).Item("flightid").ToString.Trim = FlightID Then
                            '20110214 - pab - flight type changes
                            'If dt.Rows(i).Item("FlightType").ToString.Trim <> "D" Then passengers = CInt(dt.Rows(i).Item("passengers").ToString)
                            '20110720 - pab - fix error 
                            If IsNumeric(dt.Rows(i).Item("passengers")) Then
                                'If dt.Rows(i).Item("FlightType").ToString.Trim <> "CF" Then passengers = CInt(dt.Rows(i).Item("passengers").ToString)
                                If dt.Rows(i).Item("FlightType").ToString.Trim <> "D" Then passengers = CInt(dt.Rows(i).Item("passengers").ToString)
                            Else
                                'If dt.Rows(i).Item("FlightType").ToString.Trim <> "CF" Then passengers = 0
                                If dt.Rows(i).Item("FlightType").ToString.Trim <> "D" Then passengers = 0
                            End If
                            If IsDBNull(dt.Rows(i).Item("companyname")) Or dt.Rows(i).Item("companyname").ToString.Trim = "" Then
                                username = dt.Rows(i).Item("lastname").ToString.Trim & ", " & dt.Rows(i).Item("firstname").ToString.Trim
                                If username.Trim = "," Then username = ""
                            Else
                                username = dt.Rows(i).Item("companyname").ToString.Trim
                            End If
                            Exit For
                        End If
                    Next
                End If
            End If
        End If

        Dim mouseover As String = String.Empty
        'If FlightID <> 0 Then
        If FlightID <> "" Then
            mouseover &= " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"""
            'mouseover &= " onclick=""showFlightDetailPopout(" & FlightID & ");"""
            '20110719 - pab
            'mouseover &= " oncontextmenu=""buildMenu(" & FlightID & ",'" & DepartureAirport & "','" & ArrivalAirport & "');return false;"""
            'mouseover &= " oncontextmenu=""buildMenu(" & FlightID & ",'" & DepartureAirport & "','" & ArrivalAirport & "','" & flightdtl & "');return false;"""
            'mouseover &= " oncontextmenu=""showFlightDetailPopout(" & FlightID & ");"""
        End If

        'color: #FF0000 - Red
        'color: #FF3300 - orange
        'background-color: #FFFFFF - white

        '20110208 - pab - pilot calendar changes 
        'If FlightID = 0 Then        'pilot with no flights
        If FlightID = "" Then        'pilot with no flights
            If PilotStatus = "A" Then
                'active - leave blank
            ElseIf PilotStatus = "O" Then       'Off duty
                flightdetail &= "<div align=""left"" style=""color: blue; background-color: #FFFFFF;""" & mouseover & ">"
                flightdetail &= "<table><tr><td>"
                flightdetail &= "(" & PilotStatus & ") Off duty</td></tr></table></div>"
            ElseIf PilotStatus = "T" Then       'Training
                flightdetail &= "<div align=""left"" style=""color: purple; background-color: #FFFFFF;""" & mouseover & ">"
                flightdetail &= "<table><tr><td>"
                flightdetail &= "(" & PilotStatus & ") Training</td></tr></table></div>"
            ElseIf PilotStatus = "V" Then       'Vacation
                flightdetail &= "<div align=""left"" style=""color: green; background-color: #FFFFFF;""" & mouseover & ">"
                flightdetail &= "<table><tr><td>"
                flightdetail &= "(" & PilotStatus & ") Vacation</td></tr></table></div>"
            End If

            '20110214 - pab - flight type changes
        ElseIf FlightType = "M" Then        'Maintenance
            'ElseIf FlightType = "MX" Or FlightType = "LA" Or FlightType = "G" Then        'Maintenance, Landings Left, Ground
            '20170323 - pab - use legtypecode for swap instead of airport per Richard
            If legtypecode = "SWAP" Then
                flightdetail &= "<div align=""left"" style=""color: purple; background-color: #FFFFFF;""" & mouseover & ">"
                '20170327 - pab - show AOG in different color
            ElseIf legtypecode = "AOG" Then
                flightdetail &= "<div align=""left"" style=""color: #990000; background-color: #FFFFFF;""" & mouseover & ">"
            Else
                flightdetail &= "<div align=""left"" style=""color: #FF0000; background-color: #FFFFFF;""" & mouseover & ">"
            End If
            flightdetail &= "<table><tr><td>"
            '20140818 - pab - make text display more like fos
            If bFlightInfoOnly = False Then
                flightdetail &= RequesterName & " " & FlightID
                flightdetail &= "</td></tr><tr><td>"
            End If
            'flightdetail &= "(" & FlightType & ") " & Format(DepartureTime, "HH:mm") & " " & orig & " " & Format(ArrivalTime, "HH:mm")
            flightdetail &= Format(DepartureTime, "HH:mm") & " " & orig & " " & Format(ArrivalTime, "HH:mm")
            'flightdetail &= "</td></tr><tr><td>"
            ''20110214 - pab - flight type changes
            ''If FlightType = "LA" Then
            ''    flightdetail &= "Landings Left"
            ''ElseIf FlightType = "G" Then
            ''    flightdetail &= "Ground"
            ''Else
            'flightdetail &= "Maintenance"
            ''End If
            flightdetail &= "</td></tr></table></div>"

        Else
            '20110214 - pab - flight type changes
            If FlightType = "Q" Then        'Quick flight
                'If FlightType = "Q" Or FlightType = "HO" Then        'Quick flight
                flightdetail &= "<div align=""left"" style=""color: #FF3300; background-color: #FFFFFF;""" & mouseover & ">"

                '20140806 - pab - add transient - available for sale
            ElseIf FlightType = "S" Then        'transient - available for sale
                flightdetail &= "<div align=""left"" style=""color: #00FF99; background-color: #FFFFFF;""" & mouseover & ">"

                '20140818 - pab - add bullpen
            ElseIf FlightType = "B" Then
                flightdetail &= "<div align=""left"" style=""color: purple; background-color: #FFFFFF;""" & mouseover & ">"

            ElseIf FlightType = "D" Then
                flightdetail &= "<div align=""left"" style=""color: gray; background-color: #FFFFFF;""" & mouseover & ">"

            Else
                flightdetail &= "<div align=""left"" style=""color: blue; background-color: #FFFFFF;""" & mouseover & ">"
            End If
            flightdetail &= "<table><tr><td>"
            '20140818 - pab - make text display more like fos
            If bFlightInfoOnly = False Then
                flightdetail &= RequesterName & " " & FlightID
                flightdetail &= "</td></tr><tr><td>"
            End If
            flightdetail &= Format(DepartureTime, "HH:mm") & " " & orig & " - " & dest & " " & Format(ArrivalTime, "HH:mm")
            'flightdetail &= "</td></tr><tr><td>"
            'flightdetail &= "PAX: " & passengers & " (" & FlightType & ") Dept:"
            flightdetail &= "</td></tr></table></div>"


            ''20110214 - pab - flight type changes
            'If FlightType = "Q" Then        'Quick flight
            '    'If FlightType = "Q" Or FlightType = "HO" Then        'Quick flight
            '    flightdetail &= "<div " & "id=""tbw" & FlightID & rownumber + 1 & cell & """ align=""left"" style=""font-size: xx-small; color: #FF3300; background-color: #FFFFFF; visibility: collapse;""" & mouseover & ">"

            '    '20140806 - pab - add transient - available for sale
            'ElseIf FlightType = "S" Then        'transient - available for sale
            '    flightdetail &= "<div " & "id=""tbw" & FlightID & rownumber + 1 & cell & """ align=""left"" style=""font-size: xx-small; color: #00FF99; background-color: #FFFFFF; visibility: collapse;""" & mouseover & ">"

            'Else
            '    flightdetail &= "<div " & "id=""tbw" & FlightID & rownumber + 1 & cell & """ align=""left"" style=""font-size: xx-small; background-color: #FFFFFF; visibility: collapse;""" & mouseover & ">"
            'End If
            'flightdetail &= "<table><tr><td>"
            'flightdetail &= "CHARTER Auth: " & username
            'flightdetail &= "</td></tr><tr><td>"

            'dt = da.GetRelatedFlights(_carrierid, flightdtl)
            'If dt.Rows.Count > 0 Then
            '    orig = ""
            '    Dim roundtrip As Boolean = False
            '    For i = 0 To dt.Rows.Count - 1
            '        '20110214 - pab - more calendar changes
            '        'If dt.Rows(i).Item("flighttype").ToString <> "CF" Then
            '        If dt.Rows(i).Item("flighttype").ToString <> "D" Then
            '            If purpose <> "" Then
            '                purpose &= "-"
            '            Else
            '                orig = da.GetICAOcodebyIATA(dt.Rows(i).Item("departureairport").ToString)
            '                If orig = "" Then orig = DepartureAirport
            '                '20110720 - pab - fix error 
            '                If Not IsDBNull(dt.Rows(i).Item("roundtrip").ToString) And dt.Rows(i).Item("roundtrip").ToString <> "" Then
            '                    If CBool(dt.Rows(i).Item("roundtrip").ToString) = True Then
            '                        roundtrip = True
            '                    End If
            '                End If
            '            End If
            '            dest = da.GetICAOcodebyIATA(dt.Rows(i).Item("departureairport").ToString)
            '            If dest = "" Then dest = dt.Rows(i).Item("departureairport").ToString
            '            purpose &= dest
            '        End If
            '    Next
            '    'check if round trip - add original departure airport to close loop
            '    '20110214 - pab - more calendar changes
            '    'If CBool(dt.Rows(0).Item("roundtrip").ToString) = True Then
            '    'If purpose <> "" Then purpose &= "-"
            '    'purpose &= dt.Rows(0).Item("departureairport").ToString
            '    If roundtrip = True Then
            '        If purpose <> "" And orig <> "" Then purpose &= "-" & orig
            '    End If
            'End If
            'If purpose = "" Then
            '    flightdetail &= "Purpose: " & DepartureAirport & "-" & ArrivalAirport
            'Else
            '    flightdetail &= "Purpose: " & purpose
            'End If
            'flightdetail &= "</td></tr><tr><td>"
            'flightdetail &= "Seats Avail: " & seatsavailable
            'flightdetail &= "</td></tr><tr><td>"
            'flightdetail &= Registration & " "
            'dt = da.GetFlightPilots(_carrierid, FlightID)
            'flightdetail &= "</td></tr><tr><td>"
            'If dt.Rows.Count > 0 Then
            '    Dim pilots As String = ""
            '    For i = 0 To dt.Rows.Count - 1
            '        If pilots <> "" Then pilots &= ","
            '        pilots &= dt.Rows(i).Item("initials").ToString
            '    Next
            '    flightdetail &= pilots
            'End If
            'flightdetail &= "</td></tr></table></div>"
            mouseover = " onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"""
            'mouseover &= " onclick=""hideOrShow(tbw" & FlightID & rownumber + 1 & cell & ");"""
            'mouseover &= " oncontextmenu=""buildMenu(" & FlightID & ",'" & orig & "','" & dest & "','" & flightdtl & "');return false;"""

            '20140818 - pab - make text display more like fos
            'flightdetail &= "<div style=""color: #FFFFFF; background-color: #0000FF; text-align: center"" " & mouseover & ">--------------------</div>"
        End If

        FormatFlightSummaryText = flightdetail

        AirTaxi.post_timing("PlanningFOS FormatFlightSummaryText end  " & Now.ToString)

    End Function

    '20170106 - pab - no crew flagged as m
    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function GetCssClass(ByVal FlightType As String, ByVal PilotStatus As String, ByVal legtypecode As String, ByVal _carrierid As Integer) As String

        '20170915 - pab - add more color codes for restricted an pinned flights
        If legtypecode = "OWRT" Or legtypecode = "OWGN" Or legtypecode = "CHWH" Or legtypecode = "OWNR" Then
            'pinned 
            GetCssClass = "planningCellFlightPlanningPinned"
        ElseIf legtypecode = "CHWH" Or legtypecode = "CHTR" Then
            'restricted
            GetCssClass = "planningCellFlightPlanningRestricted"

            '20170323 - pab - use legtypecode for swap instead of airport per Richard
            '20110214 - pab - more calendar changes
        ElseIf FlightType = "R" Then
            'revenue flight
            'GetCssClass = "planningCellCustomerFlight"
            GetCssClass = "planningCellFlightPlanningCustomerFlightFOS"
            'planningCellTraining
        ElseIf FlightType = "O" Then
            'owner trip
            GetCssClass = "planningCellFlightPlanningOwnerFOS"
        ElseIf FlightType = "D" Then
            'dead leg
            'GetCssClass = "planningCellCustomerDeadLeg"
            GetCssClass = "planningCellFlightPlanningCustomerDeadLegFOS"
        ElseIf FlightType = "M" Then
            'maintenance
            'GetCssClass = "planningCellMaintenance"

            '20170106 - pab - no crew flagged as m
            If legtypecode = "NC" Then
                GetCssClass = "planningCellFlightPlanningOnGround"
                '20170323 - pab - use legtypecode for swap instead of airport per Richard
            ElseIf legtypecode = "SWAP" Then
                GetCssClass = "planningCellFlightPlanningOwner"
                '20170327 - pab - show AOG in different color
            ElseIf legtypecode = "AOG" Then
                GetCssClass = "planningCellFlightPlanningMaintenanceAOG"
            Else
                GetCssClass = "planningCellFlightPlanningMaintenanceFOS"
            End If

        ElseIf FlightType = "A" Then
            'aircraft on ground
            GetCssClass = "planningCellFlightPlanningOnGroundFOS"
        ElseIf FlightType = "Q" Then
            'Quick flight
            GetCssClass = "planningCellFlightPlanningQuickFlightFOS"

            '20140204 - pab - calendar format changes
        ElseIf FlightType = "B" Or FlightType = "C" Then
            '20170102 - pab - fix calendar
            If _carrierid = 100 Then
                'swap (wu)
                GetCssClass = "planningCellFlightPlanningOwner"
            Else
                'bull pen (tmc), crew (xo)
                GetCssClass = "planningCellFlightPlanningOnGroundFOS"
            End If

            '20130710 - pab - add training
        ElseIf FlightType = "T" Then
            'training
            GetCssClass = "planningCellTrainingFOS"

            '20140806 - pab - add transient - available for sale
        ElseIf FlightType = "S" Then
            'transient - available for sale
            GetCssClass = "planningCellTransientFOS"

            '20170213 - pab - show blank as available
        ElseIf FlightType = "" Then
            'available for sale
            GetCssClass = "planningCellFlightPlanningActive"

            '20140204 - pab - calendar format changes
            '20140806 - pab - add transient - available for sale
            '20170213 - pab - show blank as available
        ElseIf FlightType <> "R" And FlightType <> "O" And FlightType <> "D" And FlightType <> "M" And FlightType <> "A" And FlightType <> "Q" And
                FlightType <> "T" And FlightType <> "B" And FlightType <> "C" And FlightType <> "S" And FlightType <> "" Then
            'other
            GetCssClass = "planningCellFlightPlanningOtherFOS"

            'If FlightType = "S" Or FlightType = "CL" Then
            'If FlightType = "S" Or FlightType = "CL" Or FlightType = "R" Then
            '    'revenue flight - Sell, Charter Live
            '    GetCssClass = "planningCellFlightPlanningCustomerFlight"
            'ElseIf FlightType = "CF" Or FlightType = "CS" Or FlightType = "TS" Or FlightType = "OT" Or FlightType = "SHOW" Or FlightType = "AP" Then
            '    'dead leg - Charter Ferry, CrewSwap, Transient, Owner Trip, Show, Airline Positioning
            '    GetCssClass = "planningCellFlightPlanningCustomerDeadLeg"
            '    'ElseIf FlightType = "G" Or FlightType = "LA" Or FlightType = "MX" Then
            'ElseIf FlightType = "G" Or FlightType = "LA" Or FlightType = "MX" Or FlightType = "M" Then
            '    'maintenance - Ground, Landings Left, Maintenance
            '    GetCssClass = "planningCellFlightPlanningMaintenance"
            'ElseIf FlightType = "Q" Or FlightType = "HO" Then
            '    'Quick flight, Hold
            '    GetCssClass = "planningCellFlightPlanningQuickFlight"
            '    'ElseIf FlightType = "DO" Or FlightType = "R" Then
            'ElseIf FlightType = "DO" Then
            '    'Day Off, Rest Overnight
            '    GetCssClass = "planningCellFlightPlanningOnGround"
        ElseIf PilotStatus = "A" Then
            GetCssClass = "planningCellFlightPlanningActiveFOS"
        ElseIf PilotStatus = "O" Then
            GetCssClass = "planningCellFlightPlanningOnGroundFOS"
        ElseIf PilotStatus = "T" Then
            GetCssClass = "planningCellTrainingFOS"
        ElseIf PilotStatus = "V" Then
            GetCssClass = "planningCellVacationFOS"
        Else
            '20110214 - pab - more calendar changes
            'no flight info - default to "on"
            GetCssClass = "planningCellOnFOS"
        End If

    End Function

    Shared Function checkdivwidth(div As String) As String

        Dim divwidth As Double = 100
        Dim s As String = String.Empty

        checkdivwidth = String.Empty

        Do While InStr(div, "width:") > 0
            If InStr(div, "width:100%;") > 0 Then
                Exit Do
            End If
            checkdivwidth &= s & Left(div, InStr(div, "width:") + 5)
            s = InBetween(1, div, "width:", "%;")
            If IsNumeric(s) Then divwidth = divwidth - CDbl(s)
            div = Mid(div, InStr(div, s & "%") + Len(s))
        Loop

        If div <> "" Then
            If divwidth = 100 Then
                'ok - do nothing
            ElseIf divwidth < 0 Then
                'width too high - subtract overage from last width
                s = CStr(CDbl(s) - Math.Abs(divwidth))
            Else
                'width too low - add shortage to last width
                s = CStr(CDbl(s) + Math.Abs(divwidth))
            End If
            checkdivwidth &= s & div
        End If

    End Function

End Class
