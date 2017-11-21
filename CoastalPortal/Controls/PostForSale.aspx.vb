Imports System.Web.Script.Serialization
Imports Telerik.Web.UI
Imports CoastalPortal.PanelClasses
Imports CoastalPortal.Models

Public Class PostForSale
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim casFlightRecords As New List(Of CASFlightsOptimizerRecord)
        Dim s As String = Request.QueryString("id")


        casFlightRecords = Session("CASRecords")

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

            Dim flighttype As String = rs.Fields("FlightType").Value

            LblChange.Text = "Post Empty Leg  " & registration & " for sale " '& priortail & " saves " & FormatCurrency(savings, 0)

            Me.Page.Title = "Post Empty Leg -> " & registration

            If flighttype <> "D" Then
                LblChange.Text = "Warning not empty leg - type " & flighttype
            End If
        End If

        'rk 12.13.2013 added this check back in.
        If DropDownList1.Items.Count = 0 Then
            loaddropdownlist()
        End If


    End Sub

    Protected Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click

        'Dim data As New Dictionary(Of String, String)
        'data.Add("checkBoxValue", Me.RadioButtonList1.SelectedItem.Text)

        'If data.Count > 0 Then
        '    Dim seraildata As String = New JavaScriptSerializer().Serialize(data)
        '    Me.hidData.Value = seraildata
        '    Me.hidIsClose.Value = "1"
        'End If


        If RadioButtonList1.SelectedItem.Value <> "Accept and Email" Then
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


        req = "select * from casflightsoptimizer where id  =" & s

        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        If Not rs.EOF Then

            Dim savings As Integer = rs.Fields("PriorTailSavings").Value
            Dim priortail As String = rs.Fields("PriorTail").Value.ToString.Trim
            Dim registration As String = rs.Fields("AircraftRegistration").Value.ToString.Trim
            Dim tripnumber As String = rs.Fields("TripNUmber").Value.ToString.Trim
            Dim DepartureAirport As String = rs.Fields("DepartureAirport").Value.ToString.Trim
            Dim ArrivalAirport As String = rs.Fields("ArrivalAirport").Value.ToString.Trim

            Dim DepartureDateGMT As String
            Dim ArrivalDateGMT As String



            If Not (IsDBNull(rs.Fields("DepartureTime").Value)) Then
                DepartureDateGMT = rs.Fields("DepartureTime").Value
            End If

            If Not (IsDBNull(rs.Fields("ArrivalTime").Value)) Then
                ArrivalDateGMT = rs.Fields("ArrivalTime").Value
            End If


            Dim AircraftRegistration As String = rs.Fields("AircraftRegistration").Value



            LblChange.Text = "Swapping " & registration & " for " & priortail & " saves " & FormatCurrency(savings, 0)

            Me.Page.Title = priortail & " -> " & registration






            req = "select * from SellEmptyFlights where carrierid = '" & Session("carrierid") & "' and tripnumber = '" & tripnumber & "'"

            If rs.State = 1 Then rs.Close()
            rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)


            If rs.EOF Then rs.AddNew()



            Dim reason As String

            reason = ""

            If txtEnterNewReason.Text <> "" Then
                reason = txtEnterNewReason.Text
                txtEnterNewReason.Text = ""
            End If

            'rk deal with reason code if entered new
            '10/21/2013
            If reason = "" Then
                If Not (IsNothing(DropDownList1.SelectedItem.ToString)) Then
                    reason = DropDownList1.SelectedItem.ToString
                End If
            End If



            rs.Fields("Reason").Value = reason

            If RadioButtonList1.SelectedItem.Value = "Accept and Email" Then rs.Fields("Reason").Value = "Accept and Email"


            rs.Fields("CarrierId").Value = Session("carrierid")
            rs.Fields("TripNumber").Value = tripnumber
            rs.Fields("DepartureAirport").Value = DepartureAirport
            rs.Fields("ArrivalAirport").Value = ArrivalAirport
            rs.Fields("RejectedOn").Value = Now
            rs.Fields("Rejected").Value = True

            rs.Fields("action").Value = RadioButtonList1.SelectedItem.Value

            rs.Fields("TripNumber").Value = tripnumber
            rs.Fields("FromDateGMT").Value = DepartureDateGMT
            rs.Fields("ToDateGMT").Value = ArrivalDateGMT
            rs.Fields("AircraftRegistration").Value = AircraftRegistration.ToString.Trim
            rs.Fields("PriorTail").Value = priortail

            If IsNumeric(txtSalePrice.Text) Then
                rs.Fields("SellPrice").Value = txtSalePrice.Text
            Else
                rs.Fields("SellPrice").Value = 0
            End If


            rs.Update()

            If rs.State = 1 Then rs.Close()


        End If

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

            Dim DepartureDateGMT As String
            Dim ArrivalDateGMT As String

            req = "flights24.aspx?modelsavings=A0[{0}]A1[{1}]A2[{2}]"
            req = Replace(req, "{0}", OptimizerRun)
            req = Replace(req, "{1}", priortail)
            req = Replace(req, "{2}", registration)
            Response.Redirect(req, False)

        End If


    End Sub

   
End Class