Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI

Public Class ModelRunHistory
    Inherits System.Web.UI.Page

    Private SqlDataSourceOptimizerRequests As New SqlDataSource

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Session("carrierid") Is Nothing Then
                Insertsys_log(_carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "ModelRunHistory.aspx.vb")
            Else
                Insertsys_log(_carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
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

            Session("carrierid") = Session("carrierid")

            If Session("carrierid") = 0 Then
                Me.lblMsg.Text = "   Credentials Lost, Please log off, log on"
                Exit Sub
            End If




            SqlDataSourceOptimizerRequests.ConnectionString = ConnectionStringHelper.GetsqladapterSQLVMConnectionString
            GridView1.DataSource = SqlDataSourceOptimizerRequests


            updategrid()



            If Not Page.IsPostBack Then

                'RadDateTimeFrom.SelectedDate = DateAdd(DateInterval.Hour, -6, Now.ToUniversalTime)

                'RaddatetimeTo.SelectedDate = DateAdd(DateInterval.Hour, 72, Now.ToUniversalTime)


                'RadSliderMBF.Value = 60
                'RadSliderAvgSpeed.Value = 600
                'RadSliderDepDelay.Value = 15


                'RadSliderUpg.Value = 72

                'RadSliderAutoPin.Value = 90
                'RadSliderTaxiTime1.Value = 15
                'RadSliderFastTurn.Value = 30


            End If

        Catch ex As Exception
            Dim s As String = ex.Message
            If s <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.Insertsys_log(_carrierid, appName, s, "Page_Load", "ModelRunHistory.aspx.vb")
                AirTaxi.InsertEmailQueue(_carrierid, "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "ModelRunHistory.aspx.vb Page_Load error", s, False, "", "", "", False)
            End If

        End Try

    End Sub

    Function updategrid()

        Dim req As String
        req = "SELECT top 20 [ID],  status,   [CarrierID]    ,[Description]   ,[GMTStart]     ,[GMTEnd]   FROM [dbo].[OptimizerRequest] where description not like 'Optimizer request %'  and status = 'X' and carrierid = 49 and requestdate > getdate() - 7 order by id desc"

        req = Replace(req, "carrierid = 49", "carrierid = " & Session("carrierid"))

        SqlDataSourceOptimizerRequests.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString

        SqlDataSourceOptimizerRequests.SelectCommand = req

        SqlDataSourceOptimizerRequests.DataBind()

        GridView1.DataBind()

    End Function

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
            AirTaxi.Insertsys_log(_carrierid, appName, Left(Now & " " & s, 500), "ModelRunHistory.aspx.vb Page_PreRender", "")
            SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " ModelRunHistory.aspx.vb Page_PreRender error", s, _carrierid)

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