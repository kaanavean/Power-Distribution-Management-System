Imports System.Management
Public Class Charging

    Private Structure SystemPowerStatus
        Public ACLineStatus As Byte
        Public BatteryFlag As Byte
        Public BatteryLifePercent As Byte
        Public SystemStatusFlag As Byte
        Public BatteryLifeTime As Integer
        Public BatteryFullLifeTime As Integer
    End Structure

    <System.Runtime.InteropServices.DllImport("kernel32.dll")>
    Private Shared Function GetSystemPowerStatus(ByRef lpSystemPowerStatus As SystemPowerStatus) As Boolean
    End Function

    Public ReadOnly Property IsCharging As Boolean
        Get
            Dim sps As New SystemPowerStatus()
            If GetSystemPowerStatus(sps) Then
                Return sps.ACLineStatus = 1
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property HasNoBattery As Boolean
        Get
            Dim sps As New SystemPowerStatus()
            If GetSystemPowerStatus(sps) Then
                ' BatteryFlag 128 = "No System Battery"
                Return (sps.BatteryFlag And 128) = 128
            End If
            Return True
        End Get
    End Property

    Public Function GetCalculatedMinutesToFull() As Integer
        Try
            Dim searcher As New ManagementObjectSearcher("root\WMI", "SELECT * FROM BatteryStatus")

            For Each obj As ManagementObject In searcher.Get()
                Dim chargeRate As Integer = Math.Abs(CInt(obj("ChargeRate")))
                Dim remaining As Integer = CInt(obj("RemainingCapacity"))
                Dim full As Integer = CInt(obj("FullChargeCapacity"))

                If chargeRate > 0 AndAlso full > remaining AndAlso remaining > 0 Then
                    Dim energyNeeded As Integer = full - remaining
                    Return CInt((energyNeeded / chargeRate) * 60)
                End If
            Next
        Catch
            Return -1
        End Try
        Return -1
    End Function

    Private _lastEnergy As Integer = -1
    Private _lastTime As DateTime = DateTime.MinValue
    Private _currentRateMWhPerMin As Double = 0

    Public ReadOnly Property CurrentRateWatts As Double
        Get
            ' (mWh/min * 60) / 1000 = Watt
            Return Math.Abs(_currentRateMWhPerMin * 60 / 1000.0)
        End Get
    End Property

    Public Function GetDeltaTime(currentMWh As Integer, fullMWh As Integer, isCharging As Boolean) As Integer
        Dim now = DateTime.Now

        If _lastEnergy = -1 OrElse (isCharging AndAlso currentMWh < _lastEnergy) OrElse (Not isCharging AndAlso currentMWh > _lastEnergy) Then
            _lastEnergy = currentMWh
            _lastTime = now
            Return -1
        End If

        Dim timePassed = (now - _lastTime).TotalMinutes

        If timePassed > 0.25 Then
            Dim energyDiff = Math.Abs(currentMWh - _lastEnergy)
            If energyDiff > 0 Then
                _currentRateMWhPerMin = energyDiff / timePassed
                _lastEnergy = currentMWh
                _lastTime = now
            End If
        End If

        If _currentRateMWhPerMin > 0 Then
            Dim energyToCalculate = If(isCharging, fullMWh - currentMWh, currentMWh)

            If energyToCalculate <= 0 Then Return 0
            Return CInt(energyToCalculate / _currentRateMWhPerMin)
        End If

        Return -1
    End Function
End Class