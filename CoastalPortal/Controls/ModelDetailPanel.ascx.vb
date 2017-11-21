Imports CoastalPortal.Models
Imports CoastalPortal.AirTaxi
Imports System.Collections.Generic
Imports System.Drawing
Imports CoastalPortal.DataAccess

Public Class ModelDetailPanel
    Inherits System.Web.UI.UserControl

    Private CarrierProfile As New CarrierProfile
    Public stest As String
    Public Property calendarcarrierid As String
    Public Property calendarcypher As String
    Private modelrunid As String
    Private db As New OptimizerContext
    Public Shared specsfromlog As Boolean = True

    Public Const FOS_FROM As Integer = 0
    Public Const FOS_TO As Integer = 1
    Public Const FOS_FROMGMT As Integer = 2
    Public Const FOS_TOGMT As Integer = 3
    Public Const FOS_NM As Integer = 4
    Public Const FOS_AC As Integer = 5
    Public Const FOS_TYPE As Integer = 6
    Public Const FOS_COST As Integer = 7
    Public Const FOS_FT As Integer = 8
    Public Const FOS_TRIP As Integer = 9
    Public Const FOS_PIN As Integer = 10
    Public Const FOS_AOG As Integer = 11
    Public Const FOS_LRC As Integer = 12
    Public Const FOS_LPC As Integer = 13
    Public Const FOS_LTC As Integer = 14
    Public Const FOS_PIC As Integer = 15
    Public Const FOS_SIC As Integer = 16
    Public Const FOS_BASE As Integer = 17
    Public Const FOS_WC As Integer = 18
    Public Const FOS_HA As Integer = 19
    Public Const FOS_QE As Integer = 20
    Public Const FUTURE_TAIL As Integer = 21
    Public Const CAS_FROM As Integer = 22
    Public Const CAS_TO As Integer = 23
    Public Const CAS_FROMGMT As Integer = 24
    Public Const CAS_TOGMT As Integer = 25
    Public Const CAS_MIN As Integer = 26
    Public Const CAS_NM As Integer = 27
    Public Const CAS_AC As Integer = 28
    Public Const CAS_TYPE As Integer = 29
    Public Const CAS_COST As Integer = 30
    Public Const CAS_FT As Integer = 31
    Public Const CAS_TRIP As Integer = 32
    Public Const CAS_IND As Integer = 33
    Public Const CAS_GRP As Integer = 34
    Public Const CAS_LRC As Integer = 35
    Public Const CAS_LPC As Integer = 36
    Public Const CAS_LTC As Integer = 37
    Public Const CAS_PIC As Integer = 38
    Public Const CAS_SIC As Integer = 39
    Public Const CAS_BASE As Integer = 40
    Public Const CAS_WC As Integer = 41
    Public Const CAS_PIN As Integer = 42
    Public Const CAS_PR As Integer = 43
    Public Const CAS_HA As Integer = 44
    Public Const CAS_OE As Integer = 45
    Public Const CAS_PT As Integer = 46

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("carrierid")) Then Exit Sub
        Dim ws As New coastalavtech.service.WebService1

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("cascalendarmodelid")) Then Session("cascalendarmodelid") = ""
        Dim cascalendarmodelid As String = Session("cascalendarmodelid")

        calendarcypher = ws.createcypher(Session("defaultemail"))
        calendarcarrierid = Session("carrierid")
        'calendarcypher = "Testcypher"
        RBcalendar.NavigateUrl = "http://169.47.243.167/FlightPlanningOpt.aspx?i="
        RBcalendar.NavigateUrl &= calendarcarrierid & "&c=" & calendarcypher

        Dim MyCarrier As Integer = Session("carrierid")

        CarrierProfile = db.CarrierProfiles.Find(MyCarrier)

        If overridemodel <> "" Then modelrunid = overridemodel
        Try
            If modelrunid = "" Then
                If Not Request.QueryString("modelrunid") Is Nothing Then
                    modelrunid = Request.QueryString("modelrunid")
                    cascalendarmodelid = modelrunid
                    Session("AcceptModelID") = cascalendarmodelid
                End If
            End If

            If Not (IsNothing(Session("airportsearchcode"))) Then
                If Session("airportsearchcode").ToString.Trim <> "" Then
                End If
            End If

            If Not Page.IsPostBack Then

                If Session("AcceptAll") <> True Then
                    radbtnAcceptAll.Enabled = False
                End If

                'FOScmdFindTrip_Click(Nothing, Nothing)
                'CAScmdFindTrip_Click(Nothing, Nothing)
                GetTrips()
                getrunstatus()
                Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx Start ColorME " & Now & " - ", "", "")
                'colorme()
                Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx End ColorME " & Now & " -k - ", "", "")
            End If
            Me.LinkPending.ForeColor = Drawing.Color.Wheat

        Catch ex As Exception

            Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx Load Failure " & ex.Message & "  ", "", "")

        End Try

    End Sub

    Public Sub GetTrips()
        Dim GridViewSource As New List(Of PanelDisplay)
        Dim FosList As New List(Of FOSFlightsOptimizerRecord)
        Dim CasList As New List(Of CASFlightsOptimizerRecord)
        Dim Panellist As New List(Of PanelRecord)
        Dim PanellistRight As New List(Of PanelRecord)
        Dim BaseList As New List(Of String)
        Dim ACList As New List(Of String)
        Dim HAList As New List(Of String)
        Dim mrcustom As String = normalizemodelrunid(modelrunid)

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        Dim carrierid As Integer = CInt(Session("carrierid"))

        FosList = db.FOSFlightsOptimizer.Where(Function(x) x.OptimizerRun = mrcustom).OrderBy(Function(y) y.AC).ThenBy(Function(y) y.DepartureDateGMT).ToList()
        CasList = db.CASFlightsOptimizer.Where(Function(x) x.OptimizerRun = modelrunid).OrderBy(Function(y) y.AircraftRegistration).ThenBy(Function(y) y.DepartureTime).ToList()
        Session("FOS") = FosList
        Session("CAS") = CasList

        Panellist = (From f In FosList Group Join c In CasList On Trim(f.AC) Equals Trim(c.AircraftRegistration) And
                                             Trim(f.ArrivalAirportICAO) Equals Trim(c.ArrivalAirport) And Trim(f.DepartureAirportICAO) Equals Trim(c.DepartureAirport) And
                                          Trim(f.TripNumber) Equals Trim(c.TripNumber) Into Plist = Group From p In Plist.DefaultIfEmpty()
                     Select New PanelRecord With {.CASRecord = p, .FOSRecord = f}).ToList()

        PanellistRight = (From c In CasList Group Join f In FosList On Trim(f.AC) Equals Trim(c.AircraftRegistration) And Trim(f.ArrivalAirportICAO) Equals Trim(c.ArrivalAirport) And
                                                Trim(f.DepartureAirportICAO) Equals Trim(c.DepartureAirport) And Trim(f.TripNumber) Equals Trim(c.TripNumber)
                                                Into Plist = Group From p In Plist.DefaultIfEmpty() Select New PanelRecord With {.FOSRecord = p, .CASRecord = c}).ToList()

        Panellist.Union(PanellistRight).ToList()
        For Each i As PanelRecord In PanellistRight
            If i.FOSRecord Is Nothing Then
                Panellist.Add(i)
            End If
        Next
        If carrierid <> JETLINX Then
            If carrierid = WHEELSUP Then
                ACList = (From a In Panellist Select a.ACType).Distinct().ToList()
                GridViewSource = (From x In ACList Select New PanelDisplay With {.AircraftType = x, .PanelRecord = (From y In Panellist Where y.ACType = x Select y).OrderBy(Function(r) r.TailNumber).ThenBy(Function(r) r.DateTimeGMT).ToList()}).ToList()
                For Each x As PanelDisplay In GridViewSource
                    AirTaxi.awclookup.TryGetValue(Trim(x.AircraftType), x.WeightClass)
                Next
            Else
                ACList = (From a In Panellist Select a.HomeBase).Distinct().ToList()
                GridViewSource = (From x In ACList Select New PanelDisplay With {.AircraftType = x, .PanelRecord = (From y In Panellist Where y.ACType = x Select y).OrderBy(Function(r) r.TailNumber).ThenBy(Function(r) r.DateTimeGMT).ToList()}).ToList()
                For Each x As PanelDisplay In GridViewSource
                    AirTaxi.awclookup.TryGetValue(Trim(x.AircraftType), x.WeightClass)
                Next
            End If
        Else
            ' "order by base, AWC,ac, [FROM GMT] asc  "
            ACList = (From a In Panellist Select a.HomeBase).Distinct().ToList()
            GridViewSource = (From x In ACList Select New PanelDisplay With {.Basecode = x, .PanelRecord = (From y In Panellist Where y.HomeBase = x Select y).OrderBy(Function(r) r.WeightClass).ThenBy(Function(r) r.TailNumber).ThenBy(Function(r) r.DateTimeGMT).ToList()}).ToList()
            For Each x As PanelDisplay In GridViewSource
                AirTaxi.awclookup.TryGetValue(Trim(x.AircraftType), x.WeightClass)
            Next

        End If


        lvflightlist.DataSource = GridViewSource
        lvflightlist.DataBind()

    End Sub
    Protected Sub CAScmdFindTrip_Click(ByVal sender As Object, ByVal e As System.EventArgs) ' Handles cmdFindTrip.Click

        Dim cnlocal As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim cnoptimizer As New ADODB.Connection
        Dim req As String
        Dim databaseFROM As String = "TMCProduction" 'Me.txtDBFrom.Text

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        If IsNothing(Session("cascalendarmodelid")) Then Session("cascalendarmodelid") = ""
        Dim carrierid As Integer = CInt(Session("carrierid"))
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
                Session("daterangeto") = daterangeto
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
            "CAST(A.DepartureDateTimeLocal as datetime) as [From Local], basecode as base," &
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
                s = s & "  order by base, [type], ac, [FROM GMT] asc  "
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
        'BindDataTrips(s)
        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx End BInd " & Now & " - modelrunid not blank - ", "", "")

    End Sub
    Protected Sub FOScmdFindTrip_Click(ByVal sender As Object, ByVal e As System.EventArgs)

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

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = Session("carrierid")

        Try
            If overridemodel = "" Then
                tst = Parent.FindControl("lblcarrier")
                desc = Parent.FindControl("lbldesc")

                If modelrunid <> "" Then
                    If Not (IsNothing(tst.Text)) Then
                        tst.Text = "Model Run: " & modelrunid
                    End If
                End If
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

        s = "SELECT  A.basecode as base, A.id, ltrim(rtrim(A.DepartureAirporticao)) as [FROM] ,ltrim(rtrim(A.ArrivalAirporticao)) as [TO], " &
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
        s = s & CarrierProfile.fosSortOrder

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
        'BindDataTrips(s)
        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx End BInd " & Now & "  ", "", "")

    End Sub

    Public Sub getrunstatus()

        Dim statusSQL As String
        Dim CustomRun As String
        Dim modelcomplete As Boolean = 0
        Dim logcount As Integer
        Dim modelpctcomplete As Integer
        Dim modelsrunsofar As Integer
        Dim tasklist As New List(Of Double)

        CustomRun = Left(modelrunid, InStr(modelrunid, "-") - 1)
        statusSQL = "select declaredcomplete from OptimizerRequest where ID = '" & CustomRun & "'"
        modelcomplete = db.Database.SqlQuery(Of Boolean)(statusSQL).FirstOrDefault()

        If modelcomplete Then
            modelstatusguage.Pointer.Value = 100
            modelstatusguage.Pointer.Color = Color.LightGreen
            modelstatusguage.Scale.Ranges(0).Color = Color.LightGreen
            modelstatusguage.ToolTip = "Model Complete"
        Else
            statusSQL = "select count(*) from sys_log where CustomRunNumber = '" & CustomRun & "'"
            logcount = db.Database.SqlQuery(Of Integer)(statusSQL).FirstOrDefault()

            Try
                statusSQL = "select cast(count(case when [Status]  = 'Z' then 1 end) as decimal(6,3)) As finishedtasks ,count( case when [Status] <> '' then 1 end)) as tasks from OptimizerQ  where left(ModelRunId,5) = '" & CustomRun & "'"
                tasklist = db.Database.SqlQuery(Of Double)(statusSQL).ToList()
                If tasklist IsNot Nothing Then
                    If tasklist.Item(0) > 4 Then
                        modelpctcomplete = CInt((tasklist.Item(0) / tasklist.Item(1)) * 100)
                    Else
                        modelpctcomplete = 0
                    End If
                Else
                    modelpctcomplete = 0
                End If
            Catch ex As Exception
                modelpctcomplete = 0
            End Try

            statusSQL = "select count(*) from optimizerlog where CustomRunNumber = '" & CustomRun & "'"
            modelsrunsofar = db.Database.SqlQuery(Of Integer)(statusSQL).FirstOrDefault()

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

    End Sub
    Function FOScolorme(ByRef gridviewtrips As GridView, ACType As String)

        Dim req As String

        '20120815 - pab - run time improvements
        Dim p1 As Date = CDate(Date.UtcNow.Month & "/" & Date.UtcNow.Day & "/" & Date.UtcNow.Year & " 10:00 AM")
        req = "select id, TripNumber, ltrim(rtrim(AirportFrom)) as AirportFrom, ltrim(rtrim(AirportTo)) as AirportTo, PinnedOn, Pinned from pinned " &
            "where pinned = 1 and pinnedon > '" & p1 & "' order by tripnumber"

        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Get Pinned Start " & Now & "  ", "", "")
        Dim dt As DataTable = DataAccess.GetPinnedFlights(req)
        Insertsys_log(Session("carrierid"), appName, "FOSFlightsAzure.ascx Start Get Pinned End " & Now & "  ", "", "")

        req = "select * from aog " &
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

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = Session("carrierid")



        If ACX(1) <> "" Then
            gridviewtrips.Width = 900
            TopStats.Style.Clear()
            TopStats.Style.Add("visible", "false")
            TopStats.Visible = False
        Else
            gridviewtrips.Width = 1000
            TopStats.Style.Clear()
        End If

        Dim i As Integer
        For i = 0 To gridviewtrips.Rows.Count - 1
            If gridviewtrips.Rows(i).Cells(8).Text <> "" Then
                gridviewtrips.Rows(i).Cells(8).ToolTip = AirTaxi.lookupac(gridviewtrips.Rows(i).Cells(8).Text, carrierid)
            End If

            If gridviewtrips.Rows(i).Cells(0).Text = "KMMU" Then
                i = i
            End If

            Dim x As String = DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text
            Select Case Left(DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text, 1)
                Case "D", "True"
                    DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat
                    DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "D"
                Case "R", "False"
                    DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.DarkSeaGreen
                    DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "R"


                    If gridviewtrips.Rows(i).Cells(17).Text = "CREW" Then
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.SteelBlue
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "C"
                    End If

                    If gridviewtrips.Rows(i).Cells(17).Text = "MXSC" Or gridviewtrips.Rows(i).Cells(17).Text = "SWAP" Then     'SWAP added by mws 11/25
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.SteelBlue
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "M"
                    End If

                    If gridviewtrips.Rows(i).Cells(17).Text = "M" Then
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.SteelBlue
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "M"
                    End If

                    If gridviewtrips.Rows(i).Cells(17).Text = "LMX" Then
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.SteelBlue
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "M"
                    End If

                    If gridviewtrips.Rows(i).Cells(17).Text = "Y" Then
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.Orange
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "S"
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).ToolTip = "Static"
                    End If

                Case Else
                    If Trim(gridviewtrips.Rows(i).Cells(12).Text) = "12345" Then
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.DarkOrange
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "M"
                    Else
                        DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.SteelBlue
                    End If
            End Select


            If Trim(gridviewtrips.Rows(i).Cells(12).Text) = "12345" Then
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.DarkOrange
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "M"
            End If

            Dim lrc As String = gridviewtrips.Rows(i).Cells(14).Text.ToString.Trim
            Dim lpc As String = gridviewtrips.Rows(i).Cells(15).Text
            Dim ltc As String = gridviewtrips.Rows(i).Cells(16).Text
            '   Dim ltcx As String = GridViewTrips.Rows(i).Cells(14).Text

            Dim testme(40) As String
            For zz = 1 To 15
                testme(zz) = gridviewtrips.Rows(i).Cells(zz).Text
            Next


            If ltc = "AOG" Or ltc = "DETL" Or ltc = "INSV" Or ltc = "MXRC" Or ltc = "MXUS" Or ltc = "MAINT" Then
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.DarkOrange
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "M"
            End If

            If ltc = "77" Or ltc = "M" Then
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.SteelBlue
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "M"
            End If


            If ltc = "77" Or ltc = "MXSC" Or ltc = "SWAP" Then 'SWAP added by mws 11/25
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.SteelBlue
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "M"
            End If

            If ltc = "Y" Or ltc = "S" Then
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.Orange
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "S"
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).ToolTip = "Static"
            End If

            If ltc = "BULP" Then
                gridviewtrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(11).Text = "B"
            End If

            If ltc = "CREW" Then
                gridviewtrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "C"
            End If

            If ltc = "TRNG" Then
                gridviewtrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "T"
            End If

            If ltc = "LINB" Then
                gridviewtrips.Rows(i).Cells(14).ForeColor = Drawing.Color.DarkRed
                gridviewtrips.Rows(i).Cells(15).ForeColor = Drawing.Color.DarkRed
                gridviewtrips.Rows(i).Cells(16).ForeColor = Drawing.Color.DarkRed
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text = "L"
                DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).BackColor = Drawing.Color.LightGreen
            End If

            Dim account As Integer

            If ACX(1) <> "" Then mycolor = Drawing.Color.White
            If i > 0 Then
                gridviewtrips.Rows(i).Cells(8).Font.Bold = False
                If gridviewtrips.Rows(i).Cells(8).Text <> gridviewtrips.Rows(i - 1).Cells(8).Text Then 'ac change
                    '   GridViewTrips.Rows(i).Cells(8).BackColor = Drawing.Color.CadetBlue

                    gridviewtrips.Rows(i).Cells(8).Font.Bold = True

                    If ACX(1) <> "" Then mycolor = Drawing.Color.White
                    If ACX(1) = "" Then
                        account = account + 1
                        If account / 2 = Int(account / 2) Then
                            mycolor = Drawing.Color.Aquamarine
                        Else
                            mycolor = Drawing.Color.White
                        End If
                    End If
                    If ACX(1) <> "" Then mycolor = Drawing.Color.White

                Else
                    If gridviewtrips.Rows(i).Cells(0).Text <> gridviewtrips.Rows(i - 1).Cells(1).Text Then
                        If gridviewtrips.Rows(i).Cells(0).Text <> "SWAP" And gridviewtrips.Rows(i - 1).Cells(1).Text <> "SWAP" Then
                            gridviewtrips.Rows(i).Cells(0).BackColor = Drawing.Color.IndianRed
                            gridviewtrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.IndianRed

                            If specsfromlog = False Then
                                If IsNumeric(Me.lblnonrevenuelegs.Text) Then
                                    Me.lblnonrevenuelegs.Text = CInt(Me.lblnonrevenuelegs.Text) + 1
                                    Me.lblnonrevenuemiles.Text = CInt(Me.lblnonrevenuemiles.Text) + 1000
                                    Me.lblnonrevenueexpense.Text = CInt(Me.lblnonrevenueexpense.Text) + 4200
                                    Me.lbltotalrevenueexpense.Text = Me.lbltotalrevenueexpense.Text + 4200
                                End If
                            End If
                        Else
                            gridviewtrips.Rows(i).Cells(0).BackColor = Drawing.Color.MediumPurple
                            gridviewtrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.MediumPurple
                        End If
                    End If

                    Dim prevflight As Date = CDate(convdate(gridviewtrips.Rows(i - 1).Cells(3).Text))
                    Dim currflight As Date = CDate(convdate(gridviewtrips.Rows(i).Cells(2).Text))
                    Dim diff As Integer = DateDiff(DateInterval.Minute, prevflight, currflight)

                    If diff < 30 Then gridviewtrips.Rows(i - 1).Cells(3).BackColor = Drawing.Color.Yellow
                    If diff < 30 Then gridviewtrips.Rows(i).Cells(2).BackColor = Drawing.Color.Yellow
                    If diff < 0 Then gridviewtrips.Rows(i - 1).Cells(3).BackColor = Drawing.Color.Salmon
                    If diff < 0 Then gridviewtrips.Rows(i).Cells(2).BackColor = Drawing.Color.Salmon
                End If
            End If

            gridviewtrips.Rows(i).BackColor = mycolor

            Dim pinned As Boolean = False
            If bpinnedflights = True Then
                dv_pinned.RowFilter = "TripNumber = '" & gridviewtrips.Rows.Item(i).Cells(12).Text & "' and AirportFrom = '" &
                    gridviewtrips.Rows.Item(i).Cells(0).Text.ToString & "' and AirportTo = '" & gridviewtrips.Rows.Item(i).Cells(1).Text.ToString & "'"
                If dv_pinned.Count > 0 Then
                    pinned = True
                End If
            End If

            If pinned Then
                gridviewtrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.Goldenrod
                gridviewtrips.Rows.Item(i).Cells(12).BackColor = Drawing.Color.Goldenrod
                gridviewtrips.Rows.Item(i).Cells(13).BackColor = Drawing.Color.Goldenrod 'rk 4.3.2013 change from 13 to 10 to show pinned field
            End If

            'rk 8.24.2013 back up auto pinned 3 hours
            If pinned = False Then
                If IsDate(gridviewtrips.Rows(i).Cells(2).Text) Then
                    Dim gmt As Date = DateTime.UtcNow
                    Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(2).Text)

                    gridviewtrips.Rows(i).Cells(2).Font.Underline = False
                    gridviewtrips.Rows(i).Cells(4).Font.Underline = False

                    If gmt > DateAdd(DateInterval.Minute, -180, departgmt) Then

                        gridviewtrips.Rows(i).Cells(2).Font.Underline = True
                        gridviewtrips.Rows(i).Cells(4).Font.Underline = True

                    End If
                End If
            End If

            'show aog flights
            Dim aog As Boolean = False
            If bAOGflights = True Then
                dv_aog.RowFilter = "Aircraft = '" & gridviewtrips.Rows.Item(i).Cells(8).Text & "'"
                If dv_aog.Count > 0 Then
                    aog = True
                End If
            End If

            If aog = False Then
                '   GridViewTrips.Rows.Item(i).Cells(8).BackColor = Drawing.Color.White

            Else
                gridviewtrips.Rows.Item(i).Cells(8).BackColor = Drawing.Color.Violet

            End If

            For z = 2 To 5
                If IsDate(gridviewtrips.Rows(i).Cells(z).Text) Then
                    Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(z).Text)
                    gridviewtrips.Rows(i).Cells(z).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
                End If
            Next z

            If Session("defaultemail") = "demo@cas.com" Then
                Dim a As String = gridviewtrips.Rows.Item(i).Cells(8).Text.ToString()

                Dim b As String
                b = ""
                For z = 1 To Len(a)
                    b = b & Chr(Asc(Mid(a, z, 1)) + 2)
                Next
                a = b
                gridviewtrips.Rows.Item(i).Cells(8).Text = b
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
        Dim endofgrid As Integer = gridviewtrips.Rows.Count - 1

        Dim ii As Integer

        For ii = 0 To gridviewtrips.Rows.Count - 1

            Dim a13, a8 As String
            a13 = gridviewtrips.Rows(ii).Cells(13).Text
            a8 = gridviewtrips.Rows(ii).Cells(8).Text

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

            departureairport = gridviewtrips.Rows(ii).Cells(0).Text
            arrivalairport = gridviewtrips.Rows(ii).Cells(1).Text

            If getnewcwpair Then
                cwarrivalport = arrivalairport
                cwdepartureport = departureairport
                getnewcwpair = False
            End If

            If ii <> endofgrid Then
                If DirectCast(gridviewtrips.Rows(ii).FindControl("fosFT"), Label).Text = "M" Or gridviewtrips.Rows(ii).Cells(16).Text = "MXSC" Or gridviewtrips.Rows(ii).Cells(16).Text = "SWAP" Or gridviewtrips.Rows(ii).Cells(16).Text = "NC" Then 'SWAP and NC added by mws 11/25

                    getnewcwpair = True
                    startofduty = addyear(gridviewtrips.Rows(ii + 1).Cells(2).Text)
                    If gridviewtrips.Rows(ii).Cells(16).Text = "SWAP" Then
                        startofduty = DateAdd(DateInterval.Hour, -7, startofduty)  'added by mws 11/27/16 .. all crew need to show 1 hour early
                    Else
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty)  'added by mws 11/27/16 .. all crew need to show 1 hour early
                    End If
                    If Left(ACType, 4) = "B350" Then
                        startofduty = DateAdd(DateInterval.Minute, -30, startofduty)  'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                    End If

                End If
            End If

            If ii > 0 Then
                If departureairport <> arrivalairport Then
                    If gridviewtrips.Rows(ii).Cells(8).Text <> gridviewtrips.Rows(ii - 1).Cells(8).Text Then 'ac change
                        getnewcwpair = True
                        startofduty = addyear(gridviewtrips.Rows(ii).Cells(2).Text)
                        Dim t As String = gridviewtrips.Rows(ii).Cells(9).Text
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty) 'modified by mws 11/27/16 all show is 60 minutes
                        If Left(ACType, 4) = "B350" Then
                            startofduty = DateAdd(DateInterval.Minute, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                        End If
                    End If
                End If
            End If

            'reset duty day if crew change
            If ii > 0 Then
                If departureairport <> arrivalairport Then
                    If gridviewtrips.Rows(ii).Cells(8).Text = gridviewtrips.Rows(ii - 1).Cells(8).Text Then 'no ac change
                        If DirectCast(gridviewtrips.Rows(ii).FindControl("fosPIC"), Label).Text.Trim <> "" Then  'make sure current and prev PIC and SIC not blank
                            If DirectCast(gridviewtrips.Rows(ii).FindControl("fosSIC"), Label).Text.Trim <> "" Then
                                If DirectCast(gridviewtrips.Rows(ii - 1).FindControl("fosPIC"), Label).Text.Trim <> "" Then
                                    If DirectCast(gridviewtrips.Rows(ii - 1).FindControl("fosSIC"), Label).Text.Trim <> "" Then
                                        If DirectCast(gridviewtrips.Rows(ii - 1).FindControl("fosPIC"), Label).Text.Trim <> DirectCast(gridviewtrips.Rows(ii).FindControl("fosPIC"), Label).Text.Trim Then 'if change of PIC
                                            If DirectCast(gridviewtrips.Rows(ii - 1).FindControl("fosSIC"), Label).Text.Trim <> DirectCast(gridviewtrips.Rows(ii).FindControl("fosSIC"), Label).Text.Trim Then ' if change of SIC
                                                getnewcwpair = True
                                                startofduty = addyear(gridviewtrips.Rows(ii).Cells(2).Text)
                                                startofduty = DateAdd(DateInterval.Hour, -1, startofduty) ' modified by mws 11/27/16 all crew need 60 minute show
                                                If Left(ACType, 4) = "B350" Then
                                                    startofduty = DateAdd(DateInterval.Hour, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
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


            i = i + 1


            departuretime = addyear(gridviewtrips.Rows(ii).Cells(2).Text)
            arrivaltime = addyear(gridviewtrips.Rows(ii).Cells(3).Text)

            Dim ri As Integer = 11.99

            dutyday = dutyday

            If i > 1 Then
                If departureairport <> arrivalairport Then

                    restinterval = DateDiff(DateInterval.Minute, prevarrival, departuretime) + 60
                    restinterval = restinterval
                    restinterval = restinterval / 60

                    'if the rest interval is ok then advance start of duty
                    If restinterval > ri Then
                        getnewcwpair = True
                        startofduty = addyear(gridviewtrips.Rows(ii).Cells(2).Text)
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty) ' modified by mws 11/27/16 all crew need 60 minute show
                        If Left(ACType, 4) = "B350" Then
                            startofduty = DateAdd(DateInterval.Minute, -30, startofduty) 'modified by mws 11/27/16 changed hour -1.5 to minute -30 B350 need extra 30 minute warmup
                        End If
                    End If

                    dutyday = DateDiff(DateInterval.Minute, startofduty, arrivaltime) + 30
                    dutyday = dutyday / 60

                    If dutyday > 11 Then
                        fail = True
                        gridviewtrips.Rows(ii).Cells(18).BackColor = Drawing.Color.YellowGreen
                        gridviewtrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Warning: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If
                    If dutyday > 12 Then
                        fail = True
                        gridviewtrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Yellow
                        gridviewtrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Concern: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If

                    If dutyday > 13 Then
                        fail = True
                        gridviewtrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Red
                        gridviewtrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Alert: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If
                End If

            End If

            If departureairport <> arrivalairport Then 'rk 10/12/13 only count prior arrival if it is really a flight
                prevarrival = addyear(gridviewtrips.Rows(ii).Cells(3).Text)
            End If

        Next ii

        If gridviewtrips.Rows.Count > 5 Then

            'linemeup()
        End If
        If ACX(1) <> "" Then
            For z = 11 To gridviewtrips.Columns.Count - 1
                gridviewtrips.Columns(z).Visible = False
            Next z
        End If
        For Each dcf As DataControlField In gridviewtrips.Columns
            Select Case dcf.HeaderText.ToUpper
                Case "P&L"
                    If carrierid = WHEELSUP Then dcf.Visible = True

                Case "OE"
                    If carrierid <> WHEELSUP Then dcf.Visible = True
                Case "HA"
                    dcf.Visible = carrierid <> WHEELSUP
            End Select
        Next
    End Function

    Function CAScolorme(ByRef gridviewtrips As GridView)

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

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = Session("carrierid")


        Dim dv_pinned As DataView = dt.DefaultView
        Dim bpinnedflights As Boolean = False

        If Not AirTaxi.isdtnullorempty(dt) Then
            bpinnedflights = True
        End If


        Dim d, r As Double

        If ACX(1) <> "" Then
            gridviewtrips.Width = 1000
            TopStats.Style.Clear()
            TopStats.Style.Add("visible", "false")
            TopStats.Visible = False
        Else
            gridviewtrips.Width = 1000
            TopStats.Style.Clear()
        End If

        Dim i As Integer
        For i = 0 To gridviewtrips.Rows.Count - 1

            '   Dim dccolor As Drawing.Color = Drawing.Color.White

            'Following 3 lines are for CAS
            'Dim a13, a8 As String
            'a13 = DirectCast(gridviewtrips.Rows(i).FindControl("fosPIN"), Label).Text
            'a8 = gridviewtrips.Rows(i).Cells(8).Text

            Dim lb As LinkButton
            lb = gridviewtrips.Rows(i).FindControl("lnkDepartmentClientSide")

            gridviewtrips.Rows(i).Cells(14).BackColor = Drawing.Color.White

            If Not (IsNothing(lb)) Then
                If Trim(lb.Text) = Trim(gridviewtrips.Rows(i).Cells(8).Text) Then
                    gridviewtrips.Rows(i).Cells(14).Text = ""
                    gridviewtrips.Rows(i).Cells(15).Text = ""
                    lb.Text = ""
                    lb.BackColor = Drawing.Color.Beige
                End If
            End If

            If Trim(gridviewtrips.Rows(i).Cells(14).Text) <> "" Then
                gridviewtrips.Rows(i).Cells(14).BackColor = Drawing.Color.Pink

                Dim dd As String = gridviewtrips.Rows(i).Cells(2).Text
                Dim dd1 As String = gridviewtrips.Rows(i).Cells(2).Text
                Dim ispace As Integer
                ispace = InStr(dd, " ")

                If Not (IsDate(dd)) Then
                    dd = Trim(Left(dd, ispace)) & "/" & Now.Year & " " & Right(dd, Len(dd) - ispace)
                End If
                If IsDate(dd) Then
                    Dim priordate As Date = CDate(dd)
                    Dim dC As Integer = DateDiff(DateInterval.Day, Now.ToUniversalTime, priordate)

                    lb.Font.Bold = True

                    If dC > 3 Then gridviewtrips.Rows(i).Cells(14).BackColor = Drawing.Color.LightBlue
                    If dC = 3 Then gridviewtrips.Rows(i).Cells(14).BackColor = Drawing.Color.SlateBlue
                    If dC = 2 Then gridviewtrips.Rows(i).Cells(14).BackColor = Drawing.Color.SkyBlue
                    If dC = 1 Then gridviewtrips.Rows(i).Cells(14).BackColor = Drawing.Color.DeepSkyBlue
                    If dC < 1 Then gridviewtrips.Rows(i).Cells(14).BackColor = Drawing.Color.DodgerBlue

                    If dC > 3 Then lb.BackColor = Drawing.Color.LightBlue
                    If dC = 3 Then lb.BackColor = Drawing.Color.SlateBlue
                    If dC = 2 Then lb.BackColor = Drawing.Color.SkyBlue
                    If dC = 1 Then lb.BackColor = Drawing.Color.DeepSkyBlue
                    If dC < 1 Then lb.BackColor = Drawing.Color.DodgerBlue
                End If

            End If

            Dim xy As String = gridviewtrips.Rows(i).Cells(17).Text
            Dim x As String = gridviewtrips.Rows(i).Cells(17).Text
            Select Case Left(gridviewtrips.Rows(i).Cells(17).Text, 1)
                Case "D", "True"
                    gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat
                    gridviewtrips.Rows(i).Cells(11).Text = "D"
                Case "R", "False"
                    'revenue
                    gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkSeaGreen
                    gridviewtrips.Rows(i).Cells(11).Text = "R"
                    If gridviewtrips.Rows(i).Cells(17).Text = "CREW" Then
                        gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        gridviewtrips.Rows(i).Cells(11).Text = "C"
                    End If
                    If gridviewtrips.Rows(i).Cells(17).Text = "M" Then
                        gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        gridviewtrips.Rows(i).Cells(11).Text = "M"
                    End If
                    If gridviewtrips.Rows(i).Cells(17).Text = "MXSC" Or gridviewtrips.Rows(i).Cells(17).Text = "SWAP" Then 'SWAP added by mws 11/25
                        gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                        gridviewtrips.Rows(i).Cells(11).Text = "M"
                    End If
                    If gridviewtrips.Rows(i).Cells(17).Text = "P" Then
                        gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
                        gridviewtrips.Rows(i).Cells(11).Text = "D"
                    End If
                    If gridviewtrips.Rows(i).Cells(17).Text = "Y" Then
                        gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.Orange
                        gridviewtrips.Rows(i).Cells(11).Text = "S"
                        gridviewtrips.Rows(i).Cells(11).ToolTip = "Static"
                    End If
                Case Else
                    If Trim(gridviewtrips.Rows(i).Cells(12).Text) = "12345" Then
                        gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkOrange
                        gridviewtrips.Rows(i).Cells(11).Text = "M"
                    End If
            End Select

            Dim lrc As String = gridviewtrips.Rows(i).Cells(15).Text
            Dim lpc As String = gridviewtrips.Rows(i).Cells(16).Text
            Dim ltc As String = gridviewtrips.Rows(i).Cells(17).Text


            If ltc = "MXSC" Or ltc = "AOG" Or ltc = "M" Or ltc = "DETL" Or ltc = "INSV" Or ltc = "MXRC" Or ltc = "SWAP" Or ltc = "MXUS" Or ltc = "MAINT" Then 'SWAP added by mws 11/25
                gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                gridviewtrips.Rows(i).Cells(11).Text = "M"
            End If

            If ltc = "77" Then
                gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
                gridviewtrips.Rows(i).Cells(11).Text = "M"
            End If

            'rk 7/20/2013
            If ltc = "BULP" Then
                gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(11).Text = "B"
            End If


            'rk 7/20/2013
            If ltc = "CREW" Then
                gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(11).Text = "C"
            End If


            If ltc = "TRNG" Then
                gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(11).Text = "T"
            End If

            If gridviewtrips.Rows(i).Cells(11).Text = "D" Then gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
            If ACX(1) <> "" Then mycolor = Drawing.Color.White            'Flight Change 
            If i > 0 Then
                gridviewtrips.Rows(i).Cells(8).Font.Bold = False
                If gridviewtrips.Rows(i).Cells(8).Text <> gridviewtrips.Rows(i - 1).Cells(8).Text Then 'ac change
                    '   GridViewTrips.Rows(i).Cells(8).BackColor = Drawing.Color.CadetBlue
                    gridviewtrips.Rows(i).Cells(8).Font.Bold = True
                    account = account + 1
                    If ACX(1) = "" Then ' added for Flight Change just this if
                        If account / 2 = Int(account / 2) Then
                            mycolor = Drawing.Color.Aquamarine
                        Else
                            mycolor = Drawing.Color.White
                        End If
                    End If
                    If ACX(1) <> "" Then mycolor = Drawing.Color.White 'Flight Change
                Else
                    If gridviewtrips.Rows(i).Cells(0).Text <> gridviewtrips.Rows(i - 1).Cells(1).Text Then
                        If gridviewtrips.Rows(i).Cells(0).Text <> "SWAP" And gridviewtrips.Rows(i - 1).Cells(1).Text <> "SWAP" Then
                            gridviewtrips.Rows(i).Cells(0).BackColor = Drawing.Color.IndianRed
                            gridviewtrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.IndianRed
                            caslinebreaks = caslinebreaks + 1
                        Else
                            gridviewtrips.Rows(i).Cells(0).BackColor = Drawing.Color.MediumPurple
                            gridviewtrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.MediumPurple
                        End If
                    End If

                    Dim prevflight As Date = CDate(convdate(gridviewtrips.Rows(i - 1).Cells(3).Text))
                    Dim currflight As Date = CDate(convdate(gridviewtrips.Rows(i).Cells(2).Text))
                    Dim diff As Integer = DateDiff(DateInterval.Minute, prevflight, currflight)

                    If diff < 30 Then gridviewtrips.Rows(i - 1).Cells(3).BackColor = Drawing.Color.Yellow
                    If diff < 30 Then gridviewtrips.Rows(i).Cells(2).BackColor = Drawing.Color.Yellow
                    If diff < 0 Then gridviewtrips.Rows(i - 1).Cells(3).BackColor = Drawing.Color.Salmon
                    If diff < 0 Then gridviewtrips.Rows(i).Cells(2).BackColor = Drawing.Color.Salmon
                End If
            End If
            gridviewtrips.Rows(i).BackColor = mycolor

            Dim pinned As Boolean = False
            If bpinnedflights = True Then
                dv_pinned.RowFilter = "TripNumber = '" & gridviewtrips.Rows.Item(i).Cells(12).Text & "' and AirportFrom = '" &
                    gridviewtrips.Rows.Item(i).Cells(0).Text.ToString & "' and AirportTo = '" & gridviewtrips.Rows.Item(i).Cells(1).Text.ToString & "'"
                If dv_pinned.Count > 0 Then
                    pinned = True
                End If
            End If

            If pinned Then
                gridviewtrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.Goldenrod
            End If

            If IsDate(gridviewtrips.Rows(i).Cells(2).Text) Then

                Dim gmt As Date = DateTime.UtcNow
                Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(2).Text)
            End If

            For z = 2 To 5
                If IsDate(gridviewtrips.Rows(i).Cells(z).Text) Then
                    Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(z).Text)
                    gridviewtrips.Rows(i).Cells(z).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
                End If
            Next z

            If Session("defaultemail") = "demo@cas.com" Then
                Dim a As String = gridviewtrips.Rows.Item(i).Cells(8).Text.ToString()

                Dim b As String
                b = ""

                For z = 1 To Len(a)
                    b = b & Chr(Asc(Mid(a, z, 1)) + 2)
                Next
                a = b
                gridviewtrips.Rows.Item(i).Cells(8).Text = b
                gridviewtrips.Rows.Item(i).Cells(14).Text = b
                gridviewtrips.Rows.Item(i).Cells(15).Text = b
            End If
        Next i

        Me.lblLineBreaks.Text = caslinebreaks

        If caslinebreaks <> 0 Then
            Me.lblLineBreaks.BackColor = Drawing.Color.Red
        Else
            Me.lblLineBreaks.BackColor = Drawing.Color.White
        End If



        Dim movedhemail As String = ""
        Dim endofgrid As Integer = gridviewtrips.Rows.Count - 1
        Dim getnewcwpair As Boolean = True 'Flags when a crew warning pair needs updating  mws 12/5/16
        Dim cwdepartureport As String 'crew warning message departure airport mws 12/5/16
        Dim cwarrivalport As String 'crew warning message arrival airport mws 12/5/16
        Dim Swapflightfail As Boolean = False
        Dim swapevent As Boolean = False
        Dim swapwarntext As String
        Dim swapflightpreptime As Integer


        Dim ii As Integer
        Dim startofdutyflight As Integer = 0

        For ii = 0 To gridviewtrips.Rows.Count - 1

            Dim a13, a8 As String
            a13 = gridviewtrips.Rows(ii).Cells(14).Text
            a8 = gridviewtrips.Rows(ii).Cells(8).Text

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

            departureairport = gridviewtrips.Rows(ii).Cells(0).Text
            arrivalairport = gridviewtrips.Rows(ii).Cells(1).Text
            If getnewcwpair Then
                cwarrivalport = arrivalairport
                cwdepartureport = departureairport
                getnewcwpair = False
            End If
            Dim t11, t16 As String
            t11 = gridviewtrips.Rows(ii).Cells(11).Text
            t16 = gridviewtrips.Rows(ii).Cells(17).Text

            'reset counter if MX event, assume crew change or rest achieved.
            If ii <> endofgrid Then
                If gridviewtrips.Rows(ii).Cells(11).Text = "M" Or gridviewtrips.Rows(ii).Cells(17).Text = "MXSC" Or gridviewtrips.Rows(ii).Cells(17).Text = "SWAP" Or gridviewtrips.Rows(ii).Cells(17).Text = "NC" Then 'SWAP and NC added by mws 11/25
                    If gridviewtrips.Rows(ii).Cells(17).Text = "SWAP" Then
                        startofduty = addyear(gridviewtrips.Rows(ii).Cells(3).Text) 'added by mws 11/27/2016
                        Swapoffset = DateDiff(DateInterval.Minute, addyear(gridviewtrips.Rows(ii).Cells(3).Text), addyear(gridviewtrips.Rows(ii).Cells(2).Text))
                        If Swapoffset < 7 Then
                            Swapoffset = 3
                        ElseIf Swapoffset < 10 Then
                            Swapoffset = 2
                        ElseIf Swapoffset < 15 Then
                            Swapoffset = 1
                        Else
                            Swapoffset = 0
                        End If
                        If gridviewtrips.Rows(ii).Cells(8).Text = gridviewtrips.Rows(ii + 1).Cells(8).Text Then
                            Swapflightfail = False
                            swapflightpreptime = DateDiff(DateInterval.Minute, addyear(gridviewtrips.Rows(ii).Cells(3).Text), addyear(gridviewtrips.Rows(ii + 1).Cells(2).Text))
                            If swapflightpreptime < 60 Then Swapflightfail = True
                            swapevent = True
                        End If
                    Else
                        swapevent = False
                        Swapoffset = 0
                        Swapflightfail = False
                        startofduty = addyear(gridviewtrips.Rows(ii + 1).Cells(2).Text)
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty)  'added by mws 11/27/16 .. all crew need to show 1 hour early
                    End If
                    startofdutyflight = ii + 1
                    getnewcwpair = True
                    If Left(gridviewtrips.Rows(ii + 1).Cells(9).Text, 4) = "B350" Then
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
                    If gridviewtrips.Rows(ii).Cells(8).Text <> gridviewtrips.Rows(ii - 1).Cells(8).Text Then 'ac change

                        startofduty = addyear(gridviewtrips.Rows(ii).Cells(2).Text)
                        Dim t As String = gridviewtrips.Rows(ii).Cells(9).Text
                        startofdutyflight = ii
                        swapevent = False
                        getnewcwpair = True
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty) 'modified by mws 11/27/16 all show is 60 minutes
                        If Left(gridviewtrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
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
                    If gridviewtrips.Rows(ii).Cells(8).Text = gridviewtrips.Rows(ii - 1).Cells(8).Text Then 'no ac change
                        If gridviewtrips.Rows(ii).Cells(18).Text.Trim <> "" Then  'make sure current and prev PIC and SIC not blank
                            If gridviewtrips.Rows(ii).Cells(19).Text.Trim <> "" Then
                                If gridviewtrips.Rows(ii - 1).Cells(18).Text.Trim <> "" Then
                                    If gridviewtrips.Rows(ii - 1).Cells(19).Text.Trim <> "" Then
                                        If gridviewtrips.Rows(ii - 1).Cells(18).Text.Trim <> gridviewtrips.Rows(ii).Cells(18).Text.Trim Then 'if change of PIC
                                            If gridviewtrips.Rows(ii - 1).Cells(19).Text.Trim <> gridviewtrips.Rows(ii).Cells(19).Text.Trim Then ' if change of SIC
                                                startofduty = addyear(gridviewtrips.Rows(ii).Cells(2).Text)
                                                getnewcwpair = True
                                                swapevent = False
                                                startofdutyflight = ii
                                                startofduty = DateAdd(DateInterval.Hour, -1, startofduty) ' modified by mws 11/27/16 all crew need 60 minute show
                                                If Left(gridviewtrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
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

            departuretime = addyear(gridviewtrips.Rows(ii).Cells(2).Text)
            arrivaltime = addyear(gridviewtrips.Rows(ii).Cells(3).Text)


            Dim ri As Integer = 10

            dutyday = dutyday

            If i > 1 Then
                If departureairport <> arrivalairport Then

                    restinterval = DateDiff(DateInterval.Minute, prevarrival, departuretime) + 60
                    restinterval = restinterval
                    restinterval = restinterval / 60

                    'if the rest interval is ok then advance start of duty
                    If restinterval > ri Then
                        startofduty = addyear(gridviewtrips.Rows(ii).Cells(2).Text)
                        getnewcwpair = True
                        swapevent = False
                        startofdutyflight = ii
                        startofduty = DateAdd(DateInterval.Hour, -1, startofduty) ' modified by mws 11/27/16 all crew need 60 minute show
                        If Left(gridviewtrips.Rows(ii).Cells(9).Text, 4) = "B350" Then
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
                        gridviewtrips.Rows(ii).Cells(18).BackColor = Drawing.Color.GreenYellow
                        gridviewtrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Warning: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If


                    If dutyday > 12 Then
                        fail = True
                        gridviewtrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Yellow
                        gridviewtrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Concern: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If



                    If dutyday > 13 Then
                        fail = True
                        gridviewtrips.Rows(ii).Cells(18).BackColor = Drawing.Color.YellowGreen
                        gridviewtrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Alert: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport

                    End If


                    If dutyday > 14 Or (dutyday > (7 + Swapoffset) And swapevent = True) Or (Swapflightfail) Then
                        gridviewtrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Red
                        swapwarntext = ""
                        If Swapflightfail Then
                            swapwarntext = swapflightpreptime & " Minutes is Not enough time to prep flight after SWAP"
                            gridviewtrips.Rows(ii).Cells(18).BackColor = Drawing.Color.Orange
                        End If
                        If swapevent And Not Swapflightfail Then
                            swapwarntext = "SWAP Event"
                            swapevent = False
                        End If
                        fail = True
                        If Swapflightfail Then
                            gridviewtrips.Rows(ii).Cells(18).ToolTip = "Flight Prep Alert:  " & departureairport & " " & arrivalairport & " " & swapwarntext
                        Else
                            gridviewtrips.Rows(ii).Cells(18).ToolTip = "Crew Duty Alert: " & dutyday.ToString("N") & " hours since " & startofduty & vbNewLine & cwdepartureport & " " & cwarrivalport & " " & swapwarntext
                        End If
                        Swapflightfail = False
                        Dim sf As Integer = startofdutyflight

                        Dim dd As Integer = DateDiff(DateInterval.Day, startofduty, Now.ToUniversalTime)
                        If dd > -1 And dd < 1 Then
                            Dim ft As String = Left(gridviewtrips.Rows(sf).Cells(11).Text, 1)
                            If Left(gridviewtrips.Rows(sf).Cells(11).Text, 1) = "D" Then
                                movedhemail = movedhemail & "Preposition " & gridviewtrips.Rows(sf).Cells(8).Text & " : " & gridviewtrips.Rows(sf).Cells(0).Text & "-" & gridviewtrips.Rows(sf).Cells(1).Text & "..." & vbNewLine
                                cmdSlideReport.ToolTip = movedhemail
                            End If
                        End If


                    End If
                End If

            End If

            If departureairport <> arrivalairport Then 'rk 10/12/13 only count prior arrival if it is really a flight
                prevarrival = addyear(gridviewtrips.Rows(ii).Cells(3).Text)
            End If

            '   GridViewTrips.Rows(ii).Cells(19).BorderStyle = BorderStyle.None
            gridviewtrips.Rows(ii).Cells(19).ToolTip = gridviewtrips.Rows(ii).Cells(20).Text & " " & gridviewtrips.Rows(ii).Cells(21).Text & " " & gridviewtrips.Rows(ii).Cells(22).Text
            If InStr("Failed", gridviewtrips.Rows(ii).Cells(19).ToolTip) <> 0 Then
                gridviewtrips.Rows(ii).Cells(19).BackColor = Drawing.Color.Red
                ' GridViewTrips.Rows(ii).Cells(19).BorderStyle = BorderStyle.Dashed
            End If
        Next ii

        For i = 0 To gridviewtrips.Rows.Count - 1

            If Left(gridviewtrips.Rows(i).Cells(11).Text, 1) = "D" Then
                d = d + gridviewtrips.Rows(i).Cells(7).Text
            End If

            If Left(gridviewtrips.Rows(i).Cells(11).Text, 1) = "R" Then
                r = r + gridviewtrips.Rows(i).Cells(7).Text
            End If

            Try
                gridviewtrips.Rows(i).Cells(1).ToolTip = fname(gridviewtrips.Rows(i).Cells(1).Text)
                gridviewtrips.Rows(i).Cells(0).ToolTip = fname(gridviewtrips.Rows(i).Cells(0).Text)
            Catch
            End Try

            Dim acchange As Boolean = False

            If i + 1 > gridviewtrips.Rows.Count - 1 Then
                acchange = True
                GoTo acskip
            End If

            If gridviewtrips.Rows(i).Cells(8).Text <> gridviewtrips.Rows(i + 1).Cells(8).Text Then acchange = True 'ac change

acskip:

            If acchange Then

                Dim pct As Double = 0
                If r + d <> 0 Then
                    pct = (r) / (r + d)
                    pct = Int(pct * 100)
                End If

                ' GridViewTrips.Rows(i - 1).Cells(23).Text = pct
                gridviewtrips.Rows(i).Cells(9).ToolTip = pct & "%"

                If r + d <> 0 Then
                    If pct < 50 Then gridviewtrips.Rows(i).Cells(9).BackColor = Drawing.Color.Yellow
                    If pct < 30 Then gridviewtrips.Rows(i).Cells(9).BackColor = Drawing.Color.Red
                    If pct >= 50 Then gridviewtrips.Rows(i).Cells(9).BackColor = Drawing.Color.GreenYellow
                    If pct > 85 Then gridviewtrips.Rows(i).Cells(9).BackColor = Drawing.Color.Green
                End If

                d = 0
                r = 0

            End If
            'added plane lookup info back in rk 7.20.17
            If gridviewtrips.Rows(i).Cells(8).Text <> "" Then
                gridviewtrips.Rows(i).Cells(8).ToolTip = AirTaxi.lookupac(gridviewtrips.Rows(i).Cells(8).Text, carrierid)
            End If
        Next i

        If gridviewtrips.Rows.Count > 5 Then

            ' linemeup()
            colorflighttype(gridviewtrips)
        End If
        If ACX(1) <> "" Then
            For z = 13 To gridviewtrips.Columns.Count - 1
                gridviewtrips.Columns(z).Visible = False
            Next z
        End If

        'If carrierid = WHEELSUP Then
        '    gridviewtrips.Columns(CAS_PANDL).Visible = True
        '    gridviewtrips.Columns(CAS_REVENUE).Visible = True
        'ElseIf carrierid = JETLINX Then
        '    gridviewtrips.Columns(CAS_WC).Visible = True
        '    gridviewtrips.Columns(CAS_BASE).Visible = True
        'Else
        '    gridviewtrips.Columns(CAS_WC).Visible = True
        '    gridviewtrips.Columns(CAS_BASE).Visible = True
        'End If
    End Function

    Protected Sub GVGridViewTrips_PreRender(sender As Object, e As EventArgs)
        Dim i As Integer = 0

        For i = 0 To lvflightlist.Items.Count - 1
            FOScolorme(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), DirectCast(lvflightlist.Items(1).FindControl("pnlACType"), Label).Text)
            CAScolorme(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView))
        Next

    End Sub
    Function convdate(d As String) As Date
        Dim s As String

        If IsDate(d) Then
            Return CDate(d)
            Exit Function
        End If
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
    Function colorflighttype(ByRef gridviewtrips As GridView)
        Dim i As Integer
        For i = 0 To gridviewtrips.Rows.Count - 1


            Dim testme(40) As String
            For zz = 1 To 15
                testme(zz) = zz & ":" & gridviewtrips.Rows(i).Cells(zz).Text
            Next

            Dim dc As Drawing.Color
            Dim dc1 As String

            gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
            Dim x As String = gridviewtrips.Rows(i).Cells(10).Text
            Dim xx As String = gridviewtrips.Rows(i).Cells(12).Text
            Dim xxx As String = gridviewtrips.Rows(i).Cells(14).Text
            If Left(gridviewtrips.Rows(i).Cells(11).Text, 1) = "D" Then gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow
            If Left(gridviewtrips.Rows(i).Cells(11).Text, 1) = "R" Then gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkSeaGreen

            '20120724 - pab - highlight mtc trips orange
            If gridviewtrips.Rows(i).Cells(11).Text = "M" Then gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
            'rk 7/20/2013
            If gridviewtrips.Rows(i).Cells(11).Text = "B" Then gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki


            Dim ltc As String = gridviewtrips.Rows(i).Cells(17).Text


            If ltc = "SWAP" Then gridviewtrips.Rows(i).Cells(11).BackColor = Drawing.Color.MediumPurple

            Dim mn As String = gridviewtrips.Rows(i).Cells(6).Text

            Dim nm As String = gridviewtrips.Rows(i).Cells(7).Text

            If IsNumeric(nm) And IsNumeric(mn) Then

                If CInt(nm) <> 0 And CInt(mn) <> 0 Then

                    Dim sp As Integer = Int(nm / mn * 60)
                    gridviewtrips.Rows(i).Cells(7).ToolTip = sp & " NM"

                    If sp > 300 Then gridviewtrips.Rows(i).Cells(7).ForeColor = Color.DarkRed

                End If
            End If

        Next i
    End Function


End Class