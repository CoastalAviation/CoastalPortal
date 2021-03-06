﻿Imports CoastalPortal.AirTaxi
Imports CoastalPortal.DataAccess
Imports CoastalPortal.Personibid
Imports CoastalPortal.Models
Imports System.Drawing
Imports System.IO
Imports SelectPdf
Imports System.Collections.Generic
Imports System.Data.Objects
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Collections
Imports System.Net
Imports System.Data.Entity


Public Class FlightChangeDetail
    Inherits System.Web.UI.Page

    Public myJS As String = String.Empty
    Public myJS1 As String = String.Empty

    Public fcdrlist As New List(Of FCDRList)
    Public demandlookup As New Dictionary(Of String, Boolean)
    Public FOSRecords As New List(Of FOSFlightsOptimizerRecord)
    Public CASRecords As New List(Of CASFlightsOptimizerRecord)
    Private carrierprofile As New CarrierProfile
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
    Public Const FOS_REV As Integer = 13
    Public Const FOS_PROREV As Integer = 14
    Public Const FOS_COST As Integer = 15
    Public Const FOS_PANDL As Integer = 16
    Public Const FOS_BASE As Integer = 17
    Public Const FOS_LEGBASE As Integer = 18
    Public Const FOS_QE As Integer = 19
    Public Const FUTURE_TAIL As Integer = 20
    Public Const CAS_FROM As Integer = 21
    Public Const CAS_TO As Integer = 22
    Public Const CAS_FROMGMT As Integer = 23
    Public Const CAS_TOGMT As Integer = 24
    Public Const CAS_NM As Integer = 25
    Public Const CAS_AC As Integer = 26
    Public Const CAS_TYPE As Integer = 27
    Public Const CAS_FT As Integer = 28
    Public Const CAS_TRIP As Integer = 29
    Public Const CAS_IND As Integer = 30
    Public Const CAS_LTC As Integer = 31
    Public Const CAS_SIC As Integer = 32
    Public Const CAS_PIC As Integer = 33
    Public Const CAS_REV As Integer = 34
    Public Const CAS_PROREV As Integer = 35
    Public Const CAS_COST As Integer = 36
    Public Const CAS_PANDL As Integer = 37
    Public Const CAS_BASE As Integer = 38
    Public Const CAS_LEGBASE As Integer = 39
    Public Const CAS_PIN As Integer = 40
    Public Const REJECTBTN As Integer = 41
    Public Const CAS_PT As Integer = 42
    Public Const RECORD_ID As Integer = 43
    Public Const FCDR_DEPARTDATE As Integer = 44
    Public Const CAS_HA As Integer = 45
    Public Const CAS_OE As Integer = 46
    Public Property DepartDate As DateTime

    '20180817 - pab - do not use session or global variables for this page
    Public carrierid As Integer
    Public casmodelrunid As String
    Public urlalias As String
    Public FCDRKey As String

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

        '20180817 - pab - do not use session or global variables for this page
        'Session("FltChange") = "FC"
        'Session("FCList") = ""

        'Dim carrierprofile As New CarrierProfile

        'TODO .. Add Code to get reject button here.

        '  Dim ac1, ac2 As String
        Dim mrid As String = ""
        Dim id As Integer = 0
        Dim pt, ar As String
        Dim mrcustom As String
        Dim casRecord As CASFlightsOptimizerRecord

        '20180820 - pab - create fcdr directly from dc request if tasks backlogged
        Dim modelrun As Integer = 0

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        '20180817 - pab - do not use session or global variables for this page
        'If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        'If IsNothing(Session("casmodelrunid")) Then Session("casmodelrunid") = ""
        'Dim carrierid As Integer = Session("carrierid")
        'Dim casmodelrunid As String = Session("casmodelrunid")
        Dim da As New DataAccess
        Dim dtcarrier As DataTable
        carrierid = 0
        casmodelrunid = ""
        urlalias = ""

        '20180817 - pab - do not use session or global variables for this page
        'FOSRecords = Session("FOS")
        'CASRecords = Session("CAS")
        'carrierprofile = Session("Profile")

        demandlookup.Clear()

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
            demandlist = db.Database.SqlQuery(Of String)("Select placeholder from DemandList").ToList()
            For Each x As String In demandlist
                demandlookup.Add(Trim(x), True)
            Next

            If Not IsPostBack Then
                '20180820 - pab - create fcdr directly from dc request if tasks backlogged
                'If Not Request.QueryString("key") Is Nothing Then
                If Not Request.QueryString("key") Is Nothing Or Not Request.QueryString("modelrun") Is Nothing Then
                    If Not Request.QueryString("key") Is Nothing Then
                        id = Request.QueryString("key")
                        fcdrlist = db.FCDRList.Where(Function(c) c.keyid = id).ToList()
                    End If
                    If Not Request.QueryString("modelrun") Is Nothing Then
                        modelrun = Request.QueryString("modelrun")
                        fcdrlist = db.FCDRList.Where(Function(c) c.ModelRun = modelrun).ToList()
                        If fcdrlist.Count = 0 Then
                            id = fcdrlist(0).keyid
                        End If
                    End If
                    If fcdrlist.Count = 0 Then
                        CreatePDF = True
                    End If
                    If Not Request.QueryString("carrier") Is Nothing Then
                        '20180817 - pab - do not use session or global variables for this page
                        'Session("carrierid") = Request.QueryString("carrier")
                        carrierid = Request.QueryString("carrier")
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

                        '20180817 - pab - do not use session or global variables for this page
                        carrierid = casRecord.CarrierId
                        dtcarrier = da.GetProviderByCarrierID(carrierid)
                        If Not isdtnullorempty(dtcarrier) Then
                            urlalias = dtcarrier.Rows(0).Item("alias")
                        End If

                        mrid = Trim(casRecord.OptimizerRun)
                        pt = casRecord.PriorTail
                        ar = casRecord.AircraftRegistration
                        ACX(0) = Trim(ar)
                        ACX(1) = Trim(pt)
                        casmodelrunid = mrid
                        carrierprofile = db.CarrierProfiles.Find(casRecord.CarrierId)
                    Else
                        mrid = fcdrlist(0).ModelRunID.ToString()
                        carrierprofile = db.CarrierProfiles.Find(fcdrlist(0).CarrierID)
                        carrierid = carrierprofile.carrierid

                        '20180817 - pab - do not use session or global variables for this page
                        If CASRecords.Count = 0 Then CASRecords = Nothing
                        If FOSRecords.Count = 0 Then FOSRecords = Nothing

                    End If
                Else
                    If Not Request.QueryString("ModelSavings") Is Nothing Then
                        mrid = Request.QueryString("ModelSavings")
                        ACX(0) = Trim(InBetween(1, mrid, "A1[", "]"))
                        ACX(1) = Trim(InBetween(1, mrid, "A2[", "]"))
                        mrid = Trim(InBetween(1, mrid, "A0[", "]"))
                    Else
                        If carrierprofile IsNot Nothing Then
                            If carrierprofile.carrierid = 0 Then Exit Sub
                        Else
                            Exit Sub
                        End If

                        Dim today = DateAdd("d", -2, DateTime.Now)

                        fcdrlist = db.FCDRList.Where(Function(c) c.CarrierID = carrierid And c.TotalSavings > 999 And c.GMTStart >= today).OrderByDescending(Function(c) c.TotalSavings).ToList()
                        mrid = fcdrlist.Where(Function(b) b.ModelRun = fcdrlist.Max(Function(c) c.ModelRun)).Select(Function(c) c.ModelRunID).FirstOrDefault()
                    End If
                    Dim i As Integer = 1
                    fcdrlist = fcdrlist.Where(Function(c) c.ModelRunID = mrid).ToList()
                    fcdrlist = fcdrlist.Where(Function(c) c.ModelRunID = mrid And c.TotalSavings = fcdrlist.Max(Function(bc) bc.TotalSavings)).ToList()
                    If fcdrlist.Count > 1 Then
                        Do While i <> fcdrlist.Count
                            Dim checkme = fcdrlist(i - 1)
                            If fcdrlist(i).PriorTailNumber = checkme.PriorTailNumber And fcdrlist(i).ModelRun = checkme.ModelRun And fcdrlist(i).TotalSavings = checkme.TotalSavings Then
                                fcdrlist.Remove(fcdrlist(i))
                                i -= 1
                            End If
                            i += 1
                        Loop
                    End If
                    If fcdrlist.Count = 0 Then
                        CreatePDF = True
                        If CASRecords Is Nothing Then
                            casRecord = db.CASFlightsOptimizer.Where(Function(x) x.OptimizerRun = mrid).First()
                        Else
                            casRecord = CASRecords.Where(Function(x) x.OptimizerRun = mrid).First()
                        End If

                        id = casRecord.ID
                        mrid = casRecord.OptimizerRun
                    Else
                        mrid = fcdrlist(0).ModelRunID.ToString()
                    End If
                End If
                If awclookup.Count = 0 Then
                    awclookup.Clear()
                    AWC = db.AircraftWeightClass.ToList()
                    For Each x As WeightClass In AWC
                        '20180905 - pab - fix error - An item with the same key has already been added
                        Try
                            awclookup.Add(Trim(x.AircraftType), x.AircraftWeightClass)
                        Catch ex As Exception
                            If InStr(ex.Message, "An item with the same key has already been added") = 0 Then
                                Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
                                If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
                                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                                AirTaxi.Insertsys_log(carrierid, appName, s, "Page_Load", "FlightChangeDetail.aspx.vb")
                                If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                                    SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                                        appName & " FlightChangeDetail.aspx.vb Page_Load error", s, carrierid)
                                End If
                            End If
                        End Try
                    Next
                End If

                '20180817 - pab - do not use session or global variables for this page
                'Session("modelrunid") = mrid
                'If Session("overridemodel") = "" Then
                '    mrcustom = normalizemodelrunid(Session("modelrunid").ToString)
                'Else
                '    mrcustom = Session("overridemodel")
                'End If
                'If Session("overridemodel") <> "" And ACX(1) <> "" Then
                '    mrcustom = normalizemodelrunid(Session("modelrunid").ToString)
                'End If

                mrcustom = normalizemodelrunid(mrid)
                If FOSRecords Is Nothing Then
                    FOSRecords = db.FOSFlightsOptimizer.Where(Function(x) x.OptimizerRun = mrcustom).ToList()
                End If
                If CASRecords Is Nothing Then
                    CASRecords = db.CASFlightsOptimizer.Where(Function(x) x.OptimizerRun = mrid).ToList()
                End If

                'demandlookup.Clear()
                If carrierprofile Is Nothing Then
                    carrierprofile = db.CarrierProfiles.Find(CASRecords(0).CarrierId)
                End If

                If fcdrlist.Count = 0 Then
                    enumerate(ACX(0), id, mrid, ACX(1))
                    '20180817 - pab - do not use session or global variables for this page
                    'Session("carrierid") = casRecord.CarrierId
                    carrierid = casRecord.CarrierId

                End If

                '20180817 - pab - do not use session or global variables for this page
                If carrierid = 0 Then carrierid = carrierprofile.carrierid
                dtcarrier = da.GetProviderByCarrierID(carrierid)
                If Not isdtnullorempty(dtcarrier) Then
                    urlalias = dtcarrier.Rows(0).Item("alias")
                End If

                '20180817 - pab - do not use session or global variables for this page
                Session("FCDRKey") = fcdrlist
                FCDRKey = fcdrlist(0).keyid
                ' If Not Page.IsPostBack Then
                For Each fr As FOSFlightsOptimizerRecord In FOSRecords
                    If fr.ProRatedRevenue < 0 Then fr.ProRatedRevenue = 0
                Next

                For Each cr As CASFlightsOptimizerRecord In CASRecords
                    cr.ProRatedRevenue = FOSRecords.Where(Function(c) c.FOSKey = cr.FOSKEY).Select(Function(c) c.ProRatedRevenue).FirstOrDefault()
                Next

                GetTrips()
            Else
                Dim btnresult As String = Request.Form("btnacpt")
                If btnresult IsNot Nothing Then
                    Dim i = InStr(btnresult, "-")
                    Dim action = Left(btnresult, i - 1)
                    Dim KeyId = Mid(btnresult, i + 1)
                    RejectFlight(KeyId)
                End If

            End If

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "Page_Load", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb Page_Load error", s, carrierid)
            End If

        End Try

    End Sub

    Private Sub Flights24_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        ACX(1) = ""
        '20180817 - pab - do not use session or global variables for this page
        'Session("overridemodel") = ""

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
        Dim optreq As New OptimizerRequest
        Dim DynamicCosting As Boolean = False
        'Dim AClist As String

        Dim da As New DataAccess
        Dim dt As DataTable

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
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
                If (From g In CASRecords Where ptlist.Contains(Trim(g.PriorTail)) And Not ptlist.Contains(g.AircraftRegistration) Select g).Count > 0 Then
                    ptlist = ptlist.Union((From g In CASRecords Where ptlist.Contains(Trim(g.PriorTail)) And Not ptlist.Contains(Trim(g.AircraftRegistration)) Select Trim(g.AircraftRegistration)).ToList()).ToList()
                End If
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

            '20180817 - pab - do not use session or global variables for this page
            'Session("PTList") = ptlist

            Dim frc, cascost As Double
            Dim dcostday0 As Double = 0
            Dim dcostday1 As Double = 0
            Dim dcostday2 As Double = 0
            Dim totalcost As Double = 0
            Dim fnrm, cnrm As Double

            Dim i As Integer = model.IndexOf("-")

            mrid = Left(model, i)

            optreq = db.OptimizerRequest.Find(CInt(mrid))
            '20180827 - pab - Day 0 is supposed to be within 24 hours of model run time (not model start).  Day 1 would be 24-48, etc per David
            'GMTStart = optreq.GMTStart
            dt = da.GetFOSOptimizerRequestByID(carrierid, optreq.ID)
            If Not isdtnullorempty(dt) Then
                GMTStart = dt.Rows(0).Item("requestdate")
            Else
                GMTStart = optreq.GMTStart
            End If
            DynamicCosting = optreq.Description.Contains("Dynamic Costing")
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

            '20180614 - pab - change day2 bucket to days 2 +
            frc = isdbn((From fr In FOSRecords Where Trim(fr.LegTypeCode) <> "77" And Trim(fr.LegTypeCode) <> "7" And Trim(fr.LegState) <> "5" And
                                                ((Not demandlookup.TryGetValue(Trim(fr.AC), dummy) And ptlist.Contains(Trim(fr.AC))) Or
                                                 (demandlookup.TryGetValue(Trim(fr.AC), dummy) And TripList.Contains(Trim(fr.TripNumber)))) And
                                                fr.DateTimeGMT >= DateAdd("d", 2, GMTStart) Select CInt(Trim(fr.DHCost))).Sum())
            cascost = isdbn((From cr In CASRecords Where ptlist.Contains(Trim(cr.AircraftRegistration)) And
                                                   ((Not demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And ptlist.Contains(Trim(cr.AircraftRegistration))) Or
                                                    (demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And TripList.Contains(Trim(cr.TripNumber)))) And
                                                   cr.DepartureTime >= DateAdd("d", 2, GMTStart) Select CInt(Trim(cr.cost))).Sum())
            dcostday2 = frc - cascost

            fnrm = isdbn((From fr In FOSRecords Where Trim(fr.DeadHead) = "True" And Trim(fr.LegTypeCode) <> "77" And Trim(fr.LegTypeCode) <> "7" And Trim(fr.LegState) <> "5" And
                                    ((Not demandlookup.TryGetValue(Trim(fr.AC), dummy) And ptlist.Contains(Trim(fr.AC))) Or
                                           (demandlookup.TryGetValue(Trim(fr.AC), dummy) And TripList.Contains(Trim(fr.TripNumber)))) And
                                           fr.DateTimeGMT >= GMTStart Select CInt(Trim(fr.NauticalMiles))).Sum())
            cnrm = isdbn((From cr In CASRecords Where Trim(cr.FlightType) = "D" And ptlist.Contains(Trim(cr.AircraftRegistration)) And
                                                  ((Not demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And ptlist.Contains(Trim(cr.AircraftRegistration))) Or
                                                    (demandlookup.TryGetValue(Trim(cr.AircraftRegistration), dummy) And TripList.Contains(Trim(cr.TripNumber)))) And
                                                cr.DepartureTime >= GMTStart Select CInt(Trim(cr.Distance))).Sum())



            fcdrlist.Add(New FCDRList With {.CASRecordList = CasIDList, .FOSRecordList = FosIdList, .ModelRunID = model, .PriorTailNumber = starttail, .DeltaNonRevMiles = CInt(fnrm - cnrm), .CarrierAcceptStatus = "NA", .isTrade = False,
                    .TotalSavings = totalcost, .SavingsDay0 = dcostday0, .SavingsDay1 = dcostday1, .SavingsDay2 = dcostday2, .keyid = fcdrkey, .ModelRun = mrid, .GMTStart = GMTStart, .CarrierID = carrierprofile.carrierid, .DynamicCost = DynamicCosting})

            Try
                db.FCDRList.AddRange(fcdrlist)
                db.SaveChanges()
            Catch ex As Exception

            End Try

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & fcdrkey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "enumerate", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb enumerate error", s, carrierid)
            End If

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
        '20180817 - pab - do not use session or global variables for this page
        'Dim FClist As String = Session("FCList")
        Dim FClist As String = ""
        Dim dummy As Boolean = False
        Dim mrcustom As String = normalizemodelrunid(modelrunid)
        Dim baserev As New List(Of baseRevenue)
        Dim ACPRemiums As New List(Of AircrafPremium)

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
            ' Dim fnrm, cnrm As Double
            'FosList = FOSRecords.Where(Function(x) FClist.Contains(Trim(x.AC)) And x.LegState <> 5 And x.DateTimeGMT > DateTime.UtcNow).OrderBy(Function(y) y.TripNumber).ThenBy(Function(y) y.DepartureDateGMT).ToList()
            'CasList = CASRecords.Where(Function(x) FClist.Contains(Trim(x.AircraftRegistration))).OrderBy(Function(y) y.TripNumber).ThenBy(Function(y) y.DepartureTime).ToList()
            fcdrcolors.Clear()
            ACPRemiums = db.Database.SqlQuery(Of AircrafPremium)("Select FosAircraftID,Premium from AircraftPremium").ToList()

            For Each fcdr As FCDRList In fcdrlist
                '20180608 - pab - slide start time
                '20180827 - pab - fix math - totals wrong, Day 0 is supposed to be within 24 hours of model run time (not model start).  Day 1 would be 24-48, etc
                GMTStart = fcdr.GMTStart
                'GMTStart = DateAdd(DateInterval.Hour, -3, fcdr.GMTStart)
                '20180511 - pab - fix fcdrs
                FosList = FOSRecords.Where(Function(x) fcdr.FOSRecordList.Contains(Trim(x.AC)) And Not demandlookup.TryGetValue(Trim(x.AC), dummy) And x.DateTimeGMT.Date >= GMTStart.Date).OrderBy(Function(y) y.AC).ThenBy(Function(y) y.DepartureDateGMT).Distinct().ToList()
                CasList = CASRecords.Where(Function(x) fcdr.CASRecordList.Contains(Trim(x.AircraftRegistration)) And Not demandlookup.TryGetValue(Trim(x.AircraftRegistration), dummy) And x.DepartureTime >= GMTStart).OrderBy(Function(y) y.AircraftRegistration).ThenBy(Function(y) y.DepartureTime).Distinct().ToList()
                'FosList = FOSRecords.Where(Function(x) fcdr.FOSRecordList.Contains(Trim(x.AC) And x.DepartureAirportICAO <> x.ArrivalAirportICAO And x.LegTypeCode <> "BULL") And Not demandlookup.TryGetValue(Trim(x.AC), dummy) And x.DateTimeGMT.Date >= GMTStart.Date).OrderBy(Function(y) y.AC).ThenBy(Function(y) y.DepartureDateGMT).Distinct().ToList()
                'CasList = CASRecords.Where(Function(x) fcdr.CASRecordList.Contains(Trim(x.AircraftRegistration) And x.DepartureAirport <> x.ArrivalAirport And x.LegTypeCode <> "BULL") And Not demandlookup.TryGetValue(Trim(x.AircraftRegistration), dummy) And x.DepartureTime >= GMTStart).OrderBy(Function(y) y.AircraftRegistration).ThenBy(Function(y) y.DepartureTime).Distinct().ToList()

                TripList = (From a In CasList Select Trim(a.TripNumber)).Distinct().ToList()
                FosList = FosList.Union(FOSRecords.Where(Function(x) demandlookup.TryGetValue(Trim(x.AC), dummy) And TripList.Contains(Trim(x.TripNumber)) And x.DateTimeGMT.Date >= GMTStart.Date).OrderBy(Function(y) y.AC).ThenBy(Function(y) y.DepartureDateGMT).ToList()).Distinct().ToList()
                CasList = CasList.Union(CASRecords.Where(Function(x) demandlookup.TryGetValue(Trim(x.AircraftRegistration), dummy) And TripList.Contains(Trim(x.TripNumber)) And x.DepartureTime >= GMTStart).OrderBy(Function(y) y.AircraftRegistration).ThenBy(Function(y) y.DepartureTime).ToList()).Distinct().ToList()

                FosList = (From a In FosList Select a).Distinct().ToList()
                CasList = (From b In CasList Select b).Distinct().ToList()

                '20180504 - pab - foslist sometimes contains rows it shouldn't - datetimegmt less than gmtstart
                'this code fixes key=1517458797 - dpj
                If FosList.Count > 1 Then
                    Dim i As Integer = 0
                    '20180518 - pab - fix error - index out of range
                    'Do While i <> FosList.Count - 1
                    Do While i <> FosList.Count - 1 And FosList.Count > 1
                        If FosList(i).DateTimeGMT < GMTStart Then
                            FosList.Remove(FosList(i))
                            If i > 0 Then i -= 1
                        End If
                        i += 1
                    Loop
                End If
                If CasList.Count > 1 Then
                    Dim i As Integer = 0
                    '20180518 - pab - fix error - index out of range
                    'Do While i <> CasList.Count - 1
                    Do While i <> CasList.Count - 1 And CasList.Count > 1
                        If CasList(i).DepartureTime < GMTStart Then
                            CasList.Remove(CasList(i))
                            If i > 0 Then i -= 1
                        End If
                        i += 1
                    Loop
                End If

                fcdrcolors.Add(New fcdrColorlist With {.FCDR_Key = fcdr.keyid, .changecolor_dictionary = getColorDictionary(CasList)})

                ACList = (From a In FosList Select Trim(a.AC)).Distinct().ToList()
                ACList = ACList.Union((From a In CasList Select Trim(a.AircraftRegistration)).Distinct()).ToList()
                '20180522 - pab - check cas prior tail too
                ACList = ACList.Union((From a In CasList Select Trim(a.PriorTail)).Distinct()).ToList()
                ACList = ACList.Distinct().ToList()
                '20180522 - pab - check cas prior tail too
                If ACList.Count > 1 Then
                    Dim i As Integer = 0
                    Do While i <> ACList.Count - 1 And ACList.Count > 1
                        If ACList(i) = "" Then
                            ACList.Remove(ACList(i))
                            If i > 0 Then i -= 1
                        End If
                        i += 1
                    Loop
                End If

                ' Panellist = (From f In FosList Group Join c In CasList On Trim(f.AC) Equals Trim(c.AircraftRegistration) And ((Trim(f.TripNumber) = Trim(c.TripNumber) And
                ' Trim(f.ArrivalAirportICAO) = Trim(c.ArrivalAirport) And Trim(f.DepartureAirportICAO) = Trim(c.DepartureAirport) And c.DepartureTime = f.DateTimeGMT) Or Trim(f.FOSKey) = Trim(c.FOSKEY))
                'Into Plist = Group From p In Plist.DefaultIfEmpty() Select New PanelRecord With {.FCDR_Key = fcdr.keyid, .CASRecord = p, .FOSRecord = f}).Distinct().ToList()
                Panellist = (From f In FosList Group Join c In CasList On Trim(f.AC) Equals Trim(c.AircraftRegistration) And Trim(f.TripNumber) Equals Trim(c.TripNumber) And Trim(f.FOSKey) Equals Trim(c.FOSKEY)
                                               Into Plist = Group From p In Plist.DefaultIfEmpty() Select New PanelRecord With {.FCDR_Key = fcdr.keyid, .CASRecord = p, .FOSRecord = f}).Distinct().ToList()

                'PanellistRight = (From c In CasList Group Join f In FosList On Trim(f.AC) Equals Trim(c.AircraftRegistration) And Trim(f.ArrivalAirportICAO) Equals Trim(c.ArrivalAirport) And
                '                                        Trim(f.DepartureAirportICAO) Equals Trim(c.DepartureAirport) And Trim(f.TripNumber) Equals Trim(c.TripNumber) And c.DepartureTime Equals f.DateTimeGMT
                '                                        Into Plist = Group From p In Plist.DefaultIfEmpty() Select New PanelRecord With {.FCDR_Key = fcdr.keyid, .FOSRecord = p, .CASRecord = c}).Distinct().ToList()
                PanellistRight = (From c In CasList Group Join f In FosList On Trim(f.AC) Equals Trim(c.AircraftRegistration) And Trim(f.TripNumber) Equals Trim(c.TripNumber) And Trim(f.FOSKey) Equals Trim(c.FOSKEY)
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
                BaseList = (From a In FosList Select Right(Trim(a.BaseCode), 3)).Distinct().ToList()
                BaseList = BaseList.Union((From a In CasList Where a.ProRatedRevenue >= 0 Select Right(Trim(a.LegBaseCode), 3)).Distinct().ToList()).ToList()

                Dim ii As Integer = 0
                Dim Premium As Decimal = 0

                If carrierprofile.FCDRPandL Then
                    For Each x As String In BaseList
                        Dim Caspandl, Fospandl, FosCost, CasRevenue, FosRevenue, cascost, CasBasePremium, FosBasePremium, BasePremium, ActualCost As Decimal
                        FosCost = 0
                        CasRevenue = 0
                        Fospandl = 0
                        Caspandl = 0
                        FosRevenue = 0
                        cascost = 0
                        ActualCost = 0
                        BasePremium = 0
                        CasBasePremium = 0
                        FosBasePremium = 0
                        For Each dd As String In TripList
                            'All home base have operating cost of thier own fleet -- note this will include premiums we will deduct later
                            FosCost += CDbl((From a In FosList Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.BaseCode), 3) = x Select CDbl(a.DHCost)).Sum())
                            cascost += CDbl((From a In CasList Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.BaseCode), 3) = x Select a.cost).Sum())
                            ' All Revenue to Quoted Base -- 
                            FosRevenue += CDbl((From a In FosList Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.LegBaseCode), 3) = x Select a.ProRatedRevenue).Sum())
                            CasRevenue += CDbl((From a In CasList Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.LegBaseCode), 3) = x Select a.ProRatedRevenue).Sum())
                            'Cost paid by Quoted Base to Other Base Aircraft + Premium 
                            cascost += CDbl((From a In CasList Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.BaseCode), 3) <> x And Right(Trim(a.LegBaseCode), 3) = x Select a.cost).Sum())
                            FosCost += CDbl((From a In FosList Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.BaseCode), 3) <> x And Right(Trim(a.LegBaseCode), 3) = x Select CDbl(a.DHCost)).Sum())
                            'Add revenue to other base this is the cost from Revenue Base and includes the premium as it should it is revenue
                            FosRevenue += CDbl((From a In FosList Join p In ACPRemiums On Trim(p.FosAircraftID) Equals Trim(a.AC) Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.LegBaseCode), 3) <> x And Right(Trim(a.BaseCode), 3) = x Select CDbl(a.DHCost)).Sum())
                            CasRevenue += CDbl((From a In CasList Join p In ACPRemiums On Trim(p.FosAircraftID) Equals Trim(a.AircraftRegistration) Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.LegBaseCode), 3) <> x And Right(Trim(a.BaseCode), 3) = x Select a.cost).Sum())
                            'Reduce Owning AC base cost by premium they dont pay to fly thier own plane and add Premium to PRemium column for show.
                            ActualCost = CDbl((From a In FosList Join p In ACPRemiums On Trim(p.FosAircraftID) Equals Trim(a.AC) Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.LegBaseCode), 3) <> x And Right(Trim(a.BaseCode), 3) = x Select CDbl(a.DHCost) / CDbl(p.Premium)).Sum())
                            BasePremium = CDbl((From a In FosList Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.LegBaseCode), 3) <> x And Right(Trim(a.BaseCode), 3) = x Select CDbl(a.DHCost)).Sum()) - ActualCost
                            FosBasePremium += BasePremium
                            FosCost -= BasePremium
                            ActualCost = CDbl((From a In CasList Join p In ACPRemiums On Trim(p.FosAircraftID) Equals Trim(a.AircraftRegistration) Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.LegBaseCode), 3) <> x And Right(Trim(a.BaseCode), 3) = x Select a.cost / CDbl(p.Premium)).Sum())
                            BasePremium = CDbl((From a In CasList Where Trim(a.TripNumber) = Trim(dd) And Right(Trim(a.LegBaseCode), 3) <> x And Right(Trim(a.BaseCode), 3) = x Select a.cost).Sum()) - ActualCost
                            CasBasePremium += BasePremium
                            cascost -= BasePremium
                        Next
                        baserev.Add(New baseRevenue With {.basecode = x, .CasPandL = CasRevenue - cascost, .FosPandL = FosRevenue - FosCost, .CasCost = cascost, .FosCost = FosCost, .CasRevenue = CasRevenue, .FosRevenue = FosRevenue, .CasBasePremium = CasBasePremium, .FosBasePremium = FosBasePremium, .GrossProfitChange = (CasRevenue - cascost) - (FosRevenue - FosCost) + FosBasePremium})
                    Next
                End If
                Dim CurrentTail, LastTail As String
                Dim Newtail As Boolean = True
                Dim ck As Integer = 0
                '20180827 - pab - fix math - totals wrong - trips removed below may have different costs so don't remove
                'If Panellist.Count > 1 Then
                '    Do While ck <> Panellist.Count - 1
                '        CurrentTail = If(Panellist(ck + 1).FOSRecord Is Nothing, Trim(Panellist(ck + 1).CASRecord.AircraftRegistration), Trim(Panellist(ck + 1).FOSRecord.AC))
                '        LastTail = If(Panellist(ck).FOSRecord Is Nothing, Trim(Panellist(ck).CASRecord.AircraftRegistration), Trim(Panellist(ck).FOSRecord.AC))
                '        If CurrentTail <> LastTail Then Newtail = True
                '        If CurrentTail = LastTail And Newtail Then
                '            If Panellist(ck).FOSRecord IsNot Nothing And Panellist(ck).CASRecord IsNot Nothing Then
                '                If Panellist(ck + 1).FOSRecord IsNot Nothing And Panellist(ck + 1).CASRecord IsNot Nothing Then
                '                    Panellist.Remove(Panellist(ck))
                '                    ck -= 1
                '                End If
                '            Else
                '                Newtail = False
                '            End If
                '        End If
                '        ck += 1
                '    Loop
                'End If

                GridViewSource.Add(New PanelDisplay With {.FCDR_Key = fcdr.keyid, .dcostday0 = fcdr.SavingsDay0, .dcostday1 = fcdr.SavingsDay1, .dcostday2 = fcdr.SavingsDay2, .TailNumber = fcdr.PriorTailNumber, .RevenueRecords = baserev,
                                                        .NRM = CDbl(fcdr.DeltaNonRevMiles), .PanelRecord = Panellist.ToList(), .ModelNumber = fcdr.ModelRunID, .TotalSavings = fcdr.TotalSavings})

                Filename = fcdr.keyid
            Next

            For Each x As PanelDisplay In GridViewSource
                AirTaxi.awclookup.TryGetValue(Trim(x.AircraftType), x.WeightClass)
            Next

            lvflightlist.DataSource = GridViewSource
            lvflightlist.DataBind()

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "GetTrips", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb GetTrips error", s, carrierid)
            End If

        End Try

    End Sub

    Function FOScolorme(ByRef gridviewtrips As GridView, ByVal FCDRColorList As fcdrColorlist) ', ACType As String)

        Dim req As String

        Dim mycolor As System.Drawing.Color
        mycolor = Drawing.Color.White

        Dim colorsArray As System.Array =
        [Enum].GetValues(GetType(KnownColor))
        Dim allColors(colorsArray.Length) As KnownColor

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        '20180817 - pab - do not use session or global variables for this page
        'If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        'Dim carrierid As Integer = Session("carrierid")

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
            Array.Copy(colorsArray, allColors, colorsArray.Length)

            '   _carrierid = Session("carrierid")
            Dim account, rownumber As Integer
            Dim currentTail, LastTail As String
            Dim i As Integer
            rownumber = 0
            For i = 0 To gridviewtrips.Rows.Count - 1
                If gridviewtrips.Rows(i).Cells(FOS_AC).Text <> "" Then
                    gridviewtrips.Rows(i).Cells(FOS_AC).ToolTip = AirTaxi.lookupac(Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text), carrierid)
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
                gridviewtrips.Rows(i).Cells(FOS_COST).Font.Bold = True


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

                If IsDate(gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text) Then
                    ' DepartDate = gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text
                    Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text)
                    gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
                End If
                If IsDate(gridviewtrips.Rows(i).Cells(FOS_TOGMT).Text) Then
                    Dim departgmt As Date = CDate(gridviewtrips.Rows(i).Cells(FOS_TOGMT).Text)
                    gridviewtrips.Rows(i).Cells(FOS_TOGMT).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
                End If

            Next i

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "FOScolorme", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb FOScolorme error", s, carrierid)
            End If

        End Try

    End Function

    Function CAScolorme(ByRef gridviewtrips As GridView, ByVal FCDRColorList As fcdrColorlist)

        caslinebreaks = 0
        Dim mycolor As System.Drawing.Color
        mycolor = Drawing.Color.Aquamarine

        Dim account As Integer = 0
        Dim req As String
        Dim currentTail, LastTail, LastFosTail, LastCasTail As String

        '  _carrierid = Session("carrierid")

        Dim colorsArray As System.Array =
        [Enum].GetValues(GetType(KnownColor))
        Dim allColors(colorsArray.Length) As KnownColor

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
            Array.Copy(colorsArray, allColors, colorsArray.Length)

            Dim d, r As Double

            '20171121 - pab - fix carriers changing midstream - change to Session variables
            '20180817 - pab - do not use session or global variables for this page
            'If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
            'Dim carrierid As Integer = Session("carrierid")


            Dim i As Integer
            For i = 0 To gridviewtrips.Rows.Count - 1
                Dim key, mycolor1 As String
                '20180826 - pab - fix error - Object reference not set to an instance of an object
                'key = Trim(gridviewtrips.Rows(i).Cells(CAS_FROM).Text) & "-" & Trim(gridviewtrips.Rows(i).Cells(CAS_TO).Text) & "-" & Trim(gridviewtrips.Rows(i).Cells(CAS_TRIP).Text)                Dim orig As String = ""
                Dim orig As String = ""
                Dim dest As String = ""
                Dim trip As String = ""
                Dim s As String = "FCDRKey " & FCDRKey
                If Not IsNothing(gridviewtrips.Rows(i).Cells(CAS_FROM)) Then
                    orig = Trim(gridviewtrips.Rows(i).Cells(CAS_FROM).Text)
                Else
                    AirTaxi.Insertsys_log(carrierid, appName, s & " from location missing in row " & i, "CAScolorme", "FlightChangeDetail.aspx.vb")
                    SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                        appName & " FlightChangeDetail.aspx.vb CAScolorme error", s & " from location missing in row " & i, carrierid)
                End If
                If Not IsNothing(gridviewtrips.Rows(i).Cells(CAS_TO)) Then
                    dest = Trim(gridviewtrips.Rows(i).Cells(CAS_TO).Text)
                Else
                    AirTaxi.Insertsys_log(carrierid, appName, s & " to location missing in row " & i, "CAScolorme", "FlightChangeDetail.aspx.vb")
                    SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                        appName & " FlightChangeDetail.aspx.vb CAScolorme error", s & " to location missing in row " & i, carrierid)
                End If
                If Not IsNothing(gridviewtrips.Rows(i).Cells(CAS_TRIP)) Then
                    trip = Trim(gridviewtrips.Rows(i).Cells(CAS_TRIP).Text)
                Else
                    AirTaxi.Insertsys_log(carrierid, appName, s & " trip number missing in row " & i, "CAScolorme", "FlightChangeDetail.aspx.vb")
                    SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                        appName & " FlightChangeDetail.aspx.vb CAScolorme error", s & " trip number location missing in row " & i, carrierid)
                End If
                key = orig & "-" & dest & "-" & trip
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
                gridviewtrips.Rows(i).Cells(CAS_COST).Font.Bold = True

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

                If IsDate(gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text) Then
                    '   DepartDate = gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text
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
                    gridviewtrips.Rows(i).Cells(CAS_AC).ToolTip = AirTaxi.lookupac(gridviewtrips.Rows(i).Cells(CAS_AC).Text, carrierid)
                End If
            Next i
            If carrierprofile.carrierid <> JETLINX Then
                gridviewtrips.Columns(FOS_LEGBASE).Visible = False
                gridviewtrips.Columns(CAS_LEGBASE).Visible = False
                gridviewtrips.Columns(FOS_PROREV).Visible = False
                gridviewtrips.Columns(CAS_PROREV).Visible = False
            ElseIf carrierprofile.carrierid = JETLINX Then
                gridviewtrips.Columns(FOS_REV).Visible = False
                gridviewtrips.Columns(CAS_REV).Visible = False

            End If

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "CAScolorme", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb CAScolorme error", s, carrierid)
            End If

        End Try

    End Function
    Function linebreaks(ByRef gridviewtrips As GridView, ByVal FCDRKey As String) ', ACType As String)

        Dim currentTail, LastTail As String
        Dim maxrows As Integer = 1
        Dim foscount, cascount As Integer
        Dim db As New OptimizerContext
        Dim FCDRdetail As New List(Of FCDRListDetail)

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
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
                If IsDate(gridviewtrips.Rows(i).Cells(FCDR_DEPARTDATE).Text) Then
                    DepartDate = gridviewtrips.Rows(i).Cells(FCDR_DEPARTDATE).Text
                End If
                currentTail = If(Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text) <> "&nbsp;", Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text), Trim(gridviewtrips.Rows(i).Cells(CAS_AC).Text))
                LastTail = If(Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text) <> "&nbsp;", Trim(gridviewtrips.Rows(i - 1).Cells(FOS_AC).Text), Trim(gridviewtrips.Rows(i - 1).Cells(CAS_AC).Text))
                If Trim(gridviewtrips.Rows(i).Cells(FOS_AC).Text) <> Trim(gridviewtrips.Rows(i).Cells(CAS_AC).Text) Then
                    If Trim(gridviewtrips.Rows(i).Cells(FOS_FT).Text) = "Rev" Or Trim(gridviewtrips.Rows(i).Cells(CAS_FT).Text) = "Rev" Then
                        FCDRdetail.Add(New FCDRListDetail With {.KeyID = FCDRKey, .Modification = Trim(gridviewtrips.Rows(i).Cells(FUTURE_TAIL).Text), .FlightID = Trim(gridviewtrips.Rows(i).Cells(RECORD_ID).Text),
                                   .TripNumber = If(gridviewtrips.Rows(i).Cells(FOS_AC).Text <> "&nbsp;", gridviewtrips.Rows(i).Cells(FOS_TRIP).Text, gridviewtrips.Rows(i).Cells(CAS_TRIP).Text),
                                   .AC = If(gridviewtrips.Rows(i).Cells(FOS_AC).Text <> "&nbsp;", gridviewtrips.Rows(i).Cells(FOS_AC).Text, gridviewtrips.Rows(i).Cells(CAS_AC).Text),
                                   .From_ICAO = If(gridviewtrips.Rows(i).Cells(FOS_AC).Text <> "&nbsp;", gridviewtrips.Rows(i).Cells(FOS_FROM).Text, gridviewtrips.Rows(i).Cells(CAS_FROM).Text),
                                   .To_ICAO = If(gridviewtrips.Rows(i).Cells(FOS_AC).Text <> "&nbsp;", gridviewtrips.Rows(i).Cells(FOS_TO).Text, gridviewtrips.Rows(i).Cells(CAS_TO).Text),
                                   .DepartDate = DepartDate})
                        '              .DepartDate = If(gridviewtrips.Rows(i).Cells(FOS_AC).Text <> "&nbsp;", Date.ParseExact(gridviewtrips.Rows(i).Cells(FOS_FROMGMT).Text, "MM/dd HH:mm", Nothing), Date.ParseExact(gridviewtrips.Rows(i).Cells(CAS_FROMGMT).Text, "MM/dd HH:mm", Nothing))})
                    End If
                End If
                If currentTail <> LastTail Then
                    gridviewtrips.Controls(0).Controls.AddAt(i + maxrows, AddRow(foscount, cascount)) ' This line will insert row at 2nd line
                    maxrows += 1
                End If
            Next
            Dim ck As Integer = 0
            If FCDRdetail.Count > 1 Then
                Do While ck <> FCDRdetail.Count
                    If FCDRdetail.Where(Function(a) Trim(a.Modification) = Trim(FCDRdetail(ck).AC) And Trim(a.From_ICAO) = Trim(FCDRdetail(ck).From_ICAO) And Trim(a.To_ICAO) = Trim(FCDRdetail(ck).To_ICAO)).Count > 0 Then
                        'remove from the list
                        FCDRdetail.Remove(FCDRdetail(ck))
                        ck -= 1
                    End If
                    ck += 1
                Loop
            End If
            If db.FCDRListDetail.Where(Function(c) c.KeyID = FCDRKey).Count() = 0 Then

                db.FCDRListDetail.AddRange(FCDRdetail)
                Try
                    db.SaveChanges()
                Catch ex As Exception

                End Try
            End If

            '20180905 - pab - fix error - Specified argument was out of the range of valid values. Parameter name: index
            If Not IsNothing(gridviewtrips) Then
                If gridviewtrips.Rows.Count > 0 Then
                    gridviewtrips.Controls(0).Controls.AddAt(gridviewtrips.Rows.Count + maxrows, AddRow(foscount, cascount)) ' This line will insert row at 2nd line
                    gridviewtrips.Columns(CAS_PT).Visible = False
                    gridviewtrips.Columns(RECORD_ID).Visible = False
                    gridviewtrips.Columns(FCDR_DEPARTDATE).Visible = False
                End If
            End If

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "linebreaks", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb linebreaks error", s, carrierid)
            End If

        End Try

    End Function

    Function AddRow(foscount As Integer, cascount As Integer) As GridViewRow

        Dim row As New GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Alternate)
        Try
            For zzz = 0 To 1
                Dim cell As New TableCell
                cell.Text = "&nbsp;"
                cell.ColumnSpan = If(zzz = 0, foscount, cascount) '14,13
                'cell.Text = Espace
                row.Cells.Add(cell)
            Next
            row.Cells(1).Style.Add("border-left", "8px solid white")
            row.BackColor = Drawing.Color.FromArgb(0, 147, 111)

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "AddRow", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb AddRow error", s, carrierid)
            End If

        End Try

        Return row

    End Function

    Protected Sub GVGridViewTrips_PreRender(sender As Object, e As EventArgs)
        '20180821 - pab - move to page prerender per Richard's suggestion
        '20180825 - pab - this event fires after prre-render
        Dim i As Integer = 0
        Dim x As String

        Try
            For i = 0 To lvflightlist.Items.Count - 1
                x = DirectCast(lvflightlist.Items(i).FindControl("pnlFCDR"), Label).Text
                If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblNonRevDeltab"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblNonRevDeltab"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
                If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostSavings"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostSavings"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
                If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay0b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay0b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
                If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay1b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay1b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
                If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay2b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay2b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
                FOScolorme(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), fcdrcolors.Where(Function(z) z.FCDR_Key = x).Select(Function(z) z).FirstOrDefault()) ', DirectCast(lvflightlist.Items(1).FindControl("pnlACType"), Label).Text)
                CAScolorme(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), fcdrcolors.Where(Function(z) z.FCDR_Key = x).Select(Function(z) z).FirstOrDefault())
                linebreaks(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), x) ', DirectCast(lvflightlist.Items(1).FindControl("pnlACType"), Label).Text)
                'GetSavings(lvflightlist.Items(i))
            Next
            If CreatePDF Then makeMYPDF()

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "GVGridViewTrips_PreRender", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb GVGridViewTrips_PreRender error", s, carrierid)
            End If


        End Try

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

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
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

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "getColorDictionary", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb getColorDictionary error", s, carrierid)
            End If

        End Try

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
        If File.Exists(DoneFile) Then
            Exit Sub
        End If

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

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
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

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "makeMYPDF", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb makeMYPDF error", s, carrierid)
            End If

        End Try

    End Sub
    Public Overrides Sub VerifyRenderingInServerForm(control As Control)
        'MyBase.VerifyRenderingInServerForm(control)
    End Sub

    '20171101 - pab - display cleanup
    Protected Sub LinkLogOut_Click(sender As Object, e As EventArgs) Handles LinkLogOut.Click

        logout()

    End Sub

    '20171101 - pab - display cleanup
    Protected Sub LinkLogOut2_Click(sender As Object, e As EventArgs) Handles LinkLogOut2.Click

        logout()

    End Sub

    '20171101 - pab - display cleanup
    Sub logout()

        If (Request.Browser.Cookies) Then
            If (Request.Cookies("CASLOGIN") Is Nothing) Then
                Response.Cookies("CASLOGIN").Expires = DateTime.Now.AddDays(60)


                Response.Cookies("CASLOGIN").Item("UNAME") = ""
                'Write password to the cookie
                '     Response.Cookies("CASLOGIN").Item("UPASS") = ""

            Else
                Response.Cookies("CASLOGIN").Item("UNAME") = ""
                'Write password to the cookie
                '     Response.Cookies("CASLOGIN").Item("UPASS") = ""
            End If

        End If

        Session("email") = Nothing
        Session("username") = Nothing

        Response.Redirect("CustomerLogin.aspx", True)

    End Sub

    Private Sub FlightChangeDetail_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        '20180817 - pab - do not use session or global variables for this page
        'If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        'If IsNothing(Session("urlalias")) Then Session("urlalias") = ""
        'Dim carrierid As Integer = Session("carrierid")
        'Dim urlalias As String = Session("urlalias")

        Try

            Dim da As New DataAccess

            If Not IsPostBack Then
                '20171101 - pab - display cleanup
                'Me.lblCarrier.Text = _urlalias.ToUpper
                Dim slogotext As String = da.GetSetting(carrierid, "CompanyLogoText")
                If slogotext = "" Then slogotext = urlalias & " Flight Schedule Optimization System"
                Me.lblCarrier.Text = slogotext.ToUpper

                Me.imglogo.Src = GetImageURLByATSSID(carrierid, 0, "logo")

                '20171017 - pab - demoair branding
                If carrierid = DEMOAIR Then
                    imglogo.Width = 56
                    imglogo.Style.Remove("position")
                    imglogo.Style.Add("position", "absolute;top:16px;lefT:50%;margin:0 0 0 -23px;width:56px;z-index:1;")
                End If

                '20171209 - pab - link to quoting portal
                '20180817 - pab - do not use session or global variables for this page
                'If CInt(Session("carrierid")) = XOJET Then
                If carrierid = XOJET Then
                    LinkQuoting.Visible = True
                Else
                    LinkQuoting.Visible = False
                End If

            End If

            '20180821 - pab - move to page prerender per Richard's suggestion
            '20180825 - pab - this event fires after prre-render
            'Dim i As Integer = 0
            'Dim x As String

            'For i = 0 To lvflightlist.Items.Count - 1
            '    x = DirectCast(lvflightlist.Items(i).FindControl("pnlFCDR"), Label).Text
            '    If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblNonRevDeltab"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblNonRevDeltab"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            '    If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostSavings"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostSavings"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            '    If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay0b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay0b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            '    If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay1b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay1b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            '    If CInt(DirectCast(lvflightlist.Items(i).FindControl("lblCostDay2b"), Label).Text) < 0 Then DirectCast(lvflightlist.Items(i).FindControl("lblCostDay2b"), Label).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            '    FOScolorme(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), fcdrcolors.Where(Function(z) z.FCDR_Key = x).Select(Function(z) z).FirstOrDefault()) ', DirectCast(lvflightlist.Items(1).FindControl("pnlACType"), Label).Text)
            '    CAScolorme(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), fcdrcolors.Where(Function(z) z.FCDR_Key = x).Select(Function(z) z).FirstOrDefault())
            '    linebreaks(DirectCast(lvflightlist.Items(i).FindControl("GVGridViewTrips"), GridView), x) ', DirectCast(lvflightlist.Items(1).FindControl("pnlACType"), Label).Text)
            '    'GetSavings(lvflightlist.Items(i))
            'Next
            'If CreatePDF Then makeMYPDF()

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "Page_PreRender", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb Page_PreRender error", s, carrierid)
            End If

        End Try

    End Sub

    '20171209 - pab - link to quoting portal
    Protected Sub LinkQuoting_Click(sender As Object, e As EventArgs) Handles LinkQuoting.Click

        '20180817 - pab - do not use session or global variables for this page
        'If IsNothing(Session("urlalias")) Then Session("urlalias") = ""
        'If Session("urlalias").ToString.Trim <> "" Then
        If urlalias.Trim <> "" Then
            'Response.Redirect("http://" & Session("urlalias").ToString.Trim & ".personiflyadminuat.com/CustomerLogin.aspx", True)
            '20180606 - pab - change url for admin portal
            'Response.Write("<script>window.open ('http://" & Session("urlalias").ToString.Trim & ".personiflyadminuat.com/CustomerLogin.aspx','_blank');</script>")
            'Response.Write("<script>window.open ('http://" & Session("urlalias").ToString.Trim & ".avaisearch.com/CustomerLogin.aspx','_blank');</script>")
            Response.Write("<script>window.open ('http://" & urlalias.Trim & ".avaisearch.com/CustomerLogin.aspx','_blank');</script>")
        End If

    End Sub

    Public Sub RejectFlight(KeyID As String)

        Dim odb As New OptimizerContext
        '20180817 - pab - do not use session or global variables for this page
        'Dim fcdr_Key = Session("FCDRKey")
        Dim fcdr_Key = FCDRKey.ToString

        Dim cr As New CASFlightsOptimizerRecord
        Dim fr As New FOSFlightsOptimizerRecord
        Dim fd = odb.FCDRListDetail.Where(Function(g) g.KeyID = fcdr_Key And g.FlightID = KeyID).FirstOrDefault()

        Dim rf As New RejectedFlight

        '20180822 - pab - add more logging to try to figure out why mountain failing
        Try
            If fd.Modification = "Added" Then
                cr = odb.CASFlightsOptimizer.Find(fd.FlightID)
                rf.FOSKEY = Trim(cr.FOSKEY)
                rf.FromDateGMT = Trim(cr.DepartureTime)
                rf.ToDateGMT = Trim(cr.ArrivalTime)
                rf.Version = cr.Version
                rf.CarrierID = cr.CarrierId
            Else
                fr = odb.FOSFlightsOptimizer.Find(fd.FlightID)
                rf.PriorTail = If(fd.Modification <> "Removed", Trim(fd.AC), "")
                rf.FOSKEY = Trim(fr.FOSKey)
                rf.FromDateGMT = Trim(fr.DateTimeGMT)
                rf.ToDateGMT = Trim(Date.Parse(fr.ArrivalDateGMT).Add(TimeSpan.Parse(fr.ArrivalTimeGMT)))
                rf.Version = fr.Version
                rf.CarrierID = fr.carrierid
            End If
            rf.Action = Trim(fd.Modification)
            rf.DepartureAirport = Trim(fd.From_ICAO)
            rf.ArrivalAirport = Trim(fd.To_ICAO)
            rf.TripNumber = Trim(fd.TripNumber)
            rf.AircraftRegistration = Trim(fd.AC)
            rf.RejectedOn = Now
            rf.Rejected = True
            rf.CASFOid = 0
            rf.PriorTailSavings = 0
            'rf.Status = "/"
            rf.TripType = "R"
            rf.StatusComment = "Rejected in FCDR"
            odb.RejectedFlights.Add(rf)
            Try
                odb.SaveChanges()
            Catch ex As Exception
            End Try

        Catch ex As Exception
            '20180825 - pab - more logging
            Dim s As String = "FCDRKey " & FCDRKey & vbCrLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(carrierid, appName, s, "RejectFlight", "FlightChangeDetail.aspx.vb")
            If InStr(s, "because it is being used by another process") = 0 And InStr(s, "Thread was being aborted") = 0 Then
                SendEmail("chartersales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeDetail.aspx.vb RejectFlight error", s, carrierid)
            End If

        End Try

    End Sub

End Class