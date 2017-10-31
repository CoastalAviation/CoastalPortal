'20140224 - pab - add threading
Imports System.Threading

Public Class AirTaxi

    '20150921 - move to SDS SQL server
    Public Shared PortalServer As String = "UATPortalServer"
    Public Shared PortalData As String = "UATPortalData"
    Public Shared PortalDriver As String = "UATPortalDriver"
    Public Shared ProdPortalDriver As String = "ProdPortalDriver"
    Public Shared ProdPortalServer As String = "ProdPortalServer"
    Public Shared UATPortalServer As String = "UATPortalServer"
    Public Shared DevPortalServer As String = "DevPortalServer"
    Public Shared DriverQS As String = ""
    Public Shared ServerQS As String = ""
    Public Shared DataSourceQS As String = ""
    Public Shared DriverOpt As String = ""
    Public Shared ServerOpt As String = ""
    Public Shared DataSourceOpt As String = ""

    '20101101 - pab - clean up connection strings
    'Public Shared connectstring As String = "PROVIDER=MSDASQL;driver={SQL Server};server=(local);uid=sa;password=CoastalPass1;database=Portal"
    'Public Shared connectstringfaa As String = "PROVIDER=MSDASQL;driver={SQL Server};server=(local);uid=sa;password=CoastalPass1;database=SuperPortalV3"
    'Public Shared ConnectionStringHelper.GetCASConnectionStringSQL As String = "Data Source=(local);Initial Catalog=Portal;Persist Security Info=True;User ID=sa;Password=CoastalPass1"
    Public Shared connectstring As String = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
    'Public Shared connectstringfaa As String = ConnectionStringHelper.GetCASConnectionString()
    Public Shared connectstringsql As String = ConnectionStringHelper.getglobalconnectionstring(PortalDriver)

    '20140523 - pab - change to dynamic optimizer database location
    Public Shared Driver As String = ""
    Public Shared Server As String = ""
    Public Shared DataSource As String = ""
    Public Shared lastcheck As DateTime = CDate("4/8/2004 10:01AM")

    '20170418 - pab - move to ibm
    Public Shared DriverIBMtest As String = ""
    Public Shared ServerIBMtest As String = ""
    Public Shared DriverROIBMtest As String = ""
    Public Shared ServerROIBMtest As String = ""

    '201206010 - pab - make app name a constant
    Public Shared appName As String = "CoastalPortal"

    Public Shared timing As String = ""

    '20111004 - pab - single db
    Public Shared _carrierid As Integer

    Public Shared _emailfrom As String

    '20101105 - pab - add code for aliases
    Public Shared _urlalias As String

    Public Shared cnsetting As New ADODB.Connection

    'rk 6.7.2010 configure name and logo
    Public Shared companyname As String
    Public Shared companylogo As String
    Public Shared companysplash As String
    Public Shared companywelcomeleft As String
    Public Shared companywelcomeright As String

    '20171027 - pab - calendar
    Public Shared modelrunid As String
    Public Shared _CalendarTimeZone As String
    Public Shared _CalendarDetailVisible As Boolean
    Public Shared usedevdb As Boolean = False

    '20171030 - pab - run optimizer page
    Public Shared testrun As Boolean = False
    Public Shared usevmdb As Boolean = True

    '20171030 - pab - run optimizer page
    Public Shared Function postToServiceBusQueue(ByVal queid As String, ByVal message As String, minutesdelay As Integer) As String

        Dim tf As String
        tf = Trim(UCase(ConnectionStringHelper.ts))

        '  If OptimizerDesktop.frmDesktop.chkTest.Checked = True Then
        ' tf = "TEST"
        'End If

        Dim retries As Integer = 0

again:

        Try
            Dim ws As New CATMSGQ.msgq

            postToServiceBusQueue = ws.Post(message, Trim(queid) & tf, 60, minutesdelay)

            If InStr(postToServiceBusQueue, "Added record") = 0 Then
                retries = retries + 1

                If retries < 20 Then
                    GoTo again
                Else
                    DataAccess.Insert_sys_log(_carrierid, "Error PostToServiceBusQueue 20 retries " & postToServiceBusQueue,
                        Trim(queid) & Trim(UCase(ConnectionStringHelper.ts)) & " " & Now, "PostToServiceBusQueue cds", "Post")
                    sendemailtemplate("5612397068@txt.att.net", "unable to post after 20 retries", "queid " & queid & "msg " & message, 100)

                End If
            End If

            Return postToServiceBusQueue

        Catch ex As Exception

            DataAccess.Insert_sys_log(_carrierid, " ERROR PSQ ", Trim(queid) & Trim(UCase(ConnectionStringHelper.ts)) & " " & Now, ex.Message & ":" & ex.StackTrace, "Post")
            Return ""
        End Try


        'Try
        '    Dim ws As New CATMSGQ.msgq
        '    postToServiceBusQueue = ws.Post(message, Trim(queid) & Trim(UCase(ConnectionStringHelper.testflag)), 60, minutesdelay)
        '    Return postToServiceBusQueue

        'Catch ex As Exception

        '    DataAccess.Insertsys_log(100, "PostToServiceBusQueue " & ex.Message & ":" & ex.StackTrace, Trim(queid) & Trim(UCase(ConnectionStringHelper.testflag)) & " " & Now, "autobatch cds", "Post")
        '    Return ""
        'End Try

        Exit Function

    End Function

    '20171030 - pab - run optimizer page
    Public Shared Function PopFromServiceBusQueue(ByVal queid As String) As String


        Try

            Dim connectionString As String

            If queid = "acceptchanges" Then
                '   connectionString = "Endpoint=sb://acceptall.servicebus.windows.net/;SharedAccessKeyName=acceptchanges;SharedAccessKey=7XoF3jyMSaEiuW6LXgQ2r2H+2qALGJItTBOs6Y09jsA="
                connectionString = "Endpoint=sb://optimizerscheduling.servicebus.windows.net/;SharedAccessKeyName=accept;SharedAccessKey=pF40xuMcLKJ9hzJZ2VgeTQsw65grr9kaZaNQzivPUuc="

            End If


            'Dim Client As QueueClient = QueueClient.CreateFromConnectionString(connectionString, queid, ReceiveMode.ReceiveAndDelete)

            '  Client.



            'Dim mbr As New BrokeredMessage


            'mbr = Client.Peek
            'If IsNothing(mbr) Then
            '    PopFromServiceBusQueue = ""
            '    Exit Function

            'End If

            'mbr = Client.Receive


            'If Not (IsNothing(mbr)) Then

            '    Dim a, b, c As String

            '    c = mbr.Label
            '    b = mbr.GetBody(Of String)()

            '    PopFromServiceBusQueue = c 'message.GetBody(Of String)

            'Else

            '    PopFromServiceBusQueue = ""
            'End If


        Catch

            Return ""
        End Try



    End Function

    '20171030 - pab - run optimizer page
    Shared Function sendemailtemplate(targetemail As String, subject As String, b As String, carrierid As Integer)


        b = Replace(b, "...", "</br>")

        Dim body As String

        body = "   <head>  <style type=%text/css% media=%screen%> " &
            "table{        border-collapse:collapse;         border:1px solid #0000FF;         } " &
    "        table td{         border:1px solid #0000FF;         }         </style>    </head>"
        body = Replace(body, "%", Chr(34))


        body &= "<div style=""font-family:tahoma;text-align:left;"">"
        body &= "<img src=cid:companylogo>"
        body &= "<h3 style=""font-size:0.8em;""> :" & subject & "</h3>"
        body &= "<table border=""0"" cellpadding=""3"" cellspacing=""0"" style=""font-size:0.8em;  border:1px solid #FF0000;"">"

        body &= "<tr>"

        body &= "<td valign=""top"" align=""right"">"
        body &= b
        body &= "</td>"

        'body &= "<td valign=""top"" align=""left"">"
        'body &= "http://www.optimizerpanel.com/panel.aspx?modelrunid=" & b
        'body &= "</td>"


        body &= "</tr>"


        body &= "</table>"
        body &= "</br>"
        body &= "</br>"

        Dim carrierlogo As String = GetSetting(carrierid, "CompanyLogo", "")

        If testrun = False Then

            body = body

            Try
                ' Dim ws As New coastalavtech.service.WebService1
                Dim ws As New aws.WebService1
                ws.SendEmail("pbaumgart@ctgi.com", "123", carrierid, targetemail, "",
                       "optimizer@coastalaviationsoftware.com", subject, body, True, "", carrierlogo, True, "")
            Catch ex As Exception

                ex = ex
            End Try

        End If


    End Function

    '20140723 - pab - use sql storage for carrier logos and aircraft images
    '20150114 - pab - use blob storage for carrier logos and aircraft images instead of sql
    Shared Function GetImageURLByATSSID(ByRef carrierid As Integer, ByRef ServiceTypeID As Integer, ByRef imgtype As String) As String

        'create querystring value
        Dim da As New DataAccess
        'Dim dt As DataTable = da.GetImageFromDBByAircraftID(carrierid, ServiceTypeID)
        Dim dt As DataTable
        Dim strID As String = String.Empty
        Dim filename As String = String.Empty

        If imgtype = "ac" Then
            dt = da.GetAircraftTypeServiceSpecsByID(carrierid, ServiceTypeID)
            If Not isdtnullorempty(dt) Then
                'If imgtype = "ac" Then
                '    For i As Integer = 0 To dt.Rows.Count - 1
                '        If InStr(dt.Rows(i).Item("image_name").ToString.ToLower, "t_") > 0 Then strID = dt.Rows(i).Item("image_ID").ToString
                '    Next
                'Else
                '    For i As Integer = 0 To dt.Rows.Count - 1
                '        If InStr(dt.Rows(i).Item("image_name").ToString.ToLower, "_sm") > 0 Then strID = dt.Rows(i).Item("image_ID").ToString
                '    Next
                'End If
                filename = dt.Rows(0).Item("LogoURL").ToString
                If InStr(filename, "t_") = 0 Then
                    filename = "t_" & filename
                End If
            End If

            '20150127 - pab - upload and display pdfs
        ElseIf imgtype = "pdf" Then
            filename = ServiceTypeID.ToString & ".pdf"

        Else
            filename = da.GetSetting(_carrierid, "CompanyLogo")
            If filename = "" Then filename = "images/no_aircraft_photo.gif"

            ''20171007 - pab - jetlinx branding
            '20171025 - pab - values updated in setting table
            'Select Case _carrierid
            '    Case 104
            '        filename = "images/jetlinx-grey-logo.png"

            '        '20171017 - pab - demoair branding
            '    Case 48
            '        filename = "images/cat.ico"

            'End Select
        End If

        '20171007 - [ab - jetlinx branding - no more blob storage
        Return filename

    End Function

    '20101105 - pab - add code for aliases
    Shared Sub geturlaliasandconnections(ByVal host As String)

        Dim da As New DataAccess
        Dim urlalias As String
        Dim dbstring As String

        Try

            dbstring = "DemoAir"

            If Left(host.ToLower, 4) = "www." Then
                host = Mid(host.ToLower, 5)
            End If

            If host = "localhost" Or Left(host, 4) = "www." Then
                'called without alias - default to superportal
                urlalias = dbstring
            Else
                '20130109 - pab - fix object not set
                If InStr(host, ".") > 0 Then
                    urlalias = Left(host, InStr(host, ".") - 1)
                Else
                    urlalias = host
                    Insertsys_log(0, appName, "host " & host, "geturlaliasandconnections", "AirTaxi.vb")
                End If
            End If

            If IsNumeric(urlalias) Then urlalias = dbstring

            _urlalias = urlalias

            '20120515 - pab - get carrierid too
            Dim dt As DataTable = da.GetProviderByAlias(_urlalias)
            If dt.Rows.Count > 0 Then
                If IsNumeric(dt.Rows(0).Item("carrierid").ToString) Then
                    _carrierid = CInt(dt.Rows(0).Item("carrierid").ToString)
                    _urlalias = urlalias
                Else
                    '20171018 - pab - clear alias and carrierid if not found
                    _carrierid = 0
                    _urlalias = ""
                    '20130328 - pab - problems addine new carrier - add logging to debug
                    Insertsys_log(_carrierid, appName, "carrierid not numeric - host " & host & "; _urlalias - " & _urlalias & "; dt.Rows(0).Item(""carrierid"").ToString - " & dt.Rows(0).Item("carrierid").ToString, "geturlaliasandconnections", "AirTaxi.vb")
                End If
            Else
                '20171018 - pab - clear alias and carrierid if not found
                _carrierid = 0
                _urlalias = ""
                '20130328 - pab - problems adding new carrier - add logging to debug
                Insertsys_log(_carrierid, appName, "da.GetProviderByAlias(_urlalias) failed - host " & host & "; _urlalias - " & _urlalias & "; _carrierid - " & _carrierid, "geturlaliasandconnections", "AirTaxi.vb")
            End If

            '20130109 - pab - fix object not set
            'connectstring = ConnectionStringHelper.GetConnectionString()
            'connectstringsql = ConnectionStringHelper.GetConnectionStringSQL()
            '20130702 - pab - remove duplicate connection strings
            'connectstring = ConnectionStringHelper.GetD2DConnectionString()
            connectstring = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
            connectstringsql = ConnectionStringHelper.getglobalconnectionstring(PortalDriver)

            '20130328 - pab - problems addine new carrier - add logging to debug
            Insertsys_log(_carrierid, appName, "host - " & host & "; _urlalias - " & _urlalias & "; _carrierid - " & _carrierid, "geturlaliasandconnections", "AirTaxi.vb")

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(_carrierid, appName, Left(s, 500), "geturlaliasandconnections", "AirTaxi.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " AirTaxi.vb geturlaliasandconnections Error", s, 0)
            '20131024 - pab - fix duplicate emails
            'SendEmail("CharterSales@coastalavtech.com", "rkane@coastalaviationsoftware.com", appName & " AirTaxi.vb geturlaliasandconnections Error", s, 0)
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " AirTaxi.vb geturlaliasandconnections Error", s, _carrierid)

        End Try

    End Sub

    '20140224 - pab - add threading
    Public Shared Sub Insertsys_log(
         ByVal CarrierID As Integer,
         ByVal UserID As String,
         ByVal Message As String,
         ByVal CallingParty As String,
         ByVal IP As String)

        Dim tu As Thread
        Dim tup As String

        tu = New Thread(AddressOf Insertsys_log_thread)
        tup = "CarrierID[" & CarrierID & "]"
        tup = tup & "UserID[" & UserID & "]"
        tup = tup & "Message[" & Message & "]"
        tup = tup & "CallingParty[" & CallingParty & "]"
        tup = tup & "IP[" & IP & "]"
        tu.Start(tup)

    End Sub

    '20140224 - pab - add threading
    Public Shared Sub Insertsys_log_thread(a As String)

        Dim CarrierID As String = InBetween(1, a, "CarrierID[", "]")
        Dim UserID As String = InBetween(1, a, "UserID[", "]")
        Dim Message As String = InBetween(1, a, "Message[", "]")
        Dim CallingParty As String = InBetween(1, a, "CallingParty[", "]")
        Dim IP As String = InBetween(1, a, "IP[", "]")

        Try
            DataAccess.Insert_sys_log(CarrierID, UserID, Message, CallingParty, IP)

        Catch ex As Exception
            Dim s As String = ex.Message
        End Try

    End Sub

    '20140224 - pab - add threading
    Public Shared Sub InsertEmailQueue(
             ByVal CarrierID As Integer,
             ByVal EmailFrom As String,
             ByVal EmailTo As String,
             ByVal EmailCC As String,
             ByVal EmailBCC As String,
             ByVal EmailSubject As String,
             ByVal EmailBody As String,
             ByVal IsBodyHtml As Boolean,
             ByVal Attachment As String,
             ByVal CompanyLogo As String,
             ByVal AircraftLogo As String,
             ByVal ShowCarrier As Boolean)

        Dim tu As Thread
        Dim tup As String

        tu = New Thread(AddressOf InsertEmailQueue_thread)
        '20141211 - pab - fix messages getting truncated
        tup = "CarrierID|||" & CarrierID & "|||"
        tup = tup & "EmailFrom|||" & EmailFrom & "H"
        tup = tup & "EmailTo|||" & EmailTo & "|||"
        tup = tup & "EmailCC|||" & EmailCC & "|||"
        tup = tup & "EmailBCC|||" & EmailBCC & "|||"
        tup = tup & "EmailSubject|||" & EmailSubject & "|||"
        tup = tup & "EmailBody|||" & EmailBody & "|||"
        tup = tup & "IsBodyHtml|||" & IsBodyHtml & "|||"
        tup = tup & "Attachment|||" & Attachment & "|||"
        tup = tup & "CompanyLogo|||" & CompanyLogo & "|||"
        tup = tup & "AircraftLogo|||" & AircraftLogo & "|||"
        tup = tup & "ShowCarrier|||" & ShowCarrier & "|||"
        tu.Start(tup)

    End Sub

    '20140224 - pab - add threading
    Public Shared Sub InsertEmailQueue_thread(a As String)

        '20141211 - pab - fix messages getting truncated
        Dim CarrierID As String = InBetween(1, a, "CarrierID|||", "|||")
        Dim EmailFrom As String = InBetween(1, a, "EmailFrom|||", "|||")
        Dim EmailTo As String = InBetween(1, a, "EmailTo|||", "|||")
        Dim EmailCC As String = InBetween(1, a, "EmailCC|||", "|||")
        Dim EmailBCC As String = InBetween(1, a, "EmailBCC|||", "|||")
        Dim EmailSubject As String = InBetween(1, a, "EmailSubject|||", "|||")
        Dim EmailBody As String = InBetween(1, a, "EmailBody|||", "|||")
        Dim IsBodyHtml As String = InBetween(1, a, "IsBodyHtml|||", "|||")
        Dim Attachment As String = InBetween(1, a, "Attachment|||", "|||")
        Dim CompanyLogo As String = InBetween(1, a, "CompanyLogo|||", "|||")
        Dim AircraftLogo As String = InBetween(1, a, "AircraftLogo|||", "|||")
        Dim ShowCarrier As String = InBetween(1, a, "ShowCarrier|||", "|||")

        Try
            DataAccess.Insert_Email_Queue(CarrierID, EmailFrom, EmailTo, EmailCC, EmailBCC, EmailSubject, EmailBody, IsBodyHtml, Attachment, CompanyLogo,
                                        AircraftLogo, ShowCarrier)

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= " - " & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            Insertsys_log(0, appName, s, "InsertEmailQueue_thread", "AirTaxi.vb")
        End Try

    End Sub

    '20120807 - pab - write to email queue
    '20150524 - pab - quote emails now html
    Shared Function SendEmail(ByVal emailFrom As String, ByVal emailTo As String, ByVal emailcc As String, ByVal emailSubject As String, ByVal emailBody As String,
                              ByVal CarrierID As Integer) As Boolean

        SendEmail = False

        Dim da As New DataAccess

        Try
            '20120807 - pab - write to email queue
            'Dim eMsg As Net.Mail.MailMessage = Nothing


            ''define smtp server client
            'Dim client As New SmtpClient(da.GetSetting(_carrierid, "smtpServer"))

            'eMsg = New MailMessage(emailFrom, emailTo, emailSubject, emailBody)


            'eMsg.IsBodyHtml = True

            ''AirTaxi.post_timing(emailBody)

            ''Send the message
            'client.Send(eMsg)

            '20131024 - pab - fix duplicate emails
            'Dim emailcc As String = String.Empty
            'If emailTo = "rkane@coastalaviationsoftware.com" Then
            '    emailcc = "pbaumgart@coastalaviationsoftware.com"
            'ElseIf emailTo = "pbaumgart@coastalaviationsoftware.com" Then
            '    emailcc = "rkane@coastalaviationsoftware.com"
            'End If
            'If emailSubject = "getsetting error" Then
            '    Exit Function

            'ElseIf emailSubject = "Optimizer Error 1" Or InStr(emailSubject, " Error!") > 0 Then
            '    SendEmail = InsertEmailQueue(CarrierID, emailFrom, "5612397068@txt.att.net", emailcc, "", emailSubject, emailBody, _
            '                                             False, "", "", "", False)

            '    'ElseIf InStr(emailSubject, "Timing Email ! Price Request from ") > 0 Then
            '    '    eMsg = New MailMessage(emailFrom, "pbaumgart@coastalaviationsoftware.com", emailSubject, emailBody)
            '    '    client.Send(eMsg)
            'ElseIf InStr(emailSubject, "Price Request from ") > 0 Then
            '    '20130614 - pab - fix invalid email
            '    'emailTo = "casquoter@coastalaviationsoftware.com; " & emailTo
            '    emailTo = "CharterSales@coastalavtech.com; " & emailTo

            'End If
            If InStr(emailTo, "rkane@coastalaviationsoftware.com") = 0 And InStr(emailTo, "pbaumgart@coastalaviationsoftware.com") = 0 Then
                If emailcc = "" Then
                    emailcc = "rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com"
                Else
                    emailcc &= "; rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com"
                End If
            ElseIf InStr(emailTo, "pbaumgart@coastalaviationsoftware.com") = 0 Then
                If emailcc = "" Then
                    emailcc = "pbaumgart@coastalaviationsoftware.com"
                Else
                    emailcc &= "; pbaumgart@coastalaviationsoftware.com"
                End If
            ElseIf InStr(emailTo, "rkane@coastalaviationsoftware.com") = 0 Then
                If emailcc = "" Then
                    emailcc = "rkane@coastalaviationsoftware.com"
                Else
                    emailcc &= "; rkane@coastalaviationsoftware.com"
                End If
            End If

            InsertEmailQueue(CarrierID, emailFrom, emailTo, emailcc, "", emailSubject, emailBody, False, "", "", "", False)

            SendEmail = True

        Catch ex As Exception
            AirTaxi.post_timing(ex.ToString)

            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then
                s &= " - " & ex.InnerException.ToString
            End If
            If Not IsNothing(ex.StackTrace) Then
                s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            End If
            Insertsys_log(CarrierID, appName, s, "SendEmail", "AirTaxi.vb")

        End Try
    End Function

    Shared Function myflightrecs(ByRef userid As String) As String




        ' Inherits System.Web.UI.Page
        '20100222 - pab - use global shared connection
        'Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset


        '20100222 - pab - use global shared connection
        'If cn.State = 1 Then cn.Close()
        'If cn.State = 0 Then

        '    '20081209 - pab - use connection string in web.config
        '    'cn.ConnectionString = connectstring
        '    cn.ConnectionString = xonfigurationmanager.ConnectionStrings("PortalConnect").ConnectionString

        '    cn.Open()
        'End If
        If cnsetting.State = 0 Then
            cnsetting.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalDriver)
            cnsetting.Open()
        End If

        If rs.State = 1 Then rs.Close()


        Dim req As String

        'req = "SELECT * FROM FlightRequest WHERE userid = '" & userid & "' and flightstatus = 'P'"
        req = "SELECT * FROM SYS_FLIGHTS_USERS sfu left join Flights f on sfu.ID = f.ID WHERE Email = '" & userid & "' and f.DepartureTime > '" & Now & "'"
        req &= " and sfu.carrierid = " & _carrierid


        'rs.Open(req)
        '20100222 - pab - use global shared connection
        'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)



        If rs.EOF Then
            myflightrecs = "You have no pending flight requests."
            rs.Close()
            Exit Function
        End If



        Dim ii As Integer


        Dim costs As Long


        Do While Not rs.EOF

            ii = ii + 1


            If IsDBNull(rs.Fields("addlcost").Value) Then
                costs = 0
            Else
                costs = costs + rs.Fields("addlcost").Value
            End If




            rs.MoveNext()

        Loop


        myflightrecs = "You have " & ii & " pending flight requests"

        If costs = 0 Then
            myflightrecs = myflightrecs & "."
        Else
            myflightrecs = myflightrecs & " at " & costs & " additional dollars."
        End If



        rs.Close()









    End Function

    Shared Function InBetween(ByVal Start As Integer, ByVal work As String, ByVal target As String, ByVal target2 As String) As String

        Dim pos As Integer
        Dim pos2 As Integer
        Dim work1 As String


        If Start = 0 Then
            InBetween = ""
            Exit Function
        End If

        pos = InStr(Start, work, target, vbTextCompare)

        pos = pos + Len(target)
        pos2 = InStr(pos, work, target2, vbTextCompare)

        If pos = 0 + Len(target) Or pos2 = 0 Then
            InBetween = ""
            Exit Function
        End If


        work1 = Mid(work, pos, pos2 - pos)

        InBetween = work1

    End Function

    Shared Sub post_timing(ByVal s As String)

        timing = timing & Now.Second & ": " & s & vbCr & vbLf & vbCr & vbLf

    End Sub

    '20120320 - pab - changes for azure
    Shared Function isdtnullorempty(ByRef dt As DataTable) As Boolean

        isdtnullorempty = True
        If IsNothing(dt) Then Exit Function
        If dt.Rows.Count > 0 Then isdtnullorempty = False

    End Function

    Shared Function IsSqlSafe(ByVal s As String) As Boolean
        If System.Text.RegularExpressions.Regex.IsMatch(s, "^[A-Za-z0-9@_\.\-]{0,200}$") Then
            Return True
        Else
            Return False
        End If
    End Function

End Class
