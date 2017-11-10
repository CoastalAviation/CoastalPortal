Imports System.Data.Entity
Imports CoastalPortal.AirTaxi

Namespace Models
    Public Class PortalContext
        Inherits DbContext
        Public Sub New()
            MyBase.New(PortalDB)
            Configuration.ProxyCreationEnabled = False
        End Sub
        Protected Overrides Sub OnModelCreating(ByVal modelBuilder As DbModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.Entity(Of PortalMember).ToTable("Members")
        End Sub
        Public Property Members As DbSet(Of PortalMember)
    End Class

    Public Class OptimizerContext
        Inherits DbContext
        Public Sub New()
            MyBase.New(OptimizerDB)
            Configuration.ProxyCreationEnabled = False
        End Sub
        Protected Overrides Sub OnModelCreating(ByVal modelBuilder As DbModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.Entity(Of CarrierProfile).HasKey(Function(x) New With {x.carrierid})
            modelBuilder.Entity(Of WeightClass).HasKey(Function(x) New With {x.AircraftType})
            modelBuilder.Entity(Of FOSFlightsOptimizerRecord).ToTable("FOSFlightsOptimizer")
            modelBuilder.Entity(Of CASFlightsOptimizerRecord).ToTable("CASFlightsOptimizer")
            modelBuilder.Entity(Of WeightClass).ToTable("AircraftWeightClass")
            modelBuilder.Entity(Of UserPB).ToTable("UserPB")
            modelBuilder.Entity(Of optimizerLog).ToTable("OptimizerLog")
            modelBuilder.Entity(Of FCDRList).ToTable("FCDRList")
            modelBuilder.Entity(Of OptimizerRequest).ToTable("OptimizerRequest")
            modelBuilder.Entity(Of FCDRListDetail).ToTable("FCDRListDetail")
            modelBuilder.Entity(Of RejectedFlight).ToTable("RejectedFlights")
            ' modelBuilder.Entity(Of CASFlightsOptimizerRecord).HasRequired(Function(x) x.WeightClass).WithMany().HasForeignKey(Function(y) New With {y.aircrafttype})
            'modelBuilder.Entity(Of FOSFlightsOptimizerRecord).HasRequired(Function(x) x.WeightClass).WithMany().HasForeignKey(Function(y) New With {y.AircraftType})

        End Sub
        Public Property CarrierProfiles As DbSet(Of CarrierProfile)
        Public Property AircraftWeightClass As DbSet(Of WeightClass)
        Public Property CASFlightsOptimizer As DbSet(Of CASFlightsOptimizerRecord)
        Public Property FOSFlightsOptimizer As DbSet(Of FOSFlightsOptimizerRecord)
        Public Property UserPB As DbSet(Of UserPB)
        Public Property OptimizerLog As DbSet(Of optimizerLog)
        Public Property FCDRList As DbSet(Of FCDRList)
        Public Property OptimizerRequest As DbSet(Of OptimizerRequest)
        Public Property FCDRListDetail As DbSet(Of FCDRListDetail)
        Public Property RejectedFlights As DbSet(Of RejectedFlight)
    End Class

End Namespace