Imports System.Web.Script.Serialization
Imports Optimizer.AirTaxi

Public Class Hotel
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim s As String = Request.QueryString("id")



        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.GetMKConnectionStringsql
            cn.Open()
        End If

        If s = "" Then s = "1520625"

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

            Dim PIC As String = rs.Fields("PIC").Value
            Dim sic As String = rs.Fields("SIC").Value

            LblChange.Text = PIC & "  " & sic

            lblArrivingAt.Text = "Arriving at: " & rs.Fields("ArrivalAirport").Value & " at " & rs.Fields("ArrivalDateTimeLocal").Value

            '  LblChange.Text = "Swapping " & registration & " for " & priortail & " saves " & FormatCurrency(savings, 0)

            Me.Page.Title = priortail & " -> " & registration

        End If

        'rk 12.13.2013 added this check back in.
        'If DropDownList1.Items.Count = 0 Then
        '    loaddropdownlist()
        'End If

        Dim apt As String = rs.Fields("ArrivalAirport").Value



        req = "select * from hotels where airport = '" & apt & "' order by preference "
        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        If Not rs.EOF Then DropDownList1.Items.Clear()
        Do While Not rs.EOF

            s = rs.Fields("Hotel").Value & " distance:" & rs.Fields("distance").Value

            DropDownList1.Items.Add(s)



            rs.MoveNext()
        Loop


    End Sub

    Protected Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click

        Dim s As String = Request.QueryString("id")



        Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset
        Dim req As String


        If cn.State = 1 Then cn.Close()
        If cn.State = 0 Then
            cn.ConnectionString = ConnectionStringHelper.GetMKConnectionStringsql
            cn.Open()
        End If

        If s = "" Then s = "1520625"

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

            Dim PIC As String = rs.Fields("PIC").Value
            Dim sic As String = rs.Fields("SIC").Value

            LblChange.Text = PIC & "  " & sic

            lblArrivingAt.Text = "Arriving at: " & rs.Fields("ArrivalAirport").Value & " at " & rs.Fields("ArrivalDateTimeLocal").Value

            '  LblChange.Text = "Swapping " & registration & " for " & priortail & " saves " & FormatCurrency(savings, 0)

            Me.Page.Title = priortail & " -> " & registration

        End If

        'rk 12.13.2013 added this check back in.
        'If DropDownList1.Items.Count = 0 Then
        '    loaddropdownlist()
        'End If

        Dim apt As String = rs.Fields("ArrivalAirport").Value



        req = "select * from hotels where airport = '" & apt & "' order by preference "
        If rs.State = 1 Then rs.Close()
        rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        If Not rs.EOF Then DropDownList1.Items.Clear()

        Dim hotellist As String = ""

        Do While Not rs.EOF

            s = rs.Fields("Hotel").Value & " distance:" & rs.Fields("distance").Value

            ' DropDownList1.Items.Add(s)

            hotellist = hotellist & s & "<br/>"



            rs.MoveNext()
        Loop

        Dim body As String
        body = LblChange.Text & "<br/>"

        body = body & lblArrivingAt.Text & "<br/>"

        body = body & "<br/>"

        body = body & hotellist

        SendEmail("support@coastalaviationsoftware.com", Me.TextBox1.Text, "HC Request" & lblArrivingAt.Text, body, 65)



        Me.Lblmsg.Text = "Request Sent "

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

End Class