Imports CoastalPortal.AirTaxi

Public Class ConnectionStringHelper

    '20171030 - pab - run optimizer page
    Public Shared ts As String = ""
    Public Shared usevmdb As Boolean = True
    Public Shared testflag As String = ""

    '20171030 - pab - run optimizer page
    Public Shared Function GetConnectionStringSQLMKAzure() As String

        If Not testrun Then

            If usevmdb Then Return getglobalconnectionstring("OptimizerDriver" & ts)
            '  If usevmdb Then Return "Driver={SQL Server Native Client 11.0};server=tcp:optimizersqlvm.cloudapp.net,1433;database=OptimizerWest;uid=cas;Password=n621kf!12;Encrypt=no;"

            'Return "Driver={SQL Server Native Client 11.0};server=tcp:b2pqcffmjl.database.windows.net,1433;database=OptimizerWest;uid=OptimizerWest@b2pqcffmjl;Password=n621kf!12;Encrypt=yes;"
        Else
            Return "Driver={SQL Server Native Client 11.0};Server=RICHARDdesktop;database=OptimizerWest;User ID=sa;Password=n621kf!12;"

        End If

    End Function

    '20140523 - pab - change to dynamic optimizer database location
    Shared Function getglobalconnectionstring(connection As String) As String

        If DateDiff(DateInterval.Minute, lastcheck, Now) > 30 Then
            Driver = ""
            Server = ""
            lastcheck = Now

            '20170418 - pab - move to ibm
            DriverIBMtest = ""
            ServerIBMtest = ""
            DriverROIBMtest = ""
            ServerROIBMtest = ""
        End If

        '20170418 - pab - move to ibm
        If UCase(connection) = "OPTIMIZERDRIVERIBMTEST" And DriverIBMtest <> "" Then Return DriverIBMtest
        If UCase(connection) = "OPTIMIZERSERVERIBMTEST" And ServerIBMtest <> "" Then Return ServerIBMtest

        If UCase(connection) = "OPTIMIZERDRIVERROIBMTEST" And DriverROIBMtest <> "" Then Return DriverROIBMtest
        If UCase(connection) = "OPTIMIZERSERVERROIBMTEST" And ServerROIBMtest <> "" Then Return ServerROIBMtest

        '20141007 - pab - make quotesupport db location dynamic 
        '20150921 - move to SDS SQL server
        'If (UCase(connection) = "OPTIMIZERDRIVER" Or UCase(connection) = "QUOTESUPPORTDRIVER") And Driver <> "" Then Return Driver
        'If (UCase(connection) = "OPTIMIZERSERVER" Or UCase(connection) = "QUOTESUPPORTSERVER") And Server <> "" Then Return Server
        'If (UCase(connection) = "OPTIMIZERDATASOURCE" Or UCase(connection) = "QUOTESUPPORTDATASOURCE") And DataSource <> "" Then Return DataSource
        Select Case UCase(connection)
            Case PortalDriver.ToUpper
                If Driver <> "" Then Return Driver
            Case PortalServer.ToUpper
                If Server <> "" Then Return Server
            Case PortalData.ToUpper
                If DataSource <> "" Then Return DataSource
            Case "QUOTESUPPORTDRIVER"
                If DriverQS <> "" Then Return DriverQS
            Case "QUOTESUPPORTSERVER"
                If ServerQS <> "" Then Return ServerQS
            Case "QUOTESUPPORTDATASOURCE"
                If DataSourceQS <> "" Then Return DataSourceQS
            Case "OPTIMIZERDRIVER"
                If DriverOpt <> "" Then Return DriverOpt
            Case "OPTIMIZERSERVER"
                If ServerOpt <> "" Then Return ServerOpt
            Case "OPTIMIZERDATASOURCE"
                If DataSourceOpt <> "" Then Return DataSourceOpt

                '20170418 - pab - move to ibm
            Case "OPTIMIZERDRIVERIBMTEST"
                If DriverIBMtest <> "" Then Return DriverIBMtest
            Case "OPTIMIZERSERVERIBMTEST"
                If ServerIBMtest <> "" Then Return ServerIBMtest
            Case "OPTIMIZERDRIVERROIBMTEST"
                If DriverROIBMtest <> "" Then Return DriverROIBMtest
            Case "OPTIMIZERSERVERROIBMTEST"
                If ServerROIBMtest <> "" Then Return ServerROIBMtest

            Case Else
                'AirTaxi.Insertsys_log(0, AirTaxi.appName, "Invalid connection type - " & connection, "getglobalconnectionstring", _
                '    "ConnectionStringHelper.vb")
                'AirTaxi.SendEmail2("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName & _
                '    " ConnectionStringHelper.vb getglobalconnectionstring Error", "Parms - connection " & connection & vbCr & vbLf & _
                '    "Invalid connection type - " & connection, _carrierid)
        End Select

        Dim cnglobal As New ADODB.Connection
        Try
            Dim rs As New ADODB.Recordset

            If cnglobal.State = 1 Then cnglobal.Close()

            If cnglobal.State = 0 Then
                '20150205 - pab - move azure db to bizspark plus subscription
                'cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=b2pqcffmjl.database.windows.net,1433;database=OptimizerWest;uid=OptimizerWest@b2pqcffmjl;Password=n621kf!12;Encrypt=no;"
                '20150921 - move to SDS SQL server
                'cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=xhmagkz1j8.database.windows.net;database=OptimizerWest;uid=cas;Password=n621kf!12;Encrypt=no;"
                '20151105 - pab - use sql azure table for fault tolerance
                'cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=tcp:QuoteSDS2.cloudapp.net,14331;database=OptimizerWest;uid=cas;Password=n621kf!12;Encrypt=no;"
                'cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=xhmagkz1j8.database.windows.net,1433;database=OptimizerWest;uid=cas@xhmagkz1j8;Password=n621kf!12;Encrypt=no;Connection Timeout = 30"
                '20160520 - pab - point to new url table
                '20160930 - pab - possbile conflicts with optimizer - move to different db
                'cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=azureurlserver.database.windows.net,1433;database=azureurls;uid=cas@azureurlserver;Password=n621kf!12;Encrypt=no;Connection Timeout = 30"
                '20161128 - pab - new ms subscription and sql server
                'cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=tcp:quotingdb5950.cloudapp.net,14331;database=azureurls;uid=cas;Password=n621kf!12;Encrypt=no;Connection Timeout = 30"
                'cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=tcp:coastalavtsqlserver3.westus.cloudapp.azure.com,12333;database=azureurls;uid=cas;Password=n621kf!12;Encrypt=no;Connection Timeout = 30"

                '20170418 - pab - move to ibm
                'If InStr(connection.ToUpper, "IBM") = 0 Then
                '    cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=catazureserver.database.windows.net,1433;database=CATUrls;uid=cas@catazureserver;Password=n621kf!12;Encrypt=no;Connection Timeout = 30"
                'Else
                '20170704 - pab - new ip
                'cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=10.176.218.77,14442;database=azureurls;uid=cas;Password=n621kf!12;Encrypt=no;Connection Timeout = 30"
                cnglobal.ConnectionString = "Driver={SQL Server Native Client 11.0};server=10.177.209.23,14442;database=azureurls;uid=cas;Password=n621kf!12;Encrypt=no;Connection Timeout = 30"
                'End If
                cnglobal.Open()

            End If

            If rs.State = 1 Then rs.Close()


            Dim req As String

            req = "SELECT * "
            req = req & "FROM url WHERE ctype = '" & connection & "' "

            'rk 11/1/2010 added self healing connection code
            Try
                rs.Open(req, cnglobal, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
            Catch ex As Exception
                cnglobal.Close()
                getglobalconnectionstring = ""
                Exit Function
            End Try


            If rs.EOF Then
                getglobalconnectionstring = ""
            Else

                getglobalconnectionstring = rs.Fields("CLOC").Value.ToString.Trim

                '20141007 - pab - make quotesupport db location dynamic 
                '20150921 - move to SDS2 SQL server
                'If (UCase(connection) = "OPTIMIZERDRIVER" Or UCase(connection) = "QUOTESUPPORTDRIVER") Then Driver = getglobalconnectionstring
                'If (UCase(connection) = "OPTIMIZERSERVER" Or UCase(connection) = "QUOTESUPPORTSERVER") Then Server = getglobalconnectionstring
                'If (UCase(connection) = "OPTIMIZERDATASOURCE" Or UCase(connection) = "QUOTESUPPORTDATASOURCE") Then DataSource = getglobalconnectionstring
                Select Case UCase(connection)
                    Case PortalDriver.ToUpper
                        Driver = getglobalconnectionstring
                    Case PortalServer.ToUpper
                        Server = getglobalconnectionstring
                    Case PortalData.ToUpper
                        DataSource = getglobalconnectionstring
                    Case "QUOTESUPPORTDRIVER"
                        DriverQS = getglobalconnectionstring
                    Case "QUOTESUPPORTSERVER"
                        ServerQS = getglobalconnectionstring
                    Case "QUOTESUPPORTDATASOURCE"
                        DataSourceQS = getglobalconnectionstring
                    Case "OPTIMIZERDRIVER"
                        DriverOpt = getglobalconnectionstring
                    Case "OPTIMIZERSERVER"
                        ServerOpt = getglobalconnectionstring
                    Case "OPTIMIZERDATASOURCE"
                        DataSourceOpt = getglobalconnectionstring

                '20170418 - pab - move to ibm
                    Case "OPTIMIZERDRIVERIBMTEST"
                        DriverIBMtest = getglobalconnectionstring
                    Case "OPTIMIZERSERVERIBMTEST"
                        ServerIBMtest = getglobalconnectionstring
                    Case "OPTIMIZERDRIVERROIBMTEST"
                        DriverROIBMtest = getglobalconnectionstring
                    Case "OPTIMIZERSERVERROIBMTEST"
                        ServerROIBMtest = getglobalconnectionstring
                    Case Else
                End Select

            End If

            rs.Close()
            cnglobal.Close()

        Catch ex As Exception
            getglobalconnectionstring = ""

        End Try

    End Function

End Class
