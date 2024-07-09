<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StartView
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.StartPanel = New System.Windows.Forms.Panel()
        Me.WhyNameLabel = New System.Windows.Forms.Label()
        Me.StartButton = New System.Windows.Forms.Button()
        Me.Authors2Label = New System.Windows.Forms.Label()
        Me.Authors1Label = New System.Windows.Forms.Label()
        Me.DescriptionLabel = New System.Windows.Forms.Label()
        Me.TitleLabel = New System.Windows.Forms.Label()
        Me.ImageLogo = New System.Windows.Forms.PictureBox()
        Me.StartPanel.SuspendLayout()
        CType(Me.ImageLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StartPanel
        '
        Me.StartPanel.Controls.Add(Me.WhyNameLabel)
        Me.StartPanel.Controls.Add(Me.StartButton)
        Me.StartPanel.Controls.Add(Me.Authors2Label)
        Me.StartPanel.Controls.Add(Me.Authors1Label)
        Me.StartPanel.Controls.Add(Me.DescriptionLabel)
        Me.StartPanel.Controls.Add(Me.TitleLabel)
        Me.StartPanel.Controls.Add(Me.ImageLogo)
        Me.StartPanel.Location = New System.Drawing.Point(12, 12)
        Me.StartPanel.Name = "StartPanel"
        Me.StartPanel.Size = New System.Drawing.Size(776, 379)
        Me.StartPanel.TabIndex = 2
        '
        'WhyNameLabel
        '
        Me.WhyNameLabel.AutoSize = True
        Me.WhyNameLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WhyNameLabel.Location = New System.Drawing.Point(36, 316)
        Me.WhyNameLabel.Name = "WhyNameLabel"
        Me.WhyNameLabel.Size = New System.Drawing.Size(600, 15)
        Me.WhyNameLabel.TabIndex = 6
        Me.WhyNameLabel.Text = "Kenapa Magnus? Karena terinspirasi dari nama Magnus Carlsen yang merupakan Grandm" &
    "aster catur dunia."
        '
        'StartButton
        '
        Me.StartButton.Location = New System.Drawing.Point(39, 121)
        Me.StartButton.Name = "StartButton"
        Me.StartButton.Size = New System.Drawing.Size(137, 23)
        Me.StartButton.TabIndex = 4
        Me.StartButton.Text = "Mulai"
        Me.StartButton.UseVisualStyleBackColor = True
        '
        'Authors2Label
        '
        Me.Authors2Label.AutoSize = True
        Me.Authors2Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Authors2Label.Location = New System.Drawing.Point(205, 129)
        Me.Authors2Label.Name = "Authors2Label"
        Me.Authors2Label.Size = New System.Drawing.Size(188, 15)
        Me.Authors2Label.TabIndex = 3
        Me.Authors2Label.Text = "- 2311025 Raihan Pratama Putra"
        '
        'Authors1Label
        '
        Me.Authors1Label.AutoSize = True
        Me.Authors1Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Authors1Label.Location = New System.Drawing.Point(205, 103)
        Me.Authors1Label.Name = "Authors1Label"
        Me.Authors1Label.Size = New System.Drawing.Size(207, 15)
        Me.Authors1Label.TabIndex = 2
        Me.Authors1Label.Text = "- 2311032 Yehezkiel Dio Sinolungan"
        '
        'DescriptionLabel
        '
        Me.DescriptionLabel.AutoSize = True
        Me.DescriptionLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DescriptionLabel.Location = New System.Drawing.Point(205, 73)
        Me.DescriptionLabel.Name = "DescriptionLabel"
        Me.DescriptionLabel.Size = New System.Drawing.Size(244, 15)
        Me.DescriptionLabel.TabIndex = 1
        Me.DescriptionLabel.Text = "Sebuah implemetasi permainan catur oleh:"
        '
        'TitleLabel
        '
        Me.TitleLabel.AutoSize = True
        Me.TitleLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TitleLabel.Location = New System.Drawing.Point(32, 57)
        Me.TitleLabel.Name = "TitleLabel"
        Me.TitleLabel.Size = New System.Drawing.Size(137, 37)
        Me.TitleLabel.TabIndex = 0
        Me.TitleLabel.Text = "Magnus"
        '
        'ImageLogo
        '
        Me.ImageLogo.Image = Global.Magnus.My.Resources.Resources.BlackKing
        Me.ImageLogo.InitialImage = Global.Magnus.My.Resources.Resources.BlackKing
        Me.ImageLogo.Location = New System.Drawing.Point(208, 166)
        Me.ImageLogo.Name = "ImageLogo"
        Me.ImageLogo.Size = New System.Drawing.Size(130, 129)
        Me.ImageLogo.TabIndex = 5
        Me.ImageLogo.TabStop = False
        '
        'StartView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.StartPanel)
        Me.Name = "StartView"
        Me.Text = "StartView"
        Me.StartPanel.ResumeLayout(False)
        Me.StartPanel.PerformLayout()
        CType(Me.ImageLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents StartPanel As Panel
    Friend WithEvents StartButton As Button
    Friend WithEvents Authors2Label As Label
    Friend WithEvents Authors1Label As Label
    Friend WithEvents DescriptionLabel As Label
    Friend WithEvents TitleLabel As Label
    Friend WithEvents ImageLogo As PictureBox
    Friend WithEvents WhyNameLabel As Label
End Class
