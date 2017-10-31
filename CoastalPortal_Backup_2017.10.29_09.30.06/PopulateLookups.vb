Imports System.Data.SqlClient
Imports CoastalPortal.AirTaxi

Public Class PopulateLookups

    Public Function TimeDD(ByVal Interval As String) As Data.DataTable
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Select Case Interval.ToUpper
            Case "HOUR"
            Case "HALF"
            Case "ALL"
            Case Else
                dt = Nothing
        End Select


        Using conn As New SqlConnection()

            '20130107 pab - fix error - The ConnectionString property has not been initialized
            'conn.ConnectionString = ConnectionStringHelper.GetConnectionString()
            conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)

            Using cmd As New SqlCommand
                cmd.Connection = conn
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandText = "sp_PopulateTimeDD"

                Dim sqlParam As SqlParameter

                sqlParam = New SqlParameter("@TimeFlag", SqlDbType.VarChar, 4, ParameterDirection.Input)
                sqlParam.Value = Interval
                cmd.Parameters.Add(sqlParam)

                da = New SqlDataAdapter(cmd)

                Try
                    conn.Open()
                    da.Fill(dt)
                Catch ex As Exception
                    '20120107 - pab - fix error - Showing a modal dialog box or form when the application is not running in UserInteractive mode is not a valid operation
                    'MsgBox(ex.Message, MsgBoxStyle.Critical, "ERROR")
                    Dim s As String = ex.Message
                    If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
                    If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
                    AirTaxi.Insertsys_log(0, AirTaxi.appName, Left(s, 500), "TimeDD", "PopulateLookups.aspx.vb")
                    '20131024 - pab - fix duplicate emails
                    AirTaxi.SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " PopulateLookups.aspx TimeDD Error", s, 0)
                End Try
            End Using
        End Using
        Return dt

    End Function

End Class
