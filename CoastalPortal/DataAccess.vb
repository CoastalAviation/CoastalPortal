Imports System.Data.SqlClient
Imports CoastalPortal.AirTaxi
Imports CoastalPortal.Models

'Imports System.Xml
'Imports System.Data
Imports System.IO
'Imports System.Drawing
'Imports System.Drawing.Imaging

Public Class DataAccess

    '20160304 - pab - add broker info
    Function GetBrokersByCompany(ByVal companyname As String) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_GetBrokersByCompany"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@companyname", SqlDbType.NVarChar)
                    sqlParam.Value = companyname
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = "parms - companyname " & companyname & vbCr & vbLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, s, "GetBrokersByCompany", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                " DataAccess.vb GetBrokersByCompany Error", s, _carrierid)
        End Try

        Return dt

    End Function

    '20160304 - pab - add broker info
    Function GetBrokerCompanies() As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_GetBrokerCompanies"

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
            Insertsys_log(0, appName, s, "GetBrokerCompanies", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                " DataAccess.vb GetBrokerCompanies Error", s, 0)
        End Try

        Return dt

    End Function

    '20160622 - pab - check for overlapping flights
    Public Shared Function GetFuelSpeedByDistance(ByRef CarrierID As Integer, ByRef aircraftTypeServiceSpecID As Integer, ByRef cn As String) As DataTable

        Dim dt As DataTable = New DataTable()

        '20111222 - pab - fix cn error
        If InStr(cn.ToUpper, "PROVIDER=MSDASQL") > 0 Or cn = "" Then
            '20120501 - pab - keep hardcoded connectionstrings in one location
            cn = ConnectionStringHelper.getglobalconnectionstring(ProdPortalServer)
        End If

        Try
            Using conn As New SqlConnection()
                conn.ConnectionString = cn
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

    '20150403 - pab - add regional discounts/surcharges
    Public Function GetRoundEarthDistanceBetweenLocations(ByRef orig As String, ByRef dest As String) As Double

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oConn.Open()
            oCmd = New SqlCommand("sp_GetRoundEarthDistanceBetweenLocations", oConn)

            oCmd.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@sourcelocationid", SqlDbType.NVarChar)
            oParam.Value = orig
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@destinationlocationid", SqlDbType.NVarChar)
            oParam.Value = dest
            oCmd.Parameters.Add(oParam)

            Return CDbl(oCmd.ExecuteScalar)

        Catch ex As Exception
            Return 0
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oCmd.Dispose()
            oConn.Dispose()
        End Try
    End Function

    '20160426 - pab - make like door2door
    Public Function CheckQuoteParms(ByVal orig As String, ByVal dest As String, ByVal pax As Integer, ByVal WeightClass As String, ByVal options As String,
                                    ByVal ratings As String, ByRef ManufactureDate As Date) As DataSet

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim ds As DataSet = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_CheckQuoteParms", oConn)

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@orig", SqlDbType.VarChar)
            oParam.Value = orig
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@dest", SqlDbType.VarChar)
            oParam.Value = dest
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@pax", SqlDbType.Int)
            oParam.Value = pax
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@WeightClass", SqlDbType.VarChar)
            oParam.Value = WeightClass
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@options", SqlDbType.VarChar)
            oParam.Value = options
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@ratings", SqlDbType.VarChar)
            oParam.Value = ratings
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@ManufactureDate", SqlDbType.Date)
            oParam.Value = ManufactureDate
            oAdp.SelectCommand.Parameters.Add(oParam)

            ds = New DataSet
            oAdp.Fill(ds)

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(0, appName, s, "CheckQuoteParms error", "DataAccess.vb")
        End Try

        Return ds

    End Function

    '20160426 - pab - make like door2door
    Public Function InsertAirBoardClickThru(
        ByVal CarrierID As Integer,
        ByVal ExternalQuote As String,
        ByVal ExternalQuote2 As String,
        ByVal QuoteNumber As Integer,
        ByVal ClickDate As Date,
        ByVal FlightRequestID As Integer,
        ByVal ExternalPrice As Double) As Integer

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oConn.Open()

            oCmd = New SqlCommand("sp_InsertAirBoardClickThru", oConn)
            oCmd.CommandType = CommandType.StoredProcedure

            oParam = Nothing

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@ExternalQuote", SqlDbType.VarChar, 10)
            oParam.Value = ExternalQuote
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@ExternalQuote2", SqlDbType.VarChar, 10)
            oParam.Value = ExternalQuote2
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@QuoteNumber", SqlDbType.Int)
            oParam.Value = QuoteNumber
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@ClickDate", SqlDbType.DateTime)
            oParam.Value = ClickDate
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@FlightRequestID", SqlDbType.Int)
            oParam.Value = FlightRequestID
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@ExternalPrice", SqlDbType.Money)
            oParam.Value = ExternalPrice
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.Direction = ParameterDirection.ReturnValue
            oCmd.Parameters.Add(oParam)

            oCmd.ExecuteNonQuery()

            Return CInt(oParam.Value)
        Catch ex As Exception
            Return 0
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oCmd.Dispose()
            oConn.Dispose()
        End Try

    End Function

    '20140620 - pab - quote from admin portal
    Public Function GetProviderByCompanyName(ByVal CompanyName As String) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oAdp = New SqlDataAdapter("sp_GetProviderByCompanyName", oConn)
            dt = New DataTable
            oParam = Nothing

            oParam = New SqlParameter("@CompanyName", SqlDbType.VarChar, 50)
            oParam.Value = CompanyName
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oAdp.Fill(dt)

            Return dt
        Catch ex As Exception
            Dim serr As String = "parms - CompanyName " & CompanyName & vbCr & vbLf & ex.Message
            If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(0, AirTaxi.appName, serr, "GetProviderByCompanyName", "DataAccess.vb")
            AirTaxi.SendEmail(0, "info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName &
                " DataAccess.vb GetProviderByCompanyName Error", serr)
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

    '20160603 - pab - add code to do additional lookup if no quotes returned by web service
    '20171025 - pab - fix dbnull conversion error
    Function GetD2DQuoteQueuebyRequestParms(ByRef CarrierID As Integer, ByRef triptype As String, ByRef ip As String, ByRef Origin As String,
            ByRef Destination As String, ByRef quotedate As Date) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    'cmd.CommandText = "sp_GetD2DQuoteQueuebyRequestParms"
                    '20171025 - pab - fix dbnull conversion error
                    cmd.CommandText = "sp_GetD2DQuoteQueuebyRequestParms2"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@CarrierID", SqlDbType.Int)
                    sqlParam.Value = CarrierID
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@triptype", SqlDbType.NVarChar)
                    sqlParam.Value = triptype
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@ip", SqlDbType.NVarChar)
                    sqlParam.Value = ip
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@Origin", SqlDbType.NVarChar)
                    sqlParam.Value = Origin
                    cmd.Parameters.Add(sqlParam)

                    sqlParam = New SqlParameter("@Destination", SqlDbType.NVarChar)
                    sqlParam.Value = Destination
                    cmd.Parameters.Add(sqlParam)

                    '20171025 - pab - fix dbnull conversion error
                    sqlParam = New SqlParameter("@quotedate", SqlDbType.DateTime)
                    sqlParam.Value = quotedate
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Dim s As String = "parms - CarrierID " & CarrierID & "; triptype " & triptype & vbCr & vbLf & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(CarrierID, appName, s, "GetD2DQuoteQueuebyRequestParms", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                " DataAccess.vb GetD2DQuoteQueuebyRequestParms Error", s, CarrierID)
        End Try

        Return dt

    End Function

    '20160426 - pab - make like door2door
    Public Function GetAircraftTypeServiceSpecsByWeightClass(ByVal CarrierID As Integer, ByVal WeightClass As String) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetAircraftTypeServiceSpecsByWeightClassRunway2", oConn)
            dt = New DataTable
            oParam = Nothing

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@WeightClass", SqlDbType.Char)
            oParam.Value = WeightClass
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

    '20140418 - pab - add quote test
    Public Function GetD2DQuoteQueuebyworknumber(ByVal worknumber As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oAdp = New SqlDataAdapter("sp_getD2DQuoteQueuebyWorkNumber", oConn)
            dt = New DataTable
            oParam = Nothing

            oParam = New SqlParameter("@worknumber", SqlDbType.Int)
            oParam.Value = worknumber
            oAdp.SelectCommand.Parameters.Add(oParam)


            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oAdp.Fill(dt)

            Return dt
        Catch ex As Exception
            Dim serr As String = "parms - worknumber " & worknumber & vbCr & vbLf & ex.Message
            If Not IsNothing(ex.InnerException) Then serr &= "; " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then serr &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(0, AirTaxi.appName, serr, "GetD2DQuoteQueuebyworknumber", "DataAccess.vb")
            AirTaxi.SendEmail(0, "info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName &
                " DataAccess.vb GetD2DQuoteQueuebyworknumber Error", serr)

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

    '20160411 - pab - show quote detail
    Public Function GetD2DQuoteQueue(ByVal id As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oAdp = New SqlDataAdapter("sp_getD2DQuoteQueue", oConn)
            dt = New DataTable
            oParam = Nothing

            oParam = New SqlParameter("@id", SqlDbType.Int)
            oParam.Value = id
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb Insert_sys_log Error", s, CarrierID)
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(CarrierID, appName, s, "Insert_Email_Queue", "DataAccess.vb")
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(0, appName, s, "GetProviderByAlias", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetProviderByAlias Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetProviderByAlias Error", s, 0)
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(CarrierID, appName, s, "GetSetting", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetSetting Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetSetting Error", s, CarrierID)
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(0, appName, s, "GetDSTbyDate", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetDSTbyDate Error", s, 0)
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetCASFlightsACType Error", s, CarrierID)
            Insertsys_log(CarrierID, appName, s, "GetCASFlightsACType", "DataAccess.vb")
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetFOSFlightsBestModels Error", s, CarrierID)
            Insertsys_log(CarrierID, appName, s, "GetFOSFlightsBestModels", "DataAccess.vb")
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(CarrierID, appName, s, "GetFOSFlightsCalendarByCarrierIDDateOffset", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetFOSFlightsCalendarByCarrierIDDateOffset Error", s, CarrierID)
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(CarrierID, appName, s, "GetFOSFlightsCalendarCrewDate", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetFOSFlightsCalendarCrewDate Error", s, CarrierID)
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(CarrierID, appName, s, "GetAircraftTypeServiceSpecsByIDProd", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetAircraftTypeServiceSpecsByIDProd Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetAircraftTypeServiceSpecsByIDProd DbUpdateException Error", s, CarrierID)
            Return Nothing

        Catch ex As System.Data.Entity.Validation.DbEntityValidationException
            Dim s As String = "DbEntityValidationException error -parms - CarrierID " & CarrierID & "; aircraftTypeServiceSpecID " &
                aircraftTypeServiceSpecID & vbCr & vbLf & vbCr & vbLf & ex.Message
            Insertsys_log(CarrierID, appName, s, "GetAircraftTypeServiceSpecsByIDProd", "DataAccess.vb")
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetAircraftTypeServiceSpecsByIDProd DbEntityValidationException Error", s, CarrierID)
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(CarrierID, appName, s, "GetAircraftByRegistrationProd", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetAircraftByRegistrationProd Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetAircraftByRegistrationProd Error", s, CarrierID)
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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(CarrierID, appName, s, "GetFOSOptimizerRequestByID", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetFOSOptimizerRequestByID Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " DataAccess.vb GetFOSOptimizerRequestByID Error", s, CarrierID)
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

    '20121210 - pab - add fos flights calendar
    Public Function GetAirportInformationByICAO(ByRef icao As String) As DataTable

        Dim dt As DataTable = New DataTable()

        Try
            Using conn As New SqlConnection()
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_GetAirportInformationByICAO"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@icao", SqlDbType.VarChar)
                    sqlParam.Value = icao
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
            Insertsys_log(0, appName, s, "GetAirportInformationByICAO", "DataAccess.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " DataAccess.vb GetAirportInformationByICAO Error", s, _carrierid)
            '20131024 - pab - fix duplicate emails
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " DataAccess.vb GetAirportInformationByICAO Error", s, 0)
        End Try

        Return dt

    End Function

    '20160426 - pab - make like door2door
    Public Function GetZIPCodes(ByVal ZIPCode As String) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oAdp = New SqlDataAdapter("sp_GetZIPCodes", oConn)
            dt = New DataTable
            oParam = Nothing

            oParam = New SqlParameter("@ZIPCode", SqlDbType.NVarChar)
            oParam.Value = ZIPCode
            oAdp.SelectCommand.Parameters.Add(oParam)


            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oAdp.Fill(dt)

            Return dt

        Catch ex As Exception
            Dim s As String = "parms - ZIPCode " & ZIPCode & vbNewLine & vbNewLine & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            AirTaxi.Insertsys_log(0, appName, s, "GetZIPCodes", "DataAccess.vb")
            AirTaxi.SendEmail(_emailfrom, "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & " DataAccess.vb GetZIPCodes Error",
                s, 0)
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

    '20160426 - pab - make like door2door
    Public Function InsertLocationLookupFail(ByVal Location As String) As Integer

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oConn.Open()

            oCmd = New SqlCommand("sp_InsertLocationLookupFail", oConn)
            oCmd.CommandType = CommandType.StoredProcedure

            oParam = Nothing

            oParam = New SqlParameter("@Location", SqlDbType.VarChar, 50)
            oParam.Value = Location
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.Direction = ParameterDirection.ReturnValue
            oCmd.Parameters.Add(oParam)

            oCmd.ExecuteNonQuery()

            Return CInt(oParam.Value)
        Catch ex As Exception
            Return 0
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            oCmd.Dispose()
            oConn.Dispose()
        End Try

    End Function

    '20160426 - pab - make like door2door
    Public Function GetLocationLookupFail(ByVal Location As String) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))

            oAdp = New SqlDataAdapter("sp_GetLocationLookupFail", oConn)
            dt = New DataTable
            oParam = Nothing

            oParam = New SqlParameter("@Location", SqlDbType.NVarChar)
            oParam.Value = Location
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

    '20160426 - pab - make like door2door
    Public Function GetMajorAirportsByLatitudeLongitude(
        ByVal latitude As Double,
        ByVal longitude As Double,
        ByVal minimumRunwayLength As Integer,
        ByVal miles As Integer,
        ByVal count As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetMajorAirportsByLatitudeLongitudeWithinDistance", oConn)
            dt = New DataTable
            oParam = Nothing

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@Latitude", SqlDbType.Float)
            oParam.Value = latitude
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@Longitude", SqlDbType.Float)
            oParam.Value = longitude
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@MinimumRunwayLength", SqlDbType.Int)
            oParam.Value = minimumRunwayLength
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@Miles", SqlDbType.Int)
            oParam.Value = miles
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@Count", SqlDbType.Int)
            oParam.Value = count
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

    Public Function GetAirportInformationByAirportCode(ByRef airportCode As String) As DataSet

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim ds As New DataSet

        Try
            '20130702 - pab - remove duplicate connection strings
            'oConn = New SqlConnection(ConnectionStringHelper.GetFAAConnectionString())
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetAirportInformationByLocationID_2_azure", oConn)
            ds = New DataSet
            oParam = Nothing

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@LocationID", SqlDbType.VarChar, 255)
            oParam.Value = airportCode
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.Fill(ds)

            Return ds
        Catch ex As Exception
            Return Nothing
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            ds.Dispose()
            oAdp.Dispose()
            oConn.Dispose()
        End Try
    End Function

    Public Function GetNearestAirportsByLatitudeLongitudeWithinDistance(
        ByRef latitude As Double,
        ByRef longitude As Double,
        ByRef minimumRunwayLength As Integer,
        ByRef miles As Integer,
        ByRef count As Integer) As DataSet

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim ds As New DataSet

        Try
            '20120830 - pab - fix connection string error; fix error - Column 'icao' does not belong to table Table
            'oConn = New SqlConnection(ConnectionStringHelper.GetConnectionString())
            'oAdp = New SqlDataAdapter("sp_GetNearestAirportsByLatitudeLongitudeWithinDistance", oConn)
            '20130702 - pab - remove duplicate connection strings
            'oConn = New SqlConnection(ConnectionStringHelper.GetD2DConnectionString)
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetNearestAirportsByLatitudeLongitudeWithinDistanceicao", oConn)
            ds = New DataSet
            oParam = Nothing

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@Latitude", SqlDbType.Float)
            oParam.Value = latitude
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@Longitude", SqlDbType.Float)
            oParam.Value = longitude
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@MinimumRunwayLength", SqlDbType.Int)
            oParam.Value = minimumRunwayLength
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@Miles", SqlDbType.Int)
            oParam.Value = miles
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@Count", SqlDbType.Int)
            oParam.Value = count
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.Fill(ds)

            Return ds
        Catch ex As Exception
            Return Nothing
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            ds.Dispose()
            oAdp.Dispose()
            oConn.Dispose()
        End Try
    End Function

    Public Function GetIATAcodebyICAO(ByVal ICAO As String) As DataSet

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim ds As New DataSet

        Try
            '20130702 - pab - remove duplicate connection strings
            'oConn = New SqlConnection(ConnectionStringHelper.GetFAAConnectionString())
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetIATAcodebyICAO", oConn)
            ds = New DataSet
            oParam = Nothing

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@ICAO", SqlDbType.VarChar, 255)
            oParam.Value = ICAO
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.Fill(ds)

            Return ds
        Catch ex As Exception
            Return Nothing
        Finally
            If oConn.State = ConnectionState.Open Then
                oConn.Close()
            End If
            ds.Dispose()
            oAdp.Dispose()
            oConn.Dispose()
        End Try
    End Function

    Public Function GetAirportInformationByLocationID(ByVal LocationID As String) As DataTable

        Dim dt As New DataTable

        Try
            Using conn As New SqlConnection()
                conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
                Using cmd As New SqlCommand
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_GetAirportInformationByLocationID"

                    Dim sqlParam As SqlParameter

                    sqlParam = New SqlParameter("@LocationID", SqlDbType.VarChar)
                    sqlParam.Value = LocationID
                    cmd.Parameters.Add(sqlParam)

                    conn.Open()

                    Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                        dt.Load(rdr)
                    End Using

                End Using
            End Using

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then
                s &= " - " & ex.InnerException.ToString
            End If
            If Not IsNothing(ex.StackTrace) Then
                s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            End If
            Insertsys_log(0, appName, s, "GetAirportInformationByLocationID", "DataAccess.vb")

        End Try

        Return dt

    End Function

    '20100323 - pab - add airport pax fees
    Public Function GetAirportStateByLocationID(ByRef airportID As String) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetAirportStateByLocationID", oConn)
            dt = New DataTable
            oParam = Nothing

            oParam = New SqlParameter("@LocationID", SqlDbType.Char)
            oParam.Value = airportID
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

    Public Function GetServiceAirports(ByRef CarrierID As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetServiceAirports", oConn)
            dt = New DataTable
            oParam = Nothing

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
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

    '20101214 - pab - record quote by provider
    Shared Function recordquotexml(ByVal dt As DataTable, ByVal origairport As String, ByVal DestAirport As String, ByVal quote As Double, ByVal outboundreturn As String,
                                   ByVal price As Double, ByVal PriceExplanation As String, ByVal portal As String, ByVal email As String, ByVal flightdate As String,
                                   ByVal provider As String, ByVal ip As String, ByVal CarrierID As Integer) As Integer

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        If email.Trim = "System.Web.UI.HtmlControls.HtmlInputText" Then
            email = "invalid email"
        End If

        Try
            Dim sw As New StringWriter
            Dim s As String
            dt.WriteXml(sw, XmlWriteMode.WriteSchema)
            s = sw.ToString

            'oConn = New SqlConnection(ConnectionStringHelper.GetProviderConnectionString(provider))
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oConn.Open()

            oCmd = New SqlCommand("sp_InsertSYS_Quotes_DT", oConn)
            oCmd.CommandType = CommandType.StoredProcedure

            oParam = Nothing

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@ModifiedBy", SqlDbType.VarChar, 50)
            oParam.Value = email
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@Quotexml", SqlDbType.Xml)
            oParam.Value = sw.ToString
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter()
            oParam.Direction = ParameterDirection.ReturnValue
            oCmd.Parameters.Add(oParam)

            oCmd.ExecuteNonQuery()

            Return CInt(oParam.Value)
        Catch ex As Exception
            Return 0

        Finally
            '20120125 - pab - fix error - Object reference not set to an instance of an object
            If Not oConn Is Nothing Then
                If oConn.State = ConnectionState.Open Then
                    oConn.Close()
                End If
                oCmd.Dispose()
                oConn.Dispose()
            End If

        End Try

    End Function

    '20120807 - pab - write to email queue
    Public Function GetSysSettingByProvider(ByVal CarrierID As Integer, ByVal setting As String, ByVal provider As String) As String

        Dim dt As DataTable = GetAliases()

        Dim oConn As SqlConnection = Nothing
        Dim oCmd As SqlCommand = Nothing
        Dim oParam As SqlParameter = Nothing

        Dim providerid As Integer = 0

        If dt.Rows.Count > 0 Then
            For n As Integer = 0 To dt.Rows.Count - 1
                '20110228 - pab - fix terms and conditions so that they are carrier specific
                'If dt.Rows(n).Item("companyname").ToString = provider Then
                If dt.Rows(n).Item("companyname").ToString.Trim = provider Or dt.Rows(n).Item("alias").ToString.Trim = provider Then
                    oConn = New SqlConnection(dt.Rows(n).Item("connectstring").ToString)
                    providerid = CInt(dt.Rows(n).Item("carrierid").ToString)
                    Exit For
                End If
            Next
        End If

        If oConn Is Nothing Then
            Return ""
        End If

        Try
            oConn.Open()
            oCmd = New SqlCommand("sp_GetSysSetting", oConn)

            oCmd.CommandType = CommandType.StoredProcedure

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = providerid
            oCmd.Parameters.Add(oParam)

            oParam = New SqlParameter("@setting", SqlDbType.VarChar, 100)
            oParam.Value = setting
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

    '20101129 - pab - add more code for aliases to aid in debugging
    Public Function GetAliases() As DataTable

        Dim dt As DataTable = New DataTable()

        Using conn As New SqlConnection()
            conn.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
            Using cmd As New SqlCommand
                cmd.Connection = conn
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandText = "sp_GetAliases"

                conn.Open()

                Using rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                    dt.Load(rdr)
                End Using
            End Using
        End Using

        Return dt
    End Function

    Public Function GetAirportServiceByCode(ByRef CarrierID As Integer, ByRef airportID As String) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try

            'oConn = New SqlConnection(ConfigurationManager.ConnectionStrings("Portal").ConnectionString)
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetServiceAirportsByCode", oConn)
            dt = New DataTable
            oParam = Nothing

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oParam = New SqlParameter("@code", SqlDbType.Char)
            oParam.Value = airportID
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

    Public Function GetHomeBaseByType(ByRef CarrierID As Integer, ByRef aircraftTypeServiceSpecID As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As New DataTable

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(ProdPortalServer))
            oAdp = New SqlDataAdapter("sp_GetAircraftHomeBaseByType", oConn)
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

    Public Function GetFlightPricesPerHour(ByRef CarrierID As Integer) As DataTable

        Dim oConn As SqlConnection = Nothing
        Dim oAdp As SqlDataAdapter = Nothing
        Dim oParam As SqlParameter = Nothing
        Dim dt As DataTable = Nothing

        Try
            oConn = New SqlConnection(ConnectionStringHelper.getglobalconnectionstring(PortalServer))
            oAdp = New SqlDataAdapter("sp_GetFlightPricesPerHourAll", oConn)

            oParam = New SqlParameter("@CarrierID", SqlDbType.Int)
            oParam.Value = CarrierID
            oAdp.SelectCommand.Parameters.Add(oParam)

            oAdp.SelectCommand.CommandType = CommandType.StoredProcedure

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
