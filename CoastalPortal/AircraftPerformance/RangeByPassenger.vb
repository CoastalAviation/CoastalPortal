Option Explicit On
Option Strict On

Imports System.Data
Imports System.Data.SqlClient
Imports CoastalPortal.AirTaxi

Namespace AircraftPerformance
    Public Class RangeByPassenger

#Region "Data Access Methods"
        Public Shared Function [Get](ByVal CarrierID As Integer, ByVal aircraftTypeServiceSpecID As Integer) As DataTable

            Dim dt As DataTable = New DataTable()

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_GetAircraftPerformanceRange_v2"

                        Dim sqlParam As SqlParameter

                        sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                        sqlParam.Value = CarrierID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@AircraftTypeServiceSpecID", SqlDbType.Int)
                        sqlParam.Value = aircraftTypeServiceSpecID
                        cmd.Parameters.Add(sqlParam)

                        conn.Open()

                        Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                            dt.Load(rdr)
                        End Using
                    End Using
                End Using

            Catch ex As Exception
                Return Nothing
            End Try

            Return dt

        End Function

        Public Shared Sub Insert(ByVal CarrierID As Integer, ByVal aircraftTypeServiceSpecID As Integer, ByVal passengers As Integer, ByVal range As Integer)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_InsertAircraftPerformanceRange"

                        Dim sqlParam As SqlParameter

                        sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                        sqlParam.Value = CarrierID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@AircraftTypeServiceSpecID", SqlDbType.Int)
                        sqlParam.Value = aircraftTypeServiceSpecID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Passengers", SqlDbType.Int)
                        sqlParam.Value = passengers
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Range", SqlDbType.Int)
                        sqlParam.Value = range
                        cmd.Parameters.Add(sqlParam)


                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

            Catch ex As Exception
                Dim s As String = ex.Message
            End Try

        End Sub

        Public Shared Sub Update(ByVal CarrierID As Integer, ByVal id As Integer, ByVal aircraftTypeServiceSpecID As Integer, _
                                                 ByVal passengers As Integer, ByVal range As Integer)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_UpdateAircraftPerformanceRange"

                        Dim sqlParam As SqlParameter

                        sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                        sqlParam.Value = CarrierID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@id", SqlDbType.Int)
                        sqlParam.Value = id
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@AircraftTypeServiceSpecID", SqlDbType.Int)
                        sqlParam.Value = aircraftTypeServiceSpecID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Passengers", SqlDbType.Int)
                        sqlParam.Value = passengers
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Range", SqlDbType.Int)
                        sqlParam.Value = range
                        cmd.Parameters.Add(sqlParam)


                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

            Catch ex As Exception
                Dim s As String = ex.Message
            End Try

        End Sub

        Public Shared Sub Delete(ByVal CarrierID As Integer, ByVal id As Integer)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_DeleteAircraftPerformanceRange"

                        Dim sqlParam As SqlParameter

                        sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                        sqlParam.Value = CarrierID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@id", SqlDbType.Int)
                        sqlParam.Value = id
                        cmd.Parameters.Add(sqlParam)

                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

            Catch ex As Exception
                Dim s As String = ex.Message
            End Try
        End Sub
#End Region

#Region "Business Rule Methods"
        '20100716 - bd - Checks a dataview and determines if the newPassengers value is unique and can be added to the set of data without conflict.
        'Returns true if the newPassengers value is unique.
        Public Shared Function ArePassengersUnique(ByVal newPassengers As Integer, ByVal dvExistingRecords As DataView) As Boolean
            dvExistingRecords.RowFilter = String.Empty 'Make sure row filter is clear.

            If dvExistingRecords.Count = 0 Then
                Return True 'No existing records so return valid.
            Else
                dvExistingRecords.RowFilter = "Passengers = " & newPassengers.ToString()

                If dvExistingRecords.Count = 0 Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function


        '20100716 - bd - Checks a dataview againsts the newRange and newPassengers variables in order to make sure that 
        'range decreases as passengers increase (plane is less efficient due to increased weight).
        'Returns true if the new values are valid.
        Public Shared Function IsRangeVsPassengersValid(ByVal newRange As Integer, ByVal newPassengers As Integer, ByVal dvExistingRecords As DataView) As Boolean
            dvExistingRecords.RowFilter = String.Empty 'Make sure row filter is clear.

            If dvExistingRecords.Count = 0 Then
                Return True 'No existing records so return valid.
            Else 'Passengers < 2 AND Range <= 1000 OR Passengers > 2 AND Range >= 1000
                dvExistingRecords.RowFilter = "Passengers < " & newPassengers.ToString() & " AND Range <= " & newRange.ToString() & " OR Passengers > " & newPassengers.ToString() & " AND Range >= " & newRange.ToString()

                If dvExistingRecords.Count = 0 Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function
#End Region

    End Class
End Namespace


