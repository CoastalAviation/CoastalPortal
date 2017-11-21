Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports CoastalPortal.AirTaxi
Imports CoastalPortal.DataAccess
Imports System.Data
Imports System.Web.Script.Serialization
Imports Telerik.Web.UI
Imports System.IO
Imports System.Drawing
Imports System.Data.SqlClient
Imports System.Configuration
Imports AspNet
Imports StackExchange.Redis
Imports CoastalPortal.Models

Public Class CasFlightsAzure6
    Inherits System.Web.UI.UserControl

    Private SqlDataSourceTrips As New SqlDataSource
    Public Const CAS_FROM As Integer = 0
    Public Const CAS_TO As Integer = 1
    Public Const CAS_FROMGMT As Integer = 2
    Public Const CAS_TOGMT As Integer = 3
    Public Const CAS_FROMLOCAL As Integer = 4
    Public Const CAS_TOLOCAL As Integer = 5
    Public Const CAS_MIN As Integer = 6
    Public Const CAS_NM As Integer = 7
    Public Const CAS_AC As Integer = 8
    Public Const CAS_TYPE As Integer = 9
    Public Const CAS_COST As Integer = 10
    Public Const CAS_FT As Integer = 11
    Public Const CAS_TRIPNUM As Integer = 12
    Public Const CAS_IND As Integer = 13
    Public Const CAS_GRP As Integer = 14
    Public Const CAS_LRC As Integer = 15
    Public Const CAS_LPC As Integer = 16
    Public Const CAS_LTC As Integer = 17
    Public Const CAS_PIC As Integer = 18
    Public Const CAS_SIC As Integer = 19
    Public Const CAS_CDFT As Integer = 20
    Public Const CAS_CDW As Integer = 21
    Public Const CAS_CDC As Integer = 22
    Public Const CAS_REVENUE As Integer = 23
    Public Const CAS_PANDL As Integer = 24
    Public Const CAS_BASE As Integer = 25
    Public Const CAS_WC As Integer = 26
    Public Const CAS_PIN As Integer = 27

    Private db As New OptimizerContext


    '20120724 - fix connection strings
    Public Shared cnmkazure As New ADODB.Connection
    'rk 7.30.2012 point to production

    Public D2DConnectionString As String = ConnectionStringHelper.GetConnectionStringSGServer


    Public ConnectionStringSQL As String = ConnectionStringHelper.GetCASConnectionStringSQL

    Public stest As String
    Public Property calendarcarrierid As String
    Public Property calendarcypher As String
    Private modelrunid As String


    Public ConnectionStringOptimizerSQL As String = ConnectionStringHelper.GetConnectionStringSQLMKAzure


    Private Sub BindDataTrips(sql As String)
        '  Dim sql As String = CStr(Session("sqlQueryTrips"))

        Dim dateFrom As DateTime = Nothing
        Dim dateThru As DateTime = Nothing


        '20110812 - pab - remove hardcoded connection string



        If InStr(UCase(sql), "FROM") = 0 Then
            Exit Sub
        End If



        '20101101 - pab
        GridViewTrips.EmptyDataText = "Your search returned 0 results"
        Dim req As String
        req = sql
        If sql <> "" Then

            GridViewTrips.DataSource = SqlDataSourceTrips

            '   sd1.Dispose()
            'GridView1.SelectedIndex = Nothing
            GridViewTrips.Dispose()

            SqlDataSourceTrips.Dispose()

            SqlDataSourceTrips.SelectCommand = sql

            SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString


            'GetConnectio.nString
            'sd1.ConnectionString = "Data Source=(local);Initial Catalog=Production;Persist Security Info=True;User ID=sa;Password=CoastalPass1"
            '    sd1.ConnectionString = ConnectionStringHelper.GetConnectionString()


            '   <add name="ConnectString" connectionString="Data Source=(local);Initial Catalog=Production;Persist Security Info=True;User ID=sa;Password=CoastalPass1"/>



            GridViewTrips.AutoGenerateColumns = False

            GridViewTrips.DataSource = SqlDataSourceTrips

            SqlDataSourceTrips.DataBind()

            GridViewTrips.DataBind()

        Else
            SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString

            SqlDataSourceTrips.DataBind()
            GridViewTrips.DataBind()
        End If

        'GridViewTrips.DataBind()

        colorflighttype()


        'grabcounts()
        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx Start Pull From CAS LOG " & Now & " -1348 - ", "", "")
        pullfromcaslog(modelrunid)
        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx End Load From CAS Log " & Now & " -1348 - ", "", "")

        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx Start Load Drop Downs " & Now & " -1348 - ", "", "")
        loaddropdowns()
        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx End Load Drop Downs " & Now & " -1348 - ", "", "")



    End Sub


    Function colorme()

        caslinebreaks = 0

        Dim mycolor As System.Drawing.Color
        mycolor = Drawing.Color.Aquamarine


        Dim account As Integer = 0
        Dim req As String

        '20120815 - pab - run time improvements
        Dim p1 As Date = CDate(Date.UtcNow.Month & "/" & Date.UtcNow.Day & "/" & Date.UtcNow.Year & " 10:00 AM")
        req = "select id, TripNumber, ltrim(rtrim(AirportFrom)) as AirportFrom, ltrim(rtrim(AirportTo)) as AirportTo, PinnedOn, Pinned from pinned " &
            "where pinned = 1 and carrierid = 'gef' and pinnedon > '" & p1 & "' order by tripnumber"
        req = Replace(req, "gef", Session("carrierid"))

        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx colorme start Get Pinnned " & Now, "", "")
        Dim dt As DataTable = DataAccess.GetPinnedFlights(req)
        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx colorme end Get Pinned " & Now, "", "")

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))


        Dim dv_pinned As DataView = dt.DefaultView
        Dim bpinnedflights As Boolean = False

        If Not AirTaxi.isdtnullorempty(dt) Then
            bpinnedflights = True
        End If


        If cnsetting.State = 1 Then cnsetting.Close()
        If cnsetting.State = 0 Then
            cnsetting.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            cnsetting.Open()
        End If



        'Add fro Flight Change
        '  Dim colorsArray As System.Array =
        ' [Enum].GetValues(GetType(KnownColor))
        'Dim allColors(colorsArray.Length) As KnownColor

        'Array.Copy(colorsArray, allColors, colorsArray.Length)



        'If ACX(1) <> "" Then
        '    GridViewTrips.Width = 1000
        '    TopStats.Style.Clear()
        '    TopStats.Style.Add("visible", "false")
        '    TopStats.Visible = False
        'Else
        GridViewTrips.Width = 1000
        TopStats.Style.Clear()
        'End If

        Dim i As Integer
        For i = 0 To GridViewTrips.Rows.Count - 1
            '   Dim dccolor As Drawing.Color = Drawing.Color.White

            Dim a13, a8 As String
            a13 = GridViewTrips.Rows(i).Cells(14).Text
            a8 = GridViewTrips.Rows(i).Cells(8).Text


            Dim lb As LinkButton
            lb = GridViewTrips.Rows(i).FindControl("lnkDepartmentClientSide")
            GridViewTrips.Rows(i).Cells(14).BackColor = Drawing.Color.White

            If Not (IsNothing(lb)) Then
                If Trim(lb.Text) = Trim(GridViewTrips.Rows(i).Cells(8).Text) Then
                    GridViewTrips.Rows(i).Cells(14).Text = ""
                    GridViewTrips.Rows(i).Cells(15).Text = ""

                    lb.Text = ""
                    lb.BackColor = Drawing.Color.Beige

                End If
            End If



            If Trim(GridViewTrips.Rows(i).Cells(14).Text) <> "" Then
                GridViewTrips.Rows(i).Cells(14).BackColor = Drawing.Color.Pink

                Dim dd As String = GridViewTrips.Rows(i).Cells(2).Text
                Dim dd1 As String = GridViewTrips.Rows(i).Cells(2).Text
                Dim ispace As Integer
                ispace = InStr(dd, " ")

                If Not (IsDate(dd)) Then
                    dd = Trim(Left(dd, ispace)) & "/" & Now.Year & " " & Right(dd, Len(dd) - ispace)
                End If


                If Not (IsDate(dd)) Then
                    dd = dd
                End If


                If IsDate(dd) Then
                    Dim priordate As Date = CDate(dd)
                    Dim dC As Integer = DateDiff(DateInterval.Day, Now.ToUniversalTime, priordate)


                    lb.Font.Bold = True

                    If dC > 3 Then GridViewTrips.Rows(i).Cells(14).BackColor = Drawing.Color.LightBlue
                    If dC = 3 Then GridViewTrips.Rows(i).Cells(14).BackColor = Drawing.Color.SlateBlue
                    If dC = 2 Then GridViewTrips.Rows(i).Cells(14).BackColor = Drawing.Color.SkyBlue
                    If dC = 1 Then GridViewTrips.Rows(i).Cells(14).BackColor = Drawing.Color.DeepSkyBlue
                    If dC < 1 Then GridViewTrips.Rows(i).Cells(14).BackColor = Drawing.Color.DodgerBlue

                    If dC > 3 Then lb.BackColor = Drawing.Color.LightBlue
                    If dC = 3 Then lb.BackColor = Drawing.Color.SlateBlue
                    If dC = 2 Then lb.BackColor = Drawing.Color.SkyBlue
                    If dC = 1 Then lb.BackColor = Drawing.Color.DeepSkyBlue
                    If dC < 1 Then lb.BackColor = Drawing.Color.DodgerBlue

                End If

            End If
            Dim lrc As String = GridViewTrips.Rows(i).Cells(15).Text
            Dim lpc As String = GridViewTrips.Rows(i).Cells(16).Text
            Dim ltc As String = Trim(GridViewTrips.Rows(i).Cells(17).Text)

            'Dim xy As String = GridViewTrips.Rows(i).Cells(17).Text
            'Dim x As String = GridViewTrips.Rows(i).Cells(17).Text
            Select Case Left(GridViewTrips.Rows(i).Cells(17).Text, 1)
                Case "D", "P", "True"

                    If ltc = "DETL" Then
                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        GridViewTrips.Rows(i).Cells(11).Text = "M"
                    Else
                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat
                        GridViewTrips.Rows(i).Cells(11).Text = "D"
                    End If
                Case "R", "False"
                    'revenue
                    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkSeaGreen
                    GridViewTrips.Rows(i).Cells(11).Text = "R"
                Case "M", "A", "I", "S", "7"
                    If Left(ltc, 1) = "7" And ltc <> "77" Then Exit Select
                    If ltc = "INFO" Then
                        'GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
                        ' GridViewTrips.Rows(i).Cells(11).Text = "D"
                    ElseIf ltc = "MEM" Or ltc = "MX-S" Then
                        'GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkSeaGreen
                        'GridViewTrips.Rows(i).Cells(11).Text = "R"
                    Else
                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        GridViewTrips.Rows(i).Cells(11).Text = "M"
                    End If
                Case "C", "B", "T"
                    If ltc = "COST" Then
                        ' GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
                        ' GridViewTrips.Rows(i).Cells(11).Text = "D"
                    ElseIf CStr("TRM,CHTR").Contains(ltc) Then
                        'GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkSeaGreen
                        ' GridViewTrips.Rows(i).Cells(11).Text = "R"
                    Else

                        '  If GridViewTrips.Rows(i).Cells(17).Text = "CREW" Then
                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki
                        GridViewTrips.Rows(i).Cells(11).Text = Left(ltc, 1)
                    End If

                    'If GridViewTrips.Rows(i).Cells(17).Text = "P" Then
                    '    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
                    '    GridViewTrips.Rows(i).Cells(11).Text = "D"
                    'End If

                Case "Y"
                    'If GridViewTrips.Rows(i).Cells(17).Text = "Y" Then
                    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.Orange
                    GridViewTrips.Rows(i).Cells(11).Text = "S"
                    GridViewTrips.Rows(i).Cells(11).ToolTip = "Static"
                    'End If

                Case Else
                    If Trim(GridViewTrips.Rows(i).Cells(12).Text) = "12345" Then
                        GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkOrange
                        GridViewTrips.Rows(i).Cells(11).Text = "M"
                    Else
                        ' GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                    End If
            End Select

            'If GridViewTrips.Rows(i).Cells(11).Text = "" Then
            '    Dim dc = 0
            'End If
            'GridViewTrips.Rows(i).Cells(10).ToolTip = lrc
            'GridViewTrips.Rows(i).Cells(9).ToolTip = lpc
            'GridViewTrips.Rows(i).Cells(11).ToolTip = ltc

            'If CStr("MXSC, AOG, M, DETL, INSV, MXRC, SWAP, MXUS, MAINT").Contains(ltc) Then 'SWAP added by mws 11/25
            '    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
            '    GridViewTrips.Rows(i).Cells(11).Text = "M"
            'End If

            'If ltc = "77" Then
            '    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
            '    GridViewTrips.Rows(i).Cells(11).Text = "M"
            'End If

            ''rk 7/20/2013
            'If ltc = "BULP" Then
            '    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki
            '    GridViewTrips.Rows(i).Cells(11).Text = "B"
            'End If


            ''rk 7/20/2013
            'If ltc = "CREW" Then
            '    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki
            '    GridViewTrips.Rows(i).Cells(11).Text = "C"
            'End If


            'If ltc = "TRNG" Then
            '    GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki
            '    GridViewTrips.Rows(i).Cells(11).Text = "T"
            'End If


            '    End If
            'End If



            If GridViewTrips.Rows(i).Cells(11).Text = "D" Then GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow

            ' If ACX(1) <> "" Then mycolor = Drawing.Color.White            'Flight Change 


            If i > 0 Then


                GridViewTrips.Rows(i).Cells(8).Font.Bold = False
                If GridViewTrips.Rows(i).Cells(8).Text <> GridViewTrips.Rows(i - 1).Cells(8).Text Then 'ac change
                    '   GridViewTrips.Rows(i).Cells(8).BackColor = Drawing.Color.CadetBlue

                    GridViewTrips.Rows(i).Cells(8).Font.Bold = True


                    account = account + 1
                    'If ACX(1) = "" Then ' added for Flight Change just this if
                    '    If account / 2 = Int(account / 2) Then
                    '        'GridViewTrips.Rows(i).Style.Add("BackColor", "#669999")
                    '        mycolor = Drawing.Color.Aquamarine
                    '    Else
                    '        'GridViewTrips.Rows(i).Style.Remove("BackColor")
                    '        'GridViewTrips.Rows(i).Style.Add("BackColor", "#000066")
                    '        mycolor = Drawing.Color.White
                    '    End If
                    'End If
                    'If ACX(1) <> "" Then mycolor = Drawing.Color.White 'Flight Change


                Else




                    If GridViewTrips.Rows(i).Cells(0).Text <> GridViewTrips.Rows(i - 1).Cells(1).Text Then

                        If GridViewTrips.Rows(i).Cells(0).Text <> "SWAP" And GridViewTrips.Rows(i - 1).Cells(1).Text <> "SWAP" Then
                            GridViewTrips.Rows(i).Cells(0).BackColor = Drawing.Color.IndianRed
                            GridViewTrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.IndianRed
                            caslinebreaks = caslinebreaks + 1
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
                '  GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.White
                '      GridViewTrips.Rows.Item(i).Cells(14).BackColor = Drawing.Color.White
            Else
                GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.Goldenrod
                '        GridViewTrips.Rows.Item(i).Cells(14).BackColor = Drawing.Color.Goldenrod
            End If

            If IsDate(GridViewTrips.Rows(i).Cells(2).Text) Then

                Dim gmt As Date = DateTime.UtcNow
                Dim departgmt As Date = CDate(GridViewTrips.Rows(i).Cells(2).Text)
                If gmt > DateAdd(DateInterval.Minute, 180, departgmt) Then
                    'GridViewTrips.Rows(i).Cells(2).BackColor = Drawing.Color.Teal
                    'GridViewTrips.Rows(i).Cells(4).BackColor = Drawing.Color.Teal
                End If


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
                GridViewTrips.Rows.Item(i).Cells(14).Text = b
                GridViewTrips.Rows.Item(i).Cells(15).Text = b

            End If

        Next i


        Me.lblLineBreaks.Text = caslinebreaks


        If caslinebreaks <> 0 Then
            Me.lblLineBreaks.BackColor = Drawing.Color.Red
        Else
            Me.lblLineBreaks.BackColor = Drawing.Color.White
        End If



        Dim movedhemail As String = ""
        Dim endofgrid As Integer = GridViewTrips.Rows.Count - 1
        Dim getnewcwpair As Boolean = True 'Flags when a crew warning pair needs updating  mws 12/5/16
        Dim cwdepartureport As String 'crew warning message departure airport mws 12/5/16
        Dim cwarrivalport As String 'crew warning message arrival airport mws 12/5/16
        Dim Swapflightfail As Boolean = False
        Dim swapevent As Boolean = False
        Dim swapwarntext As String
        Dim swapflightpreptime As Integer


        Dim ii As Integer
        Dim startofdutyflight As Integer = 0

        For ii = 0 To GridViewTrips.Rows.Count - 1

            Dim a13, a8 As String
            a13 = GridViewTrips.Rows(ii).Cells(14).Text
            a8 = GridViewTrips.Rows(ii).Cells(8).Text



            Dim departuretime As Date
            Dim arrivaltime As Date
            Dim startofduty As Date

            Dim departureairport As String
            Dim arrivalairport As String

            Dim departuretimelocal As Date
            Dim arrivaltimelocal As Date


            Dim dutyday As Double
            Dim Swapoffset As Double



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
            Dim t11, t16 As String
            t11 = GridViewTrips.Rows(ii).Cells(11).Text
            t16 = GridViewTrips.Rows(ii).Cells(17).Text

            'reset counter if MX event, assume crew change or rest achieved.
            If ii <> endofgrid Then
                If GridViewTrips.Rows(ii).Cells(11).Text = "M" Or GridViewTrips.Rows(ii).Cells(17).Text = "MXSC" Or GridViewTrips.Rows(ii).Cells(17).Text = "SWAP" Or GridViewTrips.Rows(ii).Cells(17).Text = "NC" Then 'SWAP and NC added by mws 11/25
                    If GridViewTrips.Rows(ii).Cells(17).Text = "SWAP" Then
                        startofduty = addyear(GridViewTrips.Rows(ii).Cells(3).Text) 'added by mws 11/27/2016
                        Swapoffset = DateDiff(DateInterval.Minute, addyear(GridViewTrips.Rows(ii).Cells(3).Text), addyear(GridViewTrips.Rows(ii).Cells(2).Text))
                        If Swapoffset < 7 Then
                            Swapoffset = 3
                        ElseIf Swapoffset < 10 Then
                            Swapoffset = 2
                        ElseIf Swapoffset < 15 Then
                            Swapoffset = 1
                        Else
                            Swapoffset = 0
                        End If
                        If GridViewTrips.Rows(ii).Cells(8).Text = GridViewTrips.Rows(ii + 1).Cells(8).Text Then
                            Swapflightfail = False
                            swapflightpreptime = DateDiff(DateInterval.Minute, addyear(GridViewTrips.Rows(ii).Cells(3).Text), addyear(GridViewTrips.Rows(ii + 1).Cells(2).Text))
                            If swapflightpreptime < 60 Then Swapflightfail = True
                            swapevent = True
                        End If
                    Else
                        swapevent = False
                        Swapoffset = 0
                        Swapflightfail = False
                        startofduty = addyear(GridViewTrips.Rows(ii + 1).Cells(2).Text)
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty)  'added by mws 11/27/16 .. all crew need to show 1 hour early
                    End If
                    startofdutyflight = ii + 1
                    getnewcwpair = True
                    If Left(GridViewTrips.Rows(ii + 1).Cells(9).Text, 4) = "B350" Then
                        If swapevent Then
                            If swapflightpreptime < 90 Then Swapflightfail = True
                        Else
                            startofduty = DateAdd(DateInterval.Minute, -30, startofduty)  'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                        End If
                    End If

                End If
            End If



            If ii > 0 Then
                If departureairport <> arrivalairport Then
                    If GridViewTrips.Rows(ii).Cells(8).Text <> GridViewTrips.Rows(ii - 1).Cells(8).Text Then 'ac change

                        startofduty = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
                        Dim t As String = GridViewTrips.Rows(ii).Cells(9).Text
                        startofdutyflight = ii
                        swapevent = False
                        getnewcwpair = True
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty) 'modified by mws 11/27/16 all show is 60 minutes
                        If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
                            startofduty = DateAdd(DateInterval.Minute, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                            'Else
                            ' startofduty = DateAdd(DateInterval.Hour, -1, startofduty)
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
                                                startofduty = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
                                                getnewcwpair = True
                                                swapevent = False

                                                'Dim t As String = GridViewTrips.Rows(ii).Cells(9).Text
                                                startofdutyflight = ii
                                                startofduty = DateAdd(DateInterval.Hour, -1, startofduty) ' modified by mws 11/27/16 all crew need 60 minute show
                                                If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
                                                    startofduty = DateAdd(DateInterval.Hour, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                                                    'Else
                                                    '   startofduty = DateAdd(DateInterval.Hour, -1, startofduty)
                                                End If


                                            End If
                                        End If

                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If



            i2 = i2 + 1


            departuretime = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
            arrivaltime = addyear(GridViewTrips.Rows(ii).Cells(3).Text)

            'departuretimelocal = GridViewTrips.Rows(ii).Cells(4).Text
            'arrivaltimelocal = GridViewTrips.Rows(ii).Cells(5).Text

            Dim ri As Integer = 10
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
                        startofduty = addyear(GridViewTrips.Rows(ii).Cells(2).Text)
                        getnewcwpair = True
                        swapevent = False
                        startofdutyflight = ii
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty) ' modified by mws 11/27/16 all crew need 60 minute show
                        If Left(GridViewTrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
                            startofduty = DateAdd(DateInterval.Minute, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                            'Else
                            '   startofduty = DateAdd(DateInterval.Hour, -1, startofduty)
                        End If
                    End If


                    dutyday = DateDiff(DateInterval.Minute, startofduty, arrivaltime) + 30
                    dutyday = dutyday / 60
                    dutyday = dutyday

                    If dutyday > 11 Then
                        fail = True
                        GridViewTrips.Rows(ii).Cells(18).BackColor = Drawing.Color.GreenYellow
                        GridViewTrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Warning: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If


                    If dutyday > 12 Then
                        fail = True
                        GridViewTrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Yellow
                        GridViewTrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Concern: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If



                    If dutyday > 13 Then
                        fail = True
                        GridViewTrips.Rows(ii).Cells(18).BackColor = Drawing.Color.YellowGreen
                        GridViewTrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Alert: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If


                    If dutyday > 14 Or (dutyday > (7 + Swapoffset) And swapevent = True) Or (Swapflightfail) Then
                        GridViewTrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Red
                        swapwarntext = ""
                        If Swapflightfail Then
                            swapwarntext = swapflightpreptime & " Minutes is Not enough time to prep flight after SWAP"
                            GridViewTrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Orange
                        End If
                        If swapevent And Not Swapflightfail Then
                            swapwarntext = "SWAP Event"
                            swapevent = False
                        End If
                        fail = True
                        If Swapflightfail Then
                            GridViewTrips.Rows(ii).Cells(18).ToolTip = "Flight Prep Alert:  " & departureairport & " " & arrivalairport & " " & swapwarntext
                        Else
                            GridViewTrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Alert: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport & " " & swapwarntext
                        End If
                        Swapflightfail = False
                        Dim sf As Integer = startofdutyflight

                        Dim dd As Integer = DateDiff(DateInterval.Day, startofduty, Now.ToUniversalTime)
                        If dd > -1 And dd < 1 Then
                            Dim ft As String = Left(GridViewTrips.Rows(sf).Cells(11).Text, 1)
                            If Left(GridViewTrips.Rows(sf).Cells(11).Text, 1) = "D" Then
                                movedhemail = movedhemail & "Preposition " & GridViewTrips.Rows(sf).Cells(8).Text & " : " & GridViewTrips.Rows(sf).Cells(0).Text & "-" & GridViewTrips.Rows(sf).Cells(1).Text & "..." & vbNewLine
                                cmdSlideReport.ToolTip = movedhemail
                            End If
                        End If


                    End If
                End If

            End If

            If departureairport <> arrivalairport Then 'rk 10/12/13 only count prior arrival if it is really a flight
                prevarrival = addyear(GridViewTrips.Rows(ii).Cells(3).Text)
            End If

            '   GridViewTrips.Rows(ii).Cells(19).BorderStyle = BorderStyle.None
            GridViewTrips.Rows(ii).Cells(19).ToolTip = GridViewTrips.Rows(ii).Cells(20).Text & " " & GridViewTrips.Rows(ii).Cells(21).Text & " " & GridViewTrips.Rows(ii).Cells(22).Text
            If InStr("Failed", GridViewTrips.Rows(ii).Cells(19).ToolTip) <> 0 Then
                GridViewTrips.Rows(ii).Cells(19).BackColor = Drawing.Color.Red
                ' GridViewTrips.Rows(ii).Cells(19).BorderStyle = BorderStyle.Dashed
            End If

        Next ii


        Dim d, r As Double


        For i = 0 To GridViewTrips.Rows.Count - 1
            Dim ltc = Trim(GridViewTrips.Rows(i).Cells(CAS_LTC).Text)
            If Left(GridViewTrips.Rows(i).Cells(11).Text, 1) = "D" Or CStr("D,P,INFO,COST").Contains(ltc) Then
                d = d + GridViewTrips.Rows(i).Cells(7).Text
            End If

            If Left(GridViewTrips.Rows(i).Cells(11).Text, 1) = "R" Or CStr("OWNR,TRM,MEM,CHTR").Contains(ltc) Then
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
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                GridViewTrips.Rows(i).Cells(8).ToolTip = AirTaxi.lookupac(GridViewTrips.Rows(i).Cells(8).Text, carrierid)
            End If
        Next i




        If GridViewTrips.Rows.Count > 5 Then

            linemeup()
            colorflighttype()
        End If
        'If ACX(1) <> "" Then
        '    For z = 13 To GridViewTrips.Columns.Count - 1
        '        GridViewTrips.Columns(z).Visible = False
        '    Next z
        'End If

        If carrierid = WHEELSUP Then
            GridViewTrips.Columns(CAS_PANDL).Visible = True
            GridViewTrips.Columns(CAS_REVENUE).Visible = True
        ElseIf carrierid = JETLINX Then
            GridViewTrips.Columns(CAS_WC).Visible = True
            GridViewTrips.Columns(CAS_BASE).Visible = True
        Else
            GridViewTrips.Columns(CAS_WC).Visible = True
            GridViewTrips.Columns(CAS_BASE).Visible = True
        End If


    End Function


    Function colormebak()

        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx colorme start " & Now, "", "")

        Dim req As String

        '20120815 - pab - run time improvements
        Dim p1 As Date = CDate(Date.UtcNow.Month & "/" & Date.UtcNow.Day & "/" & Date.UtcNow.Year & " 10:00 AM")
        If cnsetting.State = 1 Then cnsetting.Close()
        If cnsetting.State = 0 Then
            cnsetting.ConnectionString = ConnectionStringHelper.ReadOnlyServerConnectionString
            cnsetting.Open()
        End If

        'req = "select * from pinned where TripNumber = 'abc' and AirportFrom = 'def' and  AirportTo = 'ghi' "
        'req = Replace(req, "abc", Trim(GridViewTrips.Rows.Item(i).Cells(10).Text))
        'req = Replace(req, "def", Trim(GridViewTrips.Rows.Item(i).Cells(0).Text))
        'req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(i).Cells(1).Text))
        req = "select id, TripNumber, ltrim(rtrim(AirportFrom)) as AirportFrom, ltrim(rtrim(AirportTo)) as AirportTo, PinnedOn, Pinned from pinned " &
            "where pinned = 1 and pinnedon > '" & p1 & "' order by tripnumber"

        Dim rs As New ADODB.Recordset
        If rs.State = 1 Then rs.Close()
        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


        Dim i As Integer
        For i = 0 To GridViewTrips.Rows.Count - 1


            GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.White

            Select Case Left(GridViewTrips.Rows(i).Cells(9).Text, 1)
                Case "D"
                    GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat
                Case "R"
                    GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.DarkSeaGreen
                Case Else
                    GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.SteelBlue
                    If Trim(GridViewTrips.Rows(i).Cells(10).Text) = "12345" Then
                        GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.DarkOrange
                        GridViewTrips.Rows(i).Cells(9).Text = "M"
                    ElseIf Trim(GridViewTrips.Rows(i).Cells(10).Text) = "67890" Then
                        GridViewTrips.Rows(i).Cells(9).BackColor = Drawing.Color.DarkKhaki
                        GridViewTrips.Rows(i).Cells(9).Text = "B"


                    End If
            End Select


            If i > 0 Then
                If GridViewTrips.Rows(i).Cells(6).Text <> GridViewTrips.Rows(i - 1).Cells(6).Text Then 'ac change
                    GridViewTrips.Rows(i).Cells(6).BackColor = Drawing.Color.CadetBlue
                Else
                    If GridViewTrips.Rows(i).Cells(0).Text <> GridViewTrips.Rows(i - 1).Cells(1).Text Then
                        GridViewTrips.Rows(i).Cells(0).BackColor = Drawing.Color.IndianRed
                        GridViewTrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.IndianRed
                    End If


                End If
            End If

            'Insertsys_log(session("carrierid"), appName, "CASFlightsAzure.ascx colorme pinned lookup start " & Now, "", "")

            Dim pinned As Boolean = False

            Do While Not rs.EOF
                If rs.Fields("TripNumber").Value = CInt(GridViewTrips.Rows.Item(i).Cells(10).Text) And
                        rs.Fields("AirportFrom").Value.ToString = GridViewTrips.Rows.Item(i).Cells(0).Text.ToString And
                        rs.Fields("AirportTo").Value.ToString = GridViewTrips.Rows.Item(i).Cells(1).Text.ToString Then
                    pinned = True
                    rs.MoveFirst()
                    Exit Do
                End If
                If rs.Fields("TripNumber").Value > CInt(GridViewTrips.Rows.Item(i).Cells(10).Text) Then
                    rs.MoveFirst()
                    Exit Do
                End If
                rs.MoveNext()
                If rs.EOF Then
                    rs.MoveFirst()
                    Exit Do
                End If
            Loop

            'Insertsys_log(session("carrierid"), appName, "CASFlightsAzure.ascx colorme pinned lookup end " & Now, "", "")

            If pinned = False Then

                GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.White
                ' GridViewTrips.Rows.Item(i).Cells(11).BackColor = Drawing.Color.White
            Else
                GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.Goldenrod
                GridViewTrips.Rows.Item(i).Cells(11).BackColor = Drawing.Color.Goldenrod
            End If



            If IsDate(GridViewTrips.Rows(i).Cells(2).Text) Then

                Dim gmt As Date = DateTime.UtcNow
                Dim departgmt As Date = CDate(GridViewTrips.Rows(i).Cells(2).Text)
                If gmt > DateAdd(DateInterval.Minute, 180, departgmt) Then GridViewTrips.Rows(i).Cells(2).BackColor = Drawing.Color.Teal

            End If

        Next i

        If rs.State = 1 Then rs.Close()

        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx colorme end " & Now, "", "")

    End Function


    Public Sub counts()
        Dim rs As New ADODB.Recordset
        Dim req As String

        req = "SELECT sum(CONVERT(integer, nauticalmiles)) as nauticalmiles   FROM [tmcoptimizerworking].[dbo].[FOSFlights] where DeadHead = 'True'"


        If cnsetting.State = 0 Then
            '20120724 - fix connection strings
            cnsetting.ConnectionString = ConnectionStringHelper.ReadOnlyServerConnectionString
            'cnsetting.ConnectionString = ConnectionStringSQL
            cnsetting.Open()
        End If


        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


        If rs.State = 1 Then rs.Close()

        'me.txtCustomer.Text = rs.Fields ("

    End Sub

    Private Sub GridViewTrips_DataBound(sender As Object, e As EventArgs) Handles GridViewTrips.DataBound


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


        colorflighttype()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Session("carrierid") = Session("carrierid")

        If IsNothing(Session("carrierid")) Then Exit Sub

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("cascalendarmodelid")) Then Session("cascalendarmodelid") = ""
        Dim cascalendarmodelid As String = Session("cascalendarmodelid")

        Dim ws As New coastalavtech.service.WebService1



        calendarcypher = ws.createcypher(Session("defaultemail"))
        calendarcarrierid = Session("carrierid")
        'calendarcypher = "Testcypher"
        RBcalendar.NavigateUrl = "http://169.47.243.167/FlightPlanningOpt.aspx?i="
        RBcalendar.NavigateUrl &= calendarcarrierid & "&c=" & calendarcypher


        '  If Session("carrierid") = "" Then Exit Sub


        SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString()


        If overridemodel <> "" Then modelrunid = overridemodel

        Try


            If modelrunid = "" Then
                If Not Request.QueryString("modelrunid") Is Nothing Then
                    modelrunid = Request.QueryString("modelrunid")
                    cascalendarmodelid = modelrunid
                    Session("AcceptModelID") = cascalendarmodelid
                End If
            End If


            ' If modelrunid = "" Then Exit Sub

            If Not (IsNothing(Session("airportsearchcode"))) Then
                If Session("airportsearchcode").ToString.Trim <> "" Then
                    'If Me.txtTripName.Text.Trim = "" Then
                    '    Me.txtTripName.Text = Session("airportsearchcode")
                    'End If
                End If
            End If





            If Not Page.IsPostBack Then
                '   Me.txtAirportCode.Text = txtairportcode.text

                If Session("AcceptAll") <> True Then
                    radbtnAcceptAll.Enabled = False
                End If


                cmdFindTrip_Click(Nothing, Nothing)

                getrunstatus()



                Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx Start ColorME " & Now & " - ", "", "")
                colorme()
                Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx End ColorME " & Now & " -k - ", "", "")


            End If



            Me.LinkPending.ForeColor = Drawing.Color.Wheat
            'Me.LinkBooked.ForeColor = Drawing.Color.Green
            'Me.LinkQuoted.ForeColor = Drawing.Color.DarkSeaGreen
            'Me.LinkFlown.ForeColor = Drawing.Color.Teal
            'Me.LinkInProgress.ForeColor = Drawing.Color.IndianRed
            'Me.LinkDispatched.ForeColor = Drawing.Color.GreenYellow



        Catch ex As Exception

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx Load Failure " & ex.Message & "  ", "", "")

        End Try


        '   colorme()
        '   GridViewTrips.Rows(2).Cells(2).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat


    End Sub

    Public Sub getrunstatus()

        ' Dim cnstatus As New ADODB.Connection
        ' Dim rsstatus As New ADODB.Recordset
        Dim model_tasks As New modelstatus
        Dim statusSQL As String
        Dim CustomRun As String
        Dim modelcomplete As Boolean = 0
        Dim logcount As Integer
        Dim modelpctcomplete As Integer
        Dim modelsrunsofar As Integer
        Dim EngineVersion As String
        Dim FlightsinDemand As Integer
        Dim OptimizerLogitems As New List(Of optimizerLog)


        CustomRun = Left(modelrunid, InStr(modelrunid, "-") - 1)


        'If cnstatus.State = 0 Then
        '    cnstatus.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
        '    cnstatus.Open()
        'End If

        statusSQL = "select declaredcomplete from OptimizerRequest where ID = '" & CustomRun & "'"
        modelcomplete = db.Database.SqlQuery(Of Boolean)(statusSQL).FirstOrDefault()
        '     rsstatus.Open(statusSQL, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        'If Not rsstatus.EOF Then
        '    modelcomplete = rsstatus.Fields(0).Value
        'End If
        'If cnstatus.State = 1 Then cnstatus.Close()
        'rsstatus.Close()

        'OptimizerLogitems = db.Database.SqlQuery(Of optimizerLog)("select id, carrierid,modelrunid,CustomRunNumber,OptimizerEngineVersion,FlightsInDemand from OptimizerLog where CustomRunNumber = 10727").ToList()
        OptimizerLogitems = db.OptimizerLog.Where(Function(a) a.customrunnumber = CustomRun).ToList()

        modelsrunsofar = OptimizerLogitems.Count
        FlightsinDemand = OptimizerLogitems.Where(Function(x) Trim(x.modelrunid) = Trim(modelrunid)).Select(Function(y) y.flightsindemand).FirstOrDefault()
        EngineVersion = OptimizerLogitems.Where(Function(x) Trim(x.modelrunid) = Trim(modelrunid)).Select(Function(y) y.optimizerengineversion).FirstOrDefault()

        lblFlightsinDemand.Text = "Flights in Demand: " & FlightsinDemand.ToString()
        lblFlightsinDemand.ToolTip = "Optimizer Engine Version: " & EngineVersion

        If modelcomplete Then
            modelstatusguage.Pointer.Value = 100
            modelstatusguage.Pointer.Color = Color.LightGreen
            modelstatusguage.Scale.Ranges(0).Color = Color.LightGreen
            modelstatusguage.ToolTip = "Model Complete"
        Else
            statusSQL = "select count(*) from sys_log where CustomRunNumber = '" & CustomRun & "'"
            ' rsstatus.Open(statusSQL, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            logcount = db.Database.SqlQuery(Of Integer)(statusSQL).FirstOrDefault()
            ' rsstatus.Close()

            ' Try
            'statusSQL = "select (cast(count(case when [Status]  = 'Z' then 1 end) as decimal(6,3)) As tasks / count( case when [Status] <> '' then 1 end))*100  as runstat from OptimizerQ  where left(ModelRunId,5) = '" & CustomRun & "'"
            statusSQL = "select count(case when [Status]  = 'Z' then 1 end) As finishedtasks ,count( case when [Status] <> '' then 1 end) as tasks from OptimizerQ  where left(ModelRunId,5) = '" & CustomRun & "'"
            'removed extra _) then 1 end))
            model_tasks = db.Database.SqlQuery(Of modelstatus)(statusSQL).FirstOrDefault()
            ' rsstatus.Open(statusSQL, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            ' If Not rsstatus.EOF Then
            If model_tasks.Tasks > 4 Then
                modelpctcomplete = CInt(CDbl(model_tasks.finishedTasks / model_tasks.Tasks) * 100)
            Else
                modelpctcomplete = 0
            End If
            'Else
            'modelpctcomplete = 0
            'End If
            'rsstatus.Close()
            'Catch ex As Exception
            'modelpctcomplete = 0
            'End Try


            'statusSQL = "select count(*) from optimizerlog where CustomRunNumber = '" & CustomRun & "'"
            'rsstatus.Open(statusSQL, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            'If Not rsstatus.EOF Then
            '    modelsrunsofar = rsstatus.Fields(0).Value
            'Else
            '    modelsrunsofar = 0
            'End If
            'rsstatus.Close()

            modelstatusguage.Pointer.Color = Color.Black
            modelstatusguage.Scale.Ranges(0).Color = Color.IndianRed

            If logcount > 200 Then
                modelstatusguage.Pointer.Value = 100
                modelstatusguage.Pointer.Color = Color.IndianRed
                modelstatusguage.ToolTip = "Model Failed"
            Else
                modelstatusguage.Pointer.Value = modelpctcomplete
                modelstatusguage.ToolTip = "Model Running " & modelpctcomplete & "% Complete with " & vbNewLine & modelsrunsofar & " Models Run"
                If modelpctcomplete < 10 Then
                    modelstatusguage.ToolTip = "Model Submitted No Available Data Yet"
                ElseIf modelpctcomplete < 50 Then
                    modelstatusguage.Pointer.Color = Color.Yellow
                Else
                    modelstatusguage.Pointer.Color = Color.YellowGreen
                End If
                If modelsrunsofar > 100 Then
                    modelstatusguage.Scale.Ranges(0).Color = Color.LightGreen
                ElseIf modelsrunsofar > 50 Then
                    modelstatusguage.Scale.Ranges(0).Color = Color.Yellow
                End If

            End If
        End If
        '   If cnstatus.State = 1 Then cnstatus.Close()

    End Sub



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
            '20120724 - fix connection strings
            cnsetting.ConnectionString = ConnectionStringHelper.ReadOnlyServerConnectionString
            'cnsetting.ConnectionString = ConnectionStringSQL
            cnsetting.Open()
        End If



        '        rs.Open(req)
        '20100222 - pab - use global shared connection
        'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


        If Not rs.EOF Then

        End If


        If rs.State = 1 Then rs.Close()

        If rs.State = 1 Then rs.Close()
        Dim _flightID As Integer

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
    End Sub


    Protected Sub cmdFindTrip_Click(ByVal sender As Object, ByVal e As System.EventArgs) ' Handles cmdFindTrip.Click

        Dim cnlocal As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim cnoptimizer As New ADODB.Connection
        Dim req As String
        Dim databaseFROM As String = "TMCProduction" 'Me.txtDBFrom.Text

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("cascalendarmodelid")) Then Session("cascalendarmodelid") = ""
        Dim cascalendarmodelid As String = Session("cascalendarmodelid")
        Dim daterangefrom, daterangeto As Date

        If cnoptimizer.State = 1 Then cnoptimizer.Close()
        If cnoptimizer.State = 0 Then
            cnoptimizer.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            cnoptimizer.Open()
        End If

        If modelrunid = "" Then

            req = "SELECT top 1 * from optimizerrequest where status = 'X' and description not like 'Optimizer request %'   and carrierid = abc  order by   id desc "
            req = Replace(req, "abc", Session("carrierid"))

            Dim customid As String
            rs.Open(req, cnoptimizer, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            If Not rs.EOF Then
                'latest completed model
                customid = rs.Fields("id").Value
                Session("customid") = customid

                daterangefrom = rs.Fields("gmtstart").Value
                daterangeto = rs.Fields("gmtend").Value

                '20171121 - pab - fix carriers changing midstream - change to Session variables
                Session("daterangefrom") = daterangefrom
                Session("daterangefrom") = daterangeto
            End If

            If carrierid = JETLINX Then

                req = "SELECT top 1 * FROM [OptimizerLog]  where casrevenuemiles <> 0   and  customrunnumber = '8183'   and CASrevenueexpense <> 0 and carrierid = abc order by CAStotalrevenueexpense  "
                req = Replace(req, "8183", customid)
                req = Replace(req, "abc", Session("carrierid"))
                'rk 10.20.2012 grab the lowest of the two models

            ElseIf carrierid = WHEELSUP Then

                req = "SELECT top 1 * FROM [OptimizerLog]  where casrevenuemiles <> 0 and  left (modelrunid, 4) = '8183' and caslinebreaks <= (foslinebreaks + 7)  and  modelrunid not like '%Q-%' and ModelRunID not like '%R11%' and ModelRunID not like '%R12%'   and ModelRunID not like '%R0%'  and CASrevenueexpense <> 0 and carrierid = abc and FOSrevenuelegs - 25 <= CASrevenuelegs order by CAStotalrevenueexpense asc"
                req = Replace(req, "8183", customid)
                req = Replace(req, "(modelrunid, 4)", "(modelrunid, " & Len(customid) & ")")
                req = Replace(req, "abc", Session("carrierid"))
                'rk 10.20.2012 grab the lowest of the two models
            Else

                req = "SELECT top 1 * FROM [OptimizerLog]  where casrevenuemiles <> 0 and  customrunnumber = '8183'    and  modelrunid not like '%Q-%' and  modelrunid not like '%R0%'   and CASrevenueexpense <> 0 and carrierid = abc order by CAStotalrevenueexpense  "
                req = Replace(req, "8183", customid)
                req = Replace(req, "abc", Session("carrierid"))

            End If
            If rs.State = 1 Then rs.Close()

            rs.Open(req, cnoptimizer, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            If Not rs.EOF Then
                modelrunid = Trim(rs.Fields("modelrunid").Value)
                cascalendarmodelid = modelrunid
                Session("AcceptModelID") = cascalendarmodelid

            End If

            If rs.State = 1 Then rs.Close()

        End If

        Dim s As String
        s = "SELECT    A.id, a.pinned,   A.DepartureAirport as [FROM] ,A.ArrivalAirport as [TO] , A.departuretime as [From GMT] , A.arrivaltime as [To GMT]," &
            "CAST(A.DepartureDateTimeLocal as datetime) as [From Local], case when basecode = '' then 'ZZZ' else basecode end as base," &
            "CAST(A.ArrivalDateTimeLocal as datetime) as [To Local], " &
            "A.duration as [Minutes]  , A.distance as [NM]    ,ltrim(rtrim(A.AircraftRegistration)) as [ac], ltrim(rtrim(A.AircraftType)) as [type], " &
            "cast(A.cost as integer) as cost, A.Flighttype as FT, ltrim(rtrim(A.tripnumber)) as tripnumber, " &
            "case when priortail Is null then '' else priortail end as priortail, " &
            "ltrim(rtrim(A.legratecode)) as [LRC] ,ltrim(rtrim(A.legpurposecode)) as [LPC] ,ltrim(rtrim(A.legtypecode)) as [LTC] , " &
            "ltrim(rtrim(A.PIC)) as [PIC], LTrim(RTrim(A.SIC)) As [SIC], A.CrewDutyFlightTime   ,A.CrewDutyWindow ,A.CrewDutyComment , " &
            "A.tripcost  ,A.triprevenue, A.pandl, B.AircraftWeightClass As [AWC]  " &
            "FROM CASFlightsOptimizer as A, AircraftWeightClass As B where A.OptimizerRun = 'abc' and B.AircraftType = A.AircraftType  "
        '"   ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC] , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC], [CrewDutyFlightTime]   ,[CrewDutyWindow] ,[CrewDutyComment] ,  [tripcost]  ,[triprevenue], [pandl]  FROM CASFlightsOptimizer where OptimizerRun = 'abc'  "
        s = Replace(s, "abc", modelrunid)
        s = Replace(s, "ghi", Session("carrierid"))

        If carrierid <> JETLINX Then
            If carrierid = WHEELSUP Then
                s = s & "order by ac, [FROM GMT] asc  "
            Else
                s = s & "  order by AWC, ac, [FROM GMT] asc  "
            End If
        Else
            s = s & "order by base, AWC,ac, [FROM GMT] asc  "
        End If

        If Not (IsNothing(DropDownNNumbers.SelectedItem)) Then
            If DropDownNNumbers.SelectedItem.Value <> "----" Then
                s = "SELECT    id,  pinned,  [DepartureAirport] as [FROM]    ,[ArrivalAirport] as [TO]       , departuretime as [From GMT] , arrivaltime as [To GMT]" &
                      ", CAST(DepartureDateTimeLocal as datetime) as [From Local] " &
             ", CAST(ArrivalDateTimeLocal as datetime) as [To Local] " &
          "   ,[duration] as [Minutes]  , distance as [NM]    ,ltrim(rtrim(AircraftRegistration)) as [ac], ltrim(rtrim(aircrafttype)) as type, cast(cost as integer) as cost, [Flighttype] as FT, ltrim(rtrim(tripnumber)) as tripnumber, case when priortail is null then '' else priortail end as priortail " &
    "    ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC]  , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC],  [CrewDutyFlightTime]   ,[CrewDutyWindow] ,[CrewDutyComment]    ,  [tripcost]  ,[triprevenue], [pandl]     FROM [CASFlightsOptimizer] where AircraftRegistration = 'def' and OptimizerRun = 'abc'  "
                s = Replace(s, "def", DropDownNNumbers.SelectedItem.Value)
                s = Replace(s, "abc", modelrunid)
                s = Replace(s, "ghi", Session("carrierid"))

                If carrierid <> JETLINX Then
                    If carrierid = WHEELSUP Then
                        s = s & "order by AircraftRegistration, departuretime asc  "
                    Else
                        s = s & " order by  aircrafttype, AircraftRegistration, departuretime asc  "
                    End If
                Else
                    s = s & "order by basecode,  AircraftRegistration, departuretime asc  "
                End If

            End If
        End If


        If Not (IsNothing(DropDownTripNumbers.SelectedItem)) Then
            If DropDownTripNumbers.SelectedItem.Value <> "----" Then
                s = "SELECT     id,  pinned,  [DepartureAirport] as [FROM]    ,[ArrivalAirport] as [TO]       , departuretime as [From GMT] , arrivaltime as [To GMT]" &
                      ", CAST(DepartureDateTimeLocal as datetime) as [From Local] " &
             ", CAST(ArrivalDateTimeLocal as datetime) as [To Local] " &
            "   ,[duration] as [Minutes]  , distance as [NM]    ,ltrim(rtrim(AircraftRegistration)) as [ac], ltrim(rtrim(aircrafttype)) as type, cast(cost as integer) as cost, [Flighttype] as FT, ltrim(rtrim(tripnumber)) as tripnumber, case when priortail is null then '' else priortail end as priortail " &
      "   ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC]    , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC],  [CrewDutyFlightTime]   ,[CrewDutyWindow] ,[CrewDutyComment]   ,  [tripcost]  ,[triprevenue], [pandl]     FROM [CASFlightsOptimizer] where tripnumber = 'def' and optimizerRun = 'abc'  "
                s = Replace(s, "abc", modelrunid)
                s = Replace(s, "def", DropDownTripNumbers.SelectedItem.Value)
                s = Replace(s, "ghi", Session("carrierid"))

                If carrierid <> JETLINX Then
                    If carrierid = WHEELSUP Then
                        s = s & " order by tripnumber, departuretime asc "
                    Else
                        s = s & " order by  aircrafttype, AircraftRegistration, departuretime asc  "
                    End If
                Else
                    s = s & "order by basecode,  AircraftRegistration, departuretime asc  "
                End If

            End If
        End If


        If Not (IsNothing(DropDownOriginAirports.SelectedItem)) Then
            If DropDownOriginAirports.SelectedItem.Value <> "----" Then

                s = "SELECT    id,   pinned,  [DepartureAirport] as [FROM]    ,[ArrivalAirport] as [TO]      , departuretime as [From GMT] , arrivaltime as [To GMT]" &
                      ", CAST(DepartureDateTimeLocal as datetime) as [From Local] " &
             ", CAST(ArrivalDateTimeLocal as datetime) as [To Local] " &
          "   ,[duration] as [Minutes]  , distance as [NM]    ,ltrim(rtrim(AircraftRegistration)) as [ac], ltrim(rtrim(aircrafttype)) as type, cast(cost as integer) as cost, [Flighttype] as FT, ltrim(rtrim(tripnumber)) as tripnumber, case when priortail is null then '' else priortail end as priortail " &
    "    ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC]   , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC],  [CrewDutyFlightTime]   ,[CrewDutyWindow] ,[CrewDutyComment]    ,  [tripcost]  ,[triprevenue], [pandl]    FROM [CASFlightsOptimizer] where DepartureAirport = 'def' and optimizerRun = 'abc'  "
                s = Replace(s, "def", DropDownOriginAirports.SelectedItem.Value)
                s = Replace(s, "abc", modelrunid)
                s = Replace(s, "ghi", Session("carrierid"))

                If carrierid <> JETLINX Then
                    If carrierid = WHEELSUP Then
                        s = s & "order by AircraftRegistration, departuretime asc  "
                    Else
                        s = s & "  order by  aircrafttype, AircraftRegistration, departuretime asc  "
                    End If
                Else
                    s = s & "order by basecode,  AircraftRegistration, departuretime asc  "
                End If
            End If
        End If

        If Not (IsNothing(DropDownDestAirports.SelectedItem)) Then
            If DropDownDestAirports.SelectedItem.Value <> "----" Then
                s = "SELECT   id,   pinned,   [DepartureAirport] as [FROM]    ,[ArrivalAirport] as [TO]      , departuretime as [From GMT] , arrivaltime as [To GMT]" &
                      ", CAST(DepartureDateTimeLocal as datetime) as [From Local] " &
             ", CAST(ArrivalDateTimeLocal as datetime) as [To Local] " &
          "   ,[duration] as [Minutes]  , distance as [NM]    ,ltrim(rtrim(AircraftRegistration)) as [ac], ltrim(rtrim(aircrafttype)) as type, cast(cost as integer) as cost, [Flighttype] as FT, ltrim(rtrim(tripnumber)) as tripnumber, case when priortail is null then '' else priortail end as priortail " &
    "   ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC]    , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC] ,  [CrewDutyFlightTime]   ,[CrewDutyWindow] ,[CrewDutyComment]  ,  [tripcost]  ,[triprevenue], [pandl]     FROM [CASFlightsOptimizer] where ArrivalAirport = 'def' and optimizerRun = 'abc' "
                s = Replace(s, "def", DropDownDestAirports.SelectedItem.Value)
                s = Replace(s, "abc", modelrunid)
                s = Replace(s, "ghi", Session("carrierid"))

                If carrierid <> JETLINX Then
                    If carrierid = WHEELSUP Then
                        s = s & "order by AircraftRegistration, departuretime asc  "
                    Else
                        s = s & "  order by  aircrafttype, AircraftRegistration, departuretime asc  "
                    End If
                Else
                    s = s & "order by basecode,  AircraftRegistration, departuretime asc  "
                End If
            End If
        End If

        If Not (IsNothing(DropDownFleetType.SelectedItem)) Then
            If DropDownFleetType.SelectedItem.Value <> "----" Then

                s = "SELECT  id,    pinned,   [DepartureAirport] as [FROM]    ,[ArrivalAirport] as [TO]      , departuretime as [From GMT] , arrivaltime as [To GMT]" &
                      ", CAST(DepartureDateTimeLocal as datetime) as [From Local] " &
             ", CAST(ArrivalDateTimeLocal as datetime) as [To Local] " &
          "   ,[duration] as [Minutes]  , distance as [NM]    ,ltrim(rtrim(AircraftRegistration)) as [ac], ltrim(rtrim(aircrafttype)) as type, cast(cost as integer) as cost, [Flighttype] as FT, ltrim(rtrim(tripnumber)) as tripnumber, case when priortail is null then '' else priortail end as priortail " &
    "  ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC]   , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC],  [CrewDutyFlightTime]   ,[CrewDutyWindow] ,[CrewDutyComment]   ,  [tripcost]  ,[triprevenue], [pandl]       FROM [CASFlightsOptimizer]  where aircrafttype = 'def'  and optimizerRun = 'abc'  "
                s = Replace(s, "def", DropDownFleetType.SelectedItem.Value)
                s = Replace(s, "abc", modelrunid)
                s = Replace(s, "ghi", Session("carrierid"))

                s = Replace(s, "where aircrafttype = 'H80X'", "where (AircraftType = 'H80H' or AircraftType = 'H850')")
                If carrierid <> JETLINX Then
                    If carrierid = WHEELSUP Then
                        s = s & "order by AircraftRegistration, departuretime asc  "
                    Else
                        s = s & "  order by  aircrafttype, AircraftRegistration, departuretime asc  "
                    End If
                Else
                    s = s & "order by basecode,  AircraftRegistration, departuretime asc  "
                End If
            End If
        End If


        If AC1 <> "" Or ACX(1) <> "" Then
            If AC1 <> "" Then
                s = "SELECT    id,   pinned,  [DepartureAirport] as [FROM]    ,[ArrivalAirport] as [TO]       , departuretime as [From GMT] , arrivaltime as [To GMT]" &
                      ", CAST(DepartureDateTimeLocal as datetime) as [From Local] " &
             ", CAST(ArrivalDateTimeLocal as datetime) as [To Local] " &
          "   ,[duration] as [Minutes]  , distance as [NM]    ,ltrim(rtrim(AircraftRegistration)) as [ac], ltrim(rtrim(aircrafttype)) as type, cast(cost as integer) as cost, [Flighttype] as FT, ltrim(rtrim(tripnumber)) as tripnumber, case when priortail is null then '' else priortail end as priortail " &
    "    ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC]  , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC],  [CrewDutyFlightTime]   ,[CrewDutyWindow] ,[CrewDutyComment]    ,  [tripcost]  ,[triprevenue], [pandl] , basecode as base, '' as awc    FROM [CASFlightsOptimizer] where ( AircraftRegistration = 'def' or AircraftRegistration = 'zqp') and OptimizerRun = 'abc'  "
                s = Replace(s, "def", AC1)
                s = Replace(s, "zqp", AC2)
            ElseIf ACX(1) <> "" Then
                s = "SELECT    id,    [DepartureAirport] as [FROM]    ,[ArrivalAirport] as [TO]       , departuretime as [From GMT] , arrivaltime as [To GMT]" &
                      ", CAST(DepartureDateTimeLocal as datetime) as [From Local] " &
             ", CAST(ArrivalDateTimeLocal as datetime) as [To Local] " &
          "   ,[duration] as [Minutes]  , distance as [NM]    ,ltrim(rtrim(AircraftRegistration)) as [ac], ltrim(rtrim(aircrafttype)) as type, cast(cost as integer) as cost, [Flighttype] as FT, ltrim(rtrim(tripnumber)) as tripnumber, case when priortail is null then '' else priortail end as priortail " &
    "    ,ltrim(rtrim([legratecode])) as [LRC] ,ltrim(rtrim([legpurposecode])) as [LPC] ,ltrim(rtrim([legtypecode])) as [LTC]  , ltrim(rtrim([PIC])) as [PIC], ltrim(rtrim([SIC])) as [SIC],  [CrewDutyFlightTime]   ,[CrewDutyWindow] ,[CrewDutyComment]    ,  [tripcost]  ,[triprevenue], [pandl]     FROM [CASFlightsOptimizer] where      carrierid = 'ghi'  and OptimizerRun = 'abc' "

                s = s & " and CONVERT(datetime, departuretime)   >  SYSUTCDATETIME()  and  AircraftRegistration in (" & Session("FCList") & ")"
            End If

            s = Replace(s, "abc", modelrunid)
            s = Replace(s, "ghi", Session("carrierid"))

            If carrierid <> JETLINX Then
                If carrierid = WHEELSUP Then
                    s = s & "order by AircraftRegistration, departuretime asc  "
                Else
                    s = s & "  order by  aircrafttype, AircraftRegistration, departuretime asc  "
                End If
            Else
                s = s & "order by basecode,  AircraftRegistration, departuretime asc  "
            End If
        End If

        Session("link") = "Trip"
        lblmodelid.Text = modelrunid
        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx Start BInd " & Now & " - modelrunid not blank - ", "", "")
        BindDataTrips(s)
        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx End BInd " & Now & " - modelrunid not blank - ", "", "")

    End Sub




    Protected Sub cmdRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRefresh.Click


    End Sub

    Protected Sub AcceptAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles radbtnAcceptAll.Click

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("cascalendarmodelid")) Then Session("cascalendarmodelid") = ""
        Dim cascalendarmodelid As String = Session("cascalendarmodelid")

        If cascalendarmodelid <> "" Then
            Session("AcceptModelID") = cascalendarmodelid
        End If

    End Sub


    Protected Sub LinkPending_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkPending.Click

        Dim id As Int16 = 0

        Try
            id = GridViewTrips.SelectedRow.Cells(1).Text
        Catch ex As Exception
            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx LinkPending_Click Error " & Now & " - " & ex.Message, "", "")
        End Try

        If id <> 0 Then
            '         updateitin(id, "", "", "", "", "", "P")
        End If

        cmdRefresh_Click(Nothing, Nothing)




    End Sub


    Function pullfromcaslog(modelrunid As String)

        Try

            Dim cnmkazurelocal As New ADODB.Connection

            Dim rs As New ADODB.Recordset
            Dim req As String


            If cnmkazurelocal.State = 0 Then
                cnmkazurelocal.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
                cnmkazurelocal.ConnectionTimeout = 30
                cnmkazurelocal.CommandTimeout = 30
                cnmkazurelocal.Open()
            End If


            If rs.State = 1 Then rs.Close()


            If AC1 = "" Or ACX(1) = "" Then


                req = "select * FROM optimizerlog  where modelrunid = 'abc'"
                req = Replace(req, "abc", modelrunid)



                ' Dim rs As New ADODB.Recordset
                If rs.State = 1 Then rs.Close()
                rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

                If Not rs.EOF Then
                    Me.lblrevenuemiles.Text = FormatNumber(rs.Fields("CASrevenuemiles").Value, 0)
                    Me.lblnonrevenuemiles.Text = FormatNumber(rs.Fields("CASnonrevenuemiles").Value, 0)
                    Me.lblnonrevenueexpense.Text = "$" & FormatNumber(rs.Fields("CASnonrevenueexpense").Value, 0)
                    Me.lbltotalrevenueexpense.Text = "$" & FormatNumber(rs.Fields("CAStotalrevenueexpense").Value, 0)
                    Me.lblrevenueexpense.Text = "$" & FormatNumber(rs.Fields("CASrevenueexpense").Value, 0)
                    Me.lblrevenuelegs.Text = rs.Fields("CASrevenuelegs").Value
                    Me.lblnonrevenuelegs.Text = rs.Fields("CASnonrevenuelegs").Value
                    Me.lblLongestEmptyLeg.Text = rs.Fields("CASLongestEmptyLeg").Value
                    Me.lblShortestEmtpyLeg.Text = rs.Fields("CASShortestEmtpyLeg").Value
                    Me.lblAverageEmtpyNM.Text = rs.Fields("CASAverageEmtpyNM").Value
                    Me.lblefficiency.Text = FormatPercent(rs.Fields("CASefficiency").Value)
                    Me.lblLineBreaks.Text = rs.Fields("CASLineBreaks").Value

                    If IsNumeric(rs.Fields("CASLineBreaks").Value) Then
                        If CInt(rs.Fields("CASLineBreaks").Value) <> 0 Then
                            Me.lblLineBreaks.BackColor = Drawing.Color.Red
                        End If
                    End If



                    If caslinebreaks <> 0 Then
                        Me.lblLineBreaks.Text = caslinebreaks
                        Me.lblLineBreaks.BackColor = Drawing.Color.Red
                    Else
                        Me.lblLineBreaks.Text = 0
                        Me.lblLineBreaks.BackColor = Drawing.Color.White

                    End If

                End If

            Else

                If AC1 <> "" Then
                    req = "SELECT sum(CONVERT(integer, distance)) as nauticalmiles   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'D'  and (AircraftRegistration = 'def' or AircraftRegistration = 'ghi')"
                ElseIf ACX(1) <> "" Then
                    req = "SELECT sum(CONVERT(integer, distance)) as nauticalmiles   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'D'  and AircraftRegistration in ("
                    For zz = 0 To 50
                        If ACX(zz) <> "" Then
                            req = req & "'" & ACX(zz) & "'"
                            If ACX(zz + 1) <> "" Then req = req & ","
                        End If
                    Next zz
                    req = req & " ) "

                End If
                req = Replace(req, "abc", modelrunid)
                req = Replace(req, "def", AC1)
                req = Replace(req, "ghi", AC2)

                If rs.State = 1 Then rs.Close()

                rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
                If Not rs.EOF Then
                    If Not (IsDBNull(rs.Fields("nauticalmiles").Value)) Then
                        Me.lblnonrevenuemiles.Text = FormatNumber(rs.Fields("nauticalmiles").Value, 0)
                    End If
                End If
                If rs.State = 1 Then rs.Close()
                If ACX(1) = "" Then
                    req = "SELECT sum(CONVERT(integer, distance)) as nauticalmiles   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'R'  and (AircraftRegistration = 'def' or AircraftRegistration = 'ghi')"
                Else
                    req = "SELECT sum(CONVERT(integer, distance)) as nauticalmiles   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'R'  and AircraftRegistration  in ("
                    For zz = 0 To 50
                        If ACX(zz) <> "" Then
                            req = req & "'" & ACX(zz) & "'"
                            If ACX(zz + 1) <> "" Then req = req & ","
                        End If
                    Next zz
                    req = req & " ) "
                End If
                req = Replace(req, "abc", modelrunid)
                req = Replace(req, "def", AC1)
                req = Replace(req, "ghi", AC2)

                rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
                If Not rs.EOF Then
                    If Not (IsDBNull(rs.Fields("nauticalmiles").Value)) Then
                        Me.lblrevenuemiles.Text = FormatNumber(rs.Fields("nauticalmiles").Value, 0)
                    End If
                End If
                If rs.State = 1 Then rs.Close()

            End If
            If rs.State = 1 Then rs.Close()

        Catch ex As Exception
            Dim i As Integer
            i = i
            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx pullfromcaslog Error " & Now & " - " & ex.Message, "", "")
        End Try

        Try
            Dim cnmkazurelocal As New ADODB.Connection
            Dim rs As New ADODB.Recordset
            Dim req As String

            If cnmkazurelocal.State = 0 Then
                cnmkazurelocal.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
                cnmkazurelocal.ConnectionTimeout = 30
                cnmkazurelocal.CommandTimeout = 30
                cnmkazurelocal.Open()
            End If

            If rs.State = 1 Then rs.Close()

            req = "select * FROM optimizerlog  where modelrunid = 'abc'"
            req = Replace(req, "abc", modelrunid)

            ' Dim rs As New ADODB.Recordset
            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

            'rk 10.4.2013 check for missing record.
            If Not rs.EOF Then


                rs.Fields("ViewedBy").Value = Session("defaultemail")
                rs.Fields("ViewedOn").Value = Now

                If IsDBNull(rs.Fields("Viewed").Value) Then rs.Fields("Viewed").Value = 0
                If IsNumeric(rs.Fields("Viewed").Value) Then
                    rs.Fields("Viewed").Value = rs.Fields("Viewed").Value + 1
                Else
                    rs.Fields("Viewed").Value = 1
                End If
                rs.Update()
            End If
            If rs.State = 1 Then rs.Close()

            If cnmkazurelocal.State = 1 Then cnmkazurelocal.Close()
        Catch ex As Exception
            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx pullfromcaslog Update Viewed " & Now & " - " & ex.Message, "", "")
        End Try

    End Function
    Public Sub loaddropdowns()
        Dim list(5000) As String
        Dim count As Integer
        Dim a As String
        Dim found As Boolean = False


        If DropDownTripNumbers.Items.Count = 0 Then
            Dim dv As System.Data.DataView = CType(Me.SqlDataSourceTrips.Select(DataSourceSelectArguments.Empty), System.Data.DataView)
            count = 0

            For i = 1 To 5000
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

            DropDownTripNumbers.Items.Clear()
            DropDownTripNumbers.Items.Add("----")
            For i = 1 To count
                DropDownTripNumbers.Items.Add((Trim(list(i))))
            Next i

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx StartDrop Registration " & Now & " - 1364 - ", "", "")
            count = 0

            For i = 1 To 5000
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

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx StartDrop departureAirport " & Now & " - 1382 - ", "", "")


            count = 0

            For i = 1 To 5000
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

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx StartDrop ArrivalAirport " & Now & " - 1399 - ", "", "")

            count = 0

            For i = 1 To 5000
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

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx StartDrop AC Type " & Now & " - 1416 - ", "", "")



            count = 0

            For i = 1 To 5000
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

        End If

    End Sub

    Public Sub grabcounts()


        Dim cnmkazurelocal2 As New ADODB.Connection

        If cnmkazurelocal2.State = 0 Then
            cnmkazurelocal2.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
            cnmkazurelocal2.ConnectionTimeout = 30
            cnmkazurelocal2.CommandTimeout = 30
            cnmkazurelocal2.Open()
        End If


        Dim rs As New ADODB.Recordset


        Dim req As String

        '20120724 - pab - exclude maintenance legs - FlightType = 'M'
        'req = "SELECT sum(CONVERT(integer, cost)) as dhcost   FROM [CASFlightsOptimizer]  where OptimizerRun = 'abc' "
        req = "SELECT sum(CONVERT(integer, cost)) as dhcost   FROM [CASFlightsOptimizer]  where OptimizerRun = 'abc' and FlightType <> 'M' "
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            If Not (IsDBNull(rs.Fields("dhcost").Value)) Then
                Me.lbltotalrevenueexpense.Text = "$" & FormatNumber(rs.Fields("dhcost").Value, 0)
            End If
        End If

        If rs.State = 1 Then rs.Close()


        '20120724 - pab - exclude maintenance legs - FlightType = 'M'
        'req = "SELECT sum(CONVERT(integer, cost)) as dhcost   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'D'"
        req = "SELECT sum(CONVERT(integer, cost)) as dhcost   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'D' and FlightType <> 'M'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            If Not (IsDBNull(rs.Fields("dhcost").Value)) Then
                Me.lblnonrevenueexpense.Text = "$" & FormatNumber(rs.Fields("dhcost").Value, 0)
            End If
        End If

        If rs.State = 1 Then rs.Close()


        '20120724 - pab - exclude maintenance legs - FlightType = 'M'
        'req = "SELECT sum(CONVERT(integer, cost)) as dhcost   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'R'"
        req = "SELECT sum(CONVERT(integer, cost)) as dhcost   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'R' and FlightType <> 'M'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            If Not (IsDBNull(rs.Fields("dhcost").Value)) Then
                Me.lblrevenueexpense.Text = "$" & FormatNumber(rs.Fields("dhcost").Value, 0)
            End If

        End If
        If rs.State = 1 Then rs.Close()


        req = "SELECT sum(CONVERT(integer, distance)) as nauticalmiles   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'D'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            If Not (IsDBNull(rs.Fields("nauticalmiles").Value)) Then
                Me.lblnonrevenuemiles.Text = FormatNumber(rs.Fields("nauticalmiles").Value, 0)
            End If
        End If
        If rs.State = 1 Then rs.Close()

        req = "SELECT sum(CONVERT(integer, distance)) as nauticalmiles   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'R'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            If Not (IsDBNull(rs.Fields("nauticalmiles").Value)) Then
                Me.lblrevenuemiles.Text = FormatNumber(rs.Fields("nauticalmiles").Value, 0)
            End If
        End If
        If rs.State = 1 Then rs.Close()

        req = "SELECT count(*) as count   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'R'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblrevenuelegs.Text = rs.Fields("count").Value
        End If
        If rs.State = 1 Then rs.Close()

        req = "SELECT count(*) as count   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'D'"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblnonrevenuelegs.Text = rs.Fields("count").Value
        End If
        If rs.State = 1 Then rs.Close()


        req = "SELECT top 1 distance   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'D' order by Distance desc"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblLongestEmptyLeg.Text = rs.Fields("distance").Value
        End If
        If rs.State = 1 Then rs.Close()


        req = "SELECT top 1 distance   FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and flighttype = 'D' order by Distance asc"
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            Me.lblShortestEmtpyLeg.Text = rs.Fields("distance").Value
        End If
        If rs.State = 1 Then rs.Close()


        req = "SELECT avg( distance) as distance  FROM [CASFlightsOptimizer] where OptimizerRun = 'abc' and  flighttype = 'D' "
        req = Replace(req, "abc", modelrunid)

        rs.Open(req, cnmkazurelocal2, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        If Not rs.EOF Then
            If Not (IsDBNull(rs.Fields("distance").Value)) Then
                Me.lblAverageEmtpyNM.Text = rs.Fields("distance").Value
            End If
        End If
        If rs.State = 1 Then rs.Close()



        If IsNumeric(lblrevenuemiles.Text) Then

            Dim eff As Double = 0
            If IsNumeric(lblrevenuemiles.Text) And IsNumeric(lblnonrevenuemiles.Text) And IsNumeric(lblrevenuemiles.Text) Then

                eff = CDbl(lblrevenuemiles.Text) / (CDbl(lblnonrevenuemiles.Text) + CDbl(lblrevenuemiles.Text))

            End If


            lblefficiency.Text = FormatPercent(eff)

        End If

        If cnmkazurelocal2.State = 1 Then cnmkazurelocal2.Close()
    End Sub


    Protected Sub DropDownNNumbers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownNNumbers.SelectedIndexChanged
        DropDownDestAirports.SelectedIndex = 0
        DropDownOriginAirports.SelectedIndex = 0
        DropDownTripNumbers.SelectedIndex = 0

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
        DropDownFleetType.SelectedIndex = 0
        cmdFindTrip_Click(Nothing, Nothing)
    End Sub

    Protected Sub DropDownFleetType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFleetType.SelectedIndexChanged
        DropDownOriginAirports.SelectedIndex = 0
        DropDownDestAirports.SelectedIndex = 0
        DropDownNNumbers.SelectedIndex = 0
        DropDownTripNumbers.SelectedIndex = 0
        cmdFindTrip_Click(Nothing, Nothing)
    End Sub


    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender


        If modelrunid = "" Then
            If Not Request.QueryString("modelrunid") Is Nothing Then
                modelrunid = Request.QueryString("modelrunid")
            End If
        End If
        If GridViewTrips.Rows.Count > 5 Then
            colorflighttype()
        End If

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

    Private Sub GridViewTrips_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridViewTrips.RowEditing
        GridViewTrips.EditIndex = e.NewEditIndex
        '  GridViewTrips.DataBind()
    End Sub


    Protected Sub chkLocal_CheckedChanged(sender As Object, e As EventArgs) Handles chkLocal.CheckedChanged



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


        cmdFindTrip_Click(Nothing, Nothing)
    End Sub

    Protected Sub chkGMT_CheckedChanged(sender As Object, e As EventArgs) Handles chkGMT.CheckedChanged



        If chkGMT.Checked = True Then
            '((Label)e.Row.FindControl("lblName")).Text;
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

        cmdFindTrip_Click(Nothing, Nothing)
    End Sub


    Protected Sub radAjxMgr_AjaxRequest(sender As Object, e As AjaxRequestEventArgs)
    End Sub
    Protected Sub grdEmployeee_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewTrips.RowCommand
        If e.CommandName = "showPopUp" Then

            Dim gvr As GridViewRow = CType(CType(e.CommandSource, LinkButton).NamingContainer, GridViewRow)
            Dim RowIndex As Integer = gvr.RowIndex
            Dim row As GridViewRow = GridViewTrips.Rows(RowIndex)
            Dim EmpId As String = row.Cells(1).Text

            Dim param As String = "'" + EmpId + "'"
            ScriptManager.RegisterClientScriptBlock(Me, GetType(Page), "key", "ShowDepart(" + param + ")", True)

        End If
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

    Function linemeup() ' For CAS Side


        Dim aline(500, 4) As String ' 0=AC number, 1 = row count, 2= number of blank lines to insert after row
        Dim gridviewrowstart As Integer = 0


        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            cn.Open()
        End If
        If carrierid <> JETLINX Then
            If carrierid = WHEELSUP Then
                req = "SELECT count(*)  as cnt, aircraftregistration,aircrafttype FROM [CASFlightsOptimizer]  where optimizerrun = '1140-240797-R1-52C-96' group by aircraftregistration,aircrafttype order by aircraftregistration,aircrafttype"
            Else
                req = "SELECT count(*)  as cnt, A.aircraftregistration,B.AircraftWeightClass as AWC FROM CASFlightsOptimizer as A, AircraftWeightClass as B where A.optimizerrun = '1140-240797-R1-52C-96' and A.AircraftType = B.AircraftType group by B.AircraftWeightClass ,A.aircraftregistration order by AWC,A.aircraftregistration"
            End If
        Else
            req = "SELECT count(*)  as cnt, A.aircraftregistration,case when A.basecode = '' then 'ZZZ' else A.basecode end + B.AircraftWeightClass as AWC FROM CASFlightsOptimizer as A, AircraftWeightClass as B where A.optimizerrun = '1140-240797-R1-52C-96' and A.AircraftType = B.AircraftType group by case when A.basecode = '' then 'ZZZ' else A.basecode end + B.AircraftWeightClass ,A.aircraftregistration order by AWC,A.aircraftregistration"
        End If
        req = Replace(req, "1140-240797-R1-52C-96", modelrunid)


        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        Dim ac As Integer = 0

        'First we load the array with the Fos flights
        Do While Not rs.EOF
            ac = ac + 1
            aline(ac, 0) = rs.Fields("aircraftregistration").Value.ToString.Trim
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
                req = "SELECT count(*) as cnt, ac,aircrafttype FROM [fosFlightsOptimizer]  where optimizerrun = '1140-240797-R1-52C-96' group by ac,aircrafttype order by ac,aircrafttype"
            Else
                req = "SELECT count(*) as cnt, A.ac,B.AircraftWeightClass  as AWC FROM fosFlightsOptimizer as A, AircraftWeightClass as B  where A.optimizerrun = '1140-240797-R1-52C-96' and A.AircraftType = B.AircraftType group by B.AircraftWeightClass,A.ac order by AWC,A.ac"
            End If
        Else
            req = "SELECT count(*) as cnt, A.ac,case when A.basecode = '' then 'ZZZ' else A.basecode end + B.AircraftWeightClass  as AWC FROM fosFlightsOptimizer as A, AircraftWeightClass as B  where A.optimizerrun = '1140-240797-R1-52C-96' and A.AircraftType = B.AircraftType group by case when A.basecode = '' then 'ZZZ' else A.basecode end + B.AircraftWeightClass,A.ac order by AWC,A.ac"
        End If

        Dim mrcustom As String
        mrcustom = normalizemodelrunid(modelrunid)

        req = Replace(req, "1140-240797-R1-52C-96", mrcustom)

        Dim sTest As String = Session("FCList")

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


        'now we will loop thru all the Cas flights and find the matching ones and build the array
        'we can stop this when we run out of Fos Flights 
        Dim x As Integer  ' a generic counter for number of fos ac...

        For x = 1 To ac
            Do While Not rs.EOF  'once we run out of cas flights we are done..
                If aline(x, 3) = rs.Fields(2).Value.ToString.Trim Then
                    If aline(x, 0) > rs.Fields(1).Value.ToString.Trim And Left(aline(x, 0), 4) <> "WUND" Then  ' this tells us the cas flight is before our Fos flight we need spaces in front
                        If x = 1 Then
                            gridviewrowstart = gridviewrowstart + rs.Fields("cnt").Value + 1 ' if gridviewrowstart > 0 then we will add that many blank lines to first row
                        Else
                            aline(x - 1, 2) = aline(x - 1, 2) + rs.Fields("cnt").Value + 1
                        End If
                        rs.MoveNext()
                    ElseIf aline(x, 0) = rs.Fields(1).Value.ToString.Trim And Left(aline(x, 0), 4) <> "WUND" Then
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

                For zz = 1 To rowstoadd
                    '---------------------- To Insert empty Row At Specific location ------------------------
                    Dim row As New GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Alternate)

                    '   For Each field As DataControlField In GridViewTrips.Columns
                    Dim colstostrip As Integer
                    Select Case carrierid
                        Case WHEELSUP
                            colstostrip = 8
                            Exit Select
                        Case JETLINX
                            colstostrip = 8
                            Exit Select
                        Case Else
                            colstostrip = 8
                    End Select
                    Dim colMax = GridViewTrips.Columns.Count - colstostrip

                    For zzz = 1 To colMax
                        Dim cell As New TableCell
                        cell.Text = "&nbsp;"
                        row.Cells.Add(cell)
                    Next
                    rownumber = rownumber + 1
                    GridViewTrips.Controls(0).Controls.AddAt(rownumber, row) ' This line will insert row at 2nd line
                Next zz
            End If
        Next x
    End Function


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

    Function colorflighttype()
        Dim i As Integer
        For i = 0 To GridViewTrips.Rows.Count - 1


            Dim testme(40) As String
            For zz = 1 To 15
                testme(zz) = zz & ":" & GridViewTrips.Rows(i).Cells(zz).Text
            Next

            Dim dc As Drawing.Color
            Dim dc1 As String

            GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
            Dim x As String = GridViewTrips.Rows(i).Cells(10).Text
            Dim xx As String = GridViewTrips.Rows(i).Cells(12).Text
            Dim xxx As String = GridViewTrips.Rows(i).Cells(14).Text
            If Left(GridViewTrips.Rows(i).Cells(11).Text, 1) = "D" Then GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
            If Left(GridViewTrips.Rows(i).Cells(11).Text, 1) = "R" Then GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkSeaGreen

            '20120724 - pab - highlight mtc trips orange
            If GridViewTrips.Rows(i).Cells(11).Text = "M" Then GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
            'rk 7/20/2013
            If GridViewTrips.Rows(i).Cells(11).Text = "B" Then GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki


            Dim ltc As String = GridViewTrips.Rows(i).Cells(17).Text


            If ltc = "SWAP" Then GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.MediumPurple

            Dim mn As String = GridViewTrips.Rows(i).Cells(6).Text

            Dim nm As String = GridViewTrips.Rows(i).Cells(7).Text

            If IsNumeric(nm) And IsNumeric(mn) Then

                If CInt(nm) <> 0 And CInt(mn) <> 0 Then

                    Dim sp As Integer = Int(nm / mn * 60)
                    GridViewTrips.Rows(i).Cells(7).ToolTip = sp & " NM"

                    If sp > 300 Then GridViewTrips.Rows(i).Cells(7).ForeColor = Color.DarkRed

                End If
            End If

        Next i
    End Function

    Private Sub GridViewTrips_PreRender(sender As Object, e As EventArgs) Handles GridViewTrips.PreRender
        Try
            colorflighttype()
        Catch
        End Try

    End Sub

    Function checkfosflights(carrierid As Integer, ac As String, departureairporticao As String, ArrivalAirportICAO As String, departuredategmt As Date, arrivaldategmet As Date) As String



        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            cn.Open()
        End If



        req = " select * From [OptimizerWest].[dbo].[FOSFlights] Where carrierid = 100 And casupdate > DateAdd(hh, -1, SYSUTCDATETIME()) And ac = 'N838UP' and canceldate = ''  and departureairporticao = 'abcde' and [ArrivalAirportICAO] = 'fghij' and  CAST([DepartureDateGMT] + ' ' + [DepartureTimeGMT] as datetime) = 'depdate'  and  CAST([ArrivalDateGMT] + ' ' + [ArrivaltimeGMT] as datetime) = 'arrdate'"
        req = Replace(req, "100", carrierid)

        req = Replace(req, "abcde", departureairporticao)
        req = Replace(req, "fghij", ArrivalAirportICAO)
        req = Replace(req, "depdate", departuredategmt)
        req = Replace(req, "arrdate", arrivaldategmet)
        req = Replace(req, "N838UP", ac)

        ' req = Replace(req, "123123", foskey)

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)



        If rs.EOF Then
            checkfosflights = "X"

        Else
            checkfosflights = rs.Fields("Version").Value
        End If


        If rs.State = 1 Then rs.Close()


    End Function


    Protected Sub cmdRecovery_Click(sender As Object, e As EventArgs) Handles cmdRecovery.Click

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("cascalendarmodelid")) Then Session("cascalendarmodelid") = ""
        Dim cascalendarmodelid As String = Session("cascalendarmodelid")

        Response.Redirect("panel.aspx?modelrunid=" & normalizemodelrunid(cascalendarmodelid))
    End Sub

    Protected Sub cmdRemoveDH_Click(sender As Object, e As EventArgs) Handles cmdRemoveDH.Click




        Dim batch As String = Now
        Dim totald As Integer = 0

        Dim changecount As Integer = 0
        Dim OptimizerRun As String

        If IsNothing(Session("carrierid")) Then
            lblmsg.Text = "Credentials Lost, Please Sign in Again"
            Exit Sub
        End If

        Dim carrierid As String = Session("carrierid")


        If carrierid = 0 Then
            lblmsg.Text = "Credentials Lost, Please Sign in Again"
            Exit Sub
        End If

        Dim s As String

        Dim a As String


        Dim foskey, version As String

        'For i = 0 To GridViewTrips.Rows.Count - 1

        Dim msg As String = ""

        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        Dim daterangefrom, daterangeto As Date
        If IsNothing(Session("daterangefrom")) Then Session("daterangefrom") = daterangefrom
        If IsNothing(Session("daterangeto")) Then Session("daterangeto") = daterangeto
        daterangefrom = Session("daterangefrom")
        daterangeto = Session("daterangeto")

        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.ReadWriteDriverConnectionString
            cn.Open()
        End If


        Dim rsmaster As New ADODB.Recordset



        'scrub from fosflights to new model



        '  req = "select * from fosflightsoptimizer where   [OptimizerRun] = '8527-5/27/2015-R0-33C' and legtypecode <> 'LINB' " 'and   (ac = 'N506UP' or ac = 'N805UP') and legtypecode <> 'LINB' "
        req = "select *  FROM [OptimizerWest].[dbo].[FOSFlights] where [DepartureDateGMTKey] > = '2015-10-08' and casupdate >  DATEADD (hh , -1 , SYSUTCDATETIME() ) and  canceldate = '' and [DepartureDateGMTKey] < = '2015-10-11'  and deadhead = 'True' and carrierid = avc100"

        ' req = "select * FROM [OptimizerWest].[dbo].[CASFlightsOptimizer] where [OptimizerRun] = '8527-5/27/2015-R16-33C-4' and (aircraftregistration = 'N506UP' or aircraftregistration = 'N805UP')"
        req = Replace(req, "2015-10-08", daterangefrom)
        req = Replace(req, "2015-10-11", daterangeto)
        req = Replace(req, "avc100", carrierid)

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)



        Do While Not rs.EOF

            Dim DepartureAirportCAS As String = rs.Fields("DepartureAirporticao").Value.ToString.Trim
            Dim ArrivalAirportCAS As String = rs.Fields("ArrivalAirporticao").Value.ToString.Trim
            Dim tripnumberCAS As String = rs.Fields("TripNUmber").Value.ToString.Trim
            Dim departuretimecasGMT As Date = CDate(rs.Fields("DepartureDateGMT").Value & " " & rs.Fields("DeparturetimeGMT").Value)
            Dim arrivaltimecasGMT As Date = CDate(rs.Fields("arrivalDateGMT").Value & " " & rs.Fields("arrivaltimeGMT").Value)
            Dim ac As String = rs.Fields("ac").Value.ToString.Trim
            foskey = rs.Fields("foskey").Value.ToString.Trim
            version = rs.Fields("version").Value.ToString.Trim



            If departuretimecasGMT > DateAdd(DateInterval.Hour, 24, Now.ToUniversalTime) Then






                If Left(tripnumberCAS, 1) = "D" Then


                    req = "select * from RejectedFlights where carrierid = '" & "100" & "' and tripnumber = '" & tripnumberCAS & "' and DepartureAirport = '" & DepartureAirportCAS & "' and ArrivalAirport = '" & ArrivalAirportCAS & "'"
                    req = Replace(req, "100", carrierid)

                    Dim rs3 As New ADODB.Recordset

                    If rs3.State = 1 Then rs3.Close()
                    rs3.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

                    Dim sc As String = ""

                    If rs3.EOF Then
                        rs3.AddNew()
                        sc = "rec added 2"
                        changecount = changecount + 1
                    End If


                    rs3.Fields("CarrierId").Value = carrierid
                    '   rs3.Fields("TripNumber").Value = tripnumber
                    rs3.Fields("DepartureAirport").Value = DepartureAirportCAS
                    rs3.Fields("ArrivalAirport").Value = ArrivalAirportCAS
                    rs3.Fields("RejectedOn").Value = Now
                    rs3.Fields("Rejected").Value = True

                    rs3.Fields("action").Value = "Remove"
                    rs3.Fields("TripType").Value = "P"
                    rs3.Fields("TripNumber").Value = tripnumberCAS
                    rs3.Fields("FromDateGMT").Value = departuretimecasGMT
                    rs3.Fields("ToDateGMT").Value = arrivaltimecasGMT
                    rs3.Fields("AircraftRegistration").Value = ac
                    rs3.Fields("PriorTail").Value = ac

                    rs3.Fields("FOSKEY").Value = foskey
                    rs3.Fields("version").Value = version

                    rs3.Fields("statuscomment").Value = sc
                    rs3.Fields("status").Value = "y"
                    rs3.Fields("batch").Value = batch
                    rs3.Update()

                    msg = msg & ("Remove" & "  D:" & DepartureAirportCAS & "  A:" & ArrivalAirportCAS & "  T:" & tripnumberCAS & "<br/>")

                End If
            End If
            rs.MoveNext()

        Loop
        Dim rs5 As New ADODB.Recordset


        req = " delete FROM [OptimizerWest].[dbo].[RejectedFlights] where left(Priortail, 2) = 'WU' and left(aircraftregistration, 2) = 'WU'  and carrierid = 100"

        req = Replace(req, "100", carrierid)



        If rs5.State = 1 Then rs5.Close()
        rs5.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

        totald = totald

        '    a)       Prevent a removal Or add from a departuredate = today

        req = "update RejectedFlights Set status = 'f' where status = 'y'  And cast([FromDateGMT] as date) = cast(getutcdate() as date) and carrierid = " & "100"
        If rs5.State = 1 Then rs5.Close()
        rs5.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

        'b)      Prevent an add for anything beyond COB tomorrow.

        req = "update RejectedFlights Set status = 'e' where status = 'y'   And Action = 'AddLeg'   And cast([FromDateGMT] as date) = cast(dateadd(dd, 1, getutcdate()) as date) and carrierid = " & "100"
        If rs5.State = 1 Then rs5.Close()
        rs5.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


        req = "update RejectedFlights Set status = 'P' where status = 'y' and carrierid = " & "100"
        req = Replace(req, "100", carrierid)



        If rs5.State = 1 Then rs5.Close()
        rs5.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

        lblmsg.Text = changecount & " Changes Accepted "
        lblmsg.ToolTip = msg

    End Sub

    Protected Sub cmdScrubDH_Click(sender As Object, e As EventArgs) Handles cmdScrubDH.Click


        Dim batch As String = Now
        Dim totald As Integer = 0

        Dim changecount As Integer = 0
        Dim OptimizerRun As String

        If IsNothing(Session("carrierid")) Then
            lblmsg.Text = "Credentials Lost, Please Sign in Again"
            Exit Sub
        End If

        Dim carrierid As String = Session("carrierid")


        If carrierid = 0 Then
            lblmsg.Text = "Credentials Lost, Please Sign in Again"
            Exit Sub
        End If

        Dim s As String

        Dim a As String


        Dim foskey, version As String

        'For i = 0 To GridViewTrips.Rows.Count - 1

        Dim msg As String = ""


        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.ReadOnlyDriverConnectionString
            cn.Open()
        End If


        Dim rsmaster As New ADODB.Recordset

        Dim cflights(500, 10) As String
        Dim ccount As Integer = 0


        req = " select * FROM [OptimizerWest].[dbo].[FOSFlights] where carrierid = avc100 and    casupdate >  DATEADD (hh , -1 , SYSUTCDATETIME() ) and  cancelcode = 0 and canceldate = '' and DepartureDateGMTKey >= '2015-10-08'  and DepartureDateGMTKey <= '2015-10-11' order by ac,  CONVERT(datetime, DepartureDategmt + ' ' + DepartureTimegmt) "


        ' req = "select * FROM [OptimizerWest].[dbo].[CASFlightsOptimizer] where [OptimizerRun] = '8527-5/27/2015-R16-33C-4' and (aircraftregistration = 'N506UP' or aircraftregistration = 'N805UP')"

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        Dim daterangeto As Date

        Dim dfrom As String

        dfrom = DateAdd(DateInterval.Day, 1, CDate(Now))
        daterangeto = DateAdd(DateInterval.Day, 6, CDate(Now))

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        Session("daterangeto") = daterangeto

        req = Replace(req, "2015-10-08", dfrom)
        req = Replace(req, "2015-10-11", daterangeto)
        req = Replace(req, "avc100", carrierid)

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


        Dim lastac As String = ""
        Dim lastlt As String = ""
        Dim lt As String = ""

        Do While Not rs.EOF

            ccount = ccount + 1


            If Not (IsDBNull(rs.Fields("DepartureAirporticao").Value)) Then
                If Not (IsDBNull(rs.Fields("ArrivalAirporticao").Value)) Then

                    Dim DepartureAirportCAS As String = rs.Fields("DepartureAirporticao").Value.ToString.Trim
                    Dim ArrivalAirportCAS As String = rs.Fields("ArrivalAirporticao").Value.ToString.Trim
                    Dim tripnumberCAS As String = rs.Fields("TripNUmber").Value.ToString.Trim
                    Dim departuretimecasGMT As Date = CDate(rs.Fields("DepartureDateGMT").Value & " " & rs.Fields("DeparturetimeGMT").Value)
                    Dim arrivaltimecasGMT As Date = CDate(rs.Fields("arrivalDateGMT").Value & " " & rs.Fields("arrivaltimeGMT").Value)
                    Dim ac As String = rs.Fields("ac").Value.ToString.Trim
                    foskey = rs.Fields("foskey").Value.ToString.Trim
                    version = rs.Fields("version").Value.ToString.Trim

                    lt = rs.Fields("legtypecode").Value.ToString.Trim





                    cflights(ccount, 1) = DepartureAirportCAS
                    cflights(ccount, 2) = ArrivalAirportCAS
                    cflights(ccount, 3) = tripnumberCAS
                    cflights(ccount, 4) = departuretimecasGMT
                    cflights(ccount, 5) = arrivaltimecasGMT
                    cflights(ccount, 6) = ac
                    cflights(ccount, 7) = foskey
                    cflights(ccount, 8) = version
                    cflights(ccount, 9) = lt

                End If
            End If



            rs.MoveNext()
        Loop


        For i = 1 To ccount - 1

            Dim ac1, ac2 As String
            ac1 = cflights(i, 6)
            ac2 = cflights(i + 1, 6)
            If ac1 = ac2 Then 'same airplane

                lt = cflights(i, 9)
                Dim nextlt As String = cflights(i + 1, 9)


                Dim deleteme As Boolean = False

                If (lt = "P" Or lt = "D") And (nextlt = "P" Or nextlt = "D") Then
                    deleteme = True 'current is a DH and next is a DH ... double DH
                End If

                If (lt = "P" Or lt = "D") Then
                    If cflights(i, 2) <> cflights(i + 1, 1) Then
                        deleteme = True
                    End If
                End If



                If CDate(cflights(i, 4)) < DateAdd(DateInterval.Hour, 24, Now.ToUniversalTime) Then
                    deleteme = False
                End If


                If deleteme Then


                    Try

                        req = "select * from RejectedFlights where carrierid = '" & "100" & "' and tripnumber = '" & cflights(i, 3) & "' and DepartureAirport = '" & cflights(i, 1) & "' and ArrivalAirport = '" & cflights(i, 2) & "'"
                        req = Replace(req, "100", carrierid)

                        Dim rs55 As New ADODB.Recordset

                        If rs55.State = 1 Then rs55.Close()
                        rs55.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

                        Dim sc As String = ""

                        If rs55.EOF Then
                            rs55.AddNew()
                            sc = "rec added 3"

                        End If

                        changecount = changecount + 1

                        rs55.Fields("CarrierId").Value = carrierid
                        '   rs55.Fields("TripNumber").Value = tripnumber
                        rs55.Fields("DepartureAirport").Value = cflights(i, 1)
                        rs55.Fields("ArrivalAirport").Value = cflights(i, 2)
                        rs55.Fields("RejectedOn").Value = Now
                        rs55.Fields("Rejected").Value = False

                        rs55.Fields("action").Value = "Remove"
                        rs55.Fields("TripType").Value = cflights(i, 9)
                        rs55.Fields("TripNumber").Value = cflights(i, 3)
                        rs55.Fields("FromDateGMT").Value = cflights(i, 4)
                        rs55.Fields("ToDateGMT").Value = cflights(i, 5)
                        rs55.Fields("AircraftRegistration").Value = cflights(i, 6)
                        rs55.Fields("PriorTail").Value = cflights(i, 6)

                        rs55.Fields("FOSKEY").Value = cflights(i, 7)
                        rs55.Fields("version").Value = cflights(i, 8)

                        rs55.Fields("statuscomment").Value = sc
                        rs55.Fields("status").Value = "y"
                        rs55.Fields("batch").Value = batch
                        rs55.Update()

                        msg = msg & ("Remove" & "  D:" & cflights(i, 1) & "  A:" & cflights(i, 2) & "  T:" & cflights(i, 3) & "<br/>")

                    Catch ex As Exception

                        Dim msg1 As String = ex.Message
                        msg = msg


                    End Try




                End If

            End If


        Next i

        Dim rs5 As New ADODB.Recordset



        totald = totald

        '    a)       Prevent a removal Or add from a departuredate = today



        req = "update RejectedFlights Set status = 'c' where status = 'y'  And cast([FromDateGMT] as date) = cast(getutcdate() as date) and carrierid = " & "100"
        If rs5.State = 1 Then rs5.Close()
        rs5.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


        'b)      Prevent an add for anything beyond COB tomorrow.



        req = "update RejectedFlights Set status = 'd' where status = 'y'   And Action = 'AddLeg'   And cast([FromDateGMT] as date) = cast(dateadd(dd, 1, getutcdate()) as date) and carrierid = " & "100"
        If rs5.State = 1 Then rs5.Close()
        rs5.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


        req = "update RejectedFlights Set status = 'P' where status = 'y' and carrierid = " & "100"
        req = Replace(req, "100", carrierid)



        If rs5.State = 1 Then rs5.Close()
        rs5.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


        lblmsg.Text = changecount & " DH Deleted "
        lblmsg.ToolTip = msg

    End Sub

    Function sac(airport As String) As String

        Dim ac As String = Trim(airport)




        If ac = "KNHZ" Then ac = "KBXM"
        Return ac
    End Function



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
                GridViewTrips.Rows.Item(si).Cells(14).BackColor = Drawing.Color.Goldenrod
            Else
                rs.Fields("Pinned").Value = Not (rs.Fields("Pinned").Value)
                rs.Fields("PinnedOn").Value = Date.UtcNow
                GridViewTrips.Rows.Item(si).Cells(2).BackColor = Drawing.Color.White
                GridViewTrips.Rows.Item(si).Cells(14).BackColor = Drawing.Color.White
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

    Protected Sub cmdSlideReport_Click(sender As Object, e As EventArgs) Handles cmdSlideReport.Click
        '   sendemailtemplate("dhackett@coastalavtech.com,rkane@coastalavtech.com", " PrePosition Report " & modelrunid, cmdSlideReport.ToolTip, 100)

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If Not IsNothing(Session("carrierid")) Then
            Select Case CInt(Session("carrierid"))
                Case WHEELSUP
                    sendemailtemplate("dhacket@coastalavtech.com,bill.pickel@gamaaviation.com,mitchell.papontos@gamaaviation.com,jason.oakland@gamaaviation.com,Joshua.Stevens@gamaaviation.com,Valerie.Marma@gamaaviation.com,Anthony.McCoy@gamaaviation.com,Natalie.Caruso@gamaaviation.com,Jason.Baxter@gamaaviation.com,Robert.Petovello@gamaaviation.com,Steven.Saviour@gamaaviation.com", " PrePosition Report " & lblmodelid.Text, cmdSlideReport.ToolTip, 100)
                Case Else
                    sendemailtemplate("dhacket@coastalavtech.com", " PrePosition Report " & lblmodelid.Text, cmdSlideReport.ToolTip, 100)
            End Select
        End If


    End Sub

    Private Sub chkGMT_Load(sender As Object, e As EventArgs) Handles chkGMT.Load

    End Sub

    Protected Sub GridViewTrips_PreRender1(sender As Object, e As EventArgs)

    End Sub
    Partial Class modelstatus
        Private m_finished As Integer
        Private m_tasks As Integer
        Public Property Tasks() As Integer
            Get
                Return m_tasks
            End Get
            Set(ByVal value As Integer)
                m_tasks = value
            End Set
        End Property
        Public Property finishedTasks() As Integer
            Get
                Return m_finished
            End Get
            Set(ByVal value As Integer)
                m_finished = value
            End Set
        End Property
    End Class
End Class
