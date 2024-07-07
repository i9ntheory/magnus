<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class GameView
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Me.GamePanel = New System.Windows.Forms.Panel()
        Me.ExitButton = New System.Windows.Forms.Button()
        Me.RestartButton = New System.Windows.Forms.Button()
        Me.CurrentPlayerLabel = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'GamePanel
        '
        Me.GamePanel.Location = New System.Drawing.Point(12, 12)
        Me.GamePanel.Name = "GamePanel"
        Me.GamePanel.Size = New System.Drawing.Size(413, 451)
        Me.GamePanel.TabIndex = 0
        '
        'ExitButton
        '
        Me.ExitButton.Location = New System.Drawing.Point(638, 12)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(75, 23)
        Me.ExitButton.TabIndex = 1
        Me.ExitButton.Text = "Exit"
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'RestartButton
        '
        Me.RestartButton.Location = New System.Drawing.Point(713, 12)
        Me.RestartButton.Name = "RestartButton"
        Me.RestartButton.Size = New System.Drawing.Size(75, 23)
        Me.RestartButton.TabIndex = 2
        Me.RestartButton.Text = "Restart"
        Me.RestartButton.UseVisualStyleBackColor = True
        '
        'CurrentPlayerLabel
        '
        Me.CurrentPlayerLabel.AutoSize = True
        Me.CurrentPlayerLabel.Location = New System.Drawing.Point(455, 17)
        Me.CurrentPlayerLabel.Name = "CurrentPlayerLabel"
        Me.CurrentPlayerLabel.Size = New System.Drawing.Size(70, 13)
        Me.CurrentPlayerLabel.TabIndex = 3
        Me.CurrentPlayerLabel.Text = "Player: White"
        '
        'GameView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.CurrentPlayerLabel)
        Me.Controls.Add(Me.RestartButton)
        Me.Controls.Add(Me.ExitButton)
        Me.Controls.Add(Me.GamePanel)
        Me.Name = "GameView"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents GamePanel As Panel
    Friend WithEvents ExitButton As Button
    Friend WithEvents RestartButton As Button
    Friend WithEvents CurrentPlayerLabel As Label
End Class
