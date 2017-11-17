Option Explicit On
Option Strict On

Imports System.Data
Imports System.Data.SqlClient
Imports CoastalPortal.AirTaxi

Namespace AircraftPerformance
    Public Class FuelSpeedByHour

#Region "Data Access Methods"
        Public Shared Function [Get](ByVal CarrierID As Integer, ByVal aircraftTypeServiceSpecID As Integer) As DataTable

            Dim dt As DataTable = New DataTable()

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_GetAircraftPerformanceFuelByHour_v2"

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

        Public Shared Sub Insert(ByVal CarrierID As Integer, ByVal aircraftTypeServiceSpecID As Integer, ByVal hour As Integer, _
                                                ByVal speed As Double, ByVal gallons As Double)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_InsertAircraftPerformanceFuelByHour"

                        Dim sqlParam As SqlParameter

                        sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                        sqlParam.Value = CarrierID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@AircraftTypeServiceSpecID", SqlDbType.Int)
                        sqlParam.Value = aircraftTypeServiceSpecID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Hour", SqlDbType.Int)
                        sqlParam.Value = hour
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Speed", SqlDbType.Float)
                        sqlParam.Value = speed
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Gallons", SqlDbType.Float)
                        sqlParam.Value = gallons
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
                                                ByVal hour As Integer, ByVal speed As Double, ByVal gallons As Double)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_UpdateAircraftPerformanceFuelByHour"

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

                        sqlParam = New SqlParameter("@Hour", SqlDbType.Int)
                        sqlParam.Value = hour
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Speed", SqlDbType.Float)
                        sqlParam.Value = speed
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@Gallons", SqlDbType.Float)
                        sqlParam.Value = gallons
                        cmd.Parameters.Add(sqlParam)

                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

            Catch ex As Exception
                Dim sa As String = ex.Message
            End Try

        End Sub

        Public Shared Sub Delete(ByVal CarrierID As Integer, ByVal id As Integer)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_DeleteAircraftPerformanceFuelByHour"

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
        '20100716 - bd - Checks a dataview and determines if the newHour value is unique and can be added to the set of data without conflict.
        'Returns true if the newHour value is unique.
        Public Shared Function IsHourUnique(ByVal newHour As Integer, ByVal dvExistingRecords As DataView) As Boolean
            dvExistingRecords.RowFilter = String.Empty 'Make sure row filter is clear.

            If dvExistingRecords.Count = 0 Then
                Return True 'No existing records so return valid.
            Else
                dvExistingRecords.RowFilter = "Hour = " & newHour.ToString()

                If dvExistingRecords.Count = 0 Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function


        '20100716 - bd - Checks a dataview againsts the newHour and newGPH variables in order to make sure that 
        'speed increases as time increases.
        'Returns true if the new values are valid.
        Public Shared Function IsSpeedVsHourValid(ByVal newHour As Integer, ByVal newSpeed As Double, ByVal dvExistingRecords As DataView) As Boolean
            dvExistingRecords.RowFilter = String.Empty 'Make sure row filter is clear.

            If dvExistingRecords.Count = 0 Then
                Return True 'No existing records so return valid.
            Else
                dvExistingRecords.RowFilter = "Hour < " & newHour.ToString() & " AND Speed >= " & newSpeed.ToString() & " OR Hour > " & newHour.ToString() & " AND Speed <= " & newSpeed.ToString()

                If dvExistingRecords.Count = 0 Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function

        '20100716 - bd - Checks a dataview againsts the newHour and newGPH variables in order to make sure that 
        'fuel decreases as time increases.
        'Returns true if the new values are valid.
        Public Shared Function IsFuelVsHourValid(ByVal newHour As Integer, ByVal newGPH As Double, ByVal dvExistingRecords As DataView) As Boolean
            dvExistingRecords.RowFilter = String.Empty 'Make sure row filter is clear.

            If dvExistingRecords.Count = 0 Then
                Return True 'No existing records so return valid.
            Else
                dvExistingRecords.RowFilter = "Hour < " & newHour.ToString() & " AND Gallons <= " & newGPH.ToString() & " OR Hour > " & newHour.ToString() & " AND Gallons >= " & newGPH.ToString()

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


