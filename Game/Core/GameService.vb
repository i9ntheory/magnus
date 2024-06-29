Public Module GameService
    Public Const Rows As Integer = 8
    Public Const Cols As Integer = 8

    Public PictureBoxes(Rows - 1, Cols - 1) As PictureBox
    Public Board(Rows - 1, Cols - 1) As String

    Public SelectedPiece As String
    Public SelectedRow As Integer
    Public SelectedCol As Integer

    Public WhiteCapturedPieces As New List(Of String)
    Public BlackCapturedPieces As New List(Of String)

    Public CurrentWhiteKingPosition() As Integer = {7, 4}
    Public CurrentBlackKingPosition() As Integer = {0, 4}

    Public Sub StartGame()
        Debug.WriteLine("StartGame: Starting the game")

        SetupBoard()
        PopulateBoard()
        SetupInitialPiecesPosition()

        Debug.WriteLine("StartGame: Game started successfully")
    End Sub

    Public Sub SetupBoard()
        Debug.WriteLine("SetupBoard: Setting up the board")

        ' Initialize the chess board with the initial positions
        ' P for Pawn, R for Rook, N for Knight, B for Bishop, Q for Queen, K for King
        ' Uppercase for white, lowercase for black
        Dim initialSetup As String =
           "rnbqkbnr" &
           "pppppppp" &
           "........" &
           "........" &
           "........" &
           "........" &
           "PPPPPPPP" &
           "RNBQKBNR"

        For i As Integer = 0 To initialSetup.Length - 1
            Dim row As Integer = i \ 8
            Dim col As Integer = i Mod 8
            Board(row, col) = initialSetup(i).ToString()

            Debug.WriteLine($"SetupBoard: Setting up piece {initialSetup(i)} at ({row}, {col})")
        Next

        Debug.WriteLine("SetupBoard: Board setup complete")
    End Sub

    Public Sub PopulateBoard()
        Debug.WriteLine("PopulateBoard: Populating the board with pieces")

        ' Populate the board with PictureBoxes to represent the pieces and squares
        Dim squareSize As Integer = 50 '
        For row As Integer = 0 To Rows - 1
            For col As Integer = 0 To Cols - 1
                Dim pb As New PictureBox()

                pb.Width = squareSize
                pb.Height = squareSize

                pb.Left = col * squareSize
                pb.Top = row * squareSize

                pb.BorderStyle = BorderStyle.FixedSingle

                ' Alternate the background color to create a checkerboard pattern
                If (row + col) Mod 2 = 0 Then
                    pb.BackColor = Color.White
                Else
                    pb.BackColor = Color.Gray
                End If

                AddHandler pb.Click, AddressOf PictureBox_Click
                PictureBoxes(row, col) = pb

                Debug.WriteLine($"PopulateBoard: Creating PictureBox at ({row}, {col})")

                GameView.GamePanel.Controls.Add(pb)

                Debug.WriteLine($"PopulateBoard: Adding PictureBox to GamePanel at ({row}, {col})")
            Next
        Next

        Debug.WriteLine("PopulateBoard: Board populated successfully")
    End Sub

    Private Sub RefreshBoard()
        Debug.WriteLine("RefreshBoard: Refreshing the board")

        For row As Integer = 0 To Rows - 1
            For col As Integer = 0 To Cols - 1
                Dim p As String = Board(row, col)
                If p <> "." Then
                    PictureBoxes(row, col).Image = GetPieceImage(p)
                    PictureBoxes(row, col).SizeMode = PictureBoxSizeMode.StretchImage

                    Debug.WriteLine($"RefreshBoard: Refreshing piece {p} at ({row}, {col})")
                End If
            Next
        Next

        Debug.WriteLine("RefreshBoard: Board refreshed successfully")
    End Sub

    Public Sub SetupInitialPiecesPosition()
        Debug.WriteLine("SetupInitialPiecesPosition: Setting up initial pieces position on the board")

        For row As Integer = 0 To Rows - 1
            For col As Integer = 0 To Cols - 1
                Dim p As String = Board(row, col)
                If p <> "." Then
                    PictureBoxes(row, col).Image = GetPieceImage(p)
                    PictureBoxes(row, col).SizeMode = PictureBoxSizeMode.StretchImage

                    Debug.WriteLine($"SetupInitialPiecesPosition: Setting piece {p} at ({row}, {col})")
                End If
            Next
        Next

        Debug.WriteLine("SetupInitialPiecesPosition: Initial pieces position setup complete")
    End Sub

    Public Function GetPieceImage(piece As String) As Image
        Select Case piece
            Case "P" : Return My.Resources.WhitePawn
            Case "R" : Return My.Resources.WhiteRook
            Case "N" : Return My.Resources.WhiteKnight
            Case "B" : Return My.Resources.WhiteBishop
            Case "Q" : Return My.Resources.WhiteQueen
            Case "K" : Return My.Resources.WhiteKing
            Case "p" : Return My.Resources.BlackPawn
            Case "r" : Return My.Resources.BlackRook
            Case "n" : Return My.Resources.BlackKnight
            Case "b" : Return My.Resources.BlackBishop
            Case "q" : Return My.Resources.BlackQueen
            Case "k" : Return My.Resources.BlackKing
            Case Else : Return Nothing
        End Select
    End Function

    Private Sub PictureBox_Click(sender As Object, e As EventArgs)
        Debug.WriteLine("PictureBox_Click: PictureBox clicked")

        Dim pb As PictureBox = DirectCast(sender, PictureBox)

        Dim row As Integer = pb.Top / pb.Height
        Dim col As Integer = pb.Left / pb.Width

        Dim currentPiece As String = Board(row, col)

        If (Char.IsLower(currentPiece) AndAlso Player.CurrentPlayer = Player.PlayerType.White) OrElse
           (Char.IsUpper(currentPiece) AndAlso Player.CurrentPlayer = Player.PlayerType.Black) Then
            Debug.WriteLine($"PictureBox_Click: Selected piece: {currentPiece} at ({row}, {col})")

            ' light green color is for highlighted possible moves
            If pb.BackColor = Color.LightGreen Then
                Debug.WriteLine($"PictureBox_Click: Captured piece: {currentPiece} at ({row}, {col})")

                ' TODO: Implement capturing logic

                ' add captured piece to the respective list
                If Char.IsLower(currentPiece) Then
                    Debug.WriteLine($"PictureBox_Click: Captured black piece: {currentPiece}")
                    BlackCapturedPieces.Add(currentPiece)
                Else
                    Debug.WriteLine($"PictureBox_Click: Captured white piece: {currentPiece}")
                    WhiteCapturedPieces.Add(currentPiece)
                End If

                UnhighlightPossibleMoves()
                Player.SwitchPlayer()

                Return
            End If
        End If

        If currentPiece = "." Then
            ' light green color is for highlighted possible moves
            If pb.BackColor = Color.LightGreen Then
                Debug.WriteLine($"PictureBox_Click: Moved piece: {SelectedPiece} from ({SelectedRow}, {SelectedCol}) to ({row}, {col})")

                ' TODO: Implement moving logic

                UnhighlightPossibleMoves()
                Player.SwitchPlayer()
                Return
            End If
        End If

        UnhighlightPossibleMoves()

        If (Char.IsLower(currentPiece) AndAlso Player.CurrentPlayer = Player.PlayerType.White) OrElse
           (Char.IsUpper(currentPiece) AndAlso Player.CurrentPlayer = Player.PlayerType.Black) Then
            Debug.WriteLine($"PictureBox_Click: Clicked piece: {currentPiece} at ({row}, {col})")

            Return
        End If

        If SelectedPiece Is Nothing Then
            Debug.WriteLine($"PictureBox_Click: Selected piece: {currentPiece} at ({row}, {col})")

            SelectedPiece = currentPiece
            SelectedRow = row
            SelectedCol = col
            pb.BackColor = Color.Yellow

            HighlightPossibleMoves(row, col)

        ElseIf SelectedPiece = currentPiece AndAlso SelectedRow = row AndAlso SelectedCol = col Then
            Debug.WriteLine($"PictureBox_Click: Unselected piece: {currentPiece} at ({row}, {col})")

            SelectedPiece = Nothing
            SelectedRow = -1
            SelectedCol = -1

            pb.BackColor = If((row + col) Mod 2 = 0, Color.White, Color.Gray)

            UnhighlightPossibleMoves()

        Else
            Debug.WriteLine($"PictureBox_Click: Selected new piece: {currentPiece} at ({row}, {col})")
            PictureBoxes(SelectedRow, SelectedCol).BackColor = If((SelectedRow + SelectedCol) Mod 2 = 0, Color.White, Color.Gray)
            SelectedPiece = currentPiece
            SelectedRow = row
            SelectedCol = col
            pb.BackColor = Color.Yellow

            HighlightPossibleMoves(row, col)
        End If
    End Sub

    Public Sub UnhighlightPossibleMoves()
        For row As Integer = 0 To Rows - 1
            For col As Integer = 0 To Cols - 1
                PictureBoxes(row, col).BackColor = If((row + col) Mod 2 = 0, Color.White, Color.Gray)
            Next
        Next
    End Sub

    Private Sub HighlightPossibleMoves(row As Integer, col As Integer)
        ' white pawn first move (highlight)
        If SelectedPiece = "P" AndAlso row = 6 Then
            Dim targetRow As Integer = row - 2
            Dim targetCol As Integer = col

            If Board(targetRow, targetCol) = "." AndAlso Board(targetRow + 1, targetCol) = "." Then
                PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
            End If
        End If

        ' white pawn (highlight)
        If SelectedPiece = "P" Then
            Dim direction As Integer = If(Player.CurrentPlayer = Player.PlayerType.White, -1, 1)
            Dim targetRow As Integer = row + direction
            Dim targetCol As Integer = col

            If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                If Board(targetRow, targetCol) = "." Then
                    PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                End If
            End If
        End If

        ' white pawn if there is opponent piece in diagonal (highlight)
        If SelectedPiece = "P" Then
            Dim direction As Integer = If(Player.CurrentPlayer = Player.PlayerType.White, -1, 1)
            Dim targetRow As Integer = row + direction
            Dim targetCol As Integer = col - 1

            If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                If Char.IsLower(Board(targetRow, targetCol)) Then
                    PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                End If
            End If

            targetCol = col + 1

            If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                If Char.IsLower(Board(targetRow, targetCol)) Then
                    PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                End If
            End If
        End If

        ' white rook (highlight)
        If SelectedPiece = "R" Then
            ' Check moves to the right
            For i As Integer = col + 1 To Cols - 1
                If Board(row, i) = "." Then
                    PictureBoxes(row, i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row, i)) Then
                        PictureBoxes(row, i).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves to the left
            For i As Integer = col - 1 To 0 Step -1
                If Board(row, i) = "." Then
                    PictureBoxes(row, i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row, i)) Then
                        PictureBoxes(row, i).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves upwards
            For i As Integer = row - 1 To 0 Step -1
                If Board(i, col) = "." Then
                    PictureBoxes(i, col).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(i, col)) Then
                        PictureBoxes(i, col).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves downwards
            For i As Integer = row + 1 To Rows - 1
                If Board(i, col) = "." Then
                    PictureBoxes(i, col).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(i, col)) Then
                        PictureBoxes(i, col).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next
        End If

        ' white bishop (highlight)
        If SelectedPiece = "B" Then
            ' Check moves to the top right
            Dim i As Integer = 1
            While row - i >= 0 AndAlso col + i < Cols
                If Board(row - i, col + i) = "." Then
                    PictureBoxes(row - i, col + i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row - i, col + i)) Then
                        PictureBoxes(row - i, col + i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While row - i >= 0 AndAlso col - i >= 0
                If Board(row - i, col - i) = "." Then
                    PictureBoxes(row - i, col - i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row - i, col - i)) Then
                        PictureBoxes(row - i, col - i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While row + i < Rows AndAlso col + i < Cols
                If Board(row + i, col + i) = "." Then
                    PictureBoxes(row + i, col + i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row + i, col + i)) Then
                        PictureBoxes(row + i, col + i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While row + i < Rows AndAlso col - i >= 0
                If Board(row + i, col - i) = "." Then
                    PictureBoxes(row + i, col - i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row + i, col - i)) Then
                        PictureBoxes(row + i, col - i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While
        End If

        ' white knight (highlight)
        If SelectedPiece = "N" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(row - 2, col - 1),
                New Point(row - 2, col + 1),
                New Point(row - 1, col - 2),
                New Point(row - 1, col + 2),
                New Point(row + 1, col - 2),
                New Point(row + 1, col + 2),
                New Point(row + 2, col - 1),
                New Point(row + 2, col + 1)
            }

            For Each move In possibleMoves
                Dim targetRow As Integer = move.X
                Dim targetCol As Integer = move.Y

                If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                    If Board(targetRow, targetCol) = "." OrElse Char.IsLower(Board(targetRow, targetCol)) Then
                        PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                    End If
                End If
            Next
        End If

        ' white king (highlight)
        If SelectedPiece = "K" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(row - 1, col - 1),
                New Point(row - 1, col),
                New Point(row - 1, col + 1),
                New Point(row, col - 1),
                New Point(row, col + 1),
                New Point(row + 1, col - 1),
                New Point(row + 1, col),
                New Point(row + 1, col + 1)
            }

            For Each move In possibleMoves
                Dim targetRow As Integer = move.X
                Dim targetCol As Integer = move.Y

                If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                    If Board(targetRow, targetCol) = "." OrElse Char.IsLower(Board(targetRow, targetCol)) Then
                        PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                    End If
                End If
            Next
        End If

        ' white queen (highlight)
        If SelectedPiece = "Q" Then
            ' Check moves to the right
            For colIndex As Integer = col + 1 To Cols - 1
                If Board(row, colIndex) = "." Then
                    PictureBoxes(row, colIndex).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row, colIndex)) Then
                        PictureBoxes(row, colIndex).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves to the left
            For colIndex As Integer = col - 1 To 0 Step -1
                If Board(row, colIndex) = "." Then
                    PictureBoxes(row, colIndex).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row, colIndex)) Then
                        PictureBoxes(row, colIndex).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves upwards
            For colIndex As Integer = row - 1 To 0 Step -1
                If Board(colIndex, col) = "." Then
                    PictureBoxes(colIndex, col).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(colIndex, col)) Then
                        PictureBoxes(colIndex, col).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves downwards
            For colIndex As Integer = row + 1 To Rows - 1
                If Board(colIndex, col) = "." Then
                    PictureBoxes(colIndex, col).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(colIndex, col)) Then
                        PictureBoxes(colIndex, col).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves to the top right
            Dim i As Integer = 1
            While row - i >= 0 AndAlso col + i < Cols
                If Board(row - i, col + i) = "." Then
                    PictureBoxes(row - i, col + i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row - i, col + i)) Then
                        PictureBoxes(row - i, col + i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While row - i >= 0 AndAlso col - i >= 0
                If Board(row - i, col - i) = "." Then
                    PictureBoxes(row - i, col - i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row - i, col - i)) Then
                        PictureBoxes(row - i, col - i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While row + i < Rows AndAlso col + i < Cols
                If Board(row + i, col + i) = "." Then
                    PictureBoxes(row + i, col + i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row + i, col + i)) Then
                        PictureBoxes(row + i, col + i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While row + i < Rows AndAlso col - i >= 0
                If Board(row + i, col - i) = "." Then
                    PictureBoxes(row + i, col - i).BackColor = Color.LightGreen
                Else
                    If Char.IsLower(Board(row + i, col - i)) Then
                        PictureBoxes(row + i, col - i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While
        End If

        ' black pawn first move (highlight)
        If SelectedPiece = "p" AndAlso row = 1 Then
            Dim targetRow As Integer = row + 2
            Dim targetCol As Integer = col

            If Board(targetRow, targetCol) = "." AndAlso Board(targetRow - 1, targetCol) = "." Then
                PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
            End If
        End If

        ' black pawn (highlight)
        If SelectedPiece = "p" Then
            Dim direction As Integer = If(Player.CurrentPlayer = Player.PlayerType.White, -1, 1)
            Dim targetRow As Integer = row + direction
            Dim targetCol As Integer = col

            If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                If Board(targetRow, targetCol) = "." Then
                    PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                End If
            End If
        End If

        ' black pawn if there is opponent piece in diagonal (highlight)
        If SelectedPiece = "p" Then
            Dim direction As Integer = If(Player.CurrentPlayer = Player.PlayerType.White, -1, 1)
            Dim targetRow As Integer = row + direction
            Dim targetCol As Integer = col - 1

            If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                If Char.IsUpper(Board(targetRow, targetCol)) Then
                    PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                End If
            End If

            targetCol = col + 1

            If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                If Char.IsUpper(Board(targetRow, targetCol)) Then
                    PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                End If
            End If
        End If

        ' black rook (highlight)
        If SelectedPiece = "r" Then
            ' Check moves to the right
            For i As Integer = col + 1 To Cols - 1
                If Board(row, i) = "." Then
                    PictureBoxes(row, i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row, i)) Then
                        PictureBoxes(row, i).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves to the left
            For i As Integer = col - 1 To 0 Step -1
                If Board(row, i) = "." Then
                    PictureBoxes(row, i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row, i)) Then
                        PictureBoxes(row, i).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves upwards
            For i As Integer = row - 1 To 0 Step -1
                If Board(i, col) = "." Then
                    PictureBoxes(i, col).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(i, col)) Then
                        PictureBoxes(i, col).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves downwards
            For i As Integer = row + 1 To Rows - 1
                If Board(i, col) = "." Then
                    PictureBoxes(i, col).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(i, col)) Then
                        PictureBoxes(i, col).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next
        End If

        ' black bishop (highlight)
        If SelectedPiece = "b" Then
            ' Check moves to the top right
            Dim i As Integer = 1
            While row - i >= 0 AndAlso col + i < Cols
                If Board(row - i, col + i) = "." Then
                    PictureBoxes(row - i, col + i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row - i, col + i)) Then
                        PictureBoxes(row - i, col + i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While row - i >= 0 AndAlso col - i >= 0
                If Board(row - i, col - i) = "." Then
                    PictureBoxes(row - i, col - i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row - i, col - i)) Then
                        PictureBoxes(row - i, col - i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While row + i < Rows AndAlso col + i < Cols
                If Board(row + i, col + i) = "." Then
                    PictureBoxes(row + i, col + i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row + i, col + i)) Then
                        PictureBoxes(row + i, col + i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While row + i < Rows AndAlso col - i >= 0
                If Board(row + i, col - i) = "." Then
                    PictureBoxes(row + i, col - i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row + i, col - i)) Then
                        PictureBoxes(row + i, col - i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While
        End If

        ' black knight (highlight)
        If SelectedPiece = "n" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(row - 2, col - 1),
                New Point(row - 2, col + 1),
                New Point(row - 1, col - 2),
                New Point(row - 1, col + 2),
                New Point(row + 1, col - 2),
                New Point(row + 1, col + 2),
                New Point(row + 2, col - 1),
                New Point(row + 2, col + 1)
            }

            For Each move In possibleMoves
                Dim targetRow As Integer = move.X
                Dim targetCol As Integer = move.Y

                If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                    If Board(targetRow, targetCol) = "." OrElse Char.IsUpper(Board(targetRow, targetCol)) Then
                        PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                    End If
                End If
            Next
        End If

        ' black king (highlight)
        If SelectedPiece = "k" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(row - 1, col - 1),
                New Point(row - 1, col),
                New Point(row - 1, col + 1),
                New Point(row, col - 1),
                New Point(row, col + 1),
                New Point(row + 1, col - 1),
                New Point(row + 1, col),
                New Point(row + 1, col + 1)
            }

            For Each move In possibleMoves
                Dim targetRow As Integer = move.X
                Dim targetCol As Integer = move.Y

                If targetRow >= 0 AndAlso targetRow < Rows AndAlso targetCol >= 0 AndAlso targetCol < Cols Then
                    If Board(targetRow, targetCol) = "." OrElse Char.IsUpper(Board(targetRow, targetCol)) Then
                        PictureBoxes(targetRow, targetCol).BackColor = Color.LightGreen
                    End If
                End If
            Next
        End If

        ' black queen (highlight)
        If SelectedPiece = "q" Then
            ' Check moves to the right
            For colIndex As Integer = col + 1 To Cols - 1
                If Board(row, colIndex) = "." Then
                    PictureBoxes(row, colIndex).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row, colIndex)) Then
                        PictureBoxes(row, colIndex).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves to the left
            For colIndex As Integer = col - 1 To 0 Step -1
                If Board(row, colIndex) = "." Then
                    PictureBoxes(row, colIndex).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row, colIndex)) Then
                        PictureBoxes(row, colIndex).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves upwards
            For colIndex As Integer = row - 1 To 0 Step -1
                If Board(colIndex, col) = "." Then
                    PictureBoxes(colIndex, col).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(colIndex, col)) Then
                        PictureBoxes(colIndex, col).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves downwards
            For colIndex As Integer = row + 1 To Rows - 1
                If Board(colIndex, col) = "." Then
                    PictureBoxes(colIndex, col).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(colIndex, col)) Then
                        PictureBoxes(colIndex, col).BackColor = Color.LightGreen
                    End If
                    Exit For
                End If
            Next

            ' Check moves to the top right
            Dim i As Integer = 1
            While row - i >= 0 AndAlso col + i < Cols
                If Board(row - i, col + i) = "." Then
                    PictureBoxes(row - i, col + i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row - i, col + i)) Then
                        PictureBoxes(row - i, col + i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While row - i >= 0 AndAlso col - i >= 0
                If Board(row - i, col - i) = "." Then
                    PictureBoxes(row - i, col - i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row - i, col - i)) Then
                        PictureBoxes(row - i, col - i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While row + i < Rows AndAlso col + i < Cols
                If Board(row + i, col + i) = "." Then
                    PictureBoxes(row + i, col + i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row + i, col + i)) Then
                        PictureBoxes(row + i, col + i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While row + i < Rows AndAlso col - i >= 0
                If Board(row + i, col - i) = "." Then
                    PictureBoxes(row + i, col - i).BackColor = Color.LightGreen
                Else
                    If Char.IsUpper(Board(row + i, col - i)) Then
                        PictureBoxes(row + i, col - i).BackColor = Color.LightGreen
                    End If
                    Exit While
                End If
                i += 1
            End While
        End If

    End Sub
End Module
