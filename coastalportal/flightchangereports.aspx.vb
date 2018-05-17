Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI
Imports CoastalPortal.Models
Public Class FlightChangeReports
    Inherits System.Web.UI.Page

    Private dtflights As New DataTable
    Public Const F_KEY = 1
    Public Const F_RUN = 2
    Public Const F_MDL = 3
    Public Const F_NRM = 4
    Public Const F_TOT = 5
    Public Const F_SV0 = 6
    Public Const F_SV1 = 7
    Public Const F_SV2 = 8
    Public Const F_PT = 9
    Public Const F_ACC = 10
    Public Const F_KEY2 = 12
    Public Const F_TRADE = 13
    Public Const FD_TRIP = 0
    Public Const FD_FROM = 1
    Public Const FD_TO = 2
    Public Const FD_DEPART = 3
    Public Const FD_OAC = 4
    Public Const FD_RESULT = 5
    Public Const FD_NAC = 6


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Try

            If Session("carrierid") Is Nothing Then
                Insertsys_log(carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "FlightChangeReports.aspx.vb")
            Else
                Insertsys_log(carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - " & Session("carrierid").ToString, "FlightChangeReports.aspx.vb")
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
            Dim btnresult As String = Request.Form("btnacpt")
            Dim btnSelect As String = Request.Form("btnselect")
            Dim DynamicCost As String = Request.QueryString("DynamicCost")
            '20120503 - pab - run time improvements - execute on if not postback
            If Not IsPostBack Then

                gvFCDRList.Visible = True


                '20160517 - pab - fix carrierid = 0 preventing quotes
                If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And carrierid = 0 Then
                    carrierid = 65
                End If

                '20111121 - pab - convert to single db
                If IsNothing(carrierid) Or carrierid = 0 Then
                    '20160517 - pab - fix carrierid = 0 preventing quotes
                    AirTaxi.Insertsys_log(0, appName, Request.Url.Host & " carrierid null or 0 - user " & Session("email").ToString, "Page_Load", "FlightChangeReports.aspx.vb")

                    '20160823 - pab - redirect to logon if carrierid lost
                    Response.Redirect("CustomerLogin.aspx", True)
                End If

                '20130930 - pab - change email from
                '20171121 - pab - fix carriers changing midstream - change to Session variables
                If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""
                If Session("emailfrom").ToString = "" Then
                    Session("emailfrom") = da.GetSetting(CInt(Session("carrierid")), "emailsentfrom")
                End If

                '20180410 - pab - add headings - Trip number, Weight class requested, origin, dest and departure date/time
                divHeading.Visible = False

                '20180329 - pab - hide detail, accept/reject and paging on dc fcdrs per David - 10 fcdrs should be sufficient
                'If DynamicCost IsNot Nothing Then btnSelect = "DynamicCosting-" & DynamicCost
                If DynamicCost IsNot Nothing Then
                    btnSelect = "DynamicCosting-" & DynamicCost
                    gvFCDRList.Columns(0).Visible = False
                    gvFCDRList.Columns(11).Visible = False
                    gvFCDRDetail.PagerSettings.Visible = False

                    '20180410 - pab - add headings - Trip number, Weight class requested, origin, dest and departure date/time
                    divHeading.Visible = True
                    Dim dtq As DataTable = da.GetQuoteFlightsByQuoteNumber(carrierid, DynamicCost)
                    If Not isdtnullorempty(dtq) Then
                        Me.txtTripnumber.Text = dtq.Rows(0).Item("TripNumber").ToString.Trim
                        Me.txtWeightclass.Text = dtq.Rows(0).Item("Weightclass").ToString.Trim
                        Me.txtorigin.Text = dtq.Rows(0).Item("DepartureAirport").ToString.Trim
                        Me.txtdest.Text = dtq.Rows(0).Item("ArrivalAirport").ToString.Trim
                        Me.txtdeparture.Text = dtq.Rows(0).Item("DepartureDate").ToString.Trim & " " & dtq.Rows(0).Item("DepartureTime").ToString.Trim
                    End If
                End If
            Else
                If btnSelect IsNot Nothing Then
                    getDetail(btnSelect)
                End If
                If btnresult IsNot Nothing Then
                    Dim i = InStr(btnresult, " ")
                    Dim action = Left(btnresult, i - 1)
                    Dim KeyId = Mid(btnresult, i + 1)
                    AcceptRejectFCDR(action, KeyId)
                End If
            End If
            If btnSelect Is Nothing Then btnSelect = ""
            GetTrips(btnSelect)


            '20100608 - pab - add logo to email
            Session("ApplicationPath") = Request.PhysicalApplicationPath


        Catch ex As Exception
            Dim s As String = ex.Message
            If s <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.Insertsys_log(carrierid, appName, s, "Page_Load", "FlightChangeReports.aspx.vb")
                AirTaxi.InsertEmailQueue(carrierid, "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "FlightChangeReports.aspx.vb Page_Load error", s, False, "", "", "", False)
            End If

        End Try

    End Sub

    Private Sub RunOptimizer_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
        If IsNothing(Session("urlalias")) Then Session("urlalias") = ""
        Dim urlalias As String = Session("urlalias").ToString

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        Try

            Dim da As New DataAccess

            '20180329 - pab - hide detail, accept/reject and paging on dc fcdrs
            Dim DynamicCost As String = Request.QueryString("DynamicCost")
            If DynamicCost IsNot Nothing Then
                gvFCDRDetail.PagerSettings.Visible = False
            End If

            If Not IsPostBack Then
                '20171101 - pab - display cleanup
                'Me.lblCarrier.Text = _urlalias.ToUpper
                Dim slogotext As String = da.GetSetting(carrierid, "CompanyLogoText")
                If slogotext = "" Then slogotext = urlalias & " Flight Schedule Optimization System"
                Me.lblCarrier.Text = slogotext.ToUpper

                Me.imglogo.Src = GetImageURLByATSSID(carrierid, 0, "logo")

                '20171017 - pab - demoair branding
                If carrierid = 48 Then
                    imglogo.Width = 56
                    imglogo.Style.Remove("position")
                    imglogo.Style.Add("position", "absolute;top:16px;lefT:50%;margin:0 0 0 -23px;width:56px;z-index:1;")
                End If

                '20171209 - pab - link to quoting portal
                If CInt(Session("carrierid")) = XOJET Then
                    LinkQuoting.Visible = True
                Else
                    LinkQuoting.Visible = False
                End If

            End If

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then
                s &= "" & ex.InnerException.ToString
            End If
            If Not IsNothing(ex.StackTrace) Then
                s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            End If
            AirTaxi.Insertsys_log(carrierid, appName, Left(Now & " " & s, 500), "FlightChangeReports.aspx.vb Page_PreRender", "")
            SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeReports.aspx.vb Page_PreRender error", s, carrierid)

        End Try

    End Sub
    Public Sub GetTrips(Optional ByRef getKey As String = "")
        Dim odb As New OptimizerContext
        Dim fcdrlist As New List(Of FCDRList)
        Dim today = DateAdd("d", -2, DateTime.Now)
        Dim i As Integer = 1
        Dim ModelRun As Integer

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        If getKey.Contains("DynamicCosting") Then
            ModelRun = Mid(getKey, InStr(getKey, "-") + 1)
            '20180427 - pab - do not show if total saving > 0 per david
            'fcdrlist = odb.FCDRList.Where(Function(c) c.ModelRun = ModelRun And c.CarrierAcceptStatus = "NA").OrderByDescending(Function(c) c.TotalSavings).ToList()
            fcdrlist = odb.FCDRList.Where(Function(c) c.ModelRun = ModelRun And c.CarrierAcceptStatus = "NA" And c.TotalSavings <= 0).OrderByDescending(Function(c) c.TotalSavings).ToList()
            '20180329 - pab - hide detail, accept/reject and paging on dc fcdrs
            'getKey = Nothing
        Else
            fcdrlist = odb.FCDRList.Where(Function(c) c.CarrierID = carrierid And c.TotalSavings > 999 And c.GMTStart >= today And c.CarrierAcceptStatus = "NA" And c.DynamicCost = False).OrderByDescending(Function(c) c.ModelRun).ThenByDescending(Function(c) c.TotalSavings).ToList()
        End If
        If fcdrlist.Count > 1 Then
            Do While i <> fcdrlist.Count
                Dim checkme = fcdrlist(i - 1)
                If (fcdrlist(i).PriorTailNumber = checkme.PriorTailNumber And fcdrlist(i).ModelRun = checkme.ModelRun And fcdrlist(i).TotalSavings = checkme.TotalSavings) Or
                (fcdrlist(i).ModelRun = checkme.ModelRun And fcdrlist(i).DeltaNonRevMiles = checkme.DeltaNonRevMiles And fcdrlist(i).TotalSavings = checkme.TotalSavings) Then
                    fcdrlist.Remove(fcdrlist(i))
                    i -= 1
                End If
                i += 1
            Loop

            '20180329 - pab - hide detail, accept/reject and paging on dc fcdrs
            If getKey.Contains("DynamicCosting") Then
                Do While fcdrlist.Count > 10
                    fcdrlist.Remove(fcdrlist(fcdrlist.Count - 1))
                Loop
                getKey = Nothing
            End If
        End If

        '20180330 - pab - filter out placement fcdrs
        Dim da As New DataAccess
        Dim dt As DataTable
        'Dim prevmodel As Integer = 0
        'Dim prevdesc As String = ""
        If fcdrlist.Count > 1 Then
            i = 1
            Do While i <> fcdrlist.Count
                dt = da.GetFOSOptimizerRequestByID(fcdrlist(i).CarrierID, fcdrlist(i).ModelRun)
                If Not isdtnullorempty(dt) Then
                    If InStr(dt.Rows(0).Item("Description").ToString.ToLower, "placement request") > 0 Then
                        fcdrlist.Remove(fcdrlist(i))
                        i -= 1
                    End If
                End If
                i += 1
            Loop
        End If

        gvFCDRList.DataSource = fcdrlist
        gvFCDRList.DataBind()
        Colorme(getKey)
    End Sub
    Public Sub Colorme(ByRef GetKey As String)
        Dim i As Integer = 0

        For i = 0 To gvFCDRList.Rows.Count - 1
            gvFCDRList.Rows(i).Cells(F_KEY).ForeColor = Drawing.Color.Blue
            If gvFCDRList.Rows(i).Cells(F_KEY2).Text = GetKey And GetKey <> "" Then
                gvFCDRList.Rows(i).BackColor = Drawing.Color.Aqua
            Else
                gvFCDRList.Rows(i).Cells(F_KEY).BackColor = Drawing.Color.Wheat
            End If
            If gvFCDRList.Rows(i).Cells(F_TRADE).Text = "True" Then gvFCDRList.Rows(i).BackColor = Drawing.Color.Green
            If gvFCDRList.Rows(i).Cells(F_NRM).Text < 0 Then gvFCDRList.Rows(i).Cells(F_NRM).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If gvFCDRList.Rows(i).Cells(F_SV0).Text < 0 Then gvFCDRList.Rows(i).Cells(F_SV0).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If gvFCDRList.Rows(i).Cells(F_SV1).Text < 0 Then gvFCDRList.Rows(i).Cells(F_SV1).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If gvFCDRList.Rows(i).Cells(F_SV2).Text < 0 Then gvFCDRList.Rows(i).Cells(F_SV2).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If gvFCDRList.Rows(i).Cells(F_TOT).Text < 0 Then gvFCDRList.Rows(i).Cells(F_TOT).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            'If gvFCDRList.Rows(i).Cells(F_ACC + 1).Text <> "NA" Then gvFCDRList.Columns(F_ACC).Visible = False
            'If gvFCDRList.Rows(i).Cells(F_ACC).Text = "NA" Then gvFCDRList.Columns(F_ACC).Visible = False

            For ii = 2 To gvFCDRList.Columns.Count - 1
                If ii <> F_ACC + 1 Then
                    gvFCDRList.Rows(i).Cells(ii).Text = Trim(gvFCDRList.Rows(i).Cells(ii).Text)
                End If
            Next
        Next
        gvFCDRList.Columns(F_ACC).Visible = False
        gvFCDRList.Columns(F_KEY2).Visible = False
        gvFCDRList.Columns(F_MDL).Visible = False
        gvFCDRList.Columns(F_TRADE).Visible = False
    End Sub
    Protected Sub Address_ItemsRequested(ByVal o As Object, ByVal e As RadComboBoxItemsRequestedEventArgs)

        Try

            Exit Sub

        Catch ex As Exception

        End Try

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
    Public Sub AcceptRejectFCDR(action As String, KeyID As String)
        Dim odb As New OptimizerContext
        Dim fcdr As New FCDRList
        'Dim fcdrPartner As 

        fcdr = odb.FCDRList.Find(KeyID)
        If fcdr.isTrade Then
            'TODO .. Process a trade 
        Else

            fcdr.CarrierAcceptDate = Now
            fcdr.CarrierAcceptID = Session("username")
            fcdr.CarrierAcceptStatus = If(action = "accept", "AC", "RJ")
        End If


        If action = "accept" Then
            'Send Email to those that need it
        ElseIf action = "reject" Then
            Dim fcdrDtl As New List(Of FCDRListDetail)
            Dim cr As New CASFlightsOptimizerRecord
            Dim fr As New FOSFlightsOptimizerRecord
            fcdrDtl = odb.FCDRListDetail.Where(Function(e) e.KeyID = fcdr.keyid).ToList()
            For Each fd As FCDRListDetail In fcdrDtl
                Dim rf As New RejectedFlight
                If fd.Modification = "Added" Then
                    cr = odb.CASFlightsOptimizer.Find(fd.FlightID)
                    rf.FOSKEY = Trim(cr.FOSKEY)
                    rf.FromDateGMT = Trim(cr.DepartureTime)
                    rf.ToDateGMT = Trim(cr.ArrivalTime)
                    rf.Version = cr.Version
                Else
                    fr = odb.FOSFlightsOptimizer.Find(fd.FlightID)
                    rf.PriorTail = If(fd.Modification <> "Removed", Trim(fd.AC), "")
                    rf.FOSKEY = Trim(fr.FOSKey)
                    rf.FromDateGMT = Trim(fr.DateTimeGMT)
                    rf.ToDateGMT = Trim(Date.Parse(fr.ArrivalDateGMT).Add(TimeSpan.Parse(fr.ArrivalTimeGMT)))
                    rf.Version = fr.Version
                End If
                rf.CarrierID = Trim(fcdr.CarrierID)
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
            Next
        End If
        Try
            odb.SaveChanges()
        Catch ex As Exception
        End Try

    End Sub
    Public Sub getDetail(getKey As String)
        Dim odb As New OptimizerContext
        Dim detailitems As New List(Of FCDRListDetail)
        Session("GetKey") = getKey

        For Each row As GridViewRow In gvFCDRList.Rows

            If row.Cells(F_KEY).Text = getKey Then
                row.BackColor = Drawing.Color.Azure
            End If
        Next
        detailitems = odb.FCDRListDetail.Where(Function(c) Trim(c.KeyID) = Trim(getKey)).ToList()
        gvFCDRDetail.DataSource = detailitems
        gvFCDRDetail.DataBind()
        'If detailitems.Count > 0 Then gvFCDRDetail.Visible = True
    End Sub

    Protected Sub gvFCDRDetail_DataBound(sender As Object, e As EventArgs)
        Dim result_txt As String
        Dim OldAC As String

        For i = 0 To gvFCDRDetail.Rows.Count - 1
            result_txt = gvFCDRDetail.Rows(i).Cells(FD_RESULT).Text
            OldAC = gvFCDRDetail.Rows(i).Cells(FD_OAC).Text
            ' gvFCDRDetail.Rows(i).Cells(FD_NAC).Text = If(result_txt <> "Added" And result_txt <> "Removed", result_txt, "")
            gvFCDRDetail.Rows(i).Cells(FD_RESULT).Text = If(result_txt <> "Added" And result_txt <> "Removed", result_txt, "")
        Next


    End Sub

    Protected Sub gvFCDRList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvFCDRList.PageIndex = e.NewPageIndex
        GetTrips()
    End Sub
    Protected Sub gvFCDRDetail_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvFCDRDetail.PageIndex = e.NewPageIndex
        getDetail(Session("GetKey"))
    End Sub


    Protected Sub gvFCDRList_PreRender(sender As Object, e As EventArgs)
        Dim p = gvFCDRList.SelectedIndex
    End Sub

    '20171209 - pab - link to quoting portal
    Protected Sub LinkQuoting_Click(sender As Object, e As EventArgs) Handles LinkQuoting.Click

        If IsNothing(Session("urlalias")) Then Session("urlalias") = ""
        If Session("urlalias").ToString.Trim <> "" Then
            'Response.Redirect("http://" & Session("urlalias").ToString.Trim & ".personiflyadminuat.com/CustomerLogin.aspx", True)
            Response.Write("<script>window.open ('http://" & Session("urlalias").ToString.Trim & ".personiflyadminuat.com/CustomerLogin.aspx','_blank');</script>")
        End If

    End Sub

End Class