Option Strict On
Option Explicit On

Imports CoastalPortal.AirTaxi
Imports CoastalPortal.PlanningFOS

Public Class FOSFlightsCalendar
    Inherits System.Web.UI.UserControl

    'Private _selecteddate As Date

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '20170126 - pab - fix old tmc models being pulled up
        If Session("username") Is Nothing Then
            FormsAuthentication.RedirectToLoginPage()
            Exit Sub
        End If

        If IsNothing(Session("flightplanningdefaultdate")) Then
            'adjust from gmt
            Session("flightplanningdefaultdate") = DateAdd(DateInterval.Hour, -4, Now).ToString("d")
        End If

        If IsNothing(Session("fosmodelstartfos")) Then
            Session("fosmodelstartfos") = CDate(Session("flightplanningdefaultdate"))
        End If
        If CDate(Session("fosmodelstartfos").ToString) = CDate("12:00:00 AM") Then
            Session("fosmodelstartfos") = CDate(Session("flightplanningdefaultdate"))
        End If

        '20170203 - pab - change calendar style
        If IsNothing(Session("CalendarStyle")) Then
            'Session("CalendarStyle") = "Graphic"
            '_CalendarStyle = Me.ddlStyle.SelectedValue

            '20140817 - pab - redirect to fos calendar if fos FOSInterface = 'y'
            Dim da As New DataAccess
            '20170221 - pab - fix page going blank
            Dim setting As String = da.GetSetting(_carrierid, "CalendarStyle")
            If setting = "" Then
                Session("CalendarStyle") = "Graphic"
            End If
            Session("CalendarStyle") = setting
            '20170203 - pab - change calendar style
            'ddlStyle.SelectedValue = setting
        End If
        '_CalendarStyle = "TEXT" 
        If Session("CalendarStyle").ToString.ToUpper = "TEXT" Then
            legendTop.Visible = False
            legendBottom.Visible = False
            legendTopText.Visible = True
            legendBottomText.Visible = True
            '20170618 - pab - more weight classes
            legendTopWC.Visible = False
            legendBottomWC.Visible = False
        Else
            legendTop.Visible = True
            legendBottom.Visible = True
            legendTopText.Visible = False
            legendBottomText.Visible = False
            '20170618 - pab - more weight classes
            legendTopWC.Visible = True
            legendBottomWC.Visible = True
        End If

        modelrunid = ""
        ''If Not Request.QueryString("modelrunid") Is Nothing Then
        ''    modelrunid = Request.QueryString("modelrunid")
        ''End If
        ''If Not Request.QueryString("modelstart") Is Nothing Then
        ''    If IsDate(Request.QueryString("modelstart")) Then _selecteddate = CDate(Request.QueryString("modelstart"))
        ''End If
        'Dim ctl As Control = Me.Parent.FindControl("hddnModelRunID")
        'If Not IsNothing(ctl) Then modelrunid = CType(ctl, HiddenField).Value.Trim
        'ctl = Me.Parent.FindControl("hddnModelStart")
        'If Not IsNothing(ctl) Then
        '    If IsDate(CType(ctl, HiddenField).Value.Trim) Then _selecteddate = CDate(CType(ctl, HiddenField).Value.Trim)
        'End If

        If Not IsPostBack Then

            modelrunid = Session("fosmodelrunid").ToString
            If IsNothing(Session("fosmodelstartfos")) Or CDate(Session("fosmodelstartfos").ToString) = CDate("12:00:00 AM") Then Session("fosmodelstartfos") = Session("fosmodelstart")

            '20140416 - pab - add select time zone
            '20171027 - pab - calendar
            'If Me.ddlTimeZone.Items.Count = 0 Then HydrateddlTimeZone(CDate(Session("fosmodelstartfos").ToString))
            If Me.rcTimeZone.Items.Count = 0 Then
                HydrateddlTimeZone(CDate(Session("fosmodelstartfos").ToString))
                Me.rcTimeZone.SelectedIndex = 0
            End If

            '20170104- pab - add select by actype
            '20171027 - pab - calendar
            'If Me.ddlACType.Items.Count = 0 Then HydrateddlACType()
            If Me.rcACType.Items.Count = 0 Then
                HydrateddlACType()
                Me.rcACType.SelectedIndex = 0
            End If

            '20170316 - pab - load from and to with initial values
            If IsNothing(Me.from_date.SelectedDate) Then
                Me.from_date.SelectedDate = CDate(Now.ToShortDateString)
            End If
            If IsNothing(Me.to_date.SelectedDate) Then
                'Dim dateto As Date
                '20171017 - pab - fix date range
                Dim da As New DataAccess
                Dim FOSCalendarDays As Integer = CInt(da.getsettingnumeric(_carrierid, "FOSCalendarDays"))
                If FOSCalendarDays < 1 Then FOSCalendarDays = 4
                'Me.to_date.SelectedDate = CDate(DateAdd(DateInterval.Day, 2, CDate(Me.from_date.SelectedDate)).ToShortDateString)
                Me.to_date.SelectedDate = CDate(DateAdd(DateInterval.Day, FOSCalendarDays - 1, CDate(Me.from_date.SelectedDate)).ToShortDateString)
            End If

            '20140123 - pab - fix error - Object reference not set to an instance of an object
            '20170312 - pab - use calendar from and to
            'If Not IsNothing(_urlalias) Then Me.lblCarrier.Text = _urlalias.ToUpper
            If modelrunid <> "" Then BindData(modelrunid)

            Select Case _carrierid
                Case 49
                    lblCustomType1.Text = "Crew"
                    lblCustomType2.Text = "Crew"

                    '20170102 - pab - fix calendar
                    lblCustomType3.Text = "Quick Flight"
                    lblCustomType4.Text = "Quick Flight"

                Case 65
                    lblCustomType1.Text = "Bull Pen"
                    lblCustomType2.Text = "Bull Pen"

                    '20170102 - pab - fix calendar
                    lblCustomType3.Text = "Quick Flight"
                    lblCustomType4.Text = "Quick Flight"

                Case 100
                    lblCustomType1.Text = "No Crew"
                    lblCustomType2.Text = "No Crew"

                    '20170102 - pab - fix calendar
                    lblCustomType3.Text = "SWAP"
                    lblCustomType4.Text = "SWAP"

                Case Else
                    lblCustomType1.Text = "Other"
                    lblCustomType2.Text = "Other"

                    '20170102 - pab - fix calendar
                    lblCustomType3.Text = "Quick Flight"
                    lblCustomType4.Text = "Quick Flight"
            End Select

        End If

    End Sub

    '20140416 - pab - add select time zone
    Private Sub HydrateddlTimeZone(ByRef modelstart As Date)

        Dim da As New DataAccess
        Dim dt As DataTable = da.GetDSTbyDate(modelstart)
        Dim bDST As Boolean = False
        '20171027 - pab - calendar
        'Dim li As ListItem = Nothing
        Dim DefaultTimeZone As String = da.GetSetting(_carrierid, "Default Time Zone")

        If Not isdtnullorempty(dt) Then
            bDST = True
            DefaultTimeZone = Replace(DefaultTimeZone, "S", "D")
        End If

        dt.Clear()
        dt = da.GetValues(58, "ddlTimeZone")

        'Me.ddlTimeZone.Items.Clear()
        Me.rcTimeZone.Items.Clear()

        If Not isdtnullorempty(dt) Then
            For Each dr As DataRow In dt.Rows
                Dim ti As New Telerik.Web.UI.RadComboBoxItem
                If bDST = True Then
                    If dr("description").ToString = "GMT" Then
                        '20171027 - pab - calendar
                        'li = New ListItem(Replace(dr("description").ToString, "S", "D"), dr("value").ToString)
                        ti.Text = Replace(dr("description").ToString, "S", "D")
                        ti.Value = dr("value").ToString.Trim
                    Else
                        '20171027 - pab - calendar
                        'li = New ListItem(Replace(dr("description").ToString, "S", "D"), CStr(CInt(dr("value").ToString) + 1))
                        ti.Text = Replace(dr("description").ToString, "S", "D")
                        ti.Value = CStr(CInt(dr("value").ToString) + 1)
                    End If
                Else
                    '20171027 - pab - calendar
                    'li = New ListItem(dr("description").ToString, dr("value").ToString)
                    ti.Text = dr("description").ToString
                    ti.Value = dr("value").ToString
                End If
                rcTimeZone.Items.Add(ti)
            Next
        End If

        '20170213 - pab - default calendar time zone
        'For i As Integer = 0 To Me.ddlTimeZone.Items.Count - 1
        '    If Me.ddlTimeZone.Items(i).Text = DefaultTimeZone Then
        '        Me.ddlTimeZone.SelectedIndex = i
        '        Exit For
        '    End If
        'Next
        If IsNothing(_CalendarTimeZone) Then
            _CalendarTimeZone = "GMT"

            If da.GetSetting(_carrierid, "CalendarTimeZone") <> "" Then
                _CalendarTimeZone = da.GetSetting(_carrierid, "CalendarTimeZone")
            End If
            '20171027 - pab - calendar
            'ddlStyle.SelectedValue = _CalendarTimeZone
            rcTimeZone.SelectedValue = _CalendarTimeZone
        End If

    End Sub

    '20170104- pab - add select by actype
    Private Sub HydrateddlACType()

        Dim da As New DataAccess
        Dim dt As DataTable = da.GetCASFlightsACType(_carrierid, Session("fosmodelrunid").ToString)
        '20171027 - pab - calendar
        'Dim li As ListItem = Nothing
        Dim ti As Telerik.Web.UI.RadComboBoxItem

        '20171027 - pab - calendar
        'Me.ddlACType.Items.Clear()
        'li = New ListItem("All", "")
        'Me.ddlACType.Items.Add(li)
        Me.rcACType.Items.Clear()
        ti = New Telerik.Web.UI.RadComboBoxItem("All", "")
        Me.rcACType.Items.Add(ti)

        '20170213 - pab - add code for ac type
        If _carrierid = 100 Then
            '20171027 - pab - calendar
            'li = New ListItem("B350", "B350")
            'Me.ddlACType.Items.Add(li)
            ti = New Telerik.Web.UI.RadComboBoxItem("B350", "B350")
            Me.rcACType.Items.Add(ti)
        End If

        If Not isdtnullorempty(dt) Then
            For Each dr As DataRow In dt.Rows
                '20171027 - pab - calendar
                'li = New ListItem(dr("ACType").ToString, dr("ACType").ToString)
                'Me.ddlACType.Items.Add(li)
                ti = New Telerik.Web.UI.RadComboBoxItem(dr("ACType").ToString, dr("ACType").ToString)
                Me.rcACType.Items.Add(ti)
            Next
        End If

    End Sub

    Private Sub BindData(ByRef ModelRunID As String)

        Dim startDate As DateTime
        Dim endDate As DateTime

        AirTaxi.post_timing("FOSFlightsCalendar binddata start  " & Now.ToString)

        tblFlightPlanningWeekly.Rows.Clear()

        '20170312 - pab - use calendar from and to
        If IsNothing(from_date.SelectedDate) Then
            Session("fosmodelstartfos") = Now
        Else
            Session("fosmodelstartfos") = from_date.SelectedDate
        End If

        If CDate(Session("fosmodelstartfos").ToString) > CDate("1/1/2010 10:01 AM ") Then
            startDate = CDate(CDate(Session("fosmodelstartfos").ToString).ToShortDateString)  ' CDate(Me.Calendar1.SelectedDate.ToString("d"))
            endDate = startDate  ' CDate(Me.Calendar1.SelectedDate.ToString("d"))
            'Me.Calendar1.SelectedDate = startDate

        Else
            'startDate = CDate(Me.Calendar1.SelectedDate.ToString("d"))
            'endDate = CDate(Me.Calendar1.SelectedDate.ToString("d"))
            startDate = CDate(Now.ToString("d"))
            endDate = CDate(Now.ToString("d"))

        End If

        '20140416 - pab - add select time zone
        Dim timezone As String = "GMT"
        Dim offset As Integer = 0
        '20171027 - pab - calendar
        'If Me.ddlTimeZone.SelectedIndex >= 0 Then
        '    timezone = Me.ddlTimeZone.SelectedItem.Text
        '    offset = CInt(Me.ddlTimeZone.SelectedValue)
        'End If
        If Me.rcTimeZone.SelectedIndex >= 0 Then
            timezone = Me.rcTimeZone.SelectedItem.Text
            offset = CInt(Me.rcTimeZone.SelectedValue)
        End If

        '20170104 - pab - fix calendar
        If ModelRunID = "" Then
            ModelRunID = Session("fosmodelrunid").ToString
        End If

        Dim da As New DataAccess
        'Dim ds As DataTable = da.GetFOSFlightsCalendarByCarrierID(_carrierid, ModelRunID)

        '20170317 - pab - fix calendar
        If ModelRunID = "" Then
            ModelRunID = da.GetFOSFlightsBestModelRunID(_carrierid)
        End If

        '20140416 - pab - add select time zone
        'Dim ds As DataTable = da.GetFOSFlightsCalendarByCarrierIDDate(_carrierid, ModelRunID, startDate)
        Dim ds As DataTable = da.GetFOSFlightsCalendarByCarrierIDDateOffset(_carrierid, ModelRunID, startDate, offset)

        For i As Integer = 1 To 3
            If Not ds Is Nothing Then
                If ds.Rows.Count > 0 Then
                    Exit For
                End If
                ds = da.GetFOSFlightsCalendarByCarrierIDDateOffset(_carrierid, ModelRunID, startDate, offset)

            Else
                ds = da.GetFOSFlightsCalendarByCarrierIDDateOffset(_carrierid, ModelRunID, startDate, offset)

            End If
        Next

        Dim dtcrew As DataTable = da.GetFOSFlightsCalendarCrewDate(_carrierid, ModelRunID, startDate)

        If Not ds Is Nothing Then
            If ds.Rows.Count > 0 Then

                '20131008 - pab - FOS calendar days
                Dim FOSCalendarDays As Integer = CInt(da.getsettingnumeric(_carrierid, "FOSCalendarDays"))
                If FOSCalendarDays < 1 Then FOSCalendarDays = 4

                '20110711 - pab - improve calendar performance
                '20170312 - pab - use calendar from and to
                '20131008 - pab - FOS calendar days
                'Dim sdays As String = "3 Days"
                Dim sdays As String = FOSCalendarDays.ToString '& " Days"
                If IsNothing(to_date.SelectedDate) Then
                    'endDate = DateAdd(DateInterval.Day, 3, startDate)
                    endDate = DateAdd(DateInterval.Day, FOSCalendarDays - 1, startDate)
                    Me.to_date.SelectedDate = CDate(DateAdd(DateInterval.Day, FOSCalendarDays - 1, CDate(Me.from_date.SelectedDate)).ToShortDateString)
                Else
                    endDate = CDate(to_date.SelectedDate)
                    Dim ldays As Long = DateDiff(DateInterval.Day, startDate, endDate) + 1
                    If ldays < 0 Then
                        'sdays = "3"
                        sdays = "4"
                    ElseIf ldays = 0 Or ldays = 1 Then
                        sdays = "24 Hours"
                    ElseIf ldays > 7 Then
                        sdays = "7"
                    Else
                        sdays = ldays.ToString
                    End If
                    '20171017 - pab - fix date range
                    FOSCalendarDays = CInt(ldays)
                End If
                'Select Case ddlView.SelectedValue
                Select Case sdays
                    Case "24 Hours"
                        AirTaxi.post_timing("FOSFlightsCalendar binddata flights daily  " & Now.ToString)
                        'chg3641 - 20101008 - pab - add daily/weekly/monthly calendar views to flight planning
                        'daily view
                        'lblDate.Text = startDate.ToString("d")
                        '20130710 - pab - FOS calendar
                        'Dim FOSCalendarDays As Integer = 1
                        '20110214 - pab - more calendar changes
                        endDate = CDate(endDate.ToString("d") & " 23:59")
                        AirTaxi.post_timing("date range  " & startDate & " - " & endDate)
                        '20130710 - pab - FOS calendar
                        'ds = da.GetFleetPlanningDaily(_carrierid, startDate, endDate)
                        'AddFleetPlanningRowsDaily(startDate, endDate, ds.Tables(0))

                        '20140416 - pab - add select time zone
                        '20170102 - pab - fix calendar
                        AddFleetPlanningRowsWeekly(startDate, CDate(endDate.ToString("d")), ds, dtcrew, FOSCalendarDays, timezone, offset)

                    '20140901 = pab - add 5 and 7 day views
                    Case "3 Days", "5", "7", "2", "3", "4", "6"
                        'weekly view
                        AirTaxi.post_timing("FOSFlightsCalendar binddata flights weekly  " & Now.ToString)
                        'Dim n As Integer
                        'n = startDate.DayOfWeek
                        'startDate = DateAdd(DateInterval.Day, 0 - n, startDate)

                        'Me.lblModelRunID.Text = ds.Rows(0).Item("modelrun").ToString.Trim

                        '20130710 - pab - FOS calendar
                        '20140901 = pab - add 5 and 7 day views
                        ''Dim FOSCalendarDays As Integer = 0
                        ''20170312 - pab - use calendar from and to
                        ''If ddlView.SelectedValue = "3 Days" Then
                        ''    FOSCalendarDays = CInt(da.getsettingnumeric(_carrierid, "FOSCalendarDays"))
                        ''Else
                        ''    FOSCalendarDays = CInt(ddlView.SelectedValue)
                        ''End If
                        'If IsNumeric(sdays) Then
                        '    FOSCalendarDays = CInt(sdays)
                        'Else
                        '    FOSCalendarDays = 3
                        'End If

                        ''endDate = DateAdd(DateInterval.Day, 6, startDate)
                        'If FOSCalendarDays < 1 Then FOSCalendarDays = 3
                        endDate = DateAdd(DateInterval.Day, FOSCalendarDays - 1, startDate)
                        endDate = CDate(endDate.ToString("d") & " 23:59")
                        'ds = da.GetFleetPlanningDaily(_carrierid, startDate, endDate)
                        AirTaxi.post_timing("date range  " & startDate & " - " & endDate)
                        'If ddlView.SelectedValue = "Weekly" Then
                        '    lblDate.Text = startDate.ToString("d") & " - " & endDate.ToString("d")
                        'End If

                        '20130710 - pab - FOS calendar
                        'AddFleetPlanningRowsWeekly(startDate, CDate(endDate.ToString("d")), ds)

                        '20140416 - pab - add select time zone
                        '20170102 - pab - fix calendar
                        '20170203 - pab - fix calendar
                        'AddFleetPlanningRowsWeekly(startDate, CDate(endDate.ToString("d")), ds, dtcrew, FOSCalendarDays, timezone, offset)
                        AddFleetPlanningRowsWeekly(startDate, endDate, ds, dtcrew, FOSCalendarDays, timezone, offset)

                    ''Dim buttonToolTip As New ToolTip()
                    ''buttonToolTip.ToolTipTitle = "Tooltip"
                    ''buttonToolTip.UseFading = True
                    ''buttonToolTip.UseAnimation = True
                    ''buttonToolTip.IsBalloon = True
                    ''buttonToolTip.ShowAlways = True
                    ''buttonToolTip.AutoPopDelay = 5000
                    ''buttonToolTip.InitialDelay = 1000
                    ''buttonToolTip.ReshowDelay = 500
                    ''buttonToolTip.IsBalloon = True
                    ''buttonToolTip.SetToolTip(lblDate, "Show Tooltip")

                    'Case "Monthly"
                    '    'monthly view
                    '    AirTaxi.post_timing("FOSFlightsCalendar binddata flights monthly  " & Now.ToString)
                    '    Dim s As String = String.Empty
                    '    startDate = Me.Calendar1.SelectedDate
                    '    startDate = CDate(startDate.Month & "/" & 1 & "/" & startDate.Year)
                    '    s = endDate.Month & "/" & 31 & "/" & endDate.Year
                    '    If IsDate(s) Then
                    '        endDate = CDate(s)
                    '    Else
                    '        s = endDate.Month & "/" & 30 & "/" & endDate.Year
                    '        If IsDate(s) Then
                    '            endDate = CDate(s)
                    '        Else
                    '            s = endDate.Month & "/" & 29 & "/" & endDate.Year
                    '            If IsDate(s) Then
                    '                endDate = CDate(s)
                    '            Else
                    '                s = endDate.Month & "/" & 28 & "/" & endDate.Year
                    '                endDate = CDate(s)
                    '            End If
                    '        End If
                    '    End If
                    '    endDate = CDate(endDate.ToString("d") & " 23:59")
                    '    AirTaxi.post_timing("date range  " & startDate & " - " & endDate)
                    '    ds = da.GetFleetPlanningDaily(_carrierid, startDate, endDate)
                    '    AddFleetPlanningRowsMonthly(startDate, CDate(endDate.ToString("d")), ds.Tables(0))
                    '    If ddlView.SelectedValue = "Monthly" Then
                    '        lblDate.Text = startDate.ToString("MMMM") & ", " & startDate.Year
                    '    End If

                    '20170104 - pab - default to 3 days
                    Case Else
                        '20131008 - pab - FOS calendar days
                        'Dim FOSCalendarDays As Integer = 3
                        endDate = DateAdd(DateInterval.Day, FOSCalendarDays - 1, startDate)
                        endDate = CDate(endDate.ToString("d") & " 23:59")
                        AirTaxi.post_timing("date range  " & startDate & " - " & endDate)
                        AddFleetPlanningRowsWeekly(startDate, CDate(endDate.ToString("d")), ds, dtcrew, FOSCalendarDays, timezone, offset)

                End Select

            End If
        End If

        AirTaxi.post_timing("FOSFlightsCalendar binddata end  " & Now.ToString)

    End Sub

    '20170621 - pab - add column headers as footers
    Sub AddHeaders(ByVal startDateRange As DateTime, ByRef timezone As String, ByVal FOSCalendarDays As Integer)

        Dim da As New DataAccess
        Dim tr As TableRow = Nothing
        Dim td(8) As TableCell

        tr = New TableRow

        Dim cc As String = da.GetSetting(_carrierid, "FrameColor")
        If cc <> "" Then
            tr.Style.Add("color", "white")
            tr.Style.Add("font-weight", "bold")
            tr.Style.Add("background-color", cc)
        Else
            tr.Style.Add("background-color", "#9CCFFF")
        End If
        tr.Style.Add("border-bottom", "1px solid gray")

        '20130710 - pab - FOS calendar
        If FOSCalendarDays > 7 Then FOSCalendarDays = 7
        'For i = 0 To td.Length - 1
        For i = 0 To FOSCalendarDays
            td(i) = New TableCell
            td(i).HorizontalAlign = HorizontalAlign.Center
            If i = 0 Then
                td(i).Width = 100
            Else
                'td(i).Width = 130 * 2
                'td(i).Width = CInt(1140 / FOSCalendarDays)
                td(i).Width = CInt(1475 / FOSCalendarDays)
                td(i).Font.Size = CType(16, FontUnit)
                td(i).Font.Bold = True
            End If
        Next
        td(0).Text = "Date"
        tr.Cells.Add(td(0))

        td(1).Style.Add("border-left", "1px solid gray")
        td(1).Text = startDateRange.ToString("d")

        '20130710 - pab - FOS calendar
        'For i = 1 To td.Length - 1
        For i = 1 To FOSCalendarDays
            td(i).Style.Add("border-right", "1px solid gray")
            If i > 1 Then
                td(i).Text = DateAdd(DateInterval.Day, i - 1, startDateRange).ToString("d")
            End If
            tr.Cells.Add(td(i))
        Next

        Me.tblFlightPlanningWeekly.Rows.Add(tr)
        tr = Nothing

        tr = New TableRow

        If cc <> "" Then
            tr.Style.Add("color", "white")
            tr.Style.Add("font-weight", "bold")
            tr.Style.Add("background-color", cc)
        Else
            tr.Style.Add("background-color", "#9CCFFF")
        End If
        tr.Style.Add("border-bottom", "1px solid gray")

        '20130710 - pab - FOS calendar
        'For i = 0 To td.Length - 1
        For i = 0 To FOSCalendarDays
            td(i) = New TableCell
            td(i).HorizontalAlign = HorizontalAlign.Center
            If i = 0 Then
                td(i).Width = 100
            Else
                'td(i).Width = 130 * 2
                td(i).Width = CInt(1140 / FOSCalendarDays)
            End If
        Next
        '20140416 - pab - add select time zone
        'td(0).Text = "Time(GMT)"
        td(0).Text = "Time(" & timezone & ")"
        tr.Cells.Add(td(0))

        td(1).Style.Add("border-left", "1px solid gray")
        td(1).Text = startDateRange.ToString("d")

        '20130710 - pab - FOS calendar
        'For i = 1 To td.Length - 1
        For i = 1 To FOSCalendarDays
            'td(i).Style.Add("border-right", "1px solid gray")
            td(i).Text = "<TABLE wIDTH=100% border=1><TR><TD align=center>00:00</TD><TD align=center>06:00</TD><TD align=center>12:00</TD><TD align=center>18:00</TD></TR></TABLE>"
            tr.Cells.Add(td(i))
        Next

        Me.tblFlightPlanningWeekly.Rows.Add(tr)
        tr = Nothing

    End Sub

    '20140416 - pab - add select time zone
    '20170102 - pab - fix calendar
    Private Sub AddFleetPlanningRowsWeekly(ByVal startDateRange As DateTime, ByVal endDateRange As DateTime, ByVal dt_planning As DataTable,
            ByVal dtcrew As DataTable, ByVal FOSCalendarDays As Integer, ByRef timezone As String, ByVal offset As Integer)

        'FlightID
        'AircraftID
        'Registration	
        'DepartureAirport
        'DepartureTime
        'ArrivalAirport
        'ArrivalTime
        'FlightType
        'Status
        'FlightDetail
        'ModelRun
        'Distance
        'SeatsAvailable
        'AircraftTypeServiceSpecID

        AirTaxi.post_timing("AddFleetPlanningRowsWeekly start  " & Now.ToString)

        Dim tr As TableRow = Nothing
        Dim td(8) As TableCell

        '20140130 - add crew data
        Dim tdcrew(8) As TableCell

        Dim tdtext As New ArrayList

        Dim aircraft As String = String.Empty

        Dim timeAddedToRow As Boolean = False

        Dim startTime As Date = Nothing
        Dim endTime As DateTime = Nothing

        Dim previousRegistration As String = String.Empty
        Dim previousDepartureTime As DateTime = startDateRange
        Dim previousArrivalTime As DateTime = startDateRange
        Dim previousDepartureAirport As String = String.Empty
        Dim previousArrivalAirport As String = String.Empty
        Dim previousCssClass As String = "planningCellOn"
        '20161227 - pab - fix calendar - wu flight detail not numeric
        'Dim previousflightid As Integer = 0
        Dim previousflightid As String = ""
        Dim previousAircraftID As Integer = 0
        Dim distance As Integer = 0
        Dim previousDepartureICAO As String = String.Empty
        Dim previousArrivalICAO As String = String.Empty
        Dim previousFlightType As String = String.Empty

        '20170222 - pab - rti
        Dim previousACType As Integer = 0

        Dim dt As New DataTable
        Dim da As New DataAccess

        Dim tail As String = ""
        Dim actype As String = ""
        Dim passengers As Integer = 0
        Dim seatsavailable As Integer = 0
        Dim rownumber As Integer = 0
        Dim username As String = ""
        '20161227 - pab - fix calendar - wu flight detail not numeric
        'Dim flightdtl As Integer = 0
        Dim flightdtl As String = ""
        Dim tddetail(8) As TableCell
        Dim acname As String = ""
        Dim AircraftID As Integer = 0

        '20140818 - pab - make text display more like fos
        Dim bFlightInfoOnly As Boolean = False
        Dim previousDepartTime As DateTime = startDateRange

        '20170213 - pab - add code for ac type
        Dim tailcolor As String = ""

        '20170320 - pab - show ac when not scheduled move to after ac change check
        Dim bskip As Boolean = False

        '20170721 - pab - jlx - show bkr
        Dim basecode As String = ""

        '20170621 - pab - add column headers as footers
        Dim previousweightclass As String = String.Empty
        AddHeaders(startDateRange, timezone, FOSCalendarDays)
        tr = Nothing

        Try

            For Each dr As DataRow In dt_planning.Rows

                'for testing only
                If dr("Registration").ToString = "N503UP" Or dr("Registration").ToString = "N503UP" Then         '404T  833T    405T  N501UP N827UP
                    distance = distance
                End If

                '20170320 - pab - show ac when not scheduled move to after ac change check
                bskip = False

                '20170102 - pab - fix calendar
                If DateAdd(DateInterval.Hour, offset, CDate(dr("arrivaltimegmt"))) < startDateRange Or
                        DateAdd(DateInterval.Hour, offset, CDate(dr("departuretimegmt"))) > endDateRange Then
                    '20170320 - pab - show ac when not scheduled move to after ac change check
                    Continue For
                    'bskip = True
                End If

                '20170213 - pab - add code for ac type
                '20171027 - pab - calendar
                'If ddlACType.SelectedValue = "B350" Then
                If rcACType.SelectedValue = "B350" Then
                    'If InStr(dr("actype").ToString.ToUpper, ddlACType.SelectedValue.ToUpper) = 0 Then
                    If InStr(dr("actype").ToString.ToUpper, rcACType.SelectedValue.ToUpper) = 0 Then
                        '20170320 - pab - show ac when not scheduled move to after ac change check
                        Continue For
                        'bskip = True
                    End If
                    'ElseIf ddlACType.SelectedValue <> "" Then
                ElseIf rcACType.SelectedValue <> "" Then
                    'If dr("actype").ToString.ToUpper <> ddlACType.SelectedValue.ToUpper Then
                    If dr("actype").ToString.ToUpper <> rcACType.SelectedValue.ToUpper Then
                        '20170320 - pab - show ac when not scheduled move to after ac change check
                        Continue For
                        'bskip = True
                    End If
                End If

                'skip bad records
                If IsDBNull(dr("Distance")) Then
                    distance = 0
                Else
                    distance = CInt(dr("Distance").ToString)
                End If
                '20140416 - pab - add select time zone
                'If dr("DepartureAirport").ToString.Trim = "" And dr("ArrivalAirport").ToString.Trim = "" And distance = 0 Then
                '20140911 - pab - show all aircraft
                'If (dr("DepartureAirport").ToString.Trim = "" And dr("ArrivalAirport").ToString.Trim = "" And distance = 0) _
                '    Or (CDate(dr("departuretime")) <= startDateRange And CDate(dr("arrivaltime")) >= endDateRange And _
                '        (CStr(dr("flighttype")) = "B" Or CStr(dr("flighttype")) = "M")) Then
                '    'garbage, maintenance or bull pen - ignore and get next record

                'Else
                'for testing only
                If dr("Registration").ToString = "N514UP" Then         '404T  833T    405T  N501UP N827UP
                    distance = distance
                    'Exit For
                End If

                'new aircraft
                If dr("Registration").ToString <> previousRegistration Then

                    'write row for previous aircraft if it exists
                    If Not tr Is Nothing Then
                        'look to pad last row thru midnight
                        If previousArrivalTime >= CDate(endDateRange.ToString("d") & " 11:59PM") Then
                            'already formatted - do nothing
                        Else
                            If previousArrivalTime.ToShortTimeString = "11:59 PM" Then previousArrivalTime = DateAdd(DateInterval.Minute, 1, previousArrivalTime)
                            If UCase(Session("CalendarStyle").ToString) <> "TEXT" Then
                                'tdtext = FormatRowWeekly(startDateRange, endDateRange, 0, 0, "", previousArrivalAirport, previousArrivalTime, previousArrivalAirport, _
                                '          CDate(endDateRange.ToString("d") & " 11:59PM"), previousCssClass, previousArrivalAirport, passengers, seatsavailable, _
                                '          rownumber, "")
                                '20140416 - pab - add select time zone
                                '20161227 - pab - fix calendar - wu flight detail not numeric
                                '20170106 - pab - no crew flagged as m
                                '20170721 - pab - jlx - show bkr
                                '20170913 - pab - add cost and leg type code to mouseover
                                tdtext = FormatRowWeekly(startDateRange, endDateRange, "0", 0, "", previousArrivalICAO, previousArrivalTime, previousArrivalICAO,
                                          CDate(endDateRange.ToString("d") & " 11:59PM"), "", previousArrivalICAO, passengers, seatsavailable,
                                          rownumber, "", FOSCalendarDays, timezone, "", basecode, 0)

                                '20130710 - pab - FOS calendar
                                'For i = 1 To td.Length - 1
                                For i = 1 To FOSCalendarDays
                                    td(i).Text &= tdtext(i - 1).ToString
                                Next

                            End If

                        End If

                        td(0) = New TableCell
                        td(0).CssClass = "planningCellTextFOS"
                        'td(0).Style.Add("border-right", "1px solid gray")
                        td(0).Style.Add("border-top", "1px solid gray")
                        '20170213 - pab - add code for ac type
                        if tailcolor <> "" Then td(0).Style.Add("background-color", tailcolor)
                        '20140818 - pab - make text display more like fos
                        If UCase(Session("CalendarStyle").ToString) = "TEXT" Then td(0).VerticalAlign = VerticalAlign.Top
                        td(0).Text = tail
                        tr.Cells.Add(td(0))

                        If UCase(Session("CalendarStyle").ToString) = "TEXT" Then
                            tr.CssClass = "planningCellTextFOS"
                            td(1).Style.Add("border-left", "1px solid gray")

                            '20130710 - pab - FOS calendar
                            'For i = 1 To td.Length - 1
                            For i = 1 To FOSCalendarDays
                                td(i).Style.Add("border-right", "1px solid gray")
                                '20140818 - pab - make text display more like fos
                                td(i).VerticalAlign = VerticalAlign.Top
                            Next
                        Else
                            td(1).Style.Add("border-left", "1px solid gray")

                            '20130710 - pab - FOS calendar
                            'td(td.Length - 1).Style.Add("border-right", "1px solid gray")
                            td(FOSCalendarDays).Style.Add("border-right", "1px solid gray")
                        End If

                        '20130710 - pab - FOS calendar
                        'For i = 1 To td.Length - 1
                        For i = 1 To FOSCalendarDays
                            Dim div As String = td(i).Text.ToString
                            td(i).Text = checkdivwidth(div)
                            tr.Cells.Add(td(i))
                        Next

                        Me.tblFlightPlanningWeekly.Rows.Add(tr)

                        'tr = New TableRow

                        'rownumber = rownumber + 1
                        'tr.ID = "trw" & rownumber
                        'If _CalendarDetailVisible = True Then
                        '    tr.Style.Add("visibility", "visible")
                        'Else
                        '    tr.Style.Add("visibility", "collapse")
                        'End If
                        'tr.Style.Add("position", "relative")
                        'tr.CssClass = "planningCellText"

                        'td(0) = New TableCell
                        'td(0).Text = actype
                        'tr.Cells.Add(td(0))

                        'tddetail(1).Style.Add("border-left", "1px solid gray")
                        'For i = 1 To td.Length - 1
                        '    tddetail(i).Style.Add("border-right", "1px solid gray")
                        '    tddetail(i).Style.Add("font-size", "xx-small")
                        '    tddetail(i).Text = "<div onclick=""hideOrShow(trw" & rownumber & ");"" align=""left"">" & tddetail(i).Text & "</div>"
                        '    tr.Cells.Add(tddetail(i))
                        'Next

                        'Me.tblFlightPlanningWeekly.Rows.Add(tr)
                        'tr = Nothing

                        If UCase(Session("CalendarStyle").ToString) = "TEXT" Then
                            tr = New TableRow
                            tr.Style.Add("height", "3px")
                            'td(0) = New TableCell

                            '20130710 - pab - FOS calendar
                            'td(0).ColumnSpan = 8
                            '20140901 - pab - fix calendar - graphical line showing between text lines
                            'td(0).ColumnSpan = FOSCalendarDays + 1
                            'td(0).Text = ""
                            'tr.Cells.Add(td(0))
                            'rownumber = rownumber + 1
                            For i = 0 To FOSCalendarDays + 1
                                td(i) = New TableCell
                                td(i).Text = ""
                                tr.Cells.Add(td(i))
                            Next

                            Me.tblFlightPlanningWeekly.Rows.Add(tr)
                            tr = Nothing

                            'Else
                            '    '20140130 - add crew data
                            '    tr = Nothing
                            '    Dim bAddCrew As Boolean = False
                            '    tr = New TableRow
                            '    tr.Style.Add("height", "10px")
                            '    For i = 0 To FOSCalendarDays + 1
                            '        td(1) = New TableCell
                            '    Next
                            '    '20130710 - pab - FOS calendar
                            '    'td(0).ColumnSpan = 8
                            '    td(0).ColumnSpan = FOSCalendarDays + 1
                            '    td(0).Text = ""
                            '    tr.Cells.Add(td(0))
                            '    rownumber = rownumber + 1
                            '    For Each dr2 As DataRow In dtcrew.Rows
                            '        If dr2("Registration").ToString = previousRegistration Then
                            '            tdtext = FormatRowWeekly(startDateRange, endDateRange, CInt(dr2("flightid")), CInt(dr2("aircraftid")), CStr(dr2("Registration")), _
                            '                CStr(dr2("departureairport")), CDate(dr2("departuretime")), CStr(dr2("arrivalairport")), CDate(dr2("arrivaltime")), _
                            '                CStr(dr2("flighttype")), "", passengers, seatsavailable, rownumber, "A", FOSCalendarDays)
                            '            For i = 1 To FOSCalendarDays
                            '                td(i).Text &= tdtext(i - 1).ToString
                            '            Next
                            '            bAddCrew = True
                            '        End If
                            '    Next
                            '    If bAddCrew = True Then
                            '        td(1).Style.Add("border-left", "1px solid gray")
                            '        For i = 1 To FOSCalendarDays
                            '            tr.Cells.Add(td(i))
                            '        Next
                            '        Me.tblFlightPlanningWeekly.Rows.Add(tr)
                            '    End If
                            '    tr = Nothing

                        End If

                        '20170619 - pab - new msg queue
                        If dr("weightclass").ToString.Trim <> previousweightclass Then
                            If previousweightclass <> "" And UCase(Session("CalendarStyle").ToString) <> "TEXT" Then
                                AddHeaders(startDateRange, timezone, FOSCalendarDays)
                            End If
                        End If

                        tr = New TableRow
                        'tr.Style.Add("height", "10px")

                        '20140826 - pab - 
                        If UCase(Session("CalendarStyle").ToString) = "TEXT" Then tr.Style.Add("height", "200px")

                        tdcrew(0) = New TableCell
                        'td(0).ColumnSpan = FOSCalendarDays + 1
                        tdcrew(0).Text = "<div  style=""float:left;width:100%;"" class=""planningCellOn""></div>"
                        tr.Cells.Add(tdcrew(0))
                        Dim bAddCrew As Boolean = False
                        For i As Integer = 1 To FOSCalendarDays
                            tdcrew(i) = New TableCell
                            'tdcrew(i).Text = "<div  style=""float:left;width:100%;"" class=""planningCellFlightPlanningQuickFlight""></div>"
                            tr.Cells.Add(tdcrew(i))
                        Next
                        For Each dr2 As DataRow In dtcrew.Rows
                            If dr2("Registration").ToString = previousRegistration Then
                                '20140416 - pab - add select time zone
                                '20140818 - pab - make text display more like fos
                                '20161227 - pab - fix calendar - wu flight detail not numeric
                                '20170106 - pab - no crew flagged as m
                                '20170721 - pab - jlx - show bkr
                                '20170913 - pab - add cost and leg type code to mouseover
                                tdtext = FormatRowWeekly(startDateRange, endDateRange, dr2("FlightDetail").ToString.Trim, CInt(dr2("aircraftid")), CStr(dr2("Registration")),
                                    CStr(dr2("departureairport")), CDate(dr2("departuretime")), CStr(dr2("arrivalairport")), CDate(dr2("arrivaltime")),
                                    CStr(dr2("flighttype")), "", passengers, seatsavailable, rownumber, "A", FOSCalendarDays, timezone, dr2("legtypecode").ToString.Trim,
                                    dr2("basecode").ToString.Trim, CInt(dr2("cost")))
                                For i = 1 To FOSCalendarDays
                                    tdcrew(i).Text &= tdtext(i - 1).ToString
                                Next
                                bAddCrew = True
                            End If
                        Next
                        '20140901 - pab - fix text calendar 
                        'If bAddCrew = True Then
                        If bAddCrew = True And UCase(Session("CalendarStyle").ToString) <> "TEXT" Then
                            tdcrew(1).Style.Add("border-left", "1px solid gray")
                            For i = 1 To FOSCalendarDays
                                tr.Cells.Add(tdcrew(i))
                            Next
                            Me.tblFlightPlanningWeekly.Rows.Add(tr)
                        End If
                        tr = Nothing

                        '20130710 - pab - FOS calendar
                        'For i = 1 To td.Length - 1
                        For i = 1 To FOSCalendarDays
                            tddetail(i) = New TableCell
                        Next

                        '20170106 - pab - no crew flagged as m
                        previousCssClass = GetCssClass("", "", "")
                        previousFlightType = ""
                        previousDepartureTime = Nothing
                        previousArrivalTime = Nothing
                        previousDepartureAirport = ""
                        previousArrivalAirport = ""
                        '20161227 - pab - fix calendar - wu flight detail not numeric
                        previousflightid = ""
                        previousDepartureICAO = ""
                        previousArrivalICAO = ""

                        '20140818 - pab - make text display more like fos
                        previousDepartTime = Nothing

                    End If

                    'new aircraft
                    previousRegistration = dr("Registration").ToString
                    previousAircraftID = 0

                    '20170619 - pab - new msg queue
                    previousweightclass = dr("weightclass").ToString.Trim

                    '20170721 - pab - jlx - show bkr
                    basecode = dr("basecode").ToString.Trim

                    '20170222 - pab - rti
                    Dim dtac As New DataTable
                    If Not IsDBNull(dr("AircraftTypeServiceSpecID")) Then
                        If CInt(dr("AircraftTypeServiceSpecID").ToString) <> previousACType Then
                            dtac = da.GetAircraftTypeServiceSpecsByIDProd(_carrierid, CInt(dr("AircraftTypeServiceSpecID").ToString))
                            If Not isdtnullorempty(dtac) Then
                                acname = dtac.Rows(0).Item("name").ToString

                                previousACType = CInt(dr("AircraftTypeServiceSpecID").ToString)

                            Else
                                acname = dr("name").ToString.Trim
                                previousACType = 0
                            End If
                        End If
                    Else
                        acname = ""
                        previousACType = 0
                    End If
                    dtac = da.GetAircraftByRegistrationProd(_carrierid, dr("registration").ToString)
                    If Not isdtnullorempty(dtac) Then
                        AircraftID = CInt(dtac.Rows(0).Item("AircraftID").ToString)
                        If acname = "" Then acname = dtac.Rows(0).Item("name").ToString
                    Else
                        AircraftID = 0
                        '20170618 - pab - more weight classes
                        'acname = ""
                        acname = dr("aircrafttype").ToString.Trim
                    End If
                    previousAircraftID = AircraftID

                    seatsavailable = CInt(dr("seatsavailable").ToString)
                    '20161227 - pab - fix calendar - wu flight detail not numeric
                    flightdtl = dr("flightdetail").ToString.Trim
                    actype = ""
                    If Session("CalendarStyle").ToString.ToUpper = "TEXT" Then
                        If InStr(acname, " - ") > 0 Then
                            actype = "<br />" & Left(acname, InStr(acname, " - ") - 1).Trim
                        Else
                            actype = "<br />" & acname
                        End If

                        '20170618 - pab - more weight classes
                    Else
                        If Not IsDBNull(dr("weightclass")) Then
                            If dr("weightclass").ToString.Trim <> "" Then
                                acname &= " - Weight Class " & dr("weightclass").ToString.Trim
                            End If
                        End If
                    End If

                    '20170213 - pab - add code for ac type
                    Select Case dr("weightclass").ToString.Trim
                        Case "1"
                            tailcolor = "#FFFFCC"
                        Case "2"
                            tailcolor = "#FFCCCC"
                        Case "H"
                            tailcolor = "#FFCCFF"
                        Case "L"
                            tailcolor = "#CCCCFF"
                        Case "M"
                            tailcolor = "#CCFFFF"
                        Case "P"
                            tailcolor = "#CCFFCC"
                        Case "S"
                            tailcolor = "#99FFCC"
                        Case "T"
                            tailcolor = "#99CCFF"
                        Case "U"
                            tailcolor = "#FFCC99"
                        Case "V"
                            tailcolor = "#CCFF99"
                            '20170618 - pab - more weight classes
                        Case "SM"
                            tailcolor = "#CCFF66"
                        Case "TP1"
                            tailcolor = "#FFCC66"
                        Case "TT"
                            tailcolor = "#9999FF"
                        Case "HELO"
                            tailcolor = "#66CCFF"
                        Case Else
                            tailcolor = "White"
                    End Select

                    '20170721 - pab - jlx - show bkr
                    'tail = "<div title=""" & acname & """ style=""width:100%;"" onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"" onclick=""showAircraftDetailPopout(" & AircraftID & ");""><b>" & previousRegistration & "</b>" & actype & "</div>"
                    'actype = "<div title=""" & previousRegistration & """ style=""width:100%;"" onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"" onclick=""showAircraftDetailPopout(" & AircraftID & ");"">" & acname & "</div>"
                    If basecode = "BKR" Then
                        tail = "<div title=""" & acname & """ style=""width:100%;color:red;"" onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"" onclick=""showAircraftDetailPopout(" & AircraftID & ");""><b>" & previousRegistration & " - " & basecode & "</b>" & "</div>"
                    Else
                        If basecode = "" Then
                            tail = "<div title=""" & acname & """ style=""width:100%;"" onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"" onclick=""showAircraftDetailPopout(" & AircraftID & ");""><b>" & previousRegistration & "</b>" & "</div>"
                        Else
                            tail = "<div title=""" & acname & """ style=""width:100%;"" onmouseover=""this.style.cursor='pointer';"" onmouseout=""this.style.cursor='default';"" onclick=""showAircraftDetailPopout(" & AircraftID & ");""><b>" & previousRegistration & " - " & basecode & "</b>" & "</div>"
                        End If
                    End If

                    previousDepartureAirport = previousArrivalAirport
                    previousDepartureICAO = previousArrivalICAO
                    '20140911 - pab - show all aircraft
                    If Not IsDBNull(dr("DepartureTime")) Then
                        startTime = CDate(dr("DepartureTime"))
                    Else
                        startTime = startDateRange
                    End If
                    If Not IsDBNull(dr("ArrivalTime")) Then
                        endTime = CDate(dr("ArrivalTime"))
                    Else
                        endTime = endDateRange
                    End If
                    previousArrivalTime = CDate(startDateRange.ToString("d") & " 00:00")
                    timeAddedToRow = False

                    '20140818 - pab - make text display more like fos
                    'previousflightid = CInt(dr("flightid").ToString)
                    '20161227 - pab - fix calendar - wu flight detail not numeric
                    previousflightid = dr("FlightDetail").ToString.Trim
                    bFlightInfoOnly = False

                End If

                ''20170320 - pab - show ac when not scheduled
                'If DateAdd(DateInterval.Hour, offset, CDate(dr("arrivaltimegmt"))) < startDateRange Or
                '        DateAdd(DateInterval.Hour, offset, CDate(dr("departuretimegmt"))) >= endDateRange Then
                '    previousDepartureAirport = UCase(dr("DepartureAirport").ToString.Trim)
                '    previousDepartureICAO = UCase(dr("DepartureICAO").ToString.Trim)
                '    previousArrivalAirport = UCase(dr("ArrivalAirport").ToString.Trim)
                '    previousArrivalICAO = UCase(dr("ArrivalICAO").ToString.Trim)
                '    previousCssClass = GetCssClass(dr("FlightType").ToString.Trim.ToUpper, "", dr("legtypecode").ToString.Trim)
                '    previousflightid = dr("FlightDetail").ToString.Trim
                '    previousFlightType = dr("FlightType").ToString.Trim
                '    previousDepartureTime = startTime
                '    previousArrivalTime = endTime
                '    seatsavailable = CInt(dr("seatsavailable").ToString.Trim)
                '    flightdtl = dr("flightdetail").ToString.Trim
                '    Continue For
                'End If

                ''20170213 - pab - add code for ac type
                'If ddlACType.SelectedValue = "B350" Then
                '    If InStr(dr("actype").ToString.ToUpper, ddlACType.SelectedValue.ToUpper) = 0 Then
                '        Continue For
                '    End If
                'ElseIf ddlACType.SelectedValue <> "" Then
                '    If dr("actype").ToString.ToUpper <> ddlACType.SelectedValue.ToUpper Then
                '        Continue For
                '    End If
                'End If

                'no flights for aircraft - show aircraft on ground
                '20140818 - pab - make text display more like fos
                'If CInt(dr("FlightID").ToString) = 0 Then
                '20161227 - pab - fix calendar - wu flight detail not numeric
                'If CInt(dr("FlightDetail").ToString) = 0 Then
                If dr("FlightDetail").ToString.Trim = "" Then

                    '20170320 - pab - show ac when not scheduled move to after ac change check
                    If bskip = False Then
                        'add emtpy cell that used to be aircraft
                        tr = New TableRow
                        tr.CssClass = "planningCellTextFOS"

                        '20140826 - pab - 
                        If UCase(Session("CalendarStyle").ToString) = "TEXT" Then tr.Style.Add("height", "200px")

                        '20130710 - pab - FOS calendar
                        'For i = 0 To td.Length - 1
                        For i = 0 To FOSCalendarDays
                            td(i) = New TableCell
                        Next
                        td(0).Style.Add("border-right", "1px solid gray")
                        '20170213 - pab - add code for ac type
                        If tailcolor <> "" Then td(0).Style.Add("background-color", tailcolor)
                        td(0).Text = tail
                        tr.Cells.Add(td(0))
                        rownumber = rownumber + 1

                        td(0) = New TableCell

                        '20130710 - pab - FOS calendar
                        'td(0).ColumnSpan = 7
                        td(0).ColumnSpan = FOSCalendarDays
                        td(0).Style.Add("border-left", "1px solid gray")
                        td(0).Style.Add("border-right", "1px solid gray")
                        td(0).HorizontalAlign = HorizontalAlign.Center

                        If UCase(Session("CalendarStyle").ToString) <> "TEXT" Then
                            td(0).CssClass = "planningCellOn"
                            'If previousArrivalAirport <> String.Empty Then
                            If previousArrivalICAO <> String.Empty Then
                                Dim s As String = previousArrivalICAO
                                td(0).Text = "<div title=""" & s & " - " & s & """ style=""font-weight: normal; float:left;width:50%;"" class=""" & previousCssClass & """ align=""left"" >" & s & "</div>"
                                td(0).Text &= "<div title=""" & s & " - " & s & """ style=""font-weight: normal; float:left;width:50%;"" class=""" & previousCssClass & """ align=""right"" >" & s & "</div>"
                                tr.Cells.Add(td(0))
                            Else
                                td(0).Text = ""
                                tr.Cells.Add(td(0))
                            End If
                        Else
                            tr.CssClass = "planningCellTextFOS"
                            td(1).Style.Add("border-left", "1px solid gray")

                            '20130710 - pab - FOS calendar
                            'For i = 1 To td.Length - 1
                            For i = 1 To FOSCalendarDays
                                td(i) = New TableCell
                                td(i).Style.Add("border-right", "1px solid gray")
                                td(i).Text = ""
                                tr.Cells.Add(td(i))
                            Next
                        End If

                        Me.tblFlightPlanningWeekly.Rows.Add(tr)

                        tr = Nothing

                        If UCase(Session("CalendarStyle").ToString) = "TEXT" Then
                            tr = New TableRow
                            tr.Style.Add("height", "3px")
                            td(0) = New TableCell

                            '20130710 - pab - FOS calendar
                            'td(0).ColumnSpan = 8
                            td(0).ColumnSpan = FOSCalendarDays + 1
                            td(0).Text = ""
                            tr.Cells.Add(td(0))
                            rownumber = rownumber + 1
                            Me.tblFlightPlanningWeekly.Rows.Add(tr)
                            tr = Nothing
                        End If

                    End If

                Else

                    '20170320 - pab - show ac when not scheduled move to after ac change check
                    If bskip = False Then

                        If timeAddedToRow = False Then
                            'fill row with flight info
                            tr = New TableRow
                            rownumber = rownumber + 1

                            '20140826 - pab - 
                            If UCase(Session("CalendarStyle").ToString) = "TEXT" Then tr.Style.Add("height", "200px")

                            '20130710 - pab - FOS calendar
                            'For i = 1 To td.Length - 1
                            For i = 1 To FOSCalendarDays
                                td(i) = New TableCell
                            Next

                            If UCase(Session("CalendarStyle").ToString) = "TEXT" Then
                                tr.CssClass = "planningCellTextFOS"
                                td(1).Style.Add("border-left", "1px solid gray")

                                '20130710 - pab - FOS calendar
                                'For i = 1 To td.Length - 1
                                For i = 1 To FOSCalendarDays
                                    td(i).Style.Add("border-right", "1px solid gray")
                                Next
                            End If

                            previousCssClass = "planningCellOn"
                            previousFlightType = ""

                            timeAddedToRow = True

                            '20130710 - pab - FOS calendar
                            'For i = 1 To td.Length - 1
                            For i = 1 To FOSCalendarDays
                                tddetail(i) = New TableCell
                            Next

                        End If

                        startTime = CDate(dr("DepartureTime"))
                        endTime = CDate(dr("ArrivalTime"))

                        If startTime > previousArrivalTime Then

                            If UCase(Session("CalendarStyle").ToString) <> "TEXT" Then
                                'tdtext = FormatRowWeekly(startDateRange, endDateRange, 0, 0, previousRegistration, previousArrivalAirport, _
                                '          previousArrivalTime, UCase(dr("DepartureAirport").ToString), startTime, GetCssClass("", ""), previousArrivalAirport, _
                                '          passengers, seatsavailable, rownumber, "")
                                If previousArrivalICAO <> "" Then
                                    '20140416 - pab - add select time zone
                                    '20161227 - pab - fix calendar - wu flight detail not numeric
                                    '20170106 - pab - no crew flagged as m
                                    '20170721 - pab - jlx - show bkr
                                    '20170913 - pab - add cost and leg type code to mouseover
                                    tdtext = FormatRowWeekly(startDateRange, endDateRange, "0", 0, previousRegistration, previousArrivalICAO,
                                          previousArrivalTime, UCase(dr("DepartureICAO").ToString.Trim), startTime, "", previousArrivalICAO,
                                          passengers, seatsavailable, rownumber, "", FOSCalendarDays, timezone, "", basecode, 0)
                                Else
                                    '20140416 - pab - add select time zone
                                    '20161227 - pab - fix calendar - wu flight detail not numeric
                                    '20170106 - pab - no crew flagged as m
                                    '20170721 - pab - jlx - show bkr
                                    '20170913 - pab - add cost and leg type code to mouseover
                                    tdtext = FormatRowWeekly(startDateRange, endDateRange, "0", 0, previousRegistration, UCase(dr("DepartureICAO").ToString.Trim),
                                          previousArrivalTime, UCase(dr("DepartureICAO").ToString.Trim), startTime, "", UCase(dr("DepartureICAO").ToString.Trim),
                                          passengers, seatsavailable, rownumber, "", FOSCalendarDays, timezone, "", basecode, 0)
                                End If

                                '20130710 - pab - FOS calendar
                                'For i = 1 To td.Length - 1
                                For i = 1 To FOSCalendarDays
                                    td(i).Text &= tdtext(i - 1).ToString
                                Next
                            End If

                        End If

                        If UCase(Session("CalendarStyle").ToString) = "TEXT" Then
                            tr.CssClass = "planningCellTextFOS"
                            'tdtext = FormatRowWeeklyText(startDateRange, endDateRange, CInt(dr("FlightID").ToString), AircraftID, dr("Registration").ToString, _
                            '          UCase(dr("DepartureAirport").ToString), startTime, UCase(dr("ArrivalAirport").ToString), endTime, dr("FlightType").ToString, previousArrivalAirport, _
                            '          passengers, CInt(dr("seatsavailable").ToString), rownumber, flightdtl, "")
                            '20140818 - pab - make text display more like fos
                            If bFlightInfoOnly = True And previousDepartTime <> CDate("12:00:00 AM") Then
                                If DateDiff(DateInterval.Day, CDate(previousDepartTime.ToString("d")), CDate(startTime.ToString("d"))) <> 0 Then bFlightInfoOnly = False
                            End If
                            '20170106 - pab - no crew flagged as m
                            tdtext = FormatRowWeeklyText(startDateRange, endDateRange, dr("FlightDetail").ToString.Trim, AircraftID, dr("Registration").ToString,
                                  UCase(dr("DepartureICAO").ToString.Trim), startTime, UCase(dr("ArrivalICAO").ToString.Trim), endTime, dr("FlightType").ToString, previousArrivalICAO,
                                  passengers, CInt(dr("seatsavailable").ToString), rownumber, flightdtl, "", dr("RequesterName").ToString.Trim, bFlightInfoOnly, dr("legtypecode").ToString.Trim)
                            bFlightInfoOnly = True
                            previousDepartTime = startTime
                        Else
                            'tdtext = FormatRowWeekly(startDateRange, endDateRange, CInt(dr("FlightID").ToString), AircraftID, dr("Registration").ToString, _
                            '          UCase(dr("DepartureAirport").ToString), startTime, UCase(dr("ArrivalAirport").ToString), endTime, dr("FlightType").ToString, previousArrivalAirport, _
                            '          passengers, CInt(dr("seatsavailable").ToString), rownumber, "")
                            If endTime > startDateRange Then
                                '20140416 - pab - add select time zone
                                '20140818 - pab - make text display more like fos
                                '20161227 - pab - fix calendar - wu flight detail not numeric
                                '20170106 - pab - no crew flagged as m
                                '20170721 - pab - jlx - show bkr
                                '20170913 - pab - add cost and leg type code to mouseover
                                tdtext = FormatRowWeekly(startDateRange, endDateRange, dr("FlightDetail").ToString.Trim, AircraftID, dr("Registration").ToString,
                                    UCase(dr("DepartureICAO").ToString.Trim), startTime, UCase(dr("ArrivalICAO").ToString.Trim), endTime, dr("FlightType").ToString,
                                    previousArrivalICAO, passengers, CInt(dr("seatsavailable").ToString), rownumber, "", FOSCalendarDays, timezone,
                                    dr("legtypecode").ToString.Trim, dr("basecode").ToString.Trim, CInt(dr("cost")))
                            End If
                        End If

                        '20130710 - pab - FOS calendar
                        'For i = 1 To td.Length - 1
                        For i = 1 To FOSCalendarDays
                            '20161229 - pab - fix calendar - check if data in tdtext
                            If tdtext.Count > 0 Then td(i).Text &= tdtext(i - 1).ToString
                        Next

                        ''tdtext = FormatDetailWeekly(startDateRange, endDateRange, CInt(dr("FlightID").ToString), AircraftID, _
                        ''        dr("Registration").ToString, UCase(dr("DepartureAirport").ToString), startTime, UCase(dr("ArrivalAirport").ToString), _
                        ''        endTime, seatsavailable, rownumber, CInt(dr("flightdetail").ToString), dr("FlightType").ToString)
                        'tdtext = FormatDetailWeekly(startDateRange, endDateRange, CInt(dr("FlightID").ToString), AircraftID, _
                        '        dr("Registration").ToString, UCase(dr("DepartureICAO").ToString.Trim), startTime, UCase(dr("ArrivalICAO").ToString.Trim), _
                        '        endTime, seatsavailable, rownumber, CInt(dr("flightdetail").ToString), dr("FlightType").ToString)
                        'For i = 1 To td.Length - 1
                        '    If tddetail(i).Text <> "" And tdtext(i - 1).ToString <> "" Then
                        '        tddetail(i).Text &= "--------------------<br />"
                        '    End If
                        '    tddetail(i).Text &= tdtext(i - 1).ToString
                        'Next

                    End If

                End If      'If CInt(dr("FlightID").ToString) = 0 Then

                'for next loop thru
                previousDepartureAirport = UCase(dr("DepartureAirport").ToString.Trim)
                previousDepartureICAO = UCase(dr("DepartureICAO").ToString.Trim)
                previousArrivalAirport = UCase(dr("ArrivalAirport").ToString.Trim)
                previousArrivalICAO = UCase(dr("ArrivalICAO").ToString.Trim)
                '20170106 - pab - no crew flagged as m
                previousCssClass = GetCssClass(dr("FlightType").ToString.Trim.ToUpper, "", dr("legtypecode").ToString.Trim)
                '20140818 - pab - make text display more like fos
                'previousflightid = CInt(dr("flightid").ToString.Trim)
                '20161227 - pab - fix calendar - wu flight detail not numeric
                previousflightid = dr("FlightDetail").ToString.Trim
                previousFlightType = dr("FlightType").ToString.Trim

                previousDepartureTime = startTime
                previousArrivalTime = endTime

                seatsavailable = CInt(dr("seatsavailable").ToString.Trim)
                '20161227 - pab - fix calendar - wu flight detail not numeric
                flightdtl = dr("flightdetail").ToString.Trim

                '20140911 - pab - show all aircraft
                'End If      'If dr("DepartureAirport").ToString = "" And dr("ArrivalAirport").ToString = "" And CInt(dr("Distance").ToString) = 0 Then

            Next

            If Not tr Is Nothing Then
                'look to pad last row thru midnight
                If previousArrivalTime >= CDate(endDateRange.ToString("d") & " 11:59PM") Then
                    'already formatted - do nothing
                Else
                    If previousArrivalTime.ToShortTimeString = "11:59 PM" Then previousArrivalTime = DateAdd(DateInterval.Minute, 1, previousArrivalTime)
                    If UCase(Session("CalendarStyle").ToString) <> "TEXT" Then
                        'tdtext = FormatRowWeekly(startDateRange, endDateRange, 0, 0, "", previousArrivalAirport, previousArrivalTime, previousArrivalAirport, _
                        '          CDate(endDateRange.ToString("d") & " 11:59PM"), GetCssClass("", ""), previousArrivalAirport, passengers, seatsavailable, _
                        '          rownumber, "")
                        '20140416 - pab - add select time zone
                        '20161227 - pab - fix calendar - wu flight detail not numeric
                        '20170106 - pab - no crew flagged as m
                        '20170721 - pab - jlx - show bkr
                        '20170913 - pab - add cost and leg type code to mouseover
                        tdtext = FormatRowWeekly(startDateRange, endDateRange, "0", 0, "", previousArrivalICAO, previousArrivalTime, previousArrivalICAO,
                                  CDate(endDateRange.ToString("d") & " 11:59PM"), "", previousArrivalICAO, passengers, seatsavailable,
                                  rownumber, "", FOSCalendarDays, timezone, "", basecode, 0)

                        '20130710 - pab - FOS calendar
                        'For i = 1 To td.Length - 1
                        For i = 1 To FOSCalendarDays
                            td(i).Text &= tdtext(i - 1).ToString
                        Next
                    End If

                End If

                tr.CssClass = ""
                td(0) = New TableCell
                td(0).CssClass = "planningCellTextFOS"
                td(0).Style.Add("border-right", "1px solid gray")
                '20170213 - pab - add code for ac type
                if tailcolor <> "" Then td(0).Style.Add("background-color", tailcolor)
                td(0).Text = tail
                '20140818 - pab - make text display more like fos
                If UCase(Session("CalendarStyle").ToString) = "TEXT" Then td(0).VerticalAlign = VerticalAlign.Top
                tr.Cells.Add(td(0))

                If UCase(Session("CalendarStyle").ToString) = "TEXT" Then
                    tr.CssClass = "planningCellTextFOS"
                    td(1).Style.Add("border-left", "1px solid gray")

                    '20130710 - pab - FOS calendar
                    'For i = 1 To td.Length - 1
                    For i = 1 To FOSCalendarDays
                        td(i).Style.Add("border-right", "1px solid gray")
                        '20140818 - pab - make text display more like fos
                        td(i).VerticalAlign = VerticalAlign.Top
                    Next
                End If

                '20130710 - pab - FOS calendar
                'For i = 1 To td.Length - 1
                For i = 1 To FOSCalendarDays
                    Dim div As String = td(i).Text.ToString
                    td(i).Text = checkdivwidth(div)
                    tr.Cells.Add(td(i))
                Next

                Me.tblFlightPlanningWeekly.Rows.Add(tr)

                tr = New TableRow
                tr.CssClass = "planningCellTextFOS"

                '20140826 - pab - 
                If UCase(Session("CalendarStyle").ToString) = "TEXT" Then tr.Style.Add("height", "200px")

                rownumber = rownumber + 1
                tr.ID = "trw" & rownumber
                If _CalendarDetailVisible = True Then
                    tr.Style.Add("visibility", "visible")
                Else
                    tr.Style.Add("visibility", "collapse")
                End If
                tr.Style.Add("position", "relative")

                td(0) = New TableCell
                td(0).Style.Add("border-right", "1px solid gray")
                td(0).Text = actype
                tr.Cells.Add(td(0))

                tddetail(1).Style.Add("border-left", "1px solid gray")

                '20130710 - pab - FOS calendar
                'For i = 1 To td.Length - 1
                For i = 1 To FOSCalendarDays
                    tddetail(i).Style.Add("border-right", "1px solid gray")
                    tddetail(i).Style.Add("font-size", "xx-small")
                    'tddetail(i).Text = "<div onclick=""hideOrShow(trw" & rownumber & ");"" align=""left"">" & tddetail(i).Text & "</div>"
                    tddetail(i).Text = "<div align=""left"">" & tddetail(i).Text & "</div>"
                    tr.Cells.Add(tddetail(i))
                Next

                Me.tblFlightPlanningWeekly.Rows.Add(tr)
                tr = Nothing

            End If

            'If _CalendarCombinedView = True Then
            '    AirTaxi.post_timing("AddFleetPlanningRowsWeekly pilots  " & Now.ToString)
            '    tr = New TableRow
            '    tr.Style.Add("height", "10px")
            '    td(0) = New TableCell
            '    td(0).Style.Add("background", "black")
            '    td(0).ColumnSpan = 8
            '    td(0).Text = ""
            '    tr.Cells.Add(td(0))
            '    rownumber = rownumber + 1
            '    Me.tblFlightPlanningWeekly.Rows.Add(tr)
            '    tr = Nothing

            '    tr = New TableRow
            '    '20110214 - pab - more calendar changes
            '    'tr.CssClass = "planningCellAsset"
            '    tr.CssClass = "planningCellText"
            '    For i = 0 To td.Length - 1
            '        td(i) = New TableCell
            '        td(i).Style.Add("border-right", "1px solid gray")
            '    Next
            '    td(0).Style.Add("border-left", "1px solid gray")
            '    td(0).Text = ""
            '    td(1).Text = "Sunday"
            '    td(2).Text = "Monday"
            '    td(3).Text = "Tuesday"
            '    td(4).Text = "Wednesday"
            '    td(5).Text = "Thursday"
            '    td(6).Text = "Friday"
            '    td(7).Text = "Saturday"
            '    For i = 0 To td.Length - 1
            '        tr.Cells.Add(td(i))
            '    Next
            '    rownumber = rownumber + 1
            '    Me.tblFlightPlanningWeekly.Rows.Add(tr)
            '    tr = Nothing

            '    Dim ds As New DataSet
            '    ds = da.GetPilotScheduleDetail(_carrierid, startDateRange, CDate(endDateRange.ToString("d") & " 23:59"))
            '    Dim schedule As ArrayList

            '    rownumber = rownumber + 1
            '    schedule = AddPilotPlanningRowsWeekly(startDateRange, endDateRange, ds.Tables(0), rownumber)
            '    Dim n As Integer = schedule.Count - 1

            '    Dim rows(n) As TableRow

            '    schedule.CopyTo(rows)
            '    For i = 0 To n
            '        tr = rows(i)
            '        Me.tblFlightPlanningWeekly.Rows.Add(tr)
            '    Next

            'End If

            '20170621 - pab - add column headers as footers
            AddHeaders(startDateRange, timezone, FOSCalendarDays)

exitsub:

            AirTaxi.post_timing("AddFleetPlanningRowsWeekly end  " & Now.ToString)

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(0, appName, Left(s, 500), "AddFleetPlanningRowsWeekly", "FOSFlightsCalendar.ascx.vb")
            'SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " FOSFlightsCalendar.ascx.vb AddFleetPlanningRowsWeekly Error", s, 0)
        End Try

    End Sub

    '20170312 - pab - use calendar from and to
    'Protected Sub ddlView_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlView.SelectedIndexChanged

    '    If ddlView.SelectedValue = "24 Hours" Then
    '        Me.lbPreviousDay.Visible = True
    '        Me.lbNextDay.Visible = True
    '    Else
    '        Me.lbPreviousDay.Visible = False
    '        Me.lbNextDay.Visible = False
    '    End If
    '    Session("fosmodelstartfos") = Session("fosmodelstart")
    '    BindData(modelrunid)

    'End Sub

    '20170312 - pab - use calendar from and to
    'Protected Sub lbPreviousDay_Click(sender As Object, e As EventArgs) Handles lbPreviousDay.Click

    '    Session("fosmodelstartfos") = DateAdd(DateInterval.Day, -1, CDate(Session("fosmodelstartfos").ToString))
    '    BindData(modelrunid)

    'End Sub

    '20170312 - pab - use calendar from and to
    'Protected Sub lbNextDay_Click(sender As Object, e As EventArgs) Handles lbNextDay.Click

    '    Session("fosmodelstartfos") = DateAdd(DateInterval.Day, 1, CDate(Session("fosmodelstartfos").ToString))
    '    BindData(modelrunid)

    'End Sub

    '20140416 - pab - add select time zone
    '20171027 - pab - calendar
    'Private Sub ddlTimeZone_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlTimeZone.SelectedIndexChanged

    '    Session("fosmodelstartfos") = Session("fosmodelstart")
    '    BindData(modelrunid)

    'End Sub

    '20171027 - pab - calendar
    Private Sub rcTimeZone_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rcTimeZone.SelectedIndexChanged

        Session("fosmodelstartfos") = Session("fosmodelstart")
        BindData(modelrunid)

    End Sub

    '20171027 - pab - calendar
    'Protected Sub ddlStyle_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlStyle.SelectedIndexChanged

    '    Session("CalendarStyle") = ddlStyle.SelectedValue
    '    BindData(modelrunid)

    'End Sub

    '20171027 - pab - calendar
    Protected Sub rcStyle_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rcStyle.SelectedIndexChanged

        Session("CalendarStyle") = rcStyle.SelectedValue
        BindData(modelrunid)

    End Sub


    '20170213 - pab - add code for ac type
    '20171027 - pab - calendar
    'Private Sub ddlACType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlACType.SelectedIndexChanged

    '    '20170221 - pab - fix page going blank
    '    'If ddlStyle.SelectedIndex < 0 Then
    '    '    Session("CalendarStyle") = "Graphic"
    '    'Else
    '    '    Session("CalendarStyle") = ddlStyle.SelectedValue
    '    'End If
    '    BindData(modelrunid)

    'End Sub

    '20171027 - pab - calendar
    Private Sub rcACType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rcACType.SelectedIndexChanged

        '20170221 - pab - fix page going blank
        If rcStyle.SelectedIndex < 0 Then
            Session("CalendarStyle") = "Graphic"
        Else
            Session("CalendarStyle") = rcStyle.SelectedValue
        End If
        BindData(modelrunid)

    End Sub

    '20170213 - pab - add code for ac type
    Private Sub FOSFlightsCalendar_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        If Session("CalendarStyle").ToString.ToUpper = "TEXT" Then
            legendTop.Visible = False
            legendBottom.Visible = False
            legendTopText.Visible = True
            legendBottomText.Visible = True
            '20170618 - pab - more weight classes
            legendTopWC.Visible = False
            legendBottomWC.Visible = False
        Else
            legendTop.Visible = True
            legendBottom.Visible = True
            legendTopText.Visible = False
            legendBottomText.Visible = False
            '20170618 - pab - more weight classes
            legendTopWC.Visible = True
            legendBottomWC.Visible = True
        End If

    End Sub

    '20170312 - pab - use calendar from and to
    Protected Sub bttnGo_Click(sender As Object, e As EventArgs) Handles bttnGo.Click

        If IsNothing(from_date.SelectedDate) Then
            Session("fosmodelstartfos") = Now
        Else
            Session("fosmodelstartfos") = from_date.SelectedDate
        End If
        BindData(modelrunid)

    End Sub

    ''20140818 - pab - make text display more like fos
    'Protected Sub ddlStyle_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlStyle.SelectedIndexChanged

    '    _CalendarStyle = ddlStyle.SelectedValue
    '    If _CalendarStyle.ToUpper = "TEXT" Then
    '        legendTop.Visible = False
    '        legendBottom.Visible = False
    '    Else
    '        legendTop.Visible = True
    '        legendBottom.Visible = True
    '    End If
    '    _fosmodelstartfos = _fosmodelstart
    '    BindData(modelrunid)

    'End Sub

End Class