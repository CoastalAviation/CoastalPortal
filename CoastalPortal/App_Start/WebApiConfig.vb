Imports System
Imports System.Collections.Generic
Imports System.Net.Http.Formatting
Imports System.Linq
Imports Newtonsoft.Json.Serialization
Imports System.Web.Http
Imports System.Web.Http.OData.Builder
Imports System.Web.Http.OData.Extensions
Public Class WebApiConfig
    Public Shared Sub Register(ByVal config As HttpConfiguration)
        ' Web API configuration and services
        Dim builder As New ODataConventionModelBuilder
        config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel())
        builder.EntitySet(Of RejectedFlight)("RejectFlight")

        ' Web API routes
        config.MapHttpAttributeRoutes()

        'config.Routes.MapHttpRoute(
        'name:="ApiWithActionid",
        'routeTemplate:="api/{controller}/{action}/{id}")

        '  config.Routes.MapHttpRoute(
        'name:="ApiActionnoid",
        'routeTemplate:="api/{controller}/{action}"
        '    )

        config.Routes.MapHttpRoute(
        name:="DefaultApi",
        routeTemplate:="api/{controller}/{id}",
        defaults:=New With {.id = RouteParameter.Optional}
         )

        Dim jsonFormatter = New JsonMediaTypeFormatter
        jsonFormatter.SerializerSettings.ContractResolver = New CamelCasePropertyNamesContractResolver

    End Sub
End Class