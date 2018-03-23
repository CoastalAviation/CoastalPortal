Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI

Public Class ModelRunHistory
    Inherits System.Web.UI.Page

    Private SqlDataSourceOptimizerRequests As New SqlDataSource
    Public Shared cnopt As New ADODB.Connection
    Private modelstart As Date
    Private modelend As Date

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

            Session("carrierid") = Session("carrierid")

            If Session("carrierid") = 0 Then
                Me.lblMsg.Text = "   Credentials Lost, Please log off, log on"
                Exit Sub
            End If




            SqlDataSourceOptimizerRequests.ConnectionString = ConnectionStringHelper.GetsqladapterSQLVMConnectionString
            'GridView1.DataSource = SqlDataSourceOptimizerRequests

            GridView2.DataSource = SqlDataSourceOptimizerRequests

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
                AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "Page_Load", "ModelRunHistory.aspx.vb")
                AirTaxi.InsertEmailQueue(CInt(Session("carrierid")), "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "ModelRunHistory.aspx.vb Page_Load error", s, False, "", "", "", False)
            End If

        End Try

    End Sub

    Function updategrid()

        Dim req As String = "SELECT top 20 ID, status, CarrierID, Description, GMTStart, GMTEnd,"
        req &= " case when declaredcomplete = 1 then 'True' else 'False' end as declaredcomplete FROM OptimizerRequest"
        req &= " where description not like 'Optimizer request %' and status = 'X' and carrierid = " & Session("carrierid").ToString
        '20171215 - pab - don't show dynamic costing models
        req &= " and description not like '%Dynamic Costing Request%'"
        '20180323 - pab - don't show Placement models
        req &= " and description not like '%Placement%'"
        req &= " and requestdate > getdate() - 2 order by id desc"

        SqlDataSourceOptimizerRequests.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString

        SqlDataSourceOptimizerRequests.SelectCommand = req

        SqlDataSourceOptimizerRequests.DataBind()

        'GridView1.DataBind()

        GridView2.DataBind()

    End Function

    Private Sub RunOptimizer_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

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
                If CInt(Session("carrierid")) = DEMOAIR Then
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
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, Left(Now & " " & s, 500), "ModelRunHistory.aspx.vb Page_PreRender", "")
            SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "",
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

    'Private Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand

    '    overridemodel = ""



    '    'Exit Sub




    '    Try

    '        Dim s As String
    '        Dim si As Integer
    '        If IsNumeric(e.CommandArgument.ToString) Then si = CInt(e.CommandArgument.ToString)


    '        If GridView1.Rows.Count > si Then

    '            Dim customid As String = Trim(GridView1.Rows.Item(si).Cells(0).Text)

    '            Session("customid") = customid

    '            'rk allow up to 7 more line breaks
    '            '12.2.2013

    '            'remove this additional check  5.9.15
    '            ' and FOSrevenuelegs = CASrevenuelegs

    '            'added CASrevenuelegs <> 0 5.11.15
    '            'remove and caslinebreaks <= (foslinebreaks + 7)  ' rk 11.12.15
    '            '20140512 - pab - add model start and end for calendar
    '            Dim req As String = "SELECT top 1  right(rtrim(modelrunid), 1) as mt" &
    '                "  ,[ModelRunID]  ,[CAStotalrevenueexpense]  ,[CASefficiency]  ,ModelStart , [GMTStart], [GMTEnd] FROM [OptimizerLog] l " &
    '                "left join [OptimizerRequest] r on left(l.[ModelRunID], 5) = r.id where  casrevenuemiles <> 0 and  left(modelrunid, 2) = '28'  and l.carrierid = 49  and " &
    '                "right(rtrim(modelrunid), 1) = 'C'  and ModelRunID not like '%R0%' and CASrevenuelegs <> 0 and description not like 'Optimizer request %'      and CASrevenueexpense <> 0   order by CAStotalrevenueexpense asc"
    '            req = Replace(req, "28", customid)
    '            req = Replace(req, "carrierid = 49", "carrierid = " & Session("carrierid"))
    '            req = Replace(req, "left(l.[ModelRunID], 5", "left(modelrunid, " & Len(customid))
    '            req = Replace(req, "left(modelrunid, 2", "left(modelrunid, " & Len(customid))


    '            'and ModelRunID not like '%R0%'

    '            Dim cidstring As String = ""
    '            If Not IsNothing(Session("carrierid")) Then
    '                If IsNumeric(Session("carrierid")) Then
    '                    cidstring = Session("carrierid")
    '                End If
    '            End If
    '            req = Replace(req, "abc", cidstring)


    '            If e.CommandName.ToString = "Best" Then
    '                '   req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "and right(rtrim(modelrunid), 1) <> 'C'")
    '                req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "")
    '            End If

    '            'If e.CommandName.ToString = "Eff" Then
    '            '    ' req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "and right(rtrim(modelrunid), 1) <> 'C'")
    '            '    req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "")
    '            '    req = Replace(req, "order by CAStotalrevenueexpense asc", "order by CASefficiency desc")
    '            'End If


    '            'If e.CommandName.ToString = "Base" Then
    '            '    req = Replace(req, "and ModelRunID not like '%R0%'", "")

    '            'End If



    '            Dim c As Integer
    '            c = 6
    '            'If e.CommandName.ToString = "Eff" Then c = 8
    '            'If e.CommandName.ToString = "Best" Then c = 7

    '            'If e.CommandName.ToString = "Base" Or e.CommandName.ToString = "Eff" Or e.CommandName.ToString = "Best" Then
    '            If e.CommandName.ToString = "Best" Then


    '                Dim rs As New ADODB.Recordset

    '                'If cnsetting.State = 0 Then
    '                '    cnsetting.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString
    '                '    cnsetting.Open()
    '                'End If
    '                If cnopt.State = 0 Then
    '                    cnopt.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
    '                    cnopt.Open()
    '                End If

    '                If rs.State = 1 Then rs.Close()

    '                'rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
    '                rs.Open(req, cnopt, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

    '                If Not rs.EOF Then

    '                    modelrunid = Trim(rs.Fields("ModelRunID").Value)
    '                    '20140512 - pab - add model start for calendar
    '                    modelstart = CDate(rs.Fields("GMTStart").Value.ToString.Trim).ToShortDateString
    '                    modelend = CDate(rs.Fields("GMTEnd").Value.ToString).ToShortDateString
    '                    ' Me.txtUsersTotal.Text = rs.Fields("Count").Value
    '                    If rs.State = 1 Then rs.Close()

    '                    '20140512 - pab - add model start for calendar
    '                    Response.Redirect("/ModelDetails.aspx?modelrunid=" & modelrunid & "&modelstart=" & modelstart & "&modelend=" &
    '                                      modelend, True)

    '                    '     Call LoadCountries (RadComboBox1.Text) Response.Redirect("http://optimizerpanelwest.cloudapp.net/Panel.aspx?modelrunid=" & modelrunid)
    '                Else

    '                    GridView1.Rows.Item(si).Cells(c).BackColor = Drawing.Color.MediumVioletRed

    '                End If


    '                If rs.State = 1 Then rs.Close()

    '            End If

    '        End If
    '    Catch ex As Exception

    '        If ex.Message <> "Thread was being aborted." Then
    '            Me.lblMsg.Text = "  Please refresh page and try again"
    '        End If
    '    End Try

    'End Sub

    '20171107 - pab - show r0
    Protected Sub bttnR0_Click(sender As Object, e As EventArgs) Handles bttnR0.Click

        'Response.Redirect("FlightSchedule.aspx?r0=1", True)

        '20171110 - pab - open r0 in New tab
        Response.Write("<script>window.open ('FlightSchedule.aspx?r0=1','_blank');</script>")

        ''code below for pop-up window
        'Dim url As String = "FlightSchedule.aspx?r0=1"
        'Dim s As String = "window.open('" & url + "', 'popup_window', 'width=1200,height=800,left=0,top=0,resizable=yes');"
        'ClientScript.RegisterStartupScript(Me.GetType(), "script", s, True)

    End Sub

    Private Sub GridView2_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView2.RowCommand

        overridemodel = ""


        Exit Sub

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("fosmodelrunid")) Then Session("fosmodelrunid") = ""
        Dim modelrunid As String = Session("fosmodelrunid").ToString

        Try

            Dim s As String
            Dim si As Integer
            If IsNumeric(e.CommandArgument.ToString) Then si = CInt(e.CommandArgument.ToString)


            If GridView2.Rows.Count > si Then

                Dim customid As String = Trim(GridView2.Rows.Item(si).Cells(0).Text)

                Session("customid") = customid

                'rk allow up to 7 more line breaks
                '12.2.2013

                'remove this additional check  5.9.15
                ' and FOSrevenuelegs = CASrevenuelegs

                'added CASrevenuelegs <> 0 5.11.15
                'remove and caslinebreaks <= (foslinebreaks + 7)  ' rk 11.12.15
                '20140512 - pab - add model start and end for calendar
                Dim req As String = "SELECT top 1  right(rtrim(modelrunid), 1) as mt" &
                    "  ,[ModelRunID]  ,[CAStotalrevenueexpense]  ,[CASefficiency]  ,ModelStart , [GMTStart], [GMTEnd] FROM [dbo].[OptimizerLog] l " &
                    "left join [OptimizerRequest] r on left(l.[ModelRunID], 5) = r.id where  casrevenuemiles <> 0 and  left(modelrunid, 2) = '28'  and l.carrierid = 49  and " &
                    "right(rtrim(modelrunid), 1) = 'C'  and ModelRunID not like '%R0%' and CASrevenuelegs <> 0 and description not like 'Optimizer request %'      and CASrevenueexpense <> 0   order by CAStotalrevenueexpense asc"
                req = Replace(req, "28", customid)
                req = Replace(req, "carrierid = 49", "carrierid = " & Session("carrierid"))
                req = Replace(req, "left(l.[ModelRunID], 5", "left(modelrunid, " & Len(customid))
                req = Replace(req, "left(modelrunid, 2", "left(modelrunid, " & Len(customid))


                'and ModelRunID not like '%R0%'

                Dim cidstring As String = ""
                If Not IsNothing(Session("carrierid")) Then
                    If IsNumeric(Session("carrierid")) Then
                        cidstring = Session("carrierid")
                    End If
                End If
                req = Replace(req, "abc", cidstring)


                If e.CommandName.ToString = "Best" Then
                    '   req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "and right(rtrim(modelrunid), 1) <> 'C'")
                    req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "")
                End If

                If e.CommandName.ToString = "Eff" Then
                    ' req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "and right(rtrim(modelrunid), 1) <> 'C'")
                    req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "")
                    req = Replace(req, "order by CAStotalrevenueexpense asc", "order by CASefficiency desc")
                End If


                If e.CommandName.ToString = "Base" Then
                    req = Replace(req, "and ModelRunID not like '%R0%'", "")

                End If



                Dim c As Integer
                c = 6
                If e.CommandName.ToString = "Eff" Then c = 8
                If e.CommandName.ToString = "Best" Then c = 7

                If e.CommandName.ToString = "Base" Or e.CommandName.ToString = "Eff" Or e.CommandName.ToString = "Best" Then


                    Dim rs As New ADODB.Recordset

                    'If cnsetting.State = 0 Then
                    '    cnsetting.ConnectionString = connectstringsql
                    '    cnsetting.Open()
                    'End If
                    If cnopt.State = 0 Then
                        cnopt.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
                        cnopt.Open()
                    End If

                    If rs.State = 1 Then rs.Close()

                    'rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
                    rs.Open(req, cnopt, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

                    If Not rs.EOF Then

                        modelrunid = Trim(rs.Fields("ModelRunID").Value)
                        '20140512 - pab - add model start for calendar
                        modelstart = CDate(rs.Fields("GMTStart").Value.ToString.Trim).ToShortDateString
                        modelend = CDate(rs.Fields("GMTEnd").Value.ToString).ToShortDateString
                        ' Me.txtUsersTotal.Text = rs.Fields("Count").Value
                        If rs.State = 1 Then rs.Close()

                        '20140512 - pab - add model start for calendar
                        Response.Redirect("/ModelDetails.aspx?modelrunid=" & modelrunid & "&modelstart=" & modelstart & "&modelend=" & modelend)

                        '     Call LoadCountries (RadComboBox1.Text) Response.Redirect("http://optimizerpanelwest.cloudapp.net/Panel.aspx?modelrunid=" & modelrunid)
                    Else

                        GridView2.Rows.Item(si).Cells(c).BackColor = Drawing.Color.MediumVioletRed

                    End If


                    If rs.State = 1 Then rs.Close()

                End If

            End If

        Catch ex As Exception

            If ex.Message <> "Thread was being aborted." Then
                Me.lblMsg.Text = "  Please refresh page and try again"
            End If

        End Try

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