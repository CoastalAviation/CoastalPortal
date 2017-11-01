Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI
Imports CoastalPortal.Models
Public Class FlightChangeReports
    Inherits System.Web.UI.Page

    Private dtflights As New DataTable
    Public Const F_KEY = 0
    Public Const F_RUN = 1
    Public Const F_MDL = 2
    Public Const F_NRM = 3
    Public Const F_TOT = 4
    Public Const F_SV0 = 5
    Public Const F_SV1 = 6
    Public Const F_SV2 = 7
    Public Const F_ACC = 8

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Session("carrierid") Is Nothing Then
                Insertsys_log(_carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "FlightChangeReports.aspx.vb")
            Else
                Insertsys_log(_carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
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

            '20120503 - pab - run time improvements - execute on if not postback
            If Not IsPostBack Then

                Me.gvFCDRList.Visible = True

                '20160517 - pab - fix carrierid = 0 preventing quotes
                If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And _carrierid = 0 Then
                    _carrierid = 65
                End If

                '20111121 - pab - convert to single db
                If IsNothing(_carrierid) Or _carrierid = 0 Then
                    '20160517 - pab - fix carrierid = 0 preventing quotes
                    AirTaxi.Insertsys_log(0, appName, Request.Url.Host & " carrierid null or 0 - user " & Session("email").ToString, "Page_Load", "FlightChangeReports.aspx.vb")

                    '20160823 - pab - redirect to logon if carrierid lost
                    Response.Redirect("CustomerLogin.aspx", True)
                End If

                '20130930 - pab - change email from
                If IsNothing(_emailfrom) Then _emailfrom = ""
                If _emailfrom = "" Then
                    _emailfrom = da.GetSetting(_carrierid, "emailsentfrom")
                End If

                'Dim oLookup As New PopulateLookups
                'Me.departtime_combo.Items.Clear()
                'dt = oLookup.TimeDD("All")
                'Me.departtime_combo.DataSource = dt.DefaultView
                'Me.departtime_combo.DataBind()
                'Me.departtime_combo.SelectedValue = "09:00 AM"

                GetTrips()
            Else
                '20131016 - pab - fix session timeout

                'If IsNothing(Session("flights")) And IsNothing(Session("triptype")) Then
                '    lblMsg.Text = da.GetSetting(_carrierid, "TimeoutMessage")
                '    gvServiceProviderMatrix.EmptyDataText = lblMsg.Text
                '    dtflights.Clear()
                '    Me.gvServiceProviderMatrix.DataSource = dtflights
                '    Me.gvServiceProviderMatrix.DataBind()
                'End If
            End If

            '20100608 - pab - add logo to email
            Session("ApplicationPath") = Request.PhysicalApplicationPath


        Catch ex As Exception
            Dim s As String = ex.Message
            If s <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.Insertsys_log(_carrierid, appName, s, "Page_Load", "FlightChangeReports.aspx.vb")
                AirTaxi.InsertEmailQueue(_carrierid, "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "FlightChangeReports.aspx.vb Page_Load error", s, False, "", "", "", False)
            End If

        End Try

    End Sub

    Private Sub RunOptimizer_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Try

            Dim da As New DataAccess

            If Not IsPostBack Then
                '20171101 - pab - display cleanup
                'Me.lblCarrier.Text = _urlalias.ToUpper
                Dim slogotext As String = da.GetSetting(_carrierid, "CompanyLogoText")
                If slogotext = "" Then slogotext = _urlalias & " Flight Schedule Optimization System"
                Me.lblCarrier.Text = slogotext.ToUpper

                Me.imglogo.Src = GetImageURLByATSSID(_carrierid, 0, "logo")

                '20171017 - pab - demoair branding
                If _carrierid = 48 Then
                    imglogo.Width = 56
                    imglogo.Style.Remove("position")
                    imglogo.Style.Add("position", "absolute;top:16px;lefT:50%;margin:0 0 0 -23px;width:56px;z-index:1;")
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
            AirTaxi.Insertsys_log(_carrierid, appName, Left(Now & " " & s, 500), "FlightChangeReports.aspx.vb Page_PreRender", "")
            SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " FlightChangeReports.aspx.vb Page_PreRender error", s, _carrierid)

        End Try

    End Sub
    Public Sub GetTrips()
        Dim odb As New OptimizerContext
        Dim fcdrlist As New List(Of FCDRList)
        Dim today = DateAdd("d", -2, DateTime.Now)
        Dim i As Integer = 1

        fcdrlist = odb.FCDRList.Where(Function(c) c.CarrierID = _carrierid And c.TotalSavings > 999 And c.GMTStart >= today).OrderByDescending(Function(c) c.ModelRun).ThenByDescending(Function(c) c.TotalSavings).ToList()
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
        gvFCDRList.DataSource = fcdrlist
        gvFCDRList.DataBind()

        Colorme()
    End Sub
    Public Sub Colorme()
        Dim i As Integer = 0

        For i = 0 To gvFCDRList.Rows.Count - 1
            If gvFCDRList.Rows(i).Cells(F_NRM).Text < 0 Then gvFCDRList.Rows(i).Cells(F_NRM).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If gvFCDRList.Rows(i).Cells(F_SV0).Text < 0 Then gvFCDRList.Rows(i).Cells(F_SV0).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If gvFCDRList.Rows(i).Cells(F_SV1).Text < 0 Then gvFCDRList.Rows(i).Cells(F_SV1).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If gvFCDRList.Rows(i).Cells(F_SV2).Text < 0 Then gvFCDRList.Rows(i).Cells(F_SV2).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            If gvFCDRList.Rows(i).Cells(F_TOT).Text < 0 Then gvFCDRList.Rows(i).Cells(F_TOT).ForeColor = Drawing.Color.FromArgb(205, 0, 0)
            For ii = 1 To gvFCDRList.Columns.Count - 1
                gvFCDRList.Rows(i).Cells(ii).Text = Trim(gvFCDRList.Rows(i).Cells(ii).Text)
            Next
        Next

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

End Class