Imports System.Management

Public Class BatteryInformation
    Public ReadOnly Property CapacityPercent As Integer
        Get
            Dim sps As New PowerStatusHelper.SystemPowerStatus()
            If PowerStatusHelper.GetSystemPowerStatus(sps) Then
                ' 255 = unknown status
                If sps.BatteryLifePercent <= 100 Then
                    Return CInt(sps.BatteryLifePercent)
                End If
            End If
            Return -1
        End Get
    End Property

    Public ReadOnly Property RemainingCapacityMWh As Integer
        Get
            Return GetWmiBatteryValue("RemainingCapacity")
        End Get
    End Property

    Public ReadOnly Property FullChargeCapacityMWh As Integer
        Get
            Dim value As Integer = GetWmiBatteryValue("FullChargeCapacity")
            If value <= 0 Then
                Try
                    Dim searcher As New ManagementObjectSearcher("root\WMI", "SELECT FullChargedCapacity FROM BatteryFullChargedCapacity")
                    For Each obj As ManagementObject In searcher.Get()
                        Return CInt(obj("FullChargedCapacity"))
                    Next
                Catch
                    Return -1
                End Try
            End If

            Return value
        End Get
    End Property

    Public ReadOnly Property EstimatedRunTimeMinutes As Integer
        Get
            Dim sps As New PowerStatusHelper.SystemPowerStatus()
            If PowerStatusHelper.GetSystemPowerStatus(sps) Then
                ' -1 = unknown status
                If sps.BatteryLifeTime <> -1 Then
                    Return sps.BatteryLifeTime / 60
                End If
            End If
            Return -1
        End Get
    End Property

    Public ReadOnly Property BatteryTemperatureCelsius As Double
        Get
            Try
                Dim searcher As New ManagementObjectSearcher("root\WMI", "SELECT Charging, Temperature FROM BatteryStatus")

                For Each obj As ManagementObject In searcher.Get()
                    If obj("Temperature") IsNot Nothing Then
                        Dim tempRaw As Double = CType(obj("Temperature"), Double)
                        If tempRaw > 0 Then
                            Dim celsius = (tempRaw / 10.0) - 273.15
                            If celsius > -20 AndAlso celsius < 100 Then
                                Return celsius
                            End If
                        End If
                    End If
                Next
            Catch

            End Try

            Return Double.NaN
        End Get
    End Property

    Private Function GetWmiBatteryValue(propertyName As String) As Integer
        Try
            Dim searcher1 As New ManagementObjectSearcher("root\WMI", "SELECT " & propertyName & " FROM BatteryStatus")
            For Each obj As ManagementObject In searcher1.Get()
                Dim val = CInt(obj(propertyName))
                If val > 0 AndAlso val <> -1 Then Return val
            Next

            Dim wmiName As String = ""
            Select Case propertyName
                Case "FullChargeCapacity" : wmiName = "FullChargeCapacity"
                Case "RemainingCapacity" : wmiName = "EstimatedChargeRemaining"
            End Select

            If wmiName <> "" Then
                Dim searcher2 As New ManagementObjectSearcher("root\CIMV2", "SELECT " & wmiName & " FROM Win32_Battery")
                For Each obj As ManagementObject In searcher2.Get()
                    Return CInt(obj(wmiName))
                Next
            End If
        Catch
            Return -1
        End Try
        Return -1
    End Function

End Class

Friend Class PowerStatusHelper
    <System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>
    Public Structure SystemPowerStatus
        Public ACLineStatus As Byte
        Public BatteryFlag As Byte
        Public BatteryLifePercent As Byte
        Public SystemStatusFlag As Byte
        Public BatteryLifeTime As Integer
        Public BatteryFullLifeTime As Integer
    End Structure

    <System.Runtime.InteropServices.DllImport("kernel32.dll")>
    Public Shared Function GetSystemPowerStatus(ByRef lpSystemPowerStatus As SystemPowerStatus) As Boolean
    End Function
End Class