﻿Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI

Public Class Error_Page
    Inherits System.Web.UI.Page

    Private dtflights As New DataTable

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Session("carrierid") Is Nothing Then
                Insertsys_log(_carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "Error_Page.aspx.vb")
            Else
                Insertsys_log(_carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - " & Session("carrierid").ToString, "Error_Page.aspx.vb")
            End If

            '20120830 - pab - try to fix telerik issue - ddls not working when round trip clicked
            Response.Cache.SetNoStore()

            ''20160412 - pab - If Session("email") Is Nothing Then assume timeout and redirect to login page
            'If Session("email") Is Nothing Then
            '    'Session.Abandon()
            '    Response.Redirect("CustomerLogin.aspx", True)

            'End If

            '20111121 - pab - convert to single db
            Dim da As New DataAccess
            Dim dt As DataTable

            '20120503 - pab - run time improvements - execute on if not postback
            If Not IsPostBack Then

                '20160517 - pab - fix carrierid = 0 preventing quotes
                If Not IsNothing(Session("email")) Then
                    If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And _carrierid = 0 Then
                        _carrierid = 65
                    End If
                End If

                '20111121 - pab - convert to single db
                If IsNothing(_carrierid) Or _carrierid = 0 Then
                    '20160517 - pab - fix carrierid = 0 preventing quotes
                    AirTaxi.Insertsys_log(0, appName, Request.Url.Host & " carrierid null or 0 - user " & Session("email").ToString, "Page_Load", "Error_Page.aspx.vb")

                    '20160823 - pab - redirect to logon if carrierid lost
                    Response.Redirect("CustomerLogin.aspx", True)
                End If

                '20130930 - pab - change email from
                If IsNothing(_emailfrom) Then _emailfrom = ""
                If _emailfrom = "" Then
                    _emailfrom = da.GetSetting(_carrierid, "emailsentfrom")
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
                AirTaxi.Insertsys_log(_carrierid, appName, s, "Page_Load", "Error_Page.aspx.vb")
                AirTaxi.InsertEmailQueue(_carrierid, "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "Error_Page.aspx.vb Page_Load error", s, False, "", "", "", False)
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
            AirTaxi.Insertsys_log(_carrierid, appName, Left(Now & " " & s, 500), "Error_Page.aspx.vb Page_PreRender", "")
            SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " Error_Page.aspx.vb Page_PreRender error", s, _carrierid)

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