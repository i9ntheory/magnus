Public Module GameService
    Public Const Rows As Integer = 8
    Public Const Cols As Integer = 8

    Public PictureBoxes(Rows - 1, Cols - 1) As PictureBox
    Public Board(Rows - 1, Cols - 1) As String

    Public SelectedPiece As String
    Public SelectedRow As Integer
    Public SelectedCol As Integer

    Public LastPieceMoved As String
    Public LastPieceMovedCurrentPosition() As Integer

    Public WhiteCapturedPieces As New List(Of String)
    Public BlackCapturedPieces As New List(Of String)

    Public CurrentWhiteKingPosition() As Integer = {7, 4}
    Public CurrentBlackKingPosition() As Integer = {0, 4}

    ' list that stores all pieces, the piece key and the location of the piece
    ' piece key is the key of the piece, e.g. P for Pawn, R for Rook, N for Knight, B for Bishop, Q for Queen, K for King
    Public AllPieces As New List(Of Tuple(Of String, String, Integer, Integer))

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

            AllPieces.Add(New Tuple(Of String, String, Integer, Integer)(initialSetup(i).ToString(), initialSetup(i).ToString(), row, col))

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

                    'Debug.WriteLine($"RefreshBoard: Refreshing piece {p} at ({row}, {col})")
                End If
            Next
        Next

        If LastPieceMoved IsNot Nothing Then
            DetermineLastPiece()
        End If

        'Debug.WriteLine("RefreshBoard: Board refreshed successfully")
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

    Public Function SubstitutePieceKeyToFullName(pieceKey As String)
        ' P for Pawn, R for Rook, N for Knight, B for Bishop, Q for Queen, K for King
        ' Uppercase for white, lowercase for black

        Select Case pieceKey
            Case "P" : Return "Pawn"
            Case "R" : Return "Rook"
            Case "N" : Return "Knight"
            Case "B" : Return "Bishop"
            Case "Q" : Return "Queen"
            Case "K" : Return "King"
            Case "p" : Return "Pawn"
            Case "r" : Return "Rook"
            Case "n" : Return "Knight"
            Case "b" : Return "Bishop"
            Case "q" : Return "Queen"
            Case "k" : Return "King"
            Case Else : Return ""
        End Select
    End Function

    Public Sub MapCapturedToLabelList()
        Dim whiteListLabel = GameView.WhiteCapturedList
        Dim blackListLabel = GameView.BlackCapturedList

        ' map captured pieces to the respective label list and display the pieces name
        ' it is a label not a list so we can't use AddRange

        whiteListLabel.Text = ""

        For Each piece As String In WhiteCapturedPieces
            whiteListLabel.Text += SubstitutePieceKeyToFullName(piece) & " "
        Next

        blackListLabel.Text = ""
        For Each piece As String In BlackCapturedPieces
            blackListLabel.Text += SubstitutePieceKeyToFullName(piece) & " "
        Next

        Debug.WriteLine("MapCapturedToLabelList: Captured pieces mapped to the respective label list")

    End Sub

    Public Sub DisposeGame()
        Debug.WriteLine("DisposeGame: Disposing the game")

        GameView.GamePanel.Controls.Clear()

        For row As Integer = 0 To Rows - 1
            For col As Integer = 0 To Cols - 1
                Dim pb As PictureBox = PictureBoxes(row, col)
                RemoveHandler pb.Click, AddressOf PictureBox_Click
                GameView.GamePanel.Controls.Remove(pb)
            Next
        Next

        Array.Clear(Board, 0, Board.Length)

        WhiteCapturedPieces.Clear()
        BlackCapturedPieces.Clear()

        Player.CurrentPlayer = Player.PlayerType.White

        SelectedPiece = Nothing
        SelectedRow = -1
        SelectedCol = -1

        LastPieceMoved = Nothing
        LastPieceMovedCurrentPosition = Nothing

        CurrentWhiteKingPosition = {7, 4}
        CurrentBlackKingPosition = {0, 4}

        GameView.WhiteCapturedList.Text = "N/A"
        GameView.BlackCapturedList.Text = "N/A"

        AllPieces.Clear()

        Debug.WriteLine("DisposeGame: Game disposed successfully")
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

        ' Capturing the piece
        If (Char.IsLower(currentPiece) AndAlso Player.CurrentPlayer = Player.PlayerType.White) OrElse
           (Char.IsUpper(currentPiece) AndAlso Player.CurrentPlayer = Player.PlayerType.Black) Then
            Debug.WriteLine($"PictureBox_Click: Selected piece: {currentPiece} at ({row}, {col})")

            ' light green color is for highlighted possible moves
            If pb.BackColor = Color.LightGreen Then
                Debug.WriteLine($"PictureBox_Click: Captured piece: {currentPiece} at ({row}, {col})")
                LastPieceMoved = SelectedPiece
                LastPieceMovedCurrentPosition = {row, col}

                CapturePiece(SelectedRow, SelectedCol, row, col)

                ' add captured piece to the respective list
                If Char.IsLower(currentPiece) Then
                    Debug.WriteLine($"PictureBox_Click: Captured black piece: {currentPiece}")
                    BlackCapturedPieces.Add(currentPiece)
                Else
                    Debug.WriteLine($"PictureBox_Click: Captured white piece: {currentPiece}")
                    WhiteCapturedPieces.Add(currentPiece)
                End If

                DetermineLastPiece()

                MapCapturedToLabelList()

                UnhighlightPossibleMoves()
                Player.SwitchPlayer()


                Return
            End If
        End If

        ' Moving the piece
        If currentPiece = "." Then
            ' light green color is for highlighted possible moves
            If pb.BackColor = Color.LightGreen Then
                Debug.WriteLine($"PictureBox_Click: Moved piece: {SelectedPiece} from ({SelectedRow}, {SelectedCol}) to ({row}, {col})")

                LastPieceMoved = SelectedPiece
                LastPieceMovedCurrentPosition = {row, col}

                MovePiece(SelectedRow, SelectedCol, row, col)

                DetermineLastPiece()

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

    Public Sub DetermineAllPieces()
        Dim whiteKingRow As Integer = CurrentWhiteKingPosition(0)
        Dim whiteKingCol As Integer = CurrentWhiteKingPosition(1)

        Dim blackKingRow As Integer = CurrentBlackKingPosition(0)
        Dim blackKingCol As Integer = CurrentBlackKingPosition(1)

        ' Determine all pieces on the board
        ' check one by one for possible moves for each piece to determine if the king is in check
        For Each piece In AllPieces
            Dim pieceKey As String = piece.Item1
            Dim pieceRow As Integer = piece.Item3
            Dim pieceCol As Integer = piece.Item4

            ' piece.Item1 is the piece key, e.g. P for Pawn, R for Rook, N for Knight, B for Bishop, Q for Queen, K for King
            ' make sure to account for pieces that block the path of the attacking piece
            ' if the king is in check, highlight the king in red
            ' if the king is in checkmate, end the game
            If pieceKey = "p" Then
            ElseIf pieceKey = "r" Then
            ElseIf pieceKey = "n" Then
            ElseIf pieceKey = "b" Then
            ElseIf pieceKey = "q" Then
            ElseIf pieceKey = "k" Then
            ElseIf pieceKey = "P" Then
            ElseIf pieceKey = "R" Then
            ElseIf pieceKey = "N" Then
            ElseIf pieceKey = "B" Then
            ElseIf pieceKey = "Q" Then
            ElseIf pieceKey = "K" Then
            End If
        Next
    End Sub

    Public Sub DetermineLastPiece()
        Dim whiteKingRow As Integer = CurrentWhiteKingPosition(0)
        Dim whiteKingCol As Integer = CurrentWhiteKingPosition(1)

        Dim blackKingRow As Integer = CurrentBlackKingPosition(0)
        Dim blackKingCol As Integer = CurrentBlackKingPosition(1)

        Dim lastPieceRow As Integer = LastPieceMovedCurrentPosition(0)
        Dim lastPieceCol As Integer = LastPieceMovedCurrentPosition(1)

        Dim lastPiece As String = LastPieceMoved

        Debug.Write($"DetermineLastPiece: Determining the last piece moved: {lastPiece} at ({lastPieceRow}, {lastPieceCol})")

        ' Determine the last piece moved
        ' check the last piece possible moves for each piece to determine if the king is in check

        ' black queen (possible moves)
        If lastPiece = "q" Then
            Debug.WriteLine("DetermineLastPiece: Last piece moved is black queen")

            ' Check moves to the right
            For colIndex As Integer = lastPieceCol + 1 To Cols - 1
                If Board(lastPieceRow, colIndex) = "." Then
                    If colIndex = whiteKingCol AndAlso lastPieceRow = whiteKingRow Then
                        ' highlight the king in red
                        PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                        MessageBox.Show("Check!")
                    End If
                Else
                    If Char.IsUpper(Board(lastPieceRow, colIndex)) Then
                        If colIndex = whiteKingCol AndAlso lastPieceRow = whiteKingRow Then
                            ' highlight the king in red
                            PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                            MessageBox.Show("Check!")
                        End If
                        Exit For
                    End If
                End If
            Next

            ' Check moves to the left
            For colIndex As Integer = lastPieceCol - 1 To 0 Step -1
                If Board(lastPieceRow, colIndex) = "." Then
                    If colIndex = whiteKingCol AndAlso lastPieceRow = whiteKingRow Then
                        ' highlight the king in red
                        PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                        MessageBox.Show("Check!")
                    End If
                Else
                    If Char.IsUpper(Board(lastPieceRow, colIndex)) Then
                        If colIndex = whiteKingCol AndAlso lastPieceRow = whiteKingRow Then
                            ' highlight the king in red
                            PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                            MessageBox.Show("Check!")
                        End If
                        Exit For
                    End If
                End If
            Next

            ' Check moves upwards
            For colIndex As Integer = lastPieceRow - 1 To 0 Step -1
                If Board(colIndex, lastPieceCol) = "." Then
                    If colIndex = whiteKingRow AndAlso lastPieceCol = whiteKingCol Then
                        ' highlight the king in red
                        PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                        MessageBox.Show("Check!")
                    End If
                Else
                    If Char.IsUpper(Board(colIndex, lastPieceCol)) Then
                        If colIndex = whiteKingRow AndAlso lastPieceCol = whiteKingCol Then
                            ' highlight the king in red
                            PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                            MessageBox.Show("Check!")
                        End If
                        Exit For
                    End If
                End If
            Next

            ' Check moves downwards
            For colIndex As Integer = lastPieceRow + 1 To Rows - 1
                If Board(colIndex, lastPieceCol) = "." Then
                    If colIndex = whiteKingRow AndAlso lastPieceCol = whiteKingCol Then
                        ' highlight the king in red
                        PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                        MessageBox.Show("Check!")
                    End If
                Else
                    If Char.IsUpper(Board(colIndex, lastPieceCol)) Then
                        If colIndex = whiteKingRow AndAlso lastPieceCol = whiteKingCol Then
                            ' highlight the king in red
                            PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                            MessageBox.Show("Check!")
                        End If
                        Exit For
                    End If
                End If
            Next

            ' Check moves to the top right
            Dim i As Integer = 1
            While lastPieceRow - i >= 0 AndAlso lastPieceCol + i < Cols
                If Board(lastPieceRow - i, lastPieceCol + i) = "." Then
                    If lastPieceRow - i = whiteKingRow AndAlso lastPieceCol + i = whiteKingCol Then
                        ' highlight the king in red
                        PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                        MessageBox.Show("Check!")
                    End If
                Else
                    If Char.IsUpper(Board(lastPieceRow - i, lastPieceCol + i)) Then
                        If lastPieceRow - i = whiteKingRow AndAlso lastPieceCol + i = whiteKingCol Then
                            ' highlight the king in red
                            PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                            MessageBox.Show("Check!")
                        End If
                        Exit While
                    End If
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While lastPieceRow - i >= 0 AndAlso lastPieceCol - i >= 0
                If Board(lastPieceRow - i, lastPieceCol - i) = "." Then
                    If lastPieceRow - i = whiteKingRow AndAlso lastPieceCol - i = whiteKingCol Then
                        ' highlight the king in red
                        PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                        MessageBox.Show("Check!")
                    End If
                Else
                    If Char.IsUpper(Board(lastPieceRow - i, lastPieceCol - i)) Then
                        If lastPieceRow - i = whiteKingRow AndAlso lastPieceCol - i = whiteKingCol Then
                            ' highlight the king in red
                            PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                            MessageBox.Show("Check!")
                        End If
                        Exit While
                    End If
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While lastPieceRow + i < Rows AndAlso lastPieceCol + i < Cols
                If Board(lastPieceRow + i, lastPieceCol + i) = "." Then
                    If lastPieceRow + i = whiteKingRow AndAlso lastPieceCol + i = whiteKingCol Then
                        ' highlight the king in red
                        PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                        MessageBox.Show("Check!")
                    End If
                Else
                    If Char.IsUpper(Board(lastPieceRow + i, lastPieceCol + i)) Then
                        If lastPieceRow + i = whiteKingRow AndAlso lastPieceCol + i = whiteKingCol Then
                            ' highlight the king in red
                            PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                            MessageBox.Show("Check!")
                        End If
                        Exit While
                    End If
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While lastPieceRow + i < Rows AndAlso lastPieceCol - i >= 0
                If Board(lastPieceRow + i, lastPieceCol - i) = "." Then
                    If lastPieceRow + i = whiteKingRow AndAlso lastPieceCol - i = whiteKingCol Then
                        ' highlight the king in red
                        PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                        MessageBox.Show("Check!")
                    End If
                Else
                    If Char.IsUpper(Board(lastPieceRow + i, lastPieceCol - i)) Then
                        If lastPieceRow + i = whiteKingRow AndAlso lastPieceCol - i = whiteKingCol Then
                            ' highlight the king in red
                            PictureBoxes(whiteKingRow, whiteKingCol).BackColor = Color.Red

                            MessageBox.Show("Check!")
                        End If
                        Exit While
                    End If
                End If
                i += 1
            End While
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

    Private Sub MovePiece(fromRow As Integer, fromCol As Integer, toRow As Integer, toCol As Integer)
        ' update the piece information in the AllPieces
        Dim piece = AllPieces.FirstOrDefault(Function(p) p.Item3 = fromRow AndAlso p.Item4 = fromCol)
        AllPieces.Remove(piece)
        AllPieces.Add(New Tuple(Of String, String, Integer, Integer)(SelectedPiece, SelectedPiece, toRow, toCol))

        ' white pawn first move (mover)
        If SelectedPiece = "P" AndAlso toRow = fromRow - 2 AndAlso toCol = fromCol Then
            ' Update the board and UI
            Board(toRow, toCol) = SelectedPiece
            Board(fromRow, fromCol) = "."

            ' Update PictureBox images
            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
            PictureBoxes(fromRow, fromCol).Image = Nothing

            ' Clear selection and update turn
            SelectedPiece = Nothing
            SelectedRow = -1
            SelectedCol = -1

            UpdateCurrentPlayerLabel()

            ' Remove highlights
            UnhighlightPossibleMoves()

            RefreshBoard()
        End If

        ' white pawn (mover)
        If SelectedPiece = "P" Then
            Dim direction As Integer = If(Player.CurrentPlayer = Player.PlayerType.White, -1, 1)
            If toRow = fromRow + direction AndAlso toCol = fromCol Then
                ' Update the board and UI
                Board(toRow, toCol) = SelectedPiece
                Board(fromRow, fromCol) = "."

                ' Update PictureBox images
                PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                PictureBoxes(fromRow, fromCol).Image = Nothing

                ' Clear selection and update turn
                SelectedPiece = Nothing
                SelectedRow = -1
                SelectedCol = -1

                UpdateCurrentPlayerLabel()

                ' Remove highlights
                UnhighlightPossibleMoves()

                RefreshBoard()
            End If
        End If

        ' white rook (mover)
        If SelectedPiece = "R" Then
            ' Check moves to the right
            If toRow = fromRow AndAlso toCol > fromCol Then
                Dim validMove As Boolean = True
                For i As Integer = fromCol + 1 To toCol - 1
                    If Board(fromRow, i) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If
            End If

            ' Check moves to the left
            If toRow = fromRow AndAlso toCol < fromCol Then
                Dim validMove As Boolean = True
                For i As Integer = fromCol - 1 To toCol + 1 Step -1
                    If Board(fromRow, i) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If
            End If

            ' Check moves upwards
            If toCol = fromCol AndAlso toRow < fromRow Then
                Dim validMove As Boolean = True
                For i As Integer = fromRow - 1 To toRow + 1 Step -1
                    If Board(i, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing

                    SelectedRow = -1
                    SelectedCol = -1

                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()

                End If
            End If

            ' Check moves downwards
            If toCol = fromCol AndAlso toRow > fromRow Then
                Dim validMove As Boolean = True
                For i As Integer = fromRow + 1 To toRow - 1
                    If Board(i, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If
            End If
        End If

        ' white bishop (mover)
        If SelectedPiece = "B" Then
            ' Check moves to the top right
            Dim i As Integer = 1
            While fromRow - i >= 0 AndAlso fromCol + i < Cols
                If toRow = fromRow - i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While fromRow - i >= 0 AndAlso fromCol - i >= 0
                If toRow = fromRow - i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While fromRow + i < Rows AndAlso fromCol + i < Cols
                If toRow = fromRow + i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While fromRow + i < Rows AndAlso fromCol - i >= 0
                If toRow = fromRow + i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While
        End If

        ' white knight (mover)
        If SelectedPiece = "N" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(fromRow - 2, fromCol - 1),
                New Point(fromRow - 2, fromCol + 1),
                New Point(fromRow - 1, fromCol - 2),
                New Point(fromRow - 1, fromCol + 2),
                New Point(fromRow + 1, fromCol - 2),
                New Point(fromRow + 1, fromCol + 2),
                New Point(fromRow + 2, fromCol - 1),
                New Point(fromRow + 2, fromCol + 1)
            }

            If possibleMoves.Contains(New Point(toRow, toCol)) Then
                Board(toRow, toCol) = SelectedPiece
                Board(fromRow, fromCol) = "."

                PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                PictureBoxes(fromRow, fromCol).Image = Nothing

                SelectedPiece = Nothing
                SelectedRow = -1
                SelectedCol = -1
                UpdateCurrentPlayerLabel()

                UnhighlightPossibleMoves()

                RefreshBoard()
            End If
        End If

        ' white king (mover)
        If SelectedPiece = "K" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(fromRow - 1, fromCol - 1),
                New Point(fromRow - 1, fromCol),
                New Point(fromRow - 1, fromCol + 1),
                New Point(fromRow, fromCol - 1),
                New Point(fromRow, fromCol + 1),
                New Point(fromRow + 1, fromCol - 1),
                New Point(fromRow + 1, fromCol),
                New Point(fromRow + 1, fromCol + 1)
            }

            If possibleMoves.Contains(New Point(toRow, toCol)) Then
                Board(toRow, toCol) = SelectedPiece
                Board(fromRow, fromCol) = "."

                PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                PictureBoxes(fromRow, fromCol).Image = Nothing

                SelectedPiece = Nothing
                SelectedRow = -1
                SelectedCol = -1
                UpdateCurrentPlayerLabel()

                ' Update the king's position
                CurrentWhiteKingPosition = {toRow, toCol}

                UnhighlightPossibleMoves()

                RefreshBoard()
            End If
        End If

        ' white queen (mover)
        If SelectedPiece = "Q" Then
            ' Check moves to the right
            If toRow = fromRow AndAlso toCol > fromCol Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromCol + 1 To toCol - 1
                    If Board(fromRow, colIndex) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If
            End If

            ' Check moves to the left
            If toRow = fromRow AndAlso toCol < fromCol Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromCol - 1 To toCol + 1 Step -1
                    If Board(fromRow, colIndex) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1

                    SelectedCol = -1

                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()

                End If

            End If

            ' Check moves upwards
            If toCol = fromCol AndAlso toRow < fromRow Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromRow - 1 To toRow + 1 Step -1
                    If Board(colIndex, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1

                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()

                End If

            End If

            ' Check moves downwards
            If toCol = fromCol AndAlso toRow > fromRow Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromRow + 1 To toRow - 1
                    If Board(colIndex, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If

            End If

            ' Check moves to the top right
            Dim i As Integer = 1
            While fromRow - i >= 0 AndAlso fromCol + i < Cols
                If toRow = fromRow - i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While fromRow - i >= 0 AndAlso fromCol - i >= 0
                If toRow = fromRow - i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While fromRow + i < Rows AndAlso fromCol + i < Cols
                If toRow = fromRow + i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While fromRow + i < Rows AndAlso fromCol - i >= 0
                If toRow = fromRow + i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While
        End If

        ' black pawn first move (mover)
        If SelectedPiece = "p" AndAlso toRow = fromRow + 2 AndAlso toCol = fromCol Then
            ' Update the board and UI
            Board(toRow, toCol) = SelectedPiece
            Board(fromRow, fromCol) = "."

            ' Update PictureBox images
            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
            PictureBoxes(fromRow, fromCol).Image = Nothing

            ' Clear selection and update turn
            SelectedPiece = Nothing
            SelectedRow = -1
            SelectedCol = -1
            UpdateCurrentPlayerLabel()

            ' Remove highlights
            UnhighlightPossibleMoves()

            RefreshBoard()
        End If

        ' black pawn (mover)
        If SelectedPiece = "p" Then
            Dim direction As Integer = If(Player.CurrentPlayer = Player.PlayerType.White, -1, 1)
            If toRow = fromRow + direction AndAlso toCol = fromCol Then
                ' Update the board and UI
                Board(toRow, toCol) = SelectedPiece
                Board(fromRow, fromCol) = "."

                ' Update PictureBox images
                PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                PictureBoxes(fromRow, fromCol).Image = Nothing

                ' Clear selection and update turn
                SelectedPiece = Nothing
                SelectedRow = -1
                SelectedCol = -1
                UpdateCurrentPlayerLabel()

                ' Remove highlights
                UnhighlightPossibleMoves()

                RefreshBoard()
            End If
        End If

        ' black rook (mover)
        If SelectedPiece = "r" Then
            ' Check moves to the right
            If toRow = fromRow AndAlso toCol > fromCol Then
                Dim validMove As Boolean = True
                For i As Integer = fromCol + 1 To toCol - 1
                    If Board(fromRow, i) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If
            End If

            ' Check moves to the left
            If toRow = fromRow AndAlso toCol < fromCol Then
                Dim validMove As Boolean = True
                For i As Integer = fromCol - 1 To toCol + 1 Step -1
                    If Board(fromRow, i) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If
            End If

            ' Check moves upwards
            If toCol = fromCol AndAlso toRow < fromRow Then
                Dim validMove As Boolean = True
                For i As Integer = fromRow - 1 To toRow + 1 Step -1
                    If Board(i, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing

                    SelectedRow = -1
                    SelectedCol = -1

                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()

                End If
            End If

            ' Check moves downwards
            If toCol = fromCol AndAlso toRow > fromRow Then
                Dim validMove As Boolean = True
                For i As Integer = fromRow + 1 To toRow - 1
                    If Board(i, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If
            End If
        End If

        ' black bishop (mover)
        If SelectedPiece = "b" Then
            ' Check moves to the top right
            Dim i As Integer = 1
            While fromRow - i >= 0 AndAlso fromCol + i < Cols
                If toRow = fromRow - i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While fromRow - i >= 0 AndAlso fromCol - i >= 0
                If toRow = fromRow - i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While fromRow + i < Rows AndAlso fromCol + i < Cols
                If toRow = fromRow + i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While fromRow + i < Rows AndAlso fromCol - i >= 0
                If toRow = fromRow + i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While
        End If

        ' black knight (mover)
        If SelectedPiece = "n" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(fromRow - 2, fromCol - 1),
                New Point(fromRow - 2, fromCol + 1),
                New Point(fromRow - 1, fromCol - 2),
                New Point(fromRow - 1, fromCol + 2),
                New Point(fromRow + 1, fromCol - 2),
                New Point(fromRow + 1, fromCol + 2),
                New Point(fromRow + 2, fromCol - 1),
                New Point(fromRow + 2, fromCol + 1)
            }

            If possibleMoves.Contains(New Point(toRow, toCol)) Then
                Board(toRow, toCol) = SelectedPiece
                Board(fromRow, fromCol) = "."

                PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                PictureBoxes(fromRow, fromCol).Image = Nothing

                SelectedPiece = Nothing
                SelectedRow = -1
                SelectedCol = -1
                UpdateCurrentPlayerLabel()

                UnhighlightPossibleMoves()

                RefreshBoard()
            End If
        End If

        ' black king (mover)
        If SelectedPiece = "k" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(fromRow - 1, fromCol - 1),
                New Point(fromRow - 1, fromCol),
                New Point(fromRow - 1, fromCol + 1),
                New Point(fromRow, fromCol - 1),
                New Point(fromRow, fromCol + 1),
                New Point(fromRow + 1, fromCol - 1),
                New Point(fromRow + 1, fromCol),
                New Point(fromRow + 1, fromCol + 1)
            }

            If possibleMoves.Contains(New Point(toRow, toCol)) Then
                Board(toRow, toCol) = SelectedPiece
                Board(fromRow, fromCol) = "."

                PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                PictureBoxes(fromRow, fromCol).Image = Nothing

                SelectedPiece = Nothing
                SelectedRow = -1
                SelectedCol = -1
                UpdateCurrentPlayerLabel()

                UnhighlightPossibleMoves()

                ' Update the king's position
                CurrentBlackKingPosition = {toRow, toCol}

                RefreshBoard()
            End If
        End If

        ' black queen (mover)
        If SelectedPiece = "q" Then
            ' Check moves to the right
            If toRow = fromRow AndAlso toCol > fromCol Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromCol + 1 To toCol - 1
                    If Board(fromRow, colIndex) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1

                    SelectedCol = -1

                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()

                End If
            End If

            ' Check moves to the left
            If toRow = fromRow AndAlso toCol < fromCol Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromCol - 1 To toCol + 1 Step -1
                    If Board(fromRow, colIndex) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1

                    SelectedCol = -1

                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()

                End If

            End If

            ' Check moves upwards
            If toCol = fromCol AndAlso toRow < fromRow Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromRow - 1 To toRow + 1 Step -1
                    If Board(colIndex, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)

                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1

                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()

                End If

            End If

            ' Check moves downwards
            If toCol = fromCol AndAlso toRow > fromRow Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromRow + 1 To toRow - 1
                    If Board(colIndex, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    UpdateCurrentPlayerLabel()

                    UnhighlightPossibleMoves()

                    RefreshBoard()
                End If

            End If

            ' Check moves to the top right
            Dim i As Integer = 1
            While fromRow - i >= 0 AndAlso fromCol + i < Cols
                If toRow = fromRow - i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While fromRow - i >= 0 AndAlso fromCol - i >= 0
                If toRow = fromRow - i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While fromRow + i < Rows AndAlso fromCol + i < Cols
                If toRow = fromRow + i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While fromRow + i < Rows AndAlso fromCol - i >= 0
                If toRow = fromRow + i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        UpdateCurrentPlayerLabel()

                        UnhighlightPossibleMoves()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While
        End If

    End Sub

    Private Sub CapturePiece(fromRow As Integer, fromCol As Integer, toRow As Integer, toCol As Integer)
        Dim piece = AllPieces.FirstOrDefault(Function(p) p.Item3 = fromRow AndAlso p.Item4 = fromCol)
        If piece IsNot Nothing Then
            AllPieces.Remove(piece)
        End If

        ' pawns (capture)
        If SelectedPiece = "P" OrElse SelectedPiece = "p" Then
            Dim direction As Integer = If(Player.CurrentPlayer = Player.PlayerType.White, -1, 1)
            If toRow = fromRow + direction AndAlso
                   (toCol = fromCol - 1 OrElse toCol = fromCol + 1) Then
                If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    Player.SwitchPlayer()
                ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    Player.SwitchPlayer()
                End If

                UnhighlightPossibleMoves()

                Player.SwitchPlayer()
                RefreshBoard()
            End If
        End If

        ' rooks (capture)
        If SelectedPiece = "R" OrElse SelectedPiece = "r" Then
            ' Check moves to the right
            If toRow = fromRow AndAlso toCol > fromCol Then
                Dim validMove As Boolean = True
                For i As Integer = fromCol + 1 To toCol - 1
                    If Board(fromRow, i) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    End If

                    UnhighlightPossibleMoves()

                    Player.SwitchPlayer()
                    RefreshBoard()
                End If
            End If

            ' Check moves to the left
            If toRow = fromRow AndAlso toCol < fromCol Then
                Dim validMove As Boolean = True
                For i As Integer = fromCol - 1 To toCol + 1 Step -1
                    If Board(fromRow, i) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1

                        SelectedCol = -1
                        Player.SwitchPlayer()

                    ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    End If

                    UnhighlightPossibleMoves()

                    Player.SwitchPlayer()
                    RefreshBoard()

                End If

            End If

            ' Check moves upwards
            If toCol = fromCol AndAlso toRow < fromRow Then
                Dim validMove As Boolean = True
                For i As Integer = fromRow - 1 To toRow + 1 Step -1
                    If Board(i, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()

                    ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    End If

                    UnhighlightPossibleMoves()

                    Player.SwitchPlayer()
                    RefreshBoard()

                End If

            End If

            ' Check moves downwards
            If toCol = fromCol AndAlso toRow > fromRow Then
                Dim validMove As Boolean = True
                For i As Integer = fromRow + 1 To toRow - 1
                    If Board(i, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()

                    ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    End If

                    UnhighlightPossibleMoves()

                    Player.SwitchPlayer()
                    RefreshBoard()

                End If
            End If
        End If

        ' bishops (capture)
        If SelectedPiece = "B" OrElse SelectedPiece = "b" Then
            ' Check moves to the top right
            Dim i As Integer = 1
            While fromRow - i >= 0 AndAlso fromCol + i < Cols
                If toRow = fromRow - i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()

                        ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()
                        End If

                        UnhighlightPossibleMoves()

                        Player.SwitchPlayer()
                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While fromRow - i >= 0 AndAlso fromCol - i >= 0
                If toRow = fromRow - i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()

                        ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()
                        End If

                        UnhighlightPossibleMoves()

                        Player.SwitchPlayer()
                        RefreshBoard()
                    End If

                    Exit While

                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While fromRow + i < Rows AndAlso fromCol + i < Cols
                If toRow = fromRow + i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()

                        ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()
                        End If

                        UnhighlightPossibleMoves()

                        Player.SwitchPlayer()
                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While fromRow + i < Rows AndAlso fromCol - i >= 0
                If toRow = fromRow + i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()

                        ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()
                        End If

                        UnhighlightPossibleMoves()

                        Player.SwitchPlayer()
                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

        End If

        ' knights (capture)
        If SelectedPiece = "N" OrElse SelectedPiece = "n" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(fromRow - 2, fromCol - 1),
                New Point(fromRow - 2, fromCol + 1),
                New Point(fromRow - 1, fromCol - 2),
                New Point(fromRow - 1, fromCol + 2),
                New Point(fromRow + 1, fromCol - 2),
                New Point(fromRow + 1, fromCol + 2),
                New Point(fromRow + 2, fromCol - 1),
                New Point(fromRow + 2, fromCol + 1)
            }

            If possibleMoves.Contains(New Point(toRow, toCol)) Then
                If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    Player.SwitchPlayer()

                ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    Player.SwitchPlayer()
                End If

                UnhighlightPossibleMoves()

                Player.SwitchPlayer()
                RefreshBoard()
            End If
        End If

        ' kings (capture)
        If SelectedPiece = "K" OrElse SelectedPiece = "k" Then
            Dim possibleMoves = New List(Of Point) From {
                New Point(fromRow - 1, fromCol - 1),
                New Point(fromRow - 1, fromCol),
                New Point(fromRow - 1, fromCol + 1),
                New Point(fromRow, fromCol - 1),
                New Point(fromRow, fromCol + 1),
                New Point(fromRow + 1, fromCol - 1),
                New Point(fromRow + 1, fromCol),
                New Point(fromRow + 1, fromCol + 1)
            }

            If possibleMoves.Contains(New Point(toRow, toCol)) Then
                If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    Player.SwitchPlayer()

                ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                    Board(toRow, toCol) = SelectedPiece
                    Board(fromRow, fromCol) = "."

                    PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                    PictureBoxes(fromRow, fromCol).Image = Nothing

                    SelectedPiece = Nothing
                    SelectedRow = -1
                    SelectedCol = -1
                    Player.SwitchPlayer()
                End If

                UnhighlightPossibleMoves()

                If Player.CurrentPlayer = Player.PlayerType.White Then
                    CurrentWhiteKingPosition = {toRow, toCol}
                Else
                    CurrentBlackKingPosition = {toRow, toCol}
                End If

                Player.SwitchPlayer()
                RefreshBoard()
            End If
        End If

        ' queens (capture)
        If SelectedPiece = "Q" OrElse SelectedPiece = "q" Then
            ' Check moves to the right
            If toRow = fromRow AndAlso toCol > fromCol Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromCol + 1 To toCol - 1
                    If Board(fromRow, colIndex) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()

                    ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    End If

                    UnhighlightPossibleMoves()

                    Player.SwitchPlayer()
                    RefreshBoard()
                End If
            End If

            ' Check moves to the left
            If toRow = fromRow AndAlso toCol < fromCol Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromCol - 1 To toCol + 1 Step -1
                    If Board(fromRow, colIndex) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1

                        SelectedCol = -1

                        Player.SwitchPlayer()

                    ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    End If

                    UnhighlightPossibleMoves()

                    Player.SwitchPlayer()

                    RefreshBoard()

                End If

            End If


            ' Check moves upwards
            If toCol = fromCol AndAlso toRow < fromRow Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromRow - 1 To toRow + 1 Step -1
                    If Board(colIndex, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()

                    ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    End If

                    UnhighlightPossibleMoves()

                    Player.SwitchPlayer()

                    RefreshBoard()

                End If

            End If

            ' Check moves downwards
            If toCol = fromCol AndAlso toRow > fromRow Then
                Dim validMove As Boolean = True
                For colIndex As Integer = fromRow + 1 To toRow - 1
                    If Board(colIndex, fromCol) <> "." Then
                        validMove = False
                        Exit For
                    End If
                Next

                If validMove Then
                    If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()

                    ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then

                        Board(toRow, toCol) = SelectedPiece
                        Board(fromRow, fromCol) = "."

                        PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                        PictureBoxes(fromRow, fromCol).Image = Nothing

                        SelectedPiece = Nothing
                        SelectedRow = -1
                        SelectedCol = -1
                        Player.SwitchPlayer()
                    End If

                    UnhighlightPossibleMoves()

                    Player.SwitchPlayer()

                    RefreshBoard()

                End If

            End If

            ' Check moves to the top right
            Dim i As Integer = 1
            While fromRow - i >= 0 AndAlso fromCol + i < Cols
                If toRow = fromRow - i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()

                        ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()
                        End If

                        UnhighlightPossibleMoves()

                        Player.SwitchPlayer()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the top left
            i = 1
            While fromRow - i >= 0 AndAlso fromCol - i >= 0
                If toRow = fromRow - i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow - j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."
                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()

                        ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()
                        End If

                        UnhighlightPossibleMoves()

                        Player.SwitchPlayer()

                        RefreshBoard()
                    End If

                    Exit While

                End If
                i += 1
            End While

            ' Check moves to the bottom right
            i = 1
            While fromRow + i < Rows AndAlso fromCol + i < Cols
                If toRow = fromRow + i AndAlso toCol = fromCol + i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol + j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()

                        ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()
                        End If

                        UnhighlightPossibleMoves()

                        Player.SwitchPlayer()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

            ' Check moves to the bottom left
            i = 1
            While fromRow + i < Rows AndAlso fromCol - i >= 0
                If toRow = fromRow + i AndAlso toCol = fromCol - i Then
                    Dim validMove As Boolean = True
                    For j As Integer = 1 To i - 1
                        If Board(fromRow + j, fromCol - j) <> "." Then
                            validMove = False
                            Exit For
                        End If
                    Next

                    If validMove Then
                        If Char.IsLower(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.White Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()

                        ElseIf Char.IsUpper(Board(toRow, toCol)) AndAlso Player.CurrentPlayer = Player.PlayerType.Black Then
                            Board(toRow, toCol) = SelectedPiece
                            Board(fromRow, fromCol) = "."

                            PictureBoxes(toRow, toCol).Image = GetPieceImage(SelectedPiece)
                            PictureBoxes(fromRow, fromCol).Image = Nothing

                            SelectedPiece = Nothing
                            SelectedRow = -1
                            SelectedCol = -1
                            Player.SwitchPlayer()
                        End If

                        UnhighlightPossibleMoves()

                        Player.SwitchPlayer()

                        RefreshBoard()
                    End If

                    Exit While
                End If
                i += 1
            End While

        End If

        AllPieces.RemoveAll(Function(x) x.Item1 = SelectedPiece)
    End Sub

    Public Sub UpdateCurrentPlayerLabel()

        If Player.CurrentPlayer = Player.PlayerType.White Then
            GameView.CurrentPlayerLabel.Text = $"Current: White"
        Else
            GameView.CurrentPlayerLabel.Text = $"Current: Black"
        End If
    End Sub
End Module
