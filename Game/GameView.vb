Public Class GameView
    Private Sub GameView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GameService.StartGame()
    End Sub
End Class
