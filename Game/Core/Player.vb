Public Class Player
    Public Enum PlayerType
        White
        Black
    End Enum

    Public Shared CurrentPlayer As PlayerType = PlayerType.White

    Public Shared Sub SwitchPlayer()
        If CurrentPlayer = PlayerType.White Then
            CurrentPlayer = PlayerType.Black
        Else
            CurrentPlayer = PlayerType.White
        End If
    End Sub
End Class