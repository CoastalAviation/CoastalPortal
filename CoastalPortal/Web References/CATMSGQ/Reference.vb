﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
'
Namespace CATMSGQ
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="msgqSoap", [Namespace]:="http://tempuri.org/")>  _
    Partial Public Class msgq
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private PostOperationCompleted As System.Threading.SendOrPostCallback
        
        Private PopOperationCompleted As System.Threading.SendOrPostCallback
        
        Private DeleteOperationCompleted As System.Threading.SendOrPostCallback
        
        Private ReserveOperationCompleted As System.Threading.SendOrPostCallback
        
        Private LISTOperationCompleted As System.Threading.SendOrPostCallback
        
        Private CountOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = Global.CoastalPortal.My.MySettings.Default.CoastalPortal_CATMSGQ_msgq
            If (Me.IsLocalFileSystemWebService(Me.Url) = true) Then
                Me.UseDefaultCredentials = true
                Me.useDefaultCredentialsSetExplicitly = false
            Else
                Me.useDefaultCredentialsSetExplicitly = true
            End If
        End Sub
        
        Public Shadows Property Url() As String
            Get
                Return MyBase.Url
            End Get
            Set
                If (((Me.IsLocalFileSystemWebService(MyBase.Url) = true)  _
                            AndAlso (Me.useDefaultCredentialsSetExplicitly = false))  _
                            AndAlso (Me.IsLocalFileSystemWebService(value) = false)) Then
                    MyBase.UseDefaultCredentials = false
                End If
                MyBase.Url = value
            End Set
        End Property
        
        Public Shadows Property UseDefaultCredentials() As Boolean
            Get
                Return MyBase.UseDefaultCredentials
            End Get
            Set
                MyBase.UseDefaultCredentials = value
                Me.useDefaultCredentialsSetExplicitly = true
            End Set
        End Property
        
        '''<remarks/>
        Public Event PostCompleted As PostCompletedEventHandler
        
        '''<remarks/>
        Public Event PopCompleted As PopCompletedEventHandler
        
        '''<remarks/>
        Public Event DeleteCompleted As DeleteCompletedEventHandler
        
        '''<remarks/>
        Public Event ReserveCompleted As ReserveCompletedEventHandler
        
        '''<remarks/>
        Public Event LISTCompleted As LISTCompletedEventHandler
        
        '''<remarks/>
        Public Event CountCompleted As CountCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Post", RequestNamespace:="http://tempuri.org/", ResponseNamespace:="http://tempuri.org/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Post(ByVal value As String, ByVal queue As String, ByVal visibleuntilXMinutes As Integer, ByVal visibleNotBeforeXMinutes As Integer) As Object
            Dim results() As Object = Me.Invoke("Post", New Object() {value, queue, visibleuntilXMinutes, visibleNotBeforeXMinutes})
            Return CType(results(0),Object)
        End Function
        
        '''<remarks/>
        Public Overloads Sub PostAsync(ByVal value As String, ByVal queue As String, ByVal visibleuntilXMinutes As Integer, ByVal visibleNotBeforeXMinutes As Integer)
            Me.PostAsync(value, queue, visibleuntilXMinutes, visibleNotBeforeXMinutes, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub PostAsync(ByVal value As String, ByVal queue As String, ByVal visibleuntilXMinutes As Integer, ByVal visibleNotBeforeXMinutes As Integer, ByVal userState As Object)
            If (Me.PostOperationCompleted Is Nothing) Then
                Me.PostOperationCompleted = AddressOf Me.OnPostOperationCompleted
            End If
            Me.InvokeAsync("Post", New Object() {value, queue, visibleuntilXMinutes, visibleNotBeforeXMinutes}, Me.PostOperationCompleted, userState)
        End Sub
        
        Private Sub OnPostOperationCompleted(ByVal arg As Object)
            If (Not (Me.PostCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent PostCompleted(Me, New PostCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Pop", RequestNamespace:="http://tempuri.org/", ResponseNamespace:="http://tempuri.org/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Pop(ByVal queue As String, ByVal delete As Boolean) As String
            Dim results() As Object = Me.Invoke("Pop", New Object() {queue, delete})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub PopAsync(ByVal queue As String, ByVal delete As Boolean)
            Me.PopAsync(queue, delete, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub PopAsync(ByVal queue As String, ByVal delete As Boolean, ByVal userState As Object)
            If (Me.PopOperationCompleted Is Nothing) Then
                Me.PopOperationCompleted = AddressOf Me.OnPopOperationCompleted
            End If
            Me.InvokeAsync("Pop", New Object() {queue, delete}, Me.PopOperationCompleted, userState)
        End Sub
        
        Private Sub OnPopOperationCompleted(ByVal arg As Object)
            If (Not (Me.PopCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent PopCompleted(Me, New PopCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Delete", RequestNamespace:="http://tempuri.org/", ResponseNamespace:="http://tempuri.org/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Delete(ByVal queue As String, ByVal keyvalue As String) As String
            Dim results() As Object = Me.Invoke("Delete", New Object() {queue, keyvalue})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub DeleteAsync(ByVal queue As String, ByVal keyvalue As String)
            Me.DeleteAsync(queue, keyvalue, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub DeleteAsync(ByVal queue As String, ByVal keyvalue As String, ByVal userState As Object)
            If (Me.DeleteOperationCompleted Is Nothing) Then
                Me.DeleteOperationCompleted = AddressOf Me.OnDeleteOperationCompleted
            End If
            Me.InvokeAsync("Delete", New Object() {queue, keyvalue}, Me.DeleteOperationCompleted, userState)
        End Sub
        
        Private Sub OnDeleteOperationCompleted(ByVal arg As Object)
            If (Not (Me.DeleteCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent DeleteCompleted(Me, New DeleteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Reserve", RequestNamespace:="http://tempuri.org/", ResponseNamespace:="http://tempuri.org/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Reserve(ByVal queue As String, ByVal seconds As Integer) As String
            Dim results() As Object = Me.Invoke("Reserve", New Object() {queue, seconds})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub ReserveAsync(ByVal queue As String, ByVal seconds As Integer)
            Me.ReserveAsync(queue, seconds, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub ReserveAsync(ByVal queue As String, ByVal seconds As Integer, ByVal userState As Object)
            If (Me.ReserveOperationCompleted Is Nothing) Then
                Me.ReserveOperationCompleted = AddressOf Me.OnReserveOperationCompleted
            End If
            Me.InvokeAsync("Reserve", New Object() {queue, seconds}, Me.ReserveOperationCompleted, userState)
        End Sub
        
        Private Sub OnReserveOperationCompleted(ByVal arg As Object)
            If (Not (Me.ReserveCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent ReserveCompleted(Me, New ReserveCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/LIST", RequestNamespace:="http://tempuri.org/", ResponseNamespace:="http://tempuri.org/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function LIST(ByVal queue As String) As String
            Dim results() As Object = Me.Invoke("LIST", New Object() {queue})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub LISTAsync(ByVal queue As String)
            Me.LISTAsync(queue, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub LISTAsync(ByVal queue As String, ByVal userState As Object)
            If (Me.LISTOperationCompleted Is Nothing) Then
                Me.LISTOperationCompleted = AddressOf Me.OnLISTOperationCompleted
            End If
            Me.InvokeAsync("LIST", New Object() {queue}, Me.LISTOperationCompleted, userState)
        End Sub
        
        Private Sub OnLISTOperationCompleted(ByVal arg As Object)
            If (Not (Me.LISTCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent LISTCompleted(Me, New LISTCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Count", RequestNamespace:="http://tempuri.org/", ResponseNamespace:="http://tempuri.org/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Count(ByVal queue As String) As String
            Dim results() As Object = Me.Invoke("Count", New Object() {queue})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub CountAsync(ByVal queue As String)
            Me.CountAsync(queue, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub CountAsync(ByVal queue As String, ByVal userState As Object)
            If (Me.CountOperationCompleted Is Nothing) Then
                Me.CountOperationCompleted = AddressOf Me.OnCountOperationCompleted
            End If
            Me.InvokeAsync("Count", New Object() {queue}, Me.CountOperationCompleted, userState)
        End Sub
        
        Private Sub OnCountOperationCompleted(ByVal arg As Object)
            If (Not (Me.CountCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent CountCompleted(Me, New CountCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        Public Shadows Sub CancelAsync(ByVal userState As Object)
            MyBase.CancelAsync(userState)
        End Sub
        
        Private Function IsLocalFileSystemWebService(ByVal url As String) As Boolean
            If ((url Is Nothing)  _
                        OrElse (url Is String.Empty)) Then
                Return false
            End If
            Dim wsUri As System.Uri = New System.Uri(url)
            If ((wsUri.Port >= 1024)  _
                        AndAlso (String.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) = 0)) Then
                Return true
            End If
            Return false
        End Function
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")>  _
    Public Delegate Sub PostCompletedEventHandler(ByVal sender As Object, ByVal e As PostCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class PostCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Object
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Object)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")>  _
    Public Delegate Sub PopCompletedEventHandler(ByVal sender As Object, ByVal e As PopCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class PopCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")>  _
    Public Delegate Sub DeleteCompletedEventHandler(ByVal sender As Object, ByVal e As DeleteCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class DeleteCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")>  _
    Public Delegate Sub ReserveCompletedEventHandler(ByVal sender As Object, ByVal e As ReserveCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class ReserveCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")>  _
    Public Delegate Sub LISTCompletedEventHandler(ByVal sender As Object, ByVal e As LISTCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class LISTCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0")>  _
    Public Delegate Sub CountCompletedEventHandler(ByVal sender As Object, ByVal e As CountCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1087.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class CountCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
End Namespace
