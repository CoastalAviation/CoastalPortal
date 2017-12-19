Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel.DataAnnotations
Imports CoastalPortal.AirTaxi
Imports CoastalPortal
Imports System.Reflection

Public Class PanelClasses
    Public Shared Function checkplaceholder(ac As String) As Boolean
        'ac = Trim(ac)
        If ac = "LTJB" Or ac = "LTJT" Or ac = "MIDB" Or ac = "MIDS" Or ac = "LTO" Or ac = "MDO" Or ac = "HVO" Or ac = "SPO" Or ac = "SUPB" Or ac = "SUPR" Or ac = "TTO" Then
            Return True
        End If

        If ac = "MIDJ" Or ac = "LGHT" Or ac = "SMID" Or ac = "DPJ6" Or ac = "DPJ5" Or ac = "DPJ2" Or ac = "DPJ1" Then
            Return True
        End If

        If Left(ac, 3) = "DPJ" Then Return True

        If ac = "X400" Or ac = "X800" Or ac = "XCL" Then
            Return True
        End If

        If ac = "WUNDEX" Or ac = "WUNDKA" Or ac = "WUVEKA" Or ac = "WUVEEX" Or ac = "BULP" Then
            Return True
        End If

        'rk 11.9.15 add extra placeholders for WU
        If Left(ac, 2) = "WU" Then
            Return True
        End If

        'rk 3.8.16 added for JLX
        If Left(ac, 4) = "CHAR" Then
            Return True
        End If

        If ac = "JET III" Or ac = "JETCHAR1" Or ac = "JEXCEL1" Or ac = "JEXCEL2" Then
            Return True
        End If

        'rk check placeholder 3.20.16
        If ac = "1JETSTBY" Or ac = "1STNBY" Or ac = "2STNBY" Or ac = "3STNBY" Or ac = "4STNBY" Or ac = "4STNBY" Then
            Return True
        End If

        'rk check placeholder 4.7.16
        If ac = "1JETSTBY" Or ac = "1STNDBY" Or ac = "2STNDBY" Or ac = "3STNDBY" Or ac = "4STNDBY" Or ac = "4STNDBY" Then
            Return True
        End If

        Return False


    End Function

End Class
Public Class AirplaneTailNumber
    Private _TailNumber As String
    Public Property TailNumber() As String
        Get
            Return _TailNumber
        End Get
        Set(ByVal value As String)
            _TailNumber = value
        End Set
    End Property
End Class
Public Class UserPB
    Private _userid As String
    Private _userpw As String
    Private _carrierid As Integer
    Private _useremail As String
    Private _canAcceptAll As Boolean
    Public Property canAcceptAll() As Boolean
        Get
            Return _canAcceptAll
        End Get
        Set(ByVal value As Boolean)
            _canAcceptAll = value
        End Set
    End Property
    Public Property useremail() As String
        Get
            Return _useremail
        End Get
        Set(ByVal value As String)
            _useremail = value
        End Set
    End Property
    Public Property carrierid() As Integer
        Get
            Return _carrierid
        End Get
        Set(ByVal value As Integer)
            _carrierid = value
        End Set
    End Property
    <Key> <Column(Order:=1)>
    Public Property userpw() As String
        Get
            Return _userpw
        End Get
        Set(ByVal value As String)
            _userpw = value
        End Set
    End Property
    <Key> <Column(Order:=0)>
    Public Property userID() As String
        Get
            Return _userid
        End Get
        Set(ByVal value As String)
            _userid = value
        End Set
    End Property
End Class
Public Class optimizerLog
    Private m_id As Integer
    Private m_carrierid As Integer
    Private m_modelrunid As String
    Private m_optimizerengineversion As String
    Private m_flightsindemand As Integer
    Private m_customrunnumber As Integer
    Public Property customrunnumber() As Integer
        Get
            Return m_customrunnumber
        End Get
        Set(ByVal value As Integer)
            m_customrunnumber = value
        End Set
    End Property
    Public Property flightsindemand() As Integer
        Get
            Return m_flightsindemand
        End Get
        Set(ByVal value As Integer)
            m_flightsindemand = value
        End Set
    End Property
    Public Property optimizerengineversion() As String
        Get
            Return m_optimizerengineversion
        End Get
        Set(ByVal value As String)
            m_optimizerengineversion = value
        End Set
    End Property
    Public Property modelrunid() As String
        Get
            Return m_modelrunid
        End Get
        Set(ByVal value As String)
            m_modelrunid = value
        End Set
    End Property
    Public Property carrierid() As Integer
        Get
            Return m_carrierid
        End Get
        Set(ByVal value As Integer)
            m_carrierid = value
        End Set
    End Property
    Public Property id() As Integer
        Get
            Return m_id
        End Get
        Set(ByVal value As Integer)
            m_id = value
        End Set
    End Property
End Class
<Serializable>
Public Class CarrierProfile
    Private M_carrierid As Integer
    Private _casGridColstostrip As Integer
    Private _fosGridColstoSrip As Integer
    Private _useRevenue As Boolean
    Private _usePandL As Boolean
    Private _useWeightClass As Boolean
    Private _useOE As Boolean
    Private _useHA As Boolean
    Private _useBase As Boolean
    Private _fosSortOrder As String
    Private _casSortOrder As String
    Private _demandlist As String
    Private _fcdrpandl As Boolean
    Public Property FCDRPandL() As Boolean
        Get
            Return _fcdrpandl
        End Get
        Set(ByVal value As Boolean)
            _fcdrpandl = value
        End Set
    End Property
    Public Property DemandList() As String
        Get
            Return _demandlist
        End Get
        Set(ByVal value As String)
            _demandlist = value
        End Set
    End Property
    Public Property casSortOrder() As String
        Get
            Return _casSortOrder
        End Get
        Set(ByVal value As String)
            _casSortOrder = value
        End Set
    End Property
    Public Property fosSortOrder() As String
        Get
            Return _fosSortOrder
        End Get
        Set(ByVal value As String)
            _fosSortOrder = value
        End Set
    End Property
    Public Property useBase() As Boolean
        Get
            Return _useBase
        End Get
        Set(ByVal value As Boolean)
            _useBase = value
        End Set
    End Property
    Public Property useHA() As Boolean
        Get
            Return _useHA
        End Get
        Set(ByVal value As Boolean)
            _useHA = value
        End Set
    End Property
    Public Property useOE() As Boolean
        Get
            Return _useOE
        End Get
        Set(ByVal value As Boolean)
            _useOE = value
        End Set
    End Property
    Public Property useWeightClass() As Boolean
        Get
            Return _useWeightClass
        End Get
        Set(ByVal value As Boolean)
            _useWeightClass = value
        End Set
    End Property
    Public Property usePandL() As Boolean
        Get
            Return _usePandL
        End Get
        Set(ByVal value As Boolean)
            _usePandL = value
        End Set
    End Property
    Public Property useRevenue() As Boolean
        Get
            Return _useRevenue
        End Get
        Set(ByVal value As Boolean)
            _useRevenue = value
        End Set
    End Property
    Public Property fosGridColstoStrip() As Integer
        Get
            Return _fosGridColstoSrip
        End Get
        Set(ByVal value As Integer)
            _fosGridColstoSrip = value
        End Set
    End Property
    Public Property casGridColstoStrip() As Integer
        Get
            Return _casGridColstostrip
        End Get
        Set(ByVal value As Integer)
            _casGridColstostrip = value
        End Set
    End Property
    Public Property carrierid() As Integer
        Get
            Return M_carrierid
        End Get
        Set(ByVal value As Integer)
            M_carrierid = value
        End Set
    End Property
End Class

<Serializable>
Public Class CASFlightsOptimizerRecord
    Private _ID As Integer
    Private _OptimizerRunY As String
    Private _AircraftID As Integer
    Private _DepartureAirport As String
    Private _ArrivalAirport As String
    Private _DepartureTime As Date
    Private _ArrivalTime As Date
    Private _FlightType As String
    Private _Status As String
    Private _FlightDetail As Integer
    Private _ModelRun As String
    Private _Distance As Integer
    Private _Duration As Decimal
    Private _AircraftFailed As Boolean?
    Private _AircraftFailedBy As Integer?
    Private _AircraftFailedOnUTC As Date?
    Private _AircraftRegistration As String
    Private _Price As Decimal
    Private _PricePerSeat As Decimal
    Private _AllowSale As Boolean
    Private _FAAPart As String
    Private _PIC As String
    Private _SIC As String
    Private _CA As String
    Private _CA2 As String
    Private _P1 As String
    Private _P2 As String
    Private _P3 As String
    Private _P4 As String
    Private _cost As Decimal
    Private _aircrafttype As String
    Private _session As Date?
    Private _DepartureDateGMT As String
    Private _DepartureTimeGMT As String
    Private _ArrivalDateGMT As String
    Private _ArrivalTimeGMT As String
    Private _TripNumber As String
    Private _TripStatus As String
    Private _CancelDate As String
    Private _CancelCode As String
    Private _LegState As String
    Private _PriorTail As String
    Private _Billable As String
    Private _LegTypeCode As String
    Private _LegRateCode As String
    Private _LegPurposeCode As String
    Private _ArrivalDate As String
    Private _DepartureTimeZone As String
    Private _ArrivalTimeZone As String
    Private _DepartureDateTimeLocal As String
    Private _ArrivalDateTimeLocal As String
    Private _CarrierId As Integer
    Private _PriorTailSavings As Integer
    Private _OptimizerRun As String
    Private _OptimizerRequestNum As Integer
    Private _OutTime As String
    Private _OffTime As String
    Private _OnTime As String
    Private _InTime As String
    Private _CrewDutyFlightTime2 As Decimal?
    Private _CrewDutyWindow2 As Decimal?
    Private _CrewDutyComment As String
    Private _tripcost As Decimal
    Private _triprevenue As Decimal
    Private _PandL As Decimal
    Private _Versionx As String
    Private _FOSKEY As String
    Private _BaseCode As String
    Private _OwnerCode As String
    Private _HomeAirport As String
    Private _ListOrder As String
    Private _VendorID As String
    Private _TripBaseCode As String
    Private _RateDetailx As String
    Private _LegBaseCode As String
    Private _QuotedEquipType As String
    Private _QuotedTail As String
    Private _OwnerRateType As String
    Private _QuotedBaseCode As String
    Private _TripStartTime As String
    Private _TST As String
    Private _RateDetail As String
    Private _SavingsDescription As String
    Private _SavingsDescriptionExt As String
    Private _WarningText As String
    Private _QuotedOwnerACCost As String
    Private _DivisionCode As String
    Private _CrewDutyFlightTime As Decimal
    Private _CrewDutyWindow As Decimal
    Private _OriginalCostWRepobak As Decimal?
    Private _OriginalRepoDetailbak As String
    Private _OriginalCostWRepo As Decimal?
    Private _OriginalRepoDetail As String
    Private _ProRatedRevenue As Decimal?
    Private _ScrubText As String
    Private _ClientDetail As String
    Private _Pinned As String
    Private _Version As String
    Private M_AWC As WeightClass
    Public Overridable Property WeightClass() As WeightClass
        Get
            Return M_AWC
        End Get
        Set(ByVal value As WeightClass)
            M_AWC = value
        End Set
    End Property
    Public Property ID() As Integer
        Get
            Return _ID
        End Get
        Set(ByVal value As Integer)
            _ID = value
        End Set
    End Property
    Public Property OptimizerRunY() As String
        Get
            Return _OptimizerRunY
        End Get
        Set(ByVal value As String)
            _OptimizerRunY = value
        End Set
    End Property
    Public Property AircraftID() As Integer
        Get
            Return _AircraftID
        End Get
        Set(ByVal value As Integer)
            _AircraftID = value
        End Set
    End Property
    Public Property DepartureAirport() As String
        Get
            Return _DepartureAirport
        End Get
        Set(ByVal value As String)
            _DepartureAirport = value
        End Set
    End Property
    Public Property ArrivalAirport() As String
        Get
            Return _ArrivalAirport
        End Get
        Set(ByVal value As String)
            _ArrivalAirport = value
        End Set
    End Property
    Public Property DepartureTime() As Date
        Get
            Return _DepartureTime
        End Get
        Set(ByVal value As Date)
            _DepartureTime = value
        End Set
    End Property
    Public Property ArrivalTime() As Date
        Get
            Return _ArrivalTime
        End Get
        Set(ByVal value As Date)
            _ArrivalTime = value
        End Set
    End Property
    Public Property FlightType() As String
        Get
            Return _FlightType
        End Get
        Set(ByVal value As String)
            _FlightType = value
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
    Public Property FlightDetail() As Integer
        Get
            Return _FlightDetail
        End Get
        Set(ByVal value As Integer)
            _FlightDetail = value
        End Set
    End Property
    Public Property ModelRun() As String
        Get
            Return _ModelRun
        End Get
        Set(ByVal value As String)
            _ModelRun = value
        End Set
    End Property
    Public Property Distance() As Integer
        Get
            Return _Distance
        End Get
        Set(ByVal value As Integer)
            _Distance = value
        End Set
    End Property
    Public Property Duration() As Decimal
        Get
            Return _Duration
        End Get
        Set(ByVal value As Decimal)
            _Duration = value
        End Set
    End Property
    Public Property AircraftFailed() As Boolean?
        Get
            Return _AircraftFailed
        End Get
        Set(ByVal value As Boolean?)
            _AircraftFailed = value
        End Set
    End Property
    Public Property AircraftFailedBy() As Integer?
        Get
            Return _AircraftFailedBy
        End Get
        Set(ByVal value As Integer?)
            _AircraftFailedBy = value
        End Set
    End Property
    Public Property AircraftFailedOnUTC() As Date?
        Get
            Return _AircraftFailedOnUTC
        End Get
        Set(ByVal value As Date?)
            _AircraftFailedOnUTC = value
        End Set
    End Property
    Public Property AircraftRegistration() As String
        Get
            Return _AircraftRegistration
        End Get
        Set(ByVal value As String)
            _AircraftRegistration = value
        End Set
    End Property
    Public Property Price() As Decimal
        Get
            Return _Price
        End Get
        Set(ByVal value As Decimal)
            _Price = value
        End Set
    End Property
    Public Property PricePerSeat() As Decimal
        Get
            Return _PricePerSeat
        End Get
        Set(ByVal value As Decimal)
            _PricePerSeat = value
        End Set
    End Property
    Public Property AllowSale() As Boolean
        Get
            Return _AllowSale
        End Get
        Set(ByVal value As Boolean)
            _AllowSale = value
        End Set
    End Property
    Public Property FAAPart() As String
        Get
            Return _FAAPart
        End Get
        Set(ByVal value As String)
            _FAAPart = value
        End Set
    End Property
    Public Property PIC() As String
        Get
            Return _PIC
        End Get
        Set(ByVal value As String)
            _PIC = value
        End Set
    End Property
    Public Property SIC() As String
        Get
            Return _SIC
        End Get
        Set(ByVal value As String)
            _SIC = value
        End Set
    End Property
    Public Property CA() As String
        Get
            Return _CA
        End Get
        Set(ByVal value As String)
            _CA = value
        End Set
    End Property
    Public Property CA2() As String
        Get
            Return _CA2
        End Get
        Set(ByVal value As String)
            _CA2 = value
        End Set
    End Property
    Public Property P1() As String
        Get
            Return _P1
        End Get
        Set(ByVal value As String)
            _P1 = value
        End Set
    End Property
    Public Property P2() As String
        Get
            Return _P2
        End Get
        Set(ByVal value As String)
            _P2 = value
        End Set
    End Property
    Public Property P3() As String
        Get
            Return _P3
        End Get
        Set(ByVal value As String)
            _P3 = value
        End Set
    End Property
    Public Property P4() As String
        Get
            Return _P4
        End Get
        Set(ByVal value As String)
            _P4 = value
        End Set
    End Property
    Public Property cost() As Decimal
        Get
            Return _cost
        End Get
        Set(ByVal value As Decimal)
            _cost = value
        End Set
    End Property
    <ForeignKey("WeightClass")>
    Public Property aircrafttype() As String
        Get
            Return Trim(_aircrafttype)
        End Get
        Set(ByVal value As String)
            _aircrafttype = Trim(value)
        End Set
    End Property
    Public Property session() As Date?
        Get
            Return _session
        End Get
        Set(ByVal value As Date?)
            _session = value
        End Set
    End Property
    Public Property DepartureDateGMT() As String
        Get
            Return _DepartureDateGMT
        End Get
        Set(ByVal value As String)
            _DepartureDateGMT = value
        End Set
    End Property
    Public Property DepartureTimeGMT() As String
        Get
            Return _DepartureTimeGMT
        End Get
        Set(ByVal value As String)
            _DepartureTimeGMT = value
        End Set
    End Property
    Public Property ArrivalDateGMT() As String
        Get
            Return _ArrivalDateGMT
        End Get
        Set(ByVal value As String)
            _ArrivalDateGMT = value
        End Set
    End Property
    Public Property ArrivalTimeGMT() As String
        Get
            Return _ArrivalTimeGMT
        End Get
        Set(ByVal value As String)
            _ArrivalTimeGMT = value
        End Set
    End Property
    Public Property TripNumber() As String
        Get
            Return _TripNumber
        End Get
        Set(ByVal value As String)
            _TripNumber = value
        End Set
    End Property
    Public Property TripStatus() As String
        Get
            Return _TripStatus
        End Get
        Set(ByVal value As String)
            _TripStatus = value
        End Set
    End Property
    Public Property CancelDate() As String
        Get
            Return _CancelDate
        End Get
        Set(ByVal value As String)
            _CancelDate = value
        End Set
    End Property
    Public Property CancelCode() As String
        Get
            Return _CancelCode
        End Get
        Set(ByVal value As String)
            _CancelCode = value
        End Set
    End Property
    Public Property LegState() As String
        Get
            Return _LegState
        End Get
        Set(ByVal value As String)
            _LegState = value
        End Set
    End Property
    Public Property PriorTail() As String
        Get
            Return _PriorTail
        End Get
        Set(ByVal value As String)
            _PriorTail = value
        End Set
    End Property
    Public Property Billable() As String
        Get
            Return _Billable
        End Get
        Set(ByVal value As String)
            _Billable = value
        End Set
    End Property
    Public Property LegTypeCode() As String
        Get
            Return _LegTypeCode
        End Get
        Set(ByVal value As String)
            _LegTypeCode = value
        End Set
    End Property
    Public Property LegRateCode() As String
        Get
            Return _LegRateCode
        End Get
        Set(ByVal value As String)
            _LegRateCode = value
        End Set
    End Property
    Public Property LegPurposeCode() As String
        Get
            Return _LegPurposeCode
        End Get
        Set(ByVal value As String)
            _LegPurposeCode = value
        End Set
    End Property
    Public Property ArrivalDate() As String
        Get
            Return _ArrivalDate
        End Get
        Set(ByVal value As String)
            _ArrivalDate = value
        End Set
    End Property
    Public Property DepartureTimeZone() As String
        Get
            Return _DepartureTimeZone
        End Get
        Set(ByVal value As String)
            _DepartureTimeZone = value
        End Set
    End Property
    Public Property ArrivalTimeZone() As String
        Get
            Return _ArrivalTimeZone
        End Get
        Set(ByVal value As String)
            _ArrivalTimeZone = value
        End Set
    End Property
    Public Property DepartureDateTimeLocal() As String
        Get
            Return _DepartureDateTimeLocal
        End Get
        Set(ByVal value As String)
            _DepartureDateTimeLocal = value
        End Set
    End Property
    Public Property ArrivalDateTimeLocal() As String
        Get
            Return _ArrivalDateTimeLocal
        End Get
        Set(ByVal value As String)
            _ArrivalDateTimeLocal = value
        End Set
    End Property
    Public Property CarrierId() As Integer
        Get
            Return _CarrierId
        End Get
        Set(ByVal value As Integer)
            _CarrierId = value
        End Set
    End Property
    Public Property PriorTailSavings() As Integer
        Get
            Return _PriorTailSavings
        End Get
        Set(ByVal value As Integer)
            _PriorTailSavings = value
        End Set
    End Property
    Public Property OptimizerRun() As String
        Get
            Return _OptimizerRun
        End Get
        Set(ByVal value As String)
            _OptimizerRun = value
        End Set
    End Property
    Public Property OptimizerRequestNum() As Integer
        Get
            Return _OptimizerRequestNum
        End Get
        Set(ByVal value As Integer)
            _OptimizerRequestNum = value
        End Set
    End Property
    Public Property OutTime() As String
        Get
            Return _OutTime
        End Get
        Set(ByVal value As String)
            _OutTime = value
        End Set
    End Property
    Public Property OffTime() As String
        Get
            Return _OffTime
        End Get
        Set(ByVal value As String)
            _OffTime = value
        End Set
    End Property
    Public Property OnTime() As String
        Get
            Return _OnTime
        End Get
        Set(ByVal value As String)
            _OnTime = value
        End Set
    End Property
    Public Property InTime() As String
        Get
            Return _InTime
        End Get
        Set(ByVal value As String)
            _InTime = value
        End Set
    End Property
    Public Property CrewDutyFlightTime2() As Decimal?
        Get
            Return _CrewDutyFlightTime2
        End Get
        Set(ByVal value As Decimal?)
            _CrewDutyFlightTime2 = value
        End Set
    End Property
    Public Property CrewDutyWindow2() As Decimal?
        Get
            Return _CrewDutyWindow2
        End Get
        Set(ByVal value As Decimal?)
            _CrewDutyWindow2 = value
        End Set
    End Property
    Public Property CrewDutyComment() As String
        Get
            Return _CrewDutyComment
        End Get
        Set(ByVal value As String)
            _CrewDutyComment = value
        End Set
    End Property
    Public Property tripcost() As Decimal
        Get
            Return _tripcost
        End Get
        Set(ByVal value As Decimal)
            _tripcost = value
        End Set
    End Property
    Public Property triprevenue() As Decimal
        Get
            Return _triprevenue
        End Get
        Set(ByVal value As Decimal)
            _triprevenue = value
        End Set
    End Property
    Public Property PandL() As Decimal
        Get
            Return _PandL
        End Get
        Set(ByVal value As Decimal)
            _PandL = value
        End Set
    End Property
    Public Property Versionx() As String
        Get
            Return _Versionx
        End Get
        Set(ByVal value As String)
            _Versionx = value
        End Set
    End Property
    Public Property FOSKEY() As String
        Get
            Return _FOSKEY
        End Get
        Set(ByVal value As String)
            _FOSKEY = value
        End Set
    End Property
    Public Property BaseCode() As String
        Get
            Return _BaseCode
        End Get
        Set(ByVal value As String)
            _BaseCode = value
        End Set
    End Property
    Public Property OwnerCode() As String
        Get
            Return _OwnerCode
        End Get
        Set(ByVal value As String)
            _OwnerCode = value
        End Set
    End Property
    Public Property HomeAirport() As String
        Get
            Return _HomeAirport
        End Get
        Set(ByVal value As String)
            _HomeAirport = value
        End Set
    End Property
    Public Property ListOrder() As String
        Get
            Return _ListOrder
        End Get
        Set(ByVal value As String)
            _ListOrder = value
        End Set
    End Property
    Public Property VendorID() As String
        Get
            Return _VendorID
        End Get
        Set(ByVal value As String)
            _VendorID = value
        End Set
    End Property
    Public Property TripBaseCode() As String
        Get
            Return _TripBaseCode
        End Get
        Set(ByVal value As String)
            _TripBaseCode = value
        End Set
    End Property
    Public Property RateDetailx() As String
        Get
            Return _RateDetailx
        End Get
        Set(ByVal value As String)
            _RateDetailx = value
        End Set
    End Property
    Public Property LegBaseCode() As String
        Get
            Return _LegBaseCode
        End Get
        Set(ByVal value As String)
            _LegBaseCode = value
        End Set
    End Property
    Public Property QuotedEquipType() As String
        Get
            Return _QuotedEquipType
        End Get
        Set(ByVal value As String)
            _QuotedEquipType = value
        End Set
    End Property
    Public Property QuotedTail() As String
        Get
            Return _QuotedTail
        End Get
        Set(ByVal value As String)
            _QuotedTail = value
        End Set
    End Property
    Public Property OwnerRateType() As String
        Get
            Return _OwnerRateType
        End Get
        Set(ByVal value As String)
            _OwnerRateType = value
        End Set
    End Property
    Public Property QuotedBaseCode() As String
        Get
            Return _QuotedBaseCode
        End Get
        Set(ByVal value As String)
            _QuotedBaseCode = value
        End Set
    End Property
    Public Property TripStartTime() As String
        Get
            Return _TripStartTime
        End Get
        Set(ByVal value As String)
            _TripStartTime = value
        End Set
    End Property
    Public Property TST() As String
        Get
            Return _TST
        End Get
        Set(ByVal value As String)
            _TST = value
        End Set
    End Property
    Public Property RateDetail() As String
        Get
            Return _RateDetail
        End Get
        Set(ByVal value As String)
            _RateDetail = value
        End Set
    End Property
    Public Property SavingsDescription() As String
        Get
            Return _SavingsDescription
        End Get
        Set(ByVal value As String)
            _SavingsDescription = value
        End Set
    End Property
    Public Property SavingsDescriptionExt() As String
        Get
            Return _SavingsDescriptionExt
        End Get
        Set(ByVal value As String)
            _SavingsDescriptionExt = value
        End Set
    End Property
    Public Property WarningText() As String
        Get
            Return _WarningText
        End Get
        Set(ByVal value As String)
            _WarningText = value
        End Set
    End Property
    Public Property QuotedOwnerACCost() As String
        Get
            Return _QuotedOwnerACCost
        End Get
        Set(ByVal value As String)
            _QuotedOwnerACCost = value
        End Set
    End Property
    Public Property DivisionCode() As String
        Get
            Return _DivisionCode
        End Get
        Set(ByVal value As String)
            _DivisionCode = value
        End Set
    End Property
    Public Property CrewDutyFlightTime() As Decimal
        Get
            Return _CrewDutyFlightTime
        End Get
        Set(ByVal value As Decimal)
            _CrewDutyFlightTime = value
        End Set
    End Property
    Public Property CrewDutyWindow() As Decimal
        Get
            Return _CrewDutyWindow
        End Get
        Set(ByVal value As Decimal)
            _CrewDutyWindow = value
        End Set
    End Property
    Public Property OriginalCostWRepobak() As Decimal?
        Get
            Return _OriginalCostWRepobak
        End Get
        Set(ByVal value As Decimal?)
            _OriginalCostWRepobak = value
        End Set
    End Property
    Public Property OriginalRepoDetailbak() As String
        Get
            Return _OriginalRepoDetailbak
        End Get
        Set(ByVal value As String)
            _OriginalRepoDetailbak = value
        End Set
    End Property
    Public Property OriginalCostWRepo() As Decimal
        Get
            Return _OriginalCostWRepo
        End Get
        Set(ByVal value As Decimal)
            _OriginalCostWRepo = value
        End Set
    End Property
    Public Property OriginalRepoDetail() As String
        Get
            Return _OriginalRepoDetail
        End Get
        Set(ByVal value As String)
            _OriginalRepoDetail = value
        End Set
    End Property
    Public Property ProRatedRevenue() As Decimal?
        Get
            Return _ProRatedRevenue
        End Get
        Set(ByVal value As Decimal?)
            _ProRatedRevenue = value
        End Set
    End Property
    Public Property ScrubText() As String
        Get
            Return _ScrubText
        End Get
        Set(ByVal value As String)
            _ScrubText = value
        End Set
    End Property
    Public Property ClientDetail() As String
        Get
            Return _ClientDetail
        End Get
        Set(ByVal value As String)
            _ClientDetail = value
        End Set
    End Property
    Public Property Pinned() As String
        Get
            Return _Pinned
        End Get
        Set(ByVal value As String)
            _Pinned = value
        End Set
    End Property
    Public Property Version() As String
        Get
            Return _Version
        End Get
        Set(ByVal value As String)
            _Version = value
        End Set
    End Property
End Class

<Serializable>
Public Class FOSFlightsOptimizerRecord
    Private _id As Integer
    Private _OptimizerRunX As String
    Private _FOSKey As String
    Private _DepartureAirport As String
    Private _DepartureAirportICAO As String
    Private _ArrivalAirport As String
    Private _ArrivalAirportICAO As String
    Private _DepartureDate As String
    Private _DepartureTime As String
    Private _Landings As String
    Private _FlightTime As String
    Private _NauticalMiles As String
    Private _AC As String
    Private _HobbsBeginTime As String
    Private _HobbsEnd As String
    Private _Duration As String
    Private _AircraftType As String
    Private _AircraftID As String
    Private _PAXCount As String
    Private _DeadHead As String
    Private _DHPaxEnt As String
    Private _DHConfirmed As String
    Private _DHFlexTimes As String
    Private _DHCost As String
    Private _DHDateBegin As String
    Private _DHDateEnd As String
    Private _DHCurrency As String
    Private _DHFlexBegin As String
    Private _DHFlexEnd As String
    Private _CASNautical As String
    Private _LegTypeCode As String
    Private _LegRateCode As String
    Private _LegPurposeCode As String
    Private _Session As Date?
    Private _DepartureDateGMT As String
    Private _DepartureTimeGMT As String
    Private _ArrivalDateGMT As String
    Private _ArrivalTimeGMT As String
    Private _carrierid As Integer
    Private _TripNumber As String
    Private _TripStatus As String
    Private _CancelDate As String
    Private _CancelCode As String
    Private _LegState As String
    Private _Billable As String
    Private _ArrivalDate As String
    Private _ArrivalTime As String
    Private _DepartureTimeZone As String
    Private _ArrivalTimeZone As String
    Private _OptimizerRun As String
    Private _OptimizerID As Integer?
    Private _PIC As String
    Private _SIC As String
    Private _OutTime As String
    Private _OffTime As String
    Private _OnTime As String
    Private _InTime As String
    Private _tripcost As Decimal
    Private _triprevenue As Decimal
    Private _PandL As Decimal
    Private _DepartureDateGMTKey As Date?
    Private _CASUpdate As Date?
    Private _QuoteNumber As String
    Private _Versionx As String
    Private _BaseCode As String
    Private _OwnerCode As String
    Private _HomeAirport As String
    Private _ListOrder As String
    Private _VendorID As String
    Private _TripBaseCode As String
    Private _RateDetailx As String
    Private _LegBaseCode As String
    Private _QuotedEquipType As String
    Private _QuotedTail As String
    Private _OwnerRateType As String
    Private _QuotedBaseCode As String
    Private _TripStartTime As String
    Private _TST As String
    Private _RateDetail As String
    Private _WarningText As String
    Private _QuotedOwnerACCost As String
    Private _DivisionCode As String
    Private _OriginalCostWRepo As Decimal
    Private _OriginalRepoDetail As String
    Private _ProRatedRevenue As Decimal
    Private _ScrubText As String
    Private _ClientDetail As String
    Private _Pinned As String
    Private _Version As String
    Private M_AWC As WeightClass
    Public Overridable Property WeightClass() As WeightClass
        Get
            Return M_AWC
        End Get
        Set(ByVal value As WeightClass)
            M_AWC = value
        End Set
    End Property
    <NotMapped>
    Public ReadOnly Property DateTimeGMT() As DateTime
        Get
            Return Date.Parse(DepartureDateGMT).Add(TimeSpan.Parse(DepartureTimeGMT))
        End Get
    End Property
    Public Property id() As Integer
        Get
            Return _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property
    Public Property OptimizerRunX() As String
        Get
            Return _OptimizerRunX
        End Get
        Set(ByVal value As String)
            _OptimizerRunX = value
        End Set
    End Property
    Public Property FOSKey() As String
        Get
            Return _FOSKey
        End Get
        Set(ByVal value As String)
            _FOSKey = value
        End Set
    End Property
    Public Property DepartureAirport() As String
        Get
            Return _DepartureAirport
        End Get
        Set(ByVal value As String)
            _DepartureAirport = value
        End Set
    End Property
    Public Property DepartureAirportICAO() As String
        Get
            Return _DepartureAirportICAO
        End Get
        Set(ByVal value As String)
            _DepartureAirportICAO = value
        End Set
    End Property
    Public Property ArrivalAirport() As String
        Get
            Return _ArrivalAirport
        End Get
        Set(ByVal value As String)
            _ArrivalAirport = value
        End Set
    End Property
    Public Property ArrivalAirportICAO() As String
        Get
            Return _ArrivalAirportICAO
        End Get
        Set(ByVal value As String)
            _ArrivalAirportICAO = value
        End Set
    End Property
    Public Property DepartureDate() As String
        Get
            Return _DepartureDate
        End Get
        Set(ByVal value As String)
            _DepartureDate = value
        End Set
    End Property
    Public Property DepartureTime() As String
        Get
            Return _DepartureTime
        End Get
        Set(ByVal value As String)
            _DepartureTime = value
        End Set
    End Property
    Public Property Landings() As String
        Get
            Return _Landings
        End Get
        Set(ByVal value As String)
            _Landings = value
        End Set
    End Property
    Public Property FlightTime() As String
        Get
            Return _FlightTime
        End Get
        Set(ByVal value As String)
            _FlightTime = value
        End Set
    End Property
    Public Property NauticalMiles() As String
        Get
            Return _NauticalMiles
        End Get
        Set(ByVal value As String)
            _NauticalMiles = value
        End Set
    End Property
    Public Property AC() As String
        Get
            Return _AC
        End Get
        Set(ByVal value As String)
            _AC = value
        End Set
    End Property
    Public Property HobbsBeginTime() As String
        Get
            Return _HobbsBeginTime
        End Get
        Set(ByVal value As String)
            _HobbsBeginTime = value
        End Set
    End Property
    Public Property HobbsEnd() As String
        Get
            Return _HobbsEnd
        End Get
        Set(ByVal value As String)
            _HobbsEnd = value
        End Set
    End Property
    Public Property Duration() As String
        Get
            Return _Duration
        End Get
        Set(ByVal value As String)
            _Duration = value
        End Set
    End Property
    <ForeignKey("WeightClass")>
    Public Property AircraftType() As String
        Get
            Return Trim(_AircraftType)
        End Get
        Set(ByVal value As String)
            _AircraftType = Trim(value)
        End Set
    End Property
    Public Property AircraftID() As String
        Get
            Return _AircraftID
        End Get
        Set(ByVal value As String)
            _AircraftID = value
        End Set
    End Property
    Public Property PAXCount() As String
        Get
            Return _PAXCount
        End Get
        Set(ByVal value As String)
            _PAXCount = value
        End Set
    End Property
    Public Property DeadHead() As String
        Get
            Return _DeadHead
        End Get
        Set(ByVal value As String)
            _DeadHead = value
        End Set
    End Property
    Public Property DHPaxEnt() As String
        Get
            Return _DHPaxEnt
        End Get
        Set(ByVal value As String)
            _DHPaxEnt = value
        End Set
    End Property
    Public Property DHConfirmed() As String
        Get
            Return _DHConfirmed
        End Get
        Set(ByVal value As String)
            _DHConfirmed = value
        End Set
    End Property
    Public Property DHFlexTimes() As String
        Get
            Return _DHFlexTimes
        End Get
        Set(ByVal value As String)
            _DHFlexTimes = value
        End Set
    End Property
    Public Property DHCost() As String
        Get
            Return _DHCost
        End Get
        Set(ByVal value As String)
            _DHCost = value
        End Set
    End Property
    Public Property DHDateBegin() As String
        Get
            Return _DHDateBegin
        End Get
        Set(ByVal value As String)
            _DHDateBegin = value
        End Set
    End Property
    Public Property DHDateEnd() As String
        Get
            Return _DHDateEnd
        End Get
        Set(ByVal value As String)
            _DHDateEnd = value
        End Set
    End Property
    Public Property DHCurrency() As String
        Get
            Return _DHCurrency
        End Get
        Set(ByVal value As String)
            _DHCurrency = value
        End Set
    End Property
    Public Property DHFlexBegin() As String
        Get
            Return _DHFlexBegin
        End Get
        Set(ByVal value As String)
            _DHFlexBegin = value
        End Set
    End Property
    Public Property DHFlexEnd() As String
        Get
            Return _DHFlexEnd
        End Get
        Set(ByVal value As String)
            _DHFlexEnd = value
        End Set
    End Property
    Public Property CASNautical() As String
        Get
            Return _CASNautical
        End Get
        Set(ByVal value As String)
            _CASNautical = value
        End Set
    End Property
    Public Property LegTypeCode() As String
        Get
            Return _LegTypeCode
        End Get
        Set(ByVal value As String)
            _LegTypeCode = value
        End Set
    End Property
    Public Property LegRateCode() As String
        Get
            Return _LegRateCode
        End Get
        Set(ByVal value As String)
            _LegRateCode = value
        End Set
    End Property
    Public Property LegPurposeCode() As String
        Get
            Return _LegPurposeCode
        End Get
        Set(ByVal value As String)
            _LegPurposeCode = value
        End Set
    End Property
    Public Property Session() As Date?
        Get
            Return _Session
        End Get
        Set(ByVal value As Date?)
            _Session = value
        End Set
    End Property
    Public Property DepartureDateGMT() As String
        Get
            Return _DepartureDateGMT
        End Get
        Set(ByVal value As String)
            _DepartureDateGMT = value
        End Set
    End Property
    Public Property DepartureTimeGMT() As String
        Get
            Return _DepartureTimeGMT
        End Get
        Set(ByVal value As String)
            _DepartureTimeGMT = value
        End Set
    End Property
    Public Property ArrivalDateGMT() As String
        Get
            Return _ArrivalDateGMT
        End Get
        Set(ByVal value As String)
            _ArrivalDateGMT = value
        End Set
    End Property
    Public Property ArrivalTimeGMT() As String
        Get
            Return _ArrivalTimeGMT
        End Get
        Set(ByVal value As String)
            _ArrivalTimeGMT = value
        End Set
    End Property
    Public Property carrierid() As Integer
        Get
            Return _carrierid
        End Get
        Set(ByVal value As Integer)
            _carrierid = value
        End Set
    End Property
    Public Property TripNumber() As String
        Get
            Return _TripNumber
        End Get
        Set(ByVal value As String)
            _TripNumber = value
        End Set
    End Property
    Public Property TripStatus() As String
        Get
            Return _TripStatus
        End Get
        Set(ByVal value As String)
            _TripStatus = value
        End Set
    End Property
    Public Property CancelDate() As String
        Get
            Return _CancelDate
        End Get
        Set(ByVal value As String)
            _CancelDate = value
        End Set
    End Property
    Public Property CancelCode() As String
        Get
            Return _CancelCode
        End Get
        Set(ByVal value As String)
            _CancelCode = value
        End Set
    End Property
    Public Property LegState() As String
        Get
            Return _LegState
        End Get
        Set(ByVal value As String)
            _LegState = value
        End Set
    End Property
    Public Property Billable() As String
        Get
            Return _Billable
        End Get
        Set(ByVal value As String)
            _Billable = value
        End Set
    End Property
    Public Property ArrivalDate() As String
        Get
            Return _ArrivalDate
        End Get
        Set(ByVal value As String)
            _ArrivalDate = value
        End Set
    End Property
    Public Property ArrivalTime() As String
        Get
            Return _ArrivalTime
        End Get
        Set(ByVal value As String)
            _ArrivalTime = value
        End Set
    End Property
    Public Property DepartureTimeZone() As String
        Get
            Return _DepartureTimeZone
        End Get
        Set(ByVal value As String)
            _DepartureTimeZone = value
        End Set
    End Property
    Public Property ArrivalTimeZone() As String
        Get
            Return _ArrivalTimeZone
        End Get
        Set(ByVal value As String)
            _ArrivalTimeZone = value
        End Set
    End Property
    Public Property OptimizerRun() As String
        Get
            Return _OptimizerRun
        End Get
        Set(ByVal value As String)
            _OptimizerRun = value
        End Set
    End Property
    Public Property OptimizerID() As Integer?
        Get
            Return _OptimizerID
        End Get
        Set(ByVal value As Integer?)
            _OptimizerID = value
        End Set
    End Property
    Public Property PIC() As String
        Get
            Return _PIC
        End Get
        Set(ByVal value As String)
            _PIC = value
        End Set
    End Property
    Public Property SIC() As String
        Get
            Return _SIC
        End Get
        Set(ByVal value As String)
            _SIC = value
        End Set
    End Property
    Public Property OutTime() As String
        Get
            Return _OutTime
        End Get
        Set(ByVal value As String)
            _OutTime = value
        End Set
    End Property
    Public Property OffTime() As String
        Get
            Return _OffTime
        End Get
        Set(ByVal value As String)
            _OffTime = value
        End Set
    End Property
    Public Property OnTime() As String
        Get
            Return _OnTime
        End Get
        Set(ByVal value As String)
            _OnTime = value
        End Set
    End Property
    Public Property InTime() As String
        Get
            Return _InTime
        End Get
        Set(ByVal value As String)
            _InTime = value
        End Set
    End Property
    Public Property tripcost() As Decimal
        Get
            Return _tripcost
        End Get
        Set(ByVal value As Decimal)
            _tripcost = value
        End Set
    End Property
    Public Property triprevenue() As Decimal
        Get
            Return _triprevenue
        End Get
        Set(ByVal value As Decimal)
            _triprevenue = value
        End Set
    End Property
    Public Property PandL() As Decimal
        Get
            Return _PandL
        End Get
        Set(ByVal value As Decimal)
            _PandL = value
        End Set
    End Property
    Public Property DepartureDateGMTKey() As Date?
        Get
            Return _DepartureDateGMTKey
        End Get
        Set(ByVal value As Date?)
            _DepartureDateGMTKey = value
        End Set
    End Property
    Public Property CASUpdate() As Date?
        Get
            Return _CASUpdate
        End Get
        Set(ByVal value As Date?)
            _CASUpdate = value
        End Set
    End Property
    Public Property QuoteNumber() As String
        Get
            Return _QuoteNumber
        End Get
        Set(ByVal value As String)
            _QuoteNumber = value
        End Set
    End Property
    Public Property Versionx() As String
        Get
            Return _Versionx
        End Get
        Set(ByVal value As String)
            _Versionx = value
        End Set
    End Property
    Public Property BaseCode() As String
        Get
            Return _BaseCode
        End Get
        Set(ByVal value As String)
            _BaseCode = value
        End Set
    End Property
    Public Property OwnerCode() As String
        Get
            Return _OwnerCode
        End Get
        Set(ByVal value As String)
            _OwnerCode = value
        End Set
    End Property
    Public Property HomeAirport() As String
        Get
            Return _HomeAirport
        End Get
        Set(ByVal value As String)
            _HomeAirport = value
        End Set
    End Property
    Public Property ListOrder() As String
        Get
            Return _ListOrder
        End Get
        Set(ByVal value As String)
            _ListOrder = value
        End Set
    End Property
    Public Property VendorID() As String
        Get
            Return _VendorID
        End Get
        Set(ByVal value As String)
            _VendorID = value
        End Set
    End Property
    Public Property TripBaseCode() As String
        Get
            Return _TripBaseCode
        End Get
        Set(ByVal value As String)
            _TripBaseCode = value
        End Set
    End Property
    Public Property RateDetailx() As String
        Get
            Return _RateDetailx
        End Get
        Set(ByVal value As String)
            _RateDetailx = value
        End Set
    End Property
    Public Property LegBaseCode() As String
        Get
            Return _LegBaseCode
        End Get
        Set(ByVal value As String)
            _LegBaseCode = value
        End Set
    End Property
    Public Property QuotedEquipType() As String
        Get
            Return _QuotedEquipType
        End Get
        Set(ByVal value As String)
            _QuotedEquipType = value
        End Set
    End Property
    Public Property QuotedTail() As String
        Get
            Return _QuotedTail
        End Get
        Set(ByVal value As String)
            _QuotedTail = value
        End Set
    End Property
    Public Property OwnerRateType() As String
        Get
            Return _OwnerRateType
        End Get
        Set(ByVal value As String)
            _OwnerRateType = value
        End Set
    End Property
    Public Property QuotedBaseCode() As String
        Get
            Return _QuotedBaseCode
        End Get
        Set(ByVal value As String)
            _QuotedBaseCode = value
        End Set
    End Property
    Public Property TripStartTime() As String
        Get
            Return _TripStartTime
        End Get
        Set(ByVal value As String)
            _TripStartTime = value
        End Set
    End Property
    Public Property TST() As String
        Get
            Return _TST
        End Get
        Set(ByVal value As String)
            _TST = value
        End Set
    End Property
    Public Property RateDetail() As String
        Get
            Return _RateDetail
        End Get
        Set(ByVal value As String)
            _RateDetail = value
        End Set
    End Property
    Public Property WarningText() As String
        Get
            Return _WarningText
        End Get
        Set(ByVal value As String)
            _WarningText = value
        End Set
    End Property
    Public Property QuotedOwnerACCost() As String
        Get
            Return _QuotedOwnerACCost
        End Get
        Set(ByVal value As String)
            _QuotedOwnerACCost = value
        End Set
    End Property
    Public Property DivisionCode() As String
        Get
            Return _DivisionCode
        End Get
        Set(ByVal value As String)
            _DivisionCode = value
        End Set
    End Property
    Public Property OriginalCostWRepo() As Decimal
        Get
            Return _OriginalCostWRepo
        End Get
        Set(ByVal value As Decimal)
            _OriginalCostWRepo = value
        End Set
    End Property
    Public Property OriginalRepoDetail() As String
        Get
            Return _OriginalRepoDetail
        End Get
        Set(ByVal value As String)
            _OriginalRepoDetail = value
        End Set
    End Property
    Public Property ProRatedRevenue() As Decimal
        Get
            Return _ProRatedRevenue
        End Get
        Set(ByVal value As Decimal)
            _ProRatedRevenue = value
        End Set
    End Property
    Public Property ScrubText() As String
        Get
            Return _ScrubText
        End Get
        Set(ByVal value As String)
            _ScrubText = value
        End Set
    End Property
    Public Property ClientDetail() As String
        Get
            Return _ClientDetail
        End Get
        Set(ByVal value As String)
            _ClientDetail = value
        End Set
    End Property
    Public Property Pinned() As String
        Get
            Return _Pinned
        End Get
        Set(ByVal value As String)
            _Pinned = value
        End Set
    End Property
    Public Property Version() As String
        Get
            Return _Version
        End Get
        Set(ByVal value As String)
            _Version = value
        End Set
    End Property
End Class
<Serializable>
Public Class WeightClass
    Private _ACType As String
    Private _AircraftWC As String
    Public Property AircraftWeightClass() As String
        Get
            Return _AircraftWC
        End Get
        Set(ByVal value As String)
            _AircraftWC = value
        End Set
    End Property
    Public Property AircraftType() As String

        Get
            Return _ACType
        End Get
        Set(ByVal value As String)
            _ACType = value
        End Set
    End Property
End Class
<Serializable>
Public Class FCDRList
    Private m_keyid As String
    Private m_priorTail As String
    Private m_casrecordlist As String
    Private m_fosrecordlist As String
    Private m_modelrunid As String
    Private m_modelrun As Integer
    Private m_savingsday0 As Decimal
    Private m_savingsday1 As Decimal
    Private m_savingsday2 As Decimal
    Private m_GMTstart As DateTime
    Private m_totalsavings As Decimal
    Private m_carrieracceptstatus As String
    Private m_istrade As Boolean
    Private m_carrieracceptid As String
    Private m_carrieracceptdate As DateTime?
    Private m_deltanonrevmiles As Integer
    Private m_carrierid As Integer
    Private m_dynamiccost As Boolean
    Public Property DynamicCost() As Boolean
        Get
            Return m_dynamiccost
        End Get
        Set(ByVal value As Boolean)
            m_dynamiccost = value
        End Set
    End Property
    Public Property CarrierID() As Integer
        Get
            Return m_carrierid
        End Get
        Set(ByVal value As Integer)
            m_carrierid = value
        End Set
    End Property
    Public Property DeltaNonRevMiles() As Integer
        Get
            Return m_deltanonrevmiles
        End Get
        Set(ByVal value As Integer)
            m_deltanonrevmiles = value
        End Set
    End Property
    Public Property CarrierAcceptDate() As DateTime?
        Get
            Return m_carrieracceptdate
        End Get
        Set(ByVal value As DateTime?)
            m_carrieracceptdate = value
        End Set
    End Property
    Public Property CarrierAcceptID() As String
        Get
            Return m_carrieracceptid
        End Get
        Set(ByVal value As String)
            m_carrieracceptid = value
        End Set
    End Property
    Public Property isTrade() As Boolean
        Get
            Return m_istrade
        End Get
        Set(ByVal value As Boolean)
            m_istrade = value
        End Set
    End Property
    Public Property CarrierAcceptStatus() As String
        Get
            Return m_carrieracceptstatus
        End Get
        Set(ByVal value As String)
            m_carrieracceptstatus = value
        End Set
    End Property
    Public Property TotalSavings() As Decimal
        Get
            Return m_totalsavings
        End Get
        Set(ByVal value As Decimal)
            m_totalsavings = value
        End Set
    End Property
    Public Property GMTStart() As DateTime
        Get
            Return m_GMTstart

        End Get
        Set(ByVal value As DateTime)
            m_GMTstart = value
        End Set
    End Property
    Public Property SavingsDay2() As Decimal
        Get
            Return m_savingsday2
        End Get
        Set(ByVal value As Decimal)
            m_savingsday2 = value
        End Set
    End Property
    Public Property SavingsDay1() As Decimal
        Get
            Return m_savingsday1
        End Get
        Set(ByVal value As Decimal)
            m_savingsday1 = value
        End Set
    End Property
    Public Property SavingsDay0() As Decimal
        Get
            Return m_savingsday0
        End Get
        Set(ByVal value As Decimal)
            m_savingsday0 = value
        End Set
    End Property
    Public Property ModelRun() As Integer
        Get
            Return m_modelrun
        End Get
        Set(ByVal value As Integer)
            m_modelrun = value
        End Set
    End Property
    Public Property ModelRunID() As String
        Get
            Return m_modelrunid
        End Get
        Set(ByVal value As String)
            m_modelrunid = value
        End Set
    End Property
    Public Property FOSRecordList() As String
        Get
            Return m_fosrecordlist
        End Get
        Set(ByVal value As String)
            m_fosrecordlist = value
        End Set
    End Property
    Public Property CASRecordList() As String
        Get
            Return m_casrecordlist
        End Get
        Set(ByVal value As String)
            m_casrecordlist = value
        End Set
    End Property
    Public Property PriorTailNumber() As String
        Get
            Return m_priorTail
        End Get
        Set(ByVal value As String)
            m_priorTail = value
        End Set
    End Property
    <Key>
    Public Property keyid() As String
        Get
            Return m_keyid
        End Get
        Set(ByVal value As String)
            m_keyid = value
        End Set
    End Property
    Public ReadOnly Property PDFlink() As String
        Get
            Return "~/FCDRpages/" & keyid & ".pdf"
        End Get
    End Property
    Public ReadOnly Property FCDRPage() As String
        Get
            Return "~/FlightChangeDetail.aspx?key=" & keyid
        End Get
    End Property
End Class
Public Class PanelRecord
    'Private _CasModification As String
    Private M_CASRecord As CASFlightsOptimizerRecord
    Private M_FOSRecord As FOSFlightsOptimizerRecord
    Private M_ACW As String
    Private M_FCDR_Key As String
    Private M_Primary As String
    Private M_casmod As String
    Public Property Starttail() As String
        Get
            Return M_Primary
        End Get
        Set(ByVal value As String)
            M_Primary = value
        End Set
    End Property
    Public Property FCDR_Key() As String
        Get
            Return M_FCDR_Key
        End Get
        Set(ByVal value As String)
            M_FCDR_Key = value
        End Set
    End Property
    <NotMapped>
    Public ReadOnly Property TripNumber() As String
        Get
            Return If(FOSRecord Is Nothing, Trim(CASRecord.TripNumber), Trim(FOSRecord.TripNumber))
        End Get
    End Property
    <NotMapped>
    Public ReadOnly Property TailNumber() As String
        Get
            Return If(FOSRecord Is Nothing, Trim(CASRecord.AircraftRegistration), Trim(FOSRecord.AC))
        End Get
    End Property
    <NotMapped>
    Public ReadOnly Property HomeBase() As String
        Get
            Return If(FOSRecord Is Nothing, Trim(CASRecord.BaseCode), Trim(FOSRecord.BaseCode))
        End Get
    End Property
    <NotMapped>
    Public ReadOnly Property ACType() As String
        Get
            Return If(FOSRecord Is Nothing, Trim(CASRecord.aircrafttype), Trim(FOSRecord.AircraftType))
        End Get
    End Property
    <NotMapped>
    Public ReadOnly Property DateTimeGMT() As DateTime
        Get
            Return If(FOSRecord Is Nothing, CASRecord.DepartureTime, FOSRecord.DateTimeGMT)
        End Get
    End Property
    <NotMapped>
    Public ReadOnly Property PanelKey() As String
        Get
            Return If(FOSRecord Is Nothing, CASRecord.ID, FOSRecord.id)
        End Get
    End Property
    Public Property WeightClass() As String
        Get
            Return M_ACW
        End Get
        Set(value As String)
            M_ACW = value
        End Set
    End Property
    Public Property FOSRecord() As FOSFlightsOptimizerRecord
        Get
            Return M_FOSRecord
        End Get
        Set(ByVal value As FOSFlightsOptimizerRecord)
            M_FOSRecord = value
        End Set
    End Property
    Public Property CASRecord() As CASFlightsOptimizerRecord
        Get
            Return M_CASRecord
        End Get
        Set(ByVal value As CASFlightsOptimizerRecord)
            M_CASRecord = value
        End Set
    End Property
    <NotMapped>
    Public Property CasModification() As String
        Get
            Return M_casmod
        End Get
        Set(value As String)
            M_casmod = value
        End Set
    End Property
    ' Public sub SortByStarttail

    'Return If(FOSRecord Is Nothing, "Added", If(CASRecord Is Nothing, "Removed", If(CASRecord.PriorTail Is Nothing Or CASRecord.PriorTail = "", "Unchanged", Trim(CASRecord.PriorTail))))

End Class
Public Class PanelDisplay
    Private M_PanelRecords As List(Of PanelRecord)
    Private M_actype As String
    Private M_base As String
    Private M_wc As String
    Private M_tripnumber As String
    Private M_TailNumber As String
    Private M_FCDR_Key As String
    Private M_fcdrcostd0 As Double
    Private M_fcdrcostd1 As Double
    Private M_fcdrcostd2 As Double
    Private M_nrm As Double
    Private m_modelnumber As String
    Private m_totalsavings As Double
    Private M_RevenueRecords As List(Of baseRevenue)
    Public Property RevenueRecords() As List(Of baseRevenue)
        Get
            Return M_RevenueRecords
        End Get
        Set(ByVal value As List(Of baseRevenue))
            M_RevenueRecords = value
        End Set
    End Property
    Public Property TotalSavings() As Double
        Get
            Return m_totalsavings
        End Get
        Set(ByVal value As Double)
            m_totalsavings = value
        End Set
    End Property
    Public Property ModelNumber() As String
        Get
            Return m_modelnumber
        End Get
        Set(ByVal value As String)
            m_modelnumber = value
        End Set
    End Property
    Public Property NRM() As Double
        Get
            Return M_nrm
        End Get

        Set(ByVal value As Double)
            M_nrm = value
        End Set
    End Property
    Public Property dcostday2() As Double
        Get
            Return M_fcdrcostd2
        End Get
        Set(ByVal value As Double)
            M_fcdrcostd2 = value
        End Set
    End Property
    Public Property dcostday1() As Double
        Get
            Return M_fcdrcostd1
        End Get
        Set(ByVal value As Double)
            M_fcdrcostd1 = value
        End Set
    End Property
    Public Property dcostday0() As Double
        Get
            Return M_fcdrcostd0
        End Get
        Set(ByVal value As Double)
            M_fcdrcostd0 = value
        End Set
    End Property
    Public Property FCDR_Key() As String
        Get
            Return M_FCDR_Key
        End Get
        Set(ByVal value As String)
            M_FCDR_Key = value
        End Set
    End Property
    Public Property TailNumber() As String
        Get
            Return M_TailNumber
        End Get
        Set(ByVal value As String)
            M_TailNumber = value
        End Set
    End Property
    Public Property TripNumber() As String
        Get
            Return M_tripnumber
        End Get
        Set(ByVal value As String)
            M_tripnumber = value
        End Set
    End Property
    Public Property WeightClass() As String
        Get
            Return M_wc
        End Get
        Set(ByVal value As String)
            M_wc = value
        End Set
    End Property
    Public Property Basecode() As String
        Get
            Return M_base
        End Get
        Set(ByVal value As String)
            M_base = value
        End Set
    End Property
    Public Property AircraftType() As String
        Get
            Return M_actype
        End Get
        Set(ByVal value As String)
            M_actype = value
        End Set
    End Property
    Public Property PanelRecord() As List(Of PanelRecord)
        Get
            Return M_PanelRecords
        End Get
        Set(ByVal value As List(Of PanelRecord))
            M_PanelRecords = value
        End Set
    End Property
End Class
Public Class fcdrColorlist
    Private M_dictionary As Dictionary(Of String, String)
    Private M_FCDR_Key As String
    Public Property FCDR_Key() As String
        Get
            Return M_FCDR_Key
        End Get
        Set(ByVal value As String)
            M_FCDR_Key = value
        End Set
    End Property
    Public Property changecolor_dictionary() As Dictionary(Of String, String)
        Get
            Return M_dictionary
        End Get
        Set(ByVal value As Dictionary(Of String, String))
            M_dictionary = value
        End Set
    End Property
End Class
Public Class CASFCDRFlight
    Private M_id As String
    Private M_aircraftregistration As String
    Private M_tripnumber As String
    Public Property tripnumber() As String
        Get
            Return M_tripnumber
        End Get
        Set(ByVal value As String)
            M_tripnumber = value
        End Set
    End Property
    Public Property aircraftregistration() As String
        Get
            Return M_aircraftregistration
        End Get
        Set(ByVal value As String)
            M_aircraftregistration = value
        End Set
    End Property
    Public Property id() As String
        Get
            Return M_id
        End Get
        Set(ByVal value As String)
            M_id = value
        End Set
    End Property
    Public Function SortBYID()
        Dim sorter As IComparer = New IDSortHelper()
    End Function
    Private Class IDSortHelper
        Implements IComparer

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Throw New NotImplementedException()
        End Function

    End Class
End Class
Public Class FOSFCDRflight
    'ac, tripnumber, DepartureAirportICAO, arrivalAirportICAO
    Private M_ac As String
    Private M_trip As String
    Private M_DepartICAO As String
    Private M_ArriveICAO As String
    Private M_id As String
    Public Property id() As String
        Get
            Return M_id
        End Get
        Set(ByVal value As String)
            M_id = value
        End Set
    End Property
    Public Property ArriveICAO() As String
        Get
            Return M_ArriveICAO
        End Get
        Set(ByVal value As String)
            M_ArriveICAO = value
        End Set
    End Property
    Public Property DepartICAO() As String
        Get
            Return M_DepartICAO
        End Get
        Set(ByVal value As String)
            M_DepartICAO = value
        End Set
    End Property
    Public Property tripnumber() As String
        Get
            Return M_trip
        End Get
        Set(ByVal value As String)
            M_trip = value
        End Set
    End Property
    Public Property ac() As String
        Get
            Return M_ac
        End Get
        Set(ByVal value As String)
            M_ac = value
        End Set
    End Property

End Class
Public Class OptimizerWeight
    Private _ID As Integer
    Private _CarrierID As Integer
    Private _OptimizerRun As String
    Private _Day As Integer
    Private _Percentage As Integer
    Private _FleetType As String
    Public Property ID() As Integer
        Get
            Return _ID
        End Get
        Set(ByVal value As Integer)
            _ID = value
        End Set
    End Property
    Public Property CarrierID() As Integer
        Get
            Return _carrierid
        End Get
        Set(ByVal value As Integer)
            _carrierid = value
        End Set
    End Property
    Public Property OptimizerRun() As String
        Get
            Return _OptimizerRun
        End Get
        Set(ByVal value As String)
            _OptimizerRun = value
        End Set
    End Property
    Public Property Day() As Integer
        Get
            Return _Day
        End Get
        Set(ByVal value As Integer)
            _Day = value
        End Set
    End Property
    Public Property Percentage() As Integer
        Get
            Return _Percentage
        End Get
        Set(ByVal value As Integer)
            _Percentage = value
        End Set
    End Property
    Public Property FleetType() As String
        Get
            Return _FleetType
        End Get
        Set(ByVal value As String)
            _FleetType = value
        End Set
    End Property

End Class
Public Class OptimizerRequest
    Private m_id As Integer
    Private m_GMTStart As DateTime
    Private m_description As String
    Public Property Description() As String
        Get
            Return m_description
        End Get
        Set(ByVal value As String)
            m_description = value
        End Set
    End Property
    Public Property GMTStart() As DateTime
        Get
            Return m_GMTStart
        End Get
        Set(ByVal value As DateTime)
            m_GMTStart = value
        End Set
    End Property
    Public Property ID() As Integer
        Get
            Return m_id
        End Get
        Set(ByVal value As Integer)
            m_id = value
        End Set
    End Property
End Class
Public Class PropertyComparer(Of T)
    Implements IEqualityComparer(Of T)
    Private _PropertyInfo As PropertyInfo

    ''' <summary>
    ''' Creates a new instance of PropertyComparer.
    ''' </summary>
    ''' <param name="propertyName">The name of the property on type T 
    ''' to perform the comparison on.</param>
    Public Sub New(propertyName As String)
        'store a reference to the property info object for use during the comparison
        _PropertyInfo = GetType(T).GetProperty(propertyName, BindingFlags.GetProperty Or BindingFlags.Instance Or BindingFlags.[Public])
        If _PropertyInfo Is Nothing Then
            Throw New ArgumentException(String.Format("{0} is not a property of type {1}.", propertyName, GetType(T)))
        End If
    End Sub

#Region "IEqualityComparer<T> Members"

    Public Function Equals(x As T, y As T) As Boolean
        'get the current value of the comparison property of x and of y
        Dim xValue As Object = _PropertyInfo.GetValue(x, Nothing)
        Dim yValue As Object = _PropertyInfo.GetValue(y, Nothing)

        'if the xValue is null then we consider them equal if and only if yValue is null
        If xValue Is Nothing Then
            Return yValue Is Nothing
        End If

        'use the default comparer for whatever type the comparison property is.
        Return xValue.Equals(yValue)
    End Function

    Public Function GetHashCode(obj As T) As Integer
        'get the value of the comparison property out of obj
        Dim propertyValue As Object = _PropertyInfo.GetValue(obj, Nothing)

        If propertyValue Is Nothing Then
            Return 0
        Else

            Return propertyValue.GetHashCode()
        End If
    End Function

    Private Function IEqualityComparer_Equals(x As T, y As T) As Boolean Implements IEqualityComparer(Of T).Equals
        Throw New NotImplementedException()
    End Function

    Private Function IEqualityComparer_GetHashCode(obj As T) As Integer Implements IEqualityComparer(Of T).GetHashCode
        Throw New NotImplementedException()
    End Function

#End Region
End Class

Public Class baseRevenue
    Private m_basecode As String
    Private m_fosRevenue As Decimal
    Private m_casRevenue As Decimal
    Private m_grossprofitchange As Decimal
    Public Property GrossProfitChange() As Decimal
        Get
            Return m_grossprofitchange
        End Get
        Set(ByVal value As Decimal)
            m_grossprofitchange = value
        End Set
    End Property
    Public Property CasRevenue() As Decimal
        Get
            Return m_casRevenue
        End Get
        Set(ByVal value As Decimal)
            m_casRevenue = value
        End Set
    End Property
    Public Property FosRevenue() As Decimal
        Get
            Return m_fosRevenue
        End Get
        Set(ByVal value As Decimal)
            m_fosRevenue = value
        End Set
    End Property
    Public Property basecode() As String
        Get
            Return m_basecode
        End Get
        Set(ByVal value As String)
            m_basecode = value
        End Set
    End Property

End Class
Public Class fname_class
    Private m_airportname As String
    Private m_airportcode As String
    Private m_city As String
    Private m_state As String
    Public Property State() As String
        Get
            Return m_state
        End Get
        Set(ByVal value As String)
            m_state = value
        End Set
    End Property
    Public Property City() As String
        Get
            Return m_city
        End Get
        Set(ByVal value As String)
            m_city = value
        End Set
    End Property
    Public Property icao_id() As String
        Get
            Return m_airportcode
        End Get
        Set(ByVal value As String)
            m_airportcode = value
        End Set
    End Property
    Public Property airport_nm() As String
        Get
            Return m_airportname
        End Get
        Set(ByVal value As String)
            m_airportname = value
        End Set
    End Property
End Class
Public Class lookupac_class
    Private m_registration As String
    Private m_broker As String
    Private m_equipmenttype As String
    Private m_class As String
    Private m_operator As String
    Private m_vendorname As String
    Private m_hbCode As String
    Private m_fosac As String
    Private m_rtb As String
    Public Property RTB() As String
        Get
            Return m_rtb
        End Get
        Set(ByVal value As String)
            m_rtb = value
        End Set
    End Property
    Public Property FosAircraftID() As String
        Get
            Return m_fosac
        End Get
        Set(ByVal value As String)
            m_fosac = value
        End Set
    End Property
    Public Property HomeBaseAirportCode() As String
        Get
            Return m_hbCode
        End Get
        Set(ByVal value As String)
            m_hbCode = value
        End Set
    End Property
    Public Property VendorName() As String
        Get
            Return m_vendorname
        End Get
        Set(ByVal value As String)
            m_vendorname = value
        End Set
    End Property

    Public Property ACOperator() As String
        Get
            Return m_operator
        End Get
        Set(ByVal value As String)
            m_operator = value
        End Set
    End Property
    Public Property weightclass() As String
        Get
            Return m_class
        End Get
        Set(ByVal value As String)
            m_class = value
        End Set
    End Property
    Public Property TypeID() As String
        Get
            Return m_equipmenttype
        End Get
        Set(ByVal value As String)
            m_equipmenttype = value
        End Set
    End Property
    Public Property brokeraircraft() As String
        Get
            Return m_broker
        End Get
        Set(ByVal value As String)
            m_broker = value
        End Set
    End Property
    Public Property registration() As String
        Get
            Return m_registration
        End Get
        Set(ByVal value As String)
            m_registration = value
        End Set
    End Property
End Class
Public Class FCDRListDetail
    Private m_keyid As String
    Private m_FlightID As Integer
    Private m_ac As String
    Private m_tripnumber As String
    Private m_fromicao As String
    Private m_toicao As String
    Private m_Modification As String
    Private m_departdate As DateTime
    Public Property DepartDate() As DateTime
        Get
            Return m_departdate
        End Get
        Set(ByVal value As DateTime)
            m_departdate = value
        End Set
    End Property
    Public Property Modification() As String
        Get
            Return m_Modification
        End Get
        Set(ByVal value As String)
            m_Modification = value
        End Set
    End Property
    Public Property To_ICAO() As String
        Get
            Return m_toicao
        End Get
        Set(ByVal value As String)
            m_toicao = value
        End Set
    End Property
    Public Property From_ICAO() As String
        Get
            Return m_fromicao
        End Get
        Set(ByVal value As String)
            m_fromicao = value
        End Set
    End Property
    Public Property TripNumber() As String
        Get
            Return m_tripnumber
        End Get
        Set(ByVal value As String)
            m_tripnumber = value
        End Set
    End Property
    Public Property AC() As String
        Get
            Return m_ac
        End Get
        Set(ByVal value As String)
            m_ac = value
        End Set
    End Property
    <Key> <Column(Order:=1)>
    Public Property FlightID() As Integer
        Get
            Return m_FlightID
        End Get
        Set(ByVal value As Integer)
            m_FlightID = value
        End Set
    End Property
    <Key> <Column(Order:=0)>
    Public Property KeyID() As String
        Get
            Return m_keyid
        End Get
        Set(ByVal value As String)
            m_keyid = value
        End Set
    End Property
End Class
Public Class RejectedFlight
    Private _id As Integer
    Private _TripNumber As String
    Private _DepartureAirport As String
    Private _ArrivalAirport As String
    Private _RejectedOn As DateTime
    Private _Rejected As Boolean
    Private _TripType As String
    Private _FromDateGMT As DateTime
    Private _ToDateGMT As DateTime
    Private _AircraftRegistration As String
    Private _CarrierID As Integer
    Private _Reason As String
    Private _PriorTail As String
    Private _Action As String
    Private _Status As String
    Private _StatusComment As String
    Private _CASFOid As Integer
    Private _PriorTailSavings As String
    Private _FOSKEY As String
    Private _Version As String
    Private _Batch As String
    Public Property id() As Integer
        Get
            Return _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property
    Public Property TripNumber() As String
        Get
            Return _TripNumber
        End Get
        Set(ByVal value As String)
            _TripNumber = value
        End Set
    End Property
    Public Property DepartureAirport() As String
        Get
            Return _DepartureAirport
        End Get
        Set(ByVal value As String)
            _DepartureAirport = value
        End Set
    End Property
    Public Property ArrivalAirport() As String
        Get
            Return _ArrivalAirport
        End Get
        Set(ByVal value As String)
            _ArrivalAirport = value
        End Set
    End Property
    Public Property RejectedOn() As DateTime
        Get
            Return _RejectedOn
        End Get
        Set(ByVal value As DateTime)
            _RejectedOn = value
        End Set
    End Property
    Public Property Rejected() As Boolean
        Get
            Return _Rejected
        End Get
        Set(ByVal value As Boolean)
            _Rejected = value
        End Set
    End Property
    Public Property TripType() As String
        Get
            Return _TripType
        End Get
        Set(ByVal value As String)
            _TripType = value
        End Set
    End Property
    Public Property FromDateGMT() As DateTime
        Get
            Return _FromDateGMT
        End Get
        Set(ByVal value As DateTime)
            _FromDateGMT = value
        End Set
    End Property
    Public Property ToDateGMT() As DateTime
        Get
            Return _ToDateGMT
        End Get
        Set(ByVal value As DateTime)
            _ToDateGMT = value
        End Set
    End Property
    Public Property AircraftRegistration() As String
        Get
            Return _AircraftRegistration
        End Get
        Set(ByVal value As String)
            _AircraftRegistration = value
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
    Public Property Reason() As String
        Get
            Return _Reason
        End Get
        Set(ByVal value As String)
            _Reason = value
        End Set
    End Property
    Public Property PriorTail() As String
        Get
            Return _PriorTail
        End Get
        Set(ByVal value As String)
            _PriorTail = value
        End Set
    End Property
    Public Property Action() As String
        Get
            Return _Action
        End Get
        Set(ByVal value As String)
            _Action = value
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
    Public Property StatusComment() As String
        Get
            Return _StatusComment
        End Get
        Set(ByVal value As String)
            _StatusComment = value
        End Set
    End Property
    Public Property CASFOid() As Integer
        Get
            Return _CASFOid
        End Get
        Set(ByVal value As Integer)
            _CASFOid = value
        End Set
    End Property
    Public Property PriorTailSavings() As String
        Get
            Return _PriorTailSavings
        End Get
        Set(ByVal value As String)
            _PriorTailSavings = value
        End Set
    End Property
    Public Property FOSKEY() As String
        Get
            Return _FOSKEY
        End Get
        Set(ByVal value As String)
            _FOSKEY = value
        End Set
    End Property
    Public Property Version() As String
        Get
            Return _Version
        End Get
        Set(ByVal value As String)
            _Version = value
        End Set
    End Property
    Public Property Batch() As String
        Get
            Return _Batch
        End Get
        Set(ByVal value As String)
            _Batch = value
        End Set
    End Property
End Class