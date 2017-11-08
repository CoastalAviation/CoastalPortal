Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports CoastalPortal.AirTaxi
Imports System.Data
Imports Telerik.Web.UI
Imports System.Data.SqlClient
Imports Optimizer.ConnectionStringHelper



Public Class OD123

    Inherits System.Web.UI.UserControl

    Private modelrunid As String
    Private SqlDataSource2 As New SqlDataSource
    Private sqldatasource1 As New SqlDataSource


    Public myJS1 As String = String.Empty
    Public Shared cnmkazure As New ADODB.Connection
    Public Shared cnsgazure As New ADODB.Connection



    'rk 7.30.2012 point to production
    Public D2DConnectionString As String = ConnectionStringHelper.GetConnectionStringSGServer



    Public ConnectionStringSQL As String = ConnectionStringHelper.getglobalconnectionstring("OptimizerDriver")


    Protected Sub ImgTrip_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)


        ''        If Me.PanelLookupTrips.Visible = False Then


        ''   Me.PanelLookupTrips.Visible = True

        'Me.GridViewTrips.Visible = True
        '' Me.GridViewCustomers.Visible = False
        ''        Me.GridViewRequestors.Visible = False

        'Dim s As String

        's = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
        's &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
        's &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
        's &= "seatsavail as [Seats] FROM Itinerary where ItinFlightRequest > 1 and CarrierID = " & session("carrierid") & " order by id desc "



        'Session("link") = "Trip"

        ''20120724 - fix connection strings
        ''SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetD2DConnectionString
        'SqlDataSourceTrips.ConnectionString = D2DConnectionString
        'SqlDataSourceTrips.SelectCommand = Session("sqlQueryTrips").ToString

        ''If Me.txtTripDescription.Text <> "" Then
        ''    SqlDataSourceTrips.SelectCommand = "select * from itinerary where ItinTitle Like '%" & Me.txtTripDescription.Text & "%' order by id desc"
        ''    Session("sqlQueryTrips") = "select * from itinerary where ItinTitle Like '%" & Me.txtTripDescription.Text & "%' order by id desc"
        ''End If


        ''SqlDataSourceTrips.DataBind()
        ''GridViewTrips.DataBind()
        'BindDataTrips(s)


        ' Me.hddnSearchMembers.Value = "1"
        'GridViewTrips.Visible = True





    End Sub

    'Private Sub BindDataTrips(ByVal s As String)
    '    Dim sql As String = s

    '    Dim dateFrom As DateTime = Nothing
    '    Dim dateThru As DateTime = Nothing


    '    '20110812 - pab - remove hardcoded connection string
    '    '20120724 - fix connection strings
    '    'SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetConnectionStringSQL
    '    SqlDataSourceTrips.ConnectionString = ConnectionStringSQL


    '    'Dim bc As New CommandField
    '    'With bc
    '    '    .ShowHeader = True
    '    '    .HeaderText = "Pin"
    '    '    .ItemStyle.Width = 20
    '    '    .ItemStyle.Wrap = True
    '    '    .Visible = True

    '    '    .SelectText = "Pin"
    '    'End With
    '    'GridViewTrips.Columns.Add(bc)

    '    '20101101 - pab
    '    GridViewTrips.EmptyDataText = "Your search returned 0 results"
    '    Dim req As String
    '    req = sql
    '    If sql <> "" Then

    '        '   sd1.Dispose()
    '        'GridView1.SelectedIndex = Nothing
    '        ' GridViewTrips.Dispose()

    '        SqlDataSourceTrips.Dispose()

    '        SqlDataSourceTrips.SelectCommand = sql
    '        'GetConnectionString
    '        'sd1.ConnectionString = "Data Source=(local);Initial Catalog=Production;Persist Security Info=True;User ID=sa;Password=CoastalPass1"
    '        '    sd1.ConnectionString = ConnectionStringHelper.GetConnectionString()


    '        '   <add name="ConnectString" connectionString="Data Source=(local);Initial Catalog=Production;Persist Security Info=True;User ID=sa;Password=CoastalPass1"/>



    '        GridViewTrips.AutoGenerateColumns = False

    '        GridViewTrips.DataSourceID = "SqlDataSourceTrips"
    '        SqlDataSourceTrips.DataBind()

    '        GridViewTrips.DataBind()

    '    Else

    '        SqlDataSourceTrips.DataBind()
    '        GridViewTrips.DataBind()
    '    End If


    '    '  AddColumnNoColor(GridViewTrips.Columns, "Pin", 20, 20, True)


    '    'GridViewTrips.DataBind()



    '    ' Dim gc As GridViewTrips.Controls(0)




    'End Sub

    Public Sub AddColumnNoColor(ByVal columnsCollection As DataControlFieldCollection, ByVal dataField As String, ByVal fieldWidth As Integer, ByVal columnWidthInPixels As Integer, Optional ByVal isReadOnly As Boolean = False, Optional ByVal isVisible As Boolean = True, Optional ByVal aFormatString As String = "")
        Dim bc As New CommandField
        With bc
            .ShowHeader = True
            .HeaderText = "Pin"
            .ItemStyle.Width = fieldWidth
            .ItemStyle.Wrap = True
            .Visible = isVisible
            .ItemStyle.BorderColor = Drawing.Color.Black
            .ItemStyle.BorderStyle = BorderStyle.Solid
            .ItemStyle.BorderWidth = 1
            .SelectText = "Pin"

            If Not (columnWidthInPixels = 0) Then .ControlStyle.Width = Unit.Pixel(columnWidthInPixels)
        End With
        columnsCollection.Add(bc)
    End Sub
    'Protected Sub GridViewTrips_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewTrips.RowCommand


    '    Dim si As Integer
    '    If IsNumeric(e.CommandArgument.ToString) Then si = CInt(e.CommandArgument.ToString)
    '    '  S = "Change " &  gridview1.Rows.Item(si).Cells(3).Text & " priority to "  & newpri
    '    Dim rs As New ADODB.Recordset




    '    If cnsgazure.State = 0 Then
    '        cnsgazure.ConnectionString = ConnectionStringHelper.GetConnectionStringSGAzure
    '        cnsgazure.Open()
    '    End If

    '    If rs.State = 1 Then rs.Close()

    '    Dim s As String

    '    Dim req As String




    '    req = "delete from pinnedflights where id = 'abc'  "
    '    req = Replace(req, "abc", Trim(GridViewTrips.Rows.Item(si).Cells(0).Text))

    '    If rs.State = 1 Then rs.Close()
    '    rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '    ' Me.txtUsersTotal.Text = rs.Fields("Count").Valu


    '    If rs.State = 1 Then rs.Close()


    '    SqlDataSourceTrips.DataBind()
    '    GridViewTrips.DataBind()


    'End Sub





    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


    '    '20120724 - fix connection strings
    '    'SqlDataSourceTrips.ConnectionString = ConnectionStringHelper.GetD2DConnectionString
    '    SqlDataSourceTrips.ConnectionString = D2DConnectionString

    '    modelrunid = ""

    '    If Not Request.QueryString("modelrunid") Is Nothing Then
    '        modelrunid = Request.QueryString("modelrunid")
    '    End If



    '    If Not (IsNothing(Session("airportsearchcode"))) Then
    '        If Session("airportsearchcode").ToString.Trim <> "" Then
    '            'If Me.txtTripName.Text.Trim = "" Then
    '            '    Me.txtTripName.Text = Session("airportsearchcode")
    '            'End If
    '        End If
    '    End If


    '    'If Not Page.IsPostBack Then

    '    'End If

    '    Exit Sub
    '    'rk 2.2.2013 work on this panel for speed

    '    If cnsgazure.State = 1 Then cnsgazure.Close()
    '    If cnsgazure.State = 0 Then
    '        cnsgazure.ConnectionString = ConnectionStringHelper.GetConnectionStringSGAzure
    '        cnsgazure.Open()
    '    End If

    '    Dim req As String
    '    Dim rs As New ADODB.Recordset
    '    req = "SELECT distinct(FOSAircraftID)  FROM aircraft where carrierid = 49  order by FOSAircraftID " 'where OptimizerRun = 'abc'
    '    req = Replace(req, "abc", modelrunid)

    '    If rs.State = 1 Then rs.Close()

    '    rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

    '    If DropDownListNNumber.Items.Count = 0 Then

    '        DropDownListNNumber.Items.Add("----")

    '        Dim i As Integer
    '        Do While Not rs.EOF
    '            i = i + 1
    '            ' Dim drui2 As New Telerik.Web.UI.RadComboBoxItem
    '            Dim a As String
    '            a = Trim(rs.Fields("FOSAircraftID").Value)

    '            DropDownListNNumber.Items.Add(a)
    '            rs.MoveNext()

    '        Loop
    '        If rs.State = 1 Then rs.Close()
    '        Me.lblmsg.Text = i & " aircraft found"

    '    End If


    'End Sub




    'Public Sub grabdata()

    '    Dim rs As New ADODB.Recordset
    '    Dim req As String


    '    Dim user As String = ""
    '    Dim dbname As String = ""
    '    Dim flightid As String = ""
    '    Dim pilot As String = ""

    '    If Not Request.QueryString("flightid") Is Nothing Then
    '        flightid = CStr(Request.QueryString("flightid"))
    '    End If

    '    If Not Session("flightid") Is Nothing Then
    '        flightid = Session("flightid")
    '    End If

    '    'If flightid = "" Then
    '    '    Me.lblMessage.Text = "Unauthorized Flight"
    '    '    Exit Sub
    '    'End If

    '    'connectstring = Replace(connectstring, "dbname", dbname)


    '    Exit Sub


    '    req = "SELECT * "
    '    req = req & "FROM flights WHERE id  = '" & flightid & "'"
    '    req &= " and carrierid = " & session("carrierid")


    '    '20120724 - fix connection strings
    '    'If cnmkazure.State = 0 Then
    '    '    cnmkazure.ConnectionString = connectstringsql
    '    '    cnmkazure.Open()
    '    'End If
    '    If cnsgazure.State = 0 Then
    '        cnsgazure.ConnectionString = ConnectionStringSQL
    '        cnsgazure.Open()
    '    End If



    '    '        rs.Open(req)
    '    '20100222 - pab - use global shared connection
    '    'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '    '20120724 - fix connection strings
    '    'rs.Open(req, cnmkazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
    '    rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


    '    If Not rs.EOF Then

    '        'Me.txtFromApt.Text = rs.Fields("DepartureAirport").Value
    '        'Me.txtToApt.Text = rs.Fields("ArrivalAirport").Value
    '        'Me.txtDepartureTime.Text = rs.Fields("departuretime").Value
    '        'Me.txtArrivalTime.Text = rs.Fields("arrivaltime").Value
    '        ''  Me.txtPilot.Text = pilot
    '        'Me.drpaircraft.SelectedValue = CInt(rs.Fields("AircraftID").Value)
    '        'Me.txtFlightId.Text = flightid

    '        'Me.txtFlightDetail.Text = rs.Fields("FlightDetail").Value



    '    End If


    '    If rs.State = 1 Then rs.Close()

    '    'me.txtCustomer.Text = rs.Fields ("



    '    If rs.State = 1 Then rs.Close()
    '    Dim _flightID As Integer




    'End Sub



    Protected Sub EditableLabelCity_Changed(ByVal sender As Object, ByVal e As EventArgs)
        '  [   Label1.Text = "EditableLabel Value changed at " + DateTime.Now.ToString()

        bttnSearchMembers_Click(Nothing, Nothing)

    End Sub 'EditableLabel1_Changed

    Protected Sub EditableLabelCountry_Changed(ByVal sender As Object, ByVal e As EventArgs)
        '  [   Label1.Text = "EditableLabel Value changed at " + DateTime.Now.ToString()

        bttnSearchMembers_Click(Nothing, Nothing)

    End Sub 'EditableLabel1_Changed

    Protected Sub bttnSearchMembers_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        '        Dim req As String
        '        req = "SELECT  [icao_id] ,[city]       ,[state]        ,[country]  , ctrydesc     ,airport_nm, entry_port " & _
        '",apinfo   FROM [SuperPortalV3].[dbo].[airport] "




        '        If Me.txtCountry.Text <> "" Then
        '            req = req & "where ctrydesc Like '" & Me.txtCountry.Text & "%'"
        '        End If

        '        If Me.txtCountry.Text <> "" And Me.txtCity.Text <> "" Then req = req & " and "

        '        If Me.txtCity.Text <> "" Then req = req & "  city Like '" & Me.txtCity.Text & "%' "

        '        BindData(req)

    End Sub








    'Protected Sub bttnSearchByCustomers_Click(sender As Object, e As System.EventArgs) Handles bttnSearchByCustomers.Click

    '    Session("sqlQueryTrips") = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
    '    Session("sqlQueryTrips") &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
    '    Session("sqlQueryTrips") &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
    '    Session("sqlQueryTrips") &= "seatsavail as [Seats] FROM Itinerary where ItinFlightRequest > 1 and CarrierID = " & session("carrierid") & " order by id desc "
    '    Session("link") = "Trip"

    '    ' SqlDataSourceTrips.SelectCommand = "select * from itinerary where ItinFlightRequest > 1 order by id desc "

    '    'If Me.txtCustomer.Text <> "" Then
    '    '    '    SqlDataSourceTrips.SelectCommand = "select * from itinerary where ItinTitle Like '%" & Me.txtTripDescription.Text & "%' order by id desc"
    '    '    Session("sqlQueryTrips") = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
    '    '    Session("sqlQueryTrips") &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
    '    '    Session("sqlQueryTrips") &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
    '    '    Session("sqlQueryTrips") &= "seatsavail as [Seats] FROM Itinerary where ItinFlightRequest > 1 and ItinCustomer Like '%" & Me.txtCustomer.Text & "%' and CarrierID = " & session("carrierid")
    '    '    Session("sqlQueryTrips") &= " order by id desc "
    '    'End If


    '    'SqlDataSourceTrips.DataBind()
    '    ' GridViewTrips.DataBind()
    '    BindDataTrips()

    'End Sub

    'Protected Sub LinkRecent_Click(sender As Object, e As System.EventArgs) Handles LinkRecent.Click

    '    Session("sqlQueryTrips") = "SELECT ID, ItinTitle as Title, ItinComment as Comment, ItinCustomer as Customer, ItinDispatcher as Dispatcher, "
    '    Session("sqlQueryTrips") &= "ItinDateOut as [Date Out], ItinDateReturn as [Date Return], ItinRequestor as Requestor, ItinQuoteNumber as [Quote Number], "
    '    Session("sqlQueryTrips") &= "ItinFlightRequest as [Flight Request], ItinFirstLeg as [First Leg], ItinRequestDate as [Request Date], ItinStatus as Status, "
    '    Session("sqlQueryTrips") &= "seatsavail as [Seats] FROM Itinerary where ItinFlightRequest > 1 and CarrierID = " & session("carrierid") & " order by id desc "
    '    Session("link") = "Trip"

    '    BindDataTrips()

    'End Sub















    'Protected Sub LinkButtonAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkButtonAdd.Click

    '    Me.lblmsg.ForeColor = Drawing.Color.Red


    '    If RadComboBox1.SelectedValue = "" Then
    '        Me.lblmsg.Text = "please select an originating airport "
    '        Exit Sub

    '    End If


    '    If RadComboBox2.SelectedValue = "" Then
    '        Me.lblmsg.Text = "please select a destination airport "
    '        Exit Sub
    '    End If


    '    If Not (IsDate(RadDateTimeFrom.SelectedDate)) Then
    '        Me.lblmsg.Text = "please select a start date "
    '        Exit Sub
    '    End If



    '    If Not (IsDate(RaddatetimeTo.SelectedDate)) Then
    '        Me.lblmsg.Text = "please select an end date "
    '        Exit Sub
    '    End If


    '    Me.lblmsg.ForeColor = Drawing.Color.Black



    '    Dim rs As New ADODB.Recordset



    '    If cnsgazure.State = 0 Then
    '        cnsgazure.ConnectionString = ConnectionStringHelper.GetConnectionStringSGAzure
    '        cnsgazure.Open()
    '    End If

    '    If rs.State = 1 Then rs.Close()

    '    Dim s As String

    '    Dim req As String



    '    req = "select * from pinnedflights where 1 = 2 "

    '    If rs.State = 1 Then rs.Close()
    '    rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '    ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


    '    If rs.EOF Then
    '        rs.AddNew()
    '        rs.Fields("TripNumber").Value = Now.Millisecond
    '        rs.Fields("AirportFrom").Value = RadComboBox1.SelectedValue
    '        rs.Fields("AirportTo").Value = RadComboBox2.SelectedValue
    '        rs.Fields("PinnedOn").Value = Now
    '        rs.Fields("Pinned").Value = True

    '        rs.Fields("TripType").Value = RadComboBoxType.SelectedValue
    '        rs.Fields("FromDateGMT").Value = RadDateTimeFrom.SelectedDate
    '        rs.Fields("ToDateGMT").Value = RaddatetimeTo.SelectedDate
    '        rs.Fields("Aircraft").Value = DropDownListNNumber.SelectedItem.ToString
    '        rs.Update()

    '    End If


    '    SqlDataSourceTrips.DataBind()
    '    GridViewTrips.DataBind()


    'End Sub


    Function updategrid()

        Dim req As String = ""

        Try

            Dim rs As New ADODB.Recordset
            Dim cnoptimizer As New ADODB.Connection
            Dim customid As String = ""



            If Not (IsNothing(Session("customid"))) Then customid = Trim(Session("customid"))

            If Not (IsNumeric(customid)) Then








                If cnoptimizer.State = 1 Then cnoptimizer.Close()
                If cnoptimizer.State = 0 Then
                    '20120724 - fix connection strings
                    ''   cnoptimizer.ConnectionString = ConnectionStringHelper.GetConnectionStringByDBName(databaseName)
                    'cnoptimizer.ConnectionString = ConnectionStringHelper.GetCASConnectionStringSQL()
                    cnoptimizer.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
                    cnoptimizer.Open()
                End If


                '20160104 - pab - run time improvements
                'req = "SELECT top 1 * from optimizerrequest where  description not like 'Optimizer request %'    and status = 'X' and carrierid = abc  order by   id desc "
                req = "SELECT top 1 * from optimizerrequest where  carrierid = abc  and status = 'X' and  description not like 'Optimizer request %'   order by   id desc "

                Dim cid As String = "65"
                If Not IsNothing(Session("carrierid")) Then
                    If IsNumeric(Session("carrierid")) Then
                        cid = CInt(Session("carrierid"))
                    End If
                End If


                req = Replace(req, "abc", cid)
                Dim daterangefrom, daterangeto As String

                rs.Open(req, cnoptimizer, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
                If Not rs.EOF Then
                    'latest completed model
                    customid = Trim(rs.Fields("id").Value)

                End If

            Else

                customid = Session("customid")
            End If



            '    req = "SELECT [ID],  status,   [CarrierID]    ,[Description]   ,[GMTStart]     ,[GMTEnd]   FROM [dbo].[OptimizerRequest] where status = 'X' and carrierid = 49 order by id desc"
            '   req = Replace(req, "49", session("carrierid"))

            If customid = "" Then Exit Function

            'and caslinebreaks <= (foslinebreaks + 7) 
            '20160104 - pab - run time improvements
            'req = "SELECT top 20 [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4]  FROM [OptimizerLog]  where casrevenuemiles <> 0 and CASrevenueexpense <> 0  and  left (modelrunid, 3) = '347'  and  modelrunid not like '%Q-%' and ModelRunID not like '%R11%' and ModelRunID not like '%R12%' and ModelRunID not like '%R13%'  and ModelRunID not like '%R0%'  and CASrevenueexpense <> 0 and carrierid = abc order by CAStotalrevenueexpense asc"
            req = "SELECT  top 20  [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4]  FROM [OptimizerLog]  where carrierid = abc and left (modelrunid, 3) = '347'  and  modelrunid not like '%Q-%' and ModelRunID not like '%R11%' and ModelRunID not like '%R12%'    and ModelRunID not like '%R0%'  and casrevenuemiles <> 0 and CASrevenueexpense <> 0  and FOSrevenuelegs - 100 <= CASrevenuelegs order by CAStotalrevenueexpense asc"


            If _carrierid <> 100 Then
                req = "SELECT  top 20  [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4]  FROM [OptimizerLog]  where carrierid = abc and left (modelrunid, 3) = '347'  and  modelrunid not like '%Q-%' and ModelRunID not like '%R11%' and ModelRunID not like '%R12%'     and casrevenuemiles <> 0 and CASrevenueexpense <> 0    order by CAStotalrevenueexpense asc"

            End If


            req = Replace(req, "347", customid)
            req = Replace(req, "(modelrunid, 3)", "(modelrunid, " & Len(customid) & ")")

            Dim cidstring As String = ""
            If Not IsNothing(Session("carrierid")) Then
                If IsNumeric(Session("carrierid")) Then
                    cidstring = Session("carrierid")
                End If
            End If
            req = Replace(req, "abc", cidstring)

            '   Dim SqlDataSource2 As New SqlDataSource

            SqlDataSource2.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString
            SqlDataSource2.SelectCommand = req
            SqlDataSource2.DataBind()
            GridView3.DataSource = SqlDataSource2
            GridView3.DataBind()
            '  RadHtmlChart2.DataSource = SqlDataSource2
            '  GridView2.DataBind()



            '   Dim SqlDataSource1 As New SqlDataSource


            sqldatasource1.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString
            sqldatasource1.SelectCommand = req
            sqldatasource1.DataBind()
            RadHtmlChart1.DataSource = sqldatasource1
            RadHtmlChart1.DataBind()




            '20160104 - pab - run time improvements
            'req = "SELECT top 1 [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4], day1, day2, day3, day4  FROM [OptimizerLog]  where  casrevenuemiles <> 0 and  deltaexpense > -1 and  left (modelrunid, 3) = '347' and caslinebreaks <= (foslinebreaks + 7)  and  modelrunid not like '%Q-%'  and ModelRunID not like '%R11%' and ModelRunID not like '%R12%' and ModelRunID not like '%R13%'  and ModelRunID not like '%R0%'  and CASrevenueexpense <> 0 and carrierid = abc order by CAStotalrevenueexpense asc"
            req = "SELECT top 1 [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4], day1, day2, day3, day4  FROM [OptimizerLog]  where carrierid = abc and left (modelrunid, 3) = '347' and modelrunid not like '%Q-%'  and ModelRunID not like '%R11%' and ModelRunID not like '%R12%'    and ModelRunID not like '%R0%'  and casrevenuemiles <> 0     and  CASrevenueexpense <> 0 order by CAStotalrevenueexpense asc"
            req = Replace(req, "347", customid)
            req = Replace(req, "(modelrunid, 3)", "(modelrunid, " & Len(customid) & ")")


            If Not IsNothing(Session("carrierid")) Then
                If IsNumeric(Session("carrierid")) Then
                    cidstring = Session("carrierid")
                End If
            End If
            req = Replace(req, "abc", cidstring)

            Dim cnmkazurelocal As New ADODB.Connection



            '20120724 - fix connection strings
            'If cnsetting.State = 0 Then
            '    'cnsetting.ConnectionString = ConnectionStringHelper.GetConnectionStringSQL
            '    cnsetting.ConnectionString = ConnectionStringSQL
            '    cnsetting.Open()
            'End If
            If cnmkazurelocal.State = 0 Then
                cnmkazurelocal.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
                cnmkazurelocal.ConnectionTimeout = 30
                cnmkazurelocal.CommandTimeout = 30
                cnmkazurelocal.Open()
            End If


            If rs.State = 1 Then rs.Close()

            rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


            If Not rs.EOF Then
                If Not IsDBNull(rs.Fields("day1").Value) Then
                    GridView3.Columns(2).HeaderTooltip = rs.Fields("day1").Value
                    GridView3.Columns(3).HeaderTooltip = rs.Fields("day1").Value

                    GridView3.Columns(4).HeaderTooltip = rs.Fields("day2").Value
                    GridView3.Columns(5).HeaderTooltip = rs.Fields("day2").Value

                    GridView3.Columns(6).HeaderTooltip = rs.Fields("day3").Value
                    GridView3.Columns(7).HeaderTooltip = rs.Fields("day3").Value

                    GridView3.Columns(8).HeaderTooltip = rs.Fields("day4").Value
                    GridView3.Columns(9).HeaderTooltip = rs.Fields("day4").Value
                End If

            End If

            If rs.State = 1 Then rs.Close()

        Catch ex As Exception
            DataAccess.Insert_sys_log(Session("carrierid"), appName, "OptimizerDetail.ascx Load Failure " & ex.Message & "  ", req, "")
        End Try

    End Function

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        '   <asp:SqlDataSource ID="SqlDataSource2" runat="server"  SelectCommand="SELECT * FROM [OptimizerLog]"></asp:SqlDataSource>
        '<asp:SqlDataSource ID="SqlDataSource1" runat="server" SelectCommand="SELECT [ModelRunID], [DeltaExpense], [CASrevenueexpense], [FOSrevenueexpense], [FOSefficiency], [CASefficiency], Viewed FROM [OptimizerLog] where  casrevenuemiles <> 0 and  left (modelrunid, 3) = '347' order by deltaexpense desc"></asp:SqlDataSource>

        'Dim ws As New coastalavtech.service.WebService1
        'If Session("defaultemail") <> ws.getcypher(Session("cypher")) Then Response.Redirect("login.aspx")


        Session("carrierid") = Session("carrierid")

        If Session("carrierid") = 0 Then
            lblmsg.Text = "Credentials Lost, Please Log In Again"
            Exit Sub
        End If

        SqlDataSource2.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString
        sqldatasource1.ConnectionString = ConnectionStringHelper.GetsqladapterWestConnectionString

        GridView3.DataSource = sqldatasource1
        RadHtmlChart1.DataSource = SqlDataSource2

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



        'Try
        '    If Not IsPostBack Then
        '        'Check if the browser support cookies
        '        If Request.Browser.Cookies Then
        '            'Check if the cookies with name PBLOGIN exist on user's machine
        '            If Request.Cookies("CASOPTIMIZER") IsNot Nothing Then
        '                'Pass the user name and password to the VerifyLogin method

        '                If Request.Cookies("CASOPTIMIZER")("EMAIL") IsNot Nothing Then


        '                    If Request.Cookies("CASOPTIMIZER")("EMAIL").ToString() <> "" Then
        '                        '        Me.txtemail.Text = Request.Cookies("CASOPTIMIZER")("EMAIL").ToString()
        '                        '  Me.Session("email") = Request.Cookies("CASOPTIMIZERLOGIN")("UNAME").ToString()
        '                    End If

        '                End If


        '            End If
        '        End If
        '    End If
        'Catch

        '    ' Response.Redirect("login.aspx")'?email=" & Me.txtEmail2.Text)
        'End Try



    End Sub

    'Private Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
    '    Dim s As String
    '    Dim si As Integer
    '    If IsNumeric(e.CommandArgument.ToString) Then si = CInt(e.CommandArgument.ToString)

    '    '     Dim customid As String = Trim(GridView1.Rows.Item(si).Cells(0).Text)



    '    Dim req As String = "SELECT top 1  right(rtrim(modelrunid), 1) as mt" & _
    '        "  ,[ModelRunID]  ,[CAStotalrevenueexpense]  ,[CASefficiency]  FROM [dbo].[OptimizerLog] where " & _
    '        "left(modelrunid, 2) = '28'  and carrierid = 49  and right(rtrim(modelrunid), 1) = 'C'  order by CAStotalrevenueexpense asc"
    '    req = Replace(req, "28", customid)
    '    req = Replace(req, 49, session("carrierid"))
    '    req = Replace(req, "left(modelrunid, 2", "left(modelrunid, " & Len(customid))


    '    If e.CommandName.ToString = "Best" Then
    '        '   req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "and right(rtrim(modelrunid), 1) <> 'C'")
    '        req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "")
    '    End If

    '    If e.CommandName.ToString = "Eff" Then
    '        ' req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "and right(rtrim(modelrunid), 1) <> 'C'")
    '        req = Replace(req, "and right(rtrim(modelrunid), 1) = 'C'", "")
    '        req = Replace(req, "order by CAStotalrevenueexpense asc", "order by CASefficiency desc")
    '    End If


    '    Dim c As Integer
    '    c = 5
    '    If e.CommandName.ToString = "Eff" Then c = 7
    '    If e.CommandName.ToString = "Best" Then c = 6

    '    If e.CommandName.ToString = "Base" Or e.CommandName.ToString = "Eff" Or e.CommandName.ToString = "Best" Then


    '        Dim rs As New ADODB.Recordset

    '        If cnsetting.State = 0 Then
    '            cnsetting.ConnectionString = connectstringsql
    '            cnsetting.Open()
    '        End If

    '        If rs.State = 1 Then rs.Close()

    '        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

    '        If Not rs.EOF Then

    '            modelrunid = Trim(rs.Fields("ModelRunID").Value)
    '            ' Me.txtUsersTotal.Text = rs.Fields("Count").Value
    '            If rs.State = 1 Then rs.Close()

    '            Response.Redirect("/Panel.aspx?modelrunid=" & modelrunid)

    '            '     Call LoadCountries (RadComboBox1.Text) Response.Redirect("http://optimizerpanelwest.cloudapp.net/Panel.aspx?modelrunid=" & modelrunid)
    '        Else

    '            '    GridView1.Rows.Item(si).Cells(c).BackColor = Drawing.Color.MediumVioletRed

    '        End If


    '        If rs.State = 1 Then rs.Close()


    '    End If
    'End Sub

    'Protected Sub LinkButtonAdd_Click(sender As Object, e As EventArgs) Handles LinkButtonAdd.Click

    '    Try
    '        If (Request.Browser.Cookies) Then
    '            'Check if the cookie with name PBLOGIN exist on user's machine
    '            If (Request.Cookies("CASOPTIMIZER") Is Nothing) Then
    '                'Create a cookie with expiry of 30 days
    '                Response.Cookies("CASOPTIMIZER").Expires = DateTime.Now.AddDays(60)
    '                'Write username to the cookie
    '                Response.Cookies("CASOPTIMIZER").Item("UNAME") = Me.txtemail.Text
    '                'Write password to the cookie

    '            Else
    '                Response.Cookies("CASOPTIMIZER").Item("UNAME") = Me.txtemail.Text
    '            End If
    '        End If
    '    Catch
    '    End Try



    '    Dim req As String

    '    Dim rs As New ADODB.Recordset
    '    Dim rs2 As New ADODB.Recordset




    '    Try

    '        If cnsgazure.State = 0 Then
    '            cnsgazure.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
    '            cnsgazure.Open()
    '        End If



    '        req = "select * from OptimizerRequest where 1 = 2 "

    '        If rs.State = 1 Then rs.Close()
    '        rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '        ' Me.txtUsersTotal.Text = rs.Fields("Count").Value





    '        rs.AddNew()
    '        rs.Fields("CarrierID").Value = session("carrierid")
    '        rs.Fields("Description").Value = Me.txtDescription.Text
    '        rs.Fields("GMTStart").Value = Me.RadDateTimeFrom.SelectedDate
    '        rs.Fields("GMTEnd").Value = RaddatetimeTo.SelectedDate





    '        rs.Fields("MinutesBetweenFlights").Value = CInt(RadSliderMBF.Value.ToString)
    '        rs.Fields("MaxSpeedKts").Value = RadSliderAvgSpeed.Value.ToString
    '        rs.Fields("DepartureDelays").Value = RadSliderDepDelay.Value.ToString

    '        rs.Fields("OverrideMBF").Value = CheckOverride.Checked

    '        rs.Fields("IncludeBrokerAircraft").Value = ChkBroker.Checked

    '        rs.Fields("UpgradeWindow").Value = RadSliderUpg.Value.ToString

    '        rs.Fields("AutoPin").Value = RadSliderAutoPin.Value.ToString
    '        rs.Fields("TaxiTime").Value = RadSliderTaxiTime1.Value.ToString
    '        rs.Fields("FastTurnSameTrip").Value = RadSliderFastTurn.Value.ToString
    '        rs.Fields("PrepositionFirstFlight").Value = RadSliderPrePosition.Value.ToString

    '        rs.Fields("email").Value = Me.txtemail.Text 'rk 3/6/2013 add email for notifications

    '        rs.Fields("RequestDate").Value = Now
    '        ' rs.Fields("CompleteDate").Value =
    '        '  rs.Fields("BestModel").Value = True
    '        ' rs.Fields("BestModelCost").Value = True
    '        rs.Fields("Status").Value = "P"

    '        rs.Update()


    '        req = "select top 1 * from OptimizerRequest order by id desc "

    '        If rs.State = 1 Then rs.Close()
    '        rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '        ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


    '        Dim runid As String
    '        If Not rs.EOF Then

    '            runid = rs.Fields("id").Value


    '        End If

    '        If rs.State = 1 Then rs.Close()


    '        req = "select * from OptimizerWeights where OptimizerRun = 'Default' and carrierid = 49 "
    '        req = Replace(req, "49", session("carrierid"))


    '        If rs.State = 1 Then rs.Close()
    '        rs.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '        ' Me.txtUsersTotal.Text = rs.Fields("Count").Value



    '        req = "select * from OptimizerWeights where 1 = 2 "

    '        If rs2.State = 1 Then rs2.Close()
    '        rs2.Open(req, cnsgazure, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
    '        ' Me.txtUsersTotal.Text = rs.Fields("Count").Value


    '        Dim ow(10, 4) As String
    '        Dim i As Integer = 0

    '        Do While Not rs.EOF

    '            i = i + 1
    '            ow(i, 1) = rs.Fields("CarrierID").Value
    '            ow(i, 2) = rs.Fields("Day").Value
    '            ow(i, 3) = rs.Fields("Percentage").Value
    '            ow(i, 4) = rs.Fields("FleetType").Value

    '            rs.MoveNext()

    '        Loop

    '        Dim ii As Integer
    '        For ii = 1 To i

    '            rs2.AddNew()

    '            rs2.Fields("OptimizerRun").Value = runid
    '            rs2.Fields("CarrierID").Value = ow(ii, 1)
    '            rs2.Fields("Day").Value = ow(ii, 2)
    '            rs2.Fields("Percentage").Value = ow(ii, 3)
    '            rs2.Fields("FleetType").Value = ow(ii, 4)

    '            rs2.Update()


    '        Next ii

    '        If rs2.State = 1 Then rs2.Close()
    '        If rs.State = 1 Then rs.Close()

    '    Catch msg As Exception
    '        lblmsg.Text = "Model Request Error at " & Now

    '        Exit Sub
    '    End Try
    '    lblmsg.Text = "Model Request Submitted at " & Now

    'End Sub

    '   Function gridme()





    '       Dim s1 As String
    '       s1 = " <tr style=%background-color: DEF; text-decoration: none;% onclick=%window.location.href='http://flytherefast.com/temp-Product.aspx'%> " & _
    '                 "	<td class=%h-searchresults_img% title=%Canon FS40 HD Camcorder%> " & _
    '                  "	<a id=%productImg% href=%temp-Product.aspx%> " & _
    '                   "	<img id=%productInfoImg% alt=%No Image% src=%InfoImage%>" & _
    '                                     "</a>" & _
    '                 "	</td><td class=%h-searchresults_title% align=%left% >Canon FS40 HD Camcorder<small class=%grey%>Extra description text will go here.</small></small></td><td><a class=%btn btn-grey% href=%www.cnn.com%>Order&nbsp;Details</a></td> " & _
    '"</tr>"
    '       '"<td><a class=%btn btn-grey% href=%www.abc.com%>Tracking&nbsp;Info</a></td> " & _
    '       '"</tr>"



    '       s1 = Replace(s1, "%", Chr(34))

    '       myJS1 = ""




    '       Dim mycolor As System.Drawing.Color
    '       mycolor = Drawing.Color.Aquamarine


    '       If cnsetting.State = 1 Then cnsetting.Close()
    '       If cnsetting.State = 0 Then
    '           cnsetting.ConnectionString = ConnectionStringHelper.GetMKConnectionStringsql
    '           cnsetting.Open()
    '       End If

    '       Dim i As Integer
    '       For i = 0 To GridViewTrips.Rows.Count - 1



    '           Dim id As String = GridViewOR.Rows(i).Cells(0).Text
    '           Dim l As Integer = Len(id)

    '           Dim req As String

    '           req = "select FROM [dbo].[OptimizerLog] where left(modelrunid, xlen) = 28 and carrierid = 49 order by casefficiency  desc"
    '           req = Replace(req, "xlen", l)
    '           req = Replace(req, "49", session("carrierid"))
    '           req = Replace(req, "28", id)

    '           Dim mrid As String

    '           Dim rs As New ADODB.Recordset
    '           If rs.State = 1 Then rs.Close()
    '           rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)





    '           Do While Not rs.EOF

    '               i = i + 1


    '               mrid = rs.Fields("modelrunid").Value

    '               Dim s As String = s1


    '               If i / 2 = Int(i / 2) Then
    '                   s = Replace(s, "DEF", "white")
    '               Else
    '                   s = Replace(s, "DEF", "rgb(239, 239, 239)")
    '               End If

    '               ' s = Replace (s, "ABC", i)
    '               'GridViewHistory','Select$ABC'
    '               Dim idsx As String = rs.Fields("id").Value

    '               s = Replace(s, "http://flytherefast.com/temp-Product.aspx", "Order-Detail.aspx?order=" & idsx)




    '               Dim mandesc As String
    '               '    dr("MFG") =rs.Fields("mfg").Value &
    '               If Not (IsDBNull(rs.Fields("ManDescription").Value)) Then
    '                   mandesc = rs.Fields("ManDescription").Value
    '               End If

    '               ' dr("title") =

    '               mandesc = Replace(mandesc, "&amp;", Chr(26))
    '               mandesc = Replace(mandesc, "&quot;", Chr(34))
    '               mandesc = Replace(mandesc, "&amp", Chr(26))
    '               mandesc = Replace(mandesc, "&quot", Chr(34))

    '               mandesc = Replace(mandesc, Chr(34) & Chr(34), Chr(34))


    '               If Not (IsDBNull(rs.Fields("ManDescription").Value)) Then
    '                   Dim mandetaildesc As String = rs.Fields("ManDescription").Value
    '               End If

    '               s = Replace(s, "Canon FS40 HD Camcorder", mandesc)
    '               s = Replace(s, "Extra description text will go here.", "")

    '               Dim infoimage As String


    '               Dim pi As String = rs.Fields("ProductImage").Value
    '               pi = Replace(pi, "imagelist/fn", "imagelist/tns")
    '               pi = Replace(pi, "imagelist/tn0", "imagelist/tns0")




    '               infoimage = "http://www.CASOPTIMIZER.com/" & Replace(pi, "~/", "")

    '               s = Replace(s, "InfoImage", infoimage)
    '               s = Replace(s, "ZoomImage", Replace(infoimage, "tns", "fn"))


    '               s = Replace(s, "www.cnn.com", "Order-Detail.aspx?order=" & idsx)
    '               Dim tn As String = ""

    '               If Not (IsDBNull(rs.Fields("trackingnumber").Value)) Then
    '                   tn = rs.Fields("trackingnumber").Value
    '               End If
    '               s = Replace(s, "www.abc.com", "http://www.ups.com?tracking=" & tn)

    '               ' s = Replace(s, "'GridViewHistory','Select$ABC'", "productquote.aspx?product=" & idsx)

    '               myJS1 = myJS1 & s


    '               rs.MoveNext()
    '           Loop


    '           '    GridViewOR.Rows(i).Cells(4).
    '           '      End If





    '           'GridViewTrips.Rows.Item(i).Cells(4).BackColor = Drawing.Color.White
    '           'GridViewTrips.Rows.Item(i).Cells(9).BackColor = Drawing.Color.White



    '           '   GridViewTrips.Rows(i).Cells(0).Text = "Pin"


    '           'Pending	Quoted	Booked	Disptached	In Progress	Flown	
    '           'For z = 0 To 9
    '           '    GridViewTrips.Rows(i).Cells(z).Text = Trim(GridViewTrips.Rows(i).Cells(z).Text)
    '           'Next z
    '           ' GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue

    '           Dim x As String = GridViewTrips.Rows(i).Cells(11).Text
    '           Select Case GridViewTrips.Rows(i).Cells(11).Text
    '               Case "D", "True"
    '                   GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.GreenYellow ' Drawing.Color.Wheat
    '                   GridViewTrips.Rows(i).Cells(11).Text = "D"
    '               Case "R", "False"
    '                   GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkSeaGreen
    '                   GridViewTrips.Rows(i).Cells(11).Text = "R"
    '               Case Else
    '                   If Trim(GridViewTrips.Rows(i).Cells(12).Text) = "12345" Then
    '                       GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkOrange
    '                       GridViewTrips.Rows(i).Cells(11).Text = "M"
    '                   Else
    '                       GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
    '                   End If
    '           End Select



    '           If Trim(GridViewTrips.Rows(i).Cells(12).Text) = "12345" Then
    '               GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkOrange
    '               GridViewTrips.Rows(i).Cells(11).Text = "M"


    '           End If




    '           'req = "select * from fosflightsoptimizer where TripNumber = 'abc' and departureairporticao = 'def' and  arrivalairporticao = 'ghi' and optimizerrun = 'jkl'"
    '           'req = Replace(req, "abc", Trim(GridViewTrips.Rows.Item(i).Cells(12).Text))
    '           'req = Replace(req, "def", Trim(GridViewTrips.Rows.Item(i).Cells(0).Text))
    '           'req = Replace(req, "ghi", Trim(GridViewTrips.Rows.Item(i).Cells(1).Text))
    '           'req = Replace(req, "jkl", modelrunid)




    '           'Dim rs As New ADODB.Recordset
    '           'If rs.State = 1 Then rs.Close()
    '           'rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

    '           'If Not rs.EOF Then


    '           Dim ltc As String = GridViewTrips.Rows(i).Cells(13).Text
    '           Dim lrc As String = GridViewTrips.Rows(i).Cells(14).Text
    '           Dim lpc As String = GridViewTrips.Rows(i).Cells(15).Text


    '           'Dim lrc As String = Trim(rs.Fields("legratecode").Value)
    '           'Dim lpc As String = Trim(rs.Fields("legpurposecode").Value)
    '           ' GridViewTrips.Rows(i).Cells(10).ToolTip = lrc
    '           ' GridViewTrips.Rows(i).Cells(9).ToolTip = lpc

    '           'Dim ltc As String = Trim(rs.Fields("legtypecode").Value)
    '           ' GridViewTrips.Rows(i).Cells(11).ToolTip = ltc
    '           If ltc = "MXSC" Or ltc = "AOG" Or ltc = "DETL" Or ltc = "INSV" Or ltc = "MXRC" Or ltc = "MXSC" Or ltc = "MXUS" Or ltc = "MAINT" Then
    '               GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkOrange
    '               GridViewTrips.Rows(i).Cells(11).Text = "M"
    '           End If

    '           If ltc = "77" Then
    '               GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.SteelBlue
    '               GridViewTrips.Rows(i).Cells(11).Text = "M"
    '           End If
    '           '     End If



    '           If Trim(GridViewTrips.Rows(i).Cells(12).Text) = "67890" Then
    '               GridViewTrips.Rows(i).Cells(11).BackColor = Drawing.Color.DarkKhaki
    '               GridViewTrips.Rows(i).Cells(11).Text = "B"

    '           End If

    '           Dim account As Integer



    '           If i > 0 Then
    '               If GridViewTrips.Rows(i).Cells(8).Text <> GridViewTrips.Rows(i - 1).Cells(8).Text Then 'ac change
    '                   GridViewTrips.Rows(i).Cells(8).BackColor = Drawing.Color.CadetBlue

    '                   account = account + 1
    '                   If account / 2 = Int(account / 2) Then
    '                       'GridViewTrips.Rows(i).Style.Add("BackColor", "#669999")
    '                       mycolor = Drawing.Color.Aquamarine
    '                   Else
    '                       'GridViewTrips.Rows(i).Style.Remove("BackColor")
    '                       'GridViewTrips.Rows(i).Style.Add("BackColor", "#000066")
    '                       mycolor = Drawing.Color.White
    '                   End If


    '               Else
    '                   If GridViewTrips.Rows(i).Cells(0).Text <> GridViewTrips.Rows(i - 1).Cells(1).Text Then
    '                       GridViewTrips.Rows(i).Cells(0).BackColor = Drawing.Color.IndianRed
    '                       GridViewTrips.Rows(i - 1).Cells(1).BackColor = Drawing.Color.IndianRed


    '                   End If


    '               End If
    '           End If


    '           GridViewTrips.Rows(i).BackColor = mycolor

    '           Dim pinned As Boolean = False
    '           'If bpinnedflights = True Then
    '           '    dv_pinned.RowFilter = "TripNumber = " & GridViewTrips.Rows.Item(i).Cells(12).Text & " and AirportFrom = '" & _
    '           '        GridViewTrips.Rows.Item(i).Cells(0).Text.ToString & "' and AirportTo = '" & GridViewTrips.Rows.Item(i).Cells(1).Text.ToString & "'"
    '           '    If dv_pinned.Count > 0 Then
    '           '        pinned = True
    '           '    End If
    '           'End If

    '           If pinned = False Then
    '               GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.White
    '               GridViewTrips.Rows.Item(i).Cells(13).BackColor = Drawing.Color.White
    '           Else
    '               GridViewTrips.Rows.Item(i).Cells(2).BackColor = Drawing.Color.Goldenrod
    '               GridViewTrips.Rows.Item(i).Cells(13).BackColor = Drawing.Color.Goldenrod
    '           End If



    '           If IsDate(GridViewTrips.Rows(i).Cells(2).Text) Then

    '               Dim gmt As Date = DateTime.UtcNow
    '               Dim departgmt As Date = CDate(GridViewTrips.Rows(i).Cells(2).Text)
    '               If gmt > DateAdd(DateInterval.Minute, 180, departgmt) Then
    '                   GridViewTrips.Rows(i).Cells(2).BackColor = Drawing.Color.Teal
    '                   GridViewTrips.Rows(i).Cells(4).BackColor = Drawing.Color.Teal
    '               End If


    '           End If

    '           For z = 2 To 5
    '               If IsDate(GridViewTrips.Rows(i).Cells(z).Text) Then
    '                   Dim departgmt As Date = CDate(GridViewTrips.Rows(i).Cells(z).Text)
    '                   GridViewTrips.Rows(i).Cells(z).Text = Trim(departgmt.ToString("MM'/'dd' 'HH':'mm"))
    '               End If
    '           Next z





    '       Next i






    '   End Function


    Protected Sub LinkRefresh_Click(sender As Object, e As EventArgs) Handles LinkRefresh.Click
        updategrid()
    End Sub

    'Private Sub GridView3_PreRender(sender As Object, e As EventArgs) Handles GridView3.PreRender
    '    Dim customid As String = ""

    '    Dim req As String


    '    If Not (IsNothing(Session("carrierid"))) Then

    '        If IsNumeric(Session("carrierid")) Then

    '            If Not (IsNothing(Session("customid"))) Then customid = Session("customid")

    '            If Not (IsNumeric(customid)) Then



    '                Dim rs As New ADODB.Recordset
    '                Dim cnoptimizer As New ADODB.Connection




    '                If cnoptimizer.State = 1 Then cnoptimizer.Close()
    '                If cnoptimizer.State = 0 Then
    '                    '20120724 - fix connection strings
    '                    ''   cnoptimizer.ConnectionString = ConnectionStringHelper.GetConnectionStringByDBName(databaseName)
    '                    'cnoptimizer.ConnectionString = ConnectionStringHelper.GetCASConnectionStringSQL()
    '                    cnoptimizer.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
    '                    cnoptimizer.Open()
    '                End If


    '                req = "SELECT top 1 * from optimizerrequest where where description not like 'Optimizer request %' and status = 'X' and carrierid = abc  order by   id desc "
    '                req = Replace(req, "abc", Session("carrierid"))
    '                Dim daterangefrom, daterangeto As String

    '                rs.Open(req, cnoptimizer, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
    '                If Not rs.EOF Then
    '                    'latest completed model
    '                    customid = Trim(rs.Fields("id").Value)

    '                End If

    '            Else

    '                customid = Session("customid")
    '            End If



    '            '    req = "SELECT [ID],  status,   [CarrierID]    ,[Description]   ,[GMTStart]     ,[GMTEnd]   FROM [dbo].[OptimizerRequest] where status = 'X' and carrierid = 49 order by id desc"
    '            '   req = Replace(req, "49", session("carrierid"))



    '            Try



    '                'req = "SELECT top 1 [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4], day1, day2, day3, day4  FROM [OptimizerLog]  where deltaexpense > -1 and  left (modelrunid, 3) = '347' order by deltaexpense desc"
    '                'req = Replace(req, "347", customid)
    '                'req = Replace(req, "(modelrunid, 3)", "(modelrunid, " & Len(customid) & ")")

    '                req = "SELECT top 1 [ModelRunID], [DeltaExpense], [DeltaNRM], [CASlinebreaks], [CASefficiency], [FOSlinebreaks], [FOSefficiency], deltaexpense1, deltaexpense2, deltaexpense3, deltaexpense4, Viewed  ,[EffCAS1],  [EffCAS2]  ,[EffCAS3]  ,[EffCAS4] ,[Efffos1],  [Efffos2]  ,[Efffos3]  ,[Efffos4], day1, day2, day3, day4  FROM [OptimizerLog]  where  casrevenuemiles <> 0 and  deltaexpense > -1 and  left (modelrunid, 3) = '347' and caslinebreaks <= (foslinebreaks + 7)   order by deltaexpense desc"

    '                '      req = "SELECT top 20 *  FROM [OptimizerLog]  where deltaexpense > -1 and  left (modelrunid, 3) = '347' and caslinebreaks <= (foslinebreaks + 7)   order by deltaexpense desc"
    '                req = Replace(req, "347", customid)
    '                req = Replace(req, "(modelrunid, 3)", "(modelrunid, " & Len(customid) & ")")


    '                Dim cnmkazurelocal As New ADODB.Connection

    '                Dim rs As New ADODB.Recordset


    '                '20120724 - fix connection strings
    '                'If cnsetting.State = 0 Then
    '                '    'cnsetting.ConnectionString = ConnectionStringHelper.GetConnectionStringSQL
    '                '    cnsetting.ConnectionString = ConnectionStringSQL
    '                '    cnsetting.Open()
    '                'End If
    '                If cnmkazurelocal.State = 0 Then
    '                    cnmkazurelocal.ConnectionString = ConnectionStringHelper.GetConnectionStringSQLMKAzure
    '                    cnmkazurelocal.ConnectionTimeout = 30
    '                    cnmkazurelocal.CommandTimeout = 30
    '                    cnmkazurelocal.Open()
    '                End If


    '                If rs.State = 1 Then rs.Close()

    '                rs.Open(req, cnmkazurelocal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)


    '                If Not rs.EOF Then
    '                    If Not IsDBNull(rs.Fields("day1").Value) Then
    '                        GridView3.Columns(2).HeaderTooltip = rs.Fields("day1").Value
    '                        GridView3.Columns(3).HeaderTooltip = rs.Fields("day1").Value

    '                        GridView3.Columns(4).HeaderTooltip = rs.Fields("day2").Value
    '                        GridView3.Columns(5).HeaderTooltip = rs.Fields("day2").Value

    '                        GridView3.Columns(6).HeaderTooltip = rs.Fields("day3").Value
    '                        GridView3.Columns(7).HeaderTooltip = rs.Fields("day3").Value

    '                        GridView3.Columns(8).HeaderTooltip = rs.Fields("day4").Value
    '                        GridView3.Columns(9).HeaderTooltip = rs.Fields("day4").Value
    '                    End If

    '                End If

    '                If rs.State = 1 Then rs.Close()

    '            Catch ex As Exception

    '            End Try


    '        End If
    '    End If

    'End Sub

    Private Sub GridView3_SelectedCellChanged(sender As Object, e As EventArgs) Handles GridView3.SelectedCellChanged
        updategrid()
    End Sub

    'Private Sub GridView3_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView3.RowCommand

    '    Dim s As String
    '    Dim si As Integer
    '    If IsNumeric(e.CommandArgument.ToString) Then


    '        si = CInt(e.CommandArgument.ToString)

    '        Dim customid As String = Trim(GridView3.Rows.Item(si).Cells(0).Text)



    '        Response.Redirect("/Panel.aspx?modelrunid=" & customid)
    '    End If

    'End Sub




    Private Sub GridView3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridView3.SelectedIndexChanged
        updategrid()


        ''          var dataItem = RadGrid1.SelectedItems[0] as GridDataItem; 
        ''10	        if (dataItem != null) 
        ''11	        { 
        ''12	            var name = dataItem["ProductName"].Text; 
        ''13	            Literal1.Text += String.Format("{0}<br/>", name); 


        'Dim s As String
        'Dim si As Integer

        'Dim var As GridDataItem

        'If GridView3.SelectedItems.Count <> 0 Then

        '    var = GridView3.SelectedItems(0)
        '    If Not (IsNothing(var)) Then

        '        s = var("modelrunid").Text


        '    End If



        '    Dim customid As String = Trim(s)



        '    Response.Redirect("/Panel.aspx?modelrunid=" & customid)
        '    ' End If

        '    'End Sub
        'End If

    End Sub


    Protected Sub GridView3_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles GridView3.NeedDataSource


        updategrid()

        'Dim s As String
        'Dim si As Integer

        'Dim var As GridDataItem

        'If GridView3.SelectedItems.Count <> 0 Then

        '    var = GridView3.SelectedItems(0)
        '    If Not (IsNothing(var)) Then

        '        s = var("modelrunid").Text


        '    End If



        '    Dim customid As String = Trim(s)



        '    Response.Redirect("/Panel.aspx?modelrunid=" & customid)
        '    ' End If

        '    'End Sub
        'End If
    End Sub



End Class
