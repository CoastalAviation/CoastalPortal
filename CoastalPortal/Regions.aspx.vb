Imports CoastalPortal.AirTaxi

Public Class define_regions
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("email") Is Nothing Then
            'Session.Abandon()
            Response.Redirect("CustomerLogin.aspx")

        End If

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0

        '20160916 - pab - add states drop down
        If Not Page.IsPostBack Then
            'popuate dropdowns
            Dim da As New DataAccess
            Dim dt As DataTable

            If RadComboBoxStates.Items.Count < 10 Then
                dt = da.GetValues(CInt(Session("carrierid")), "ddlStates")

                If Not isdtnullorempty(dt) Then
                    RadComboBoxStates.Items.Clear
                    RadComboBoxStates1.Items.Clear

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim ti As New Telerik.Web.UI.RadComboBoxItem

                        'ignore blank and us territories
                        If Trim(dt.Rows(i).Item("value").ToString) <> "" And Trim(dt.Rows(i).Item("value").ToString) <> "GU" And
                                Trim(dt.Rows(i).Item("value").ToString) <> "AS" And Trim(dt.Rows(i).Item("value").ToString) <> "FM" And
                                Trim(dt.Rows(i).Item("value").ToString) <> "MH" And Trim(dt.Rows(i).Item("value").ToString) <> "MP" And
                                Trim(dt.Rows(i).Item("value").ToString) <> "PW" And Trim(dt.Rows(i).Item("value").ToString) <> "PR" And
                                Trim(dt.Rows(i).Item("value").ToString) <> "VI" Then
                            ti.Text = Trim(dt.Rows(i).Item("description").ToString)
                            ti.Value = Trim(dt.Rows(i).Item("value").ToString)

                            '20161015 - pab - fix state drop downs - don't know why this stopped working
                            'RadComboBoxStates.Items.Add(ti)
                            'RadComboBoxStates1.Items.Add(ti)
                            RadComboBoxStates.Items.Add(New Telerik.Web.UI.RadComboBoxItem(Trim(dt.Rows(i).Item("description").ToString), Trim(dt.Rows(i).Item("value").ToString)))
                            RadComboBoxStates1.Items.Add(New Telerik.Web.UI.RadComboBoxItem(Trim(dt.Rows(i).Item("description").ToString), Trim(dt.Rows(i).Item("value").ToString)))                            'Try

                        End If
                    Next

                End If
            End If

            '20170429 - fix select - carrierid missing
            find_button_Click(sender, e)

        End If

    End Sub

    Protected Sub find_button_Click(sender As Object, e As EventArgs) Handles find_button.Click

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Dim s As String = "SELECT ZoneID, ZoneName, ZoneDescription, BaseAirport, facilityname, RadiusNM FROM PricingZones z "
        s &= "left join NfdcFacilities f on z.BaseAirport = f.locationid "

        If find_region_txt.Text.Trim <> "" And find_airport_txt.Text.Trim <> "" Then
            s &= "where ZoneName >= '" & find_region_txt.Text.Trim & "' and BaseAirport >= '" & find_airport_txt.Text.Trim & "' "
        ElseIf find_region_txt.Text.Trim <> "" Then
            s &= "where ZoneName >= '" & find_region_txt.Text.Trim & "' "
        ElseIf find_airport_txt.Text.Trim <> "" Then
            s &= "where BaseAirport >= '" & find_airport_txt.Text.Trim & "' "
        End If
        '20170429 - fix select - carrierid missing
        If InStr(s, "where") > 0 Then
            s &= "and carrierid = " & carrierid
        Else
            s &= "where carrierid = " & carrierid
        End If
        s &= " order by ZoneName"

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
        Dim ZoneID As Integer = CInt(keys.Item(0).ToString)
        Dim ZoneName As String = keys.Item(1).ToString
        Dim ZoneDescription As String = keys.Item(2).ToString
        Dim BaseAirport As String = keys.Item(3).ToString
        Dim RadiusNM As String = keys.Item(4).ToString

        'Dim ZoneID As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ZoneID"), TextBox)).Text
        'Dim ZoneName As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ZoneName"), TextBox)).Text
        'Dim ZoneDescription As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ZoneDescription"), TextBox)).Text
        'Dim BaseAirport As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("BaseAirport"), TextBox)).Text
        'Dim RadiusNM As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("RadiusNM"), TextBox)).Text

        Dim dt As DataTable = da.GetPricingZones(carrierid, ZoneID)
        Dim dt2 As DataTable
        Dim dr As DataRow

        Me.pnlEdit.Visible = True

        If dt.Rows.Count > 0 Then
            dr = dt.Rows(0)
            Me.edit_region_name_1.Text = dr("ZoneName").ToString
            Me.edit_description_1.Text = dr("ZoneDescription").ToString
            Me.edit_airport_code_1.Text = dr("BaseAirport").ToString
            Me.edit_radius_1.Text = dr("RadiusNM").ToString
            '    Me.hddnID.Value = dr("ID").ToString
            Me.hddnZoneID.Value = dr("ZoneID").ToString
            '    Me.hddnAUTH_FUNC.Value = dr("AUTH_FUNC").ToString
            'Else
            '    Me.edit_region_name_1.Text = ZoneID
            '    Me.edit_description_1.Text = AUTH_FUNC
            '    Me.edit_airport_code_1.Text = AUTH_FUNC
            '    Me.edit_radius_1.Text = AUTH_FUNC
            '    Me.hddnID.Value = "0"
            '    Me.hddnZoneID.Value = ZoneID
            '    Me.hddnAUTH_FUNC.Value = AUTH_FUNC

            '20160916 - pab - add states drop down
            If dr("RadiusNM") = 0 Then
                dt2 = da.GetPricingZonesStates(carrierid, dr("ZoneID"))
                If Not isdtnullorempty(dt2) Then
                    For i As Integer = 0 To dt2.Rows.Count - 1
                        RadComboBoxStates1.SelectedValue = dt2.Rows(i).Item("state")
                        RadComboBoxStates1.SelectedItem.Checked = True
                    Next
                End If
            End If
        End If

        'If Not IsNumeric(RadiusNM) Then lblMsg.Text = "Radius must be numeric"

        'Exit Sub

        'table_main_gridview.EditIndex = -1
        'table_main_gridview.DataBind()

    End Sub

    Private Sub table_main_gridview_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles table_main_gridview.RowUpdating

        update_popup_ok_Click(sender, e)

        ''20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        'If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        'Dim carrierid As Integer = CInt(Session("carrierid"))

        'Dim da As New DataAccess
        'Dim keys As System.Collections.Specialized.IOrderedDictionary = Me.table_main_gridview.DataKeys(e.RowIndex).Values
        'Dim ZoneID As Integer = CInt(keys.Item(0).ToString)
        'Dim ZoneName As String = keys.Item(1).ToString
        'Dim ZoneDescription As String = keys.Item(2).ToString
        'Dim BaseAirport As String = keys.Item(3).ToString
        'Dim RadiusNM As String = keys.Item(4).ToString

        ''Dim ZoneID As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ZoneID"), TextBox)).Text
        ''Dim ZoneName As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ZoneName"), TextBox)).Text
        ''Dim ZoneDescription As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("ZoneDescription"), TextBox)).Text
        ''Dim BaseAirport As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("BaseAirport"), TextBox)).Text
        ''Dim RadiusNM As String = (CType(table_main_gridview.Rows(e.RowIndex).FindControl("RadiusNM"), TextBox)).Text

        'Dim dt As DataTable = da.GetUserSecurityByID(carrierid, ID)
        'Dim dr As DataRow

        'Me.pnlEdit.Visible = True

        'If dt.Rows.Count > 0 Then
        '    dr = dt.Rows(0)
        '    Me.edit_region_name_1.Text = dr("ZoneName").ToString
        '    Me.edit_description_1.Text = dr("ZoneDescription").ToString
        '    Me.edit_airport_code_1.Text = dr("BaseAirport").ToString
        '    Me.edit_radius_1.Text = dr("RadiusNM").ToString
        '    '    Me.hddnID.Value = dr("ID").ToString
        '    '    Me.hddnZoneID.Value = dr("ZoneID").ToString
        '    '    Me.hddnAUTH_FUNC.Value = dr("AUTH_FUNC").ToString
        '    'Else
        '    '    Me.edit_region_name_1.Text = ZoneID
        '    '    Me.edit_description_1.Text = AUTH_FUNC
        '    '    Me.edit_airport_code_1.Text = AUTH_FUNC
        '    '    Me.edit_radius_1.Text = AUTH_FUNC
        '    '    Me.hddnID.Value = "0"
        '    '    Me.hddnZoneID.Value = ZoneID
        '    '    Me.hddnAUTH_FUNC.Value = AUTH_FUNC
        'End If

        ''If Not IsNumeric(RadiusNM) Then lblMsg.Text = "Radius must be numeric"

        ''Exit Sub

        ''table_main_gridview.EditIndex = -1
        ''table_main_gridview.DataBind()

    End Sub

    Protected Sub update_popup_ok_Click(sender As Object, e As EventArgs) Handles update_popup_ok.Click

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Dim da As New DataAccess
        Dim dt As New DataTable

        lblMsg.Text = ""
        lblMsg.Visible = False

        If edit_region_name_1.Text.ToString.Trim = "" Then
            lblMsg.Text &= "Please enter Region Name. "
        End If

        dt = da.GetPricingZonesByName(carrierid, edit_region_name_1.Text.ToString.Trim)
        If Not isdtnullorempty(dt) Then
            If dt.Rows(0).Item("zoneid") <> hddnZoneID.Value then
                lblMsg.Text = "Region Name already exists. Enter new Region Name. "
            End If
        End If

        If edit_description_1.Text.ToString.Trim = "" Then
            lblMsg.Text = "Please enter Region Description. "
        End If

        'If edit_description_1.Text <> "" Then
        '    dt = da.GetUserSecurity(_carrierid, edit_description_1.Text)
        '    If dt.Rows.Count > 0 Then
        '        lblMsg.Text &= "New User ID already exists. "
        '    End If
        'End If

        If edit_airport_code_1.Text.Trim = "" Then
            lblMsg.Text &= "Please enter Airport Code. "
        Else
            dt = da.GetAirportInformationByLocationID(edit_airport_code_1.Text.ToString.Trim)
            If isdtnullorempty(dt) Then
                dt = da.GetAirportInformationByLocationID(edit_airport_code_1.Text.ToString.Trim)
                If isdtnullorempty(dt) Then
                    'check icao
                    dt = da.GetAirportInformationByICAO(edit_airport_code_1.Text.ToString.Trim)
                    If isdtnullorempty(dt) Then
                        'check icao
                        lblMsg.Text &= "Please enter valid Airport code. "
                    Else
                        edit_airport_code_1.Text = dt.Rows(0).Item("locationid")
                    End If
                End If
            End If
        End If

        '20160916 - pab - add states drop down
        'If edit_radius_1.Text = "" Then
        '    lblMsg.Text &= "Please enter Radius. "
        'ElseIf Not IsNumeric(edit_radius_1.Text) Then
        '    lblMsg.Text &= "Radius must be numeric. "
        'End If
        If edit_radius_1.Text = "" And RadComboBoxStates1.CheckedItems.Count = 0 Then
            lblMsg.Text &= "Please enter Radius OR select State(s). "
        ElseIf edit_radius_1.Text <> "" and edit_radius_1.Text <> "0" And RadComboBoxStates1.CheckedItems.Count > 0 Then
            lblMsg.Text &= "Either Enter Radius OR select State(s). "
        ElseIf edit_radius_1.Text <> "" And Not IsNumeric(edit_radius_1.Text) Then
            lblMsg.Text &= "Radius must be numeric. "
        End If

        If lblMsg.Text <> "" Then
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Visible = True
            Exit Sub
        End If

        Dim RadiusNM As Integer = 0
        If IsNumeric(edit_radius_1.Text) Then RadiusNM = cint(edit_radius_1.Text)

        '20160919 - pab - convert to statute miles for insert
        Dim RadiusSM As Integer = CInt(RadiusNM * 1.15078)

        Dim bok As Boolean = da.UpdatePricingZones(hddnZoneID.Value, carrierid, edit_region_name_1.Text.ToString, edit_description_1.Text,
                edit_airport_code_1.Text.ToUpper, RadiusNM)

        If bok = True Then
            bok = da.DeletePricingZonesAirports(carrierid, CInt(hddnZoneID.Value))
            bok = da.DeletePricingZonesStates(carrierid, CInt(hddnZoneID.Value))

            '20160916 - pab - add states drop down
            Dim id As Integer = 0

            If RadiusNM > 0 Then
                id = da.InsertPricingZonesAirports(carrierid, CInt(hddnZoneID.Value), edit_airport_code_1.Text.ToUpper, RadiusSM)
                If id > 0 Then
                    lblMsg.Text = "Record Updated"
                Else
                    lblMsg.Text = "Error Updating Record"
                End If
            Else
                For i As Integer = 0 To RadComboBoxStates1.CheckedItems.Count - 1
                    id = da.InsertPricingZonesStates(carrierid, CInt(hddnZoneID.Value), RadComboBoxStates1.CheckedItems(i).Value)
                    If id >= 0 Then
                        lblMsg.Text = "Record(s) Updated"
                    Else
                        lblMsg.Text = "Error Updating Record"
                        Exit For
                    End If
                Next
            End If

        Else
            lblMsg.Text = "Error Updating Record"
        End If
        'lblMsg.ForeColor = Drawing.Color.Black

        Me.edit_region_name_1.Text = ""
        Me.edit_description_1.Text = ""
        Me.edit_airport_code_1.Text = ""
        Me.edit_radius_1.Text = ""
        'Me.hddnID.Value = "0"
        'Me.hddnUSER_ID.Value = ""
        'Me.hddnAUTH_FUNC.Value = ""

        '20160916 - pab - add states drop down
        RadComboBoxStates1.ClearCheckedItems
        RadComboBoxStates1.ClearSelection

        Me.pnlEdit.Visible = False

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Protected Sub update_popup_cancel_Click(sender As Object, e As EventArgs) Handles update_popup_cancel.Click

        Me.hddnZoneID.Value = 0
        Me.edit_region_name_1.Text = ""
        Me.edit_description_1.Text = ""
        Me.edit_airport_code_1.Text = ""
        Me.edit_radius_1.Text = ""
        'Me.hddnID.Value = "0"
        'Me.hddnUSER_ID.Value = ""
        'Me.hddnAUTH_FUNC.Value = ""

        Me.lblMsg.Text = ""
        Me.lblMsg.Visible = False

        Me.pnledit.Visible = False

        'Me.bttnAddUser.Visible = True
        'Me.bttnCopyUser.Visible = True
        'Me.lblZoneID.Visible = True
        'Me.ddlZoneID.Visible = True

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Private Sub BindRegion(ZoneID As Integer)

        Me.table_main_gridview.DataBind()

    End Sub

    Private Sub popup_add_Click(sender As Object, e As EventArgs) Handles popup_add.Click

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Dim da As New DataAccess
        Dim dt As New DataTable

        lblMsg.Text = ""
        lblMsg.Visible = False

        If popup_regionname_txt.Text.ToString.Trim = "" Then
            lblMsg.Text &= "Please enter Region Name. "
        End If

        dt = da.GetPricingZonesByName(carrierid, popup_regionname_txt.Text.ToString.Trim)
        If Not isdtnullorempty(dt) Then
            lblMsg.Text = "Region Name already exists. Enter new Region Name. "
        End If

        If popup_rdescription_txt.Text.ToString.Trim = "" Then
            lblMsg.Text = "Please enter Region Description. "
        End If

        'If popup_rdescription_txt.Text <> "" Then
        '    dt = da.GetUserSecurity(_carrierid, popup_rdescription_txt.Text)
        '    If dt.Rows.Count > 0 Then
        '        lblMsg.Text &= "New User ID already exists. "
        '    End If
        'End If

        If popup_airportcode_txt.Text.Trim = "" Then
            lblMsg.Text &= "Please enter Airport Code. "
        Else
            dt = da.GetAirportInformationByLocationID(popup_airportcode_txt.Text.ToString.Trim)
            If isdtnullorempty(dt) Then
                'check icao
                dt = da.GetAirportInformationByICAO(popup_airportcode_txt.Text.ToString.Trim)
                If isdtnullorempty(dt) Then
                    'check icao
                    lblMsg.Text &= "Please enter valid Airport code. "
                Else
                    popup_airportcode_txt.Text = dt.Rows(0).Item("locationid")
                End If
            End If
        End If

        '20160916 - pab - add states drop down
        If popup_radius_txt.Text = "" And RadComboBoxStates.CheckedItems.Count = 0 Then
            lblMsg.Text &= "Please enter Radius OR select State(s). "
        ElseIf popup_radius_txt.Text <> "" And RadComboBoxStates.CheckedItems.Count > 0 Then
            lblMsg.Text &= "Either Enter Radius OR select State(s). "
        ElseIf popup_radius_txt.Text <> "" And Not IsNumeric(popup_radius_txt.Text) Then
            lblMsg.Text &= "Radius must be numeric. "
        End If
        'If popup_radius_txt.Text = "" Then
        '    lblMsg.Text &= "Please enter Radius. "
        'ElseIf Not IsNumeric(popup_radius_txt.Text) Then
        '    lblMsg.Text &= "Radius must be numeric. "
        'End If

        If lblMsg.Text <> "" Then
            lblMsg.ForeColor = Drawing.Color.Red
            lblMsg.Visible = True
            Exit Sub
        End If

        Dim RadiusNM As Integer = 0
        If IsNumeric(edit_radius_1.Text) Then RadiusNM = cint(edit_radius_1.Text)

        '20160919 - pab - convert to statute miles for insert
        Dim RadiusSM As Integer = CInt(RadiusNM * 1.15078)

        Dim zoneid As Integer = da.InsertPricingZones(carrierid, popup_regionname_txt.Text.ToString, popup_rdescription_txt.Text,
                                                      popup_airportcode_txt.Text.ToUpper, RadiusNM)

        If zoneid > 0 Then
            '20160916 - pab - add states drop down
            Dim id As Integer = 0

            If RadiusNM > 0 Then
                id = da.InsertPricingZonesAirports(carrierid, zoneid, popup_airportcode_txt.Text.ToUpper, RadiusSM)
                If id > 0 Then
                    lblMsg.Text = "Record Inserted"
                Else
                    lblMsg.Text = "Error Inaserting Record"
                End If
            Else
                For i As Integer = 0 To RadComboBoxStates.CheckedItems.Count - 1
                    id = da.InsertPricingZonesStates(carrierid, zoneid, RadComboBoxStates.CheckedItems(i).Value)
                    If id >= 0 Then
                        lblMsg.Text = "Record(s) Inserted"
                    Else
                        lblMsg.Text = "Error Inaserting Record"
                        Exit For
                    End If
                Next
            End If
        Else
            lblMsg.Text = "Error Inaserting Record"
        End If
        'lblMsg.ForeColor = Drawing.Color.Black

        Me.popup_regionname_txt.Text = ""
        Me.popup_rdescription_txt.Text = ""
        Me.popup_airportcode_txt.Text = ""
        Me.popup_radius_txt.Text = ""
        'Me.hddnID.Value = "0"
        'Me.hddnUSER_ID.Value = ""
        'Me.hddnAUTH_FUNC.Value = ""

        '20160916 - pab - add states drop down
        RadComboBoxStates.ClearCheckedItems
        RadComboBoxStates.ClearSelection

        Me.pnlEdit.Visible = False

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Private Sub popup_cancel_Click(sender As Object, e As EventArgs) Handles popup_cancel.Click

        Me.hddnZoneID.Value = 0
        Me.popup_regionname_txt.Text = ""
        Me.popup_rdescription_txt.Text = ""
        Me.popup_airportcode_txt.Text = ""
        Me.popup_radius_txt.Text = ""
        'Me.hddnID.Value = "0"
        'Me.hddnUSER_ID.Value = ""
        'Me.hddnAUTH_FUNC.Value = ""

        Me.lblMsg.Text = ""
        Me.lblMsg.Visible = False

        'Me.pnlEdit.Visible = False

        'Me.bttnAddUser.Visible = True
        'Me.bttnCopyUser.Visible = True
        'Me.lblZoneID.Visible = True
        'Me.ddlZoneID.Visible = True

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Protected Sub update_popup_delete_Click(sender As Object, e As EventArgs) Handles update_popup_delete.Click

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If Session("carrierid") Is Nothing Then Session("carrierid") = 0
        Dim carrierid As Integer = CInt(Session("carrierid"))

        Dim da As New DataAccess
        Dim bok As Boolean = da.DeletePricingZones(carrierid, hddnZoneID.Value)

        If bok = True Then
            lblMsg.Text = "Record(s) Deleted"

            bok = da.DeletePricingZonesAirports(carrierid, CInt(hddnZoneID.Value))
            If bok = False Then lblMsg.Text = "Error Deleting Record(s)"

            bok = da.DeletePricingZonesStates(carrierid, CInt(hddnZoneID.Value))
            If bok = False Then lblMsg.Text = "Error Deleting Record(s)"

            bok = da.DeletePricingZonesRatesAllByID(carrierid, CInt(hddnZoneID.Value), CInt(hddnZoneID.Value))
            If bok = False Then lblMsg.Text = "Error Deleting Record(s)"

        Else
            lblMsg.Text = "Error Deleting Record"
        End If
        'lblMsg.ForeColor = Drawing.Color.Black

        Me.edit_region_name_1.Text = ""
        Me.edit_description_1.Text = ""
        Me.edit_airport_code_1.Text = ""
        Me.edit_radius_1.Text = ""
        'Me.hddnID.Value = "0"
        'Me.hddnUSER_ID.Value = ""
        'Me.hddnAUTH_FUNC.Value = ""

        '20160916 - pab - add states drop down
        RadComboBoxStates1.ClearCheckedItems()
        RadComboBoxStates1.ClearSelection()

        Me.pnlEdit.Visible = False

        Me.table_main_gridview.EditIndex = -1
        BindRegion(0)

    End Sub

    Private Sub define_regions_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

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
    Private Sub LinkLogOut_Click(sender As Object, e As EventArgs) Handles LinkLogOut.Click

        logout()

    End Sub

    '20171101 - pab - display cleanup
    Private Sub LinkLogOut2_Click(sender As Object, e As EventArgs) Handles LinkLogOut2.Click

        logout()

    End Sub

    Private Sub table_main_gridview_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles table_main_gridview.RowCancelingEdit

        update_popup_cancel_Click(sender, e)

    End Sub

End Class