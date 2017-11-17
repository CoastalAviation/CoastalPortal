Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports CoastalPortal.AirTaxi



Public Class SaveQuoteInfo
    Dim aPriceDesc As New ArrayList
    Dim aPriceAmt As New ArrayList


    Public Sub QuoteInfoArray(ByVal p_desc As String, ByVal p_amt As String)
        'add info to global array
        aPriceDesc.Add(p_desc)
        aPriceAmt.Add(p_amt)
    End Sub

    Public Sub InsertQuote()

        '20111222 - pab - skip - save storage
        Exit Sub

        Dim retval As Integer

        retval = InsertQuoteMaster()
        InsertQuoteDetail(retval, aPriceDesc, aPriceAmt)
    End Sub

    Public Function InsertQuoteMaster() As Integer

        '20111222 - pab - skip - save storage
        Return 0
        Exit Function

        Dim oConn As New SqlConnection
        Dim oCmd As New SqlCommand
        Dim oParam As SqlParameter

        'oConn = New SqlConnection(xonfigurationmanager.ConnectionStrings("Portal").ConnectionString)
        oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

        '20110607 - pab - fix timeout issue
        If oConn.ConnectionString = "" Then
            InsertQuoteMaster = 0
            Exit Function
        End If

        oCmd = New SqlCommand("sp_deb_quotemaster")
        oCmd.CommandType = CommandType.StoredProcedure

        oParam = Nothing

        '        Using System.Web.HttpRequest
        '2:
        '3:          Dim client_ip As String
        '4:
        '5:          client_ip = Request.UserHostAddress()

        oParam = New SqlParameter
        oParam.ParameterName = "@IPinfo"
        oParam.Value = 1        '-- ~DHA ***this will need to be changed
        oCmd.Parameters.Add(oParam)

        oParam = New SqlParameter()
        oParam.Direction = ParameterDirection.ReturnValue
        oCmd.Parameters.Add(oParam)

        oConn.Open()
        oCmd.Connection = oConn
        oCmd.ExecuteNonQuery()
        oConn.Close()

        Return oParam.Value    'contains return value from stored proc

    End Function

    Public Sub InsertQuoteDetail(ByVal p_id As Integer, ByRef p_alabel As ArrayList, ByRef p_aamt As ArrayList)

        '20111222 - pab - skip - save storage
        Exit Sub

        Dim oConn As New SqlConnection
        Dim oCmd As New SqlCommand
        Dim oParam As SqlParameter
        Dim t As Integer

        Try
            'oConn = New SqlConnection(xonfigurationmanager.ConnectionStrings("Portal").ConnectionString)
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oCmd = New SqlCommand("sp_deb_quotedetail")
            oCmd.CommandType = CommandType.StoredProcedure

            oConn.Open()
            oCmd.Connection = oConn

            t = p_alabel.Count - 1
            For i As Integer = 0 To t

                oParam = New SqlParameter
                oParam.ParameterName = "@QuoteID"
                oParam.Value = p_id
                oCmd.Parameters.Add(oParam)

                oParam = New SqlParameter
                oParam.ParameterName = "@Descr"
                oParam.DbType = DbType.String
                oParam.Size = 200
                oParam.Value = p_alabel(i).ToString
                oCmd.Parameters.Add(oParam)

                oParam = New SqlParameter
                oParam.ParameterName = "@DescrAmt"
                oParam.DbType = DbType.String
                oParam.Size = 200
                oParam.Value = p_aamt(i).ToString
                oCmd.Parameters.Add(oParam)

                oCmd.ExecuteNonQuery()
                oCmd.Parameters.Clear()

            Next i

            oConn.Close()
        Catch ex As Exception
            'message box doesn't work on web causese error rk 10.4.2011
            '     MsgBox(ex.Message, MsgBoxStyle.Critical, "Exception")
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(0, AirTaxi.appName, Left(s, 500), "InsertQuoteDetail", "SaveQuoteInfo.aspx.vb")
            '20131024 - pab - fix duplicate emails
            AirTaxi.SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " SaveQuoteInfo.aspx InsertQuoteDetail Error", s, 0)
        End Try

    End Sub


End Class
