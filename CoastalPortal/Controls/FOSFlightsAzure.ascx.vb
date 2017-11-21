Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports CoastalPortal.Models
Imports CoastalPortal.AirTaxi
Imports CoastalPortal.DataAccess
Imports System.Data
Imports MoreLinq

Public Class FOSFlightsAzureabc123
    Inherits System.Web.UI.UserControl
    Private modelrunid As String
    Private SqlDataSourceTrips As New SqlDataSource

    Public Const FOS_FROM As Integer = 0
    Public Const FOS_TO As Integer = 1
    Public Const FOS_FROMGMT As Integer = 2
    Public Const FOS_TOGMT As Integer = 3
    Public Const FOS_FROMLOCAL As Integer = 4
    Public Const FOS_TOLOCAL As Integer = 5
    Public Const FOS_MIN As Integer = 6
    Public Const FOS_NM As Integer = 7
    Public Const FOS_AC As Integer = 8
    Public Const FOS_TYPE As Integer = 9
    Public Const FOS_COST As Integer = 10
    Public Const FOS_FT As Integer = 11
    Public Const FOS_TRIPNUM As Integer = 12
    Public Const FOS_PIN As Integer = 13
    Public Const FOS_AOG As Integer = 14
    Public Const FOS_LRC As Integer = 15
    Public Const FOS_LPC As Integer = 16
    Public Const FOS_LTC As Integer = 17
    Public Const FOS_PIC As Integer = 18
    Public Const FOS_SIC As Integer = 19
    Public Const FOS_REVENUE As Integer = 20
    Public Const FOS_PANDL As Integer = 21
    Public Const FOS_BASE As Integer = 22
    Public Const FOS_WC As Integer = 23
    Public Const FOS_HA As Integer = 24
    Public Const FOS_OE As Integer = 25

    Private FOS_CarrierProfile As New CarrierProfile
    Private db As New OptimizerContext
    Public Shared specsfromlog As Boolean = True

    '20120724 - fix connection strings
    Public Shared cnmkazure As New ADODB.Connection

    'rk 7.30.2012 point to production
    Public D2DConnectionString As String = ConnectionStringHelper.GetConnectionStringSGServer

    Public ConnectionStringSQL As String = ConnectionStringHelper.GetD2DConnectionString

    Public ConnectionStringOptimizerSQL As String = ConnectionStringHelper.GetConnectionStringSQLMKAzure





    Protected Sub ImgTrip_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)


        '        If Me.PanelLookupTrips.Visible = False Then


        '   Me.PanelLookupTrips.Visible = True

        Me.GridViewTrips.Visible = True
        ' Me.GridViewCustomers.Visible = False
        '        Me.GridViewRequestors.Visible = False

        Dim s As String

        s = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
        s &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
        s &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
        s &= "seatsavail as [Seats] FROM Itinerary where ItinFlightRequest > 1 and CarrierID = " & Session("carrierid") & " order by id desc "



        Session("link") = "Trip"

        '20120724 - fix connection strings
        'SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetD2DConnectionString
        SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString

        SqlDataSourceTrips.SelectCommand = Session("sqlQueryTrips").ToString

        'If Me.txtTripDescription.Text <> "" Then
        '    SqlDataSourceTrips.SelectCommand = "select * from itinerary where ItinTitle Like '%" & Me.txtTripDescription.Text & "%' order by id desc"
        '    Session("sqlQueryTrips") = "select * from itinerary where ItinTitle Like '%" & Me.txtTripDescription.Text & "%' order by id desc"
        'End If


        'SqlDataSourceTrips.DataBind()
        'GridViewTrips.DataBind()
        BindDataTrips(s)


        ' Me.hddnSearchMembers.Value = "1"
        GridViewTrips.Visible = True





    End Sub

    Private Sub BindDataTrips(ByVal s As String)

        Dim GridViewSource As New List(Of PanelDisplay)
        Dim FosList As New List(Of FOSFlightsOptimizerRecord)
        Dim CasList As New List(Of CASFlightsOptimizerRecord)
        Dim Panellist As New List(Of PanelRecord)
        Dim PanellistRight As New List(Of PanelRecord)
        Dim mrcustom As String = normalizemodelrunid(modelrunid)
        Dim BaseList As New List(Of String)
        Dim ACList As New List(Of String)
        Dim HAList As New List(Of String)

        FosList = db.FOSFlightsOptimizer.Where(Function(x) x.OptimizerRun = mrcustom).ToList()
        CasList = db.CASFlightsOptimizer.Where(Function(x) x.OptimizerRun = modelrunid).ToList()
        Session("FOS") = FosList
        Session("CAS") = CasList

        Panellist = (From f In FosList Group Join c In CasList On f.AC Equals c.AircraftRegistration And
                                             f.DateTimeGMT Equals c.DepartureTime Into Plist = Group From p In Plist.DefaultIfEmpty()
                     Select New PanelRecord With {.CASRecord = p, .FOSRecord = f}).ToList()

        PanellistRight = (From c In CasList Group Join f In FosList On f.AC Equals c.AircraftRegistration And
        f.DateTimeGMT Equals c.DepartureTime Into Plist = Group From p In Plist.DefaultIfEmpty() Select New PanelRecord With {.FOSRecord = p, .CASRecord = c}).ToList()

        ' Panellist.Union(PanellistRight).ToList()
        For Each i As PanelRecord In PanellistRight
            If i.FOSRecord Is Nothing Then
                Panellist.Add(i)
            End If
        Next

        ACList = (From a In Panellist Select a.ACType).Distinct().ToList()
        GridViewSource = (From x In ACList Select New PanelDisplay With {.AircraftType = x, .PanelRecord = (From y In Panellist Where y.ACType = x Select y).ToList()}).ToList()
        For Each x As PanelDisplay In GridViewSource
            AirTaxi.awclookup.TryGetValue(Trim(x.AircraftType), x.WeightClass)
        Next
        If InStr(UCase(s), "FROM") = 0 Then
            Exit Sub
        End If


        Dim sql As String = s

        Dim dateFrom As DateTime = Nothing
        Dim dateThru As DateTime = Nothing


        GridViewTrips.DataSource = SqlDataSourceTrips
        SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString

        '20101101 - pab
        GridViewTrips.EmptyDataText = "Your search returned 0 results"
        Dim req As String
        req = sql
        If sql <> "" Then


            SqlDataSourceTrips.Dispose()

            SqlDataSourceTrips.SelectCommand = sql

            SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString

            GridViewTrips.AutoGenerateColumns = False

            GridViewTrips.DataSource = SqlDataSourceTrips
            SqlDataSourceTrips.DataBind()

            GridViewTrips.DataBind()
        Else

            SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString


            SqlDataSourceTrips.DataBind()
            GridViewTrips.DataBind()
        End If
    End Sub

    Public Sub AddColumnNoColor(ByVal columnsCollection As DataControlFieldCollection, ByVal dataField As String, ByVal fieldWidth As Integer, ByVal columnWidthInPixels As Integer, Optional ByVal isReadOnly As Boolean = False, Optional ByVal isVisible As Boolean = True, Optional ByVal aFormatString As String = "")
        Dim bc As New CommandField
        With bc
            .ShowHeader = True
            .HeaderText = "Pin"
            .ItemStyle.Width = fieldWidth
            .ItemStyle.Wrap = True
            .Visible = isVisible
            .ItemStyle.BorderColor = Drawing.Color.Black
            .ItemStyle.BorderStyle = BorderStyle.Solid
            .ItemStyle.BorderWidth = 1
            .SelectText = "Pin"

            If Not (columnWidthInPixels = 0) Then .ControlStyle.Width = Unit.Pixel(columnWidthInPixels)
        End With
        columnsCollection.Add(bc)
    End Sub

    Private Sub GridViewTrips_DataBound(sender As Object, e As EventArgs) Handles GridViewTrips.DataBound
        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Start ColorME " & Now & " - ", "", "")
        colorme()
        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx End ColorME " & Now & " -k - ", "", "")

        If chkGMT.Checked = True Then
            GridViewTrips.Columns(2).Visible = True
            GridViewTrips.Columns(3).Visible = True
        Else
            GridViewTrips.Columns(2).Visible = False
            GridViewTrips.Columns(3).Visible = False
        End If


        If chkLocal.Checked = True Then
            GridViewTrips.Columns(4).Visible = True
            GridViewTrips.Columns(5).Visible = True
        Else
            GridViewTrips.Columns(4).Visible = False
            GridViewTrips.Columns(5).Visible = False
        End If



    End Sub




    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        Session("carrierid") = Session("carrierid")

        SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString

        If overridemodel <> "" Then modelrunid = overridemodel


        Try


            modelrunid = Trim(modelrunid)



            If modelrunid = "" Then
                If Not Request.QueryString("modelrunid") Is Nothing Then
                    modelrunid = Request.QueryString("modelrunid")
                End If
            End If


            '    If modelrunid = "" Then Exit Sub



done:




            lblcarrier.Text = UCase(Request.Url.Host)
            If Session("carrierid") = 49 Then lblcarrier.Text = "XOJET"
            If Session("carrierid") = 65 Then lblcarrier.Text = "TMC"
            If Session("carrierid") = 100 Then lblcarrier.Text = "Wheels UP"
            If Session("carrierid") = 104 Then lblcarrier.Text = "Jetlinx"

            If Session("defaultemail") = "demo@cas.com" Then lblcarrier.Text = "Operator"



            Dim n As Date = Now


            If Not Page.IsPostBack Then

                Dim MyCarrier As Integer = Session("carrierid")

                FOS_CarrierProfile = db.CarrierProfiles.Find(MyCarrier)
                ' If Not Page.IsPostBack Then
                '   Me.txtAirportCode.Text = txtairportcode.text

                Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Start Find Trip " & Now & "  ", "", "")
                n = Now
                cmdFindTrip_Click(Nothing, Nothing)
                Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx End Find Trip " & DateDiff(DateInterval.Second, n, Now) & "  ", "", "")


                If modelrunid = "" Then Exit Sub





                Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Start Load Drop Downs " & Now & "  ", "", "")
                n = Now
                loaddropdowns()
                Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx End Load Drop Downs" & DateDiff(DateInterval.Second, n, Now) & "  ", "", "")


                Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Start Pull From FOSLOG " & Now & "  ", "", "")
                n = Now
                pullfromfoslog(Trim(modelrunid))
                Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx End Pull From FOSLOG " & DateDiff(DateInterval.Second, n, Now) & "  ", "", "")



                'Insertsys_log(session("carrierid"), appName, "FOSFlightsAzure.ascx Start Color Me " & Now & "  ", "", "")
                'n = Now
                'colorme()
                'Insertsys_log(session("carrierid"), appName, "FOSFlightsAzure.ascx End Color Me " & DateDiff(DateInterval.Second, n, Now) & "  ", "", "")




            End If


            '  End If






            Me.LinkPending.ForeColor = Drawing.Color.Wheat
            ' Me.LinkBooked.ForeColor = Drawing.Color.Green
            '   Me.LinkQuoted.ForeColor = Drawing.Color.DarkSeaGreen
            '  Me.LinkFlown.ForeColor = Drawing.Color.Teal
            '  Me.LinkInProgress.ForeColor = Drawing.Color.IndianRed
            '  Me.LinkDispatched.ForeColor = Drawing.Color.GreenYellow





        Catch ex As Exception

            Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Load Failure " & ex.Message & "  ", "", "")

        End Try



    End Sub


    Function normalizemodelrunid(m As String) As String

        'replace the 'R" in the model run id back to role zero.

        Dim ib As String = InBetween(1, m, "-R", "-")
        If ib <> "0" Then
            m = Replace(m, "-R" & ib & "-", "-R0-")
        End If



        If Right(m, 1) <> "C" Then
            If m <> "" Then

                If Mid(m, Len(m) - 1, 1) = "-" Then
                    m = Left(m, Len(m) - 2)
                    GoTo done
                End If



                If Mid(m, Len(m) - 2, 1) = "-" Then
                    m = Left(m, Len(m) - 3)
                    GoTo done
                End If


                If Mid(m, Len(m) - 3, 1) = "-" Then
                    m = Left(m, Len(m) - 4)
                    GoTo done
                End If


            End If
        End If


done:

        Return m

    End Function

    Public Sub grabdata()

        Dim rs As New ADODB.Recordset
        Dim req As String


        Dim user As String = ""
        Dim dbname As String = ""
        Dim flightid As String = ""
        Dim pilot As String = ""

        If Not Request.QueryString("flightid") Is Nothing Then
            flightid = CStr(Request.QueryString("flightid"))
        End If

        If Not (IsNothing(Session("flightid"))) Then
            flightid = Session("flightid")
        End If

        'If flightid = "" Then
        '    Me.lblMessage.Text = "Unauthorized Flight"
        '    Exit Sub
        'End If

        'connectstring = Replace(connectstring, "dbname", dbname)


        Exit Sub


        req = "SELECT * "
        req = req & "FROM flights WHERE id  = '" & flightid & "'"
        req &= " and carrierid = " & Session("carrierid")


        If cnsetting.State = 0 Then
            cnsetting.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            cnsetting.Open()
        End If



        '        rs.Open(req)
        '20100222 - pab - use global shared connection
        'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


        If Not rs.EOF Then

            'Me.txtFromApt.Text = rs.Fields("DepartureAirport").Value
            'Me.txtToApt.Text = rs.Fields("ArrivalAirport").Value
            'Me.txtDepartureTime.Text = rs.Fields("departuretime").Value
            'Me.txtArrivalTime.Text = rs.Fields("arrivaltime").Value
            ''  Me.txtPilot.Text = pilot
            'Me.drpaircraft.SelectedValue = CInt(rs.Fields("AircraftID").Value)
            'Me.txtFlightId.Text = flightid

            'Me.txtFlightDetail.Text = rs.Fields("FlightDetail").Value



        End If


        If rs.State = 1 Then rs.Close()

        'me.txtCustomer.Text = rs.Fields ("



        If rs.State = 1 Then rs.Close()
        Dim _flightID As Integer




    End Sub

    Function pullfromfoslog(modelrunid As String)




        Try

            Dim cnmkazurelocal As New ADODB.Connection

            Dim rs As New ADODB.Recordset
            Dim req As String

            '20120724 - fix connection strings
            'If cnsetting.State = 0 Then
            '    'cnsetting.ConnectionString = ConnectionStringHelper.GetConnectionStringSQL
            '    cnsetting.ConnectionString = ConnectionStringSQL
            '    cnsetting.Open()
            'End If
            If cnmkazurelocal.State = 0 Then
                cnmkazurelocal.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
                cnmkazurelocal.ConnectionTimeout = 30
                cnmkazurelocal.CommandTimeout = 30
                cnmkazurelocal.Open()
            End If


            If rs.State = 1 Then rs.Close()



            Dim fosmodelrunid As String = modelrunid
            Dim ib As String = InBetween(1, modelrunid, "-R", "-")
            If ib <> "0" Then
                fosmodelrunid = Replace(modelrunid, "-R" & ib & "-", "-R0-")
            End If

            Dim dashline As Integer
            If Right(fosmodelrunid, 1) <> "C" Then
                For i = Len(fosmodelrunid) To 1 Step -1
                    If Mid(fosmodelrunid, i, 1) = "-" Then
                        dashline = i
                        Exit For
                    End If
                Next
                fosmodelrunid = Left(fosmodelrunid, dashline - 1)

            End If


            If overridemodel <> "" Then fosmodelrunid = overridemodel


            If AC1 = "" And ACX(1) = "" Then 'Fos Flight


                req = "select * FROM optimizerlog  where modelrunid = 'abc'"
                req = Replace(req, "abc", fosmodelrunid)



                ' Dim rs As New ADODB.Recordset
                If rs.State = 1 Then rs.Close()
                rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

                specsfromlog = True
                If rs.EOF Then
                    specsfromlog = False
                    ' grabcounts()
                    Exit Function

                End If



                If Not rs.EOF Then


                    Me.lblrevenuemiles.Text = FormatNumber(rs.Fields("FOSrevenuemiles").Value, 0)
                    Me.lblnonrevenuemiles.Text = FormatNumber(rs.Fields("FOSnonrevenuemiles").Value, 0)
                    Me.lblnonrevenueexpense.Text = "$" & FormatNumber(rs.Fields("FOSnonrevenueexpense").Value, 0)
                    Me.lbltotalrevenueexpense.Text = "$" & FormatNumber(rs.Fields("FOStotalrevenueexpense").Value, 0)
                    Me.lblrevenueexpense.Text = "$" & FormatNumber(rs.Fields("FOSrevenueexpense").Value, 0)
                    Me.lblrevenuelegs.Text = rs.Fields("FOSrevenuelegs").Value
                    Me.lblnonrevenuelegs.Text = rs.Fields("FOSnonrevenuelegs").Value
                    Me.lblLongestEmptyLeg.Text = rs.Fields("FOSLongestEmptyLeg").Value
                    Me.lblShortestEmtpyLeg.Text = rs.Fields("FOSShortestEmtpyLeg").Value
                    Me.lblAverageEmtpyNM.Text = rs.Fields("FOSAverageEmtpyNM").Value

                    'rs.Fields("FOSlinebreaks").Value = FOSlinebreaks
                    Me.lblefficiency.Text = FormatPercent(rs.Fields("FOSefficiency").Value)



                    Me.lblLineBreaks.Text = rs.Fields("FOSLineBreaks").Value

                    If IsNumeric(rs.Fields("FOSLineBreaks").Value) Then
                        If CInt(rs.Fields("FOSLineBreaks").Value) <> 0 Then
                            Me.lblLineBreaks.BackColor = Drawing.Color.Red

                            If Not (IsDBNull(rs.Fields("comments").Value)) Then
                                Me.lblLineBreaks.ToolTip = rs.Fields("comments").Value
                            End If


                        End If
                    End If




                End If

                If rs.State = 1 Then rs.Close()



            Else

                Dim ss As String = Session("FCList")
                Dim mrcustom As String

                If overridemodel = "" Then

                    mrcustom = normalizemodelrunid(modelrunid)

                Else
                    mrcustom = overridemodel

                End If

                If overridemodel <> "" And (AC1 <> "" Or ACX(1) <> "") Then 'Flight Changes
                    mrcustom = normalizemodelrunid(modelrunid)
                End If

                If AC1 <> "" Then
                    req = "SELECT sum(CONVERT(integer, nauticalmiles)) as nauticalmiles   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and  DeadHead = 'True' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5' and (ac = 'def' or ac = 'ghi')"
                Else
                    req = "SELECT sum(CONVERT(integer, nauticalmiles)) as nauticalmiles   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and  DeadHead = 'True' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5' and ac in (aclist)"
                End If
                req = Replace(req, "abc", mrcustom)
                req = Replace(req, "def", AC1)
                req = Replace(req, "ghi", AC2)
                req = Replace(req, "aclist", ss)

                rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
                If Not rs.EOF Then
                    If Not (IsDBNull(rs.Fields("nauticalmiles").Value)) Then
                        Me.lblnonrevenuemiles.Text = FormatNumber(rs.Fields("nauticalmiles").Value, 0)
                    Else
                        Exit Function
                        End If
                    End If

                    If rs.State = 1 Then rs.Close()

                If AC1 <> "" Then
                    req = "SELECT sum(CONVERT(integer, nauticalmiles)) as nauticalmiles   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'False' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5' and (ac = 'def' or ac = 'ghi')"
                Else
                    req = "SELECT sum(CONVERT(integer, nauticalmiles)) as nauticalmiles   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'False' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5' and ac in (aclist)"
                End If
                req = Replace(req, "abc", mrcustom)
                req = Replace(req, "def", AC1)
                req = Replace(req, "ghi", AC2)
                req = Replace(req, "aclist", ss)

                rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
                If Not rs.EOF Then
                    Me.lblrevenuemiles.Text = FormatNumber(rs.Fields("nauticalmiles").Value, 0)
                End If
                If rs.State = 1 Then rs.Close()

                End If



                If cnmkazurelocal.State = 1 Then cnmkazurelocal.Close()


        Catch ex As Exception
            Dim i As Integer
            i = i
            Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx pullfromfoslog Error " & Now & " - " & ex.Message, "", "")

        End Try



    End Function

    Public Sub loaddropdowns()
        Dim list(500) As String
        Dim count As Integer
        Dim a As String
        Dim found As Boolean = False


        '   If Me.GridViewTrips.Rows.Count = 0 Then Exit Sub

        If DropDownTripNumbers.Items.Count = 0 Then


            '  Insertsys_log(session("carrierid"), appName, "CASFlightsAzure.ascx Start Load Drop Downs " & Now & " -1348 - ", "", "")


            '          = "SELECT       [DepartureAirport] as [FROM]    ,[ArrivalAirport] as [TO]       , departuretime as [From GMT] , arrivaltime as [To GMT]" & _
            '      ", CAST(DepartureDateTimeLocal as datetime) as [From Local] " & _
            '       ", CAST(ArrivalDateTimeLocal as datetime) as [To Local] " & _
            '      "   ,[duration] as [Minutes]  , distance as [NM]    ,ltrim(rtrim(AircraftRegistration)) as [ac], ltrim(rtrim(aircrafttype)) as type, cast(cost as integer) as cost, [Flighttype] as FT, ltrim(rtrim(tripnumber)) as tripnumber, case when priortail is null then '' else priortail end as priortail " & _
            '"       FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and carrierid = 'ghi' order by AircraftRegistration, departuretime asc  "




            Dim dv As System.Data.DataView = CType(Me.SqlDataSourceTrips.Select(DataSourceSelectArguments.Empty), System.Data.DataView)

            '  Dim dr As System.Data.DataRow = dv.Table.Rows(1



            count = 0

            For i = 1 To 500
                list(i) = ""
            Next




            For i = 0 To dv.Table.Rows.Count - 1
                a = dv.Table.Rows(i).Item("tripnumber").ToString
                found = False
                For z = 1 To count
                    If list(z) = a Then
                        found = True
                        Exit For
                    End If
                Next
                If found = False Then
                    If count < 500 Then
                        count = count + 1
                        list(count) = a
                    End If
                End If

            Next i


            For i = 1 To count
                For z = i To count
                    If list(i) > list(z) Then
                        a = list(i)
                        list(i) = list(z)
                        list(z) = a

                    End If

                Next z
            Next i

            DropDownTripNumbers.Items.Clear()
            DropDownTripNumbers.Items.Add("----")
            For i = 1 To count
                DropDownTripNumbers.Items.Add((Trim(list(i))))
            Next i





            'req = "SELECT distinct(TripNumber)  FROM CASFlightsOptimizer where OptimizerRun = 'abc'  and carrierid = 'def'  order by TripNumber "
            'req = Replace(req, "abc", modelrunid)
            'req = Replace(req, "def", session("carrierid"))
            'DropDownTripNumbers.Items.Clear()
            'rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            'DropDownTripNumbers.Items.Add("----")
            'Do While Not rs.EOF
            '    DropDownTripNumbers.Items.Add(rs.Fields("tripnumber").Value)
            '    rs.MoveNext()
            'Loop
            'If rs.State = 1 Then rs.Close()


            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx StartDrop Registration " & Now & " - 1364 - ", "", "")


            count = 0

            For i = 1 To 500
                list(i) = ""
            Next




            For i = 0 To dv.Table.Rows.Count - 1
                a = dv.Table.Rows(i).Item("AC").ToString
                found = False
                For z = 1 To count
                    If list(z) = a Then
                        found = True
                        Exit For
                    End If
                Next
                If found = False Then
                    count = count + 1
                    list(count) = a
                End If
            Next i


            For i = 1 To count
                For z = i To count
                    If list(i) > list(z) Then
                        a = list(i)
                        list(i) = list(z)
                        list(z) = a

                    End If

                Next z
            Next i

            DropDownNNumbers.Items.Add("----")
            For i = 1 To count
                DropDownNNumbers.Items.Add((Trim(list(i))))
            Next i



            'req = "SELECT distinct(aircraftregistration)  FROM CASFlightsOptimizer where OptimizerRun = 'abc' and carrierid = 'def'  order by  aircraftregistration"
            'req = Replace(req, "abc", modelrunid)
            'req = Replace(req, "def", session("carrierid"))
            'DropDownNNumbers.Items.Clear()
            'rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            'DropDownNNumbers.Items.Add("----")
            'Do While Not rs.EOF
            '    If Not (IsDBNull(rs.Fields("aircraftregistration").Value)) Then
            '        DropDownNNumbers.Items.Add(Trim(rs.Fields("aircraftregistration").Value))
            '    End If

            '    rs.MoveNext()
            'Loop
            'If rs.State = 1 Then rs.Close()

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx StartDrop departureAirport " & Now & " - 1382 - ", "", "")


            count = 0

            For i = 1 To 500
                list(i) = ""
            Next



            For i = 0 To dv.Table.Rows.Count - 1
                a = dv.Table.Rows(i).Item("FROM").ToString
                found = False
                For z = 1 To count
                    If list(z) = a Then
                        found = True
                        Exit For
                    End If
                Next
                If found = False Then
                    count = count + 1
                    list(count) = a
                End If
            Next i


            For i = 1 To count
                For z = i To count
                    If list(i) > list(z) Then
                        a = list(i)
                        list(i) = list(z)
                        list(z) = a

                    End If

                Next z
            Next i

            DropDownOriginAirports.Items.Clear()
            DropDownOriginAirports.Items.Add("----")
            For i = 1 To count
                DropDownOriginAirports.Items.Add(Trim(list(i)))
            Next i


            'req = "SELECT distinct(DepartureAirport)  FROM CASFlightsOptimizer where OptimizerRun = 'abc'  and carrierid = 'def' order by DepartureAirport "
            'req = Replace(req, "abc", modelrunid)
            'req = Replace(req, "def", session("carrierid"))
            'DropDownOriginAirports.Items.Clear()
            'rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            'DropDownOriginAirports.Items.Add("----")
            'Do While Not rs.EOF
            '    If Not (IsDBNull(rs.Fields("DepartureAirport").Value)) Then
            '        DropDownOriginAirports.Items.Add(Trim(rs.Fields("DepartureAirport").Value))
            '    End If

            '    rs.MoveNext()
            'Loop
            'If rs.State = 1 Then rs.Close()

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx StartDrop ArrivalAirport " & Now & " - 1399 - ", "", "")

            count = 0

            For i = 1 To 500
                list(i) = ""
            Next



            For i = 0 To dv.Table.Rows.Count - 1
                a = dv.Table.Rows(i).Item("TO").ToString
                found = False
                For z = 1 To count
                    If list(z) = a Then
                        found = True
                        Exit For
                    End If
                Next
                If found = False Then
                    count = count + 1
                    list(count) = a
                End If
            Next i


            For i = 1 To count
                For z = i To count
                    If list(i) > list(z) Then
                        a = list(i)
                        list(i) = list(z)
                        list(z) = a

                    End If

                Next z
            Next i

            DropDownDestAirports.Items.Clear()
            DropDownDestAirports.Items.Add("----")
            For i = 1 To count
                DropDownDestAirports.Items.Add((Trim(list(i))))
            Next i


            'req = "SELECT distinct(ArrivalAirport)  FROM CASFlightsOptimizer where OptimizerRun = 'abc'  and carrierid = 'def' order by ArrivalAirport"
            'req = Replace(req, "abc", modelrunid)
            'req = Replace(req, "def", session("carrierid"))
            'DropDownDestAirports.Items.Clear()
            'rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            'DropDownDestAirports.Items.Add("----")
            'Do While Not rs.EOF
            '    If Not (IsDBNull(rs.Fields("ArrivalAirport").Value)) Then
            '        DropDownDestAirports.Items.Add(Trim(rs.Fields("ArrivalAirport").Value))
            '    End If

            '    rs.MoveNext()
            'Loop
            'If rs.State = 1 Then rs.Close()

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx StartDrop AC Type " & Now & " - 1416 - ", "", "")



            count = 0

            For i = 1 To 500
                list(i) = ""
            Next




            For i = 0 To dv.Table.Rows.Count - 1
                a = dv.Table.Rows(i).Item("type").ToString


                If a = "H80H" Or a = "H850" Then a = "H80X"


                found = False
                For z = 1 To count
                    If list(z) = a Then
                        found = True
                        Exit For
                    End If
                Next
                If found = False Then
                    count = count + 1
                    list(count) = a
                End If
            Next i


            For i = 1 To count
                For z = i To count
                    If list(i) > list(z) Then
                        a = list(i)
                        list(i) = list(z)
                        list(z) = a

                    End If

                Next z
            Next i

            DropDownFleetType.Items.Clear()
            DropDownFleetType.Items.Add("----")
            For i = 1 To count



                DropDownFleetType.Items.Add((Trim(list(i))))
            Next i



            'req = "SELECT distinct(AircraftType)  FROM casFlightsOptimizer where OptimizerRun = 'abc'  and carrierid = 'def' order by AircraftType "
            'req = Replace(req, "abc", modelrunid)
            'req = Replace(req, "def", session("carrierid"))
            'DropDownFleetType.Items.Clear()
            'rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            'DropDownFleetType.Items.Add("----")
            'Do While Not rs.EOF
            '    DropDownFleetType.Items.Add(Trim(rs.Fields("AircraftType").Value))
            '    rs.MoveNext()
            'Loop
            'If rs.State = 1 Then rs.Close()

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx End Load Drop Downs " & Now & " -1348 - ", "", "")

        End If

    End Sub


    Public Sub grabcounts()

        If IsNothing(modelrunid) Then Exit Sub
        If modelrunid = "" Then Exit Sub

        Dim cnmkazurelocal3 As New ADODB.Connection

        Dim rs As New ADODB.Recordset
        Dim req As String

        If cnmkazurelocal3.State = 1 Then cnmkazurelocal3.Close()
        If cnmkazurelocal3.State = 0 Then
            cnmkazurelocal3.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            cnmkazurelocal3.Open()
        End If


        req = "SELECT sum(CONVERT(integer, nauticalmiles)) as nauticalmiles   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and  DeadHead = 'True' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            If Not (IsDBNull(rs.Fields("nauticalmiles").Value)) Then
                Me.lblnonrevenuemiles.Text = FormatNumber(rs.Fields("nauticalmiles").Value, 0)
            Else
                Exit Sub
            End If
        End If

        If rs.State = 1 Then rs.Close()


        req = "SELECT sum(CONVERT(integer, nauticalmiles)) as nauticalmiles   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'False' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblrevenuemiles.Text = FormatNumber(rs.Fields("nauticalmiles").Value, 0)
        End If
        If rs.State = 1 Then rs.Close()


        '20120724 - pab - exclude maintenance legs - LegTypeCode = 77
        'req = "SELECT sum(CONVERT(integer, dhcost)) as dhcost   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'True'"
        req = "SELECT sum(CONVERT(integer, dhcost)) as dhcost   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'True' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblnonrevenueexpense.Text = "$" & FormatNumber(rs.Fields("dhcost").Value, 0)
        End If
        If rs.State = 1 Then rs.Close()



        '20120724 - pab - exclude maintenance legs - LegTypeCode = 77
        'req = "SELECT sum(CONVERT(integer, dhcost)) as dhcost FROM FOSFlightsOptimizer where OptimizerRun = 'abc'   "
        req = "SELECT sum(CONVERT(integer, dhcost)) as dhcost FROM FOSFlightsOptimizer where OptimizerRun = 'abc'  and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5' "
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lbltotalrevenueexpense.Text = "$" & FormatNumber(rs.Fields("dhcost").Value, 0)
        End If
        If rs.State = 1 Then rs.Close()


        '20120724 - pab - exclude maintenance legs - LegTypeCode = 77
        'req = "SELECT sum(CONVERT(integer, dhcost)) as dhcost FROM FOSFlightsOptimizer  where OptimizerRun = 'abc'  and DeadHead = 'False'"
        req = "SELECT sum(CONVERT(integer, dhcost)) as dhcost FROM FOSFlightsOptimizer  where OptimizerRun = 'abc'  and DeadHead = 'False' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblrevenueexpense.Text = "$" & FormatNumber(rs.Fields("dhcost").Value, 0)
        End If
        If rs.State = 1 Then rs.Close()

        req = "SELECT count(*) as count   FROM FOSFlightsOptimizer  where OptimizerRun = 'abc' and DeadHead = 'False' and LegTypeCode <> '77' and LegTypeCode <> '7' and tripnumber <> '12345' and legstate <> '5'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblrevenuelegs.Text = rs.Fields("count").Value
        End If
        If rs.State = 1 Then rs.Close()

        req = "SELECT count(*) as count   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'True' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblnonrevenuelegs.Text = rs.Fields("count").Value
        End If
        If rs.State = 1 Then rs.Close()


        req = "SELECT top 1 CONVERT(integer, nauticalmiles) as nauticalmiles  FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'True' and LegTypeCode <> '77' and LegTypeCode <> '7'  and legstate <> '5' order by nauticalmiles desc"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblLongestEmptyLeg.Text = rs.Fields("nauticalmiles").Value
        End If
        If rs.State = 1 Then rs.Close()


        req = "SELECT top 1 cONVERT(integer, nauticalmiles) as nauticalmiles   FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'True' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5' order by nauticalmiles asc"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblShortestEmtpyLeg.Text = rs.Fields("nauticalmiles").Value
        End If
        If rs.State = 1 Then rs.Close()


        req = "SELECT avg( cONVERT(integer, nauticalmiles)) as nauticalmiles  FROM FOSFlightsOptimizer where OptimizerRun = 'abc' and DeadHead = 'True' and LegTypeCode <> '77' and LegTypeCode <> '7' and legstate <> '5'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal3, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblAverageEmtpyNM.Text = rs.Fields("nauticalmiles").Value
        End If
        If rs.State = 1 Then rs.Close()


        Dim eff As Double
        eff = CDbl(lblrevenuemiles.Text) / (CDbl(lblnonrevenuemiles.Text) + CDbl(lblrevenuemiles.Text))

        lblefficiency.Text = FormatPercent(eff)

        If cnmkazurelocal3.State = 1 Then cnmkazurelocal3.Close()

    End Sub


    Protected Sub EditableLabelCity_Changed(ByVal sender As Object, ByVal e As EventArgs)
        '  [   Label1.Text = "EditableLabel Value changed at " + DateTime.Now.ToString()

        bttnSearchMembers_Click(Nothing, Nothing)

    End Sub 'EditableLabel1_Changed

    Protected Sub EditableLabelCountry_Changed(ByVal sender As Object, ByVal e As EventArgs)
        '  [   Label1.Text = "EditableLabel Value changed at " + DateTime.Now.ToString()

        bttnSearchMembers_Click(Nothing, Nothing)

    End Sub 'EditableLabel1_Changed

    Protected Sub bttnSearchMembers_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        '        Dim req As String
        '        req = "SELECT  [icao_id] ,[city]       ,[state]        ,[country]  , ctrydesc     ,airport_nm, entry_port " & _
        '",apinfo   FROM [SuperPortalV3].[dbo].[airport] "




        '        If Me.txtCountry.Text <> "" Then
        '            req = req & "where ctrydesc Like '" & Me.txtCountry.Text & "%'"
        '        End If

        '        If Me.txtCountry.Text <> "" And Me.txtCity.Text <> "" Then req = req & " and "

        '        If Me.txtCity.Text <> "" Then req = req & "  city Like '" & Me.txtCity.Text & "%' "

        '        BindData(req)

    End Sub







    Protected Sub cmdFindTrip_Click(ByVal sender As Object, ByVal e As System.EventArgs)



        Dim req As String


        Dim cnlocal As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim cnoptimizer As New ADODB.Connection

        ' Dim databaseTO As String = Me.txtDBto.Text
        Dim databaseFROM As String = "TMCProduction" 'Me.txtDBFrom.Text
        'Me.txtDBto.Text = "TMCOptimizerWorking"

        If cnoptimizer.State = 1 Then cnoptimizer.Close()
        If cnoptimizer.State = 0 Then
            '20120724 - fix connection strings
            ''   cnoptimizer.ConnectionString = ConnectionStringHelper.GetConnectionStringByDBName(databaseName)
            cnoptimizer.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            'cnoptimizer.ConnectionString = ConnectionStringOptimizerSQL
            cnoptimizer.Open()
        End If
        ' Dim modelrunid As String


        Dim tst As Label
        Dim desc As Label
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))


        Try
            If overridemodel = "" Then
                tst = Parent.FindControl("lblcarrier")
                desc = Parent.FindControl("lbldesc")


                If modelrunid <> "" Then
                    If Not (IsNothing(tst.Text)) Then
                        tst.Text = "Model Run: " & modelrunid
                    End If
                End If

            Else



            End If
        Catch
        End Try


        If modelrunid = "" Then


            req = "SELECT top 1 * from optimizerrequest where status = 'X' and description not like 'Optimizer request %'    and carrierid = abc  order by   id desc "
            req = Replace(req, "abc", Session("carrierid"))
            Dim daterangefrom, daterangeto As String
            Dim customid As String
            rs.Open(req, cnoptimizer, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            If Not rs.EOF Then
                'latest completed model
                customid = rs.Fields("id").Value

                Session("customid") = customid


                daterangefrom = rs.Fields("gmtstart").Value
                daterangeto = rs.Fields("gmtend").Value
                Dim description As String
                description = rs.Fields("description").Value & "   "

                If Not (IsNothing(desc)) Then
                    desc.Text = description
                End If
            End If



            'rk 7.30.14 replace default model
            If carrierid = WHEELSUP Then
                req = "SELECT top 1 [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4]  FROM [OptimizerLog]  where casrevenuemiles <> 0 and  left (modelrunid, 3) = '347' and caslinebreaks <= (foslinebreaks + 7)  and ModelRunID not like '%R11%' and ModelRunID not like '%R12%'    and ModelRunID not like '%R0%'  and CASrevenueexpense <> 0 and FOSrevenuelegs - 25 <= CASrevenuelegs and carrierid = abc order by CAStotalrevenueexpense asc"
                req = Replace(req, "347", customid)
                req = Replace(req, "(modelrunid, 3)", "(modelrunid, " & Len(customid) & ")")
                req = Replace(req, "abc", Session("carrierid"))

            ElseIf carrierid <> JETLINX Then
                req = "SELECT top 1 [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4]  FROM [OptimizerLog]  where casrevenuemiles <> 0 and  customrunnumber = '347'    and  modelrunid not like '%Q-%'   and carrierid = abc  order by deltaexpense desc"
                req = Replace(req, "347", customid)
                req = Replace(req, "abc", Session("carrierid"))
            Else
                req = "SELECT top 1 [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4]  FROM [OptimizerLog]  where casrevenuemiles <> 0 and  customrunnumber = '347'      and carrierid = abc  order by deltaexpense desc"
                req = Replace(req, "347", customid)
                req = Replace(req, "abc", Session("carrierid"))

            End If



            'rk 10.20.2012 grab the lowest of the two models
            If rs.State = 1 Then rs.Close()


            rs.Open(req, cnoptimizer, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            If Not rs.EOF Then
                modelrunid = Trim(rs.Fields("modelrunid").Value)


            End If
            'rk 7/21/2013 since we are not reading fos log don't have timestamp

            ' tst.Text = "Model Run: " & modelrunid & " From: " & daterangefrom & " To: " & daterangeto  '& " from data captured at  " & timestamp


            If rs.State = 1 Then rs.Close()


        End If

        If overridemodel = "" Then
            If Not (IsNothing(desc)) Then
                If Len(desc.Text) > 60 Then
                    desc.Font.Size = 10
                End If
            End If

        End If



        Dim s As String


        s = "SELECT  case when A.basecode = '' then 'ZZZ' else A.basecode end as base, A.id, ltrim(rtrim(A.DepartureAirporticao)) as [FROM] ,ltrim(rtrim(A.ArrivalAirporticao)) as [TO], " &
            "CAST(A.departuredate + ' ' + A.departuretime as datetime) as [departuredate] , " &
            "CAST(A.departuredategmt + ' ' + A.departuretimegmt as datetime) as [datetimeGMT], " &
            "CAST(A.ArrivalDate + ' ' + A.ArrivalTime as datetime) as [ArrivalDate], " &
            "CAST(A.ArrivalDateGMT + ' ' + A.ArrivalTimeGMT as datetime) as [TodatetimeGMT], " &
            "ltrim(rtrim(A.FlightTime)) as [Minutes]  , CONVERT(integer, A.NauticalMiles) as [NM], " &
            "ltrim(rtrim(A.AC)) as [AC] ,ltrim(rtrim(A.AircraftType)) as [Type]  , ltrim(rtrim(A.dhcost)) as cost," &
            "ltrim(rtrim(A.DeadHead)) as [DeadHead], ltrim(rtrim(A.tripnumber)) as tripnumber, " &
            "ltrim(rtrim(A.legratecode)) as [LRC] ,ltrim(rtrim(A.legpurposecode)) as [LPC] ,ltrim(rtrim(A.legtypecode)) as [LTC], " &
            "ltrim(rtrim(A.PIC)) as [PIC], ltrim(rtrim(A.SIC)) as [SIC], " &
            "ltrim(rtrim(A.HomeAirport)) as [HA], A.QuotedEquipType as [OE]," &
            "A.tripcost  ,A.triprevenue, A.pandl, B.AircraftWeightClass as [AWC]  FROM FOSFlightsOptimizer as A, AircraftWeightClass as B where A.OptimizerRun = 'abc' and B.AircraftType = A.AircraftType  order by "
        s = s & FOS_CarrierProfile.fosSortOrder


        If Not (IsNothing(DropDownNNumbers.SelectedItem)) Then
            If DropDownNNumbers.SelectedItem.Value <> "----" Then
                s = "SELECT  basecode as base,    id,   ltrim(rtrim([DepartureAirporticao])) as [FROM]    ,ltrim(rtrim([ArrivalAirporticao])) as [TO], CAST([departuredate] + ' ' + [departuretime] as datetime) as [departuredate]  " &
  ", CAST([departuredategmt] + ' ' + [departuretimegmt] as datetime) as [datetimeGMT] " &
     ", CAST([ArrivalDate] + ' ' + [ArrivalTime] as datetime) as [ArrivalDate] " &
     ", CAST([ArrivalDateGMT] + ' ' + [ArrivalTimeGMT] as datetime) as [TodatetimeGMT]   , ltrim(rtrim([FlightTime])) as [Minutes]  , CONVERT(integer, [NauticalMiles]) as [NM]   " &
                    "  ,ltrim(rtrim([AC])) as [AC]       ,ltrim(rtrim([AircraftType])) as [Type]  , ltrim(rtrim([dhcost])) as cost, ltrim(rtrim([DeadHead])) as [DeadHead], ltrim(rtrim(tripnumber)) as tripnumber " &
                    " ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC] , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC]      ,  [tripcost]  ,[triprevenue], [pandl]         FROM [FOSFlightsOptimizer]  where ac = 'def' and  OptimizerRun = 'abc'  and legstate <> '5'   and carrierid = 'ghi' "
                s = Replace(s, "def", DropDownNNumbers.SelectedItem.Value)
                If carrierid <> JETLINX Then
                    s = s & "  order by   aircrafttype, AC, datetimegmt  "
                ElseIf carrierid = WHEELSUP Then
                    s = s & " order by AC, datetimegmt  "
                Else
                    s = s & " order by base,  AC, datetimegmt  "
                End If

            End If
        End If

        If Not (IsNothing(DropDownTripNumbers.SelectedItem)) Then
            If DropDownTripNumbers.SelectedItem.Value <> "----" Then
                s = "SELECT  basecode as base,   id,    ltrim(rtrim([DepartureAirporticao])) as [FROM]    ,ltrim(rtrim([ArrivalAirporticao])) as [TO], CAST([departuredate] + ' ' + [departuretime] as datetime) as [departuredate]  " &
     ", CAST([ArrivalDate] + ' ' + [ArrivalTime] as datetime) as [ArrivalDate] " &
", CAST([departuredategmt] + ' ' + [departuretimegmt] as datetime) as [datetimeGMT] " &
     ", CAST([ArrivalDateGMT] + ' ' + [ArrivalTimeGMT] as datetime) as [TodatetimeGMT]  , ltrim(rtrim([FlightTime])) as [Minutes]  , CONVERT(integer, [NauticalMiles]) as [NM]   " &
                    "   ,ltrim(rtrim([AC])) as [AC]       ,ltrim(rtrim([AircraftType])) as [Type]  , ltrim(rtrim([dhcost])) as cost, ltrim(rtrim([DeadHead])) as [DeadHead], ltrim(rtrim(tripnumber)) as tripnumber " &
                    "  ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC] , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC]      ,  [tripcost]  ,[triprevenue], [pandl]       FROM [FOSFlightsOptimizer]  where tripnumber = 'def' and  OptimizerRun = 'abc'  and legstate <> '5'    and carrierid = 'ghi' "
                s = Replace(s, "def", DropDownTripNumbers.SelectedItem.Value)
                If carrierid <> JETLINX Then
                    s = s & "  order by   aircrafttype, AC, datetimegmt  "
                ElseIf carrierid = WHEELSUP Then
                    s = s & "order by AC, datetimegmt  "
                Else
                    s = s & " order by base,  AC, datetimegmt  "
                End If

            End If
        End If


        If Not (IsNothing(DropDownOriginAirports.SelectedItem)) Then
            If DropDownOriginAirports.SelectedItem.Value <> "----" Then
                s = "SELECT  basecode as base,    id,   ltrim(rtrim([DepartureAirporticao])) as [FROM]    ,ltrim(rtrim([ArrivalAirporticao])) as [TO], CAST([departuredate] + ' ' + [departuretime] as datetime) as [departuredate]  " &
  ", CAST([departuredategmt] + ' ' + [departuretimegmt] as datetime) as [datetimeGMT] " &
     ", CAST([ArrivalDate] + ' ' + [ArrivalTime] as datetime) as [ArrivalDate] " &
", CAST([ArrivalDateGMT] + ' ' + [ArrivalTimeGMT] as datetime) as [TodatetimeGMT]  , ltrim(rtrim([FlightTime])) as [Minutes]  , CONVERT(integer, [NauticalMiles]) as [NM]  " &
                    "    ,ltrim(rtrim([AC])) as [AC]       ,ltrim(rtrim([AircraftType])) as [Type]  , ltrim(rtrim([dhcost])) as cost, ltrim(rtrim([DeadHead])) as [DeadHead], ltrim(rtrim(tripnumber)) as tripnumber " &
                    "  ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC],  ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC]     ,  [tripcost]  ,[triprevenue], [pandl]       FROM [FOSFlightsOptimizer]  where DepartureAirporticao = 'def' and  OptimizerRun = 'abc'  and legstate <> '5'     and carrierid = 'ghi' "
                s = Replace(s, "def", DropDownOriginAirports.SelectedItem.Value)
                If carrierid <> JETLINX Then
                    s = s & " order by  ltrim(rtrim([AircraftType])),  AC, datetimegmt  "
                ElseIf carrierid = WHEELSUP Then
                    s = s & "order by AC, datetimegmt  "
                Else
                    s = s & " order by base,  AC, datetimegmt  "
                End If

            End If
        End If

        If Not (IsNothing(DropDownDestAirports.SelectedItem)) Then
            If DropDownDestAirports.SelectedItem.Value <> "----" Then
                s = "SELECT  basecode as base,    id,   ltrim(rtrim([DepartureAirporticao])) as [FROM]    ,ltrim(rtrim([ArrivalAirporticao])) as [TO], CAST([departuredate] + ' ' + [departuretime] as datetime) as [departuredate]  " &
  ", CAST([departuredategmt] + ' ' + [departuretimegmt] as datetime) as [datetimeGMT] " &
     ", CAST([ArrivalDate] + ' ' + [ArrivalTime] as datetime) as [ArrivalDate] " &
 ", CAST([ArrivalDateGMT] + ' ' + [ArrivalTimeGMT] as datetime) as [TodatetimeGMT]  , ltrim(rtrim([FlightTime])) as [Minutes]  , CONVERT(integer, [NauticalMiles]) as [NM]   " &
                    "   ,ltrim(rtrim([AC])) as [AC]       ,ltrim(rtrim([AircraftType])) as [Type]  , ltrim(rtrim([dhcost])) as cost, ltrim(rtrim([DeadHead])) as [DeadHead], ltrim(rtrim(tripnumber)) as tripnumber " &
                    "   ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC] , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC]    ,  [tripcost]  ,[triprevenue], [pandl]        FROM [FOSFlightsOptimizer]  where ArrivalAirporticao = 'def' and  OptimizerRun = 'abc'  and legstate <> '5'    and carrierid = 'ghi' "
                s = Replace(s, "def", DropDownDestAirports.SelectedItem.Value)
                If carrierid <> JETLINX Then
                    s = s & "  order by   aircrafttype, AC, datetimegmt  "
                ElseIf carrierid = WHEELSUP Then
                    s = s & "order by AC, datetimegmt  "
                Else
                    s = s & " order by base,  AC, datetimegmt  "
                End If

            End If
        End If



        If Not (IsNothing(DropDownFleetType.SelectedItem)) Then
            If DropDownFleetType.SelectedItem.Value <> "----" Then
                s = "SELECT  basecode as base,   id,    ltrim(rtrim([DepartureAirporticao])) as [FROM]    ,ltrim(rtrim([ArrivalAirporticao])) as [TO], CAST([departuredate] + ' ' + [departuretime] as datetime) as [departuredate]  " &
     ", CAST([ArrivalDate] + ' ' + [ArrivalTime] as datetime) as [ArrivalDate] " &
", CAST([departuredategmt] + ' ' + [departuretimegmt] as datetime) as [datetimeGMT] " &
  ", CAST([ArrivalDateGMT] + ' ' + [ArrivalTimeGMT] as datetime) as [TodatetimeGMT]  , ltrim(rtrim([FlightTime])) as [Minutes]  , CONVERT(integer, [NauticalMiles]) as [NM]   " &
                    "   ,ltrim(rtrim([AC])) as [AC]       ,ltrim(rtrim([AircraftType])) as [Type]  , ltrim(rtrim([dhcost])) as cost, ltrim(rtrim([DeadHead])) as [DeadHead], ltrim(rtrim(tripnumber)) as tripnumber " &
                    "  ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC], ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC]      ,  [tripcost]  ,[triprevenue], [pandl]         FROM [FOSFlightsOptimizer]  where AircraftType = 'def' and  OptimizerRun = 'abc'  and legstate <> '5'    and carrierid = 'ghi' "


                s = Replace(s, "def", DropDownFleetType.SelectedItem.Value)
                s = Replace(s, "where AircraftType = 'H80X'", "where (AircraftType = 'H80H' or AircraftType = 'H850')")

                If carrierid <> JETLINX Then
                    s = s & "  order by   aircrafttype, AC, datetimegmt  "
                ElseIf carrierid = WHEELSUP Then
                    s = s & "order by AC, datetimegmt  "
                Else
                    s = s & " order by base,  AC, datetimegmt  "
                End If

            End If
        End If



        Dim ss As String = Session("FCList")
        If AC1 <> "" Or ACX(1) <> "" Then
            If AC1 <> "" Then
                s = "SELECT      id,   ltrim(rtrim([DepartureAirporticao])) as [FROM]    ,ltrim(rtrim([ArrivalAirporticao])) as [TO], CAST([departuredate] + ' ' + [departuretime] as datetime) as [departuredate]  " &
", CAST([departuredategmt] + ' ' + [departuretimegmt] as datetime) as [datetimeGMT] " &
 ", CAST([ArrivalDate] + ' ' + [ArrivalTime] as datetime) as [ArrivalDate] " &
 ", CAST([ArrivalDateGMT] + ' ' + [ArrivalTimeGMT] as datetime) as [TodatetimeGMT]   , ltrim(rtrim([FlightTime])) as [Minutes]  , CONVERT(integer, [NauticalMiles]) as [NM]   " &
                "  ,ltrim(rtrim([AC])) as [AC]       ,ltrim(rtrim([AircraftType])) as [Type]  , ltrim(rtrim([dhcost])) as cost, ltrim(rtrim([DeadHead])) as [DeadHead], ltrim(rtrim(tripnumber)) as tripnumber " &
                " ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC] , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC]      ,  [tripcost]  ,[triprevenue], [pandl], basecode as base, '' as awc, '' as HA, '' as OE         FROM [FOSFlightsOptimizer]  where ( ac = 'def' or ac = 'zqg') and  OptimizerRun = 'abc'  and legstate <> '5'   and carrierid = 'ghi' "
            Else
                s = "SELECT      id,   ltrim(rtrim([DepartureAirporticao])) as [FROM]    ,ltrim(rtrim([ArrivalAirporticao])) as [TO], CAST([departuredate] + ' ' + [departuretime] as datetime) as [departuredate]  " &
", CAST([departuredategmt] + ' ' + [departuretimegmt] as datetime) as [datetimeGMT] " &
 ", CAST([ArrivalDate] + ' ' + [ArrivalTime] as datetime) as [ArrivalDate] " &
 ", CAST([ArrivalDateGMT] + ' ' + [ArrivalTimeGMT] as datetime) as [TodatetimeGMT]   , ltrim(rtrim([FlightTime])) as [Minutes]  , CONVERT(integer, [NauticalMiles]) as [NM]   " &
                "  ,ltrim(rtrim([AC])) as [AC]       ,ltrim(rtrim([AircraftType])) as [Type]  , ltrim(rtrim([dhcost])) as cost, ltrim(rtrim([DeadHead])) as [DeadHead], ltrim(rtrim(tripnumber)) as tripnumber " &
                " ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC] , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC]      ,  [tripcost]  ,[triprevenue], [pandl]         FROM [FOSFlightsOptimizer]  where ac in (aclist) and  OptimizerRun = 'abc'  and legstate <> '5'   and carrierid = 'ghi'  "
            End If

            s = Replace(s, "aclist", ss)
            s = Replace(s, "def", AC1)
            s = Replace(s, "zqg", AC2)
            If carrierid <> JETLINX Then
                s = s & "  order by   aircrafttype, AC, datetimegmt  "
            ElseIf carrierid = WHEELSUP Then
                s = s & "order by AC, datetimegmt  "
            Else
                s = s & " order by base,  AC, datetimegmt  "
            End If

        End If



        Dim mrcustom As String

        If overridemodel = "" Then

            mrcustom = normalizemodelrunid(modelrunid)

        Else
            mrcustom = overridemodel

        End If

        If overridemodel <> "" And (AC1 <> "" Or ACX(1) <> "") Then
            mrcustom = normalizemodelrunid(modelrunid)
        End If


        s = Replace(s, "abc", mrcustom)
        s = Replace(s, "ghi", Session("carrierid"))

        Session("sqlQueryTrips") = s
        Session("link") = "Trip"



        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Start BInd " & Now & "  ", "", "")
        BindDataTrips(s)
        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx End BInd " & Now & "  ", "", "")


    End Sub

    'Protected Sub bttnSearchByCustomers_Click(sender As Object, e As System.EventArgs) Handles bttnSearchByCustomers.Click

    '    Session("sqlQueryTrips") = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
    '    Session("sqlQueryTrips") &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
    '    Session("sqlQueryTrips") &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
    '    Session("sqlQueryTrips") &= "seatsavail as [Seats] FROM Itinerary where ItinFlightRequest > 1 and CarrierID = " & session("carrierid") & " order by id desc "
    '    Session("link") = "Trip"

    '    ' SqlDataSourceTrips.SelectCommand = "select * from itinerary where ItinFlightRequest > 1 order by id desc "

    '    'If Me.txtCustomer.Text <> "" Then
    '    '    '    SqlDataSourceTrips.SelectCommand = "select * from itinerary where ItinTitle Like '%" & Me.txtTripDescription.Text & "%' order by id desc"
    '    '    Session("sqlQueryTrips") = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
    '    '    Session("sqlQueryTrips") &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
    '    '    Session("sqlQueryTrips") &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
    '    '    Session("sqlQueryTrips") &= "seatsavail as [Seats] FROM Itinerary where ItinFlightRequest > 1 and ItinCustomer Like '%" & Me.txtCustomer.Text & "%' and CarrierID = " & session("carrierid")
    '    '    Session("sqlQueryTrips") &= " order by id desc "
    '    'End If


    '    'SqlDataSourceTrips.DataBind()
    '    ' GridViewTrips.DataBind()
    '    BindDataTrips()

    'End Sub

    'Protected Sub LinkRecent_Click(sender As Object, e As System.EventArgs) Handles LinkRecent.Click

    '    Session("sqlQueryTrips") = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
    '    Session("sqlQueryTrips") &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
    '    Session("sqlQueryTrips") &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
    '    Session("sqlQueryTrips") &= "seatsavail as [Seats] FROM Itinerary where ItinFlightRequest > 1 and CarrierID = " & session("carrierid") & " order by id desc "
    '    Session("link") = "Trip"

    '    BindDataTrips()

    'End Sub



    Protected Sub cmdRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRefresh.Click

        Dim req As String = ""

        Dim r1 As String = ""

        '  If Me.chkBooked.Checked = True Then r1 = r1 & " or itinstatus = 'B'"
        '  If Me.chkDispatched.Checked = True Then r1 = r1 & " or itinstatus = 'D'"
        '  If Me.chkFlown.Checked = True Then r1 = r1 & " or itinstatus = 'F'"
        '  If Me.chkInProgress.Checked = True Then r1 = r1 & " or itinstatus = 'A'"
        '  If Me.chkPending.Checked = True Then r1 = r1 & " or itinstatus = 'P'"
        '  If Me.chkQuoted.Checked = True Then r1 = r1 & " or itinstatus = 'Q'"

        'req = "select * from itinerary where  (itinstatus = 'z' " & r1 & ") and carrierid = " & session("carrierid") & "  order by id desc"
        req = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
        req &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
        req &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
        req &= "seatsavail as [Seats] FROM Itinerary where (itinstatus = 'z' " & r1 & ") and CarrierID = " & Session("carrierid") & " order by id desc "

        Session("sqlQueryTrips") = req


        BindDataTrips(req)


    End Sub



    Protected Sub LinkPending_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkPending.Click

        Dim id As Int16 = 0

        Try
            id = GridViewTrips.SelectedRow.Cells(1).Text
        Catch ex As Exception
            Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx LinkPending_Click Error " & Now & " - " & ex.Message, "", "")
        End Try

        If id <> 0 Then
            '         updateitin(id, "", "", "", "", "", "P")
        End If

        cmdRefresh_Click(Nothing, Nothing)




    End Sub

    'Protected Sub LinkQuoted_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkQuoted.Click
    '    Dim id As Int16 = 0

    '    Try
    '        id = GridViewTrips.SelectedRow.Cells(1).Text
    '    Catch ex As Exception
    '        Insertsys_log(session("carrierid"), appName, "FOSFlightsAzure.ascx LinkQuoted_Click Error " & Now & " - " & ex.Message, "", "")
    '    End Try

    '    If id <> 0 Then
    '        '          updateitin(id, "", "", "", "", "", "Q")
    '    End If

    '    cmdRefresh_Click(Nothing, Nothing)



    'End Sub

    'Protected Sub LinkBooked_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkBooked.Click
    '    Dim id As Int16 = 0

    '    Try
    '        id = GridViewTrips.SelectedRow.Cells(1).Text
    '    Catch ex As Exception
    '        Insertsys_log(session("carrierid"), appName, "FOSFlightsAzure.ascx LinkBooked_Click Error " & Now & " - " & ex.Message, "", "")
    '    End Try

    '    If id <> 0 Then
    '        '  updateitin(id, "", "", "", "", "", "B")
    '    End If

    '    cmdRefresh_Click(Nothing, Nothing)



    'End Sub

    'Protected Sub LinkInProgress_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkInProgress.Click
    '    Dim id As Int16 = 0

    '    Try
    '        id = GridViewTrips.SelectedRow.Cells(1).Text
    '    Catch ex As Exception
    '        Insertsys_log(session("carrierid"), appName, "FOSFlightsAzure.ascx LinkInProgress_Click Error " & Now & " - " & ex.Message, "", "")
    '    End Try

    '    If id <> 0 Then
    '        '   updateitin(id, "", "", "", "", "", "A")
    '    End If

    '    cmdRefresh_Click(Nothing, Nothing)

    'End Sub

    'Protected Sub LinkFlown_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkFlown.Click
    '    Dim id As Int16 = 0

    '    Try
    '        id = GridViewTrips.SelectedRow.Cells(1).Text
    '    Catch ex As Exception
    '        Insertsys_log(session("carrierid"), appName, "FOSFlightsAzure.ascx LinkFlown_Click Error " & Now & " - " & ex.Message, "", "")
    '    End Try

    '    If id <> 0 Then
    '        '      updateitin(id, "", "", "", "", "", "F")
    '    End If

    '    cmdRefresh_Click(Nothing, Nothing)


    'End Sub

    'Protected Sub LinkDispatched_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkDispatched.Click
    '    Dim id As Int16 = 0

    '    Try
    '        id = GridViewTrips.SelectedRow.Cells(1).Text
    '    Catch ex As Exception
    '        Insertsys_log(session("carrierid"), appName, "FOSFlightsAzure.ascx LinkDispatched_Click Error " & Now & " - " & ex.Message, "", "")
    '    End Try

    '    If id <> 0 Then
    '        '    updateitin(id, "", "", "", "", "", "D")
    '    End If

    '    cmdRefresh_Click(Nothing, Nothing)


    'End Sub


    Protected Sub DropDownNNumbers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownNNumbers.SelectedIndexChanged
        DropDownDestAirports.SelectedIndex = 0
        DropDownOriginAirports.SelectedIndex = 0
        DropDownTripNumbers.SelectedIndex = 0
        DropDownFleetType.SelectedIndex = 0

        cmdFindTrip_Click(Nothing, Nothing)
    End Sub

    Protected Sub DropDownTripNumbers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownTripNumbers.SelectedIndexChanged

        DropDownDestAirports.SelectedIndex = 0
        DropDownOriginAirports.SelectedIndex = 0
        DropDownNNumbers.SelectedIndex = 0
        DropDownFleetType.SelectedIndex = 0

        cmdFindTrip_Click(Nothing, Nothing)
    End Sub

    Protected Sub DropDownOriginAirports_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownOriginAirports.SelectedIndexChanged

        DropDownDestAirports.SelectedIndex = 0
        DropDownNNumbers.SelectedIndex = 0
        DropDownTripNumbers.SelectedIndex = 0
        DropDownFleetType.SelectedIndex = 0


        cmdFindTrip_Click(Nothing, Nothing)
    End Sub

    Protected Sub DropDownDestAirports_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownDestAirports.SelectedIndexChanged

        DropDownOriginAirports.SelectedIndex = 0
        DropDownNNumbers.SelectedIndex = 0
        DropDownTripNumbers.SelectedIndex = 0
        DropDownFleetType.SelectedIndex = 0
        cmdFindTrip_Click(Nothing, Nothing)
    End Sub



    Function colorme()




        Dim req As String

        '20120815 - pab - run time improvements
        Dim p1 As Date = CDate(Date.UtcNow.Month & "/" & Date.UtcNow.Day & "/" & Date.UtcNow.Year & " 10:00 AM")
        req = "select id, TripNumber, ltrim(rtrim(AirportFrom)) as AirportFrom, ltrim(rtrim(AirportTo)) as AirportTo, PinnedOn, Pinned from pinned " & _
            "where pinned = 1 and pinnedon > '" & p1 & "' order by tripnumber"


        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Get Pinned Start " & Now & "  ", "", "")
        Dim dt As DataTable = DataAccess.GetPinnedFlights(req)
        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Start Get Pinned End " & Now & "  ", "", "")


        req = "select * from aog " & _
         "where aog = 1 "
        Dim dtaog As DataTable = DataAccess.GetPinnedFlights(req)

        Dim dv_aog As DataView = dtaog.DefaultView


        Dim dv_pinned As DataView = dt.DefaultView



        Dim bAOGflights As Boolean = False


        If Not AirTaxi.isdtnullorempty(dtaog) Then
            bAOGflights = True
        End If



        Dim bpinnedflights As Boolean = False


        If Not AirTaxi.isdtnullorempty(dt) Then
            bpinnedflights = True
        End If

        Dim mycolor As System.Drawing.Color
        mycolor = Drawing.Color.Aquamarine

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))


        If cnsetting.State = 1 Then cnsetting.Close()
        If cnsetting.State = 0 Then
            cnsetting.ConnectionString = ConnectionStringHelper.GetMKConnectionStringsql
            cnsetting.Open()
        End If

        If ACX(1) <> "" Then
            GridViewTrips.Width = 900
            TopStats.Style.Clear()
            TopStats.Style.Add("visible", "false")
            TopStats.Visible = False
        Else
            GridViewTrips.Width = 1000
            TopStats.Style.Clear()
        End If




        Dim i As Integer
        For i = 0 To GridViewTrips.Rows.Count - 1


            'added plane lookup info back in rk 7.20.17
            If GridViewTrips.Rows(i).Cells(8).Text <> "" Then
                GridViewTrips.Rows(i).Cells(8).ToolTip = AirTaxi.lookupac(GridViewTrips.Rows(i).Cells(8).Text, carrierid)
            End If


            If GridViewTrips.Rows(i).Cells(0).Text = "KMMU" Then
                i = i
            End If


            Dim x As String = GridViewTrips.Rows(i).Cells(11).Text
            Select Case Left(GridViewTrips.Rows(i).Cells(11).Text, 1)
                Case "D", "T"
                    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat
                    GridViewTrips.Rows(i).Cells(11).Text = "DH"
                Case "R", "F"

                    'revenue
                    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkSeaGreen
                    GridViewTrips.Rows(i).Cells(11).Text = "Rev"


                    If GridViewTrips.Rows(i).Cells(17).Text = "CREW" Then
                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        GridViewTrips.Rows(i).Cells(11).Text = "C"
                    End If


                    If GridViewTrips.Rows(i).Cells(17).Text = "MXSC" Or GridViewTrips.Rows(i).Cells(17).Text = "SWAP" Then     'SWAP added by mws 11/25


                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        GridViewTrips.Rows(i).Cells(11).Text = "M"

                    End If


                    If GridViewTrips.Rows(i).Cells(17).Text = "M" Then


                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        GridViewTrips.Rows(i).Cells(11).Text = "M"

                    End If

                    If GridViewTrips.Rows(i).Cells(17).Text = "LMX" Then


                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        GridViewTrips.Rows(i).Cells(11).Text = "M"

                    End If


                    If GridViewTrips.Rows(i).Cells(17).Text = "Y" Then


                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.Orange
                        GridViewTrips.Rows(i).Cells(11).Text = "S"
                        GridViewTrips.Rows(i).Cells(11).ToolTip = "Static"
                    End If




                Case Else
                    If Trim(GridViewTrips.Rows(i).Cells(12).Text) = "12345" Then
                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkOrange
                        GridViewTrips.Rows(i).Cells(11).Text = "M"
                    Else
                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                    End If
            End Select






            'If Trim(GridViewTrips.Rows(i).Cells(18).Text) <> "" Then
            '    GridViewTrips.Rows(i).Cells(18).BackColor = Drawing.Color.Thistle
            'End If





            If Trim(GridViewTrips.Rows(i).Cells(12).Text) = "12345" Then
                GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkOrange
                GridViewTrips.Rows(i).Cells(11).Text = "M"


            End If




            'req = "select * from fosflightsoptimizer where TripNumber = 'abc' and departureairporticao = 'def' and  arrivalairporticao = 'ghi' and optimizerrun = 'jkl'"
            'req = Replace(req, "abc", Trim(GridViewTrips.Rows.Item(i).Cells(12).Text))
            'req = Replace(req, "def", Trim(GridViewTrips.Rows.Item(i).Cells(0).Text))
            'req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(i).Cells(1).Text))
            'req = Replace(req, "jkl", modelrunid)




            'Dim rs As New ADODB.Recordset
            'If rs.State = 1 Then rs.Close()
            'rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

            'If Not rs.EOF Then


            Dim lrc As String = GridViewTrips.Rows(i).Cells(14).Text.ToString.Trim
            Dim lpc As String = GridViewTrips.Rows(i).Cells(15).Text
            Dim ltc As String = GridViewTrips.Rows(i).Cells(16).Text
            '   Dim ltcx As String = GridViewTrips.Rows(i).Cells(14).Text

            Dim testme(40) As String
            For zz = 1 To 15
                testme(zz) = GridViewTrips.Rows(i).Cells(zz).Text
            Next



            'Dim lrc As String = Trim(rs.Fields("legratecode").Value)
            'Dim lpc As String = Trim(rs.Fields("legpurposecode").Value)
            ' GridViewTrips.Rows(i).Cells(10).ToolTip = lrc
            ' GridViewTrips.Rows(i).Cells(9).ToolTip = lpc

            'Dim ltc As String = Trim(rs.Fields("legtypecode").Value)
            ' GridViewTrips.Rows(i).Cells(11).ToolTip = ltc
            If ltc = "AOG" Or ltc = "DETL" Or ltc = "INSV" Or ltc = "MXRC" Or ltc = "MXUS" Or ltc = "MAINT" Then
                GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkOrange
                GridViewTrips.Rows(i).Cells(11).Text = "M"
            End If

            If ltc = "77" Or ltc = "M" Then
                GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                GridViewTrips.Rows(i).Cells(11).Text = "M"
            End If


            If ltc = "77" Or ltc = "MXSC" Or ltc = "SWAP" Then 'SWAP added by mws 11/25
                GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                GridViewTrips.Rows(i).Cells(11).Text = "M"
            End If
            '     End If


            If ltc = "Y" Or ltc = "S" Then
                GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.Orange
                GridViewTrips.Rows(i).Cells(11).Text = "S"
                GridViewTrips.Rows(i).Cells(11).ToolTip = "Static"
            End If


            'rk 7/20/2013
            '  If Trim(GridViewTrips.Rows(i).Cells(12).Text) = "67890" Then

            If ltc = "BULP" Then

                GridViewTrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                GridViewTrips.Rows(i).Cells(11).Text = "B"

            End If



            If ltc = "CREW" Then

                GridViewTrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                GridViewTrips.Rows(i).Cells(11).Text = "C"

            End If


            If ltc = "TRNG" Then

                GridViewTrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                GridViewTrips.Rows(i).Cells(11).Text = "T"

            End If


            If ltc = "LINB" Then
                GridViewTrips.Rows(i).Cells(14).ForeColor = Drawing.Color.DarkRed
                GridViewTrips.Rows(i).Cells(15).ForeColor = Drawing.Color.DarkRed
                GridViewTrips.Rows(i).Cells(16).ForeColor = Drawing.Color.DarkRed
                GridViewTrips.Rows(i).Cells(11).Text = "L"
                GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.LightGreen
            End If


            Dim account As Integer


            If ACX(1) <> "" Then mycolor = Drawing.Color.White
            If i > 0 Then
                GridViewTrips.Rows(i).Cells(8).Font.Bold = False

                If GridViewTrips.Rows(i).Cells(8).Text <> GridViewTrips.Rows(i - 1).Cells(8).Text Then 'ac change
                    '   GridViewTrips.Rows(i).Cells(8).BackColor = Drawing.Color.CadetBlue

                    GridViewTrips.Rows(i).Cells(8).Font.Bold = True


                    If ACX(1) <> "" Then mycolor = Drawing.Color.White
                    If ACX(1) = "" Then
                        account = account + 1
                        If account / 2 = Int(account / 2) Then
                            'GridViewTrips.Rows(i).Style.Add("BackColor", "#669999")
                            mycolor = Drawing.Color.Aquamarine
                        Else
                            'GridViewTrips.Rows(i).Style.Remove("BackColor")
                            'GridViewTrips.Rows(i).Style.Add("BackColor", "#000066")
                            mycolor = Drawing.Color.White
                        End If
                    End If
                    If ACX(1) <> "" Then mycolor = Drawing.Color.White

                Else
                        If GridViewTrips.Rows(i).Cells(0).Text <> GridViewTrips.Rows(i - 1).Cells(1).Text Then


                        If GridViewTrips.Rows(i).Cells(0).Text <> "SWAP" And GridViewTrips.Rows(i - 1).Cells(1).Text <> "SWAP" Then


                            GridViewTrips.Rows(i).Cells(0).BackColor = Drawing.Color.IndianRed
                            GridViewTrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.IndianRed

                            If specsfromlog = False Then
                                If IsNumeric(Me.lblnonrevenuelegs.Text) Then
                                    Me.lblnonrevenuelegs.Text = CInt(Me.lblnonrevenuelegs.Text) + 1
                                    Me.lblnonrevenuemiles.Text = CInt(Me.lblnonrevenuemiles.Text) + 1000
                                    Me.lblnonrevenueexpense.Text = CInt(Me.lblnonrevenueexpense.Text) + 4200
                                    Me.lbltotalrevenueexpense.Text = Me.lbltotalrevenueexpense.Text + 4200
                                End If

                            End If



                        Else
                            GridViewTrips.Rows(i).Cells(0).BackColor = Drawing.Color.MediumPurple
                            GridViewTrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.MediumPurple

                        End If

                    End If




                        Dim prevflight As Date = CDate(convdate(GridViewTrips.Rows(i - 1).Cells(3).Text))
                    Dim currflight As Date = CDate(convdate(GridViewTrips.Rows(i).Cells(2).Text))
                    Dim diff As Integer = DateDiff(DateInterval.Minute, prevflight, currflight)


                    If diff < 30 Then GridViewTrips.Rows(i - 1).Cells(3).BackColor = Drawing.Color.Yellow
                    If diff < 30 Then GridViewTrips.Rows(i).Cells(2).BackColor = Drawing.Color.Yellow


                    If diff < 0 Then GridViewTrips.Rows(i - 1).Cells(3).BackColor = Drawing.Color.Salmon
                    If diff < 0 Then GridViewTrips.Rows(i).Cells(2).BackColor = Drawing.Color.Salmon


                End If
            End If


            GridViewTrips.Rows(i).BackColor = mycolor







            Dim pinned As Boolean = False
            If bpinnedflights = True Then
                dv_pinned.RowFilter = "TripNumber = '" & GridViewTrips.Rows.Item(i).Cells(12).Text & "' and AirportFrom = '" &
                    GridViewTrips.Rows.Item(i).Cells(0).Text.ToString & "' and AirportTo = '" & GridViewTrips.Rows.Item(i).Cells(1).Text.ToString & "'"
                If dv_pinned.Count > 0 Then
                    pinned = True
                End If
            End If

            If pinned = False Then
                'GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.White
                'GridViewTrips.Rows.Item(i).Cells(12).BackColor = Drawing.Color.White  'rk 4.3.2013
                'GridViewTrips.Rows.Item(i).Cells(13).BackColor = Drawing.Color.White
            Else
                GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.Goldenrod
                GridViewTrips.Rows.Item(i).Cells(12).BackColor = Drawing.Color.Goldenrod
                GridViewTrips.Rows.Item(i).Cells(13).BackColor = Drawing.Color.Goldenrod 'rk 4.3.2013 change from 13 to 10 to show pinned field
            End If

            'rk 8.24.2013 back up auto pinned 3 hours
            If pinned = False Then
                If IsDate(GridViewTrips.Rows(i).Cells(2).Text) Then
                    Dim gmt As Date = DateTime.UtcNow
                    Dim departgmt As Date = CDate(GridViewTrips.Rows(i).Cells(2).Text)


                    GridViewTrips.Rows(i).Cells(2).Font.Underline = False
                    GridViewTrips.Rows(i).Cells(4).Font.Underline = False

                    If gmt > DateAdd(DateInterval.Minute, -180, departgmt) Then
                        'GridViewTrips.Rows(i).Cells(2).BackColor = Drawing.Color.Teal
                        'GridViewTrips.Rows(i).Cells(4).BackColor = Drawing.Color.Teal

                        GridViewTrips.Rows(i).Cells(2).Font.Underline = True
                        GridViewTrips.Rows(i).Cells(4).Font.Underline = True

                    End If
                End If
            End If




            'show aog flights
            Dim aog As Boolean = False
            If bAOGflights = True Then
                dv_aog.RowFilter = "Aircraft = '" & GridViewTrips.Rows.Item(i).Cells(8).Text & "'"
                If dv_aog.Count > 0 Then
                    aog = True
                End If
            End If

            If aog = False Then
                '   GridViewTrips.Rows.Item(i).Cells(8).BackColor = Drawing.Color.White

            Else
                GridViewTrips.Rows.Item(i).Cells(8).BackColor = Drawing.Color.Violet

            End If

            For z = 2 To 5
                If IsDate(GridViewTrips.Rows(i).Cells(z).Text) Then
                    Dim departgmt As Date = CDate(GridViewTrips.Rows(i).Cells(z).Text)
                    GridViewTrips.Rows(i).Cells(z).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
                End If
            Next z



            If Session("defaultemail") = "demo@cas.com" Then
                Dim a As String = GridViewTrips.Rows.Item(i).Cells(8).Text.ToString()

                Dim b As String
                b = ""

                For z = 1 To Len(a)
                    b = b & Chr(Asc(Mid(a, z, 1)) + 2)
                Next

                a = b


                GridViewTrips.Rows.Item(i).Cells(8).Text = b

            End If
        Next i


        If IsNumeric(lblrevenuemiles.Text) And IsNumeric(lblnonrevenueexpense.Text) And IsNumeric(lbltotalrevenueexpense.Text) And IsNumeric(lblnonrevenuemiles.Text) Then

            Dim eff As Double
            eff = CDbl(lblrevenuemiles.Text) / (CDbl(lblnonrevenuemiles.Text) + CDbl(lblrevenuemiles.Text))

            lblefficiency.Text = FormatPercent(eff)


            Me.lblnonrevenueexpense.Text = "$" & FormatNumber(Me.lblnonrevenueexpense.Text, 0)
            Me.lbltotalrevenueexpense.Text = "$" & FormatNumber(Me.lbltotalrevenueexpense.Text, 0)


            Me.lblnonrevenuemiles.Text = FormatNumber(Me.lblnonrevenuemiles.Text, 0)

        End If


        Dim getnewcwpair As Boolean = True 'Flags when a crew warning pair needs updating  mws 12/5/16
        Dim cwdepartureport As String 'crew warning message departure airport mws 12/5/16
        Dim cwarrivalport As String 'crew warning message arrival airport mws 12/5/16
        Dim endofgrid As Integer = GridViewTrips.Rows.Count - 1

        Dim ii As Integer

        For ii = 0 To GridViewTrips.Rows.Count - 1

            Dim a13, a8 As String
            a13 = GridViewTrips.Rows(ii).Cells(13).Text
            a8 = GridViewTrips.Rows(ii).Cells(8).Text



            Dim departuretime As Date
            Dim arrivaltime As Date
            Dim startofduty As Date

            Dim departureairport As String
            Dim arrivalairport As String

            Dim departuretimelocal As Date
            Dim arrivaltimelocal As Date


            Dim dutyday As Double



            Dim restinterval As Double
            Dim prevarrival As Date
            Dim fail As Boolean = False

            Dim i2 As Integer
            i2 = 0

            departureairport = GridViewTrips.Rows(ii).Cells(0).Text
            arrivalairport = GridViewTrips.Rows(ii).Cells(1).Text

            If getnewcwpair Then
                cwarrivalport = arrivalairport
                cwdepartureport = departureairport
                getnewcwpair = False
            End If



            'reset counter if MX event, assume crew change or rest achieved.
            'If GridViewTrips.Rows(ii).Cells(11).Text = "M" Then

            'startofduty = addyear(GridViewTrips.Rows(ii + 1).Cells(2).Text)
            'startofduty = DateAdd(DateInterval.Hour, -1, startofduty)
            ' End If

            If ii <> endofgrid Then
                If GridViewTrips.Rows(ii).Cells(11).Text = "M" Or GridViewTrips.Rows(ii).Cells(16).Text = "MXSC" Or GridViewTrips.Rows(ii).Cells(16).Text = "SWAP" Or GridViewTrips.Rows(ii).Cells(16).Text = "NC" Then 'SWAP and NC added by mws 11/25

                    getnewcwpair = True
                    startofduty = addyear(GridViewTrips.Rows(ii + 1).Cells(2).Text)
                    If GridViewTrips.Rows(ii).Cells(16).Text = "SWAP" Then
                        startofduty = DateAdd(DateInterval.Hour, -7, startofduty)  'added by mws 11/27/16 .. all crew need to show 1 hour early
                    Else
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty)  'added by mws 11/27/16 .. all crew need to show 1 hour early
                    End If
                    If Left(GridViewTrips.Rows(ii + 1).Cells(9).Text, 4) = "B350" Then
                        startofduty = DateAdd(DateInterval.Minute, -30, startofduty)  'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                    End If

                End If
            End If

            'If departureairport <> arrivalairport Then
            'If ii > 0 Then
            'If GridViewTrips.Rows(ii).Cells(8).Text <> GridViewTrips.Rows(ii - 1).Cells(8).Text Then 'ac change

            'startofduty = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
            'If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
            'startofduty = DateAdd(DateInterval.Hour, -1.5, startofduty)
            'Else
            'startofduty = DateAdd(DateInterval.Hour, -1, startofduty)
            'End If
            '
            ' End If
            'End If
            'End If
            If ii > 0 Then
                If departureairport <> arrivalairport Then
                    If GridViewTrips.Rows(ii).Cells(8).Text <> GridViewTrips.Rows(ii - 1).Cells(8).Text Then 'ac change
                        getnewcwpair = True
                        startofduty = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
                        Dim t As String = GridViewTrips.Rows(ii).Cells(9).Text
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty) 'modified by mws 11/27/16 all show is 60 minutes
                        If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
                            startofduty = DateAdd(DateInterval.Minute, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                        End If
                    End If
                End If
            End If

            'reset duty day if crew change
            If ii > 0 Then
                If departureairport <> arrivalairport Then
                    If GridViewTrips.Rows(ii).Cells(8).Text = GridViewTrips.Rows(ii - 1).Cells(8).Text Then 'no ac change


                        If GridViewTrips.Rows(ii).Cells(18).Text.Trim <> "" Then  'make sure current and prev PIC and SIC not blank
                            If GridViewTrips.Rows(ii).Cells(19).Text.Trim <> "" Then
                                If GridViewTrips.Rows(ii - 1).Cells(18).Text.Trim <> "" Then
                                    If GridViewTrips.Rows(ii - 1).Cells(19).Text.Trim <> "" Then

                                        If GridViewTrips.Rows(ii - 1).Cells(18).Text.Trim <> GridViewTrips.Rows(ii).Cells(18).Text.Trim Then 'if change of PIC
                                            If GridViewTrips.Rows(ii - 1).Cells(19).Text.Trim <> GridViewTrips.Rows(ii).Cells(19).Text.Trim Then ' if change of SIC
                                                getnewcwpair = True
                                                startofduty = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
                                                startofduty = DateAdd(DateInterval.Hour, -1, startofduty) ' modified by mws 11/27/16 all crew need 60 minute show
                                                If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
                                                    startofduty = DateAdd(DateInterval.Hour, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                                                End If
                                                'If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
                                                'startofduty = DateAdd(DateInterval.Hour, -1.5, startofduty)
                                                'Else
                                                'startofduty = DateAdd(DateInterval.Hour, -1, startofduty)
                                                'End If

                                            End If
                                        End If

                                    End If
                                End If
                            End If
                        End If



                    End If
                End If
            End If


            i = i + 1


            departuretime = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
            arrivaltime = addyear(GridViewTrips.Rows(ii).Cells(3).Text)

            'departuretimelocal = GridViewTrips.Rows(ii).Cells(4).Text
            'arrivaltimelocal = GridViewTrips.Rows(ii).Cells(5).Text

            Dim ri As Integer = 11.99
            'If (departuretimelocal.Hour > 2 And departuretimelocal.Hour < 6) Or (arrivaltimelocal.Hour > 2 And arrivaltimelocal.Hour < 6) Then
            '    ri = 12.99
            'End If

            dutyday = dutyday

            If i > 1 Then
                If departureairport <> arrivalairport Then

                    restinterval = DateDiff(DateInterval.Minute, prevarrival, departuretime) + 60
                    restinterval = restinterval
                    restinterval = restinterval / 60

                    'if the rest interval is ok then advance start of duty
                    If restinterval > ri Then
                        getnewcwpair = True
                        startofduty = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty) ' modified by mws 11/27/16 all crew need 60 minute show
                        If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
                            startofduty = DateAdd(DateInterval.Minute, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                        End If
                        ' If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
                        'startofduty = DateAdd(DateInterval.Hour, -1.5, startofduty)
                        'Else
                        'startofduty = DateAdd(DateInterval.Hour, -1, startofduty)
                        'End If
                    End If


                    dutyday = DateDiff(DateInterval.Minute, startofduty, arrivaltime) + 30
                    dutyday = dutyday / 60


                    If dutyday > 11 Then
                        fail = True
                        GridViewTrips.Rows(ii).Cells(18).BackColor = Drawing.Color.YellowGreen
                        GridViewTrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Warning: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If


                    If dutyday > 12 Then
                        fail = True
                        GridViewTrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Yellow
                        GridViewTrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Concern: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If



                    If dutyday > 13 Then
                        fail = True
                        GridViewTrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Red
                        GridViewTrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Alert: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If
                End If

            End If

            If departureairport <> arrivalairport Then 'rk 10/12/13 only count prior arrival if it is really a flight
                prevarrival = addyear(GridViewTrips.Rows(ii).Cells(3).Text)
            End If

        Next ii

        Dim d, r As Double

        For i = 0 To GridViewTrips.Rows.Count - 1

            If Left(GridViewTrips.Rows(i).Cells(11).Text, 1) = "T" Then
                d = d + GridViewTrips.Rows(i).Cells(7).Text
            End If

            If Left(GridViewTrips.Rows(i).Cells(11).Text, 1) = "F" Then
                r = r + GridViewTrips.Rows(i).Cells(7).Text
            End If
            Try
                GridViewTrips.Rows(i).Cells(1).ToolTip = fname(GridViewTrips.Rows(i).Cells(1).Text)
                GridViewTrips.Rows(i).Cells(0).ToolTip = fname(GridViewTrips.Rows(i).Cells(0).Text)
            Catch
            End Try

            Dim acchange As Boolean = False

            If i + 1 > GridViewTrips.Rows.Count - 1 Then
                acchange = True
                GoTo acskip
            End If
            If GridViewTrips.Rows(i).Cells(8).Text <> GridViewTrips.Rows(i + 1).Cells(8).Text Then acchange = True 'ac change
acskip:
            If acchange Then
                Dim pct As Double = 0
                If r + d <> 0 Then
                    pct = (r) / (r + d)
                    pct = Int(pct * 100)
                End If
                ' GridViewTrips.Rows(i - 1).Cells(23).Text = pct
                GridViewTrips.Rows(i).Cells(9).ToolTip = pct & "%"

                If r + d <> 0 Then
                    If pct < 50 Then GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.Yellow
                    If pct < 30 Then GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.Red
                    If pct >= 50 Then GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.GreenYellow
                    If pct > 85 Then GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.Green
                End If
                d = 0
                r = 0
            End If
            'added plane lookup info back in rk 7.20.17
            If GridViewTrips.Rows(i).Cells(8).Text <> "" Then
                GridViewTrips.Rows(i).Cells(8).ToolTip = AirTaxi.lookupac(GridViewTrips.Rows(i).Cells(8).Text, carrierid)
            End If
        Next i



        If GridViewTrips.Rows.Count > 5 Then

            linemeup()

        End If
        If ACX(1) <> "" Then
            For z = 11 To GridViewTrips.Columns.Count - 1
                GridViewTrips.Columns(z).Visible = False
            Next z
        End If
        Select Case carrierid
            Case WHEELSUP
                GridViewTrips.Columns(FOS_PANDL).Visible = True
                GridViewTrips.Columns(FOS_REVENUE).Visible = True
                Exit Select
            Case JETLINX
                GridViewTrips.Columns(FOS_WC).Visible = True
                GridViewTrips.Columns(FOS_BASE).Visible = True
                GridViewTrips.Columns(FOS_HA).Visible = True
                GridViewTrips.Columns(FOS_OE).Visible = True
                Exit Select
            Case DELTA
                GridViewTrips.Columns(FOS_BASE).Visible = True
                GridViewTrips.Columns(FOS_HA).Visible = True
                GridViewTrips.Columns(FOS_OE).Visible = True
        End Select



    End Function

    Function linemeup() ' For FOS Side

        Dim aline(500, 4) As String ' 0=AC number, 1 = row count, 2= number of blank lines to insert after row
        Dim gridviewrowstart As Integer = 0


        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim s As String = ""
        Dim req As String

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        If ACX(1) <> "" Then
            s = Session("FCList")
        End If


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            cn.Open()
        End If

        If carrierid <> JETLINX Then
            If carrierid = WHEELSUP Then
                req = "SELECT count(*) as cnt, ac,aircrafttype FROM [fosFlightsOptimizer]  where optimizerrun = '1140-240797-R1-52C-96' group by ac,aircrafttype order by ac,aircrafttype"
            Else
                req = "SELECT count(*) as cnt, A.ac, B.AircraftWeightClass  as AWC FROM fosFlightsOptimizer as A, AircraftWeightClass as B  where A.optimizerrun = '1140-240797-R1-52C-96' and A.AircraftType = B.AircraftType group by B.AircraftWeightClass,A.ac order by AWC,A.ac"
            End If
        Else
            'req = "SELECT count(*) as cnt, ac,basecode FROM fosFlightsOptimizer where optimizerrun = '1140-240797-R1-52C-96'  group by basecode,ac order by basecode,ac"
            req = "SELECT count(*) as cnt, A.ac,case when A.basecode = '' then 'ZZZ' else A.basecode end + B.AircraftWeightClass  as AWC FROM fosFlightsOptimizer as A, AircraftWeightClass as B  where A.optimizerrun = '1140-240797-R1-52C-96' and A.AircraftType = B.AircraftType group by case when A.basecode = '' then 'ZZZ' else A.basecode end + B.AircraftWeightClass,A.ac order by AWC,A.ac"
        End If

        Dim mrcustom As String
        Dim ac As Integer = 0
        mrcustom = normalizemodelrunid(modelrunid)
        req = Replace(req, "1140-240797-R1-52C-96", mrcustom)

        'If s <> "" Then
        '    req = "SELECT count(*)  as cnt, ac,aircrafttype FROM [FOSFlightsOptimizer]  where optimizerrun = '1140-240797-R1-52C-96'  and legstate <> '5' group by aircrafttype,ac order by aircrafttype,ac"
        '    req = Replace(req, "group", "and ac in (" & s & ") and CONVERT(datetime, DepartureDategmt + ' ' + DepartureTimegmt)   >  SYSUTCDATETIME() group")
        '    req = Replace(req, "1140-240797-R1-52C-96", mrcustom & "-1")
        'End If




        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        'First we load the array with the Fos flights
        Do While Not rs.EOF
            ac = ac + 1
            aline(ac, 0) = rs.Fields("ac").Value.ToString.Trim
            aline(ac, 1) = rs.Fields("cnt").Value
            aline(ac, 3) = rs.Fields(2).Value.ToString.Trim
            If Left(aline(ac, 0), 4) <> "WUND" Then
                aline(ac, 2) = 1
            Else
                aline(ac, 2) = 0
            End If
            rs.MoveNext()
        Loop


        If carrierid <> JETLINX Then
            If carrierid = WHEELSUP Then
                req = "SELECT count(*)  as cnt, aircraftregistration,aircrafttype FROM [CASFlightsOptimizer]  where optimizerrun = '1140-240797-R1-52C-96' group by aircraftregistration,aircrafttype order by aircraftregistration,aircrafttype"
            Else
                req = "SELECT count(*)  as cnt, A.aircraftregistration,B.AircraftWeightClass  as AWC FROM CASFlightsOptimizer as A, AircraftWeightClass as B where A.optimizerrun = '1140-240797-R1-52C-96' and A.AircraftType = B.AircraftType group by B.AircraftWeightClass ,A.aircraftregistration order by AWC,A.aircraftregistration"
            End If
        Else
            ' req = "SELECT count(*)  as cnt, aircraftregistration,basecode FROM CASFlightsOptimizer where optimizerrun = '1140-240797-R1-52C-96' group by basecode ,aircraftregistration order by basecode,aircraftregistration"
            req = "SELECT count(*)  as cnt, A.aircraftregistration,case when A.basecode = '' then 'ZZZ' else A.basecode end + B.AircraftWeightClass  as AWC FROM CASFlightsOptimizer as A, AircraftWeightClass as B where A.optimizerrun = '1140-240797-R1-52C-96' and A.AircraftType = B.AircraftType group by case when A.basecode = '' then 'ZZZ' else A.basecode end + B.AircraftWeightClass ,A.aircraftregistration order by AWC,A.aircraftregistration"
        End If
        'If s <> "" Then
        '        req = Replace(req, "group", "and aircraftregistration in (" & s & ") and CONVERT(datetime, departuretime)   >  SYSUTCDATETIME() group")
        '    End If
        req = Replace(req, "1140-240797-R1-52C-96", modelrunid)

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


        'now we will loop thru all the Cas flights and find the matching ones and build the array
        'we can stop this when we run out of Fos Flights 
        Dim x As Integer  ' a generic counter for number of fos ac...

        For x = 1 To ac
            Do While Not rs.EOF  'once we run out of cas flights we are done..
                If aline(x, 3) = rs.Fields(2).Value.ToString.Trim Then
                    If aline(x, 0) > rs.Fields(1).Value.ToString.Trim Then  ' this tells us the cas flight is before our Fos flight we need spaces in front
                        If x = 1 Then
                            gridviewrowstart = gridviewrowstart + rs.Fields("cnt").Value + 1 ' if gridviewrowstart > 0 then we will add that many blank lines to first row
                        Else
                            aline(x - 1, 2) = aline(x - 1, 2) + rs.Fields("cnt").Value + 1
                        End If
                        rs.MoveNext()
                    ElseIf aline(x, 0) = rs.Fields(1).Value.ToString.Trim Then
                        If rs.Fields("cnt").Value > aline(x, 1) Then
                            aline(x, 2) = aline(x, 2) + (rs.Fields("cnt").Value - aline(x, 1))
                        End If
                        rs.MoveNext()
                        Exit Do
                    Else
                        Exit Do
                    End If
                ElseIf aline(x, 3) > rs.Fields(2).Value.ToString.Trim Then
                    aline(x - 1, 2) = aline(x - 1, 2) + rs.Fields(0).Value + 1
                    rs.MoveNext()
                Else
                    Exit Do
                End If
            Loop
        Next x
        aline(ac, 2) = 0

        If rs.State = 1 Then rs.Close()

        Dim rowstoadd As Integer ' now we start looping through array to build spaces
        Dim rownumber As Integer = 0
        Dim zz As Integer
        For x = 0 To ac
            If gridviewrowstart > 0 Then ' we need to pre fill empty spaces before all data
                rowstoadd = gridviewrowstart
                gridviewrowstart = 0
            Else
                rowstoadd = aline(x, 2)
                rownumber = rownumber + aline(x, 1)
            End If
            If Left(aline(x, 0), 4) <> "WUND" Then
                Dim Test As Integer = GridViewTrips.Rows.Count

                For zz = 1 To rowstoadd
                    '---------------------- To Insert empty Row At Specific location ------------------------
                    Dim row As New GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Alternate)

                    '   For Each field As DataControlField In GridViewTrips.Columns
                    Dim colstostrip As Integer = FOS_CarrierProfile.fosGridColstoStrip
                    ''Select Case _carrierid
                    'Case WHEELSUP
                    '        colstostrip = 7
                    '        Exit Select
                    '    Case JETLINX
                    '        colstostrip = 5
                    '        Exit Select
                    '    Case Else
                    '        colstostrip = 6
                    'End Select
                    Dim colMax = GridViewTrips.Columns.Count - colstostrip

                    For zzz = 1 To colMax
                        Dim cell As New TableCell
                        cell.Text = "&nbsp;"
                        row.Cells.Add(cell)
                    Next
                    rownumber = rownumber + 1
                    'If rownumber > GridViewTrips.Rows.Count Then rownumber = GridViewTrips.Rows.Count

                    GridViewTrips.Controls(0).Controls.AddAt(rownumber, row) ' This line will insert row at 2nd line
                Next zz
            End If
        Next x
    End Function

    Function colormebak()

        Dim req As String

        Dim i As Integer
        For i = 0 To GridViewTrips.Rows.Count - 1


            GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.White
            GridViewTrips.Rows.Item(i).Cells(9).BackColor = Drawing.Color.White



            '   GridViewTrips.Rows(i).Cells(0).Text = "Pin"


            'Pending	Quoted	Booked	Disptached	In Progress	Flown	
            For z = 0 To 9
                GridViewTrips.Rows(i).Cells(z).Text = Trim(GridViewTrips.Rows(i).Cells(z).Text)
            Next z

            GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.SteelBlue


            If GridViewTrips.Rows(i).Cells(9).Text = "True" Or GridViewTrips.Rows(i).Cells(9).Text = "D" Then
                GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat
                GridViewTrips.Rows(i).Cells(9).Text = "D"
            End If


            If GridViewTrips.Rows(i).Cells(9).Text = "False" Or Left(GridViewTrips.Rows(i).Cells(9).Text, 1) = "R" Then
                GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.DarkSeaGreen
                GridViewTrips.Rows(i).Cells(9).Text = "R"
            End If
            'If GridViewTrips.Rows(i).Cells(13).Text = "B" Then GridViewTrips.Rows(i).Cells(13).BackColor = Drawing.Color.Green
            'If GridViewTrips.Rows(i).Cells(13).Text = "D" Then GridViewTrips.Rows(i).Cells(13).BackColor = Drawing.Color.GreenYellow
            'If GridViewTrips.Rows(i).Cells(13).Text = "A" Then GridViewTrips.Rows(i).Cells(13).BackColor = Drawing.Color.IndianRed
            'If GridViewTrips.Rows(i).Cells(13).Text = "F" Then GridViewTrips.Rows(i).Cells(13).BackColor = Drawing.Color.Teal



            req = "select * from fosflightsoptimizer where TripNumber = 'abc' and AirportFrom = 'def' and  AirportTo = 'ghi' and optimizerrun = 'jkl'"
            req = Replace(req, "abc", Trim(GridViewTrips.Rows.Item(i).Cells(10).Text))
            req = Replace(req, "def", Trim(GridViewTrips.Rows.Item(i).Cells(0).Text))
            req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(i).Cells(1).Text))
            req = Replace(req, "jkl", modelrunid)

            Dim rs As New ADODB.Recordset
            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

            If Not rs.EOF Then
                Dim ltc As String = rs.Fields("legtypecode").Value
                GridViewTrips.Rows(i).Cells(9).ToolTip = ltc
                If ltc = "MXSC" Or ltc = "AOG" Or ltc = "M" Or ltc = "DETL" Or ltc = "INSV" Or ltc = "MXRC" Or ltc = "SWAP" Or ltc = "MXUS" Or ltc = "MAINT" Then 'SWAP added by mws 11/25
                    GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.DarkOrange
                    GridViewTrips.Rows(i).Cells(9).Text = "M"
                End If


                GridViewTrips.Rows(i).Cells(9).ToolTip = ltc
                If ltc = "Y" Then
                    GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.Orange

                    GridViewTrips.Rows(i).Cells(9).Text = "S"
                    GridViewTrips.Rows(i).Cells(9).ToolTip = "Static"

                End If



                If ltc = "Y" Then
                    GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.Orange

                    GridViewTrips.Rows(i).Cells(9).Text = "S"
                    GridViewTrips.Rows(i).Cells(9).ToolTip = "Static"

                End If



                If ltc = "Y" Then
                    GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.MediumPurple

                    GridViewTrips.Rows(i).Cells(9).Text = "SWAP"
                    GridViewTrips.Rows(i).Cells(9).ToolTip = "Swa["

                End If


            End If

            If Trim(GridViewTrips.Rows(i).Cells(10).Text) = "12345" Then
                GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.DarkOrange
                GridViewTrips.Rows(i).Cells(9).Text = "M"
            End If

            If i > 0 Then
                If GridViewTrips.Rows(i).Cells(6).Text <> GridViewTrips.Rows(i - 1).Cells(6).Text Then 'ac change
                    GridViewTrips.Rows(i).Cells(6).BackColor = Drawing.Color.CadetBlue
                Else
                    If GridViewTrips.Rows(i).Cells(0).Text <> GridViewTrips.Rows(i - 1).Cells(1).Text Then

                        If GridViewTrips.Rows(i).Cells(0).Text <> "SWAP" And GridViewTrips.Rows(i - 1).Cells(1).Text <> "SWAP" Then


                            GridViewTrips.Rows(i).Cells(0).BackColor = Drawing.Color.IndianRed
                            GridViewTrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.IndianRed

                            If specsfromlog = False Then
                                Me.lblnonrevenuelegs.Text = CInt(Me.lblnonrevenuelegs.Text) + 1
                                Me.lblnonrevenuemiles.Text = CInt(Me.lblnonrevenuemiles.Text) + 1000
                                Me.lblnonrevenueexpense.Text = CInt(Me.lblnonrevenueexpense.Text) + 4200
                                Me.lbltotalrevenueexpense.Text = Me.lbltotalrevenueexpense.Text + 4200
                            End If
                        End If

                    Else

                        GridViewTrips.Rows(i).Cells(0).BackColor = Drawing.Color.MediumPurple
                        GridViewTrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.MediumPurple


                    End If


                End If
            End If


            If cnsetting.State = 1 Then cnsetting.Close()
            If cnsetting.State = 0 Then
                cnsetting.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
                cnsetting.Open()
            End If


            req = "select * from pinned where TripNumber = 'abc' and AirportFrom = 'def' and  AirportTo = 'ghi' "
            req = Replace(req, "abc", Trim(GridViewTrips.Rows.Item(i).Cells(10).Text))
            req = Replace(req, "def", Trim(GridViewTrips.Rows.Item(i).Cells(0).Text))
            req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(i).Cells(1).Text))

            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


            Dim pinned As Boolean = False

            If Not (rs.EOF) Then
                If rs.Fields("Pinned").Value = True Then
                    Dim po As Date = rs.Fields("pinnedon").Value
                    Dim p1 As Date = CDate(Date.UtcNow.Month & "/" & Date.UtcNow.Day & "/" & Date.UtcNow.Year & " 10:00 AM")
                    If po > p1 Then
                        pinned = True
                    End If

                End If
            End If

            If pinned = False Then

                GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.White
                GridViewTrips.Rows.Item(i).Cells(11).BackColor = Drawing.Color.White
            Else
                GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.Goldenrod
                GridViewTrips.Rows.Item(i).Cells(11).BackColor = Drawing.Color.Goldenrod
            End If



            If IsDate(GridViewTrips.Rows(i).Cells(2).Text) Then

                Dim gmt As Date = DateTime.UtcNow
                Dim departgmt As Date = CDate(GridViewTrips.Rows(i).Cells(2).Text)
                If gmt > DateAdd(DateInterval.Minute, 180, departgmt) Then GridViewTrips.Rows(i).Cells(2).BackColor = Drawing.Color.Teal

            End If

            '20120724 - pab - highlight mtc trips orange
            If Trim(GridViewTrips.Rows(i).Cells(10).Text) = "12345" Then
                GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.DarkOrange
                GridViewTrips.Rows(i).Cells(9).Text = "M"
            End If



            If rs.State = 1 Then rs.Close()

        Next i


        If IsNumeric(lblrevenuemiles.Text) Then

            Dim eff As Double
            eff = CDbl(lblrevenuemiles.Text) / (CDbl(lblnonrevenuemiles.Text) + CDbl(lblrevenuemiles.Text))

            lblefficiency.Text = FormatPercent(eff)


            Me.lblnonrevenueexpense.Text = "$" & FormatNumber(Me.lblnonrevenueexpense.Text, 0)
            Me.lbltotalrevenueexpense.Text = "$" & FormatNumber(Me.lbltotalrevenueexpense.Text, 0)


            Me.lblnonrevenuemiles.Text = FormatNumber(Me.lblnonrevenuemiles.Text, 0)

        End If


    End Function


    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        ' grabcounts()

        'If modelrunid = "" Then
        '    If Not Request.QueryString("modelrunid") Is Nothing Then
        '        modelrunid = Request.QueryString("modelrunid")
        '    End If

        '    If modelrunid <> "" Then

        '        If Mid(modelrunid, Len(modelrunid) - 1, 1) = "-" Then
        '            modelrunid = Left(modelrunid, Len(modelrunid) - 2)
        '        End If


        '        If Mid(modelrunid, Len(modelrunid) - 2, 1) = "-" Then
        '            modelrunid = Left(modelrunid, Len(modelrunid) - 3)
        '        End If

        '    End If
        'End If

        Dim n As DateTime




    End Sub

    Protected Sub DropDownFleetType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFleetType.SelectedIndexChanged
        DropDownOriginAirports.SelectedIndex = 0
        DropDownDestAirports.SelectedIndex = 0
        DropDownNNumbers.SelectedIndex = 0
        DropDownTripNumbers.SelectedIndex = 0
        cmdFindTrip_Click(Nothing, Nothing)
    End Sub

    Function convdate(d As String) As Date


        If IsDate(d) Then
            Return CDate(d)
            Exit Function
        End If

        Dim s As String
        For i = 1 To Len(d)


            If Mid(d, i, 1) = " " Then
                s = s & "/" & Now.Year
            End If

            s = s & Mid(d, i, 1)


        Next

        If IsDate(s) Then
            Return s
        Else

            Return CDate("1/1/2001")
        End If



    End Function

    Protected Sub chkLocal_CheckedChanged(sender As Object, e As EventArgs) Handles chkLocal.CheckedChanged
        cmdFindTrip_Click(Nothing, Nothing)
    End Sub

    Protected Sub chkGMT_CheckedChanged(sender As Object, e As EventArgs) Handles chkGMT.CheckedChanged
        cmdFindTrip_Click(Nothing, Nothing)
    End Sub



    Function addyear(d As String) As Date
        Dim dd As String = d
        Dim dd1 As String = d
        Dim ispace As Integer
        ispace = InStr(dd, " ")

        If Not (IsDate(dd)) Then
            dd = Trim(Left(dd, ispace)) & "/" & Now.Year & " " & Right(dd, Len(dd) - ispace)
        End If

        If IsDate(dd) Then Return dd


    End Function


    'Protected Sub GridViewTrips_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewTrips.RowCommand


    '    If IsNothing(Session("carrierid")) Then Exit Sub


    '    Dim cnpinme As New ADODB.Connection
    '    ' cnpinme.ConnectionString = ConnectionStringHelper.GetCASConnectionStringSQL
    '    If cnpinme.State = 1 Then cnpinme.Close()
    '    If cnpinme.State = 0 Then
    '        cnpinme.ConnectionString = ConnectionStringHelper.GetConnectionStringsql
    '        cnpinme.Open()
    '    End If

    '    Dim si As Integer
    '    If IsNumeric(e.CommandArgument.ToString) Then si = CInt(e.CommandArgument.ToString)

    '    If e.CommandName.ToString = "X" Or e.CommandName.ToString = "D" Then
    '        '  S = "Change " &  gridview1.Rows.Item(si).Cells(3).Text & " priority to "  & newpri
    '        Dim rs As New ADODB.Recordset



    '        If rs.State = 1 Then rs.Close()

    '        Dim s As String

    '        Dim req As String

    '        Dim newsa As String
    '        If e.CommandName.ToString = "X" Then
    '            newsa = "N"
    '            '  s = "Don't Push " & GridView1.Rows.Item(si).Cells(4).Text
    '        End If

    '        If e.CommandName.ToString = "D" Then
    '            newsa = "Y"
    '            '   s = "Push " & GridView1.Rows.Item(si).Cells(4).Text
    '        End If


    '        '  GridView1.Rows.Item(si).Cells(9).Text = newsa



    '        'id
    '        'TripNumber()
    '        'AirportFrom()
    '        'AirportTo()
    '        'PinnedOn()
    '        'Pinned()


    '        req = "select * from pinned where TripNumber = 'abc' and AirportFrom = 'def' and  AirportTo = 'ghi' and carrierid = 'jkl' "
    '        req = Replace(req, "abc", Trim(GridViewTrips.Rows.Item(si).Cells(12).Text))
    '        req = Replace(req, "def", Trim(GridViewTrips.Rows.Item(si).Cells(0).Text))
    '        req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(si).Cells(1).Text))
    '        req = Replace(req, "jkl", Session("carrierid"))


    '        If rs.State = 1 Then rs.Close()
    '        rs.Open(req, cnpinme, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '        ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


    '        If rs.EOF Then
    '            rs.AddNew()
    '            rs.Fields("TripNumber").Value = Trim(GridViewTrips.Rows.Item(si).Cells(12).Text)
    '            rs.Fields("AirportFrom").Value = Trim(GridViewTrips.Rows.Item(si).Cells(0).Text)
    '            rs.Fields("AirportTo").Value = Trim(GridViewTrips.Rows.Item(si).Cells(1).Text)
    '            rs.Fields("PinnedOn").Value = Date.UtcNow
    '            rs.Fields("Pinned").Value = True
    '            rs.Fields("Carrierid").Value = Session("carrierid")
    '            GridViewTrips.Rows.Item(si).Cells(2).BackColor = Drawing.Color.Goldenrod
    '            GridViewTrips.Rows.Item(si).Cells(13).BackColor = Drawing.Color.Goldenrod
    '        Else
    '            rs.Fields("Pinned").Value = Not (rs.Fields("Pinned").Value)
    '            rs.Fields("PinnedOn").Value = Date.UtcNow
    '            GridViewTrips.Rows.Item(si).Cells(2).BackColor = Drawing.Color.White
    '            GridViewTrips.Rows.Item(si).Cells(13).BackColor = Drawing.Color.White
    '        End If
    '        rs.Update()

    '        '    chkNewOnly_CheckedChanged(Nothing, nothing)

    '        '  SqlDataSource2.SelectCommand =  Replace( "select distinct  displaymfg,  category, displaycategory, categorypriority  FROM [Product2] where displaymfg = 'Apple'", "Apple", GridViewPrices.SelectedRow.Cells(2).Text.ToString)

    '        '  SqlDataSource2.DataBind()
    '        'gridview1.DataBind()

    '        If cnpinme.State = 1 Then cnpinme.Close()
    '        If rs.State = 1 Then rs.Close()



    '        Exit Sub
    '    End If



    '    If e.CommandName.ToString = "A" Then

    '        Dim rs As New ADODB.Recordset



    '        If rs.State = 1 Then rs.Close()

    '        Dim s As String

    '        Dim req As String


    '        Dim ac As String = GridViewTrips.Rows.Item(si).Cells(8).Text


    '        req = "select * from aog where aircraft = 'ghi' and carrierid = 'jkl' "
    '        req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(si).Cells(8).Text))
    '        req = Replace(req, "jkl", Session("carrierid"))


    '        If rs.State = 1 Then rs.Close()
    '        rs.Open(req, cnpinme, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '        ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


    '        If rs.EOF Then
    '            rs.AddNew()


    '            Dim acc As String = Trim(GridViewTrips.Rows.Item(si).Cells(8).Text)
    '            rs.Fields("Aircraft").Value = Trim(GridViewTrips.Rows.Item(si).Cells(8).Text)

    '            rs.Fields("TripNumber").Value = Trim(GridViewTrips.Rows.Item(si).Cells(12).Text)
    '            rs.Fields("AirportFrom").Value = Trim(GridViewTrips.Rows.Item(si).Cells(0).Text)
    '            rs.Fields("AirportTo").Value = Trim(GridViewTrips.Rows.Item(si).Cells(1).Text)
    '            rs.Fields("AOGOn").Value = Date.UtcNow
    '            rs.Fields("AOG").Value = True
    '            rs.Fields("Carrierid").Value = Session("carrierid")

    '            GridViewTrips.Rows.Item(si).Cells(8).BackColor = Drawing.Color.Violet
    '        Else
    '            rs.Fields("AOG").Value = Not (rs.Fields("AOG").Value)
    '            rs.Fields("AOGon").Value = Now

    '            If rs.Fields("AOG").Value = False Then

    '                GridViewTrips.Rows.Item(si).Cells(8).BackColor = Drawing.Color.White
    '            Else

    '                GridViewTrips.Rows.Item(si).Cells(8).BackColor = Drawing.Color.Violet
    '            End If

    '        End If
    '        rs.Update()

    '        '    chkNewOnly_CheckedChanged(Nothing, nothing)

    '        '  SqlDataSource2.SelectCommand =  Replace( "select distinct  displaymfg,  category, displaycategory, categorypriority  FROM [Product2] where displaymfg = 'Apple'", "Apple", GridViewPrices.SelectedRow.Cells(2).Text.ToString)

    '        '  SqlDataSource2.DataBind()
    '        'gridview1.DataBind()

    '        If cnpinme.State = 1 Then cnpinme.Close()
    '        If rs.State = 1 Then rs.Close()


    '        colorme()
    '        Exit Sub
    '    End If

    'End Sub


    Protected Sub GridViewTrips_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewTrips.RowCommand


        If IsNothing(Session("carrierid")) Then Exit Sub


        Dim cnpinme As New ADODB.Connection
        ' cnpinme.ConnectionString = ConnectionStringHelper.GetCASConnectionStringSQL
        If cnpinme.State = 1 Then cnpinme.Close()
        If cnpinme.State = 0 Then
            cnpinme.ConnectionString = ConnectionStringHelper.GetConnectionStringsql
            cnpinme.Open()
        End If

        Dim si As Integer
        If IsNumeric(e.CommandArgument.ToString) Then si = CInt(e.CommandArgument.ToString)

        If e.CommandName.ToString = "X" Or e.CommandName.ToString = "D" Then
            '  S = "Change " &  gridview1.Rows.Item(si).Cells(3).Text & " priority to "  & newpri
            Dim rs As New ADODB.Recordset



            If rs.State = 1 Then rs.Close()

            Dim s As String

            Dim req As String

            Dim newsa As String
            If e.CommandName.ToString = "X" Then
                newsa = "N"
                '  s = "Don't Push " & GridView1.Rows.Item(si).Cells(4).Text
            End If

            If e.CommandName.ToString = "D" Then
                newsa = "Y"
                '   s = "Push " & GridView1.Rows.Item(si).Cells(4).Text
            End If


            '  GridView1.Rows.Item(si).Cells(9).Text = newsa



            'id
            'TripNumber()
            'AirportFrom()
            'AirportTo()
            'PinnedOn()
            'Pinned()


            req = "select * from pinned where TripNumber = 'abc' and AirportFrom = 'def' and  AirportTo = 'ghi' and carrierid = 'jkl' "
            req = Replace(req, "abc", Trim(GridViewTrips.Rows.Item(si).Cells(12).Text))
            req = Replace(req, "def", Trim(GridViewTrips.Rows.Item(si).Cells(0).Text))
            req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(si).Cells(1).Text))
            req = Replace(req, "jkl", Session("carrierid"))


            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnpinme, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
            ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


            If rs.EOF Then
                rs.AddNew()
                rs.Fields("TripNumber").Value = Trim(GridViewTrips.Rows.Item(si).Cells(12).Text)
                rs.Fields("AirportFrom").Value = Trim(GridViewTrips.Rows.Item(si).Cells(0).Text)
                rs.Fields("AirportTo").Value = Trim(GridViewTrips.Rows.Item(si).Cells(1).Text)
                rs.Fields("PinnedOn").Value = Date.UtcNow
                rs.Fields("Pinned").Value = True
                rs.Fields("Carrierid").Value = Session("carrierid")
                GridViewTrips.Rows.Item(si).Cells(2).BackColor = Drawing.Color.Goldenrod
                GridViewTrips.Rows.Item(si).Cells(13).BackColor = Drawing.Color.Goldenrod
            Else
                rs.Fields("Pinned").Value = Not (rs.Fields("Pinned").Value)
                rs.Fields("PinnedOn").Value = Date.UtcNow
                GridViewTrips.Rows.Item(si).Cells(2).BackColor = Drawing.Color.White
                GridViewTrips.Rows.Item(si).Cells(13).BackColor = Drawing.Color.White
            End If
            rs.Update()

            '    chkNewOnly_CheckedChanged(Nothing, nothing)

            '  SqlDataSource2.SelectCommand =  Replace( "select distinct  displaymfg,  category, displaycategory, categorypriority  FROM [Product2] where displaymfg = 'Apple'", "Apple", GridViewPrices.SelectedRow.Cells(2).Text.ToString)

            '  SqlDataSource2.DataBind()
            'gridview1.DataBind()

            If cnpinme.State = 1 Then cnpinme.Close()
            If rs.State = 1 Then rs.Close()



            Exit Sub
        End If



        If e.CommandName.ToString = "A" Then

            Dim rs As New ADODB.Recordset



            If rs.State = 1 Then rs.Close()

            Dim s As String

            Dim req As String


            Dim ac As String = GridViewTrips.Rows.Item(si).Cells(8).Text


            req = "select * from aog where aircraft = 'ghi' and carrierid = 'jkl' "
            req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(si).Cells(8).Text))
            req = Replace(req, "jkl", Session("carrierid"))


            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnpinme, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
            ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


            If rs.EOF Then
                rs.AddNew()


                Dim acc As String = Trim(GridViewTrips.Rows.Item(si).Cells(8).Text)
                rs.Fields("Aircraft").Value = Trim(GridViewTrips.Rows.Item(si).Cells(8).Text)

                rs.Fields("TripNumber").Value = Trim(GridViewTrips.Rows.Item(si).Cells(12).Text)
                rs.Fields("AirportFrom").Value = Trim(GridViewTrips.Rows.Item(si).Cells(0).Text)
                rs.Fields("AirportTo").Value = Trim(GridViewTrips.Rows.Item(si).Cells(1).Text)
                rs.Fields("AOGOn").Value = Date.UtcNow
                rs.Fields("AOG").Value = True
                rs.Fields("Carrierid").Value = Session("carrierid")

                GridViewTrips.Rows.Item(si).Cells(8).BackColor = Drawing.Color.Violet
            Else
                rs.Fields("AOG").Value = Not (rs.Fields("AOG").Value)
                rs.Fields("AOGon").Value = Now

                If rs.Fields("AOG").Value = False Then

                    GridViewTrips.Rows.Item(si).Cells(8).BackColor = Drawing.Color.White
                Else

                    GridViewTrips.Rows.Item(si).Cells(8).BackColor = Drawing.Color.Violet
                End If

            End If
            rs.Update()

            '    chkNewOnly_CheckedChanged(Nothing, nothing)

            '  SqlDataSource2.SelectCommand =  Replace( "select distinct  displaymfg,  category, displaycategory, categorypriority  FROM [Product2] where displaymfg = 'Apple'", "Apple", GridViewPrices.SelectedRow.Cells(2).Text.ToString)

            '  SqlDataSource2.DataBind()
            'gridview1.DataBind()

            If cnpinme.State = 1 Then cnpinme.Close()
            If rs.State = 1 Then rs.Close()


            colorme()
            Exit Sub
        End If

    End Sub


End Class
