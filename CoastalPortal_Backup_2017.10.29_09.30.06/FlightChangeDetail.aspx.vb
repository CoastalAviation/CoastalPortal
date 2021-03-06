﻿Imports CoastalPortal.AirTaxi
Imports CoastalPortal.DataAccess
Imports CoastalPortal.Personibid
Imports CoastalPortal.Models
Imports System.Drawing
Imports System.IO
Imports SelectPdf


Public Class FlightChangeDetail
    Inherits System.Web.UI.Page
    Public myJS As String = String.Empty
    Public myJS1 As String = String.Empty

    Public fcdrlist As New List(Of FCDRList)
    Public demandlookup As New Dictionary(Of String, Boolean)
    Private CarrierProfile As New CarrierProfile
    Public newTail_Dictionary As New Dictionary(Of String, String)
    Public stest As String
    Public Property calendarcarrierid As String
    Public Property calendarcypher As String
    Private modelrunid As String
    Public Filename As String = ""
    Public CreatePDF As Boolean = False
    Private db As New OptimizerContext
    ' Public Shared fcdrlist As New List(Of FCDRList)
    Public Shared specsfromlog As Boolean = True
    Public fcdrcolors As New List(Of fcdrColorlist)
    Public Const FOS_FROM As Integer = 0
    Public Const FOS_TO As Integer = 1
    Public Const FOS_FROMGMT As Integer = 2
    Public Const FOS_TOGMT As Integer = 3
    Public Const FOS_NM As Integer = 4
    Public Const FOS_AC As Integer = 5
    Public Const FOS_TYPE As Integer = 6
    Public Const FOS_PAX As Integer = 7
    Public Const FOS_FT As Integer = 8
    Public Const FOS_TRIP As Integer = 9
    Public Const FOS_LTC As Integer = 10
    Public Const FOS_SIC As Integer = 11
    Public Const FOS_PIC As Integer = 12
    Public Const FOS_COST As Integer = 13
    Public Const FOS_PANDL As Integer = 14
    Public Const FOS_REV As Integer = 15
    Public Const FOS_BASE As Integer = 16
    Public Const FOS_QE As Integer = 17
    Public Const FUTURE_TAIL As Integer = 18
    Public Const CAS_FROM As Integer = 19
    Public Const CAS_TO As Integer = 20
    Public Const CAS_FROMGMT As Integer = 21
    Public Const CAS_TOGMT As Integer = 22
    Public Const CAS_NM As Integer = 23
    Public Const CAS_AC As Integer = 24
    Public Const CAS_TYPE As Integer = 25
    Public Const CAS_FT As Integer = 26
    Public Const CAS_TRIP As Integer = 27
    Public Const CAS_IND As Integer = 28
    Public Const CAS_LTC As Integer = 29
    Public Const CAS_SIC As Integer = 30
    Public Const CAS_PIC As Integer = 31
    Public Const CAS_COST As Integer = 32
    Public Const CAS_PANDL As Integer = 33
    Public Const CAS_REV As Integer = 34
    Public Const CAS_BASE As Integer = 35
    Public Const CAS_PIN As Integer = 36
    Public Const CAS_PT As Integer = 37
    Public Const CAS_HA As Integer = 38
    Public Const CAS_LEGBASE As Integer = 39
    Public Const CAS_OE As Integer = 40



    Private M_carrier As Integer
    Protected Property MyCarrier As Integer
        Get
            Return M_carrier
        End Get
        Set(value As Integer)
            M_carrier = value
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim db As New OptimizerContext
        Dim demandlist As New List(Of String)
        Dim AWC As New List(Of WeightClass)
        setcolors()

        Session("FltChange") = "FC"
        Session("FCList") = ""

        Dim carrierprofile As New CarrierProfile

        '  Dim ac1, ac2 As String
        Dim mrid As String = ""
        Dim id As Integer
        Dim pt, ar As String
        Dim mrcustom As String
        Dim casRecord As CASFlightsOptimizerRecord



        FOSRecords = Session("FOS")
        CASRecords = Session("CAS")

        demandlookup.Clear()

        demandlist = db.Database.SqlQuery(Of String)("Select placeholder from DemandList").ToList()
        For Each x As String In demandlist
            demandlookup.Add(Trim(x), True)
        Next
        CreatePDF = Not (Request.QueryString("pdf") Is Nothing)

        If Not Request.QueryString("key") Is Nothing Then
            id = Request.QueryString("key")
            fcdrlist = db.FCDRList.Where(Function(c) c.keyid = id).ToList()
            If Not Request.QueryString("carrier") Is Nothing Then
                Session("carrierid") = Request.QueryString("carrier")
            End If
            If fcdrlist.Count = 0 Then
                If CASRecords Is Nothing Then
                    casRecord = db.CASFlightsOptimizer.Find(id)
                Else
                    casRecord = CASRecords.Find(Function(x) x.ID = id)
                    If casRecord Is Nothing Then 'we Have the Wrong Carrier ID
                        casRecord = db.CASFlightsOptimizer.Find(id)
                        CASRecords = Nothing
                        FOSRecords = Nothing
                    End If
                End If
                If casRecord Is Nothing Then Exit Sub ' ID supplied is bad
                mrid = Trim(casRecord.OptimizerRun)
                pt = casRecord.PriorTail
                ar = casRecord.AircraftRegistration
                ACX(0) = Trim(ar)
                ACX(1) = Trim(pt)
                casmodelrunid = mrid
            Else
                mrid = fcdrlist(0).ModelRunID.ToString()
            End If
        Else
            If Not Request.QueryString("ModelSavings") Is Nothing Then
                mrid = Request.QueryString("ModelSavings")
                ACX(0) = Trim(InBetween(1, mrid, "A1[", "]"))
                ACX(1) = Trim(InBetween(1, mrid, "A2[", "]"))
                mrid = Trim(InBetween(1, mrid, "A0[", "]"))

            End If
            fcdrlist = db.FCDRList.Where(Function(c) c.ModelRunID = mrid).ToList()
            If fcdrlist.Count = 0 Then
                If CASRecords Is Nothing Then
                    casRecord = db.CASFlightsOptimizer.Where(Function(x) x.ModelRun = mrid).First()
                Else
                    casRecord = CASRecords.Where(Function(x) x.ModelRun = mrid).First()
                End If

                id = casRecord.ID
                mrid = casRecord.ModelRun
            Else
                mrid = fcdrlist(0).ModelRunID.ToString()
            End If
        End If
        If awclookup.Count = 0 Then
            awclookup.Clear()
            AWC = db.AircraftWeightClass.ToList()
            For Each x As WeightClass In AWC
                awclookup.Add(Trim(x.AircraftType), x.AircraftWeightClass)
            Next
        End If

        Session("modelrunid") = mrid

        If Session("overridemodel") = "" Then
            mrcustom = normalizemodelrunid(Session("modelrunid").ToString)
        Else
            mrcustom = Session("overridemodel")
        End If

        If Session("overridemodel") <> "" And ACX(1) <> "" Then
            mrcustom = normalizemodelrunid(Session("modelrunid").ToString)
        End If
        mrcustom = normalizemodelrunid(mrid)
        If FOSRecords Is Nothing Then
            FOSRecords = db.FOSFlightsOptimizer.Where(Function(x) x.OptimizerRun = mrcustom).ToList()
        End If
        If CASRecords Is Nothing Then
            CASRecords = db.CASFlightsOptimizer.Where(Function(x) x.OptimizerRun = mrid).ToList()
        End If

        demandlookup.Clear()
        carrierprofile = Session("Profile")

        If fcdrlist.Count = 0 Then
            enumerate(ACX(0), id, mrid, ACX(1))
            Session("carrierid") = casRecord.CarrierId

        End If
        Session("FCDRKey") = fcdrlist
        ' If Not Page.IsPostBack Then
        GetTrips()
        ' End If
    End Sub

    Private Sub Flights24_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        ACX(1) = ""
        Session("overridemodel") = ""

    End Sub


    Public Function enumerate(starttail As String, fcdrkey As String, model As String, priortail As String)
        Dim db As New OptimizerContext
        Dim FosIdList, CasIDList As String
        Dim casflights As New List(Of CASFCDRFlight)
        Dim fosflights As New List(Of FOSFCDRflight)
        Dim pt As String
        Dim GMTStart As DateTime
        Dim dummy As Boolean = False
        Dim mrid As String = ""
        Dim id As String = ""
        Dim ptlist As New List(Of String)
        Dim TripList As New List(Of String)
        Dim zz As Integer = 0
        Dim AssignTrip As Boolean = False
        'Dim AClist As String

        casflights.Clear()
        fosflights.Clear()
        fcdrlist.Clear()

        AssignTrip = demandlookup.TryGetValue(priortail, dummy)

        ptlist.Add(starttail)
        If Not AssignTrip Then
            ptlist = ptlist.Union((From a In CASRecords Where Trim(a.AircraftRegistration) = starttail And Trim(a.PriorTail) <> "" And Trim(a.PriorTail) <> starttail And
                                           Trim(a.DepartureAirport) <> Trim(a.ArrivalAirport) And Trim(a.LegTypeCode) <> "BULL" Select Trim(a.PriorTail)).Distinct().ToList()).ToList()
            Do While zz <> ptlist.Count
                zz = ptlist.Count
                fosflights = (From a In FOSRecords Where ptlist.Contains(Trim(a.AC)) And Trim(a.DepartureAirportICAO) <> Trim(a.ArrivalAirportICAO) And Trim(a.LegTypeCode) <> "BULL"
                              Select New FOSFCDRflight With {.ac = Trim(a.AC), .tripnumber = Trim(a.TripNumber), .DepartICAO = Trim(a.DepartureAirportICAO),
                                                       .ArriveICAO = Trim(a.ArrivalAirportICAO), .id = a.id}).ToList()

                For z = 0 To fosflights.Count - 1
                    Dim DepartureAirportICAO, arrivalAirportICAO, tn As String

                    DepartureAirportICAO = fosflights(z).DepartICAO
                    arrivalAirportICAO = fosflights(z).ArriveICAO
                    tn = fosflights(z).tripnumber

                    casflights = casflights.Union((From b In CASRecords Where Trim(b.TripNumber) = tn And Trim(b.DepartureAirport) <> Trim(b.ArrivalAirport) And
                                 Trim(b.LegTypeCode) <> "BULL" And Trim(b.ArrivalAirport) = arrivalAirportICAO And Trim(b.DepartureAirport) = DepartureAirportICAO And
                                    Not demandlookup.TryGetValue(Trim(b.PriorTail), dummy)
                                                   Select New CASFCDRFlight With {.id = b.ID, .aircraftregistration = Trim(b.AircraftRegistration), .tripnumber = Trim(b.TripNumber)}).ToList()).ToList()

                    ptlist = ptlist.Union((From v In casflights Where Not ptlist.Contains(v.aircraftregistration) Select v.aircraftregistration).Distinct().ToList()).ToList()
                Next
            Loop

            ptlist = ptlist.Union((From a In CASRecords Where ptlist.Contains(Trim(a.AircraftRegistration)) And Trim(a.PriorTail) <> "" And Trim(a.PriorTail) <> Trim(a.AircraftRegistration) And
                                           Trim(a.DepartureAirport) <> Trim(a.ArrivalAirport) And Trim(a.LegTypeCode) <> "BULL" Select Trim(a.PriorTail)).Distinct().ToList()).ToList()

            ptlist = ptlist.Distinct().ToList()
            'add the enumerated list to the fcdr for display...
            TripList = (From a In casflights Select a.tripnumber).Distinct.ToList()
        Else
            TripList = (From a In CASRecords Where a.ID = fcdrkey Select Trim(a.TripNumber)).ToList()
            ptlist = ptlist.Union(From a In CASRecords Where a.ID = fcdrkey Select Trim(a.AircraftRegistration)).ToList()
        End If

        For a = 0 To ptlist.Count - 1
            FosIdList &= ptlist(a) & ","
            CasIDList &= ptlist(a) & ","
        Next
        FosIdList = Left(FosIdList, Len(FosIdList) - 1)
        CasIDList = Left(CasIDList, Len(CasIDList) - 1)

        Session("PTList") = ptlist

        Dim frc, cascost As Double
        Dim dcostday0 As Double = 0
        Dim dcostday1 As Double = 0
        Dim dcostday2 As Double = 0
        Dim totalcost As Double = 0
        Dim fnrm, cnrm As Double

        Dim i As Integer = model.IndexOf("-")

        mrid = Left(model, i)

        GMTStart = db.OptimizerRequest.Find(CInt(mrid)).GMTStart
        frc = isdbn((From fr In FOSRecords Where Trim(fr.LegTypeCode) <> "77" And Trim(fr.LegTypeCode) <> "7" And Trim(fr.LegState) <> "5" And
                                                ((Not demandlookup.TryGetValue(Trim(fr.AC), dummy) And ptlist.Contains(Trim(fr.AC))) Or
                                                 (demandlookup.TryGetValue(Trim(fr.AC), dummy) And TripList.Contains(Trim(fr.TripNumber)))) And
                                                fr.DateTimeGMT >= GMTStart Select CInt(Trim(fr.DHCost))).Sum())
        cascost = isdbn((From cr In CASRecords Where ptlist.Contains(Trim(cr.AircraftRegistration)) And
                                                   ((Not demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And ptlist.Contains(Trim(cr.AircraftRegistration))) Or
                                                    (demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And TripList.Contains(Trim(cr.TripNumber)))) And
                                                   cr.DepartureTime >= GMTStart Select CInt(Trim(cr.cost))).Sum())
        totalcost = frc - cascost


        frc = isdbn((From fr In FOSRecords Where Trim(fr.LegTypeCode) <> "77" And Trim(fr.LegTypeCode) <> "7" And Trim(fr.LegState) <> "5" And
                                                ((Not demandlookup.TryGetValue(Trim(fr.AC), dummy) And ptlist.Contains(Trim(fr.AC))) Or
                                                 (demandlookup.TryGetValue(Trim(fr.AC), dummy) And TripList.Contains(Trim(fr.TripNumber)))) And
                                                fr.DateTimeGMT >= GMTStart And fr.DateTimeGMT < DateAdd("d", 1, GMTStart) Select CInt(Trim(fr.DHCost))).Sum())
        cascost = isdbn((From cr In CASRecords Where ptlist.Contains(Trim(cr.AircraftRegistration)) And
                                                   ((Not demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And ptlist.Contains(Trim(cr.AircraftRegistration))) Or
                                                    (demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And TripList.Contains(Trim(cr.TripNumber)))) And
                                                   cr.DepartureTime >= GMTStart And cr.DepartureTime < DateAdd("d", 1, GMTStart) Select CInt(Trim(cr.cost))).Sum())
        dcostday0 = frc - cascost

        frc = isdbn((From fr In FOSRecords Where Trim(fr.LegTypeCode) <> "77" And Trim(fr.LegTypeCode) <> "7" And Trim(fr.LegState) <> "5" And
                                               ((Not demandlookup.TryGetValue(Trim(fr.AC), dummy) And ptlist.Contains(Trim(fr.AC))) Or
                                                (demandlookup.TryGetValue(Trim(fr.AC), dummy) And TripList.Contains(Trim(fr.TripNumber)))) And
                                               fr.DateTimeGMT >= DateAdd("d", 1, GMTStart) And fr.DateTimeGMT < DateAdd("d", 2, GMTStart) Select CInt(Trim(fr.DHCost))).Sum())
        cascost = isdbn((From cr In CASRecords Where ptlist.Contains(Trim(cr.AircraftRegistration)) And
                                                   ((Not demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And ptlist.Contains(Trim(cr.AircraftRegistration))) Or
                                                    (demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And TripList.Contains(Trim(cr.TripNumber)))) And
                                                   cr.DepartureTime >= DateAdd("d", 1, GMTStart) And cr.DepartureTime < DateAdd("d", 2, GMTStart) Select CInt(Trim(cr.cost))).Sum())
        dcostday1 = frc - cascost

        frc = isdbn((From fr In FOSRecords Where Trim(fr.LegTypeCode) <> "77" And Trim(fr.LegTypeCode) <> "7" And Trim(fr.LegState) <> "5" And
                                                ((Not demandlookup.TryGetValue(Trim(fr.AC), dummy) And ptlist.Contains(Trim(fr.AC))) Or
                                                 (demandlookup.TryGetValue(Trim(fr.AC), dummy) And TripList.Contains(Trim(fr.TripNumber)))) And
                                                fr.DateTimeGMT >= DateAdd("d", 2, GMTStart) And fr.DateTimeGMT < DateAdd("d", 3, GMTStart) Select CInt(Trim(fr.DHCost))).Sum())
        cascost = isdbn((From cr In CASRecords Where ptlist.Contains(Trim(cr.AircraftRegistration)) And
                                                   ((Not demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And ptlist.Contains(Trim(cr.AircraftRegistration))) Or
                                                    (demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And TripList.Contains(Trim(cr.TripNumber)))) And
                                                   cr.DepartureTime >= DateAdd("d", 2, GMTStart) And cr.DepartureTime < DateAdd("d", 3, GMTStart) Select CInt(Trim(cr.cost))).Sum())
        dcostday2 = frc - cascost

        fnrm = isdbn((From fr In FOSRecords Where Trim(fr.DeadHead) = "True" And Trim(fr.LegTypeCode) <> "77" And Trim(fr.LegTypeCode) <> "7" And Trim(fr.LegState) <> "5" And
                                    ((Not demandlookup.TryGetValue(Trim(fr.AC), dummy) And ptlist.Contains(Trim(fr.AC))) Or
                                           (demandlookup.TryGetValue(Trim(fr.AC), dummy) And TripList.Contains(Trim(fr.TripNumber)))) And
                                           fr.DateTimeGMT >= GMTStart Select CInt(Trim(fr.NauticalMiles))).Sum())
        cnrm = isdbn((From cr In CASRecords Where Trim(cr.FlightType) = "D" And ptlist.Contains(Trim(cr.AircraftRegistration)) And
                                                  ((Not demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And ptlist.Contains(Trim(cr.AircraftRegistration))) Or
                                                    (demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And TripList.Contains(Trim(cr.TripNumber)))) And
                                                cr.DepartureTime >= GMTStart Select CInt(Trim(cr.Distance))).Sum())



        fcdrlist.Add(New FCDRList With {.CASRecordList = CasIDList, .FOSRecordList = FosIdList, .ModelRunID = model, .PriorTailNumber = starttail, .DeltaNonRevMiles = CInt(fnrm - cnrm),
                    .TotalSavings = totalcost, .SavingsDay0 = dcostday0, .SavingsDay1 = dcostday1, .SavingsDay2 = dcostday2, .keyid = fcdrkey, .ModelRun = mrid, .GMTStart = GMTStart})

        Try
            db.FCDRList.AddRange(fcdrlist)
            db.SaveChanges()
        Catch ex As Exception

        End Try

    End Function

    Public Function isdbn(field As Object) As Object

        If Not (IsDBNull(field)) Then
            Return field
        Else
            Return 0
        End If

    End Function

    Public Sub GetTrips()
        Dim GridViewSource As New List(Of PanelDisplay)
        Dim FosList As New List(Of FOSFlightsOptimizerRecord)
        Dim CasList As New List(Of CASFlightsOptimizerRecord)
        Dim Panellist As New List(Of PanelRecord)
        Dim TestPanellist As New List(Of PanelRecord)
        Dim PanellistRight As New List(Of PanelRecord)
        Dim BaseList As New List(Of String)
        Dim ACList As New List(Of String)
        Dim HAList As New List(Of String)
        Dim TripList As New List(Of String)
        Dim GMTStart As DateTime
        Dim FClist As String = Session("FCList")
        Dim dummy As Boolean = False
        Dim mrcustom As String = normalizemodelrunid(modelrunid)
        Dim baserev As New List(Of baseRevenue)

        ' Dim fnrm, cnrm As Double
        'FosList = FOSRecords.Where(Function(x) FClist.Contains(Trim(x.AC)) And x.LegState <> 5 And x.DateTimeGMT > DateTime.UtcNow).OrderBy(Function(y) y.TripNumber).ThenBy(Function(y) y.DepartureDateGMT).ToList()
        'CasList = CASRecords.Where(Function(x) FClist.Contains(Trim(x.AircraftRegistration))).OrderBy(Function(y) y.TripNumber).ThenBy(Function(y) y.DepartureTime).ToList()
        fcdrcolors.Clear()

        For Each fcdr As FCDRList In fcdrlist
            GMTStart = fcdr.GMTStart
            FosList = FOSRecords.Where(Function(x) fcdr.FOSRecordList.Contains(Trim(x.AC)) And Not demandlookup.TryGetValue(Trim(x.AC), dummy) And x.DateTimeGMT.Date >= GMTStart.Date).OrderBy(Function(y) y.AC).ThenBy(Function(y) y.DepartureDateGMT).Distinct().ToList()
            CasList = CASRecords.Where(Function(x) fcdr.CASRecordList.Contains(Trim(x.AircraftRegistration)) And Not demandlookup.TryGetValue(Trim(x.AircraftRegistration), dummy) And x.DepartureTime >= GMTStart).OrderBy(Function(y) y.AircraftRegistration).ThenBy(Function(y) y.DepartureTime).Distinct().ToList()

            TripList = (From a In CasList Select a.TripNumber).Distinct().ToList()
            FosList = FosList.Union(FOSRecords.Where(Function(x) demandlookup.TryGetValue(Trim(x.AC), dummy) And TripList.Contains(x.TripNumber) And x.DateTimeGMT.Date >= GMTStart.Date).OrderBy(Function(y) y.AC).ThenBy(Function(y) y.DepartureDateGMT).ToList()).Distinct().ToList()
            CasList = CasList.Union(CASRecords.Where(Function(x) demandlookup.TryGetValue(Trim(x.AircraftRegistration), dummy) And TripList.Contains(x.TripNumber) And x.DepartureTime >= GMTStart).OrderBy(Function(y) y.AircraftRegistration).ThenBy(Function(y) y.DepartureTime).ToList()).Distinct().ToList()

            FosList = (From a In FosList Select a).Distinct().ToList()
            CasList = (From b In CasList Select b).Distinct().ToList()

            fcdrcolors.Add(New fcdrColorlist With {.FCDR_Key = fcdr.keyid, .changecolor_dictionary = getColorDictionary(CasList)})

            ACList = (From a In FosList Select Trim(a.AC)).Distinct().ToList()
            ACList = ACList.Union((From a In CasList Select Trim(a.AircraftRegistration)).Distinct()).ToList()
            ACList = ACList.Distinct().ToList()


            Panellist = (From f In FosList Group Join c In CasList On Trim(f.AC) Equals Trim(c.AircraftRegistration) And Trim(f.TripNumber) Equals Trim(c.TripNumber) And
                         Trim(f.ArrivalAirportICAO) Equals Trim(c.ArrivalAirport) And Trim(f.DepartureAirportICAO) Equals Trim(c.DepartureAirport) And c.DepartureTime Equals f.DateTimeGMT
                                               Into Plist = Group From p In Plist.DefaultIfEmpty() Select New PanelRecord With {.FCDR_Key = fcdr.keyid, .CASRecord = p, .FOSRecord = f}).Distinct().ToList()

            PanellistRight = (From c In CasList Group Join f In FosList On Trim(f.AC) Equals Trim(c.AircraftRegistration) And Trim(f.ArrivalAirportICAO) Equals Trim(c.ArrivalAirport) And
                                                    Trim(f.DepartureAirportICAO) Equals Trim(c.DepartureAirport) And Trim(f.TripNumber) Equals Trim(c.TripNumber) And c.DepartureTime Equals f.DateTimeGMT
                                                    Into Plist = Group From p In Plist.DefaultIfEmpty() Select New PanelRecord With {.FCDR_Key = fcdr.keyid, .FOSRecord = p, .CASRecord = c}).Distinct().ToList()

            For Each i As PanelRecord In PanellistRight
                If i.FOSRecord Is Nothing Then
                    Panellist.Add(i)
                End If
            Next
            'Dim f = FosList.Count - 1
            ' Dim c = CasList.Count - 1

            For Each i As PanelRecord In Panellist
                Dim Chktail = Trim(If(i.FOSRecord Is Nothing, i.CASRecord.AircraftRegistration, i.FOSRecord.AC))
                i.Starttail = If(Chktail = Trim(fcdr.PriorTailNumber), "True", "False")
            Next
            Panellist = (From a In Panellist Select a).OrderByDescending(Function(x) x.Starttail).ThenBy(Function(x) x.TailNumber).ThenBy(Function(x) x.DateTimeGMT).Distinct().ToList()
            BaseList = (From a In FosList Select Trim(a.BaseCode)).Distinct().ToList()
            BaseList = BaseList.Union((From a In CasList Select Trim(a.BaseCode)).Distinct().ToList()).ToList()

            For Each x As String In BaseList
                Dim CasRev, Fosrev As Decimal
                CasRev = 0
                Fosrev = 0
                For Each dd As String In TripList
                    Fosrev += CDbl((From a In FosList Where Trim(a.TripNumber) = Trim(dd) And Trim(a.BaseCode) = Trim(x) Select a.PandL).FirstOrDefault())
                    CasRev += CDbl((From a In CasList Where Trim(a.TripNumber) = Trim(dd) And Trim(a.BaseCode) = Trim(x) Select a.PandL).FirstOrDefault())
                Next
                baserev.Add(New baseRevenue With {.basecode = x, .CasRevenue = CasRev, .FosRevenue = Fosrev})
            Next

            GridViewSource.Add(New PanelDisplay With {.FCDR_Key = fcdr.keyid, .dcostday0 = fcdr.SavingsDay0, .dcostday1 = fcdr.SavingsDay1, .dcostday2 = fcdr.SavingsDay2, .TailNumber = fcdr.PriorTailNumber, .RevenueRecords = baserev,
                                                        .NRM = CDbl(fcdr.DeltaNonRevMiles), .PanelRecord = Panellist.ToList(), .ModelNumber = fcdr.ModelRunID, .TotalSavings = fcdr.TotalSavings})

            Filename = fcdr.keyid
        Next

        For Each x As PanelDisplay In GridViewSource
            AirTaxi.awclookup.TryGetValue(Trim(x.AircraftType), x.WeightClass)
        Next

        lvflightlist.DataSource = GridViewSource
        lvflightlist.DataBind()

    End Sub
    Function FOScolorme(ByRef gridviewtrips As GridView, ByVal FCDRColorList As fcdrColorlist) ', ACType As String)

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
        mycolor = Drawing.Color.White

        Dim colorsArray As System.Array =
        [Enum].GetValues(GetType(KnownColor))
        Dim allColors(colorsArray.Length) As KnownColor

        Array.Copy(colorsArray, allColors, colorsArray.Length)

        _carrierid = Session("carrierid")
        Dim account, rownumber As Integer
        Dim currentTail, LastTail As String
        Dim i As Integer
        rownumber = 0
        For i = 0 To gridviewtrips.Rows.Count - 1
            If gridviewtrips.Rows(i).Cells(FOS_AC).Text <> "" Then
                gridviewtrips.Rows(i).Cells(FOS_AC).ToolTip = AirTaxi.lookupac(Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text))
            End If
            rownumber += 1
            If IsNumeric(gridviewtrips.Rows(i).Cells(FOS_COST).Text) Then
                gridviewtrips.Rows(i).Cells(FOS_COST).Text = Convert.ToDecimal(gridviewtrips.Rows(i).Cells(FOS_COST).Text).ToString("c0")
            End If
            If i > 0 Then
                currentTail = If(Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text) <> "&nbsp;", Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text), Trim(gridviewtrips.Rows(i).Cells(CAS_AC).Text))
                LastTail = If(Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text) <> "&nbsp;", Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text), Trim(gridviewtrips.Rows(i - 1).Cells(CAS_AC).Text))
            End If

            gridviewtrips.Rows(i).BackColor = mycolor

            Dim key, mycolor1 As String
            key = Trim(gridviewtrips.Rows(i).Cells(FOS_FROM).Text) & "-" & Trim(gridviewtrips.Rows(i).Cells(FOS_TO).Text) & "-" & Trim(gridviewtrips.Rows(i).Cells(FOS_TRIP).Text)
            '  mycolor1 = changecolor_dictionary(key)

            Dim value As String
            If (FCDRColorList.changecolor_dictionary.TryGetValue(key, value)) Then
                mycolor1 = value

                If CInt(mycolor1) > 40 Then
                    gridviewtrips.Rows(i).Cells(FOS_FROM).BackColor = Color.FromName(allColors(CInt(mycolor1) + 2).ToString)
                    gridviewtrips.Rows(i).Cells(FOS_TO).BackColor = Color.FromName(allColors(CInt(mycolor1) + 2).ToString)
                Else
                    gridviewtrips.Rows(i).Cells(FOS_FROM).BackColor = Color.FromName(colornames(CInt(mycolor1)))
                    gridviewtrips.Rows(i).Cells(FOS_TO).BackColor = Color.FromName(colornames(CInt(mycolor1)))

                End If
            Else
                mycolor1 = 0
            End If
            If (newTail_Dictionary.TryGetValue(key, value)) Then
                gridviewtrips.Rows(i).Cells(FUTURE_TAIL).Text = value
            ElseIf gridviewtrips.Rows(i).Cells(FOS_AC).Text = "&nbsp;" Then
                gridviewtrips.Rows(i).Cells(FUTURE_TAIL).Text = "Added"
            ElseIf Trim(gridviewtrips.Rows(i).Cells(CAS_AC).Text) = "&nbsp;" Then
                gridviewtrips.Rows(i).Cells(FUTURE_TAIL).Text = "Removed"
            End If

            'If gridviewtrips.Rows(i).Cells(FOS_FROM).Text = "KMMU" Then
            '    i = i
            'End If

            'Dim x As String = DirectCast(gridviewtrips.Rows(i).FindControl("fosFT"), Label).Text
            Dim ltc As String = Trim(gridviewtrips.Rows(i).Cells(FOS_LTC).Text)
            Select Case Trim(gridviewtrips.Rows(i).Cells(FOS_FT).Text)
                Case "True"
                    ' gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat
                    gridviewtrips.Rows(i).Cells(FOS_FT).Text = "DH"
                Case "False"
                    'gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.DarkSeaGreen
                    gridviewtrips.Rows(i).Cells(FOS_FT).Text = "Rev"


                    If ltc = "CREW" Then
                        gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.White
                        gridviewtrips.Rows(i).Cells(FOS_FT).Text = "C"
                    End If

                    If ltc = "MXSC" Or ltc = "SWAP" Then     'SWAP added by mws 11/25
                        gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.White
                        gridviewtrips.Rows(i).Cells(FOS_FT).Text = "M"
                    End If

                    If ltc = "M" Then
                        gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.White
                        gridviewtrips.Rows(i).Cells(FOS_FT).Text = "M"
                    End If

                    If ltc = "LMX" Then
                        gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.White
                        gridviewtrips.Rows(i).Cells(FOS_FT).Text = "M"
                    End If

                    If ltc = "Y" Then
                        gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.Orange
                        gridviewtrips.Rows(i).Cells(FOS_FT).Text = "S"
                        gridviewtrips.Rows(i).Cells(FOS_FT).ToolTip = "Static"
                    End If

                Case Else
                    If Trim(gridviewtrips.Rows(i).Cells(FOS_TRIP).Text) = "12345" Then
                        gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.DarkOrange
                        gridviewtrips.Rows(i).Cells(FOS_FT).Text = "M"
                    Else
                        gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.White
                    End If
            End Select


            If Trim(gridviewtrips.Rows(i).Cells(FOS_TRIP).Text) = "12345" Then
                gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.DarkOrange
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "M"
            End If

            ''   Dim ltcx As String = GridViewTrips.Rows(i).Cells(14).Text



            If CStr("AOG,DETL,INSV,MXRC,MXUS,MAINT").Contains(ltc) And ltc <> "D" Then
                gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.DarkOrange
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "M"
            End If

            If ltc = "77" Or ltc = "M" Then
                gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.SteelBlue
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "M"
            End If


            If ltc = "77" Or ltc = "MXSC" Or ltc = "SWAP" Then 'SWAP added by mws 11/25
                gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.SteelBlue
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "M"
            End If

            If ltc = "Y" Or ltc = "S" Then
                gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.Orange
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "S"
                gridviewtrips.Rows(i).Cells(FOS_FT).ToolTip = "Static"
            End If

            If ltc = "BULP" Then
                gridviewtrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "B"
            End If

            If ltc = "CREW" Then
                gridviewtrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "C"
            End If

            If ltc = "TRNG" Then
                gridviewtrips.Rows(i).Cells(15).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "T"
            End If

            If ltc = "LINB" Then
                gridviewtrips.Rows(i).Cells(FOS_FT).Text = "L"
                gridviewtrips.Rows(i).Cells(FOS_FT).BackColor = Drawing.Color.LightGreen
            End If


            If i > 0 Then
                gridviewtrips.Rows(i).Cells(FOS_AC).Font.Bold = False

                If Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text) <> Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text) Then

                    gridviewtrips.Rows(i).Cells(FOS_AC).Font.Bold = True
                Else
                    Dim result As Boolean = False
                    result = demandlookup.TryGetValue(Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text), result)

                    Dim prevflight As Date = CDate(convdate(gridviewtrips.Rows(i - 1).Cells(FOS_TOGMT).Text))
                    Dim currflight As Date = CDate(convdate(gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text))
                    Dim diff As Integer = DateDiff(DateInterval.Minute, prevflight, currflight)

                    If diff < 30 And Trim(gridviewtrips.Rows(i - 1).Cells(FOS_TOGMT).Text) <> "" Then gridviewtrips.Rows(i - 1).Cells(FOS_TOGMT).BackColor = Drawing.Color.Yellow
                    If diff < 30 And Trim(gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text) <> "" Then gridviewtrips.Rows(i).Cells(FOS_FROMGMT).BackColor = Drawing.Color.Yellow
                    If diff < 0 And Trim(gridviewtrips.Rows(i - 1).Cells(FOS_TOGMT).Text) <> "" Then gridviewtrips.Rows(i - 1).Cells(FOS_TOGMT).BackColor = Drawing.Color.Salmon
                    If diff < 0 And Trim(gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text) <> "" Then gridviewtrips.Rows(i).Cells(FOS_FROMGMT).BackColor = Drawing.Color.Salmon
                End If
            End If
            ' gridviewtrips.Rows(i).BackColor = mycolor

            Dim pinned As Boolean = False
            If bpinnedflights = True Then
                dv_pinned.RowFilter = "TripNumber = '" & gridviewtrips.Rows(i).Cells(FOS_TRIP).Text & "' and AirportFrom = '" &
                    Trim(gridviewtrips.Rows(i).Cells(FOS_FROM).Text).ToString & "' and AirportTo = '" & Trim(gridviewtrips.Rows(i).Cells(FOS_TO).Text).ToString & "'"
                If dv_pinned.Count > 0 Then
                    pinned = True
                End If
            End If

            If pinned Then
                gridviewtrips.Rows(i).Cells(FOS_FROMGMT).BackColor = Drawing.Color.Goldenrod
                gridviewtrips.Rows(i).Cells(FOS_TRIP).BackColor = Drawing.Color.Goldenrod
                ' gridviewtrips.Rows.Item(i).Cells(13).BackColor = Drawing.Color.Goldenrod 'rk 4.3.2013 change from 13 to 10 to show pinned field
            End If

            'rk 8.24.2013 back up auto pinned 3 hours
            If pinned = False Then
                If IsDate(gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text) Then
                    Dim gmt As Date = DateTime.UtcNow
                    Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text)

                    gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Font.Underline = False
                    'gridviewtrips.Rows(i).Cells(4).Font.Underline = False

                    If gmt > DateAdd(DateInterval.Minute, -180, departgmt) Then

                        gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Font.Underline = True
                        'gridviewtrips.Rows(i).Cells(4).Font.Underline = True

                    End If
                End If
            End If

            'show aog flights
            Dim aog As Boolean = False
            If bAOGflights = True Then
                dv_aog.RowFilter = "Aircraft = '" & Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text) & "'"
                If dv_aog.Count > 0 Then
                    aog = True
                End If
            End If

            If aog = False Then
                '   GridViewTrips.Rows.Item(i).Cells(8).BackColor = Drawing.Color.White

            Else
                gridviewtrips.Rows(i).Cells(FOS_AC).BackColor = Drawing.Color.Violet

            End If

            For z = 2 To 5
                If IsDate(gridviewtrips.Rows(i).Cells(z).Text) Then
                    Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(z).Text)
                    gridviewtrips.Rows(i).Cells(z).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
                End If
            Next z

        Next i
    End Function

    Function CAScolorme(ByRef gridviewtrips As GridView, ByVal FCDRColorList As fcdrColorlist)

        caslinebreaks = 0
        Dim mycolor As System.Drawing.Color
        mycolor = Drawing.Color.Aquamarine

        Dim account As Integer = 0
        Dim req As String
        Dim currentTail, LastTail, LastFosTail, LastCasTail As String



        '20120815 - pab - run time improvements
        Dim p1 As Date = CDate(Date.UtcNow.Month & "/" & Date.UtcNow.Day & "/" & Date.UtcNow.Year & " 10:00 AM")
        req = "select id, TripNumber, ltrim(rtrim(AirportFrom)) as AirportFrom, ltrim(rtrim(AirportTo)) as AirportTo, PinnedOn, Pinned from pinned " &
            "where pinned = 1 and carrierid = 'gef' and pinnedon > '" & p1 & "' order by tripnumber"
        req = Replace(req, "gef", Session("carrierid"))

        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx colorme start Get Pinnned " & Now, "", "")
        Dim dt As DataTable = DataAccess.GetPinnedFlights(req)
        Insertsys_log(Session("carrierid"), appName, "CASFlightsAzure.ascx colorme end Get Pinned " & Now, "", "")

        _carrierid = Session("carrierid")


        Dim dv_pinned As DataView = dt.DefaultView
        Dim bpinnedflights As Boolean = False

        If Not AirTaxi.isdtnullorempty(dt) Then
            bpinnedflights = True
        End If
        Dim colorsArray As System.Array =
        [Enum].GetValues(GetType(KnownColor))
        Dim allColors(colorsArray.Length) As KnownColor

        Array.Copy(colorsArray, allColors, colorsArray.Length)

        Dim d, r As Double


        Dim i As Integer
        For i = 0 To gridviewtrips.Rows.Count - 1
            Dim key, mycolor1 As String
            key = Trim(gridviewtrips.Rows(i).Cells(CAS_FROM).Text) & "-" & Trim(gridviewtrips.Rows(i).Cells(CAS_TO).Text) & "-" & Trim(gridviewtrips.Rows(i).Cells(CAS_TRIP).Text)
            '  mycolor1 = changecolor_dictionary(key)

            Dim value As String
            If (FCDRColorList.changecolor_dictionary.TryGetValue(key, value)) Then
                mycolor1 = value

                If CInt(mycolor1) > 40 Then
                    gridviewtrips.Rows(i).Cells(CAS_FROM).BackColor = Color.FromName(allColors(CInt(mycolor1) + 2).ToString)
                    gridviewtrips.Rows(i).Cells(CAS_TO).BackColor = Color.FromName(allColors(CInt(mycolor1) + 2).ToString)
                Else
                    gridviewtrips.Rows(i).Cells(CAS_FROM).BackColor = Color.FromName(colornames(CInt(mycolor1)))
                    gridviewtrips.Rows(i).Cells(CAS_TO).BackColor = Color.FromName(colornames(CInt(mycolor1)))

                End If

            Else
                mycolor1 = 0
            End If

            Dim xy As String = gridviewtrips.Rows(i).Cells(CAS_LTC).Text
            Dim x As String = gridviewtrips.Rows(i).Cells(CAS_LTC).Text
            Dim ltc As String = Trim(gridviewtrips.Rows(i).Cells(CAS_LTC).Text)
            Select Case Left(gridviewtrips.Rows(i).Cells(CAS_LTC).Text, 1)
                Case "D"
                    gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.White ' Drawing.Color.Wheat
                    gridviewtrips.Rows(i).Cells(CAS_FT).Text = "DH"
                Case "R"
                    'revenue
                    gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.White
                    gridviewtrips.Rows(i).Cells(CAS_FT).Text = "Rev"
                    If ltc = "CREW" Then
                        gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.SteelBlue
                        gridviewtrips.Rows(i).Cells(CAS_FT).Text = "C"
                    End If
                    If ltc = "M" Then
                        gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.SteelBlue
                        gridviewtrips.Rows(i).Cells(CAS_FT).Text = "M"
                    End If
                    If ltc = "MXSC" Or ltc = "SWAP" Then 'SWAP added by mws 11/25
                        gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.SteelBlue
                        gridviewtrips.Rows(i).Cells(CAS_FT).Text = "M"
                    End If
                    If ltc = "P" Then
                        gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.GreenYellow
                        gridviewtrips.Rows(i).Cells(CAS_FT).Text = "D"
                    End If
                    If ltc = "Y" Then
                        gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.Orange
                        gridviewtrips.Rows(i).Cells(CAS_FT).Text = "S"
                        gridviewtrips.Rows(i).Cells(CAS_FT).ToolTip = "Static"
                    End If
                Case Else
                    If Trim(gridviewtrips.Rows(i).Cells(CAS_TRIP).Text) = "12345" Then
                        gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.DarkOrange
                        gridviewtrips.Rows(i).Cells(CAS_FT).Text = "M"
                    End If
            End Select

            'Dim lrc As String = gridviewtrips.Rows(i).Cells(15).Text
            'Dim lpc As String = gridviewtrips.Rows(i).Cells(16).Text


            If CStr("AOG,DETL,INSV,MXRC,MXUS,MAINT,MXSC,M,SWAP").Contains(ltc) And ltc <> "D" Then 'SWAP added by mws 11/25
                gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.SteelBlue
                gridviewtrips.Rows(i).Cells(CAS_FT).Text = "M"
            End If

            If ltc = "77" Then
                gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.SteelBlue
                gridviewtrips.Rows(i).Cells(CAS_FT).Text = "M"
            End If

            'rk 7/20/2013
            If ltc = "BULP" Then
                gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(CAS_FT).Text = "B"
            End If


            'rk 7/20/2013
            If ltc = "CREW" Then
                gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(CAS_FT).Text = "C"
            End If


            If ltc = "TRNG" Then
                gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.DarkKhaki
                gridviewtrips.Rows(i).Cells(CAS_FT).Text = "T"
            End If
            If ltc = "LINB" Then
                gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.GreenYellow
                gridviewtrips.Rows(i).Cells(CAS_FT).Text = "L"
            End If

            'If gridviewtrips.Rows(i).Cells(CAS_FT).Text = "DH" Then gridviewtrips.Rows(i).Cells(CAS_FT).BackColor = Drawing.Color.GreenYellow
            If i > 0 Then
                currentTail = If(Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text) <> "&nbsp;", Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text), Trim(gridviewtrips.Rows(i).Cells(CAS_AC).Text))
                LastTail = If(Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text) <> "&nbsp;", Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text), Trim(gridviewtrips.Rows(i - 1).Cells(CAS_AC).Text))

                gridviewtrips.Rows(i).Cells(CAS_AC).Font.Bold = False
                If gridviewtrips.Rows(i).Cells(CAS_AC).Text <> gridviewtrips.Rows(i - 1).Cells(CAS_AC).Text Then 'ac change
                    gridviewtrips.Rows(i).Cells(CAS_AC).Font.Bold = True
                Else
                    If gridviewtrips.Rows(i).Cells(CAS_FROM).Text <> gridviewtrips.Rows(i - 1).Cells(CAS_TO).Text Then
                        If gridviewtrips.Rows(i).Cells(CAS_FROM).Text <> "SWAP" And gridviewtrips.Rows(i - 1).Cells(CAS_TO).Text <> "SWAP" Then
                            gridviewtrips.Rows(i).Cells(CAS_FROM).BackColor = Drawing.Color.IndianRed
                            gridviewtrips.Rows(i - 1).Cells(CAS_TO).BackColor = Drawing.Color.IndianRed
                            ' caslinebreaks = caslinebreaks + 1
                        Else
                            gridviewtrips.Rows(i).Cells(CAS_FROM).BackColor = Drawing.Color.MediumPurple
                            gridviewtrips.Rows(i - 1).Cells(CAS_TO).BackColor = Drawing.Color.MediumPurple
                        End If
                    End If

                    Dim prevflight As Date = CDate(convdate(gridviewtrips.Rows(i - 1).Cells(CAS_TOGMT).Text))
                    Dim currflight As Date = CDate(convdate(gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text))
                    Dim diff As Integer = DateDiff(DateInterval.Minute, prevflight, currflight)

                    If diff < 30 And Trim(gridviewtrips.Rows(i - 1).Cells(CAS_TOGMT).Text) <> "&nbsp;" Then gridviewtrips.Rows(i - 1).Cells(CAS_TOGMT).BackColor = Drawing.Color.Yellow
                    If diff < 30 And Trim(gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text) <> "&nbsp;" Then gridviewtrips.Rows(i).Cells(CAS_FROMGMT).BackColor = Drawing.Color.Yellow
                    If diff < 0 And Trim(gridviewtrips.Rows(i - 1).Cells(CAS_TOGMT).Text) <> "&nbsp;" Then gridviewtrips.Rows(i - 1).Cells(CAS_TOGMT).BackColor = Drawing.Color.Salmon
                    If diff < 0 And Trim(gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text) <> "&nbsp;" Then gridviewtrips.Rows(i).Cells(CAS_FROMGMT).BackColor = Drawing.Color.Salmon
                End If
            End If
            'gridviewtrips.Rows(i).BackColor = mycolor

            Dim pinned As Boolean = False
            If bpinnedflights = True Then
                dv_pinned.RowFilter = "TripNumber = '" & gridviewtrips.Rows(i).Cells(CAS_TRIP).Text & "' and AirportFrom = '" &
                    gridviewtrips.Rows(i).Cells(CAS_FROM).Text.ToString & "' and AirportTo = '" & gridviewtrips.Rows(i).Cells(CAS_TO).Text.ToString & "'"
                If dv_pinned.Count > 0 Then
                    pinned = True
                End If
            End If

            If pinned Then
                gridviewtrips.Rows(i).Cells(CAS_FROMGMT).BackColor = Drawing.Color.Goldenrod
            End If

            If IsDate(gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text) Then
                Dim gmt As Date = DateTime.UtcNow
                Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text)
                gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
            End If
            If IsDate(gridviewtrips.Rows(i).Cells(CAS_TOGMT).Text) Then
                Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(CAS_TOGMT).Text)
                gridviewtrips.Rows(i).Cells(CAS_TOGMT).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
            End If

            'gridviewtrips.Rows(i).Cells(CAS_COST) =
            Dim lb As LinkButton
            lb = gridviewtrips.Rows(i).FindControl("lnkPTSindividual")
            gridviewtrips.Rows(i).Cells(CAS_FROM).Style.Add("border-left", "8px solid White")
            If Trim(gridviewtrips.Rows(i).Cells(CAS_AC).Text) = Trim(gridviewtrips.Rows(i).Cells(CAS_PT).Text) Then
                lb.Text = ""
                ' gridviewtrips.Rows(i).Cells(CAS_IND).Text = ""
            End If
        Next i
        gridviewtrips.Columns(CAS_FROM).HeaderStyle.CssClass = "borderbreak"


        For i = 0 To gridviewtrips.Rows.Count - 1

            If Left(gridviewtrips.Rows(i).Cells(CAS_FT).Text, 1) = "D" Then
                d = d + gridviewtrips.Rows(i).Cells(CAS_NM).Text
                gridviewtrips.Rows(i).Cells(CAS_FT).Text = "DH"
            End If

            If Left(gridviewtrips.Rows(i).Cells(CAS_FT).Text, 1) = "R" Then
                r = r + gridviewtrips.Rows(i).Cells(CAS_NM).Text
                gridviewtrips.Rows(i).Cells(CAS_FT).Text = "Rev"
            End If

            Try
                gridviewtrips.Rows(i).Cells(CAS_TO).ToolTip = fname(gridviewtrips.Rows(i).Cells(CAS_TO).Text)
                gridviewtrips.Rows(i).Cells(CAS_FROM).ToolTip = fname(gridviewtrips.Rows(i).Cells(CAS_FROM).Text)
            Catch
            End Try

            'added plane lookup info back in rk 7.20.17
            If gridviewtrips.Rows(i).Cells(CAS_AC).Text <> "" Then
                gridviewtrips.Rows(i).Cells(CAS_AC).ToolTip = AirTaxi.lookupac(gridviewtrips.Rows(i).Cells(CAS_AC).Text)
            End If
        Next i

    End Function
    Function linebreaks(ByRef gridviewtrips As GridView) ', ACType As String)
        Dim currentTail, LastTail As String
        Dim maxrows As Integer = 1
        Dim foscount, cascount As Integer

        foscount = 0
        cascount = 0
        For i = 0 To gridviewtrips.Columns.Count - 1
            If gridviewtrips.Columns(i).Visible Then
                If i < CAS_FROM Then
                    foscount += 1
                Else
                    cascount += 1
                End If
            End If
        Next

        For i = 1 To gridviewtrips.Rows.Count - 1
            currentTail = If(Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text) <> "&nbsp;", Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text), Trim(gridviewtrips.Rows(i).Cells(CAS_AC).Text))
            LastTail = If(Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text) <> "&nbsp;", Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text), Trim(gridviewtrips.Rows(i - 1).Cells(CAS_AC).Text))
            If currentTail <> LastTail Then
                Dim row As New GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Alternate)
                For zzz = 0 To 1
                    Dim cell As New TableCell
                    cell.Text = "&nbsp;"
                    cell.ColumnSpan = If(zzz = 0, foscount, cascount) '14,13
                    'cell.Text = Espace
                    row.Cells.Add(cell)
                Next
                row.Cells(1).Style.Add("border-left", "8px solid white")
                row.BackColor = Drawing.Color.FromArgb(0, 147, 111)
                gridviewtrips.Controls(0).Controls.AddAt(i + maxrows, row) ' This line will insert row at 2nd line
                maxrows += 1
            End If
        Next
        gridviewtrips.Columns(CAS_PT).Visible = False
    End Function

    Protected Sub GVGridViewTrips_PreRender(sender As Object, e As EventArgs)
        Dim i As Integer = 0
        Dim x As String

        For i = 0 To lvflightlist.Items.Count - 1
            x = DirectCast(lvflightlist.Items(i).FindControl("pnlFCDR"), Label).Text
            If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblNonRevDeltab"), Label).Text) > 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblNonRevDeltab"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostSavings"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostSavings"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay0b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay0b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay1b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay1b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay2b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay2b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            FOScolorme(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), fcdrcolors.Where(Function(z) z.FCDR_Key = x).Select(Function(z) z).FirstOrDefault()) ', DirectCast(lvflightlist.Items(1).FindControl("pnlACType"), Label).Text)
            CAScolorme(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), fcdrcolors.Where(Function(z) z.FCDR_Key = x).Select(Function(z) z).FirstOrDefault())
            linebreaks(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView)) ', DirectCast(lvflightlist.Items(1).FindControl("pnlACType"), Label).Text)
            'GetSavings(lvflightlist.Items(i))
        Next
        'If CreatePDF Then makeMYPDF(lvflightlist)
        makeMYPDF()
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

    Function getColorDictionary(ByVal caslist As List(Of CASFlightsOptimizerRecord)) As Dictionary(Of String, String)
        changecolor_dictionary.Clear()
        newTail_Dictionary.Clear()

        Dim mycolor As Integer = 1
        Dim oldprior, oldtail As String
        Dim key As String

        For x = 0 To caslist.Count - 1

            If Trim(caslist(x).AircraftRegistration) <> oldtail Then
                oldtail = Trim(caslist(x).AircraftRegistration)
                oldprior = ""
            End If
            If Trim(caslist(x).PriorTail) <> "" Then
                If Trim(caslist(x).PriorTail) <> Trim(caslist(x).AircraftRegistration) Then
                    If Trim(caslist(x).PriorTail) = oldprior Then
                        If Trim(caslist(x).LegTypeCode) <> "B" Then
                            key = Trim(caslist(x).DepartureAirport) & "-" & Trim(caslist(x).ArrivalAirport) & "-" & Trim(caslist(x).TripNumber)
                            changecolor_dictionary(key) = mycolor
                            newTail_Dictionary(key) = Trim(caslist(x).AircraftRegistration)
                        End If
                    End If
                End If
            End If

            If Trim(caslist(x).PriorTail) <> "" Then
                If Trim(caslist(x).PriorTail) <> Trim(caslist(x).AircraftRegistration) Then
                    If Trim(caslist(x).PriorTail) <> Trim(oldprior) Then
                        oldprior = Trim(caslist(x).PriorTail)
                        If Trim(caslist(x).LegTypeCode) <> "B" Then
                            mycolor = mycolor + 1
                            key = Trim(caslist(x).DepartureAirport) & "-" & Trim(caslist(x).ArrivalAirport) & "-" & Trim(caslist(x).TripNumber)
                            changecolor_dictionary(key) = mycolor
                            newTail_Dictionary(key) = Trim(caslist(x).AircraftRegistration)
                        End If

                    End If

                End If
            End If
        Next
        Return changecolor_dictionary
    End Function
    Public Sub setcolors()
        colornames(0) = "AliceBlue"
        colornames(1) = "Wheat"
        colornames(2) = "SkyBlue"
        colornames(3) = "Wheat"
        colornames(4) = "Palegreen"
        colornames(5) = "LightPink"
        colornames(6) = "YellowGreen"
        colornames(7) = "CadetBlue"
        colornames(8) = "Coral"
        colornames(9) = "CornflowerBlue"
        colornames(10) = "ForestGreen"
        colornames(11) = "Gold"
        colornames(12) = "LemonChiffon"
        colornames(13) = "LightBlue"
        colornames(14) = "LightCoral"
        colornames(15) = "LightPink"
        colornames(16) = "LightSalmon"
        colornames(17) = "Lime"
        colornames(18) = "LightYellow"
        colornames(19) = "LightSteelBlue"
        colornames(20) = "MediumOrchid"
        colornames(21) = "MediumPurple"
        colornames(22) = "MediumSpringGreen"
        colornames(23) = "MediumSlateBlue"
        colornames(24) = "MintCream"
        colornames(25) = "Orange"
        colornames(26) = "OrangeRed"
        colornames(27) = "PowderBlue"
        colornames(28) = "Bisque"
        colornames(29) = "SeaGreen"
        colornames(30) = "SeaShell"
        colornames(31) = "Sienna"
        colornames(32) = "SkyBlue"
        colornames(33) = "SpringGreen"
        colornames(34) = "MintCream"
        colornames(35) = "SteelBlue"
        colornames(36) = "Teal"
        colornames(37) = "Wheat"
        colornames(38) = "Violet"
        colornames(39) = "YellowGreen"
        colornames(40) = "Tomato"

    End Sub
    Sub makeMYPDF()
        Dim DoneFile As String = Server.MapPath("FCDRpages/" + Filename + ".pdf")
        If File.Exists(DoneFile) Then File.Delete(DoneFile)

        Dim htmlString As String
        Dim baseUrl As String = "http://optimizerpanel.com/FCDRpages/"

        Dim pdf_page_size As String = "Letter"
        Dim pageSize As PdfPageSize = DirectCast([Enum].Parse(GetType(PdfPageSize), pdf_page_size, True), PdfPageSize)

        Dim pdf_orientation As String = "Landscape"
        Dim pdfOrientation As PdfPageOrientation = DirectCast([Enum].Parse(GetType(PdfPageOrientation), pdf_orientation, True), PdfPageOrientation)

        Dim webPageWidth As Integer = 1400
        Dim webPageHeight As Integer = 0

        ' instantiate a html to pdf converter object
        Dim converter As New HtmlToPdf()
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        lvflightlist.RenderControl(hw) 'form1
        htmlString = sw.ToString()

        ' set converter options
        converter.Options.PdfPageSize = pageSize
        converter.Options.PdfPageOrientation = pdfOrientation
        converter.Options.WebPageWidth = webPageWidth
        converter.Options.WebPageHeight = webPageHeight

        ' create a new pdf document converting an url
        Dim doc As PdfDocument = converter.ConvertHtmlString(htmlString, baseUrl)

        ' save pdf document
        doc.Save(File.Create(DoneFile))

        ' close pdf document
        doc.Close()

    End Sub
    Public Overrides Sub VerifyRenderingInServerForm(control As Control)
        'MyBase.VerifyRenderingInServerForm(control)
    End Sub

End Class