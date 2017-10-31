Imports System.Data.SqlClient
Imports CoastalPortal.AirTaxi
Imports CoastalPortal.Models

'Imports System.Xml
'Imports System.Data
'Imports System.IO
'Imports System.Drawing
'Imports System.Drawing.Imaging

Public Class DataAccess

    '20120427 - pab - add logging
    '20140224 - pab - add threading
    Shared Function Insert_sys_log(ByVal CarrierID As Integer, ByVal UserID As String, ByVal Message As String,
            ByVal CallingParty As String, ByVal IP As String) As Integer

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            '20120501 - pab - keep hardcoded connectionstrings in one location
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oConn.Open()
            oCmd = New SqlCommand("sp_Insertsys_log", oConn)
            oCmd.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@UserID", SqlDbType.VarChar, 60)
            oParam.Value = UserID
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@Message", SqlDbType.VarChar, 500)
            oParam.Value = Message
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@CallingParty", SqlDbType.VarChar, 500)
            oParam.Value = CallingParty
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@IP", SqlDbType.VarChar, 60)
            oParam.Value = IP
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.Direction = ParameterDirection.ReturnValue
            oCmd.Parameters.Add(oParam)

            oCmd.ExecuteNonQuery()

            Return CInt(oParam.Value)

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb Insertsys_log Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb Insert_sys_log Error", s, _carrierid)
            Return 0
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oCmd.Dispose()
            oConn.Dispose()
        End Try

    End Function

    Shared Function GetPinnedFlights(ByRef req As String) As DataTable
        Dim db As New OptimizerContext
        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("TEST")
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = req

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
        End Try

        Return dt
    End Function


    '20120807 - pab - write to email queue
    '20140224 - pab - add threading
    Shared Function Insert_Email_Queue(ByVal CarrierID As Integer, ByVal EmailFrom As String, ByVal EmailTo As String,
             ByVal EmailCC As String, ByVal EmailBCC As String, ByVal EmailSubject As String, ByVal EmailBody As String,
             ByVal IsBodyHtml As Boolean, ByVal Attachment As String, ByVal CompanyLogo As String, ByVal AircraftLogo As String,
             ByVal ShowCarrier As Boolean) As Integer

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            '20120830 - pab - fix connection string error
            'oConn = New SqlConnection(ConnectionStringHelper.GetConnectionString)
            '20130702 - pab - remove duplicate connection strings
            'oConn = New SqlConnection(ConnectionStringHelper.GetD2DConnectionString)
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oConn.Open()
            oCmd = New SqlCommand("sp_InsertEmailQueue", oConn)
            oCmd.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@EmailFrom", SqlDbType.VarChar, 60)
            oParam.Value = EmailFrom
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@EmailTo", SqlDbType.VarChar, 500)
            oParam.Value = EmailTo
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@EmailCC", SqlDbType.VarChar, 500)
            oParam.Value = EmailCC
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@EmailBCC", SqlDbType.VarChar, 500)
            oParam.Value = EmailBCC
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@EmailSubject", SqlDbType.VarChar, 500)
            oParam.Value = EmailSubject
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@EmailBody", SqlDbType.VarChar)
            oParam.Value = EmailBody
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@IsBodyHtml", SqlDbType.Bit)
            oParam.Value = IsBodyHtml.GetHashCode
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@Attachment", SqlDbType.VarChar, 500)
            oParam.Value = Attachment
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@CompanyLogo", SqlDbType.VarChar, 200)
            oParam.Value = CompanyLogo
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@AircraftLogo", SqlDbType.VarChar, 200)
            oParam.Value = AircraftLogo
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@ShowCarrier", SqlDbType.Bit)
            oParam.Value = ShowCarrier.GetHashCode
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.Direction = ParameterDirection.ReturnValue
            oCmd.Parameters.Add(oParam)

            oCmd.ExecuteNonQuery()

            Return CInt(oParam.Value)

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "Insert_Email_Queue", "DataAccess.vb")
            Return 0
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oCmd.Dispose()
            oConn.Dispose()
        End Try

    End Function

    '20101105 - pab - add code for aliases
    Public Function GetProviderByAlias(ByVal urlalias As String) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oAdp = New SqlDataAdapter("sp_GetProviderByAlias", oConn)
            dt = New DataTable
            oParam = Nothing

            oParam = New SqlParameter("@alias", SqlDbType.VarChar, 50)
            oParam.Value = urlalias
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oAdp.Fill(dt)

            Return dt
        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetProviderByAlias", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetProviderByAlias Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetProviderByAlias Error", s, _carrierid)
            Return Nothing
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            dt.Dispose()
            oAdp.Dispose()
            oConn.Dispose()
        End Try
    End Function

    '20081209 - pab - change book flight screen to match d2d
    Public Function UpdateUserLastActivity(ByRef CarrierID As Integer, ByRef userID As Integer, ByRef lastActivityURL As String, ByRef ipAddress As String) As Boolean

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oConn.Open()
            oCmd = New SqlCommand("sp_UpdateUserLastActivity", oConn)

            oCmd.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@UserID", SqlDbType.Int)
            oParam.Value = userID
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@LastActivityURL", SqlDbType.VarChar, 500)
            oParam.Value = lastActivityURL
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@IpAddress", SqlDbType.VarChar, 20)
            oParam.Value = ipAddress
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.Direction = ParameterDirection.ReturnValue
            oCmd.Parameters.Add(oParam)

            oCmd.ExecuteNonQuery()

            Return Convert.ToBoolean(oParam.Value)
        Catch ex As Exception
            Return False
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            '20120705 - pab - fix error - Object reference not set to an instance of an object
            If Not IsNothing(oCmd) Then oCmd.Dispose()
            oConn.Dispose()
        End Try
    End Function

    Public Function GetAircraftTypeServiceSpecsByID(ByRef CarrierID As Integer, ByRef aircraftTypeServiceSpecID As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            '20120210 - pab - add aircraftpremiums
            'oAdp = New SqlDataAdapter("sp_GetAircraftTypeServiceSpecsByIDRunway", oConn)
            oAdp = New SqlDataAdapter("sp_GetAircraftTypeServiceSpecsByIDRunway2", oConn)
            dt = New DataTable
            oParam = Nothing

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@AircraftTypeServiceSpecID", SqlDbType.Int)
            oParam.Value = aircraftTypeServiceSpecID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.Fill(dt)

            Return dt
        Catch ex As Exception
            Return Nothing
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            dt.Dispose()
            oAdp.Dispose()
            oConn.Dispose()
        End Try
    End Function

    '20090120 - pab - add additional fees
    Public Function GetSetting(ByRef CarrierID As Integer, ByRef setting As String) As String

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oConn.Open()
            oCmd = New SqlCommand("sp_GetSysSetting", oConn)

            oCmd.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@setting", SqlDbType.VarChar, 100)
            oParam.Value = setting
            oCmd.Parameters.Add(oParam)

            Return CStr(oCmd.ExecuteScalar)

        Catch ex As Exception
            Dim s As String = "parms - CarrierID " & CarrierID & "; setting " & setting & vbCr & vbLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetSetting", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetSetting Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetSetting Error", s, _carrierid)
            Return String.Empty

        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oCmd.Dispose()
            oConn.Dispose()
        End Try

    End Function

    '20171027 - pab - calendar
    Shared Function getsettingnumeric(ByRef CarrierID As Integer, ByRef setting As String) As Double

        Dim s As String = String.Empty

        getsettingnumeric = 0

        Try
            Dim da As New DataAccess
            s = da.GetSetting(CarrierID, setting)
            If IsNumeric(s) Then
                Return CDbl(s)
            End If

        Catch ex As Exception
            Return 0

        End Try

    End Function

    '20171027 - pab - calendar
    Shared Function GetDSTbyDate(ByRef fromdate As Date) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                '20140523 - pab - change to dynamic optimizer database location
                'conn.ConnectionString = ConnectionStringHelper.GetConnectionStringMKAzure()
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OPTIMIZERSERVER")
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_GetDSTbyDate"

                    Dim sqlParam As SqlParameter

                    conn.Open()

                    sqlParam = New SqlParameter("@fromdate", SqlDbType.Date)
                    sqlParam.Value = fromdate
                    cmd.Parameters.Add(sqlParam)

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetDSTbyDate", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetDSTbyDate Error", s, _carrierid)
        End Try

        Return dt

    End Function

    '20171027 - pab - calendar
    Public Function GetValues(ByRef CarrierID As Integer, ByRef controlname As String) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetValues", oConn)
            oParam = Nothing

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@controlname", SqlDbType.VarChar, 100)
            oParam.Value = controlname
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oAdp.Fill(dt)

            Return dt
        Catch ex As Exception
            Return Nothing
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            dt.Dispose()
            oAdp.Dispose()
            oConn.Dispose()
        End Try
    End Function

    '20171027 - pab - calendar
    Shared Function GetCASFlightsACType(ByRef CarrierID As Integer, ByRef ModelRunID As String) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OPTIMIZERSERVER")
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_GetCASFlightsACType"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                    sqlParam.Value = CarrierID
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@ModelRunID", SqlDbType.NVarChar)
                    sqlParam.Value = ModelRunID
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetCASFlightsACType Error", s, _carrierid)
            Insertsys_log(_carrierid, appName, s, "GetCASFlightsACType", "DataAccess.vb")
        End Try

        Return dt

    End Function

    '20171027 - pab - calendar
    Shared Function GetFOSFlightsBestModels(ByRef CarrierID As Integer, ByRef includeR0 As Boolean) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                '20170924 - pab - show dev models
                If usedevdb = False Then
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OPTIMIZERSERVER")
                Else
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OptimizerServerTest")
                End If

                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    '20170723 - pab - include r0 models 
                    'cmd.CommandText = "sp_GetFOSFlightsBestModels"
                    cmd.CommandText = "sp_GetFOSFlightsBestModelsR0"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                    sqlParam.Value = CarrierID
                    cmd.Parameters.Add(sqlParam)

                    '20170723 - pab - include r0 models 
                    sqlParam = New SqlParameter("@includeR0", SqlDbType.Bit)
                    sqlParam.Value = includeR0.GetHashCode
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetFOSFlightsBestModels Error", s, _carrierid)
            Insertsys_log(_carrierid, appName, s, "GetFOSFlightsBestModels", "DataAccess.vb")
        End Try

        Return dt

    End Function

    '20171027 - pab - calendar
    Public Function GetFOSFlightsBestModelRunID(ByVal CarrierID As Integer) As String

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            '20170924 - pab - show dev models
            If usedevdb = False Then
                oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring("OPTIMIZERSERVER"))
            Else
                oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring("OptimizerServerTest"))
            End If
            oConn.Open()
            oCmd = New SqlCommand("sp_GetFOSFlightsBestModelRunID", oConn)

            oCmd.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oCmd.Parameters.Add(oParam)

            Return CStr(oCmd.ExecuteScalar)

        Catch ex As Exception
            Return String.Empty

        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oCmd.Dispose()
            oConn.Dispose()
        End Try

    End Function

    '20171027 - pab - calendar
    Shared Function GetFOSFlightsCalendarByCarrierIDDateOffset(ByRef CarrierID As Integer, ByRef ModelRunID As String, ByRef StartDate As Date,
            ByRef offset As Integer) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                '20140523 - pab - change to dynamic optimizer database location
                'conn.ConnectionString = ConnectionStringHelper.GetConnectionStringMKAzure()
                '20170924 - pab - show dev models
                If usedevdb = False Then
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OPTIMIZERSERVER")
                Else
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OptimizerServerTest")
                End If
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    '20161221 - pab - fix calendar
                    'cmd.CommandText = "sp_GetFOSFlightsCalendarByCarrierIDDateOffset"
                    cmd.CommandText = "sp_GetCASFlightsCalendarByCarrierIDDateOffset2"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                    sqlParam.Value = CarrierID
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@ModelRunID", SqlDbType.VarChar)
                    sqlParam.Value = ModelRunID
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@StartDate", SqlDbType.Date)
                    sqlParam.Value = StartDate
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@offset", SqlDbType.Int)
                    sqlParam.Value = offset
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetFOSFlightsCalendarByCarrierIDDateOffset", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetFOSFlightsCalendarByCarrierIDDateOffset Error", s, _carrierid)
        End Try

        Return dt

    End Function

    '20171027 - pab - calendar
    Shared Function GetFOSFlightsCalendarCrewDate(ByRef CarrierID As Integer, ByRef ModelRunID As String, ByRef StartDate As Date) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                '20140523 - pab - change to dynamic optimizer database location
                'conn.ConnectionString = ConnectionStringHelper.GetConnectionStringMKAzure()
                '20170924 - pab - show dev models
                If usedevdb = False Then
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OPTIMIZERSERVER")
                Else
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OptimizerServerTest")
                End If
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    'cmd.CommandText = "sp_GetFOSFlightsCalendarCrewDate"
                    cmd.CommandText = "sp_GetFOSFlightsCalendarCrewDatetest"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                    sqlParam.Value = CarrierID
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@ModelRunID", SqlDbType.VarChar)
                    sqlParam.Value = ModelRunID
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@StartDate", SqlDbType.Date)
                    sqlParam.Value = StartDate
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetFOSFlightsCalendarCrewDate", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetFOSFlightsCalendarCrewDate Error", s, _carrierid)
        End Try

        Return dt

    End Function

    '20171027 - pab - calendar
    Public Function GetAircraftTypeServiceSpecsByIDProd(ByRef CarrierID As Integer, ByRef aircraftTypeServiceSpecID As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            '20150921 - move to SDS SQL server
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(ProdPortalServer))
            oAdp = New SqlDataAdapter("sp_GetAircraftTypeServiceSpecsByIDRunway2", oConn)
            dt = New DataTable
            oParam = Nothing

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@AircraftTypeServiceSpecID", SqlDbType.Int)
            oParam.Value = aircraftTypeServiceSpecID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.Fill(dt)

            Return dt

        Catch ex As System.Data.Entity.Infrastructure.DbUpdateException
            Dim s As String = "DbUpdateException error - parms - CarrierID " & CarrierID & "; aircraftTypeServiceSpecID " &
                aircraftTypeServiceSpecID & vbCr & vbLf & vbCr & vbLf & ex.InnerException.InnerException.Message
            'If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            'If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetAircraftTypeServiceSpecsByIDProd", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetAircraftTypeServiceSpecsByIDProd Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetAircraftTypeServiceSpecsByIDProd DbUpdateException Error", s, _carrierid)
            Return Nothing

        Catch ex As System.Data.Entity.Validation.DbEntityValidationException
            Dim s As String = "DbEntityValidationException error -parms - CarrierID " & CarrierID & "; aircraftTypeServiceSpecID " &
                aircraftTypeServiceSpecID & vbCr & vbLf & vbCr & vbLf & ex.Message
            Insertsys_log(_carrierid, appName, s, "GetAircraftTypeServiceSpecsByIDProd", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetAircraftTypeServiceSpecsByIDProd DbEntityValidationException Error", s, _carrierid)
            Return Nothing

        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            dt.Dispose()
            oAdp.Dispose()
            oConn.Dispose()
        End Try

    End Function

    '20171027 - pab - calendar
    Public Function GetAircraftByRegistrationProd(ByRef CarrierID As Integer, ByRef Registration As String) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                '20150921 - move to SDS SQL server
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(ProdPortalServer)
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_GetAircraftByRegistration"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                    sqlParam.Value = CarrierID
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@Registration", SqlDbType.VarChar)
                    sqlParam.Value = Registration
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetAircraftByRegistrationProd", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetAircraftByRegistrationProd Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetAircraftByRegistrationProd Error", s, _carrierid)
        End Try

        Return dt

    End Function

    '20171027 - pab - calendar
    Shared Function GetFOSOptimizerRequestByID(ByRef CarrierID As Integer, ByRef ID As Integer) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                '20140523 - pab - change to dynamic optimizer database location
                'conn.ConnectionString = ConnectionStringHelper.GetConnectionStringMKAzure()
                '20170924 - pab - show dev models
                If usedevdb = False Then
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OPTIMIZERSERVER")
                Else
                    conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring("OptimizerServerTest")
                End If
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_GetFOSOptimizerRequestByID"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                    sqlParam.Value = CarrierID
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@ID", SqlDbType.Int)
                    sqlParam.Value = ID
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetFOSOptimizerRequestByID", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetFOSOptimizerRequestByID Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetFOSOptimizerRequestByID Error", s, _carrierid)
        End Try

        Return dt

    End Function

    '20171027 - pab - calendar
    Public Function GetICAOcodebyIATA(ByRef IATA As String) As String

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            '20130702 - pab - remove duplicate connection strings
            'oConn = New SqlConnection(ConnectionStringHelper.GetFAAConnectionString())
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oConn.Open()
            oCmd = New SqlCommand("sp_GetICAOcodebyIATA", oConn)

            oCmd.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@IATA", SqlDbType.VarChar, 255)
            oParam.Value = IATA
            oCmd.Parameters.Add(oParam)

            Return CStr(oCmd.ExecuteScalar)

        Catch ex As Exception
            Return String.Empty
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oCmd.Dispose()
            oConn.Dispose()
        End Try
    End Function

    '20171027 - pab - calendar
    Public Function GetFlightDetails(ByRef CarrierID As Integer, ByRef flightID As Integer) As DataSet

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim ds As DataSet = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            '20120308 - pab - per seat changes
            oAdp = New SqlDataAdapter("sp_GetFlightDetails", oConn)
            'oAdp = New SqlDataAdapter("sp_GetFlightDetails2", oConn)

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@FlightID", SqlDbType.Int)
            oParam.Value = flightID
            oAdp.SelectCommand.Parameters.Add(oParam)

            ds = New DataSet
            oAdp.Fill(ds)

            Return ds

        Catch ex As Exception
            Return Nothing
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oAdp.Dispose()
            oConn.Dispose()
        End Try
    End Function

    '20171027 - pab - calendar
    Public Function GetSysFlightRequestPAXInfo(ByRef CarrierID As Integer, ByRef FlightDetail As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As DataTable = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetSysFlightRequestPAXInfo", oConn)

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@FlightID", SqlDbType.Int)
            oParam.Value = FlightDetail
            oAdp.SelectCommand.Parameters.Add(oParam)

            dt = New DataTable
            oAdp.Fill(dt)

            Return dt

        Catch ex As Exception
            Return Nothing
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oAdp.Dispose()
            oConn.Dispose()
        End Try
    End Function

End Class
