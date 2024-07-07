Public Class GameView
    Private Sub GameView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Magnus"
        GameService.UpdateCurrentPlayerLabel()
        GamePanel.Visible = False
    End Sub

    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles ExitButton.Click
        GameService.DisposeGame()
        StartView.Visible = True
        StartView.StartButton.Enabled = True
        Me.Visible = False

    End Sub

    Private Sub RestartButton_Click(sender As Object, e As EventArgs) Handles RestartButton.Click
        GameService.DisposeGame()
        GameService.StartGame()
    End Sub
End Class
