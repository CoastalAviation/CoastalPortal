Imports CoastalPortal.AirTaxi

Public Class OptimizerCalendar
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '20170924 - pab - show dev models
        Me.lblDev.Visible = False
        Me.lblDev.Enabled = False
        Me.chkDev.Visible = False
        Me.chkDev.Enabled = False

        '20170126 - pab - fix old tmc models being pulled up
        If Session("username") Is Nothing Then
            FormsAuthentication.RedirectToLoginPage()
            Exit Sub

            '20170924 - pab - show dev models
        Else
            If InStr(Session("username").ToString.ToLower, "@coastalav") > 0 Then
                Me.lblDev.Visible = True
                Me.lblDev.Enabled = True
                Me.chkDev.Visible = True
                Me.chkDev.Enabled = True
            End If
        End If

        ''If Not Request.QueryString("modelrunid") Is Nothing Then
        ''    modelrunid = Request.QueryString("modelrunid")
        ''End If
        'If Not hddnModelRunID Is Nothing Then
        '    modelrunid = hddnModelRunID.Value
        'End If
        If Session("fosmodelrunid") Is Nothing Then
            Session("fosmodelrunid") = ""
        End If

        If _carrierid = 0 Then
            '20170126 - pab - no more tmc
            'Dim host As String = "tmcjets."
            Dim host As String = "wheelsup."
            geturlaliasandconnections(host)
            Session("urlalias") = _urlalias

            '20160908 - pab - carrierid getting lost - set as session variable
            Session("carrierid") = _carrierid

        End If

        '20170924 - pab - show dev models
        If Me.chkDev.Checked = True Then
            usedevdb = True
        Else
            usedevdb = False
        End If

        '20161222 - pab - fix calendar
        'Dim dt As DataTable = DataAccess.GetFOSOptimizerRunsByCarrierID(_carrierid)
        '20170723 - pab - include r0 models 
        Dim dt As DataTable = DataAccess.GetFOSFlightsBestModels(_carrierid, True)
        rcbModelRun.Items.Clear()
        If Not isdtnullorempty(dt) Then
            Dim i As Integer = 0
            For i = 0 To dt.Rows.Count - 1
                Dim ti As New Telerik.Web.UI.RadComboBoxItem
                If Not IsDBNull(dt.Rows(i).Item("ModelRunID")) Then
                    ti.Text = Trim(dt.Rows(i).Item("ModelRunID").ToString) & " - " & Trim(dt.Rows(i).Item("ModelStart").ToString)
                    ti.Value = Trim(dt.Rows(i).Item("ModelRunID").ToString)
                    rcbModelRun.Items.Add(ti)
                End If
            Next

            If Not IsPostBack Then
                If Session("fosmodelrunid").ToString.Trim <> "" Then
                    'lblModelRunID.Text = modelrunid.Trim
                    'Dim i As Integer = InStr(modelrunid, " - ")
                    'If i > 0 Then
                    '    If IsDate((Mid(modelrunid, i + 2))) Then Session("fosmodelstart") = CDate(Mid(modelrunid, i + 2))
                    '    _fosmodelrunid = Left(modelrunid, i - 1)
                    'End If
                    lblModelRunID.Text = Session("fosmodelrunid").ToString & " - " & Session("fosmodelstart")
                Else
                    '20161222 - pab - fix calendar
                    Dim da As New DataAccess
                    Session("fosmodelrunid") = da.GetFOSFlightsBestModelRunID(_carrierid).Trim
                    modelrunid = Session("fosmodelrunid").ToString
                    Session("fosmodelstart") = CDate(dt.Rows(0).Item("ModelStart").ToString)

                    '20170929 - fix initial display bug
                    If Session("fosmodelrunid").ToString = "" Then
                        lblModelRunID.Text = rcbModelRun.Items(0).Text
                        modelrunid = dt.Rows(0).Item("ModelRunID").ToString.Trim
                        Session("fosmodelrunid") = modelrunid
                        Session("fosmodelstart") = CDate(dt.Rows(0).Item("ModelStart").ToString)
                    End If

                    lblModelRunID.Text = Session("fosmodelrunid").ToString & " - " & Session("fosmodelstart")
                End If
            End If

            lblModelDesc.Text = ""
            i = InStr(lblModelRunID.Text, "-")
            If 1 > 0 Then
                dt = DataAccess.GetFOSOptimizerRequestByID(_carrierid, CInt(Left(lblModelRunID.Text, i - 1)))
                If Not isdtnullorempty(dt) Then
                    lblModelDesc.Text = dt.Rows(0).Item("description").ToString.Trim
                End If
            End If

        End If

    End Sub

    'Protected Sub LinkButtonLoadReport_Click(sender As Object, e As EventArgs) Handles LinkButtonLoadReport.Click

    '    Dim modelrunid As String = rcbModelRun.Text
    '    Dim i As Integer = InStr(modelrunid, " - ")

    '    lblModelRunID.Text = modelrunid

    '    If i > 0 Then
    '        modelrunid = Left(modelrunid, i - 1)
    '    End If
    '    Response.Redirect("FlightPlanningOpt.aspx?modelrunid=" & modelrunid)

    'End Sub

    Protected Sub rcbModelRun_SelectedIndexChanged(sender As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles rcbModelRun.SelectedIndexChanged

        Dim modelrunid As String = rcbModelRun.Text
        Dim modelstart As Date = Now
        Dim i As Integer = InStr(modelrunid, " - ")

        lblModelRunID.Text = modelrunid

        If i > 0 Then
            If IsDate((Mid(modelrunid, i + 2))) Then modelstart = CDate(Mid(modelrunid, i + 2))
            modelrunid = Left(modelrunid, i - 1)
        End If
        Session("fosmodelrunid") = modelrunid
        Session("fosmodelrunidcas") = ""
        Session("fosmodelstart") = modelstart
        Response.Redirect("FlightPlanningOpt.aspx")

    End Sub

End Class