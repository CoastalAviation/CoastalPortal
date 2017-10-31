Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel.DataAnnotations
Imports CoastalPortal.AirTaxi
Imports CoastalPortal
Imports System.Reflection

Public Class PortalClasses

End Class
Public Class PortalMember
    Private _CarrierID As Integer
    Private _UserID As Integer
    Private _AccountType As String
    Private _PlanType As String
    Private _family As String
    Private _PlanPeriod As String
    Private _OriginationCity As String
    Private _Username As String
    Private _PIN As String
    Private _Email As String
    Private _CompanyName As String
    Private _Title As String
    Private _FirstName As String
    Private _LastName As String
    Private _Address As String
    Private _Address2 As String
    Private _City As String
    Private _State As String
    Private _Zip As String
    Private _DayPhone As String
    Private _DayPhoneExt As String
    Private _EveningPhone As String
    Private _EveningPhoneExt As String
    Private _CellPhone As String
    Private _CellCarrier As String
    Private _ReferrerFrom As String
    Private _ReferrerName As String
    Private _ReferrerMember As String
    Private _FavDestination1 As String
    Private _FavDestination2 As String
    Private _FavDestination3 As String
    Private _FavDestination4 As String
    Private _FavDestination5 As String
    Private _TravelsPerYear As String
    Private _LicenseName As String
    Private _LicenseNumber As String
    Private _LicenseExpires As Date
    Private _LicenseState As String
    Private _LicenseBirthDate As Date
    Private _CardType As String
    Private _CardName As String
    Private _CardNumber As String
    Private _CardExpiresMonth As String
    Private _CardExpiresYear As String
    Private _CardCVV As String
    Private _BillingAddress As String
    Private _BillingAddress2 As String
    Private _BillingCity As String
    Private _BillingState As String
    Private _BillingZip As String
    Private _PersonalMessage As String
    Private _Status As String
    Private _Billed As String
    Private _DateCreated As Date
    Private _DateUpdated As Date
    Private _LastSignIn As Date
    Private _MemberSystem As Integer
    Private _UserType As String
    Private _Active As Boolean
    Private _Country As String
    Private _ReminderQuestion As String
    Private _ReminderAnswer As String
    Private _CountryofResidence As String
    Private _CityofBirth As String
    Private _StateofBirth As String
    Private _CountryOfBirth As String
    Private _DateOfBirth As Date
    Private _Citizenship As String
    Private _DocumentType As String
    Private _DocumentNumber As String
    Private _CountryOfIssuance As String
    Private _DocumentExpDate As Date
    Private _AdditionalDocType As String
    Private _AdditionalDocNumber As String
    Private _AdditionalDocCountry As String
    Private _AdditionalDocExpDate As Date
    Private _Gender As String
    Private _MiddleName As String
    Private _IncludeInPromotions As Boolean
    Private _canAcceptAll As Boolean
    Public Property canAcceptAll() As Boolean
        Get
            Return _canAcceptAll
        End Get
        Set(ByVal value As Boolean)
            _canAcceptAll = value
        End Set
    End Property

    Public Property CarrierID() As Integer
        Get
            Return _CarrierID
        End Get
        Set(ByVal value As Integer)
            _CarrierID = value
        End Set
    End Property
    <Key>
    Public Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal value As Integer)
            _UserID = value
        End Set
    End Property
    Public Property AccountType() As String
        Get
            Return _AccountType
        End Get
        Set(ByVal value As String)
            _AccountType = value
        End Set
    End Property
    Public Property PlanType() As String
        Get
            Return _PlanType
        End Get
        Set(ByVal value As String)
            _PlanType = value
        End Set
    End Property
    Public Property family() As String
        Get
            Return _family
        End Get
        Set(ByVal value As String)
            _family = value
        End Set
    End Property
    Public Property PlanPeriod() As String
        Get
            Return _PlanPeriod
        End Get
        Set(ByVal value As String)
            _PlanPeriod = value
        End Set
    End Property
    Public Property OriginationCity() As String
        Get
            Return _OriginationCity
        End Get
        Set(ByVal value As String)
            _OriginationCity = value
        End Set
    End Property
    Public Property Username() As String
        Get
            Return _Username
        End Get
        Set(ByVal value As String)
            _Username = value
        End Set
    End Property
    Public Property PIN() As String
        Get
            Return _PIN
        End Get
        Set(ByVal value As String)
            _PIN = value
        End Set
    End Property
    Public Property Email() As String
        Get
            Return _Email
        End Get
        Set(ByVal value As String)
            _Email = value
        End Set
    End Property
    Public Property CompanyName() As String
        Get
            Return _CompanyName
        End Get
        Set(ByVal value As String)
            _CompanyName = value
        End Set
    End Property
    Public Property Title() As String
        Get
            Return _Title
        End Get
        Set(ByVal value As String)
            _Title = value
        End Set
    End Property
    Public Property FirstName() As String
        Get
            Return _FirstName
        End Get
        Set(ByVal value As String)
            _FirstName = value
        End Set
    End Property
    Public Property LastName() As String
        Get
            Return _LastName
        End Get
        Set(ByVal value As String)
            _LastName = value
        End Set
    End Property
    Public Property Address() As String
        Get
            Return _Address
        End Get
        Set(ByVal value As String)
            _Address = value
        End Set
    End Property
    Public Property Address2() As String
        Get
            Return _Address2
        End Get
        Set(ByVal value As String)
            _Address2 = value
        End Set
    End Property
    Public Property City() As String
        Get
            Return _City
        End Get
        Set(ByVal value As String)
            _City = value
        End Set
    End Property
    Public Property State() As String
        Get
            Return _State
        End Get
        Set(ByVal value As String)
            _State = value
        End Set
    End Property
    Public Property Zip() As String
        Get
            Return _Zip
        End Get
        Set(ByVal value As String)
            _Zip = value
        End Set
    End Property
    Public Property DayPhone() As String
        Get
            Return _DayPhone
        End Get
        Set(ByVal value As String)
            _DayPhone = value
        End Set
    End Property
    Public Property DayPhoneExt() As String
        Get
            Return _DayPhoneExt
        End Get
        Set(ByVal value As String)
            _DayPhoneExt = value
        End Set
    End Property
    Public Property EveningPhone() As String
        Get
            Return _EveningPhone
        End Get
        Set(ByVal value As String)
            _EveningPhone = value
        End Set
    End Property
    Public Property EveningPhoneExt() As String
        Get
            Return _EveningPhoneExt
        End Get
        Set(ByVal value As String)
            _EveningPhoneExt = value
        End Set
    End Property
    Public Property CellPhone() As String
        Get
            Return _CellPhone
        End Get
        Set(ByVal value As String)
            _CellPhone = value
        End Set
    End Property
    Public Property CellCarrier() As String
        Get
            Return _CellCarrier
        End Get
        Set(ByVal value As String)
            _CellCarrier = value
        End Set
    End Property
    Public Property ReferrerFrom() As String
        Get
            Return _ReferrerFrom
        End Get
        Set(ByVal value As String)
            _ReferrerFrom = value
        End Set
    End Property
    Public Property ReferrerName() As String
        Get
            Return _ReferrerName
        End Get
        Set(ByVal value As String)
            _ReferrerName = value
        End Set
    End Property
    Public Property ReferrerMember() As String
        Get
            Return _ReferrerMember
        End Get
        Set(ByVal value As String)
            _ReferrerMember = value
        End Set
    End Property
    Public Property FavDestination1() As String
        Get
            Return _FavDestination1
        End Get
        Set(ByVal value As String)
            _FavDestination1 = value
        End Set
    End Property
    Public Property FavDestination2() As String
        Get
            Return _FavDestination2
        End Get
        Set(ByVal value As String)
            _FavDestination2 = value
        End Set
    End Property
    Public Property FavDestination3() As String
        Get
            Return _FavDestination3
        End Get
        Set(ByVal value As String)
            _FavDestination3 = value
        End Set
    End Property
    Public Property FavDestination4() As String
        Get
            Return _FavDestination4
        End Get
        Set(ByVal value As String)
            _FavDestination4 = value
        End Set
    End Property
    Public Property FavDestination5() As String
        Get
            Return _FavDestination5
        End Get
        Set(ByVal value As String)
            _FavDestination5 = value
        End Set
    End Property
    Public Property TravelsPerYear() As String
        Get
            Return _TravelsPerYear
        End Get
        Set(ByVal value As String)
            _TravelsPerYear = value
        End Set
    End Property
    Public Property LicenseName() As String
        Get
            Return _LicenseName
        End Get
        Set(ByVal value As String)
            _LicenseName = value
        End Set
    End Property
    Public Property LicenseNumber() As String
        Get
            Return _LicenseNumber
        End Get
        Set(ByVal value As String)
            _LicenseNumber = value
        End Set
    End Property
    Public Property LicenseExpires() As Date
        Get
            Return _LicenseExpires
        End Get
        Set(ByVal value As Date)
            _LicenseExpires = value
        End Set
    End Property
    Public Property LicenseState() As String
        Get
            Return _LicenseState
        End Get
        Set(ByVal value As String)
            _LicenseState = value
        End Set
    End Property
    Public Property LicenseBirthDate() As Date
        Get
            Return _LicenseBirthDate
        End Get
        Set(ByVal value As Date)
            _LicenseBirthDate = value
        End Set
    End Property
    Public Property CardType() As String
        Get
            Return _CardType
        End Get
        Set(ByVal value As String)
            _CardType = value
        End Set
    End Property
    Public Property CardName() As String
        Get
            Return _CardName
        End Get
        Set(ByVal value As String)
            _CardName = value
        End Set
    End Property
    Public Property CardNumber() As String
        Get
            Return _CardNumber
        End Get
        Set(ByVal value As String)
            _CardNumber = value
        End Set
    End Property
    Public Property CardExpiresMonth() As String
        Get
            Return _CardExpiresMonth
        End Get
        Set(ByVal value As String)
            _CardExpiresMonth = value
        End Set
    End Property
    Public Property CardExpiresYear() As String
        Get
            Return _CardExpiresYear
        End Get
        Set(ByVal value As String)
            _CardExpiresYear = value
        End Set
    End Property
    Public Property CardCVV() As String
        Get
            Return _CardCVV
        End Get
        Set(ByVal value As String)
            _CardCVV = value
        End Set
    End Property
    Public Property BillingAddress() As String
        Get
            Return _BillingAddress
        End Get
        Set(ByVal value As String)
            _BillingAddress = value
        End Set
    End Property
    Public Property BillingAddress2() As String
        Get
            Return _BillingAddress2
        End Get
        Set(ByVal value As String)
            _BillingAddress2 = value
        End Set
    End Property
    Public Property BillingCity() As String
        Get
            Return _BillingCity
        End Get
        Set(ByVal value As String)
            _BillingCity = value
        End Set
    End Property
    Public Property BillingState() As String
        Get
            Return _BillingState
        End Get
        Set(ByVal value As String)
            _BillingState = value
        End Set
    End Property
    Public Property BillingZip() As String
        Get
            Return _BillingZip
        End Get
        Set(ByVal value As String)
            _BillingZip = value
        End Set
    End Property
    Public Property PersonalMessage() As String
        Get
            Return _PersonalMessage
        End Get
        Set(ByVal value As String)
            _PersonalMessage = value
        End Set
    End Property
    Public Property Status() As String
        Get
            Return _Status
        End Get
        Set(ByVal value As String)
            _Status = value
        End Set
    End Property
    Public Property Billed() As String
        Get
            Return _Billed
        End Get
        Set(ByVal value As String)
            _Billed = value
        End Set
    End Property
    Public Property DateCreated() As Date
        Get
            Return _DateCreated
        End Get
        Set(ByVal value As Date)
            _DateCreated = value
        End Set
    End Property
    Public Property DateUpdated() As Date
        Get
            Return _DateUpdated
        End Get
        Set(ByVal value As Date)
            _DateUpdated = value
        End Set
    End Property
    Public Property LastSignIn() As Date
        Get
            Return _LastSignIn
        End Get
        Set(ByVal value As Date)
            _LastSignIn = value
        End Set
    End Property
    Public Property MemberSystem() As Integer
        Get
            Return _MemberSystem
        End Get
        Set(ByVal value As Integer)
            _MemberSystem = value
        End Set
    End Property
    Public Property UserType() As String
        Get
            Return _UserType
        End Get
        Set(ByVal value As String)
            _UserType = value
        End Set
    End Property
    Public Property Active() As Boolean
        Get
            Return _Active
        End Get
        Set(ByVal value As Boolean)
            _Active = value
        End Set
    End Property
    Public Property Country() As String
        Get
            Return _Country
        End Get
        Set(ByVal value As String)
            _Country = value
        End Set
    End Property
    Public Property ReminderQuestion() As String
        Get
            Return _ReminderQuestion
        End Get
        Set(ByVal value As String)
            _ReminderQuestion = value
        End Set
    End Property
    Public Property ReminderAnswer() As String
        Get
            Return _ReminderAnswer
        End Get
        Set(ByVal value As String)
            _ReminderAnswer = value
        End Set
    End Property
    Public Property CountryofResidence() As String
        Get
            Return _CountryofResidence
        End Get
        Set(ByVal value As String)
            _CountryofResidence = value
        End Set
    End Property
    Public Property CityofBirth() As String
        Get
            Return _CityofBirth
        End Get
        Set(ByVal value As String)
            _CityofBirth = value
        End Set
    End Property
    Public Property StateofBirth() As String
        Get
            Return _StateofBirth
        End Get
        Set(ByVal value As String)
            _StateofBirth = value
        End Set
    End Property
    Public Property CountryOfBirth() As String
        Get
            Return _CountryOfBirth
        End Get
        Set(ByVal value As String)
            _CountryOfBirth = value
        End Set
    End Property
    Public Property DateOfBirth() As Date
        Get
            Return _DateOfBirth
        End Get
        Set(ByVal value As Date)
            _DateOfBirth = value
        End Set
    End Property
    Public Property Citizenship() As String
        Get
            Return _Citizenship
        End Get
        Set(ByVal value As String)
            _Citizenship = value
        End Set
    End Property
    Public Property DocumentType() As String
        Get
            Return _DocumentType
        End Get
        Set(ByVal value As String)
            _DocumentType = value
        End Set
    End Property
    Public Property DocumentNumber() As String
        Get
            Return _DocumentNumber
        End Get
        Set(ByVal value As String)
            _DocumentNumber = value
        End Set
    End Property
    Public Property CountryOfIssuance() As String
        Get
            Return _CountryOfIssuance
        End Get
        Set(ByVal value As String)
            _CountryOfIssuance = value
        End Set
    End Property
    Public Property DocumentExpDate() As Date
        Get
            Return _DocumentExpDate
        End Get
        Set(ByVal value As Date)
            _DocumentExpDate = value
        End Set
    End Property
    Public Property AdditionalDocType() As String
        Get
            Return _AdditionalDocType
        End Get
        Set(ByVal value As String)
            _AdditionalDocType = value
        End Set
    End Property
    Public Property AdditionalDocNumber() As String
        Get
            Return _AdditionalDocNumber
        End Get
        Set(ByVal value As String)
            _AdditionalDocNumber = value
        End Set
    End Property
    Public Property AdditionalDocCountry() As String
        Get
            Return _AdditionalDocCountry
        End Get
        Set(ByVal value As String)
            _AdditionalDocCountry = value
        End Set
    End Property
    Public Property AdditionalDocExpDate() As Date
        Get
            Return _AdditionalDocExpDate
        End Get
        Set(ByVal value As Date)
            _AdditionalDocExpDate = value
        End Set
    End Property
    Public Property Gender() As String
        Get
            Return _Gender
        End Get
        Set(ByVal value As String)
            _Gender = value
        End Set
    End Property
    Public Property MiddleName() As String
        Get
            Return _MiddleName
        End Get
        Set(ByVal value As String)
            _MiddleName = value
        End Set
    End Property
    Public Property IncludeInPromotions() As Boolean
        Get
            Return _IncludeInPromotions
        End Get
        Set(ByVal value As Boolean)
            _IncludeInPromotions = value
        End Set
    End Property
End Class