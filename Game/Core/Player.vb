Public Class Player
    Public Enum PlayerType
        White
        Black
    End Enum

    Public Property PlayerTypeCompare As PlayerType

    Public Property CurrentPlayerClass As Player

    Public Shared CurrentPlayer As PlayerType = PlayerType.White
    Public Property CurrentPlayerProperty As PlayerType = PlayerType.White

    Public Shared Sub SwitchPlayer()
        If CurrentPlayer = PlayerType.White Then
            CurrentPlayer = PlayerType.Black
        Else
            CurrentPlayer = PlayerType.White
        End If

        UpdateCurrentPlayerLabel()
    End Sub

End Class