Imports Power_Distribution_Management_System

Public Class TestForm
    Private batterySvc As New BatteryInformation()
    Private chargeSvc As New Charging()

    Private Sub TestForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BatteryClock.Interval = 5000
        BatteryClock.Enabled = True
        UpdateUI()
    End Sub

    Private Sub BatteryClock_Tick(sender As Object, e As EventArgs) Handles BatteryClock.Tick
        UpdateUI()
    End Sub

    Private Sub UpdateUI()
        Dim currentPercent As Integer = batterySvc.CapacityPercent
        Dim curMWh As Integer = batterySvc.RemainingCapacityMWh
        Dim fullMWh As Integer = batterySvc.FullChargeCapacityMWh
        Dim isCharging As Boolean = chargeSvc.IsCharging

        If currentPercent >= 0 Then
            lblPercent.Text = $"{currentPercent}% charged"
            PercentageBar.Value = Math.Min(currentPercent, 100)
        Else
            lblPercent.Text = "Charge unknown"
        End If

        If chargeSvc.HasNoBattery Then
            lblStatus.Text = "No battery found"
            lblStatus.ForeColor = Color.Red
            lblTimeToFull.Text = "-"
            lblDischargeRate.Text = "Rate: 0 W"
        Else
            If fullMWh <= 0 AndAlso currentPercent > 0 AndAlso curMWh > 0 Then
                fullMWh = CInt((curMWh / currentPercent) * 100)
            End If

            If curMWh > 0 AndAlso fullMWh > 0 Then
                lblCapacity.Text = $"Details: {curMWh} mWh / {fullMWh} mWh"
            Else
                lblCapacity.Text = "Capacity values not available"
            End If

            Dim minutesResult = chargeSvc.GetDeltaTime(curMWh, fullMWh, isCharging)

            Dim watts As Double = chargeSvc.CurrentRateWatts

            If isCharging Then
                lblStatus.Text = "Cable powered (Charging)"
                lblStatus.ForeColor = Color.Green
                lblDischargeRate.Text = $"Charging rate: {watts:F1} W"

                If minutesResult > 0 Then
                    Dim h = minutesResult \ 60
                    Dim m = minutesResult Mod 60
                    lblTimeToFull.Text = If(h > 0, $"{h}h {m}m until full", $"{m} Min. until full")
                Else
                    lblTimeToFull.Text = "Calculating charging time..."
                End If
            Else
                lblStatus.Text = "On Battery"
                lblStatus.ForeColor = Color.Black
                lblDischargeRate.Text = $"Usage: {watts:F1} W"

                If minutesResult > 0 Then
                    Dim h = minutesResult \ 60
                    Dim m = minutesResult Mod 60
                    lblTimeToFull.Text = If(h > 0, $"Remaining: {h}h {m}m", $"Remaining: {m} Min.")
                Else
                    lblTimeToFull.Text = "Calculating remaining time..."
                End If
            End If
        End If

        Dim bTemp As Double = batterySvc.BatteryTemperatureCelsius
        If Double.IsNaN(bTemp) Then
            lblTemp.Text = "Temp: Not supported"
            lblTemp.ForeColor = Color.Gray
        Else
            lblTemp.Text = $"Battery-Temperature: {bTemp:F1} °C"
            lblTemp.ForeColor = If(bTemp > 45, Color.Red, Color.DarkGreen)
        End If
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        UpdateUI()
    End Sub
End Class