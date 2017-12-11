Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI

Public Class RunOptimizer
    Inherits System.Web.UI.Page

    'Private dtflights As New DataTable
    Public Shared cnmkazure As New ADODB.Connection
    Public Shared cnsgazure As New ADODB.Connection

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Session("carrierid") Is Nothing Then
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                Insertsys_log(0, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "RunOptimizer.aspx.vb")
            Else
                Insertsys_log(CInt(Session("carrierid")), appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - " & Session("carrierid").ToString, "RunOptimizer.aspx.vb")
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

                '20160517 - pab - fix carrierid = 0 preventing quotes
                If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And CInt(Session("carrierid")) = 0 Then

                    Session("carrierid") = 65
                End If

                '20111121 - pab - convert to single db
                If IsNothing(CInt(Session("carrierid"))) Or CInt(Session("carrierid")) = 0 Then
                    '20160517 - pab - fix carrierid = 0 preventing quotes
                    AirTaxi.Insertsys_log(0, appName, Request.Url.Host & " carrierid null or 0 - user " & Session("email").ToString, "Page_Load", "RunOptimizer.aspx.vb")

                    '20160823 - pab - redirect to logon if carrierid lost
                    Response.Redirect("CustomerLogin.aspx", True)
                End If

                '20130930 - pab - change email from
                '20171121 - pab - fix carriers changing midstream - change to Session variables
                If IsNothing(Session("emailfrom")) Then Session("emailfrom") = ""
                If Session("emailfrom").ToString = "" Then
                    Session("emailfrom") = da.GetSetting(CInt(Session("carrierid")), "emailsentfrom")
                End If

                Dim startdate As Date = Now.ToUniversalTime
                Dim enddate As Date
                startdate = DateAdd(DateInterval.Day, 1, startdate)
                Me.RadDateTimeFrom.SelectedDate = CDate(startdate.ToShortDateString & " 04:00")
                enddate = DateAdd(DateInterval.Day, 3, CDate(Me.RadDateTimeFrom.SelectedDate))
                enddate = DateAdd(DateInterval.Hour, 3, enddate)
                Me.RaddatetimeTo.SelectedDate = enddate

                Me.txtemail.Text = Session("email").ToString

                '20171031 - pab - fix model run defaults
                RadSliderUpg.Value = 72
                RadSliderMBF.Value = 60
                RadSliderDepDelay.Value = 15
                ChkBroker.Checked = False
                RadSliderAutoPin.Value = 6
                RadSliderTaxiTime1.Value = 15
                RadSliderFastTurn.Value = 30
                RadSliderPrePosition.Value = 0
                chkValidation.Checked = True
                chkTrailingDH.Checked = True
                chkCrewRules.Checked = True
                chkDeconflict.Checked = True
                chkRejects.Checked = False
                RadSliderR60Delay.Value = 5
                RadSliderCycleCost.Value = 0
                chkScrubAfterModel.Checked = False
                chkIterate.Checked = True
                chkDetangleCrewIncoming.Checked = True
                RadSliderCrewWithinX.Value = 240
                chkRebuild.Checked = True
                chkallowupgrades.Checked = True
                chkallowslides.Checked = False
                chkproratecostbyday.Checked = False
                chkscrubincoming.Checked = False
                RadSlidercrewdutyday.Value = 14
                RadSliderswapwindow.Value = 18
                CheckOverride.Checked = False
                chkAssigns.Checked = False
                RadSliderMaxSlideMinutes.Value = 0
                chkFCDRPublish.Checked = True
                '20171101 - pab - add AssignNewTrips per David - not used by optimzer yet
                chkAssignNewTrips.Checked = False

                '20171128 - pab - add 3 new options - hide until ready for prime time
                chkPinCharter.Checked = False
                chkPinNetJets.Checked = False
                'If CInt(Session("carrierid")) = JETLINX Then
                '    chkPinNetJets.Visible = True
                'Else
                '    chkPinNetJets.Visible = False
                'End If
                RadSliderAllowFlex.Value = 0
                'If CInt(Session("carrierid")) = XOJET Then
                '    txtAllowFlex.Visible = True
                '    lblAllowFlex.Visible = True
                '    RadSliderAllowFlex.Visible = True
                'Else
                txtAllowFlex.Visible = False
                '    lblAllowFlex.Visible = False
                '    RadSliderAllowFlex.Visible = False
                'End If

                If InStr(Session("email").ToString.ToLower, "@coastal") > 0 Then
                    pnlAdvancedSettings.Visible = True
                Else
                    pnlAdvancedSettings.Visible = False
                End If

            End If

            '20100608 - pab - add logo to email
            Session("ApplicationPath") = Request.PhysicalApplicationPath

        Catch ex As Exception
            Dim s As String = ex.Message
            If s <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.InsertEmailQueue(CInt(Session("carrierid")), "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "RunOptimizer.aspx.vb Page_Load error", s, False, "", "", "", False)
                AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, s, "Page_Load", "RunOptimizer.aspx.vb")
            End If

        End Try

    End Sub

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
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            SendEmail(Session("emailfrom"), "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " RunOptimizer.aspx.vb Page_PreRender error", s, CInt(Session("carrierid")))
            AirTaxi.Insertsys_log(CInt(Session("carrierid")), appName, Left(Now & " " & s, 500), "RunOptimizer.aspx.vb Page_PreRender", "")

        End Try

    End Sub

    Protected Sub LinkButtonAdd_Click(sender As Object, e As EventArgs) Handles LinkButtonAdd.Click

        If Session("carrierid") = 0 Then

            Response.Redirect("CustomerLogin.aspx", True)
            Exit Sub

        End If

        'rk 8/2/2013 prevent a duplicate write
        Dim corecount As Integer = 60
        Try

            Dim req As String

            Dim rs As New ADODB.Recordset
            Dim rs2 As New ADODB.Recordset

            Me.lblMsg.Text = ""
            If Me.txtDescription.Text.Trim = "" Then
                Me.lblMsg.Text = "Please enter Model Run Description."
                Me.lblMsg.Focus()
                Exit Sub
            End If

            If cnsgazure.State = 0 Then
                cnsgazure.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
                cnsgazure.Open()
            End If

            req = "select top 1 * from CoreCounts order by updatetime desc"

            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


            If Not rs.EOF Then
                corecount = rs.Fields("corecount").Value
            End If


            req = "SELECT top 1 * from optimizerrequest where     description not like 'Optimizer request %'   and carrierid = abc  order by   id desc "


            '   req = "select top 1 * from OptimizerRequest where carrierid = 'abc' order by id desc"

            req = Replace(req, "abc", Session("carrierid"))


            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            ' Me.txtUsersTotal.Text = rs.Fields("Count").Value

            If Not rs.EOF Then
                Dim reqdate As Date = rs.Fields("requestdate").Value

                Dim dd As Integer = DateDiff(DateInterval.Hour, reqdate, Now)
                If rs.Fields("description").Value = Me.txtDescription.Text Then

                    If dd < 3 Then
                        lblMsg.Text = "Model Request Error at " & Now & " Please change name if submitting additional models"
                        lblMsg.BackColor = System.Drawing.Color.Red
                        Exit Sub

                    End If
                End If
            End If

            If rs.State = 1 Then rs.Close()
        Catch ex As Exception

            'if we couldn't not super critical here.
        End Try

        Try

            Dim req As String
            Dim rs As New ADODB.Recordset
            Dim rs2 As New ADODB.Recordset

            If cnsgazure.State = 0 Then
                cnsgazure.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
                cnsgazure.Open()
            End If

            req = "select * from OptimizerRequest where 1 = 2 "

            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
            ' Me.txtUsersTotal.Text = rs.Fields("Count").Value

            rs.AddNew()
            rs.Fields("CarrierID").Value = Session("carrierid")
            rs.Fields("Description").Value = Me.txtDescription.Text
            rs.Fields("GMTStart").Value = Me.RadDateTimeFrom.SelectedDate
            rs.Fields("GMTEnd").Value = RaddatetimeTo.SelectedDate
            rs.Fields("MinutesBetweenFlights").Value = CInt(RadSliderMBF.Value.ToString)
            rs.Fields("MaxSpeedKts").Value = 680 'RadSliderAvgSpeed.Value.ToString
            rs.Fields("DepartureDelays").Value = RadSliderDepDelay.Value.ToString

            If chkallowslides.Checked = True Then
                rs.Fields("allowslides").Value = 1
            Else
                rs.Fields("allowslides").Value = 0
            End If

            If chkallowupgrades.Checked = True Then
                rs.Fields("checkallowupgrades").Value = 1
            Else
                rs.Fields("checkallowupgrades").Value = 0
            End If

            If CheckOverride.Checked = True Then
                rs.Fields("OverrideMBF").Value = 1
            Else
                rs.Fields("OverrideMBF").Value = 0
            End If

            If chkRebuild.Checked = True Then
                rs.Fields("RebuildModel").Value = 1
            Else
                rs.Fields("RebuildModel").Value = 0
            End If

            If chkDeconflict.Checked = True Then
                rs.Fields("DeConflict").Value = 1
            Else
                rs.Fields("DeConflict").Value = 0
            End If

            If chkIterate.Checked = True Then
                rs.Fields("Iterate").Value = 1
            Else
                rs.Fields("Iterate").Value = 0
            End If

            If chkScrubAfterModel.Checked = True Then
                rs.Fields("ScrubAfterModel").Value = 1
            Else
                rs.Fields("ScrubAfterModel").Value = 0
            End If

            If chkDetangleCrewIncoming.Checked = True Then
                rs.Fields("DetangleCrewIncoming").Value = 1
            Else
                rs.Fields("DetangleCrewIncoming").Value = 0
            End If

            If chkValidation.Checked = True Then
                rs.Fields("RunAddlValidation").Value = 1
            Else
                rs.Fields("RunAddlValidation").Value = 0
            End If

            If chkTrailingDH.Checked = True Then
                rs.Fields("ExcludeTrailingDH").Value = 1
            Else
                rs.Fields("ExcludeTrailingDH").Value = 0
            End If

            If ChkBroker.Checked = True Then
                rs.Fields("IncludeBrokerAircraft").Value = 1
            Else
                rs.Fields("IncludeBrokerAircraft").Value = 0
            End If

            'If chkFastRun.Checked = True Then
            If rblModelType.SelectedValue = "Fast" Then
                rs.Fields("FastRun").Value = 1
            Else
                rs.Fields("FastRun").Value = 0
            End If

            'If cbScheduleUpdate.Checked = True Then
            If rblModelType.SelectedValue = "Schedule" Then
                rs.Fields("ScheduleUpdate").Value = 1
            Else
                rs.Fields("ScheduleUpdate").Value = 0
            End If

            'If cbRunR0only.Checked = True Then
            'If rblModelType.SelectedValue = "Full" Then
            rs.Fields("RunR0only").Value = 0
            'Else
            '    rs.Fields("RunR0only").Value = 1
            'End If

            If chkAssigns.Checked = True Then
                rs.Fields("UseAssigns").Value = 1
            Else
                rs.Fields("UseAssigns").Value = 0
            End If

            ' RejectReasons
            If chkRejects.Checked = True Then
                rs.Fields("RecordRR").Value = 1
            Else
                rs.Fields("RecordRR").Value = 0
            End If

            rs.Fields("R60Wait").Value = CInt(RadSliderR60Delay.Value.ToString)

            If chkCrewRules.Checked = True Then
                rs.Fields("CheckCrewRules").Value = 1
            Else
                rs.Fields("CheckCrewRules").Value = 0
            End If

            If chkFCDRPublish.Checked = True Then
                rs.Fields("PublishFCDR").Value = 1
            Else
                rs.Fields("PublishFCDR").Value = 0
            End If

            '20171101 - pab - add AssignNewTrips per David - not used by optimzer yet
            If chkAssignNewTrips.Checked = True Then
                rs.Fields("AssignNewTrips").Value = 1
            Else
                rs.Fields("AssignNewTrips").Value = 0
            End If

            Dim st As New System.TimeSpan

            'If Not (IsNothing(RadTimeFrom.SelectedTime)) Then
            '    rs.Fields("CrewTimeFrom").Value = RadTimeFrom.SelectedTime.Value.Hours & ":" & RadTimeFrom.SelectedTime.Value.Minutes
            'End If

            'If Not (IsNothing(RadTimeTo.SelectedTime)) Then
            '    rs.Fields("CrewTimeTo").Value = RadTimeTo.SelectedTime.Value.Hours & ":" & RadTimeTo.SelectedTime.Value.Minutes
            'End If

            rs.Fields("CrewDutyDay").Value = RadSlidercrewdutyday.Value.ToString
            rs.Fields("SwapWindow").Value = RadSliderswapwindow.Value.ToString
            rs.Fields("MaxSlideMinutes").Value = RadSliderMaxSlideMinutes.Value.ToString
            rs.Fields("UpgradeWindow").Value = RadSliderUpg.Value.ToString
            rs.Fields("AutoPin").Value = RadSliderAutoPin.Value.ToString
            rs.Fields("TaxiTime").Value = RadSliderTaxiTime1.Value.ToString
            rs.Fields("FastTurnSameTrip").Value = RadSliderFastTurn.Value.ToString
            rs.Fields("PrepositionFirstFlight").Value = RadSliderPrePosition.Value.ToString
            rs.Fields("CrewWithinX").Value = RadSliderCrewWithinX.Value.ToString
            rs.Fields("email").Value = Me.txtemail.Text 'rk 3/6/2013 add email for notifications
            rs.Fields("RequestDate").Value = Now
            ' rs.Fields("CompleteDate").Value =
            rs.Fields("BestModel").Value = ""
            rs.Fields("BestModelCost").Value = 0

            'if we don't have enough cores do something to spool up.
            'cause a delay by adding a status of C
            If corecount > 2 Then
                rs.Fields("Status").Value = "P"
            Else
                rs.Fields("Status").Value = "C"
            End If

            rs.Fields("BaseLink").Value = ""
            rs.Fields("BestLink").Value = ""
            rs.Fields("Template").Value = 0
            'If chkTemplate.Checked = True Then
            '    rs.Fields("Template").Value = 1
            'End If

            rs.Fields("CycleCost").Value = RadSliderCycleCost.Value
            If chkproratecostbyday.Checked Then
                rs.Fields("ProRateCostsByDay").Value = 1
            Else
                rs.Fields("ProRateCostsByDay").Value = 0
            End If

            If chkscrubincoming.Checked Then
                rs.Fields("ScrubIncoming").Value = 1
            Else
                rs.Fields("ScrubIncoming").Value = 0
            End If

            '20171128 - pab - add 3 new options - hide until ready for prime time
            If chkPinCharter.Checked Then
                rs.Fields("PinCharter").Value = 1
            Else
                rs.Fields("PinCharter").Value = 0
            End If
            If chkPinNetJets.Checked Then
                rs.Fields("PinNetJets").Value = 1
            Else
                rs.Fields("PinNetJets").Value = 0
            End If
            rs.Fields("AllowFlex").Value = CInt(RadSliderAllowFlex.Value.ToString)

            rs.Update()

            If rs.State = 1 Then rs.Close()

            req = "select top 1 * from OptimizerRequest order by id desc "

            If rs.State = 1 Then rs.Close()
            rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            ' Me.txtUsersTotal.Text = rs.Fields("Count").Value

            Dim runid As Integer
            runid = 0

            If Not rs.EOF Then
                runid = rs.Fields("id").Value
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                postToServiceBusQueue("OPTREQUEST", "OPTIMIZERREQUEST[" & runid & "]", 0, CInt(Session("carrierid")), False)
            End If

            If runid <> 0 And Session("carrierid") <> 0 Then



                If rs.State = 1 Then rs.Close()


                req = "select * from OptimizerWeights where OptimizerRun = 'Default' and carrierid = 49 "
                req = Replace(req, "carrierid = 49", "carrierid = " & Session("carrierid"))


                If rs.State = 1 Then rs.Close()
                rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
                ' Me.txtUsersTotal.Text = rs.Fields("Count").Value



                req = "select * from OptimizerWeights where 1 = 2 "

                If rs2.State = 1 Then rs2.Close()
                rs2.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
                ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


                Dim ow(10, 4) As String
                Dim i As Integer = 0

                Do While Not rs.EOF

                    i = i + 1
                    ow(i, 1) = rs.Fields("CarrierID").Value
                    ow(i, 2) = rs.Fields("Day").Value
                    ow(i, 3) = rs.Fields("Percentage").Value
                    ow(i, 4) = rs.Fields("FleetType").Value

                    rs.MoveNext()

                Loop

                Dim ii As Integer
                For ii = 1 To i

                    rs2.AddNew()

                    rs2.Fields("OptimizerRun").Value = runid
                    rs2.Fields("CarrierID").Value = ow(ii, 1)
                    rs2.Fields("Day").Value = ow(ii, 2)
                    rs2.Fields("Percentage").Value = ow(ii, 3)
                    rs2.Fields("FleetType").Value = ow(ii, 4)

                    rs2.Update()

                Next ii
            End If

            If rs2.State = 1 Then rs2.Close()
            If rs.State = 1 Then rs.Close()

        Catch msg As Exception
            lblMsg.Text = "Model Request Error at " & Now

            Exit Sub
        End Try

        If corecount > 2 Then
            lblMsg.Text = "Model Request Submitted at " & Now
        Else
            lblMsg.Text = "Spooling up additional cores ... please expect a five minute delay ... Model Request Submitted at " & Now
        End If

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
            Response.Write("<script>window.open ('http://" & Session("urlalias").ToString.Trim & ".personiflyadminuat.com/CustomerLogin.aspx','_blank');</script>")
        End If

    End Sub

End Class