﻿Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI

Public Class pagetemplate
    Inherits System.Web.UI.Page

    Private dtflights As New DataTable

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Session("carrierid") Is Nothing Then
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                Insertsys_log(0, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "ModelRunHistory.aspx.vb")
            Else
                Insertsys_log(CInt(Session("carrierid")), appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - " & Session("carrierid").ToString, "ModelRunHistory.aspx.vb")
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

                '20160517 - pab - fix carrierid = 0 preventing quotes
                If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And CInt(Session("carrierid")) = 0 Then
                    Session("carrierid") = 65
                End If

                '20111121 - pab - convert to single db
                If IsNothing(CInt(Session("carrierid"))) Or CInt(Session("carrierid")) = 0 Then
                    '20160517 - pab - fix carrierid = 0 preventing quotes
                    AirTaxi.Insertsys_log(0, appName, Request.Url.Host & " carrierid null or 0 - user " & Session("email").ToString, "Page_Load", "ModelRunHistory.aspx.vb")

                    '20160823 - pab - redirect to logon if carrierid lost
                    Response.Redirect("CustomerLogin.aspx", True)
                End If

                '20130930 - pab - change email from
                If IsNothing(_emailfrom) Then _emailfrom = ""
                If _emailfrom = "" Then
                    _emailfrom = da.GetSetting(CInt(Session("carrierid")), "emailsentfrom")
                End If

                Dim oLookup As New PopulateLookups
                Me.departtime_combo.Items.Clear()
                dt = oLookup.TimeDD("All")
                Me.departtime_combo.DataSource = dt.DefaultView
                Me.departtime_combo.DataBind()
                Me.departtime_combo.SelectedValue = "09:00 AM"

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

            '20100608 - pab - add logo to email
            Session("ApplicationPath") = Request.PhysicalApplicationPath

            If Not (IsNothing(Session("flights"))) Then
                dtflights = Session("flights")
            Else
                'chg3641 - 20101008 - pab - fix clearing session variables when going back to request another flight
                dtflights.Clear()
            End If

        Catch ex As Exception
            Dim s As String = ex.Message
            If s <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "Page_Load", "ModelRunHistory.aspx.vb")
                AirTaxi.InsertEmailQueue(CInt(Session("carrierid")), "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "ModelRunHistory.aspx.vb Page_Load error", s, False, "", "", "", False)
            End If

        End Try

    End Sub

    Private Sub RunOptimizer_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Try

            Dim da As New DataAccess

            If Not IsPostBack Then
                '20171101 - pab - display cleanup
                'Me.lblCarrier.Text = _urlalias.ToUpper
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                Dim slogotext As String = da.GetSetting(CInt(Session("carrierid")), "CompanyLogoText")
                '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
                If slogotext = "" Then slogotext = Session("urlalias").ToString & " Flight Schedule Optimization System"
                Me.lblCarrier.Text = slogotext.ToUpper

                Me.imglogo.Src = GetImageURLByATSSID(CInt(Session("carrierid")), 0, "logo")

                '20171017 - pab - demoair branding
                If CInt(Session("carrierid")) = 48 Then
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
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, Left(Now & " " & s, 500), "ModelRunHistory.aspx.vb Page_PreRender", "")
            SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " ModelRunHistory.aspx.vb Page_PreRender error", s, CInt(Session("carrierid")))

        End Try

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