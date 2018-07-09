Imports CoastalPortal.AirTaxi

Public Class _Default
    Inherits Page

    Private _navDest As Enumerations.NavigationDestinationTypes

    Private _userID As Integer = 0

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim da As New DataAccess

        If Not Session("Token") Is Nothing Then
            _userID = CInt(Session("UserID"))
        End If

        '20101105 - pab - add code for aliases
        Dim host As String = Request.Url.Host
        'If host = "localhost" Or InStr(host, "127.0.0.") > 0 Then
        '    Response.Redirect("SelectAlias.aspx")
        '    Exit Sub
        'End If
        If Session("carrierid") Is Nothing Then
            '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
            Insertsys_log(0, appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString &
                "; Session(carrierid) - null", "Page_Load", "Default.aspx.vb")
        Else
            Insertsys_log(CInt(Session("carrierid")), appName, "AbsoluteUri - " & Request.Url.AbsoluteUri & "; DnsSafeHost - " & Request.Url.DnsSafeHost &
                "; Host - " & Request.Url.Host & "; Query - " & Request.Url.Query & "; ToString - " & Request.Url.ToString &
                "; Session(carrierid) - " & Session("carrierid").ToString, "Page_Load", "Default.aspx.vb")
        End If

        If Left(host.ToLower, 4) = "www." Then
            host = Mid(host.ToLower, 5)
        End If

        If host = "corporateportaluatbeta.cloudapp.net" Or host = "corporateportaluat.cloudapp.net" Or host = "localhost" Then
            '20161227 - pab - default to wheelsup
            'host = "tmcjets"
            'host = "wheelsup"
            host = "jetlinx"
            'host = "demoair"
            'host = "xojet"
            'host = "dpj"
            'host = "instantjet"
            'host = "jetsmarter"
            'host = "hopajet"
        End If

        '20130109 - pab - fix changing carrier
        'If _urlalias Is Nothing Then
        '20171115 - pab - fix carriers changing midstream - change _carrierid to Session("carrierid")
        'geturlaliasandconnections(host)
        Dim alist As ArrayList = geturlaliasandconnections(host)
        If Not IsNothing(alist) Then
            If alist.Count >= 3 Then
                Session("carrierid") = CInt(alist.Item(0))
                'Session("urlalias") = _urlalias
                Session("urlalias") = alist.Item(1)
                Session("connectstring") = alist.Item(2)
            End If
        End If

        '20160908 - pab - carrierid getting lost - set as session variable
        'Session("carrierid") = _carrierid

        '20130328 - pab - problems addine new carrier - add logging to debug
        Insertsys_log(CInt(Session("carrierid")), appName, "host - " & host & "; Session(carrierid) - " & Session("carrierid").ToString, "Page_Load", "Default.aspx.vb")

        Session("reset") = "T"

        '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
        '20180606 - pab - change url for admin portal
        'If CInt(Session("carrierid")) = 0 Or Session("urlalias").ToString = "" Or InStr(Session("urlalias").ToString.ToLower, "personiflyadmin") > 0 Then
        If CInt(Session("carrierid")) = 0 Or Session("urlalias").ToString = "" Or InStr(Session("urlalias").ToString.ToLower, "avaisearch") > 0 Then
            '20130702 - pab - insert into log - don't sent email
            'SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "carrier not resolved", appName & " carrierid - " & _carrierid & "; _urlalias - " & _urlalias & "; host - " & host & "; connectstring - " & connectstring & vbNewLine, _carrierid)
            Insertsys_log(CInt(Session("carrierid")), appName, "carrier not resolved; carrierid - " & CInt(Session("carrierid")) &
                "; _urlalias - " & Session("urlalias").ToString & "; host - " & host & "; connectstring - " & Session("connectstring").ToString, "Page_Load", "Default.aspx.vb")
            Response.Redirect("error_page2.aspx", True)
            Exit Sub
        End If

        '    'Dim oQuote As New SaveQuoteInfo

        '    'oQuote.QuoteInfoArray("urlalias", _urlalias)
        '    'oQuote.QuoteInfoArray("connectstring", connectstring)
        '    'oQuote.QuoteInfoArray("ConnectionStringHelper.GetCASConnectionStringSQL", ConnectionStringHelper.GetCASConnectionStringSQL)
        '    'oQuote.InsertQuote()

        'End If

        '20120524 - pab - log alias info
        If Not IsPostBack Then
            Insertsys_log(CInt(Session("carrierid")), appName, "host - " & host & "; carrierid - " & CInt(Session("carrierid")) &
                "; _urlalias - " & Session("urlalias").ToString & "; connectstring - " & Session("connectstring").ToString, "Page_Load", "Default.aspx.vb ")
        End If

        '20160408 - pab - redirect to login page if not logged on
        Dim bIsLoggedIn As Boolean = False
        Dim email As String = String.Empty
        If Not IsNothing(Session("IsLoggedIn")) Then bIsLoggedIn = CBool(Session("IsLoggedIn"))
        If Not IsNothing(Session("email")) Then email = Session("email").ToString
        If bIsLoggedIn = False Or email = "" Then
            Response.Redirect("CustomerLogin.aspx", True)
        End If

        da.UpdateUserLastActivity(CInt(Session("carrierid")), _userID, Request.Url.PathAndQuery, Request.UserHostAddress)

        Dim ContentUserControl As UserControl = Nothing

        'check querystring to determine which control to show
        '20120518 - pab - fix error - Object reference not set to an instance of an object
        If IsNothing(Request.QueryString) Then
            If Not Request.QueryString("navDest") Is Nothing Then
                _navDest = CType(Request.QueryString("navDest"), Enumerations.NavigationDestinationTypes)
            Else 'default to home
                _navDest = Enumerations.NavigationDestinationTypes.Home
            End If
        Else 'default to home
            _navDest = Enumerations.NavigationDestinationTypes.Home
        End If

        'clear out any currently loaded user controls in pnlUserControl
        pnlUserControl.Controls.Clear()

        '//make sure ssl is used
        'if (_navDest == Enumerations.NavigationDestinationTypes.Change_Password ||
        '        _navDest == Enumerations.NavigationDestinationTypes.Login ||
        '        _navDest == Enumerations.NavigationDestinationTypes.User_Information ||
        '        _navDest == Enumerations.NavigationDestinationTypes.Credit_Card_Transaction ||
        '        _navDest == Enumerations.NavigationDestinationTypes.Confirmation)
        '{
        '    //need ssl
        '    if (Request.Url.AbsoluteUri.ToUpper().Substring(0, 8) != "HTTPS://")
        '    {
        '        Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://").Replace("HTTP://", "HTTPS://"));
        '    }
        '}
        'else //do not need ssl
        '{
        '    if (Request.Url.AbsoluteUri.ToUpper().Substring(0, 8) == "HTTPS://")
        '    {
        '        Response.Redirect(Request.Url.AbsoluteUri.Replace("https://", "http://").Replace("HTTPS://", "HTTP://"));
        '    }
        '}


        Select Case _navDest
            Case Enumerations.NavigationDestinationTypes.Home
                '20171115 - pab - fix carriers changing midstream - change _urlalias to Session("urlalias")
                Insertsys_log(CInt(Session("carrierid")), appName, "_navDest - Home; Request.Url.Host - " & Request.Url.Host & "; Request.Url.Host - " &
                    Request.Url.Host & "; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString, "Page_Load", "Default.aspx.vb ")
                'Response.Redirect("http://www.flyPortal.com")
                '20101104 - pab - let it default to selectairports.aspx
                'Response.Redirect("http://fly-fast.com/Portal/selectairports.aspx")
                Response.Redirect("home.aspx", True)
                Exit Select
            Case Enumerations.NavigationDestinationTypes.Change_Password

                Insertsys_log(CInt(Session("carrierid")), appName, "_navDest - Change_Password; Request.Url.Host - " & Request.Url.Host & "; Request.Url.Host - " &
                    Request.Url.Host & "; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString, "Page_Load", "Default.aspx.vb ")
                ContentUserControl = CType(LoadControl("~/Controls/ChangePassword.ascx"), UserControl)
                ContentUserControl.ID = "ChangePassword1"
                pnlUserControl.Controls.Add(ContentUserControl)

                Exit Select
            Case Enumerations.NavigationDestinationTypes.User_Information

                Insertsys_log(CInt(Session("carrierid")), appName, "_navDest - User_Information; Request.Url.Host - " & Request.Url.Host & "; Request.Url.Host - " &
                    Request.Url.Host & "; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString, "Page_Load", "Default.aspx.vb ")
                ContentUserControl = CType(LoadControl("~/Controls/UserInformation.ascx"), UserControl)
                ContentUserControl.ID = "UserInformation1"
                pnlUserControl.Controls.Add(ContentUserControl)

                Exit Select
            Case Enumerations.NavigationDestinationTypes.Book_Flight

                Insertsys_log(CInt(Session("carrierid")), appName, "_navDest - Book_Flight; Request.Url.Host - " & Request.Url.Host & "; Request.Url.Host - " &
                    Request.Url.Host & "; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString, "Page_Load", "Default.aspx.vb ")
                ContentUserControl = CType(LoadControl("~/Controls/BookFlight.ascx"), UserControl)
                ContentUserControl.ID = "BookFlight1"
                pnlUserControl.Controls.Add(ContentUserControl)

                Exit Select
            Case Enumerations.NavigationDestinationTypes.Confirmation

                Insertsys_log(CInt(Session("carrierid")), appName, "_navDest - Confirmation; Request.Url.Host - " & Request.Url.Host & "; Request.Url.Host - " &
                    Request.Url.Host & "; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString, "Page_Load", "Default.aspx.vb ")
                Response.Redirect("Confirmation.aspx", True)

                Exit Select
            Case Enumerations.NavigationDestinationTypes.Login

                Insertsys_log(CInt(Session("carrierid")), appName, "_navDest - Login; Request.Url.Host - " & Request.Url.Host & "; Request.Url.Host - " &
                    Request.Url.Host & "; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString, "Page_Load", "Default.aspx.vb ")
                Response.Redirect("CustomerLogin.aspx", True)

                Exit Select
            Case Enumerations.NavigationDestinationTypes.Logout

                Session.Abandon()

                FormsAuthentication.SignOut()

                Insertsys_log(CInt(Session("carrierid")), appName, "_navDest - Logout; Request.Url.Host - " & Request.Url.Host & "; Request.Url.Host - " &
                    Request.Url.Host & "; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString, "Page_Load", "Default.aspx.vb ")
                Response.Redirect("Default.aspx", True)

                Exit Select
            Case Else
                Insertsys_log(CInt(Session("carrierid")), appName, "_navDest - else; Request.Url.Host - " & Request.Url.Host & "; Request.Url.Host - " &
                    Request.Url.Host & "; carrierid - " & CInt(Session("carrierid")) & "; _urlalias - " & Session("urlalias").ToString, "Page_Load", "Default.aspx.vb ")
                'Response.Redirect("http://www.flyPortal.com")
                '20101104 - pab - let it default to selectairports.aspx
                'Response.Redirect("http://fly-fast.com/Portal/selectairports.aspx")
                Response.Redirect("Default.aspx", True)
        End Select
    End Sub

End Class