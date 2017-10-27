Public Class Enumerations

    'Public Enum FlightTypes

    'End Enum

    '20081209 - pab - change book flight screen to match d2d
    'do not change order..synched to dB
    Public Enum UserTypes
        Guest
        RegisteredUser
    End Enum

    Public Enum NavigationDestinationTypes
        Home
        Confirmation
        Login
        Change_Password
        User_Information
        Book_Flight
        Logout
    End Enum

    'DO NOT CHANGE THE ORDER OF ENUMS
    Public Enum PaymentCardTypes
        Visa
        Mastercard
        Amex
        Other
    End Enum

    Public Enum FlightStatusTypes
        All
        Cancelled
        Confirmed
        Pending
    End Enum

    '20130111 - pab - show next 7 days instead of sun-sat
    Public Enum DayOfWeek
        Sunday
        Monday
        Tuesday
        Wednesday
        Thursday
        Friday
        Saturday
    End Enum

End Class
