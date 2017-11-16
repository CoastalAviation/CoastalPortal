Imports Telerik.Web.UI
Imports System.Data.SqlClient
Imports System.Data
Imports CoastalPortal.Mapping
Imports CoastalPortal.AirTaxi
'20140224 - pab - add threading for airport drop downs
Imports System.Threading
Imports System.Drawing

Public Class home2
    Inherits System.Web.UI.Page

    '20160117 - pab - quote multi-leg trips
    Private trip As ArrayList
    Private Structure leg
        Public fromloc As String
        Public toloc As String
        Public depart As Date
    End Structure

    '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
    Private TurnAroundMinutes As Integer

    ''20161028 - drive time
    'Private latLongorig As LatLong
    'Private latLongdest As LatLong

    '20160125 - pab - fix airport dropdowns
    'Sub loadairports(ByVal airport As String, ByVal comboname As String)

    '    Try
    '        Dim miles As Integer = 0
    '        Dim airportcount As Integer = 0
    '        Dim MinRunwayLength As Integer = 2800
    '        Dim fromloc As String = String.Empty
    '        Dim dt As New DataTable()
    '        Dim da As New DataAccess
    '        Dim ds As New DataSet
    '        Dim slocations As String = String.Empty

    '        AirTaxi.post_timing("home.aspx.vb loadairports start  " & Now.ToString)

    '        miles = DataAccess.getsettingnumeric(_carrierid, "AirportDistance")
    '        airportcount = DataAccess.getsettingnumeric(_carrierid, "AirportListCount")
    '        MinRunwayLength = DataAccess.getsettingnumeric(_carrierid, "MinRunwayLength")

    '        If miles = 0 Then miles = 50 '25 mi radius
    '        If airportcount = 0 Then airportcount = 25
    '        If MinRunwayLength = 0 Then MinRunwayLength = 2800

    '        If comboname = "OriginAddress" Then
    '            Me.OriginAddress.Items.Clear()
    '        ElseIf comboname = "DestinationAddress" Then
    '            Me.DestinationAddress.Items.Clear()
    '        End If

    '        Dim adapter As New SqlDataAdapter("", ConnectionStringHelper.GetCASConnectionString)

    '        fromloc = airport.ToUpper

    '        Select Case Len(airport)
    '            Case 1, 2
    '                'too short to look up
    '                Exit Sub

    '            Case 3
    '                '20161031 - pab - drive time
    '                adapter.SelectCommand.CommandText = ("SELECT  rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, " &
    '                    "rtrim(icao) as icao, rtrim(iata) as locationid , 0 as miles,cast(lat as float) as latitude,cast(long as float) as longitude from ICAO_IATA_2 i WHERE iata LIKE @text + '%'")

    '                adapter.SelectCommand.Parameters.AddWithValue("@text", fromloc)
    '                adapter.Fill(dt)

    '            Case 4
    '                '                   	SELECT rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, rtrim(icao) as icao, rtrim(iata) as locationid , 0 as miles 
    '                'from ICAO_IATA_2 i 
    '                'WHERE icao = @text 
    '                If Left(airport, 1).ToUpper = "K" Then
    '                    '20161031 - pab - drive time
    '                    adapter.SelectCommand.CommandText = ("SELECT  rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, " &
    '                    "rtrim(icao) as icao, rtrim(iata) as locationid , 0 as miles,cast(lat as float) as latitude,cast(long as float) as longitude from ICAO_IATA_2 WHERE icao  = @text")
    '                Else
    '                    '20160228 - pab - add more caribbean countries
    '                    '20161025 - pab - add bermuda
    '                    '20161031 - pab - drive time
    '                    adapter.SelectCommand.CommandText = ("SELECT  rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, " &
    '                      "rtrim(icao) as icao, rtrim(iata) as locationid , 0 as miles,cast(lat as float) as latitude,cast(long as float) as longitude from ICAO_IATA_2 i left join NfdcFacilities nf on i.iata = nf.locationid " &
    '                      "WHERE (icao  LIKE '%' + @text + '%' OR iata LIKE @text + '%' OR name LIKE @text + '%') and iata <> ''  and (region <> 'INTL' or " &
    '                      "country in ('bahamas','VG','VI','BS','Jamaica','AN','AG','PR','CAN','CA','DO','HN','HT','JM','KY','TC','AG','AB','AN','GP','KN','LC','TT','VC','VG','MX','BM')) ")
    '                End If

    '                adapter.SelectCommand.Parameters.AddWithValue("@text", fromloc)
    '                adapter.Fill(dt)

    '            Case Else
    '                'if this looks like a geographic place instead of an airport name ... 
    '                '20161028 - drive time
    '                dt = findairports(airport, MinRunwayLength, miles, airportcount, comboname)

    '                If Not (IsNothing(dt)) Then

    '                    '20161031 - pab - fix double loading - done below
    '                    'additemradcombobox(dt, "", "", airportcount, comboname)
    '                    'If dt.Rows.Count < 6 Then Insertsys_log(_carrierid, appName, "findairports returned < 6 - from loc " & airport & _
    '                    '    "; MinRunwayLength " & MinRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "loadairports", _
    '                    '    "home.aspx.vb")

    '                Else
    '                    If IsNumeric(airport.Trim) And Len(airport.Trim) = 5 Then
    '                        Dim ti As New Telerik.Web.UI.RadComboBoxItem
    '                        ti.Text = "Zip Code not found"
    '                        ti.Value = ""
    '                        Me.OriginAddress.Items.Add(ti)
    '                    End If
    '                End If

    '        End Select

    '        If Not isdtnullorempty(dt) Then
    '            '20161031 - pab - drive time
    '            '20170523 - pab - fix ddl not populating - different name
    '            'If comboname = "RadComboBox1" Then
    '            If comboname = "RadComboBox1" Or comboname = "OriginAddress" Then
    '                If IsNothing(latLongorig) Then
    '                    Dim ll As New LatLong
    '                    ll.Latitude = dt.Rows(0).Item("latitude")
    '                    ll.Longitude = dt.Rows(0).Item("Longitude")
    '                    latLongorig = ll
    '                End If
    '            Else
    '                If IsNothing(latLongdest) Then
    '                    Dim ll As New LatLong
    '                    ll.Latitude = dt.Rows(0).Item("latitude")
    '                    ll.Longitude = dt.Rows(0).Item("Longitude")
    '                    latLongdest = ll
    '                End If
    '            End If
    '            If dt.Rows.Count = 1 Then
    '                ds = da.GetAirportInformationByAirportCode(dt.Rows(0).Item("locationid"))
    '                If Not IsNothing(ds) Then
    '                    If Not isdtnullorempty(ds.Tables(0)) Then
    '                        dt = da.GetMajorAirportsByLatitudeLongitude(ds.Tables(0).Rows(0).Item("Latitude"), ds.Tables(0).Rows(0).Item("Longitude"),
    '                                MinRunwayLength, miles, airportcount)
    '                        If dt.Rows.Count < 6 Then Insertsys_log(_carrierid, appName, "hardcoded script returned < 6 - to loc " & fromloc & "; lat " &
    '                            ds.Tables(0).Rows(0).Item("Latitude") & "; long " & ds.Tables(0).Rows(0).Item("Longitude"), "loadairports",
    '                            "home.aspx.vb")
    '                    End If
    '                End If
    '            End If

    '            If dt.Rows.Count > 0 Then slocations = additemradcombobox(dt, slocations, fromloc, airportcount, comboname)

    '        Else
    '            '20160126 - pab - checked failed lookups, add if not found
    '            dt = da.GetLocationLookupFail(airport)
    '            If Not isdtnullorempty(dt) Then
    '                If dt.Rows.Count > 0 Then
    '                    For i As Integer = 0 To dt.Rows.Count - 1
    '                        If dt.Rows(i).Item("latitude") <> 0 And dt.Rows(i).Item("longitude") <> 0 Then
    '                            dt = da.GetMajorAirportsByLatitudeLongitude(dt.Rows(i).Item("Latitude"), dt.Rows(i).Item("Longitude"), _
    '                                MinRunwayLength, miles, airportcount)
    '                            slocations = additemradcombobox(dt, slocations, fromloc, airportcount, comboname)
    '                            Exit For
    '                        End If
    '                    Next
    '                Else
    '                    da.InsertLocationLookupFail(airport)
    '                End If

    '            Else
    '                da.InsertLocationLookupFail(airport)
    '            End If
    '        End If

    '    Catch ex As Exception
    '        Dim serr As String = ex.Message
    '        If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
    '        If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
    '        AirTaxi.SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " home2.aspx loadairports Error",
    '            serr, _carrierid)
    '        AirTaxi.Insertsys_log(_carrierid, AirTaxi.appName, serr, "home2.aspx", "loadairports")

    '    End Try

    '    If comboname = "OriginAddress" Then
    '        Me.OriginAddress.Items.Insert(0, New RadComboBoxItem("- Select airport below -"))
    '    ElseIf comboname = "DestinationAddress" Then
    '        Me.DestinationAddress.Items.Insert(0, New RadComboBoxItem("- Select airport below -"))
    '    End If

    '    AirTaxi.post_timing("home.aspx.vb loadairports end  " & Now.ToString)

    'End Sub

    '20160125 - pab - fix airport dropdowns
    'Function additemradcombobox(dt As DataTable, items As String, fromloc As String, airportcount As Integer, ByVal comboname As String) As String

    '    additemradcombobox = items

    '    '20161031 - pab - drive time
    '    Dim latlongto As New LatLong
    '    Dim drivetime As Integer = 0
    '    Dim wp As Integer = 0
    '    Dim dc As New DataColumn("drivetime", System.Type.GetType("System.Int32"))
    '    dt.Columns.Add(dc)
    '    For i = 0 To dt.Rows.Count - 1
    '        drivetime = 0
    '        '20161108 - tmc doesn't need drive time per David
    '        If _carrierid <> 65 Then
    '            latlongto.Latitude = dt.Rows(i).Item("latitude")
    '            latlongto.Longitude = dt.Rows(i).Item("Longitude")
    '            '20170523 - pab - fix ddl not populating - different name
    '            'If comboname = "RadComboBox1" Then
    '            If comboname = "RadComboBox1" Or comboname = "OriginAddress" Then
    '                If latLongorig.Latitude <> latlongto.Latitude Or latLongorig.Longitude <> latlongto.Longitude Then
    '                    '20170523 - pab - fix error - Operator '<>' is not defined for type 'DBNull' and string "".
    '                    If IsDBNull(dt.Rows(i).Item("locationid")) Then
    '                        drivetime = Mapping.GetDriveTime(latLongorig, latlongto, "")
    '                        '20161101 - pab - travel time too high per David
    '                    ElseIf dt.Rows(i).Item("locationid") <> "" Then
    '                        '20170901 - pab - fix distance if airport code entered
    '                        If dt.Rows(i).Item("locationid").ToString.ToUpper <> fromloc.ToUpper And
    '                                    dt.Rows(i).Item("icao").ToString.Trim.ToUpper <> fromloc.ToUpper Then
    '                            drivetime = Mapping.GetDriveTime(latLongorig, latlongto, dt.Rows(i).Item("locationid"))
    '                            '20161107 - pab - fix bad durations - ida airport code not found - returns ida, iowa
    '                            'If drivetime >= 9999999 Then
    '                            If drivetime >= 300 Then
    '                                drivetime = Mapping.GetDriveTime(latLongorig, latlongto, "")
    '                            End If
    '                        End If
    '                    Else
    '                        drivetime = Mapping.GetDriveTime(latLongorig, latlongto, "")
    '                    End If
    '                End If
    '            Else
    '                If latLongdest.Latitude <> latlongto.Latitude Or latLongdest.Longitude <> latlongto.Longitude Then
    '                    '20170523 - pab - fix error - Operator '<>' is not defined for type 'DBNull' and string "".
    '                    If IsDBNull(dt.Rows(i).Item("locationid")) Then
    '                        drivetime = Mapping.GetDriveTime(latLongorig, latlongto, "")
    '                        '20161101 - pab - travel time too high per David
    '                    ElseIf dt.Rows(i).Item("locationid") <> "" Then
    '                        '20170901 - pab - fix distance if airport code entered
    '                        If dt.Rows(i).Item("locationid").ToString.ToUpper <> fromloc.ToUpper And
    '                                    dt.Rows(i).Item("icao").ToString.Trim.ToUpper <> fromloc.ToUpper Then
    '                            drivetime = Mapping.GetDriveTime(latLongdest, latlongto, dt.Rows(i).Item("locationid"))
    '                            '20161107 - pab - fix bad durations - ida airport code not found - returns ida, iowa
    '                            'If drivetime >= 9999999 Then
    '                            If drivetime >= 300 Then
    '                                drivetime = Mapping.GetDriveTime(latLongdest, latlongto, "")
    '                            End If
    '                        End If
    '                    Else
    '                        drivetime = Mapping.GetDriveTime(latLongdest, latlongto, "")
    '                    End If
    '                End If
    '            End If
    '        End If
    '        dt.Rows(i).Item("drivetime") = drivetime
    '    Next
    '    Dim sortExp As String = "drivetime,miles"
    '    Dim drarray() As DataRow
    '    drarray = dt.Select(Nothing, sortExp, DataViewRowState.CurrentRows)

    '    'For i = 0 To dt.Rows.Count - 1
    '    For i = 0 To drarray.Count - 1
    '        Dim dr As DataRow = drarray(i)
    '        Dim ti As New Telerik.Web.UI.RadComboBoxItem

    '        'If Trim(dt.Rows(i).Item("locationid").ToString).ToUpper = "07FA" Then
    '        '    ti.Text = Trim(dt.Rows(i).Item("facilityname").ToString) & " (" & Trim(dt.Rows(i).Item("locationid").ToString) & ")"
    '        '    ti.Value = "OCA"
    '        'ElseIf Trim(dt.Rows(i).Item("icao").ToString) = "" Then
    '        '    ti.Text = Trim(dt.Rows(i).Item("facilityname").ToString) & " (" & Trim(dt.Rows(i).Item("locationid").ToString) & ")"
    '        '    ti.Value = Trim(dt.Rows(i).Item("locationid").ToString)
    '        'Else
    '        '    ti.Text = Trim(dt.Rows(i).Item("facilityname").ToString) & " (" & Trim(dt.Rows(i).Item("icao").ToString) & ")"
    '        '    ti.Value = Trim(dt.Rows(i).Item("locationid").ToString)
    '        'End If
    '        If Trim(dr("locationid").ToString).ToUpper = "07FA" Then
    '            ti.Text = Trim(dr("facilityname").ToString) & " (" & Trim(dr("locationid").ToString) & ")"
    '            ti.Value = "OCA"
    '        ElseIf Trim(dr("icao").ToString) = "" Then
    '            ti.Text = Trim(dr("facilityname").ToString) & " (" & Trim(dr("locationid").ToString) & ")"
    '            ti.Value = Trim(dr("locationid").ToString)
    '        Else
    '            ti.Text = Trim(dr("facilityname").ToString) & " (" & Trim(dr("icao").ToString) & ")"
    '            ti.Value = Trim(dr("locationid").ToString)
    '        End If

    '        If dr("drivetime") > 3 Then
    '            ti.Text &= "; " & dr("drivetime") & " minutes drive time"
    '        End If

    '        If dt.Rows(i).Item("miles") > 0 Then
    '            ti.Text &= "; " & dt.Rows(i).Item("miles") & " air miles"
    '        End If
    '        ti.ForeColor = Drawing.Color.Black

    '        If InStr(additemradcombobox, ti.Value) = 0 Then
    '            '20160426 - pab - make like door2door
    '            'If comboname = "OriginAddress" Then
    '            '    If OriginAddress.Items.Count >= airportcount Then Exit For
    '            'ElseIf comboname = "DestinationAddress" Then
    '            '    If DestinationAddress.Items.Count >= airportcount Then Exit For
    '            'End If
    '            Select Case comboname
    '                Case "OriginAddress"
    '                    If OriginAddress.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress2"
    '                    If OriginAddress2.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress3"
    '                    If OriginAddress3.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress4"
    '                    If OriginAddress4.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress5"
    '                    If OriginAddress5.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress6"
    '                    If OriginAddress6.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress7"
    '                    If OriginAddress7.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress8"
    '                    If OriginAddress8.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress9"
    '                    If OriginAddress9.Items.Count >= airportcount Then Exit For
    '                Case "OriginAddress10"
    '                    If OriginAddress10.Items.Count >= airportcount Then Exit For

    '                Case "DestinationAddress"
    '                    If DestinationAddress.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress2"
    '                    If DestinationAddress2.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress3"
    '                    If DestinationAddress3.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress4"
    '                    If DestinationAddress4.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress5"
    '                    If DestinationAddress5.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress6"
    '                    If DestinationAddress6.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress7"
    '                    If DestinationAddress7.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress8"
    '                    If DestinationAddress8.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress9"
    '                    If DestinationAddress9.Items.Count >= airportcount Then Exit For
    '                Case "DestinationAddress10"
    '                    If DestinationAddress10.Items.Count >= airportcount Then Exit For
    '            End Select

    '            'If (InStr(dt.Rows(i).Item("icao").ToString, fromloc) > 0 Or InStr("(" & ti.Text & ")", fromloc) > 0) And fromloc <> "" Then
    '            '    ti.ForeColor = Drawing.Color.Red
    '            'End If
    '            If (InStr(dr("icao").ToString, fromloc) > 0 Or InStr("(" & ti.Text & ")", fromloc) > 0) And fromloc <> "" Then
    '                ti.ForeColor = Drawing.Color.Red
    '            End If
    '            '20160426 - pab - make like door2door
    '            'If comboname = "OriginAddress" Then
    '            '    OriginAddress.Items.Add(ti)
    '            'ElseIf comboname = "DestinationAddress" Then
    '            '    DestinationAddress.Items.Add(ti)
    '            'End If
    '            Select Case comboname
    '                Case "OriginAddress"
    '                    OriginAddress.Items.Add(ti)
    '                Case "OriginAddress2"
    '                    OriginAddress2.Items.Add(ti)
    '                Case "OriginAddress3"
    '                    OriginAddress3.Items.Add(ti)
    '                Case "OriginAddress4"
    '                    OriginAddress4.Items.Add(ti)
    '                Case "OriginAddress5"
    '                    OriginAddress5.Items.Add(ti)
    '                Case "OriginAddress6"
    '                    OriginAddress6.Items.Add(ti)
    '                Case "OriginAddress7"
    '                    OriginAddress7.Items.Add(ti)
    '                Case "OriginAddress8"
    '                    OriginAddress8.Items.Add(ti)
    '                Case "OriginAddress9"
    '                    OriginAddress9.Items.Add(ti)
    '                Case "OriginAddress10"
    '                    OriginAddress10.Items.Add(ti)

    '                Case "DestinationAddress"
    '                    DestinationAddress.Items.Add(ti)
    '                Case "DestinationAddress2"
    '                    DestinationAddress2.Items.Add(ti)
    '                Case "DestinationAddress3"
    '                    DestinationAddress3.Items.Add(ti)
    '                Case "DestinationAddress4"
    '                    DestinationAddress4.Items.Add(ti)
    '                Case "DestinationAddress5"
    '                    DestinationAddress5.Items.Add(ti)
    '                Case "DestinationAddress6"
    '                    DestinationAddress6.Items.Add(ti)
    '                Case "DestinationAddress7"
    '                    DestinationAddress7.Items.Add(ti)
    '                Case "DestinationAddress8"
    '                    DestinationAddress8.Items.Add(ti)
    '                Case "DestinationAddress9"
    '                    DestinationAddress9.Items.Add(ti)
    '                Case "DestinationAddress10"
    '                    DestinationAddress10.Items.Add(ti)
    '            End Select

    '            additemradcombobox &= ti.Value & ";"
    '        End If
    '    Next

    'End Function

    Protected Sub Address_ItemsRequested(ByVal o As Object, ByVal e As RadComboBoxItemsRequestedEventArgs)

        Try

            AirTaxi.post_timing("home.aspx.vb OriginAddress_ItemsRequested start  " & Now.ToString)

            '20160617 - pab - fix screen hanging up after sitting idle
            If Session("email") Is Nothing Then
                Response.Redirect("CustomerLogin.aspx", True)
            End If

            Dim id As String = o.id

            'loadairports(e.Text.Trim, id)

            Exit Sub

        Catch ex As Exception
            Dim serr As String = ex.Message
            If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(0, AirTaxi.appName, serr, "home2.aspx OriginAddress_ItemsRequested", "")
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            AirTaxi.SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " home2.aspx OriginAddress_ItemsRequested Error",
                              serr, 0)

        End Try

        'Me.OriginAddress.DataTextField = "airport"
        'Me.OriginAddress.DataValueField = "airport"
        'Me.OriginAddress.DataSource = dt
        'Me.OriginAddress.DataBind()
        ' Insert the first item.
        Me.OriginAddress.Items.Insert(0, New RadComboBoxItem("- Select airport below -"))

        AirTaxi.post_timing("home.aspx.vb OriginAddress_ItemsRequested end  " & Now.ToString)

    End Sub

    ''20150511 - pab - fix airport list too short
    ''20161028 - drive time
    'Function findairports(ByVal a As String, ByVal minRunwayLength As Integer, ByVal miles As Integer, ByVal airportcount As Integer,
    '        ByVal comboname As String) As DataTable

    '    AirTaxi.post_timing("home.aspx.vb findairports start  " & Now.ToString)

    '    Dim omapping As New WebRole1.Mapping

    '    'System.Diagnostics.Debug.WriteLine("Check Orig Lat  " & Now.ToString)

    '    Dim latLong2 As New LatLong

    '    Try

    '        '20120621 - pab - default country to USA if not iata or icao
    '        'Dim b As String = String.Empty
    '        '20150427 - pab - fix spaces sometimes being converted - geocode mapping location invalid - boca%20raton
    '        If InStr(a, "%20") > 0 Then a = Replace(a, "%20", " ")
    '        Dim da As New DataAccess
    '        If Len(a.Trim) = 3 Then
    '            '20120831 - pab - fix APA (Centennial CO) not being found
    '            'b = fname(a)
    '            ''text passed in is not airport iata
    '            'If a = b Then a &= ", US"
    '            Dim ds As DataSet
    '            ds = da.GetAirportInformationByAirportCode(a.Trim.ToUpper)
    '            If Not IsNothing(ds) Then
    '                If Not isdtnullorempty(ds.Tables(0)) Then
    '                    latLong2.Latitude = CDbl(ds.Tables(0).Rows(0).Item("latitude"))
    '                    latLong2.Longitude = CDbl(ds.Tables(0).Rows(0).Item("longitude"))
    '                End If
    '            End If
    '        ElseIf Len(a.Trim) = 4 Then
    '            'If Left(a.Trim.ToUpper, 1) = "K" Then
    '            '    Dim da As New DataAccess
    '            '    Dim ds As DataSet = da.GetIATAcodebyICAO(a.Trim)
    '            '    If ds.Tables.Count > 0 Then
    '            '        If ds.Tables(0).Rows.Count > 0 Then
    '            '            'text passed in is icao
    '            '        Else
    '            '            a &= ", US"
    '            '        End If
    '            '    Else
    '            '        a &= ", US"
    '            '    End If
    '            'End If

    '            '20120831 - pab - fix APA (Centennial CO) not being found
    '            Dim dt As DataTable
    '            dt = da.GetAirportInformationByICAO(a.Trim.ToUpper)
    '            If Not isdtnullorempty(dt) Then
    '                latLong2.Latitude = CDbl(dt.Rows(0).Item("latitude"))
    '                latLong2.Longitude = CDbl(dt.Rows(0).Item("longitude"))
    '            End If

    '            '20150625 - pab - add zip code table lookup to reduce bing mapping issues
    '        ElseIf Len(a.Trim) = 5 And IsNumeric(a.Trim) Then
    '            Dim dt As DataTable
    '            dt = da.GetZIPCodes(a.Trim.ToUpper)
    '            If Not isdtnullorempty(dt) Then
    '                latLong2.Latitude = CDbl(dt.Rows(0).Item("latitude"))
    '                latLong2.Longitude = CDbl(dt.Rows(0).Item("longitude"))
    '            End If

    '        Else
    '            '20150424 - pab - fix locations not being found 
    '            If InStr(a.Trim, ",") > 0 Then
    '                'check for incomplete address entry
    '                Dim s As String = Mid(a, InStr(a.Trim, ",") + 1).Trim
    '                If Len(s) < 2 Then
    '                    Return Nothing
    '                End If

    '            ElseIf InStr(a.Trim, ",") = 0 And InStr(a.Trim, " ") = 0 Then
    '                '20120823 - pab - oakland, ca not showing up in ddl when US appended to text. location set to somewhere in IL
    '                'a &= ", US"
    '            End If
    '        End If

    '        '20120831 - pab - fix APA (Centennial CO) not being found
    '        'latLong2 = omapping.GeoCodeText(a)    'oMapping.GeocodeAddress(a.Trim, AddressCaptureVertical1.OriginCity.Trim, Me.AddressCaptureVertical1.OriginState, Me.AddressCaptureVertical1.OriginZip.Trim, Me.AddressCaptureVertical1.OriginCountry.Trim)
    '        If IsNothing(latLong2) Then
    '            latLong2 = omapping.GeoCodeText(a) 'oMapping.GeocodeAddress(a.Trim, AddressCaptureVertical1.OriginCity.Trim, Me.AddressCaptureVertical1.OriginState, Me.AddressCaptureVertical1.OriginZip.Trim, Me.AddressCaptureVertical1.OriginCountry.Trim)
    '        ElseIf latLong2.Latitude = 0 Then
    '            '20150625 - pab - add zip code table lookup to reduce bing mapping issues
    '            If Len(a.Trim) = 5 And IsNumeric(a.Trim) Then
    '                'assume invalid zip code and exit
    '                Insertsys_log(_carrierid, appName, "invalid zip code - " & a, "findairports", "home.aspx.vb")
    '                Return Nothing
    '            Else
    '                latLong2 = omapping.GeoCodeText(a) 'oMapping.GeocodeAddress(a.Trim, AddressCaptureVertical1.OriginCity.Trim, Me.AddressCaptureVertical1.OriginState, Me.AddressCaptureVertical1.OriginZip.Trim, Me.AddressCaptureVertical1.OriginCountry.Trim)
    '            End If
    '        End If

    '        '20090107 - pab - mappoint subscription error - don't return value
    '        If latLong2 Is Nothing Then
    '            '20150422 - pab - add logging - lookup is flaky
    '            Insertsys_log(_carrierid, appName, "geocode mapping location not found - " & a &
    '            "; MinRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "findairports", "home.aspx.vb")

    '        Else
    '            If latLong2.Latitude = 0 Then
    '                '  Me.bttnGetFlights.Enabled = True
    '                '  Me.lblmsg.Text = "Is the originating city/state spelled correctly?"
    '                ' Me.lblmsg.Visible = True
    '                '  Me.AddressCaptureVertical1.Focus()

    '                '20150422 - pab - add logging - lookup is flaky
    '                Insertsys_log(_carrierid, appName, "geocode mapping location invalid - " & a &
    '                "; MinRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "findairports", "home.aspx.vb")

    '                Return Nothing 'Exit Function
    '            Else
    '                '20100622 - pab - fix floating point error
    '                latLong2.Latitude = Math.Round(latLong2.Latitude, 13)
    '                latLong2.Longitude = Math.Round(latLong2.Longitude, 13)
    '                'Session("origlat") = latLong2
    '                'Session("origplace") = a
    '                'System.Diagnostics.Debug.WriteLine(latLong2.ToString & "Orig Lat  " & Now.ToString)

    '            End If
    '        End If

    '        '20161028 - drive time
    '        '20170523 - pab - fix ddl not populating - different name
    '        'If comboname = "RadComboBox1" Then
    '        If comboname = "RadComboBox1" Or comboname = "OriginAddress" Then
    '            latLongorig = latLong2
    '        Else
    '            latLongdest = latLong2
    '        End If


    '        Dim o As New DataTable
    '        '20150511 - pab - fix airport list too short
    '        '20170623 - pab - fix object not set
    '        If Not IsNothing(latLong2) Then
    '            o = GetAirportsLatLong(latLong2, minRunwayLength, miles, airportcount)

    '            If isdtnullorempty(o) Then
    '                Insertsys_log(_carrierid, appName, "GetAirportsLatLong returned no records - lat " & latLong2.Latitude.ToString &
    '                    "; long " & latLong2.Longitude.ToString & "; MinRunwayLength " & minRunwayLength & "; miles " & miles &
    '                    "; airportcount " & airportcount, "findairports", "home.aspx.vb")
    '            End If

    '        End If

    '        AirTaxi.post_timing("home.aspx.vb findairports end  " & Now.ToString)

    '        Return o


    '    Catch ex As Exception
    '        Dim serr As String = "parms a - " & a & vbCr & vbLf & ex.Message
    '        If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
    '        If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
    '        AirTaxi.SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " home2.aspx findairports Error",
    '                            serr, _carrierid)
    '        AirTaxi.Insertsys_log(0, AirTaxi.appName, serr, "home2.aspx findairports", "")

    '        Return Nothing

    '    End Try

    'End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        if Session("carrierid") Is Nothing Then
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(0, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString &
                "; Session(carrierid) - null", "Page_Load", "home.aspx.vb")
        Else
            Insertsys_log(CInt(Session("carrierid")), appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString &
                "; Session(carrierid) - " & Session("carrierid").ToString, "Page_Load", "home.aspx.vb")
        End If

        '20120830 - pab - try to fix telerik issue - ddls not working when round trip clicked
        Response.Cache.SetNoStore()

        '20120622 - pab - remove hardcoded connection string
        Dim adapter As New SqlDataAdapter("", ConnectionStringHelper.getglobalconnectionstring(PortalServer))

        '20140421 - pab - fix background image
        '20150317 - pab - remove acg branding
        'If IsNothing(_acg) Then _acg = ""
        ''20140817 - pab - fix default to coastal if not acg
        'If Request.QueryString("acg") Is Nothing Then
        '    _acg = ""
        'End If

        ''20140528 - pab - fix default going to acg
        'Dim urlreferrer As Object = Request.UrlReferrer
        'If IsNothing(urlreferrer) Then urlreferrer = ""
        'Insertsys_log(58, appName, "Request.UrlReferrer acg - " & urlreferrer.ToString & " " & _acg.ToString, "Page_Load", "home.aspx.vb")


        'If Len(e.Text) < 2 Then
        '    '  OriginAddress.showdropdown = False
        '    Exit Sub

        'End If

        '20101105 - pab - add code for aliases
        Dim host As String = Request.Url.Host
        Dim url As String = Request.Url.ToString
        Dim serror As String = ""
        Dim da As New DataAccess
        '20120515 - pab - fix all portals going to xojet
        'If _urlalias Is Nothing Or ConnectionStringHelper.GetCASConnectionStringSQL Is Nothing Or _
        'ConnectionStringHelper.GetCASConnectionStringSQL = "" Then
        '20130107 - pab - fix error - Object reference not set to an instance of an object
        'If _urlalias Is Nothing Or ConnectionStringHelper.GetCASConnectionStringSQL Is Nothing _
        'Or ConnectionStringHelper.GetCASConnectionStringSQL = "" _
        '        Or (InStr(host.ToUpper, _urlalias.ToUpper) = 0 And host <> "127.0.0.1" And host <> "localhost") Then
        '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
        If Session("urlalias") Is Nothing Then Session("urlalias") = ""

        '20160412 - pab - If Session("email") Is Nothing Then assume timeout and redirect to login page
        If Session("email") Is Nothing Then
            'Session.Abandon()
            Response.Redirect("CustomerLogin.aspx", True)

            '20120114 - pab - fix carrier not resolving
        ElseIf InStr(Session("urlalias").tostring.ToLower, "personiflyadmin") > 0 Then
            Session("urlalias") = ""


        End If

        If Left(host.ToLower, 4) = "www." Then
            host = Mid(host.ToLower, 5)
        End If

        '20160412 - pab - beta for tmc
        '20170126 - pab - no more tmc
        'If InStr(host, "corporateportaluatbeta") > 0 Then
        '    host = "tmcjets." & host
        'End If

        If Session("urlalias").ToString = "" Then

            serror &= "line 1456" & vbNewLine

            '20101129 - pab - force last connection to close when starting new session
            If cnsetting.State = 1 Then cnsetting.Close()

            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            'geturlaliasandconnections(host)
            Dim alist As ArrayList = geturlaliasandconnections(host)
            If Not IsNothing(alist) Then
                If alist.Count >= 3 Then
                    Session("carrierid") = CInt(alist.Item(0))
                    'Session("urlalias") = _urlalias
                    Session("urlalias") = alist.Item(1)
                    Session("connectstring") = alist.Item(2)
                End If
            End If

            '20160908 - pab - carrierid getting lost - set as session variable
            'Session("carrierid") = _carrierid

            'Dim oQuote As New SaveQuoteInfo

            'oQuote.QuoteInfoArray("urlalias", _urlalias)
            'oQuote.QuoteInfoArray("connectstring", connectstring)
            'oQuote.QuoteInfoArray("ConnectionStringHelper.GetCASConnectionStringSQL", ConnectionStringHelper.GetCASConnectionStringSQL)
            'oQuote.InsertQuote()

            '20120524 - pab - log alias info
            Insertsys_log(CInt(Session("carrierid")), appName, "carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString & "; host - " & host &
                "; Session(carrierid) - " & Session("carrierid").ToString & "; connectstring - " & Session("connectstring").ToString, "Page_Load", "Home.aspx.vb")

            If CInt(Session("carrierid")) = 0 Or Session("urlalias").ToString = "" Or InStr(Session("urlalias").ToString.ToLower, "personiflyadmin") > 0 Then
                serror &= "line 1475" & vbNewLine
                '20130702 - pab - insert into log - don't sent email
                'SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "carrier not resolved", appName & _
                '" carrierid - " & _carrierid & "; _urlalias - " & _urlalias & "; host - " & host & "; connectstring - " & _
                'connectstring & vbNewLine & serror, _carrierid)
                Insertsys_log(CInt(Session("carrierid")), appName, "carrier not resolved; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString &
                    "; host - " & host & "; connectstring - " & Session("connectstring").ToString, "Page_Load", "Home.aspx.vb")
                Response.Redirect("error_page2.aspx")
                Exit Sub
            End If

            'Else
            '    SendEmail("pbaumgart@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "error opening connection string", _
            '       "_urlalias string - " & _urlalias & vbNewLine & vbNewLine & "connection string - " & connectstring & vbNewLine & vbNewLine & _
            '"ConnectionStringHelper.GetCASConnectionStringSQL string - " & ConnectionStringHelper.GetCASConnectionStringSQL & vbNewLine & vbNewLine)
        End If

        '20160517 - pab - fix carrierid = 0 preventing quotes
        If InStr(Session("email").ToString.ToLower, "tmcjets.com") > 0 And CInt(Session("carrierid")) = 0 Then
            Session("carrierid") = 65
        End If

        If Not IsPostBack Then

            '20140224 - pab - add threading for airport drop downs
            Dim tu As Thread
            ''tu = New Thread(AddressOf update_pbid_email(pbid, email, provider))
            'tu = New Thread(AddressOf wakeupwebservice)
            'tu.Start("wakeup")

            '20140310 - pab - acg background image
            'AirTaxi.Insertsys_log(0, appName, Left("Request.UrlReferrer " & Request.UrlReferrer.ToString, 500), "Page_Load", "SelectAirports.aspx.vb")
            '20150317 - pab - remove acg branding
            'If Not Request.QueryString("acg") Is Nothing Then
            '    _acg = Request.QueryString("acg")
            '    'If _acg <> "" Then Me.content.Style("background-image") = "/images/personifly_ACG_logo.png"
            '    ''20140528 - pab - fix default going to acg
            '    'Insertsys_log(58, appName, "Request.QueryString acg - " & _acg.ToString, "Page_Load", "home.aspx.vb")
            'End If
            ''20140317 - pab - acg branding testing - comment out for release
            ''_acg = "2"

            Me.OriginAddress.ClearSelection()
            Me.DestinationAddress.ClearSelection()
            Me.rblOneWayRoundTrip.SelectedValue = "OneWay"
            'RadComboBoxPax.ClearSelection()
            'RadComboBoxFlexMiles.ClearSelection()
            'RadComboFlexFrom.ClearSelection()
            'RadComboBoxFlexTo.ClearSelection()
            RadComboBoxACInclude.ClearSelection()
            'RadComboBoxACExclude.ClearSelection()
            'RadComboBoxCertifications.ClearSelection()
            'RadComboBoxRequests.ClearSelection()

            '20160117 - pab - quote multi-leg trips
            'Me.lbLegs.Items.Clear()

            '20120628 - pab - remove hardcoded db
            adapter.SelectCommand.CommandText = ("SELECT  distinct( [Name])  FROM [AircraftTypeServiceSpecs] order by  name")


            Dim dt As New DataTable()
            adapter.Fill(dt)

            'RadComboBoxACExclude.Items.Clear()


            'For i = 0 To dt.Rows.Count - 1

            '    Dim ti As New Telerik.Web.UI.RadComboBoxItem
            '    ti.Text = Trim(dt.Rows(i).Item(0).ToString)
            '    ti.Value = Trim(dt.Rows(i).Item(0).ToString)


            '    RadComboBoxACExclude.Items.Add(ti)
            'Next

            If ddllBrokerCompanies.Items.Count = 0 Then
                'HydrateddllBrokerCompanies()
                'HydrateddllBrokers("")
            End If

            If (IsNothing(Me.depart_date.SelectedDate)) Then

                Me.depart_date.Clear()
                Me.depart_date2.Clear()
                '20120830 - pab - default time to 9:00 per David
                'Me.depart_date.SelectedDate = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy") & " " & Now.Hour & ":" & "00"
                If DateDiff(DateInterval.Hour, Now, CDate(DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy") & " 9:00")) > 8 Then
                    Me.depart_date.SelectedDate = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy") & " 9:00"
                    Me.depart_date2.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy") & " 17:00"
                Else
                    Me.depart_date.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy") & " 9:00"
                    Me.depart_date2.SelectedDate = DateAdd(DateInterval.Day, 3, Now).ToString("MM/dd/yyyy") & " 17:00"
                End If

            End If

            Dim oLookup As New PopulateLookups
            'Dim dt As New Data.DataTable

            Me.departtime_combo.Items.Clear()
            dt = oLookup.TimeDD("All")
            Me.departtime_combo.DataSource = dt.DefaultView
            Me.departtime_combo.DataBind()

            'For i As Integer = 0 To dt.Rows.Count - 1
            '    Dim li As New ListItem
            '    li.Text = dt.Rows(i).Item("timestr")
            '    li.Value = dt.Rows(i).Item("timestr")
            '    Me.ddlTimeLeave.Items.Add(li)
            'Next

            Me.departtime_combo.SelectedValue = "09:00 AM"

            Me.departtime_combo2.Items.Clear()
            Me.departtime_combo2.DataSource = dt.DefaultView
            Me.departtime_combo2.DataBind()
            Me.departtime_combo2.SelectedValue = "09:00 AM"

            Me.departtime_combo3.Items.Clear()
            Me.departtime_combo3.DataSource = dt.DefaultView
            Me.departtime_combo3.DataBind()
            Me.departtime_combo3.SelectedValue = "09:00 AM"

            Me.departtime_combo4.Items.Clear()
            Me.departtime_combo4.DataSource = dt.DefaultView
            Me.departtime_combo4.DataBind()
            Me.departtime_combo4.SelectedValue = "09:00 AM"

            Me.departtime_combo5.Items.Clear()
            Me.departtime_combo5.DataSource = dt.DefaultView
            Me.departtime_combo5.DataBind()
            Me.departtime_combo5.SelectedValue = "09:00 AM"

            Me.departtime_combo6.Items.Clear()
            Me.departtime_combo6.DataSource = dt.DefaultView
            Me.departtime_combo6.DataBind()
            Me.departtime_combo6.SelectedValue = "09:00 AM"

            Me.departtime_combo7.Items.Clear()
            Me.departtime_combo7.DataSource = dt.DefaultView
            Me.departtime_combo7.DataBind()
            Me.departtime_combo7.SelectedValue = "09:00 AM"

            Me.departtime_combo8.Items.Clear()
            Me.departtime_combo8.DataSource = dt.DefaultView
            Me.departtime_combo8.DataBind()
            Me.departtime_combo8.SelectedValue = "09:00 AM"

            Me.departtime_combo9.Items.Clear()
            Me.departtime_combo9.DataSource = dt.DefaultView
            Me.departtime_combo9.DataBind()
            Me.departtime_combo9.SelectedValue = "09:00 AM"

            Me.departtime_combo10.Items.Clear()
            Me.departtime_combo10.DataSource = dt.DefaultView
            Me.departtime_combo10.DataBind()
            Me.departtime_combo10.SelectedValue = "09:00 AM"

            ' RadComboBox3.Items.Insert(0, New RadComboBoxItem("- Select aircraft types to exclude -"))

            '20120628 - pab - add one way/round trip radio buttons
            'DisplayBoxBasedRadioBtns()

            If CInt(Session("carrierid")) > 0 Then
                Session.Timeout = CInt(da.GetSetting(CInt(Session("carrierid")), "SessionTimeout")) 'rk 10.18.2010 set session timeout as seteting
            Else
                Session.Timeout = 2000
            End If

            '20130930 - pab - change email from
            If IsNothing(_emailfrom) Then _emailfrom = ""
            If _emailfrom = "" Then
                _emailfrom = da.GetSetting(CInt(Session("carrierid")), "emailsentfrom")
            End If
            'If IsNothing(_emailfromquote) Then _emailfromquote = ""
            'If _emailfromquote = "" Then
            '    _emailfromquote = da.GetSetting(_carrierid, "emailsentfromQuote")
            'End If

            '20120507 - pab - prevent confirmation email from going out more than once if back button pressed
            Session("confirmemailsent") = "N"

            ''20130603 - pab - add configurable tag line
            'Dim tagline As String = da.GetSetting(_carrierid, "TagLine").Trim
            'If tagline <> "" Then lblTagLine.Text = tagline

            '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
            TurnAroundMinutes = CInt(da.GetSetting(CInt(Session("carrierid")), "TurnAroundMinutes"))

        End If

        '20121016 - pab - broker login
        '20130409 - pab - add member login
        'If Session("BrokerID") = 0 Then
        Dim bIsLoggedIn As Boolean = False
        If Not IsNothing(Session("IsLoggedIn")) Then bIsLoggedIn = CBool(Session("IsLoggedIn"))
        'If bIsLoggedIn = True Then
        '    Me.lnkBrokerLogin.Visible = False
        '    Me.lnkBrokerLogout.Visible = True
        '    '20140313 - pab - acg branding
        '    '20150317 - pab - remove acg branding
        '    'Me.lnkBrokerLoginacg.Visible = False
        'Else
        '    '20140313 - pab - acg branding
        '    '20150317 - pab - remove acg branding
        '    'If _acg = "1" Or _acg = "2" Then
        '    '    Me.lnkBrokerLoginacg.Visible = True
        '    'Else
        '    Me.lnkBrokerLogin.Visible = True
        '    'End If
        '    Me.lnkBrokerLogout.Visible = False
        'End If

    End Sub



    'Protected Sub findflights()

    '    Dim fromairport As String
    '    Dim toairport As String
    '    '20120621 - pab - fix if iata code entered (did not selct from ddl)
    '    Dim fromaddr As String = String.Empty
    '    Dim toaddr As String = String.Empty
    '    Dim da As New DataAccess
    '    Dim ds As New DataSet
    '    Dim ld As String = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy")
    '    Dim rd As String = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy")
    '    Dim lt As String = "09:00 AM"
    '    Dim rt As String = "05:00 PM"
    '    Dim s As String = ""

    '    '20160117 - pab - quote multi-leg trips
    '    lblMsg.Text = ""
    '    lblMsg.ForeColor = Color.Red
    '    lblMsg.Font.Bold = True
    '    lblMsg.Visible = False

    '    Try
    '        '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
    '        TurnAroundMinutes = cint(da.GetSetting(_carrierid, "TurnAroundMinutes"))

    '        '20160117 - pab - quote multi-leg trips
    '        'If Session("triptype") = "M" And lbLegs.Items.Count = 1 Then
    '        '    Session("triptype") = "O"
    '        'End If
    '        If Session("triptype") = "M" Then
    '            s = editflightsmultileg(TurnAroundMinutes)
    '            If s <> "" Then s = "&Legs=" & s
    '            's = ""
    '            'If lbLegs.Items.Count > 0 Then
    '            '    s = "&legs="
    '            '    For i As Integer = 0 To lbLegs.Items.Count - 1
    '            '        If i = 0 Then
    '            '            Dim s1 As String = lbLegs.Items(0).Value
    '            '            Session("A") = Left(s1, InStr(s1, " to ") - 1).Trim
    '            '            Session("B") = Mid(s1, InStr(s1, " to ") + 4, 4).Trim
    '            '            s1 = Mid(s1, InStr(s1, " at ") + 4).Trim
    '            '            If IsDate(s1) Then
    '            '                ld = CDate(s1).ToString("d")
    '            '                lt = CDate(s1).ToString("t")
    '            '                rd = DateAdd(DateInterval.Day, 1, CDate(ld)).ToString("d")
    '            '                rt = DateAdd(DateInterval.Hour, 8, CDate(rd)).ToString("t")
    '            '            End If
    '            '            fromairport = Session("A")
    '            '            toairport = Session("B")
    '            '        End If
    '            '        s &= lbLegs.Items(i).Value & ";"
    '            '    Next
    '            'End If
    '        Else
    '            editflights(TurnAroundMinutes)

    '        End If

    '        If lblMsg.Visible = True Then
    '            Exit Sub
    '        End If

    '        Session("A") = Me.OriginAddress.Text
    '        Session("B") = Me.DestinationAddress.Text

    '        If Me.OriginAddress.Text <> "" Then fromaddr = Me.OriginAddress.Text
    '        If Me.OriginAddress.SelectedValue <> "" Then
    '            fromairport = Me.OriginAddress.SelectedValue
    '        ElseIf Me.OriginAddress.Text <> "" Then
    '            fromairport = Me.OriginAddress.Text.ToUpper
    '        End If
    '        If Me.DestinationAddress.Text <> "" Then toaddr = Me.DestinationAddress.Text
    '        If Me.DestinationAddress.SelectedValue <> "" Then
    '            toairport = Me.DestinationAddress.SelectedValue
    '        ElseIf Me.DestinationAddress.Text <> "" Then
    '            toairport = Me.DestinationAddress.Text.ToUpper
    '        End If

    '        '20121204 - pab - implement aircraft to include
    '        'Session("achourly") = RadComboBoxACInclude.SelectedValue
    '        Dim actoinclude As String = String.Empty
    '        For i As Integer = 0 To RadComboBoxACInclude.CheckedItems.Count - 1
    '            If actoinclude <> "" Then actoinclude &= ","
    '            actoinclude &= RadComboBoxACInclude.CheckedItems(i).Value
    '        Next

    '        '20160117 - pab - suppress all but light, mid and heavy per David 1/15/2016
    '        '20171025 - pab - put turboprops and supermids back in
    '        If actoinclude = "" Then actoinclude &= "2,L,M,U,H"

    '        Session("achourly") = actoinclude

    '        Session("actype") = RadComboBoxACInclude.Text
    '        'Session("actypeexclude") = RadComboBoxACExclude.Text
    '        'Session("certifications") = RadComboBoxCertifications.Text

    '        'Session("flexto") = RadComboBoxFlexTo.Text
    '        'Session("flexfrom") = RadComboFlexFrom.Text
    '        Session("outdate") = Me.depart_date.SelectedDate
    '        Session("returndate") = Me.depart_date2.SelectedDate


    '        Dim pax As String = 1


    '        If Not (IsNothing(Me.ddlPassengers.Text)) Then
    '            If IsNumeric(Me.ddlPassengers.Text) Then
    '                pax = CInt(Me.ddlPassengers.Text)
    '            End If
    '        End If


    '        '20120619 - pab - add filters for pets, smoking, wifi
    '        'Dim script As String = "selectairports.aspx?" & _
    '        '                   "origAddr=" & fromairport & _
    '        '                   "&origCity=" & "" & _
    '        '                   "&origState=" & "" & _
    '        '                   "&origZip=" & "" & _
    '        '                   "&origCountry=" & "" & _
    '        '                   "&destAddr=" & toairport & _
    '        '                   "&destCity=" & "" & _
    '        '                   "&destState=" & "" & _
    '        '                   "&destZip=" & "" & _
    '        '                   "&destCountry=" & "" & _
    '        '                   "&roundTrip=" & "False" & _
    '        '                   "&passengers=" & pax & _
    '        '                   "&leaveDate=" & ld & _
    '        '                   "&returnDate=" & rd & _
    '        '                   "&leaveTime=" & lt & _
    '        '                   "&returnTime=" & rt & _
    '        '                   "&origAirportCode=" & fromairport & _
    '        '                   "&destAirportCode=" & toairport
    '        Dim sb As New StringBuilder()
    '        'Dim collection As IList(Of RadComboBoxItem) = RadComboBoxRequests.CheckedItems
    '        Dim bAllowPets As Boolean = False
    '        Dim bAllowSmoking As Boolean = False
    '        Dim bWiFi As Boolean = False

    '        '20120709 - pab - add lav, power
    '        Dim bLav As Boolean = False
    '        Dim bPower As Boolean = False

    '        '20131118 - pab - add more fields to aircraft
    '        Dim bInFlightEntertainment As Boolean = False

    '        '20130626 - pab - check parms before running quote
    '        Dim options As String = " and ("

    '        'For Each item As RadComboBoxItem In collection
    '        '    sb.Append(item.Text + "<br />")
    '        '    Select Case item.Text
    '        '        Case "Pets"
    '        '            bAllowPets = True

    '        '            '20130626 - pab - check parms before running quote
    '        '            If options = " and (" Then
    '        '                options &= " AllowPets = 1 "
    '        '            Else
    '        '                options &= " and AllowPets = 1 "
    '        '            End If

    '        '        Case "Smoking"
    '        '            bAllowSmoking = True

    '        '            '20130626 - pab - check parms before running quote
    '        '            If options = " and (" Then
    '        '                options &= " AllowSmoking = 1 "
    '        '            Else
    '        '                options &= " and AllowSmoking = 1 "
    '        '            End If

    '        '        Case "WiFi"
    '        '            bWiFi = True

    '        '            '20130626 - pab - check parms before running quote
    '        '            If options = " and (" Then
    '        '                options &= " WiFi = 1 "
    '        '            Else
    '        '                options &= " and WiFi = 1 "
    '        '            End If

    '        '            '20120709 - pab - add lav, power
    '        '        Case "Enclosed Lav"
    '        '            bLav = True

    '        '            '20130626 - pab - check parms before running quote
    '        '            If options = " and (" Then
    '        '                options &= " EnclosedLav = 1 "
    '        '            Else
    '        '                options &= " and EnclosedLav = 1 "
    '        '            End If

    '        '        Case "Power"
    '        '            bPower = True

    '        '            '20130626 - pab - check parms before running quote
    '        '            If options = " and (" Then
    '        '                options &= " PowerAvailable = 1 "
    '        '            Else
    '        '                options &= " and PowerAvailable = 1 "
    '        '            End If

    '        '            '20131118 - pab - add more fields to aircraft
    '        '        Case "InFlight Entertainment"
    '        '            bInFlightEntertainment = True
    '        '            If options = " and (" Then
    '        '                options &= " InflightEntertainment = 1 "
    '        '            Else
    '        '                options &= " and InflightEntertainment = 1 "
    '        '            End If

    '        '    End Select
    '        'Next

    '        '20130626 - pab - check parms before running quote
    '        If options = " and (" Then
    '            options = ""
    '        Else
    '            options &= ")"
    '        End If

    '        Dim sb2 As New StringBuilder()
    '        Dim collection2 As IList(Of RadComboBoxItem) = RadComboBoxACInclude.CheckedItems
    '        Dim weightclass As String = " and WeightClass in ("
    '        Dim ratings As String = ""

    '        For Each item As RadComboBoxItem In collection2
    '            sb2.Append(item.Text + "<br />")

    '            If weightclass = " and WeightClass in (" Then
    '                weightclass &= " '" & item.Value & "'"
    '            Else
    '                weightclass &= ", '" & item.Value & "'"
    '            End If

    '        Next
    '        If weightclass = " and WeightClass in (" Then
    '            weightclass = ""
    '        Else
    '            weightclass &= ")"
    '        End If

    '        'If Not (IsNothing(RadComboBoxCertifications.SelectedItem)) Then
    '        '    Select Case RadComboBoxCertifications.SelectedItem.Text
    '        '        Case "ARGUS Gold/Wyvern Registered/IS-BAO Stage I"
    '        '            ratings = " and (ARGUSlevel <> ''  or WYVERNlevel <> '' or Sentientlevel <> '' or ISBAOlevel <> '')"
    '        '        Case "ARGUS Gold Plus/IS-BAO Stage II"
    '        '            ratings = " and (ARGUSlevel in ('ARG/US Gold Plus', 'ARG/US Platinum') or WYVERNlevel <> '' or Sentientlevel <> '' or ISBAOlevel in ('IS-BAO Stage Two', 'IS-BAO Stage Three'))"
    '        '        Case "ARGUS Platinum/Wyvern Wingman/IS-BAO Stage III/Sentient Approved"
    '        '            ratings = " and (ARGUSlevel in ('ARG/US Platinum') or WYVERNlevel in ('WYVERN Wingman') or Sentientlevel in ('Sentient Certified') or ISBAOlevel in ('IS-BAO Stage Three'))"
    '        '        Case Else
    '        '            ratings = ""
    '        '    End Select
    '        'End If

    '        '20131118 - pab - add more fields to aircraft
    '        Dim ManufactureDate As Date
    '        'If Not (IsNothing(RadComboBoxMfcDates.SelectedItem)) Then
    '        '    Select Case RadComboBoxMfcDates.SelectedItem.Text
    '        '        Case "Any"
    '        '            'ok - do nothing
    '        '        Case "< 5 Years"
    '        '            ManufactureDate = DateAdd(DateInterval.Year, -5, Now)
    '        '        Case "< 10 Years"
    '        '            ManufactureDate = DateAdd(DateInterval.Year, -10, Now)
    '        '        Case "< 15 Years"
    '        '            ManufactureDate = DateAdd(DateInterval.Year, -15, Now)
    '        '        Case "< 20 Years"
    '        '            ManufactureDate = DateAdd(DateInterval.Year, -20, Now)
    '        '    End Select
    '        'End If

    '        '20120626 - pab - enable round trips - was originaly hardcoded one-way only
    '        Dim roundTrip As String = "False"
    '        '20120628 - pab - add one way/round trip radio buttons
    '        'If minuteswaitbetweensegments > 0 Then
    '        If Me.rblOneWayRoundTrip.SelectedValue = "RoundTrip" Then
    '            roundTrip = "True"
    '        End If


    '        '20130626 - pab - check parms before running quote
    '        '20131118 - pab - add more fields to aircraft
    '        Dim dsparms As DataSet = da.CheckQuoteParms(fromairport, toairport, CInt(pax), weightclass, options, ratings, ManufactureDate)
    '        '20151214 - pab - fix intermittant sql errors
    '        If IsNothing(dsparms) Then
    '            Insertsys_log(_carrierid, appName, "findflights CheckQuoteParms first lookup returned empty dataset - fromairport " & fromairport & _
    '                "; toairport " & toairport & "; pax " & pax.ToString & "; weightclass " & weightclass & "; options " & options & "; ratings " & ratings & _
    '                "; ManufactureDate " & ManufactureDate, "findflights", "home.aspx.vb")
    '            dsparms = da.CheckQuoteParms(fromairport, toairport, CInt(pax), weightclass, options, ratings, CDate("0001-01-01"))
    '        ElseIf dsparms.Tables.Count < 6 Then
    '            Insertsys_log(_carrierid, appName, "findflights CheckQuoteParms first lookup returned partial dataset - fromairport " & fromairport & _
    '                "; toairport " & toairport & "; pax " & pax.ToString & "; weightclass " & weightclass & "; options " & options & "; ratings " & ratings & _
    '                "; ManufactureDate " & ManufactureDate & "; table count " & dsparms.Tables.Count.ToString, "findflights", "home.aspx.vb")
    '            dsparms = da.CheckQuoteParms(fromairport, toairport, CInt(pax), weightclass, options, ratings, CDate("0001-01-01"))
    '        End If
    '        If IsNothing(dsparms) Then
    '            If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '            '20151214 - pab - fix intermittant sql errors
    '            'lblmsg.Text &= "The from or to airport is not serviced. Please select other airports."
    '            lblMsg.Text &= "Please update search criteria and try again."
    '            Insertsys_log(_carrierid, appName, "findflights CheckQuoteParms second lookup returned empty dataset - fromairport " & fromairport & _
    '                "; toairport " & toairport & "; pax " & pax.ToString & "; weightclass " & weightclass & "; options " & options & "; ratings " & ratings & _
    '                "; ManufactureDate " & ManufactureDate, "findflights", "home.aspx.vb")
    '        ElseIf dsparms.Tables.Count < 6 Then
    '            '20151214 - pab - fix intermittant sql errors
    '            If lblMsg.Text <> "" Then
    '                If InStr(lblMsg.Text, "update search criteria") = 0 Then
    '                    lblMsg.Text &= "<br />"
    '                    lblMsg.Text &= "Please update search criteria and try again."
    '                    Insertsys_log(_carrierid, appName, "findflights CheckQuoteParms second lookup returned partial dataset - fromairport " & fromairport & _
    '                        "; toairport " & toairport & "; pax " & pax.ToString & "; weightclass " & weightclass & "; options " & options & "; ratings " & ratings & _
    '                        "; ManufactureDate " & ManufactureDate & "; table count " & dsparms.Tables.Count.ToString, "findflights", "home.aspx.vb")
    '                End If
    '            End If
    '        Else
    '            'check if service airports
    '            '20151214 - pab - fix intermittant sql errors
    '            'If dsparms.Tables(0).Rows.Count <= 0 Then
    '            If dsparms.Tables(0).Rows.Count <= 0 Or dsparms.Tables(0).Rows(0).Item("TableType").ToString.ToLower <> "Airports".ToLower Then
    '                If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                lblMsg.Text &= "The from and to airports are not serviced. Please select other airports."
    '                '20160622 - pab - do not show other messages pre David
    '                lblMsg.Visible = True
    '                Exit Sub

    '            Else
    '                Dim bfrom As Boolean = False
    '                Dim bto As Boolean = False
    '                For i = 0 To dsparms.Tables(0).Rows.Count - 1
    '                    If dsparms.Tables(0).Rows(i).Item("locationid").ToString.Trim = fromairport Then
    '                        bfrom = True
    '                        Exit For
    '                    End If
    '                Next
    '                For i = 0 To dsparms.Tables(0).Rows.Count - 1
    '                    If dsparms.Tables(0).Rows(i).Item("locationid").ToString.Trim = toairport Then
    '                        bto = True
    '                        Exit For
    '                    End If
    '                Next
    '                If bfrom = False And bto = False Then
    '                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                    lblMsg.Text &= "The from and to airports are not serviced. Please select other airports."
    '                    '20160622 - pab - do not show other messages pre David
    '                    lblMsg.Visible = True
    '                    Exit Sub

    '                ElseIf bfrom = False Then
    '                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                    lblMsg.Text &= "The from airport is not serviced. Please select another departure airport."
    '                    '20160622 - pab - do not show other messages pre David
    '                    lblMsg.Visible = True
    '                    Exit Sub

    '                ElseIf bto = False Then
    '                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                    lblMsg.Text &= "The to airport is not serviced. Please select another arrival airport."
    '                    '20160622 - pab - do not show other messages pre David
    '                    lblMsg.Visible = True
    '                    Exit Sub

    '                End If
    '            End If

    '            'check runway lengths
    '            '20151214 - pab - fix intermittant sql errors
    '            'If dsparms.Tables(1).Rows.Count <= 0 Then
    '            If dsparms.Tables(1).Rows.Count <= 0 Or dsparms.Tables(1).Rows(0).Item("TableType").ToString.ToLower <> "Runways".ToLower Then
    '                If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                lblMsg.Text &= "The runways at the from and to airports are too short. Please select other airports."
    '            Else
    '                Dim bfrom As Boolean = False
    '                Dim bto As Boolean = False
    '                For i = 0 To dsparms.Tables(1).Rows.Count - 1
    '                    If dsparms.Tables(1).Rows(i).Item("locationid").ToString.Trim = fromairport Then
    '                        bfrom = True
    '                        Exit For
    '                    End If
    '                Next
    '                For i = 0 To dsparms.Tables(1).Rows.Count - 1
    '                    If dsparms.Tables(1).Rows(i).Item("locationid").ToString.Trim = toairport Then
    '                        bto = True
    '                        Exit For
    '                    End If
    '                Next
    '                If bfrom = False And bto = False Then
    '                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                    lblMsg.Text &= "The runways at the from and to airports are too short. Please select other airports."
    '                ElseIf bfrom = False Then
    '                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                    lblMsg.Text &= "The runway at the from airport is too short. Please select another departure airport."
    '                ElseIf bto = False Then
    '                    If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                    lblMsg.Text &= "The runway at the to airport is too short. Please select another arrival airport."
    '                End If
    '            End If

    '            'check passengers
    '            '20140416 - pab - remove this message 
    '            'If dsparms.Tables(2).Rows.Count <= 0 Then
    '            '    If lblmsg.Text <> "" Then lblmsg.Text &= "<br />"
    '            '    If weightclass <> "" Then
    '            '        lblmsg.Text &= "Passenger count exceeds passenger capacity of selected aircraft types. Please adjust passenger count or select larger aircraft types."
    '            '    Else
    '            '        lblmsg.Text &= "Passenger count exceeds passenger capacity of fleet. Please adjust passenger count."
    '            '    End If
    '            'End If

    '            'check weight classes
    '            '20151214 - pab - fix intermittant sql errors
    '            'If dsparms.Tables(3).Rows.Count <= 0 Then
    '            If dsparms.Tables(3).Rows.Count <= 0 Or dsparms.Tables(3).Rows(0).Item("TableType").ToString.ToLower <> "WeightClass".ToLower Then
    '                If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                If weightclass <> "" Then
    '                    lblMsg.Text &= "There are no aircraft of selected types available. Please change optional aircraft types to include."
    '                Else
    '                    lblMsg.Text &= "There are no aircraft of any type available."
    '                End If
    '            End If

    '            'check ratings
    '            '20151214 - pab - fix intermittant sql errors
    '            'If dsparms.Tables(5).Rows.Count <= 0 Then
    '            If dsparms.Tables(5).Rows.Count <= 0 Or dsparms.Tables(5).Rows(0).Item("TableType").ToString.ToLower <> "ratings".ToLower Then
    '                If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                If options <> "" Then
    '                    lblMsg.Text &= "There are no aircraft with selected certifications available. Please change certifications."
    '                Else
    '                    lblMsg.Text &= "There are no aircraft with any certifications available."
    '                End If
    '            End If

    '            'check options
    '            '20151214 - pab - fix intermittant sql errors
    '            'If dsparms.Tables(4).Rows.Count <= 0 Then
    '            If dsparms.Tables(4).Rows.Count <= 0 Or dsparms.Tables(4).Rows(0).Item("TableType").ToString.ToLower <> "services".ToLower Then
    '                If lblMsg.Text <> "" Then lblMsg.Text &= "<br />"
    '                If options <> "" Then
    '                    lblMsg.Text &= "There are no aircraft with selected options available. Please change optional services."
    '                Else
    '                    lblMsg.Text &= "There are no aircraft with any options available."
    '                End If
    '            End If

    '        End If

    '        If lblMsg.Text <> "" Then
    '            lblMsg.Visible = True
    '            Exit Sub
    '        End If

    '        '20160490 - pab - set session variables
    '        Session("reset") = "x"
    '        Session("passengers") = pax
    '        Session("origairportcode") = fromairport
    '        Session("destairportcode") = toairport
    '        Session("weightclass") = weightclass

    '        Dim dt As DataTable
    '        If weightclass = "" Then
    '            da.GetAircraftTypeServiceSpecsByWeightClass(_carrierid, "L")
    '        Else
    '            da.GetAircraftTypeServiceSpecsByWeightClass(_carrierid, weightclass)
    '        End If
    '        If Not isdtnullorempty(dt) Then
    '            If Not IsDBNull(dt.Rows(0).Item("currtype")) Then
    '                Session("currtype") = dt.Rows(0).Item("currtype")
    '            End If
    '        End If

    '        'for testing only 
    '        '_acg = "2"

    '        '20140310 - pab - use table instead of parms
    '        '20150317 - pab - remove acg branding
    '        'Dim bacg As Boolean = False
    '        'If _acg = "1" Or _acg = "2" Then bacg = True
    '        'Dim qn As Integer = da.InsertQuoteRequests(_carrierid, CInt(pax), fromaddr, toaddr, CBool(roundTrip), CDate(ld), CDate(rd), _
    '        '    CDate(lt).ToLongTimeString, CDate(rt).ToLongTimeString, fromairport, toairport, bAllowPets, bAllowSmoking, bWiFi, bLav, bPower, _
    '        '    bInFlightEntertainment, ManufactureDate, Nothing, bacg)
    '        'If qn > 0 Then

    '        '20160511 - pab - fix date reverting back to tomorrow
    '        ld = CDate(Me.depart_date.SelectedDate).ToShortDateString
    '        lt = departtime_combo.SelectedValue
    '        '20160716 - pab - fix error - Nullable object must have a value
    '        If Not IsNothing (Me.depart_date2.SelectedDate) Then
    '            rd = CDate(Me.depart_date2.SelectedDate).ToShortDateString
    '            rt = departtime_combo2.SelectedValue
    '        Else
    '            rd = cdate(DateAdd(DateInterval.Day, 1, cdate(ld))).ToShortDateString
    '            rt = "05:00 PM"
    '        End If

    '        '20120709 - pab - add lav, power
    '        '20131118 - pab - add more fields to aircraft - InflightEntertainment, ManufactureDate
    '        '20140310 - pab - acg background image
    '        '20140310 - pab - use table instead of parms
    '        '20150317 - pab - remove acg branding
    '        '20160117 - pab - quote multi-leg trips
    '        Dim script As String = "selectairports.aspx?" & _
    '                           "origAddr=" & fromaddr & _
    '                           "&destAddr=" & toaddr & _
    '                           "&roundTrip=" & roundTrip & _
    '                           "&passengers=" & pax & _
    '                           "&leaveDate=" & ld & _
    '                           "&returnDate=" & rd & _
    '                           "&leaveTime=" & lt & _
    '                           "&returnTime=" & rt & _
    '                           "&origAirportCode=" & fromairport & _
    '                           "&destAirportCode=" & toairport & _
    '                           "&AllowPets=" & bAllowPets.GetHashCode & _
    '                           "&AllowSmoking=" & bAllowSmoking.GetHashCode & _
    '                           "&WiFi=" & bWiFi.GetHashCode & _
    '                           "&EnclosedLav=" & bLav.GetHashCode & _
    '                           "&PowerAvailable=" & bPower.GetHashCode & _
    '                           "&InflightEntertainment=" & bInFlightEntertainment.GetHashCode & _
    '                           "&ManufactureDate=" & ManufactureDate & _
    '                            s
    '        '"&acg=" & _acg
    '        '    Dim script As String = "selectairports.aspx?" & _
    '        '            "qn=" & qn & _
    '        '            "&acg=" & _acg

    '        '20150330 - pab - force logon before getting quotes
    '        Dim bIsLoggedIn As Boolean = False
    '        Dim email As String = String.Empty
    '        If Not IsNothing(Session("IsLoggedIn")) Then bIsLoggedIn = CBool(Session("IsLoggedIn"))
    '        If Not IsNothing(Session("email")) Then email = Session("email").ToString
    '        If bIsLoggedIn = True And email <> "" Then
    '            Response.Redirect(script)
    '        Else
    '            script = Replace(script, "&", "||")
    '            Response.Redirect("customerLogin.aspx?ReturnUrl=" & script)
    '        End If

    '        'End If

    '    Catch ex As Exception
    '        Dim serr As String = ex.Message
    '        If serr <> "Thread was being aborted." Then
    '            If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
    '            If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
    '            AirTaxi.Insertsys_log(0, AirTaxi.appName, serr, "home2.aspx findflights", "")
    '            AirTaxi.SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " home2.aspx findflights Error", _
    '                              serr, _carrierid)
    '        End If
    '    End Try

    'End Sub


    '20120625 - pab - remove duplicate code
    'Shared Function InBetween(ByVal Start As Integer, ByVal work As String, ByVal target As String, ByVal target2 As String) As String

    '    Dim pos As Integer
    '    Dim pos2 As Integer
    '    Dim work1 As String


    '    If Start = 0 Then
    '        InBetween = ""
    '        Exit Function
    '    End If

    '    pos = InStr(Start, work, target, vbTextCompare)

    '    pos = pos + Len(target)
    '    pos2 = InStr(pos, work, target2, vbTextCompare)

    '    If pos = 0 + Len(target) Or pos2 = 0 Then
    '        InBetween = ""
    '        Exit Function
    '    End If


    '    work1 = Mid(work, pos, pos2 - pos)

    '    InBetween = work1

    'End Function

    Protected Sub cmdQuote_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdQuote.Click

        lblMsg.Text = ""
        lblMsg.Visible = False

        'findflights()

    End Sub


    '20150511 - pab - fix airport list too short
    'Private Function GetAirportsLatLong(ByVal latlong As LatLong, ByVal minRunwayLength As Integer, ByVal miles As Integer, _
    '        ByVal airportcount As Integer) As DataTable

    '    AirTaxi.post_timing("home.aspx.vb getairports start  " & Now.ToString)

    '    Dim oMapping As New Mapping

    '    Dim ds As DataSet = Nothing

    '    Dim maxRunwayLength As Integer = 0

    '    Dim da As New DataAccess
    '    'find nearby airports
    '    '20150511 - pab - fix airport list too short
    '    ds = da.GetNearestAirportsByLatitudeLongitudeWithinDistance(latlong.Latitude, latlong.Longitude, minRunwayLength, miles, airportcount)

    '    Dim dt As New DataTable
    '    dt = ds.Tables(0)

    '    '20150507 - pab - add more logging for lookup issue
    '    If Not isdtnullorempty(dt) Then
    '        If dt.Rows.Count < 6 Then
    '            Insertsys_log(_carrierid, appName, "lookup returned  < 6 - lat " & latlong.Latitude & "; long " & latlong.Longitude & _
    '                "; minRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "GetAirportsLatLong", "home.aspx.vb")
    '        End If
    '    Else
    '        Insertsys_log(_carrierid, appName, "lookup returned no results - lat " & latlong.Latitude & "; long " & latlong.Longitude & _
    '            "; minRunwayLength " & minRunwayLength & "; miles " & miles & "; airportcount " & airportcount, "GetAirportsLatLong", "home.aspx.vb")
    '    End If

    '    AirTaxi.post_timing("home.aspx.vb getairports end  " & Now.ToString)

    '    Return dt

    'End Function

    '20120628 - pab - add one way/round trip radio buttons
    Private Sub rblOneWayRoundTrip_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblOneWayRoundTrip.SelectedIndexChanged

        'DisplayBoxBasedRadioBtns()
        ''Session("newselection") = "T"
        ''If rblOneWayRoundTrip.SelectedValue = "MultiLeg" Then
        ''    Session("triptype") = "M"
        ''    Session("ShowMulti") = "M"
        ''    Response.Redirect("selectairports.aspx")
        ''    Exit Sub
        ''End If

    End Sub

    ''20120628 - pab - add one way/round trip radio buttons
    'Private Sub DisplayBoxBasedRadioBtns()

    '    If Request.QueryString.Count = 0 Then
    '        'Me.depart_txt.Text = DateAdd(DateInterval.Day, 1, Now).ToShortDateString
    '        Me.depart_date.SelectedDate = CDate(DateAdd(DateInterval.Day, 1, Now).ToShortDateString)
    '        Me.departtime_combo.SelectedIndex = Me.departtime_combo.Items.IndexOf(Me.departtime_combo.Items.FindByText("09:00 AM"))

    '    Else
    '        If Not IsNothing(Request.QueryString("legs")) Then
    '            'Me.depart_txt.Text = DateAdd(DateInterval.Day, 1, Now).ToShortDateString
    '            Me.depart_date.SelectedDate = CDate(DateAdd(DateInterval.Day, 1, Now).ToShortDateString)
    '            Me.departtime_combo.SelectedIndex = Me.departtime_combo.Items.IndexOf(Me.departtime_combo.Items.FindByText("09:00 AM"))

    '        Else

    '            If Not IsNothing(Request.QueryString("leaveDate")) Then
    '                'Me.depart_txt.Text = Request.QueryString("leaveDate").ToString
    '                Me.depart_date.SelectedDate = Request.QueryString("leaveDate").ToString
    '            Else
    '                'Me.depart_txt.Text = DateAdd(DateInterval.Day, 1, Now).ToShortDateString
    '                Me.depart_date.SelectedDate = CDate(DateAdd(DateInterval.Day, 1, Now).ToShortDateString)
    '            End If

    '            'If Not IsNothing(Request.QueryString("returnDate")) Then
    '            '    Me.calReturn.SelectedDate = CDate(Request.QueryString("returnDate"))
    '            'Else
    '            '    Me.calReturn.SelectedDate = CDate(DateAdd(DateInterval.Day, 2, Now).ToShortDateString)
    '            'End If

    '            If Not IsNothing(Request.QueryString("leaveTime")) Then
    '                Me.departtime_combo.SelectedIndex = _
    '                Me.departtime_combo.Items.IndexOf( _
    '                    Me.departtime_combo.Items.FindByText(Request.QueryString("returnTime"))) + 1
    '            Else
    '                Me.departtime_combo.SelectedIndex = Me.departtime_combo.Items.IndexOf(Me.departtime_combo.Items.FindByText("09:00 AM"))
    '            End If

    '            'If Not IsNothing(Request.QueryString("leaveTime")) Then
    '            '    Me.ddlTimeReturn.SelectedIndex = _
    '            '    Me.ddlTimeReturn.Items.IndexOf( _
    '            '        Me.ddlTimeReturn.Items.FindByText(Request.QueryString("returnTime")))
    '            'Else
    '            '    Me.ddlTimeReturn.SelectedIndex = Me.ddlTimeReturn.Items.IndexOf(Me.ddlTimeReturn.Items.FindByText("05:00 PM"))
    '            'End If

    '            ''20160310 - pab - fix empty fields after multileg quote
    '            'If CDate(Me.calReturn.SelectedDate.ToShortDateString & " " & ddlTimeReturn.Text) < DateAdd(DateInterval.Minute, 90, _
    '            '        CDate(Me.calLeave.SelectedDate.ToShortDateString & " " & ddlTimeLeave.Text)) Then
    '            '    Me.calReturn.SelectedDate = CDate(DateAdd(DateInterval.Day, 1, Me.calLeave.SelectedDate).ToShortDateString)
    '            'End If

    '        End If

    '    End If

    '    Select Case Me.rblOneWayRoundTrip.SelectedValue
    '        Case "RoundTrip"
    '            Dim ssender As String

    '            If Me.pnlLeg10.Visible = True Then
    '                ssender = "bttnRemoveLeg10"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg9.Visible = True Then
    '                ssender = "bttnRemoveLeg9"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg8.Visible = True Then
    '                ssender = "bttnRemoveLeg8"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg7.Visible = True Then
    '                ssender = "bttnRemoveLeg7"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg6.Visible = True Then
    '                ssender = "bttnRemoveLeg6"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg5.Visible = True Then
    '                ssender = "bttnRemoveLeg5"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg4.Visible = True Then
    '                ssender = "bttnRemoveLeg4"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg3.Visible = True Then
    '                ssender = "bttnRemoveLeg3"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg2.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)
    '            End If

    '            If Me.DestinationAddress.SelectedValue.ToString <> "" Then
    '                Me.OriginAddress2.Text = Me.DestinationAddress.SelectedValue.ToString
    '            End If

    '            If Me.OriginAddress.SelectedValue.ToString <> "" Then
    '                Me.DestinationAddress2.Text = Me.OriginAddress.SelectedValue.ToString
    '            End If

    '            Session("triptype") = "R"
    '            Session("showcal") = "Y"

    '            '20160117 - pab - quote multi-leg trips
    '        Case "MultiLeg"
    '            Session("triptype") = "M"
    '            Session("showcal") = "N"
    '            'pnlOneWayRT.Visible = False
    '            'pnlMultiLeg.Visible = True

    '            If Me.pnlLeg10.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress9.Text <> "" Then
    '                    Me.OriginAddress10.Text = Me.DestinationAddress9.SelectedValue.ToString
    '                End If

    '            ElseIf Me.pnlLeg9.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress8.Text <> "" Then
    '                    Me.OriginAddress9.Text = Me.DestinationAddress8.SelectedValue.ToString
    '                End If

    '            ElseIf Me.pnlLeg8.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress7.Text <> "" Then
    '                    Me.OriginAddress8.Text = Me.DestinationAddress7.SelectedValue.ToString
    '                End If

    '            ElseIf Me.pnlLeg7.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress6.Text <> "" Then
    '                    Me.OriginAddress7.Text = Me.DestinationAddress6.SelectedValue.ToString
    '                End If

    '            ElseIf Me.pnlLeg6.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress5.Text <> "" Then
    '                    Me.OriginAddress6.Text = Me.DestinationAddress5.SelectedValue.ToString
    '                End If

    '            ElseIf Me.pnlLeg5.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress4.Text <> "" Then
    '                    Me.OriginAddress5.Text = Me.DestinationAddress4.SelectedValue.ToString
    '                End If

    '            ElseIf Me.pnlLeg4.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress3.Text <> "" Then
    '                    Me.OriginAddress4.Text = Me.DestinationAddress3.SelectedValue.ToString
    '                End If

    '            ElseIf Me.pnlLeg3.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress2.Text <> "" Then
    '                    Me.OriginAddress3.Text = Me.DestinationAddress2.SelectedValue.ToString
    '                End If

    '            ElseIf Me.pnlLeg2.Visible = False Then
    '                bttnAddLeg_Click(Nothing, Nothing)

    '                If Me.DestinationAddress.SelectedValue.ToString <> "" Then
    '                    Me.OriginAddress2.Text = Me.DestinationAddress.SelectedValue.ToString
    '                End If

    '            End If

    '        Case "OneWay"
    '            Session("triptype") = "O"
    '            Session("showcal") = "Y"

    '            Dim ssender As String

    '            If Me.pnlLeg10.Visible = True Then
    '                ssender = "bttnRemoveLeg10"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg9.Visible = True Then
    '                ssender = "bttnRemoveLeg9"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg8.Visible = True Then
    '                ssender = "bttnRemoveLeg8"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg7.Visible = True Then
    '                ssender = "bttnRemoveLeg7"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg6.Visible = True Then
    '                ssender = "bttnRemoveLeg6"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg5.Visible = True Then
    '                ssender = "bttnRemoveLeg5"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg4.Visible = True Then
    '                ssender = "bttnRemoveLeg4"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg3.Visible = True Then
    '                ssender = "bttnRemoveLeg3"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '            If Me.pnlLeg2.Visible = True Then
    '                ssender = "bttnRemoveLeg2"
    '                removeleg(Nothing, Nothing, ssender)
    '            End If

    '        Case Else

    '    End Select

    'End Sub

    ''20140310 - pab - acg background image
    'Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

    '    'AirTaxi.Insertsys_log(0, appName, Left("Request.UrlReferrer " & Request.UrlReferrer.ToString, 500), "Page_Load", "SelectAirports.aspx.vb")
    '    '20150317 - pab - remove acg branding
    '    'If IsNothing(_acg) Then _acg = ""
    '    ' ''20140528 - pab - fix default going to acg
    '    ''Insertsys_log(58, appName, "acg - " & _acg.ToString, "Page_PreRender", "home.aspx.vb")
    '    'If Not Request.QueryString("acg") Is Nothing Then
    '    '    _acg = Request.QueryString("acg")
    '    '    ''20140528 - pab - fix default going to acg
    '    '    'Insertsys_log(58, appName, "Request.QueryString acg - " & _acg.ToString, "Page_PreRender", "home.aspx.vb")
    '    'End If

    '    '_acg = "2"     'for testing only

    '    'bg_acg.Style.Remove("class")
    '    'bg_acg.Style.Remove("background-image")
    '    'justification.Style.Remove("class")
    '    'justification.Style.Remove("background-image")
    '    Content.Style.Remove("class")
    '    Content.Style.Remove("background-image")
    '    lblTagLine.Visible = True

    '    '20150317 - pab - remove acg branding
    '    'imgACGLogo.Visible = False

    '    'RadAjaxPanel1.BackImageUrl = ""
    '    pflybody.Style.Remove("background")
    '    pflybody.Style.Remove("filter")
    '    widget.Style.Remove("background-image")
    '    PleaseWait.Style.Remove("color")
    '    lnkBrokerLogin.ForeColor = Color.Blue

    '    '20150317 - pab - remove acg branding
    '    'lnkBrokerLoginacg.ForeColor = Color.Blue

    '    lnkBrokerLogout.ForeColor = Color.Blue
    '    lblDepart.ForeColor = Color.Black
    '    lblReturn.ForeColor = Color.Black
    '    'Me.OriginAddress.ForeColor = Color.Black
    '    'Me.DestinationAddress.ForeColor = Color.Black
    '    'Me.DestinationAddress.ForeColor = Color.Black
    '    'RadComboBoxPax.ForeColor = Color.Black
    '    'RadComboBoxFlexMiles.ForeColor = Color.Black
    '    'RadComboFlexFrom.ForeColor = Color.Black
    '    'RadComboBoxFlexTo.ForeColor = Color.Black
    '    'RadComboBoxACInclude.ForeColor = Color.Black
    '    'Me.OriginAddress.BackColor = Color.FromArgb(255, 255, 255)   '#ffffff
    '    'Me.DestinationAddress.BackColor = Color.FromArgb(255, 255, 255)   '#ffffff
    '    'Me.DestinationAddress.BackColor = Color.FromArgb(255, 255, 255)   '#ffffff
    '    'RadComboBoxPax.BackColor = Color.FromArgb(255, 255, 255)   '#ffffff
    '    'RadComboBoxFlexMiles.BackColor = Color.FromArgb(255, 255, 255)   '#ffffff
    '    'RadComboFlexFrom.BackColor = Color.FromArgb(255, 255, 255)   '#ffffff
    '    'RadComboBoxFlexTo.BackColor = Color.FromArgb(255, 255, 255)   '#ffffff
    '    'RadComboBoxACInclude.BackColor = Color.FromArgb(255, 255, 255)   '#ffffff
    '    If Session("IsLoggedIn") = False Then
    '        lnkBrokerLogin.Visible = True
    '        '20150317 - pab - remove acg branding
    '        'lnkBrokerLoginacg.Visible = False
    '        lnkBrokerLogout.Visible = False
    '    Else
    '        lnkBrokerLogin.Visible = False
    '        '20150317 - pab - remove acg branding
    '        'lnkBrokerLoginacg.Visible = False
    '        lnkBrokerLogout.Visible = True
    '    End If
    '    bottomnavlogo.Visible = False
    '    '20150317 - pab - remove acg branding
    '    'If _acg = "1" Or _acg = "2" Then
    '    '    pflybody.Style.Remove("background-image")

    '    '    bimage.Src = "images/Background_1500x1000.jpg"
    '    '    bimage.Visible = True

    '    '    ''bg_acg.Style.Add("class", "bg_acg")
    '    '    ''bg_acg.Style.Add("background-image", "url('../images/Background_1500x1000.jpg')")
    '    '    'content.Style.Add("class", "home_acg")
    '    '    'content.Style.Add("background-image", "url('../images/Background_1500x1000.jpg')")
    '    '    lblTagLine.Visible = False
    '    '    imgACGLogo.Visible = True
    '    '    ''RadAjaxPanel1.BackImageUrl = "~/images/acgwidget.png"
    '    '    'pflybody.Style.Add("background", "#e6e1d6;-moz-linear-gradient(top,  #e6e1d6 0%, #d8d4ca 46%, #d8d4ca 100%);-webkit-gradient(linear, left top, left bottom, color-stop(0%,#e6e1d6), color-stop(46%,#d8d4ca), color-stop(100%,#d8d4ca));-webkit-linear-gradient(top,  #e6e1d6 0%,#d8d4ca 46%,#d8d4ca 100%);-o-linear-gradient(top,  #e6e1d6 0%,#d8d4ca 46%,#d8d4ca 100%);-ms-linear-gradient(top,  #e6e1d6 0%,#d8d4ca 46%,#d8d4ca 100%);linear-gradient(to bottom,  #e6e1d6 0%,#d8d4ca 46%,#d8d4ca 100%);")
    '    '    'pflybody.Style.Add("filter", "progid:DXImageTransform.Microsoft.gradient( startColorstr='#e6e1d6', endColorstr='#d8d4ca',GradientType=0 );")
    '    '    ''widget.Style.Add("background-image", "url('../images/acgwidget.png')")
    '    '    widget.Style.Add("background-image", "url('../images/DarkBlueBox_400x500.png')")
    '    '    PleaseWait.Style.Add("color", "#FFFFFF")
    '    '    lnkBrokerLogin.ForeColor = Color.White
    '    '    lnkBrokerLoginacg.ForeColor = Color.White
    '    '    lnkBrokerLogout.ForeColor = Color.White
    '    '    lblDepart.ForeColor = Color.White
    '    '    lblReturn.ForeColor = Color.White
    '    '    'Me.OriginAddress.ForeColor = Color.White
    '    '    'Me.DestinationAddress.ForeColor = Color.White
    '    '    'Me.DestinationAddress.ForeColor = Color.White
    '    '    'RadComboBoxPax.ForeColor = Color.White
    '    '    'RadComboBoxFlexMiles.ForeColor = Color.White
    '    '    'RadComboFlexFrom.ForeColor = Color.White
    '    '    'RadComboBoxFlexTo.ForeColor = Color.White
    '    '    'RadComboBoxACInclude.ForeColor = Color.White
    '    '    'Me.OriginAddress.BackColor = Color.FromArgb(17, 122, 179)   '#117ab3
    '    '    'Me.DestinationAddress.BackColor = Color.FromArgb(17, 122, 179)   '#117ab3
    '    '    'Me.DestinationAddress.BackColor = Color.FromArgb(17, 122, 179)   '#117ab3
    '    '    'RadComboBoxPax.BackColor = Color.FromArgb(17, 122, 179)   '#117ab3
    '    '    'RadComboBoxFlexMiles.BackColor = Color.FromArgb(17, 122, 179)   '#117ab3
    '    '    'RadComboFlexFrom.BackColor = Color.FromArgb(17, 122, 179)   '#117ab3
    '    '    'RadComboBoxFlexTo.BackColor = Color.FromArgb(17, 122, 179)   '#117ab3
    '    '    'RadComboBoxACInclude.BackColor = Color.FromArgb(17, 122, 179)   '#117ab3
    '    '    If Session("IsLoggedIn") = False Then
    '    '        lnkBrokerLogin.Visible = False
    '    '        lnkBrokerLoginacg.Visible = True
    '    '    End If
    '    '    bottomnavlogo.Visible = True
    '    'Else
    '    pflybody.Style.Remove("class")
    '    Content.Style.Remove("background-image")
    '    Content.Style.Add("background-image", "url('images/personifly_CAT_logo_4.png')")
    '    bimage.Visible = False
    '    PleaseWait.Style.Add("color", "#666")
    '    'End If

    'End Sub

    'Private Sub RadComboFlexFrom_SelectedIndexChanged(sender As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles RadComboFlexFrom.SelectedIndexChanged

    '    'lblFrom.Visible = True
    '    'RadComboFlexFromTime.Visible = True
    '    'lblTo.Visible = True
    '    'RadComboFlexToTime.Visible = True

    'End Sub

    '20140224 - pab - add threading for airport drop downs
    'Sub wakeupwebservice(a As String)

    '    Try

    '        '20151009 - pab - this sub did not call the web service to wake it up....
    '        Dim ws As New AviationWebService1_10.WebService1
    '        ws.Alive("pbaumgart@ctgi.com", "123", _carrierid)

    '        ''20120622 - pab - remove hardcoded connection string
    '        'Dim adapter As New SqlDataAdapter("", ConnectionStringHelper.GetConnectionString)


    '        '20120621 - pab - fix dropdown for airport codes
    '        '20120823 - pab - get major airports by lat long
    '        Dim fromloc As String = String.Empty
    '        'If Left(e.Text.ToUpper, 1) = "K" And Len(e.Text.Trim) = 4 Then
    '        '    fromloc = Mid(e.Text.ToUpper, 2)
    '        'Else
    '        'fromloc = "KPBI"
    '        fromloc = "PBI"
    '        'End If

    '        ''20130306 - pab - allow bahamian airports
    '        ''adapter.SelectCommand.CommandText = ("SELECT  rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, rtrim(icao) as icao, " & _
    '        ''    "rtrim(iata) as locationid , 0 as miles from ICAO_IATA_2 i left join NfdcFacilities nf on i.iata = nf.locationid WHERE (icao  LIKE '%' + @text + '%' " & _
    '        ''    "OR iata LIKE @text + '%' OR name LIKE @text + '%') and iata <> ''  and region <> 'INTL' ")
    '        'adapter.SelectCommand.CommandText = ("SELECT  rtrim(name) + ' (' + rtrim(icao) +  ')' as airport,  rtrim(name) as facilityname, rtrim(icao) as icao, " & _
    '        '    "rtrim(iata) as locationid , 0 as miles from ICAO_IATA_2 i left join NfdcFacilities nf on i.iata = nf.locationid WHERE (icao  LIKE '%' + @text + '%' " & _
    '        '    "OR iata LIKE @text + '%' OR name LIKE @text + '%') and iata <> ''  and (region <> 'INTL' or country = 'bahamas') ")

    '        ''adapter.SelectCommand.Parameters.AddWithValue("@text", e.Text)
    '        'adapter.SelectCommand.Parameters.AddWithValue("@text", fromloc)

    '        Dim dt As New DataTable()
    '        'adapter.Fill(dt)

    '        ''20120823 - pab - fix old airports left displaying while ddl loading
    '        ''Me.OriginAddress.Items.Clear()

    '        '20120823 - pab - get major airports by lat long
    '        Dim da As New DataAccess
    '        Dim ds As New DataSet
    '        'Dim slocations As String = String.Empty


    '        '20120823 - pab - get major airports by lat long
    '        ds = da.GetAirportInformationByAirportCode(fromloc)
    '        'If Not IsNothing(ds) Then
    '        '    If Not isdtnullorempty(ds.Tables(0)) Then
    '        '        dt = da.GetMajorAirportsByLatitudeLongitude(ds.Tables(0).Rows(0).Item("Latitude"), ds.Tables(0).Rows(0).Item("Longitude"), 3000, 50, 5)

    '        '    End If
    '        'End If




    '        ''dt = findairports(fromloc)




    '        ''20150511 - pab - fix airport list too short
    '        'dt = findairports("Palm Beach, FL", 2800, 50, 50)




    '    Catch ex As Exception
    '        Dim serr As String = ex.Message
    '        If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
    '        If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
    '        AirTaxi.SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " home2.aspx wakeupwebservice Error", _
    '                          serr, _carrierid)
    '    End Try

    'End Sub

    ''20160117 - pab - quote multi-leg trips
    'Protected Sub cmdAddLeg_Click(sender As Object, e As EventArgs) Handles cmdAddLeg.Click

    '    Try
    '        ''Dim trip As New ArrayList
    '        'Dim tripleg As New leg

    '        ''trip = Me.lbLegs.Items(0)

    '        'tripleg.fromloc = "FXE"
    '        'tripleg.toloc = "HPN"
    '        'tripleg.depart = DateAdd(DateInterval.Day, 1, Now)

    '        'trip.Add(tripleg)

    '        lblMsg.Text = ""
    '        editflights()
    '        If lblMsg.Text <> "" Then
    '            Exit Sub
    '        End If

    '        If Not IsNothing(Me.DestinationAddress.SelectedValue) Then
    '            Me.lbLegs.Items.Add(Me.OriginAddress.SelectedValue & " to " & Me.DestinationAddress.SelectedValue & " at " & CDate(Me.depart_date.SelectedDate.Value).ToString)
    '            Me.OriginAddress.Items.Clear()
    '            Dim ti As New Telerik.Web.UI.RadComboBoxItem
    '            ti.Text = Me.DestinationAddress.Text
    '            ti.Value = Me.DestinationAddress.SelectedValue
    '            Me.OriginAddress.Items.Add(ti)
    '            Me.OriginAddress.Text = Me.DestinationAddress.Text
    '            Me.OriginAddress.SelectedIndex = 0
    '            Me.DestinationAddress.Text = ""
    '            Me.DestinationAddress.Items.Clear()
    '            Me.depart_date.SelectedDate = DateAdd(DateInterval.Hour, 6, Me.depart_date.SelectedDate.Value)
    '            Me.DestinationAddress.Focus()

    '            'HydrateLegs()

    '        End If

    '    Catch ex As Exception
    '        Dim s As String = ex.Message
    '    End Try

    'End Sub

    ''20160117 - pab - quote multi-leg trips
    'Sub editflights(ByRef TurnAroundMinutes As Integer)

    '    Dim ld As String = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy")
    '    Dim rd As String = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy")
    '    Dim lt As String = "09:00 AM"
    '    Dim rt As String = "05:00 PM"

    '    Dim ma As Integer = 120
    '    Dim d As Integer = 0
    '    Dim bintl As Boolean = False
    '    Dim leavedate As DateTime
    '    Dim returndate As DateTime

    '    If Not (IsNothing(Me.depart_date.SelectedDate)) Then
    '        If IsDate(Me.depart_date.SelectedDate.Value) Then
    '            leavedate = CDate(Me.depart_date.SelectedDate.Value & " " & Me.departtime_combo.SelectedValue)
    '            ld = leavedate.ToString("MM/dd/yyyy")
    '            lt = Me.departtime_combo.SelectedValue

    '            '20141114 - pab - do not allow date less than current date
    '            If leavedate <= Now Then
    '                lblMsg.Text = "Depart Date must be in the future"
    '                lblMsg.Visible = True

    '                Exit Sub
    '            End If
    '        End If
    '    End If

    '    '20120626 - pab - edit return date
    '    Dim minuteswaitbetweensegments As Long = 0

    '    If Not (IsNothing(Me.depart_date2.SelectedDate)) Then
    '        If IsDate(Me.depart_date2.SelectedDate.Value) Then
    '            returndate = CDate(Me.depart_date2.SelectedDate.Value & " " & Me.departtime_combo2.SelectedValue)
    '            rd = returndate.ToString("MM/dd/yyyy")
    '            rt = Me.departtime_combo2.SelectedValue

    '            '20120626 - pab - edit return date
    '            If Session("triptype") <> "O" Then
    '                If IsDate(rd & " " & rt) Then
    '                    minuteswaitbetweensegments = DateDiff(DateInterval.Minute, CDate(ld & " " & lt), CDate(rd & " " & rt))
    '                    If minuteswaitbetweensegments <= 0 Then
    '                        lblMsg.Text = "Return Date must be after Depart Date"
    '                        lblMsg.Visible = True

    '                        Exit Sub
    '                    End If
    '                End If
    '            End If

    '        End If
    '    End If

    '    Dim fromairport As String
    '    Dim toairport As String
    '    '20120621 - pab - fix if iata code entered (did not selct from ddl)
    '    Dim fromaddr As String = String.Empty
    '    Dim toaddr As String = String.Empty
    '    Dim da As New DataAccess
    '    Dim ds As New DataSet

    '    '20120830 - pab - fix error when back button on selectairports clicked and ueser returns here
    '    If Me.OriginAddress.Text.Trim <> "" Then
    '        If InStr(Me.OriginAddress.Text, "(") = 0 Then
    '            fromaddr = Me.OriginAddress.Text
    '            '20120824 - pab - fix if airport not selected from ddl
    '            If fname(fromaddr) <> fromaddr Then
    '                fromairport = fromaddr.ToUpper
    '            Else
    '                ds = da.GetIATAcodebyICAO(fromaddr)
    '                If Not IsNothing(ds) Then
    '                    If Not isdtnullorempty(ds.Tables(0)) Then
    '                        fromairport = ds.Tables(0).Rows(0).Item("iata").ToString.Trim
    '                    Else
    '                        lblMsg.Text &= "Please select from airport. "
    '                        lblMsg.Visible = True

    '                    End If
    '                End If
    '            End If
    '        Else
    '            'fromairport = InBetween(1, Me.OriginAddress.Text, "(K", ")")
    '            fromairport = Me.OriginAddress.SelectedValue
    '            fromaddr = Me.OriginAddress.Text
    '        End If
    '    Else
    '        lblMsg.Text &= "Enter from location. "
    '        lblMsg.Visible = True

    '    End If

    '    '20120830 - pab - fix error when back button on selectairports clicked and ueser returns here
    '    If Me.DestinationAddress.Text.Trim <> "" Then
    '        If InStr(Me.DestinationAddress.Text, "(") = 0 Then
    '            toaddr = Me.DestinationAddress.Text
    '            '20120824 - pab - fix if airport not selected from ddl
    '            If fname(toaddr) <> toaddr Then
    '                toairport = toaddr.ToUpper
    '            Else
    '                ds = da.GetIATAcodebyICAO(toaddr)
    '                If Not IsNothing(ds) Then
    '                    If Not isdtnullorempty(ds.Tables(0)) Then
    '                        toairport = ds.Tables(0).Rows(0).Item("iata").ToString.Trim
    '                    Else
    '                        lblMsg.Text &= "Please select to airport. "
    '                        lblMsg.Visible = True

    '                    End If
    '                End If
    '            End If
    '        Else
    '            'toairport = InBetween(1, Me.DestinationAddress.Text, "(K", ")")
    '            toairport = Me.DestinationAddress.SelectedValue
    '            toaddr = Me.DestinationAddress.Text
    '        End If
    '    Else
    '        lblMsg.Text &= "Enter to location. "
    '        lblMsg.Visible = True

    '    End If

    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
    '    ma += TurnAroundMinutes    'add turn time
    '    If Session("triptype") <> "O" Then
    '        If returndate < DateAdd(DateInterval.Minute, ma, leavedate) Then
    '            '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
    '            'If Session("triptype") = "R" Then
    '            '    lblMsg.Text = "Round trip cannot be performed in the requested time frame"
    '            'Else
    '            '    lblMsg.Text = "Multi-leg trip cannot be performed in the requested time frame"
    '            'End If
    '            'lblMsg.Visible = True
    '            'Exit Sub
    '            Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leavedate)
    '            If DateDiff(DateInterval.Day, newdate, returndate) > 0 Then
    '                Me.depart_date2.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, returndate), 
    '                    cdate(Me.depart_date2.SelectedDate))
    '            End If
    '            For i As Integer = 0 To departtime_combo2.Items.Count - 1
    '                If IsDate(departtime_combo2.Items(i).Value) Then
    '                    If cdate(CDate(departtime_combo2.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                        departtime_combo2.SelectedValue = departtime_combo2.Items(i).Value
    '                        Exit For
    '                    End If
    '                End If
    '            Next
    '        End If
    '    End If

    '    '20150424 - pab - fix locations not being found 
    '    If lblMsg.Text <> "" Then
    '        lblMsg.Visible = True
    '        Exit Sub
    '    End If

    'End Sub

    '20160117 - pab - quote multi-leg trips
    'Function editflightsmultileg(ByRef TurnAroundMinutes As Integer) As String

    '    Dim prevairport As String
    '    Dim fromairport As String
    '    Dim toairport As String
    '    Dim leave1 As DateTime
    '    Dim leave2 As DateTime
    '    Dim s As String = ""

    '    editflightsmultileg = ""

    '    editflights(TurnAroundMinutes)
    '    If lblMsg.Text <> "" Then Exit Function

    '    leave1 = CDate(depart_date.SelectedDate & " " & departtime_combo.SelectedValue)

    '    If Me.OriginAddress.SelectedValue.Trim <> "" Then
    '        editflightsmultileg = Me.OriginAddress.SelectedValue.Trim
    '    Else
    '        editflightsmultileg = Me.OriginAddress.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave1 & ";"

    '    If Me.pnlLeg2.Visible = False Then Exit Function

    '    If Me.OriginAddress2.Text.Trim = "" And Me.OriginAddress2.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress2.Text.Trim = "" And Me.DestinationAddress2.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date2.SelectedDate & " " & departtime_combo2.SelectedValue)

    '    '20120626 - pab - edit return date
    '    Dim minuteswaitbetweensegments As Long = 0
    '    '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress2.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress2.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress2.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress2.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress2.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress2.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    If Me.pnlLeg3.Visible = False Then Exit Function

    '    If Me.OriginAddress3.Text.Trim = "" And Me.OriginAddress3.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress3.Text.Trim = "" And Me.DestinationAddress3.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date3.SelectedDate & " " & departtime_combo3.SelectedValue)
    '    Dim d As Double = 0
    '    Dim ma As Integer = 0
    '    Dim bintl As Boolean = False
    '    Dim da As New DataAccess
    '    If Me.OriginAddress3.SelectedValue.Trim = "" Then
    '        fromairport = Me.OriginAddress3.Text.Trim
    '    Else
    '        fromairport = Me.OriginAddress3.SelectedValue.Trim
    '    End If
    '    If Me.DestinationAddress3.SelectedValue.Trim = "" Then
    '        toairport = Me.DestinationAddress3.Text.Trim
    '    Else
    '        toairport = Me.DestinationAddress3.SelectedValue.Trim
    '    End If
    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    ma += TurnAroundMinutes    'add turn time
    '    If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
    '        Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
    '        If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
    '            Me.depart_date3.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2), 
    '                cdate(Me.depart_date3.SelectedDate))
    '        End If
    '        For i As Integer = 0 To departtime_combo3.Items.Count - 1
    '            If IsDate(departtime_combo3.Items(i).Value) Then
    '                If cdate(CDate(departtime_combo3.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                    departtime_combo3.SelectedValue = departtime_combo3.Items(i).Value
    '                    Exit For
    '                End If
    '            End If
    '        Next

    '        leave2 = CDate(Me.depart_date3.SelectedDate & " " & departtime_combo3.SelectedValue)
    '    End If

    '    'minuteswaitbetweensegments = 0
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress3.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress3.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress3.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress3.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress3.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress3.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    If Me.pnlLeg4.Visible = False Then Exit Function

    '    If Me.OriginAddress4.Text.Trim = "" And Me.OriginAddress4.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress4.Text.Trim = "" And Me.DestinationAddress4.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date4.SelectedDate & " " & departtime_combo4.SelectedValue)
    '    If Me.OriginAddress4.SelectedValue.Trim = "" Then
    '        fromairport = Me.OriginAddress4.Text.Trim
    '    Else
    '        fromairport = Me.OriginAddress4.SelectedValue.Trim
    '    End If
    '    If Me.DestinationAddress4.SelectedValue.Trim = "" Then
    '        toairport = Me.DestinationAddress4.Text.Trim
    '    Else
    '        toairport = Me.DestinationAddress4.SelectedValue.Trim
    '    End If
    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    ma += TurnAroundMinutes    'add turn time
    '    If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
    '        Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
    '        If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
    '            Me.depart_date4.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2), 
    '                cdate(Me.depart_date4.SelectedDate))
    '        End If
    '        For i As Integer = 0 To departtime_combo4.Items.Count - 1
    '            If IsDate(departtime_combo4.Items(i).Value) Then
    '                If cdate(CDate(departtime_combo4.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                    departtime_combo4.SelectedValue = departtime_combo4.Items(i).Value
    '                    Exit For
    '                End If
    '            End If
    '        Next

    '        leave2 = CDate(Me.depart_date4.SelectedDate & " " & departtime_combo4.SelectedValue)
    '    End If

    '    'minuteswaitbetweensegments = 0
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress4.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress4.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress4.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress4.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress4.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress4.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    If Me.pnlLeg5.Visible = False Then Exit Function

    '    If Me.OriginAddress5.Text.Trim = "" And Me.OriginAddress5.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress5.Text.Trim = "" And Me.DestinationAddress5.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date5.SelectedDate & " " & departtime_combo5.SelectedValue)
    '    If Me.OriginAddress5.SelectedValue.Trim = "" Then
    '        fromairport = Me.OriginAddress5.Text.Trim
    '    Else
    '        fromairport = Me.OriginAddress5.SelectedValue.Trim
    '    End If
    '    If Me.DestinationAddress5.SelectedValue.Trim = "" Then
    '        toairport = Me.DestinationAddress5.Text.Trim
    '    Else
    '        toairport = Me.DestinationAddress5.SelectedValue.Trim
    '    End If
    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    ma += TurnAroundMinutes    'add turn time
    '    If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
    '        Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
    '        If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
    '            Me.depart_date5.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2), 
    '                cdate(Me.depart_date5.SelectedDate))
    '        End If
    '        For i As Integer = 0 To departtime_combo5.Items.Count - 1
    '            If IsDate(departtime_combo5.Items(i).Value) Then
    '                If cdate(CDate(departtime_combo5.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                    departtime_combo5.SelectedValue = departtime_combo5.Items(i).Value
    '                    Exit For
    '                End If
    '            End If
    '        Next

    '        leave2 = CDate(Me.depart_date5.SelectedDate & " " & departtime_combo5.SelectedValue)
    '    End If

    '    'minuteswaitbetweensegments = 0
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress5.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress5.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress5.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress5.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress5.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress5.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    If Me.pnlLeg6.Visible = False Then Exit Function

    '    If Me.OriginAddress6.Text.Trim = "" And Me.OriginAddress6.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress6.Text.Trim = "" And Me.DestinationAddress6.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date6.SelectedDate & " " & departtime_combo6.SelectedValue)
    '    If Me.OriginAddress6.SelectedValue.Trim = "" Then
    '        fromairport = Me.OriginAddress6.Text.Trim
    '    Else
    '        fromairport = Me.OriginAddress6.SelectedValue.Trim
    '    End If
    '    If Me.DestinationAddress6.SelectedValue.Trim = "" Then
    '        toairport = Me.DestinationAddress6.Text.Trim
    '    Else
    '        toairport = Me.DestinationAddress6.SelectedValue.Trim
    '    End If
    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    ma += TurnAroundMinutes    'add turn time
    '    If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
    '        Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
    '        If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
    '            Me.depart_date6.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2), 
    '                cdate(Me.depart_date6.SelectedDate))
    '        End If
    '        For i As Integer = 0 To departtime_combo6.Items.Count - 1
    '            If IsDate(departtime_combo6.Items(i).Value) Then
    '                If cdate(CDate(departtime_combo6.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                    departtime_combo6.SelectedValue = departtime_combo6.Items(i).Value
    '                    Exit For
    '                End If
    '            End If
    '        Next

    '        leave2 = CDate(Me.depart_date6.SelectedDate & " " & departtime_combo6.SelectedValue)
    '    End If

    '    'minuteswaitbetweensegments = 0
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress6.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress6.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress6.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress6.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress6.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress6.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    If Me.pnlLeg7.Visible = False Then Exit Function

    '    If Me.OriginAddress7.Text.Trim = "" And Me.OriginAddress7.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress7.Text.Trim = "" And Me.DestinationAddress7.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date7.SelectedDate & " " & departtime_combo7.SelectedValue)
    '    If Me.OriginAddress7.SelectedValue.Trim = "" Then
    '        fromairport = Me.OriginAddress7.Text.Trim
    '    Else
    '        fromairport = Me.OriginAddress7.SelectedValue.Trim
    '    End If
    '    If Me.DestinationAddress7.SelectedValue.Trim = "" Then
    '        toairport = Me.DestinationAddress7.Text.Trim
    '    Else
    '        toairport = Me.DestinationAddress7.SelectedValue.Trim
    '    End If
    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    ma += TurnAroundMinutes    'add turn time
    '    If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
    '        Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
    '        If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
    '            Me.depart_date7.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2), 
    '                cdate(Me.depart_date7.SelectedDate))
    '        End If
    '        For i As Integer = 0 To departtime_combo7.Items.Count - 1
    '            If IsDate(departtime_combo7.Items(i).Value) Then
    '                If cdate(CDate(departtime_combo7.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                    departtime_combo7.SelectedValue = departtime_combo7.Items(i).Value
    '                    Exit For
    '                End If
    '            End If
    '        Next

    '        leave2 = CDate(Me.depart_date7.SelectedDate & " " & departtime_combo7.SelectedValue)
    '    End If

    '    'minuteswaitbetweensegments = 0
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress7.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress7.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress7.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress7.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress7.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress7.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    If Me.pnlLeg8.Visible = False Then Exit Function

    '    If Me.OriginAddress8.Text.Trim = "" And Me.OriginAddress8.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress8.Text.Trim = "" And Me.DestinationAddress8.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date8.SelectedDate & " " & departtime_combo8.SelectedValue)
    '    If Me.OriginAddress8.SelectedValue.Trim = "" Then
    '        fromairport = Me.OriginAddress8.Text.Trim
    '    Else
    '        fromairport = Me.OriginAddress8.SelectedValue.Trim
    '    End If
    '    If Me.DestinationAddress8.SelectedValue.Trim = "" Then
    '        toairport = Me.DestinationAddress8.Text.Trim
    '    Else
    '        toairport = Me.DestinationAddress8.SelectedValue.Trim
    '    End If
    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    ma += TurnAroundMinutes    'add turn time
    '    If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
    '        Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
    '        If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
    '            Me.depart_date8.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2), 
    '                cdate(Me.depart_date8.SelectedDate))
    '        End If
    '        For i As Integer = 0 To departtime_combo8.Items.Count - 1
    '            If IsDate(departtime_combo8.Items(i).Value) Then
    '                If cdate(CDate(departtime_combo8.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                    departtime_combo8.SelectedValue = departtime_combo8.Items(i).Value
    '                    Exit For
    '                End If
    '            End If
    '        Next

    '        leave2 = CDate(Me.depart_date8.SelectedDate & " " & departtime_combo8.SelectedValue)
    '    End If

    '    'minuteswaitbetweensegments = 0
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress8.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress8.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress8.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress8.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress8.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress8.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    If Me.pnlLeg9.Visible = False Then Exit Function

    '    If Me.OriginAddress9.Text.Trim = "" And Me.OriginAddress9.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress9.Text.Trim = "" And Me.DestinationAddress9.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date9.SelectedDate & " " & departtime_combo9.SelectedValue)
    '    If Me.OriginAddress9.SelectedValue.Trim = "" Then
    '        fromairport = Me.OriginAddress9.Text.Trim
    '    Else
    '        fromairport = Me.OriginAddress9.SelectedValue.Trim
    '    End If
    '    If Me.DestinationAddress9.SelectedValue.Trim = "" Then
    '        toairport = Me.DestinationAddress9.Text.Trim
    '    Else
    '        toairport = Me.DestinationAddress9.SelectedValue.Trim
    '    End If
    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    ma += TurnAroundMinutes    'add turn time
    '    If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
    '        Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
    '        If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
    '            Me.depart_date9.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2), 
    '                cdate(Me.depart_date9.SelectedDate))
    '        End If
    '        For i As Integer = 0 To departtime_combo9.Items.Count - 1
    '            If IsDate(departtime_combo9.Items(i).Value) Then
    '                If cdate(CDate(departtime_combo9.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                    departtime_combo9.SelectedValue = departtime_combo9.Items(i).Value
    '                    Exit For
    '                End If
    '            End If
    '        Next

    '        leave2 = CDate(Me.depart_date9.SelectedDate & " " & departtime_combo9.SelectedValue)
    '    End If

    '    'minuteswaitbetweensegments = 0
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress9.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress9.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress9.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress9.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress9.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress9.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    If Me.pnlLeg10.Visible = False Then Exit Function

    '    If Me.OriginAddress10.Text.Trim = "" And Me.OriginAddress10.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select from airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    If Me.DestinationAddress10.Text.Trim = "" And Me.DestinationAddress10.SelectedValue.Trim = "" Then
    '        lblMsg.Text &= "Please select to airport. "
    '        lblMsg.Visible = True
    '        editflightsmultileg = ""
    '        Exit Function
    '    End If

    '    leave2 = CDate(depart_date10.SelectedDate & " " & departtime_combo10.SelectedValue)
    '    If Me.OriginAddress10.SelectedValue.Trim = "" Then
    '        fromairport = Me.OriginAddress10.Text.Trim
    '    Else
    '        fromairport = Me.OriginAddress10.SelectedValue.Trim
    '    End If
    '    If Me.DestinationAddress10.SelectedValue.Trim = "" Then
    '        toairport = Me.DestinationAddress10.Text.Trim
    '    Else
    '        toairport = Me.DestinationAddress10.SelectedValue.Trim
    '    End If
    '    d = da.GetRoundEarthDistanceBetweenLocations(fromairport, toairport)
    '    bintl = isflightintl(fromairport, toairport)
    '    If d > 0 Then
    '        ma = traveltime(d, bintl)
    '    End If
    '    ma += TurnAroundMinutes    'add turn time
    '    If leave2 < DateAdd(DateInterval.Minute, ma, leave1) Then
    '        Dim newdate As Date = DateAdd(DateInterval.Minute, ma, leave1)
    '        If DateDiff(DateInterval.Day, newdate, leave2) > 0 Then
    '            Me.depart_date10.SelectedDate = DateAdd(DateInterval.Day, DateDiff(DateInterval.Day, newdate, leave2), 
    '                cdate(Me.depart_date10.SelectedDate))
    '        End If
    '        For i As Integer = 0 To departtime_combo10.Items.Count - 1
    '            If IsDate(departtime_combo10.Items(i).Value) Then
    '                If cdate(CDate(departtime_combo10.Items(i).Value).ToShortTimeString) > CDate(newdate.ToShortTimeString) then
    '                    departtime_combo10.SelectedValue = departtime_combo10.Items(i).Value
    '                    Exit For
    '                End If
    '            End If
    '        Next

    '        leave2 = CDate(Me.depart_date10.SelectedDate & " " & departtime_combo10.SelectedValue)
    '    End If

    '    'minuteswaitbetweensegments = 0
    '    'minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    'If minuteswaitbetweensegments <= 0 Then
    '    '    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'ElseIf minuteswaitbetweensegments < 30 then
    '    '    lblMsg.Text = "Time between segments too short"
    '    '    lblMsg.Visible = True
    '    '    Exit Function
    '    'End If
    '    leave1 = leave2

    '    If Me.OriginAddress10.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.OriginAddress10.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.OriginAddress10.Text.Trim
    '    End If
    '    editflightsmultileg &= " to "
    '    If Me.DestinationAddress10.SelectedValue.Trim <> "" Then
    '        editflightsmultileg &= Me.DestinationAddress10.SelectedValue.Trim
    '    Else
    '        editflightsmultileg &= Me.DestinationAddress10.Text.Trim
    '    End If
    '    editflightsmultileg &= " at " & leave2.ToString & ";"

    '    'If Not IsNothing(lbLegs) Then
    '    '    For i As Integer = 0 To lbLegs.Items.Count - 1
    '    '        s = lbLegs.Items(i).Value
    '    '        fromairport = Left(s, InStr(s, " to ") - 1)
    '    '        toairport = Mid(s, InStr(s, " to ") + 4, 4).Trim
    '    '        s = Mid(s, InStr(s, " at ") + 4)
    '    '        If IsDate(s) Then
    '    '            leave2 = CDate(s)
    '    '            If 1 > 0 Then
    '    '                minuteswaitbetweensegments = DateDiff(DateInterval.Minute, leave1, leave2)
    '    '                If minuteswaitbetweensegments <= 0 Then
    '    '                    lblMsg.Text = "Depart Date for subsequent leg must be after previous Depart Date"
    '    '                    lblMsg.Visible = True

    '    '                    Exit Sub
    '    '                End If
    '    '            End If
    '    '            leave1 = leave2
    '    '            fromairport = toairport
    '    '            toairport = ""
    '    '        End If
    '    '    Next

    '    'End If

    'End Function

    '20160430 - pab - 
    'Function isairport(ByVal loc As String) As Boolean

    '    Dim da As New DataAccess
    '    Dim ds As DataSet
    '    Dim dt As DataTable

    '    isairport = False

    '    If loc.Trim <> "" Then
    '        If InStr(loc, "(") > 0 Then
    '            loc = InBetween(1, loc, "(", ")")
    '        End If

    '        dt = da.GetAirportInformationByLocationID(loc)
    '        If Not isdtnullorempty(dt) Then
    '            isairport = True
    '        Else
    '            ds = da.GetIATAcodebyICAO(loc)
    '            If Not IsNothing(ds) Then
    '                If Not isdtnullorempty(ds.Tables(0)) Then
    '                    isairport = True
    '                End If
    '            End If
    '        End If

    '    End If

    'End Function

    Protected Sub bttnRemoveLeg_Click(sender As Object, e As EventArgs)

        removeleg(sender, e, "")

    End Sub

    Sub removeleg(sender As Object, e As EventArgs, button As String)

        lblMsg.Text = ""

        Dim s As String
        If Not IsNothing(sender) Then
            s = sender.id.ToString
        Else
            s = button
        End If
        Select Case s
            Case "bttnRemoveLeg9"
                move10to9()
            Case "bttnRemoveLeg8"
                move9to8()
                move10to9()
            Case "bttnRemoveLeg7"
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg6"
                move7to6()
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg5"
                move6to5()
                move7to6()
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg4"
                move5to4()
                move6to5()
                move7to6()
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg3"
                move4to3()
                move5to4()
                move6to5()
                move7to6()
                move8to7()
                move9to8()
                move10to9()
            Case "bttnRemoveLeg2"
                move3to2()
                move4to3()
                move5to4()
                move6to5()
                move7to6()
                move8to7()
                move9to8()
                move10to9()
        End Select

        If Me.pnlLeg10.Visible = True Then
            Me.pnlLeg10.Visible = False
        ElseIf Me.pnlLeg9.Visible = True Then
            Me.pnlLeg9.Visible = False
        ElseIf Me.pnlLeg8.Visible = True Then
            Me.pnlLeg8.Visible = False
        ElseIf Me.pnlLeg7.Visible = True Then
            Me.pnlLeg7.Visible = False
        ElseIf Me.pnlLeg6.Visible = True Then
            Me.pnlLeg6.Visible = False
        ElseIf Me.pnlLeg5.Visible = True Then
            Me.pnlLeg5.Visible = False
        ElseIf Me.pnlLeg4.Visible = True Then
            Me.pnlLeg4.Visible = False
        ElseIf Me.pnlLeg3.Visible = True Then
            Me.pnlLeg3.Visible = False
        ElseIf Me.pnlLeg2.Visible = True Then
            Me.pnlLeg2.Visible = False

            '20160429 - pab - change trip type if round trip or multi-leg
            If Session("triptype") <> "O" Then
                Session("triptype") = "O"
                Session("showcal") = "Y"

                Me.rblOneWayRoundTrip.SelectedIndex = 0
            End If

        End If

    End Sub

    Sub move10to9()

        Me.OriginAddress9.Text = Me.OriginAddress10.Text
        Me.DestinationAddress9.Text = Me.DestinationAddress9.Text
        Me.ddlPassengers9.Text = Me.ddlPassengers10.Text
        Me.depart_date9.SelectedDate = Me.depart_date10.SelectedDate
        Me.departtime_combo9.Text = Me.departtime_combo10.Text
        Me.OriginAddress10.Text = ""
        Me.DestinationAddress10.Text = ""
        Me.ddlPassengers10.Text = ""
        If Not IsNothing(Me.depart_date10.SelectedDate) Then
            Me.depart_date10.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date9.SelectedDate))
        Else
            Me.depart_date10.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo10.Text = "09:00 AM"

    End Sub

    Sub move9to8()

        Me.OriginAddress8.Text = Me.OriginAddress9.Text
        Me.DestinationAddress8.Text = Me.DestinationAddress9.Text
        Me.ddlPassengers8.Text = Me.ddlPassengers9.Text
        Me.depart_date8.SelectedDate = Me.depart_date9.SelectedDate
        Me.departtime_combo9.Text = Me.departtime_combo8.Text
        Me.OriginAddress9.Text = ""
        Me.DestinationAddress9.Text = ""
        Me.ddlPassengers9.Text = ""
        If Not IsNothing(Me.depart_date9.SelectedDate) Then
            Me.depart_date9.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date9.SelectedDate))
        Else
            Me.depart_date9.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo9.Text = "09:00 AM"

    End Sub

    Sub move8to7()

        Me.OriginAddress7.Text = Me.OriginAddress8.Text
        Me.DestinationAddress7.Text = Me.DestinationAddress8.Text
        Me.ddlPassengers7.Text = Me.ddlPassengers8.Text
        Me.depart_date7.SelectedDate = Me.depart_date8.SelectedDate
        Me.departtime_combo8.Text = Me.departtime_combo7.Text
        Me.OriginAddress8.Text = ""
        Me.DestinationAddress8.Text = ""
        Me.ddlPassengers8.Text = ""
        If Not IsNothing(Me.depart_date8.SelectedDate) Then
            Me.depart_date8.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date8.SelectedDate))
        Else
            Me.depart_date8.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo8.Text = "09:00 AM"

    End Sub

    Sub move7to6()

        Me.OriginAddress6.Text = Me.OriginAddress7.Text
        Me.DestinationAddress6.Text = Me.DestinationAddress7.Text
        Me.ddlPassengers6.Text = Me.ddlPassengers7.Text
        Me.depart_date6.SelectedDate = Me.depart_date7.SelectedDate
        Me.departtime_combo7.Text = Me.departtime_combo6.Text
        Me.OriginAddress7.Text = ""
        Me.DestinationAddress7.Text = ""
        Me.ddlPassengers7.Text = ""
        If Not IsNothing(Me.depart_date7.SelectedDate) Then
            Me.depart_date7.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date7.SelectedDate))
        Else
            Me.depart_date7.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo7.Text = "09:00 AM"

    End Sub

    Sub move6to5()

        Me.OriginAddress5.Text = Me.OriginAddress6.Text
        Me.DestinationAddress5.Text = Me.DestinationAddress6.Text
        Me.ddlPassengers5.Text = Me.ddlPassengers6.Text
        Me.depart_date5.SelectedDate = Me.depart_date6.SelectedDate
        Me.departtime_combo6.Text = Me.departtime_combo5.Text
        Me.OriginAddress6.Text = ""
        Me.DestinationAddress6.Text = ""
        Me.ddlPassengers6.Text = ""
        If Not IsNothing(Me.depart_date6.SelectedDate) Then
            Me.depart_date6.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date6.SelectedDate))
        Else
            Me.depart_date6.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo6.Text = "09:00 AM"

    End Sub

    Sub move5to4()

        Me.OriginAddress4.Text = Me.OriginAddress5.Text
        Me.DestinationAddress4.Text = Me.DestinationAddress5.Text
        Me.ddlPassengers4.Text = Me.ddlPassengers5.Text
        Me.depart_date4.SelectedDate = Me.depart_date5.SelectedDate
        Me.departtime_combo5.Text = Me.departtime_combo4.Text
        Me.OriginAddress5.Text = ""
        Me.DestinationAddress5.Text = ""
        Me.ddlPassengers5.Text = ""
        If Not IsNothing(Me.depart_date5.SelectedDate) Then
            Me.depart_date5.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date5.SelectedDate))
        Else
            Me.depart_date5.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo5.Text = "09:00 AM"

    End Sub

    Sub move4to3()

        Me.OriginAddress3.Text = Me.OriginAddress4.Text
        Me.DestinationAddress3.Text = Me.DestinationAddress4.Text
        Me.ddlPassengers3.Text = Me.ddlPassengers4.Text
        Me.depart_date3.SelectedDate = Me.depart_date4.SelectedDate
        Me.departtime_combo4.Text = Me.departtime_combo3.Text
        Me.OriginAddress4.Text = ""
        Me.DestinationAddress4.Text = ""
        Me.ddlPassengers4.Text = ""
        If Not IsNothing(Me.depart_date4.SelectedDate) Then
            Me.depart_date4.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date4.SelectedDate))
        Else
            Me.depart_date4.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo4.Text = "03:00 AM"

    End Sub

    Sub move3to2()

        Me.OriginAddress2.Text = Me.OriginAddress3.Text
        Me.DestinationAddress2.Text = Me.DestinationAddress3.Text
        Me.ddlPassengers2.Text = Me.ddlPassengers3.Text
        Me.depart_date2.SelectedDate = Me.depart_date3.SelectedDate
        Me.departtime_combo3.Text = Me.departtime_combo2.Text
        Me.OriginAddress3.Text = ""
        Me.DestinationAddress3.Text = ""
        Me.ddlPassengers3.Text = ""
        If Not IsNothing(Me.depart_date3.SelectedDate) Then
            Me.depart_date3.SelectedDate = DateAdd(DateInterval.Day, 1, CDate(Me.depart_date3.SelectedDate))
        Else
            Me.depart_date3.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToShortDateString
        End If
        Me.departtime_combo3.Text = "09:00 AM"

    End Sub

    '20160117 - pab - quote multi-leg trips
    'Private Sub HydrateLegs()

    '    Try

    '        Me.lbLegs.Items.Clear()

    '        If Not IsNothing(trip) Then
    '            For i As Integer = 0 To trip.Count - 1
    '                Dim tripleg As New leg
    '                tripleg = trip.Item(i)
    '                Me.lbLegs.Items.Add(tripleg.fromloc & " to " & tripleg.toloc & " at " & tripleg.depart.ToString)
    '            Next
    '        End If

    '    Catch ex As Exception
    '        Dim s As String = ex.Message
    '    End Try

    'End Sub

    ''20160117 - pab - quote multi-leg trips
    'Protected Sub cmdClearLegs_Click(sender As Object, e As EventArgs) Handles cmdClearLegs.Click

    '    lbLegs.Items.Clear()

    '    '20160406 - pab - reset other entry fields
    '    Me.OriginAddress.Items.Clear()
    '    Me.DestinationAddress.Items.Clear()
    '    Me.OriginAddress.ClearSelection()
    '    Me.DestinationAddress.ClearSelection()
    '    Me.OriginAddress.Text = ""
    '    Me.DestinationAddress.Text = ""

    '    Me.depart_date.Clear()
    '    Me.depart_date2.Clear()
    '    If DateDiff(DateInterval.Hour, Now, CDate(DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy") & " 9:00")) > 8 Then
    '        Me.depart_date.SelectedDate = DateAdd(DateInterval.Day, 1, Now).ToString("MM/dd/yyyy") & " 9:00"
    '        Me.depart_date2.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy") & " 17:00"
    '    Else
    '        Me.depart_date.SelectedDate = DateAdd(DateInterval.Day, 2, Now).ToString("MM/dd/yyyy") & " 9:00"
    '        Me.depart_date2.SelectedDate = DateAdd(DateInterval.Day, 3, Now).ToString("MM/dd/yyyy") & " 17:00"
    '    End If

    'End Sub

    'Protected Sub bttnAddLeg_Click(sender As Object, e As EventArgs) Handles bttnAddLeg.Click

    '    Dim broundtrip As Boolean = False

    '    lblMsg.Text = ""

    '    If Me.pnlLeg10.Visible = True Then
    '        lblMsg.Text = "Maximum number of legs added"
    '        Exit Sub
    '    End If

    '    Dim sdate As Date
    '    Dim n As Integer = 0
    '    If Me.pnlLeg2.Visible = False Then
    '        If Me.DestinationAddress.SelectedValue.trim <> "" Then
    '            Me.OriginAddress2.Text = Me.DestinationAddress.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress.Text.trim <> "" Then
    '            Me.OriginAddress2.Text = Me.DestinationAddress.Text.trim
    '        End If
    '        If Me.OriginAddress2.Text.trim <> "" Then loadairports(Me.OriginAddress2.Text.trim, "OriginAddress2")
    '        If Not IsNothing(Me.depart_date.SelectedDate) Then
    '            n = Me.departtime_combo.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date2.SelectedDate = sdate
    '        Me.departtime_combo2.SelectedIndex = n
    '        Me.pnlLeg2.Visible = True
    '        '20160913 - pab - fix radio button
    '        'broundtrip = True

    '    ElseIf Me.pnlLeg3.Visible = False Then
    '        If Me.DestinationAddress2.SelectedValue.trim <> "" Then 
    '            Me.OriginAddress3.Text = Me.DestinationAddress2.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress2.Text.trim <> "" Then
    '            Me.OriginAddress3.Text = Me.DestinationAddress2.Text.trim
    '        End If
    '        If Me.OriginAddress3.Text.trim <> "" Then loadairports(Me.OriginAddress3.Text.trim, "OriginAddress3")
    '        If Not IsNothing(Me.depart_date2.SelectedDate) Then
    '            n = Me.departtime_combo2.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date2.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date3.SelectedDate = sdate
    '        Me.departtime_combo3.SelectedIndex = n
    '        Me.pnlLeg3.Visible = True

    '    ElseIf Me.pnlLeg4.Visible = False Then
    '        If Me.DestinationAddress3.SelectedValue.trim <> "" Then 
    '            Me.OriginAddress4.Text = Me.DestinationAddress3.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress3.Text.trim <> "" Then
    '            Me.OriginAddress4.Text = Me.DestinationAddress3.Text.trim
    '        End If
    '        If Me.OriginAddress4.Text.trim <> "" Then loadairports(Me.OriginAddress4.Text.trim, "OriginAddress4")
    '        If Not IsNothing(Me.depart_date3.SelectedDate) Then
    '            n = Me.departtime_combo3.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date3.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date4.SelectedDate = sdate
    '        Me.departtime_combo4.SelectedIndex = n
    '        Me.pnlLeg4.Visible = True

    '    ElseIf Me.pnlLeg5.Visible = False Then
    '        If Me.DestinationAddress4.SelectedValue.trim <> "" Then 
    '            Me.OriginAddress5.Text = Me.DestinationAddress4.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress4.Text.trim <> "" Then
    '            Me.OriginAddress5.Text = Me.DestinationAddress4.Text.trim
    '        End If
    '        If Me.OriginAddress5.Text.trim <> "" Then loadairports(Me.OriginAddress5.Text.trim, "OriginAddress5")
    '        If Not IsNothing(Me.depart_date4.SelectedDate) Then
    '            n = Me.departtime_combo4.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date4.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date5.SelectedDate = sdate
    '        Me.departtime_combo5.SelectedIndex = n
    '        Me.pnlLeg5.Visible = True

    '    ElseIf Me.pnlLeg6.Visible = False Then
    '        If Me.DestinationAddress5.SelectedValue.trim <> "" Then 
    '            Me.OriginAddress6.Text = Me.DestinationAddress5.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress5.Text.trim <> "" Then
    '            Me.OriginAddress6.Text = Me.DestinationAddress5.Text.trim
    '        End If
    '        If Me.OriginAddress6.Text.trim <> "" Then loadairports(Me.OriginAddress6.Text.trim, "OriginAddress6")
    '        If Not IsNothing(Me.depart_date5.SelectedDate) Then
    '            n = Me.departtime_combo5.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date5.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date6.SelectedDate = sdate
    '        Me.departtime_combo6.SelectedIndex = n
    '        Me.pnlLeg6.Visible = True

    '    ElseIf Me.pnlLeg7.Visible = False Then
    '        If Me.DestinationAddress6.SelectedValue.trim <> "" Then 
    '            Me.OriginAddress7.Text = Me.DestinationAddress6.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress6.Text.trim <> "" Then
    '            Me.OriginAddress7.Text = Me.DestinationAddress6.Text.trim
    '        End If
    '        If Me.OriginAddress7.Text.trim <> "" Then loadairports(Me.OriginAddress7.Text.trim, "OriginAddress7")
    '        If Not IsNothing(Me.depart_date6.SelectedDate) Then
    '            n = Me.departtime_combo6.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date6.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date7.SelectedDate = sdate
    '        Me.departtime_combo7.SelectedIndex = n
    '        Me.pnlLeg7.Visible = True

    '    ElseIf Me.pnlLeg8.Visible = False Then
    '        If Me.DestinationAddress7.SelectedValue.trim <> "" Then 
    '            Me.OriginAddress8.Text = Me.DestinationAddress7.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress7.Text.trim <> "" Then
    '            Me.OriginAddress8.Text = Me.DestinationAddress7.Text.trim
    '        End If
    '        If Me.OriginAddress8.Text.trim <> "" Then loadairports(Me.OriginAddress8.Text.trim, "OriginAddress8")
    '        If Not IsNothing(Me.depart_date7.SelectedDate) Then
    '            n = Me.departtime_combo7.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date7.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date8.SelectedDate = sdate
    '        Me.departtime_combo8.SelectedIndex = n
    '        Me.pnlLeg8.Visible = True

    '    ElseIf Me.pnlLeg9.Visible = False Then
    '        If Me.DestinationAddress8.SelectedValue.trim <> "" Then 
    '            Me.OriginAddress9.Text = Me.DestinationAddress8.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress8.Text.trim <> "" Then
    '            Me.OriginAddress9.Text = Me.DestinationAddress8.Text.trim
    '        End If
    '        If Me.OriginAddress9.Text.trim <> "" Then loadairports(Me.OriginAddress9.Text.trim, "OriginAddress9")
    '        If Not IsNothing(Me.depart_date8.SelectedDate) Then
    '            n = Me.departtime_combo8.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date8.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date9.SelectedDate = sdate
    '        Me.departtime_combo9.SelectedIndex = n
    '        Me.pnlLeg9.Visible = True

    '    ElseIf Me.pnlLeg10.Visible = False Then
    '        If Me.DestinationAddress9.SelectedValue.trim <> "" Then 
    '            Me.OriginAddress10.Text = Me.DestinationAddress9.SelectedValue.ToString.trim
    '        elseIf Me.DestinationAddress9.Text.trim <> "" Then
    '            Me.OriginAddress10.Text = Me.DestinationAddress9.Text.trim
    '        End If
    '        If Me.OriginAddress10.Text.trim <> "" Then loadairports(Me.OriginAddress10.Text.trim, "OriginAddress10")
    '        If Not IsNothing(Me.depart_date9.SelectedDate) Then
    '            n = Me.departtime_combo9.SelectedIndex
    '            sdate = nextlegdate(Me.depart_date9.SelectedDate, n)
    '        Else
    '            sdate = DateAdd(DateInterval.Day, 2, CDate(Now.ToShortDateString))
    '            n = 37 ' "09:00 AM"
    '        End If
    '        Me.depart_date10.SelectedDate = sdate
    '        Me.departtime_combo10.SelectedIndex = n
    '        Me.pnlLeg10.Visible = True

    '    End If

    '    '20160429 - pab - change trip type if one way or round trip
    '    '20160913 - pab - fix radio button
    '    'If Session("triptype") <> "M" And broundtrip = False Then
    '    If Session("triptype") <> "M" Then
    '        Session("triptype") = "M"
    '        Session("showcal") = "Y"

    '        Me.rblOneWayRoundTrip.SelectedIndex = 2
    '    End If

    'End Sub

    '20160808 - pab - fix next depart date
    Function nextlegdate(ByVal departdate As Date, ByRef timeindex As Integer) As Date

        nextlegdate = departdate

        If Not IsNothing(timeindex) Then
            'sdate = DateAdd(DateInterval.Hour, 8, CDate(Me.departdate.SelectedDate & " " & Me.departtime_combo.SelectedValue))
            If timeindex + 32 <= 85 Then
                nextlegdate = departdate
                timeindex = timeindex + 32
            Else
                nextlegdate = DateAdd(DateInterval.Day, 1, CDate(departdate))
                timeindex = 37
            End If
        Else
            nextlegdate = DateAdd(DateInterval.Day, 1, CDate(departdate))
            timeindex = 37
        End If

    End Function

    '20160304 - pab - add broker info
    'Sub HydrateddllBrokerCompanies()

    '    Dim da As New DataAccess
    '    Dim dt As DataTable = da.GetBrokerCompanies
    '    Dim li As ListItem = Nothing

    '    Me.ddllBrokerCompanies.Items.Clear()

    '    li = New ListItem("", "")
    '    Me.ddllBrokerCompanies.Items.Add(li)

    '    If Not isdtnullorempty(dt) Then
    '        For n As Integer = 0 To dt.Rows.Count - 1
    '            li = New ListItem(dt.Rows(n).Item("companyname"), dt.Rows(n).Item("companyname"))

    '            Me.ddllBrokerCompanies.Items.Add(li)

    '        Next

    '    End If

    'End Sub

    'Sub HydrateddllBrokers(ByVal companyname As String)

    '    Dim da As New DataAccess
    '    Dim dt As DataTable = da.GetBrokersByCompany(companyname)
    '    Dim li As ListItem = Nothing

    '    Me.ddllBrokers.Items.Clear()

    '    li = New ListItem("", 0)
    '    Me.ddllBrokers.Items.Add(li)

    '    If Not isdtnullorempty(dt) Then
    '        For n As Integer = 0 To dt.Rows.Count - 1
    '            li = New ListItem(dt.Rows(n).Item("lastname") & ", " & dt.Rows(n).Item("firstname"), dt.Rows(n).Item("brokerid"))

    '            Me.ddllBrokers.Items.Add(li)

    '        Next

    '    End If

    'End Sub

    'Protected Sub ddllBrokerCompanies_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddllBrokerCompanies.SelectedIndexChanged

    '    HydrateddllBrokers(Me.ddllBrokerCompanies.SelectedValue.ToString)

    'End Sub

    Protected Sub ddllBrokers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddllBrokers.SelectedIndexChanged

        If Me.ddllBrokers.SelectedIndex < 0 Then
            Session("BrokerID") = 0
        Else
            Session("BrokerID") = ddllBrokers.SelectedValue
        End If

    End Sub

    Protected Sub bttnRemoveLeg2_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg2.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg3_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg3.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg4_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg4.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg5_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg5.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg6_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg6.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg7_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg7.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg8_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg8.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg9_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg9.Click

        removeleg(sender, e, "")

    End Sub

    Protected Sub bttnRemoveLeg10_Click(sender As Object, e As EventArgs) Handles bttnRemoveLeg10.Click

        removeleg(sender, e, "")

    End Sub

    '20160912 - pab - add start over
    Protected Sub cmdStartOver_Click(sender As Object, e As EventArgs) Handles cmdStartOver.Click

        cmdStartOverClicked(sender, e)

    End Sub

    '20160912 - pab - add start over
    Protected Sub cmdStartOverClicked(ByVal sender As Object, ByVal e As System.EventArgs)

        AirTaxi.post_timing("cmdStartOverClicked Start  " & Now.ToString)

        Session("origairportcode") = ""
        Session("destairportcode") = ""

        Session("confirmemailsent") = "N"

        Dim saveusername, saveusertype As String
        saveusername = ""
        saveusertype = ""
        If Not IsNothing(Session("username")) Then
            If Not IsNothing(Session("usertype")) Then
                saveusername = Session("username")
                saveusertype = Session("usertype")
            End If
        End If
        AirTaxi.post_timing("cmdStartOverClicked End  " & Now.ToString)

        Response.Redirect("home.aspx", True)

    End Sub

    Private Sub home2_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        '20161227 - pab - dynamic carrier images
        If Not IsPostBack Then
            '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
            Me.lblCarrier.Text = Session("urlalias").ToString.ToUpper
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Me.imglogo.Src = GetImageURLByATSSID(CInt(Session("carrierid")), 0, "logo")

            '20171114 - pab - remove hardcoded image names
            Dim da As New DataAccess
            Dim simage As String = da.GetSetting(CInt(Session("carrierid")), "background-image")
            If simage = "" Then simage = "images/bg2.jpg"
            content.Style.Remove("background-image")
            content.Style.Add("background-image", simage)
            If CInt(Session("carrierid")) = 48 Then
                imglogo.Width = 56
                imglogo.Style.Remove("position")
                imglogo.Style.Add("position", "absolute;top:16px;lefT:50%;margin:0 0 0 -23px;width:56px;z-index:1;")
            End If

        End If

    End Sub

End Class