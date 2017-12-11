Imports System.Data
Imports System.IO
Imports CoastalPortal.AirTaxi
Imports Telerik.Web.UI

Public Class special_pricing
Inherits System.Web.UI.Page

    'Private Property tbRegion As Object

    '20160217 - pab - TMC zone pricing
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("email") Is Nothing Then
            Response.Redirect("CustomerLogin.aspx")

        End If

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0

        If Not IsPostBack Then

            Dim da As New DataAccess
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Dim dt As DataTable = da.GetPricingZones(CInt(Session("carrierid")), 0)
            If Not isdtnullorempty(dt) Then
                Me.ddlFromAdd.Items.Clear()
                Me.ddlToAdd.Items.Clear()
                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim ti As New Telerik.Web.UI.RadComboBoxItem
                    ti.Text = Trim(dt.Rows(i).Item("zonename").ToString) & " (" & Trim(dt.Rows(i).Item("ZoneDescription").ToString) & ")"
                    ti.Value = Trim(dt.Rows(i).Item("zonename").ToString)

                    ddlFromAdd.Items.Add(ti)
                Next

                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim ti As New Telerik.Web.UI.RadComboBoxItem
                    ti.Text = Trim(dt.Rows(i).Item("zonename").ToString) & " (" & Trim(dt.Rows(i).Item("ZoneDescription").ToString) & ")"
                    ti.Value = Trim(dt.Rows(i).Item("zonename").ToString)

                    ddlToAdd.Items.Add(ti)
                Next
            End If

            dt = da.GetAircraftTypeServiceSpecsActive_2(CInt(Session("carrierid")), 0)
            If Not isdtnullorempty(dt) Then
                Me.RadComboBoxfleet_add.Items.Clear()
                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim ti As New Telerik.Web.UI.RadComboBoxItem
                    ti.Text = Trim(dt.Rows(i).Item("Name").ToString)
                    ti.Value = Trim(dt.Rows(i).Item("AircraftTypeServiceSpecID").ToString)

                    RadComboBoxfleet_add.Items.Add(ti)
                Next
            End If


            '20170429 - fix select - carrierid missing
            find_button_Click(sender, e)

        End If

        ''20101105 - pab - add code for aliases
        'Dim url As String = Request.Url.ToString
        'Dim host As String = Request.Url.Host

        ' ''for testing only 
        ''If Not Page.IsPostBack Then
        ''    host = "Universal."
        ''    _urlalias = ""
        ''    Session("username") = "pbaumgart@coastalaviationsoftware.com"
        ''End If

        'If _urlalias Is Nothing Or _urlalias = "" Then

        '    '20101129 - pab - force last connection to close when starting new session
        '    If cnsetting.State = 1 Then cnsetting.Close()

        '    geturlaliasandconnections(host)
        '    Session("urlalias") = _urlalias

        'End If

        'If Session("username") Is Nothing Then
        '    FormsAuthentication.RedirectToLoginPage()
        '    '20130117 - pab - fix object not set
        '    Exit Sub
        'End If

        'Dim da As New DataAccess
        'Dim cc As String = da.GetSetting(_carrierid, "framecolor")
        'If cc <> "" Then
        '    '20160414 - pab - selworthy integration
        '    'gvRegionZoneRates.BorderColor = System.Drawing.ColorTranslator.FromHtml(cc)
        '    'gvRegionZoneRates.FooterStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
        '    'gvRegionZoneRates.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
        'End If

        'cc = da.GetSetting(_carrierid, "ButtonColor")
        'If cc <> "" Then
        '    '20160414 - pab - selworthy integration
        '    'Me.bttnSaveRegion.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
        '    ''Me.bttnDelete.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
        '    'Me.bttnCancel.BackColor = System.Drawing.ColorTranslator.FromHtml(cc)
        'End If

        'Dim bview As Boolean = da.GetUserSecurityByFunction(_carrierid, Session("username").ToString, "Flights", "VIEW")
        'Dim bedit As Boolean = da.GetUserSecurityByFunction(_carrierid, Session("username").ToString, "Flights", "EDIT")

        'If Not Page.IsPostBack Then

        '    '20160414 - pab - selworthy integration
        '    ''Me.lblRegion.Visible = True
        '    'Me.bttnAddRegion.Visible = True
        '    ''Me.ddlRegion.Visible = True

        '    'Me.pnlRegion.Visible = True

        '    If bview = False And bedit = False Then
        '        Me.lblMsg.Text = "You are not authorized to view/edit this page"
        '        Me.lblMsg.ForeColor = Drawing.Color.Red
        '        Me.lblMsg.Visible = True
        '        '20160414 - pab - selworthy integration
        '        ''Me.lblRegion.Visible = False
        '        'Me.bttnAddRegion.Visible = False
        '        ''Me.ddlRegion.Visible = False
        '        'Me.pnlRegion.Visible = False
        '        Exit Sub
        '    End If

        '    '20160414 - pab - selworthy integration
        '    'If bedit = False Then
        '    '    Me.bttnAddRegion.Visible = False
        '    '    Me.hddnMode.Value = "VIEW"
        '    'Else
        '    '    Me.hddnMode.Value = "EDIT"
        '    'End If

        '    'HydrateddlSearchDDLs()

        '    BindDataZoneRates(0)

        'End If

        'Me.lblMsg.Visible = False

    End Sub

    '20160414 - pab - selworthy integration
    'Private Sub HydrateddlSearchDDLs()

    '    Dim da As New DataAccess
    '    Dim dt As DataTable = da.GetPricingZones(_carrierid, 0)

    '    Dim li As ListItem = Nothing
    '    Dim prevFlightID As Integer = 0

    '    Me.ddlRegion.Items.Clear()
    '    Me.ddlFrom.Items.Clear()
    '    Me.ddlTo.Items.Clear()

    '    li = New ListItem("", "")

    '    Me.ddlRegion.Items.Add(li)
    '    Me.ddlFrom.Items.Add(li)
    '    Me.ddlTo.Items.Add(li)

    '    If dt.Rows.Count > 0 Then
    '        For Each dr As DataRow In dt.Rows
    '            li = New ListItem(dr("ZoneName").ToString, dr("ID").ToString)
    '            Me.ddlRegion.Items.Add(li)
    '            Me.ddlFrom.Items.Add(li)
    '            Me.ddlTo.Items.Add(li)
    '        Next
    '    End If

    '    dt = da.ListAircraftTypeServiceSpecs(_carrierid)

    '    Me.ddlACType.Items.Clear()

    '    li = New ListItem("", "")

    '    Me.ddlACType.Items.Add(li)

    '    For Each dr As DataRow In dt.Rows
    '        li = New ListItem(CStr(dr("Name")), CStr(dr("AircraftTypeServiceSpecID")))

    '        Me.ddlACType.Items.Add(li)
    '    Next


    '    'Me.ddlRegion.Visible = True

    'End Sub

    'Private Sub BindDataZoneRates(ID As Integer)

    '    Dim da As New DataAccess
    '    Dim dt As New DataTable
    '    Dim dtout As New DataTable
    '    Dim dc As DataColumn = Nothing
    '    Dim dr As DataRow = Nothing

    '    dt = da.GetPricingZoneRates(_carrierid, 0)

    '    '20160414 - pab - selworthy integration
    '    'If hddnMode.Value = "EDIT" Then
    '    '    Me.gvRegionZoneRates.AutoGenerateEditButton = True
    '    '    Me.gvRegionZoneRates.AutoGenerateDeleteButton = True
    '    'End If
    '    'Me.gvRegionZoneRates.DataSource = dt
    '    'Me.gvRegionZoneRates.DataBind()

    'End Sub

    '20160414 - pab - selworthy integration
    'Protected Sub ddlRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegion.SelectedIndexChanged

    '    Dim i As Integer = 0

    '    If IsNumeric(ddlRegion.SelectedValue.ToString) Then i = CInt(ddlRegion.SelectedValue.ToString)

    '    BindDataZoneRates(i)

    'End Sub

    '20160414 - pab - selworthy integration
    'Private Sub gvRegionZoneRates_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles gvRegionZoneRates.RowCancelingEdit

    '    bttnCancel_Click(sender, e)

    'End Sub

    '20160414 - pab - selworthy integration
    'Private Sub gvRegionZoneRates_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvRegionZoneRates.RowDeleting

    '    bttnCancel_Click(sender, e)

    'End Sub

    '20160414 - pab - selworthy integration
    'Private Sub gvRegionZoneRates_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles gvRegionZoneRates.RowEditing

    '    Dim da As New DataAccess

    '    'populate controls
    '    Dim keys As System.Collections.Specialized.IOrderedDictionary = Me.gvRegionZoneRates.DataKeys(e.NewEditIndex).Values
    '    Dim ID As String = keys.Item(0).ToString
    '    Dim ZoneName As String = keys.Item(1).ToString
    '    Dim ZoneDescription As String = keys.Item(2).ToString
    '    Dim dt As DataTable = da.GetPricingZoneRates(_carrierid, ID)
    '    Dim dr As DataRow

    '    Me.pnlAddEditRegion.Visible = True
    '    Me.bttnAddRegion.Visible = False
    '    Me.pnlRegion.Visible = False

    '    'Me.lblRegion.Visible = False
    '    'Me.ddlRegion.Visible = False
    '    Me.bttnAddRegion.Visible = False
    '    Me.lblMessage.Visible = False


    '    If dt.Rows.Count > 0 Then
    '        dr = dt.Rows(0)
    '        Me.ddlFrom.SelectedIndex = Me.ddlFrom.Items.IndexOf(Me.ddlFrom.Items.FindByValue(CStr(dr("ZoneNamefrom"))))
    '        Me.ddlTo.SelectedIndex = Me.ddlTo.Items.IndexOf(Me.ddlTo.Items.FindByValue(CStr(dr("ZoneNameto"))))
    '        Me.FromDate.SelectedDate = CDate(dr("startdate").ToString)
    '        Me.ToDate.SelectedDate = CDate(dr("enddate").ToString)
    '        Me.ddlACType.SelectedIndex = Me.ddlACType.Items.IndexOf(Me.ddlACType.Items.FindByValue(CStr(dr("actypeid"))))
    '        Me.tbPrice.Text = dr("rate").ToString
    '        Me.tbDiscount.Text = dr("discountpct").ToString
    '        Me.tbPremium.Text = dr("premiumpct").ToString
    '        Me.cblDays.SelectedIndex = 0

    '        Me.hddnID.Value = dr("ID").ToString
    '        Me.hddnRegion.Value = dr("ZoneNamefrom").ToString
    '    Else
    '        Me.ddlFrom.SelectedIndex = -1
    '        Me.ddlTo.SelectedIndex = -1
    '        Me.FromDate.SelectedDate = CDate(Now.ToShortDateString)
    '        Me.ToDate.SelectedDate = CDate(Now.ToShortDateString)
    '        Me.ddlACType.SelectedIndex = -1
    '        Me.tbPrice.Text = ""
    '        Me.tbDiscount.Text = ""
    '        Me.tbPremium.Text = ""
    '        Me.cblDays.SelectedIndex = 0

    '        Me.hddnID.Value = "0"
    '        Me.hddnRegion.Value = ""
    '    End If

    '    'Me.bttnDelete.Visible = True

    'End Sub

    '20160414 - pab - selworthy integration
    'Protected Sub bttnAddRegion_Click(sender As Object, e As EventArgs) Handles bttnAddRegion.Click

    '    Me.pnlAddEditRegion.Visible = True
    '    Me.bttnAddRegion.Visible = False

    '    Me.ddlFrom.SelectedIndex = -1
    '    Me.ddlTo.SelectedIndex = -1
    '    Me.FromDate.SelectedDate = CDate(Now.ToShortDateString)
    '    Me.ToDate.SelectedDate = CDate(Now.ToShortDateString)
    '    Me.ddlACType.SelectedIndex = -1
    '    Me.tbPrice.Text = ""
    '    Me.tbDiscount.Text = ""
    '    Me.tbPremium.Text = ""
    '    Me.cblDays.SelectedIndex = 0

    '    Me.hddnID.Value = "0"
    '    Me.hddnRegion.Value = ""

    '    'Me.bttnDelete.Visible = False

    'End Sub

    '20160414 - pab - selworthy integration
    'Protected Sub bttnSaveRegion_Click(sender As Object, e As EventArgs) Handles bttnSaveRegion.Click

    '    Dim da As New DataAccess
    '    Dim dt As New DataTable

    '    lblMessage.Text = ""
    '    lblMessage.Visible = False

    '    If tbRegion.Text.ToString = "" Then
    '        lblMessage.Text = "Please enter Region Name. "
    '    End If

    '    'If tbBaseAirport.Text.ToString = "" Then
    '    '    lblMessage.Text &= "Please enter Base Airport code. "
    '    'Else
    '    '    Select Case Len(tbBaseAirport.Text.ToString)
    '    '        Case 3
    '    '            dt = da.GetAirportInformationByLocationID(tbBaseAirport.Text.ToString)
    '    '            If isdtnullorempty(dt) Then
    '    '                lblMessage.Text &= "Please enter valid Base Airport code. "
    '    '            Else
    '    '                tbBaseAirport.Text = tbBaseAirport.Text.ToString.ToUpper
    '    '            End If

    '    '        Case 4
    '    '            dt = da.GetAirportInformationByICAO(tbBaseAirport.Text.ToString)
    '    '            If isdtnullorempty(dt) Then
    '    '                lblMessage.Text &= "Please enter valid Base Airport code. "
    '    '            Else
    '    '                If Not IsDBNull(dt.Rows(0).Item("locationid")) Then tbBaseAirport.Text = dt.Rows(0).Item("locationid")
    '    '            End If

    '    '        Case Else
    '    '            lblMessage.Text &= "Please enter valid Base Airport code. "
    '    '    End Select
    '    'End If

    '    'If tbRadiusNM.Text.ToString = "" Then
    '    '    lblMessage.Text &= "Please enter valid Radius. "
    '    'End If

    '    If lblMessage.Text <> "" Then
    '        lblMessage.ForeColor = Drawing.Color.Red
    '        lblMessage.Visible = True
    '        Exit Sub
    '    End If

    '    'add mode - check if user id in member table before adding function
    '    If hddnID.Value = "0" Then
    '        'inset
    '        'Me.hddnID.Value = da.InsertPricingZones(_carrierid, tbRegion.Text.ToString.Trim, Me.tbDescription.Text.ToString.Trim, _
    '        '    Me.tbBaseAirport.Text.ToString.Trim, CInt(Me.tbRadiusNM.Text.ToString.Trim))

    '    Else
    '        'update
    '        'If IsNumeric(Me.hddnID.Value) Then da.UpdatePricingZones(CInt(Me.hddnID.Value), _carrierid, tbRegion.Text.ToString.Trim, _
    '        '    Me.tbDescription.Text.ToString.Trim, Me.tbBaseAirport.Text.ToString.Trim, CInt(Me.tbRadiusNM.Text.ToString.Trim))

    '    End If

    '    lblMessage.Text = "Record saved"
    '    lblMessage.ForeColor = Drawing.Color.Black

    '    Me.ddlFrom.SelectedIndex = -1
    '    Me.ddlTo.SelectedIndex = -1
    '    Me.FromDate.SelectedDate = CDate(Now.ToShortDateString)
    '    Me.ToDate.SelectedDate = CDate(Now.ToShortDateString)
    '    Me.ddlACType.SelectedIndex = -1
    '    Me.tbPrice.Text = ""
    '    Me.tbDiscount.Text = ""
    '    Me.tbPremium.Text = ""
    '    Me.cblDays.SelectedIndex = 0

    '    Me.hddnID.Value = "0"
    '    Me.hddnRegion.Value = ""

    '    Me.pnlAddEditRegion.Visible = False
    '    Me.pnlRegion.Visible = True

    '    Me.bttnAddRegion.Visible = True

    '    Me.lblMessage.Visible = True
    '    Me.lblMessage.ForeColor = Drawing.Color.IndianRed

    '    HydrateddlSearchDDLs()

    '    Me.gvRegionZoneRates.EditIndex = -1
    '    BindDataZoneRates(0)

    'End Sub

    '20160414 - pab - selworthy integration
    'Protected Sub bttnCancel_Click(sender As Object, e As EventArgs) Handles bttnCancel.Click

    '    Dim da As New DataAccess

    '    Me.ddlFrom.SelectedIndex = -1
    '    Me.ddlTo.SelectedIndex = -1
    '    Me.FromDate.SelectedDate = CDate(Now.ToShortDateString)
    '    Me.ToDate.SelectedDate = CDate(Now.ToShortDateString)
    '    Me.ddlACType.SelectedIndex = -1
    '    Me.tbPrice.Text = ""
    '    Me.tbDiscount.Text = ""
    '    Me.tbPremium.Text = ""
    '    Me.cblDays.SelectedIndex = 0

    '    Me.hddnID.Value = "0"
    '    Me.hddnRegion.Value = ""

    '    Me.lblMessage.Visible = True

    '    Me.pnlAddEditRegion.Visible = False
    '    Me.pnlRegion.Visible = True

    '    Me.bttnAddRegion.Visible = True

    '    HydrateddlSearchDDLs()

    '    Me.gvRegionZoneRates.EditIndex = -1
    '    BindDataZoneRates(0)

    'End Sub

    Protected Sub find_button_Click(sender As Object, e As EventArgs) Handles find_button.Click

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0

        Dim s As String = "SELECT Id, z1.ZoneName as ZoneIDFrom, z2.ZoneName as ZoneIDTo, StartDate, EndDate, name, Rate, DiscountPCT, PremiumPCT, [DayofWeek], TimeofDay "
        s &= "FROM PricingZonesRates r join PricingZones z1 on r.ZoneIDFrom = z1.ZoneID join PricingZones z2 on r.ZoneIDTo = z2.ZoneID "
        s &= "join AircraftTypeServiceSpecs ss on r.CarrierID = ss.CarrierID and r.ACTypeID = ss.AircraftTypeServiceSpecID "

        If find_region_txt.Text.Trim <> "" And find_airport_txt.Text.Trim <> "" Then
            s &= "where z1.ZoneName >= '" & find_region_txt.Text.Trim & "' and name >= '" & find_airport_txt.Text.Trim & "' "
        ElseIf find_region_txt.Text.Trim <> "" Then
            s &= "where z1.ZoneName >= '" & find_region_txt.Text.Trim & "' "
        ElseIf find_airport_txt.Text.Trim <> "" Then
            s &= "where name >= '" & find_airport_txt.Text.Trim & "' "
        End If

        '20170429 - fix select - carrierid missing
        If InStr(s, "where") > 0 Then
            s &= "and r.carrierid = " & CInt(Session("carrierid"))
        Else
            s &= "where r.carrierid = " & CInt(Session("carrierid"))
        End If

        s &= " order by z1.Zonename, z2.Zonename, name, StartDate"

        Me.get_defineregions.SelectCommand = s
        Me.get_defineregions.DataBind()

        Me.table_main_gridview.DataSourceID = "get_defineregions"
        Me.table_main_gridview.DataBind()

    End Sub

    Private Sub table_main_gridview_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles table_main_gridview.RowEditing

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Dim da As New DataAccess
        Dim keys As System.Collections.Specialized.IOrderedDictionary = Me.table_main_gridview.DataKeys(e.NewEditIndex).Values
        Dim ID As Integer = CInt(keys.Item(0).ToString)
        'Dim ZoneName As String = keys.Item(1).ToString
        'Dim ZoneDescription As String = keys.Item(2).ToString
        'Dim BaseAirport As String = keys.Item(3).ToString
        'Dim RadiusNM As String = keys.Item(4).ToString

        'Dim ID As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ID"), TextBox)).Text
        'Dim ZoneName As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ZoneName"), TextBox)).Text
        'Dim ZoneDescription As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ZoneDescription"), TextBox)).Text
        'Dim BaseAirport As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("BaseAirport"), TextBox)).Text
        'Dim RadiusNM As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("RadiusNM"), TextBox)).Text

        Dim dt As DataTable = da.GetPricingZones(carrierid, 0)
        If Not isdtnullorempty(dt) Then
            Me.ddlFrom.Items.Clear()
            Me.ddlTo.Items.Clear()
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim ti As New Telerik.Web.UI.RadComboBoxItem
                ti.Text = Trim(dt.Rows(i).Item("zonename").ToString) & " (" & Trim(dt.Rows(i).Item("ZoneDescription").ToString) & ")"
                ti.Value = Trim(dt.Rows(i).Item("zonename").ToString)

                ddlFrom.Items.Add(ti)
            Next
            'Me.ddlFrom.Text = Trim(dt.Rows(10).Item("zonename").ToString) & " (" & Trim(dt.Rows(10).Item("ZoneDescription").ToString) & ")"

            'Me.OriginAddress2.Items.Clear()
            'For i As Integer = 0 To dt.Rows.Count - 1
            '    Dim ti As New Telerik.Web.UI.RadComboBoxItem
            '    ti.Text = Trim(dt.Rows(i).Item("zonename").ToString) & " (" & Trim(dt.Rows(i).Item("ZoneDescription").ToString) & ")"
            '    ti.Value = Trim(dt.Rows(i).Item("zonename").ToString)

            '    Me.OriginAddress2.Items.Add(ti)
            'Next
            'Me.OriginAddress2.Text = Trim(dt.Rows(0).Item("zonename").ToString) & " (" & Trim(dt.Rows(0).Item("ZoneDescription").ToString) & ")"

            For i As Integer = 0 To dt.Rows.Count - 1
                Dim ti As New Telerik.Web.UI.RadComboBoxItem
                ti.Text = Trim(dt.Rows(i).Item("zonename").ToString) & " (" & Trim(dt.Rows(i).Item("ZoneDescription").ToString) & ")"
                ti.Value = Trim(dt.Rows(i).Item("zonename").ToString)

                ddlTo.Items.Add(ti)
            Next
        End If

        If RadComboBoxACInclude.Items.Count = 0 Then
            dt = da.GetAircraftTypeServiceSpecsActive_2(carrierid, 0)
            If Not isdtnullorempty(dt) Then
                Me.RadComboBoxACInclude.Items.Clear()
                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim ti As New Telerik.Web.UI.RadComboBoxItem
                    ti.Text = Trim(dt.Rows(i).Item("Name").ToString)
                    ti.Value = Trim(dt.Rows(i).Item("AircraftTypeServiceSpecID").ToString)

                    RadComboBoxACInclude.Items.Add(ti)
                Next
            End If
        End If

        dt = da.GetPricingZoneRates(carrierid, ID)
        Dim dr As DataRow

        Me.pnlEdit.Visible = True

        'Dim ti2 As New Telerik.Web.UI.RadComboBoxItem
        'ti2.Text = "fxe"
        'ti2.Value = "fxe"
        'Me.OriginAddress2.Items.Add(ti2)
        'ti2.Text = "tpa"
        'ti2.Value = "tpa"
        'Me.OriginAddress2.Items.Add(ti2)
        'ti2.Text = "pns"
        'ti2.Value = "pns"
        'Me.OriginAddress2.Items.Add(ti2)
        'Me.OriginAddress2.Text = "fxe"

        If dt.Rows.Count > 0 Then
            dr = dt.Rows(0)
            'Me.ddlFrom.SelectedItem.Value = dr("ZoneNameFrom").ToString
            'Me.ddlTo.SelectedItem.Value = dr("ZoneNameTo").ToString
            'Me.ddlFrom.SelectedIndex = Me.ddlFrom.Items.IndexOf(Me.ddlFrom.Items.FindItemByValue(CStr(dr("ZoneNamefrom"))))
            Me.ddlFrom.Text = Trim(dr("zonenamefrom").ToString) '& " (" & Trim(dr("ZoneDescription").ToString) & ")"
            'If Me.ddlTo.SelectedIndex >= 0 Then Me.ddlTo.SelectedIndex = Me.ddlTo.Items.IndexOf(Me.ddlFrom.Items.FindItemByValue(CStr(dr("ZoneNameTo"))))
            Me.ddlTo.Text = Trim(dr("zonenameto").ToString) '& " (" & Trim(dt.Rows(0).Item("ZoneDescription").ToString) & ")"
            Me.start_date.SelectedDate = CDate(dr("StartDate").ToString).ToShortDateString
            Me.end_date.SelectedDate = CDate(dr("EndDate").ToString).ToShortDateString
            'Me.RadComboBoxACInclude.SelectedItem.Value = dr("name").ToString
            Me.RadComboBoxACInclude.SelectedIndex = Me.ddlTo.Items.IndexOf(Me.ddlFrom.Items.FindItemByText(CStr(dr("name"))))
            Me.RadComboBoxACInclude.Text = dr("name").ToString
            Me.edit_price_1.Text = dr("rate").ToString
            Me.edit_or_sidc_1.Text = dr("discountpct").ToString
            Me.edit_or_prem_1.Text = dr("premiumpct").ToString

            Dim sDays As String
            Dim aryDays() As String

            sDays = dr("dayofweek").ToString
            aryDays = Split(sDays, ",")

            ddlDayofWeek.ClearCheckedItems()
            For i As Integer = LBound(aryDays) To UBound(aryDays)
                For n As Integer = 0 To ddlDayofWeek.Items.Count - 1
                    If ddlDayofWeek.Items(n).Value = aryDays(i) Then
                        ddlDayofWeek.Items(n).Checked = True
                    End If
                Next
            Next
            'If dr("dayofweek").ToString = "" Or dr("dayofweek").ToString = "All" Then
            '    Me.ddlDayofWeek.Text = "All items checked"
            'Else
            '    Me.ddlDayofWeek.Text = dr("dayofweek").ToString
            'End If

            '20161017 - pab - time of day pricing
            'Me.ddlTimeFrom.SelectedValue = dr("starttime").ToString
            'Me.ddlTimeTo.SelectedValue = dr("endtime").ToString
            if dr("timeofday").ToString <> "" Then Me.ddlTimeofDay.SelectedValue = left(dr("timeofday").ToString, 1)

            Me.hddnID.Value = dr("ID").ToString
            '    Me.hddnAUTH_FUNC.Value = dr("AUTH_FUNC").ToString
            'Else
            '    Me.edit_from_1.Text = ID
            '    Me.edit_to_1.Text = AUTH_FUNC
            '    Me.edit_eff_1.Text = AUTH_FUNC
            '    Me.edit_disc_1.Text = AUTH_FUNC
            '    Me.hddnID.Value = "0"
            '    Me.hddnID.Value = ID
            '    Me.hddnAUTH_FUNC.Value = AUTH_FUNC
        End If

        'If Not IsNumeric(RadiusNM) Then lblMsg.Text = "Radius must be numeric"

        'Exit Sub

        'table_main_gridview.EditIndex = -1
        'table_main_gridview.DataBind()

    End Sub

    Private Sub update_popup_ok_Click(sender As Object, e As EventArgs) Handles update_popup_ok.Click

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Dim da As New DataAccess
        Dim dt As New DataTable
        Dim zonename As String = ""
        Dim zoneidfrom As Integer = 0
        Dim zoneidto As Integer = 0

        lblMsg.Text = ""
        lblMsg.Visible = False

        If ddlFrom.SelectedValue <> "" Then
            zonename = ddlFrom.SelectedValue
        Else
            If ddlFrom.Text.Trim <> "" Then
                zonename = ddlFrom.Text

                dt = da.GetPricingZonesByName(carrierid, zonename)
                If isdtnullorempty(dt) Then
                    lblMsg.Text &= "Invalid Region From. "
                Else
                    zoneidfrom = dt.Rows(0).Item("zoneid")
                End If
                If zoneidfrom = 0 Then
                    lblMsg.Text &= "Invalid Region From. "
                End If
            Else
                lblMsg.Text &= "Please select From Region Name. "
            End If
        End If

        zonename = ""
        If ddlTo.SelectedValue <> "" Then
            zonename = ddlTo.SelectedValue
        Else
            If ddlTo.Text.Trim <> "" Then
                zonename = ddlTo.Text

                dt = da.GetPricingZonesByName(carrierid, zonename)
                If isdtnullorempty(dt) Then
                    lblMsg.Text &= "Invalid Region To. "
                    Exit Sub
                Else
                    zoneidto = dt.Rows(0).Item("zoneid")
                End If
                If zoneidto = 0 Then
                    lblMsg.Text &= "Invalid Region To"
                ElseIf zoneidfrom = zoneidto Then
                    '20160916 - pab - add states drop down
                    'lblMsg.Text &= "From and To Region must be different"
                End If
            Else
                lblMsg.Text = "Please select To Region Description. "
            End If
        End If

        If start_date.SelectedDate > end_date.SelectedDate Then
            lblMsg.Text = "Effective Date must be prior to Discontinue Date"
            Exit Sub
        End If

        If start_date.SelectedDate.ToString = "" Then
            lblMsg.Text &= "Please enter Effective Date. "
        ElseIf Not IsDate(start_date.SelectedDate.ToString) Then
            lblMsg.Text &= "Please enter valid Effective Date. "
        End If

        If end_date.SelectedDate.ToString = "" Then
            lblMsg.Text &= "Please enter Discontinue Date. "
        ElseIf Not IsDate(end_date.SelectedDate.ToString) Then
            lblMsg.Text &= "Please enter valid Discontinue Date. "
        End If

        If start_date.SelectedDate > end_date.SelectedDate Then
            lblMsg.Text &= "Effective Date must be prior to Discontinue Date. "
        End If

        If edit_price_1.Text = "0.0000" And edit_or_sidc_1.Text = "0" And edit_or_prem_1.Text = "0" Then
            lblMsg.Text &= "Please enter Fixed Price, Dicount % or Premium %. "
        ElseIf edit_price_1.Text <> "0.0000" And (edit_or_sidc_1.Text <> "0" Or edit_or_prem_1.Text <> "0") Then
            lblMsg.Text &= "Please enter Fixed Price, Dicount % or Premium %. "
        ElseIf edit_or_sidc_1.Text <> "0" And (edit_price_1.Text <> "0.0000" Or edit_or_prem_1.Text <> "0") Then
            lblMsg.Text &= "Please enter Fixed Price, Dicount % or Premium %. "
        ElseIf edit_or_prem_1.Text <> "0" And (edit_or_sidc_1.Text <> "0" Or edit_price_1.Text <> "0.0000") Then
            lblMsg.Text &= "Please enter Fixed Price, Dicount % or Premium %. "
        ElseIf edit_price_1.Text <> "0.0000" Then
            If Not IsNumeric(edit_price_1.Text) Then
                lblMsg.Text &= "Please enter valid Fixed Price. "
            ElseIf CLng(edit_price_1.Text) <= 0 Then
                lblMsg.Text &= "Fixed Price must be greater than $0. "
            End If
        ElseIf edit_or_sidc_1.Text <> "0" Then
            If Not IsNumeric(edit_or_sidc_1.Text) Then
                lblMsg.Text &= "Please enter valid Discount %. "
            End If
        ElseIf edit_or_prem_1.Text <> "0" Then
            If Not IsNumeric(edit_or_prem_1.Text) Then
                lblMsg.Text &= "Please enter valid Premium %. "
            End If
        End If

        If RadComboBoxACInclude.SelectedValue = "" And RadComboBoxACInclude.Text = "" Then
            lblMsg.Text &= "Please select Fleet Type. "
        End If

        'Me.edit_dayweek_1.Text = dr("dayofweek").ToString

        If lblMsg.Text <> "" Then
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Visible = True
            Exit Sub
        End If

        Dim actypeid As Integer = 0
        If RadComboBoxACInclude.SelectedValue <> "" Then
            actypeid = CInt(RadComboBoxACInclude.SelectedValue)
        Else
            dt = da.GetAircraftTypeServiceSpecsActive_2(carrierid, 0)
            If Not isdtnullorempty(dt) Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    If dt.Rows(i).Item("Name").ToString = RadComboBoxACInclude.Text Then
                        actypeid = dt.Rows(i).Item("AircraftTypeServiceSpecID")
                        Exit For
                    End If
                Next
            End If
        End If

        Dim DayofWeek As String = ""
        'If ddlDayofWeek.SelectedValue <> "" Then
        '    DayofWeek = ddlDayofWeek.SelectedValue
        'ElseIf ddlDayofWeek.Text <> "" Then
        '    DayofWeek = ddlDayofWeek.Text
        'End If
        For i As Integer = 0 To ddlDayofWeek.Items.Count - 1
            If ddlDayofWeek.Items(i).Checked = True Then
                If DayofWeek <> "" Then DayofWeek &= ","
                DayofWeek &= ddlDayofWeek.Items(i).Value
            End If
        Next
        If DayofWeek = "Su,M,Tu,W,Th,F,Sa" Then DayofWeek = ""

        '20161017 - pab - time of day pricing
        'Dim StartTime As String = ""
        'Dim EndTime As String = ""
        'If ddlTimeFrom.SelectedIndex >= 0 Then
        '    StartTime = ddlTimeFrom.SelectedValue
        'End If
        'If ddlTimeTo.SelectedIndex >= 0 Then
        '    EndTime = ddlTimeTo.SelectedValue
        'End If
        'If StartTime <> "" Or EndTime <> "" Then
        '    If (StartTime <> "" And EndTime = "") Then
        '        lblMsg.Text &= "Please select End Time. "
        '    ElseIf (StartTime = "" And EndTime <> "" ) then
        '        lblMsg.Text &= "Please select Start Time. "
        '    ElseIf CDate(StartTime) >= CDate(EndTime) then
        '        lblMsg.Text &= "End Time must be after Start Time. "
        '    End If
        'End If
        Dim TimeofDay As String = ""
        If ddlTimeofDay.SelectedIndex >= 0 Then
            TimeofDay = ddlTimeofDay.SelectedValue
        End If

        '20161017 - pab - time of day pricing
        Dim bok As Boolean = da.UpdatePricingZonesRates(hddnID.Value, carrierid, actypeid, zoneidfrom,
            zoneidto, CDbl(edit_price_1.Text), CInt(edit_or_sidc_1.Text), CInt(edit_or_prem_1.Text), start_date.SelectedDate,
            end_date.SelectedDate, DayofWeek, TimeofDay)

        If bok = True Then
            lblMsg.Text = "Record Updated"
        Else
            lblMsg.Text = "Error Updating Record"
        End If
        'lblMsg.ForeColor = Drawing.Color.Black

        Me.ddlFrom.SelectedIndex = -1
        Me.ddlTo.SelectedIndex = -1
        Me.start_date.SelectedDate = Now.ToShortDateString
        Me.end_date.SelectedDate = Now.ToShortDateString
        Me.RadComboBoxACInclude.SelectedIndex = -1
        Me.edit_price_1.Text = ""
        Me.edit_or_sidc_1.Text = ""
        Me.edit_or_prem_1.Text = ""
        Me.ddlDayofWeek.SelectedIndex = -1
        Me.hddnID.Value = 0
        '20161017 - pab - time of day pricing
        'Me.ddlTimeFrom.ClearCheckedItems
        'Me.ddlTimeFrom.ClearSelection
        'Me.ddlTimeTo.ClearCheckedItems
        'Me.ddlTimeTo.ClearSelection
        Me.ddlTimeofDay.ClearSelection

        Me.pnlEdit.Visible = False

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Private Sub update_popup_cancel_Click(sender As Object, e As EventArgs) Handles update_popup_cancel.Click

        Me.ddlFrom.SelectedIndex = -1
        Me.ddlTo.SelectedIndex = -1
        Me.start_date.SelectedDate = Now.ToShortDateString
        Me.end_date.SelectedDate = Now.ToShortDateString
        Me.RadComboBoxACInclude.SelectedIndex = -1
        Me.edit_price_1.Text = ""
        Me.edit_or_sidc_1.Text = ""
        Me.edit_or_prem_1.Text = ""
        Me.ddlDayofWeek.SelectedIndex = -1
        Me.hddnID.Value = 0

        Me.lblMsg.Text = ""
        Me.lblMsg.Visible = False

        Me.pnlEdit.Visible = False

        'Me.bttnAddUser.Visible = True
        'Me.bttnCopyUser.Visible = True
        'Me.lblID.Visible = True
        'Me.ddlID.Visible = True

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Private Sub BindRegion(ID As Integer)

        Me.table_main_gridview.DataBind()

    End Sub

    Private Sub popup_add_Click(sender As Object, e As EventArgs) Handles popup_add.Click

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Dim da As New DataAccess
        Dim dt As New DataTable
        Dim zonename As String = ""
        Dim zoneidfrom As Integer = 0
        Dim zoneidto As Integer = 0

        lblMsg.Text = ""
        lblMsg.Visible = False

        If ddlFromAdd.SelectedValue <> "" Then
            zonename = ddlFromAdd.SelectedValue
        Else
            If ddlFromAdd.Text.Trim <> "" Then
                zonename = ddlFromAdd.Text

            Else
                lblMsg.Text &= "Please select From Region. "
            End If
        End If

        If zonename <> "" Then
            dt = da.GetPricingZonesByName(carrierid, zonename)
            If isdtnullorempty(dt) Then
                lblMsg.Text &= "Invalid Region From. "
            Else
                zoneidfrom = dt.Rows(0).Item("zoneid")
            End If
            If zoneidfrom = 0 Then
                lblMsg.Text &= "Invalid Region From. "
            End If

        End If

        zonename = ""
        If ddlToAdd.SelectedValue <> "" Then
            zonename = ddlToAdd.SelectedValue
        Else
            If ddlToAdd.Text.Trim <> "" Then
                zonename = ddlToAdd.Text

            Else
                lblMsg.Text &= "Please select To Region. "
            End If
        End If

        If zonename <> "" Then
            dt = da.GetPricingZonesByName(carrierid, zonename)
            If isdtnullorempty(dt) Then
                lblMsg.Text &= "Invalid Region To. "
            Else
                zoneidto = dt.Rows(0).Item("zoneid")
            End If
            If zoneidto = 0 Then
                lblMsg.Text &= "Invalid Region To. "
            ElseIf zoneidfrom = zoneidto Then
                '20160916 - pab - add states drop down
                'lblMsg.Text &= "From and To Region must be different. "
            End If

        End If

        If IsNothing(start_date_add.SelectedDate) Then
            lblMsg.Text &= "Plese select Effective Date. "
        End If

        If IsNothing(end_date_add.SelectedDate) Then
            lblMsg.Text &= "Plese select Discontinue Date. "
        End If

        If IsDate(start_date_add.SelectedDate) And IsDate(end_date_add.SelectedDate) And start_date_add.SelectedDate > end_date_add.SelectedDate Then
            lblMsg.Text &= "Effective Date must be prior to Discontinue Date. "
        End If

        If popup_fixedprice_txt.Text = "" Then popup_fixedprice_txt.Text = "0.0000"
        If popup_ordisc_txt.Text = "" Then popup_ordisc_txt.Text = "0"
        If popup_orprem_txt.Text = "" Then popup_orprem_txt.Text = "0"
        If popup_fixedprice_txt.Text = "0.0000" And popup_ordisc_txt.Text = "0" And popup_orprem_txt.Text = "0" Then
            lblMsg.Text &= "Please enter Fixed Price, Dicount % or Premium %. "
        ElseIf popup_fixedprice_txt.Text <> "0.0000" And (popup_ordisc_txt.Text <> "0" Or popup_orprem_txt.Text <> "0") Then
            lblMsg.Text &= "Please enter Fixed Price, Dicount % or Premium %. "
        ElseIf popup_ordisc_txt.Text <> "0" And (popup_fixedprice_txt.Text <> "0.0000" Or popup_orprem_txt.Text <> "0") Then
            lblMsg.Text &= "Please enter Fixed Price, Dicount % or Premium %. "
        ElseIf popup_orprem_txt.Text <> "0" And (popup_ordisc_txt.Text <> "0" Or popup_fixedprice_txt.Text <> "0.0000") Then
            lblMsg.Text &= "Please enter Fixed Price, Dicount % or Premium %. "
        ElseIf popup_fixedprice_txt.Text <> "0.0000" Then
            If Not IsNumeric(popup_fixedprice_txt.Text) Then
                lblMsg.Text &= "Please enter valid Fixed Price. "
            ElseIf CLng(popup_fixedprice_txt.Text) <= 0 Then
                lblMsg.Text &= "Fixed Price must be greater than $0. "
            End If
        ElseIf popup_ordisc_txt.Text <> "0" Then
            If Not IsNumeric(popup_ordisc_txt.Text) Then
                lblMsg.Text &= "Please enter valid Discount %. "
            End If
        ElseIf popup_orprem_txt.Text <> "0" Then
            If Not IsNumeric(popup_orprem_txt.Text) Then
                lblMsg.Text &= "Please enter valid Premium %. "
            End If
        End If

        If RadComboBoxfleet_add.SelectedValue = "" And RadComboBoxfleet_add.Text = "" Then
            lblMsg.Text &= "Please select Fleet Type. "
        End If

        'Me.edit_dayweek_1.Text = dr("dayofweek").ToString

        If lblMsg.Text <> "" Then
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Visible = True
            Exit Sub
        End If

        Dim actypeid As Integer = 0
        If RadComboBoxfleet_add.SelectedValue <> "" Then
            actypeid = CInt(RadComboBoxfleet_add.SelectedValue)
        Else
            dt = da.GetAircraftTypeServiceSpecsActive_2(carrierid, 0)
            If Not isdtnullorempty(dt) Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    If dt.Rows(i).Item("Name").ToString = RadComboBoxfleet_add.Text Then
                        actypeid = dt.Rows(i).Item("AircraftTypeServiceSpecID")
                        Exit For
                    End If
                Next
            End If
        End If

        Dim DayofWeek As String = ""
        'If ddlDayofWeekAdd.SelectedValue <> "" Then
        '    DayofWeek = ddlDayofWeekAdd.SelectedValue
        'ElseIf ddlDayofWeekAdd.Text <> "" Then
        '    DayofWeek = ddlDayofWeekAdd.Text
        'End If
        For i As Integer = 0 To ddlDayofWeekAdd.Items.Count - 1
            If ddlDayofWeekAdd.Items(i).Checked = True Then
                If DayofWeek <> "" Then DayofWeek &= ","
                DayofWeek &= ddlDayofWeekAdd.Items(i).Value
            End If
        Next
        If DayofWeek = "Su,M,Tu,W,Th,F,Sa" Then DayofWeek = ""

        '20161017 - pab - time of day pricing
        'Dim StartTime As String = ""
        'Dim EndTime As String = ""
        'If ddlTimeFromAdd.SelectedIndex >= 0 Then
        '    StartTime = ddlTimeFromAdd.SelectedValue
        'End If
        'If ddlTimeToAdd.SelectedIndex >= 0 Then
        '    EndTime = ddlTimeToAdd.SelectedValue
        'End If
        'If StartTime <> "" Or EndTime <> "" Then
        '    If (StartTime <> "" And EndTime = "") Then
        '        lblMsg.Text &= "Please select End Time. "
        '    ElseIf (StartTime = "" And EndTime <> "" ) then
        '        lblMsg.Text &= "Please select Start Time. "
        '    ElseIf CDate(StartTime) >= CDate(EndTime) then
        '        lblMsg.Text &= "End Time must be after Start Time. "
        '    End If
        'End If
        Dim TimeofDay As String = ""
        If ddlTimeofDayAdd.SelectedIndex >= 0 Then
            TimeofDay = ddlTimeofDayAdd.SelectedValue
        End If

        If lblMsg.Text <> "" Then
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Visible = True
            Exit Sub
        End If

        Dim ID As Integer = da.InsertPricingZonesRates(carrierid, RadComboBoxfleet_add.SelectedValue, zoneidfrom, zoneidto,
            CDbl(popup_fixedprice_txt.Text), CInt(popup_ordisc_txt.Text), CInt(popup_orprem_txt.Text), start_date_add.SelectedDate,
            end_date_add.SelectedDate, DayofWeek, TimeofDay)

        If ID > 0 Then
            lblMsg.Text = "Record Inserted"
        Else
            lblMsg.Text = "Error Inaserting Record"
        End If
        'lblMsg.ForeColor = Drawing.Color.Black

        Me.ddlFromAdd.ClearSelection()
        Me.ddlToAdd.ClearSelection()
        Me.start_date_add.SelectedDate = CDate(Now.ToShortDateString)
        Me.end_date_add.SelectedDate = CDate(Now.ToShortDateString)
        Me.RadComboBoxfleet_add.ClearSelection()
        Me.popup_fixedprice_txt.Text = ""
        Me.popup_ordisc_txt.Text = ""
        Me.popup_orprem_txt.Text = ""
        Me.hddnID.Value = "0"
        '20161017 - pab - time of day pricing
        'Me.ddlTimeFromAdd.ClearCheckedItems
        'Me.ddlTimeFromAdd.ClearSelection
        'Me.ddlTimeToAdd.ClearCheckedItems
        'Me.ddlTimeToAdd.ClearSelection
        Me.ddlTimeofDayAdd.ClearSelection

        Me.pnlEdit.Visible = False

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Private Sub popup_cancel_Click(sender As Object, e As EventArgs) Handles popup_cancel.Click

        Me.hddnID.Value = 0
        Me.ddlFromAdd.ClearSelection()
        Me.ddlToAdd.ClearSelection()
        Me.start_date_add.SelectedDate = CDate(Now.ToShortDateString)
        Me.end_date_add.SelectedDate = CDate(Now.ToShortDateString)
        Me.RadComboBoxfleet_add.ClearSelection()
        Me.popup_fixedprice_txt.Text = ""
        Me.popup_ordisc_txt.Text = ""
        Me.popup_orprem_txt.Text = ""
        Me.hddnID.Value = "0"

        Me.lblMsg.Text = ""
        Me.lblMsg.Visible = False

        'Me.pnlEdit.Visible = False

        'Me.bttnAddUser.Visible = True
        'Me.bttnCopyUser.Visible = True
        'Me.lblID.Visible = True
        'Me.ddlID.Visible = True

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    '20161025 - pab - add delete function
    Protected Sub update_popup_delete_Click(sender As Object, e As EventArgs) Handles update_popup_delete.Click

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Dim bok As Boolean = DataAccess.DeletePricingZonesRates(carrierid, Me.hddnID.Value)

        If bok = True Then
            lblMsg.Text = "Record Deleted"
        Else
            lblMsg.Text = "Error Deleting Record"
        End If
        'lblMsg.ForeColor = Drawing.Color.Black

        Me.ddlFrom.SelectedIndex = -1
        Me.ddlTo.SelectedIndex = -1
        Me.start_date.SelectedDate = Now.ToShortDateString
        Me.end_date.SelectedDate = Now.ToShortDateString
        Me.RadComboBoxACInclude.SelectedIndex = -1
        Me.edit_price_1.Text = ""
        Me.edit_or_sidc_1.Text = ""
        Me.edit_or_prem_1.Text = ""
        Me.ddlDayofWeek.SelectedIndex = -1
        Me.hddnID.Value = 0
        '20161017 - pab - time of day pricing
        'Me.ddlTimeFrom.ClearCheckedItems
        'Me.ddlTimeFrom.ClearSelection
        'Me.ddlTimeTo.ClearCheckedItems
        'Me.ddlTimeTo.ClearSelection
        Me.ddlTimeofDay.ClearSelection()

        Me.pnlEdit.Visible = False

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Private Sub special_pricing_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        '20161227 - pab - dynamic carrier images
        If Not IsPostBack Then
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Dim da As New DataAccess
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
        End If

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

    '20171101 - pab - display cleanup
    Protected Sub LinkLogOut_Click(sender As Object, e As EventArgs) Handles LinkLogOut.Click

        logout()

    End Sub

    '20171101 - pab - display cleanup
    Private Sub LinkLogOut2_Click(sender As Object, e As EventArgs) Handles LinkLogOut2.Click

        logout()

    End Sub

    Private Sub table_main_gridview_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles table_main_gridview.RowCancelingEdit

        update_popup_cancel_Click(sender, e)

    End Sub

    Private Sub table_main_gridview_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles table_main_gridview.RowUpdating

        update_popup_ok_Click(sender, e)

    End Sub

End Class
