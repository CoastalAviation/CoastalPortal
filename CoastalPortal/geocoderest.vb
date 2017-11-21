Imports System
Imports System.Xml
Imports System.Xml.XPath
Imports System.Net

Namespace RESTgeocode
    Public Class geocoderest

        'https://msdn.microsoft.com/en-us/library/hh534080.aspx

        '20170428 - pab - move to ibm 
        'Shared BingMapsKey As String =  "AgedsgIDWN_Qft7HltrU1gR7k8WFUxvJZwU_mQmlq9WFrZaMQrsXdYAmYA2y2GG0" 
        'Shared BingMapsKey As String =  "AgjTMzpOHLCsmzHnjg5nna0HcMgPx4GeKSe1UXoKIoqBZmwWLqHXGzy19Nl2ZpqK" 

        Shared Function getgeocoderest(ByVal args As String, ByVal BingMapsKey As String) As AirTaxi.LatLong
            Try

                '20170623 - pab - fix special characters
                If InStr(args, "!") > 0 Then args = Replace(args, "#", "%21")
                If InStr(args, "#") > 0 Then args = Replace(args, "#", "%23")
                If InStr(args, "$") > 0 Then args = Replace(args, "$", "%24")
                If InStr(args, "&") > 0 Then args = Replace(args, "&", "%26")
                If InStr(args, "'") > 0 Then args = Replace(args, "'", "%27")
                If InStr(args, "(") > 0 Then args = Replace(args, "(", "%28")
                If InStr(args, ")") > 0 Then args = Replace(args, ")", "%29")
                If InStr(args, "*") > 0 Then args = Replace(args, "*", "%2A")
                If InStr(args, "+") > 0 Then args = Replace(args, "+", "%2B")
                If InStr(args, ",") > 0 Then args = Replace(args, ",", "%2C")
                If InStr(args, "/") > 0 Then args = Replace(args, "/", "%2F")
                If InStr(args, ":") > 0 Then args = Replace(args, ":", "%3A")
                If InStr(args, ";") > 0 Then args = Replace(args, ";", "%3B")
                If InStr(args, "=") > 0 Then args = Replace(args, "=", "%3D")
                If InStr(args, "?") > 0 Then args = Replace(args, "?", "%3F")
                If InStr(args, "@") > 0 Then args = Replace(args, "@", "%40")
                If InStr(args, "[") > 0 Then args = Replace(args, "[", "%5B")
                If InStr(args, "]") > 0 Then args = Replace(args, "]", "%5D")

                'Create the REST Services 'Find by Query' request
                Dim locationsRequest As String = CreateRequest(args, BingMapsKey)
                Dim locationsResponse As XmlDocument = MakeRequest(locationsRequest)
                Dim latlong As New AirTaxi.LatLong
                '20170623 - pab - fix object not set
                If Not IsNothing(locationsResponse) Then
                    latlong = ProcessResponse(locationsResponse)
                End If
                Return latlong

            Catch ex As Exception
                Dim s As String = ex.Message
                If Not IsNothing(ex.InnerException) Then s &= vbNewLine & vbNewLine & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName &
                    " geocoderest.vb getgeocoderest error", s, 0)
                AirTaxi.Insertsys_log(0, AirTaxi.appName, s, "geocoderest.aspx", "getgeocoderest")
                Return Nothing
            End Try

        End Function

        'Create the request URL
        Public Shared Function CreateRequest(ByVal queryString As String, ByVal BingMapsKey As String) As String

            Dim UrlRequest As String = "http://dev.virtualearth.net/REST/v1/Locations/" &
                                            queryString &
                                            "?output=xml" &
                                            " &key=" & BingMapsKey
            Return (UrlRequest)

        End Function

        'Submit the HTTP Request and return the XML response
        Public Shared Function MakeRequest(ByVal requestUrl As String) As XmlDocument

            Try
                Dim request As HttpWebRequest = CType(WebRequest.Create(requestUrl), HttpWebRequest)
                Dim response As HttpWebResponse = request.GetResponse

                Dim xmlDoc As XmlDocument = New XmlDocument()
                xmlDoc.Load(response.GetResponseStream())
                Return (xmlDoc)

            Catch ex As Exception
                Dim s As String = ex.Message
                If Not IsNothing(ex.InnerException) Then s &= vbNewLine & vbNewLine & ex.InnerException.ToString
                If Not IsNothing(ex.StackTrace) Then s &= vbNewLine & vbNewLine & ex.StackTrace.ToString
                AirTaxi.SendEmail("info@coastalaviationsoftware.com", "pbaumgart@coastalaviationsoftware.com", "", AirTaxi.appName &
                    " geocoderest.vb MakeRequest error", s, 0)
                AirTaxi.Insertsys_log(0, AirTaxi.appName, s, "geocoderest.aspx", "MakeRequest")
                Return Nothing
            End Try
        End Function

        Shared Function ProcessResponse(ByVal locationsResponse As XmlDocument) As AirTaxi.LatLong

            'Create namespace manager
            Dim nsmgr As XmlNamespaceManager = New XmlNamespaceManager(locationsResponse.NameTable)
            nsmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1")

            ''Get formatted addresses: Option 1
            ''Get all locations in the response and then extract the formatted address for each location
            Dim locationElements As XmlNodeList = locationsResponse.SelectNodes("//rest:Location", nsmgr)
            'Console.WriteLine("Show all formatted addresses: Option 1")
            'Dim location As XmlNode
            'For Each location In locationElements
            '    Console.WriteLine(location.SelectSingleNode(".//rest:FormattedAddress", nsmgr).InnerText)
            'Next
            'Console.WriteLine()

            ''Get formatted addresses: Option 2
            ''Get all formatted addresses directly. This works because there is only one formatted address for each location.
            'Dim formattedAddressElements As XmlNodeList =  locationsResponse.SelectNodes("//rest:FormattedAddress",nsmgr) 
            'Console.WriteLine("Show all formatted addresses: Option 2")
            'Dim formattedAddress As XmlNode
            'For Each formattedAddress In formattedAddressElements
            '    Console.WriteLine(formattedAddress.InnerText)
            'Next
            'Console.WriteLine()

            'Get the Geocode Points to use for display for each Location
            Dim locationElementsForGP As XmlNodeList = locationsResponse.SelectNodes("//rest:Location", nsmgr)
            Console.WriteLine("Show Goeocode Point Data")
            Dim location As XmlNode
            Dim s As String = ""
            Dim latlong As New AirTaxi.LatLong
            For Each location In locationElements
                Dim displayGeocodePoints As XmlNodeList = location.SelectNodes(".//rest:GeocodePoint/rest:UsageType[.='Display']/parent::node()", nsmgr)
                'Console.Write(location.SelectSingleNode(".//rest:FormattedAddress", nsmgr).InnerText)
                'Console.WriteLine(" has " + displayGeocodePoints.Count.ToString() + " display geocode point(s).")
                If IsNumeric(location.SelectSingleNode(".//rest:Latitude", nsmgr).InnerText) Then latlong.Latitude = CDbl(location.SelectSingleNode(".//rest:Latitude", nsmgr).InnerText)
                If IsNumeric(location.SelectSingleNode(".//rest:Longitude", nsmgr).InnerText) Then latlong.Longitude = CDbl(location.SelectSingleNode(".//rest:Longitude", nsmgr).InnerText)
                Exit For
            Next
            'Console.WriteLine()

            ''Get all locations that have a MatchCode=Good and Confidence=High
            'Dim matchCodeGoodElements As XmlNodeList =  locationsResponse.SelectNodes("//rest:Location/rest:MatchCode[.='Good']/parent::node()",nsmgr) 
            'Console.WriteLine("Show all addresses with MatchCode=Good and Confidence=High")
            ''Dim location As XmlNode
            'For Each location In matchCodeGoodElements
            '    If location.SelectSingleNode(".//rest:Confidence",nsmgr).InnerText = "High" Then
            '        Console.WriteLine(location.SelectSingleNode(".//rest:FormattedAddress", nsmgr).InnerText)
            '    End If
            'Next

            'Console.WriteLine("Press any key to exit")
            'Console.ReadKey()

            Return latlong

        End Function

    End Class

End Namespace
