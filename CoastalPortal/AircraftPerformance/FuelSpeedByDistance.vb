Option Explicit On
Option Strict On

Imports System.Data
Imports System.Data.SqlClient
Imports CoastalPortal.AirTaxi

Namespace AircraftPerformance
    Public Class FuelSpeedByDistance

#Region "Data Access Methods"
        Public Shared Function [Get](ByVal CarrierID As Integer, ByVal aircraftTypeServiceSpecID As Integer) As DataTable

            Dim dt As DataTable = New DataTable()

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_GetAircraftPerformanceByDistance_v2"

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

        Public Shared Sub Insert(ByVal CarrierID As Integer, ByVal aircraftTypeServiceSpecID As Integer, ByVal rangeLow As Integer, _
                                                    ByVal rangeHigh As Integer, ByVal speed As Double, ByVal gallons As Double)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_InsertAircraftPerformanceByDistance"

                        Dim sqlParam As SqlParameter

                        sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                        sqlParam.Value = CarrierID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@AircraftTypeServiceSpecID", SqlDbType.Int)
                        sqlParam.Value = aircraftTypeServiceSpecID
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@RangeLow", SqlDbType.Int)
                        sqlParam.Value = rangeLow
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@RangeHigh", SqlDbType.Int)
                        sqlParam.Value = rangeHigh
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
                                                    ByVal rangeLow As Integer, ByVal rangeHigh As Integer, ByVal speed As Double, _
                                                    ByVal gallons As Double)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_UpdateAircraftPerformanceByDistance"

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

                        sqlParam = New SqlParameter("@RangeLow", SqlDbType.Int)
                        sqlParam.Value = rangeLow
                        cmd.Parameters.Add(sqlParam)

                        sqlParam = New SqlParameter("@RangeHigh", SqlDbType.Int)
                        sqlParam.Value = rangeHigh
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

        Public Shared Sub Delete(ByVal CarrierID As Integer, ByVal id As Integer)

            Try
                Using conn As New SqlConnection()
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                    Using cmd As New SqlCommand
                        cmd.Connection = conn
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_DeleteAircraftPerformanceByDistance"

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
        'fuel decreases as range increases but gallons entry is optional on this table – 0 entry ok but must be 0 for all entries (plane becomes more efficient as the plane climbs higher over distance); error messages – “Ranges cannot overlap”, “Speed increases over distance”, “Fuel burn decreases over distance”


        '20100716 - bd - Make sure that the low range is lower than the high range. Also, this method checks a dataview and determines if 
        'the new range overlaps any of the old ranges.
        'Returns true if both checks are successful.
        Public Shared Function IsRangeValid(ByVal newRangeLow As Integer, ByVal newRangeHigh As Integer, ByVal dvExistingRecords As DataView) As Boolean

            Dim flgIsRangeValid As Boolean = False

            If newRangeLow < newRangeHigh Then
                flgIsRangeValid = True
            Else
                flgIsRangeValid = False
            End If


            If flgIsRangeValid = True Then
                dvExistingRecords.RowFilter = String.Empty 'Make sure row filter is clear.

                If dvExistingRecords.Count = 0 Then
                    flgIsRangeValid = True 'No existing records so return valid.
                Else 'Determine if there are overlaps.
                    dvExistingRecords.RowFilter = "RangeLow <=" & newRangeHigh.ToString() & "AND RangeHigh >= " & newRangeLow.ToString()

                    If dvExistingRecords.Count = 0 Then 'No overlaps
                        flgIsRangeValid = True
                    Else
                        flgIsRangeValid = False 'Overlap found
                    End If
                End If
            End If


            Return flgIsRangeValid

        End Function


        '20100716 - bd - Checks a dataview and determines if the value of newSpeed increases as the value of newRangeHigh increases.
        'Returns true if valid.
        Public Shared Function IsSpeedVsRangeValid(ByVal newRangeHigh As Integer, ByVal newSpeed As Double, ByVal dvExistingRecords As DataView) As Boolean
            dvExistingRecords.RowFilter = String.Empty 'Make sure row filter is clear.

            If dvExistingRecords.Count = 0 Then
                Return True 'No existing records so return valid.
            Else
                dvExistingRecords.RowFilter = "RangeHigh < " & newRangeHigh.ToString() & " AND Speed >= " & newSpeed.ToString() & " OR RangeHigh > " & newRangeHigh.ToString() & " AND Speed <= " & newSpeed.ToString()

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


