Imports System.Web.Script.Serialization
Imports Telerik.Web.UI


Public Class AcceptRejectChange
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Session("readonly") = "true" Then
            Me.btnOk.Enabled = False
        Else
            Me.btnOk.Enabled = True

        End If

        Dim s As String = Request.QueryString("id")



        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.GetMKConnectionStringsql
            cn.Open()
        End If


        req = "select * from casflightsoptimizer where id  =" & s

        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        If Not rs.EOF Then

            Dim savings As Integer
            If Not (IsDBNull(rs.Fields("PriorTailSavings").Value)) Then
                savings = rs.Fields("PriorTailSavings").Value
            Else
                savings = 0
            End If

            Dim priortail As String = rs.Fields("PriorTail").Value
            Dim registration As String = rs.Fields("AircraftRegistration").Value
            Dim tripnumber As String = rs.Fields("TripNUmber").Value


            LblChange.Text = "Swapping " & registration & " for " & priortail & " saves " & FormatCurrency(savings, 0)

            Me.Page.Title = priortail & " -> " & registration


            Dim ss As String
            ss = rs.Fields("OriginalRepoDetail").Value
            ss = Replace(ss, "</br>", vbNewLine)
            txtBefore.Text = rs.Fields("OriginalCostWRepo").Value & vbNewLine & ss


            ss = rs.Fields("SavingsDescriptionExt").Value
            ss = Replace(ss, "</br>", vbNewLine)
            txtAfter.Text = ss


        End If

        'rk 12.13.2013 added this check back in.
        If DropDownList1.Items.Count = 0 Then
            loaddropdownlist()
        End If


    End Sub

    Protected Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click


        If Session("carrierid") = 0 Then
            Lblmsg.Text = "Credentials Lost please log in again"
            Exit Sub

        End If


        'Dim data As New Dictionary(Of String, String)
        'data.Add("checkBoxValue", Me.RadioButtonList1.SelectedItem.Text)

        'If data.Count > 0 Then
        '    Dim seraildata As String = New JavaScriptSerializer().Serialize(data)
        '    Me.hidData.Value = seraildata
        '    Me.hidIsClose.Value = "1"
        'End If

        Dim version As String
        Dim foskey As String
        Dim ss As String = RadioButtonList1.SelectedItem.Value

        If RadioButtonList1.SelectedItem.Value <> "Accept and Email" And RadioButtonList1.SelectedItem.Value <> "Accept" Then
            If DropDownList1.SelectedItem.Value = "Select a Reject Reason" Then
                Lblmsg.Text = "Please select a reject reason"
                Exit Sub
            End If
        End If



        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.GetMKConnectionStringsql
            cn.Open()
        End If

        If Me.txtEnterNewReason.Text <> "" Then

            req = "select * from sys_values where carrierid = '" & Session("carrierid") & "' and description = '" & Me.txtEnterNewReason.Text & "'"

            If rs.State = 1 Then rs.Close()
            rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


            If rs.EOF Then rs.AddNew()

            rs.Fields("Description").Value = Me.txtEnterNewReason.Text
            rs.Fields("ControlName").Value = "DDReject"
            rs.Fields("CarrierId").Value = Session("carrierid")
            rs.Fields("DataType").Value = "S"
            rs.Fields("UserCanModify").Value = True
            rs.Fields("Value").Value = "USR"

            rs.Update()

            loaddropdownlist()
            '    DropDownList1.Items.Clear()
            If rs.State = 1 Then rs.Close()

        End If

        Dim s As String = Request.QueryString("id")

        '________________________________________________________________________________




        req = "select * from casflightsoptimizer where id  = " & s

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        If Not rs.EOF Then

            Dim savings As Integer = rs.Fields("PriorTailSavings").Value
            Dim priortail As String = rs.Fields("PriorTail").Value.ToString.Trim
            Dim registration As String = rs.Fields("AircraftRegistration").Value.ToString.Trim
            Dim OptimizerRun As String = rs.Fields("OptimizerRun").Value.ToString.Trim
            Dim tripnumber As String = rs.Fields("TripNUmber").Value.ToString.Trim
            Dim DepartureAirport As String = rs.Fields("DepartureAirport").Value.ToString.Trim
            Dim ArrivalAirport As String = rs.Fields("ArrivalAirport").Value.ToString.Trim

            Dim DepartureDateGMT As String
            Dim ArrivalDateGMT As String

            foskey = rs.Fields("foskey").Value.ToString.Trim
            version = rs.Fields("version").Value.ToString.Trim

            If Not (IsDBNull(rs.Fields("DepartureTime").Value)) Then
                DepartureDateGMT = rs.Fields("DepartureTime").Value
            End If

            If Not (IsDBNull(rs.Fields("ArrivalTime").Value)) Then
                ArrivalDateGMT = rs.Fields("ArrivalTime").Value
            End If


            Dim AircraftRegistration As String = rs.Fields("AircraftRegistration").Value




            drpDown.Items.Add("Swapping " & registration & " for " & priortail & " saves " & FormatCurrency(savings, 0) & "<br/>")


            drpDown.Items.Add("CAS ---> Model" & "<br/>")


            req = "select * FROM [OptimizerWest].[dbo].[CASFlightsOptimizer] where [OptimizerRun] = '8527-5/27/2015-R16-33C-4' and (aircraftregistration = 'N506UP' or aircraftregistration = 'N805UP')"

            req = Replace(req, "N506UP", priortail)
            req = Replace(req, "N805UP", registration)
            req = Replace(req, "8527-5/27/2015-R16-33C-4", OptimizerRun)

            If rs.State = 1 Then rs.Close()
            rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)



            Do While Not rs.EOF

                Dim DepartureAirportCAS As String = rs.Fields("DepartureAirport").Value.ToString.Trim
                Dim ArrivalAirportCAS As String = rs.Fields("ArrivalAirport").Value.ToString.Trim
                Dim tripnumberCAS As String = rs.Fields("TripNUmber").Value.ToString.Trim
                Dim departuretimecasGMT As Date = rs.Fields("Departuretime").Value
                Dim arrivaltimecasGMT As Date = rs.Fields("arrivaltime").Value
                Dim ac2 As String = rs.Fields("aircraftregistration").Value.ToString.Trim
                Dim ft As String = rs.Fields("FlightType").Value.ToString.Trim
                foskey = rs.Fields("foskey").Value.ToString.Trim
                version = rs.Fields("version").Value.ToString.Trim

                req = "select * from fosflightsoptimizer where   [OptimizerRun] = '8527-5/27/2015-R0-33C'   and legtypecode <> 'LINB' and departureairporticao = 'abcde' and arrivalairporticao = 'fghij' and tripnumber = 'lmnop' " ' and foskey = '123123'"
                req = Replace(req, "abcde", DepartureAirportCAS)
                req = Replace(req, "fghij", ArrivalAirportCAS)
                req = Replace(req, "lmnop", tripnumberCAS)
                req = Replace(req, "123123", foskey)

                '  req = Replace(req, "N506UP", ac2)
                req = Replace(req, "8527-5/27/2015-R0-33C", AirTaxi.normalizemodelrunid(OptimizerRun))



                Dim rs2 As New ADODB.Recordset
                If rs2.State = 1 Then rs2.Close()
                rs2.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

                Dim eventstring As String = ""


                eventstring = "NoChange"

                If rs2.EOF Then
                    eventstring = "AddLeg"
                End If


                If Not rs2.EOF Then
                    Dim ddate As Date
                    ddate = CDate(rs2.Fields("DepartureDateGMT").Value & " " & rs2.Fields("DeparturetimeGMT").Value)

                    If ddate = departuretimecasGMT Then
                        eventstring = "NoChange"
                    Else
                        eventstring = "Slide"
                    End If

                    Dim fosasc As String = rs2.Fields("ac").Value.ToString.Trim
                    If rs2.Fields("ac").Value.ToString.Trim <> ac2 Then eventstring = "AssignTail"

                    version = rs2.Fields("version").Value

                End If



                If eventstring <> "NoChange" Then
                    req = "select * from RejectedFlights where carrierid = '" & "100" & "' and tripnumber = '" & tripnumberCAS & "' and DepartureAirport = '" & DepartureAirportCAS & "' and ArrivalAirport = '" & ArrivalAirportCAS & "'"
                    req = Replace(req, "100", Session("carrierid"))

                    Dim rs4 As New ADODB.Recordset

                    If rs4.State = 1 Then rs4.Close()
                    rs4.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


                    If rs4.EOF Then rs4.AddNew()



                    rs4.Fields("CarrierId").Value = Session("carrierid")
                    rs4.Fields("TripNumber").Value = tripnumberCAS
                    rs4.Fields("DepartureAirport").Value = DepartureAirportCAS
                    rs4.Fields("ArrivalAirport").Value = ArrivalAirportCAS
                    rs4.Fields("RejectedOn").Value = Now
                    rs4.Fields("Rejected").Value = True

                    rs4.Fields("action").Value = eventstring
                    rs4.Fields("TripType").Value = ft

                    rs4.Fields("TripNumber").Value = tripnumberCAS
                    rs4.Fields("FromDateGMT").Value = departuretimecasGMT
                    rs4.Fields("ToDateGMT").Value = arrivaltimecasGMT
                    rs4.Fields("AircraftRegistration").Value = ac2.ToString.Trim
                    rs4.Fields("PriorTail").Value = priortail
                    rs4.Fields("FOSKEY").Value = foskey
                    rs4.Fields("version").Value = version
                    '  rs4.Fields("version").Value = version

                    rs4.Update()


                    drpDown.Items.Add(eventstring & "  D:" & DepartureAirportCAS & "  A:" & ArrivalAirportCAS & "  T:" & tripnumberCAS & "<br/>")


                    '    If rs.State = 1 Then rs.Close()




                End If


                rs.MoveNext()

            Loop


            'Assigncrew()
            'Assigntail()
            'Slide()
            'Remove()
            'add()




            drpDown.Items.Add("Model ---> CAS" & "<br/>")



            req = "select * from fosflightsoptimizer where   [OptimizerRun] = '8527-5/27/2015-R0-33C' and   (ac = 'N506UP' or ac = 'N805UP') and legtypecode <> 'LINB' "
            req = Replace(req, "abcde", DepartureAirport)
            req = Replace(req, "fghij", ArrivalAirport)
            req = Replace(req, "lmnop", tripnumber)
            req = Replace(req, "8527-5/27/2015-R0-33C", AirTaxi.normalizemodelrunid(OptimizerRun))
            req = Replace(req, "N506UP", priortail)
            req = Replace(req, "N805UP", registration)

            ' req = "select * FROM [OptimizerWest].[dbo].[CASFlightsOptimizer] where [OptimizerRun] = '8527-5/27/2015-R16-33C-4' and (aircraftregistration = 'N506UP' or aircraftregistration = 'N805UP')"



            If rs.State = 1 Then rs.Close()
            rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)



            Do While Not rs.EOF

                Dim DepartureAirportCAS As String = rs.Fields("DepartureAirporticao").Value.ToString.Trim
                Dim ArrivalAirportCAS As String = rs.Fields("ArrivalAirporticao").Value.ToString.Trim
                Dim tripnumberCAS As String = rs.Fields("TripNUmber").Value.ToString.Trim
                Dim departuretimecasGMT As Date = CDate(rs.Fields("DepartureDateGMT").Value & " " & rs.Fields("DeparturetimeGMT").Value)
                Dim arrivaltimecasGMT As Date = CDate(rs.Fields("arrivalDateGMT").Value & " " & rs.Fields("arrivaltimeGMT").Value)
                Dim ac As String = rs.Fields("ac").Value.ToString.Trim
                foskey = rs.Fields("foskey").Value.ToString.Trim
                version = rs.Fields("version").Value.ToString.Trim


                req = "select * from casflightsoptimizer where   [OptimizerRun] = '8527-5/27/2015-R16-33C-4'      and departureairport = 'abcde' and arrivalairport = 'fghij' and tripnumber = 'lmnop' " ' and foskey = '123123'"
                req = Replace(req, "abcde", DepartureAirportCAS)
                req = Replace(req, "fghij", ArrivalAirportCAS)
                req = Replace(req, "lmnop", tripnumberCAS)
                req = Replace(req, "N506UP", ac)
                req = Replace(req, "8527-5/27/2015-R16-33C-4", OptimizerRun)
                req = Replace(req, "123123", foskey)
                LblChange.Text = "Swapping " & registration & " for " & priortail & " saves " & FormatCurrency(savings, 0)

                Me.Page.Title = priortail & " -> " & registration




                Dim rs2 As New ADODB.Recordset
                If rs2.State = 1 Then rs2.Close()
                rs2.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

                Dim eventstring As String = ""

                If rs2.EOF Then
                    eventstring = "Remove"
                End If


                Dim ft1 As String = ""

                Dim casac As String

                If Not rs2.EOF Then
                    Dim ddate As Date
                    Dim ddatestring As String = (rs2.Fields("DepartureDateGMT").Value & " " & rs2.Fields("DeparturetimeGMT").Value)
                    If Not (IsDate(ddatestring)) Then
                        ddate = ddate
                    End If
                    If IsDate((rs.Fields("DepartureDateGMT").Value & " " & rs.Fields("DeparturetimeGMT").Value)) Then
                        ddate = CDate(rs.Fields("DepartureDateGMT").Value & " " & rs.Fields("DeparturetimeGMT").Value)
                    End If

                    If ddate = departuretimecasGMT Then
                        eventstring = "NoChange"
                    Else
                        eventstring = "Slide"
                    End If

                    If rs2.Fields("AircraftRegistration").Value.ToString.Trim <> ac Then eventstring = "AssignTail"

                    casac = rs2.Fields("AircraftRegistration").Value.ToString.Trim
                    ft1 = rs2.Fields("FlightType").Value.ToString.Trim

                End If



                If eventstring <> "NoChange" Then
                    req = "select * from RejectedFlights where carrierid = '" & "100" & "' and tripnumber = '" & tripnumberCAS & "' and DepartureAirport = '" & DepartureAirportCAS & "' and ArrivalAirport = '" & ArrivalAirportCAS & "'"
                    req = Replace(req, "100", Session("carrierid"))

                    Dim rs3 As New ADODB.Recordset

                    If rs3.State = 1 Then rs3.Close()
                    rs3.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


                    If rs3.EOF Then rs3.AddNew()



                    rs3.Fields("CarrierId").Value = Session("carrierid")
                    rs3.Fields("TripNumber").Value = tripnumber
                    rs3.Fields("DepartureAirport").Value = DepartureAirportCAS
                    rs3.Fields("ArrivalAirport").Value = ArrivalAirportCAS
                    rs3.Fields("RejectedOn").Value = Now
                    rs3.Fields("Rejected").Value = True

                    rs3.Fields("action").Value = eventstring
                    rs3.Fields("TripType").Value = ft1
                    rs3.Fields("TripNumber").Value = tripnumberCAS
                    rs3.Fields("FromDateGMT").Value = departuretimecasGMT
                    rs3.Fields("ToDateGMT").Value = arrivaltimecasGMT
                    rs3.Fields("AircraftRegistration").Value = casac
                    rs3.Fields("PriorTail").Value = ac

                    rs3.Fields("FOSKEY").Value = foskey
                    rs3.Fields("version").Value = version
                    rs3.Update()


                    drpDown.Items.Add(eventstring & "  D:" & DepartureAirportCAS & "  A:" & ArrivalAirportCAS & "  T:" & tripnumberCAS & "<br/>")






                End If


                rs.MoveNext()

            Loop






        End If











        '_____________________________________________________________________________________

        'req = "select * from casflightsoptimizer where id  =" & s

        'If rs.State = 1 Then rs.Close()
        'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        'If Not rs.EOF Then

        '    Dim savings As Integer = rs.Fields("PriorTailSavings").Value
        '    Dim priortail As String = rs.Fields("PriorTail").Value.ToString.Trim
        '    Dim registration As String = rs.Fields("AircraftRegistration").Value.ToString.Trim
        '    Dim tripnumber As String = rs.Fields("TripNUmber").Value.ToString.Trim
        '    Dim DepartureAirport As String = rs.Fields("DepartureAirport").Value.ToString.Trim
        '    Dim ArrivalAirport As String = rs.Fields("ArrivalAirport").Value.ToString.Trim

        '    Dim DepartureDateGMT As String
        '    Dim ArrivalDateGMT As String



        '    If Not (IsDBNull(rs.Fields("DepartureTime").Value)) Then
        '        DepartureDateGMT = rs.Fields("DepartureTime").Value
        '    End If

        '    If Not (IsDBNull(rs.Fields("ArrivalTime").Value)) Then
        '        ArrivalDateGMT = rs.Fields("ArrivalTime").Value
        '    End If


        '    Dim AircraftRegistration As String = rs.Fields("AircraftRegistration").Value



        ''LblChange.Text = "Swapping " & registration & " for " & priortail & " saves " & FormatCurrency(savings, 0)

        ''Me.Page.Title = priortail & " -> " & registration




        'req = "select * from RejectedFlights where carrierid = '" & Session("carrierid") & "' and tripnumber = '" & tripnumber & "' and DepartureAirport = '" & DepartureAirport & "' and ArrivalAirport = '" & ArrivalAirport & "'"


        ''     req = "select * from RejectedFlights where carrierid = '" & Session("carrierid") & "' and tripnumber = '" & tripnumber & "'"

        'If rs.State = 1 Then rs.Close()
        'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


        'If rs.EOF Then rs.AddNew()



        'Dim reason As String

        'reason = ""

        'If txtEnterNewReason.Text <> "" Then
        '    reason = txtEnterNewReason.Text
        '    txtEnterNewReason.Text = ""
        'End If

        ''rk deal with reason code if entered new
        ''10/21/2013
        'If reason = "" Then
        '    If Not (IsNothing(DropDownList1.SelectedItem.ToString)) Then
        '        reason = DropDownList1.SelectedItem.ToString
        '    End If
        'End If



        'rs.Fields("Reason").Value = reason

        'If RadioButtonList1.SelectedItem.Value = "Accept and Email" Then rs.Fields("Reason").Value = "Accept and Email"


        'rs.Fields("CarrierId").Value = Session("carrierid")
        'rs.Fields("TripNumber").Value = tripnumber
        'rs.Fields("DepartureAirport").Value = DepartureAirport
        'rs.Fields("ArrivalAirport").Value = ArrivalAirport
        'rs.Fields("RejectedOn").Value = Now
        'rs.Fields("Rejected").Value = True

        'rs.Fields("action").Value = RadioButtonList1.SelectedItem.Value

        'rs.Fields("TripNumber").Value = tripnumber
        'rs.Fields("FromDateGMT").Value = DepartureDateGMT
        'rs.Fields("ToDateGMT").Value = ArrivalDateGMT
        'rs.Fields("AircraftRegistration").Value = AircraftRegistration.ToString.Trim
        'rs.Fields("PriorTail").Value = priortail



        'rs.Update()

        'If rs.State = 1 Then rs.Close()


        'End If

        Me.Lblmsg.Text = "Change Confirmed"

    End Sub

    Protected Sub DropDownList1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList1.SelectedIndexChanged
        If DropDownList1.SelectedValue = "Not Listed" Then
            txtEnterNewReason.Visible = True
        End If
    End Sub

    Protected Sub LinkButton1_Click(sender As Object, e As EventArgs) Handles LinkButton1.Click
        txtEnterNewReason.Visible = True
    End Sub

    Public Sub loaddropdownlist()
        DropDownList1.Items.Clear()
        DropDownList1.Items.Add("Select a Reject Reason")


        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.GetMKConnectionStringsql
            cn.Open()
        End If


        req = "select * from sys_values where controlname = 'DDReject' and carrierid  = '" & Session("carrierid") & "'"

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        Do While Not rs.EOF

            DropDownList1.Items.Add(rs.Fields("Description").Value)

            rs.MoveNext()
        Loop


        Dim it As New System.Web.UI.WebControls.ListItem

        it.Text = "Not Listed"
        it.Value = "Not Listed"


        DropDownList1.Items.Add(it)

    End Sub

    Protected Sub btnMore_Click(sender As Object, e As EventArgs) Handles btnMore.Click



        Dim tst As Telerik.Web.UI.RadWindowManager




        'Try
        '    tst = Parent.FindControl("radWinMgr")
        '    tst.Windows(0).Height = Unit.Pixel(800)
        '    tst.Windows(0).Width = Unit.Pixel(1200)



        'Catch

        'End Try




        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.GetMKConnectionStringsql
            cn.Open()
        End If



        Dim s As String = Request.QueryString("id")




        req = "select * from casflightsoptimizer where id  =" & s

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        If Not rs.EOF Then

            Dim savings As Integer = rs.Fields("PriorTailSavings").Value.ToString.Trim
            Dim OptimizerRun As String = rs.Fields("OptimizerRun").Value.ToString.Trim
            Dim priortail As String = rs.Fields("PriorTail").Value.ToString.Trim
            Dim registration As String = rs.Fields("AircraftRegistration").Value.ToString.Trim
            Dim tripnumber As String = rs.Fields("TripNUmber").Value.ToString.Trim
            Dim DepartureAirport As String = rs.Fields("DepartureAirport").Value.ToString.Trim
            Dim ArrivalAirport As String = rs.Fields("ArrivalAirport").Value.ToString.Trim

            ' Dim DepartureDateGMT As String
            'Dim ArrivalDateGMT As String

            req = "flights24.aspx?modelsavings=A0[{0}]A1[{1}]A2[{2}]"
            req = Replace(req, "{0}", OptimizerRun)
            req = Replace(req, "{1}", priortail)
            req = Replace(req, "{2}", registration)
            Response.Redirect(req, False)

        End If


    End Sub

    'Protected Sub btnOk0_Click(sender As Object, e As EventArgs) Handles btnOk0.Click
    '    Response.Redirect("savingsRealTime.aspx")
    'End Sub
End Class