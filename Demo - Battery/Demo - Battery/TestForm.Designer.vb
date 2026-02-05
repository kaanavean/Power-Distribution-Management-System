<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TestForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        BatteryClock = New Timer(components)
        lblPercent = New Label()
        PercentageBar = New ProgressBar()
        lblStatus = New Label()
        lblTimeToFull = New Label()
        lblTemp = New Label()
        lblCapacity = New Label()
        btnRefresh = New Button()
        lblDischargeRate = New Label()
        SuspendLayout()
        ' 
        ' BatteryClock
        ' 
        ' 
        ' lblPercent
        ' 
        lblPercent.AutoSize = True
        lblPercent.Location = New Point(12, 28)
        lblPercent.Name = "lblPercent"
        lblPercent.Size = New Size(91, 20)
        lblPercent.TabIndex = 0
        lblPercent.Text = "--% charged"
        ' 
        ' PercentageBar
        ' 
        PercentageBar.Location = New Point(143, 24)
        PercentageBar.Name = "PercentageBar"
        PercentageBar.Size = New Size(197, 29)
        PercentageBar.TabIndex = 1
        ' 
        ' lblStatus
        ' 
        lblStatus.AutoSize = True
        lblStatus.Location = New Point(12, 74)
        lblStatus.Name = "lblStatus"
        lblStatus.Size = New Size(126, 20)
        lblStatus.TabIndex = 2
        lblStatus.Text = "Waiting for data..."
        ' 
        ' lblTimeToFull
        ' 
        lblTimeToFull.AutoSize = True
        lblTimeToFull.Location = New Point(12, 120)
        lblTimeToFull.Name = "lblTimeToFull"
        lblTimeToFull.Size = New Size(105, 20)
        lblTimeToFull.TabIndex = 3
        lblTimeToFull.Text = "--m remaining"
        ' 
        ' lblTemp
        ' 
        lblTemp.AutoSize = True
        lblTemp.Location = New Point(12, 167)
        lblTemp.Name = "lblTemp"
        lblTemp.Size = New Size(168, 20)
        lblTemp.TabIndex = 4
        lblTemp.Text = "Temp: waiting for data..."
        ' 
        ' lblCapacity
        ' 
        lblCapacity.AutoSize = True
        lblCapacity.Location = New Point(12, 214)
        lblCapacity.Name = "lblCapacity"
        lblCapacity.Size = New Size(126, 20)
        lblCapacity.TabIndex = 5
        lblCapacity.Text = "Waiting for data..."
        ' 
        ' btnRefresh
        ' 
        btnRefresh.Location = New Point(246, 260)
        btnRefresh.Name = "btnRefresh"
        btnRefresh.Size = New Size(94, 29)
        btnRefresh.TabIndex = 6
        btnRefresh.Text = "Refresh"
        btnRefresh.UseVisualStyleBackColor = True
        ' 
        ' lblDischargeRate
        ' 
        lblDischargeRate.AutoSize = True
        lblDischargeRate.Location = New Point(12, 260)
        lblDischargeRate.Name = "lblDischargeRate"
        lblDischargeRate.Size = New Size(126, 20)
        lblDischargeRate.TabIndex = 7
        lblDischargeRate.Text = "Waiting for data..."
        ' 
        ' TestForm
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(352, 299)
        Controls.Add(lblDischargeRate)
        Controls.Add(btnRefresh)
        Controls.Add(lblCapacity)
        Controls.Add(lblTemp)
        Controls.Add(lblTimeToFull)
        Controls.Add(lblStatus)
        Controls.Add(PercentageBar)
        Controls.Add(lblPercent)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Name = "TestForm"
        Text = "Demo - Battery"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents BatteryClock As Timer
    Friend WithEvents lblPercent As Label
    Friend WithEvents PercentageBar As ProgressBar
    Friend WithEvents lblStatus As Label
    Friend WithEvents lblTimeToFull As Label
    Friend WithEvents lblTemp As Label
    Friend WithEvents lblCapacity As Label
    Friend WithEvents btnRefresh As Button
    Friend WithEvents lblDischargeRate As Label

End Class
