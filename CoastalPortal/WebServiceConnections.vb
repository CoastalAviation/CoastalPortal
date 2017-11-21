Option Strict On
Option Explicit On

Imports Microsoft.VisualBasic
'Imports MapPointService
'Imports OptimizationService
Imports CoastalPortal.AviationWebService1_10
Imports System.Web.Services.Protocols
Imports System.Net

Public Class WebServiceConnections

    'Public Shared renderService As MapPointService.RenderServiceSoap
    'Public Shared findService As MapPointService.FindServiceSoap
    'Public Shared routeService As MapPointService.RouteServiceSoap

    'Public Shared ctgiOptimizationService As OptimizationService.OptimizationService
    '20110929 - pab - fix web reference
    Public Shared ctgiOptimizationService As CoastalPortal.AviationWebService1_10.WebService1

    'credentials for MapPoint API staging environment
    Private Const MPUser As String = "137607" ' "128095"
    Private Const MPPass As String = "AFafD35423!121" '"c1rrU5!!"

    Public Shared Sub InstantiateMapPointServices()
        ' Create and set the logon information (note comment in web.config -- here would be the place to
        ' decrypt/unhash the user/password from the config file).
        'NEW - Revised configuration settings (add ref to System.Configuration first):

        Dim ourCredentials As New NetworkCredential(MPUser, MPPass)

        ' Create the render service, pointing at the correct location
        'renderService = New MapPointService.RenderServiceSoap()
        'renderService.Credentials = ourCredentials
        'renderService.PreAuthenticate = True

        '' Create the find service, pointing at the correct location
        'findService = New MapPointService.FindServiceSoap()
        '' set the logon information
        'findService.Credentials = ourCredentials
        'findService.PreAuthenticate = True

        ' Create the route service, pointing at the correct location
        '    routeService = New MapPointService.RouteServiceSoap()
        '    routeService.Credentials = ourCredentials
        '    routeService.PreAuthenticate = True
    End Sub

    Public Shared Sub InstantiateCTGiOptimizationService()
        'ctgiOptimizationService = New OptimizationService.OptimizationService
        '20110929 - pab - fix web reference
        ctgiOptimizationService = New AviationWebService1_10.WebService1
    End Sub

End Class
