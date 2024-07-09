Public Class StartView
    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        GameView.Show()
        GameView.GamePanel.Visible = True
        GameService.StartGame()

        StartButton.Enabled = False

        Me.Visible = False

    End Sub

    Private Sub StartView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Magnus"
    End Sub

    Private Sub StartPanel_Paint(sender As Object, e As PaintEventArgs) Handles StartPanel.Paint

    End Sub
End Class