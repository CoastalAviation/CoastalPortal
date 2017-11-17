Option Strict On
Option Explicit On
Imports System.Net
Imports CoastalPortal.AirTaxi
Imports CoastalPortal.GeoCode

'20161031 - pab - drive time
Imports System.IO

Public Class Mapping
    ' <summary>
    ' These are the actual instances of the objects that call the MapPoint .NET service
    ' </summary>

    '20120424 - pab - convert from mappoint to bing
    'Private renderService As MapPointService.RenderServiceSoap
    'Private findService As MapPointService.FindServiceSoap
    'Private routeService As MapPointService.RouteServiceSoap

    '20110926 - pab - update web services
    'Private ctgiOptimizationService As optimizationservice.OptimizationService
    '20110929 - pab - fix web reference
    'Private ctgiOptimizationService As AviationService1_8.Service
    '20120201 - pab - changes for azure
    'Private ctgiOptimizationService As AviationService1_9.Service
    '20120423 - pab - call azure web service
    'Private ctgiOptimizationService As AviationService1_10.Service
    Private ctgiOptimizationService As AviationWebService1_10.WebService1

    Public Sub New()
        InstantiateServices()
    End Sub

    Private Sub InstantiateServices()

        '20120424 - pab - convert from mappoint to bing
        'only instantiate the seb service one time as needed
        'If WebServiceConnections.findService Is Nothing Or _
        '    WebServiceConnections.renderService Is Nothing Or _
        '    WebServiceConnections.routeService Is Nothing Then

        '    WebServiceConnections.InstantiateMapPointServices()

        'End If

        If WebServiceConnections.ctgiOptimizationService Is Nothing Then
            WebServiceConnections.InstantiateCTGiOptimizationService()
        End If

        '20120424 - pab - convert from mappoint to bing
        'renderService = WebServiceConnections.renderService
        'findService = WebServiceConnections.findService
        'routeService = WebServiceConnections.routeService

        '  ctgiOptimizationService = WebServiceConnections.ctgiOptimizationService

        '' Create and set the logon information (note comment in web.config -- here would be the place to
        '' decrypt/unhash the user/password from the config file).
        ''NEW - Revised configuration settings (add ref to System.Configuration first):

        'Dim ourCredentials As New NetworkCredential(MPUser, MPPass)

        '' Create the render service, pointing at the correct location
        'renderService = New MapPointService.RenderServiceSoap()
        'renderService.Credentials = ourCredentials
        'renderService.PreAuthenticate = True

        '' Create the find service, pointing at the correct location
        'findService = New MapPointService.FindServiceSoap()
        '' set the logon information
        'findService.Credentials = ourCredentials
        'findService.PreAuthenticate = True

        '' Create the route service, pointing at the correct location
        'routeService = New MapPointService.RouteServiceSoap()
        'routeService.Credentials = ourCredentials
        'routeService.PreAuthenticate = True
    End Sub

    '20161031 - pab - drive time
    '20161101 - pab - travel time too high per David
    Shared Function GetDriveTime(ByVal latlongfrom As LatLong, ByVal latlongto As LatLong, ByVal aptto As String) As Integer

        GetDriveTime = 0

        '20161206 - pab - new key
        'Dim key = "AkBlRJjqHmKJ4X5SusXQ19twofZeiN-Ffkcm418hGRWvR1GiSTlkYibs141Kn_ke"
        '20170428 - pab - move to ibm 
        Dim key = "AgedsgIDWN_Qft7HltrU1gR7k8WFUxvJZwU_mQmlq9WFrZaMQrsXdYAmYA2y2GG0"
        'Dim key = "AgjTMzpOHLCsmzHnjg5nna0HcMgPx4GeKSe1UXoKIoqBZmwWLqHXGzy19Nl2ZpqK"

        'https://msdn.microsoft.com/en-us/library/gg636957.aspx
        Dim uri As String = ""

        Try

            '20161101 - pab - travel time too high per David
            If aptto <> "" Then
                uri = "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.1=" & latlongfrom.Latitude & "," & latlongfrom.Longitude &
                "&wp.2=" & aptto & "&output=xml&key=" & key
            Else
                uri = "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.1=" & latlongfrom.Latitude & "," & latlongfrom.Longitude &
                "&wp.2=" & latlongto.Latitude & "," & latlongto.Longitude & "&output=xml&key=" & key
            End If

            'https://msdn.microsoft.com/en-us/library/456dfw4f(v=vs.110).aspx
            Dim request As WebRequest = WebRequest.Create(uri)
            Dim response As WebResponse = request.GetResponse()
            Dim dataStream As System.IO.Stream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()

            '20161101 - pab - use regular travel time per David
            'If InStr(responseFromServer, "<TravelDurationTraffic>") > 0 Then
            If InStr(responseFromServer, "<TravelDuration>") > 0 Then
                GetDriveTime = CInt(Math.Round(CInt(InBetween(1, responseFromServer, "<TravelDuration>", "</TravelDuration>")) / 60))
            End If

            reader.Close()
            response.Close()

            Return GetDriveTime

        Catch ex As Exception

            'The remote server returned an error: (404) Not Found.
            Return 99999999

        End Try

    End Function

    'chg3631 - 20100929 - pab - add code to handle icao code lookup
    Public Function GetIATAcodebyICAO(ByVal ICAO As String) As DataSet

        Dim da As New DataAccess
        Return da.GetIATAcodebyICAO(ICAO)

    End Function

    ''' <summary>
    ''' Returns the geocode coordinates of an address. 
    ''' </summary>
    ''' <param name="addressLine">The address</param>
    ''' <param name="city">The city</param>
    ''' <param name="state">The state</param>
    ''' <param name="postalCode">The postal/zip code</param>
    ''' <param name="country">The country. e.g.: USA, Canada</param>
    ''' 

    '20120424 - pab - convert from mappoint to bing
    'Public Function GeocodeAddress(ByVal addressLine As String, ByVal city As String, _
    '    ByVal state As String, ByVal postalCode As String, ByVal country As String) _
    '        As LATLONG
    '    ' Set up the address
    '    Dim address As Address = New Address()
    '    address.AddressLine = addressLine
    '    address.PrimaryCity = city
    '    address.PostalCode = postalCode
    '    address.Subdivision = state
    '    address.CountryRegion = country

    '    ' Set up the specification for the address
    '    ' Set up the specification object.
    '    Dim findAddressSpec As FindAddressSpecification = New FindAddressSpecification()
    '    findAddressSpec.InputAddress = address

    '    '    If country = "United States of America" Then
    '    findAddressSpec.DataSourceName = "MapPoint.NA" ' More info: http://msdn2.microsoft.com/en-us/library/ms982198.aspx and http://msdn2.microsoft.com/en-us/library/aa493004.aspx
    '    'Else
    '    ' findAddressSpec.DataSourceName = "MapPoint.World"
    '    'End If

    '    ' Set the find options. Allow more return values by decreasing
    '    ' the value of the ThresholdScore option.
    '    ' Also, limit the number of results returned to 20.
    '    Dim myFindOptions As FindOptions = New FindOptions()
    '    myFindOptions.ThresholdScore = 0.5
    '    myFindOptions.Range = New FindRange()
    '    myFindOptions.Range.StartIndex = 0
    '    myFindOptions.Range.Count = 20
    '    findAddressSpec.Options = myFindOptions

    '    ' Create a FindResults object to store the results of the FindAddress request.
    '    Dim myFindResults As FindResults
    '    Dim latLong As LATLONG = New LATLONG()

    '    Try
    '        ' Get the results and return them if there are any. 
    '        myFindResults = findService.FindAddress(findAddressSpec)
    '        'Dim myResults() As FindResult = myFindResults.Results()
    '        Dim myResults As FindResult = myFindResults.Results
    '        If Not myResults Is Nothing Then
    '            If myResults.Length > 0 Then latLong = myResults(0).FoundLocation.LatLong
    '        End If

    '    Catch myException As SoapException
    '        ' Your exception handling process goes here.
    '        AirTaxi.post_timing(myException.ToString)
    '    End Try

    '    Return latLong
    'End Function


    '20120424 - pab - convert from mappoint to bing
    'Public Function GeocodeAddress2(ByVal addressLine As String) As LATLONG




    '    'Use ParseAddress to parse the user string first, and then find 
    '    'the address
    '    ' Dim findService As New FindServiceSoap()
    '    Dim myAddress As Address
    '    myAddress = findService.ParseAddress(addressLine, "USA")

    '    Dim findAddressSpec As New FindAddressSpecification()
    '    '20100920 - pab - comment out redundant code - mappoint called below
    '    'findAddressSpec.InputAddress = myAddress
    '    'findAddressSpec.DataSourceName = "MapPoint.NA"

    '    'Declare the return variable and find the address
    '    ' Dim findAddressResults As FindResults
    '    'findAddressResults = findService.FindAddress(findAddressSpec)


    '    '' Set up the address
    '    'Dim address As Address = New Address()
    '    'address.AddressLine = addressLine
    '    'address.PrimaryCity = city
    '    'address.PostalCode = postalCode
    '    'address.Subdivision = state
    '    'address.CountryRegion = country

    '    ' Set up the specification for the address
    '    ' Set up the specification object.
    '    ' Dim findAddressSpec As FindAddressSpecification = New FindAddressSpecification()
    '    findAddressSpec.InputAddress = myAddress

    '    '    If country = "United States of America" Then
    '    findAddressSpec.DataSourceName = "MapPoint.NA" ' More info: http://msdn2.microsoft.com/en-us/library/ms982198.aspx and http://msdn2.microsoft.com/en-us/library/aa493004.aspx
    '    'Else
    '    ' findAddressSpec.DataSourceName = "MapPoint.World"
    '    'End If

    '    ' Set the find options. Allow more return values by decreasing
    '    ' the value of the ThresholdScore option.
    '    ' Also, limit the number of results returned to 20.
    '    Dim myFindOptions As FindOptions = New FindOptions()
    '    myFindOptions.ThresholdScore = 0.5
    '    myFindOptions.Range = New FindRange()
    '    myFindOptions.Range.StartIndex = 0
    '    myFindOptions.Range.Count = 20
    '    findAddressSpec.Options = myFindOptions

    '    ' Create a FindResults object to store the results of the FindAddress request.
    '    Dim myFindResults As FindResults
    '    Dim latLong As LATLONG = New LATLONG()

    '    Try
    '        ' Get the results and return them if there are any. 
    '        myFindResults = findService.FindAddress(findAddressSpec)
    '        Dim myResults() As FindResult = myFindResults.Results
    '        If Not myResults Is Nothing Then
    '            If myResults.Length > 0 Then latLong = myResults(0).FoundLocation.LatLong
    '        End If

    '    Catch myException As SoapException
    '        ' Your exception handling process goes here.
    '        AirTaxi.post_timing(myException.ToString)
    '    End Try

    '    Return latLong
    'End Function

    Public Function GeoCodeText(ByVal place As String) As LatLong

        Dim inputstring As String
        '20170402 - pab - fix error with trailing blanks
        inputstring = place.ToString.Trim

        'count commas
        Dim commacount, i As Integer
        For i = 1 To Len(inputstring)
            If Mid(inputstring, i, 1) = "," Then commacount = commacount + 1
        Next


        'rk 10.4.2011 use the new bing map geocoding regardless of number of commas :)
        GeoCodeText = geocodebing(inputstring)
        'If commacount < 2 Then
        '    GeoCodeText = GeocodePlace(inputstring)
        'Else
        '    GeoCodeText = GeocodeAddress2(inputstring)

        'End If






    End Function

    Public Function geocodebing(ByVal addressLine As String) As LatLong

        Dim latLong As LatLong = New LatLong()

        '20111230 - pab - fix error if address line blank - Either Query or Address must be specified
        If addressLine = "" Then Return latLong

        'rk 10.4.2011 use new bing map geocodeing before move from microsoft
        'click here for development details http://msdn.microsoft.com/en-us/library/dd221354.aspx



        '  Try
        ' Set a Bing Maps key before making a request
        '20150824 - pab - switch to azure bing 
        'Dim key = "Aih8QrwW_xFB2gv6lm8gvxPo8IVvdwAXrIakTPaxSc5o2PDcxdJzfG5GdUzRVdnO"
        '20161206 - pab - new key
        'Dim key = "AkBlRJjqHmKJ4X5SusXQ19twofZeiN-Ffkcm418hGRWvR1GiSTlkYibs141Kn_ke"
        '20170428 - pab - move to ibm 
        Dim key = "AgedsgIDWN_Qft7HltrU1gR7k8WFUxvJZwU_mQmlq9WFrZaMQrsXdYAmYA2y2GG0"
        'Dim key = "AgjTMzpOHLCsmzHnjg5nna0HcMgPx4GeKSe1UXoKIoqBZmwWLqHXGzy19Nl2ZpqK"

        '20170109 - pab - call geocode rest services instead of web services
        Try
            latLong = RESTgeocode.geocoderest.getgeocoderest(addressLine, key)
            Return latLong

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " Mapping.vb geocodebing error", s, _carrierid)
            AirTaxi.Insertsys_log(_carrierid, AirTaxi.appName, s, "Mapping.aspx", "geocodebing")

            Return Nothing

        End Try





        Dim geocodeRequest As New GeoCode.GeocodeRequest

        ' Set the credentials using a valid Bing Maps Key
        geocodeRequest.Credentials =
        New GeoCode.Credentials() With {.ApplicationId = key}

        ' Set the full address query
        geocodeRequest.Query = addressLine

        ' Set the options to only return high confidence results
        Dim filters() As GeoCode.ConfidenceFilter =
        {New GeoCode.ConfidenceFilter() _
        With {.MinimumConfidence = GeoCode.Confidence.Low}}

        Dim geocodeOptions As New GeoCode.GeocodeOptions() _
        With {.filters = filters}

        geocodeRequest.Options = geocodeOptions




        ' Make the geocode request
        '  Dim geocodeServiceInstance As New GeoCodeClient("BasicHttpBinding_IGeocodeService")

        Try

            Dim ws As New GeoCode.GeocodeService
            Dim geocodresponse As New GeoCode.GeocodeResponse

            geocodresponse = ws.Geocode(geocodeRequest)

            If geocodresponse.Results.Length > 0 Then
                latLong.Longitude = geocodresponse.Results(0).Locations(0).Longitude
                latLong.Latitude = geocodresponse.Results(0).Locations(0).Latitude
            End If


            'If geocodresponse.Results.Length > 0 Then
            '    Results = geocodeResponse.Results(0).Locations(0).Longitude.ToString
            'Else
            '    Results = "No Results Found"
            'End If


            ' Use the results in your application.
            '   Results = geocodresponse.Results(0).ToString
            Return latLong

            ' Catch ex As Exception
            '  Results = "An exception occurred: " & ex.Message

            'End Try

        Catch ex As Exception
            Dim s As String = ex.Message
            If Not IsNothing(ex.InnerException) Then s &= vbNewLine & vbNewLine & ex.InnerException.ToString
            If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
            '20120807 - pab - write to email queue
            '20131024 - pab - fix duplicate emails
            SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", appName & " Mapping.vb geocodebing error", s, _carrierid)
            AirTaxi.Insertsys_log(_carrierid, AirTaxi.appName, s, "Mapping.aspx", "geocodebing")

            Return Nothing

        End Try

    End Function





    '20120424 - pab - convert from mappoint to bing
    'Public Function GeocodePlace(ByVal addressLine As String) As LATLONG


    '    'Output the returned find results
    '    ' Dim findService As New FindServiceSoap()
    '    Dim findSpec As New FindSpecification()
    '    findSpec.DataSourceName = "MapPoint.World"
    '    findSpec.InputPlace = addressLine

    '    'Dim foundResults As FindResults
    '    'foundResults = findService.Find(findSpec)




    '    ' Set the find options. Allow more return values by decreasing
    '    ' the value of the ThresholdScore option.
    '    ' Also, limit the number of results returned to 20.
    '    Dim myFindOptions As FindOptions = New FindOptions()
    '    myFindOptions.ThresholdScore = 0.5
    '    myFindOptions.Range = New FindRange()
    '    myFindOptions.Range.StartIndex = 0
    '    myFindOptions.Range.Count = 20
    '    ' myFindOptions.ThresholdScore 
    '    findSpec.Options = myFindOptions

    '    ' Create a FindResults object to store the results of the FindAddress request.
    '    Dim myFindResults As FindResults
    '    Dim latLong As LATLONG = New LATLONG()

    '    'chg3620 - 20100920 - pab - add GetGeocodePlaceOverride for places like freeport, bahamas that return the wrong lat/long
    '    Dim da As New DataAccess
    '    Dim ds As New DataSet
    '    ds = da.GetGeocodePlaceOverride(UCase(addressLine))
    '    If ds.Tables(0).Rows.Count > 0 Then
    '        latLong.Latitude = Math.Round(CDbl(ds.Tables(0).Rows(0)("latitude")), 13)
    '        latLong.Longitude = Math.Round(CDbl(ds.Tables(0).Rows(0)("longitude")), 13)

    '    Else
    '        Try
    '            ' Get the results and return them if there are any. 
    '            myFindResults = findService.Find(findSpec)
    '            Dim myResults() As FindResult = myFindResults.Results
    '            If Not myResults Is Nothing Then
    '                If myResults.Length > 0 Then latLong = myResults(0).FoundLocation.LatLong
    '            End If

    '        Catch myException As SoapException
    '            ' Your exception handling process goes here.
    '            AirTaxi.post_timing(myException.ToString)
    '        End Try
    '    End If

    '    Return latLong
    'End Function







    '20120424 - pab - convert from mappoint to bing
    'Public Function FindAddressByLatLong(ByVal latitude As Double, ByVal longitude As Double) As String

    '    Dim sb As New StringBuilder

    '    'Output possible locations for a user at a specific latitude and longitude coordinate
    '    Dim myLatLong As New LATLONG()
    '    myLatLong.Latitude = latitude
    '    myLatLong.Longitude = longitude

    '    Dim returnedLocations() As Location
    '    'returnedLocations = findService.GetLocationInfo(myLatLong, "MapPoint.NA", Nothing)
    '    returnedLocations = findService.GetLocationInfo(myLatLong, "MapPoint.World", Nothing)
    '    'returnedLocations = findService.GetLocationInfo(myLatLong, "MapPoint.EU", Nothing)

    '    Dim i As Integer
    '    For i = 0 To returnedLocations.Length - 1
    '        'AirTaxi.post_timing(returnedLocations(i).Entity.DisplayName)
    '        sb.Append(returnedLocations(i).Entity.DisplayName)
    '    Next
    '    Return sb.ToString
    'End Function

    ''' <summary>
    ''' Returns points of interest based on the datasource and filter used
    ''' </summary>
    ''' <param name="addressLine"></param>
    ''' <param name="city"></param>
    ''' <param name="state"></param>
    ''' <param name="postalCode"></param>
    ''' <param name="country"></param>
    ''' <param name="distance"></param>
    ''' <param name="count"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    '20120424 - pab - convert from mappoint to bing
    'Public Function FindNearby(ByVal addressLine As String, ByVal city As String, _
    '    ByVal state As String, ByVal postalCode As String, ByVal country As String, _
    '    ByVal distance As Double, ByVal count As Integer, ByVal findWhat As String) _
    '        As StringBuilder

    '    Dim sb As New StringBuilder

    '    ' Set up the address
    '    Dim address As Address = New Address()
    '    address.AddressLine = addressLine
    '    address.PrimaryCity = city
    '    address.PostalCode = postalCode
    '    address.Subdivision = state
    '    address.CountryRegion = country

    '    'Set up the find options
    '    Dim myFindOptions As New FindOptions
    '    myFindOptions.ThresholdScore = 0

    '    ' Set up the specification object
    '    Dim findAddressSpec As New FindAddressSpecification
    '    findAddressSpec.Options = myFindOptions

    '    If country = "United States of America" Then
    '        findAddressSpec.DataSourceName = "MapPoint.NA" ' More info: http://msdn2.microsoft.com/en-us/library/ms982198.aspx and http://msdn2.microsoft.com/en-us/library/aa493004.aspx
    '    Else
    '        findAddressSpec.DataSourceName = "MapPoint.World"
    '    End If

    '    findAddressSpec.InputAddress = address

    '    'Create FindResults object to store the results of the FindAddress request
    '    Dim myFindResults As FindResults

    '    Try

    '        myFindResults = findService.FindAddress(findAddressSpec)

    '        If myFindResults.NumberFound = 0 Then
    '            'If there are no results from the address find, then just tell the user
    '            Dim myListItem As New System.Web.UI.WebControls.ListItem
    '            myListItem.Text = "No addresses matches found."

    '        Else
    '            'Set up the FindNearby options
    '            Dim myFindNearbyOptions As New FindOptions
    '            myFindNearbyOptions.Range = New FindRange
    '            myFindNearbyOptions.Range.Count = count

    '            'Set up the FindNearby specification object
    '            Dim findNearbySpec As New FindNearbySpecification
    '            findNearbySpec.Options = myFindNearbyOptions

    '            If country = "United States of America" Then
    '                findAddressSpec.DataSourceName = "MapPoint.NA" ' More info: http://msdn2.microsoft.com/en-us/library/ms982198.aspx and http://msdn2.microsoft.com/en-us/library/aa493004.aspx
    '            Else
    '                findAddressSpec.DataSourceName = "MapPoint.World"
    '            End If

    '            findNearbySpec.Distance = distance
    '            findNearbySpec.LatLong = myFindResults.Results(0).FoundLocation.LatLong
    '            findNearbySpec.Filter = New FindFilter

    '            Select Case findWhat.ToUpper
    '                Case "LIMOUSINE"
    '                    findNearbySpec.Filter.EntityTypeName = "NAICS485320" 'Limo
    '                Case "TAXI"
    '                    findNearbySpec.Filter.EntityTypeName = "NAICS485310" 'Taxi
    '                Case "HOTEL"
    '                    findNearbySpec.Filter.EntityTypeName = "NAICS721110" 'Hotel
    '                Case "CAR_RENTAL"
    '                    findNearbySpec.Filter.EntityTypeName = "NAICS532111" 'Car Rental
    '            End Select

    '            'findNearbySpec.Filter.EntityTypeName = "NAICS48811" 'Airports, Flying Fields, and Services

    '            findNearbySpec.Filter.PropertyNames = New String() {"DisplayName"}

    '            'Create a FindResults object to store the results of the FindNearby request
    '            Dim myFindNearbyResults As FindResults

    '            myFindNearbyResults = findService.FindNearby(findNearbySpec)

    '            'Now use the found address and the FindNearby results to render a map.
    '            'Output the results of the FindNearby search as well.

    '            'Create an array of Location and Pushpin objects, setting upperbound via the upperbound of the FindNearby results,
    '            'adding one array element for the original address.  Draw the map to encompass all these points.

    '            Dim myLocations(myFindNearbyResults.Results.Length) As Location
    '            Dim myPushPins(myFindNearbyResults.Results.Length) As Pushpin

    '            'Declare necessary variables
    '            Dim i As Integer

    '            'Add FindNearby results to the list box and fill the pushpin and location arrays
    '            For i = 0 To myLocations.Length - 2
    '                myLocations(i) = New Location
    '                myLocations(i).LatLong = myFindNearbyResults.Results(i).FoundLocation.LatLong

    '                'get route info for each returned result
    '                Dim routeInfo As MapPointService.Route = GetRoute(findNearbySpec.LatLong.Latitude, _
    '                                        findNearbySpec.LatLong.Longitude, _
    '                                        myLocations(i).LatLong.LATITUDE, _
    '                                        myLocations(i).LatLong.Longitude)

    '                sb.Append(myFindNearbyResults.Results(i).FoundLocation.Entity.DisplayName & _
    '                    " " & Math.Round(routeInfo.Itinerary.Distance, 1).ToString & " mi</br>")
    '            Next i
    '        End If

    '        Return sb
    '    Catch myException As SoapException
    '        ' Your exception handling process goes here.
    '        AirTaxi.post_timing(myException.ToString)

    '        ''Catch any errors, output them if they are user errors (MapPoint exceptions)
    '        ''and assert if they are not

    '        'If (myException.Detail("Type").InnerText = "MapPointArgumentException") Then
    '        '    'If it's a user error, output it, otherwise assert
    '        '    Dim myListItem As New System.Web.UI.WebControls.ListItem
    '        '    myListItem.Text = myException.Message
    '        '    listboxFindResults.Items.Add(myListItem)
    '        'Else
    '        '    'Your exception handling goes here
    '        'End If
    '        Return Nothing
    '    End Try
    'End Function

    '20120424 - pab - convert from mappoint to bing
    'Public Function FindNearby(ByVal latitude As Double, ByVal longitude As Double, _
    '    ByVal distance As Double, ByVal count As Integer, ByVal findWhat As String) _
    '        As Generic.SortedList(Of Integer, Generic.SortedList(Of FindResult, MapPointService.Route))

    '    Dim latLong As New LATLONG
    '    latLong.Latitude = latitude
    '    latLong.Longitude = longitude

    '    Try

    '        'Set up the FindNearby options
    '        Dim myFindNearbyOptions As New FindOptions
    '        myFindNearbyOptions.Range = New FindRange
    '        myFindNearbyOptions.Range.Count = count

    '        'Set up the FindNearby specification object
    '        Dim findNearbySpec As New FindNearbySpecification
    '        findNearbySpec.Options = myFindNearbyOptions
    '        findNearbySpec.DataSourceName = "MapPointNAICS.World" '"MapPointNAICS.NA"
    '        findNearbySpec.Distance = distance
    '        findNearbySpec.LatLong = latLong
    '        findNearbySpec.Filter = New FindFilter

    '        Select Case findWhat.ToUpper
    '            Case "LIMOUSINE"
    '                findNearbySpec.Filter.EntityTypeName = "NAICS485320" 'Limo
    '            Case "TAXI"
    '                findNearbySpec.Filter.EntityTypeName = "NAICS485310" 'Taxi
    '            Case "HOTEL"
    '                findNearbySpec.Filter.EntityTypeName = "NAICS721110" 'Hotel
    '            Case "CAR_RENTAL"
    '                findNearbySpec.Filter.EntityTypeName = "NAICS532111" 'Car Rental
    '        End Select

    '        'findNearbySpec.Filter.EntityTypeName = "NAICS48811" 'Airports, Flying Fields, and Services

    '        findNearbySpec.Filter.PropertyNames = New String() {"DisplayName"}

    '        'Create a FindResults object to store the results of the FindNearby request
    '        Dim myFindNearbyResults As FindResults

    '        myFindNearbyResults = findService.FindNearby(findNearbySpec)

    '        'Now use the found address and the FindNearby results to render a map.
    '        'Output the results of the FindNearby search as well.

    '        'Create an array of Location and Pushpin objects, setting upperbound via the upperbound of the FindNearby results,
    '        'adding one array element for the original address.  Draw the map to encompass all these points.

    '        Dim myLocations(myFindNearbyResults.Results.Length) As Location
    '        Dim myPushPins(myFindNearbyResults.Results.Length) As Pushpin

    '        'Declare necessary variables
    '        Dim i As Integer

    '        Dim routeArray As New Generic.SortedList(Of Integer, Generic.SortedList(Of FindResult, MapPointService.Route))

    '        'Add FindNearby results to the list box and fill the pushpin and location arrays
    '        For i = 0 To myLocations.Length - 2
    '            myLocations(i) = New Location
    '            myLocations(i).LatLong = myFindNearbyResults.Results(i).FoundLocation.LatLong

    '            'get route info for each returned result
    '            Dim routeInfo As MapPointService.Route = GetRoute(findNearbySpec.LatLong.Latitude, _
    '                                    findNearbySpec.LatLong.Longitude, _
    '                                    myLocations(i).LatLong.LATITUDE, _
    '                                    myLocations(i).LatLong.Longitude)

    '            Dim mapInfo As New SortedList(Of FindResult, MapPointService.Route)
    '            mapInfo.Add(myFindNearbyResults.Results(i), routeInfo)

    '            routeArray.Add(i, mapInfo)

    '            'sb.Append(myFindNearbyResults.Results(i).FoundLocation.Entity.DisplayName & _
    '            '    " " & Math.Round(routeInfo.Itinerary.Distance, 1).ToString & " mi</br>")
    '        Next i

    '        Return routeArray
    '    Catch myException As SoapException
    '        ' Your exception handling process goes here.
    '        AirTaxi.post_timing(myException.ToString)

    '        ''Catch any errors, output them if they are user errors (MapPoint exceptions)
    '        ''and assert if they are not

    '        'If (myException.Detail("Type").InnerText = "MapPointArgumentException") Then
    '        '    'If it's a user error, output it, otherwise assert
    '        '    Dim myListItem As New System.Web.UI.WebControls.ListItem
    '        '    myListItem.Text = myException.Message
    '        '    listboxFindResults.Items.Add(myListItem)
    '        'Else
    '        '    'Your exception handling goes here
    '        'End If
    '        Return Nothing
    '    End Try
    'End Function

    ''' <summary>
    '''  Returns a dataset containing airport info
    ''' </summary>
    ''' <param name="latitude"></param>
    ''' <param name="longitude"></param>
    ''' <param name="minimumRunwayLength">runway length in feet</param>
    ''' <param name="miles">radius to search</param>
    ''' <param name="count">max number of airports to return</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindNearbyAirports(ByVal latitude As Double, ByVal longitude As Double,
        ByVal minimumRunwayLength As Integer, ByVal miles As Integer,
        ByVal count As Integer) As DataSet

        '20090612 - pab - use local call instead of web service
        'Return ctgiOptimizationService.GetNearbyAirportsByLatitudeLongitudeWithinDistance(latitude, longitude, _
        '    minimumRunwayLength, miles, count)
        Dim da As New DataAccess
        Return da.GetNearestAirportsByLatitudeLongitudeWithinDistance(latitude, longitude,
                        minimumRunwayLength, miles, count)
    End Function

    Public Function GetAirportInformationByAirportCode(ByVal airportCode As String) As DataSet

        '20090612 - pab - use local call instead of web service
        'Return ctgiOptimizationService.GetAirportInformationByAirportCode(airportCode)
        Dim da As New DataAccess
        Return da.GetAirportInformationByAirportCode(airportCode)

    End Function

    'Public Function GetAllAirportsInState(ByVal state As String, ByVal minimumRunwayLength As Integer) As DataSet
    Public Function GetAllAirportsInState(ByVal userid As String, ByVal pswd As String, ByVal state As String, ByVal minimumRunwayLength As Integer) As DataSet

        Return ctgiOptimizationService.GetAllAirportsInState(userid, pswd, _carrierid, state, minimumRunwayLength)

    End Function


    'Public Function GetRegionalAirportsByCarriers(ByVal latitude As Double, ByVal longitude As Double, _
    'ByVal minimumRunwayLength As Integer, ByVal miles As Integer, _
    'ByVal count As Integer) As DataSet
    Public Function GetRegionalAirportsByCarriers(ByVal userid As String, ByVal pswd As String, ByVal latitude As Double, ByVal longitude As Double,
        ByVal minimumRunwayLength As Integer, ByVal miles As Integer,
        ByVal count As Integer) As String

        Return ctgiOptimizationService.GetRegionalAirportsByCarriers(userid, pswd, _carrierid, latitude.ToString, longitude.ToString,
            minimumRunwayLength.ToString, miles.ToString, count.ToString)

    End Function

    Public Function GetRoundEarthDistanceBetweenLocations(ByVal userid As String, ByVal pswd As String, ByVal carrierid As Integer, ByVal originLocationID As String,
        ByVal destinationLocationID As String) As Double

        '20100219 - pab - fix error - Object reference not set to an instance of an object
        'Return ctgiOptimizationService.GetRoundEarthDistanceBetweenLocations(originLocationID, destinationLocationID)

        '20110926 - pab - update web services
        'Dim ws As New WebRole1.AviationWebService1_10.WebService1
        '20110929 - pab - fix web reference
        'Dim ws As New AviationService1_8.Service
        '20120201 - pab - changes for azure
        'Dim ws As New AviationService1_9.Service
        '20120423 - pab - call azure web service
        'Dim ws As New AviationService1_10.Service
        Dim ws As New AviationWebService1_10.WebService1

        'chg3640 - 20101014 - pab - fix error - Object reference not set to an instance of an object
        If originLocationID = "" Or destinationLocationID = "" Then
            Return 0
        Else
            Dim da As New DataAccess
            Dim dt As DataTable = da.GetAirportInformationByLocationID(originLocationID)
            If isdtnullorempty(dt) Then
                Return 0
            End If
            dt = da.GetAirportInformationByLocationID(destinationLocationID)
            If isdtnullorempty(dt) Then
                Return 0
            End If
            Return ws.GetRoundEarthDistanceBetweenLocations(userid, pswd, _carrierid, originLocationID, destinationLocationID)
        End If

    End Function

    '20120424 - pab - convert from mappoint to bing
    'Public Function GetRoute(ByVal originLat As Double, ByVal originLong As Double, _
    '    ByVal destLat As Double, ByVal destLong As Double) As MapPointService.Route


    '    Return Nothing


    '    AirTaxi.post_timing("get route start  " & Now.ToString)



    '    Dim latLong As New LATLONG

    '    Dim latLongs(1) As LATLONG

    '    latLong.Latitude = originLat
    '    latLong.Longitude = originLong

    '    latLongs(0) = latLong

    '    latLong = New LATLONG

    '    latLong.Latitude = destLat
    '    latLong.Longitude = destLong

    '    latLongs(1) = latLong

    '    Dim myRoute As MapPointService.Route = Nothing


    '    AirTaxi.post_timing("try route   " & Now.ToString)
    '    Try
    '        myRoute = routeService.CalculateSimpleRoute(latLongs, "MapPoint.NA", _
    '                                    SegmentPreference.Quickest)
    '        AirTaxi.post_timing("try route complete  " & Now.ToString)
    '        'myRoute = routeService.CalculateSimpleRoute(latLongs, "MapPoint.World", _
    '        '                           SegmentPreference.Quickest)


    '        'myRoute = routeService.CalculateSimpleRoute(latLongs, "MapPoint.EU", _
    '        '                            SegmentPreference.Quickest)
    '    Catch myException As SoapException
    '        ' Your exception handling process goes here.
    '        AirTaxi.post_timing("route exception  " & Now.ToString)
    '        AirTaxi.post_timing(myException.ToString)

    '        ''Catch any errors, output them if they are user errors (MapPoint exceptions)
    '        ''and assert if they are not

    '        'If (myException.Detail("Type").InnerText = "MapPointArgumentException") Then
    '        '    'If it's a user error, output it, otherwise assert
    '        '    Dim myListItem As New System.Web.UI.WebControls.ListItem
    '        '    myListItem.Text = myException.Message
    '        '    listboxFindResults.Items.Add(myListItem)
    '        'Else
    '        '    'Your exception handling goes here
    '        'End If
    '        AirTaxi.post_timing("get route end nothing  " & Now.ToString)

    '        Return Nothing
    '    End Try

    '    Return myRoute

    '    AirTaxi.post_timing("get route end  " & Now.ToString)


    'End Function




End Class
