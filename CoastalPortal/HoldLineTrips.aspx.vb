Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI
Imports CoastalPortal.Models

Public Class HoldLineTrips
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

    '20180723 - pab - fix bug - second page of grid not displaying
    Public carrierid As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        carrierid = CInt(Session("carrierid"))

        Try

            If Session("carrierid") Is Nothing Then
                Insertsys_log(carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "HoldLineTrips.aspx.vb")
            Else
                Insertsys_log(carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - " & Session("carrierid").ToString, "HoldLineTrips.aspx.vb")
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
            '20120503 - pab - run time improvements - execute on if not postback
            If Not IsPostBack Then

                '20160517 - pab - fix carrierid = 0 preventing quotes
                If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And carrierid = 0 Then
                    carrierid = TMC
                End If

                '20111121 - pab - convert to single db
                If IsNothing(carrierid) Or carrierid = 0 Then
                    '20160517 - pab - fix carrierid = 0 preventing quotes
                    AirTaxi.Insertsys_log(0, appName, Request.Url.Host & " carrierid null or 0 - user " & Session("email").ToString, "Page_Load", "HoldLineTrips.aspx.vb")

                    '20160823 - pab - redirect to logon if carrierid lost
                    Response.Redirect("CustomerLogin.aspx", True)
                End If

                '20180723 - pab - fix bug - second page of grid not displaying
                'SqlDataSource1.SelectCommand = "SELECT distinct r.ID as modelrun,Description,replace(TripNumber,r.CarrierID + '-','') " &
                '    "as TripNumber,Weightclass,DepartureAirport,ArrivalAirport,cast(DepartureDate + ' ' + DepartureTime as datetime) " &
                '    "as DEPARTS,r.CarrierID,r.GMTStart FROM OptimizerRequest r join QuoteFlights q on r.ID = q.QuoteNumber " &
                '    "join FCDRList l on r.ID = l.modelrun where ParentRequestNumber > 0 " &
                '    "and r.GMTStart >= DATEADD(d,-7,getdate()) and r.CarrierID = " & carrierid & " order by r.id desc"
                ''20180718 - pab - add new master level and shift other levels up
                'SqlDataSource4.SelectCommand = "SELECT distinct r.ID as modelrun,r.Description,r.RequestDate,r.declaredcomplete," &
                '    "r.CompleteDate,case when r.CompleteDate is not null then (case when r.CarrierID = 108 then 'Y' else " &
                '    "format(r.CompleteDate,'g','en-US') end) else 'N' end as Complete,r.GMTStart,r.GMTEnd,r.CarrierID FROM " &
                '    "OptimizerRequest r join OptimizerRequest r2 on r.id = r2.ParentRequestNumber where r.CarrierID = " &
                '    carrierid & " and r.DemandFlights = 1 and r.GMTStart >= DATEADD(d,-2,r.GMTStart) order by r.id desc"
                SqlDataSource1.SelectParameters(0).DefaultValue = carrierid
                SqlDataSource4.SelectParameters(0).DefaultValue = carrierid

                '20130930 - pab - change email from
                '20171121 - pab - fix carriers changing midstream - change to Session variables
                If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""
                If Session("emailfrom").ToString = "" Then
                    Session("emailfrom") = da.GetSetting(CInt(Session("carrierid")), "emailsentfrom")
                End If

            End If

            '20100608 - pab - add logo to email
            Session("ApplicationPath") = Request.PhysicalApplicationPath


        Catch ex As Exception
            Dim s As String = ex.Message
            If s <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.InsertEmailQueue(carrierid, "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "HoldLineTrips.aspx.vb Page_Load error", s, False, "", "", "", False)
                AirTaxi.Insertsys_log(carrierid, appName, s, "Page_Load", "HoldLineTrips.aspx.vb")
            End If

        End Try

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        carrierid = CInt(Session("carrierid"))

        '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
        If IsNothing(Session("urlalias")) Then Session("urlalias") = ""
        Dim urlalias As String = Session("urlalias").ToString

        '20171121 - pab - fix carriers changing midstream - change to Session variables
        If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""

        Try

            Dim da As New DataAccess

            If Not IsNothing(Session("username")) Then
                If InStr(Session("username").ToString.ToLower, "coastalav") = 0 Then
                    RadGrid1.MasterTableView.RenderColumns(3).Visible = False
                    RadGrid1.MasterTableView.RenderColumns(4).Visible = False

                    '20180718 - pab - add new master level and shift other levels up
                    RadGrid2.MasterTableView.RenderColumns(5).Visible = False
                    RadGrid2.MasterTableView.RenderColumns(8).Visible = False
                End If
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
            SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " HoldLineTrips.aspx.vb Page_PreRender error", s, carrierid)
            AirTaxi.Insertsys_log(carrierid, appName, Left(Now & " " & s, 500), "HoldLineTrips.aspx.vb Page_PreRender", "")

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

    '20171209 - pab - link to quoting portal
    Protected Sub LinkQuoting_Click(sender As Object, e As EventArgs) Handles LinkQuoting.Click

        If IsNothing(Session("urlalias")) Then Session("urlalias") = ""
        If Session("urlalias").ToString.Trim <> "" Then
            'Response.Redirect("http://" & Session("urlalias").ToString.Trim & ".personiflyadminuat.com/CustomerLogin.aspx", True)
            '20180606 - pab - change url for admin portal
            'Response.Write("<script>window.open ('http://" & Session("urlalias").ToString.Trim & ".personiflyadminuat.com/CustomerLogin.aspx','_blank');</script>")
            Response.Write("<script>window.open ('http://" & Session("urlalias").ToString.Trim & ".avaisearch.com/CustomerLogin.aspx','_blank');</script>")
        End If

    End Sub

    '20180712 - pab - hide some columns for users
    Private Sub RadGrid1_DetailTableDataBind(sender As Object, e As GridDetailTableDataBindEventArgs) Handles RadGrid1.DetailTableDataBind

        '20180624 - pab - add tracking fields to fcdr
        If Not IsNothing(Session("username")) Then
            If InStr(Session("username").ToString.ToLower, "coastalav") = 0 Then
                Select Case e.DetailTableView.Name
                    Case "Child1"
                        For Each column As GridColumn In e.DetailTableView.Columns
                            Select Case column.UniqueName
                                Case "SavingsDay0", "SavingsDay1", "SavingsDay2", "CarrierID", "modelrun"
                                    column.Visible = False
                            End Select
                        Next
                End Select
            End If
        End If

    End Sub

    '20180712 - pab - hide some columns for users
    Private Sub RadGrid1_PreRender(sender As Object, e As EventArgs) Handles RadGrid1.PreRender

        For Each item As GridDataItem In RadGrid1.Items
            If (item.OwnerTableView.Name = "Child1") Then
                For i As Integer = 0 To item.OwnerTableView.Items.Count - 1
                    For i2 As Integer = 3 To 7
                        If IsNumeric(item.OwnerTableView.Items(i).Cells(i2).Text) Then
                            If CInt(item.OwnerTableView.Items(i).Cells(i2).Text) < 0 Then
                                item.OwnerTableView.Items(i).Cells(i2).ForeColor = System.Drawing.Color.Red
                            Else
                                item.OwnerTableView.Items(i).Cells(i2).ForeColor = System.Drawing.Color.Green
                            End If
                        End If
                    Next
                    '20180716 - pab - add pdf link
                    item.OwnerTableView.Items(i).Cells(11).ForeColor = System.Drawing.Color.Blue
                    item.OwnerTableView.Items(i).Cells(11).BackColor = System.Drawing.Color.FromArgb(215, 230, 247)
                Next
            End If
        Next

    End Sub

    '20180718 - pab - add new master level and shift other levels up
    Private Sub RadGrid2_DetailTableDataBind(sender As Object, e As GridDetailTableDataBindEventArgs) Handles RadGrid2.DetailTableDataBind

        If Not IsNothing(Session("username")) Then
            If InStr(Session("username").ToString.ToLower, "coastalav") = 0 Then
                Select Case e.DetailTableView.Name
                    Case "Child3"
                        For Each column As GridColumn In e.DetailTableView.Columns
                            Select Case column.UniqueName
                                Case "Description", "GMTStart"
                                    column.Visible = False
                            End Select
                        Next
                    Case "Child4"
                        For Each column As GridColumn In e.DetailTableView.Columns
                            Select Case column.UniqueName
                                Case "SavingsDay0", "SavingsDay1", "SavingsDay2", "CarrierID", "modelrun"
                                    column.Visible = False
                            End Select
                        Next
                End Select
            End If
        End If

    End Sub

    '20180718 - pab - add new master level and shift other levels up
    Private Sub RadGrid2_PreRender(sender As Object, e As EventArgs) Handles RadGrid2.PreRender

        For Each item As GridDataItem In RadGrid2.Items
            If (item.OwnerTableView.Name = "Child4") Then
                For i As Integer = 0 To item.OwnerTableView.Items.Count - 1
                    For i2 As Integer = 3 To 7
                        If IsNumeric(item.OwnerTableView.Items(i).Cells(i2).Text) Then
                            If CInt(item.OwnerTableView.Items(i).Cells(i2).Text) < 0 Then
                                item.OwnerTableView.Items(i).Cells(i2).ForeColor = System.Drawing.Color.Red
                            Else
                                item.OwnerTableView.Items(i).Cells(i2).ForeColor = System.Drawing.Color.Green
                            End If
                        End If
                    Next
                    '20180716 - pab - add pdf link
                    item.OwnerTableView.Items(i).Cells(11).ForeColor = System.Drawing.Color.Blue
                    item.OwnerTableView.Items(i).Cells(11).BackColor = System.Drawing.Color.FromArgb(215, 230, 247)
                Next
            End If
        Next

    End Sub

End Class