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
                Insertsys_log(_carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                    "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString, "Page_Load" &
                    "; Session(carrierid) - null", "RunOptimizer.aspx.vb")
            Else
                Insertsys_log(_carrierid, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
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
                If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And _carrierid = 0 Then
                    _carrierid = 65
                End If

                '20111121 - pab - convert to single db
                If IsNothing(_carrierid) Or _carrierid = 0 Then
                    '20160517 - pab - fix carrierid = 0 preventing quotes
                    AirTaxi.Insertsys_log(0, appName, Request.Url.Host & " carrierid null or 0 - user " & Session("email").ToString, "Page_Load", "RunOptimizer.aspx.vb")

                    '20160823 - pab - redirect to logon if carrierid lost
                    Response.Redirect("CustomerLogin.aspx", True)
                End If

                '20130930 - pab - change email from
                If IsNothing(_emailfrom) Then _emailfrom = ""
                If _emailfrom = "" Then
                    _emailfrom = da.GetSetting(_carrierid, "emailsentfrom")
                End If

                Dim startdate As Date = Now.ToUniversalTime
                Dim enddate As Date
                startdate = DateAdd(DateInterval.Day, 1, startdate)
                enddate = DateAdd(DateInterval.Day, 3, startdate)
                Me.RadDateTimeFrom.SelectedDate = CDate(startdate.ToShortDateString & " 04:00")
                Me.RaddatetimeTo.SelectedDate = CDate(enddate.ToShortDateString & " 19:00")

                Me.txtemail.Text = Session("email").ToString

            End If

            '20100608 - pab - add logo to email
            Session("ApplicationPath") = Request.PhysicalApplicationPath

        Catch ex As Exception
            Dim s As String = ex.Message
            If s <> "Thread was being aborted." Then
                If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.InsertEmailQueue(_carrierid, "CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", "",
                    "RunOptimizer.aspx.vb Page_Load error", s, False, "", "", "", False)
                AirTaxi.Insertsys_log(_carrierid, appName, s, "Page_Load", "RunOptimizer.aspx.vb")
            End If

        End Try

    End Sub

    Private Sub RunOptimizer_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Try

            Dim da As New DataAccess

            If Not IsPostBack Then
                Me.lblCarrier.Text = _urlalias.ToUpper
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
            If Not IsNothing(ex.InnerException) Then s &= "" & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "",
                      appName & " RunOptimizer.aspx.vb Page_PreRender error", s, _carrierid)
            AirTaxi.Insertsys_log(_carrierid, appName, Left(Now & " " & s, 500), "RunOptimizer.aspx.vb Page_PreRender", "")

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
            'rs.Fields("DepartureDelays").Value = RadSliderDepDelay.Value.ToString
            rs.Fields("DepartureDelays").Value = 0

            'If chkallowslides.Checked = True Then
            '    rs.Fields("allowslides").Value = 1
            'Else
            rs.Fields("allowslides").Value = 0
            'End If

            'If chkallowupgrades.Checked = True Then
            '    rs.Fields("checkallowupgrades").Value = 1
            'Else
            rs.Fields("checkallowupgrades").Value = 0
            'End If

            'If CheckOverride.Checked = True Then
            '    rs.Fields("OverrideMBF").Value = 1
            'Else
            rs.Fields("OverrideMBF").Value = 0
            'End If

            'If chkRebuild.Checked = True Then
            '    rs.Fields("RebuildModel").Value = 1
            'Else
            rs.Fields("RebuildModel").Value = 0
            'End If

            'If chkDeconflict.Checked = True Then
            '    rs.Fields("DeConflict").Value = 1
            'Else
            rs.Fields("DeConflict").Value = 0
            'End If

            'If chkIterate.Checked = True Then
            '    rs.Fields("Iterate").Value = 1
            'Else
            rs.Fields("Iterate").Value = 0
            'End If

            'If chkScrubAfterModel.Checked = True Then
            '    rs.Fields("ScrubAfterModel").Value = 1
            'Else
            rs.Fields("ScrubAfterModel").Value = 0
            'End If

            'If chkDetangleCrewIncoming.Checked = True Then
            '    rs.Fields("DetangleCrewIncoming").Value = 1
            'Else
            rs.Fields("DetangleCrewIncoming").Value = 0
            'End If

            'If chkValidation.Checked = True Then
            '    rs.Fields("RunAddlValidation").Value = 1
            'Else
            rs.Fields("RunAddlValidation").Value = 0
            'End If

            'If chkTrailingDH.Checked = True Then
            '    rs.Fields("ExcludeTrailingDH").Value = 1
            'Else
            rs.Fields("ExcludeTrailingDH").Value = 0
            'End If

            'If ChkBroker.Checked = True Then
            '    rs.Fields("IncludeBrokerAircraft").Value = 1
            'Else
            rs.Fields("IncludeBrokerAircraft").Value = 0
            'End If

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
            If rblModelType.SelectedValue = "Full" Then
                rs.Fields("RunR0only").Value = 0
            Else
                rs.Fields("RunR0only").Value = 1
            End If

            'If chkAssigns.Checked = True Then
            '    rs.Fields("UseAssigns").Value = 1
            'Else
            rs.Fields("UseAssigns").Value = 0
            'End If

            'If chkRejects.Checked = True Then
            '    rs.Fields("RecordRR").Value = 1
            'Else
            rs.Fields("RecordRR").Value = 0
            'End If

            'rs.Fields("R60Wait").Value = CInt(RadSliderR60Delay.Value.ToString)
            rs.Fields("R60Wait").Value = 0
            ' RejectReasons

            'If chkCrewRules.Checked = True Then
            '    rs.Fields("CheckCrewRules").Value = 1
            'Else
            rs.Fields("CheckCrewRules").Value = 0
            'End If

            Dim st As New System.TimeSpan

            'If Not (IsNothing(RadTimeFrom.SelectedTime)) Then
            '    rs.Fields("CrewTimeFrom").Value = RadTimeFrom.SelectedTime.Value.Hours & ":" & RadTimeFrom.SelectedTime.Value.Minutes
            'End If

            'If Not (IsNothing(RadTimeTo.SelectedTime)) Then
            '    rs.Fields("CrewTimeTo").Value = RadTimeTo.SelectedTime.Value.Hours & ":" & RadTimeTo.SelectedTime.Value.Minutes
            'End If

            rs.Fields("CrewDutyDay").Value = RadSlidercrewdutyday.Value.ToString
            'rs.Fields("SwapWindow").Value = RadSliderswapwindow.Value.ToString
            'rs.Fields("MaxSlideMinutes").Value = RadSliderMaxSlideMinutes.Value.ToString
            rs.Fields("SwapWindow").Value = 0
            rs.Fields("MaxSlideMinutes").Value = 0
            rs.Fields("UpgradeWindow").Value = RadSliderUpg.Value.ToString
            rs.Fields("AutoPin").Value = RadSliderAutoPin.Value.ToString
            'rs.Fields("TaxiTime").Value = RadSliderTaxiTime1.Value.ToString
            'rs.Fields("FastTurnSameTrip").Value = RadSliderFastTurn.Value.ToString
            'rs.Fields("PrepositionFirstFlight").Value = RadSliderPrePosition.Value.ToString
            'rs.Fields("CrewWithinX").Value = RadSliderCrewWithinX.Value.ToString
            rs.Fields("TaxiTime").Value = 0
            rs.Fields("FastTurnSameTrip").Value = 0
            rs.Fields("PrepositionFirstFlight").Value = 0
            rs.Fields("CrewWithinX").Value = 0
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

            'rs.Fields("CycleCost").Value = RadSliderCycleCost.Value
            rs.Fields("CycleCost").Value = 0
            'If chkproratecostbyday.Checked Then
            '    rs.Fields("ProRateCostsByDay").Value = 1
            'Else
            rs.Fields("ProRateCostsByDay").Value = 0
            'End If

            'If chkscrubincoming.Checked Then
            '    rs.Fields("ScrubIncoming").Value = 1
            'Else
            rs.Fields("ScrubIncoming").Value = 0
            'End If

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
                postToServiceBusQueue("OPTREQUEST", "OPTIMIZERREQUEST[" & runid & "]", 0)
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

End Class