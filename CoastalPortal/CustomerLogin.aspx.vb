﻿Imports CoastalPortal.AirTaxi
Imports CoastalPortal.Models

Public Class loginpage
    Inherits System.Web.UI.Page

    '20180313 - pab - fix bug - when Change Details is selected, the list of tails reverts to the last optimizer run not the DC request
    Private DynamicCost As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim url As String = ""
        Dim host As String = ""

        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        If IsNothing(Session("carrierid")) Then Session("carrierid") = 0
        If IsNothing(Session("urlalias")) Then Session("urlalias") = ""
        Dim carrierid As Integer = CInt(Session("carrierid"))
        Dim urlalias As String = Session("urlalias").ToString

        '20180313 - pab - fix bug - when Change Details is selected, the list of tails reverts to the last optimizer run not the DC request
        DynamicCost = Request.QueryString("DynamicCost")

        Insertsys_log(CInt(Session("carrierid")), appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " &
            Request.Url.DnsSafeHost & "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; Request - " &
            Request.Url.ToString, "Page_Load", "CustomerLogin.aspx.vb")

        Try

            '20101105 - pab - add code for aliases
            url = Request.Url.ToString
            host = Request.Url.Host

            '20171114 - pab - redirct to dpj for delta
            If InStr(host.ToLower, "delta") > 0 Then
                Response.Redirect("http://dpj.optimizerpanel.com/CustomerLogin", True)
                Exit Sub
            End If

            If host = "corporateportaluatbeta.cloudapp.net" Or host = "localhost" Then
                '20161227 - pab - default to wheelsup
                'host = "wheelsup"
                'host = "tmcjets"
                'host = "jetlinx"
                host = "dpj"
                'host = "xojet"
                'host = "demoair"
                'host = "instantjet"
                'host = "jetsmarter"
                'host = "hopajet"
                'host = "gajet"
                'host = "mountainaviation"
            End If

            '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
            If Session("urlalias") Is Nothing Then Session("urlalias") = ""

            '20101129 - pab - force last connection to close when starting new session
            If cnsetting.State = 1 Then cnsetting.Close()

            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            'geturlaliasandconnections(host)
            Dim alist As ArrayList = geturlaliasandconnections(host)
            If Not IsNothing(alist) Then
                If alist.Count >= 3 Then
                    Session("carrierid") = CInt(alist.Item(0))
                    'Session("urlalias") = _urlalias
                    Session("urlalias") = alist.Item(1)
                    Session("connectstring") = alist.Item(2)
                    carrierid = Session("carrierid")
                    urlalias = Session("urlalias")
                    connectstring = Session("connectstring")
                End If
            End If

            '20160908 - pab - carrierid getting lost - set as session variable
            'Session("carrierid") = _carrierid

            '20171018 - pab - fix invalid alias
            '20180606 - pab - change url for admin portal
            'If CInt(Session("carrierid")) = 0 Or Session("urlalias").ToString = "" Or InStr(Session("urlalias").ToString.ToLower, "personiflyadmin") > 0 Then
            If CInt(Session("carrierid")) = 0 Or Session("urlalias").ToString = "" Or InStr(Session("urlalias").ToString.ToLower, "avaisearch") > 0 Then
                Session("reset") = "T"
                Insertsys_log(CInt(Session("carrierid")), appName, "carrier not resolved; carrierid - " & CInt(Session("carrierid")) &
                    "; urlalias - " & Session("urlalias").ToString & "; host - " & host & "; connectstring - " & Session("connectstring").ToString, "Page_Load", "Default.aspx.vb")
                Response.Redirect("error_page2.aspx", True)
                Exit Sub
            End If

            Insertsys_log(CInt(Session("carrierid")), appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; Request - " & Request.Url.ToString, "Page_Load", "CustomerLogin.aspx.vb")

            Dim da As New DataAccess
            '20171121 - pab - fix carriers changing midstream - change to Session variables
            'companylogo = Replace(da.GetSetting(CInt(Session("carrierid")), "companylogo").ToLower, "images/", "")

            '20101104 - pab - remove hardcoded value
            Title = da.GetSetting(CInt(Session("carrierid")), "CompanyName") & Title

            '20171127 - pab - keep me loggin in
            If Not IsPostBack Then
                'Check if the browser support cookies
                If Request.Browser.Cookies Then
                    'Check if the cookies with name CASLOGIN exist on user's machine
                    If Request.Cookies("CASLOGIN") IsNot Nothing Then
                        'Pass the user name and password to the VerifyLogin method

                        If Request.Cookies("CASLOGIN")("UNAME").ToString() <> "" Then
                            If Request.Cookies("CASLOGIN")("UNAME") IsNot Nothing Then

                                txtEmail.Text = Request.Cookies("CASLOGIN")("UNAME").ToString()
                                '  Me.txtEmail2.Text = Request.Cookies("CASOPTIMIZERLOGIN")("UNAME").ToString()
                                '  Me.Session("email") = Request.Cookies("CASOPTIMIZERLOGIN")("UNAME").ToString()
                            End If
                        End If

                        If Request.Cookies("CASLOGIN")("UPASS") IsNot Nothing Then

                            If Request.Cookies("CASLOGIN")("UPASS").ToString() <> "" Then
                                '     Me.txtPassword.Text = Request.Cookies("CASOPTIMIZERLOGIN")("UPASS").ToString()
                                Dim p As String
                                p = Request.Cookies("CASLOGIN")("UPASS").ToString()
                                txtPin.Text = p

                            End If

                        End If


                        If Me.txtEmail.Text <> "" Then
                            If Me.txtPin.Text <> "" Then
                                imgLogin_Click(Nothing, Nothing)
                            End If
                        End If



                    End If
                End If
            End If

        Catch ex As Exception
            Dim s As String = "url " & url & "; host " & host & vbNewLine & ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            If ex.Message <> "Thread was being aborted." Then
                Insertsys_log(0, appName, s, "Page_Load", "CustomerLogin.aspx.vb")
                SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " CustomerLogin.aspx Page_Load Error", s, 0)
            End If

        End Try


    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        Try

            '20161227 - pab - dynamic carrier images
            If Not IsPostBack Then
                '20171101 - pab - display cleanup
                'Me.lblCarrier.Text = _urlalias.ToUpper
                Dim da As New DataAccess
                '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
                Dim slogotext As String = da.GetSetting(CInt(Session("carrierid")), "CompanyLogoText")
                '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
                If slogotext = "" Then slogotext = Session("urlalias").ToString & " Flight Schedule Optimization System"
                Me.lblCarrier.Text = slogotext.ToUpper

                Me.imglogo.Src = GetImageURLByATSSID(CInt(Session("carrierid")), 0, "logo")

                '20171114 - pab - remove hardcoded image names
                Dim simage As String = da.GetSetting(CInt(Session("carrierid")), "background-image")
                If simage = "" Then simage = "images/bg2.jpg"
                login.Style.Remove("background-image")
                login.Style.Add("background-image", simage)
                If CInt(Session("carrierid")) = DEMOAIR Then
                    imglogo.Width = 56
                    imglogo.Style.Remove("position")
                    imglogo.Style.Add("position", "absolute;top:16px;lefT:50%;margin:0 0 0 -23px;width:56px;z-index:1;")
                End If

                '20171209 - pab - link to quoting portal
                If CInt(Session("carrierid")) = XOJET Then
                    LnkQuoting.Visible = True
                Else
                    LnkQuoting.Visible = False
                End If

                'Select Case _carrierid
                '    Case 100
                '        login.Style.Remove("background-image")
                '        login.Style.Add("background-image", "images/wheels_up_hero-king-excel.jpg")

                '    '20171007 - pab - jetlinx branding
                '    Case 104
                '        login.Style.Remove("background-image")
                '        login.Style.Add("background-image", "images/jetlinxbackground.jpg")

                '        '20171110 - pab - delta
                '    Case 108
                '        login.Style.Remove("background-image")
                '        login.Style.Add("background-image", "images/delta-denim.jpg")

                '        '20171110 - pab - xo
                '    Case 49
                '        login.Style.Remove("background-image")
                '        login.Style.Add("background-image", "images/XOJET-Plane-denim.jpg")

                '    '20171017 - pab - demoair branding
                '    Case 48
                '        login.Style.Remove("background-image")
                '        login.Style.Add("background-image", "images/bg2.jpg")
                '        imglogo.Width = 56
                '        imglogo.Style.Remove("position")
                '        imglogo.Style.Add("position", "absolute;top:16px;lefT:50%;margin:0 0 0 -23px;width:56px;z-index:1;")

                '    Case 65
                '        login.Style.Remove("background-image")
                '        login.Style.Add("background-image", "images/bg3.jpg")
                '        '20171025 - pab - fix tmc branding
                '        imglogo.Src = "images/logo.png"

                '    Case Else
                '        login.Style.Remove("background-image")
                '        login.Style.Add("background-image", "images/bg2.jpg")

                'End Select
            End If


        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & ex.StackTrace.ToString
            Insertsys_log(0, appName, Left(s, 500), "Page_PreRender", "CustomerLogin.aspx.vb")
            '20131024 - pab - fix duplicate emails
            SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " CustomerLogin.aspx Page_PreRender Error", s, 0)

        End Try

    End Sub

    Protected Sub imgLogin_Click(sender As Object, e As EventArgs) Handles imgLogin.Click
        Dim AWC As New List(Of WeightClass)
        Dim LoginUser As New PortalMember
        Dim mycarrierprofile As New CarrierProfile
        Dim odb As New OptimizerContext
        Dim pdb As New PortalContext


        '20160602 - pab - implement remember me 
        Dim username As String = Trim(txtEmail.Text).ToUpper
        Dim Password As String = Trim(txtPin.Text)

        'Dim ValidationResult As Boolean = ValidateLogin(username, Password)
        'If ValidationResult = True Then

        awclookup.Clear()
            AWC = odb.AircraftWeightClass.ToList()
        For Each x As WeightClass In AWC
            awclookup.Add(Trim(x.AircraftType), Trim(x.AircraftWeightClass))
        Next
        'LoginUser = pdb.Members.FirstOrDefault()
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        Dim _carrierid = 0
        If Not IsNothing(Session("carrierid")) Then _carrierid = CInt(Session("carrierid"))

        LoginUser = pdb.Members.Where(Function(x) Trim(x.Email).ToUpper = username And Trim(x.PIN) = Password And Trim(x.CarrierID) = _carrierid).FirstOrDefault()
        'If Not rs.EOF Then
        If LoginUser IsNot Nothing Then
            If LoginUser.Active Then
                Session("defaultemail") = LoginUser.Email.Trim
                Session("carrierid") = LoginUser.CarrierID
                Session("AcceptAll") = LoginUser.canAcceptAll
                mycarrierprofile = odb.CarrierProfiles.Find(LoginUser.CarrierID)
                Session("Profile") = mycarrierprofile
                Session("LoginUser") = LoginUser
                Session("PlanType") = LoginUser.PlanType
                Session("FirstName") = LoginUser.FirstName
                Session("email") = Me.txtEmail.Text
                Session("Pin") = Me.txtPin.Text
                Session("DateCreated") = LoginUser.DateCreated
                Session("IsLoggedIn") = True
                Session("MemberID") = LoginUser.UserID
                Session("MemberIDCarrier") = If(LoginUser.CarrierID.ToString, 0)
                Session("datecreated") = If(LoginUser.DateCreated.ToString, Now)

                '20171031 - fix missing session variables from validatelogin that was commented out
                Session("userid") = LoginUser.UserID
                Session("username") = LoginUser.Email.Trim
                Session("Active") = True

                '20180624 - pab - add tracking fields to fcdr
                Session("MiddleName") = LoginUser.MiddleName
                Session("LastName") = LoginUser.LastName
                If Len(LoginUser.FirstName.Trim) > 0 Then Session("Initials") = Left(LoginUser.FirstName.Trim, 1).ToUpper
                If Len(LoginUser.MiddleName.Trim) > 0 Then Session("Initials") &= Left(LoginUser.MiddleName.Trim, 1).ToUpper
                If Len(LoginUser.LastName.Trim) > 0 Then Session("Initials") &= Left(LoginUser.LastName.Trim, 1).ToUpper

                Me.txtmsg.Text = "Welcome Back " & LoginUser.FirstName & "." & vbCr & vbLf & vbCr & vbLf & "You are a valued " & LoginUser.PlanType & " level member. " & vbCr & vbLf

                If LoginUser.LastSignIn.ToString() IsNot Nothing Then
                    Me.txtmsg.Text = Me.txtmsg.Text & vbCr & vbLf
                    Me.txtmsg.Text = Me.txtmsg.Text & "Your last sign in was " & LoginUser.LastSignIn & vbCr & vbLf
                End If

                Session("family") = If(LoginUser.family.ToString(), "NO")

                Dim da As New DataAccess
                If LoginUser.UserType = "A" Then
                    'login successful
                    Session("usertype") = LoginUser.UserType
                    Session.Timeout = CInt(da.GetSetting(CInt(Session("carrierid")), "SessionTimeout")) 'rk 10.18.2010 set session timeout as seteting
                Else
                    Session("usertype") = "M"

                End If
                If (LoginUser.DateCreated.ToString Is Nothing) Then LoginUser.DateCreated = Now
                LoginUser.LastSignIn = Now
                pdb.SaveChanges()
            Else
                    Me.txtmsg.Text = ("Your account is no longer active")
                Exit Sub
            End If
        Else
                Me.txtmsg.Text = ("Invalid Email Address or Pin")
            Exit Sub
        End If


        '20171117 - pab - skip home page and go straight to quoting - do not pass go
        'Session("IsLoggedIn") = False
        Dim path As String

        path = Me.Application.Item("path")

        'Session.Abandon()



        'Dim dc As Date
        'If IsDate(rs.Fields("DateCreated").Value) Then dc = rs.Fields("DateCreated").Value

        'Session("DateCreated") = dc

        ''used to check if user logged in throughout this page
        'Session("IsLoggedIn") = True

        'Me.ImgJumpOn.Visible = True

        'Me.txtmsg.Text = "Welcome Back " & rs.Fields("FirstName").Value & "." & vbCr & vbLf & vbCr & vbLf & "You are a valued " & rs.Fields("PlanType").Value & " level member. " & vbCr & vbLf


        'If Not (IsDBNull(rs.Fields("LastSignIn").Value)) Then

        '    Me.txtmsg.Text = Me.txtmsg.Text & vbCr & vbLf

        '    Me.txtmsg.Text = Me.txtmsg.Text & "Your last sign in was " & rs.Fields("LastSignIn").Value & vbCr & vbLf

        'End If

        '    rs.Fields("LastSignIn").Value = Now


        'If Not (IsDBNull(rs.Fields("family").Value)) Then
        '    Session("family") = rs.Fields("family").Value
        'Else
        '    Session("family") = "NO"
        'End If


        'Session("usertype") = "M"

        ''    Me.Menu1.Visible = False

        ''20110207 - pab - change navigation bar to match admin navigation
        ''20160410 - pab - selworthy integration
        ''Me.Navigation1.Visible = False

        'If Not (IsDBNull(rs.Fields("usertype").Value)) Then
        '    Session("usertype") = rs.Fields("usertype").Value

        '    Dim da As New DataAccess
        '    If rs.Fields("usertype").Value = "A" Then
        '        'login successful
        '        Session.Timeout = CInt(da.GetSetting(_carrierid, "SessionTimeout")) 'rk 10.18.2010 set session timeout as seteting

        '        '20101129 - pab - display menu if admin
        '        '20160410 - pab - selworthy integration
        '        'Me.Navigation1.Visible = True

        '    End If



        'Else
        '    Session("usertype") = "M"
        'End If


        'If Not (IsDBNull(rs.Fields("userid").Value)) Then
        '    Session("userid") = rs.Fields("userid").Value
        'Else
        '    Session("userid") = 0
        'End If


        'If Not (IsDBNull(rs.Fields("firstname").Value)) Then
        '    Session("firstname") = rs.Fields("firstname").Value
        '    'Session("username") = rs.Fields("firstname").Value
        '    Session("username") = rs.Fields("email").Value

        'Else
        '    Session("firstname") = ""
        'End If

        '20150821 - pab - force login before getting quotes to track users
        'If Not (IsDBNull(rs.Fields("userID").Value)) Then
        '    Session("MemberID") = rs.Fields("userID").Value
        'Else
        '    Session("MemberID") = 0
        'End If
        'If Not (IsDBNull(rs.Fields("CarrierID").Value)) Then
        '    Session("MemberIDCarrier") = rs.Fields("CarrierID").Value
        'Else
        '    Session("MemberIDCarrier") = 0
        'End If

        'If Session("usertype") = "A" Then
        '    Me.Menu1.Visible = True
        'Else : Me.Menu1.Visible = False
        'End If



        'If Not (IsDBNull(rs.Fields("datecreated").Value)) Then
        '    Session("datecreated") = rs.Fields("datecreated").Value
        'Else
        '    rs.Fields("datecreated").Value = Now
        '    Session("datecreated") = Now
        'End If

        'is user active? if not, cannot use the system
        'Session("Active") = CBool(rs.Fields("Active").Value)

        'Me.txtmsg.Text = Me.txtmsg.Text & vbCr & vbLf

        'Dim myf As String
        'myf = CStr(myflightrecs(Session("email")))

        'Me.txtmsg.Text = Me.txtmsg.Text & myf


        'rs.Update()
        'rs.Close()

        '** rk 9.5.2010 check if pilot
        If (pdb.Database.SqlQuery(Of Integer)("SELECT 1 FROM pilots WHERE pilotemail = '" & LoginUser.Email & "' and carrierid = " & LoginUser.CarrierID).FirstOrDefault() = 1) Then
            Session("isapilot") = "YES"
        End If

        'req = "SELECT * FROM pilots WHERE pilotemail = '" & Me.txtEmail.Text & "' "
        '    req &= " and carrierid = " & _carrierid


        'rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        'If Not rs.EOF Then isapilot = True
        'Session("isapilot") = "YES"
        '20160410 - pab - selworthy integration
        'If isapilot Then Me.PilotSchedule.Visible = True


        'If rs.State = 1 Then rs.Close()

        '20171127 - pab - keep me loggin in
        If Me.chkbox.Checked = True Then
            If txtEmail.Text <> "" Then
                If txtPin.Text <> "" Then
                    If (Request.Browser.Cookies) Then
                        If (Request.Cookies("CASLOGIN") Is Nothing) Then
                            Response.Cookies("CASLOGIN").Expires = DateTime.Now.AddDays(60)


                            Response.Cookies("CASLOGIN").Item("UNAME") = txtEmail.Text
                            'Write password to the cookie
                            Response.Cookies("CASLOGIN").Item("UPASS") = txtPin.Text

                        Else
                            Response.Cookies("CASLOGIN").Expires = DateTime.Now.AddDays(60)


                            Response.Cookies("CASLOGIN").Item("UNAME") = txtEmail.Text
                            'Write password to the cookie
                            Response.Cookies("CASLOGIN").Item("UPASS") = txtPin.Text
                        End If

                    End If
                End If
            End If
        End If
        Session.Timeout = 24 * 60 * 10

        Me.txtmsg.Visible = True

        '20160410 - pab - selworthy integration
        If IsNothing(DynamicCost) Then
            Response.Redirect("RunOptimizer.aspx", True)
        Else
            Response.Redirect("FlightChangeReports?DynamicCost=" & DynamicCost, True)
        End If

    End Sub

    '20160602 - pab - implement remember me 
    Public Function ValidateLogin(ByVal U As String, ByVal P As String) As Boolean

        Dim result As Boolean = False

        Session("IsLoggedIn") = False




        Dim path As String

        path = Me.Application.Item("path")



        'Session.Abandon()


        Session("email") = Me.txtEmail.Text
        Session("Pin") = Me.txtPin.Text


        '** rk 9.5.2010 RK protect against sql injection attack
        If Not (IsSqlSafe(Me.txtEmail.Text) And IsSqlSafe(Me.txtPin.Text)) Then
            Me.txtmsg.Text = ("Invalid Email Address or Pin")
            Return result
            Exit Function
        End If


        ' Inherits System.Web.UI.Page
        '20100222 - pab - use global shared connection
        'Dim cn As New ADODB.Connection
        Dim rs As New ADODB.Recordset


        '20100222 - pab - use global shared connection
        'If cn.State = 1 Then cn.Close()
        'If cn.State = 0 Then
        '    cn.ConnectionString = xonfigurationmanager.ConnectionStrings("PortalConnect").ConnectionString

        '    cn.Open()
        'End If
        If cnsetting.State = 0 Then
            cnsetting.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalDriver)
            cnsetting.Open()
        End If

        If rs.State = 1 Then rs.Close()


        Dim req As String



        req = "SELECT * FROM members WHERE email = '" & Me.txtEmail.Text & "' and pin = '" & Me.txtPin.Text & "'"
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        req &= " and carrierid = " & CInt(Session("carrierid"))



        'rs.Open(req)
        '20100222 - pab - use global shared connection
        'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)




        If rs.EOF Then

            Me.txtmsg.Text = ("Invalid Email Address or Pin")
            Return result
            Exit Function


        End If

        '20110517 - pab - check active flag - do not allow if not active
        If CBool(rs.Fields("active").Value) = False Then
            Me.txtmsg.Text = ("Your account is no longer active")
            rs.CancelUpdate()
            rs.Close()
            Return result
            Exit Function
        End If




        'If Not rs.EOF Then


        'Dim souls As String
        'If IsDBNull(rs.Fields("SoulsOnBoard").Value) Then
        '    souls = 0
        'End If
        'souls = 0

        Session("PlanType") = rs.Fields("PlanType").Value
        Session("FirstName") = rs.Fields("firstname").Value

        Dim dc As Date
        If IsDate(rs.Fields("DateCreated").Value) Then dc = rs.Fields("DateCreated").Value

        Session("DateCreated") = dc

        'used to check if user logged in throughout this page
        Session("IsLoggedIn") = True

        'Me.ImgJumpOn.Visible = True

        Me.txtmsg.Text = "Welcome Back " & rs.Fields("FirstName").Value & "." & vbCr & vbLf & vbCr & vbLf & "You are a valued " & rs.Fields("PlanType").Value & " level member. " & vbCr & vbLf


        If Not (IsDBNull(rs.Fields("LastSignIn").Value)) Then

            Me.txtmsg.Text = Me.txtmsg.Text & vbCr & vbLf

            Me.txtmsg.Text = Me.txtmsg.Text & "Your last sign in was " & rs.Fields("LastSignIn").Value & vbCr & vbLf

        End If

        rs.Fields("LastSignIn").Value = Now


        If Not (IsDBNull(rs.Fields("family").Value)) Then
            Session("family") = rs.Fields("family").Value
        Else
            Session("family") = "NO"
        End If


        Session("usertype") = "M"

        '    Me.Menu1.Visible = False

        '20110207 - pab - change navigation bar to match admin navigation
        '20160410 - pab - selworthy integration
        'Me.Navigation1.Visible = False

        If Not (IsDBNull(rs.Fields("usertype").Value)) Then
            Session("usertype") = rs.Fields("usertype").Value

            Dim da As New DataAccess
            If rs.Fields("usertype").Value = "A" Then
                'login successful
                Session.Timeout = CInt(da.GetSetting(CInt(Session("carrierid")), "SessionTimeout")) 'rk 10.18.2010 set session timeout as seteting

                '20101129 - pab - display menu if admin
                '20160410 - pab - selworthy integration
                'Me.Navigation1.Visible = True

            End If



        Else
            Session("usertype") = "M"
        End If


        If Not (IsDBNull(rs.Fields("userid").Value)) Then
            Session("userid") = rs.Fields("userid").Value
        Else
            Session("userid") = 0
        End If


        If Not (IsDBNull(rs.Fields("firstname").Value)) Then
            Session("firstname") = rs.Fields("firstname").Value
            'Session("username") = rs.Fields("firstname").Value
            Session("username") = rs.Fields("email").Value

        Else
            Session("firstname") = ""
        End If

        '20150821 - pab - force login before getting quotes to track users
        If Not (IsDBNull(rs.Fields("userID").Value)) Then
            Session("MemberID") = rs.Fields("userID").Value
        Else
            Session("MemberID") = 0
        End If
        If Not (IsDBNull(rs.Fields("CarrierID").Value)) Then
            Session("MemberIDCarrier") = rs.Fields("CarrierID").Value
        Else
            Session("MemberIDCarrier") = 0
        End If

        'If Session("usertype") = "A" Then
        '    Me.Menu1.Visible = True
        'Else : Me.Menu1.Visible = False
        'End If



        If Not (IsDBNull(rs.Fields("datecreated").Value)) Then
            Session("datecreated") = rs.Fields("datecreated").Value
        Else
            rs.Fields("datecreated").Value = Now
            Session("datecreated") = Now
        End If

        'is user active? if not, cannot use the system
        Session("Active") = CBool(rs.Fields("Active").Value)

        Me.txtmsg.Text = Me.txtmsg.Text & vbCr & vbLf

        Dim myf As String
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        myf = CStr(myflightrecs(Session("email"), CInt(Session("carrierid"))))

        Me.txtmsg.Text = Me.txtmsg.Text & myf


        rs.Update()
        rs.Close()

        '** rk 9.5.2010 check if pilot
        Dim isapilot As Boolean = False
        req = "SELECT * FROM pilots WHERE pilotemail = '" & Me.txtEmail.Text & "' "
        req &= " and carrierid = " & CInt(Session("carrierid"))
        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        If Not rs.EOF Then isapilot = True
        Session("isapilot") = "YES"
        '20160410 - pab - selworthy integration
        'If isapilot Then Me.PilotSchedule.Visible = True


        If rs.State = 1 Then rs.Close()



        Me.txtmsg.Visible = True

        result = True

        Return result

    End Function

    Protected Sub cmdEmailPin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)





        ' Inherits System.Web.UI.Page
        '20100222 - pab - use global shared connection
        'Dim cn As New ADODB.Connection
        Dim da As New DataAccess
        Dim rs As New ADODB.Recordset


        '20100222 - pab - use global shared connection
        'If cn.State = 1 Then cn.Close()
        'If cn.State = 0 Then
        '    cn.ConnectionString = xonfigurationmanager.ConnectionStrings("PortalConnect").ConnectionString

        '    cn.Open()
        'End If
        If cnsetting.State = 0 Then
            cnsetting.ConnectionString = ConnectionStringHelper.getglobalconnectionstring(PortalDriver)
            cnsetting.Open()
        End If

        If rs.State = 1 Then rs.Close()


        Dim req As String

        req = "SELECT * FROM members WHERE email = '" & Me.txtEmail.Text & "'"
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        req &= " and carrierid = " & CInt(Session("carrierid"))




        'rs.Open(req)
        '20100222 - pab - use global shared connection
        'rs.Open(req, cn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)
        rs.Open(req, cnsetting, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockReadOnly)




        If rs.EOF Then

            Me.txtmsg.Text = ("Invalid Email Address")
            Exit Sub


        End If




        If Not rs.EOF Then



            Me.txtmsg.Text = "Welcome " & rs.Fields("FirstName").Value & "." & vbCr & vbLf & vbCr & vbLf & "You are a valued " & rs.Fields("PlanType").Value & " level member. " & vbCr & vbLf


            If Not (IsDBNull(rs.Fields("pin").Value)) Then
                Session("pin") = rs.Fields("pin").Value
            Else
                Session("pin") = "unavailable"
            End If


        End If


        'send email
        'Dim cea As New EmailAgent

        Dim emailSubject As String = da.GetSetting(CInt(Session("carrierid")), "CompanyName") & " is sending you your pin"

        Dim emailBody As String = ""

        emailBody = "Hello," & ControlChars.CrLf & "Your pin for the " & da.GetSetting(CInt(Session("carrierid")), "CompanyName") & " service is: " & Session("pin")

        ' cea.SendEmail("info@coastalaviationsoftware.com", "", Me.txtEmail.Text.Trim, emailSubject, emailBody)

        '20120807 - pab - write to email queue
        '20131024 - pab - fix duplicate emails
        SendEmail("info@coastalaviationsoftware.com", Me.txtEmail.Text.Trim, "", emailSubject, emailBody, CInt(Session("carrierid")))


        Me.txtmsg.Text = ("Your pin has been emailed to you.")


    End Sub

End Class