'20140224 - pab - add threading
Imports System.Threading
Imports CoastalPortal.DataAccess
Imports StackExchange.Redis
Imports CoastalPortal.Models
Imports CoastalPortal.ConnectionStringHelper
Imports System.Data.SqlClient
Imports System.Collections.Generic

Public Class AirTaxi

    Public Shared OptimizerDB As String = If(ConnectionStringHelper.ts = "Test", "OptimizerTestDB", "OptimizerDB")
    Public Shared PortalDB As String = If(ConnectionStringHelper.ts = "Test", "PortalTestDB", "PortalDB")
    Public Shared cache As StackExchange.Redis.IDatabase
    Public Shared rdc As StackExchange.Redis.ConnectionMultiplexer
    Public Shared redisdown As DateTime = CDate("7/18/2017 10:00")

    '20150921 - move to SDS SQL server
    Public Shared PortalServer As String = "ProdPortalServer"
    Public Shared PortalData As String = "ProdPortalData"
    Public Shared PortalDriver As String = "ProdPortalDriver"
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
    Public Shared colornames(50) As String

    '20101101 - pab - clean up connection strings
    'Public Shared connectstring As String = "PROVIDER=MSDASQL;driver={SQL Server};server=(local);uid=sa;password=CoastalPass1;database=Portal"
    'Public Shared connectstringfaa As String = "PROVIDER=MSDASQL;driver={SQL Server};server=(local);uid=sa;password=CoastalPass1;database=SuperPortalV3"
    'Public Shared ConnectionStringHelper.GetCASConnectionStringSQL As String = "Data Source=(local);Initial Catalog=Portal;Persist Security Info=True;User ID=sa;Password=CoastalPass1"
    'Public Shared connectstringfaa As String = ConnectionStringHelper.GetCASConnectionString()

    Public Shared connectstring As String = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
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
    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    'Public Shared _carrierid As Integer

    '20171121 - pab - fix carriers changing midstream - change to Session variables
    'Public Shared _emailfrom As String
    'Public Shared _emailfromquote As String

    '20101105 - pab - add code for aliases
    '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
    'Public Shared _urlalias As String

    Public Shared cnsetting As New ADODB.Connection

    'rk 6.7.2010 configure name and logo
    '20171121 - pab - fix carriers changing midstream - change to Session variables
    'Public Shared companyname As String
    'Public Shared companylogo As String
    'Public Shared companysplash As String
    'Public Shared companywelcomeleft As String
    'Public Shared companywelcomeright As String
    Public Shared changecolor_dictionary As New Dictionary(Of String, String)
    Public Shared fstart_dictionary As New Dictionary(Of String, String)
    Public Shared aclookup_dictionary As New Dictionary(Of String, String)
    Public Shared awclookup As New Dictionary(Of String, String)
    'Public Shared FOSRecords As New List(Of FOSFlightsOptimizerRecord)
    'Public Shared CASRecords As New List(Of CASFlightsOptimizerRecord)
    Public Shared fname_dictionary As New Dictionary(Of String, String)

    Public Const DEMOAIR As Integer = 48
    Public Const XOJET As Integer = 49
    Public Const TMC As Integer = 65
    Public Const WHEELSUP As Integer = 100
    Public Const JETLINX As Integer = 104
    Public Const ASI As Integer = 107
    Public Const DELTA As Integer = 108
    '20171209 - pab - add instant jet
    Public Const INSTANTJET As Integer = 110
    '20180823 - pab - add gajet and mountain
    Public Const MOUNTAIN As Integer = 78
    Public Const GAJET As Integer = 112

    '20171121 - pab - fix carriers changing midstream - change to Session variables
    'Public Shared _fosmodelstartfos As Date
    'Public Shared modelrunid As String
    'Public Shared cascalendarmodelid As String
    'Public Shared casmodelrunid As String = ""

    'Public Shared _fosmodelrunid As String
    'Public Shared _fosmodelrunidcas As String
    'Public Shared _fosmodelstart As Date
    'Public Shared _fosmodelend As Date
    Public Shared overridemodel As String
    Public Shared AC1 As String
    Public Shared AC2 As String
    Public Shared ACX(50) As String
    Public Shared groups(50, 50) As String 'group number, tail number
    Public Shared groupcount As Integer

    Public Shared caslinebreaks As Integer

    Public Shared ocp As Double

    '20171027 - pab - calendar
    'Public Shared modelrunid As String
    '20171121 - pab - fix carriers changing midstream - change to Session variables
    'Public Shared _CalendarTimeZone As String
    'Public Shared _CalendarDetailVisible As Boolean
    'Public Shared usedevdb As Boolean = False

    ''20171030 - pab - run optimizer page
    'Public Shared testrun As Boolean = False
    'Public Shared usevmdb As Boolean = True

    ''20171109 - pab - add optimmizer model page
    'Public Shared daterangefrom, daterangeto As String

    'Public Shared ip As String
    'Public Shared distance_text As String = ""

    'Public Shared _waittime As Decimal
    'Public Shared _landingfees As Decimal
    'Public Shared _taxes As Decimal
    'Public Shared _internationalfees As Decimal
    'Public Shared _fuelsurcharges As Decimal
    'Public Shared _crewovernight As Decimal
    'Public Shared _aircraftname As String
    'Public Shared _aircraftlogo As String
    'Public Shared _totalprice As Decimal

    ''rk 10302010 add taxes to quote editor
    'Public Shared _segmentfee As Decimal

    ''20101027 - pab - add return to base bucket
    'Public Shared _rtbcost As Decimal

    ''20110214 - pab - more calendar changes
    'Public Shared _CalendarCombinedView As Boolean

    '20160726 - pab - add redis session state - fix serializable error
    <Serializable()> Partial Public Class LatLong

        Private LatitudeField As Double
        Private LongitudeField As Double

        '''<remarks/>
        Public Property Latitude() As Double
            Get
                Return Me.LatitudeField
            End Get
            Set(ByVal value As Double)
                Me.LatitudeField = value
            End Set
        End Property

        Public Property Longitude() As Double
            Get
                Return Me.LongitudeField
            End Get
            Set(ByVal value As Double)
                Me.LongitudeField = value
            End Set
        End Property

    End Class

    '20160622 - pab - check for overlapping flights
    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function traveltime(ByVal d As Integer, ByVal bIntl As Boolean, ByVal carrierid As Integer) As Integer

        Try

            Dim da As New DataAccess
            Dim _airSpeed As Double = 0
            Dim t As Long
            Dim currentplane As Integer = 0
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Dim dt_priceTable As DataTable = da.GetAircraftTypeServiceSpecsByWeightClass(carrierid, "L")
            Dim FlightTimeSwag As Double = dt_priceTable.Rows(currentplane)("FlightTimeSwag")
            Dim longrangedistance As Integer = CInt(dt_priceTable.Rows(currentplane)("longrangedistance") * 1.15077945)
            Dim dtAPD As DataTable = da.GetFuelSpeedByDistance(carrierid, dt_priceTable.Rows(currentplane)("currtype"), "")
            Dim fuelstops As Integer = 0
            Dim FuelStopMinutes As Integer = 30
            Dim CustomsTurnAroundMins As Integer = 30

            _airSpeed = CInt(dt_priceTable.Rows(currentplane)("Blockspeed")) * 1.15077945

            If dtAPD.Rows.Count <> 0 Then
                For i As Integer = 0 To dtAPD.Rows.Count - 1
                    If d >= dtAPD.Rows(i)("RangeLow") And d <= dtAPD.Rows(i)("RangeHigh") Then
                        _airSpeed = dtAPD.Rows(i)("speed") * 1.15077945
                        Exit For
                    End If
                Next i
            End If

            t = CInt((d / _airSpeed) * 60)
            t = t + (t * FlightTimeSwag) 'add an optional percentage to each flight time calculation

            If d > longrangedistance Then
                fuelstops = CInt(Math.Ceiling(d / longrangedistance)) - 1
                If fuelstops > 0 Then
                    t = t + (fuelstops * FuelStopMinutes)
                End If
            End If

            If CustomsTurnAroundMins > 0 And bIntl Then
                t = CInt(t) + CustomsTurnAroundMins
            End If

            '20160909 - pab - adjust departure times to allow for quick turns when flight times are unknown to user
            'If t < 120 Then t = 120

            traveltime = CInt(t)

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(carrierid, AirTaxi.appName, Left(Now & " " & s, 500), "traveltime", "AirTaxi.vb")

            Return 120

        End Try

    End Function

    '20171030 - pab - run optimizer page
    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Public Shared Function postToServiceBusQueue(ByVal queid As String, ByVal message As String, minutesdelay As Integer,
                                                 ByVal carrierid As Integer, ByVal testrun As Boolean) As String

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
                    DataAccess.Insert_sys_log(carrierid, "Error PostToServiceBusQueue 20 retries " & postToServiceBusQueue,
                        Trim(queid) & Trim(UCase(ConnectionStringHelper.testflag)) & " " & Now, "PostToServiceBusQueue cds", "Post")
                    sendemailtemplate("5612397068@txt.att.net", "unable to post after 20 retries", "queid " & queid & "msg " &
                        message, carrierid, testrun)

                End If


            End If

            Return postToServiceBusQueue



        Catch ex As Exception
            DataAccess.Insert_sys_log(carrierid, " ERROR PSQ ", Trim(queid) & Trim(UCase(ConnectionStringHelper.testflag)) & " " & Now,
                ex.Message & ":" & ex.StackTrace, "Post")
            Return ""
        End Try

    End Function

    '20120807 - pab - write to email queue
    Shared Function SendEmailLogo(ByVal recipient As String, ByVal emailcc As String, ByVal bcc As String, ByVal sender As String, ByVal subject As String, ByVal body As String,
            ByVal attachments As Generic.Stack(Of System.Net.Mail.Attachment), ByVal isBodyHtml As Boolean, ByVal ApplicationPath As String, ByVal CarrierID As Integer,
            ByVal aircraftlogo As String, ByVal serviceprovider As String, ByVal showcarrier As Boolean, ByVal attachment As String) As Boolean

        SendEmailLogo = False

        Try

            '20120807 - pab - write to email queue
            'Dim message As New MailMessage()



            ''set the addresses
            ''New MailAddress("noreply@Door2DoorAir.com", "reservations@Door2DoorAir.com"), New MailAddress(recipient)
            'message.From = New MailAddress(sender)
            'message.To.Add(recipient)

            ''set the content
            'message.Subject = subject

            ''first we create the Plain Text part
            'Dim plainView As AlternateView = AlternateView.CreateAlternateViewFromString(Replace(body, "<img src=cid:companylogo>", ""), Nothing, "text/plain")

            ''then we create the Html part
            ''to embed images, we need to use the prefix 'cid' in the img src value
            ''the cid value will map to the Content-Id of a Linked resource.
            ''thus <img src='cid:companylogo'> will map to a LinkedResource with a ContentId of 'companylogo'
            'Dim htmlView As AlternateView = AlternateView.CreateAlternateViewFromString(body, Nothing, "text/html")

            ''create the LinkedResource (embedded image)
            'Dim path As String = ApplicationPath

            ''rk 11/8/2010 check if logo setup correctly
            'If File.Exists(path & companylogo) Then
            '    Dim logo As New LinkedResource(path & companylogo)
            '    logo.ContentId = "companylogo"
            '    'add the LinkedResource to the appropriate view
            '    htmlView.LinkedResources.Add(logo)
            'End If

            ''rk 11/8/2010 check if logo setup correctly
            'If File.Exists(path & _aircraftlogo) Then
            '    Dim logo2 As New LinkedResource(path & _aircraftlogo)
            '    logo2.ContentId = "aircraftlogo"
            '    'add the LinkedResource to the appropriate view
            '    htmlView.LinkedResources.Add(logo2)
            'End If

            ''add the views
            'message.AlternateViews.Add(plainView)
            'message.AlternateViews.Add(htmlView)


            'If bcc.Trim().Length > 0 Then
            '    message.Bcc.Add(bcc.Trim().Replace(" ", ",").Replace(";", ","))
            'End If

            ''add attachments if included 
            'If attachments IsNot Nothing Then
            '    For Each currAttachment As Attachment In attachments
            '        message.Attachments.Add(currAttachment)
            '    Next
            'End If

            'message.Subject = subject
            ''message.Body = body

            'message.CC.Add("rkane@coastalaviationsoftware.com")
            'message.CC.Add("pbaumgart@coastalaviationsoftware.com")
            ''20110425 - pab - remove Carolyn 
            ''message.CC.Add("ckeating@coastalaviationsoftware.com")

            'message.IsBodyHtml = isBodyHtml

            ''define smtp server client
            'Dim da As New DataAccess
            'Dim client As New SmtpClient(da.GetSetting(_carrierid, "smtpServer"))

            'client.Send(message)

            'Return True

            Dim da As New DataAccess
            Dim providerlogo As String = da.GetSysSettingByProvider(CarrierID, "CompanyLogo", serviceprovider)
            Dim personiflylogo As String = "images\Personifly_clear.png"
            Dim logo As String = String.Empty

            If showcarrier Then
                If providerlogo <> "" Then
                    logo = providerlogo
                End If
            Else
                logo = personiflylogo
            End If

            Dim logo2 As String = String.Empty
            If aircraftlogo <> "" Then
                logo2 = aircraftlogo
            End If

            '20131024 - pab - fix duplicate emails
            'Dim emailcc As String = String.Empty
            'If InStr(recipient.ToLower, "rkane@coastalaviationsoftware.com") = 0 Then
            '    emailcc = "rkane@coastalaviationsoftware.com"
            'End If
            'If InStr(recipient.ToLower, "pbaumgart@coastalaviationsoftware.com") = 0 Then
            '    If emailcc = "" Then
            '        emailcc = "pbaumgart@coastalaviationsoftware.com"
            '    Else
            '        emailcc &= "; pbaumgart@coastalaviationsoftware.com"
            '    End If
            'End If
            ''20120113 - pab - send confirmations to David
            'If InStr(subject, "Flight Purchase Confirmation") > 0 Then
            '    If emailcc = "" Then
            '        emailcc = "dhackett@coastalaviationsoftware.com"
            '    Else
            '        emailcc &= "; dhackett@coastalaviationsoftware.com"
            '    End If
            'End If
            ''20120113 - pab - send Provider Notifications to Richard and Paula
            'If InStr(subject, "Provider Notification New Flight Request") > 0 Then
            '    If emailcc = "" Then
            '        emailcc = "rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com"
            '    Else
            '        If InStr(emailcc.ToLower, "rkane@coastalaviationsoftware.com") = 0 And InStr(bcc.ToLower, "rkane@coastalaviationsoftware.com") = 0 And _
            '                InStr(recipient.ToLower, "rkane@coastalaviationsoftware.com") = 0 Then
            '            emailcc &= "; rkane@coastalaviationsoftware.com"
            '        End If
            '        If InStr(emailcc.ToLower, "pbaumgart@coastalaviationsoftware.com") = 0 And InStr(bcc.ToLower, "pbaumgart@coastalaviationsoftware.com") = 0 _
            '                And InStr(recipient.ToLower, "pbaumgart@coastalaviationsoftware.com") = 0 Then
            '            emailcc &= "; pbaumgart@coastalaviationsoftware.com"
            '        End If
            '    End If
            'End If
            If InStr(recipient, "rkane@coastalaviationsoftware.com") = 0 And InStr(recipient, "pbaumgart@coastalaviationsoftware.com") = 0 Then
                If emailcc = "" Then
                    emailcc = "rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com"
                Else
                    emailcc &= "; rkane@coastalaviationsoftware.com; pbaumgart@coastalaviationsoftware.com"
                End If
            ElseIf InStr(recipient, "pbaumgart@coastalaviationsoftware.com") = 0 Then
                If emailcc = "" Then
                    emailcc = "pbaumgart@coastalaviationsoftware.com"
                Else
                    emailcc &= "; pbaumgart@coastalaviationsoftware.com"
                End If
            ElseIf InStr(recipient, "rkane@coastalaviationsoftware.com") = 0 Then
                If emailcc = "" Then
                    emailcc = "rkane@coastalaviationsoftware.com"
                Else
                    emailcc &= "; rkane@coastalaviationsoftware.com"
                End If
            End If

            InsertEmailQueue(CarrierID, sender, recipient, emailcc, bcc, subject, body, isBodyHtml, attachment, logo, logo2, showcarrier)

            SendEmailLogo = True

            'Catch ex As System.Net.Mail.SmtpException
            '    AirTaxi.post_timing(ex.ToString)
            '    Return ex.ToString
        Catch ex As Exception
            Return ex.ToString

            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then
                s &= " - " & ex.InnerException.ToString
            End If
            If Not IsNothing(ex.StackTrace) Then
                s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            End If
            Insertsys_log(CarrierID, appName, s, "SendEmailLogo", "AirTaxi.vb")

        End Try
    End Function

    '20141121 - pab - rewrite quoteworker routine
    Shared Function dtflightstoxml(ByVal dtflights As DataTable) As String

        Dim dr As DataRow
        Dim xmlflights As String = "<dtflights>"

        For n As Integer = 0 To dtflights.Rows.Count - 1
            dr = dtflights.Rows(n)
            xmlflights &= "<row" & n & ">"
            'xmlflights &= "<Origin>" & dr.Item("Origin").ToString & "</Origin>"
            'xmlflights &= "<Departs>" & dr.Item("Departs").ToString & "</Departs>"
            'xmlflights &= "<Destination>" & dr.Item("Destination").ToString & "</Destination>"
            For n2 As Integer = 0 To dtflights.Columns.Count - 1
                xmlflights &= "<" & dtflights.Columns(n2).ColumnName.ToString & ">" & dr.Item(n2).ToString & "</" & dtflights.Columns(n2).ColumnName.ToString & ">"
            Next
            xmlflights &= "</row" & n & ">"
        Next

        xmlflights &= "</dtflights>"

        dtflightstoxml = xmlflights

    End Function

    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function recordquotexml(ByVal dt As DataTable, ByVal origairport As String, ByVal DestAirport As String, ByVal quote As Double,
        ByVal outboundreturn As String, ByVal price As Double, ByVal PriceExplanation As String, ByVal portal As String,
        ByVal email As String, ByVal flightdate As String, ByVal provider As String, ByVal ip As String, ByVal carrierid As Integer) As String


        'Dim portal As String = Session("portal")
        'Dim email As String = Session("email")
        If Trim(email) = "" Then
            email = "not entered"
        End If



        ' Inherits System.Web.UI.Page
        '20100222 - pab - use global shared connection
        'Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset


        '20100222 - pab - use global shared connection
        'If cn.State = 1 Then cn.Close()
        'If cn.State = 0 Then
        '    cn.ConnectionString = connectstring
        '    cn.Open()
        'End If
        If cnsetting.State = 0 Then
            cnsetting.ConnectionString = ConnectionStringHelper.GetCASConnectionStringSQL
            cnsetting.Open()
        End If

        '-- ~DHA commented out to see if record will save... 20100721
        'If rs.State = 1 Then rs.Close()


        Dim req As String

        '20120807 - pab - use dataaccess to insert
        'req = "SELECT * "
        'req = req & "FROM sys_quotes_dt WHERE 1 = 2"

        ''20100222 - pab - use global shared connection
        ''rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        'rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)

        'rs.AddNew()

        'rs.Fields("carrierid").Value = _carrierid
        'rs.Fields("QuoteDateTime").Value = Month(Now) & "/" & Day(Now) & "/" & Year(Now)

        ''rs.Fields("dataxml3").Value = dt.WriteXmlSchema

        'dt.TableName = "SaveQuote"

        'Dim sw As New StringWriter
        'Dim s As String
        'dt.WriteXml(sw, XmlWriteMode.WriteSchema)
        's = sw.ToString
        'rs.Fields("QuoteXML").Value = sw
        'rs.Fields("ModifiedOn").Value = Now
        'rs.Fields("Modifiedby").Value = ""

        'rs.Update()
        'rs.Close()
        'req = "SELECT * "
        'req = req & "FROM sys_quotes_dt "
        'req &= " where carrierid = " & _carrierid & " order by quotenumber desc"

        ''20100222 - pab - use global shared connection
        ''rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        'rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)

        'Dim qn As Integer = 0
        'If Not rs.EOF Then
        '    qn = rs.Fields("quotenumber").Value
        'End If

        'recordquotexml = qn

        dt.TableName = "dtflights"
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        recordquotexml = DataAccess.recordquotexml(dt, origairport, DestAirport, quote, outboundreturn, price, PriceExplanation, portal,
            email, flightdate, provider, ip, carrierid)

    End Function


    '20171030 - pab - run optimizer page
    '20171121 - pab - fix carriers changing midstream - change to Session variables
    Shared Function sendemailtemplate(targetemail As String, subject As String, b As String, carrierid As Integer,
                                      Optional ByVal testrun As Boolean = False)


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

        Dim da As New DataAccess
        Dim carrierlogo As String = da.GetSetting(carrierid, "CompanyLogo")

        If testrun = False Then

            body = body

            Try
                ' Dim ws As New coastalavtech.service.WebService1
                Dim ws As New AviationWebService1_10.WebService1
                ws.SendEmail("pbaumgart@ctgi.com", "123", carrierid, targetemail, "",
                       "optimizer@coastalaviationsoftware.com", subject, body, True, "", carrierlogo, True, "")
            Catch ex As Exception

                ex = ex
            End Try

        End If


    End Function

    '20131223 - pab - add carrier specific quotes for admin portal
    Shared Function Create_dtflights() As DataTable

        Dim dtmodel As New DataTable

        '21020511 - pab - dim below overrode dtflights definition for the class
        'Dim dtflights As New DataTable
        Dim dc As DataColumn

        dc = New DataColumn("ID", System.Type.GetType("System.Int32"))
        dc.AutoIncrement = True
        dc.AutoIncrementSeed = 0
        dc.AutoIncrementStep = 1
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Service Provider", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Origin", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("OriginFacilityName", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Departs", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Destination", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("DestinationFacilityName", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Arrives", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Flight Duration", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        '20141117 - pab - quoteworker routine rewrite
        'dc = New DataColumn("Minutes", System.Type.GetType("System.Int32"))
        dc = New DataColumn("Minutes", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Price", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        ' rk 10/20/2010 add quote editor
        dc = New DataColumn("PriceEdit", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        '20081218 - pab - add additional fees
        dc = New DataColumn("PriceExplanationDetail", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("ShowPriceExplanation", System.Type.GetType("System.Boolean"))
        dtmodel.Columns.Add(dc)

        '20141117 - pab - quoteworker routine rewrite
        'dc = New DataColumn("EmptyLeg", System.Type.GetType("System.Int32"))
        dc = New DataColumn("EmptyLeg", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("AircraftType", System.Type.GetType("System.Int32"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("WeightClass", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("dbname", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("aircraftlogo", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Name", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("FAQPageURL", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        ''20111212 - pab - return table for itinerary
        'dc = New DataColumn("Itinerary", System.Type.GetType("System.String"))
        'dtmodel.Columns.Add(dc)

        '20120125 - pab - add carrier logo
        dc = New DataColumn("carrierlogo", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        '20120403 - pab - add fuel stops, pets, smoking
        dc = New DataColumn("FuelStops", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Pets", System.Type.GetType("System.Boolean"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("Smoking", System.Type.GetType("System.Boolean"))
        dtmodel.Columns.Add(dc)

        '20120525 - pab - add certifications
        dc = New DataColumn("certifications", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        '20120619 - pab - add wifi
        dc = New DataColumn("WiFi", System.Type.GetType("System.Boolean"))
        dtmodel.Columns.Add(dc)

        '20120709 - pab - add lav, power
        dc = New DataColumn("EnclosedLav", System.Type.GetType("System.Boolean"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("PowerAvailable", System.Type.GetType("System.Boolean"))
        dtmodel.Columns.Add(dc)

        '20131118 - pab - add more fields to aircraft
        dc = New DataColumn("InflightEntertainment", System.Type.GetType("System.Boolean"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("ManufactureDate", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        '20140219 - pab - owner confirmation
        dc = New DataColumn("OwnerConfirmation", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        '20140620 - pab - quote from admin portal
        dc = New DataColumn("EmptyLegPricing", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        '20141020 - pab - rewrite quote routine
        dc = New DataColumn("legcode", System.Type.GetType("System.String"))
        dtmodel.Columns.Add(dc)

        dc = New DataColumn("quoteID", System.Type.GetType("System.Int32"))
        dtmodel.Columns.Add(dc)

        ''20130204 - pab - show weight class under aircraft picture
        'dc = New DataColumn("WeightClassTitle", System.Type.GetType("System.String"))
        'dtmodel.Columns.Add(dc)

        Return dtmodel

    End Function

    '20100323 - pab - add airport pax fees
    Shared Function isflightintl(ByVal orig As String, ByVal dest As String) As Boolean

        isflightintl = False
        If orig = "" Or dest = "" Then Exit Function

        Try

            Dim da As New DataAccess
            Dim dt_AirportState As DataTable = da.GetAirportStateByLocationID(orig)
            Dim State As String = ""

            'rk 7.12.2011 handle if no rows returned
            If dt_AirportState.Rows.Count = 0 Then
                isflightintl = True
                Exit Function
            End If

            If Not IsDBNull(dt_AirportState.Rows(0)("Region")) Then
                If dt_AirportState.Rows(0)("Region").ToString.ToUpper.Trim = "INTL" Then
                    isflightintl = True
                    Exit Function
                End If
            End If

            If Not IsDBNull(dt_AirportState.Rows(0)("State")) Then
                State = dt_AirportState.Rows(0)("State").ToString.ToUpper.Trim
                Select Case State
                    Case "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA", "HI", "IA", "ID",
                            "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MS", "MT", "NC",
                            "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD",
                            "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY"
                        isflightintl = False

                    Case Else
                        isflightintl = True
                        Exit Function
                End Select
            End If

            dt_AirportState = da.GetAirportStateByLocationID(dest)


            'rk 7.12.2011 handle if no rows returned
            If dt_AirportState.Rows.Count = 0 Then
                isflightintl = True
                Exit Function
            End If


            If Not IsDBNull(dt_AirportState.Rows(0)("Region")) Then
                If dt_AirportState.Rows(0)("Region").ToString.ToUpper.Trim = "INTL" Then
                    isflightintl = True
                    Exit Function
                End If
            End If

            If Not IsDBNull(dt_AirportState.Rows(0)("State")) Then
                State = dt_AirportState.Rows(0)("State").ToString.ToUpper.Trim
                Select Case State
                    Case "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA", "HI", "IA", "ID",
                            "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MS", "MT", "NC",
                            "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD",
                            "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY"
                        isflightintl = False

                    Case Else
                        isflightintl = True
                        Exit Function
                End Select
            End If

        Catch
            isflightintl = False
            Exit Function
        End Try


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
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            filename = da.GetSetting(carrierid, "CompanyLogo")
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
    Public Shared Function fname(ByVal airportcode As String) As String
        Dim odb As New OptimizerContext
        Dim newfname As New fname_class

        airportcode = Trim(airportcode)

        Dim value As String
        If (fname_dictionary.TryGetValue(airportcode, value)) Then
            Return value
        End If

        If Trim(airportcode) = "" Then
            fname = "?"
            Exit Function
        End If


        If Trim(airportcode) = "PID" Then
            fname = "Nassau"
            Exit Function
        End If

        Dim req As String

        '20110721 - pab - fix when called with icao code
        airportcode = airportcode.ToUpper

        Dim fn As String

        'rk 7.19.17 harden against REDIS errors
        If DateDiff(DateInterval.Minute, redisdown, Now) > 10 Then

            Try
                If IsNothing(AirTaxi.rdc) Then
                    ' AirTaxi.rdc = ConnectionMultiplexer.Connect("pub-redis-11392.dal-05.1.sl.garantiadata.com:11392,password=5acbAlcy3jeeBFzu,ssl=false,abortConnect=False")
                    AirTaxi.rdc = ConnectionMultiplexer.Connect("10.177.209.57:14001,ssl=false,abortConnect=False")
                End If
                If AirTaxi.rdc.IsConnected = False Then
                    'AirTaxi.rdc = ConnectionMultiplexer.Connect("pub-redis-11392.dal-05.1.sl.garantiadata.com:11392,password=5acbAlcy3jeeBFzu,ssl=false,abortConnect=False")
                    AirTaxi.rdc = ConnectionMultiplexer.Connect("10.177.209.10:14001,ssl=false,abortConnect=False")
                End If
                AirTaxi.cache = rdc.GetDatabase

                fn = AirTaxi.cache.StringGet(airportcode)

                If IsNothing(fn) Then fn = ""

                If fn <> "" Then
                    fname_dictionary(airportcode) = fn
                    Return fn
                End If

            Catch
                redisdown = Now
            End Try

        End If

        req = "SELECT icao_id, airport_nm,city,state from airport WHERE icao_id = '" & airportcode & "'"
        newfname = odb.Database.SqlQuery(Of fname_class)(req).First()

        If newfname IsNot Nothing Then
            fname = Trim(newfname.airport_nm) & "  " & Trim(newfname.City) & "," & Trim(newfname.State) & "(" & Trim(airportcode) & ")"
            fname_dictionary(airportcode) = fname
            AirTaxi.cache.StringSet(airportcode, fname)

        Else
            fname = airportcode
        End If

    End Function

    Shared Function normalizemodelrunid(m As String) As String

        'replace the 'R" In the model run id back To role zero.

        Dim ib As String = InBetween(1, m, "-R", "-")
        If ib <> "0" Then
            m = Replace(m, "-R" & ib & "-", "-R0-")
        End If

        If Right(m, 1) <> "C" Then
            If m <> "" Then

                If Mid(m, Len(m) - 1, 1) = "-" Then
                    m = Left(m, Len(m) - 2)
                    GoTo done
                End If

                If Mid(m, Len(m) - 2, 1) = "-" Then
                    m = Left(m, Len(m) - 3)
                    GoTo done
                End If

                If Mid(m, Len(m) - 3, 1) = "-" Then
                    m = Left(m, Len(m) - 4)
                    GoTo done
                End If

            End If
        End If
done:

        Return m
    End Function

    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Public Shared Function lookupac(ac As String, ByVal carrierid As Integer) As String
        Dim odb As New OptimizerContext
        Dim newac As New lookupac_class
        ac = Trim(ac)

        Dim value As String = ""
        If (aclookup_dictionary.TryGetValue(ac, value)) Then
            Return value
        End If

        Dim fn As String
        If DateDiff(DateInterval.Minute, redisdown, Now) > 10 Then

            Try

                If IsNothing(AirTaxi.rdc) Then
                    AirTaxi.rdc = ConnectionMultiplexer.Connect("10.177.209.57:14001,ssl=false,abortConnect=False")
                End If
                If AirTaxi.rdc.IsConnected = False Then
                    AirTaxi.rdc = ConnectionMultiplexer.Connect("10.177.209.10:14001,ssl=false,abortConnect=False")

                End If
                AirTaxi.cache = rdc.GetDatabase

                fn = AirTaxi.cache.StringGet(ac)

                If IsNothing(fn) Then fn = ""

                If fn <> "" Then
                    aclookup_dictionary(ac) = fn
                    Return fn
                End If
            Catch
                redisdown = Now
            End Try
        End If

        'rk 7.19.17 harden against REDIS errors
        aclookup_dictionary.Clear()
        Dim req As String
        Dim lookup As String
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        req = "SELECT Registration,BrokerAircraft,TypeID,HomeBaseAirportCode,VendorName,RTB,FosAircraftID,Operator as ACOperator FROM [OptimizerWest].[dbo].[Aircraft] where [FOSAircraftID] = '" & ac & "' and carrierid = " & carrierid

        newac = odb.Database.SqlQuery(Of lookupac_class)(req).FirstOrDefault()
        If newac IsNot Nothing Then
            If newac.brokeraircraft = "False" Then
                awclookup.TryGetValue(Trim(newac.TypeID), lookup)
                req = Trim(newac.registration) & "  Broker:" & Trim(newac.brokeraircraft) & "  Equip Type:" & Trim(newac.TypeID) & "  Class:" & lookup & "  Operator:" & Trim(newac.ACOperator)
            Else
                fname_dictionary.TryGetValue(Trim(newac.HomeBaseAirportCode), lookup)
                req = Trim(newac.registration) & "  Broker:" & Trim(newac.brokeraircraft) & "  Equip Type:" & Trim(newac.TypeID) & "  HB:" & lookup & "  Vendor:" & Trim(newac.VendorName) & "  RTB:" & Trim(newac.RTB)
            End If
            aclookup_dictionary(ac) = req
            AirTaxi.cache.StringSet(ac, req)
            Return req
            'fname_dictionary.TryGetValue(ac, value)
        End If
        Return ""

    End Function



    '20101105 - pab - add code for aliases
    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function geturlaliasandconnections(ByVal host As String) As ArrayList

        Dim da As New DataAccess
        Dim urlalias As String
        Dim dbstring As String

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        Dim alist As New ArrayList
        Dim carrierid As Integer = 0
        'Dim urlalias As String = ""
        Dim connectstring As String = ""
        Dim connectstringsql As String = ""

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

            '_urlalias = urlalias

            '20120515 - pab - get carrierid too
            '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
            Dim dt As DataTable = da.GetProviderByAlias(urlalias)
            If dt.Rows.Count > 0 Then
                If IsNumeric(dt.Rows(0).Item("carrierid").ToString) Then
                    carrierid = CInt(dt.Rows(0).Item("carrierid").ToString)
                    '_urlalias = urlalias
                Else
                    '20171018 - pab - clear alias and carrierid if not found
                    carrierid = 0
                    urlalias = ""
                    '20130328 - pab - problems addine new carrier - add logging to debug
                    Insertsys_log(carrierid, appName, "carrierid not numeric - host " & host & "; urlalias - " & urlalias & "; dt.Rows(0).Item(""carrierid"").ToString - " & dt.Rows(0).Item("carrierid").ToString, "geturlaliasandconnections", "AirTaxi.vb")
                End If
            Else
                '20171018 - pab - clear alias and carrierid if not found
                carrierid = 0
                urlalias = ""
                '20130328 - pab - problems adding new carrier - add logging to debug
                Insertsys_log(carrierid, appName, "da.GetProviderByAlias(_urlalias) failed - host " & host & "; urlalias - " & urlalias & "; carrierid - " & carrierid, "geturlaliasandconnections", "AirTaxi.vb")
            End If

            '20130109 - pab - fix object not set
            'connectstring = ConnectionStringHelper.GetConnectionString()
            'connectstringsql = ConnectionStringHelper.GetConnectionStringSQL()
            '20130702 - pab - remove duplicate connection strings
            'connectstring = ConnectionStringHelper.GetD2DConnectionString()
            connectstring = ConnectionStringHelper.getglobalconnectionstring(PortalServer)
            connectstringsql = ConnectionStringHelper.getglobalconnectionstring(PortalDriver)

            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            alist.Add(carrierid.ToString)
            alist.Add(urlalias)
            alist.Add(connectstring)
            alist.Add(connectstringsql)

            '20130328 - pab - problems addine new carrier - add logging to debug
            Insertsys_log(carrierid, appName, "host - " & host & "; urlalias - " & urlalias & "; carrierid - " & carrierid, "geturlaliasandconnections", "AirTaxi.vb")

            Return alist

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(carrierid, appName, Left(s, 500), "geturlaliasandconnections", "AirTaxi.vb")
            '20131002 - pab - change email from
            'SendEmail("info@coastalaviationsoftware.com", "rkane@coastalaviationsoftware.com", appName & " AirTaxi.vb geturlaliasandconnections Error", s, 0)
            '20131024 - pab - fix duplicate emails
            'SendEmail("CharterSales@coastalavtech.com", "rkane@coastalaviationsoftware.com", appName & " AirTaxi.vb geturlaliasandconnections Error", s, 0)
            SendEmail("CharterSales@coastalavtech.com", "pbaumgart@coastalaviationsoftware.com", "", appName &
                      " AirTaxi.vb geturlaliasandconnections Error", s, carrierid)
            Return Nothing

        End Try

    End Function

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

    '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
    Shared Function myflightrecs(ByRef userid As String, ByRef carrierid As Integer) As String




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
        req &= " and sfu.carrierid = " & carrierid


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

    Shared Function closesttobase(ByVal CarrierID As Integer, ByVal base1 As String, ByVal base2 As String, ByVal airport As String) As String

        Dim b1, b2 As Double
        Dim oMapping As New Mapping

        b1 = oMapping.GetRoundEarthDistanceBetweenLocations("pbaumgart@ctgi.com", "123", CarrierID, base1, airport)
        b2 = oMapping.GetRoundEarthDistanceBetweenLocations("pbaumgart@ctgi.com", "123", CarrierID, base2, airport)
        If b1 < b2 Then
            closesttobase = base1
        Else
            closesttobase = base2
        End If

    End Function

    '20160225 - pab - quote multi-leg trips
    Shared Function IsBool(ByVal data As String) As Boolean
        Dim result As Boolean = True
        Try
            Boolean.Parse(data)
        Catch generatedExceptionName As FormatException
            result = False
        End Try
        Return result
    End Function

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
